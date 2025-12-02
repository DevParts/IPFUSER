using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.Targets;

namespace NLog.MessageTemplates;

/// <summary>
/// Convert, Render or serialize a value, with optionally backwards-compatible with <see cref="M:System.String.Format(System.IFormatProvider,System.String,System.Object[])" />
/// </summary>
internal sealed class ValueFormatter : IValueFormatter
{
	private sealed class JsonConverterWithSpaces : IJsonConverter
	{
		private readonly DefaultJsonSerializer _serializer;

		private readonly JsonSerializeOptions _serializerOptions;

		private readonly JsonSerializeOptions _exceptionSerializerOptions;

		public static IJsonConverter CreateJsonConverter(IJsonConverter jsonConverter)
		{
			if (jsonConverter is DefaultJsonSerializer jsonConverter2)
			{
				return new JsonConverterWithSpaces(jsonConverter2);
			}
			return jsonConverter;
		}

		private JsonConverterWithSpaces(DefaultJsonSerializer jsonConverter)
		{
			_serializer = jsonConverter;
			_serializerOptions = new JsonSerializeOptions
			{
				SuppressSpaces = false
			};
			_exceptionSerializerOptions = new JsonSerializeOptions
			{
				SuppressSpaces = false,
				SanitizeDictionaryKeys = true
			};
		}

		public bool SerializeObject(object? value, StringBuilder builder)
		{
			if (value is Exception)
			{
				return _serializer.SerializeObject(value, builder, _exceptionSerializerOptions);
			}
			return _serializer.SerializeObject(value, builder, _serializerOptions);
		}
	}

	private static readonly IEqualityComparer<object> _referenceEqualsComparer = SingleItemOptimizedHashSet<object>.ReferenceEqualityComparer.Default;

	private readonly MruCache<Enum, string> _enumCache = new MruCache<Enum, string>(2000);

	private readonly IServiceProvider _serviceProvider;

	private readonly bool _legacyStringQuotes;

	private IJsonConverter? _jsonConverter;

	private const int MaxRecursionDepth = 2;

	private const int MaxValueLength = 524288;

	private const string LiteralFormatSymbol = "l";

	public const string FormatAsJson = "@";

	public const string FormatAsString = "$";

	private IJsonConverter JsonConverter => _jsonConverter ?? (_jsonConverter = JsonConverterWithSpaces.CreateJsonConverter(_serviceProvider.GetService<IJsonConverter>()));

	public ValueFormatter(IServiceProvider serviceProvider, bool legacyStringQuotes)
	{
		_serviceProvider = serviceProvider;
		_legacyStringQuotes = legacyStringQuotes;
	}

	/// <summary>
	/// Serialization of an object, e.g. JSON and append to <paramref name="builder" />
	/// </summary>
	/// <param name="value">The object to serialize to string.</param>
	/// <param name="format">Parameter Format</param>
	/// <param name="captureType">Parameter CaptureType</param>
	/// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
	/// <param name="builder">Output destination.</param>
	/// <returns>Serialize succeeded (true/false)</returns>
	public bool FormatValue(object? value, string? format, CaptureType captureType, IFormatProvider? formatProvider, StringBuilder builder)
	{
		int num;
		switch (captureType)
		{
		case CaptureType.Serialize:
			return JsonConverter.SerializeObject(value, builder);
		case CaptureType.Stringify:
			if (_legacyStringQuotes)
			{
				if (builder.Length != 0)
				{
					num = ((builder[builder.Length - 1] != '"') ? 1 : 0);
					if (num == 0)
					{
						goto IL_0054;
					}
				}
				else
				{
					num = 1;
				}
				builder.Append('"');
			}
			else
			{
				num = 0;
			}
			goto IL_0054;
		default:
			{
				return FormatObject(value, format, formatProvider, builder);
			}
			IL_0054:
			FormatToString(value, format, formatProvider, builder);
			if (num != 0)
			{
				builder.Append('"');
			}
			return true;
		}
	}

	/// <summary>
	/// Format an object to a readable string, or if it's an object, serialize
	/// </summary>
	/// <param name="value">The value to convert</param>
	/// <param name="format"></param>
	/// <param name="formatProvider"></param>
	/// <param name="builder"></param>
	/// <returns></returns>
	public bool FormatObject(object? value, string? format, IFormatProvider? formatProvider, StringBuilder builder)
	{
		if (SerializeSimpleObject(value, format, formatProvider, builder))
		{
			return true;
		}
		if (value is IEnumerable collection)
		{
			return SerializeWithoutCyclicLoop(collection, format, formatProvider, builder, default(SingleItemOptimizedHashSet<object>), 0);
		}
		SerializeConvertToString(value, formatProvider, builder);
		return true;
	}

	/// <summary>
	/// Try serializing a scalar (string, int, NULL) or simple type (IFormattable)
	/// </summary>
	private bool SerializeSimpleObject(object? value, string? format, IFormatProvider? formatProvider, StringBuilder builder)
	{
		if (value is string stringValue)
		{
			SerializeStringObject(stringValue, format, builder);
			return true;
		}
		if (value is IConvertible value2)
		{
			SerializeConvertibleObject(value2, format, formatProvider, builder);
			return true;
		}
		if (value is IFormattable formattable)
		{
			builder.Append(formattable.ToString(format, formatProvider));
			return true;
		}
		if (value == null)
		{
			builder.Append("NULL");
			return true;
		}
		return false;
	}

	private void SerializeConvertibleObject(IConvertible value, string? format, IFormatProvider? formatProvider, StringBuilder builder)
	{
		TypeCode typeCode = value.GetTypeCode();
		if (typeCode == TypeCode.String)
		{
			SerializeStringObject(value.ToString(), format, builder);
			return;
		}
		if (!string.IsNullOrEmpty(format) && value is IFormattable formattable)
		{
			builder.Append(formattable.ToString(format, formatProvider));
			return;
		}
		switch (typeCode)
		{
		case TypeCode.Boolean:
			builder.Append(value.ToBoolean(CultureInfo.InvariantCulture) ? "true" : "false");
			break;
		case TypeCode.Char:
		{
			int num;
			if (_legacyStringQuotes)
			{
				num = ((format != "l") ? 1 : 0);
				if (num != 0)
				{
					builder.Append('"');
				}
			}
			else
			{
				num = 0;
			}
			builder.Append(value.ToChar(CultureInfo.InvariantCulture));
			if (num != 0)
			{
				builder.Append('"');
			}
			break;
		}
		case TypeCode.SByte:
		case TypeCode.Byte:
		case TypeCode.Int16:
		case TypeCode.UInt16:
		case TypeCode.Int32:
		case TypeCode.UInt32:
		case TypeCode.Int64:
		case TypeCode.UInt64:
			if (value is Enum value2)
			{
				AppendEnumAsString(builder, value2);
			}
			else
			{
				builder.AppendNumericInvariant(value, typeCode);
			}
			break;
		default:
			SerializeConvertToString(value, formatProvider, builder);
			break;
		}
	}

	private static void SerializeConvertToString(object? value, IFormatProvider? formatProvider, StringBuilder builder)
	{
		builder.Append(Convert.ToString(value, formatProvider));
	}

	private void SerializeStringObject(string stringValue, string? format, StringBuilder builder)
	{
		int num;
		if (_legacyStringQuotes)
		{
			num = ((format != "l") ? 1 : 0);
			if (num != 0)
			{
				builder.Append('"');
			}
		}
		else
		{
			num = 0;
		}
		builder.Append(stringValue);
		if (num != 0)
		{
			builder.Append('"');
		}
	}

	private void AppendEnumAsString(StringBuilder sb, Enum value)
	{
		if (!_enumCache.TryGetValue(value, out string value2))
		{
			value2 = value.ToString();
			_enumCache.TryAddValue(value, value2);
		}
		sb.Append(value2);
	}

	private bool SerializeWithoutCyclicLoop(IEnumerable collection, string? format, IFormatProvider? formatProvider, StringBuilder builder, SingleItemOptimizedHashSet<object> objectsInPath, int depth)
	{
		if (objectsInPath.Contains(collection))
		{
			return false;
		}
		if (depth > 2)
		{
			return false;
		}
		if (collection is IDictionary dictionary)
		{
			using (new SingleItemOptimizedHashSet<object>.SingleItemScopedInsert(dictionary, ref objectsInPath, forceHashSet: true, _referenceEqualsComparer))
			{
				return SerializeDictionaryObject(dictionary, format, formatProvider, builder, objectsInPath, depth);
			}
		}
		using (new SingleItemOptimizedHashSet<object>.SingleItemScopedInsert(collection, ref objectsInPath, forceHashSet: true, _referenceEqualsComparer))
		{
			return SerializeCollectionObject(collection, format, formatProvider, builder, objectsInPath, depth);
		}
	}

	/// <summary>
	/// Serialize Dictionary as JSON like structure, without { and }
	/// </summary>
	/// <example>
	/// "FirstOrder"=true, "Previous login"=20-12-2017 14:55:32, "number of tries"=1
	/// </example>
	/// <param name="dictionary"></param>
	/// <param name="format">format string of an item</param>
	/// <param name="formatProvider"></param>
	/// <param name="builder"></param>
	/// <param name="objectsInPath"></param>
	/// <param name="depth"></param>
	/// <returns></returns>
	private bool SerializeDictionaryObject(IDictionary dictionary, string? format, IFormatProvider? formatProvider, StringBuilder builder, SingleItemOptimizedHashSet<object> objectsInPath, int depth)
	{
		bool flag = false;
		foreach (DictionaryEntry item in new DictionaryEntryEnumerable(dictionary))
		{
			if (builder.Length > 524288)
			{
				return false;
			}
			if (flag)
			{
				builder.Append(", ");
			}
			SerializeCollectionItem(item.Key, format, formatProvider, builder, ref objectsInPath, depth);
			builder.Append('=');
			SerializeCollectionItem(item.Value, format, formatProvider, builder, ref objectsInPath, depth);
			flag = true;
		}
		return true;
	}

	private bool SerializeCollectionObject(IEnumerable collection, string? format, IFormatProvider? formatProvider, StringBuilder builder, SingleItemOptimizedHashSet<object> objectsInPath, int depth)
	{
		bool flag = false;
		foreach (object item in collection)
		{
			if (builder.Length > 524288)
			{
				return false;
			}
			if (flag)
			{
				builder.Append(", ");
			}
			SerializeCollectionItem(item, format, formatProvider, builder, ref objectsInPath, depth);
			flag = true;
		}
		return true;
	}

	private void SerializeCollectionItem(object item, string? format, IFormatProvider? formatProvider, StringBuilder builder, ref SingleItemOptimizedHashSet<object> objectsInPath, int depth)
	{
		if (item is IConvertible value)
		{
			SerializeConvertibleObject(value, format, formatProvider, builder);
		}
		else if (item is IEnumerable collection)
		{
			SerializeWithoutCyclicLoop(collection, format, formatProvider, builder, objectsInPath, depth + 1);
		}
		else if (!SerializeSimpleObject(item, format, formatProvider, builder))
		{
			SerializeConvertToString(item, formatProvider, builder);
		}
	}

	/// <summary>
	/// Convert a value to a string with format and append to <paramref name="builder" />.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <param name="format">Format sting for the value.</param>
	/// <param name="formatProvider">Format provider for the value.</param>
	/// <param name="builder">Append to this</param>
	public static void FormatToString(object? value, string? format, IFormatProvider? formatProvider, StringBuilder builder)
	{
		if (value is string value2)
		{
			builder.Append(value2);
		}
		else if (value is IFormattable formattable)
		{
			builder.Append(formattable.ToString(format, formatProvider));
		}
		else
		{
			builder.Append(Convert.ToString(value, formatProvider));
		}
	}
}

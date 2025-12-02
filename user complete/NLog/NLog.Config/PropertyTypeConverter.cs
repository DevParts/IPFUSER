using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using NLog.Common;
using NLog.Internal;
using NLog.Targets;

namespace NLog.Config;

/// <summary>
/// Default implementation of <see cref="T:NLog.Config.IPropertyTypeConverter" />
/// </summary>
internal sealed class PropertyTypeConverter : IPropertyTypeConverter
{
	private static Dictionary<Type, Func<string, string?, IFormatProvider?, object?>>? _stringConverters;

	/// <summary>
	/// Singleton instance of the serializer.
	/// </summary>
	public static PropertyTypeConverter Instance { get; } = new PropertyTypeConverter();

	private static Dictionary<Type, Func<string, string?, IFormatProvider?, object?>> StringConverterLookup => _stringConverters ?? (_stringConverters = BuildStringConverterLookup());

	private static Dictionary<Type, Func<string, string?, IFormatProvider?, object?>> BuildStringConverterLookup()
	{
		return new Dictionary<Type, Func<string, string, IFormatProvider, object>>
		{
			{
				typeof(Encoding),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertToEncoding(stringvalue)
			},
			{
				typeof(CultureInfo),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertToCultureInfo(stringvalue)
			},
			{
				typeof(Type),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertToType(stringvalue, throwOnError: true)
			},
			{
				typeof(LineEndingMode),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => LineEndingMode.FromString(stringvalue)
			},
			{
				typeof(LogLevel),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => LogLevel.FromString(stringvalue)
			},
			{
				typeof(Uri),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => new Uri(stringvalue)
			},
			{
				typeof(DateTime),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertToDateTime(format, formatProvider, stringvalue)
			},
			{
				typeof(DateTimeOffset),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertToDateTimeOffset(format, formatProvider, stringvalue)
			},
			{
				typeof(TimeSpan),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertToTimeSpan(format, formatProvider, stringvalue)
			},
			{
				typeof(Guid),
				(string stringvalue, string? format, IFormatProvider? formatProvider) => ConvertGuid(format, stringvalue)
			}
		};
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2057")]
	internal static Type ConvertToType(string stringvalue, bool throwOnError)
	{
		return Type.GetType(stringvalue, throwOnError);
	}

	internal static bool IsComplexType(Type type)
	{
		if (!type.IsValueType && !typeof(IConvertible).IsAssignableFrom(type) && !StringConverterLookup.ContainsKey(type))
		{
			return type.GetFirstCustomAttribute<TypeConverterAttribute>() == null;
		}
		return false;
	}

	/// <inheritdoc />
	public object? Convert(object? propertyValue, Type propertyType, string? format, IFormatProvider? formatProvider)
	{
		if (propertyValue == null || (object)propertyType == null || propertyType.Equals(typeof(object)))
		{
			return propertyValue;
		}
		Type type = propertyValue.GetType();
		if (propertyType.IsAssignableFrom(type))
		{
			return propertyValue;
		}
		Type underlyingType = Nullable.GetUnderlyingType(propertyType);
		if (underlyingType != null)
		{
			if (underlyingType.IsAssignableFrom(type))
			{
				return propertyValue;
			}
			if (propertyValue is string value && StringHelpers.IsNullOrWhiteSpace(value))
			{
				return null;
			}
			propertyType = underlyingType;
		}
		return ChangeObjectType(propertyValue, propertyType, format, formatProvider);
	}

	private static bool TryConvertFromString(string propertyString, Type propertyType, string? format, IFormatProvider? formatProvider, out object? propertyValue)
	{
		propertyValue = (propertyString = propertyString.Trim());
		if (StringConverterLookup.TryGetValue(propertyType, out Func<string, string, IFormatProvider, object> value))
		{
			propertyValue = value(propertyString, format, formatProvider);
			return true;
		}
		if (propertyType.IsEnum)
		{
			return ConversionHelpers.TryParseEnum(propertyString, propertyType, out propertyValue);
		}
		if (PropertyHelper.TryTypeConverterConversion(propertyType, propertyString, out object newValue))
		{
			propertyValue = newValue;
			return true;
		}
		return false;
	}

	private static object? ChangeObjectType(object propertyValue, Type propertyType, string? format, IFormatProvider? formatProvider)
	{
		if (propertyValue is string propertyString && TryConvertFromString(propertyString, propertyType, format, formatProvider, out object propertyValue2))
		{
			return propertyValue2;
		}
		object convertedValue;
		if (propertyValue is IConvertible convertible)
		{
			switch (convertible.GetTypeCode())
			{
			case TypeCode.DBNull:
				return convertible;
			case TypeCode.Empty:
				return null;
			}
		}
		else if (TryConvertToType(propertyValue, propertyType, out convertedValue))
		{
			return convertedValue;
		}
		if (!string.IsNullOrEmpty(format) && propertyValue is IFormattable formattable)
		{
			propertyValue = formattable.ToString(format, formatProvider);
		}
		return System.Convert.ChangeType(propertyValue, propertyType, formatProvider);
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2026")]
	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2067")]
	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2072")]
	private static bool TryConvertToType(object propertyValue, Type propertyType, out object? convertedValue)
	{
		if (propertyValue == null || propertyType.IsAssignableFrom(propertyValue.GetType()))
		{
			convertedValue = null;
			return false;
		}
		TypeConverter converter = TypeDescriptor.GetConverter(propertyValue.GetType());
		if (converter != null && converter.CanConvertTo(propertyType))
		{
			convertedValue = converter.ConvertTo(propertyValue, propertyType);
			return true;
		}
		convertedValue = null;
		return false;
	}

	private static Guid ConvertGuid(string? format, string propertyString)
	{
		if (!string.IsNullOrEmpty(format))
		{
			return Guid.ParseExact(propertyString, format);
		}
		return Guid.Parse(propertyString);
	}

	internal static CultureInfo? ConvertToCultureInfo(string? stringValue)
	{
		if (StringHelpers.IsNullOrWhiteSpace(stringValue))
		{
			return null;
		}
		if ("InvariantCulture".Equals(stringValue, StringComparison.OrdinalIgnoreCase))
		{
			return CultureInfo.InvariantCulture;
		}
		if ("CurrentCulture".Equals(stringValue, StringComparison.OrdinalIgnoreCase))
		{
			return CultureInfo.CurrentCulture;
		}
		return new CultureInfo(stringValue);
	}

	internal static Encoding ConvertToEncoding(string stringValue)
	{
		stringValue = stringValue.Trim();
		if (string.Equals(stringValue, "UTF8", StringComparison.OrdinalIgnoreCase))
		{
			stringValue = Encoding.UTF8.WebName;
		}
		return Encoding.GetEncoding(stringValue);
	}

	private static TimeSpan ConvertToTimeSpan(string? format, IFormatProvider? formatProvider, string propertyString)
	{
		if (!string.IsNullOrEmpty(format))
		{
			return TimeSpan.ParseExact(propertyString, format, formatProvider);
		}
		return TimeSpan.Parse(propertyString, formatProvider);
	}

	private static DateTimeOffset ConvertToDateTimeOffset(string? format, IFormatProvider? formatProvider, string propertyString)
	{
		if (!string.IsNullOrEmpty(format))
		{
			return DateTimeOffset.ParseExact(propertyString, format, formatProvider);
		}
		return DateTimeOffset.Parse(propertyString, formatProvider);
	}

	private static DateTime ConvertToDateTime(string? format, IFormatProvider? formatProvider, string propertyString)
	{
		if (!string.IsNullOrEmpty(format))
		{
			return DateTime.ParseExact(propertyString, format, formatProvider);
		}
		return DateTime.Parse(propertyString, formatProvider);
	}
}

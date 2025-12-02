using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters;

/// <summary>
/// Converts an Entity Framework <see cref="T:System.Data.EntityKeyMember" /> to and from JSON.
/// </summary>
[RequiresUnreferencedCode("Newtonsoft.Json relies on reflection over types that may be removed when trimming.")]
[RequiresDynamicCode("Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.")]
public class EntityKeyMemberConverter : JsonConverter
{
	private const string EntityKeyMemberFullTypeName = "System.Data.EntityKeyMember";

	private const string KeyPropertyName = "Key";

	private const string TypePropertyName = "Type";

	private const string ValuePropertyName = "Value";

	private static ReflectionObject? _reflectionObject;

	/// <summary>
	/// Writes the JSON representation of the object.
	/// </summary>
	/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
	/// <param name="value">The value.</param>
	/// <param name="serializer">The calling serializer.</param>
	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}
		EnsureReflectionObject(value.GetType());
		DefaultContractResolver defaultContractResolver = serializer.ContractResolver as DefaultContractResolver;
		string value2 = (string)_reflectionObject.GetValue(value, "Key");
		object value3 = _reflectionObject.GetValue(value, "Value");
		Type type = value3?.GetType();
		writer.WriteStartObject();
		writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Key") : "Key");
		writer.WriteValue(value2);
		writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Type") : "Type");
		writer.WriteValue(type?.FullName);
		writer.WritePropertyName((defaultContractResolver != null) ? defaultContractResolver.GetResolvedPropertyName("Value") : "Value");
		if (type != null)
		{
			if (JsonSerializerInternalWriter.TryConvertToString(value3, type, out string s))
			{
				writer.WriteValue(s);
			}
			else
			{
				writer.WriteValue(value3);
			}
		}
		else
		{
			writer.WriteNull();
		}
		writer.WriteEndObject();
	}

	private static void ReadAndAssertProperty(JsonReader reader, string propertyName)
	{
		reader.ReadAndAssert();
		if (reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value?.ToString(), propertyName, StringComparison.OrdinalIgnoreCase))
		{
			throw new JsonSerializationException("Expected JSON property '{0}'.".FormatWith(CultureInfo.InvariantCulture, propertyName));
		}
	}

	/// <summary>
	/// Reads the JSON representation of the object.
	/// </summary>
	/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
	/// <param name="objectType">Type of the object.</param>
	/// <param name="existingValue">The existing value of object being read.</param>
	/// <param name="serializer">The calling serializer.</param>
	/// <returns>The object value.</returns>
	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		EnsureReflectionObject(objectType);
		object obj = _reflectionObject.Creator();
		ReadAndAssertProperty(reader, "Key");
		reader.ReadAndAssert();
		_reflectionObject.SetValue(obj, "Key", reader.Value?.ToString());
		ReadAndAssertProperty(reader, "Type");
		reader.ReadAndAssert();
		Type type = Type.GetType(reader.Value?.ToString());
		ReadAndAssertProperty(reader, "Value");
		reader.ReadAndAssert();
		_reflectionObject.SetValue(obj, "Value", serializer.Deserialize(reader, type));
		reader.ReadAndAssert();
		return obj;
	}

	private static void EnsureReflectionObject(Type objectType)
	{
		if (_reflectionObject == null)
		{
			_reflectionObject = ReflectionObject.Create(objectType, "Key", "Value");
		}
	}

	/// <summary>
	/// Determines whether this instance can convert the specified object type.
	/// </summary>
	/// <param name="objectType">Type of the object.</param>
	/// <returns>
	/// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
	/// </returns>
	public override bool CanConvert(Type objectType)
	{
		return objectType.AssignableToTypeName("System.Data.EntityKeyMember");
	}
}

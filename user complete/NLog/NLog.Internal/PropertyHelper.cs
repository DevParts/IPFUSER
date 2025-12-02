using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using NLog.Common;
using NLog.Conditions;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.Internal;

/// <summary>
/// Reflection helpers for accessing properties.
/// </summary>
internal static class PropertyHelper
{
	private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>?> _parameterInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

	private static readonly Dictionary<Type, Func<string, ConfigurationItemFactory, object?>> _propertyConversionMapper = BuildPropertyConversionMapper();

	private static readonly ArrayParameterAttribute _arrayParameterAttribute = new ArrayParameterAttribute(typeof(string), string.Empty);

	private static readonly DefaultParameterAttribute _defaultParameterAttribute = new DefaultParameterAttribute();

	private static readonly NLogConfigurationIgnorePropertyAttribute _ignorePropertyAttribute = new NLogConfigurationIgnorePropertyAttribute();

	private static readonly NLogConfigurationItemAttribute _configPropertyAttribute = new NLogConfigurationItemAttribute();

	private static readonly FlagsAttribute _flagsAttribute = new FlagsAttribute();

	private static Dictionary<Type, Func<string, ConfigurationItemFactory, object?>> BuildPropertyConversionMapper()
	{
		return new Dictionary<Type, Func<string, ConfigurationItemFactory, object>>
		{
			{
				typeof(Layout),
				TryParseLayoutValue
			},
			{
				typeof(SimpleLayout),
				TryParseLayoutValue
			},
			{
				typeof(ConditionExpression),
				TryParseConditionValue
			},
			{
				typeof(Encoding),
				(string stringvalue, ConfigurationItemFactory factory) => PropertyTypeConverter.ConvertToEncoding(stringvalue)
			},
			{
				typeof(string),
				(string stringvalue, ConfigurationItemFactory factory) => stringvalue
			},
			{
				typeof(int),
				(string stringvalue, ConfigurationItemFactory factory) => Convert.ChangeType(stringvalue.Trim(), TypeCode.Int32, CultureInfo.InvariantCulture)
			},
			{
				typeof(bool),
				(string stringvalue, ConfigurationItemFactory factory) => Convert.ChangeType(stringvalue.Trim(), TypeCode.Boolean, CultureInfo.InvariantCulture)
			},
			{
				typeof(CultureInfo),
				(string stringvalue, ConfigurationItemFactory factory) => PropertyTypeConverter.ConvertToCultureInfo(stringvalue)
			},
			{
				typeof(Type),
				(string stringvalue, ConfigurationItemFactory factory) => PropertyTypeConverter.ConvertToType(stringvalue.Trim(), throwOnError: true)
			},
			{
				typeof(LineEndingMode),
				(string stringvalue, ConfigurationItemFactory factory) => LineEndingMode.FromString(stringvalue.Trim())
			},
			{
				typeof(Uri),
				(string stringvalue, ConfigurationItemFactory factory) => new Uri(stringvalue.Trim())
			}
		};
	}

	internal static void SetPropertyFromString(object targetObject, PropertyInfo propInfo, string stringValue, ConfigurationItemFactory configurationItemFactory)
	{
		object newValue = null;
		try
		{
			Type type = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
			if (((object)type == propInfo.PropertyType || !StringHelpers.IsNullOrWhiteSpace(stringValue)) && !TryNLogSpecificConversion(type, stringValue, configurationItemFactory, out newValue))
			{
				if (propInfo.IsDefined(_arrayParameterAttribute.GetType(), inherit: false))
				{
					throw new NotSupportedException("'" + targetObject?.GetType()?.Name + "' cannot assign property '" + propInfo.Name + "', because property of type array and not scalar value: '" + stringValue + "'.");
				}
				if (!TryGetEnumValue(type, stringValue, out newValue) && !TryImplicitConversion(type, stringValue, out newValue) && !TryFlatListConversion(targetObject, propInfo, stringValue, configurationItemFactory, out newValue) && !TryTypeConverterConversion(type, stringValue, out newValue))
				{
					newValue = Convert.ChangeType(stringValue, type, CultureInfo.InvariantCulture);
				}
			}
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new NLogConfigurationException("'" + targetObject?.GetType()?.Name + "' cannot assign property '" + propInfo.Name + "'='" + stringValue + "'. Error: " + ex.Message, ex);
		}
		SetPropertyValueForObject(targetObject, newValue, propInfo);
	}

	internal static void SetPropertyValueForObject(object targetObject, object? value, PropertyInfo propInfo)
	{
		try
		{
			propInfo.SetValue(targetObject, value, null);
		}
		catch (TargetInvocationException ex)
		{
			throw new NLogConfigurationException("'" + targetObject?.GetType()?.Name + "' cannot assign property '" + propInfo.Name + "'", ex.InnerException ?? ex);
		}
		catch (Exception ex2)
		{
			if (ex2.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new NLogConfigurationException("'" + targetObject?.GetType()?.Name + "' cannot assign property '" + propInfo.Name + "'. Error=" + ex2.Message, ex2);
		}
	}

	/// <summary>
	/// Get property info
	/// </summary>
	/// <param name="configFactory">Configuration Reflection Helper</param>
	/// <param name="obj">object which could have property <paramref name="propertyName" /></param>
	/// <param name="propertyName">property name on <paramref name="obj" /></param>
	/// <param name="result">result when success.</param>
	/// <returns>success.</returns>
	internal static bool TryGetPropertyInfo(ConfigurationItemFactory configFactory, object obj, string propertyName, out PropertyInfo? result)
	{
		Dictionary<string, PropertyInfo> dictionary = TryLookupConfigItemProperties(configFactory, obj.GetType());
		if (dictionary == null)
		{
			if (!string.IsNullOrEmpty(propertyName))
			{
				return TryGetPropertyInfo(obj, propertyName, out result);
			}
			result = null;
			return false;
		}
		return dictionary.TryGetValue(propertyName, out result);
	}

	[UnconditionalSuppressMessage("Trimming - Ignore since obsolete", "IL2075")]
	[Obsolete("Instead use RegisterType<T>, as dynamic Assembly loading will be moved out. Marked obsolete with NLog v5.2")]
	private static bool TryGetPropertyInfo(object obj, string propertyName, out PropertyInfo? result)
	{
		InternalLogger.Debug("Object reflection needed to configure instance of type: {0} (Lookup property={1})", obj.GetType(), propertyName);
		PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
		if (property != null)
		{
			result = property;
			return true;
		}
		result = null;
		return false;
	}

	internal static Type? GetArrayItemType(PropertyInfo propInfo)
	{
		return propInfo.GetFirstCustomAttribute<ArrayParameterAttribute>()?.ItemType;
	}

	internal static bool IsConfigurationItemType(ConfigurationItemFactory configFactory, Type? type)
	{
		if ((object)type == null || IsSimplePropertyType(type))
		{
			return false;
		}
		if (typeof(ISupportsInitialize).IsAssignableFrom(type))
		{
			return true;
		}
		if (typeof(IEnumerable).IsAssignableFrom(type))
		{
			return true;
		}
		return TryLookupConfigItemProperties(configFactory, type) != null;
	}

	internal static Dictionary<string, PropertyInfo> GetAllConfigItemProperties(ConfigurationItemFactory configFactory, Type type)
	{
		return TryLookupConfigItemProperties(configFactory, type) ?? new Dictionary<string, PropertyInfo>();
	}

	private static Dictionary<string, PropertyInfo>? TryLookupConfigItemProperties(ConfigurationItemFactory configFactory, Type type)
	{
		lock (_parameterInfoCache)
		{
			if (!_parameterInfoCache.TryGetValue(type, out Dictionary<string, PropertyInfo> value))
			{
				if (TryCreatePropertyInfoDictionary(configFactory, type, out value))
				{
					_parameterInfoCache[type] = value;
				}
				else
				{
					_parameterInfoCache[type] = null;
				}
			}
			return value;
		}
	}

	internal static bool IsSimplePropertyType(Type type)
	{
		if (Type.GetTypeCode(type) != TypeCode.Object)
		{
			return true;
		}
		if (type == typeof(CultureInfo))
		{
			return true;
		}
		if (type == typeof(Type))
		{
			return true;
		}
		if (type == typeof(Encoding))
		{
			return true;
		}
		if (type == typeof(LogLevel))
		{
			return true;
		}
		return false;
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2070")]
	private static bool TryImplicitConversion(Type resultType, string value, out object? result)
	{
		try
		{
			if (IsSimplePropertyType(resultType))
			{
				result = null;
				return false;
			}
			MethodInfo method = resultType.GetMethod("op_Implicit", BindingFlags.Static | BindingFlags.Public, null, new Type[1] { value.GetType() }, null);
			if ((object)method == null || !resultType.IsAssignableFrom(method.ReturnType))
			{
				result = null;
				return false;
			}
			result = method.Invoke(null, new object[1] { value });
			return true;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Implicit Conversion Failed of {0} to {1}", value, resultType);
		}
		result = null;
		return false;
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2067")]
	private static bool TryNLogSpecificConversion(Type propertyType, string value, ConfigurationItemFactory configurationItemFactory, out object? newValue)
	{
		if (_propertyConversionMapper.TryGetValue(propertyType, out Func<string, ConfigurationItemFactory, object> value2))
		{
			newValue = value2(value, configurationItemFactory);
			return true;
		}
		if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Layout<>))
		{
			SimpleLayout simpleLayout = (string.IsNullOrEmpty(value) ? SimpleLayout.Default : new SimpleLayout(value, configurationItemFactory));
			newValue = Activator.CreateInstance(propertyType, BindingFlags.Instance | BindingFlags.Public, null, new object[1] { simpleLayout }, null);
			return true;
		}
		newValue = null;
		return false;
	}

	private static bool TryGetEnumValue(Type resultType, string value, out object? result)
	{
		if (!resultType.IsEnum)
		{
			result = null;
			return false;
		}
		if (!StringHelpers.IsNullOrWhiteSpace(value))
		{
			try
			{
				result = (Enum)Enum.Parse(resultType, value, ignoreCase: true);
				return true;
			}
			catch (ArgumentException innerException)
			{
				throw new ArgumentException("Failed parsing Enum " + resultType.Name + " from value: " + value, innerException);
			}
		}
		result = null;
		return false;
	}

	private static object TryParseLayoutValue(string stringValue, ConfigurationItemFactory configurationItemFactory)
	{
		if (!string.IsNullOrEmpty(stringValue))
		{
			return new SimpleLayout(stringValue, configurationItemFactory);
		}
		return Layout.Empty;
	}

	private static object TryParseConditionValue(string stringValue, ConfigurationItemFactory configurationItemFactory)
	{
		try
		{
			return ConditionParser.ParseExpression(stringValue, configurationItemFactory);
		}
		catch (ConditionParseException ex)
		{
			throw new NLogConfigurationException("Cannot parse ConditionExpression '" + stringValue + "'. Error: " + ex.Message, ex);
		}
	}

	/// <summary>
	/// Try parse of string to (Generic) list, comma separated.
	/// </summary>
	/// <remarks>
	/// If there is a comma in the value, then (single) quote the value. For single quotes, use the backslash as escape
	/// </remarks>
	private static bool TryFlatListConversion(object obj, PropertyInfo propInfo, string valueRaw, ConfigurationItemFactory configurationItemFactory, out object? newValue)
	{
		Type propertyType = propInfo.PropertyType;
		if (!propertyType.IsGenericType || !typeof(IEnumerable).IsAssignableFrom(propertyType))
		{
			newValue = null;
			return false;
		}
		try
		{
			if (TryCreateCollectionObject(obj, propInfo, out object collectionObject, out MethodInfo collectionAddMethod, out Type collectionItemType) && collectionAddMethod != null && collectionItemType != null)
			{
				foreach (string item in valueRaw.SplitQuoted(',', '\'', '\\'))
				{
					if (!TryNLogSpecificConversion(collectionItemType, item, configurationItemFactory, out newValue) && !TryGetEnumValue(collectionItemType, item, out newValue) && !TryImplicitConversion(collectionItemType, item, out newValue) && !TryTypeConverterConversion(collectionItemType, item, out newValue))
					{
						newValue = Convert.ChangeType(item, collectionItemType, CultureInfo.InvariantCulture);
					}
					collectionAddMethod.Invoke(collectionObject, new object[1] { newValue });
				}
				newValue = collectionObject;
				return true;
			}
			newValue = null;
			return false;
		}
		catch (Exception innerException)
		{
			throw new NLogConfigurationException($"Failed to parse collection for property '{propInfo.Name}' on {obj.GetType()} with value '{valueRaw}'", innerException);
		}
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2072")]
	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2075")]
	private static bool TryCreateCollectionObject(object obj, PropertyInfo propInfo, out object? collectionObject, out MethodInfo? collectionAddMethod, out Type? collectionItemType)
	{
		collectionObject = null;
		collectionAddMethod = null;
		collectionItemType = null;
		Type propertyType = propInfo.PropertyType;
		if (TryCreateListCollection<string>(propertyType, out collectionObject, out collectionAddMethod, out collectionItemType))
		{
			return true;
		}
		if (TryCreateListCollection<int>(propertyType, out collectionObject, out collectionAddMethod, out collectionItemType))
		{
			return true;
		}
		Type genericTypeDefinition = propertyType.GetGenericTypeDefinition();
		if (genericTypeDefinition == typeof(List<>))
		{
			collectionObject = Activator.CreateInstance(propertyType);
			collectionAddMethod = propertyType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
			collectionItemType = propertyType.GetGenericArguments()[0];
			return true;
		}
		if (genericTypeDefinition != typeof(IList<>) && genericTypeDefinition != typeof(IEnumerable<>) && genericTypeDefinition != typeof(HashSet<>) && genericTypeDefinition != typeof(ISet<>))
		{
			return false;
		}
		object obj2 = (propInfo.IsValidPublicProperty() ? propInfo.GetPropertyValue(obj) : null);
		if (obj2 != null && obj2.GetType().IsGenericType)
		{
			if (obj2.GetType().GetGenericTypeDefinition() == typeof(List<>))
			{
				Type type = obj2.GetType();
				collectionObject = Activator.CreateInstance(type);
				collectionAddMethod = type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
				collectionItemType = type.GetGenericArguments()[0];
				return true;
			}
			if (obj2.GetType().GetGenericTypeDefinition() == typeof(HashSet<>))
			{
				object obj3 = null;
				Type type2 = obj2.GetType();
				PropertyInfo property = type2.GetProperty("Comparer", BindingFlags.Instance | BindingFlags.Public);
				if (property.IsValidPublicProperty())
				{
					obj3 = property.GetPropertyValue(obj2);
				}
				if (obj3 != null)
				{
					ConstructorInfo constructor = type2.GetConstructor(new Type[1] { obj3.GetType() });
					if (constructor != null)
					{
						collectionObject = constructor.Invoke(new object[1] { obj3 });
					}
					else
					{
						collectionObject = Activator.CreateInstance(type2);
					}
				}
				else
				{
					collectionObject = Activator.CreateInstance(type2);
				}
				collectionAddMethod = type2.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
				collectionItemType = type2.GetGenericArguments()[0];
				return true;
			}
		}
		if (TryCreateHashSetCollection<string>(propertyType, out collectionObject, out collectionAddMethod, out collectionItemType))
		{
			return true;
		}
		if (TryCreateHashSetCollection<int>(propertyType, out collectionObject, out collectionAddMethod, out collectionItemType))
		{
			return true;
		}
		if (genericTypeDefinition == typeof(HashSet<>))
		{
			collectionObject = Activator.CreateInstance(propertyType);
			collectionAddMethod = propertyType.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
			collectionItemType = propertyType.GetGenericArguments()[0];
			return true;
		}
		InternalLogger.Debug("Object type reflection needed to configure instance of type: {0}", propertyType);
		return TryCreateTypeCollection(propertyType, out collectionObject, out collectionAddMethod, out collectionItemType);
	}

	private static bool TryCreateListCollection<T>(Type collectionType, out object? collectionObject, out MethodInfo? collectionAddMethod, out Type? collectionItemType)
	{
		if (collectionType.IsAssignableFrom(typeof(List<T>)))
		{
			collectionObject = new List<T>();
			collectionAddMethod = typeof(List<T>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
			collectionItemType = typeof(T);
			return true;
		}
		collectionObject = null;
		collectionAddMethod = null;
		collectionItemType = null;
		return false;
	}

	private static bool TryCreateHashSetCollection<T>(Type collectionType, out object? collectionObject, out MethodInfo? collectionAddMethod, out Type? collectionItemType)
	{
		if (collectionType.IsAssignableFrom(typeof(HashSet<T>)))
		{
			collectionObject = new HashSet<T>();
			collectionAddMethod = typeof(HashSet<T>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
			collectionItemType = typeof(T);
			return true;
		}
		collectionObject = null;
		collectionAddMethod = null;
		collectionItemType = null;
		return false;
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL3050")]
	private static bool TryCreateTypeCollection(Type propertyType, out object collectionObject, out MethodInfo collectionAddMethod, out Type collectionItemType)
	{
		if (propertyType.GetGenericTypeDefinition() == typeof(ISet<>))
		{
			Type type = typeof(HashSet<>).MakeGenericType(propertyType.GetGenericArguments());
			collectionAddMethod = type.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
			collectionItemType = propertyType.GetGenericArguments()[0];
			collectionObject = Activator.CreateInstance(type);
			return true;
		}
		Type type2 = typeof(List<>).MakeGenericType(propertyType.GetGenericArguments());
		collectionAddMethod = type2.GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
		collectionItemType = propertyType.GetGenericArguments()[0];
		collectionObject = Activator.CreateInstance(type2);
		return true;
	}

	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2026")]
	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2067")]
	[UnconditionalSuppressMessage("Trimming - Allow converting option-values from config", "IL2072")]
	internal static bool TryTypeConverterConversion(Type type, string value, out object? newValue)
	{
		if (typeof(IConvertible).IsAssignableFrom(type) || type.IsAssignableFrom(typeof(string)))
		{
			newValue = null;
			return false;
		}
		try
		{
			InternalLogger.Debug("Object reflection needed for creating external type: {0} from string-value: {1}", type, value);
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (converter.CanConvertFrom(typeof(string)))
			{
				newValue = converter.ConvertFromInvariantString(value);
				return true;
			}
			newValue = null;
			return false;
		}
		catch (MissingMethodException ex)
		{
			InternalLogger.Error(ex, "Error in lookup of TypeDescriptor for type={0} to convert value '{1}'", type, value);
			newValue = null;
			return false;
		}
	}

	private static bool TryCreatePropertyInfoDictionary(ConfigurationItemFactory configFactory, Type objectType, out Dictionary<string, PropertyInfo>? result)
	{
		result = null;
		try
		{
			if (!typeof(ISupportsInitialize).IsAssignableFrom(objectType) && !objectType.IsDefined(_configPropertyAttribute.GetType(), inherit: true))
			{
				return false;
			}
			Dictionary<string, PropertyInfo> dictionary = configFactory.TryGetTypeProperties(objectType);
			if (dictionary == null)
			{
				return false;
			}
			if (dictionary.Count == 0)
			{
				result = dictionary;
				return true;
			}
			if (!HasCustomConfigurationProperties(objectType, dictionary))
			{
				result = dictionary;
				return true;
			}
			bool checkDefaultValue = typeof(LayoutRenderer).IsAssignableFrom(objectType);
			result = new Dictionary<string, PropertyInfo>(dictionary.Count + 4, StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, PropertyInfo> item in dictionary)
			{
				PropertyInfo value = item.Value;
				if (IncludeConfigurationPropertyInfo(objectType, value, checkDefaultValue, out string overridePropertyName))
				{
					result[value.Name] = value;
					if (overridePropertyName != null)
					{
						result[overridePropertyName] = value;
					}
				}
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Debug(ex, "Type reflection not possible for type {0}. Maybe because of .NET Native.", objectType);
		}
		return result != null;
	}

	private static bool HasCustomConfigurationProperties(Type objectType, Dictionary<string, PropertyInfo> objectProperties)
	{
		bool checkDefaultValue = typeof(LayoutRenderer).IsAssignableFrom(objectType);
		foreach (KeyValuePair<string, PropertyInfo> objectProperty in objectProperties)
		{
			if (!IncludeConfigurationPropertyInfo(objectType, objectProperty.Value, checkDefaultValue, out string overridePropertyName) || overridePropertyName != null)
			{
				return true;
			}
		}
		return false;
	}

	private static bool IncludeConfigurationPropertyInfo(Type objectType, PropertyInfo propInfo, bool checkDefaultValue, out string? overridePropertyName)
	{
		overridePropertyName = null;
		try
		{
			Type propertyType = propInfo.PropertyType;
			if ((object)propertyType == null)
			{
				return false;
			}
			if (checkDefaultValue && propInfo.IsDefined(_defaultParameterAttribute.GetType(), inherit: false))
			{
				overridePropertyName = string.Empty;
				return true;
			}
			if (IsSimplePropertyType(propertyType))
			{
				return true;
			}
			if (typeof(ISupportsInitialize).IsAssignableFrom(propertyType))
			{
				return true;
			}
			if (propInfo.IsDefined(_ignorePropertyAttribute.GetType(), inherit: false))
			{
				return false;
			}
			if (typeof(IEnumerable).IsAssignableFrom(propertyType))
			{
				ArrayParameterAttribute firstCustomAttribute = propInfo.GetFirstCustomAttribute<ArrayParameterAttribute>();
				if (firstCustomAttribute != null)
				{
					overridePropertyName = firstCustomAttribute.ElementName;
					return true;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			InternalLogger.Debug(ex, "Type reflection not possible for property {0} on type {1}. Maybe because of .NET Native.", propInfo.Name, objectType);
			return false;
		}
	}
}

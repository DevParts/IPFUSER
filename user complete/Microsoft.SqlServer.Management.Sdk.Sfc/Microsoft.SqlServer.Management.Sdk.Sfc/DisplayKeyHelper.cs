using System;
using System.Reflection;
using System.Resources;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal static class DisplayKeyHelper
{
	private static DisplayNameKeyAttribute displayNameKey = new DisplayNameKeyAttribute(" ");

	private static DisplayDescriptionKeyAttribute displayDiscKey = new DisplayDescriptionKeyAttribute(" ");

	private static DisplayCategoryKeyAttribute displayCatKey = new DisplayCategoryKeyAttribute(" ");

	private static IDisplayKey GetDisplayKey(Type keyAttribute)
	{
		IDisplayKey result = null;
		if (keyAttribute.Equals(typeof(DisplayNameKeyAttribute)))
		{
			result = displayNameKey;
		}
		else if (keyAttribute.Equals(typeof(DisplayDescriptionKeyAttribute)))
		{
			result = displayDiscKey;
		}
		else if (keyAttribute.Equals(typeof(DisplayCategoryKeyAttribute)))
		{
			result = displayCatKey;
		}
		else
		{
			TraceHelper.Assert(condition: false, "keyAttribute " + keyAttribute.Name + " is of unknown type");
		}
		return result;
	}

	public static string GetValueFromCustomAttribute(FieldInfo field, Type keyAttribute, ResourceManager resourceManager, bool isDefault)
	{
		string text = null;
		if (isDefault)
		{
			string defaultKey = GetDisplayKey(keyAttribute).GetDefaultKey(field);
			return GetDisplayValue(defaultKey, resourceManager);
		}
		object[] array = null;
		array = field.GetCustomAttributes(keyAttribute, inherit: true);
		return GetCustomDisplayValue(array, resourceManager);
	}

	public static string GetValueFromCustomAttribute(PropertyInfo property, Type keyAttribute, ResourceManager resourceManager, bool isDefault)
	{
		string text = null;
		if (isDefault)
		{
			string defaultKey = GetDisplayKey(keyAttribute).GetDefaultKey(property);
			return GetDisplayValue(defaultKey, resourceManager);
		}
		object[] array = null;
		array = property.GetCustomAttributes(keyAttribute, inherit: true);
		return GetCustomDisplayValue(array, resourceManager);
	}

	public static string GetValueFromCustomAttribute(Type type, Type keyAttribute, ResourceManager resourceManager, bool isDefault)
	{
		string text = null;
		if (isDefault)
		{
			string defaultKey = GetDisplayKey(keyAttribute).GetDefaultKey(type);
			return GetDisplayValue(defaultKey, resourceManager);
		}
		object[] array = null;
		array = type.GetCustomAttributes(keyAttribute, inherit: true);
		return GetCustomDisplayValue(array, resourceManager);
	}

	private static string GetDisplayValue(string key, ResourceManager resourceManager)
	{
		string result = null;
		if (resourceManager != null)
		{
			result = resourceManager.GetString(key);
		}
		return result;
	}

	private static string GetCustomDisplayValue(object[] customAttributes, ResourceManager resourceManager)
	{
		string result = null;
		if (customAttributes != null && 0 < customAttributes.GetLength(0) && resourceManager != null)
		{
			string key = ((IDisplayKey)customAttributes[0]).Key;
			result = resourceManager.GetString(key);
		}
		return result;
	}

	public static string ConvertNullToEmptyString(string value)
	{
		if (value != null)
		{
			return value;
		}
		return string.Empty;
	}

	public static string ConstructDefaultKey(string postfix, string delim, PropertyInfo property)
	{
		return property.DeclaringType.Name + delim + property.Name + postfix;
	}

	public static string ConstructDefaultKey(string postfix, string delim, Type type)
	{
		return type.Name + delim + postfix;
	}

	public static string ConstructDefaultKey(string postfix, string delim, FieldInfo field)
	{
		return string.Concat(field.DeclaringType.Name, delim, field.MemberType, delim, field.Name, postfix);
	}
}

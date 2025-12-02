using System;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class LocalizablePropertyDescriptor : PropertyDescriptor
{
	private PropertyInfo property;

	private string displayName;

	private string displayCategory;

	private string displayDescription;

	private int displayOrdinal;

	private TypeConverter typeConverter;

	private bool readonlyOverride;

	public override string Category => displayCategory;

	public override Type ComponentType => property.ReflectedType;

	public override string Description => displayDescription;

	public override bool DesignTimeOnly
	{
		get
		{
			bool result = false;
			object[] customAttributes = property.GetCustomAttributes(typeof(DesignOnlyAttribute), inherit: true);
			if (customAttributes != null && 0 < customAttributes.GetLength(0))
			{
				result = ((DesignOnlyAttribute)customAttributes[0]).IsDesignOnly;
			}
			return result;
		}
	}

	public override string DisplayName => displayName;

	public override bool IsBrowsable
	{
		get
		{
			bool result = true;
			object[] customAttributes = property.GetCustomAttributes(typeof(BrowsableAttribute), inherit: true);
			if (customAttributes != null && 0 < customAttributes.GetLength(0))
			{
				result = ((BrowsableAttribute)customAttributes[0]).Browsable;
			}
			return result;
		}
	}

	public override bool IsLocalizable
	{
		get
		{
			bool result = false;
			object[] customAttributes = property.GetCustomAttributes(typeof(LocalizableAttribute), inherit: true);
			if (customAttributes != null && 0 < customAttributes.GetLength(0))
			{
				result = ((LocalizableAttribute)customAttributes[0]).IsLocalizable;
			}
			return result;
		}
	}

	public override bool IsReadOnly
	{
		get
		{
			if (readonlyOverride)
			{
				return true;
			}
			if (!property.CanWrite)
			{
				return true;
			}
			bool result = false;
			object[] customAttributes = property.GetCustomAttributes(typeof(ReadOnlyAttribute), inherit: true);
			if (customAttributes != null && 0 < customAttributes.GetLength(0))
			{
				result = ((ReadOnlyAttribute)customAttributes[0]).IsReadOnly;
			}
			return result;
		}
	}

	public override string Name => property.Name;

	public override Type PropertyType => property.PropertyType;

	public int DisplayOrdinal => displayOrdinal;

	public override TypeConverter Converter
	{
		get
		{
			if (typeConverter == null)
			{
				return base.Converter;
			}
			return typeConverter;
		}
	}

	public LocalizablePropertyDescriptor(PropertyInfo property, ResourceManager resourceManager, bool isDefaultResourceManager)
		: base(property.Name, null)
	{
		TraceHelper.Assert(property != null, "unexpected null property object");
		TraceHelper.Assert(resourceManager != null, "resourceManager is null, is the resource string name correct?");
		if (property == null)
		{
			throw new ArgumentNullException("property");
		}
		this.property = property;
		displayCategory = DisplayKeyHelper.GetValueFromCustomAttribute(property, typeof(DisplayCategoryKeyAttribute), resourceManager, isDefaultResourceManager);
		if (displayCategory == null)
		{
			displayCategory = DisplayKeyHelper.ConvertNullToEmptyString(GetCategoryAttribute(property));
		}
		displayName = DisplayKeyHelper.GetValueFromCustomAttribute(property, typeof(DisplayNameKeyAttribute), resourceManager, isDefaultResourceManager);
		if (displayName == null || displayName.Length == 0)
		{
			displayName = property.Name;
		}
		displayDescription = DisplayKeyHelper.GetValueFromCustomAttribute(property, typeof(DisplayDescriptionKeyAttribute), resourceManager, isDefaultResourceManager);
		if (displayDescription == null)
		{
			displayDescription = DisplayKeyHelper.ConvertNullToEmptyString(GetDescriptionAttribute(property));
		}
		object[] array = null;
		array = property.GetCustomAttributes(typeof(PropertyOrderAttribute), inherit: true);
		if (array != null && 0 < array.GetLength(0))
		{
			displayOrdinal = ((PropertyOrderAttribute)array[0]).Order;
		}
		else
		{
			displayOrdinal = 0;
		}
		array = property.GetCustomAttributes(typeof(DynamicValuesAttribute), inherit: true);
		if (array != null && 0 < array.GetLength(0))
		{
			typeConverter = new DynamicValueTypeConverter();
		}
		if (property.PropertyType.IsEnum())
		{
			typeConverter = new LocalizableEnumConverter(property.PropertyType, resourceManager);
		}
	}

	public override bool CanResetValue(object component)
	{
		return false;
	}

	public override bool ShouldSerializeValue(object component)
	{
		return false;
	}

	public override void ResetValue(object component)
	{
	}

	public override object GetValue(object component)
	{
		if (component == null)
		{
			throw new ArgumentNullException("component");
		}
		if (!ComponentType.IsInstanceOfType(component))
		{
			throw new ArgumentException("Unexpected argument type", "component");
		}
		return property.GetValue(component, null);
	}

	public override void SetValue(object component, object value)
	{
		if (component == null)
		{
			throw new ArgumentNullException("component");
		}
		if (!ComponentType.IsInstanceOfType(component))
		{
			throw new ArgumentException("Unexpected argument type", "component");
		}
		if (property.PropertyType.IsEnum() && value is string && typeConverter is LocalizableEnumConverter)
		{
			object enumValue = ((LocalizableEnumConverter)typeConverter).GetEnumValue((string)value);
			if (enumValue != null)
			{
				property.SetValue(component, enumValue, null);
				return;
			}
		}
		property.SetValue(component, value, null);
	}

	public void ForceReadOnly()
	{
		readonlyOverride = true;
	}

	private static string GetCategoryAttribute(PropertyInfo property)
	{
		string result = null;
		object[] customAttributes = property.GetCustomAttributes(typeof(CategoryAttribute), inherit: true);
		if (customAttributes != null && 0 < customAttributes.GetLength(0))
		{
			result = ((CategoryAttribute)customAttributes[0]).Category;
		}
		return result;
	}

	private static string GetDescriptionAttribute(PropertyInfo property)
	{
		string result = null;
		object[] customAttributes = property.GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
		if (customAttributes != null && 0 < customAttributes.GetLength(0))
		{
			result = ((DescriptionAttribute)customAttributes[0]).Description;
		}
		return result;
	}
}

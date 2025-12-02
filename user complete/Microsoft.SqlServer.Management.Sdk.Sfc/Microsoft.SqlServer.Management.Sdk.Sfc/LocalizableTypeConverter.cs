using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Resources;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class LocalizableTypeConverter : TypeConverter
{
	private string defaultResourceName = "LocalizableResources";

	private void GetResourceManager(Type valueType, out ResourceManager resourceManager, out bool isDefaultResources)
	{
		resourceManager = null;
		isDefaultResources = false;
		try
		{
			object[] customAttributes = valueType.GetCustomAttributes(typeof(LocalizedPropertyResourcesAttribute), inherit: true);
			string text = null;
			if (customAttributes != null && 0 < customAttributes.GetLength(0))
			{
				text = ((LocalizedPropertyResourcesAttribute)customAttributes[0]).ResourcesName;
				isDefaultResources = ((LocalizedPropertyResourcesAttribute)customAttributes[0]).UseDefaultKeys;
			}
			else
			{
				text = valueType.Namespace + "." + defaultResourceName;
				isDefaultResources = true;
			}
			resourceManager = new ResourceManager(text, valueType.Assembly());
		}
		catch (ArgumentNullException)
		{
		}
	}

	private PropertyDescriptorCollection GetPropertiesFromObject(ITypeDescriptorContext context, object value, Attribute[] filter)
	{
		PropertyDescriptorCollection propertiesFromType = GetPropertiesFromType(value.GetType());
		if (value is IDynamicProperties dynamicProperties)
		{
			dynamicProperties.AddProperties(propertiesFromType, context, value, filter);
		}
		if (value is IDynamicReadOnly dynamicReadOnly)
		{
			List<LocalizablePropertyDescriptor> list = new List<LocalizablePropertyDescriptor>(propertiesFromType.Count);
			foreach (PropertyDescriptor item in propertiesFromType)
			{
				list.Add((LocalizablePropertyDescriptor)item);
			}
			dynamicReadOnly.OverrideReadOnly(list, context, value, filter);
		}
		return propertiesFromType;
	}

	private PropertyDescriptorCollection GetPropertiesFromType(Type valueType)
	{
		TraceHelper.Assert(valueType != null, "unexpected null value");
		if (valueType == null)
		{
			throw new ArgumentNullException("value");
		}
		PropertyDescriptorCollection result = null;
		GetResourceManager(valueType, out var resourceManager, out var isDefaultResources);
		if (resourceManager != null)
		{
			PropertyInfo[] properties = valueType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			result = GetPropertyDescriptorsFromPropertyInfo(properties, resourceManager, isDefaultResources);
		}
		return result;
	}

	private PropertyDescriptorCollection GetPropertyDescriptorsFromPropertyInfo(PropertyInfo[] properties, ResourceManager resourceManager, bool isDefaultResources)
	{
		PropertyDescriptorCollection propertyDescriptorCollection = null;
		if (resourceManager != null)
		{
			ArrayList arrayList = new ArrayList();
			foreach (PropertyInfo property in properties)
			{
				LocalizablePropertyDescriptor localizablePropertyDescriptor = new LocalizablePropertyDescriptor(property, resourceManager, isDefaultResources);
				if (localizablePropertyDescriptor.IsBrowsable)
				{
					arrayList.Add(localizablePropertyDescriptor);
				}
			}
			int count = arrayList.Count;
			if (0 < count)
			{
				propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				arrayList.Sort(new LocalizablePropertyComparer());
				for (int j = 0; j < count; j++)
				{
					propertyDescriptorCollection.Insert(j, (PropertyDescriptor)arrayList[j]);
				}
			}
		}
		return propertyDescriptorCollection;
	}

	public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] filter)
	{
		if (value is Type)
		{
			return GetPropertiesFromType((Type)value);
		}
		return GetPropertiesFromObject(context, value, filter);
	}

	public PropertyDescriptorCollection GetProperties(PropertyInfo[] properties)
	{
		if (properties == null)
		{
			throw new ArgumentNullException("properties");
		}
		PropertyDescriptorCollection result = null;
		if (properties.Length != 0)
		{
			Type declaringType = properties[0].DeclaringType;
			GetResourceManager(declaringType, out var resourceManager, out var isDefaultResources);
			if (resourceManager != null)
			{
				result = GetPropertyDescriptorsFromPropertyInfo(properties, resourceManager, isDefaultResources);
			}
		}
		return result;
	}

	public override bool GetPropertiesSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public LocalizableMemberDescriptor GetTypeMemberDescriptor(Type type)
	{
		TraceHelper.Assert(type != null, "unexpected null value");
		if (type == null)
		{
			throw new ArgumentNullException("value");
		}
		GetResourceManager(type, out var resourceManager, out var isDefaultResources);
		return new LocalizableMemberDescriptor(type, resourceManager, isDefaultResources);
	}
}

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Microsoft.SqlServer.Management.Common;

public class CommonLocalizableEnumConverter : EnumConverter
{
	private SortedList localizedEnumFields = new SortedList(StringComparer.CurrentCulture);

	private void LoadLocalizedNames(Type type, ResourceManager manager)
	{
		if (manager != null)
		{
			LoadLocalizedFieldNames(type, manager);
		}
		else
		{
			LoadUnlocalizedFieldNames(type);
		}
	}

	private void LoadLocalizedFieldNames(Type type, ResourceManager manager)
	{
		string[] names = Enum.GetNames(type);
		foreach (string text in names)
		{
			FieldInfo field = type.GetField(text);
			object[] customAttributes = field.GetCustomAttributes(typeof(CommonDisplayNameKeyAttribute), inherit: true);
			CommonDisplayNameKeyAttribute[] array = customAttributes.OfType<CommonDisplayNameKeyAttribute>().ToArray();
			if (array.Length > 0)
			{
				string text2 = manager.GetString(array[0].Key);
				if (text2 == null)
				{
					text2 = text;
				}
				localizedEnumFields[text2] = Enum.Parse(type, text);
			}
			else
			{
				localizedEnumFields[text] = Enum.Parse(type, text);
			}
		}
	}

	private void LoadUnlocalizedFieldNames(Type type)
	{
		string[] names = Enum.GetNames(type);
		foreach (string text in names)
		{
			localizedEnumFields[text] = Enum.Parse(type, text);
		}
	}

	internal CommonLocalizableEnumConverter(Type type, ResourceManager manager)
		: base(type.GetType())
	{
		LoadLocalizedNames(type, manager);
	}

	public CommonLocalizableEnumConverter(Type type)
		: base(type.GetType())
	{
		ResourceManager manager = null;
		object[] customAttributes = type.GetCustomAttributes(typeof(CommonLocalizedPropertyResourcesAttribute), inherit: true);
		if (customAttributes != null && 0 < customAttributes.GetLength(0))
		{
			string resourcesName = ((CommonLocalizedPropertyResourcesAttribute)customAttributes[0]).ResourcesName;
			manager = new ResourceManager(resourcesName, type.Assembly());
		}
		LoadLocalizedNames(type, manager);
	}

	private string GetEnumDescription(Enum value)
	{
		string result = string.Empty;
		IDictionaryEnumerator enumerator = localizedEnumFields.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Value.Equals(value))
			{
				result = (string)enumerator.Key;
				break;
			}
		}
		return result;
	}

	internal object GetEnumValue(string description)
	{
		return localizedEnumFields[description];
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (value is Enum && destinationType == typeof(string))
		{
			return GetEnumDescription((Enum)value);
		}
		if (value is string && destinationType == typeof(string))
		{
			return value;
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		if (value is string)
		{
			return GetEnumValue((string)value);
		}
		if (value is Enum)
		{
			return value;
		}
		return base.ConvertFrom(context, culture, value);
	}

	public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
	{
		StandardValuesCollection standardValuesCollection = null;
		if (context.Instance is ICommonDynamicVisible)
		{
			ICollection collection = ((ICommonDynamicVisible)context.Instance).ConfigureVisibleEnumFields(context, new ArrayList(localizedEnumFields.Values));
			return new StandardValuesCollection(collection);
		}
		return new StandardValuesCollection(localizedEnumFields.Keys);
	}

	public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
	{
		return true;
	}

	public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
	{
		return true;
	}
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class EnumToDisplayNameConverter : EnumConverter
{
	private Type type;

	protected EnumToDisplayNameConverter(Type type)
		: base(type)
	{
		this.type = type;
	}

	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}
		return base.CanConvertFrom(context, sourceType);
	}

	private object LocalizedStringToEnumValue(string value, Type enumType, CultureInfo culture)
	{
		CultureInfo culture2 = CultureInfo.CurrentCulture;
		if (culture != null)
		{
			culture2 = culture;
		}
		MemberInfo[] members = enumType.GetMembers();
		foreach (MemberInfo memberInfo in members)
		{
			if (string.Compare(memberInfo.Name, value, ignoreCase: true, culture2) == 0)
			{
				return Enum.Parse(enumType, memberInfo.Name, ignoreCase: true);
			}
			object[] customAttributes = SqlEnumNetCoreExtension.GetCustomAttributes(memberInfo, typeof(LocDisplayNameAttribute), inherit: true);
			object[] array = customAttributes;
			for (int j = 0; j < array.Length; j++)
			{
				LocDisplayNameAttribute locDisplayNameAttribute = (LocDisplayNameAttribute)array[j];
				if (string.Compare(locDisplayNameAttribute.DisplayName, value, ignoreCase: true, culture2) == 0)
				{
					return Enum.Parse(enumType, memberInfo.Name, ignoreCase: true);
				}
			}
		}
		return Enum.Parse(enumType, value, ignoreCase: true);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
	{
		string value2 = value as string;
		if (!string.IsNullOrEmpty(value2))
		{
			return LocalizedStringToEnumValue(value2, type, culture);
		}
		return base.ConvertFrom(context, culture, value);
	}

	private string EnumValueToLocString(string enumMemberName, Type enumType)
	{
		MemberInfo[] members = enumType.GetMembers();
		foreach (MemberInfo memberInfo in members)
		{
			if (memberInfo.Name == enumMemberName)
			{
				object[] customAttributes = SqlEnumNetCoreExtension.GetCustomAttributes(memberInfo, typeof(LocDisplayNameAttribute), inherit: true);
				object[] array = customAttributes;
				int num = 0;
				if (num < array.Length)
				{
					LocDisplayNameAttribute locDisplayNameAttribute = (LocDisplayNameAttribute)array[num];
					return locDisplayNameAttribute.DisplayName;
				}
				break;
			}
		}
		return enumMemberName;
	}

	private string EnumValueToTsqlSyntax(string enumMemberName, Type enumType)
	{
		MemberInfo[] members = enumType.GetMembers();
		foreach (MemberInfo memberInfo in members)
		{
			if (memberInfo.Name == enumMemberName)
			{
				object[] customAttributes = SqlEnumNetCoreExtension.GetCustomAttributes(memberInfo, typeof(TsqlSyntaxStringAttribute), inherit: true);
				object[] array = customAttributes;
				int num = 0;
				if (num < array.Length)
				{
					TsqlSyntaxStringAttribute tsqlSyntaxStringAttribute = (TsqlSyntaxStringAttribute)array[num];
					return tsqlSyntaxStringAttribute.DisplayName;
				}
				break;
			}
		}
		return null;
	}

	public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			if (culture == CultureInfo.InvariantCulture)
			{
				string text = EnumValueToTsqlSyntax(value.ToString(), type);
				if (text != null)
				{
					return text;
				}
			}
			return EnumValueToLocString(value.ToString(), type);
		}
		return base.ConvertTo(context, culture, value, destinationType);
	}
}

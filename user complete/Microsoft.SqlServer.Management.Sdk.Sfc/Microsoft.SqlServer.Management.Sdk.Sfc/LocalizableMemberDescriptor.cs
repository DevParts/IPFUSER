using System;
using System.ComponentModel;
using System.Resources;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class LocalizableMemberDescriptor : MemberDescriptor
{
	private Type type;

	private string displayName;

	private string displayCategory;

	private string displayDescription;

	public override string Category => displayCategory;

	public override string Description => displayDescription;

	public override bool DesignTimeOnly
	{
		get
		{
			bool result = false;
			object[] customAttributes = type.GetCustomAttributes(typeof(DesignOnlyAttribute), inherit: true);
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
			object[] customAttributes = type.GetCustomAttributes(typeof(BrowsableAttribute), inherit: true);
			if (customAttributes != null && 0 < customAttributes.GetLength(0))
			{
				result = ((BrowsableAttribute)customAttributes[0]).Browsable;
			}
			return result;
		}
	}

	public override string Name => type.Name;

	public LocalizableMemberDescriptor(Type type, ResourceManager resourceManager, bool isDefaultResourceManager)
		: base(type.Name, null)
	{
		TraceHelper.Assert(type != null, "unexpected null type object");
		TraceHelper.Assert(resourceManager != null, "resourceManager is null, is the resource string name correct?");
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		this.type = type;
		displayCategory = DisplayKeyHelper.GetValueFromCustomAttribute(type, typeof(DisplayCategoryKeyAttribute), resourceManager, isDefaultResourceManager);
		if (displayCategory == null)
		{
			displayCategory = DisplayKeyHelper.ConvertNullToEmptyString(GetCategoryAttribute(type));
		}
		displayName = DisplayKeyHelper.GetValueFromCustomAttribute(type, typeof(DisplayNameKeyAttribute), resourceManager, isDefaultResourceManager);
		if (displayName == null || displayName.Length == 0)
		{
			displayName = type.Name;
		}
		displayDescription = DisplayKeyHelper.GetValueFromCustomAttribute(type, typeof(DisplayDescriptionKeyAttribute), resourceManager, isDefaultResourceManager);
		if (displayDescription == null)
		{
			displayDescription = DisplayKeyHelper.ConvertNullToEmptyString(GetDescriptionAttribute(type));
		}
	}

	private static string GetCategoryAttribute(Type type)
	{
		string result = null;
		object[] customAttributes = type.GetCustomAttributes(typeof(CategoryAttribute), inherit: true);
		if (customAttributes != null && 0 < customAttributes.GetLength(0))
		{
			result = ((CategoryAttribute)customAttributes[0]).Category;
		}
		return result;
	}

	private static string GetDescriptionAttribute(Type type)
	{
		string result = null;
		object[] customAttributes = type.GetCustomAttributes(typeof(DescriptionAttribute), inherit: true);
		if (customAttributes != null && 0 < customAttributes.GetLength(0))
		{
			result = ((DescriptionAttribute)customAttributes[0]).Description;
		}
		return result;
	}
}

using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Common;

[AttributeUsage(AttributeTargets.Property)]
public class CommonDisplayCategoryKeyAttribute : Attribute, ICommonDisplayKey
{
	private string key;

	private static string postfix = "Cat";

	private static string delim = "_";

	public string Key => key;

	public string GetDefaultKey(PropertyInfo property)
	{
		return CommonDisplayKeyHelper.ConstructDefaultKey(postfix, delim, property);
	}

	public string GetDefaultKey(Type type)
	{
		return CommonDisplayKeyHelper.ConstructDefaultKey(postfix, delim, type);
	}

	public string GetDefaultKey(FieldInfo field)
	{
		return CommonDisplayKeyHelper.ConstructDefaultKey(postfix, delim, field);
	}

	public CommonDisplayCategoryKeyAttribute(string key)
	{
		this.key = key;
	}
}

using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Common;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface)]
public class CommonDisplayDescriptionKeyAttribute : Attribute, ICommonDisplayKey
{
	private string key;

	private static string postfix = "Desc";

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

	public CommonDisplayDescriptionKeyAttribute(string key)
	{
		this.key = key;
	}
}

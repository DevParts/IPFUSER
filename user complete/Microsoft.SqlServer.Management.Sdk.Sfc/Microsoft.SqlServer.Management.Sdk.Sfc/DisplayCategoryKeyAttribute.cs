using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayCategoryKeyAttribute : Attribute, IDisplayKey
{
	private string key;

	private static string postfix = "Cat";

	private static string delim = "_";

	public string Key => key;

	public string GetDefaultKey(PropertyInfo property)
	{
		return DisplayKeyHelper.ConstructDefaultKey(postfix, delim, property);
	}

	public string GetDefaultKey(Type type)
	{
		return DisplayKeyHelper.ConstructDefaultKey(postfix, delim, type);
	}

	public string GetDefaultKey(FieldInfo field)
	{
		return DisplayKeyHelper.ConstructDefaultKey(postfix, delim, field);
	}

	public DisplayCategoryKeyAttribute(string key)
	{
		TraceHelper.Assert(key != null, "unexpected null key parameter");
		TraceHelper.Assert(0 < key.Length, "unexpected empty key");
		this.key = key;
	}
}

using System;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface)]
public class DisplayNameKeyAttribute : Attribute, IDisplayKey
{
	private string key;

	private static string postfix = "Name";

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

	public DisplayNameKeyAttribute(string key)
	{
		TraceHelper.Assert(key != null, "unexpected null key parameter");
		TraceHelper.Assert(0 < key.Length, "unexpected empty key");
		this.key = key;
	}
}

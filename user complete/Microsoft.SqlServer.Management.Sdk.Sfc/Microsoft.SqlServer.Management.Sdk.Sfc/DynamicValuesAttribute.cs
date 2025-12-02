using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[AttributeUsage(AttributeTargets.Property)]
public class DynamicValuesAttribute : Attribute
{
	private bool dynamicValuesEnabled;

	public bool Enabled => dynamicValuesEnabled;

	public DynamicValuesAttribute(bool enabled)
	{
		dynamicValuesEnabled = enabled;
	}
}

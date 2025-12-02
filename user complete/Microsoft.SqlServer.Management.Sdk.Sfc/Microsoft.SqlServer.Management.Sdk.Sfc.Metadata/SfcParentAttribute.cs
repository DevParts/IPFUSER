using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
public class SfcParentAttribute : Attribute
{
	private string m_parentName = string.Empty;

	public string Parent => m_parentName;

	public SfcParentAttribute()
	{
	}

	public SfcParentAttribute(string parentName)
	{
		m_parentName = parentName;
	}
}

using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcKeyAttribute : Attribute
{
	private int m_position;

	public int Position => m_position;

	public SfcKeyAttribute()
	{
	}

	public SfcKeyAttribute(int position)
	{
		m_position = position;
	}
}

using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class SfcBrowsableAttribute : Attribute
{
	private bool m_isBrowsable = true;

	public bool IsBrowsable
	{
		get
		{
			return m_isBrowsable;
		}
		set
		{
			m_isBrowsable = value;
		}
	}

	public SfcBrowsableAttribute(bool isBrowsable)
	{
		m_isBrowsable = isBrowsable;
	}
}

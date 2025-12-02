using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
internal class SfcElementAttribute : Attribute
{
	private SfcElementFlags m_flags;

	public bool Standalone
	{
		get
		{
			return (m_flags & SfcElementFlags.Standalone) == SfcElementFlags.Standalone;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcElementFlags.Standalone) : (m_flags & ~SfcElementFlags.Standalone));
		}
	}

	public bool SqlAzureDatabase
	{
		get
		{
			return (m_flags & SfcElementFlags.SqlAzureDatabase) == SfcElementFlags.SqlAzureDatabase;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcElementFlags.SqlAzureDatabase) : (m_flags & ~SfcElementFlags.SqlAzureDatabase));
		}
	}

	public SfcElementAttribute(SfcElementFlags flags)
	{
		m_flags = flags;
	}
}

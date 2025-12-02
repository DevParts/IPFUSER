using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SfcPropertyAttribute : Attribute
{
	private SfcPropertyFlags m_flags;

	private string m_defaultValue;

	public SfcPropertyFlags Flags
	{
		get
		{
			return m_flags;
		}
		set
		{
			m_flags = value;
		}
	}

	public bool Computed
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Computed) == SfcPropertyFlags.Computed;
		}
		set
		{
			if (value)
			{
				Required = false;
			}
			m_flags = (value ? (m_flags | SfcPropertyFlags.Computed) : (m_flags & ~SfcPropertyFlags.Computed));
		}
	}

	public bool Data
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Data) == SfcPropertyFlags.Data;
		}
		set
		{
			if (value)
			{
				Required = false;
			}
			m_flags = (value ? (m_flags | SfcPropertyFlags.Data) : (m_flags & ~SfcPropertyFlags.Data));
		}
	}

	public bool Encrypted
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Encrypted) == SfcPropertyFlags.Encrypted;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.Encrypted) : (m_flags & ~SfcPropertyFlags.Encrypted));
		}
	}

	public bool Expensive
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Expensive) == SfcPropertyFlags.Expensive;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.Expensive) : (m_flags & ~SfcPropertyFlags.Expensive));
		}
	}

	public bool Standalone
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Standalone) == SfcPropertyFlags.Standalone;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.Standalone) : (m_flags & ~SfcPropertyFlags.Standalone));
		}
	}

	public bool SqlAzureDatabase
	{
		get
		{
			return (m_flags & SfcPropertyFlags.SqlAzureDatabase) == SfcPropertyFlags.SqlAzureDatabase;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.SqlAzureDatabase) : (m_flags & ~SfcPropertyFlags.SqlAzureDatabase));
		}
	}

	public bool ReadOnlyAfterCreation
	{
		get
		{
			return (m_flags & SfcPropertyFlags.ReadOnlyAfterCreation) == SfcPropertyFlags.ReadOnlyAfterCreation;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.ReadOnlyAfterCreation) : (m_flags & ~SfcPropertyFlags.ReadOnlyAfterCreation));
		}
	}

	public bool Required
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Required) == SfcPropertyFlags.Required;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.Required) : (m_flags & ~SfcPropertyFlags.Required));
		}
	}

	public bool Design
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Design) == SfcPropertyFlags.Design;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.Design) : (m_flags & ~SfcPropertyFlags.Design));
		}
	}

	public bool Deploy
	{
		get
		{
			return (m_flags & SfcPropertyFlags.Deploy) == SfcPropertyFlags.Deploy;
		}
		set
		{
			m_flags = (value ? (m_flags | SfcPropertyFlags.Deploy) : (m_flags & ~SfcPropertyFlags.Deploy));
		}
	}

	public string DefaultValue => m_defaultValue;

	public SfcPropertyAttribute()
		: this(SfcPropertyFlags.Required, string.Empty)
	{
	}

	public SfcPropertyAttribute(SfcPropertyFlags flags)
		: this(flags, string.Empty)
	{
	}

	public SfcPropertyAttribute(SfcPropertyFlags flags, string defaultValue)
	{
		m_flags = flags;
		m_defaultValue = defaultValue;
	}
}

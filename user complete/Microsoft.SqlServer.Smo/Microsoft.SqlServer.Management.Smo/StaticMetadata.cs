using System;
using System.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

[DebuggerDisplay("{Name} : {PropertyType}")]
internal struct StaticMetadata
{
	private string m_name;

	private Type m_propertyType;

	private bool m_expensive;

	private bool m_readonly;

	internal static readonly StaticMetadata Empty = new StaticMetadata(null, expensive: false, readOnly: false, null);

	internal string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}

	internal bool Expensive
	{
		get
		{
			return m_expensive;
		}
		set
		{
			m_expensive = value;
		}
	}

	internal Type PropertyType
	{
		get
		{
			return m_propertyType;
		}
		set
		{
			m_propertyType = value;
		}
	}

	internal bool ReadOnly
	{
		get
		{
			return m_readonly;
		}
		set
		{
			m_readonly = value;
		}
	}

	internal bool IsEnumeration => m_propertyType.GetIsEnum();

	public Predicate<StaticMetadata> Match => CompareNameOnly;

	internal StaticMetadata(string name, bool expensive, bool readOnly, Type propertyType)
	{
		m_name = name;
		m_expensive = expensive;
		m_readonly = readOnly;
		m_propertyType = propertyType;
	}

	internal StaticMetadata(string name)
		: this(name, expensive: false, readOnly: false, typeof(object))
	{
	}

	private bool CompareNameOnly(StaticMetadata p)
	{
		return m_name == p.m_name;
	}
}

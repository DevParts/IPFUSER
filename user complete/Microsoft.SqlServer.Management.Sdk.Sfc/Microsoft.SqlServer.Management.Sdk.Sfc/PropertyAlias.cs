using System;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Serializable]
[ComVisible(false)]
public class PropertyAlias
{
	public enum AliasKind
	{
		Each,
		Prefix,
		NodeName
	}

	private AliasKind m_Kind;

	private string m_Prefix;

	private string[] m_Aliases;

	public AliasKind Kind
	{
		get
		{
			return m_Kind;
		}
		set
		{
			m_Kind = value;
		}
	}

	public string Prefix
	{
		get
		{
			return m_Prefix;
		}
		set
		{
			m_Prefix = value;
		}
	}

	public string[] Aliases
	{
		get
		{
			return m_Aliases;
		}
		set
		{
			m_Aliases = value;
		}
	}

	public PropertyAlias()
	{
		m_Kind = AliasKind.NodeName;
	}

	public PropertyAlias(string prefix)
	{
		m_Kind = AliasKind.Prefix;
		m_Prefix = prefix;
	}

	public PropertyAlias(string[] aliases)
	{
		m_Kind = AliasKind.Each;
		m_Aliases = aliases;
	}
}

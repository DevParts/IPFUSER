using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class Dependency
{
	private Urn m_urn;

	private bool schemaBound;

	private DependencyChainCollection m_links;

	public Urn Urn
	{
		get
		{
			return m_urn;
		}
		set
		{
			m_urn = value;
		}
	}

	public bool IsSchemaBound
	{
		get
		{
			return schemaBound;
		}
		set
		{
			schemaBound = value;
		}
	}

	public DependencyChainCollection Links => m_links;

	public Dependency()
	{
		m_links = new DependencyChainCollection();
	}

	public Dependency(Dependency dep)
	{
		m_urn = new Urn(dep.Urn);
		schemaBound = dep.IsSchemaBound;
		m_links = new DependencyChainCollection(dep.Links);
	}

	public Dependency Copy()
	{
		return new Dependency(this);
	}
}

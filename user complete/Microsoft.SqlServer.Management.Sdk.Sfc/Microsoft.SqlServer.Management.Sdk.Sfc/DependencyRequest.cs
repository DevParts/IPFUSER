using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class DependencyRequest
{
	private Urn[] m_listUrn;

	private bool m_bParentDeps;

	public Urn[] Urns
	{
		get
		{
			return m_listUrn;
		}
		set
		{
			m_listUrn = value;
		}
	}

	public bool ParentDependencies
	{
		get
		{
			return m_bParentDeps;
		}
		set
		{
			m_bParentDeps = value;
		}
	}

	public DependencyRequest()
	{
		m_bParentDeps = true;
	}
}

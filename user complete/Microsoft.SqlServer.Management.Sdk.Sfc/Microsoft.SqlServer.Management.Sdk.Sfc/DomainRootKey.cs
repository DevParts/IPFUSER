namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class DomainRootKey : SfcKey
{
	private ISfcDomain m_Domain;

	public ISfcDomain Domain
	{
		get
		{
			return m_Domain;
		}
		set
		{
			m_Domain = value;
		}
	}

	protected DomainRootKey(ISfcDomain domain)
	{
		m_Domain = domain;
	}
}

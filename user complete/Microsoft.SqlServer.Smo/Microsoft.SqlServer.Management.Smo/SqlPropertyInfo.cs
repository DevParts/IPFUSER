using System;

namespace Microsoft.SqlServer.Management.Smo;

public class SqlPropertyInfo
{
	private StaticMetadata m_sm;

	private SqlServerVersions m_versions;

	public string Name => m_sm.Name;

	public bool IsWriteable => m_sm.ReadOnly;

	public bool IsExpensive => m_sm.Expensive;

	public Type PropertyType => m_sm.PropertyType;

	public SqlServerVersions Versions => m_versions;

	internal SqlPropertyInfo(StaticMetadata sm, SqlServerVersions versions)
	{
		m_sm = sm;
		m_versions = versions;
	}
}

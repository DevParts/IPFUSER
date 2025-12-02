using System.Data.SqlClient;

namespace Microsoft.SqlServer.Management.Common;

public class SqlDirectConnection : ConnectionInfoBase
{
	private string m_sbServerName;

	private SqlConnection m_sqlConnection;

	public string ServerName
	{
		get
		{
			if (m_sbServerName == null)
			{
				return string.Empty;
			}
			return m_sbServerName;
		}
		set
		{
			if (m_sbServerName == null || m_sbServerName.ToString().StringCompare(value, ignoreCase: true, ConnectionInfoBase.DefaultCulture) != 0)
			{
				m_sbServerName = value;
				base.ServerVersion = null;
				ConnectionParmsChanged();
			}
		}
	}

	public SqlConnection SqlConnection
	{
		get
		{
			return m_sqlConnection;
		}
		set
		{
			m_sqlConnection = value;
			ConnectionParmsChanged();
		}
	}

	public SqlDirectConnection()
		: base(ConnectionType.SqlConnection)
	{
	}

	public SqlDirectConnection(SqlConnection sqlConnection)
		: base(ConnectionType.SqlConnection)
	{
		m_sqlConnection = sqlConnection;
	}

	private SqlDirectConnection(SqlDirectConnection conn)
		: base(ConnectionType.SqlConnection)
	{
		m_sqlConnection = conn.SqlConnection;
		m_sbServerName = conn.ServerName;
	}

	public SqlDirectConnection Copy()
	{
		return new SqlDirectConnection(this);
	}

	protected override void ConnectionParmsChanged()
	{
	}
}

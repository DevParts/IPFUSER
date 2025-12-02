using System;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public class SqlCeConnectionInfo : SqlOlapConnectionInfoBase
{
	private int m_MaxDatabaseSize = -1;

	private int m_DefaultLockEscalation = -1;

	private IDbConnection connection;

	[Browsable(false)]
	public override string ConnectionString
	{
		get
		{
			if (base.RebuildConnectionStringInternal)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Data Source = \"{0}\"; Password=\"{1}\";", EscapeString(base.ServerName), EscapeString(base.Password));
				if (-1 != base.ConnectionTimeout)
				{
					stringBuilder.AppendFormat("Timeout = \"{0}\";", base.ConnectionTimeout);
				}
				if (-1 != m_MaxDatabaseSize)
				{
					stringBuilder.AppendFormat("Max Database Size = \"{0}\";", m_MaxDatabaseSize);
				}
				if (-1 != m_DefaultLockEscalation)
				{
					stringBuilder.AppendFormat("Default Lock Escalation = \"{0}\";", m_DefaultLockEscalation);
				}
				base.ConnectionStringInternal = EncryptionUtility.EncryptString(stringBuilder.ToString());
				stringBuilder = null;
				base.RebuildConnectionStringInternal = false;
			}
			return EncryptionUtility.DecryptSecureString(base.ConnectionStringInternal);
		}
	}

	public IDbConnection Connection
	{
		get
		{
			return connection;
		}
		set
		{
			connection = value;
		}
	}

	public int MaxDatabaseSize
	{
		get
		{
			return m_MaxDatabaseSize;
		}
		set
		{
			if (value != m_MaxDatabaseSize)
			{
				m_MaxDatabaseSize = value;
				ConnectionParmsChanged();
			}
		}
	}

	public int DefaultLockEscalation
	{
		get
		{
			return m_DefaultLockEscalation;
		}
		set
		{
			if (value != m_DefaultLockEscalation)
			{
				m_DefaultLockEscalation = value;
				ConnectionParmsChanged();
			}
		}
	}

	public SqlCeConnectionInfo()
		: base(ConnectionType.SqlCE)
	{
	}

	public SqlCeConnectionInfo(IDbConnection connection)
		: this(connection.Database, string.Empty)
	{
		this.connection = connection;
	}

	public SqlCeConnectionInfo(string connStr)
		: base(ConnectionType.SqlCE)
	{
		base.ConnectionStringInternal = EncryptionUtility.EncryptString(connStr);
	}

	public SqlCeConnectionInfo(string database, string password)
		: base(ConnectionType.SqlCE)
	{
		_ = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Data Source = \"");
		stringBuilder.Append(EscapeString(database));
		stringBuilder.Append("\"; Password=\"");
		stringBuilder.Append(EscapeString(password));
		stringBuilder.Append("\";");
		base.ServerName = database;
		base.Password = password;
		base.ServerVersion = new ServerVersion(9, 0);
		base.ConnectionStringInternal = EncryptionUtility.EncryptString(stringBuilder.ToString());
		stringBuilder = null;
	}

	public SqlCeConnectionInfo(SqlCeConnectionInfo conn)
		: base(conn)
	{
	}

	private string EscapeString(string s)
	{
		s = s.Replace("\"", "\"\"");
		return s;
	}

	public SqlCeConnectionInfo Copy()
	{
		SqlCeConnectionInfo sqlCeConnectionInfo = new SqlCeConnectionInfo(this);
		sqlCeConnectionInfo.MaxDatabaseSize = MaxDatabaseSize;
		sqlCeConnectionInfo.DefaultLockEscalation = DefaultLockEscalation;
		return sqlCeConnectionInfo;
	}

	public override IDbConnection CreateConnectionObject()
	{
		throw new NotImplementedException();
	}
}

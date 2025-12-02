using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
[ComVisible(false)]
public abstract class SqlOlapConnectionInfoBase : ConnectionInfoBase
{
	protected const int NoTimeOut = -1;

	private StringBuilder m_sbServerName;

	[NonSerialized]
	private SecureString m_sbConnectionString;

	private bool m_fRebuildConnectionString = true;

	private StringBuilder m_sbUserName;

	[NonSerialized]
	[XmlIgnore]
	private SecureString m_password;

	private bool m_fIntegratedSecurity = true;

	private StringBuilder m_sbDatabaseName;

	public static readonly int DefaultConnTimeout = -1;

	public static readonly int DefaultQueryTimeout = -1;

	private int m_iConnectionTimeout = DefaultConnTimeout;

	private int m_iQueryTimeout = DefaultQueryTimeout;

	protected StringBuilder ServerNameInternal
	{
		get
		{
			return m_sbServerName;
		}
		set
		{
			m_sbServerName = value;
		}
	}

	protected SecureString ConnectionStringInternal
	{
		get
		{
			return m_sbConnectionString;
		}
		set
		{
			if (value != null)
			{
				m_sbConnectionString = value.Copy();
			}
			else
			{
				m_sbConnectionString = null;
			}
		}
	}

	protected bool RebuildConnectionStringInternal
	{
		get
		{
			if (!m_fRebuildConnectionString)
			{
				return m_sbConnectionString == null;
			}
			return true;
		}
		set
		{
			m_fRebuildConnectionString = value;
		}
	}

	internal StringBuilder UserNameInternal
	{
		get
		{
			return m_sbUserName;
		}
		set
		{
			m_sbUserName = value;
		}
	}

	internal SecureString PasswordInternal
	{
		get
		{
			return (m_password == null) ? new SecureString() : m_password.Copy();
		}
		set
		{
			if (value != null)
			{
				m_password = value.Copy();
			}
			else
			{
				m_password = null;
			}
		}
	}

	protected bool IntegratedSecurityInternal
	{
		get
		{
			return m_fIntegratedSecurity;
		}
		set
		{
			m_fIntegratedSecurity = value;
		}
	}

	protected StringBuilder DatabaseNameInternal
	{
		get
		{
			return m_sbDatabaseName;
		}
		set
		{
			m_sbDatabaseName = value;
		}
	}

	protected int ConnectionTimeoutInternal
	{
		get
		{
			return m_iConnectionTimeout;
		}
		set
		{
			m_iConnectionTimeout = value;
		}
	}

	protected int QueryTimeoutInternal
	{
		get
		{
			return m_iQueryTimeout;
		}
		set
		{
			m_iQueryTimeout = value;
		}
	}

	public string ServerName
	{
		get
		{
			if (m_sbServerName == null)
			{
				return string.Empty;
			}
			return m_sbServerName.ToString();
		}
		set
		{
			if (m_sbServerName == null || m_sbServerName.ToString().StringCompare(value, ignoreCase: true, ConnectionInfoBase.DefaultCulture) != 0)
			{
				m_sbServerName = new StringBuilder(value);
				base.ServerVersion = null;
				ConnectionParmsChanged();
			}
		}
	}

	[XmlIgnore]
	public string UserName
	{
		get
		{
			if (m_sbUserName == null)
			{
				return string.Empty;
			}
			return m_sbUserName.ToString();
		}
		set
		{
			bool flag = false;
			if (m_fIntegratedSecurity)
			{
				m_fIntegratedSecurity = false;
				flag = true;
			}
			if (m_sbUserName == null || m_sbUserName.ToString().StringCompare(value, ignoreCase: false, ConnectionInfoBase.DefaultCulture) != 0)
			{
				m_sbUserName = new StringBuilder(value);
				flag = true;
			}
			if (flag)
			{
				ConnectionParmsChanged();
			}
		}
	}

	[Browsable(false)]
	[XmlIgnore]
	public string Password
	{
		get
		{
			return (m_password == null) ? string.Empty : EncryptionUtility.DecryptSecureString(m_password);
		}
		set
		{
			bool flag = false;
			if (value == null || value.Length == 0)
			{
				if (m_password != null)
				{
					m_password = null;
					flag = true;
				}
				return;
			}
			if (m_fIntegratedSecurity)
			{
				m_fIntegratedSecurity = false;
				flag = true;
			}
			string text = ((m_password == null) ? string.Empty : EncryptionUtility.DecryptSecureString(m_password));
			if (text != value)
			{
				m_password = EncryptionUtility.EncryptString(value);
				flag = true;
			}
			text = null;
			if (flag)
			{
				ConnectionParmsChanged();
			}
		}
	}

	[XmlIgnore]
	[Browsable(false)]
	public SecureString SecurePassword
	{
		get
		{
			if (m_password != null)
			{
				return m_password.Copy();
			}
			return new SecureString();
		}
		set
		{
			bool flag = false;
			if (value == null)
			{
				if (m_password != null)
				{
					m_password = null;
					flag = true;
				}
				return;
			}
			if (m_fIntegratedSecurity)
			{
				m_fIntegratedSecurity = false;
				flag = true;
			}
			string text = ((m_password == null) ? string.Empty : EncryptionUtility.DecryptSecureString(m_password));
			string text2 = EncryptionUtility.DecryptSecureString(value);
			bool flag2 = text != text2;
			text = null;
			text2 = null;
			if (flag2)
			{
				m_password = value.Copy();
				flag = true;
			}
			if (flag)
			{
				ConnectionParmsChanged();
			}
		}
	}

	public bool UseIntegratedSecurity
	{
		get
		{
			return m_fIntegratedSecurity;
		}
		set
		{
			if (m_fIntegratedSecurity != value)
			{
				m_fIntegratedSecurity = value;
				ConnectionParmsChanged();
			}
		}
	}

	public string DatabaseName
	{
		get
		{
			if (m_sbDatabaseName == null)
			{
				return string.Empty;
			}
			return m_sbDatabaseName.ToString();
		}
		set
		{
			if (m_sbDatabaseName == null || m_sbDatabaseName.ToString().StringCompare(value, ignoreCase: false, ConnectionInfoBase.DefaultCulture) != 0)
			{
				m_sbDatabaseName = new StringBuilder(value);
				ConnectionParmsChanged();
			}
		}
	}

	public int ConnectionTimeout
	{
		get
		{
			return m_iConnectionTimeout;
		}
		set
		{
			if (value != m_iConnectionTimeout)
			{
				m_iConnectionTimeout = value;
				ConnectionParmsChanged();
			}
		}
	}

	public int QueryTimeout
	{
		get
		{
			return m_iQueryTimeout;
		}
		set
		{
			m_iQueryTimeout = value;
		}
	}

	[Browsable(false)]
	public abstract string ConnectionString { get; }

	protected SqlOlapConnectionInfoBase()
	{
		throw new InvalidOperationException(StringConnectionInfo.ClassDefaulConstructorCannotBeUsed("SqlOlapConnectionInfoBase"));
	}

	protected SqlOlapConnectionInfoBase(ConnectionType serverType)
		: base(serverType)
	{
	}

	protected SqlOlapConnectionInfoBase(string serverName, ConnectionType serverType)
		: base(serverType)
	{
		ServerName = serverName;
	}

	public SqlOlapConnectionInfoBase(string serverName, string userName, string password, ConnectionType serverType)
		: base(serverType)
	{
		ServerName = serverName;
		UseIntegratedSecurity = false;
		UserName = userName;
		Password = password;
	}

	protected SqlOlapConnectionInfoBase(SqlOlapConnectionInfoBase conn)
		: base(conn)
	{
		m_sbServerName = conn.m_sbServerName;
		m_sbConnectionString = conn.m_sbConnectionString;
		m_fRebuildConnectionString = conn.m_fRebuildConnectionString;
		m_sbUserName = conn.m_sbUserName;
		m_password = ((conn.m_password != null) ? conn.m_password.Copy() : null);
		m_fIntegratedSecurity = conn.m_fIntegratedSecurity;
		m_sbDatabaseName = conn.m_sbDatabaseName;
		m_iConnectionTimeout = conn.m_iConnectionTimeout;
		m_iQueryTimeout = conn.m_iQueryTimeout;
	}

	public abstract IDbConnection CreateConnectionObject();

	protected override void ConnectionParmsChanged()
	{
		m_fRebuildConnectionString = true;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(base.ToString());
		stringBuilder.AppendFormat(", server name = {0}", ServerName);
		return stringBuilder.ToString();
	}
}

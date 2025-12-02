using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
[ComVisible(false)]
public class SqlConnectionInfo : SqlOlapConnectionInfoBase
{
	public enum AuthenticationMethod
	{
		NotSpecified = 0,
		SqlPassword = 1,
		ActiveDirectoryPassword = 2,
		ActiveDirectoryIntegrated = 3,
		ActiveDirectoryInteractive = 5
	}

	public static readonly NetworkProtocol DefaultNetworkProtocol = NetworkProtocol.NotSpecified;

	private static bool? s_IsAuthenticationSupported;

	private static object lockThis = new object();

	private StringBuilder m_sbApplicationName;

	private StringBuilder m_sbWorkstationID;

	private NetworkProtocol m_eNetworkProtocol = DefaultNetworkProtocol;

	private int m_PoolConnectionLifeTime = -1;

	private int m_MaxPoolSize = -1;

	private int m_MinPoolSize = -1;

	private int m_PacketSize = -1;

	private bool shouldEncryptConnection;

	private string additionalParameters;

	private bool trustServerCertificate;

	private AuthenticationMethod m_Authentication;

	private string m_ApplicationIntent;

	[NonSerialized]
	private SqlBoolean m_Pooled = SqlBoolean.Null;

	public string ApplicationName
	{
		get
		{
			if (m_sbApplicationName == null)
			{
				return string.Empty;
			}
			return m_sbApplicationName.ToString();
		}
		set
		{
			if (m_sbApplicationName == null || m_sbApplicationName.ToString().StringCompare(value, ignoreCase: false, ConnectionInfoBase.DefaultCulture) != 0)
			{
				m_sbApplicationName = new StringBuilder(value);
				ConnectionParmsChanged();
			}
		}
	}

	public string WorkstationId
	{
		get
		{
			if (m_sbWorkstationID == null)
			{
				return string.Empty;
			}
			return m_sbWorkstationID.ToString();
		}
		set
		{
			if (m_sbWorkstationID == null || m_sbWorkstationID.ToString().StringCompare(value, ignoreCase: false, ConnectionInfoBase.DefaultCulture) != 0)
			{
				m_sbWorkstationID = new StringBuilder(value);
				ConnectionParmsChanged();
			}
		}
	}

	public NetworkProtocol ConnectionProtocol
	{
		get
		{
			return m_eNetworkProtocol;
		}
		set
		{
			if (value != m_eNetworkProtocol)
			{
				m_eNetworkProtocol = value;
				ConnectionParmsChanged();
			}
		}
	}

	public AuthenticationMethod Authentication
	{
		get
		{
			return m_Authentication;
		}
		set
		{
			if (value != m_Authentication)
			{
				m_Authentication = value;
				if (base.UseIntegratedSecurity && value == AuthenticationMethod.ActiveDirectoryIntegrated)
				{
					base.UseIntegratedSecurity = false;
				}
				ConnectionParmsChanged();
			}
		}
	}

	public string ApplicationIntent
	{
		get
		{
			return m_ApplicationIntent;
		}
		set
		{
			if (!(value == m_ApplicationIntent))
			{
				m_ApplicationIntent = value;
				ConnectionParmsChanged();
			}
		}
	}

	public bool TrustServerCertificate
	{
		get
		{
			return trustServerCertificate;
		}
		set
		{
			if (value != trustServerCertificate)
			{
				trustServerCertificate = value;
				ConnectionParmsChanged();
			}
		}
	}

	public IRenewableToken AccessToken { get; set; }

	private string NetworkProtocolString
	{
		get
		{
			string result = string.Empty;
			switch (m_eNetworkProtocol)
			{
			case NetworkProtocol.TcpIp:
				result = "dbmssocn";
				break;
			case NetworkProtocol.NamedPipes:
				result = "dbnmpntw";
				break;
			case NetworkProtocol.Multiprotocol:
				result = "dbmsrpcn";
				break;
			case NetworkProtocol.AppleTalk:
				result = "dbmsadsn";
				break;
			case NetworkProtocol.BanyanVines:
				result = "dbmsvinn";
				break;
			case NetworkProtocol.Via:
				result = "dbmsgnet";
				break;
			case NetworkProtocol.SharedMemory:
				result = "dbmslpcn";
				break;
			case NetworkProtocol.NWLinkIpxSpx:
				result = "dbmsspxn";
				break;
			}
			return result;
		}
	}

	[Browsable(false)]
	public override string ConnectionString
	{
		get
		{
			if (base.RebuildConnectionStringInternal)
			{
				ConnectionSettings connectionSettings = new ConnectionSettings(this);
				base.ConnectionStringInternal = EncryptionUtility.EncryptString(connectionSettings.ConnectionString);
				base.RebuildConnectionStringInternal = false;
			}
			return EncryptionUtility.DecryptSecureString(base.ConnectionStringInternal);
		}
	}

	public int PoolConnectionLifeTime
	{
		get
		{
			return m_PoolConnectionLifeTime;
		}
		set
		{
			m_PoolConnectionLifeTime = value;
		}
	}

	public int PacketSize
	{
		get
		{
			return m_PacketSize;
		}
		set
		{
			m_PacketSize = value;
		}
	}

	public int MaxPoolSize
	{
		get
		{
			return m_MaxPoolSize;
		}
		set
		{
			m_MaxPoolSize = value;
		}
	}

	public int MinPoolSize
	{
		get
		{
			return m_MinPoolSize;
		}
		set
		{
			m_MinPoolSize = value;
		}
	}

	public SqlBoolean Pooled
	{
		get
		{
			return m_Pooled;
		}
		set
		{
			m_Pooled = value;
		}
	}

	public bool EncryptConnection
	{
		get
		{
			return shouldEncryptConnection;
		}
		set
		{
			shouldEncryptConnection = value;
		}
	}

	public string AdditionalParameters
	{
		get
		{
			return additionalParameters;
		}
		set
		{
			additionalParameters = value;
			base.RebuildConnectionStringInternal = true;
		}
	}

	public static bool IsAuthenticationKeywordSupported()
	{
		if (s_IsAuthenticationSupported.HasValue)
		{
			return s_IsAuthenticationSupported.Value;
		}
		lock (lockThis)
		{
			if (s_IsAuthenticationSupported.HasValue)
			{
				return s_IsAuthenticationSupported.Value;
			}
			s_IsAuthenticationSupported = false;
			Assembly assembly = typeof(SqlConnection).GetAssembly();
			if (assembly != null)
			{
				Type type = assembly.GetType("System.Data.SqlClient.SqlAuthenticationMethod");
				s_IsAuthenticationSupported = type != null;
			}
		}
		return s_IsAuthenticationSupported.Value;
	}

	public static AuthenticationMethod GetAuthenticationMethod(SqlConnectionStringBuilder connectionStringBuilder)
	{
		if (!IsAuthenticationKeywordSupported())
		{
			return AuthenticationMethod.NotSpecified;
		}
		Assembly assembly = typeof(SqlConnectionStringBuilder).GetAssembly();
		Type typeFromHandle = typeof(SqlConnectionStringBuilder);
		Type type = assembly.GetType("System.Data.SqlClient.SqlAuthenticationMethod");
		PropertyInfo property = typeFromHandle.GetProperty("Authentication");
		if (type == null || property == null)
		{
			throw new NotSupportedException(StringConnectionInfo.MethodNotSupported("System.Data.SqlClient.SqlAuthenticationMethod"));
		}
		object value = property.GetValue(connectionStringBuilder, null);
		if (value == null)
		{
			return AuthenticationMethod.NotSpecified;
		}
		string text = value.ToString();
		if (text == AuthenticationMethod.ActiveDirectoryIntegrated.ToString())
		{
			return AuthenticationMethod.ActiveDirectoryIntegrated;
		}
		if (text == AuthenticationMethod.ActiveDirectoryPassword.ToString())
		{
			return AuthenticationMethod.ActiveDirectoryPassword;
		}
		if (text == AuthenticationMethod.SqlPassword.ToString())
		{
			return AuthenticationMethod.SqlPassword;
		}
		if (text == AuthenticationMethod.ActiveDirectoryInteractive.ToString())
		{
			return AuthenticationMethod.ActiveDirectoryInteractive;
		}
		if (text == AuthenticationMethod.NotSpecified.ToString())
		{
			return AuthenticationMethod.NotSpecified;
		}
		return AuthenticationMethod.NotSpecified;
	}

	public SqlConnectionInfo()
		: base(ConnectionType.Sql)
	{
	}

	public SqlConnectionInfo(string serverName)
		: base(serverName, ConnectionType.Sql)
	{
	}

	public SqlConnectionInfo(string serverName, string userName, string password)
		: base(serverName, userName, password, ConnectionType.Sql)
	{
	}

	public SqlConnectionInfo(SqlConnectionInfo conn)
		: base(conn)
	{
		m_sbApplicationName = conn.m_sbApplicationName;
		m_sbWorkstationID = conn.m_sbWorkstationID;
		m_eNetworkProtocol = conn.m_eNetworkProtocol;
		m_PacketSize = conn.m_PacketSize;
		shouldEncryptConnection = conn.shouldEncryptConnection;
		additionalParameters = conn.additionalParameters;
		m_Authentication = conn.Authentication;
		m_ApplicationIntent = conn.ApplicationIntent;
		trustServerCertificate = conn.TrustServerCertificate;
		AccessToken = conn.AccessToken;
	}

	public SqlConnectionInfo(ServerConnection serverConnection, ConnectionType connectionType)
		: base(connectionType)
	{
		if (serverConnection.IsApplicationNameInitialized)
		{
			m_sbApplicationName = new StringBuilder(serverConnection.ApplicationName);
		}
		if (serverConnection.IsWorkstationIdInitialized)
		{
			m_sbWorkstationID = new StringBuilder(serverConnection.WorkstationId);
		}
		m_eNetworkProtocol = serverConnection.NetworkProtocol;
		m_PoolConnectionLifeTime = serverConnection.PooledConnectionLifetime;
		m_MaxPoolSize = serverConnection.MaxPoolSize;
		m_MinPoolSize = serverConnection.MinPoolSize;
		m_Pooled = !serverConnection.NonPooledConnection;
		base.ServerNameInternal = new StringBuilder(serverConnection.ServerInstance);
		if (serverConnection.IsLoginInitialized)
		{
			base.UserNameInternal = new StringBuilder(serverConnection.Login);
		}
		if (serverConnection.IsPasswordInitialized)
		{
			base.PasswordInternal = EncryptionUtility.EncryptString(serverConnection.Password);
		}
		base.IntegratedSecurityInternal = serverConnection.LoginSecure;
		if (serverConnection.IsDatabaseNameInitialized)
		{
			base.DatabaseNameInternal = new StringBuilder(serverConnection.DatabaseName);
		}
		base.ConnectionTimeoutInternal = serverConnection.ConnectTimeout;
		EncryptConnection = serverConnection.EncryptConnection;
		additionalParameters = serverConnection.AdditionalParameters;
		AccessToken = serverConnection.AccessToken;
	}

	public SqlConnectionInfo Copy()
	{
		return new SqlConnectionInfo(this);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(base.ToString());
		stringBuilder.AppendFormat(", timeout = {0}, database = {1}, protocol = {2}, workstation = {3}, integrated security = {4}", base.ConnectionTimeout, base.DatabaseName, ConnectionProtocol, WorkstationId, base.UseIntegratedSecurity);
		return stringBuilder.ToString();
	}

	public override IDbConnection CreateConnectionObject()
	{
		SqlConnection sqlConnection = new SqlConnection(ConnectionString);
		if (AccessToken != null)
		{
			ConnectionInfoHelper.SetTokenOnConnection(sqlConnection, AccessToken.GetAccessToken());
		}
		return sqlConnection;
	}

	public static bool IsApplicationIntentKeywordSupported()
	{
		return true;
	}
}

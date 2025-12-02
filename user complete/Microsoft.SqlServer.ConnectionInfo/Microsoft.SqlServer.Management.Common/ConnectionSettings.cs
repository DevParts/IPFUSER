using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Reflection;
using System.Security;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Common;

public class ConnectionSettings
{
	public const int NoConnectionTimeout = 0;

	private const int ConnectionTimeout_Default = 15;

	private const int PooledConnectionLifetime_Default = 0;

	private const int MinPoolSize_Default = 0;

	private const int MaxPoolSize_Default = 100;

	private const NetworkProtocol NetworkProtocol_Default = NetworkProtocol.NotSpecified;

	private const int PacketSize_Default = 8192;

	private const bool NonPooledConnection_Default = false;

	private const bool MultipleActiveResultSets_Default = false;

	private bool m_BlockUpdates;

	private bool m_ResetConnectionString;

	private string m_ServerInstance;

	private string m_Login;

	private SecureString m_Password;

	private bool m_LoginSecure;

	private string m_ConnectAsUserName;

	private SecureString m_ConnectAsUserPassword;

	private bool m_ConnectAsUser;

	private bool m_NonPooledConnection;

	private bool m_TrustServerCertificate;

	private int m_PooledConnectionLifetime;

	private int m_MinPoolSize;

	private int m_MaxPoolSize;

	private int m_ConnectTimeout;

	private NetworkProtocol m_NetworkProtocol;

	private string m_ApplicationName;

	private string m_WorkstationId;

	private string m_DatabaseName;

	private int m_PacketSize;

	private SecureString m_ConnectionString;

	private bool m_MultipleActiveResultSets;

	private bool shouldEncryptConnection;

	private string additionalParameters;

	private SqlConnectionInfo.AuthenticationMethod m_Authentication;

	private string m_ApplicationIntent;

	public IRenewableToken AccessToken { get; set; }

	public string ServerInstance
	{
		get
		{
			return m_ServerInstance;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("ServerInstance");
			ThrowIfInvalidValue(value, "ServerInstance", checkEmpty: false);
			m_ServerInstance = value;
		}
	}

	internal bool IsLoginInitialized => IsValidString(m_Login);

	public string Login
	{
		get
		{
			return m_Login;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("Login");
			ThrowIfLoginSecure("Login");
			ThrowIfInvalidValue(value, "Login");
			m_Login = value;
		}
	}

	internal bool IsPasswordInitialized => IsValidString(Password, checkEmpty: false);

	[Browsable(false)]
	public string Password
	{
		get
		{
			string result = string.Empty;
			if (m_Password != null)
			{
				result = EncryptionUtility.DecryptSecureString(m_Password);
			}
			return result;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("Password");
			ThrowIfLoginSecure("Password");
			ThrowIfInvalidValue(value, "Password", checkEmpty: false);
			if (value != null && value.Length != 0)
			{
				m_Password = EncryptionUtility.EncryptString(value);
			}
			else
			{
				m_Password = null;
			}
		}
	}

	[Browsable(false)]
	public SecureString SecurePassword
	{
		get
		{
			SecureString secureString = null;
			if (m_Password == null)
			{
				return new SecureString();
			}
			return m_Password.Copy();
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("Password");
			ThrowIfLoginSecure("Password");
			if (value != null)
			{
				ThrowIfInvalidValue(EncryptionUtility.DecryptSecureString(value), "Password", checkEmpty: false);
				m_Password = value.Copy();
			}
			else
			{
				ThrowIfInvalidValue(null, "Password", checkEmpty: false);
				m_Password = null;
			}
		}
	}

	public bool LoginSecure
	{
		get
		{
			return m_LoginSecure;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("LoginSecure");
			m_LoginSecure = value;
		}
	}

	public string ConnectAsUserName
	{
		get
		{
			return m_ConnectAsUserName;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("ConnectAsUserName");
			ThrowIfInvalidValue(value, "ConnectAsUserName");
			m_ConnectAsUserName = value;
		}
	}

	public string ConnectAsUserPassword
	{
		get
		{
			string result = string.Empty;
			if (m_ConnectAsUserPassword != null)
			{
				result = EncryptionUtility.DecryptSecureString(m_ConnectAsUserPassword);
			}
			return result;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("ConnectAsUserPassword");
			ThrowIfInvalidValue(value, "ConnectAsUserPassword");
			if (!string.IsNullOrEmpty(value))
			{
				m_ConnectAsUserPassword = EncryptionUtility.EncryptString(value);
			}
			else
			{
				m_ConnectAsUserPassword = null;
			}
		}
	}

	public bool ConnectAsUser
	{
		get
		{
			return m_ConnectAsUser;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("ConnectAsUser");
			m_ConnectAsUser = value;
		}
	}

	public bool NonPooledConnection
	{
		get
		{
			return m_NonPooledConnection;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("NonPooledConnection");
			m_NonPooledConnection = value;
		}
	}

	public int PooledConnectionLifetime
	{
		get
		{
			return m_PooledConnectionLifetime;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("PooledConnectionLifetime");
			ThrowIfInvalidValue(value, 0, "PooledConnectionLifetime");
			m_PooledConnectionLifetime = value;
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
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("MinPoolSize");
			ThrowIfInvalidValue(value, 0, "MinPoolSize");
			m_MinPoolSize = value;
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
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("MaxPoolSize");
			ThrowIfInvalidValue(value, 2, "MaxPoolSize");
			m_MaxPoolSize = value;
		}
	}

	public int ConnectTimeout
	{
		get
		{
			return m_ConnectTimeout;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("ConnectTimeout");
			ThrowIfInvalidValue(value, 0, "ConnectTimeout");
			m_ConnectTimeout = value;
		}
	}

	public SqlConnectionInfo.AuthenticationMethod Authentication
	{
		get
		{
			return m_Authentication;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			m_Authentication = value;
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
			ThrowIfUpdatesAreBlocked();
			m_ApplicationIntent = value;
		}
	}

	public bool TrustServerCertificate
	{
		get
		{
			return m_TrustServerCertificate;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			m_TrustServerCertificate = value;
		}
	}

	[Browsable(false)]
	public string ConnectionString
	{
		get
		{
			if (m_ConnectionString == null)
			{
				return GetConnectionString();
			}
			return EncryptionUtility.DecryptSecureString(m_ConnectionString);
		}
		set
		{
			ThrowIfInvalidValue(value, "ConnectionString", checkEmpty: false);
			if (!string.IsNullOrEmpty(value))
			{
				m_ConnectionString = EncryptionUtility.EncryptString(value);
			}
			else
			{
				m_ConnectionString = null;
			}
			m_ResetConnectionString = true;
		}
	}

	[Browsable(false)]
	public SecureString SecureConnectionString
	{
		get
		{
			if (m_ConnectionString == null)
			{
				return EncryptionUtility.EncryptString(GetConnectionString());
			}
			return m_ConnectionString;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			if (value != null && value.Length != 0)
			{
				m_ConnectionString = value;
			}
			else
			{
				m_ConnectionString = null;
			}
			m_ResetConnectionString = true;
		}
	}

	public NetworkProtocol NetworkProtocol
	{
		get
		{
			return m_NetworkProtocol;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("NetworkProtocol");
			m_NetworkProtocol = value;
		}
	}

	internal bool IsApplicationNameInitialized => IsValidString(m_ApplicationName);

	public string ApplicationName
	{
		get
		{
			return m_ApplicationName;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("ApplicationName");
			ThrowIfInvalidValue(value, "ApplicationName");
			m_ApplicationName = value;
		}
	}

	internal bool IsWorkstationIdInitialized => IsValidString(m_WorkstationId);

	public string WorkstationId
	{
		get
		{
			return m_WorkstationId;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("WorkstationId");
			ThrowIfInvalidValue(value, "WorkstationId", checkEmpty: false);
			m_WorkstationId = value;
		}
	}

	internal bool IsDatabaseNameInitialized => IsValidString(m_DatabaseName);

	public string DatabaseName
	{
		get
		{
			return m_DatabaseName;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("DatabaseName");
			ThrowIfInvalidValue(value, "DatabaseName", checkEmpty: false);
			m_DatabaseName = value;
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
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("PacketSize");
			ThrowIfInvalidValue(value, 0, "PacketSize");
			m_PacketSize = value;
		}
	}

	public bool MultipleActiveResultSets
	{
		get
		{
			return m_MultipleActiveResultSets;
		}
		set
		{
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("MultipleActiveResultSets");
			m_MultipleActiveResultSets = value;
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
			ThrowIfUpdatesAreBlocked();
			ThrowIfConnectionStringIsSet("EncryptConnection");
			shouldEncryptConnection = value;
		}
	}

	internal string AdditionalParameters => additionalParameters;

	internal bool IsReadAccessBlocked
	{
		get
		{
			if (m_ConnectionString != null)
			{
				return IsValidString(ConnectionString);
			}
			return false;
		}
	}

	internal string InitialCatalog
	{
		get
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(ConnectionString))
			{
				SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
				result = sqlConnectionStringBuilder.InitialCatalog;
			}
			return result;
		}
	}

	internal bool BlockUpdates
	{
		get
		{
			return m_BlockUpdates;
		}
		set
		{
			m_BlockUpdates = value;
		}
	}

	protected bool ResetConnectionString
	{
		get
		{
			return m_ResetConnectionString;
		}
		set
		{
			m_ResetConnectionString = value;
		}
	}

	internal ConnectionSettings()
	{
		InitDefaults();
	}

	internal ConnectionSettings(SqlConnectionInfo sci)
	{
		InitDefaults();
		InitFromSqlConnectionInfo(sci);
	}

	private void InitDefaults()
	{
		m_BlockUpdates = false;
		m_ResetConnectionString = true;
		m_ServerInstance = "(local)";
		m_Login = string.Empty;
		m_Password = null;
		m_LoginSecure = true;
		m_ConnectAsUserName = string.Empty;
		m_ConnectAsUserPassword = null;
		m_ConnectAsUser = false;
		m_NonPooledConnection = false;
		m_PooledConnectionLifetime = 0;
		m_MinPoolSize = 0;
		m_MaxPoolSize = 100;
		m_ConnectTimeout = 15;
		m_NetworkProtocol = NetworkProtocol.NotSpecified;
		m_ApplicationName = string.Empty;
		m_WorkstationId = string.Empty;
		m_DatabaseName = string.Empty;
		m_PacketSize = 8192;
		m_MultipleActiveResultSets = false;
		m_TrustServerCertificate = false;
		m_Authentication = SqlConnectionInfo.AuthenticationMethod.NotSpecified;
		m_ApplicationIntent = null;
	}

	internal void CopyConnectionSettings(ConnectionSettings cs)
	{
		cs.m_ServerInstance = m_ServerInstance;
		cs.m_Login = m_Login;
		cs.m_Password = ((m_Password != null) ? m_Password.Copy() : null);
		cs.m_LoginSecure = m_LoginSecure;
		cs.m_ConnectAsUserName = m_ConnectAsUserName;
		cs.m_ConnectAsUserPassword = ((m_ConnectAsUserPassword != null) ? m_ConnectAsUserPassword.Copy() : null);
		cs.m_ConnectAsUser = m_ConnectAsUser;
		cs.m_NonPooledConnection = m_NonPooledConnection;
		cs.m_PooledConnectionLifetime = m_PooledConnectionLifetime;
		cs.m_MinPoolSize = m_MinPoolSize;
		cs.m_MaxPoolSize = m_MaxPoolSize;
		cs.m_ConnectTimeout = m_ConnectTimeout;
		cs.m_NetworkProtocol = m_NetworkProtocol;
		cs.m_ApplicationName = m_ApplicationName;
		cs.m_WorkstationId = m_WorkstationId;
		cs.m_DatabaseName = m_DatabaseName;
		cs.m_PacketSize = m_PacketSize;
		cs.m_MultipleActiveResultSets = m_MultipleActiveResultSets;
		cs.shouldEncryptConnection = shouldEncryptConnection;
		cs.additionalParameters = additionalParameters;
		cs.m_TrustServerCertificate = m_TrustServerCertificate;
		cs.m_Authentication = m_Authentication;
		cs.m_ApplicationIntent = m_ApplicationIntent;
		cs.AccessToken = AccessToken;
		cs.m_ConnectionString = m_ConnectionString;
	}

	internal void InitFromSqlConnectionInfo(SqlConnectionInfo sci)
	{
		if (IsValidString(sci.ApplicationName))
		{
			m_ApplicationName = sci.ApplicationName;
		}
		if (IsValidString(sci.WorkstationId))
		{
			m_WorkstationId = sci.WorkstationId;
		}
		m_NetworkProtocol = sci.ConnectionProtocol;
		if (sci.PoolConnectionLifeTime >= 0)
		{
			m_PooledConnectionLifetime = sci.PoolConnectionLifeTime;
		}
		if (sci.MaxPoolSize > 0)
		{
			m_MaxPoolSize = sci.MaxPoolSize;
		}
		if (sci.MinPoolSize >= 0)
		{
			m_MinPoolSize = sci.MinPoolSize;
		}
		if (false == sci.Pooled)
		{
			m_NonPooledConnection = true;
		}
		if (IsValidString(sci.ServerName, checkEmpty: false))
		{
			m_ServerInstance = sci.ServerName;
		}
		LoginSecure = sci.UseIntegratedSecurity;
		if (!m_LoginSecure)
		{
			if (IsValidString(sci.UserName))
			{
				m_Login = sci.UserName;
			}
			if (IsValidString(sci.Password, checkEmpty: false))
			{
				Password = sci.Password;
			}
		}
		if (IsValidString(sci.DatabaseName, checkEmpty: false))
		{
			m_DatabaseName = sci.DatabaseName;
		}
		if (sci.ConnectionTimeout >= 0)
		{
			m_ConnectTimeout = sci.ConnectionTimeout;
		}
		if (sci.PacketSize >= 0)
		{
			m_PacketSize = sci.PacketSize;
		}
		if (sci.EncryptConnection)
		{
			shouldEncryptConnection = true;
		}
		m_TrustServerCertificate = sci.TrustServerCertificate;
		m_Authentication = sci.Authentication;
		m_ApplicationIntent = sci.ApplicationIntent;
		AccessToken = sci.AccessToken;
		additionalParameters = sci.AdditionalParameters;
	}

	internal void InitFromSqlConnection(SqlConnection sc)
	{
		NonPooledConnection = true;
		if (!SqlContext.IsAvailable)
		{
			ServerInstance = sc.DataSource;
			PacketSize = sc.PacketSize;
			ConnectTimeout = sc.ConnectionTimeout;
			WorkstationId = sc.WorkstationId ?? string.Empty;
		}
		DatabaseName = sc.Database;
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(sc.ConnectionString);
		if (sc.Credential != null)
		{
			sqlConnectionStringBuilder.UserID = sc.Credential.UserId;
			sqlConnectionStringBuilder.Password = EncryptionUtility.DecryptSecureString(sc.Credential.Password);
		}
		m_LoginSecure = sqlConnectionStringBuilder.IntegratedSecurity;
		m_Login = sqlConnectionStringBuilder.UserID;
		m_Password = EncryptionUtility.EncryptString(sqlConnectionStringBuilder.Password);
		ConnectionString = sqlConnectionStringBuilder.ConnectionString;
	}

	internal void ForceSetPassword(string value)
	{
		if (value != null && value.Length != 0)
		{
			m_Password = EncryptionUtility.EncryptString(value);
		}
		else
		{
			m_Password = null;
		}
	}

	private void ThrowIfConnectionStringIsSet(string propertyName)
	{
		if (m_ConnectionString != null && IsValidString(ConnectionString))
		{
			throw new PropertyNotAvailableException(StringConnectionInfo.PropertyNotAvailable(propertyName));
		}
		m_ResetConnectionString = true;
	}

	private void ThrowIfUpdatesAreBlocked()
	{
		if (BlockUpdates)
		{
			throw new ConnectionCannotBeChangedException(StringConnectionInfo.ConnectionCannotBeChanged);
		}
	}

	private void ThrowIfLoginSecure(string propertyName)
	{
		if (LoginSecure)
		{
			throw new InvalidPropertyValueException(StringConnectionInfo.CannotSetWhenLoginSecure(propertyName));
		}
	}

	private void ThrowIfInvalidValue(string str, string propertyName)
	{
		ThrowIfInvalidValue(str, propertyName, checkEmpty: true);
	}

	private void ThrowIfInvalidValue(string str, string propertyName, bool checkEmpty)
	{
		if (!IsValidString(str, checkEmpty))
		{
			throw new InvalidPropertyValueException(StringConnectionInfo.InvalidPropertyValue("null", propertyName, StringConnectionInfo.InvalidPropertyValueReasonString));
		}
	}

	protected string ThrowIfPropertyNotSet(string propertyName, string str)
	{
		return ThrowIfPropertyNotSet(propertyName, str, checkEmpty: true);
	}

	protected string ThrowIfPropertyNotSet(string propertyName, string str, bool checkEmpty)
	{
		if (!IsValidString(str, checkEmpty))
		{
			throw new PropertyNotSetException(StringConnectionInfo.PropertyNotSetException(propertyName));
		}
		return str;
	}

	private void ThrowIfInvalidValue(int n, int value, string propertyName)
	{
		if (n < value)
		{
			throw new InvalidPropertyValueException(StringConnectionInfo.InvalidPropertyValue(n.ToString(ConnectionInfoBase.DefaultCulture), propertyName, StringConnectionInfo.InvalidPropertyValueReasonInt(value.ToString(ConnectionInfoBase.DefaultCulture))));
		}
	}

	private bool IsValidString(string str)
	{
		return IsValidString(str, checkEmpty: true);
	}

	private bool IsValidString(string str, bool checkEmpty)
	{
		if (str == null || (checkEmpty && str.Length <= 0))
		{
			return false;
		}
		return true;
	}

	private string GetNetworkProtocolString()
	{
		string result = string.Empty;
		switch (NetworkProtocol)
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

	private string GetConnectionString()
	{
		SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
		string text;
		if (SqlContext.IsAvailable)
		{
			sqlConnectionStringBuilder.ContextConnection = true;
			text = sqlConnectionStringBuilder.ToString();
		}
		else
		{
			sqlConnectionStringBuilder.DataSource = ServerInstance;
			if (LoginSecure)
			{
				sqlConnectionStringBuilder.IntegratedSecurity = true;
			}
			else if (Authentication == SqlConnectionInfo.AuthenticationMethod.ActiveDirectoryInteractive)
			{
				sqlConnectionStringBuilder.UserID = ThrowIfPropertyNotSet("Login", Login);
			}
			else if (Authentication != SqlConnectionInfo.AuthenticationMethod.ActiveDirectoryIntegrated && AccessToken == null)
			{
				sqlConnectionStringBuilder.UserID = ThrowIfPropertyNotSet("Login", Login);
				sqlConnectionStringBuilder.Password = ThrowIfPropertyNotSet("Password", Password, checkEmpty: false);
			}
			if (ConnectTimeout != 15)
			{
				sqlConnectionStringBuilder.ConnectTimeout = ConnectTimeout;
			}
			if (NetworkProtocol != NetworkProtocol.NotSpecified)
			{
				sqlConnectionStringBuilder.NetworkLibrary = GetNetworkProtocolString();
			}
			if (IsValidString(m_DatabaseName))
			{
				sqlConnectionStringBuilder.InitialCatalog = DatabaseName;
			}
			if (IsValidString(m_WorkstationId))
			{
				sqlConnectionStringBuilder.WorkstationID = WorkstationId;
			}
			if (IsValidString(m_ApplicationName))
			{
				sqlConnectionStringBuilder.ApplicationName = ApplicationName;
			}
			if (PooledConnectionLifetime != 0)
			{
				sqlConnectionStringBuilder.LoadBalanceTimeout = PooledConnectionLifetime;
			}
			if (MaxPoolSize != 100 && MaxPoolSize > 0)
			{
				sqlConnectionStringBuilder.MaxPoolSize = MaxPoolSize;
			}
			if (MinPoolSize != 0)
			{
				sqlConnectionStringBuilder.MinPoolSize = MinPoolSize;
			}
			if (NonPooledConnection)
			{
				sqlConnectionStringBuilder.Pooling = false;
			}
			if (PacketSize != 8192)
			{
				sqlConnectionStringBuilder.PacketSize = PacketSize;
			}
			sqlConnectionStringBuilder.Encrypt = shouldEncryptConnection;
			sqlConnectionStringBuilder.TrustServerCertificate = TrustServerCertificate;
			if (AccessToken == null && SqlConnectionInfo.IsAuthenticationKeywordSupported() && Authentication != SqlConnectionInfo.AuthenticationMethod.NotSpecified)
			{
				SetAuthentication(sqlConnectionStringBuilder);
			}
			if (IsValidString(ApplicationIntent))
			{
				SetApplicationIntent(sqlConnectionStringBuilder);
			}
			sqlConnectionStringBuilder.MultipleActiveResultSets = MultipleActiveResultSets;
			text = sqlConnectionStringBuilder.ToString();
			if (!string.IsNullOrEmpty(additionalParameters))
			{
				text = text + ";" + additionalParameters;
			}
		}
		return text;
	}

	private void SetAuthentication(SqlConnectionStringBuilder sbConnectionString)
	{
		Assembly assembly = typeof(SqlConnectionStringBuilder).GetAssembly();
		Type type = assembly.GetType("System.Data.SqlClient.SqlConnectionStringBuilder");
		Type type2 = assembly.GetType("System.Data.SqlClient.SqlAuthenticationMethod");
		PropertyInfo property = type.GetProperty("Authentication");
		FieldInfo field = type2.GetField(Authentication.ToString());
		if (type2 == null || property == null)
		{
			throw new NotSupportedException(StringConnectionInfo.MethodNotSupported("System.Data.SqlClient.SqlAuthenticationMethod"));
		}
		if (field == null)
		{
			throw new InvalidPropertyValueException(StringConnectionInfo.InvalidPropertyValue("null", "System.Data.SqlClient.SqlAuthenticationMethod", StringConnectionInfo.InvalidPropertyValueReasonString));
		}
		property.SetValue(sbConnectionString, field.GetRawConstantValue(), null);
	}

	private void SetApplicationIntent(SqlConnectionStringBuilder sbConnectionString)
	{
		if (Enum.TryParse<ApplicationIntent>(ApplicationIntent, ignoreCase: true, out var result))
		{
			sbConnectionString.ApplicationIntent = result;
		}
	}

	public override string ToString()
	{
		return ConnectionString;
	}
}

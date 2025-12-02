using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Smo.UnSafeInternals;

namespace Microsoft.SqlServer.Management.Common;

public abstract class ConnectionManager : ConnectionSettings
{
	protected enum ExecuteTSqlAction
	{
		Unknown,
		ExecuteNonQuery,
		ExecuteReader,
		ExecuteScalar,
		FillDataSet
	}

	private SqlConnection m_SqlConnectionObject;

	private object connectionLock = new object();

	private bool m_InUse;

	private int m_LoginFailedClients;

	private ServerMessageEventHandler m_RemoteLoginFailedHandler;

	private SqlInfoMessageEventHandler m_SqlInfoMessageHandler;

	private CapturedSql m_CapturedSQL;

	private AutoDisconnectMode m_AutoDisconnectMode;

	private bool bIsUserConnected;

	private bool isContainedAuthentication;

	private bool m_forceDisconnected;

	private ServerInformation m_serverInformation;

	private ServerVersion m_serverVersionOverride;

	private int lockTimeout = -1;

	private bool containedAuthenticationUpdated;

	private StatementEventHandler statementEventHandler;

	public bool InUse
	{
		get
		{
			return m_InUse;
		}
		set
		{
			m_InUse = value;
		}
	}

	public int LockTimeout
	{
		get
		{
			return lockTimeout;
		}
		set
		{
			if (value < -1)
			{
				throw new InvalidPropertyValueException(StringConnectionInfo.InvalidLockTimeout(value));
			}
			if (lockTimeout == value)
			{
				return;
			}
			lockTimeout = value;
			if (!IsOpen)
			{
				return;
			}
			SqlConnection sqlConnection = null;
			try
			{
				sqlConnection = SqlConnectionObject;
				if (sqlConnection != null)
				{
					Monitor.Enter(SqlConnectionObject);
				}
				SqlCommand sqlCommand = sqlConnection.CreateCommand();
				sqlCommand.CommandText = "SET LOCK_TIMEOUT " + LockTimeout * 1000;
				sqlCommand.CommandType = CommandType.Text;
				ExecuteTSql(ExecuteTSqlAction.ExecuteNonQuery, sqlCommand, null, catchException: true);
			}
			finally
			{
				if (sqlConnection != null)
				{
					Monitor.Exit(sqlConnection);
				}
			}
		}
	}

	public ServerVersion ServerVersion
	{
		get
		{
			return m_serverVersionOverride ?? GetServerInformation().ServerVersion;
		}
		set
		{
			if (!IsForceDisconnected && IsOpen)
			{
				throw new ConnectionException(StringConnectionInfo.CannotBeSetWhileConnected);
			}
			if (m_serverVersionOverride != value)
			{
				m_serverVersionOverride = value;
				m_serverInformation = null;
			}
		}
	}

	public Version ProductVersion => GetServerInformation().ProductVersion;

	public DatabaseEngineType DatabaseEngineType => GetServerInformation().DatabaseEngineType;

	public DatabaseEngineEdition DatabaseEngineEdition => GetServerInformation().DatabaseEngineEdition;

	public string HostPlatform => GetServerInformation().HostPlatform;

	public NetworkProtocol ConnectionProtocol => GetServerInformation().ConnectionProtocol;

	public bool IsContainedAuthentication
	{
		get
		{
			if (!containedAuthenticationUpdated)
			{
				if (IsForceDisconnected)
				{
					return false;
				}
				PoolConnect();
				try
				{
					CheckIfContainedAuthenticationIsUsed();
				}
				finally
				{
					PoolDisconnect();
				}
				containedAuthenticationUpdated = true;
			}
			return isContainedAuthentication;
		}
	}

	public SqlConnection SqlConnectionObject
	{
		get
		{
			if (m_SqlConnectionObject == null)
			{
				m_SqlConnectionObject = new SqlConnection();
			}
			lock (connectionLock)
			{
				if (!IsOpen)
				{
					if (base.AccessToken != null)
					{
						ConnectionInfoHelper.SetTokenOnConnection(m_SqlConnectionObject, base.AccessToken.GetAccessToken());
					}
					if (base.ResetConnectionString || base.ConnectionString != m_SqlConnectionObject.ConnectionString)
					{
						m_SqlConnectionObject.Credential = null;
						m_SqlConnectionObject.ConnectionString = base.ConnectionString;
					}
				}
			}
			return m_SqlConnectionObject;
		}
	}

	public string CurrentDatabase
	{
		get
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(SqlConnectionObject.Database))
			{
				result = SqlConnectionObject.Database;
			}
			return result;
		}
	}

	internal abstract bool BlockPoolDisconnect { get; }

	public bool IsOpen => IsConnectionOpen(m_SqlConnectionObject);

	public CapturedSql CapturedSql => m_CapturedSQL;

	public AutoDisconnectMode AutoDisconnectMode
	{
		get
		{
			return m_AutoDisconnectMode;
		}
		set
		{
			m_AutoDisconnectMode = value;
		}
	}

	public bool IsForceDisconnected => m_forceDisconnected;

	public event StateChangeEventHandler StateChange
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				SqlConnectionObject.StateChange += value;
			}
		}
		remove
		{
			SqlConnectionObject.StateChange -= value;
		}
	}

	public event SqlInfoMessageEventHandler InfoMessage
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				SqlConnectionObject.InfoMessage += value;
			}
		}
		remove
		{
			SqlConnectionObject.InfoMessage -= value;
		}
	}

	private event ServerMessageEventHandler ServerMessageInternal;

	public event ServerMessageEventHandler ServerMessage
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				if (this.ServerMessageInternal == null)
				{
					m_SqlInfoMessageHandler = SerializeInfoMessage;
					InfoMessage += m_SqlInfoMessageHandler;
				}
				ServerMessageInternal += value;
			}
		}
		remove
		{
			ServerMessageInternal -= value;
			if (this.ServerMessageInternal == null)
			{
				InfoMessage -= m_SqlInfoMessageHandler;
				m_SqlInfoMessageHandler = null;
			}
		}
	}

	public event StatementEventHandler StatementExecuted
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				statementEventHandler = (StatementEventHandler)Delegate.Combine(statementEventHandler, value);
			}
		}
		remove
		{
			statementEventHandler = (StatementEventHandler)Delegate.Remove(statementEventHandler, value);
		}
	}

	private event ServerMessageEventHandler RemoteLoginFailedInternal;

	public event ServerMessageEventHandler RemoteLoginFailed
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				if (m_LoginFailedClients++ == 0)
				{
					m_RemoteLoginFailedHandler = OnRemoteLoginFailedMessage;
					ServerMessage += m_RemoteLoginFailedHandler;
				}
				RemoteLoginFailedInternal += value;
			}
		}
		remove
		{
			if (0 <= --m_LoginFailedClients)
			{
				ServerMessage -= m_RemoteLoginFailedHandler;
				m_RemoteLoginFailedHandler = null;
			}
			RemoteLoginFailedInternal -= value;
		}
	}

	internal ConnectionManager()
		: this(null, removeIntegratedSecurity: false)
	{
	}

	internal ConnectionManager(IRenewableToken token)
		: this(token, removeIntegratedSecurity: true)
	{
	}

	private ConnectionManager(IRenewableToken token, bool removeIntegratedSecurity)
	{
		if (!CallerHavePermissionToUseSQLCLR())
		{
			throw new InvalidOperationException(StringConnectionInfo.SmoSQLCLRUnAvailable);
		}
		InitDefaults();
		base.AccessToken = token;
		m_SqlConnectionObject = new SqlConnection();
		InitSqlConnectionObject(setConnectionString: true, removeIntegratedSecurity);
	}

	internal ConnectionManager(SqlConnection sqlConnectionObject, IRenewableToken accessToken)
	{
		if (!CallerHavePermissionToUseSQLCLR())
		{
			throw new Exception(StringConnectionInfo.SmoSQLCLRUnAvailable);
		}
		InitDefaults();
		base.AccessToken = accessToken;
		m_SqlConnectionObject = sqlConnectionObject;
		InitFromSqlConnection(sqlConnectionObject);
		InitSqlConnectionObject(setConnectionString: false);
	}

	internal ConnectionManager(SqlConnectionInfo sci)
		: base(sci)
	{
		if (!CallerHavePermissionToUseSQLCLR())
		{
			throw new Exception(StringConnectionInfo.SmoSQLCLRUnAvailable);
		}
		InitDefaults();
		m_SqlConnectionObject = new SqlConnection();
		InitSqlConnectionObject(setConnectionString: true);
	}

	private void InitSqlConnectionObject(bool setConnectionString, bool removeIntegratedSecurity = false)
	{
		if (setConnectionString)
		{
			lock (connectionLock)
			{
				if (removeIntegratedSecurity)
				{
					SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(base.ConnectionString);
					sqlConnectionStringBuilder.IntegratedSecurity = false;
					base.ConnectionString = sqlConnectionStringBuilder.ConnectionString;
				}
				m_SqlConnectionObject.ConnectionString = base.ConnectionString;
			}
		}
		base.ResetConnectionString = false;
	}

	private bool CallerHavePermissionToUseSQLCLR()
	{
		if (!SqlContext.IsAvailable)
		{
			return true;
		}
		return ManagementUtil.CallerIsMicrosoftAssembly(Assembly.GetExecutingAssembly());
	}

	private void InitDefaults()
	{
		bIsUserConnected = false;
		m_AutoDisconnectMode = AutoDisconnectMode.DisconnectIfPooled;
		m_InUse = false;
		m_LoginFailedClients = 0;
		m_RemoteLoginFailedHandler = null;
		m_CapturedSQL = new CapturedSql();
	}

	internal void CopyConnectionManager(ConnectionManager cm)
	{
		CopyConnectionSettings(cm);
	}

	private ServerInformation GetServerInformation()
	{
		if (m_serverInformation == null)
		{
			if (!IsForceDisconnected)
			{
				PoolConnect();
				SqlConnection sqlConnectionObject = SqlConnectionObject;
				try
				{
					Monitor.Enter(sqlConnectionObject);
					using SqlDataAdapter dataAdapter = new SqlDataAdapter();
					m_serverInformation = ServerInformation.GetServerInformation(sqlConnectionObject, dataAdapter, sqlConnectionObject.ServerVersion);
				}
				finally
				{
					Monitor.Exit(sqlConnectionObject);
					PoolDisconnect();
				}
			}
			else
			{
				m_serverInformation = new ServerInformation(m_serverVersionOverride, new Version(m_serverVersionOverride.Major, m_serverVersionOverride.Minor, m_serverVersionOverride.BuildNumber), DatabaseEngineType.Standalone, DatabaseEngineEdition.Unknown, "Windows", NetworkProtocol.NotSpecified);
			}
		}
		return m_serverInformation;
	}

	private bool IsConnectionOpen(SqlConnection sqlConnection)
	{
		return ConnectionState.Open == (ConnectionState.Open & sqlConnection.State);
	}

	private void CheckIfContainedAuthenticationIsUsed()
	{
		if (!IsOpen || containedAuthenticationUpdated)
		{
			return;
		}
		string initialCatalog = base.InitialCatalog;
		if (DatabaseEngineType.Standalone == DatabaseEngineType && ServerVersion.Major > 10 && !string.IsNullOrEmpty(initialCatalog))
		{
			SqlConnection sqlConnection = null;
			try
			{
				sqlConnection = SqlConnectionObject;
				Monitor.Enter(sqlConnection);
				string text = string.Format(CultureInfo.InvariantCulture, "use {0};", new object[1] { CommonUtils.MakeSqlBraket(initialCatalog) });
				string text2 = string.Empty;
				if (!string.IsNullOrEmpty(SqlConnectionObject.Database))
				{
					text2 = string.Format(CultureInfo.InvariantCulture, "use {0}; --resetting the context", new object[1] { CommonUtils.MakeSqlBraket(SqlConnectionObject.Database) });
				}
				string text3 = "\r\nif (db_id() = 1)\r\nbegin\r\n-- contained auth is 0 when connected to master\r\nselect 0\r\nend\r\nelse\r\nbegin\r\n-- need dynamic sql so that we compile this query only when we know resource db is available\r\nexec('select case when authenticating_database_id = 1 then 0 else 1 end from\r\nsys.dm_exec_sessions where session_id = @@SPID')\r\nend;";
				SqlCommand sqlCommand = sqlConnection.CreateCommand();
				sqlCommand.CommandText = text + text3 + text2;
				sqlCommand.CommandType = CommandType.Text;
				isContainedAuthentication = (int)ExecuteTSql(ExecuteTSqlAction.ExecuteScalar, sqlCommand, null, catchException: true) == 1;
			}
			finally
			{
				Monitor.Exit(sqlConnection);
			}
		}
		else
		{
			isContainedAuthentication = false;
		}
		containedAuthenticationUpdated = true;
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	private static extern bool CloseHandle(IntPtr handle);

	private WindowsIdentity GetWindowsIdentityForConnection()
	{
		if (base.IsReadAccessBlocked || !base.ConnectAsUser || SqlContext.IsAvailable)
		{
			return null;
		}
		IntPtr hToken = IntPtr.Zero;
		try
		{
			int userToken = SafeNativeMethods.GetUserToken(ThrowIfPropertyNotSet("ConnectAsUserName", base.ConnectAsUserName), null, ThrowIfPropertyNotSet("ConnectAsUserPassword", base.ConnectAsUserPassword), out hToken);
			if (userToken != 0)
			{
				throw new ConnectionFailureException(SafeNativeMethods.GetLastErrorMessage(userToken));
			}
			return new WindowsIdentity(hToken);
		}
		finally
		{
			if (hToken != IntPtr.Zero)
			{
				CloseHandle(hToken);
			}
		}
	}

	private void InternalConnect(WindowsIdentity impersonatedIdentity)
	{
		try
		{
			WindowsImpersonationContext windowsImpersonationContext = null;
			if (impersonatedIdentity != null)
			{
				windowsImpersonationContext = impersonatedIdentity.Impersonate();
			}
			try
			{
				SqlConnection sqlConnectionObject = SqlConnectionObject;
				lock (connectionLock)
				{
					if (!IsConnectionOpen(sqlConnectionObject))
					{
						sqlConnectionObject.Open();
					}
				}
				CheckServerVersion(ServerInformation.ParseStringServerVersion(sqlConnectionObject.ServerVersion));
			}
			finally
			{
				windowsImpersonationContext?.Undo();
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void Connect()
	{
		if (IsForceDisconnected)
		{
			return;
		}
		if (IsOpen)
		{
			bIsUserConnected = true;
			return;
		}
		WindowsIdentity windowsIdentityForConnection = GetWindowsIdentityForConnection();
		SqlConnection sqlConnection = null;
		try
		{
			InternalConnect(windowsIdentityForConnection);
			if (LockTimeout != -1)
			{
				sqlConnection = SqlConnectionObject;
				if (sqlConnection != null)
				{
					Monitor.Enter(sqlConnection);
				}
				SqlCommand sqlCommand = SqlConnectionObject.CreateCommand();
				sqlCommand.CommandText = "SET LOCK_TIMEOUT " + LockTimeout * 1000;
				sqlCommand.CommandType = CommandType.Text;
				ExecuteTSql(ExecuteTSqlAction.ExecuteNonQuery, sqlCommand, null, catchException: true);
			}
			base.BlockUpdates = true;
			m_InUse = true;
		}
		catch (Exception innerException)
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(base.ConnectionString);
			throw new ConnectionFailureException(StringConnectionInfo.ConnectionFailure(sqlConnectionStringBuilder.DataSource), innerException);
		}
		finally
		{
			if (sqlConnection != null)
			{
				Monitor.Exit(sqlConnection);
			}
		}
		bIsUserConnected = true;
	}

	protected object ExecuteTSql(ExecuteTSqlAction action, object execObject, DataSet fillDataSet, bool catchException)
	{
		string database = SqlConnectionObject.Database;
		try
		{
			return action switch
			{
				ExecuteTSqlAction.FillDataSet => (execObject as SqlDataAdapter).Fill(fillDataSet), 
				ExecuteTSqlAction.ExecuteNonQuery => (execObject as SqlCommand).ExecuteNonQuery(), 
				ExecuteTSqlAction.ExecuteReader => (execObject as SqlCommand).ExecuteReader(), 
				ExecuteTSqlAction.ExecuteScalar => (execObject as SqlCommand).ExecuteScalar(), 
				_ => null, 
			};
		}
		catch (SqlException ex)
		{
			SqlConnection sqlConnectionObject = SqlConnectionObject;
			bool flag = false;
			lock (connectionLock)
			{
				if (catchException)
				{
					if (base.AccessToken != null && ex.Number == 0 && ex.Class == 11 && sqlConnectionObject.State == ConnectionState.Open)
					{
						sqlConnectionObject.Close();
					}
					if (!IsConnectionOpen(sqlConnectionObject))
					{
						if (base.AccessToken != null)
						{
							ConnectionInfoHelper.SetTokenOnConnection(sqlConnectionObject, base.AccessToken.GetAccessToken());
						}
						sqlConnectionObject.Open();
						if (sqlConnectionObject.Database != database && IsDatabaseValid(sqlConnectionObject, database))
						{
							sqlConnectionObject.ChangeDatabase(database);
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				try
				{
					return ExecuteTSql(action, execObject, fillDataSet, catchException: false);
				}
				catch (SqlException)
				{
				}
			}
			throw ex;
		}
	}

	private bool IsDatabaseValid(SqlConnection sqlConnection, string dbName)
	{
		try
		{
			if (sqlConnection.State != ConnectionState.Open)
			{
				sqlConnection.Open();
			}
			SqlCommand sqlCommand = new SqlCommand();
			sqlCommand.Connection = sqlConnection;
			sqlCommand.Parameters.Add("@db_name", SqlDbType.NVarChar).Value = dbName;
			sqlCommand.CommandText = "SELECT CASE WHEN db_id(@db_name) IS NULL THEN 0 ELSE 1 END";
			return Convert.ToBoolean(sqlCommand.ExecuteScalar());
		}
		catch (SqlException)
		{
			return false;
		}
	}

	protected void CheckServerVersion(ServerVersion version)
	{
		if (version.Major <= 7)
		{
			throw new ConnectionFailureException(StringConnectionInfo.ConnectToInvalidVersion(version.ToString()));
		}
	}

	public void Disconnect()
	{
		if (!IsOpen)
		{
			bIsUserConnected = false;
			return;
		}
		SqlConnection sqlConnectionObject = SqlConnectionObject;
		lock (connectionLock)
		{
			sqlConnectionObject.Close();
		}
		bIsUserConnected = false;
	}

	internal void PoolConnect()
	{
		if (!bIsUserConnected && !IsForceDisconnected)
		{
			Connect();
			bIsUserConnected = false;
		}
	}

	internal void PoolDisconnect()
	{
		if (!bIsUserConnected && AutoDisconnectMode == AutoDisconnectMode.DisconnectIfPooled && !base.IsReadAccessBlocked && !base.NonPooledConnection && !BlockPoolDisconnect)
		{
			Disconnect();
		}
	}

	internal abstract void InitAfterConnect();

	internal void GenerateStatementExecutedEvent(string query)
	{
		if (statementEventHandler != null)
		{
			statementEventHandler(this, new StatementEventArgs(query, DateTime.Now));
		}
	}

	private void OnRemoteLoginFailedMessage(object sender, ServerMessageEventArgs e)
	{
		if (e.Error.Number <= 18480 && e.Error.Number >= 18489)
		{
			this.RemoteLoginFailedInternal(this, e);
		}
	}

	private void SerializeInfoMessage(object sender, SqlInfoMessageEventArgs e)
	{
		foreach (SqlError error in e.Errors)
		{
			ServerMessageEventArgs e2 = new ServerMessageEventArgs(error);
			this.ServerMessageInternal(this, e2);
		}
	}

	public void ForceDisconnected()
	{
		m_forceDisconnected = true;
		Disconnect();
	}
}

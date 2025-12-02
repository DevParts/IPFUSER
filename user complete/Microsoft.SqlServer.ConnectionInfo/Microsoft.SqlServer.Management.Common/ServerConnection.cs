using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;

namespace Microsoft.SqlServer.Management.Common;

public sealed class ServerConnection : ConnectionManager, ISfcConnection
{
	internal class ConnectionFactory
	{
		private string connectionString;

		private ServerComparer serverComparer;

		private Dictionary<string, ServerConnection> pooledDbConnectionsCache;

		private static Dictionary<string, ConnectionFactory> connFactoriesCache = new Dictionary<string, ConnectionFactory>();

		public ServerComparer ServerComparer => serverComparer;

		public static ConnectionFactory GetInstance(ServerConnection serverConnection)
		{
			if (!connFactoriesCache.TryGetValue(serverConnection.ConnectionString, out var value))
			{
				lock (typeof(ConnectionFactory))
				{
					if (!connFactoriesCache.TryGetValue(serverConnection.ConnectionString, out value))
					{
						value = new ConnectionFactory(serverConnection);
						connFactoriesCache[serverConnection.ConnectionString] = value;
					}
				}
			}
			return value;
		}

		private ConnectionFactory(ServerConnection serverConnection)
		{
			connectionString = serverConnection.ConnectionString;
			serverComparer = new ServerComparer(serverConnection, "master");
			pooledDbConnectionsCache = new Dictionary<string, ServerConnection>(new DatabaseNameEqualityComparer(serverComparer));
		}

		public ServerConnection GetDatabaseConnection(string dbName, bool poolConnection = true, IRenewableToken accessToken = null)
		{
			ServerConnection value;
			if (poolConnection)
			{
				if (!pooledDbConnectionsCache.TryGetValue(dbName, out value))
				{
					lock (this)
					{
						if (!pooledDbConnectionsCache.TryGetValue(dbName, out value))
						{
							value = CreateServerConnection(connectionString, dbName, poolConnection, accessToken);
							pooledDbConnectionsCache[dbName] = value;
						}
					}
				}
				value.AccessToken = accessToken ?? value.AccessToken;
			}
			else
			{
				value = CreateServerConnection(connectionString, dbName, poolConnection, accessToken);
			}
			return value;
		}

		private ServerConnection CreateServerConnection(string connString, string initialCatalog, bool poolConn, IRenewableToken accessToken = null)
		{
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connString);
			sqlConnectionStringBuilder.InitialCatalog = initialCatalog;
			sqlConnectionStringBuilder.Pooling = poolConn;
			if (accessToken != null)
			{
				ServerConnection serverConnection = new ServerConnection(accessToken);
				serverConnection.ConnectionString = sqlConnectionStringBuilder.ConnectionString;
				return serverConnection;
			}
			ServerConnection serverConnection2 = new ServerConnection();
			serverConnection2.ConnectionString = sqlConnectionStringBuilder.ConnectionString;
			return serverConnection2;
		}
	}

	private const int CACHE_SIZE = 128;

	private const string BatchSeparator_Default = "GO";

	private const int StatementTimeout_Default = 30;

	private const int MaxParams_Default = 2090;

	internal const string Database_Default = "master";

	private int m_StatementTimeout;

	private string m_BatchSeparator;

	private int m_TransactionDepth;

	private SqlExecutionModes m_ExecutionMode;

	private SqlCommand m_SqlCommand;

	private SqlCommand currentSqlCommand;

	private List<SqlParameter> m_Parameters;

	private bool isSqlConnectionUsed;

	private ExecutionCache<string, SqlBatch> m_CommandCache = new ExecutionCache<string, SqlBatch>(128);

	private static Regex reUseDb = new Regex("^USE\\s*(?<left>\\[)*((?(left)[^\\]]|[^;\\s])+)(?(left)\\]|\\s)*\\s*;*\\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

	private static Regex reQueryTags = new Regex("<msparam\\>((?<HEX>(\\s)*(0[xX][0-9a-fA-F]+)(\\s)*)|(?<STR>((.|\\n)*?)))\\</msparam\\>", RegexOptions.Compiled);

	private static Regex reQueryPrepStatement = new Regex("exec[^_]*((sp_executesql)|(sp_cursorprepexec)|(sp_prepexec)|(sp_cursorprepare)|(sp_cursoropen)|(sp_prepare))([^']|'(?=@)|'(?=,))*'(?![@])(?<qry>([^']|''|'(?!\\s*,))*)'.*", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

	private static Regex reQueryParametersQIOn = new Regex("(?<left>[\\s,(?=!<>])(((((?<mid>[N])?(?<term>')([^']|'')*(?<right>'))|(0x[\\da-fA-F]*)|([-+]?(([\\d]*\\.[\\d]*|[\\d]+)([eE]?[\\d]*)))|([~]?[-+]?([\\d]+)))([\\s]?[\\+\\-\\*\\/\\%\\&\\|\\^][\\s]?)?)+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

	private static Regex reQueryParametersQIOff = new Regex("(?<left>[\\s,(?=!<>])(((((?<mid>[N])?(?<term>('|\"))([^'\"]|''|\"\")*(?<right>('|\")))|(0x[\\da-fA-F]*)|([-+]?(([\\d]*\\.[\\d]*|[\\d]+)([eE]?[\\d]*)))|([~]?[-+]?([\\d]+)))([\\s]?[\\+\\-\\*\\/\\%\\&\\|\\^][\\s]?)?)+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

	private static QueryParameterizationMode parameterizationMode = QueryParameterizationMode.ForcedParameterization;

	private static DeferredUseMode deferredUseMode = DeferredUseMode.None;

	private static bool cachedQueries = false;

	private string m_TrueName;

	public static QueryParameterizationMode ParameterizationMode
	{
		get
		{
			return parameterizationMode;
		}
		set
		{
			parameterizationMode = value;
		}
	}

	public static DeferredUseMode UseMode
	{
		get
		{
			return deferredUseMode;
		}
		set
		{
			deferredUseMode = value;
		}
	}

	internal static bool CachedQueries
	{
		get
		{
			return cachedQueries;
		}
		set
		{
			cachedQueries = value;
		}
	}

	public int StatementTimeout
	{
		get
		{
			return m_StatementTimeout;
		}
		set
		{
			if (0 > value)
			{
				throw new InvalidPropertyValueException();
			}
			m_StatementTimeout = value;
		}
	}

	Version ISfcConnection.ServerVersion => (Version)base.ServerVersion;

	public ServerType ConnectionType => ServerType.DatabaseEngine;

	public string BatchSeparator
	{
		get
		{
			return m_BatchSeparator;
		}
		set
		{
			if (0 >= m_BatchSeparator.Length)
			{
				throw new InvalidPropertyValueException();
			}
			m_BatchSeparator = value;
		}
	}

	public int TransactionDepth
	{
		get
		{
			CheckDisconnected();
			m_TransactionDepth = (int)ExecuteScalar("select @@TRANCOUNT");
			return m_TransactionDepth;
		}
	}

	public SqlExecutionModes SqlExecutionModes
	{
		get
		{
			return m_ExecutionMode;
		}
		set
		{
			m_ExecutionMode = value;
		}
	}

	public FixedServerRoles FixedServerRoles
	{
		get
		{
			SqlExecutionModes sqlExecutionModes = SqlExecutionModes;
			try
			{
				SqlExecutionModes = SqlExecutionModes.ExecuteSql;
				return (FixedServerRoles)(int)ExecuteScalar("select is_srvrolemember('sysadmin') * 1 +is_srvrolemember('serveradmin') * 2 +is_srvrolemember('setupadmin') * 4 +is_srvrolemember('securityadmin') * 8 +is_srvrolemember('processadmin') * 16 +is_srvrolemember('dbcreator') * 32 +is_srvrolemember('diskadmin') * 64" + ((7 < base.ServerVersion.Major) ? "+ is_srvrolemember('bulkadmin') * 128" : ""));
			}
			finally
			{
				SqlExecutionModes = sqlExecutionModes;
			}
		}
	}

	public ServerUserProfiles UserProfile
	{
		get
		{
			SqlExecutionModes sqlExecutionModes = SqlExecutionModes;
			try
			{
				SqlExecutionModes = SqlExecutionModes.ExecuteSql;
				return (ServerUserProfiles)(int)ExecuteScalar("exec master.dbo.sp_MSdbuserpriv N'serv'");
			}
			catch (ExecutionFailureException ex)
			{
				if (ex.InnerException is SqlException { Number: 15517, Class: 16 })
				{
					return ServerUserProfiles.None;
				}
				throw;
			}
			finally
			{
				SqlExecutionModes = sqlExecutionModes;
			}
		}
	}

	public int ProcessID
	{
		get
		{
			SqlExecutionModes sqlExecutionModes = SqlExecutionModes;
			try
			{
				SqlExecutionModes = SqlExecutionModes.ExecuteSql;
				return Convert.ToInt32(ExecuteScalar("select @@SPID"), ConnectionInfoBase.DefaultCulture);
			}
			finally
			{
				SqlExecutionModes = sqlExecutionModes;
			}
		}
	}

	public string TrueLogin
	{
		get
		{
			SqlExecutionModes sqlExecutionModes = SqlExecutionModes;
			try
			{
				SqlExecutionModes = SqlExecutionModes.ExecuteSql;
				return (string)ExecuteScalar("select suser_sname()");
			}
			finally
			{
				SqlExecutionModes = sqlExecutionModes;
			}
		}
	}

	public string TrueName
	{
		get
		{
			if (m_TrueName == null)
			{
				if (base.IsForceDisconnected)
				{
					throw new DisconnectedConnectionException(StringConnectionInfo.TrueNameMustBeSet);
				}
				SqlExecutionModes sqlExecutionModes = SqlExecutionModes;
				try
				{
					SqlExecutionModes = SqlExecutionModes.ExecuteSql;
					if (7 < base.ServerVersion.Major)
					{
						m_TrueName = ExecuteScalar("select SERVERPROPERTY(N'servername')") as string;
					}
					else
					{
						m_TrueName = (string)ExecuteScalar("select @@SERVERNAME");
					}
				}
				finally
				{
					SqlExecutionModes = sqlExecutionModes;
				}
			}
			return m_TrueName;
		}
		set
		{
			if (!base.IsForceDisconnected && base.IsOpen)
			{
				throw new DisconnectedConnectionException(StringConnectionInfo.CannotSetTrueName);
			}
			m_TrueName = value;
		}
	}

	internal override bool BlockPoolDisconnect
	{
		get
		{
			if (m_TransactionDepth <= 0)
			{
				return deferredUseMode > DeferredUseMode.None;
			}
			return true;
		}
	}

	public ServerConnection()
	{
		InitDefaults();
	}

	public ServerConnection(IRenewableToken token)
		: base(token)
	{
		InitDefaults();
	}

	public ServerConnection(SqlConnectionInfo sci)
		: base(sci)
	{
		InitDefaults();
		if (sci.QueryTimeout >= 0)
		{
			StatementTimeout = sci.QueryTimeout;
		}
	}

	public ServerConnection(SqlConnection sqlConnection)
		: this(sqlConnection, null)
	{
	}

	public ServerConnection(SqlConnection sqlConnection, IRenewableToken accessToken)
		: base(sqlConnection, accessToken)
	{
		InitDefaults();
		isSqlConnectionUsed = true;
	}

	public ServerConnection(string serverInstance)
	{
		InitDefaults();
		try
		{
			base.ServerInstance = serverInstance;
		}
		catch (InvalidPropertyValueException ex)
		{
			throw new InvalidArgumentException(ex.Message, ex);
		}
	}

	public ServerConnection(string serverInstance, string userName, string password)
	{
		InitDefaults();
		try
		{
			base.ServerInstance = serverInstance;
			base.LoginSecure = false;
			base.Login = userName;
			base.Password = password;
		}
		catch (InvalidPropertyValueException ex)
		{
			throw new InvalidArgumentException(ex.Message, ex);
		}
	}

	public ServerConnection(string serverInstance, string userName, SecureString password)
	{
		InitDefaults();
		try
		{
			base.ServerInstance = serverInstance;
			base.LoginSecure = false;
			base.Login = userName;
			base.SecurePassword = password;
		}
		catch (InvalidPropertyValueException ex)
		{
			throw new InvalidArgumentException(ex.Message, ex);
		}
	}

	private void InitDefaults()
	{
		m_SqlCommand = base.SqlConnectionObject.CreateCommand();
		currentSqlCommand = m_SqlCommand;
		m_Parameters = new List<SqlParameter>();
		m_StatementTimeout = 600;
		m_TransactionDepth = 0;
		m_BatchSeparator = "GO";
		m_ExecutionMode = SqlExecutionModes.ExecuteSql;
	}

	private void CopyServerConnection(ServerConnection sc)
	{
		CopyConnectionManager(sc);
		sc.StatementTimeout = m_StatementTimeout;
		sc.BatchSeparator = m_BatchSeparator;
		sc.SqlExecutionModes = m_ExecutionMode;
	}

	public ServerConnection Copy()
	{
		ServerConnection serverConnection = null;
		if (isSqlConnectionUsed)
		{
			bool flag = ConnectionState.Open == (ConnectionState.Open & base.SqlConnectionObject.State);
			SqlConnection sqlConnection = ((ICloneable)base.SqlConnectionObject).Clone() as SqlConnection;
			if (flag)
			{
				sqlConnection.Open();
			}
			serverConnection = new ServerConnection(sqlConnection);
		}
		else
		{
			serverConnection = new ServerConnection();
		}
		CopyServerConnection(serverConnection);
		return serverConnection;
	}

	private SqlCommand AllocSqlCommand(string query)
	{
		SqlCommand sqlCommand = base.SqlConnectionObject.CreateCommand();
		sqlCommand.CommandText = query;
		foreach (SqlParameter parameter in m_Parameters)
		{
			SqlParameter sqlParameter = new SqlParameter(parameter.ParameterName, parameter.Value);
			sqlParameter.SqlDbType = parameter.SqlDbType;
			sqlParameter.Size = parameter.Size;
			sqlCommand.Parameters.Add(sqlParameter);
		}
		return sqlCommand;
	}

	private SqlCommand CacheQuery(string query)
	{
		SqlCommand sqlCommand;
		if (m_Parameters.Count > 0)
		{
			if (m_CommandCache.ContainsKey(query))
			{
				SqlBatch sqlBatch = m_CommandCache[query];
				sqlBatch.ExecutionCount++;
				_ = sqlBatch.ExecutionCount;
				_ = 3;
				foreach (SqlParameter parameter in m_Parameters)
				{
					sqlBatch.Command.Parameters[parameter.ParameterName].Value = parameter.Value;
				}
				return sqlBatch.Command;
			}
			sqlCommand = AllocSqlCommand(query);
			m_CommandCache.Add(new SqlBatch(sqlCommand));
		}
		else
		{
			sqlCommand = AllocSqlCommand(query);
		}
		return sqlCommand;
	}

	private string AddParameterLiterals(Match match)
	{
		SqlParameter sqlParameter = new SqlParameter("@_msparam_" + m_Parameters.Count, match.Groups[1].Value);
		sqlParameter.SqlDbType = SqlDbType.NVarChar;
		sqlParameter.Size = 4000;
		m_Parameters.Add(sqlParameter);
		return sqlParameter.ParameterName;
	}

	private string AddParameterForced(Match match)
	{
		string text = string.Empty;
		SqlDbType sqlDbType = SqlDbType.NVarChar;
		Group obj = match.Groups["HEX"];
		Group obj2 = match.Groups["STR"];
		if (obj != null && obj.Length > 0)
		{
			text = Convert.ToInt64(obj.ToString(), 16).ToString();
			sqlDbType = SqlDbType.BigInt;
		}
		else if (obj2 != null)
		{
			text = obj2.ToString();
		}
		if (m_Parameters.Count < 2090)
		{
			SqlParameter sqlParameter = new SqlParameter("@_msparam_" + m_Parameters.Count, text);
			sqlParameter.SqlDbType = sqlDbType;
			sqlParameter.Size = 4000;
			m_Parameters.Add(sqlParameter);
			return sqlParameter.ParameterName;
		}
		string text2 = CommonUtils.EscapeString(text, "'");
		return "'" + text2 + "'";
	}

	public static string NormalizeQuery(string QueryText, bool QuotedIdentifiers)
	{
		string input = QueryText;
		if (reQueryPrepStatement.IsMatch(input))
		{
			input = reQueryPrepStatement.Replace(input, "${qry}").Replace("''", "'");
		}
		if (QuotedIdentifiers)
		{
			return reQueryParametersQIOn.Replace(input, "${left}${mid}${term}?${right}");
		}
		return reQueryParametersQIOff.Replace(input, "${left}${mid}${term}?${right}");
	}

	public static string NormalizeQuery(string QueryText)
	{
		return NormalizeQuery(QueryText, QuotedIdentifiers: true);
	}

	private SqlCommand GetSqlCommand(string query)
	{
		SqlCommand sqlCommand;
		if (parameterizationMode >= QueryParameterizationMode.ForcedParameterization)
		{
			m_Parameters.Clear();
			string text = (reQueryTags.IsMatch(query) ? reQueryTags.Replace(query, AddParameterForced) : ((parameterizationMode != QueryParameterizationMode.ParameterizeLiterals) ? query : reQueryParametersQIOn.Replace(query, AddParameterLiterals)));
			if (deferredUseMode >= DeferredUseMode.CollapseRedundant)
			{
				Match match = reUseDb.Match(query);
				if (match.Groups.Count > 1)
				{
					string value = match.Groups[1].Value;
					if (value == base.SqlConnectionObject.Database && match.Groups[0].Value == query)
					{
						text = string.Empty;
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				sqlCommand = CacheQuery(text);
			}
			else
			{
				sqlCommand = m_SqlCommand;
				sqlCommand.CommandText = "";
			}
		}
		else
		{
			sqlCommand = m_SqlCommand;
			sqlCommand.CommandText = query;
		}
		sqlCommand.CommandTimeout = StatementTimeout;
		currentSqlCommand = sqlCommand;
		return sqlCommand;
	}

	private void CaptureCommand(string query)
	{
		GenerateStatementExecutedEvent(query);
		if (SqlExecutionModes.CaptureSql == (SqlExecutionModes.CaptureSql & SqlExecutionModes))
		{
			base.CapturedSql.Add(query);
		}
	}

	private bool IsDirectExecutionMode()
	{
		return SqlExecutionModes.ExecuteSql == (SqlExecutionModes.ExecuteSql & SqlExecutionModes);
	}

	bool ISfcConnection.Connect()
	{
		Connect();
		if (!base.IsOpen)
		{
			return false;
		}
		return true;
	}

	bool ISfcConnection.Disconnect()
	{
		if (base.IsOpen)
		{
			Disconnect();
		}
		return !base.IsOpen;
	}

	ISfcConnection ISfcConnection.Copy()
	{
		return Copy();
	}

	object ISfcConnection.ToEnumeratorObject()
	{
		return this;
	}

	public void ChangePassword(SecureString newPassword)
	{
		CheckDisconnected();
		try
		{
			SqlConnection.ChangePassword(base.ConnectionString, EncryptionUtility.DecryptSecureString(newPassword));
			base.SecurePassword = newPassword;
		}
		catch (SqlException innerException)
		{
			throw new ChangePasswordFailureException(StringConnectionInfo.PasswordCouldNotBeChanged, innerException);
		}
	}

	public void ChangePassword(string newPassword)
	{
		CheckDisconnected();
		try
		{
			SqlConnection.ChangePassword(base.ConnectionString, newPassword);
			ForceSetPassword(newPassword);
		}
		catch (SqlException innerException)
		{
			throw new ChangePasswordFailureException(StringConnectionInfo.PasswordCouldNotBeChanged, innerException);
		}
	}

	public int[] ExecuteNonQuery(StringCollection sqlCommands)
	{
		return ExecuteNonQuery(sqlCommands, ExecutionTypes.Default);
	}

	public int[] ExecuteNonQuery(StringCollection sqlCommands, ExecutionTypes executionType)
	{
		return ExecuteNonQuery(sqlCommands, executionType, retry: true);
	}

	public int[] ExecuteNonQuery(StringCollection sqlCommands, ExecutionTypes executionType, bool retry)
	{
		CheckDisconnected();
		AutoDisconnectMode autoDisconnectMode = base.AutoDisconnectMode;
		base.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
		try
		{
			ArrayList arrayList = new ArrayList();
			StringEnumerator enumerator = sqlCommands.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					arrayList.Add(ExecuteNonQuery(current, executionType, retry));
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			int[] array = new int[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}
		finally
		{
			base.AutoDisconnectMode = autoDisconnectMode;
			PoolDisconnect();
		}
	}

	public int ExecuteNonQuery(string sqlCommand)
	{
		return ExecuteNonQuery(sqlCommand, ExecutionTypes.Default);
	}

	public int ExecuteNonQuery(string sqlCommand, ExecutionTypes executionType)
	{
		return ExecuteNonQuery(sqlCommand, executionType, retry: true);
	}

	public int ExecuteNonQuery(string sqlCommand, ExecutionTypes executionType, bool retry)
	{
		CheckDisconnected();
		int statementsToReverse = 0;
		int num = 0;
		StringCollection statements = GetStatements(sqlCommand, executionType, ref statementsToReverse);
		if (!IsDirectExecutionMode())
		{
			StringEnumerator enumerator = statements.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					CaptureCommand(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			return 0;
		}
		PoolConnect();
		try
		{
			int num2 = 0;
			StringEnumerator enumerator2 = statements.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current2 = enumerator2.Current;
					try
					{
						SqlCommand sqlCommand2 = GetSqlCommand(current2);
						if (!string.IsNullOrEmpty(sqlCommand2.CommandText))
						{
							CaptureCommand(current2);
							num2 += (int)ExecuteTSql(ExecuteTSqlAction.ExecuteNonQuery, sqlCommand2, null, retry);
							num++;
						}
					}
					catch (SqlException ex)
					{
						RefreshTransactionDepth(ex.Class);
						if (ExecutionTypes.ContinueOnError == (ExecutionTypes.ContinueOnError & executionType))
						{
							continue;
						}
						if (num > 0 && ex.Class <= 20)
						{
							int num3 = ((statementsToReverse < num) ? statementsToReverse : num);
							for (int i = statements.Count - num3; i < statements.Count; i++)
							{
								ExecuteNonQuery(statements[i], ExecutionTypes.ContinueOnError, retry);
							}
						}
						throw new ExecutionFailureException(StringConnectionInfo.ExecutionFailure, ex);
					}
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable2)
				{
					disposable2.Dispose();
				}
			}
			return num2;
		}
		finally
		{
			PoolDisconnect();
		}
	}

	private StringCollection GetStatements(string query, ExecutionTypes executionType, ref int statementsToReverse)
	{
		statementsToReverse = 0;
		StringCollection stringCollection = new StringCollection();
		if (ExecutionTypes.NoCommands == (ExecutionTypes.NoCommands & executionType))
		{
			stringCollection.Add(query);
		}
		else
		{
			string assemblyName = typeof(ServerConnection).GetAssembly().FullName.Replace("ConnectionInfo", "BatchParserClient");
			Assembly assembly = NetCoreHelpers.LoadAssembly(assemblyName);
			Type type = assembly.GetType("Microsoft.SqlServer.Management.Common.ExecuteBatch", throwOnError: true);
			object target = type.InvokeMember(".ctor", BindingFlags.CreateInstance, null, null, new object[0], CultureInfo.InvariantCulture);
			StringCollection stringCollection2 = (StringCollection)type.InvokeMember("GetStatements", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, target, new object[1] { query }, CultureInfo.InvariantCulture);
			StringEnumerator enumerator = stringCollection2.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					stringCollection.Add(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		if (ExecutionTypes.QuotedIdentifierOn == (ExecutionTypes.QuotedIdentifierOn & executionType))
		{
			statementsToReverse++;
			stringCollection.Insert(0, "SET QUOTED_IDENTIFIER ON");
			stringCollection.Add("SET QUOTED_IDENTIFIER OFF");
		}
		if (ExecutionTypes.ParseOnly == (ExecutionTypes.ParseOnly & executionType))
		{
			statementsToReverse++;
			stringCollection.Insert(0, "SET PARSEONLY ON");
			stringCollection.Add("SET PARSEONLY OFF");
		}
		if (ExecutionTypes.NoExec == (ExecutionTypes.NoExec & executionType))
		{
			statementsToReverse++;
			stringCollection.Insert(0, "SET NOEXEC ON");
			stringCollection.Add("SET NOEXEC OFF");
		}
		return stringCollection;
	}

	public DataSet[] ExecuteWithResults(StringCollection sqlCommands)
	{
		AutoDisconnectMode autoDisconnectMode = base.AutoDisconnectMode;
		base.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
		try
		{
			ArrayList arrayList = new ArrayList();
			StringEnumerator enumerator = sqlCommands.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					arrayList.Add(ExecuteWithResults(current));
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			DataSet[] array = new DataSet[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}
		finally
		{
			base.AutoDisconnectMode = autoDisconnectMode;
			PoolDisconnect();
		}
	}

	public DataSet ExecuteWithResults(string sqlCommand)
	{
		return ExecuteWithResults(sqlCommand, retry: true);
	}

	public DataSet ExecuteWithResults(string sqlCommand, bool retry)
	{
		CheckDisconnected();
		CaptureCommand(sqlCommand);
		if (!IsDirectExecutionMode())
		{
			return null;
		}
		PoolConnect();
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Locale = CultureInfo.InvariantCulture;
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
			sqlDataAdapter.SelectCommand = GetSqlCommand(sqlCommand);
			if (!string.IsNullOrEmpty(sqlDataAdapter.SelectCommand.CommandText))
			{
				ExecuteTSql(ExecuteTSqlAction.FillDataSet, sqlDataAdapter, dataSet, retry);
			}
			return dataSet;
		}
		catch (SqlException ex)
		{
			RefreshTransactionDepth(ex.Class);
			throw new ExecutionFailureException(StringConnectionInfo.ExecutionFailure, ex);
		}
		finally
		{
			PoolDisconnect();
		}
	}

	public SqlDataReader ExecuteReader(string sqlCommand)
	{
		SqlCommand command;
		return ExecuteReader(sqlCommand, out command);
	}

	public SqlDataReader ExecuteReader(string sqlCommand, out SqlCommand command)
	{
		command = null;
		if (string.IsNullOrEmpty(sqlCommand))
		{
			return null;
		}
		command = GetSqlCommand(sqlCommand);
		return GetExecuteReader(command);
	}

	internal SqlDataReader ExecuteReader(SqlCommand command)
	{
		if (command == null)
		{
			return null;
		}
		command.Connection = base.SqlConnectionObject;
		return GetExecuteReader(command);
	}

	private SqlDataReader GetExecuteReader(SqlCommand command)
	{
		if (command == null || string.IsNullOrEmpty(command.CommandText))
		{
			return null;
		}
		CheckDisconnected();
		CaptureCommand(command.CommandText);
		if (!IsDirectExecutionMode())
		{
			return null;
		}
		PoolConnect();
		try
		{
			return ExecuteTSql(ExecuteTSqlAction.ExecuteReader, command, null, catchException: true) as SqlDataReader;
		}
		catch (SqlException ex)
		{
			RefreshTransactionDepth(ex.Class);
			throw new ExecutionFailureException(StringConnectionInfo.ExecutionFailure, ex);
		}
	}

	public object[] ExecuteScalar(StringCollection sqlCommands)
	{
		CheckDisconnected();
		AutoDisconnectMode autoDisconnectMode = base.AutoDisconnectMode;
		base.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
		try
		{
			ArrayList arrayList = new ArrayList();
			StringEnumerator enumerator = sqlCommands.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					arrayList.Add(ExecuteScalar(current));
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			object[] array = new object[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}
		finally
		{
			base.AutoDisconnectMode = autoDisconnectMode;
			PoolDisconnect();
		}
	}

	public object ExecuteScalar(string sqlCommand)
	{
		CheckDisconnected();
		CaptureCommand(sqlCommand);
		if (!IsDirectExecutionMode())
		{
			return null;
		}
		PoolConnect();
		try
		{
			object result = null;
			SqlCommand sqlCommand2 = GetSqlCommand(sqlCommand);
			if (!string.IsNullOrEmpty(sqlCommand2.CommandText))
			{
				if (cachedQueries)
				{
					SqlBatch sqlBatch;
					if (!m_CommandCache.ContainsKey(sqlCommand2.CommandText))
					{
						sqlBatch = new SqlBatch(sqlCommand2);
						m_CommandCache.Add(sqlBatch);
					}
					else
					{
						sqlBatch = m_CommandCache[sqlCommand2.CommandText];
					}
					result = (sqlBatch.HasResult() ? sqlBatch.Result : (sqlBatch.Result = ExecuteTSql(ExecuteTSqlAction.ExecuteScalar, sqlCommand2, null, catchException: true)));
				}
				else
				{
					result = ExecuteTSql(ExecuteTSqlAction.ExecuteScalar, sqlCommand2, null, catchException: true);
				}
			}
			return result;
		}
		catch (SqlException ex)
		{
			RefreshTransactionDepth(ex.Class);
			throw new ExecutionFailureException(StringConnectionInfo.ExecutionFailure, ex);
		}
		finally
		{
			PoolDisconnect();
		}
	}

	public void BeginTransaction()
	{
		CheckDisconnected();
		AutoDisconnectMode autoDisconnectMode = base.AutoDisconnectMode;
		base.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
		try
		{
			ExecuteNonQuery("BEGIN TRANSACTION", ExecutionTypes.Default);
			m_TransactionDepth++;
		}
		finally
		{
			base.AutoDisconnectMode = autoDisconnectMode;
			if (0 >= m_TransactionDepth)
			{
				PoolDisconnect();
			}
		}
	}

	public void CommitTransaction()
	{
		CheckDisconnected();
		try
		{
			ExecuteNonQuery("if (@@trancount > 0) COMMIT TRANSACTION", ExecutionTypes.Default);
			if (m_TransactionDepth > 0)
			{
				m_TransactionDepth--;
			}
		}
		catch (ExecutionFailureException ex)
		{
			if (ex.InnerException is SqlException ex2 && 16 == ex2.Class && 3902 == ex2.Number)
			{
				m_TransactionDepth = 0;
				throw new NotInTransactionException(StringConnectionInfo.NotInTransaction);
			}
			throw;
		}
		finally
		{
			PoolDisconnect();
		}
	}

	public void RollBackTransaction()
	{
		CheckDisconnected();
		try
		{
			ExecuteNonQuery("if (@@trancount > 0) ROLLBACK TRANSACTION", ExecutionTypes.Default);
			if (m_TransactionDepth > 0)
			{
				m_TransactionDepth--;
			}
		}
		catch (ExecutionFailureException ex)
		{
			if (ex.InnerException is SqlException ex2 && 16 == ex2.Class && 3903 == ex2.Number)
			{
				m_TransactionDepth = 0;
				throw new NotInTransactionException(StringConnectionInfo.NotInTransaction);
			}
			throw;
		}
		finally
		{
			PoolDisconnect();
		}
	}

	private void RefreshTransactionDepth(byte severity)
	{
		CheckDisconnected();
		if (m_TransactionDepth <= 0 || severity >= 20)
		{
			m_TransactionDepth = 0;
			return;
		}
		SqlExecutionModes sqlExecutionModes = SqlExecutionModes;
		try
		{
			SqlExecutionModes = SqlExecutionModes.ExecuteSql;
			m_TransactionDepth = (int)ExecuteScalar("select @@TRANCOUNT");
		}
		finally
		{
			SqlExecutionModes = sqlExecutionModes;
		}
	}

	public bool IsInFixedServerRole(FixedServerRoles fixedServerRole)
	{
		return fixedServerRole == (fixedServerRole & FixedServerRoles);
	}

	public void Cancel()
	{
		if (!string.IsNullOrEmpty(currentSqlCommand.CommandText))
		{
			currentSqlCommand.Cancel();
		}
	}

	internal override void InitAfterConnect()
	{
		m_TransactionDepth = 0;
	}

	internal void CheckDisconnected()
	{
		if (base.IsForceDisconnected)
		{
			throw new DisconnectedConnectionException(StringConnectionInfo.CannotPerformOperationWhileDisconnected);
		}
	}

	public ServerConnection GetDatabaseConnection(string dbName, bool poolConnection = true)
	{
		return ConnectionFactory.GetInstance(this).GetDatabaseConnection(dbName, poolConnection, base.AccessToken);
	}

	public ServerConnection GetDatabaseConnection(string dbName, bool poolConnection, IRenewableToken accessToken)
	{
		return ConnectionFactory.GetInstance(this).GetDatabaseConnection(dbName, poolConnection, accessToken ?? base.AccessToken);
	}
}

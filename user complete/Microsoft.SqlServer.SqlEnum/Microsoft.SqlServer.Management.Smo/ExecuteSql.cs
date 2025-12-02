using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class ExecuteSql
{
	private ServerConnection m_conctx;

	private bool bHasConnected;

	private ArrayList m_Messages;

	private SqlExecutionModes m_semInitial;

	private SqlInfoMessageEventHandler m_ServerInfoMessage;

	private string databaseName;

	public ExecuteSql(object con)
	{
		bHasConnected = false;
		InitConnection(con);
	}

	public ExecuteSql(object con, string databaseName, bool poolConnection = true)
	{
		bHasConnected = false;
		InitConnection(con);
		this.databaseName = databaseName;
		InitServerConnectionWithDatabaseName(poolConnection);
	}

	private void InitServerConnectionWithDatabaseName(bool poolConnection = true)
	{
		if (m_conctx != null && !string.IsNullOrEmpty(databaseName))
		{
			m_conctx = m_conctx.GetDatabaseConnection(databaseName, poolConnection);
		}
	}

	internal void Connect()
	{
		m_semInitial = m_conctx.SqlExecutionModes;
		m_conctx.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
		if (!m_conctx.IsOpen)
		{
			try
			{
				m_conctx.Connect();
			}
			catch
			{
				m_conctx.SqlExecutionModes = m_semInitial;
				throw;
			}
			bHasConnected = true;
		}
	}

	internal void Disconnect()
	{
		m_conctx.SqlExecutionModes = m_semInitial;
		if (bHasConnected)
		{
			m_conctx.Disconnect();
		}
	}

	private void InitConnection(object con)
	{
		m_conctx = con as ServerConnection;
		if (m_conctx != null)
		{
			return;
		}
		if (con is SqlConnectionInfoWithConnection sqlConnectionInfoWithConnection)
		{
			m_conctx = sqlConnectionInfoWithConnection.ServerConnection;
			return;
		}
		if (con is SqlConnectionInfo sci)
		{
			m_conctx = new ServerConnection(sci);
			return;
		}
		if (con is SqlConnection sqlConnection)
		{
			m_conctx = new ServerConnection(sqlConnection);
			return;
		}
		if (con is SqlDirectConnection sqlDirectConnection)
		{
			m_conctx = new ServerConnection(sqlDirectConnection.SqlConnection);
			return;
		}
		throw new InternalEnumeratorException(StringSqlEnumerator.InvalidConnectionType);
	}

	private void StartCapture()
	{
		m_Messages = new ArrayList();
		m_ServerInfoMessage = RecordMessage;
		m_conctx.InfoMessage += m_ServerInfoMessage;
	}

	private void RecordMessage(object sender, SqlInfoMessageEventArgs e)
	{
		m_Messages.Add(e);
	}

	private ArrayList ClearCapture()
	{
		if (m_ServerInfoMessage != null)
		{
			m_conctx.InfoMessage -= m_ServerInfoMessage;
			m_ServerInfoMessage = null;
		}
		ArrayList messages = m_Messages;
		m_Messages = null;
		return messages;
	}

	private bool TryToReconnect(ExecutionFailureException e)
	{
		if (e == null)
		{
			return false;
		}
		if (((SqlException)e.InnerException).Class >= 20 && !m_conctx.IsOpen)
		{
			try
			{
				m_conctx.SqlConnectionObject.Open();
			}
			catch (SqlException)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void ExecuteImmediate(string query)
	{
		try
		{
			m_conctx.ExecuteNonQuery(query, ExecutionTypes.NoCommands);
		}
		catch (ExecutionFailureException e)
		{
			if (TryToReconnect(e))
			{
				m_conctx.ExecuteNonQuery(query, ExecutionTypes.NoCommands);
				return;
			}
			throw;
		}
	}

	public DataTable ExecuteWithResults(string query)
	{
		DataSet dataSet = null;
		try
		{
			dataSet = m_conctx.ExecuteWithResults(query);
		}
		catch (ExecutionFailureException e)
		{
			if (!TryToReconnect(e))
			{
				throw;
			}
			dataSet = m_conctx.ExecuteWithResults(query);
		}
		if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
		{
			return dataSet.Tables[0];
		}
		return null;
	}

	public SqlDataReader GetDataReader(string query, out SqlCommand command)
	{
		SqlDataReader sqlDataReader = null;
		try
		{
			return m_conctx.ExecuteReader(query, out command);
		}
		catch (ExecutionFailureException e)
		{
			if (TryToReconnect(e))
			{
				return m_conctx.ExecuteReader(query, out command);
			}
			throw;
		}
	}

	public SqlDataReader GetDataReader(string query)
	{
		SqlCommand command;
		return GetDataReader(query, out command);
	}

	public ServerVersion GetServerVersion()
	{
		return m_conctx.ServerVersion;
	}

	public DatabaseEngineType GetDatabaseEngineType()
	{
		return m_conctx.DatabaseEngineType;
	}

	public DatabaseEngineEdition GetDatabaseEngineEdition()
	{
		return m_conctx.DatabaseEngineEdition;
	}

	internal bool IsContainedAuthentication()
	{
		return m_conctx.IsContainedAuthentication;
	}

	public static ArrayList ExecuteImmediateGetMessage(string query, object con)
	{
		ExecuteSql executeSql = new ExecuteSql(con);
		executeSql.Connect();
		ArrayList result;
		try
		{
			executeSql.StartCapture();
			executeSql.ExecuteImmediate(query);
		}
		finally
		{
			result = executeSql.ClearCapture();
			executeSql.Disconnect();
		}
		return result;
	}

	public static void ExecuteImmediate(string query, object con)
	{
		ExecuteSql executeSql = new ExecuteSql(con);
		executeSql.Connect();
		try
		{
			executeSql.ExecuteImmediate(query);
		}
		finally
		{
			executeSql.Disconnect();
		}
	}

	public static DataTable ExecuteWithResults(string query, object con)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(query);
		return ExecuteWithResults(stringCollection, con);
	}

	public static DataTable ExecuteWithResults(string query, object con, string database, bool poolConnection = true)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(query);
		return ExecuteWithResults(stringCollection, con, database, poolConnection);
	}

	public static DataTable ExecuteWithResults(StringCollection query, object con, StatementBuilder sb)
	{
		DataProvider dataProvider = null;
		dataProvider = GetDataProvider(query, con, sb, DataProvider.RetriveMode.RetriveDataTable);
		return dataProvider.GetTable();
	}

	internal static DataProvider GetDataProvider(StringCollection query, object con, StatementBuilder sb)
	{
		return GetDataProvider(query, con, sb, DataProvider.RetriveMode.RetriveDataReader);
	}

	internal static DataProvider GetDataProvider(StringCollection query, object con, StatementBuilder sb, DataProvider.RetriveMode rm)
	{
		ExecuteSql executeSql = new ExecuteSql(con);
		executeSql.Connect();
		bool flag = false;
		DataProvider dataProvider = null;
		try
		{
			dataProvider = new DataProvider(sb, rm);
			int i;
			for (i = 0; i < query.Count - 1; i++)
			{
				executeSql.ExecuteImmediate(query[i]);
			}
			dataProvider.SetConnectionAndQuery(executeSql, query[i]);
			flag = true;
		}
		catch (ExecutionFailureException ex)
		{
			if (((SqlException)ex.InnerException).Class < 20 && sb.SqlPostfix.Length > 0)
			{
				try
				{
					executeSql.ExecuteImmediate(sb.SqlPostfix);
				}
				catch (ExecutionFailureException)
				{
				}
			}
			throw;
		}
		finally
		{
			if (dataProvider != null && !flag)
			{
				dataProvider.Close();
			}
		}
		return dataProvider;
	}

	public static DataTable ExecuteWithResults(StringCollection query, object con)
	{
		ExecuteSql executeSql = new ExecuteSql(con);
		return executeSql.Execute(query);
	}

	public static DataTable ExecuteWithResults(StringCollection query, object con, string dbName, bool poolConnection = true)
	{
		ExecuteSql executeSql;
		if (con is ServerConnection { CurrentDatabase: var currentDatabase } serverConnection)
		{
			if (!string.IsNullOrEmpty(currentDatabase) && string.Compare(currentDatabase, dbName, StringComparison.Ordinal) == 0)
			{
				executeSql = new ExecuteSql(con);
				return executeSql.Execute(query);
			}
			if (serverConnection.DatabaseEngineType == DatabaseEngineType.Standalone && serverConnection.NonPooledConnection)
			{
				executeSql = new ExecuteSql(con);
				executeSql.ExecuteImmediate(string.Format(CultureInfo.InvariantCulture, "use [{0}];", new object[1] { Util.EscapeString(dbName, ']') }));
				try
				{
					return executeSql.Execute(query);
				}
				finally
				{
					try
					{
						if (serverConnection.IsOpen)
						{
							executeSql.ExecuteImmediate(string.Format(CultureInfo.InvariantCulture, "use [{0}];", new object[1] { Util.EscapeString(currentDatabase, ']') }));
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}
		executeSql = new ExecuteSql(con, dbName, poolConnection);
		return executeSql.Execute(query);
	}

	private DataTable Execute(StringCollection query)
	{
		Connect();
		try
		{
			int i;
			for (i = 0; i < query.Count - 1; i++)
			{
				ExecuteImmediate(query[i]);
			}
			return ExecuteWithResults(query[i]);
		}
		finally
		{
			Disconnect();
		}
	}

	public static ServerVersion GetServerVersion(object con)
	{
		if (con is ServerVersion result)
		{
			return result;
		}
		if (con is ServerInformation serverInformation)
		{
			return serverInformation.ServerVersion;
		}
		return new ExecuteSql(con).GetServerVersion();
	}

	public static DatabaseEngineType GetDatabaseEngineType(object con)
	{
		if (con is int && Enum.IsDefined(typeof(DatabaseEngineType), con))
		{
			return (DatabaseEngineType)con;
		}
		if (con is ServerInformation serverInformation)
		{
			return serverInformation.DatabaseEngineType;
		}
		return new ExecuteSql(con).GetDatabaseEngineType();
	}

	public static DatabaseEngineEdition GetDatabaseEngineEdition(object con)
	{
		if (con is int && Enum.IsDefined(typeof(DatabaseEngineEdition), con))
		{
			return (DatabaseEngineEdition)con;
		}
		if (con is ServerInformation serverInformation)
		{
			return serverInformation.DatabaseEngineEdition;
		}
		return new ExecuteSql(con).GetDatabaseEngineEdition();
	}

	internal static bool IsContainedAuthentication(object con)
	{
		return new ExecuteSql(con).IsContainedAuthentication();
	}

	internal static bool GetIsDatabaseAccessibleNoThrow(object con, string databaseName)
	{
		bool result = false;
		try
		{
			result = GetDatabaseEngineType(con) == DatabaseEngineType.SqlAzureDatabase || bool.Parse(ExecuteWithResults("SELECT CASE WHEN has_dbaccess(N'" + Util.EscapeString(databaseName, '\'') + "') = 1 THEN 'true' ELSE 'false' END", con).Rows[0][0].ToString());
		}
		catch
		{
		}
		return result;
	}
}

using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class ExecuteSql
{
	private ServerConnection m_conctx;

	private bool bHasConnected;

	private ArrayList m_Messages;

	private SqlExecutionModes m_semInitial;

	private SqlInfoMessageEventHandler m_ServerInfoMessage;

	public ExecuteSql(object con)
	{
		bHasConnected = false;
		InitConnection(con);
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
		throw new InternalEnumeratorException(SfcStrings.InvalidConnectionType);
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
		return dataSet.Tables[0];
	}

	public SqlDataReader GetDataReader(string query)
	{
		SqlCommand command;
		return GetDataReader(query, out command);
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

	internal string GetHostPlatform()
	{
		return m_conctx.HostPlatform;
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
		executeSql.Connect();
		try
		{
			int i;
			for (i = 0; i < query.Count - 1; i++)
			{
				executeSql.ExecuteImmediate(query[i]);
			}
			return executeSql.ExecuteWithResults(query[i]);
		}
		finally
		{
			executeSql.Disconnect();
		}
	}

	public static ServerVersion GetServerVersion(object con)
	{
		return new ExecuteSql(con).GetServerVersion();
	}

	public static DatabaseEngineType GetDatabaseEngineType(object con)
	{
		return new ExecuteSql(con).GetDatabaseEngineType();
	}

	public static DatabaseEngineEdition GetDatabaseEngineEdition(object con)
	{
		return new ExecuteSql(con).GetDatabaseEngineEdition();
	}

	public static bool IsContainedAuthentication(object con)
	{
		return new ExecuteSql(con).IsContainedAuthentication();
	}

	public static string GetHostPlatform(object con)
	{
		return new ExecuteSql(con).GetHostPlatform();
	}
}

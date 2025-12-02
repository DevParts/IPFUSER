using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

public class ExecutionManager
{
	internal class ExecResult
	{
		private SqlError sqlError;

		private ServerMessageEventHandler serverMessageEventHandler;

		internal ExecResult()
		{
			serverMessageEventHandler = OnInfoMessage;
		}

		private void OnInfoMessage(object sender, ServerMessageEventArgs e)
		{
			if (sqlError == null && e.Error.Class > 10)
			{
				sqlError = e.Error;
			}
		}

		internal ServerMessageEventHandler GetEventHandler()
		{
			return serverMessageEventHandler;
		}

		internal SqlError GetError()
		{
			return sqlError;
		}
	}

	private ServerConnection m_conctx;

	private SqlSmoObject m_parent;

	private StringCollection m_ServerMessages;

	private EventHandler beforeExecuteSql;

	private ExecuteNonQueryCompletedEventHandler executeNonQueryCompleted;

	private AutoResetEvent asyncWaitHandle;

	public ServerConnection ConnectionContext => m_conctx;

	internal SqlSmoObject Parent
	{
		get
		{
			return m_parent;
		}
		set
		{
			m_parent = value;
		}
	}

	internal AutoResetEvent AsyncWaitHandle => asyncWaitHandle;

	internal bool Recording
	{
		get
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "recording: " + (SqlExecutionModes.ExecuteSql != (SqlExecutionModes.ExecuteSql & ConnectionContext.SqlExecutionModes)).ToString(SmoApplication.DefaultCulture));
			return SqlExecutionModes.ExecuteSql != (SqlExecutionModes.ExecuteSql & ConnectionContext.SqlExecutionModes);
		}
	}

	internal string TrueServerName => ConnectionContext.TrueName;

	internal event EventHandler BeforeExecuteSql
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				beforeExecuteSql = (EventHandler)Delegate.Combine(beforeExecuteSql, value);
			}
		}
		remove
		{
			beforeExecuteSql = (EventHandler)Delegate.Remove(beforeExecuteSql, value);
		}
	}

	internal event ExecuteNonQueryCompletedEventHandler ExecuteNonQueryCompleted
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				executeNonQueryCompleted = (ExecuteNonQueryCompletedEventHandler)Delegate.Combine(executeNonQueryCompleted, value);
			}
		}
		remove
		{
			executeNonQueryCompleted = (ExecuteNonQueryCompletedEventHandler)Delegate.Remove(executeNonQueryCompleted, value);
		}
	}

	internal ExecutionManager(string name)
	{
		m_conctx = new ServerConnection();
		m_conctx.ServerInstance = name;
		m_conctx.ApplicationName = ExceptionTemplatesImpl.SqlManagement;
	}

	internal ExecutionManager(ServerConnection cc)
	{
		m_conctx = cc;
	}

	internal DataTable GetEnumeratorData(Request req)
	{
		ConnectionContext.CheckDisconnected();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "get data for urn: " + req.Urn.ToString());
		return new Enumerator().Process(ConnectionContext, req);
	}

	internal IDataReader GetEnumeratorDataReader(Request req)
	{
		ConnectionContext.CheckDisconnected();
		req.ResultType = ResultType.IDataReader;
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "get data for urn: " + req.Urn.ToString());
		return EnumResult.ConvertToDataReader(Enumerator.GetData(ConnectionContext, req));
	}

	internal ObjectInfo GetEnumeratorInfo(RequestObjectInfo roi)
	{
		ConnectionContext.CheckDisconnected();
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "get object info for urn: " + roi.Urn.ToString());
		return new Enumerator().Process(ConnectionContext, roi);
	}

	internal DependencyChainCollection GetDependencies(DependencyRequest dependencyRequest)
	{
		ConnectionContext.CheckDisconnected();
		try
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "get dependencies ");
			return new Enumerator().EnumDependencies(ConnectionContext, dependencyRequest);
		}
		catch (ConnectionException innerException)
		{
			throw new SmoException(ExceptionTemplatesImpl.SqlInnerException, innerException);
		}
	}

	private void OnServerMessage(object sender, ServerMessageEventArgs e)
	{
		if (e.Error.Number != 5701)
		{
			m_ServerMessages.Add(e.Error.Message);
		}
	}

	internal void OnExecuteNonQueryCompleted(ExecuteNonQueryCompletedEventArgs args)
	{
		if (executeNonQueryCompleted != null)
		{
			executeNonQueryCompleted(this, args);
		}
	}

	internal void ExecuteNonQuery(StringCollection queries, ExecutionTypes executionType)
	{
		StringEnumerator enumerator = queries.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				DumpTraceString("execute sql: " + current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		ConnectionContext.ExecuteNonQuery(queries, executionType);
	}

	internal void ExecuteNonQuery(string sqlCommand, ExecutionTypes executionType)
	{
		DumpTraceString("execute sql: " + sqlCommand);
		ConnectionContext.ExecuteNonQuery(sqlCommand, executionType);
	}

	internal StringCollection ExecuteNonQueryWithMessage(StringCollection queries)
	{
		m_ServerMessages = new StringCollection();
		ServerMessageEventHandler dbccMessageHandler = OnServerMessage;
		ExecuteNonQueryWithMessage(queries, dbccMessageHandler);
		StringCollection serverMessages = m_ServerMessages;
		m_ServerMessages = null;
		return serverMessages;
	}

	internal void ExecuteNonQueryWithMessage(StringCollection queries, ServerMessageEventHandler dbccMessageHandler)
	{
		ExecuteNonQueryWithMessage(queries, dbccMessageHandler, errorsAsMessages: false);
	}

	internal void ExecuteNonQueryWithMessage(StringCollection queries, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages)
	{
		ExecuteNonQueryWithMessage(queries, dbccMessageHandler, errorsAsMessages, retry: true);
	}

	internal void ExecuteNonQueryWithMessage(StringCollection queries, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages, bool retry)
	{
		ExecResult execResult = null;
		bool fireInfoMessageEventOnUserErrors = false;
		ConnectionContext.ServerMessage += dbccMessageHandler;
		if (errorsAsMessages)
		{
			fireInfoMessageEventOnUserErrors = ConnectionContext.SqlConnectionObject.FireInfoMessageEventOnUserErrors;
			ConnectionContext.SqlConnectionObject.FireInfoMessageEventOnUserErrors = true;
			execResult = new ExecResult();
			ConnectionContext.ServerMessage += execResult.GetEventHandler();
		}
		try
		{
			ExecuteNonQuery(queries, retry);
		}
		finally
		{
			ConnectionContext.ServerMessage -= dbccMessageHandler;
			if (errorsAsMessages)
			{
				ConnectionContext.SqlConnectionObject.FireInfoMessageEventOnUserErrors = fireInfoMessageEventOnUserErrors;
				if (execResult != null)
				{
					ConnectionContext.ServerMessage -= execResult.GetEventHandler();
				}
			}
		}
		if (errorsAsMessages && execResult != null && execResult.GetError() != null)
		{
			throw new SmoException(execResult.GetError().ToString());
		}
	}

	internal DataSet ExecuteWithResultsAndMessages(string cmd, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages)
	{
		return ExecuteWithResultsAndMessages(cmd, dbccMessageHandler, errorsAsMessages, retry: true);
	}

	internal DataSet ExecuteWithResultsAndMessages(string cmd, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages, bool retry)
	{
		ExecResult execResult = null;
		bool fireInfoMessageEventOnUserErrors = false;
		DataSet dataSet = new DataSet();
		dataSet.Locale = CultureInfo.InvariantCulture;
		ConnectionContext.ServerMessage += dbccMessageHandler;
		if (errorsAsMessages)
		{
			fireInfoMessageEventOnUserErrors = ConnectionContext.SqlConnectionObject.FireInfoMessageEventOnUserErrors;
			ConnectionContext.SqlConnectionObject.FireInfoMessageEventOnUserErrors = true;
			execResult = new ExecResult();
			ConnectionContext.ServerMessage += execResult.GetEventHandler();
		}
		try
		{
			DumpTraceString("execute sql: " + cmd);
			dataSet = ConnectionContext.ExecuteWithResults(cmd, retry);
		}
		finally
		{
			ConnectionContext.ServerMessage -= dbccMessageHandler;
			if (errorsAsMessages)
			{
				ConnectionContext.SqlConnectionObject.FireInfoMessageEventOnUserErrors = fireInfoMessageEventOnUserErrors;
				if (execResult != null)
				{
					ConnectionContext.ServerMessage -= execResult.GetEventHandler();
				}
			}
		}
		if (errorsAsMessages && execResult != null && execResult.GetError() != null)
		{
			throw new SmoException(execResult.GetError().ToString());
		}
		return dataSet;
	}

	internal void ExecuteNonQueryWithMessageAsync(StringCollection queries, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages)
	{
		ExecuteNonQueryWithMessageAsync(queries, dbccMessageHandler, errorsAsMessages, retry: true);
	}

	internal void ExecuteNonQueryWithMessageAsync(StringCollection queries, ServerMessageEventHandler dbccMessageHandler, bool errorsAsMessages, bool retry)
	{
		asyncWaitHandle = new AutoResetEvent(initialState: false);
		ExecuteNonQueryThread executeNonQueryThread = new ExecuteNonQueryThread(this, queries, dbccMessageHandler, errorsAsMessages, retry);
		executeNonQueryThread.Start();
	}

	internal void ExecuteNonQuery(StringCollection queries)
	{
		ExecuteNonQuery(queries, retry: true);
	}

	internal void ExecuteNonQuery(StringCollection queries, bool retry)
	{
		if (beforeExecuteSql != null)
		{
			beforeExecuteSql(this, EventArgs.Empty);
		}
		StringEnumerator enumerator = queries.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				DumpTraceString("execute sql: " + current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		ConnectionContext.ExecuteNonQuery(queries, ExecutionTypes.NoCommands, retry);
	}

	internal void ExecuteNonQueryAsync(StringCollection queries)
	{
		ExecuteNonQueryAsync(queries, retry: true);
	}

	internal void ExecuteNonQueryAsync(StringCollection queries, bool retry)
	{
		asyncWaitHandle = new AutoResetEvent(initialState: false);
		ExecuteNonQueryThread executeNonQueryThread = new ExecuteNonQueryThread(this, queries, null, errorsAsMessages: false, retry);
		executeNonQueryThread.Start();
	}

	internal void ExecuteNonQuery(string cmd)
	{
		ExecuteNonQuery(cmd, retry: true);
	}

	internal void ExecuteNonQuery(string cmd, bool retry)
	{
		DumpTraceString("execute sql: " + cmd);
		if (beforeExecuteSql != null)
		{
			beforeExecuteSql(this, EventArgs.Empty);
		}
		ConnectionContext.ExecuteNonQuery(cmd, ExecutionTypes.NoCommands, retry);
	}

	internal DataSet ExecuteWithResults(StringCollection query)
	{
		StringEnumerator enumerator = query.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				DumpTraceString("execute sql: " + current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		DataSet[] array = ConnectionContext.ExecuteWithResults(query);
		DataSet dataSet = new DataSet();
		dataSet.Locale = CultureInfo.InvariantCulture;
		DataSet[] array2 = array;
		foreach (DataSet dataSet2 in array2)
		{
			DataTable[] array3 = new DataTable[dataSet2.Tables.Count];
			dataSet2.Tables.CopyTo(array3, 0);
			DataTable[] array4 = array3;
			foreach (DataTable table in array4)
			{
				dataSet2.Tables.Remove(table);
				dataSet.Tables.Add(table);
			}
		}
		return dataSet;
	}

	internal DataSet ExecuteWithResults(string query)
	{
		return ExecuteWithResults(query, retry: true);
	}

	internal DataSet ExecuteWithResults(string query, bool retry)
	{
		DumpTraceString("execute sql: " + query);
		return ConnectionContext.ExecuteWithResults(query, retry);
	}

	internal object[] ExecuteScalar(StringCollection query)
	{
		StringEnumerator enumerator = query.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				DumpTraceString("execute sql: " + current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		return ConnectionContext.ExecuteScalar(query);
	}

	internal object ExecuteScalar(string query)
	{
		DumpTraceString("execute sql: " + query);
		return ConnectionContext.ExecuteScalar(query);
	}

	internal Version GetProductVersion()
	{
		return ConnectionContext.ProductVersion;
	}

	internal ServerVersion GetServerVersion()
	{
		ServerVersion serverVersion = ConnectionContext.ServerVersion;
		if (serverVersion == null)
		{
			if (Parent.IsDesignMode)
			{
				throw new SfcDesignModeException(ExceptionTemplatesImpl.ServerVersionNotSpecified);
			}
			ConnectionContext.Connect();
			serverVersion = ConnectionContext.ServerVersion;
			ConnectionContext.Disconnect();
		}
		return serverVersion;
	}

	internal DatabaseEngineType GetDatabaseEngineType()
	{
		return ConnectionContext.DatabaseEngineType;
	}

	internal DatabaseEngineEdition GetDatabaseEngineEdition()
	{
		return ConnectionContext.DatabaseEngineEdition;
	}

	internal NetworkProtocol GetConnectionProtocol()
	{
		return ConnectionContext.ConnectionProtocol;
	}

	internal bool IsCurrentConnectionStandardLogin(string name)
	{
		if (!ConnectionContext.LoginSecure && ConnectionContext.Login == name)
		{
			return true;
		}
		return false;
	}

	internal void Abort()
	{
		ConnectionContext.Cancel();
	}

	private void DumpTraceString(string s)
	{
		if (s.ToLower(SmoApplication.DefaultCulture).Contains("password"))
		{
			s = "This statement contains sensitive information and has been replaced for security reasons.";
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, s);
	}
}

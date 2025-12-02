using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public static class ConnectionHelpers
{
	private static string GetSqlDatabaseName(this Urn urn)
	{
		do
		{
			if (urn.Type == "Database" || urn.Type == "DatabaseXEStore")
			{
				return urn.GetAttribute("Name");
			}
			urn = urn.Parent;
		}
		while (urn != null);
		return null;
	}

	internal static bool UpdateConnectionInfoIfCloud(ref object connectionInfo, Urn urn)
	{
		ServerConnection serverConnection = GetServerConnection(connectionInfo);
		bool result = false;
		if (serverConnection != null && serverConnection.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			string sqlDatabaseName = urn.GetSqlDatabaseName();
			if (sqlDatabaseName != null)
			{
				IComparer<string> serverComparer = ServerConnection.ConnectionFactory.GetInstance(serverConnection).ServerComparer;
				if (serverComparer.Compare(sqlDatabaseName, serverConnection.DatabaseName) != 0)
				{
					connectionInfo = serverConnection.GetDatabaseConnection(sqlDatabaseName);
					result = true;
				}
			}
		}
		return result;
	}

	internal static void UpdateConnectionInfoIfContainedAuthentication(ref object connectionInfo, Urn urn)
	{
		ServerConnection serverConnection = GetServerConnection(connectionInfo);
		if (serverConnection == null || !serverConnection.IsContainedAuthentication)
		{
			return;
		}
		string text = urn.GetSqlDatabaseName();
		if (text == null)
		{
			text = (string.IsNullOrEmpty(serverConnection.InitialCatalog) ? "master" : serverConnection.InitialCatalog);
		}
		IComparer<string> comparer = new ServerComparer(serverConnection);
		if (comparer.Compare(text, serverConnection.CurrentDatabase) == 0)
		{
			return;
		}
		SqlExecutionModes sqlExecutionModes = serverConnection.SqlExecutionModes;
		serverConnection.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
		try
		{
			serverConnection.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "use [{0}]", new object[1] { Util.EscapeString(text, ']') }));
		}
		catch (Exception ex)
		{
			if (ex.InnerException is SqlException ex2 && (ex2.Number == 916 || ex2.Number == 911))
			{
				TraceHelper.LogExCatch(ex);
				return;
			}
			throw ex;
		}
		finally
		{
			if (serverConnection.SqlExecutionModes != sqlExecutionModes)
			{
				serverConnection.SqlExecutionModes = sqlExecutionModes;
			}
		}
	}

	private static ServerConnection GetServerConnection(object connectionInfo)
	{
		ServerConnection serverConnection = null;
		if (connectionInfo is SqlConnectionInfoWithConnection sqlConnectionInfoWithConnection)
		{
			serverConnection = sqlConnectionInfoWithConnection.ServerConnection;
		}
		if (serverConnection == null)
		{
			serverConnection = connectionInfo as ServerConnection;
		}
		return serverConnection;
	}

	public static ServerConnection ToScopedServerConnection(this SqlConnection sqlConnection, Urn urn)
	{
		object connectionInfo = new ServerConnection(sqlConnection);
		UpdateConnectionInfoIfCloud(ref connectionInfo, urn);
		return (ServerConnection)connectionInfo;
	}
}

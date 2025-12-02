using System;
using System.Data.SqlClient;
using System.Reflection;

namespace Microsoft.SqlServer.Management.Common;

public static class ConnectionInfoHelper
{
	private static PropertyInfo _accessTokenPropertyInfo;

	static ConnectionInfoHelper()
	{
		_accessTokenPropertyInfo = GetAccessTokenProperty();
	}

	public static void SetTokenOnConnection(SqlConnection conn, string accessToken)
	{
		CheckForNull(conn);
		if (_accessTokenPropertyInfo != null)
		{
			_accessTokenPropertyInfo.SetValue(conn, accessToken, null);
		}
	}

	public static string GetTokenFromSqlConnection(SqlConnection conn)
	{
		CheckForNull(conn);
		if (_accessTokenPropertyInfo != null)
		{
			return (string)_accessTokenPropertyInfo.GetValue(conn, null);
		}
		return string.Empty;
	}

	public static SqlConnection CreateSqlConnection(SqlConnectionInfo connectionInfo)
	{
		SqlConnection sqlConnection = new SqlConnection(connectionInfo.ConnectionString);
		if (connectionInfo.AccessToken != null)
		{
			SetTokenOnConnection(sqlConnection, connectionInfo.AccessToken.GetAccessToken());
		}
		return sqlConnection;
	}

	private static PropertyInfo GetAccessTokenProperty()
	{
		Assembly assembly = Assembly.GetAssembly(typeof(SqlConnectionStringBuilder));
		Type type = assembly.GetType("System.Data.SqlClient.SqlConnection");
		type.GetProperties();
		return type.GetProperty("AccessToken");
	}

	private static void CheckForNull(SqlConnection conn)
	{
		if (conn == null)
		{
			throw new ArgumentNullException("conn");
		}
	}
}

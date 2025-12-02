using System;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public class SqlStoreConnection : SfcConnection
{
	private ServerConnection sqlServerConnection;

	public override bool IsOpen => sqlServerConnection.IsOpen;

	public override string ServerInstance
	{
		get
		{
			return sqlServerConnection.ServerInstance;
		}
		set
		{
			sqlServerConnection.ServerInstance = value;
		}
	}

	public override Version ServerVersion
	{
		get
		{
			return ((ISfcConnection)sqlServerConnection).ServerVersion;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public override ServerType ConnectionType => ServerType.DatabaseEngine;

	public override int ConnectTimeout
	{
		get
		{
			return sqlServerConnection.ConnectTimeout;
		}
		set
		{
			sqlServerConnection.ConnectTimeout = value;
		}
	}

	public override int StatementTimeout
	{
		get
		{
			return sqlServerConnection.StatementTimeout;
		}
		set
		{
			sqlServerConnection.StatementTimeout = value;
		}
	}

	public ServerConnection ServerConnection => sqlServerConnection;

	public SqlStoreConnection(SqlConnection sqlConnection)
	{
		sqlServerConnection = new ServerConnection(sqlConnection);
	}

	private SqlStoreConnection(ServerConnection connection)
	{
		sqlServerConnection = connection;
	}

	public override object ToEnumeratorObject()
	{
		return ServerConnection;
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	public override bool Equals(SfcConnection conn)
	{
		if (!(conn is SqlStoreConnection sqlStoreConnection))
		{
			return false;
		}
		return ToString() == sqlStoreConnection.ToString();
	}

	public override bool Connect()
	{
		if (!IsOpen)
		{
			sqlServerConnection.Connect();
		}
		return IsOpen;
	}

	public override bool Disconnect()
	{
		sqlServerConnection.Disconnect();
		return !IsOpen;
	}

	public override ISfcConnection Copy()
	{
		return new SqlStoreConnection(ServerConnection.Copy());
	}
}

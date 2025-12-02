using System;
using System.ComponentModel;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Common;

[Serializable]
public sealed class SqlConnectionInfoWithConnection : SqlConnectionInfo, IDisposable, IRestrictedAccess
{
	[NonSerialized]
	private ServerConnection serverConnection;

	[NonSerialized]
	private bool closeConnectionOnDispose;

	[NonSerialized]
	private bool singleConnection;

	private EventHandler connectionClosedHandler;

	[Browsable(false)]
	public ServerConnection ServerConnection
	{
		get
		{
			if (serverConnection == null)
			{
				serverConnection = new ServerConnection(this);
				serverConnection.NonPooledConnection = true;
				serverConnection.AutoDisconnectMode = AutoDisconnectMode.NoAutoDisconnect;
				closeConnectionOnDispose = true;
			}
			IRenewableToken accessToken = base.AccessToken;
			if (accessToken != null)
			{
				serverConnection.AccessToken = accessToken;
			}
			return serverConnection;
		}
		set
		{
			serverConnection = value;
			closeConnectionOnDispose = false;
			ConnectionParmsChanged();
		}
	}

	public bool SingleConnection
	{
		get
		{
			return singleConnection;
		}
		set
		{
			singleConnection = value;
		}
	}

	public event EventHandler ConnectionClosed
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				connectionClosedHandler = (EventHandler)Delegate.Combine(connectionClosedHandler, value);
			}
		}
		remove
		{
			connectionClosedHandler = (EventHandler)Delegate.Remove(connectionClosedHandler, value);
		}
	}

	public SqlConnectionInfoWithConnection()
	{
	}

	public void Dispose()
	{
		if (closeConnectionOnDispose && serverConnection != null)
		{
			serverConnection.Disconnect();
			OnConnectionClosed(EventArgs.Empty);
		}
		serverConnection = null;
	}

	public SqlConnectionInfoWithConnection(string serverName)
		: base(serverName)
	{
	}

	public SqlConnectionInfoWithConnection(string serverName, string userName, string password)
		: base(serverName, userName, password)
	{
	}

	public SqlConnectionInfoWithConnection(SqlConnection sqlConnection)
		: base(sqlConnection.DataSource)
	{
		serverConnection = new ServerConnection(sqlConnection);
		closeConnectionOnDispose = false;
	}

	private SqlConnectionInfoWithConnection(SqlConnectionInfoWithConnection conn)
		: base(conn)
	{
		singleConnection = conn.singleConnection;
		if (singleConnection)
		{
			serverConnection = conn.serverConnection;
			closeConnectionOnDispose = false;
		}
	}

	public new SqlConnectionInfoWithConnection Copy()
	{
		return new SqlConnectionInfoWithConnection(this);
	}

	protected override void ConnectionParmsChanged()
	{
	}

	private void OnConnectionClosed(EventArgs args)
	{
		if (connectionClosedHandler != null)
		{
			connectionClosedHandler(this, args);
		}
	}
}

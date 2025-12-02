using System;
using System.Data.SqlClient;

namespace Microsoft.SqlServer.Management.Common;

public class ServerMessageEventArgs : EventArgs
{
	private SqlError error;

	public SqlError Error => error;

	public ServerMessageEventArgs(SqlError sqlError)
	{
		error = sqlError;
	}

	public override string ToString()
	{
		return error.ToString();
	}
}

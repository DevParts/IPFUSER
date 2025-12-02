using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public sealed class SfcTSqlExecutor
{
	public enum ExecutionMode
	{
		Scalar,
		NonQuery,
		WithResults
	}

	private SfcTSqlExecutor()
	{
	}

	internal static object Execute(ServerConnection connection, string script, ExecutionMode mode)
	{
		return Execute(connection, script, mode, ExecutionTypes.NoCommands);
	}

	internal static object Execute(ServerConnection connection, string script, ExecutionMode mode, ExecutionTypes type)
	{
		try
		{
			switch (mode)
			{
			case ExecutionMode.Scalar:
				return connection.ExecuteScalar(script);
			case ExecutionMode.NonQuery:
				connection.ExecuteNonQuery(script, type);
				return null;
			case ExecutionMode.WithResults:
				return connection.ExecuteWithResults(script);
			default:
				TraceHelper.Assert(condition: false, "Unknown ExecutionMode supplied");
				return null;
			}
		}
		catch (ExecutionFailureException ex)
		{
			if (ex.InnerException is SqlException)
			{
				SqlException ex2 = (SqlException)ex.InnerException;
				if (ex2.Number == 229)
				{
					throw new SfcSecurityException(SfcStrings.PermissionDenied, ex2);
				}
			}
			throw;
		}
	}

	public static object ExecuteScalar(ServerConnection connection, string script)
	{
		return Execute(connection, script, ExecutionMode.Scalar);
	}

	public static void ExecuteNonQuery(ServerConnection connection, string script)
	{
		Execute(connection, script, ExecutionMode.NonQuery);
	}
}

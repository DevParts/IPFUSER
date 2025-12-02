using System.Data.SqlClient;

namespace Microsoft.SqlServer.Management.Common;

internal class SqlBatch : CacheItem<string>
{
	public SqlCommand Command;

	public override string Key => Command.CommandText;

	public SqlBatch(SqlCommand command)
	{
		Command = command;
	}
}

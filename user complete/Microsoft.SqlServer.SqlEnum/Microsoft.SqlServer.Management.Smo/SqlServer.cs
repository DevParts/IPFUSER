using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class SqlServer : SqlObject, ISupportVersions, ISupportDatabaseEngineTypes, ISupportDatabaseEngineEditions
{
	public override EnumResult GetData(EnumResult erParent)
	{
		base.Filter = null;
		if (base.Request == null)
		{
			base.StatementBuilder = new StatementBuilder();
			base.ConditionedSqlList.AddHits(this, string.Empty, base.StatementBuilder);
			DatabaseEngineType databaseEngineType = ExecuteSql.GetDatabaseEngineType(base.ConnectionInfo);
			return new SqlEnumResult(base.StatementBuilder, ResultType.Reserved1, databaseEngineType);
		}
		return base.GetData(erParent);
	}

	public ServerVersion GetServerVersion(object conn)
	{
		return ExecuteSql.GetServerVersion(conn);
	}

	public DatabaseEngineType GetDatabaseEngineType(object conn)
	{
		return ExecuteSql.GetDatabaseEngineType(conn);
	}

	public DatabaseEngineEdition GetDatabaseEngineEdition(object conn)
	{
		return ExecuteSql.GetDatabaseEngineEdition(conn);
	}
}

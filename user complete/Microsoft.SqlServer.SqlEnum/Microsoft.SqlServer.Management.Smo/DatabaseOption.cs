using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class DatabaseOption : SqlObject
{
	public override Request RetrieveParentRequest()
	{
		SqlRequest sqlRequest = (SqlRequest)base.RetrieveParentRequest();
		if (sqlRequest == null)
		{
			sqlRequest = new SqlRequest();
		}
		sqlRequest.ResolveDatabases = false;
		return sqlRequest;
	}
}

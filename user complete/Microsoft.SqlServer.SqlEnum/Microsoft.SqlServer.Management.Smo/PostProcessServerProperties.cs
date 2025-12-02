namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessServerProperties : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (name == "ServerType")
		{
			return ExecuteSql.GetDatabaseEngineType(base.ConnectionInfo);
		}
		if (name == "IsContainedAuthentication")
		{
			return ExecuteSql.IsContainedAuthentication(base.ConnectionInfo);
		}
		return data;
	}
}

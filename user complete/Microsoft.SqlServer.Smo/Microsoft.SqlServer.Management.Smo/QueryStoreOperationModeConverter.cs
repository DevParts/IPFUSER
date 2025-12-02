namespace Microsoft.SqlServer.Management.Smo;

public class QueryStoreOperationModeConverter : EnumToDisplayNameConverter
{
	public QueryStoreOperationModeConverter()
		: base(typeof(QueryStoreOperationMode))
	{
	}
}

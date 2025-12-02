namespace Microsoft.SqlServer.Management.Smo;

public class QueryStoreSizeBasedCleanupModeConverter : EnumToDisplayNameConverter
{
	public QueryStoreSizeBasedCleanupModeConverter()
		: base(typeof(QueryStoreSizeBasedCleanupMode))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

public class QueryStoreCaptureModeConverter : EnumToDisplayNameConverter
{
	public QueryStoreCaptureModeConverter()
		: base(typeof(QueryStoreCaptureMode))
	{
	}
}

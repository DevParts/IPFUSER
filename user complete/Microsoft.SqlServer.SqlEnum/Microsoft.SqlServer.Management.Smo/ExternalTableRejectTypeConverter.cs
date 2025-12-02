namespace Microsoft.SqlServer.Management.Smo;

public class ExternalTableRejectTypeConverter : EnumToDisplayNameConverter
{
	public ExternalTableRejectTypeConverter()
		: base(typeof(ExternalTableRejectType))
	{
	}
}

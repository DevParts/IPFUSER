namespace Microsoft.SqlServer.Management.Smo;

public class ExternalDataSourceTypeConverter : EnumToDisplayNameConverter
{
	public ExternalDataSourceTypeConverter()
		: base(typeof(ExternalDataSourceType))
	{
	}
}

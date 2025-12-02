namespace Microsoft.SqlServer.Management.Smo;

public class ExternalTableDistributionConverter : EnumToDisplayNameConverter
{
	public ExternalTableDistributionConverter()
		: base(typeof(ExternalTableDistributionType))
	{
	}
}

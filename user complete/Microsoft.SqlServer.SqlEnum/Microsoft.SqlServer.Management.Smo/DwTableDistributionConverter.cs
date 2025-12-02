namespace Microsoft.SqlServer.Management.Smo;

public class DwTableDistributionConverter : EnumToDisplayNameConverter
{
	public DwTableDistributionConverter()
		: base(typeof(DwTableDistributionType))
	{
	}
}

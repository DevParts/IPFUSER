namespace Microsoft.SqlServer.Management.Smo;

internal class HADRManagerStatusConverter : EnumToDisplayNameConverter
{
	public HADRManagerStatusConverter()
		: base(typeof(HadrManagerStatus))
	{
	}
}

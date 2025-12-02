namespace Microsoft.SqlServer.Management.Smo;

internal class MirroringStatusConverter : EnumToDisplayNameConverter
{
	public MirroringStatusConverter()
		: base(typeof(MirroringStatus))
	{
	}
}

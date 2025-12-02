namespace Microsoft.SqlServer.Management.Smo;

internal class RecoveryModelConverter : EnumToDisplayNameConverter
{
	public RecoveryModelConverter()
		: base(typeof(RecoveryModel))
	{
	}
}

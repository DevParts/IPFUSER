namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaSeedingModeConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaSeedingModeConverter()
		: base(typeof(AvailabilityReplicaSeedingMode))
	{
	}
}

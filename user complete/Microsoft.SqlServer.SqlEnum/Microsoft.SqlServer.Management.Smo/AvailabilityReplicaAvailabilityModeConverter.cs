namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaAvailabilityModeConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaAvailabilityModeConverter()
		: base(typeof(AvailabilityReplicaAvailabilityMode))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaFailoverModeConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaFailoverModeConverter()
		: base(typeof(AvailabilityReplicaFailoverMode))
	{
	}
}

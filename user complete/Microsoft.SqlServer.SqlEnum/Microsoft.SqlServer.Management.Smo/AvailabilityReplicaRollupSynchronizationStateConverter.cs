namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaRollupSynchronizationStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaRollupSynchronizationStateConverter()
		: base(typeof(AvailabilityReplicaRollupSynchronizationState))
	{
	}
}

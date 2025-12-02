namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaRollupRecoveryStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaRollupRecoveryStateConverter()
		: base(typeof(AvailabilityReplicaRollupRecoveryState))
	{
	}
}

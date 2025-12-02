namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupRollupSynchronizationStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityGroupRollupSynchronizationStateConverter()
		: base(typeof(AvailabilityGroupRollupSynchronizationState))
	{
	}
}

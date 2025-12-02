namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityDatabaseSynchronizationStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityDatabaseSynchronizationStateConverter()
		: base(typeof(AvailabilityDatabaseSynchronizationState))
	{
	}
}

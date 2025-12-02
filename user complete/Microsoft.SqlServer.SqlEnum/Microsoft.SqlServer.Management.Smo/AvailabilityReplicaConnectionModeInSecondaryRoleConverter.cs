namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaConnectionModeInSecondaryRoleConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaConnectionModeInSecondaryRoleConverter()
		: base(typeof(AvailabilityReplicaConnectionModeInSecondaryRole))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaConnectionModeInPrimaryRoleConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaConnectionModeInPrimaryRoleConverter()
		: base(typeof(AvailabilityReplicaConnectionModeInPrimaryRole))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaRoleConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaRoleConverter()
		: base(typeof(AvailabilityReplicaRole))
	{
	}
}

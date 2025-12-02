namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaConnectionStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaConnectionStateConverter()
		: base(typeof(AvailabilityReplicaConnectionState))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityReplicaOperationalStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityReplicaOperationalStateConverter()
		: base(typeof(AvailabilityReplicaOperationalState))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupClusterTypeConverter : EnumToDisplayNameConverter
{
	public AvailabilityGroupClusterTypeConverter()
		: base(typeof(AvailabilityGroupClusterType))
	{
	}
}

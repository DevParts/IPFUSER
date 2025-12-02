namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupListenerIPStateConverter : EnumToDisplayNameConverter
{
	public AvailabilityGroupListenerIPStateConverter()
		: base(typeof(AvailabilityGroupListenerIPState))
	{
	}
}

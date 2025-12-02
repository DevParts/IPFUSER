namespace Microsoft.SqlServer.Management.Smo;

internal class AvailabilityGroupFailureConditionLevelConverter : EnumToDisplayNameConverter
{
	public AvailabilityGroupFailureConditionLevelConverter()
		: base(typeof(AvailabilityGroupFailureConditionLevel))
	{
	}
}

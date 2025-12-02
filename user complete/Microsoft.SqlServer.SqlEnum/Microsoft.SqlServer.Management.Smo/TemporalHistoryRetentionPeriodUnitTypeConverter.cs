namespace Microsoft.SqlServer.Management.Smo;

public class TemporalHistoryRetentionPeriodUnitTypeConverter : EnumToDisplayNameConverter
{
	public TemporalHistoryRetentionPeriodUnitTypeConverter()
		: base(typeof(TemporalHistoryRetentionPeriodUnit))
	{
	}
}

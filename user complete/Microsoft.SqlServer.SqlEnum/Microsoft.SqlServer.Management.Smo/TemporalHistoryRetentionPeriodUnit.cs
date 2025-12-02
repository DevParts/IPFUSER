using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(TemporalHistoryRetentionPeriodUnitTypeConverter))]
public enum TemporalHistoryRetentionPeriodUnit
{
	[TsqlSyntaxString("UNDEFINED")]
	[LocDisplayName("Undefined")]
	Undefined = -2,
	[LocDisplayName("Infinite")]
	[TsqlSyntaxString("INFINITE")]
	Infinite = -1,
	[TsqlSyntaxString("DAY")]
	[LocDisplayName("Day")]
	Day = 3,
	[LocDisplayName("Week")]
	[TsqlSyntaxString("WEEK")]
	Week = 4,
	[TsqlSyntaxString("MONTH")]
	[LocDisplayName("Month")]
	Month = 5,
	[LocDisplayName("Year")]
	[TsqlSyntaxString("YEAR")]
	Year = 6
}

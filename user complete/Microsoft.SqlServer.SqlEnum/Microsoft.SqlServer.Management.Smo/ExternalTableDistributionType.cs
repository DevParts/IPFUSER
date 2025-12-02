using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ExternalTableDistributionConverter))]
public enum ExternalTableDistributionType
{
	[TsqlSyntaxString("SHARDED")]
	Sharded = 0,
	[TsqlSyntaxString("REPLICATED")]
	Replicated = 1,
	[TsqlSyntaxString("ROUND_ROBIN")]
	RoundRobin = 2,
	None = 255
}

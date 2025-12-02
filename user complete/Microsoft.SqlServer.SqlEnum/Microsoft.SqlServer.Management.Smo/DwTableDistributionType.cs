using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(DwTableDistributionConverter))]
public enum DwTableDistributionType
{
	Undefined,
	None,
	[TsqlSyntaxString("HASH")]
	Hash,
	[TsqlSyntaxString("REPLICATE")]
	Replicate,
	[TsqlSyntaxString("ROUND_ROBIN")]
	RoundRobin
}

using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(SecurityPredicateTypeConverter))]
public enum SecurityPredicateType
{
	[TsqlSyntaxString("FILTER")]
	[LocDisplayName("securityPredicateTypeFilter")]
	Filter,
	[TsqlSyntaxString("BLOCK")]
	[LocDisplayName("securityPredicateTypeBlock")]
	Block
}

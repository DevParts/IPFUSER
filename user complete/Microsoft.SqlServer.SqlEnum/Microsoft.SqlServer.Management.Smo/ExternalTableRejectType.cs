using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ExternalTableRejectTypeConverter))]
public enum ExternalTableRejectType
{
	[TsqlSyntaxString("VALUE")]
	Value = 0,
	[TsqlSyntaxString("PERCENTAGE")]
	Percentage = 1,
	None = 255
}

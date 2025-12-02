using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(RangeTypeConverter))]
public enum RangeType
{
	[TsqlSyntaxString("")]
	None = -1,
	[TsqlSyntaxString("LEFT")]
	Left,
	[TsqlSyntaxString("RIGHT")]
	Right
}

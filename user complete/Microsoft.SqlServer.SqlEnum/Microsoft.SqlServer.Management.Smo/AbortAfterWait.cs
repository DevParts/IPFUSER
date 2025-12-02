using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AbortAfterWaitConverter))]
public enum AbortAfterWait
{
	[TsqlSyntaxString("NONE")]
	None,
	[TsqlSyntaxString("BLOCKERS")]
	Blockers,
	[TsqlSyntaxString("SELF")]
	Self
}

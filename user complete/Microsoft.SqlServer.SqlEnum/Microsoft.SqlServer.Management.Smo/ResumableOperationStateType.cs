using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ResumableOperationStateTypeConverter))]
public enum ResumableOperationStateType
{
	[LocDisplayName("ResumableOperationStateTypeRunning")]
	[TsqlSyntaxString("RUNNING")]
	Running,
	[TsqlSyntaxString("PAUSED")]
	[LocDisplayName("ResumableOperationStateTypePaused")]
	Paused,
	[TsqlSyntaxString("NONE")]
	[LocDisplayName("ResumableOperationStateTypeNone")]
	None
}

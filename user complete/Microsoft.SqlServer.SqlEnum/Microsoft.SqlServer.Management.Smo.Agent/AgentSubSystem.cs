using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[TypeConverter(typeof(AgentSubSystemTypeConverter))]
public enum AgentSubSystem
{
	[LocDisplayName("TransactSql")]
	[TsqlSyntaxString("TSQL")]
	TransactSql = 1,
	[TsqlSyntaxString("ActiveScripting")]
	[LocDisplayName("ActiveScripting")]
	ActiveScripting,
	[LocDisplayName("CmdExec")]
	[TsqlSyntaxString("CmdExec")]
	CmdExec,
	[LocDisplayName("ReplSnapshot")]
	[TsqlSyntaxString("Snapshot")]
	Snapshot,
	[TsqlSyntaxString("LogReader")]
	[LocDisplayName("ReplLogReader")]
	LogReader,
	[LocDisplayName("ReplDistribution")]
	[TsqlSyntaxString("Distribution")]
	Distribution,
	[TsqlSyntaxString("Merge")]
	[LocDisplayName("ReplMerge")]
	Merge,
	[TsqlSyntaxString("QueueReader")]
	[LocDisplayName("ReplQueueReader")]
	QueueReader,
	[TsqlSyntaxString("ANALYSISQUERY")]
	[LocDisplayName("AnalysisQuery")]
	AnalysisQuery,
	[LocDisplayName("AnalysisCommand")]
	[TsqlSyntaxString("ANALYSISCOMMAND")]
	AnalysisCommand,
	[TsqlSyntaxString("SSIS")]
	[LocDisplayName("SSIS")]
	Ssis,
	[LocDisplayName("PowerShell")]
	[TsqlSyntaxString("PowerShell")]
	PowerShell
}

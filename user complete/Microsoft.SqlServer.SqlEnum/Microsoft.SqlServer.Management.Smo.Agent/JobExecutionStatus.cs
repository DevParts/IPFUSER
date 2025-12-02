using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[TypeConverter(typeof(JobExecutionStatusConverter))]
public enum JobExecutionStatus
{
	[LocDisplayName("Executing")]
	Executing = 1,
	[LocDisplayName("WaitingForWorkerThread")]
	WaitingForWorkerThread,
	[LocDisplayName("BetweenRetries")]
	BetweenRetries,
	[LocDisplayName("Idle")]
	Idle,
	[LocDisplayName("Suspended")]
	Suspended,
	[LocDisplayName("WaitingForStepToFinish")]
	WaitingForStepToFinish,
	[LocDisplayName("PerformingCompletionAction")]
	PerformingCompletionAction
}

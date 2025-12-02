using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum JobStepFlags
{
	None = 0,
	AppendToLogFile = 2,
	AppendToJobHistory = 4,
	LogToTableWithOverwrite = 8,
	AppendToTableLog = 0x10,
	AppendAllCmdExecOutputToJobHistory = 0x20,
	ProvideStopProcessEvent = 0x40
}

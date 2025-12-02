using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum AgentLogLevels
{
	Errors = 1,
	Warnings = 2,
	Informational = 4,
	All = 7
}

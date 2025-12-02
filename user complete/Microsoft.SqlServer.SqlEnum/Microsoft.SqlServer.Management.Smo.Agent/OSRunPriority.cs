namespace Microsoft.SqlServer.Management.Smo.Agent;

public enum OSRunPriority
{
	AboveNormal = 1,
	BelowNormal = -1,
	Idle = -15,
	Normal = 0,
	TimeCritical = 15
}

namespace Microsoft.SqlServer.Management.Smo.Agent;

public enum AlertType
{
	SqlServerEvent = 1,
	SqlServerPerformanceCondition,
	NonSqlServerEvent,
	WmiEvent
}

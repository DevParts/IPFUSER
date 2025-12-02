namespace Microsoft.SqlServer.Management.Smo.Agent;

public enum CompletionResult
{
	Failed,
	Succeeded,
	Retry,
	Cancelled,
	InProgress,
	Unknown
}

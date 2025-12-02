namespace Microsoft.SqlServer.Management.Smo;

public enum DatabaseDdlTriggerExecutionContext
{
	Caller = 1,
	ExecuteAsUser,
	Self
}

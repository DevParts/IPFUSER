namespace Microsoft.SqlServer.Management.Smo;

public enum ServerDdlTriggerExecutionContext
{
	Caller = 1,
	ExecuteAsLogin,
	Self
}

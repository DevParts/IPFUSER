namespace Microsoft.SqlServer.Management.Smo;

public enum ExecutionContext
{
	Caller = 1,
	Owner,
	ExecuteAsUser,
	Self
}

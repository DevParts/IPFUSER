namespace Microsoft.SqlServer.Management.Smo;

internal sealed class UserDefinedFunctionEventsWorker : ObjectInSchemaEventsWorker
{
	protected override string ObjectType => "Function";

	public UserDefinedFunctionEventsWorker(UserDefinedFunction target)
		: base(target, typeof(UserDefinedFunctionEventSet), typeof(UserDefinedFunctionEventValues))
	{
	}
}

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class StoredProcedureEventsWorker : ObjectInSchemaEventsWorker
{
	protected override string ObjectType => "Procedure";

	public StoredProcedureEventsWorker(StoredProcedure target)
		: base(target, typeof(StoredProcedureEventSet), typeof(StoredProcedureEventValues))
	{
	}
}

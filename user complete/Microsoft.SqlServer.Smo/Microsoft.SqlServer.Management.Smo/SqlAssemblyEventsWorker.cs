using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class SqlAssemblyEventsWorker : EventsWorkerBase
{
	private SqlAssembly target;

	protected override SqlSmoObject Target => target;

	public SqlAssemblyEventsWorker(SqlAssembly target)
		: base(target, typeof(SqlAssemblyEventSet), typeof(SqlAssemblyEventValues))
	{
		this.target = target;
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		return EventsWorkerBase.CreateWqlQueryForDatabaseObject(eventClass, target.ParentColl.ParentInstance.InternalName, target.Name, "Assembly");
	}
}

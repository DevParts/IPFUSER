using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class DatabaseEventsWorker : EventsWorkerBase
{
	private Database target;

	protected override SqlSmoObject Target => target;

	public DatabaseEventsWorker(Database target)
		: base(target, typeof(DatabaseEventSet), typeof(DatabaseEventValues))
	{
		this.target = target;
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		return EventsWorkerBase.CreateWqlQueryForDatabase(eventClass, target.Name);
	}
}

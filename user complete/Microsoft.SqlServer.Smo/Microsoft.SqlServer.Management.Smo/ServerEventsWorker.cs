using System;
using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class ServerEventsWorker : EventsWorkerBase
{
	private Server target;

	protected override SqlSmoObject Target => target;

	public ServerEventsWorker(Server target, Type eventSetType, Type eventEnumType)
		: base(target, eventSetType, eventEnumType)
	{
		this.target = target;
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		return EventsWorkerBase.CreateWqlQueryForServer(eventClass);
	}
}

using System.Management;
using Microsoft.SqlServer.Management.Smo.Broker;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class ServiceQueueEventsWorker : ObjectInSchemaEventsWorker
{
	protected override string ObjectType => "Queue";

	public ServiceQueueEventsWorker(ServiceQueue target)
		: base(target, typeof(ServiceQueueEventSet), typeof(ServiceQueueEventValues))
	{
	}

	protected override EventQuery CreateWqlQuery(string eventClass)
	{
		ServiceQueue serviceQueue = (ServiceQueue)Target;
		return EventsWorkerBase.CreateWqlQueryForSourceObject(eventClass, serviceQueue.Parent.Parent.Name, serviceQueue.Schema, serviceQueue.Name, ObjectType);
	}
}

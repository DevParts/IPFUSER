namespace Microsoft.SqlServer.Management.Smo.Broker;

public class ServiceQueueEvents
{
	private ServiceQueueEventsWorker serverEventsWorker;

	private ServiceQueue parent;

	public event ServerEventHandler ServerEvent
	{
		add
		{
			InitializeEvents();
			serverEventsWorker.AddDefaultEventHandler(value);
		}
		remove
		{
			if (serverEventsWorker != null)
			{
				serverEventsWorker.RemoveDefaultEventHandler(value);
			}
		}
	}

	internal ServiceQueueEvents(ServiceQueue parent)
	{
		this.parent = parent;
	}

	public ServiceQueueEventSet GetEventSelection()
	{
		InitializeEvents();
		return (ServiceQueueEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(ServiceQueueEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(ServiceQueueEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(ServiceQueueEventSet events)
	{
		if (serverEventsWorker != null)
		{
			serverEventsWorker.UnsubscribeFromEvents(events);
		}
	}

	public void UnsubscribeAllEvents()
	{
		if (serverEventsWorker != null)
		{
			serverEventsWorker.Dispose();
			serverEventsWorker = null;
		}
	}

	public void StartEvents()
	{
		if (serverEventsWorker != null)
		{
			serverEventsWorker.StartEvents();
		}
	}

	public void StopEvents()
	{
		if (serverEventsWorker != null)
		{
			serverEventsWorker.StopEvents();
		}
	}

	private void InitializeEvents()
	{
		if (serverEventsWorker == null)
		{
			serverEventsWorker = new ServiceQueueEventsWorker(parent);
		}
	}
}

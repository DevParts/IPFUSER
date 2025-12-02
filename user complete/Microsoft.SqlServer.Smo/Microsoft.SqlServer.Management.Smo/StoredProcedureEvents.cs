namespace Microsoft.SqlServer.Management.Smo;

public class StoredProcedureEvents
{
	private StoredProcedureEventsWorker serverEventsWorker;

	private StoredProcedure parent;

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

	internal StoredProcedureEvents(StoredProcedure parent)
	{
		this.parent = parent;
	}

	public StoredProcedureEventSet GetEventSelection()
	{
		InitializeEvents();
		return (StoredProcedureEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(StoredProcedureEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(StoredProcedureEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(StoredProcedureEventSet events)
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
			serverEventsWorker = new StoredProcedureEventsWorker(parent);
		}
	}
}

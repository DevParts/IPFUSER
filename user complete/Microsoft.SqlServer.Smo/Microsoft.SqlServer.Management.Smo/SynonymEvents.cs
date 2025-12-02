namespace Microsoft.SqlServer.Management.Smo;

public class SynonymEvents
{
	private ObjectEventsWorker serverEventsWorker;

	private Synonym parent;

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

	internal SynonymEvents(Synonym parent)
	{
		this.parent = parent;
	}

	public ObjectEventSet GetEventSelection()
	{
		InitializeEvents();
		return (ObjectEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(ObjectEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(ObjectEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(ObjectEventSet events)
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
			serverEventsWorker = new ObjectEventsWorker(parent);
		}
	}
}

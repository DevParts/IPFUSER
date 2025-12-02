namespace Microsoft.SqlServer.Management.Smo;

public class DatabaseEvents
{
	private DatabaseEventsWorker serverEventsWorker;

	private Database parent;

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

	internal DatabaseEvents(Database parent)
	{
		this.parent = parent;
	}

	public DatabaseEventSet GetEventSelection()
	{
		InitializeEvents();
		return (DatabaseEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(DatabaseEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(DatabaseEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(DatabaseEventSet events)
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
			serverEventsWorker = new DatabaseEventsWorker(parent);
		}
	}
}

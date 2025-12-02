namespace Microsoft.SqlServer.Management.Smo;

public class SqlAssemblyEvents
{
	private SqlAssemblyEventsWorker serverEventsWorker;

	private SqlAssembly parent;

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

	internal SqlAssemblyEvents(SqlAssembly parent)
	{
		this.parent = parent;
	}

	public SqlAssemblyEventSet GetEventSelection()
	{
		InitializeEvents();
		return (SqlAssemblyEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(SqlAssemblyEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(SqlAssemblyEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(SqlAssemblyEventSet events)
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
			serverEventsWorker = new SqlAssemblyEventsWorker(parent);
		}
	}
}

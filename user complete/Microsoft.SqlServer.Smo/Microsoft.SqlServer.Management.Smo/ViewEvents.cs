namespace Microsoft.SqlServer.Management.Smo;

public class ViewEvents
{
	private ViewEventsWorker serverEventsWorker;

	private View parent;

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

	internal ViewEvents(View parent)
	{
		this.parent = parent;
	}

	public ViewEventSet GetEventSelection()
	{
		InitializeEvents();
		return (ViewEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(ViewEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(ViewEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(ViewEventSet events)
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
			serverEventsWorker = new ViewEventsWorker(parent);
		}
	}
}

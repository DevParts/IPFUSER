namespace Microsoft.SqlServer.Management.Smo;

public class UserDefinedFunctionEvents
{
	private UserDefinedFunctionEventsWorker serverEventsWorker;

	private UserDefinedFunction parent;

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

	internal UserDefinedFunctionEvents(UserDefinedFunction parent)
	{
		this.parent = parent;
	}

	public UserDefinedFunctionEventSet GetEventSelection()
	{
		InitializeEvents();
		return (UserDefinedFunctionEventSet)serverEventsWorker.GetEventSelection();
	}

	public void SubscribeToEvents(UserDefinedFunctionEventSet events)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(UserDefinedFunctionEventSet events, ServerEventHandler eventHandler)
	{
		InitializeEvents();
		serverEventsWorker.SubscribeToEvents(events, eventHandler);
	}

	public void UnsubscribeFromEvents(UserDefinedFunctionEventSet events)
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
			serverEventsWorker = new UserDefinedFunctionEventsWorker(parent);
		}
	}
}

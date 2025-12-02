using System.Management;

namespace Microsoft.SqlServer.Management.Smo;

public sealed class ServerEvents
{
	private ManagementScope managementScope;

	private ConnectionOptions connectionOptions;

	private ServerEventsWorker serverEventWorker;

	private ServerEventsWorker serverTraceEventWorker;

	private Server parent;

	internal ManagementScope ManagementScope
	{
		get
		{
			if (managementScope == null)
			{
				string netName = parent.Information.NetName;
				string text = parent.InstanceName;
				if (text.Length == 0)
				{
					text = "MSSQLSERVER";
				}
				managementScope = new ManagementScope();
				managementScope.Path = new ManagementPath(string.Format(SmoApplication.DefaultCulture, "\\\\{0}\\root\\Microsoft\\SqlServer\\ServerEvents\\{1}", new object[2] { netName, text }));
				if (connectionOptions != null)
				{
					managementScope.Options = connectionOptions;
				}
			}
			return managementScope;
		}
	}

	public event ServerEventHandler ServerEvent
	{
		add
		{
			InitializeServerEvent();
			InitializeServerTraceEvent();
			serverEventWorker.AddDefaultEventHandler(value);
			serverTraceEventWorker.AddDefaultEventHandler(value);
		}
		remove
		{
			if (serverEventWorker != null)
			{
				serverEventWorker.RemoveDefaultEventHandler(value);
			}
			if (serverTraceEventWorker != null)
			{
				serverTraceEventWorker.RemoveDefaultEventHandler(value);
			}
		}
	}

	internal ServerEvents(Server parent)
	{
		this.parent = parent;
	}

	public void SetCredentials(string username, string password)
	{
		if (connectionOptions == null)
		{
			connectionOptions = new ConnectionOptions();
		}
		connectionOptions.Username = username;
		connectionOptions.Password = password;
	}

	public ServerEventSet GetEventSelection()
	{
		InitializeServerEvent();
		return (ServerEventSet)serverEventWorker.GetEventSelection();
	}

	public ServerTraceEventSet GetTraceEventSelection()
	{
		InitializeServerTraceEvent();
		return (ServerTraceEventSet)serverTraceEventWorker.GetEventSelection();
	}

	public void SubscribeToEvents(ServerEventSet events)
	{
		InitializeServerEvent();
		serverEventWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(ServerEventSet events, ServerEventHandler eventHandler)
	{
		InitializeServerEvent();
		serverEventWorker.SubscribeToEvents(events, eventHandler);
	}

	public void SubscribeToEvents(ServerTraceEventSet events)
	{
		InitializeServerTraceEvent();
		serverTraceEventWorker.SubscribeToEvents(events, null);
	}

	public void SubscribeToEvents(ServerTraceEventSet events, ServerEventHandler eventHandler)
	{
		InitializeServerTraceEvent();
		serverTraceEventWorker.SubscribeToEvents(events, eventHandler);
	}

	public void StartEvents()
	{
		InitializeServerEvent();
		InitializeServerTraceEvent();
		serverEventWorker.StartEvents();
		serverTraceEventWorker.StartEvents();
	}

	public void StopEvents()
	{
		if (serverEventWorker != null)
		{
			serverEventWorker.StopEvents();
		}
		if (serverTraceEventWorker != null)
		{
			serverTraceEventWorker.StopEvents();
		}
	}

	public void UnsubscribeFromEvents(ServerEventSet events)
	{
		if (serverEventWorker != null)
		{
			serverEventWorker.UnsubscribeFromEvents(events);
		}
	}

	public void UnsubscribeFromEvents(ServerTraceEventSet events)
	{
		if (serverTraceEventWorker != null)
		{
			serverTraceEventWorker.UnsubscribeFromEvents(events);
		}
	}

	public void UnsubscribeAllEvents()
	{
		if (serverEventWorker != null)
		{
			serverEventWorker.Dispose();
			serverEventWorker = null;
		}
		if (serverTraceEventWorker != null)
		{
			serverTraceEventWorker.Dispose();
			serverTraceEventWorker = null;
		}
	}

	private void InitializeServerEvent()
	{
		if (serverEventWorker == null)
		{
			serverEventWorker = new ServerEventsWorker(parent, typeof(ServerEventSet), typeof(ServerEventValues));
		}
	}

	private void InitializeServerTraceEvent()
	{
		if (serverTraceEventWorker == null)
		{
			serverTraceEventWorker = new ServerEventsWorker(parent, typeof(ServerTraceEventSet), typeof(ServerTraceEventValues));
		}
	}
}

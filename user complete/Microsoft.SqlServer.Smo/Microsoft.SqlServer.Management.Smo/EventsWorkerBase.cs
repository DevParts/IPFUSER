using System;
using System.Collections;
using System.Globalization;
using System.Management;
using System.Text;
using Microsoft.SqlServer.Management.Diagnostics;

namespace Microsoft.SqlServer.Management.Smo;

internal abstract class EventsWorkerBase : IDisposable
{
	private sealed class EventSubscription : IDisposable
	{
		public readonly ManagementEventWatcher EventWatcher;

		public object EventHandlerKey;

		public EventSubscription(ManagementEventWatcher eventWatcher, object eventHandlerKey)
		{
			EventWatcher = eventWatcher;
			EventHandlerKey = eventHandlerKey;
		}

		public void Dispose()
		{
			if (EventWatcher != null)
			{
				EventWatcher.Stop();
				EventWatcher.Dispose();
			}
		}
	}

	private EventHandlerList eventHandlers = new EventHandlerList();

	private Hashtable eventSubscriptions = new Hashtable();

	private Type eventEnumType;

	private EventSetBase events;

	private bool eventsStarted;

	private ManagementScope managementScope;

	private static readonly object defaultEventHandlerKey = new object();

	protected abstract SqlSmoObject Target { get; }

	internal EventsWorkerBase(SqlSmoObject target, Type eventSetType, Type eventEnumType)
	{
		Server serverObject = target.GetServerObject();
		if (serverObject.ServerVersion.Major < 9)
		{
			throw new InvalidVersionSmoOperationException(serverObject.ServerVersion);
		}
		this.eventEnumType = eventEnumType;
		events = (EventSetBase)Activator.CreateInstance(eventSetType);
		managementScope = serverObject.Events.ManagementScope;
	}

	public EventSetBase GetEventSelection()
	{
		return events.Copy();
	}

	public void AddDefaultEventHandler(ServerEventHandler eventHandler)
	{
		eventHandlers.AddHandler(defaultEventHandlerKey, eventHandler);
	}

	public void RemoveDefaultEventHandler(ServerEventHandler eventHandler)
	{
		eventHandlers.RemoveHandler(defaultEventHandlerKey, eventHandler);
	}

	public void SubscribeToEvents(EventSetBase addEvents, ServerEventHandler eventHandler)
	{
		try
		{
			object eventHandlerKey = GetEventHandlerKey(eventHandler);
			for (int i = 0; i < addEvents.NumberOfElements; i++)
			{
				if (addEvents.GetBitAt(i))
				{
					string eventClass = GetEventClass(i);
					EventSubscription eventSubscription = (EventSubscription)eventSubscriptions[eventClass];
					if (eventSubscription != null)
					{
						TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Updating event handler for event " + eventClass + " on class " + Target.GetType().Name);
						eventSubscription.EventHandlerKey = eventHandlerKey;
					}
					else
					{
						TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Adding subscription for event " + eventClass + " on class " + Target.GetType().Name);
						CreateSubscription(i, eventClass, eventHandlerKey);
						events.SetBitAt(i, value: true);
					}
				}
			}
		}
		catch (Exception innerException)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CannotSubscribe, this, innerException);
		}
	}

	public void UnsubscribeFromEvents(EventSetBase removeEvents)
	{
		for (int i = 0; i < removeEvents.NumberOfElements; i++)
		{
			if (removeEvents.GetBitAt(i))
			{
				string eventClass = GetEventClass(i);
				EventSubscription eventSubscription = (EventSubscription)eventSubscriptions[eventClass];
				if (eventSubscription != null)
				{
					TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Removing subscription for event " + eventClass + " on class " + Target.GetType().Name);
					eventSubscriptions.Remove(eventClass);
					eventSubscription.Dispose();
					events.SetBitAt(i, value: false);
				}
			}
		}
	}

	public void StartEvents()
	{
		if (eventsStarted)
		{
			return;
		}
		try
		{
			foreach (EventSubscription value in eventSubscriptions.Values)
			{
				TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Starting event subscription on: " + value.EventWatcher.Scope.Path.ToString() + ", query: " + value.EventWatcher.Query.QueryString);
				value.EventWatcher.Start();
			}
			eventsStarted = true;
		}
		catch (Exception innerException)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CannotStartSubscription, this, innerException);
		}
	}

	public void StopEvents()
	{
		if (!eventsStarted)
		{
			return;
		}
		foreach (EventSubscription value in eventSubscriptions.Values)
		{
			value.EventWatcher.Stop();
		}
		eventsStarted = false;
	}

	public void Dispose()
	{
		TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Removing all subscriptions");
		foreach (EventSubscription value in eventSubscriptions.Values)
		{
			value.Dispose();
		}
		eventSubscriptions.Clear();
		eventHandlers.Dispose();
	}

	protected abstract EventQuery CreateWqlQuery(string eventClass);

	private object GetEventHandlerKey(ServerEventHandler eventHandler)
	{
		if (eventHandler != null)
		{
			object obj = eventHandlers.FindHandler(eventHandler);
			if (obj == null)
			{
				obj = new object();
				eventHandlers.AddHandler(obj, eventHandler);
			}
			return obj;
		}
		return defaultEventHandlerKey;
	}

	protected virtual string GetEventClass(int eventID)
	{
		StringBuilder eventName = new StringBuilder(Enum.GetName(eventEnumType, eventID));
		return ConvertToEventClass(eventName);
	}

	protected static string ConvertToEventClass(StringBuilder eventName)
	{
		eventName.Replace("DB", "Db");
		for (int i = 1; i < eventName.Length; i++)
		{
			if (char.IsUpper(eventName[i]) || char.IsDigit(eventName[i]))
			{
				eventName.Insert(i++, '_');
			}
		}
		eventName.Replace("Trace", "Trc");
		return eventName.ToString().ToUpper(CultureInfo.InvariantCulture);
	}

	protected static EventType ConvertFromEventClass(string eventClass)
	{
		StringBuilder stringBuilder = new StringBuilder(eventClass);
		for (int i = 1; i < stringBuilder.Length; i++)
		{
			if (stringBuilder[i] == '_')
			{
				stringBuilder.Remove(i, 1);
			}
		}
		stringBuilder.Replace("Trc", "Trace");
		return (EventType)Enum.Parse(typeof(EventType), stringBuilder.ToString(), ignoreCase: true);
	}

	private void CreateSubscription(int eventID, string eventClass, object eventHandlerKey)
	{
		EventQuery eventQuery = CreateWqlQuery(eventClass);
		TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Subscription query " + eventQuery.QueryString);
		TryConnect();
		ManagementEventWatcher managementEventWatcher = new ManagementEventWatcher(managementScope, eventQuery);
		managementEventWatcher.EventArrived += OnEventArrived;
		EventSubscription value = new EventSubscription(managementEventWatcher, eventHandlerKey);
		if (eventsStarted)
		{
			managementEventWatcher.Start();
		}
		eventSubscriptions.Add(eventClass, value);
	}

	private void OnEventArrived(object sender, EventArrivedEventArgs args)
	{
		string text = args.NewEvent.ClassPath.ClassName;
		EventType eventType = ConvertFromEventClass(args.NewEvent.ClassPath.ClassName);
		EventSubscription eventSubscription = null;
		TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Recieved event " + args.NewEvent.ClassPath.ClassName + " on class " + Target.GetType().Name);
		while (text != null && text.Length > 0)
		{
			eventSubscription = (EventSubscription)eventSubscriptions[text];
			if (eventSubscription != null)
			{
				ServerEventHandler serverEventHandler = (ServerEventHandler)eventHandlers[eventSubscription.EventHandlerKey];
				if (serverEventHandler != null)
				{
					TraceHelper.Trace(SmoApplication.ModuleName, SmoApplication.trAlways, "Raising event " + args.NewEvent.ClassPath.ClassName + "on subscription: " + text);
					serverEventHandler(Target, new ServerEventArgs(eventType, args.NewEvent.Properties));
				}
			}
			text = null;
		}
	}

	private void TryConnect()
	{
		if (managementScope.IsConnected)
		{
			return;
		}
		try
		{
			managementScope.Connect();
			ManagementPath path = new ManagementPath("Win32Provider.Name=\"MSSQL_ManagementProvider\"");
			new ManagementObject(managementScope, path, new ObjectGetOptions());
		}
		catch (ManagementException ex)
		{
			if (ex.ErrorCode == ManagementStatus.InvalidNamespace || ex.ErrorCode == ManagementStatus.ProviderLoadFailure)
			{
				throw new SmoException(ExceptionTemplatesImpl.WMIProviderNotInstalled(Target.GetServerObject().Name), ex);
			}
			throw;
		}
	}

	internal static EventQuery CreateWqlQueryForServer(string eventClass)
	{
		return new EventQuery(string.Format(SmoApplication.DefaultCulture, "SELECT * FROM {0}", new object[1] { eventClass }));
	}

	internal static EventQuery CreateWqlQueryForDatabase(string eventClass, string databaseName)
	{
		return new EventQuery(string.Format(SmoApplication.DefaultCulture, "SELECT * FROM {0} WHERE DatabaseName = '{1}'", new object[2]
		{
			eventClass,
			EscapeWqlParameter(databaseName)
		}));
	}

	internal static EventQuery CreateWqlQueryForDatabaseObject(string eventClass, string databaseName, string objectName, string objectType)
	{
		return new EventQuery(string.Format(SmoApplication.DefaultCulture, "SELECT * FROM {0} WHERE DatabaseName = '{1}' AND ObjectName = '{2}' AND ObjectType = '{3}'", eventClass, EscapeWqlParameter(databaseName), EscapeWqlParameter(objectName), objectType));
	}

	internal static EventQuery CreateWqlQueryForTargetObject(string eventClass, string databaseName, string schemaName, string objectType, string targetObjectName, string targetObjectType)
	{
		return new EventQuery(string.Format(SmoApplication.DefaultCulture, "SELECT * FROM {0} WHERE DatabaseName = '{1}' AND SchemaName = '{2}' AND ObjectType = '{3}' AND TargetObjectName = '{4}' AND TargetObjectType='{5}'", eventClass, EscapeWqlParameter(databaseName), EscapeWqlParameter(schemaName), objectType, EscapeWqlParameter(targetObjectName), targetObjectType));
	}

	internal static EventQuery CreateWqlQueryForSourceObject(string eventClass, string databaseName, string schemaName, string objectName, string objectType)
	{
		return new EventQuery(string.Format(SmoApplication.DefaultCulture, "SELECT * FROM {0} WHERE DatabaseName = '{1}' AND SchemaName = '{2}' AND ObjectName = '{3}' AND ObjectType='{4}'", eventClass, EscapeWqlParameter(databaseName), EscapeWqlParameter(schemaName), EscapeWqlParameter(objectName), objectType));
	}

	private static string EscapeWqlParameter(string parameter)
	{
		return SqlSmoObject.EscapeString(SqlSmoObject.EscapeString(parameter, '\\'), '\'');
	}
}

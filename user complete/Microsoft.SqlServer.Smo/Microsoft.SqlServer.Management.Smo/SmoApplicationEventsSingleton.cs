using System;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

internal class SmoApplicationEventsSingleton : ISmoApplicationEvents
{
	private SmoApplication.ObjectCreatedEventHandler objectCreated;

	private SmoApplication.ObjectDroppedEventHandler objectDropped;

	private SmoApplication.ObjectRenamedEventHandler objectRenamed;

	private SmoApplication.ObjectAlteredEventHandler objectAltered;

	private SmoApplication.AnyObjectEventHandler anyObjectEvent;

	private SmoApplication.DatabaseEventHandler databaseEvent;

	public event SmoApplication.ObjectCreatedEventHandler ObjectCreated
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				objectCreated = (SmoApplication.ObjectCreatedEventHandler)Delegate.Combine(objectCreated, value);
			}
		}
		remove
		{
			objectCreated = (SmoApplication.ObjectCreatedEventHandler)Delegate.Remove(objectCreated, value);
		}
	}

	public event SmoApplication.ObjectDroppedEventHandler ObjectDropped
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				objectDropped = (SmoApplication.ObjectDroppedEventHandler)Delegate.Combine(objectDropped, value);
			}
		}
		remove
		{
			objectDropped = (SmoApplication.ObjectDroppedEventHandler)Delegate.Remove(objectDropped, value);
		}
	}

	public event SmoApplication.ObjectRenamedEventHandler ObjectRenamed
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				objectRenamed = (SmoApplication.ObjectRenamedEventHandler)Delegate.Combine(objectRenamed, value);
			}
		}
		remove
		{
			objectRenamed = (SmoApplication.ObjectRenamedEventHandler)Delegate.Remove(objectRenamed, value);
		}
	}

	public event SmoApplication.ObjectAlteredEventHandler ObjectAltered
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				objectAltered = (SmoApplication.ObjectAlteredEventHandler)Delegate.Combine(objectAltered, value);
			}
		}
		remove
		{
			objectAltered = (SmoApplication.ObjectAlteredEventHandler)Delegate.Remove(objectAltered, value);
		}
	}

	public event SmoApplication.AnyObjectEventHandler AnyObjectEvent
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				anyObjectEvent = (SmoApplication.AnyObjectEventHandler)Delegate.Combine(anyObjectEvent, value);
			}
		}
		remove
		{
			anyObjectEvent = (SmoApplication.AnyObjectEventHandler)Delegate.Remove(anyObjectEvent, value);
		}
	}

	public event SmoApplication.DatabaseEventHandler DatabaseEvent
	{
		add
		{
			if (!SqlContext.IsAvailable)
			{
				databaseEvent = (SmoApplication.DatabaseEventHandler)Delegate.Combine(databaseEvent, value);
			}
		}
		remove
		{
			databaseEvent = (SmoApplication.DatabaseEventHandler)Delegate.Remove(databaseEvent, value);
		}
	}

	internal void CallObjectCreated(object sender, ObjectCreatedEventArgs e)
	{
		if (objectCreated != null)
		{
			objectCreated(sender, e);
		}
		if (anyObjectEvent != null)
		{
			anyObjectEvent(sender, e);
		}
	}

	internal bool IsNullObjectCreated()
	{
		return objectCreated == null;
	}

	internal void CallObjectDropped(object sender, ObjectDroppedEventArgs e)
	{
		if (objectDropped != null)
		{
			objectDropped(sender, e);
		}
		if (anyObjectEvent != null)
		{
			anyObjectEvent(sender, e);
		}
	}

	internal bool IsNullObjectDropped()
	{
		return objectDropped == null;
	}

	internal void CallObjectRenamed(object sender, ObjectRenamedEventArgs e)
	{
		if (objectRenamed != null)
		{
			objectRenamed(sender, e);
		}
		if (anyObjectEvent != null)
		{
			anyObjectEvent(sender, e);
		}
	}

	internal bool IsNullObjectRenamed()
	{
		return objectRenamed == null;
	}

	internal void CallObjectAltered(object sender, ObjectAlteredEventArgs e)
	{
		if (objectAltered != null)
		{
			objectAltered(sender, e);
		}
		if (anyObjectEvent != null)
		{
			anyObjectEvent(sender, e);
		}
	}

	internal bool IsNullObjectAltered()
	{
		return objectAltered == null;
	}

	internal void CallDatabaseEvent(object sender, DatabaseEventArgs e)
	{
		if (databaseEvent != null)
		{
			databaseEvent(sender, e);
		}
		if (anyObjectEvent != null)
		{
			anyObjectEvent(sender, e);
		}
	}

	internal bool IsNullDatabaseEvent()
	{
		return databaseEvent == null;
	}
}

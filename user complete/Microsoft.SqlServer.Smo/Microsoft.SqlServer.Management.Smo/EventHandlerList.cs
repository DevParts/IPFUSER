using System;

namespace Microsoft.SqlServer.Management.Smo;

internal sealed class EventHandlerList : IDisposable
{
	private sealed class ListEntry
	{
		internal ListEntry next;

		internal object key;

		internal Delegate handler;

		public ListEntry(object key, Delegate handler, ListEntry next)
		{
			this.next = next;
			this.key = key;
			this.handler = handler;
		}
	}

	private object syncRoot = new object();

	private ListEntry head;

	public Delegate this[object key]
	{
		get
		{
			return Find(key)?.handler;
		}
		set
		{
			ListEntry listEntry = Find(key);
			if (listEntry != null)
			{
				listEntry.handler = value;
				return;
			}
			lock (syncRoot)
			{
				head = new ListEntry(key, value, head);
			}
		}
	}

	public void AddHandler(object key, Delegate value)
	{
		ListEntry listEntry = Find(key);
		if (listEntry != null)
		{
			listEntry.handler = Delegate.Combine(listEntry.handler, value);
			return;
		}
		lock (syncRoot)
		{
			head = new ListEntry(key, value, head);
		}
	}

	public void RemoveHandler(object key, Delegate value)
	{
		ListEntry listEntry = Find(key);
		if (listEntry != null)
		{
			listEntry.handler = Delegate.Remove(listEntry.handler, value);
		}
	}

	public object FindHandler(Delegate value)
	{
		ListEntry listEntry = null;
		lock (syncRoot)
		{
			listEntry = head;
		}
		while (listEntry != null)
		{
			if (listEntry.handler.Equals(value))
			{
				return listEntry.key;
			}
			listEntry = listEntry.next;
		}
		return null;
	}

	public void Dispose()
	{
		lock (syncRoot)
		{
			head = null;
		}
	}

	private ListEntry Find(object key)
	{
		ListEntry listEntry = null;
		lock (syncRoot)
		{
			listEntry = head;
		}
		while (listEntry != null && listEntry.key != key)
		{
			listEntry = listEntry.next;
		}
		return listEntry;
	}
}

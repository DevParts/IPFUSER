using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Common;

internal class ExecutionCache<K, C> where K : class where C : CacheItem<K>
{
	private List<C> items;

	private readonly int capacity;

	public C this[K key]
	{
		get
		{
			if (key == null)
			{
				throw new ArgumentNullException();
			}
			if (items == null)
			{
				return null;
			}
			foreach (C item in items)
			{
				C current = item;
				if (key.Equals(current.Key))
				{
					return current;
				}
			}
			return null;
		}
	}

	public int Count
	{
		get
		{
			if (items == null)
			{
				return 0;
			}
			return items.Count;
		}
	}

	public ExecutionCache(int capacity)
	{
		if (capacity <= 1)
		{
			throw new ArgumentOutOfRangeException(StringConnectionInfo.InvalidArgumentCacheCapacity(1));
		}
		this.capacity = capacity;
	}

	public bool ContainsKey(K key)
	{
		if (key == null)
		{
			throw new ArgumentNullException();
		}
		if (items == null)
		{
			return false;
		}
		foreach (C item in items)
		{
			C current = item;
			if (key.Equals(current.Key))
			{
				return true;
			}
		}
		return false;
	}

	public void Add(C item)
	{
		if (item == null)
		{
			throw new ArgumentNullException();
		}
		if (item.Key == null)
		{
			throw new ArgumentException(StringConnectionInfo.InvalidArgumentCacheNullKey);
		}
		if (ContainsKey(item.Key))
		{
			throw new ArgumentException(StringConnectionInfo.InvalidArgumentCacheDuplicateKey(item.Key));
		}
		if (items == null)
		{
			items = new List<C>(capacity);
		}
		else
		{
			while (items.Count >= capacity)
			{
				items.RemoveAt(0);
			}
		}
		items.Add(item);
	}

	public void ClearResults()
	{
		if (items == null)
		{
			return;
		}
		foreach (C item in items)
		{
			C current = item;
			current.ClearResult();
		}
	}

	public void Clear()
	{
		if (items != null)
		{
			items.Clear();
		}
	}

	public bool IsEmpty()
	{
		return Count == 0;
	}
}

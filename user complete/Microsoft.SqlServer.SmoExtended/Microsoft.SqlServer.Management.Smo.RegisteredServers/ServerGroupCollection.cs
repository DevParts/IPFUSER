using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public sealed class ServerGroupCollection : RegSvrCollectionBase, ICollection, IEnumerable
{
	internal sealed class ServerGroupCollectionEnumerator : IEnumerator
	{
		internal IDictionaryEnumerator baseEnumerator;

		object IEnumerator.Current => baseEnumerator.Value as ServerGroup;

		public ServerGroup Current => baseEnumerator.Value as ServerGroup;

		internal ServerGroupCollectionEnumerator(RegSvrCollectionBase col)
		{
			baseEnumerator = col.innerColl.GetEnumerator();
		}

		public bool MoveNext()
		{
			return baseEnumerator.MoveNext();
		}

		public void Reset()
		{
			baseEnumerator.Reset();
		}
	}

	public ServerGroup Parent => base.ParentInstance as ServerGroup;

	public ServerGroup this[int index] => GetObjectByIndex(index);

	public ServerGroup this[string name] => GetObjectByName(name);

	public int Count
	{
		get
		{
			if (!initialized)
			{
				InitializeChildCollection();
			}
			return innerColl.Count;
		}
	}

	internal ServerGroupCollection(RegSvrSmoObject parentInstance)
		: base(parentInstance)
	{
	}

	public bool Contains(string key)
	{
		if (innerColl.ContainsKey(key))
		{
			return true;
		}
		if (!initialized && InitializeChildObject(key) != null)
		{
			return true;
		}
		return false;
	}

	internal ServerGroup GetObjectByIndex(int index)
	{
		if (!initialized)
		{
			InitializeChildCollection();
		}
		return innerColl.GetByIndex(index) as ServerGroup;
	}

	internal ServerGroup GetObjectByName(string name)
	{
		object obj = innerColl[name];
		if (obj == null && !initialized)
		{
			obj = InitializeChildObject(name);
		}
		return obj as ServerGroup;
	}

	internal object InitializeChildObject(string name)
	{
		ServerGroup serverGroup = new ServerGroup(this, name);
		if (serverGroup.Initialize())
		{
			return serverGroup;
		}
		return null;
	}

	internal void InitializeChildCollection()
	{
		SortedList sortedList = innerColl;
		innerColl = new SortedList(System.StringComparer.Ordinal);
		base.ParentInstance.EnumChildren(typeof(ServerGroup).ToString(), this);
		foreach (DictionaryEntry item in sortedList)
		{
			ServerGroup serverGroup = item.Value as ServerGroup;
			innerColl[serverGroup.Name] = serverGroup;
		}
		initialized = true;
	}

	public void CopyTo(Array array, int index)
	{
		int num = index;
		foreach (DictionaryEntry item in innerColl)
		{
			array.SetValue((ServerGroup)item.Value, num++);
		}
	}

	public IEnumerator GetEnumerator()
	{
		if (!initialized)
		{
			InitializeChildCollection();
		}
		return new ServerGroupCollectionEnumerator(this);
	}

	public void Add(ServerGroup registeredServer)
	{
		registeredServer.SetParentImpl(base.ParentInstance);
		AddInternal(registeredServer);
	}

	public void Remove(string key)
	{
		RemoveInternal(key);
	}
}

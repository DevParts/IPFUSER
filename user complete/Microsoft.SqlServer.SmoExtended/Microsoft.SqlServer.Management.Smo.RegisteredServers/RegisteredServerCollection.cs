using System;
using System.Collections;

namespace Microsoft.SqlServer.Management.Smo.RegisteredServers;

[Obsolete("Instead use namespace Microsoft.SqlServer.Management.RegisteredServers")]
public sealed class RegisteredServerCollection : RegSvrCollectionBase, ICollection, IEnumerable
{
	internal sealed class RegisteredServerCollectionEnumerator : IEnumerator
	{
		internal IDictionaryEnumerator baseEnumerator;

		object IEnumerator.Current => baseEnumerator.Value as RegisteredServer;

		public RegisteredServer Current => baseEnumerator.Value as RegisteredServer;

		internal RegisteredServerCollectionEnumerator(RegSvrCollectionBase col)
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

	public RegisteredServer this[int index] => GetObjectByIndex(index);

	public RegisteredServer this[string name] => GetObjectByName(name);

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

	internal RegisteredServerCollection(RegSvrSmoObject parentInstance)
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

	internal RegisteredServer GetObjectByIndex(int index)
	{
		if (!initialized)
		{
			InitializeChildCollection();
		}
		return innerColl.GetByIndex(index) as RegisteredServer;
	}

	internal RegisteredServer GetObjectByName(string name)
	{
		object obj = innerColl[name];
		if (obj == null && !initialized)
		{
			obj = InitializeChildObject(name);
		}
		return obj as RegisteredServer;
	}

	internal object InitializeChildObject(string name)
	{
		RegisteredServer registeredServer = new RegisteredServer(this, name);
		if (registeredServer.Initialize())
		{
			return registeredServer;
		}
		return null;
	}

	internal void InitializeChildCollection()
	{
		SortedList sortedList = innerColl;
		innerColl = new SortedList(System.StringComparer.Ordinal);
		base.ParentInstance.EnumChildren(typeof(RegisteredServer).ToString(), this);
		foreach (DictionaryEntry item in sortedList)
		{
			RegisteredServer registeredServer = item.Value as RegisteredServer;
			innerColl[registeredServer.Name] = registeredServer;
		}
		initialized = true;
	}

	public void CopyTo(Array array, int index)
	{
		int num = index;
		foreach (DictionaryEntry item in innerColl)
		{
			array.SetValue((RegisteredServer)item.Value, num++);
		}
	}

	public IEnumerator GetEnumerator()
	{
		if (!initialized)
		{
			InitializeChildCollection();
		}
		return new RegisteredServerCollectionEnumerator(this);
	}

	public void Add(RegisteredServer registeredServer)
	{
		registeredServer.SetParentImpl(base.ParentInstance);
		AddInternal(registeredServer);
	}

	public void Remove(string key)
	{
		RemoveInternal(key);
	}
}

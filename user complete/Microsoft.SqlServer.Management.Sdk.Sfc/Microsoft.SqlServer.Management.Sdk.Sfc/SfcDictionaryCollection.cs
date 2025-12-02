using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcDictionaryCollection<T, K, ParentT> : SfcCollection<T, K, ParentT>, IEnumerable<T>, IEnumerable where T : SfcInstance where K : SfcKey where ParentT : SfcInstance
{
	private Dictionary<K, T> m_collection;

	private Dictionary<K, T> m_shadow;

	public override int Count
	{
		get
		{
			EnsureCollectionInitialized();
			return m_collection.Count;
		}
	}

	public override bool IsReadOnly => false;

	protected SfcDictionaryCollection(ParentT parent)
		: base(parent)
	{
	}

	protected override void AddImpl(T obj)
	{
		m_collection.Add(obj.AbstractIdentityKey as K, obj);
	}

	public override void Clear()
	{
		EnsureCollectionInitialized();
		m_collection.Clear();
	}

	public override bool Contains(T obj)
	{
		EnsureCollectionInitialized();
		if (Contains(obj.AbstractIdentityKey as K))
		{
			return obj.State != SfcObjectState.Dropped;
		}
		return false;
	}

	public override void CopyTo(T[] array, int arrayIndex)
	{
		EnsureCollectionInitialized();
		int num = 0;
		foreach (T value in m_collection.Values)
		{
			array.SetValue(value, num + arrayIndex);
			num++;
		}
	}

	public override bool Remove(T obj)
	{
		EnsureCollectionInitialized();
		return RemoveInternal(obj);
	}

	public override IEnumerator<T> GetEnumerator()
	{
		EnsureCollectionInitialized();
		return m_collection.Values.GetEnumerator();
	}

	public override bool Contains(K key)
	{
		EnsureCollectionInitialized();
		return m_collection.ContainsKey(key);
	}

	public bool TryGetValue(K key, out T obj)
	{
		EnsureCollectionInitialized();
		return m_collection.TryGetValue(key, out obj);
	}

	protected override bool RemoveImpl(T obj)
	{
		return m_collection.Remove(obj.AbstractIdentityKey as K);
	}

	protected override T GetObjectByKey(K key)
	{
		if (m_collection.TryGetValue(key, out var value))
		{
			return value;
		}
		return CreateAndInitializeChildObject(key);
	}

	protected override T GetExistingObjectByKey(K key)
	{
		if (m_collection.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	protected override void InitInnerCollection()
	{
		m_collection = new Dictionary<K, T>();
	}

	protected override void PrepareMerge()
	{
		m_shadow = new Dictionary<K, T>();
	}

	protected override bool AddShadow(T obj)
	{
		if (m_shadow != null)
		{
			m_shadow.Add(obj.AbstractIdentityKey as K, obj);
			TraceHelper.Assert(obj.Parent == base.Parent);
			return true;
		}
		return false;
	}

	protected override void FinishMerge()
	{
		if (m_shadow != null)
		{
			m_collection = m_shadow;
			m_shadow = null;
		}
	}
}

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcListCollection<T, K, ParentT> : SfcCollection<T, K, ParentT>, IEnumerable<T>, IEnumerable where T : SfcInstance where K : SfcKey where ParentT : SfcInstance
{
	private List<T> m_list;

	private List<T> m_shadow;

	public override int Count
	{
		get
		{
			EnsureCollectionInitialized();
			return m_list.Count;
		}
	}

	public override bool IsReadOnly => false;

	protected SfcListCollection(ParentT parent)
		: base(parent)
	{
	}

	protected override void AddImpl(T obj)
	{
		m_list.Add(obj);
	}

	public override void Clear()
	{
		EnsureCollectionInitialized();
		m_list.Clear();
	}

	public override bool Contains(T obj)
	{
		EnsureCollectionInitialized();
		int num = m_list.IndexOf(obj);
		if (num != -1)
		{
			return obj.State != SfcObjectState.Dropped;
		}
		return false;
	}

	public override void CopyTo(T[] array, int arrayIndex)
	{
		EnsureCollectionInitialized();
		for (int i = 0; i < Count; i++)
		{
			array.SetValue(m_list[i], i + arrayIndex);
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
		return m_list.GetEnumerator();
	}

	protected ICollection<T> GetInternalCollectionImpl()
	{
		return m_list;
	}

	public override bool Contains(K key)
	{
		EnsureCollectionInitialized();
		foreach (T item in m_list)
		{
			T current = item;
			K val = current.AbstractIdentityKey as K;
			if (val.Equals(key))
			{
				TraceHelper.Assert(current.State != SfcObjectState.Dropped);
				return true;
			}
		}
		return false;
	}

	protected override bool RemoveImpl(T obj)
	{
		return m_list.Remove(obj);
	}

	protected override T GetObjectByKey(K key)
	{
		T existingObjectByKey = GetExistingObjectByKey(key);
		if (existingObjectByKey != null)
		{
			return existingObjectByKey;
		}
		return CreateAndInitializeChildObject(key);
	}

	protected override T GetExistingObjectByKey(K key)
	{
		foreach (T item in m_list)
		{
			T current = item;
			K val = current.AbstractIdentityKey as K;
			if (val.Equals(key) && current.State != SfcObjectState.Dropped)
			{
				return current;
			}
		}
		return null;
	}

	protected override void InitInnerCollection()
	{
		m_list = new List<T>();
	}

	protected override void PrepareMerge()
	{
		m_shadow = new List<T>();
	}

	protected override bool AddShadow(T obj)
	{
		if (m_shadow != null)
		{
			m_shadow.Add(obj);
			TraceHelper.Assert(obj.Parent == base.Parent);
			return true;
		}
		return false;
	}

	protected override void FinishMerge()
	{
		if (m_shadow != null)
		{
			m_list = m_shadow;
			m_shadow = null;
		}
	}
}

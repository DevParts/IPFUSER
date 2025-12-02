using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcCollatedDictionaryCollection<T, K, ParentT> : SfcCollection<T, K, ParentT>, IComparer<K>, IEnumerable<T>, IEnumerable where T : SfcInstance where K : SfcKey where ParentT : SfcInstance
{
	private SortedDictionary<K, T> m_collection;

	private SortedDictionary<K, T> m_shadow;

	private bool m_dirty;

	private CultureInfo m_cultureInfo = CultureInfo.InvariantCulture;

	private bool m_ignoreCase;

	private bool m_ascending = true;

	private IComparer<string> comparer;

	protected CultureInfo CultureInfo
	{
		get
		{
			return m_cultureInfo;
		}
		set
		{
			if (m_cultureInfo != value)
			{
				m_dirty = true;
			}
			m_cultureInfo = value;
		}
	}

	protected bool IgnoreCase
	{
		get
		{
			return m_ignoreCase;
		}
		set
		{
			if (m_ignoreCase != value)
			{
				m_dirty = true;
			}
			m_ignoreCase = value;
		}
	}

	public bool Ascending
	{
		get
		{
			return m_ascending;
		}
		set
		{
			if (m_ascending != value)
			{
				m_dirty = true;
			}
			m_ascending = value;
		}
	}

	public override int Count
	{
		get
		{
			EnsureCollectionInitialized();
			return m_collection.Count;
		}
	}

	public override bool IsReadOnly => false;

	protected SfcCollatedDictionaryCollection(ParentT parent)
		: this(parent, (IComparer<string>)null)
	{
	}

	protected SfcCollatedDictionaryCollection(ParentT parent, IComparer<string> customComparer)
		: base(parent)
	{
		comparer = customComparer;
	}

	protected void ResetInnerCollection()
	{
		if (m_dirty)
		{
			m_dirty = false;
			if (m_collection != null)
			{
				SortedDictionary<K, T> collection = new SortedDictionary<K, T>(m_collection, this);
				m_collection.Clear();
				m_collection = collection;
			}
		}
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
		m_collection = new SortedDictionary<K, T>(this);
		m_dirty = false;
	}

	protected override void PrepareMerge()
	{
		m_shadow = new SortedDictionary<K, T>(this);
		m_dirty = false;
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

	int IComparer<K>.Compare(K key1, K key2)
	{
		if (comparer == null)
		{
			return key1.ToString().Compare(key2.ToString(), m_ignoreCase, m_cultureInfo);
		}
		return comparer.Compare(key1.ToString(), key2.ToString());
	}
}

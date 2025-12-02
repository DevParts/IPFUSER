using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

public abstract class SfcReferenceCollection<K, T, S> : IReadOnlyDictionary<K, T>, IReadOnlyCollection<T>, IReadOnlyCollection, IEnumerable<T>, IEnumerable, IListSource where K : IComparable<K> where T : SfcInstance where S : SfcInstance
{
	private IComparer<K> comparer;

	private SortedDictionary<K, T> dict;

	private ISfcReferenceCollectionResolver<T, S> resolver;

	private S owner;

	bool IListSource.ContainsListCollection => false;

	public T this[K key] => dict[key];

	public IEnumerable<K> Keys => dict.Keys;

	public IEnumerable<T> Values => dict.Values;

	public int Count => dict.Count;

	protected SfcReferenceCollection(S owner, ISfcReferenceCollectionResolver<T, S> resolver, IComparer<K> comparer)
	{
		this.owner = owner;
		this.resolver = resolver;
		this.comparer = comparer;
		Reset();
	}

	public void Refresh()
	{
		Reset();
	}

	private void Reset()
	{
		if (comparer == null)
		{
			dict = new SortedDictionary<K, T>(comparer);
		}
		else
		{
			dict = new SortedDictionary<K, T>();
		}
		foreach (T item in resolver.ResolveCollection(owner, null))
		{
			dict.Add(GetKeyFromValue(item), item);
		}
	}

	protected abstract K GetKeyFromValue(T value);

	IList IListSource.GetList()
	{
		return dict.Values.ToList();
	}

	public bool ContainsKey(K key)
	{
		return dict.ContainsKey(key);
	}

	public bool TryGetValue(K key, out T value)
	{
		return dict.TryGetValue(key, out value);
	}

	public bool Contains(T item)
	{
		return dict.ContainsValue(item);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		dict.Values.CopyTo(array, arrayIndex);
	}

	public IEnumerator GetEnumerator()
	{
		return dict.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return ((IEnumerable<T>)dict).GetEnumerator();
	}
}

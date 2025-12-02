using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

internal struct ReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<TValue>, IReadOnlyCollection, IEnumerable<TValue>, IEnumerable
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct Enumerable
	{
		public static IEnumerator<KeyValuePair<TKey, TValue>> Empty
		{
			get
			{
				yield break;
			}
		}
	}

	private readonly IDictionary<TKey, TValue> dictionary;

	public int Count
	{
		get
		{
			if (dictionary == null)
			{
				return 0;
			}
			return dictionary.Count;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			if (dictionary == null)
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				throw new KeyNotFoundException();
			}
			return dictionary[key];
		}
	}

	public IEnumerable<TKey> Keys
	{
		get
		{
			if (dictionary == null)
			{
				yield break;
			}
			foreach (TKey key in dictionary.Keys)
			{
				yield return key;
			}
		}
	}

	public IEnumerable<TValue> Values
	{
		get
		{
			if (dictionary == null)
			{
				yield break;
			}
			foreach (TValue value in dictionary.Values)
			{
				yield return value;
			}
		}
	}

	public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
	{
		this.dictionary = dictionary;
	}

	public bool Contains(TValue item)
	{
		if (dictionary != null)
		{
			return dictionary.Values.Contains(item);
		}
		return false;
	}

	public bool ContainsKey(TKey key)
	{
		if (dictionary != null)
		{
			return dictionary.ContainsKey(key);
		}
		return false;
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		if (dictionary != null)
		{
			return dictionary.TryGetValue(key, out value);
		}
		value = default(TValue);
		return false;
	}

	public void CopyTo(TValue[] array, int arrayIndex)
	{
		dictionary.Values.CopyTo(array, arrayIndex);
	}

	IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
	{
		return dictionary.Values.GetEnumerator();
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		if (dictionary == null)
		{
			return Enumerable.Empty;
		}
		return dictionary.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public static implicit operator ReadOnlyDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
	{
		return new ReadOnlyDictionary<TKey, TValue>(dictionary);
	}

	public static implicit operator ReadOnlyDictionary<TKey, TValue>(SortedList<TKey, TValue> sortedList)
	{
		return new ReadOnlyDictionary<TKey, TValue>(sortedList);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NLog.Internal;

[DebuggerDisplay("Count = {Count}")]
internal class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
	{
		private Dictionary<TKey, TValue>.Enumerator _enumerator;

		public KeyValuePair<TKey, TValue> Current => _enumerator.Current;

		object IEnumerator.Current => _enumerator.Current;

		public Enumerator(Dictionary<TKey, TValue>.Enumerator enumerator)
		{
			_enumerator = enumerator;
		}

		public void Dispose()
		{
			_enumerator.Dispose();
		}

		public bool MoveNext()
		{
			return _enumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			((IEnumerator)_enumerator).Reset();
		}
	}

	private readonly object _lockObject = new object();

	private Dictionary<TKey, TValue> _dict;

	private Dictionary<TKey, TValue>? _dictReadOnly;

	public TValue this[TKey key]
	{
		get
		{
			return GetReadOnlyDict()[key];
		}
		set
		{
			lock (_lockObject)
			{
				GetWritableDict()[key] = value;
			}
		}
	}

	public IEqualityComparer<TKey> Comparer => _dict.Comparer;

	public ICollection<TKey> Keys => GetReadOnlyDict().Keys;

	public ICollection<TValue> Values => GetReadOnlyDict().Values;

	public int Count => GetReadOnlyDict().Count;

	public bool IsReadOnly => false;

	public ThreadSafeDictionary()
		: this((IEqualityComparer<TKey>)EqualityComparer<TKey>.Default)
	{
	}

	public ThreadSafeDictionary(IEqualityComparer<TKey> comparer)
	{
		_dict = new Dictionary<TKey, TValue>(comparer);
	}

	public ThreadSafeDictionary(ThreadSafeDictionary<TKey, TValue> source)
	{
		Dictionary<TKey, TValue> readOnlyDict = source.GetReadOnlyDict();
		_dict = new Dictionary<TKey, TValue>(readOnlyDict.Count, readOnlyDict.Comparer);
		foreach (KeyValuePair<TKey, TValue> item in readOnlyDict)
		{
			_dict.Add(item.Key, item.Value);
		}
	}

	public void Add(TKey key, TValue value)
	{
		lock (_lockObject)
		{
			GetWritableDict().Add(key, value);
		}
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		lock (_lockObject)
		{
			GetWritableDict().Add(item.Key, item.Value);
		}
	}

	public void Clear()
	{
		lock (_lockObject)
		{
			GetWritableDict(clearDictionary: true);
		}
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return GetReadOnlyDict().Contains(item);
	}

	public bool ContainsKey(TKey key)
	{
		return GetReadOnlyDict().ContainsKey(key);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		((ICollection<KeyValuePair<TKey, TValue>>)GetReadOnlyDict()).CopyTo(array, arrayIndex);
	}

	public void CopyFrom(IDictionary<TKey, TValue> source)
	{
		if (this == source || source == null || source.Count <= 0)
		{
			return;
		}
		lock (_lockObject)
		{
			IDictionary<TKey, TValue> writableDict = GetWritableDict();
			foreach (KeyValuePair<TKey, TValue> item in source)
			{
				writableDict[item.Key] = item.Value;
			}
		}
	}

	public bool Remove(TKey key)
	{
		lock (_lockObject)
		{
			return GetWritableDict().Remove(key);
		}
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		lock (_lockObject)
		{
			return GetWritableDict().Remove(item);
		}
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		return GetReadOnlyDict().TryGetValue(key, out value);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return GetReadOnlyDict().GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetReadOnlyDict().GetEnumerator();
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(GetReadOnlyDict().GetEnumerator());
	}

	private Dictionary<TKey, TValue> GetReadOnlyDict()
	{
		Dictionary<TKey, TValue> dictionary = _dictReadOnly;
		if (dictionary == null)
		{
			lock (_lockObject)
			{
				dictionary = (_dictReadOnly = _dict);
			}
		}
		return dictionary;
	}

	private IDictionary<TKey, TValue> GetWritableDict(bool clearDictionary = false)
	{
		if (_dictReadOnly == null)
		{
			if (clearDictionary)
			{
				_dict.Clear();
			}
			return _dict;
		}
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>((!clearDictionary) ? (_dict.Count + 1) : 0, _dict.Comparer);
		if (!clearDictionary)
		{
			foreach (KeyValuePair<TKey, TValue> item in _dict)
			{
				dictionary[item.Key] = item.Value;
			}
		}
		_dict = dictionary;
		_dictReadOnly = null;
		return dictionary;
	}
}

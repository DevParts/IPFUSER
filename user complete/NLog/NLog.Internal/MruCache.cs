using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Most-Recently-Used-Cache, that discards less frequently used items on overflow
/// </summary>
internal sealed class MruCache<TKey, TValue>
{
	private struct MruCacheItem
	{
		public readonly TValue Value;

		public readonly long Version;

		public readonly bool Virgin;

		public MruCacheItem(TValue value, long version, bool virgin)
		{
			Value = value;
			Version = version;
			Virgin = virgin;
		}
	}

	private readonly Dictionary<TKey, MruCacheItem> _dictionary;

	private readonly int _maxCapacity;

	private long _currentVersion;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="maxCapacity">Maximum number of items the cache will hold before discarding.</param>
	public MruCache(int maxCapacity)
	{
		_maxCapacity = maxCapacity;
		_dictionary = new Dictionary<TKey, MruCacheItem>(_maxCapacity / 4);
		_currentVersion = 1L;
	}

	/// <summary>
	/// Attempt to insert item into cache.
	/// </summary>
	/// <param name="key">Key of the item to be inserted in the cache.</param>
	/// <param name="value">Value of the item to be inserted in the cache.</param>
	/// <returns><see langword="true" /> when the key does not already exist in the cache, <see langword="false" /> otherwise.</returns>
	public bool TryAddValue(TKey key, TValue value)
	{
		lock (_dictionary)
		{
			if (_dictionary.TryGetValue(key, out var value2))
			{
				long currentVersion = _currentVersion;
				if (value2.Version != currentVersion || !EqualityComparer<TValue>.Default.Equals(value, value2.Value))
				{
					_dictionary[key] = new MruCacheItem(value, currentVersion, virgin: false);
				}
				return false;
			}
			if (_dictionary.Count >= _maxCapacity)
			{
				_currentVersion++;
				PruneCache();
			}
			_dictionary.Add(key, new MruCacheItem(value, _currentVersion, virgin: true));
			return true;
		}
	}

	private void PruneCache()
	{
		long num = _currentVersion - 2;
		long num2 = 1L;
		List<TKey> list = new List<TKey>((int)((double)_dictionary.Count / 2.5));
		for (int i = 0; i < 3; i++)
		{
			long num3 = _currentVersion - 5;
			switch (i)
			{
			case 0:
				num3 = _currentVersion - (int)((double)_maxCapacity / 1.5);
				break;
			case 1:
				num3 = _currentVersion - 10;
				break;
			}
			if (num3 < num2)
			{
				num3 = num2;
			}
			num2 = long.MaxValue;
			foreach (KeyValuePair<TKey, MruCacheItem> item in _dictionary)
			{
				long version = item.Value.Version;
				if (version <= num3 || (item.Value.Virgin && (i != 0 || version < num)))
				{
					list.Add(item.Key);
					if ((double)(_dictionary.Count - list.Count) < (double)_maxCapacity / 1.5)
					{
						i = 3;
						break;
					}
				}
				else if (version < num2)
				{
					num2 = version;
				}
			}
		}
		foreach (TKey item2 in list)
		{
			_dictionary.Remove(item2);
		}
		if (_dictionary.Count >= _maxCapacity)
		{
			_dictionary.Clear();
		}
	}

	/// <summary>
	/// Lookup existing item in cache.
	/// </summary>
	/// <param name="key">Key of the item to be searched in the cache.</param>
	/// <param name="value">Output value of the item found in the cache.</param>
	/// <returns><see langword="true" /> when the key is found in the cache, <see langword="false" /> otherwise.</returns>
	public bool TryGetValue(TKey key, out TValue? value)
	{
		MruCacheItem value2;
		try
		{
			if (!_dictionary.TryGetValue(key, out value2))
			{
				value = default(TValue);
				return false;
			}
		}
		catch
		{
			value2 = default(MruCacheItem);
		}
		if (value2.Version != _currentVersion || value2.Virgin)
		{
			lock (_dictionary)
			{
				long num = _currentVersion;
				if (!_dictionary.TryGetValue(key, out value2))
				{
					value = default(TValue);
					return false;
				}
				if (value2.Version != num || value2.Virgin)
				{
					if (value2.Virgin)
					{
						num = ++_currentVersion;
					}
					_dictionary[key] = new MruCacheItem(value2.Value, num, virgin: false);
				}
			}
		}
		value = value2.Value;
		return true;
	}
}

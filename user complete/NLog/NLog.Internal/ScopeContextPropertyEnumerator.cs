using System;
using System.Collections;
using System.Collections.Generic;

namespace NLog.Internal;

internal struct ScopeContextPropertyEnumerator<TValue> : IEnumerator<KeyValuePair<string, object?>>, IDisposable, IEnumerator
{
	private readonly IEnumerator<KeyValuePair<string, object?>>? _scopeEnumerator;

	private readonly IReadOnlyList<KeyValuePair<string, object?>>? _scopeList;

	private int _scopeIndex;

	private Dictionary<string, object?>.Enumerator _dictionaryEnumerator;

	public KeyValuePair<string, object?> Current
	{
		get
		{
			if (_scopeList != null)
			{
				return _scopeList[_scopeIndex];
			}
			if (_scopeEnumerator != null)
			{
				return _scopeEnumerator.Current;
			}
			return _dictionaryEnumerator.Current;
		}
	}

	object IEnumerator.Current => Current;

	public ScopeContextPropertyEnumerator(IEnumerable<KeyValuePair<string, TValue>> scopeProperties)
	{
		if (scopeProperties is IReadOnlyList<KeyValuePair<string, object>> scopeList)
		{
			_scopeEnumerator = null;
			_scopeList = scopeList;
			_scopeIndex = -1;
			_dictionaryEnumerator = default(Dictionary<string, object>.Enumerator);
			return;
		}
		_scopeList = null;
		_scopeIndex = 0;
		if (scopeProperties is Dictionary<string, object> dictionary)
		{
			_scopeEnumerator = null;
			_dictionaryEnumerator = dictionary.GetEnumerator();
		}
		else if (scopeProperties is IEnumerable<KeyValuePair<string, object>> enumerable)
		{
			_scopeEnumerator = enumerable.GetEnumerator();
			_dictionaryEnumerator = default(Dictionary<string, object>.Enumerator);
		}
		else
		{
			_scopeEnumerator = CreateScopeEnumerable(scopeProperties).GetEnumerator();
			_dictionaryEnumerator = default(Dictionary<string, object>.Enumerator);
		}
	}

	public static void CopyScopePropertiesToDictionary(IReadOnlyCollection<KeyValuePair<string, TValue?>> parentContext, Dictionary<string, object?> scopeDictionary)
	{
		using ScopeContextPropertyEnumerator<TValue> scopeContextPropertyEnumerator = new ScopeContextPropertyEnumerator<TValue>(parentContext);
		while (scopeContextPropertyEnumerator.MoveNext())
		{
			KeyValuePair<string, object> current = scopeContextPropertyEnumerator.Current;
			scopeDictionary[current.Key] = current.Value;
		}
	}

	public static bool HasUniqueCollectionKeys(IEnumerable<KeyValuePair<string, TValue>> scopeProperties, IEqualityComparer<string> keyComparer)
	{
		int num = 1;
		using (ScopeContextPropertyEnumerator<TValue> scopeContextPropertyEnumerator = new ScopeContextPropertyEnumerator<TValue>(scopeProperties))
		{
			while (scopeContextPropertyEnumerator.MoveNext())
			{
				num++;
				string key = scopeContextPropertyEnumerator.Current.Key;
				if (!scopeContextPropertyEnumerator.MoveNext())
				{
					return true;
				}
				num++;
				string key2 = scopeContextPropertyEnumerator.Current.Key;
				if (keyComparer.Equals(key, key2))
				{
					return false;
				}
				int num2 = 0;
				using ScopeContextPropertyEnumerator<TValue> scopeContextPropertyEnumerator2 = new ScopeContextPropertyEnumerator<TValue>(scopeProperties);
				while (scopeContextPropertyEnumerator2.MoveNext())
				{
					if (++num2 >= num)
					{
						if (num2 > 10)
						{
							return false;
						}
						string key3 = scopeContextPropertyEnumerator2.Current.Key;
						if (keyComparer.Equals(key, key3))
						{
							return false;
						}
						if (keyComparer.Equals(key2, key3))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	private static IEnumerable<KeyValuePair<string, object?>> CreateScopeEnumerable(IEnumerable<KeyValuePair<string, TValue>> scopeProperties)
	{
		foreach (KeyValuePair<string, TValue> scopeProperty in scopeProperties)
		{
			yield return new KeyValuePair<string, object>(scopeProperty.Key, scopeProperty.Value);
		}
	}

	public void Dispose()
	{
		if (_scopeEnumerator != null)
		{
			_scopeEnumerator.Dispose();
		}
		else if (_scopeList == null)
		{
			_dictionaryEnumerator.Dispose();
		}
	}

	public bool MoveNext()
	{
		if (_scopeList != null)
		{
			if (_scopeIndex < _scopeList.Count - 1)
			{
				_scopeIndex++;
				return true;
			}
			return false;
		}
		if (_scopeEnumerator != null)
		{
			return _scopeEnumerator.MoveNext();
		}
		return _dictionaryEnumerator.MoveNext();
	}

	public void Reset()
	{
		if (_scopeList != null)
		{
			_scopeIndex = -1;
		}
		else if (_scopeEnumerator != null)
		{
			_scopeEnumerator.Reset();
		}
		else
		{
			_dictionaryEnumerator = default(Dictionary<string, object>.Enumerator);
		}
	}
}

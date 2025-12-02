using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Internal;

[DebuggerDisplay("Count = {Count}")]
internal sealed class ConfigVariablesDictionary : IDictionary<string, Layout>, ICollection<KeyValuePair<string, Layout>>, IEnumerable<KeyValuePair<string, Layout>>, IEnumerable
{
	private readonly ThreadSafeDictionary<string, Layout> _variables = new ThreadSafeDictionary<string, Layout>(StringComparer.OrdinalIgnoreCase);

	private readonly LoggingConfiguration _configuration;

	private ThreadSafeDictionary<string, Layout>? _dynamicVariables;

	private ThreadSafeDictionary<string, bool>? _apiVariables;

	public int Count => _variables.Count;

	public ICollection<string> Keys => _variables.Keys;

	public ICollection<Layout> Values => _variables.Values;

	public Layout this[string key]
	{
		get
		{
			return _variables[key];
		}
		set
		{
			_variables[key] = value;
			RegisterApiVariable(key);
		}
	}

	bool ICollection<KeyValuePair<string, Layout>>.IsReadOnly => false;

	public ConfigVariablesDictionary(LoggingConfiguration configuration)
	{
		_configuration = configuration;
	}

	public void InsertParsedConfigVariable(string key, Layout value, bool keepVariablesOnReload)
	{
		if (keepVariablesOnReload)
		{
			ThreadSafeDictionary<string, bool>? apiVariables = _apiVariables;
			if (apiVariables != null && apiVariables.ContainsKey(key) && _variables.ContainsKey(key))
			{
				return;
			}
		}
		_variables[key] = value;
		_dynamicVariables?.Remove(key);
	}

	public bool TryLookupDynamicVariable(string key, out Layout dynamicLayout)
	{
		if (_dynamicVariables == null)
		{
			if (!_variables.TryGetValue(key, out dynamicLayout))
			{
				return false;
			}
			Interlocked.CompareExchange(ref _dynamicVariables, new ThreadSafeDictionary<string, Layout>(_variables.Comparer), null);
		}
		bool result = true;
		if (!_dynamicVariables.TryGetValue(key, out dynamicLayout))
		{
			result = false;
			if (_variables.TryGetValue(key, out dynamicLayout))
			{
				result = true;
				if (dynamicLayout != null)
				{
					dynamicLayout.Initialize(_configuration);
					_dynamicVariables[key] = dynamicLayout;
				}
			}
		}
		return result;
	}

	public void PrepareForReload(ConfigVariablesDictionary oldVariables)
	{
		if (oldVariables._apiVariables == null)
		{
			return;
		}
		foreach (KeyValuePair<string, bool> apiVariable in oldVariables._apiVariables)
		{
			if (oldVariables._variables.TryGetValue(apiVariable.Key, out Layout value))
			{
				_variables[apiVariable.Key] = value;
				RegisterApiVariable(apiVariable.Key);
			}
		}
	}

	public bool ContainsKey(string key)
	{
		return _variables.ContainsKey(key);
	}

	public bool TryGetValue(string key, out Layout value)
	{
		return _variables.TryGetValue(key, out value);
	}

	IEnumerator<KeyValuePair<string, Layout>> IEnumerable<KeyValuePair<string, Layout>>.GetEnumerator()
	{
		return _variables.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _variables.GetEnumerator();
	}

	public ThreadSafeDictionary<string, Layout>.Enumerator GetEnumerator()
	{
		return _variables.GetEnumerator();
	}

	public void Add(string key, Layout value)
	{
		_variables.Add(key, value);
		RegisterApiVariable(key);
	}

	public bool Remove(string key)
	{
		_apiVariables?.Remove(key);
		_dynamicVariables?.Remove(key);
		return _variables.Remove(key);
	}

	public void Clear()
	{
		_variables.Clear();
		_apiVariables?.Clear();
		_dynamicVariables?.Clear();
	}

	bool ICollection<KeyValuePair<string, Layout>>.Contains(KeyValuePair<string, Layout> item)
	{
		return _variables.Contains(item);
	}

	void ICollection<KeyValuePair<string, Layout>>.CopyTo(KeyValuePair<string, Layout>[] array, int arrayIndex)
	{
		_variables.CopyTo(array, arrayIndex);
	}

	void ICollection<KeyValuePair<string, Layout>>.Add(KeyValuePair<string, Layout> item)
	{
		Add(item.Key, item.Value);
	}

	bool ICollection<KeyValuePair<string, Layout>>.Remove(KeyValuePair<string, Layout> item)
	{
		return Remove(item.Key);
	}

	private void RegisterApiVariable(string key)
	{
		if (_apiVariables == null)
		{
			Interlocked.CompareExchange(ref _apiVariables, new ThreadSafeDictionary<string, bool>(_variables.Comparer), null);
		}
		_apiVariables[key] = true;
		_dynamicVariables?.Remove(key);
	}
}

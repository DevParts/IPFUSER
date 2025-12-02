using System;
using System.Collections.Generic;

namespace NLog.Internal;

internal struct ScopeContextPropertyCollector
{
	private IReadOnlyCollection<KeyValuePair<string, object?>>? _allProperties;

	private List<KeyValuePair<string, object?>> _propertyCollector;

	public bool IsCollectorEmpty
	{
		get
		{
			if (_allProperties != null)
			{
				if (_allProperties.Count == 0)
				{
					return _propertyCollector == null;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsCollectorInactive => _allProperties == null;

	public ScopeContextPropertyCollector(List<KeyValuePair<string, object?>> propertyCollector)
	{
		_allProperties = (_propertyCollector = propertyCollector);
	}

	public IReadOnlyCollection<KeyValuePair<string, object?>> StartCaptureProperties(IScopeContextAsyncState? state)
	{
		while (state != null)
		{
			IReadOnlyCollection<KeyValuePair<string, object>> readOnlyCollection = state.CaptureContextProperties(ref this);
			if (readOnlyCollection != null)
			{
				return readOnlyCollection;
			}
			state = state.Parent;
		}
		return CaptureCompleted(null);
	}

	public IReadOnlyCollection<KeyValuePair<string, object?>> CaptureCompleted(IReadOnlyCollection<KeyValuePair<string, object?>>? properties)
	{
		IReadOnlyCollection<KeyValuePair<string, object?>>? allProperties = _allProperties;
		if (allProperties != null && allProperties.Count > 0)
		{
			if (properties != null && properties.Count > 0)
			{
				if (_propertyCollector == null)
				{
					return _allProperties = MergeUniqueProperties(_allProperties, properties);
				}
				AddProperties(properties);
			}
			return _allProperties = EnsureUniqueProperties(_allProperties);
		}
		if (properties != null && properties.Count > 0)
		{
			return _allProperties = EnsureUniqueProperties(properties);
		}
		return _allProperties = (IReadOnlyCollection<KeyValuePair<string, object?>>?)(object)Array.Empty<KeyValuePair<string, object>>();
	}

	private static Dictionary<string, object?> MergeUniqueProperties(IReadOnlyCollection<KeyValuePair<string, object?>> currentProperties, IReadOnlyCollection<KeyValuePair<string, object?>> properties)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>(currentProperties.Count + properties.Count, ScopeContext.DefaultComparer);
		ScopeContextPropertyEnumerator<object>.CopyScopePropertiesToDictionary(properties, dictionary);
		ScopeContextPropertyEnumerator<object>.CopyScopePropertiesToDictionary(currentProperties, dictionary);
		return dictionary;
	}

	private static IReadOnlyCollection<KeyValuePair<string, object?>> EnsureUniqueProperties(IReadOnlyCollection<KeyValuePair<string, object?>> properties)
	{
		int count = properties.Count;
		if (count > 1)
		{
			if (properties is Dictionary<string, object> dictionary && dictionary.Comparer == ScopeContext.DefaultComparer)
			{
				return properties;
			}
			if (count > 10 || !ScopeContextPropertyEnumerator<object>.HasUniqueCollectionKeys(properties, ScopeContext.DefaultComparer))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>(Math.Min(count, 10), ScopeContext.DefaultComparer);
				ScopeContextPropertyEnumerator<object>.CopyScopePropertiesToDictionary(properties, dictionary2);
				return dictionary2;
			}
		}
		return properties;
	}

	public void AddProperty(string propertyName, object? propertyValue)
	{
		if (_allProperties == null || IsCollectorEmpty)
		{
			_allProperties = (IReadOnlyCollection<KeyValuePair<string, object?>>?)(object)new KeyValuePair<string, object>[1]
			{
				new KeyValuePair<string, object>(propertyName, propertyValue)
			};
		}
		else if (_propertyCollector == null)
		{
			_propertyCollector = new List<KeyValuePair<string, object>>(Math.Max(4, _allProperties.Count + 1));
			_propertyCollector.Add(new KeyValuePair<string, object>(propertyName, propertyValue));
			CollectProperties(_allProperties);
			_allProperties = _propertyCollector;
		}
		else
		{
			_propertyCollector.Insert(0, new KeyValuePair<string, object>(propertyName, propertyValue));
		}
	}

	public void AddProperties(IReadOnlyCollection<KeyValuePair<string, object?>> properties)
	{
		if (_allProperties == null || IsCollectorEmpty)
		{
			_allProperties = properties;
		}
		else
		{
			if (properties == null || properties.Count <= 0)
			{
				return;
			}
			if (_propertyCollector == null)
			{
				_propertyCollector = new List<KeyValuePair<string, object>>(Math.Max(4, _allProperties.Count + properties.Count));
				CollectProperties(properties);
				CollectProperties(_allProperties);
				_allProperties = _propertyCollector;
				return;
			}
			if (_propertyCollector.Count != 0)
			{
				int num = 0;
				using ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = new ScopeContextPropertyEnumerator<object>(properties);
				while (scopeContextPropertyEnumerator.MoveNext())
				{
					KeyValuePair<string, object> current = scopeContextPropertyEnumerator.Current;
					_propertyCollector.Insert(num++, current);
				}
				return;
			}
			CollectProperties(properties);
		}
	}

	private void CollectProperties(IReadOnlyCollection<KeyValuePair<string, object?>> properties)
	{
		using ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = new ScopeContextPropertyEnumerator<object>(properties);
		while (scopeContextPropertyEnumerator.MoveNext())
		{
			KeyValuePair<string, object> current = scopeContextPropertyEnumerator.Current;
			_propertyCollector.Add(current);
		}
	}
}

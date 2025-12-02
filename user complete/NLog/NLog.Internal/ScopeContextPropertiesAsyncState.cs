using System;
using System.Collections;
using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Immutable state for ScopeContext Multiple Properties (MDLC)
/// </summary>
internal sealed class ScopeContextPropertiesAsyncState<TValue> : ScopeContextAsyncState, IScopeContextPropertiesAsyncState, IScopeContextAsyncState, IDisposable, IReadOnlyCollection<KeyValuePair<string, object?>>, IEnumerable<KeyValuePair<string, object?>>, IEnumerable
{
	private readonly IReadOnlyCollection<KeyValuePair<string, TValue?>> _scopeProperties;

	private IReadOnlyCollection<KeyValuePair<string, object?>>? _allProperties;

	public long NestedStateTimestamp { get; }

	public object? NestedState { get; }

	public int Count => _scopeProperties.Count;

	public ScopeContextPropertiesAsyncState(IScopeContextAsyncState? parent, Dictionary<string, object?> allProperties)
		: base(parent)
	{
		_allProperties = allProperties;
		_scopeProperties = (IReadOnlyCollection<KeyValuePair<string, TValue?>>)(object)Array.Empty<KeyValuePair<string, TValue>>();
	}

	public ScopeContextPropertiesAsyncState(IScopeContextAsyncState? parent, Dictionary<string, object?> allProperties, object? nestedState)
		: base(parent)
	{
		_allProperties = allProperties;
		_scopeProperties = (IReadOnlyCollection<KeyValuePair<string, TValue?>>)(object)Array.Empty<KeyValuePair<string, TValue>>();
		NestedState = nestedState;
		NestedStateTimestamp = ScopeContext.GetNestedContextTimestampNow();
	}

	public ScopeContextPropertiesAsyncState(IScopeContextAsyncState? parent, IReadOnlyCollection<KeyValuePair<string, TValue?>> scopeProperties)
		: base(parent)
	{
		_scopeProperties = scopeProperties;
	}

	public ScopeContextPropertiesAsyncState(IScopeContextAsyncState? parent, IReadOnlyCollection<KeyValuePair<string, TValue?>> scopeProperties, object? nestedState)
		: base(parent)
	{
		_scopeProperties = scopeProperties;
		NestedState = nestedState;
		NestedStateTimestamp = ScopeContext.GetNestedContextTimestampNow();
	}

	IList<object>? IScopeContextAsyncState.CaptureNestedContext(ref ScopeContextNestedStateCollector contextCollector)
	{
		if (NestedState == null)
		{
			if (contextCollector.IsCollectorInactive)
			{
				return contextCollector.StartCaptureNestedStates(base.Parent);
			}
			return null;
		}
		if (contextCollector.IsCollectorEmpty)
		{
			if (base.Parent == null)
			{
				return new object[1] { NestedState };
			}
			if (contextCollector.IsCollectorInactive)
			{
				contextCollector.PushNestedState(NestedState);
				return contextCollector.StartCaptureNestedStates(base.Parent);
			}
		}
		contextCollector.PushNestedState(NestedState);
		return null;
	}

	IReadOnlyCollection<KeyValuePair<string, object?>>? IScopeContextAsyncState.CaptureContextProperties(ref ScopeContextPropertyCollector contextCollector)
	{
		if (contextCollector.IsCollectorEmpty)
		{
			if (_allProperties == null)
			{
				contextCollector.AddProperties((_scopeProperties as IReadOnlyCollection<KeyValuePair<string, object>>) ?? this);
				_allProperties = contextCollector.StartCaptureProperties(base.Parent);
			}
			return _allProperties;
		}
		if (_allProperties == null)
		{
			contextCollector.AddProperties((_scopeProperties as IReadOnlyCollection<KeyValuePair<string, object>>) ?? this);
			return null;
		}
		return contextCollector.CaptureCompleted(_allProperties);
	}

	public override string ToString()
	{
		return NestedState?.ToString() ?? $"Count = {Count}";
	}

	IEnumerator<KeyValuePair<string, object?>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
	{
		return new ScopeContextPropertyEnumerator<TValue>(_scopeProperties);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ScopeContextPropertyEnumerator<TValue>(_scopeProperties);
	}
}

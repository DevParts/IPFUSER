using System;
using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Immutable state for ScopeContext Nested State (NDLC)
/// </summary>
internal sealed class ScopedContextNestedAsyncState<T> : ScopeContextAsyncState, IScopeContextAsyncState, IDisposable
{
	private readonly T _value;

	object? IScopeContextAsyncState.NestedState => _value;

	public long NestedStateTimestamp { get; }

	public ScopedContextNestedAsyncState(IScopeContextAsyncState? parent, T state)
		: base(parent)
	{
		NestedStateTimestamp = ScopeContext.GetNestedContextTimestampNow();
		_value = state;
	}

	IList<object>? IScopeContextAsyncState.CaptureNestedContext(ref ScopeContextNestedStateCollector contextCollector)
	{
		object obj = _value;
		if (contextCollector.IsCollectorEmpty)
		{
			if (base.Parent == null)
			{
				if (obj != null)
				{
					return new object[1] { obj };
				}
				return Array.Empty<object>();
			}
			if (contextCollector.IsCollectorInactive)
			{
				if (obj != null)
				{
					contextCollector.PushNestedState(obj);
				}
				return contextCollector.StartCaptureNestedStates(base.Parent);
			}
		}
		if (obj != null)
		{
			contextCollector.PushNestedState(obj);
		}
		return null;
	}

	IReadOnlyCollection<KeyValuePair<string, object?>>? IScopeContextAsyncState.CaptureContextProperties(ref ScopeContextPropertyCollector contextCollector)
	{
		if (contextCollector.IsCollectorInactive)
		{
			if (base.Parent == null)
			{
				return (IReadOnlyCollection<KeyValuePair<string, object?>>?)(object)Array.Empty<KeyValuePair<string, object>>();
			}
			contextCollector.AddProperties((IReadOnlyCollection<KeyValuePair<string, object?>>)(object)Array.Empty<KeyValuePair<string, object>>());
			return contextCollector.StartCaptureProperties(base.Parent);
		}
		return null;
	}

	public override string ToString()
	{
		T value = _value;
		return ((value != null) ? value.ToString() : null) ?? "null";
	}
}

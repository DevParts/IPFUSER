using System;
using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Immutable state for ScopeContext Single Property (MDLC)
/// </summary>
internal sealed class ScopeContextPropertyAsyncState<TValue> : ScopeContextAsyncState, IScopeContextPropertiesAsyncState, IScopeContextAsyncState, IDisposable
{
	private IReadOnlyCollection<KeyValuePair<string, object?>>? _allProperties;

	long IScopeContextAsyncState.NestedStateTimestamp => 0L;

	object? IScopeContextAsyncState.NestedState => null;

	public string Name { get; }

	public TValue? Value { get; }

	public ScopeContextPropertyAsyncState(IScopeContextAsyncState? parent, string name, TValue? value)
		: base(parent)
	{
		Name = name;
		Value = value;
	}

	IList<object>? IScopeContextAsyncState.CaptureNestedContext(ref ScopeContextNestedStateCollector contextCollector)
	{
		if (contextCollector.IsCollectorInactive)
		{
			if (base.Parent == null)
			{
				return Array.Empty<object>();
			}
			return contextCollector.StartCaptureNestedStates(base.Parent);
		}
		return null;
	}

	IReadOnlyCollection<KeyValuePair<string, object?>>? IScopeContextAsyncState.CaptureContextProperties(ref ScopeContextPropertyCollector contextCollector)
	{
		if (contextCollector.IsCollectorEmpty)
		{
			if (_allProperties == null)
			{
				contextCollector.AddProperty(Name, Value);
				_allProperties = contextCollector.StartCaptureProperties(base.Parent);
			}
			return _allProperties;
		}
		if (_allProperties == null)
		{
			contextCollector.AddProperty(Name, Value);
			return null;
		}
		return contextCollector.CaptureCompleted(_allProperties);
	}

	public override string ToString()
	{
		string name = Name;
		TValue value = Value;
		return name + "=" + (((value != null) ? value.ToString() : null) ?? "null");
	}
}

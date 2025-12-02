using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Internal;

/// <summary>
/// Immutable state for ScopeContext handling legacy MDLC + NDLC operations
/// </summary>
[Obsolete("Replaced by ScopeContext.PushProperty / ScopeContext.PushNestedState")]
internal sealed class ScopeContextLegacyAsyncState : ScopeContextAsyncState, IScopeContextAsyncState, IDisposable
{
	public object[] NestedContext { get; }

	public IReadOnlyCollection<KeyValuePair<string, object?>>? MappedContext { get; }

	public long NestedStateTimestamp { get; }

	object? IScopeContextAsyncState.NestedState
	{
		get
		{
			object[] nestedContext = NestedContext;
			if (nestedContext == null || nestedContext.Length == 0)
			{
				return null;
			}
			return NestedContext[0];
		}
	}

	public ScopeContextLegacyAsyncState(IReadOnlyCollection<KeyValuePair<string, object?>>? allProperties, object[] nestedContext, long nestedContextTimestamp)
		: base(null)
	{
		MappedContext = allProperties;
		NestedContext = nestedContext;
		NestedStateTimestamp = nestedContextTimestamp;
	}

	public static void CaptureLegacyContext(IScopeContextAsyncState? contextState, out Dictionary<string, object?> allProperties, out object[] nestedContext, out long nestedContextTimestamp)
	{
		ScopeContextNestedStateCollector contextCollector = default(ScopeContextNestedStateCollector);
		ScopeContextPropertyCollector contextCollector2 = default(ScopeContextPropertyCollector);
		IList<object> list = contextState?.CaptureNestedContext(ref contextCollector) ?? Array.Empty<object>();
		IReadOnlyCollection<KeyValuePair<string, object>> readOnlyCollection = (IReadOnlyCollection<KeyValuePair<string, object>>)(((object)contextState?.CaptureContextProperties(ref contextCollector2)) ?? ((object)Array.Empty<KeyValuePair<string, object>>()));
		allProperties = new Dictionary<string, object>(readOnlyCollection.Count, ScopeContext.DefaultComparer);
		ScopeContextPropertyEnumerator<object>.CopyScopePropertiesToDictionary(readOnlyCollection, allProperties);
		nestedContextTimestamp = 0L;
		if (list.Count > 0)
		{
			for (IScopeContextAsyncState scopeContextAsyncState = contextState; scopeContextAsyncState != null; scopeContextAsyncState = scopeContextAsyncState.Parent)
			{
				if (scopeContextAsyncState.NestedStateTimestamp != 0L)
				{
					nestedContextTimestamp = scopeContextAsyncState.NestedStateTimestamp;
				}
			}
			if (nestedContextTimestamp == 0L)
			{
				nestedContextTimestamp = ScopeContext.GetNestedContextTimestampNow();
			}
			nestedContext = (list as object[]) ?? list.ToArray();
		}
		else
		{
			nestedContext = Array.Empty<object>();
		}
	}

	IList<object> IScopeContextAsyncState.CaptureNestedContext(ref ScopeContextNestedStateCollector contextCollector)
	{
		if (contextCollector.IsCollectorEmpty)
		{
			object[] nestedContext = NestedContext;
			if (nestedContext != null && nestedContext.Length != 0)
			{
				object[] array = new object[NestedContext.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = NestedContext[i];
				}
				return array;
			}
			return Array.Empty<object>();
		}
		for (int j = 0; j < NestedContext.Length; j++)
		{
			contextCollector.PushNestedState(NestedContext[j]);
		}
		return contextCollector.StartCaptureNestedStates(null);
	}

	IReadOnlyCollection<KeyValuePair<string, object?>>? IScopeContextAsyncState.CaptureContextProperties(ref ScopeContextPropertyCollector contextCollector)
	{
		if (contextCollector.IsCollectorEmpty)
		{
			return MappedContext;
		}
		return contextCollector.CaptureCompleted(MappedContext);
	}

	public override string ToString()
	{
		object[] nestedContext = NestedContext;
		if (nestedContext != null && nestedContext.Length != 0)
		{
			return NestedContext[NestedContext.Length - 1]?.ToString() ?? "null";
		}
		return base.ToString();
	}
}

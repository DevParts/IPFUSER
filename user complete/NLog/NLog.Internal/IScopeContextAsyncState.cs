using System;
using System.Collections.Generic;

namespace NLog.Internal;

/// <summary>
/// Immutable state that combines ScopeContext MDLC + NDLC for <see cref="T:System.Threading.AsyncLocal`1" />
/// </summary>
internal interface IScopeContextAsyncState : IDisposable
{
	IScopeContextAsyncState? Parent { get; }

	object? NestedState { get; }

	long NestedStateTimestamp { get; }

	IReadOnlyCollection<KeyValuePair<string, object?>>? CaptureContextProperties(ref ScopeContextPropertyCollector contextCollector);

	IList<object>? CaptureNestedContext(ref ScopeContextNestedStateCollector contextCollector);
}

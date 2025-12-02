using System;

namespace NLog.Internal;

/// <summary>
/// Immutable state for ScopeContext Mapped Context (MDLC)
/// </summary>
internal interface IScopeContextPropertiesAsyncState : IScopeContextAsyncState, IDisposable
{
}

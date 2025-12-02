using System;

namespace NLog.Internal;

/// <summary>
/// Immutable state that combines ScopeContext MDLC + NDLC for <see cref="T:System.Threading.AsyncLocal`1" />
/// </summary>
internal abstract class ScopeContextAsyncState : IDisposable
{
	private bool _disposed;

	public IScopeContextAsyncState? Parent { get; }

	protected ScopeContextAsyncState(IScopeContextAsyncState? parent)
	{
		Parent = parent;
	}

	void IDisposable.Dispose()
	{
		if (!_disposed)
		{
			ScopeContext.SetAsyncLocalContext(Parent);
			_disposed = true;
		}
	}
}

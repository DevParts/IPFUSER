using System;
using System.Threading;
using NLog.Common;

namespace NLog.Internal;

/// <summary>
/// Implements a single-call guard around given continuation function.
/// </summary>
internal sealed class SingleCallContinuation
{
	internal static readonly AsyncContinuation Completed = new SingleCallContinuation(null).CompletedFunction;

	private AsyncContinuation? _asyncContinuation;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Internal.SingleCallContinuation" /> class.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	public SingleCallContinuation(AsyncContinuation? asyncContinuation)
	{
		_asyncContinuation = asyncContinuation;
	}

	/// <summary>
	/// Continuation function which implements the single-call guard.
	/// </summary>
	/// <param name="exception">The exception.</param>
	public void Function(Exception? exception)
	{
		try
		{
			Interlocked.Exchange(ref _asyncContinuation, null)?.Invoke(exception);
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "Exception in asynchronous handler.");
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
	}

	private void CompletedFunction(Exception? exception)
	{
	}
}

using System;
using System.Threading;
using NLog.Common;

namespace NLog.Internal;

/// <summary>
/// Wraps <see cref="T:NLog.Common.AsyncContinuation" /> with a timeout.
/// </summary>
[Obsolete("Marked obsolete on NLog 6.0")]
internal sealed class TimeoutContinuation : IDisposable
{
	private AsyncContinuation? _asyncContinuation;

	private Timer? _timeoutTimer;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Internal.TimeoutContinuation" /> class.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	/// <param name="timeout">The timeout.</param>
	public TimeoutContinuation(AsyncContinuation asyncContinuation, TimeSpan timeout)
	{
		_asyncContinuation = asyncContinuation;
		_timeoutTimer = new Timer(TimerElapsed, null, (int)timeout.TotalMilliseconds, -1);
	}

	/// <summary>
	/// Continuation function which implements the timeout logic.
	/// </summary>
	/// <param name="exception">The exception.</param>
	public void Function(Exception? exception)
	{
		try
		{
			AsyncContinuation? asyncContinuation = Interlocked.Exchange(ref _asyncContinuation, null);
			StopTimer();
			asyncContinuation?.Invoke(exception);
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

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		StopTimer();
	}

	private void StopTimer()
	{
		Interlocked.Exchange(ref _timeoutTimer, null)?.WaitForDispose(TimeSpan.Zero);
	}

	private void TimerElapsed(object state)
	{
		Function(new TimeoutException("Timeout."));
	}
}

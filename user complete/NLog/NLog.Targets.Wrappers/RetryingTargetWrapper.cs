using System;
using System.Collections.Generic;
using System.Threading;
using NLog.Common;
using NLog.Layouts;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Retries in case of write error.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/RetryingWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/RetryingWrapper-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes each write attempt to be repeated 3 times,
/// sleeping 1 second between attempts if first one fails.</p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/RetryingWrapper/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/RetryingWrapper/Simple/Example.cs" />
/// </example>
[Target("RetryingWrapper", IsWrapper = true)]
public class RetryingTargetWrapper : WrapperTargetBase
{
	/// <summary>
	/// Special SyncObject to allow closing down Target while busy retrying
	/// </summary>
	private readonly object _retrySyncObject = new object();

	/// <summary>
	/// Gets or sets the number of retries that should be attempted on the wrapped target in case of a failure.
	/// </summary>
	/// <remarks>Default: <see langword="3" /></remarks>
	/// <docgen category="Retrying Options" order="10" />
	public Layout<int> RetryCount { get; set; }

	/// <summary>
	/// Gets or sets the time to wait between retries in milliseconds.
	/// </summary>
	/// <remarks>Default: <see langword="100" />ms</remarks>
	/// <docgen category="Retrying Options" order="10" />
	public Layout<int> RetryDelayMilliseconds { get; set; }

	/// <summary>
	/// Gets or sets whether to enable batching, and only apply single delay when a whole batch fails
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Retrying Options" order="10" />
	public bool EnableBatchWrite { get; set; } = true;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RetryingTargetWrapper" /> class.
	/// </summary>
	public RetryingTargetWrapper()
	{
		RetryCount = 3;
		RetryDelayMilliseconds = 100;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RetryingTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="retryCount">The retry count.</param>
	/// <param name="retryDelayMilliseconds">The retry delay milliseconds.</param>
	public RetryingTargetWrapper(string name, Target wrappedTarget, int retryCount, int retryDelayMilliseconds)
		: this(wrappedTarget, retryCount, retryDelayMilliseconds)
	{
		base.Name = name ?? base.Name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RetryingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="retryCount">The retry count.</param>
	/// <param name="retryDelayMilliseconds">The retry delay milliseconds.</param>
	public RetryingTargetWrapper(Target wrappedTarget, int retryCount, int retryDelayMilliseconds)
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		RetryCount = retryCount;
		RetryDelayMilliseconds = retryDelayMilliseconds;
	}

	/// <summary>
	/// Writes the specified log event to the wrapped target, retrying and pausing in case of an error.
	/// </summary>
	/// <param name="logEvents">The log event.</param>
	protected override void WriteAsyncThreadSafe(IList<AsyncLogEventInfo> logEvents)
	{
		if (logEvents.Count == 1)
		{
			WriteAsyncThreadSafe(logEvents[0]);
			return;
		}
		if (EnableBatchWrite)
		{
			int initialSleep = 1;
			Func<int, bool> sleepBeforeRetry = (int retryNumber) => retryNumber > 1 || Interlocked.Exchange(ref initialSleep, 0) == 1;
			for (int num = 0; num < logEvents.Count; num++)
			{
				logEvents[num] = WrapWithRetry(logEvents[num], sleepBeforeRetry);
			}
			lock (_retrySyncObject)
			{
				base.WrappedTarget?.WriteAsyncLogEvents(logEvents);
				return;
			}
		}
		lock (_retrySyncObject)
		{
			for (int num2 = 0; num2 < logEvents.Count; num2++)
			{
				WriteAsyncThreadSafe(logEvents[num2]);
			}
		}
	}

	/// <summary>
	/// Writes the specified log event to the wrapped target in a thread-safe manner.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	protected override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
	{
		lock (_retrySyncObject)
		{
			Write(logEvent);
		}
	}

	/// <summary>
	/// Writes the specified log event to the wrapped target, retrying and pausing in case of an error.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		base.WrappedTarget?.WriteAsyncLogEvent(WrapWithRetry(logEvent, (int retryNumber) => true));
	}

	private AsyncLogEventInfo WrapWithRetry(AsyncLogEventInfo logEvent, Func<int, bool> sleepBeforeRetry)
	{
		AsyncContinuation continuation = null;
		int counter = 0;
		continuation = delegate(Exception? ex)
		{
			if (ex == null)
			{
				logEvent.Continuation(null);
			}
			else
			{
				int num = Interlocked.Increment(ref counter);
				int num2 = RetryCount.RenderValue(logEvent.LogEvent, 0);
				int num3 = RetryDelayMilliseconds.RenderValue(logEvent.LogEvent, 0);
				InternalLogger.Warn(ex, "{0}: Error while writing to '{1}'. Try {2}/{3}", this, base.WrappedTarget, num, num2);
				if (num >= num2)
				{
					InternalLogger.Warn("{0}: Too many retries. Aborting.", this);
					logEvent.Continuation(ex);
				}
				else
				{
					if (sleepBeforeRetry(num))
					{
						int num4 = 0;
						while (num4 < num3)
						{
							int num5 = Math.Min(100, num3 - num4);
							AsyncHelpers.WaitForDelay(TimeSpan.FromMilliseconds(num5));
							num4 += num5;
							if (!base.IsInitialized)
							{
								InternalLogger.Warn("{0}): Target closed. Aborting.", this);
								logEvent.Continuation(ex);
								return;
							}
						}
					}
					base.WrappedTarget?.WriteAsyncLogEvent((continuation == null) ? logEvent : logEvent.LogEvent.WithContinuation(continuation));
				}
			}
		};
		return logEvent.LogEvent.WithContinuation(continuation);
	}
}

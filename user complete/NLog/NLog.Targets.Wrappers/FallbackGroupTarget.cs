using System;
using System.Collections.Generic;
using System.Threading;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Provides fallback-on-error.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/FallbackGroup-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/FallbackGroup-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes the messages to be written to server1,
/// and if it fails, messages go to server2.</p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/FallbackGroup/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/FallbackGroup/Simple/Example.cs" />
/// </example>
[Target("FallbackGroup", IsCompound = true)]
public class FallbackGroupTarget : CompoundTargetBase
{
	private long _currentTarget;

	/// <summary>
	/// Gets or sets a value indicating whether to return to the first target after any successful write.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Fallback Options" order="10" />
	public bool ReturnToFirstOnSuccess { get; set; }

	/// <summary>
	/// Gets or sets whether to enable batching, but fallback will be handled individually
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Fallback Options" order="50" />
	public bool EnableBatchWrite { get; set; } = true;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.FallbackGroupTarget" /> class.
	/// </summary>
	public FallbackGroupTarget()
		: this(ArrayHelper.Empty<Target>())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.FallbackGroupTarget" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="targets">The targets.</param>
	public FallbackGroupTarget(string name, params Target[] targets)
		: this(targets)
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.FallbackGroupTarget" /> class.
	/// </summary>
	/// <param name="targets">The targets.</param>
	public FallbackGroupTarget(params Target[] targets)
		: base(targets)
	{
	}

	/// <summary>
	/// Forwards the log event to the sub-targets until one of them succeeds.
	/// </summary>
	/// <param name="logEvents">The log event.</param>
	protected override void WriteAsyncThreadSafe(IList<AsyncLogEventInfo> logEvents)
	{
		if (logEvents.Count == 1)
		{
			WriteAsyncThreadSafe(logEvents[0]);
		}
		else if (EnableBatchWrite)
		{
			int num = (int)Interlocked.Read(ref _currentTarget);
			for (int i = 0; i < logEvents.Count; i++)
			{
				logEvents[i] = WrapWithFallback(logEvents[i], num);
			}
			base.Targets[num].WriteAsyncLogEvents(logEvents);
		}
		else
		{
			for (int j = 0; j < logEvents.Count; j++)
			{
				WriteAsyncThreadSafe(logEvents[j]);
			}
		}
	}

	/// <inheritdoc />
	protected override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
	{
		Write(logEvent);
	}

	/// <summary>
	/// Forwards the log event to the sub-targets until one of them succeeds.
	/// </summary>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		int num = (int)Interlocked.Read(ref _currentTarget);
		AsyncLogEventInfo logEvent2 = WrapWithFallback(logEvent, num);
		base.Targets[num].WriteAsyncLogEvent(logEvent2);
	}

	private AsyncLogEventInfo WrapWithFallback(AsyncLogEventInfo logEvent, int targetToInvoke)
	{
		for (int i = 0; i < base.Targets.Count; i++)
		{
			if (i != targetToInvoke)
			{
				base.Targets[i].PrecalculateVolatileLayouts(logEvent.LogEvent);
			}
		}
		AsyncContinuation continuation = null;
		int tryCounter = 0;
		continuation = delegate(Exception? ex)
		{
			if (ex == null)
			{
				if (ReturnToFirstOnSuccess && Interlocked.Read(ref _currentTarget) != 0L)
				{
					InternalLogger.Debug("{0}: Target '{1}' succeeded. Returning to the first one.", this, base.Targets[targetToInvoke]);
					Interlocked.Exchange(ref _currentTarget, 0L);
				}
				logEvent.Continuation(null);
			}
			else
			{
				tryCounter++;
				int num = (targetToInvoke + 1) % base.Targets.Count;
				Interlocked.CompareExchange(ref _currentTarget, num, targetToInvoke);
				if (tryCounter < base.Targets.Count)
				{
					InternalLogger.Warn(ex, "{0}: Target '{1}' failed. Fallback to next: `{2}`", this, base.Targets[targetToInvoke], base.Targets[num]);
					targetToInvoke = num;
					base.Targets[targetToInvoke].WriteAsyncLogEvent((continuation == null) ? logEvent : logEvent.LogEvent.WithContinuation(continuation));
				}
				else
				{
					InternalLogger.Warn(ex, "{0}: Target '{1}' failed. Fallback not possible", this, base.Targets[targetToInvoke]);
					logEvent.Continuation(ex);
				}
			}
		};
		return logEvent.LogEvent.WithContinuation(continuation);
	}
}

using System;
using System.Collections.Generic;
using System.Threading;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Writes log events to all targets.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/SplitGroup-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/SplitGroup-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes the messages to be written to both file1.txt or file2.txt
/// </p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/SplitGroup/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/SplitGroup/Simple/Example.cs" />
/// </example>
[Target("SplitGroup", IsCompound = true)]
public class SplitGroupTarget : CompoundTargetBase
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.SplitGroupTarget" /> class.
	/// </summary>
	public SplitGroupTarget()
		: this(ArrayHelper.Empty<Target>())
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.SplitGroupTarget" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="targets">The targets.</param>
	public SplitGroupTarget(string name, params Target[] targets)
		: this(targets)
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.SplitGroupTarget" /> class.
	/// </summary>
	/// <param name="targets">The targets.</param>
	public SplitGroupTarget(params Target[] targets)
		: base(targets)
	{
	}

	/// <summary>
	/// Forwards the specified log event to all sub-targets.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		if (base.Targets.Count == 0)
		{
			logEvent.Continuation(null);
			return;
		}
		if (base.Targets.Count > 1)
		{
			logEvent = logEvent.LogEvent.WithContinuation(CreateCountedContinuation(logEvent.Continuation, base.Targets.Count));
		}
		for (int i = 0; i < base.Targets.Count; i++)
		{
			base.Targets[i].WriteAsyncLogEvent(logEvent);
		}
	}

	/// <summary>
	/// Writes an array of logging events to the log target. By default it iterates on all
	/// events and passes them to "Write" method. Inheriting classes can use this method to
	/// optimize batch writes.
	/// </summary>
	/// <param name="logEvents">Logging events to be written out.</param>
	protected override void Write(IList<AsyncLogEventInfo> logEvents)
	{
		InternalLogger.Trace("{0}: Writing {1} events", this, logEvents.Count);
		if (logEvents.Count == 1)
		{
			Write(logEvents[0]);
			return;
		}
		if (base.Targets.Count == 0 || logEvents.Count == 0)
		{
			for (int i = 0; i < logEvents.Count; i++)
			{
				logEvents[i].Continuation(null);
			}
			return;
		}
		if (base.Targets.Count > 1)
		{
			for (int j = 0; j < logEvents.Count; j++)
			{
				AsyncLogEventInfo asyncLogEventInfo = logEvents[j];
				logEvents[j] = asyncLogEventInfo.LogEvent.WithContinuation(CreateCountedContinuation(asyncLogEventInfo.Continuation, base.Targets.Count));
			}
		}
		for (int k = 0; k < base.Targets.Count; k++)
		{
			InternalLogger.Trace("{0}: Sending {1} events to {2}", this, logEvents.Count, base.Targets[k]);
			IList<AsyncLogEventInfo> logEvents2 = logEvents;
			if (k < base.Targets.Count - 1)
			{
				AsyncLogEventInfo[] array = new AsyncLogEventInfo[logEvents.Count];
				logEvents.CopyTo(array, 0);
				logEvents2 = array;
			}
			base.Targets[k].WriteAsyncLogEvents(logEvents2);
		}
	}

	private static AsyncContinuation CreateCountedContinuation(AsyncContinuation originalContinuation, int targetCounter)
	{
		List<Exception> exceptions = new List<Exception>();
		return delegate(Exception? ex)
		{
			if (ex != null)
			{
				lock (exceptions)
				{
					exceptions.Add(ex);
				}
			}
			int num = Interlocked.Decrement(ref targetCounter);
			if (num == 0)
			{
				Exception combinedException = AsyncHelpers.GetCombinedException(exceptions);
				InternalLogger.Trace("SplitGroup: Combined exception: {0}", combinedException);
				originalContinuation(combinedException);
			}
			else
			{
				InternalLogger.Trace("SplitGroup: {0} remaining.", num);
			}
		};
	}
}

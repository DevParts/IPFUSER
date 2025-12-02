using System;
using NLog.Common;
using NLog.Layouts;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Limits the number of messages written per timespan to the wrapped target.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/LimitingWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/LimitingWrapper-target">Documentation on NLog Wiki</seealso>
[Target("LimitingWrapper", IsWrapper = true)]
public class LimitingTargetWrapper : WrapperTargetBase
{
	private DateTime _firstWriteInInterval;

	/// <summary>
	/// Gets or sets the maximum allowed number of messages written per <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.Interval" />.
	/// </summary>
	/// <remarks>Default: <see langword="1000" /> . Messages received after <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessageLimit" /> has been reached in the current <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.Interval" /> will be discarded.</remarks>
	/// <docgen category="General Options" order="10" />
	public Layout<int> MessageLimit { get; set; }

	/// <summary>
	/// Gets or sets the interval in which messages will be written up to the <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessageLimit" /> number of messages.
	/// </summary>
	/// <remarks>Default: <see langword="1" /> hour. Messages received after <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessageLimit" /> has been reached in the current <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.Interval" /> will be discarded.</remarks>
	/// <docgen category="General Options" order="10" />
	public Layout<TimeSpan> Interval { get; set; }

	/// <summary>
	/// Gets the number of <see cref="T:NLog.Common.AsyncLogEventInfo" /> written in the current <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.Interval" />.
	/// </summary>
	/// <docgen category="General Options" order="10" />
	public int MessagesWrittenCount { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.LimitingTargetWrapper" /> class.
	/// </summary>
	public LimitingTargetWrapper()
	{
		MessageLimit = 1000;
		Interval = TimeSpan.FromHours(1.0);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.LimitingTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">The name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public LimitingTargetWrapper(string name, Target wrappedTarget)
		: this(wrappedTarget, 1000, TimeSpan.FromHours(1.0))
	{
		base.Name = name ?? base.Name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.LimitingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public LimitingTargetWrapper(Target wrappedTarget)
		: this(wrappedTarget, 1000, TimeSpan.FromHours(1.0))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.LimitingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="messageLimit">Maximum number of messages written per interval.</param>
	/// <param name="interval">Interval in which the maximum number of messages can be written.</param>
	public LimitingTargetWrapper(Target wrappedTarget, int messageLimit, TimeSpan interval)
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		MessageLimit = messageLimit;
		Interval = interval;
	}

	/// <summary>
	/// Initializes the target and resets the current Interval and <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessagesWrittenCount" />.
	///  </summary>
	protected override void InitializeTarget()
	{
		if (MessageLimit.IsFixed && MessageLimit.FixedValue <= 0)
		{
			throw new NLogConfigurationException("The LimitingTargetWrapper's MessageLimit property must be > 0.");
		}
		if (Interval.IsFixed && Interval.FixedValue <= TimeSpan.Zero)
		{
			throw new NLogConfigurationException("The LimitingTargetWrapper's property Interval must be > 0.");
		}
		base.InitializeTarget();
		ResetInterval(DateTime.MinValue);
		InternalLogger.Trace("{0}: Initialized with MessageLimit={1} and Interval={2}.", this, MessageLimit, Interval);
	}

	/// <summary>
	/// Writes log event to the wrapped target if the current <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessagesWrittenCount" /> is lower than <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessageLimit" />.
	/// If the <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessageLimit" /> is already reached, no log event will be written to the wrapped target.
	/// <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.MessagesWrittenCount" /> resets when the current <see cref="P:NLog.Targets.Wrappers.LimitingTargetWrapper.Interval" /> is expired.
	/// </summary>
	/// <param name="logEvent">Log event to be written out.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		int num = RenderLogEvent(MessageLimit, logEvent.LogEvent, 0);
		TimeSpan timeSpan = RenderLogEvent(Interval, logEvent.LogEvent);
		if (logEvent.LogEvent.TimeStamp - _firstWriteInInterval > timeSpan)
		{
			ResetInterval(logEvent.LogEvent.TimeStamp);
			InternalLogger.Debug("{0}: New interval of '{1}' started.", this, timeSpan);
		}
		if (num <= 0 || MessagesWrittenCount < num)
		{
			base.WrappedTarget?.WriteAsyncLogEvent(logEvent);
			MessagesWrittenCount++;
		}
		else
		{
			logEvent.Continuation(null);
			InternalLogger.Trace("{0}: Discarded event, because MessageLimit of '{1}' was reached.", this, num);
		}
	}

	private void ResetInterval(DateTime timestamp)
	{
		_firstWriteInInterval = timestamp;
		MessagesWrittenCount = 0;
	}
}

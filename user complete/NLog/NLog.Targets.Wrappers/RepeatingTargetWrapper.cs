using NLog.Common;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Repeats each log event the specified number of times.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/RepeatingWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/RepeatingWrapper-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes each log message to be repeated 3 times.</p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/RepeatingWrapper/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/RepeatingWrapper/Simple/Example.cs" />
/// </example>
[Target("RepeatingWrapper", IsWrapper = true)]
public class RepeatingTargetWrapper : WrapperTargetBase
{
	/// <summary>
	/// Gets or sets the number of times to repeat each log message.
	/// </summary>
	/// <remarks>Default: <see langword="3" /></remarks>
	/// <docgen category="Repeating Options" order="10" />
	public int RepeatCount { get; set; } = 3;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RepeatingTargetWrapper" /> class.
	/// </summary>
	public RepeatingTargetWrapper()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RepeatingTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="repeatCount">The repeat count.</param>
	public RepeatingTargetWrapper(string name, Target wrappedTarget, int repeatCount)
		: this(wrappedTarget, repeatCount)
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.RepeatingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="repeatCount">The repeat count.</param>
	public RepeatingTargetWrapper(Target wrappedTarget, int repeatCount)
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		RepeatCount = repeatCount;
	}

	/// <summary>
	/// Forwards the log message to the <see cref="P:NLog.Targets.Wrappers.WrapperTargetBase.WrappedTarget" /> by calling the <see cref="M:NLog.Targets.Target.Write(NLog.LogEventInfo)" /> method <see cref="P:NLog.Targets.Wrappers.RepeatingTargetWrapper.RepeatCount" /> times.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		AsyncHelpers.Repeat(RepeatCount, logEvent.Continuation, delegate(AsyncContinuation cont)
		{
			base.WrappedTarget?.WriteAsyncLogEvent(logEvent.LogEvent.WithContinuation(cont));
		});
	}
}

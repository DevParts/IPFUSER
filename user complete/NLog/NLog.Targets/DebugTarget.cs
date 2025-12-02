namespace NLog.Targets;

/// <summary>
/// Mock target - useful for testing.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/Debug-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/Debug-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/Debug/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/Debug/Simple/Example.cs" />
/// </example>
[Target("Debug")]
public sealed class DebugTarget : TargetWithLayout
{
	/// <summary>
	/// Gets the number of times this target has been called.
	/// </summary>
	/// <docgen category="Debugging Options" order="10" />
	public int Counter { get; private set; }

	/// <summary>
	/// Gets the last message rendered by this target.
	/// </summary>
	/// <docgen category="Debugging Options" order="10" />
	public string LastMessage { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.DebugTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	public DebugTarget()
	{
		LastMessage = string.Empty;
		Counter = 0;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.DebugTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	/// <param name="name">Name of the target.</param>
	public DebugTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		Counter++;
		LastMessage = RenderLogEvent(Layout, logEvent);
	}
}

using System.Text;

namespace NLog.Targets;

/// <summary>
/// Discards log messages. Used mainly for debugging and benchmarking.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/Null-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/Null-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/Null/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/Null/Simple/Example.cs" />
/// </example>
[Target("Null")]
public sealed class NullTarget : TargetWithLayout
{
	private int _logEventCounter;

	private StringBuilder? _lastMesageBuilder;

	private LogEventInfo? _lastLogEvent;

	/// <summary>
	/// Gets the number of times this target has been called.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public int LogEventCounter => _logEventCounter;

	/// <summary>
	/// Gets or sets a value indicating whether to render <see cref="P:NLog.Targets.TargetWithLayout.Layout" /> for LogEvent
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool FormatMessage { get; set; }

	/// <summary>
	/// Gets the last message rendered by this target.
	/// </summary>
	/// <remarks>Requires <see cref="P:NLog.Targets.NullTarget.FormatMessage" /> = <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string LastMessage => _lastMesageBuilder?.ToString() ?? string.Empty;

	/// <summary>
	/// Gets the last LogEvent rendered by this target.
	/// </summary>
	/// <remarks>Requires <see cref="P:NLog.Targets.NullTarget.FormatMessage" /> = <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public LogEventInfo? LastLogEvent => _lastLogEvent;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.NullTarget" /> class.
	/// </summary>
	public NullTarget()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.NullTarget" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	public NullTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		_logEventCounter++;
		if (FormatMessage)
		{
			_lastLogEvent = logEvent;
			StringBuilder stringBuilder = _lastMesageBuilder ?? (_lastMesageBuilder = new StringBuilder(128));
			stringBuilder.Length = 0;
			Layout?.Render(logEvent, stringBuilder);
		}
	}
}

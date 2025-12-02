#define DEBUG
using System.Diagnostics;

namespace NLog.Targets;

/// <summary>
/// Outputs log messages through <see cref="M:System.Diagnostics.Debug.WriteLine(System.String)" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/DebugSystem-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/DebugSystem-target">Documentation on NLog Wiki</seealso>
[Target("DebugSystem")]
public sealed class DebugSystemTarget : TargetWithLayoutHeaderAndFooter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.DebugSystemTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	public DebugSystemTarget()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.DebugSystemTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	/// <param name="name">Name of the target.</param>
	public DebugSystemTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		if (base.Header != null)
		{
			DebugWriteLine(RenderLogEvent(base.Header, LogEventInfo.CreateNullEvent()));
		}
	}

	/// <inheritdoc />
	protected override void CloseTarget()
	{
		if (base.Footer != null)
		{
			DebugWriteLine(RenderLogEvent(base.Footer, LogEventInfo.CreateNullEvent()));
		}
		base.CloseTarget();
	}

	/// <summary>
	/// Outputs the rendered logging event through <see cref="M:System.Diagnostics.Debug.WriteLine(System.String)" />
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	protected override void Write(LogEventInfo logEvent)
	{
		DebugWriteLine(RenderLogEvent(Layout, logEvent));
	}

	private static void DebugWriteLine(string message)
	{
		Debug.WriteLine(message);
	}
}

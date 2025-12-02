using System;
using System.Diagnostics;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets;

/// <summary>
/// Writes log messages to the attached managed debugger.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Debugger-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Debugger-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/Debugger/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/Debugger/Simple/Example.cs" />
/// </example>
[Target("Debugger")]
public sealed class DebuggerTarget : TargetWithLayoutHeaderAndFooter
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.DebuggerTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	public DebuggerTarget()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.DebuggerTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	/// <param name="name">Name of the target.</param>
	public DebuggerTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		if (!Debugger.IsLogging())
		{
			InternalLogger.Debug("{0}: System.Diagnostics.Debugger.IsLogging()==false. Output has been disabled.", this);
		}
		if (base.Header != null)
		{
			Debugger.Log(LogLevel.Off.Ordinal, string.Empty, RenderLogEvent(base.Header, LogEventInfo.CreateNullEvent()) + "\n");
		}
	}

	/// <inheritdoc />
	protected override void CloseTarget()
	{
		if (base.Footer != null)
		{
			Debugger.Log(LogLevel.Off.Ordinal, string.Empty, RenderLogEvent(base.Footer, LogEventInfo.CreateNullEvent()) + "\n");
		}
		base.CloseTarget();
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		if (Debugger.IsLogging())
		{
			ReusableObjectCreator<System.Text.StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
			string message;
			try
			{
				Layout.Render(logEvent, lockOject.Result);
				lockOject.Result.Append('\n');
				message = lockOject.Result.ToString();
			}
			finally
			{
				((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
			}
			Debugger.Log(logEvent.Level.Ordinal, logEvent.LoggerName, message);
		}
	}
}

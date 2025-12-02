using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;

namespace NLog.LayoutRenderers;

/// <summary>
/// The identifier of the current process.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ProcessId-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ProcessId-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("processid")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class ProcessIdLayoutRenderer : LayoutRenderer, IRawValue
{
	private readonly int _processId;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ProcessIdLayoutRenderer" /> class.
	/// </summary>
	public ProcessIdLayoutRenderer()
		: this(LogFactory.DefaultAppEnvironment)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ProcessIdLayoutRenderer" /> class.
	/// </summary>
	internal ProcessIdLayoutRenderer(IAppEnvironment appEnvironment)
	{
		_processId = appEnvironment.CurrentProcessId;
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.AppendInvariant(_processId);
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = _processId;
		return true;
	}
}

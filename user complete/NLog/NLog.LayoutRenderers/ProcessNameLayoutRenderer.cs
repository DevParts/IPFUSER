using System.Text;
using NLog.Config;
using NLog.Internal.Fakeables;

namespace NLog.LayoutRenderers;

/// <summary>
/// The name of the current process.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ProcessName-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ProcessName-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("processname")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class ProcessNameLayoutRenderer : LayoutRenderer
{
	private readonly string _processFilePath;

	private readonly string _processBaseName;

	/// <summary>
	/// Gets or sets a value indicating whether to write the full path to the process executable.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool FullName { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ProcessNameLayoutRenderer" /> class.
	/// </summary>
	public ProcessNameLayoutRenderer()
		: this(LogFactory.DefaultAppEnvironment)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ProcessNameLayoutRenderer" /> class.
	/// </summary>
	internal ProcessNameLayoutRenderer(IAppEnvironment appEnvironment)
	{
		_processFilePath = appEnvironment.CurrentProcessFilePath;
		_processBaseName = appEnvironment.CurrentProcessBaseName;
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string value = (FullName ? _processFilePath : _processBaseName);
		builder.Append(value);
	}
}

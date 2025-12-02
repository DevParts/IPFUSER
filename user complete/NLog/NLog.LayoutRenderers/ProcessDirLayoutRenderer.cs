using System.IO;
using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;

namespace NLog.LayoutRenderers;

/// <summary>
/// The executable directory from the <see cref="P:System.Diagnostics.Process.MainModule" /> FileName,
/// using the current process <see cref="M:System.Diagnostics.Process.GetCurrentProcess" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ProcessDir-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ProcessDir-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("processdir")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class ProcessDirLayoutRenderer : LayoutRenderer
{
	private readonly string _processDir;

	private string? _nlogCombinedPath;

	/// <summary>
	/// Gets or sets the name of the file to be Path.Combine()'d with the directory name.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Advanced Options" order="50" />
	public string File { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the name of the directory to be Path.Combine()'d with the directory name.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Advanced Options" order="50" />
	public string Dir { get; set; } = string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ProcessDirLayoutRenderer" /> class.
	/// </summary>
	public ProcessDirLayoutRenderer()
		: this(LogFactory.DefaultAppEnvironment)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ProcessDirLayoutRenderer" /> class.
	/// </summary>
	internal ProcessDirLayoutRenderer(IAppEnvironment appEnvironment)
	{
		_processDir = Path.GetDirectoryName(appEnvironment.CurrentProcessFilePath);
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		_nlogCombinedPath = null;
		base.InitializeLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string value = _nlogCombinedPath ?? (_nlogCombinedPath = PathHelpers.CombinePaths(_processDir, Dir, File));
		builder.Append(value);
	}
}

using System.IO;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// A temporary directory.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/TempDir-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/TempDir-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("tempdir")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class TempDirLayoutRenderer : LayoutRenderer
{
	private static string? _tempDir;

	private string? _nlogCombinedPath;

	private static string TempDir => _tempDir ?? (_tempDir = Path.GetTempPath());

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

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		_nlogCombinedPath = null;
		base.InitializeLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string value = _nlogCombinedPath ?? (_nlogCombinedPath = PathHelpers.CombinePaths(TempDir, Dir, File));
		builder.Append(value);
	}
}

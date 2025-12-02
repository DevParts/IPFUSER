using System.IO;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The directory where NLog.dll is located.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/NLogDir-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/NLogDir-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("nlogdir")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class NLogDirLayoutRenderer : LayoutRenderer
{
	private static string? _nlogDir;

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

	private static string NLogDir => _nlogDir ?? (_nlogDir = ResolveNLogDir());

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		_nlogCombinedPath = null;
		base.InitializeLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string value = _nlogCombinedPath ?? (_nlogCombinedPath = PathHelpers.CombinePaths(NLogDir, Dir, File));
		builder.Append(value);
	}

	private static string ResolveNLogDir()
	{
		string assemblyFileLocation = AssemblyHelpers.GetAssemblyFileLocation(typeof(LogFactory).Assembly);
		if (!string.IsNullOrEmpty(assemblyFileLocation))
		{
			return Path.GetDirectoryName(assemblyFileLocation);
		}
		return string.Empty;
	}
}

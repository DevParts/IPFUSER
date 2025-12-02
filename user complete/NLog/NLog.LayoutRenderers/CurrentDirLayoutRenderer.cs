using System.IO;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The current working directory of the application.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/CurrentDir-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/CurrentDir-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("currentdir")]
[ThreadAgnostic]
public class CurrentDirLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
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
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(GetStringValue());
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		return GetStringValue();
	}

	private string GetStringValue()
	{
		return PathHelpers.CombinePaths(Directory.GetCurrentDirectory(), Dir, File);
	}
}

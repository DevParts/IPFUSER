using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Filters characters not allowed in the file names by replacing them with safe character.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Filesystem-Normalize-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Filesystem-Normalize-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("filesystem-normalize")]
[AmbientProperty("FSNormalize")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class FileSystemNormalizeLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating whether to modify the output of this renderer so it can be used as a part of file path
	/// (illegal characters are replaced with '_').
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Advanced Options" order="50" />
	public bool FSNormalize { get; set; } = true;

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner.Render(logEvent, builder);
		if (FSNormalize && builder.Length > orgLength)
		{
			TransformFileSystemNormalize(builder, orgLength);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	private static void TransformFileSystemNormalize(StringBuilder builder, int startPos)
	{
		for (int i = startPos; i < builder.Length; i++)
		{
			if (!IsSafeCharacter(builder[i]))
			{
				builder[i] = '_';
			}
		}
	}

	private static bool IsSafeCharacter(char c)
	{
		if (!char.IsLetterOrDigit(c) && c != '_' && c != '-' && c != '.')
		{
			return c == ' ';
		}
		return true;
	}
}

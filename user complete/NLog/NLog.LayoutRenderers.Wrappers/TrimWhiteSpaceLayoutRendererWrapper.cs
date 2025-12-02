using System;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Trims the whitespace from the result of another layout renderer.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Trim-Whitespace-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Trim-Whitespace-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("trim-whitespace")]
[AmbientProperty("TrimWhiteSpace")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class TrimWhiteSpaceLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating whether whitespace should be trimmed.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool TrimWhiteSpace { get; set; } = true;

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner.Render(logEvent, builder);
		if (TrimWhiteSpace && builder.Length > orgLength)
		{
			TransformTrimWhiteSpaces(builder, orgLength);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	private static void TransformTrimWhiteSpaces(StringBuilder builder, int startPos)
	{
		builder.TrimRight(startPos);
		if (builder.Length > startPos && char.IsWhiteSpace(builder[startPos]))
		{
			string text = builder.ToString(startPos, builder.Length - startPos);
			builder.Length = startPos;
			builder.Append(text.Trim());
		}
	}
}

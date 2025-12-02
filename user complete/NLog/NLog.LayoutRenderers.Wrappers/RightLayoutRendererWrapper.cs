using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Right part of a text
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Right-layout-renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Right-layout-renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("right")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class RightLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets the length in characters. Zero or negative means disabled.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int Length { get; set; }

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (Length > 0)
		{
			base.Inner.Render(logEvent, builder);
			if (builder.Length - orgLength > Length)
			{
				string value = builder.ToString(builder.Length - Length, Length);
				builder.Length = orgLength;
				builder.Append(value);
			}
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}
}

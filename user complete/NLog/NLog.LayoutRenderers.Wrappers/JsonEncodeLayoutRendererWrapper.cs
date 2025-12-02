using System;
using System.ComponentModel;
using System.Text;
using NLog.Config;
using NLog.Targets;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Escapes output of another layout using JSON rules.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Json-Encode-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Json-Encode-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("json-encode")]
[AmbientProperty("JsonEncode")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class JsonEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets whether output should be encoded with Json-string escaping.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool JsonEncode { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to escape non-ascii characters
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool EscapeUnicode { get; set; } = true;

	/// <summary>
	/// Should forward slashes be escaped? If <see langword="true" />, / will be converted to \/
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Marked obsolete with NLog 5.5. Should never escape forward slash")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool EscapeForwardSlash { get; set; }

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner.Render(logEvent, builder);
		if (JsonEncode && builder.Length > orgLength)
		{
			DefaultJsonSerializer.PerformJsonEscapeWhenNeeded(builder, orgLength, EscapeUnicode);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}
}

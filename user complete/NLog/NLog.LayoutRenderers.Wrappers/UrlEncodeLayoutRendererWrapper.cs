using System;
using System.ComponentModel;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Encodes the result of another layout output for use with URLs.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Url-Encode-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Url-Encode-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("url-encode")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class UrlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating whether spaces should be translated to '+' or '%20'.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool SpaceAsPlus { get; set; } = true;

	/// <summary>
	/// Gets or sets a value whether escaping be done according to Rfc3986 (Supports Internationalized Resource Identifiers - IRIs)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool EscapeDataRfc3986 { get; set; }

	/// <summary>
	/// Gets or sets a value whether escaping be done according to the old NLog style (Very non-standard)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	[Obsolete("Instead use default Rfc2396 or EscapeDataRfc3986. Marked obsolete with NLog v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool EscapeDataNLogLegacy { get; set; }

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			UrlHelper.EscapeEncodingOptions uriStringEncodingFlags = UrlHelper.GetUriStringEncodingFlags(EscapeDataNLogLegacy, SpaceAsPlus, EscapeDataRfc3986);
			StringBuilder stringBuilder = new StringBuilder(text.Length + 20);
			UrlHelper.EscapeDataEncode(text, stringBuilder, uriStringEncodingFlags);
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner.Render(logEvent, builder);
		if (builder.Length > orgLength)
		{
			string source = builder.ToString(orgLength, builder.Length - orgLength);
			builder.Length = orgLength;
			UrlHelper.EscapeEncodingOptions uriStringEncodingFlags = UrlHelper.GetUriStringEncodingFlags(EscapeDataNLogLegacy, SpaceAsPlus, EscapeDataRfc3986);
			UrlHelper.EscapeDataEncode(source, builder, uriStringEncodingFlags);
		}
	}
}

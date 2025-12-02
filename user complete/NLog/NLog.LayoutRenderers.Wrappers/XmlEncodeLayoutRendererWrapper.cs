using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Converts the result of another layout output to be XML-compliant.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Xml-Encode-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Xml-Encode-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("xml-encode")]
[AmbientProperty("XmlEncode")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class XmlEncodeLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets whether output should be encoded with XML-string escaping.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool XmlEncode { get; set; } = true;

	/// <summary>
	/// Gets or sets whether output should be wrapped using CDATA section instead of XML-string escaping
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	public bool CDataEncode { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to transform newlines (\r\n) into (
	/// )
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool XmlEncodeNewlines { get; set; }

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (CDataEncode)
		{
			builder.Append("<![CDATA[");
			orgLength = builder.Length;
		}
		base.Inner?.Render(logEvent, builder);
		XmlHelper.RemoveInvalidXmlIfNeeded(builder, orgLength);
		if (CDataEncode)
		{
			XmlHelper.EscapeCDataIfNeeded(builder, orgLength);
			builder.Append("]]>");
		}
		else if (XmlEncode)
		{
			XmlHelper.PerformXmlEscapeWhenNeeded(builder, orgLength, XmlEncodeNewlines);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		if (CDataEncode)
		{
			return XmlHelper.EscapeCData(text);
		}
		if (XmlEncode)
		{
			return XmlHelper.EscapeXmlString(text, XmlEncodeNewlines);
		}
		return text;
	}
}

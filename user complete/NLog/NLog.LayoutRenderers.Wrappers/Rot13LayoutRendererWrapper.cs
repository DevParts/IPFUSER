using System;
using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Decodes text "encrypted" with ROT-13.
/// </summary>
/// <remarks>
/// See <a href="https://en.wikipedia.org/wiki/ROT13">https://en.wikipedia.org/wiki/ROT13</a>.
///
/// <a href="https://github.com/NLog/NLog/wiki/Rot13-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Rot13-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("rot13")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class Rot13LayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets the layout to be wrapped.
	/// </summary>
	/// <value>The layout to be wrapped.</value>
	/// <remarks>This variable is for backwards compatibility</remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by Inner. Marked obsolete with NLog 2.0")]
	public Layout Text
	{
		get
		{
			return base.Inner;
		}
		set
		{
			base.Inner = value;
		}
	}

	/// <summary>
	/// Encodes/Decodes ROT-13-encoded string.
	/// </summary>
	/// <param name="encodedValue">The string to be encoded/decoded.</param>
	/// <returns>Encoded/Decoded text.</returns>
	public static string DecodeRot13(string encodedValue)
	{
		StringBuilder stringBuilder = new StringBuilder(encodedValue.Length);
		stringBuilder.Append(encodedValue);
		DecodeRot13(stringBuilder, 0);
		return stringBuilder.ToString();
	}

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner.Render(logEvent, builder);
		if (builder.Length > orgLength)
		{
			DecodeRot13(builder, orgLength);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	/// <summary>
	/// Encodes/Decodes ROT-13-encoded string.
	/// </summary>
	internal static void DecodeRot13(StringBuilder encodedValue, int startPos)
	{
		if (encodedValue != null)
		{
			for (int i = startPos; i < encodedValue.Length; i++)
			{
				encodedValue[i] = DecodeRot13Char(encodedValue[i]);
			}
		}
	}

	private static char DecodeRot13Char(char c)
	{
		if (c >= 'A' && c <= 'M')
		{
			return (char)(78 + (c - 65));
		}
		if (c >= 'a' && c <= 'm')
		{
			return (char)(110 + (c - 97));
		}
		if (c >= 'N' && c <= 'Z')
		{
			return (char)(65 + (c - 78));
		}
		if (c >= 'n' && c <= 'z')
		{
			return (char)(97 + (c - 110));
		}
		return c;
	}
}

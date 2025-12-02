using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Replaces newline characters from the result of another layout renderer with spaces.
/// </summary>
[LayoutRenderer("wrapline")]
[AmbientProperty("WrapLine")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class WrapLineLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets the line length for wrapping. Only positive values are allowed.
	/// </summary>
	/// <remarks>Default: <see langword="80" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int WrapLine { get; set; } = 80;

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		if (WrapLine <= 0)
		{
			return text;
		}
		int num = WrapLine;
		if (text.Length <= num)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length + text.Length / num * Environment.NewLine.Length);
		for (int i = 0; i < text.Length; i += num)
		{
			if (num + i > text.Length)
			{
				num = text.Length - i;
			}
			stringBuilder.Append(text.Substring(i, num));
			if (num + i < text.Length)
			{
				stringBuilder.AppendLine();
			}
		}
		return stringBuilder.ToString();
	}
}

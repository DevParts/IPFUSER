using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Replaces newline characters from the result of another layout renderer with spaces.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Replace-NewLines-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Replace-NewLines-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("replace-newlines")]
[AmbientProperty("ReplaceNewLines")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class ReplaceNewLinesLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating the string that should be used to replace newlines.
	/// </summary>
	/// <remarks>Default: <c> </c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string Replacement { get; set; } = " ";

	/// <summary>
	/// Gets or sets a value indicating the string that should be used to replace newlines (alias for <see cref="P:NLog.LayoutRenderers.Wrappers.ReplaceNewLinesLayoutRendererWrapper.Replacement" />)
	/// </summary>
	/// <remarks>Default: <c> </c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string ReplaceNewLines
	{
		get
		{
			return Replacement;
		}
		set
		{
			Replacement = value;
		}
	}

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner?.Render(logEvent, builder);
		if (builder.Length <= orgLength)
		{
			return;
		}
		int num = IndexOfLineEndCharacters(builder, orgLength);
		if (num <= -1)
		{
			return;
		}
		orgLength = ((num > orgLength) ? (num - 1) : orgLength);
		string text = builder.ToString(orgLength, builder.Length - orgLength);
		builder.Length = orgLength;
		bool flag = false;
		string text2 = text;
		foreach (char c in text2)
		{
			if (IsLineEndCharacter(c))
			{
				if (!flag || c != '\n')
				{
					builder.Append(Replacement);
				}
				flag = c == '\r';
			}
			else
			{
				builder.Append(c);
				flag = false;
			}
		}
	}

	private static bool IsLineEndCharacter(char ch)
	{
		switch (ch)
		{
		case '\n':
		case '\f':
		case '\r':
		case '\u0085':
		case '\u2028':
		case '\u2029':
			return true;
		default:
			return false;
		}
	}

	private static int IndexOfLineEndCharacters(StringBuilder builder, int startPosition)
	{
		for (int i = startPosition; i < builder.Length; i++)
		{
			if (IsLineEndCharacter(builder[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}
}

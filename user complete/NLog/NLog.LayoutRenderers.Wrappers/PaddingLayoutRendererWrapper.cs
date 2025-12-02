using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Applies padding to another layout output.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Pad-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Pad-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("pad")]
[AmbientProperty("Padding")]
[AmbientProperty("PadCharacter")]
[AmbientProperty("FixedLength")]
[AmbientProperty("AlignmentOnTruncation")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class PaddingLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets the number of characters to pad the output to.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="0" /> .
	/// Positive padding values cause left padding, negative values
	/// cause right padding to the desired width.
	/// </remarks>
	/// <docgen category="Layout Options" order="10" />
	public int Padding { get; set; }

	/// <summary>
	/// Gets or sets the padding character.
	/// </summary>
	/// <remarks>Default: <c> </c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public char PadCharacter { get; set; } = ' ';

	/// <summary>
	/// Gets or sets a value indicating whether to trim the
	/// rendered text to the absolute value of the padding length.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool FixedLength { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether a value that has
	/// been truncated (when <see cref="P:NLog.LayoutRenderers.Wrappers.PaddingLayoutRendererWrapper.FixedLength" /> is <see langword="true" />)
	/// will be left-aligned (characters removed from the right)
	/// or right-aligned (characters removed from the left). The
	/// default is left alignment.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.LayoutRenderers.Wrappers.PaddingHorizontalAlignment.Left" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public PaddingHorizontalAlignment AlignmentOnTruncation { get; set; }

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner?.Render(logEvent, builder);
		if (Padding == 0)
		{
			return;
		}
		int num = Padding;
		if (num < 0)
		{
			num = -num;
		}
		int num2 = AppendPadding(builder, orgLength, num);
		if (FixedLength && num2 > num)
		{
			if (AlignmentOnTruncation == PaddingHorizontalAlignment.Left)
			{
				builder.Length = orgLength + num;
			}
			else
			{
				builder.Remove(orgLength, num2 - num);
			}
		}
	}

	private int AppendPadding(StringBuilder builder, int orgLength, int absolutePadding)
	{
		int num = builder.Length - orgLength;
		if (Padding > 0)
		{
			if (num < 10 || num >= absolutePadding)
			{
				for (int i = num; i < absolutePadding; i++)
				{
					builder.Append(PadCharacter);
					for (int num2 = builder.Length - 1; num2 > orgLength; num2--)
					{
						builder[num2] = builder[num2 - 1];
					}
					builder[orgLength] = PadCharacter;
					num++;
				}
			}
			else
			{
				string value = builder.ToString(orgLength, num);
				builder.Length = orgLength;
				for (int j = num; j < absolutePadding; j++)
				{
					builder.Append(PadCharacter);
					num++;
				}
				builder.Append(value);
			}
		}
		else
		{
			for (int k = num; k < absolutePadding; k++)
			{
				builder.Append(PadCharacter);
				num++;
			}
		}
		return num;
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}
}

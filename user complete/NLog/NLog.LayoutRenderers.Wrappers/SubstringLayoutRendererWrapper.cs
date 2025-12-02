using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Substring the result
/// </summary>
/// <example>
/// ${substring:${level}:start=2:length=2}
/// ${substring:${level}:start=-2:length=2}
/// ${substring:Inner=${level}:start=2:length=2}
/// </example>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Substring-layout-renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Substring-layout-renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("substring")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class SubstringLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets the start index.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int Start { get; set; }

	/// <summary>
	/// Gets or sets the length in characters. If <c>null</c>, then the whole string
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int? Length { get; set; }

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (Length != 0)
		{
			base.Inner?.Render(logEvent, builder);
			int num = builder.Length - orgLength;
			if (num > 0)
			{
				int num2 = CalcStart(num);
				int num3 = CalcLength(num, num2);
				string value = builder.ToString(orgLength + num2, num3);
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

	/// <summary>
	/// Calculate start position
	/// </summary>
	/// <returns>0 or positive number</returns>
	private int CalcStart(int textLength)
	{
		int num = Start;
		if (num > textLength)
		{
			num = textLength;
		}
		if (num < 0)
		{
			num = textLength + num;
			if (num < 0)
			{
				num = 0;
			}
		}
		return num;
	}

	/// <summary>
	/// Calculate needed length
	/// </summary>
	/// <returns>0 or positive number</returns>
	private int CalcLength(int textLength, int start)
	{
		int num = textLength - start;
		if (Length.HasValue && textLength > Length.Value + start)
		{
			num = Length.Value;
		}
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}
}

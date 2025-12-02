using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The process time in format HH:mm:ss.mmm.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ProcessTime-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ProcessTime-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("processtime")]
[ThreadAgnostic]
public class ProcessTimeLayoutRenderer : LayoutRenderer, IRawValue
{
	/// <summary>
	/// Gets or sets a value indicating whether to output in culture invariant format
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool Invariant
	{
		get
		{
			return Culture == CultureInfo.InvariantCulture;
		}
		set
		{
			Culture = (value ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture);
		}
	}

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		TimeSpan value = GetValue(logEvent);
		CultureInfo culture = GetCulture(logEvent, Culture);
		WriteTimestamp(builder, value, culture);
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent);
		return true;
	}

	/// <summary>
	/// Write timestamp to builder with format hh:mm:ss:fff
	/// </summary>
	internal static void WriteTimestamp(StringBuilder builder, TimeSpan ts, CultureInfo culture)
	{
		string value = ":";
		string value2 = ".";
		if (culture != CultureInfo.InvariantCulture)
		{
			value = culture.DateTimeFormat.TimeSeparator;
			value2 = culture.NumberFormat.NumberDecimalSeparator;
		}
		builder.Append2DigitsZeroPadded(ts.Hours);
		builder.Append(value);
		builder.Append2DigitsZeroPadded(ts.Minutes);
		builder.Append(value);
		builder.Append2DigitsZeroPadded(ts.Seconds);
		builder.Append(value2);
		int milliseconds = ts.Milliseconds;
		if (milliseconds < 100)
		{
			builder.Append('0');
			if (milliseconds < 10)
			{
				builder.Append('0');
				if (milliseconds < 0)
				{
					builder.Append('0');
					return;
				}
			}
		}
		builder.AppendInvariant(milliseconds);
	}

	private static TimeSpan GetValue(LogEventInfo logEvent)
	{
		return logEvent.TimeStamp.ToUniversalTime() - LogEventInfo.ZeroDate;
	}
}

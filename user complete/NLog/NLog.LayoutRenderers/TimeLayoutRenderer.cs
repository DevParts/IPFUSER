using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The time in a 24-hour, sortable format HH:mm:ss.mmmm.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("time")]
[ThreadAgnostic]
public class TimeLayoutRenderer : LayoutRenderer, IRawValue
{
	private bool? _universalTime;

	/// <summary>
	/// Gets or sets a value indicating whether to output UTC time instead of local time.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool UniversalTime
	{
		get
		{
			return _universalTime == true;
		}
		set
		{
			_universalTime = value;
		}
	}

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
		DateTime value = GetValue(logEvent);
		CultureInfo culture = GetCulture(logEvent, Culture);
		string value2 = ":";
		string value3 = ".";
		if (culture != CultureInfo.InvariantCulture)
		{
			value2 = culture.DateTimeFormat.TimeSeparator;
			value3 = culture.NumberFormat.NumberDecimalSeparator;
		}
		builder.Append2DigitsZeroPadded(value.Hour);
		builder.Append(value2);
		builder.Append2DigitsZeroPadded(value.Minute);
		builder.Append(value2);
		builder.Append2DigitsZeroPadded(value.Second);
		builder.Append(value3);
		builder.Append4DigitsZeroPadded((int)(value.Ticks % 10000000) / 1000);
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent);
		return true;
	}

	private DateTime GetValue(LogEventInfo logEvent)
	{
		DateTime result = logEvent.TimeStamp;
		if (_universalTime.HasValue)
		{
			result = ((!_universalTime.Value) ? result.ToLocalTime() : result.ToUniversalTime());
		}
		return result;
	}
}

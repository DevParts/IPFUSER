using System;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The date and time in a long, sortable format yyyy-MM-dd HH:mm:ss.ffff.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/LongDate-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/LongDate-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("longdate")]
[ThreadAgnostic]
public class LongDateLayoutRenderer : LayoutRenderer, IRawValue
{
	private bool? _universalTime;

	/// <summary>
	/// Gets or sets a value indicating whether to output UTC time instead of local time.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
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

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		DateTime value = GetValue(logEvent);
		builder.Append4DigitsZeroPadded(value.Year);
		builder.Append('-');
		builder.Append2DigitsZeroPadded(value.Month);
		builder.Append('-');
		builder.Append2DigitsZeroPadded(value.Day);
		builder.Append(' ');
		builder.Append2DigitsZeroPadded(value.Hour);
		builder.Append(':');
		builder.Append2DigitsZeroPadded(value.Minute);
		builder.Append(':');
		builder.Append2DigitsZeroPadded(value.Second);
		builder.Append('.');
		builder.Append4DigitsZeroPadded((int)(value.Ticks % 10000000) / 1000);
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent);
		return true;
	}

	private DateTime GetValue(LogEventInfo logEvent)
	{
		DateTime timeStamp = logEvent.TimeStamp;
		if (_universalTime.HasValue)
		{
			if (!_universalTime.Value)
			{
				return timeStamp.ToLocalTime();
			}
			return timeStamp.ToUniversalTime();
		}
		return timeStamp;
	}
}

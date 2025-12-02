using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The short date in a sortable format yyyy-MM-dd.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ShortDate-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ShortDate-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("shortdate")]
[ThreadAgnostic]
public class ShortDateLayoutRenderer : LayoutRenderer, IRawValue, IStringValueRenderer
{
	private sealed class CachedDateFormatted
	{
		public readonly DateTime Date;

		public readonly string FormattedDate;

		public CachedDateFormatted(DateTime date, string formattedDate)
		{
			Date = date;
			FormattedDate = formattedDate;
		}
	}

	private bool? _universalTime;

	private CachedDateFormatted _cachedDateFormatted = new CachedDateFormatted(DateTime.MaxValue, string.Empty);

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
		string stringValue = GetStringValue(logEvent);
		builder.Append(stringValue);
	}

	private string GetStringValue(LogEventInfo logEvent)
	{
		DateTime value = GetValue(logEvent);
		CachedDateFormatted cachedDateFormatted = _cachedDateFormatted;
		if (cachedDateFormatted.Date != value.Date)
		{
			string formattedDate = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			cachedDateFormatted = (_cachedDateFormatted = new CachedDateFormatted(value.Date, formattedDate));
		}
		return cachedDateFormatted.FormattedDate;
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

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent).Date;
		return true;
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		return GetStringValue(logEvent);
	}
}

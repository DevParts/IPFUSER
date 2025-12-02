using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The Ticks value of current date and time.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Ticks-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Ticks-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("ticks")]
[ThreadAgnostic]
public class TicksLayoutRenderer : LayoutRenderer, IRawValue
{
	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(GetValue(logEvent).ToString(CultureInfo.InvariantCulture));
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent);
		return true;
	}

	private static long GetValue(LogEventInfo logEvent)
	{
		return logEvent.TimeStamp.Ticks;
	}
}

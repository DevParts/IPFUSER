using System.Globalization;
using System.Text;

namespace NLog.Internal;

internal sealed class LogMessageStringFormatter : ILogMessageFormatter
{
	public static readonly LogMessageStringFormatter Default = new LogMessageStringFormatter();

	/// <summary>
	/// The MessageFormatter delegate
	/// </summary>
	public LogMessageFormatter MessageFormatter { get; }

	public bool? EnableMessageTemplateParser => false;

	private LogMessageStringFormatter()
	{
		MessageFormatter = FormatMessage;
	}

	public void AppendFormattedMessage(LogEventInfo logEvent, StringBuilder builder)
	{
		builder.Append(logEvent.FormattedMessage);
	}

	public string FormatMessage(LogEventInfo logEvent)
	{
		if (HasParameters(logEvent))
		{
			return string.Format(logEvent.FormatProvider ?? CultureInfo.CurrentCulture, logEvent.Message, logEvent.Parameters);
		}
		return logEvent.Message;
	}

	internal static bool HasParameters(LogEventInfo logEvent)
	{
		object?[]? parameters = logEvent.Parameters;
		if (parameters != null && parameters.Length != 0)
		{
			return !string.IsNullOrEmpty(logEvent.Message);
		}
		return false;
	}

	public bool HasProperties(LogEventInfo logEvent)
	{
		return false;
	}
}

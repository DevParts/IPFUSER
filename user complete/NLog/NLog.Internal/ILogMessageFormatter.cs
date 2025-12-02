using System.Text;

namespace NLog.Internal;

/// <summary>
/// Format a log message
/// </summary>
internal interface ILogMessageFormatter
{
	/// <summary>
	/// Perform message template parsing and formatting of LogEvent messages:
	/// - <see langword="true" /> = Always
	/// - <see langword="false" /> = Never
	/// - <see langword="null" /> = Auto Detect
	/// </summary>
	bool? EnableMessageTemplateParser { get; }

	/// <summary>
	/// Format the message and return
	/// </summary>
	/// <param name="logEvent">LogEvent with message to be formatted</param>
	/// <returns>formatted message</returns>
	string FormatMessage(LogEventInfo logEvent);

	/// <summary>
	/// Has the logevent properties?
	/// </summary>
	/// <param name="logEvent">LogEvent with message to be formatted</param>
	/// <returns><see langword="false" /> when logevent has no properties to be extracted</returns>
	bool HasProperties(LogEventInfo logEvent);

	/// <summary>
	/// Appends the logevent message to the provided StringBuilder
	/// </summary>
	/// <param name="logEvent">LogEvent with message to be formatted</param>
	/// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to append the formatted message.</param>
	void AppendFormattedMessage(LogEventInfo logEvent, StringBuilder builder);
}

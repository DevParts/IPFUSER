namespace NLog;

/// <summary>
/// Generates a formatted message from the log event
/// </summary>
/// <param name="logEvent">Log event.</param>
/// <returns>Formatted message</returns>
public delegate string? LogMessageFormatter(LogEventInfo logEvent);

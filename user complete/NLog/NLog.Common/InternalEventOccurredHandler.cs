namespace NLog.Common;

/// <summary>
/// Handle Internal LogEvent written to the InternalLogger
/// </summary>
/// <remarks>
/// Never use/call NLog Logger-objects when handling these internal events, as it will lead to deadlock / stackoverflow.
/// </remarks>
public delegate void InternalEventOccurredHandler(object? sender, InternalLogEventArgs e);

namespace NLog.Targets;

/// <summary>
/// Action that should be taken if the message is greater than
/// the max message size allowed by the Event Log.
/// </summary>
public enum EventLogTargetOverflowAction
{
	/// <summary>
	/// Truncate the message before writing to the Event Log.
	/// </summary>
	Truncate,
	/// <summary>
	/// Split the message and write multiple entries to the Event Log.
	/// </summary>
	Split,
	/// <summary>
	/// Discard of the message. It will not be written to the Event Log.
	/// </summary>
	Discard
}

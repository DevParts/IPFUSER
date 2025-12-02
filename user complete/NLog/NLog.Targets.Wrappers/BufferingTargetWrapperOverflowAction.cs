namespace NLog.Targets.Wrappers;

/// <summary>
/// The action to be taken when the buffer overflows.
/// </summary>
public enum BufferingTargetWrapperOverflowAction
{
	/// <summary>
	/// Flush the content of the buffer.
	/// </summary>
	Flush,
	/// <summary>
	/// Discard the oldest item.
	/// </summary>
	Discard
}

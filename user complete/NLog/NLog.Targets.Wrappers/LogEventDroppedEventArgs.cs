using System;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Arguments for <see cref="E:NLog.Targets.Wrappers.AsyncTargetWrapper.LogEventDropped" /> events.
/// </summary>
public class LogEventDroppedEventArgs : EventArgs
{
	/// <summary>
	/// Instance of <see cref="T:NLog.LogEventInfo" /> that was dropped by <see cref="T:NLog.Targets.Target" />
	/// </summary>
	public LogEventInfo DroppedLogEventInfo { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.LogEventDroppedEventArgs" /> class.
	/// </summary>
	/// <param name="logEventInfo">LogEvent that have been dropped</param>
	public LogEventDroppedEventArgs(LogEventInfo logEventInfo)
	{
		DroppedLogEventInfo = logEventInfo;
	}
}

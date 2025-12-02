using System;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Raises by  <see cref="T:NLog.Targets.Wrappers.AsyncRequestQueue" /> when
/// queue is full
/// and <see cref="P:NLog.Targets.Wrappers.AsyncRequestQueueBase.OnOverflow" /> set to <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Grow" />
/// By default queue doubles it size.
/// </summary>
public class LogEventQueueGrowEventArgs : EventArgs
{
	/// <summary>
	/// New queue size
	/// </summary>
	public long NewQueueSize { get; }

	/// <summary>
	/// Current requests count
	/// </summary>
	public long RequestsCount { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.LogEventQueueGrowEventArgs" /> class.
	/// </summary>
	/// <param name="newQueueSize">Required queue size</param>
	/// <param name="requestsCount">Current queue size</param>
	public LogEventQueueGrowEventArgs(long newQueueSize, long requestsCount)
	{
		NewQueueSize = newQueueSize;
		RequestsCount = requestsCount;
	}
}

using System;
using System.Collections.Generic;
using NLog.Common;

namespace NLog.Targets.Wrappers;

internal abstract class AsyncRequestQueueBase
{
	public abstract bool IsEmpty { get; }

	/// <summary>
	/// Gets or sets the queue max-size
	/// </summary>
	public int QueueLimit { get; set; }

	/// <summary>
	/// Gets or sets the action to be taken when there's no more room in
	/// the queue and another request is enqueued.
	/// </summary>
	public AsyncTargetWrapperOverflowAction OnOverflow { get; set; }

	/// <summary>
	/// Occurs when LogEvent has been dropped, because internal queue is full and <see cref="P:NLog.Targets.Wrappers.AsyncRequestQueueBase.OnOverflow" /> set to <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Discard" />
	/// </summary>
	public event EventHandler<LogEventDroppedEventArgs>? LogEventDropped;

	/// <summary>
	/// Occurs when internal queue size is growing, because internal queue is full and <see cref="P:NLog.Targets.Wrappers.AsyncRequestQueueBase.OnOverflow" /> set to <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Grow" />
	/// </summary>
	public event EventHandler<LogEventQueueGrowEventArgs>? LogEventQueueGrow;

	public abstract bool Enqueue(AsyncLogEventInfo logEventInfo);

	public abstract AsyncLogEventInfo[] DequeueBatch(int count);

	public abstract void DequeueBatch(int count, IList<AsyncLogEventInfo> result);

	public abstract void Clear();

	/// <summary>
	/// Raise event when queued element was dropped because of queue overflow
	/// </summary>
	/// <param name="logEventInfo">Dropped queue item</param>
	protected void OnLogEventDropped(LogEventInfo logEventInfo)
	{
		this.LogEventDropped?.Invoke(this, new LogEventDroppedEventArgs(logEventInfo));
	}

	/// <summary>
	/// Raise event when RequestCount overflow <see cref="P:NLog.Targets.Wrappers.AsyncRequestQueueBase.QueueLimit" />
	/// </summary>
	/// <param name="requestsCount"> current requests count</param>
	protected void OnLogEventQueueGrows(long requestsCount)
	{
		this.LogEventQueueGrow?.Invoke(this, new LogEventQueueGrowEventArgs(QueueLimit, requestsCount));
	}
}

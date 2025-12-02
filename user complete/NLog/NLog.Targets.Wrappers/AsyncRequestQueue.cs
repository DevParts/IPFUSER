using System.Collections.Generic;
using System.Threading;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Asynchronous request queue.
/// </summary>
internal sealed class AsyncRequestQueue : AsyncRequestQueueBase
{
	private readonly Queue<AsyncLogEventInfo> _logEventInfoQueue = new Queue<AsyncLogEventInfo>(1000);

	/// <summary>
	/// Gets the number of requests currently in the queue.
	/// </summary>
	public int RequestCount
	{
		get
		{
			lock (_logEventInfoQueue)
			{
				return _logEventInfoQueue.Count;
			}
		}
	}

	public override bool IsEmpty => RequestCount == 0;

	/// <summary>
	/// Initializes a new instance of the AsyncRequestQueue class.
	/// </summary>
	/// <param name="queueLimit">Queue max size.</param>
	/// <param name="overflowAction">The overflow action.</param>
	public AsyncRequestQueue(int queueLimit, AsyncTargetWrapperOverflowAction overflowAction)
	{
		base.QueueLimit = queueLimit;
		base.OnOverflow = overflowAction;
	}

	/// <summary>
	/// Enqueues another item. If the queue is overflown the appropriate
	/// action is taken as specified by <see cref="P:NLog.Targets.Wrappers.AsyncRequestQueueBase.OnOverflow" />.
	/// </summary>
	/// <param name="logEventInfo">The log event info.</param>
	/// <returns>Queue was empty before enqueue</returns>
	public override bool Enqueue(AsyncLogEventInfo logEventInfo)
	{
		lock (_logEventInfoQueue)
		{
			int count = _logEventInfoQueue.Count;
			if (count >= base.QueueLimit)
			{
				switch (base.OnOverflow)
				{
				case AsyncTargetWrapperOverflowAction.Discard:
					InternalLogger.Debug("AsyncQueue - Discarding single item, because queue is full");
					OnLogEventDropped(_logEventInfoQueue.Dequeue().LogEvent);
					break;
				case AsyncTargetWrapperOverflowAction.Grow:
					InternalLogger.Debug("AsyncQueue - Growing the size of queue, because queue is full");
					base.QueueLimit *= 2;
					OnLogEventQueueGrows(count + 1);
					break;
				case AsyncTargetWrapperOverflowAction.Block:
					while (count >= base.QueueLimit)
					{
						InternalLogger.Debug("AsyncQueue - Blocking until ready, because queue is full");
						Monitor.Wait(_logEventInfoQueue);
						InternalLogger.Trace("AsyncQueue - Entered critical section.");
						count = _logEventInfoQueue.Count;
					}
					InternalLogger.Trace("AsyncQueue - Limit ok.");
					break;
				}
			}
			_logEventInfoQueue.Enqueue(logEventInfo);
			return count == 0;
		}
	}

	/// <summary>
	/// Dequeues a maximum of <c>count</c> items from the queue
	/// and adds returns the list containing them.
	/// </summary>
	/// <param name="count">Maximum number of items to be dequeued</param>
	/// <returns>The array of log events.</returns>
	public override AsyncLogEventInfo[] DequeueBatch(int count)
	{
		AsyncLogEventInfo[] array;
		lock (_logEventInfoQueue)
		{
			if (_logEventInfoQueue.Count < count)
			{
				count = _logEventInfoQueue.Count;
			}
			if (count == 0)
			{
				return ArrayHelper.Empty<AsyncLogEventInfo>();
			}
			array = new AsyncLogEventInfo[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = _logEventInfoQueue.Dequeue();
			}
			if (base.OnOverflow == AsyncTargetWrapperOverflowAction.Block)
			{
				Monitor.PulseAll(_logEventInfoQueue);
			}
		}
		return array;
	}

	/// <summary>
	/// Dequeues into a preallocated array, instead of allocating a new one
	/// </summary>
	/// <param name="count">Maximum number of items to be dequeued</param>
	/// <param name="result">Preallocated list</param>
	public override void DequeueBatch(int count, IList<AsyncLogEventInfo> result)
	{
		lock (_logEventInfoQueue)
		{
			if (_logEventInfoQueue.Count < count)
			{
				count = _logEventInfoQueue.Count;
			}
			for (int i = 0; i < count; i++)
			{
				result.Add(_logEventInfoQueue.Dequeue());
			}
			if (base.OnOverflow == AsyncTargetWrapperOverflowAction.Block)
			{
				Monitor.PulseAll(_logEventInfoQueue);
			}
		}
	}

	/// <summary>
	/// Clears the queue.
	/// </summary>
	public override void Clear()
	{
		lock (_logEventInfoQueue)
		{
			_logEventInfoQueue.Clear();
			if (base.OnOverflow == AsyncTargetWrapperOverflowAction.Block)
			{
				Monitor.PulseAll(_logEventInfoQueue);
			}
		}
	}
}

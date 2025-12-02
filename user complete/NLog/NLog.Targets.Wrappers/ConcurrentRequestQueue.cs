using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Concurrent Asynchronous request queue based on <see cref="T:System.Collections.Concurrent.ConcurrentQueue`1" />
/// </summary>
internal sealed class ConcurrentRequestQueue : AsyncRequestQueueBase
{
	private readonly ConcurrentQueue<AsyncLogEventInfo> _logEventInfoQueue = new ConcurrentQueue<AsyncLogEventInfo>();

	private long _count;

	public override bool IsEmpty
	{
		get
		{
			if (_logEventInfoQueue.IsEmpty)
			{
				return Interlocked.Read(ref _count) == 0;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets the number of requests currently in the queue.
	/// </summary>
	/// <remarks>
	/// Only for debugging purposes
	/// </remarks>
	public int Count => (int)_count;

	/// <summary>
	/// Initializes a new instance of the AsyncRequestQueue class.
	/// </summary>
	/// <param name="queueLimit">Queue max size.</param>
	/// <param name="overflowAction">The overflow action.</param>
	public ConcurrentRequestQueue(int queueLimit, AsyncTargetWrapperOverflowAction overflowAction)
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
		long num = Interlocked.Increment(ref _count);
		bool result = num == 1;
		if (num > base.QueueLimit)
		{
			switch (base.OnOverflow)
			{
			case AsyncTargetWrapperOverflowAction.Discard:
				result = DequeueUntilBelowRequestLimit();
				break;
			case AsyncTargetWrapperOverflowAction.Block:
				result = WaitForBelowRequestLimit();
				break;
			case AsyncTargetWrapperOverflowAction.Grow:
			{
				InternalLogger.Debug("AsyncQueue - Growing the size of queue, because queue is full");
				int queueLimit = base.QueueLimit;
				if (num > queueLimit)
				{
					base.QueueLimit = queueLimit * 2;
				}
				OnLogEventQueueGrows(num);
				break;
			}
			}
		}
		_logEventInfoQueue.Enqueue(logEventInfo);
		return result;
	}

	private bool DequeueUntilBelowRequestLimit()
	{
		bool flag = false;
		long num;
		do
		{
			if (_logEventInfoQueue.TryDequeue(out var result))
			{
				InternalLogger.Debug("AsyncQueue - Discarding single item, because queue is full");
				flag = Interlocked.Decrement(ref _count) == 1 || flag;
				OnLogEventDropped(result.LogEvent);
				break;
			}
			num = Interlocked.Read(ref _count);
			flag = true;
		}
		while (num > base.QueueLimit);
		return flag;
	}

	private bool WaitForBelowRequestLimit()
	{
		long num = TrySpinWaitForLowerCount();
		if (num > base.QueueLimit)
		{
			InternalLogger.Debug("AsyncQueue - Blocking until ready, because queue is full");
			lock (_logEventInfoQueue)
			{
				InternalLogger.Trace("AsyncQueue - Entered critical section.");
				for (num = Interlocked.Read(ref _count); num > base.QueueLimit; num = Interlocked.Increment(ref _count))
				{
					Interlocked.Decrement(ref _count);
					Monitor.Wait(_logEventInfoQueue);
					InternalLogger.Trace("AsyncQueue - Entered critical section.");
				}
			}
		}
		InternalLogger.Trace("AsyncQueue - Limit ok.");
		return true;
	}

	private long TrySpinWaitForLowerCount()
	{
		long num = 0L;
		bool flag = true;
		SpinWait spinWait = default(SpinWait);
		for (int i = 0; i < 15; i++)
		{
			if (spinWait.NextSpinWillYield)
			{
				if (flag)
				{
					InternalLogger.Debug("AsyncQueue - Blocking with yield, because queue is full");
				}
				flag = false;
			}
			spinWait.SpinOnce();
			num = Interlocked.Read(ref _count);
			if (num <= base.QueueLimit)
			{
				break;
			}
		}
		return num;
	}

	/// <summary>
	/// Dequeues a maximum of <c>count</c> items from the queue
	/// and adds returns the list containing them.
	/// </summary>
	/// <param name="count">Maximum number of items to be dequeued</param>
	/// <returns>The array of log events.</returns>
	public override AsyncLogEventInfo[] DequeueBatch(int count)
	{
		if (_logEventInfoQueue.IsEmpty)
		{
			return ArrayHelper.Empty<AsyncLogEventInfo>();
		}
		if (_count < count)
		{
			count = Math.Min(count, Count);
		}
		List<AsyncLogEventInfo> list = new List<AsyncLogEventInfo>(count);
		DequeueBatch(count, list);
		if (list.Count == 0)
		{
			return ArrayHelper.Empty<AsyncLogEventInfo>();
		}
		return list.ToArray();
	}

	/// <summary>
	/// Dequeues into a preallocated array, instead of allocating a new one
	/// </summary>
	/// <param name="count">Maximum number of items to be dequeued</param>
	/// <param name="result">Preallocated list</param>
	public override void DequeueBatch(int count, IList<AsyncLogEventInfo> result)
	{
		bool flag = base.OnOverflow == AsyncTargetWrapperOverflowAction.Block;
		for (int i = 0; i < count; i++)
		{
			if (_logEventInfoQueue.TryDequeue(out var result2))
			{
				if (!flag)
				{
					Interlocked.Decrement(ref _count);
				}
				result.Add(result2);
				continue;
			}
			count = i;
			break;
		}
		if (flag)
		{
			lock (_logEventInfoQueue)
			{
				Interlocked.Add(ref _count, -count);
				Monitor.PulseAll(_logEventInfoQueue);
			}
		}
	}

	/// <summary>
	/// Clears the queue.
	/// </summary>
	public override void Clear()
	{
		while (!_logEventInfoQueue.IsEmpty)
		{
			_logEventInfoQueue.TryDequeue(out var _);
		}
		Interlocked.Exchange(ref _count, 0L);
		if (base.OnOverflow == AsyncTargetWrapperOverflowAction.Block)
		{
			lock (_logEventInfoQueue)
			{
				Monitor.PulseAll(_logEventInfoQueue);
			}
		}
	}
}

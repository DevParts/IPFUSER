using System;
using System.Collections.Generic;
using System.Threading;
using NLog.Common;

namespace NLog.Internal;

/// <summary>
/// Keeps track of pending operation count, and can notify when pending operation count reaches zero
/// </summary>
internal sealed class AsyncOperationCounter
{
	private int _pendingOperationCounter;

	private readonly LinkedList<AsyncContinuation?> _pendingCompletionList = new LinkedList<AsyncContinuation>();

	/// <summary>
	/// Mark operation has started
	/// </summary>
	public void BeginOperation()
	{
		Interlocked.Increment(ref _pendingOperationCounter);
	}

	/// <summary>
	/// Mark operation has completed
	/// </summary>
	/// <param name="exception">Exception coming from the completed operation [optional]</param>
	public void CompleteOperation(Exception? exception)
	{
		NotifyCompletion(exception);
	}

	private int NotifyCompletion(Exception? exception)
	{
		int result = Interlocked.Decrement(ref _pendingOperationCounter);
		if (_pendingCompletionList.Count > 0)
		{
			lock (_pendingCompletionList)
			{
				LinkedListNode<AsyncContinuation> linkedListNode = _pendingCompletionList.First;
				while (linkedListNode != null)
				{
					AsyncContinuation value = linkedListNode.Value;
					linkedListNode = linkedListNode.Next;
					value?.Invoke(exception);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Registers an AsyncContinuation to be called when all pending operations have completed
	/// </summary>
	/// <param name="asyncContinuation">Invoked on completion</param>
	/// <returns>AsyncContinuation operation</returns>
	public AsyncContinuation RegisterCompletionNotification(AsyncContinuation asyncContinuation)
	{
		int remainingCompletionCounter = Interlocked.Increment(ref _pendingOperationCounter);
		if (remainingCompletionCounter <= 1)
		{
			if (NotifyCompletion(null) < 0)
			{
				Interlocked.Exchange(ref _pendingOperationCounter, 0);
			}
			return asyncContinuation;
		}
		lock (_pendingCompletionList)
		{
			if (NotifyCompletion(null) <= 0)
			{
				return asyncContinuation;
			}
			LinkedListNode<AsyncContinuation?> pendingCompletion = new LinkedListNode<AsyncContinuation>(null);
			_pendingCompletionList.AddLast(pendingCompletion);
			remainingCompletionCounter = Interlocked.Increment(ref _pendingOperationCounter);
			if (remainingCompletionCounter <= 0)
			{
				remainingCompletionCounter = 1;
			}
			pendingCompletion.Value = delegate(Exception? ex)
			{
				if (Interlocked.Decrement(ref remainingCompletionCounter) == 0)
				{
					lock (_pendingCompletionList)
					{
						_pendingCompletionList.Remove(pendingCompletion);
						NotifyCompletion(ex);
					}
					asyncContinuation(ex);
				}
			};
			return pendingCompletion.Value;
		}
	}

	/// <summary>
	/// Clear o
	/// </summary>
	public void Clear()
	{
		_pendingCompletionList.Clear();
		Interlocked.Exchange(ref _pendingOperationCounter, 0);
	}
}

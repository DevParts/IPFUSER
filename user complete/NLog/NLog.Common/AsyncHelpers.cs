using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using NLog.Internal;

namespace NLog.Common;

/// <summary>
/// Helpers for asynchronous operations.
/// </summary>
public static class AsyncHelpers
{
	internal static int GetManagedThreadId()
	{
		return Thread.CurrentThread.ManagedThreadId;
	}

	internal static void StartAsyncTask(WaitCallback asyncDelegate, object? state)
	{
		ThreadPool.QueueUserWorkItem(asyncDelegate, state);
	}

	internal static void WaitForDelay(TimeSpan delay)
	{
		Thread.Sleep(delay);
	}

	/// <summary>
	/// Iterates over all items in the given collection and runs the specified action
	/// in sequence (each action executes only after the preceding one has completed without an error).
	/// </summary>
	/// <typeparam name="T">Type of each item.</typeparam>
	/// <param name="items">The items to iterate.</param>
	/// <param name="asyncContinuation">The asynchronous continuation to invoke once all items
	/// have been iterated.</param>
	/// <param name="action">The action to invoke for each item.</param>
	[Obsolete("Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ForEachItemSequentially<T>(IEnumerable<T> items, AsyncContinuation asyncContinuation, AsynchronousAction<T> action)
	{
		action = ExceptionGuard(action);
		IEnumerator<T> enumerator = items.GetEnumerator();
		InvokeNext(null);
		void InvokeNext(Exception? ex)
		{
			if (ex != null)
			{
				asyncContinuation(ex);
			}
			else if (!enumerator.MoveNext())
			{
				enumerator.Dispose();
				asyncContinuation(null);
			}
			else
			{
				action(enumerator.Current, PreventMultipleCalls(InvokeNext));
			}
		}
	}

	/// <summary>
	/// Repeats the specified asynchronous action multiple times and invokes asynchronous continuation at the end.
	/// </summary>
	/// <param name="repeatCount">The repeat count.</param>
	/// <param name="asyncContinuation">The asynchronous continuation to invoke at the end.</param>
	/// <param name="action">The action to invoke.</param>
	public static void Repeat(int repeatCount, AsyncContinuation asyncContinuation, AsynchronousAction action)
	{
		action = ExceptionGuard(action);
		int remaining = repeatCount;
		InvokeNext(null);
		void InvokeNext(Exception? ex)
		{
			if (ex != null)
			{
				asyncContinuation(ex);
			}
			else if (remaining-- <= 0)
			{
				asyncContinuation(null);
			}
			else
			{
				action(PreventMultipleCalls(InvokeNext));
			}
		}
	}

	/// <summary>
	/// Modifies the continuation by pre-pending given action to execute just before it.
	/// </summary>
	/// <param name="asyncContinuation">The async continuation.</param>
	/// <param name="action">The action to pre-pend.</param>
	/// <returns>Continuation which will execute the given action before forwarding to the actual continuation.</returns>
	[Obsolete("Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static AsyncContinuation PrecededBy(AsyncContinuation asyncContinuation, AsynchronousAction action)
	{
		action = ExceptionGuard(action);
		return delegate(Exception? ex)
		{
			if (ex != null)
			{
				asyncContinuation(ex);
			}
			else
			{
				action(PreventMultipleCalls(asyncContinuation));
			}
		};
	}

	/// <summary>
	/// Attaches a timeout to a continuation which will invoke the continuation when the specified
	/// timeout has elapsed.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>Wrapped continuation.</returns>
	[Obsolete("Marked obsolete on NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static AsyncContinuation WithTimeout(AsyncContinuation asyncContinuation, TimeSpan timeout)
	{
		return new TimeoutContinuation(asyncContinuation, timeout).Function;
	}

	/// <summary>
	/// Iterates over all items in the given collection and runs the specified action
	/// in parallel (each action executes on a thread from thread pool).
	/// </summary>
	/// <typeparam name="T">Type of each item.</typeparam>
	/// <param name="values">The items to iterate.</param>
	/// <param name="asyncContinuation">The asynchronous continuation to invoke once all items
	/// have been iterated.</param>
	/// <param name="action">The action to invoke for each item.</param>
	public static void ForEachItemInParallel<T>(IEnumerable<T> values, AsyncContinuation asyncContinuation, AsynchronousAction<T> action)
	{
		action = ExceptionGuard(action);
		List<T> list = new List<T>(values);
		int remaining = list.Count;
		List<Exception> exceptions = new List<Exception>();
		InternalLogger.Trace("ForEachItemInParallel() {0} items", list.Count);
		if (remaining == 0)
		{
			asyncContinuation(null);
			return;
		}
		AsyncContinuation continuation = delegate(Exception? ex)
		{
			InternalLogger.Trace("Continuation invoked: {0}", ex);
			if (ex != null)
			{
				lock (exceptions)
				{
					exceptions.Add(ex);
				}
			}
			int num = Interlocked.Decrement(ref remaining);
			InternalLogger.Trace("Parallel task completed. {0} items remaining", num);
			if (num == 0)
			{
				asyncContinuation(GetCombinedException(exceptions));
			}
		};
		foreach (T item in list)
		{
			T itemCopy = item;
			StartAsyncTask(delegate
			{
				AsyncContinuation asyncContinuation2 = PreventMultipleCalls(continuation);
				try
				{
					action(itemCopy, asyncContinuation2);
				}
				catch (Exception ex)
				{
					InternalLogger.Error(ex, "ForEachItemInParallel - Unhandled Exception");
					asyncContinuation2(ex);
				}
			}, null);
		}
	}

	/// <summary>
	/// Runs the specified asynchronous action synchronously (blocks until the continuation has
	/// been invoked).
	/// </summary>
	/// <param name="action">The action.</param>
	/// <remarks>
	/// Using this method is not recommended because it will block the calling thread.
	/// </remarks>
	[Obsolete("Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void RunSynchronously(AsynchronousAction action)
	{
		ManualResetEvent ev = new ManualResetEvent(initialState: false);
		Exception lastException = null;
		action(PreventMultipleCalls(delegate(Exception? ex)
		{
			lastException = ex;
			ev.Set();
		}));
		ev.WaitOne();
		if (lastException != null)
		{
			throw new NLogRuntimeException("Asynchronous exception has occurred.", lastException);
		}
	}

	/// <summary>
	/// Wraps the continuation with a guard which will only make sure that the continuation function
	/// is invoked only once.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	/// <returns>Wrapped asynchronous continuation.</returns>
	public static AsyncContinuation PreventMultipleCalls(AsyncContinuation asyncContinuation)
	{
		if (asyncContinuation.Target is SingleCallContinuation)
		{
			return asyncContinuation;
		}
		return new SingleCallContinuation(asyncContinuation).Function;
	}

	/// <summary>
	/// Gets the combined exception from all exceptions in the list.
	/// </summary>
	/// <param name="exceptions">The exceptions.</param>
	/// <returns>Combined exception or null if no exception was thrown.</returns>
	public static Exception? GetCombinedException(IList<Exception> exceptions)
	{
		if (exceptions.Count == 0)
		{
			return null;
		}
		if (exceptions.Count == 1)
		{
			return exceptions[0];
		}
		StringBuilder stringBuilder = new StringBuilder();
		string value = string.Empty;
		string newLine = Environment.NewLine;
		foreach (Exception exception in exceptions)
		{
			stringBuilder.Append(value);
			stringBuilder.Append(exception.ToString());
			stringBuilder.Append(newLine);
			value = newLine;
		}
		return new NLogRuntimeException("Got multiple exceptions:\r\n" + stringBuilder);
	}

	private static AsynchronousAction ExceptionGuard(AsynchronousAction action)
	{
		return delegate(AsyncContinuation cont)
		{
			try
			{
				action(cont);
			}
			catch (Exception exception)
			{
				if (exception.MustBeRethrown())
				{
					throw;
				}
				cont(exception);
			}
		};
	}

	private static AsynchronousAction<T> ExceptionGuard<T>(AsynchronousAction<T> action)
	{
		return delegate(T argument, AsyncContinuation cont)
		{
			try
			{
				action(argument, cont);
			}
			catch (Exception exception)
			{
				if (exception.MustBeRethrown())
				{
					throw;
				}
				cont(exception);
			}
		};
	}

	/// <summary>
	/// Disposes the Timer, and waits for it to leave the Timer-callback-method
	/// </summary>
	/// <param name="timer">The Timer object to dispose</param>
	/// <param name="timeout">Timeout to wait (TimeSpan.Zero means dispose without waiting)</param>
	/// <returns>Timer disposed within timeout (true/false)</returns>
	internal static bool WaitForDispose(this Timer timer, TimeSpan timeout)
	{
		timer.Change(-1, -1);
		if (timeout != TimeSpan.Zero)
		{
			ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
			if (timer.Dispose(manualResetEvent) && !manualResetEvent.WaitOne((int)timeout.TotalMilliseconds))
			{
				return false;
			}
			manualResetEvent.Close();
		}
		else
		{
			timer.Dispose();
		}
		return true;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Targets.Wrappers;

namespace NLog.Targets;

/// <summary>
/// Abstract Target with async Task support
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/How-to-write-a-custom-async-target">See NLog Wiki</a>
/// </remarks>
/// <example><code>
/// [Target("MyFirst")]
/// public sealed class MyFirstTarget : AsyncTaskTarget
/// {
///    public MyFirstTarget()
///    {
///        this.Host = "localhost";
///    }
///
///    public Layout Host { get; set; }
///
///    protected override Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken token)
///    {
///        string logMessage = this.RenderLogEvent(this.Layout, logEvent);
///        string hostName = this.RenderLogEvent(this.Host, logEvent);
///        return SendTheMessageToRemoteHost(hostName, logMessage);
///    }
///
///    private async Task SendTheMessageToRemoteHost(string hostName, string message)
///    {
///        // To be implemented
///    }
/// }
/// </code></example>
/// <seealso href="https://github.com/NLog/NLog/wiki/How-to-write-a-custom-async-target">Documentation on NLog Wiki</seealso>
public abstract class AsyncTaskTarget : TargetWithContext
{
	private readonly Timer _taskTimeoutTimer;

	private CancellationTokenSource _cancelTokenSource;

	private AsyncRequestQueueBase _requestQueue;

	private readonly Action _taskCancelledTokenReInit;

	private readonly Action<Task, object> _taskCompletion;

	private Task? _previousTask;

	private readonly Timer _lazyWriterTimer;

	private readonly LogEventInfo _flushEvent = LogEventInfo.Create(LogLevel.Off, null, "NLog Async Task Flush Event");

	private readonly ReusableAsyncLogEventList _reusableAsyncLogEventList = new ReusableAsyncLogEventList(200);

	private Tuple<List<LogEventInfo>, List<AsyncContinuation>>? _reusableLogEvents;

	private WaitCallback? _flushEventsInQueueDelegate;

	private bool _missingServiceTypes;

	private int? _retryDelayMilliseconds;

	private bool? _forceLockingQueue;

	/// <summary>
	/// How many milliseconds to delay the actual write operation to optimize for batching
	/// </summary>
	/// <remarks>Default: <see langword="1" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int TaskDelayMilliseconds { get; set; } = 1;

	/// <summary>
	/// How many seconds a Task is allowed to run before it is cancelled.
	/// </summary>
	/// <remarks>Default: <see langword="150" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int TaskTimeoutSeconds { get; set; } = 150;

	/// <summary>
	/// How many attempts to retry the same Task, before it is aborted
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int RetryCount { get; set; }

	/// <summary>
	/// How many milliseconds to wait before next retry (will double with each retry)
	/// </summary>
	/// <remarks>Default: <see langword="500" />ms</remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int RetryDelayMilliseconds
	{
		get
		{
			int? retryDelayMilliseconds = _retryDelayMilliseconds;
			if (!retryDelayMilliseconds.HasValue)
			{
				if (RetryCount <= 0 && OverflowAction == AsyncTargetWrapperOverflowAction.Discard)
				{
					return 50;
				}
				return 500;
			}
			return retryDelayMilliseconds.GetValueOrDefault();
		}
		set
		{
			_retryDelayMilliseconds = value;
		}
	}

	/// <summary>
	/// Gets or sets whether to use the locking queue, instead of a lock-free concurrent queue
	/// The locking queue is less concurrent when many logger threads, but reduces memory allocation
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public bool ForceLockingQueue
	{
		get
		{
			return _forceLockingQueue == true;
		}
		set
		{
			_forceLockingQueue = value;
		}
	}

	/// <summary>
	/// Gets or sets the action to be taken when the lazy writer thread request queue count
	/// exceeds the set limit.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Discard" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public AsyncTargetWrapperOverflowAction OverflowAction
	{
		get
		{
			return _requestQueue.OnOverflow;
		}
		set
		{
			_requestQueue.OnOverflow = value;
		}
	}

	/// <summary>
	/// Gets or sets the limit on the number of requests in the lazy writer thread request queue.
	/// </summary>
	/// <remarks>Default: <see langword="10000" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int QueueLimit
	{
		get
		{
			return _requestQueue.QueueLimit;
		}
		set
		{
			_requestQueue.QueueLimit = value;
		}
	}

	/// <summary>
	/// Gets or sets the number of log events that should be processed in a batch
	/// by the lazy writer thread.
	/// </summary>
	/// <remarks>Default: <see langword="1" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int BatchSize { get; set; } = 1;

	/// <summary>
	/// Task Scheduler used for processing async Tasks
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Threading.Tasks.TaskScheduler.Default" /></remarks>
	protected virtual TaskScheduler TaskScheduler => System.Threading.Tasks.TaskScheduler.Default;

	/// <summary>
	/// Constructor
	/// </summary>
	protected AsyncTaskTarget()
	{
		_taskCompletion = TaskCompletion;
		_taskCancelledTokenReInit = delegate
		{
			TaskCancelledTokenReInit(out CancellationTokenSource _);
		};
		_taskTimeoutTimer = new Timer(TaskTimeout, null, -1, -1);
		_requestQueue = new AsyncRequestQueue(10000, AsyncTargetWrapperOverflowAction.Discard);
		_lazyWriterTimer = new Timer(delegate
		{
			TaskStartNext(null, fullBatchCompleted: false);
		}, null, -1, -1);
		TaskCancelledTokenReInit(out _cancelTokenSource);
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		_missingServiceTypes = false;
		TaskCancelledTokenReInit(out CancellationTokenSource _);
		base.InitializeTarget();
		if (BatchSize <= 0)
		{
			BatchSize = 1;
		}
		if (!ForceLockingQueue && OverflowAction == AsyncTargetWrapperOverflowAction.Block && (decimal)BatchSize * 1.5m > (decimal)QueueLimit)
		{
			ForceLockingQueue = true;
		}
		if (_forceLockingQueue.HasValue && _forceLockingQueue.Value != _requestQueue is AsyncRequestQueue)
		{
			_requestQueue = (ForceLockingQueue ? ((AsyncRequestQueueBase)new AsyncRequestQueue(QueueLimit, OverflowAction)) : ((AsyncRequestQueueBase)new ConcurrentRequestQueue(QueueLimit, OverflowAction)));
		}
		if (BatchSize > QueueLimit)
		{
			BatchSize = QueueLimit;
		}
	}

	/// <summary>
	/// Override this to provide async task for writing a single logevent.
	/// <example>
	/// Example of how to override this method, and call custom async method
	/// <code>
	/// protected override Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken token)
	/// {
	///    return CustomWriteAsync(logEvent, token);
	/// }
	///
	/// private async Task CustomWriteAsync(LogEventInfo logEvent, CancellationToken token)
	/// {
	///     await MyLogMethodAsync(logEvent, token).ConfigureAwait(false);
	/// }
	/// </code></example>
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	protected abstract Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken);

	/// <summary>
	/// Override this to provide async task for writing a batch of logevents.
	/// </summary>
	/// <param name="logEvents">A batch of logevents.</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns></returns>
	protected virtual Task WriteAsyncTask(IList<LogEventInfo> logEvents, CancellationToken cancellationToken)
	{
		if (logEvents.Count == 1)
		{
			return WriteAsyncTask(logEvents[0], cancellationToken);
		}
		Task task = null;
		for (int i = 0; i < logEvents.Count; i++)
		{
			LogEventInfo logEvent = logEvents[i];
			task = ((task != null) ? task.ContinueWith((Task t) => WriteAsyncTask(logEvent, cancellationToken), cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler).Unwrap() : WriteAsyncTask(logEvent, cancellationToken));
		}
		return task ?? Task.CompletedTask;
	}

	/// <summary>
	/// Handle cleanup after failed write operation
	/// </summary>
	/// <param name="exception">Exception from previous failed Task</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <param name="retryCountRemaining">Number of retries remaining</param>
	/// <param name="retryDelay">Time to sleep before retrying</param>
	/// <returns>Should attempt retry</returns>
	protected virtual bool RetryFailedAsyncTask(Exception exception, CancellationToken cancellationToken, int retryCountRemaining, out TimeSpan retryDelay)
	{
		if (cancellationToken.IsCancellationRequested || retryCountRemaining < 0)
		{
			retryDelay = TimeSpan.Zero;
			return false;
		}
		retryDelay = TimeSpan.FromMilliseconds((double)RetryDelayMilliseconds * Math.Pow(2.0, RetryCount - (1 + retryCountRemaining)));
		return true;
	}

	/// <summary>
	/// Block for override. Instead override <see cref="M:NLog.Targets.AsyncTaskTarget.WriteAsyncTask(NLog.LogEventInfo,System.Threading.CancellationToken)" />
	/// </summary>
	protected sealed override void Write(LogEventInfo logEvent)
	{
		base.Write(logEvent);
	}

	/// <summary>
	/// Block for override. Instead override <see cref="M:NLog.Targets.AsyncTaskTarget.WriteAsyncTask(System.Collections.Generic.IList{NLog.LogEventInfo},System.Threading.CancellationToken)" />
	/// </summary>
	protected sealed override void Write(IList<AsyncLogEventInfo> logEvents)
	{
		base.Write(logEvents);
	}

	/// <inheritdoc />
	protected sealed override void Write(AsyncLogEventInfo logEvent)
	{
		if (_cancelTokenSource.IsCancellationRequested)
		{
			logEvent.Continuation(null);
			return;
		}
		PrecalculateVolatileLayouts(logEvent.LogEvent);
		if (!_requestQueue.Enqueue(logEvent))
		{
			return;
		}
		bool lockTaken = false;
		try
		{
			if (_previousTask == null)
			{
				Monitor.Enter(base.SyncRoot, ref lockTaken);
			}
			else
			{
				Monitor.TryEnter(base.SyncRoot, 50, ref lockTaken);
			}
			if (_previousTask == null)
			{
				_lazyWriterTimer.Change(TaskDelayMilliseconds, -1);
			}
		}
		finally
		{
			if (lockTaken)
			{
				Monitor.Exit(base.SyncRoot);
			}
		}
	}

	/// <summary>
	/// Write to queue without locking <see cref="P:NLog.Targets.Target.SyncRoot" />
	/// </summary>
	/// <param name="logEvent"></param>
	protected sealed override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
	{
		try
		{
			Write(logEvent);
		}
		catch (Exception exception)
		{
			if (ExceptionMustBeRethrown(exception, "WriteAsyncThreadSafe"))
			{
				throw;
			}
			logEvent.Continuation(exception);
		}
	}

	/// <summary>
	/// Block for override. Instead override <see cref="M:NLog.Targets.AsyncTaskTarget.WriteAsyncTask(System.Collections.Generic.IList{NLog.LogEventInfo},System.Threading.CancellationToken)" />
	/// </summary>
	protected sealed override void WriteAsyncThreadSafe(IList<AsyncLogEventInfo> logEvents)
	{
		base.WriteAsyncThreadSafe(logEvents);
	}

	/// <summary>
	/// LogEvent is written to target, but target failed to successfully initialize
	///
	/// Enqueue logevent for later processing when target failed to initialize because of unresolved service dependency.
	/// </summary>
	protected override void WriteFailedNotInitialized(AsyncLogEventInfo logEvent, Exception initializeException)
	{
		if (initializeException is NLogDependencyResolveException && OverflowAction == AsyncTargetWrapperOverflowAction.Discard)
		{
			_missingServiceTypes = true;
			Write(logEvent);
		}
		else
		{
			base.WriteFailedNotInitialized(logEvent, initializeException);
		}
	}

	/// <summary>
	/// Schedules notification of when all messages has been written
	/// </summary>
	/// <param name="asyncContinuation"></param>
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		Task? previousTask = _previousTask;
		if ((previousTask != null && !previousTask.IsCompleted) || !_requestQueue.IsEmpty)
		{
			InternalLogger.Debug("{0}: Flushing {1}", this, _requestQueue.IsEmpty ? "empty queue" : "pending queue items");
			if (_requestQueue.OnOverflow != AsyncTargetWrapperOverflowAction.Block)
			{
				_requestQueue.Enqueue(new AsyncLogEventInfo(_flushEvent, asyncContinuation));
				_lazyWriterTimer.Change(0, -1);
				return;
			}
			if (_flushEventsInQueueDelegate == null)
			{
				_flushEventsInQueueDelegate = delegate(object cont)
				{
					_requestQueue.Enqueue(new AsyncLogEventInfo(_flushEvent, (AsyncContinuation)cont));
					lock (base.SyncRoot)
					{
						_lazyWriterTimer.Change(0, -1);
					}
				};
			}
			AsyncHelpers.StartAsyncTask(_flushEventsInQueueDelegate, asyncContinuation);
			_lazyWriterTimer.Change(0, -1);
		}
		else
		{
			InternalLogger.Debug("{0}: Flushing Nothing", this);
			asyncContinuation(null);
		}
	}

	/// <summary>
	/// Closes Target by updating CancellationToken
	/// </summary>
	protected override void CloseTarget()
	{
		_taskTimeoutTimer.Change(-1, -1);
		_cancelTokenSource.Cancel();
		_requestQueue.Clear();
		_previousTask = null;
		base.CloseTarget();
	}

	/// <summary>
	/// Releases any managed resources
	/// </summary>
	/// <param name="disposing"></param>
	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		if (disposing)
		{
			_cancelTokenSource.Dispose();
			_taskTimeoutTimer.WaitForDispose(TimeSpan.Zero);
			_lazyWriterTimer.WaitForDispose(TimeSpan.Zero);
		}
	}

	/// <summary>
	/// Checks the internal queue for the next <see cref="T:NLog.LogEventInfo" /> to create a new task for
	/// </summary>
	/// <param name="previousTask">Used for race-condition validation between task-completion and timeout</param>
	/// <param name="fullBatchCompleted">Signals whether previousTask completed an almost full BatchSize</param>
	private void TaskStartNext(object? previousTask, bool fullBatchCompleted)
	{
		do
		{
			lock (base.SyncRoot)
			{
				if (CheckOtherTask(previousTask))
				{
					break;
				}
				if (_missingServiceTypes)
				{
					_previousTask = null;
					_lazyWriterTimer.Change(50, -1);
					break;
				}
				if (!base.IsInitialized)
				{
					_previousTask = null;
					break;
				}
				if (previousTask != null && !fullBatchCompleted && TaskDelayMilliseconds >= 50 && !_requestQueue.IsEmpty)
				{
					InternalLogger.Trace("{0}: Throttle to optimize batching", this);
					_previousTask = null;
					_lazyWriterTimer.Change(TaskDelayMilliseconds, -1);
					break;
				}
				ReusableObjectCreator<IList<AsyncLogEventInfo>>.LockOject lockOject = _reusableAsyncLogEventList.Allocate();
				try
				{
					IList<AsyncLogEventInfo> result = lockOject.Result;
					_requestQueue.DequeueBatch(BatchSize, result);
					if (result.Count > 0)
					{
						if (TaskCreation(result))
						{
							break;
						}
						continue;
					}
					_previousTask = null;
					break;
				}
				finally
				{
					((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
				}
			}
		}
		while (!_requestQueue.IsEmpty || previousTask != null);
	}

	private bool CheckOtherTask(object? previousTask)
	{
		if (previousTask == null)
		{
			Task? previousTask2 = _previousTask;
			if (previousTask2 != null && !previousTask2.IsCompleted)
			{
				return true;
			}
		}
		else if (_previousTask != null && previousTask != _previousTask)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Generates recursive task-chain to perform retry of writing logevents with increasing retry-delay
	/// </summary>
	internal Task WriteAsyncTaskWithRetry(Task firstTask, IList<LogEventInfo> logEvents, CancellationToken cancellationToken, int retryCount)
	{
		TaskCompletionSource<object?> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
		return firstTask.ContinueWith(delegate(Task t)
		{
			if (t.IsFaulted || t.IsCanceled)
			{
				if (t.Exception != null)
				{
					tcs.TrySetException(t.Exception);
				}
				Exception ex = ExtractActualException(t.Exception) ?? new TaskCanceledException("Task failed without exception");
				if (RetryFailedAsyncTask(ex, cancellationToken, retryCount - 1, out var retryDelay))
				{
					InternalLogger.Warn(ex, "{0}: Write operation failed. {1} attempts left. Sleep {2} ms", this, retryCount, retryDelay.TotalMilliseconds);
					AsyncHelpers.WaitForDelay(retryDelay);
					if (!cancellationToken.IsCancellationRequested)
					{
						Task task;
						lock (base.SyncRoot)
						{
							task = StartWriteAsyncTask(logEvents, cancellationToken);
						}
						if (task != null)
						{
							return WriteAsyncTaskWithRetry(task, logEvents, cancellationToken, retryCount - 1);
						}
					}
				}
				InternalLogger.Warn(ex, "{0}: Write operation failed after {1} retries", this, RetryCount - retryCount);
			}
			else
			{
				tcs.SetResult(null);
			}
			return tcs.Task;
		}, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler).Unwrap();
	}

	/// <summary>
	/// Creates new task to handle the writing of the input <see cref="T:NLog.LogEventInfo" />
	/// </summary>
	/// <param name="logEvents">LogEvents to write</param>
	/// <returns>New Task created [true / false]</returns>
	private bool TaskCreation(IList<AsyncLogEventInfo> logEvents)
	{
		Tuple<List<LogEventInfo>, List<AsyncContinuation>> tuple = null;
		try
		{
			if (_cancelTokenSource.IsCancellationRequested)
			{
				for (int i = 0; i < logEvents.Count; i++)
				{
					logEvents[i].Continuation(null);
				}
				return false;
			}
			tuple = Interlocked.CompareExchange(ref _reusableLogEvents, null, _reusableLogEvents) ?? Tuple.Create(new List<LogEventInfo>(), new List<AsyncContinuation>());
			for (int j = 0; j < logEvents.Count; j++)
			{
				if (logEvents[j].LogEvent == _flushEvent)
				{
					tuple.Item2.Add(logEvents[j].Continuation);
					continue;
				}
				tuple.Item1.Add(logEvents[j].LogEvent);
				tuple.Item2.Add(logEvents[j].Continuation);
			}
			if (tuple.Item1.Count == 0)
			{
				NotifyTaskCompletion(tuple.Item2, null);
				tuple.Item2.Clear();
				Interlocked.CompareExchange(ref _reusableLogEvents, tuple, null);
				InternalLogger.Debug("{0}: Flush Completed", this);
				return false;
			}
			Task task = StartWriteAsyncTask(tuple.Item1, _cancelTokenSource.Token);
			if (task == null)
			{
				InternalLogger.Debug("{0}: WriteAsyncTask returned null", this);
				NotifyTaskCompletion(tuple.Item2, null);
				return false;
			}
			if (RetryCount > 0)
			{
				task = WriteAsyncTaskWithRetry(task, tuple.Item1, _cancelTokenSource.Token, RetryCount);
			}
			_previousTask = task;
			if (TaskTimeoutSeconds > 0)
			{
				_taskTimeoutTimer.Change(TaskTimeoutSeconds * 1000, -1);
			}
			task.ContinueWith(_taskCompletion, tuple, TaskScheduler);
			return true;
		}
		catch (Exception ex)
		{
			_previousTask = null;
			InternalLogger.Error(ex, "{0}: WriteAsyncTask failed on creation", this);
			IList<AsyncContinuation> list = tuple?.Item2;
			NotifyTaskCompletion(list ?? ArrayHelper.Empty<AsyncContinuation>(), ex);
		}
		return false;
	}

	private Task StartWriteAsyncTask(IList<LogEventInfo> logEvents, CancellationToken cancellationToken)
	{
		try
		{
			InternalLogger.Trace("{0}: Writing {1} events", this, logEvents.Count);
			Task task = WriteAsyncTask(logEvents, cancellationToken);
			if (task == null)
			{
				return Task.CompletedTask;
			}
			if (task.Status == TaskStatus.Created)
			{
				task.Start(TaskScheduler);
			}
			return task;
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: WriteAsyncTask failed on creation", this);
			return Task.FromException(ex);
		}
	}

	private static void NotifyTaskCompletion(IList<AsyncContinuation> reusableContinuations, Exception? ex)
	{
		try
		{
			for (int i = 0; i < reusableContinuations.Count; i++)
			{
				reusableContinuations[i](ex);
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Handles that scheduled task has completed (successfully or failed), and starts the next pending task
	/// </summary>
	/// <param name="completedTask">Task just completed</param>
	/// <param name="continuation">AsyncContinuation to notify of success or failure</param>
	private void TaskCompletion(Task completedTask, object continuation)
	{
		bool flag = true;
		bool fullBatchCompleted = true;
		TimeSpan retryDelay = TimeSpan.Zero;
		try
		{
			if (completedTask == _previousTask)
			{
				if (TaskTimeoutSeconds > 0)
				{
					_taskTimeoutTimer.Change(-1, -1);
				}
			}
			else
			{
				flag = false;
				if (!base.IsInitialized)
				{
					return;
				}
			}
			Exception ex = ExtractActualException(completedTask.Exception);
			if (completedTask.IsCanceled)
			{
				flag = false;
				if (completedTask.Exception != null)
				{
					InternalLogger.Warn(completedTask.Exception, "{0}: WriteAsyncTask was cancelled", this);
				}
				else
				{
					InternalLogger.Info("{0}: WriteAsyncTask was cancelled", this);
				}
			}
			else if (ex != null)
			{
				flag = false;
				if (RetryCount <= 0)
				{
					if (RetryFailedAsyncTask(ex, CancellationToken.None, 0, out retryDelay))
					{
						InternalLogger.Warn(ex, "{0}: WriteAsyncTask failed on completion. Delay {1} ms", this, retryDelay.TotalMilliseconds);
					}
				}
				else
				{
					InternalLogger.Warn(ex, "{0}: WriteAsyncTask failed on completion", this);
				}
			}
			Tuple<List<LogEventInfo>, List<AsyncContinuation>> tuple = continuation as Tuple<List<LogEventInfo>, List<AsyncContinuation>>;
			if (tuple != null)
			{
				NotifyTaskCompletion(tuple.Item2, ex);
			}
			else
			{
				flag = false;
			}
			if (flag && tuple != null)
			{
				fullBatchCompleted = tuple.Item2.Count * 2 > BatchSize;
				tuple.Item1.Clear();
				tuple.Item2.Clear();
				Interlocked.CompareExchange(ref _reusableLogEvents, tuple, null);
			}
		}
		finally
		{
			if (retryDelay > TimeSpan.Zero)
			{
				_lazyWriterTimer.Change((int)retryDelay.TotalMilliseconds, -1);
			}
			else
			{
				TaskStartNext(completedTask, fullBatchCompleted);
			}
		}
	}

	/// <summary>
	/// Timer method, that is fired when pending task fails to complete within timeout
	/// </summary>
	/// <param name="state"></param>
	private void TaskTimeout(object state)
	{
		try
		{
			if (!base.IsInitialized)
			{
				return;
			}
			InternalLogger.Warn("{0}: WriteAsyncTask had timeout. Task will be cancelled.", this);
			Task task = _previousTask;
			try
			{
				lock (base.SyncRoot)
				{
					if (task != null && task == _previousTask)
					{
						_previousTask = null;
						_cancelTokenSource.Cancel();
					}
					else
					{
						task = null;
					}
				}
				if (task != null)
				{
					if (!WaitTaskIsCompleted(task, TimeSpan.FromSeconds((double)TaskTimeoutSeconds / 10.0)))
					{
						InternalLogger.Debug("{0}: WriteAsyncTask had timeout. Task did not cancel properly: {1}.", this, task.Status);
					}
					Exception ex = ExtractActualException(task.Exception);
					RetryFailedAsyncTask(ex ?? new TimeoutException("WriteAsyncTask had timeout"), CancellationToken.None, 0, out var _);
				}
			}
			catch (Exception ex2)
			{
				InternalLogger.Debug(ex2, "{0}: WriteAsyncTask had timeout. Task failed to cancel properly.", this);
			}
			TaskStartNext(null, fullBatchCompleted: false);
		}
		catch (Exception ex3)
		{
			InternalLogger.Error(ex3, "{0}: WriteAsyncTask failed on timeout", this);
		}
	}

	private static bool WaitTaskIsCompleted(Task task, TimeSpan timeout)
	{
		while (!task.IsCompleted && timeout > TimeSpan.Zero)
		{
			timeout -= TimeSpan.FromMilliseconds(10.0);
			AsyncHelpers.WaitForDelay(TimeSpan.FromMilliseconds(10.0));
		}
		return task.IsCompleted;
	}

	private static Exception? ExtractActualException(AggregateException? taskException)
	{
		if (taskException != null && taskException.InnerExceptions?.Count == 1 && !(taskException.InnerExceptions[0] is AggregateException))
		{
			return taskException.InnerExceptions[0];
		}
		ReadOnlyCollection<Exception> readOnlyCollection = taskException?.Flatten()?.InnerExceptions;
		if (readOnlyCollection == null || readOnlyCollection.Count != 1)
		{
			return taskException;
		}
		return readOnlyCollection[0];
	}

	private void TaskCancelledTokenReInit(out CancellationTokenSource cancelTokenSource)
	{
		_cancelTokenSource = (cancelTokenSource = new CancellationTokenSource());
		_cancelTokenSource.Token.Register(_taskCancelledTokenReInit);
	}
}

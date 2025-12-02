using System;
using System.Collections.Generic;
using System.Threading;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Provides asynchronous, buffered execution of target writes.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/AsyncWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/AsyncWrapper-target">Documentation on NLog Wiki</seealso>
/// <remarks>
/// <p>
/// Asynchronous target wrapper allows the logger code to execute more quickly, by queuing
/// messages and processing them in a separate thread. You should wrap targets
/// that spend a non-trivial amount of time in their Write() method with asynchronous
/// target to speed up logging.
/// </p>
/// <p>
/// Because asynchronous logging is quite a common scenario, NLog supports a
/// shorthand notation for wrapping all targets with AsyncWrapper. Just add async="true" to
/// the &lt;targets/&gt; element in the configuration file.
/// </p>
/// <code lang="XML">
/// <![CDATA[
/// <targets async="true">
///    ... your targets go here ...
/// </targets>
/// ]]></code>
/// </remarks>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/AsyncWrapper/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/AsyncWrapper/Wrapping File/Example.cs" />
/// </example>
[Target("AsyncWrapper", IsWrapper = true)]
public class AsyncTargetWrapper : WrapperTargetBase
{
	private readonly object _writeLockObject = new object();

	private readonly object _timerLockObject = new object();

	private Timer? _lazyWriterTimer;

	private readonly ReusableAsyncLogEventList _reusableAsyncLogEventList = new ReusableAsyncLogEventList(200);

	private readonly WaitCallback _flushEventsInQueueDelegate;

	private bool _missingServiceTypes;

	private bool? _forceLockingQueue;

	/// <summary>
	/// Gets the queue of lazy writer thread requests.
	/// </summary>
	private AsyncRequestQueueBase _requestQueue;

	/// <summary>
	/// Gets or sets the number of log events that should be processed in a batch
	/// by the lazy writer thread.
	/// </summary>
	/// <remarks>Default: <see langword="200" /></remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int BatchSize { get; set; } = 200;

	/// <summary>
	/// Gets or sets the time in milliseconds to sleep between batches. (1 or less means trigger on new activity)
	/// </summary>
	/// <remarks>Default: <see langword="1" />ms</remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int TimeToSleepBetweenBatches { get; set; } = 1;

	/// <summary>
	/// Gets or sets the action to be taken when the lazy writer thread request queue count
	/// exceeds the set limit.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Discard" /></remarks>
	/// <docgen category="Buffering Options" order="10" />
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
	/// <docgen category="Buffering Options" order="10" />
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
	/// Gets or sets the number of batches of <see cref="P:NLog.Targets.Wrappers.AsyncTargetWrapper.BatchSize" /> to write before yielding into <see cref="P:NLog.Targets.Wrappers.AsyncTargetWrapper.TimeToSleepBetweenBatches" />
	/// </summary>
	/// <remarks>Default: <see langword="10" /> . Better performance when writing small batches, than single large batch.</remarks>
	/// <docgen category="Buffering Options" order="100" />
	public int FullBatchSizeWriteLimit { get; set; } = 10;

	/// <summary>
	/// Gets or sets whether to use the locking queue, instead of a lock-free concurrent queue
	/// </summary>
	/// <remarks>Default: <see langword="false" /> . Queue with Monitor.Lock is less concurrent when many logger threads, but reduces memory allocation</remarks>
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

	private event EventHandler<LogEventDroppedEventArgs>? _logEventDroppedEvent;

	private event EventHandler<LogEventQueueGrowEventArgs>? _eventQueueGrowEvent;

	/// <summary>
	/// Occurs when LogEvent has been dropped, because internal queue is full and <see cref="P:NLog.Targets.Wrappers.AsyncTargetWrapper.OverflowAction" /> set to <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Discard" />
	/// </summary>
	public event EventHandler<LogEventDroppedEventArgs> LogEventDropped
	{
		add
		{
			if (this._logEventDroppedEvent == null)
			{
				_requestQueue.LogEventDropped += OnRequestQueueDropItem;
			}
			_logEventDroppedEvent += value;
		}
		remove
		{
			_logEventDroppedEvent -= value;
			if (this._logEventDroppedEvent == null)
			{
				_requestQueue.LogEventDropped -= OnRequestQueueDropItem;
			}
		}
	}

	/// <summary>
	/// Occurs when internal queue size is growing, because internal queue is full and <see cref="P:NLog.Targets.Wrappers.AsyncTargetWrapper.OverflowAction" /> set to <see cref="F:NLog.Targets.Wrappers.AsyncTargetWrapperOverflowAction.Grow" />
	/// </summary>
	public event EventHandler<LogEventQueueGrowEventArgs> EventQueueGrow
	{
		add
		{
			if (this._eventQueueGrowEvent == null)
			{
				_requestQueue.LogEventQueueGrow += OnRequestQueueGrow;
			}
			_eventQueueGrowEvent += value;
		}
		remove
		{
			_eventQueueGrowEvent -= value;
			if (this._eventQueueGrowEvent == null)
			{
				_requestQueue.LogEventQueueGrow -= OnRequestQueueGrow;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AsyncTargetWrapper" /> class.
	/// </summary>
	public AsyncTargetWrapper()
	{
		_requestQueue = new AsyncRequestQueue(10000, AsyncTargetWrapperOverflowAction.Discard);
		_flushEventsInQueueDelegate = FlushEventsInQueue;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AsyncTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public AsyncTargetWrapper(string name, Target wrappedTarget)
		: this(wrappedTarget)
	{
		base.Name = name ?? base.Name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AsyncTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public AsyncTargetWrapper(Target wrappedTarget)
		: this(wrappedTarget, 10000, AsyncTargetWrapperOverflowAction.Discard)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AsyncTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="queueLimit">Maximum number of requests in the queue.</param>
	/// <param name="overflowAction">The action to be taken when the queue overflows.</param>
	public AsyncTargetWrapper(Target wrappedTarget, int queueLimit, AsyncTargetWrapperOverflowAction overflowAction)
		: this()
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		QueueLimit = queueLimit;
		OverflowAction = overflowAction;
	}

	/// <summary>
	/// Schedules a flush of pending events in the queue (if any), followed by flushing the WrappedTarget.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		AsyncHelpers.StartAsyncTask(_flushEventsInQueueDelegate, asyncContinuation);
	}

	/// <summary>
	/// Initializes the target by starting the lazy writer timer.
	/// </summary>
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		if (!ForceLockingQueue && OverflowAction == AsyncTargetWrapperOverflowAction.Block && (decimal)BatchSize * 1.5m > (decimal)QueueLimit)
		{
			ForceLockingQueue = true;
		}
		if (_forceLockingQueue.HasValue && _forceLockingQueue.Value != _requestQueue is AsyncRequestQueue)
		{
			_requestQueue = (ForceLockingQueue ? ((AsyncRequestQueueBase)new AsyncRequestQueue(QueueLimit, OverflowAction)) : ((AsyncRequestQueueBase)new ConcurrentRequestQueue(QueueLimit, OverflowAction)));
		}
		if (BatchSize > QueueLimit && TimeToSleepBetweenBatches <= 1)
		{
			BatchSize = QueueLimit;
		}
		_layoutWithLock = _layoutWithLock ?? base.WrappedTarget?._layoutWithLock;
		if (base.WrappedTarget != null && base.WrappedTarget.InitializeException is NLogDependencyResolveException && OverflowAction == AsyncTargetWrapperOverflowAction.Discard)
		{
			_missingServiceTypes = true;
			InternalLogger.Debug("{0} WrappedTarget has unresolved missing dependencies.", this);
		}
		_requestQueue.Clear();
		InternalLogger.Trace("{0}: Start Timer", this);
		_lazyWriterTimer = new Timer(ProcessPendingEvents, null, -1, -1);
		StartLazyWriterTimer();
	}

	/// <summary>
	/// Shuts down the lazy writer timer.
	/// </summary>
	protected override void CloseTarget()
	{
		StopLazyWriterThread();
		int millisecondsTimeout = ((OverflowAction == AsyncTargetWrapperOverflowAction.Discard) ? 500 : 1500);
		if (Monitor.TryEnter(_writeLockObject, millisecondsTimeout))
		{
			try
			{
				WriteLogEventsInQueue(int.MaxValue, "Closing Target");
			}
			finally
			{
				Monitor.Exit(_writeLockObject);
			}
		}
		else
		{
			InternalLogger.Debug("{0}: Failed to flush after lock timeout", this);
		}
		if (OverflowAction == AsyncTargetWrapperOverflowAction.Block)
		{
			_requestQueue.Clear();
		}
		base.CloseTarget();
	}

	/// <summary>
	/// Starts the lazy writer thread which periodically writes
	/// queued log messages.
	/// </summary>
	protected virtual void StartLazyWriterTimer()
	{
		if (TimeToSleepBetweenBatches <= 1)
		{
			StartTimerUnlessWriterActive(instant: false);
			return;
		}
		lock (_timerLockObject)
		{
			_lazyWriterTimer?.Change(TimeToSleepBetweenBatches, -1);
		}
	}

	/// <summary>
	/// Attempts to start an instant timer-worker-thread which can write
	/// queued log messages.
	/// </summary>
	/// <returns>Returns <see langword="true" /> when scheduled a timer-worker-thread</returns>
	protected virtual bool StartInstantWriterTimer()
	{
		return StartLazyWriterThread(instant: true);
	}

	private void StartTimerUnlessWriterActive(bool instant)
	{
		bool flag = false;
		try
		{
			flag = Monitor.TryEnter(_writeLockObject);
			if (flag)
			{
				if (instant)
				{
					StartInstantWriterTimer();
				}
				else
				{
					StartLazyWriterThread(instant: false);
				}
			}
			else
			{
				InternalLogger.Trace("{0}: Timer not scheduled, since already active", this);
			}
		}
		finally
		{
			if (flag)
			{
				Monitor.Exit(_writeLockObject);
			}
		}
	}

	private bool StartLazyWriterThread(bool instant)
	{
		lock (_timerLockObject)
		{
			if (_lazyWriterTimer != null)
			{
				if (instant)
				{
					InternalLogger.Trace("{0}: Timer scheduled instantly", this);
					_lazyWriterTimer.Change(0, -1);
				}
				else
				{
					InternalLogger.Trace("{0}: Timer scheduled throttled", this);
					_lazyWriterTimer.Change(1, -1);
				}
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Stops the lazy writer thread.
	/// </summary>
	protected virtual void StopLazyWriterThread()
	{
		lock (_timerLockObject)
		{
			Timer lazyWriterTimer = _lazyWriterTimer;
			if (lazyWriterTimer != null)
			{
				_lazyWriterTimer = null;
				lazyWriterTimer.WaitForDispose(TimeSpan.FromSeconds(1.0));
			}
		}
	}

	/// <summary>
	/// Adds the log event to asynchronous queue to be processed by
	/// the lazy writer thread.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <remarks>
	/// The <see cref="M:NLog.Targets.Target.PrecalculateVolatileLayouts(NLog.LogEventInfo)" /> is called
	/// to ensure that the log event can be processed in another thread.
	/// </remarks>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		PrecalculateVolatileLayouts(logEvent.LogEvent);
		if (_requestQueue.Enqueue(logEvent))
		{
			if (TimeToSleepBetweenBatches == 0)
			{
				StartTimerUnlessWriterActive(instant: true);
			}
			else if (TimeToSleepBetweenBatches <= 1)
			{
				StartLazyWriterTimer();
			}
		}
	}

	/// <summary>
	/// Write to queue without locking <see cref="P:NLog.Targets.Target.SyncRoot" />
	/// </summary>
	/// <param name="logEvent"></param>
	protected override void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
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

	private void ProcessPendingEvents(object state)
	{
		if (_lazyWriterTimer == null)
		{
			return;
		}
		bool flag = false;
		try
		{
			lock (_writeLockObject)
			{
				flag = WriteLogEventsInQueue(BatchSize, "Timer") == BatchSize;
				if (flag && TimeToSleepBetweenBatches <= 1)
				{
					StartInstantWriterTimer();
				}
			}
		}
		catch (Exception ex)
		{
			flag = false;
			InternalLogger.Error(ex, "{0}: Error in lazy writer timer procedure.", this);
		}
		finally
		{
			if (TimeToSleepBetweenBatches <= 1)
			{
				if (!flag)
				{
					if (!_requestQueue.IsEmpty)
					{
						StartLazyWriterTimer();
					}
					else
					{
						InternalLogger.Trace("{0}: Timer not scheduled, since queue empty", this);
					}
				}
			}
			else
			{
				StartLazyWriterTimer();
			}
		}
	}

	private void FlushEventsInQueue(object state)
	{
		AsyncContinuation asyncContinuation = state as AsyncContinuation;
		if (Monitor.TryEnter(_writeLockObject, 1500))
		{
			try
			{
				WriteLogEventsInQueue(int.MaxValue, "Flush Async");
				if (asyncContinuation != null)
				{
					base.FlushAsync(asyncContinuation);
				}
				return;
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "{0}: Error in flush procedure.", this);
				return;
			}
			finally
			{
				Monitor.Exit(_writeLockObject);
				if (TimeToSleepBetweenBatches <= 1 && !_requestQueue.IsEmpty)
				{
					StartLazyWriterTimer();
				}
			}
		}
		asyncContinuation?.Invoke(new NLogRuntimeException($"Target {this} failed to flush after lock timeout."));
	}

	private int WriteLogEventsInQueue(int batchSize, string reason)
	{
		if (base.WrappedTarget == null)
		{
			InternalLogger.Error("{0}: WrappedTarget is NULL", this);
			return 0;
		}
		if (_missingServiceTypes)
		{
			if (base.WrappedTarget.InitializeException is NLogDependencyResolveException)
			{
				return 0;
			}
			_missingServiceTypes = false;
			InternalLogger.Debug("{0}: WrappedTarget has resolved missing dependency", this);
		}
		if (batchSize == int.MaxValue)
		{
			AsyncLogEventInfo[] logEvents = _requestQueue.DequeueBatch(int.MaxValue);
			return WriteLogEventsToTarget(logEvents, reason);
		}
		int num = 0;
		for (int i = 0; i < FullBatchSizeWriteLimit; i++)
		{
			ReusableObjectCreator<IList<AsyncLogEventInfo>>.LockOject lockOject = _reusableAsyncLogEventList.Allocate();
			try
			{
				IList<AsyncLogEventInfo> result = lockOject.Result;
				_requestQueue.DequeueBatch(batchSize, result);
				num = WriteLogEventsToTarget(result, reason);
			}
			finally
			{
				((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
			}
			if (num < batchSize)
			{
				break;
			}
		}
		return num;
	}

	private int WriteLogEventsToTarget(IList<AsyncLogEventInfo> logEvents, string reason)
	{
		int count = logEvents.Count;
		if (count > 0)
		{
			if (reason != null)
			{
				InternalLogger.Trace("{0}: Writing {1} events ({2})", this, count, reason);
			}
			base.WrappedTarget?.WriteAsyncLogEvents(logEvents);
		}
		return count;
	}

	private void OnRequestQueueDropItem(object sender, LogEventDroppedEventArgs logEventDroppedEventArgs)
	{
		this._logEventDroppedEvent?.Invoke(this, logEventDroppedEventArgs);
	}

	private void OnRequestQueueGrow(object sender, LogEventQueueGrowEventArgs logEventQueueGrowEventArgs)
	{
		this._eventQueueGrowEvent?.Invoke(this, logEventQueueGrowEventArgs);
	}
}

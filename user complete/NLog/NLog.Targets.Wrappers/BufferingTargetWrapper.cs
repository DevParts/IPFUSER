using System;
using System.Threading;
using NLog.Common;
using NLog.Layouts;

namespace NLog.Targets.Wrappers;

/// <summary>
/// A target that buffers log events and sends them in batches to the wrapped target.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/BufferingWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/BufferingWrapper-target">Documentation on NLog Wiki</seealso>
[Target("BufferingWrapper", IsWrapper = true)]
public class BufferingTargetWrapper : WrapperTargetBase
{
	private AsyncRequestQueue _buffer = new AsyncRequestQueue(100, AsyncTargetWrapperOverflowAction.Discard);

	private Timer? _flushTimer;

	private readonly object _lockObject = new object();

	/// <summary>
	/// Gets or sets the number of log events to be buffered.
	/// </summary>
	/// <remarks>Default: <see langword="100" /></remarks>
	/// <docgen category="Buffering Options" order="10" />
	public Layout<int> BufferSize { get; set; }

	/// <summary>
	/// Gets or sets the timeout (in milliseconds) after which the contents of buffer will be flushed
	/// if there's no write in the specified period of time. Use -1 to disable timed flushes.
	/// </summary>
	/// <remarks>Default: <see langword="-1" /> . Zero or Negative means disabled.</remarks>
	/// <docgen category="Buffering Options" order="100" />
	public Layout<int> FlushTimeout { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to use sliding timeout.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="true" /> . 
	/// This value determines how the inactivity period is determined. When <see langword="true" />
	/// the inactivity timer is reset after each write, if <see langword="false" />- inactivity timer will
	/// count from the first event written to the buffer.
	/// </remarks>
	/// <docgen category="Buffering Options" order="100" />
	public bool SlidingTimeout { get; set; } = true;

	/// <summary>
	/// Gets or sets the action to take if the buffer overflows.
	/// </summary>
	/// <remarks>
	/// Default: <see cref="F:NLog.Targets.Wrappers.BufferingTargetWrapperOverflowAction.Flush" /> . Setting to <see cref="F:NLog.Targets.Wrappers.BufferingTargetWrapperOverflowAction.Flush" />
	/// will flush the entire buffer to the wrapped target. Setting to <see cref="F:NLog.Targets.Wrappers.BufferingTargetWrapperOverflowAction.Discard" />
	/// will replace the oldest event with new events without sending events down to the wrapped target.
	/// </remarks>
	/// <docgen category="Buffering Options" order="50" />
	public BufferingTargetWrapperOverflowAction OverflowAction { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> class.
	/// </summary>
	public BufferingTargetWrapper()
	{
		BufferSize = 100;
		FlushTimeout = -1;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public BufferingTargetWrapper(string name, Target wrappedTarget)
		: this(wrappedTarget)
	{
		base.Name = name ?? base.Name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public BufferingTargetWrapper(Target wrappedTarget)
		: this(wrappedTarget, 100)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="bufferSize">Size of the buffer.</param>
	public BufferingTargetWrapper(Target wrappedTarget, int bufferSize)
		: this(wrappedTarget, bufferSize, -1)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="bufferSize">Size of the buffer.</param>
	/// <param name="flushTimeout">The flush timeout.</param>
	public BufferingTargetWrapper(Target wrappedTarget, int bufferSize, int flushTimeout)
		: this(wrappedTarget, bufferSize, flushTimeout, BufferingTargetWrapperOverflowAction.Flush)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="bufferSize">Size of the buffer.</param>
	/// <param name="flushTimeout">The flush timeout.</param>
	/// <param name="overflowAction">The action to take when the buffer overflows.</param>
	public BufferingTargetWrapper(Target wrappedTarget, int bufferSize, int flushTimeout, BufferingTargetWrapperOverflowAction overflowAction)
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		BufferSize = bufferSize;
		FlushTimeout = flushTimeout;
		OverflowAction = overflowAction;
	}

	/// <summary>
	/// Flushes pending events in the buffer (if any), followed by flushing the WrappedTarget.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		if (Monitor.TryEnter(_lockObject, 1500))
		{
			try
			{
				WriteEventsInBuffer("Flush Async");
				base.FlushAsync(asyncContinuation);
				return;
			}
			finally
			{
				Monitor.Exit(_lockObject);
			}
		}
		asyncContinuation(new NLogRuntimeException($"Target {this} failed to flush after lock timeout."));
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		int queueLimit = RenderLogEvent(BufferSize, LogEventInfo.CreateNullEvent(), 0);
		_buffer = new AsyncRequestQueue(queueLimit, AsyncTargetWrapperOverflowAction.Discard);
		InternalLogger.Trace("{0}: Create Timer", this);
		int num = RenderLogEvent(FlushTimeout, LogEventInfo.CreateNullEvent(), 0);
		_flushTimer = new Timer(FlushCallback, num, -1, -1);
	}

	/// <summary>
	/// Closes the target by flushing pending events in the buffer (if any).
	/// </summary>
	protected override void CloseTarget()
	{
		Timer flushTimer = _flushTimer;
		if (flushTimer != null)
		{
			_flushTimer = null;
			if (flushTimer.WaitForDispose(TimeSpan.FromSeconds(1.0)))
			{
				if (OverflowAction == BufferingTargetWrapperOverflowAction.Discard)
				{
					_buffer.Clear();
				}
				else
				{
					int millisecondsTimeout = ((OverflowAction == BufferingTargetWrapperOverflowAction.Discard) ? 500 : 1500);
					if (Monitor.TryEnter(_lockObject, millisecondsTimeout))
					{
						try
						{
							WriteEventsInBuffer("Closing Target");
						}
						finally
						{
							Monitor.Exit(_lockObject);
						}
					}
					else
					{
						InternalLogger.Debug("{0}: Failed to flush after lock timeout", this);
					}
				}
			}
		}
		base.CloseTarget();
	}

	/// <summary>
	/// Adds the specified log event to the buffer and flushes
	/// the buffer in case the buffer gets full.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		PrecalculateVolatileLayouts(logEvent.LogEvent);
		bool flag = _buffer.Enqueue(logEvent);
		if (_buffer.RequestCount >= _buffer.QueueLimit)
		{
			if (OverflowAction == BufferingTargetWrapperOverflowAction.Flush)
			{
				lock (_lockObject)
				{
					WriteEventsInBuffer("Exceeding BufferSize");
				}
			}
		}
		else if (SlidingTimeout || flag)
		{
			int num = RenderLogEvent(FlushTimeout, logEvent.LogEvent, 0);
			if (num > 0)
			{
				_flushTimer?.Change(num, -1);
			}
		}
	}

	private void FlushCallback(object state)
	{
		bool flag = false;
		try
		{
			int num = Math.Min((state as int?).GetValueOrDefault() / 2, 100);
			flag = Monitor.TryEnter(_lockObject, num);
			if (flag)
			{
				if (_flushTimer != null)
				{
					WriteEventsInBuffer(string.Empty);
				}
			}
			else if (!_buffer.IsEmpty)
			{
				_flushTimer?.Change(num, -1);
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: Error in flush procedure.", this);
		}
		finally
		{
			if (flag)
			{
				Monitor.Exit(_lockObject);
			}
		}
	}

	private void WriteEventsInBuffer(string reason)
	{
		if (base.WrappedTarget == null)
		{
			InternalLogger.Error("{0}: WrappedTarget is NULL", this);
			return;
		}
		AsyncLogEventInfo[] array = _buffer.DequeueBatch(int.MaxValue);
		if (array.Length != 0)
		{
			if (!string.IsNullOrEmpty(reason))
			{
				InternalLogger.Trace("{0}: Writing {1} events ({2})", this, array.Length, reason);
			}
			base.WrappedTarget.WriteAsyncLogEvents(array);
		}
	}
}

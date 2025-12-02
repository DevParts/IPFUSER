using System;
using NLog.Common;
using NLog.Conditions;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Causes a flush on a wrapped target if LogEvent satisfies the <see cref="P:NLog.Targets.Wrappers.AutoFlushTargetWrapper.Condition" />.
/// If condition isn't set, flushes on each write.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/AutoFlushWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/AutoFlushWrapper-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/AutoFlushWrapper/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/AutoFlushWrapper/Simple/Example.cs" />
/// </example>
[Target("AutoFlushWrapper", IsWrapper = true)]
public class AutoFlushTargetWrapper : WrapperTargetBase
{
	private bool? _asyncFlush;

	private readonly AsyncOperationCounter _pendingManualFlushList = new AsyncOperationCounter();

	private readonly AsyncContinuation _flushCompletedContinuation;

	/// <summary>
	/// Gets or sets the condition expression. Log events who meet this condition will cause
	/// a flush on the wrapped target.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="General Options" order="10" />
	public ConditionExpression? Condition { get; set; }

	/// <summary>
	/// Delay the flush until the LogEvent has been confirmed as written
	/// </summary>
	/// <remarks>Default: <see langword="true" /> . When not explicitly set, then automatically disabled when <see cref="T:NLog.Targets.Wrappers.BufferingTargetWrapper" /> or AsyncTaskTarget</remarks>
	/// <docgen category="General Options" order="10" />
	public bool AsyncFlush
	{
		get
		{
			return _asyncFlush ?? true;
		}
		set
		{
			_asyncFlush = value;
		}
	}

	/// <summary>
	/// Only flush when LogEvent matches condition. Ignore explicit-flush, config-reload-flush and shutdown-flush
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="General Options" order="10" />
	public bool FlushOnConditionOnly { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AutoFlushTargetWrapper" /> class.
	/// </summary>
	public AutoFlushTargetWrapper()
	{
		_flushCompletedContinuation = delegate(Exception? ex)
		{
			_pendingManualFlushList.CompleteOperation(ex);
		};
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AutoFlushTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="name">Name of the target</param>
	public AutoFlushTargetWrapper(string name, Target wrappedTarget)
		: this(wrappedTarget)
	{
		base.Name = name ?? base.Name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.AutoFlushTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public AutoFlushTargetWrapper(Target wrappedTarget)
		: this()
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		if (!_asyncFlush.HasValue && !TargetSupportsAsyncFlush())
		{
			AsyncFlush = false;
		}
	}

	private bool TargetSupportsAsyncFlush()
	{
		if (base.WrappedTarget is BufferingTargetWrapper)
		{
			return false;
		}
		if (base.WrappedTarget is AsyncTaskTarget)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Forwards the call to the <see cref="P:NLog.Targets.Wrappers.WrapperTargetBase.WrappedTarget" />.Write()
	/// and calls <see cref="M:NLog.Targets.Target.Flush(NLog.Common.AsyncContinuation)" /> on it if LogEvent satisfies
	/// the flush condition or condition is null.
	/// </summary>
	/// <param name="logEvent">Logging event to be written out.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		if (Condition == null || ConditionExpression.BoxedTrue.Equals(Condition.Evaluate(logEvent.LogEvent)))
		{
			if (AsyncFlush)
			{
				AsyncContinuation currentContinuation = logEvent.Continuation;
				AsyncContinuation asyncContinuation = delegate(Exception? ex)
				{
					_pendingManualFlushList.CompleteOperation(ex);
					if (ex == null)
					{
						AsyncContinuation asyncContinuation2 = _pendingManualFlushList.RegisterCompletionNotification(delegate
						{
						});
						FlushWrappedTarget(asyncContinuation2);
					}
					currentContinuation(ex);
				};
				_pendingManualFlushList.BeginOperation();
				base.WrappedTarget?.WriteAsyncLogEvent(logEvent.LogEvent.WithContinuation(asyncContinuation));
			}
			else
			{
				_pendingManualFlushList.BeginOperation();
				base.WrappedTarget?.WriteAsyncLogEvent(logEvent);
				FlushWrappedTarget(_flushCompletedContinuation);
			}
		}
		else
		{
			base.WrappedTarget?.WriteAsyncLogEvent(logEvent);
		}
	}

	/// <summary>
	/// Schedules a flush operation, that triggers when all pending flush operations are completed (in case of asynchronous targets).
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		AsyncContinuation asyncContinuation2 = _pendingManualFlushList.RegisterCompletionNotification(asyncContinuation);
		if (FlushOnConditionOnly)
		{
			asyncContinuation2(null);
		}
		else
		{
			FlushWrappedTarget(asyncContinuation2);
		}
	}

	private void FlushWrappedTarget(AsyncContinuation asyncContinuation)
	{
		base.WrappedTarget?.Flush(asyncContinuation);
	}

	/// <inheritdoc />
	protected override void CloseTarget()
	{
		_pendingManualFlushList.Clear();
		base.CloseTarget();
	}
}

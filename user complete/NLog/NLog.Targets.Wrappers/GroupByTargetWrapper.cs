using System.Collections.Generic;
using NLog.Common;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Targets.Wrappers;

/// <summary>
/// A target that buffers log events and sends them in batches to the wrapped target.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/GroupByWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/GroupByWrapper-target">Documentation on NLog Wiki</seealso>
[Target("GroupByWrapper", IsWrapper = true)]
public class GroupByTargetWrapper : WrapperTargetBase
{
	private readonly SortHelpers.KeySelector<AsyncLogEventInfo, string> _buildKeyStringDelegate;

	/// <summary>
	/// Identifier to perform group-by
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	public Layout Key { get; set; } = Layout.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.GroupByTargetWrapper" /> class.
	/// </summary>
	public GroupByTargetWrapper()
	{
		_buildKeyStringDelegate = (AsyncLogEventInfo logEvent) => RenderLogEvent(Key, logEvent.LogEvent);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.GroupByTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public GroupByTargetWrapper(Target wrappedTarget)
		: this(string.Empty, wrappedTarget)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.GroupByTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">The name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public GroupByTargetWrapper(string name, Target wrappedTarget)
		: this(name, wrappedTarget, Layout.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.GroupByTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">The name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="key">Group by identifier.</param>
	public GroupByTargetWrapper(string name, Target wrappedTarget, Layout key)
		: this()
	{
		base.Name = ((!string.IsNullOrEmpty(name) || wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		Key = key;
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		if (Key == null || Key == Layout.Empty)
		{
			throw new NLogConfigurationException("GroupByTargetWrapper Key-property must be assigned. Group LogEvents using blank Key not supported.");
		}
		base.InitializeTarget();
	}

	/// <inheritdoc />
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		base.WrappedTarget?.WriteAsyncLogEvent(logEvent);
	}

	/// <inheritdoc />
	protected override void Write(IList<AsyncLogEventInfo> logEvents)
	{
		foreach (KeyValuePair<string, IList<AsyncLogEventInfo>> item in logEvents.BucketSort(_buildKeyStringDelegate))
		{
			base.WrappedTarget?.WriteAsyncLogEvents(item.Value);
		}
	}
}

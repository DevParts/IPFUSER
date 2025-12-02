using System.Collections.Generic;
using NLog.Common;
using NLog.Conditions;
using NLog.Filters;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Filters log entries based on a condition.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/FilteringWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/FilteringWrapper-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>This example causes the messages not contains the string '1' to be ignored.</p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/FilteringWrapper/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/FilteringWrapper/Simple/Example.cs" />
/// </example>
[Target("FilteringWrapper", IsWrapper = true)]
public class FilteringTargetWrapper : WrapperTargetBase
{
	/// <summary>
	/// Gets or sets the condition expression. Log events who meet this condition will be forwarded
	/// to the wrapped target.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Filtering Options" order="10" />
	public ConditionExpression? Condition
	{
		get
		{
			return (Filter as ConditionBasedFilter)?.Condition;
		}
		set
		{
			Filter = CreateFilter(value);
		}
	}

	/// <summary>
	/// Gets or sets the filter. Log events who evaluates to <see cref="F:NLog.Filters.FilterResult.Ignore" /> will be discarded
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see langword="null" /></remarks>
	/// <docgen category="Filtering Options" order="10" />
	public Filter Filter { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.FilteringTargetWrapper" /> class.
	/// </summary>
	public FilteringTargetWrapper()
	{
		Filter = ConditionBasedFilter.Empty;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.FilteringTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="condition">The condition.</param>
	public FilteringTargetWrapper(string name, Target wrappedTarget, ConditionExpression condition)
		: this(wrappedTarget, condition)
	{
		base.Name = name ?? base.Name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.FilteringTargetWrapper" /> class.
	/// </summary>
	/// <param name="wrappedTarget">The wrapped target.</param>
	/// <param name="condition">The condition.</param>
	public FilteringTargetWrapper(Target wrappedTarget, ConditionExpression condition)
	{
		base.Name = ((wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? base.Name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
		Filter = CreateFilter(condition);
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		if (Filter == null || Filter == ConditionBasedFilter.Empty)
		{
			throw new NLogConfigurationException("FilteringTargetWrapper Filter-property must be assigned. Filter LogEvents using blank Filter not supported.");
		}
		base.InitializeTarget();
	}

	/// <summary>
	/// Checks the condition against the passed log event.
	/// If the condition is met, the log event is forwarded to
	/// the wrapped target.
	/// </summary>
	/// <param name="logEvent">Log event.</param>
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		if (ShouldLogEvent(logEvent, Filter))
		{
			base.WrappedTarget?.WriteAsyncLogEvent(logEvent);
		}
	}

	/// <inheritdoc />
	protected override void Write(IList<AsyncLogEventInfo> logEvents)
	{
		IList<AsyncLogEventInfo> list = logEvents.Filter(Filter, (AsyncLogEventInfo logEvent, Filter filter) => ShouldLogEvent(logEvent, filter));
		if (list.Count > 0)
		{
			base.WrappedTarget?.WriteAsyncLogEvents(list);
		}
	}

	private static bool ShouldLogEvent(AsyncLogEventInfo logEvent, Filter filter)
	{
		FilterResult filterResult = filter.GetFilterResult(logEvent.LogEvent);
		if (filterResult != FilterResult.Ignore && filterResult != FilterResult.IgnoreFinal)
		{
			return true;
		}
		logEvent.Continuation(null);
		return false;
	}

	private static ConditionBasedFilter CreateFilter(ConditionExpression? value)
	{
		if (value == null)
		{
			return ConditionBasedFilter.Empty;
		}
		return new ConditionBasedFilter
		{
			Condition = value,
			FilterDefaultAction = FilterResult.Ignore
		};
	}
}

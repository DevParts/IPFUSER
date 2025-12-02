using System.Collections.Generic;
using NLog.Common;
using NLog.Conditions;
using NLog.Config;
using NLog.Internal;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Filters buffered log entries based on a set of conditions that are evaluated on a group of events.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/PostFilteringWrapper-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/PostFilteringWrapper-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// This example works like this. If there are no Warn,Error or Fatal messages in the buffer
/// only Info messages are written to the file, but if there are any warnings or errors,
/// the output includes detailed trace (levels &gt;= Debug).
/// </p>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/PostFilteringWrapper/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/PostFilteringWrapper/Simple/Example.cs" />
/// </example>
[Target("PostFilteringWrapper", IsWrapper = true)]
public class PostFilteringTargetWrapper : WrapperTargetBase
{
	/// <summary>
	/// Gets or sets the default filter to be applied when no specific rule matches.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Filtering Options" order="10" />
	public ConditionExpression? DefaultFilter { get; set; }

	/// <summary>
	/// Gets the collection of filtering rules. The rules are processed top-down
	/// and the first rule that matches determines the filtering condition to
	/// be applied to log events.
	/// </summary>
	/// <docgen category="Filtering Rules" order="10" />
	[ArrayParameter(typeof(FilteringRule), "when")]
	public IList<FilteringRule> Rules { get; } = new List<FilteringRule>();

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.PostFilteringTargetWrapper" /> class.
	/// </summary>
	public PostFilteringTargetWrapper()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.PostFilteringTargetWrapper" /> class.
	/// </summary>
	public PostFilteringTargetWrapper(Target wrappedTarget)
		: this(string.Empty, wrappedTarget)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.Wrappers.PostFilteringTargetWrapper" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="wrappedTarget">The wrapped target.</param>
	public PostFilteringTargetWrapper(string name, Target wrappedTarget)
	{
		base.Name = ((!string.IsNullOrEmpty(name) || wrappedTarget == null || string.IsNullOrEmpty(wrappedTarget.Name)) ? name : (wrappedTarget.Name + "_wrapper"));
		base.WrappedTarget = wrappedTarget;
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		foreach (FilteringRule rule in Rules)
		{
			if (rule.Exists == null)
			{
				throw new NLogConfigurationException("PostFilteringTargetWrapper When-Rules with unassigned Exists-property.");
			}
			if (rule.Filter == null)
			{
				throw new NLogConfigurationException("PostFilteringTargetWrapper When-Rules with unassigned Filter-property.");
			}
		}
	}

	/// <inheritdoc />
	protected override void Write(AsyncLogEventInfo logEvent)
	{
		Write(new AsyncLogEventInfo[1] { logEvent });
	}

	/// <summary>
	/// Evaluates all filtering rules to find the first one that matches.
	/// The matching rule determines the filtering condition to be applied
	/// to all items in a buffer. If no condition matches, default filter
	/// is applied to the array of log events.
	/// </summary>
	/// <param name="logEvents">Array of log events to be post-filtered.</param>
	protected override void Write(IList<AsyncLogEventInfo> logEvents)
	{
		InternalLogger.Trace("{0}: Running on {1} events", this, logEvents.Count);
		ConditionExpression conditionExpression = EvaluateAllRules(logEvents);
		if (conditionExpression == null)
		{
			base.WrappedTarget?.WriteAsyncLogEvents(logEvents);
			return;
		}
		InternalLogger.Trace("{0}: Filter to apply: {1}", this, conditionExpression);
		IList<AsyncLogEventInfo> list = logEvents.Filter(conditionExpression, (AsyncLogEventInfo logEvent, ConditionExpression filter) => ShouldLogEvent(logEvent, filter));
		InternalLogger.Trace("{0}: After filtering: {1} events.", this, list.Count);
		if (list.Count > 0)
		{
			InternalLogger.Trace("{0}: Sending to {1}", this, base.WrappedTarget);
			base.WrappedTarget?.WriteAsyncLogEvents(list);
		}
	}

	private static bool ShouldLogEvent(AsyncLogEventInfo logEvent, ConditionExpression resultFilter)
	{
		object obj = resultFilter.Evaluate(logEvent.LogEvent);
		if (ConditionExpression.BoxedTrue.Equals(obj))
		{
			return true;
		}
		logEvent.Continuation(null);
		return false;
	}

	/// <summary>
	/// Evaluate all the rules to get the filtering condition
	/// </summary>
	/// <param name="logEvents"></param>
	/// <returns></returns>
	private ConditionExpression? EvaluateAllRules(IList<AsyncLogEventInfo> logEvents)
	{
		if (Rules.Count == 0)
		{
			return DefaultFilter;
		}
		for (int i = 0; i < logEvents.Count; i++)
		{
			for (int j = 0; j < Rules.Count; j++)
			{
				FilteringRule filteringRule = Rules[j];
				object obj = filteringRule.Exists?.Evaluate(logEvents[i].LogEvent);
				if (ConditionExpression.BoxedTrue.Equals(obj))
				{
					InternalLogger.Trace("{0}: Rule matched: {1}", this, filteringRule.Exists);
					return filteringRule.Filter;
				}
			}
		}
		return DefaultFilter;
	}
}

using NLog.Conditions;
using NLog.Config;

namespace NLog.Targets.Wrappers;

/// <summary>
/// Filtering rule for <see cref="T:NLog.Targets.Wrappers.PostFilteringTargetWrapper" />.
/// </summary>
[NLogConfigurationItem]
public class FilteringRule
{
	/// <summary>
	/// Gets or sets the condition to be tested.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public ConditionExpression? Exists { get; set; }

	/// <summary>
	/// Gets or sets the resulting filter to be applied when the condition matches.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public ConditionExpression? Filter { get; set; }

	/// <summary>
	/// Initializes a new instance of the FilteringRule class.
	/// </summary>
	public FilteringRule()
	{
	}

	/// <summary>
	/// Initializes a new instance of the FilteringRule class.
	/// </summary>
	/// <param name="whenExistsExpression">Condition to be tested against all events.</param>
	/// <param name="filterToApply">Filter to apply to all log events when the first condition matches any of them.</param>
	public FilteringRule(ConditionExpression whenExistsExpression, ConditionExpression filterToApply)
	{
		Exists = whenExistsExpression;
		Filter = filterToApply;
	}
}

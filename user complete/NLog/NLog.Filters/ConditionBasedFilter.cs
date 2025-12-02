using NLog.Conditions;

namespace NLog.Filters;

/// <summary>
/// Matches when the specified condition is met.
/// </summary>
/// <remarks>
/// Conditions are expressed using a simple language.
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Conditions">Documentation on NLog Wiki</seealso>
[Filter("when")]
public class ConditionBasedFilter : Filter
{
	internal static readonly ConditionBasedFilter Empty = new ConditionBasedFilter();

	/// <summary>
	/// Gets or sets the condition expression.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public ConditionExpression Condition { get; set; } = ConditionLiteralExpression.Null;

	internal FilterResult FilterDefaultAction { get; set; }

	/// <inheritdoc />
	protected override FilterResult Check(LogEventInfo logEvent)
	{
		object obj = Condition?.Evaluate(logEvent);
		if (!ConditionExpression.BoxedTrue.Equals(obj))
		{
			return FilterDefaultAction;
		}
		return base.Action;
	}
}

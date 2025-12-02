using NLog.Conditions;
using NLog.Config;

namespace NLog.Targets;

/// <summary>
/// The row-highlighting condition.
/// </summary>
[NLogConfigurationItem]
public class ConsoleRowHighlightingRule
{
	/// <summary>
	/// Gets the default highlighting rule. Doesn't change the color.
	/// </summary>
	public static ConsoleRowHighlightingRule Default { get; } = new ConsoleRowHighlightingRule(null, ConsoleOutputColor.NoChange, ConsoleOutputColor.NoChange);

	/// <summary>
	/// Gets or sets the condition that must be met in order to set the specified foreground and background color.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public ConditionExpression? Condition { get; set; }

	/// <summary>
	/// Gets or sets the foreground color.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.ConsoleOutputColor.NoChange" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public ConsoleOutputColor ForegroundColor { get; set; } = ConsoleOutputColor.NoChange;

	/// <summary>
	/// Gets or sets the background color.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.ConsoleOutputColor.NoChange" /></remarks>
	/// <docgen category="Highlighting Rules" order="10" />
	public ConsoleOutputColor BackgroundColor { get; set; } = ConsoleOutputColor.NoChange;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ConsoleRowHighlightingRule" /> class.
	/// </summary>
	public ConsoleRowHighlightingRule()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ConsoleRowHighlightingRule" /> class.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <param name="foregroundColor">Color of the foreground.</param>
	/// <param name="backgroundColor">Color of the background.</param>
	public ConsoleRowHighlightingRule(ConditionExpression? condition, ConsoleOutputColor foregroundColor, ConsoleOutputColor backgroundColor)
	{
		Condition = condition;
		ForegroundColor = foregroundColor;
		BackgroundColor = backgroundColor;
	}

	/// <summary>
	/// Checks whether the specified log event matches the condition (if any).
	/// </summary>
	public bool CheckCondition(LogEventInfo logEvent)
	{
		if (Condition != null)
		{
			return true.Equals(Condition.Evaluate(logEvent));
		}
		return true;
	}
}

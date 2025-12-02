using System;

namespace NLog.Filters;

/// <summary>
/// Matches when the calculated layout does NOT contain the specified substring.
/// This filter is deprecated in favor of <c>&lt;when /&gt;</c> which is based on <a href="https://github.com/NLog/NLog/wiki/Conditions">conditions</a>.
/// </summary>
[Filter("whenNotContains")]
public class WhenNotContainsFilter : LayoutBasedFilter
{
	/// <summary>
	/// Gets or sets the substring to be matched.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public string Substring { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a value indicating whether to ignore case when comparing strings.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public bool IgnoreCase { get; set; }

	/// <inheritdoc />
	protected override FilterResult Check(LogEventInfo logEvent)
	{
		StringComparison comparisonType = (IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		string text = base.Layout.Render(logEvent);
		if (!string.IsNullOrEmpty(Substring) && text.IndexOf(Substring, comparisonType) < 0)
		{
			return base.Action;
		}
		return FilterResult.Neutral;
	}
}

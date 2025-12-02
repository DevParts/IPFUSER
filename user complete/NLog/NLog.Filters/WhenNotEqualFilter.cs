using System;

namespace NLog.Filters;

/// <summary>
/// Matches when the calculated layout is NOT equal to the specified substring.
/// This filter is deprecated in favor of <c>&lt;when /&gt;</c> which is based on <a href="https://github.com/NLog/NLog/wiki/Conditions">conditions</a>.
/// </summary>
[Filter("whenNotEqual")]
public class WhenNotEqualFilter : LayoutBasedFilter
{
	/// <summary>
	/// Gets or sets a string to compare the layout to.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public string CompareTo { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets a value indicating whether to ignore case when comparing strings.
	/// </summary>
	/// <docgen category="Filtering Options" order="10" />
	public bool IgnoreCase { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Filters.WhenNotEqualFilter" /> class.
	/// </summary>
	public WhenNotEqualFilter()
	{
	}

	/// <inheritdoc />
	protected override FilterResult Check(LogEventInfo logEvent)
	{
		StringComparison comparisonType = (IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		if (!base.Layout.Render(logEvent).Equals(CompareTo, comparisonType))
		{
			return base.Action;
		}
		return FilterResult.Neutral;
	}
}

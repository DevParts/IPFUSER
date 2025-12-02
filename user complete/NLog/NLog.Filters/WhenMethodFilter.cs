using System;
using NLog.Internal;

namespace NLog.Filters;

/// <summary>
/// Matches the provided filter-method
/// </summary>
public class WhenMethodFilter : Filter
{
	private readonly Func<LogEventInfo, FilterResult> _filterMethod;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Filters.WhenMethodFilter" /> class.
	/// </summary>
	public WhenMethodFilter(Func<LogEventInfo, FilterResult> filterMethod)
	{
		Guard.ThrowIfNull(filterMethod, "filterMethod");
		_filterMethod = filterMethod;
	}

	/// <inheritdoc />
	protected override FilterResult Check(LogEventInfo logEvent)
	{
		FilterResult filterResult = _filterMethod(logEvent);
		if (base.Action == FilterResult.Neutral)
		{
			return filterResult;
		}
		if (filterResult != FilterResult.Neutral)
		{
			return base.Action;
		}
		return FilterResult.Neutral;
	}
}

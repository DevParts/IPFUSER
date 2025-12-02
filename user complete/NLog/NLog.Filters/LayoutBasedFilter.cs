using NLog.Layouts;

namespace NLog.Filters;

/// <summary>
/// A base class for filters that are based on comparing a value to a layout.
/// </summary>
public abstract class LayoutBasedFilter : Filter
{
	/// <summary>
	/// Gets or sets the layout to be used to filter log messages.
	/// </summary>
	/// <value>The layout.</value>
	/// <docgen category="Filtering Options" order="10" />
	public Layout Layout { get; set; } = NLog.Layouts.Layout.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Filters.LayoutBasedFilter" /> class.
	/// </summary>
	protected LayoutBasedFilter()
	{
	}
}

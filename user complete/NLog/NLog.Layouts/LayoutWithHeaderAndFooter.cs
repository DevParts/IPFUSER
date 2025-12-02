using System.Text;
using NLog.Config;

namespace NLog.Layouts;

/// <summary>
/// A specialized layout that supports header and footer.
/// </summary>
[Layout("LayoutWithHeaderAndFooter")]
[ThreadAgnostic]
[AppDomainFixedOutput]
public class LayoutWithHeaderAndFooter : Layout
{
	/// <summary>
	/// Gets or sets the body layout (can be repeated multiple times).
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public Layout Layout { get; set; } = NLog.Layouts.Layout.Empty;

	/// <summary>
	/// Gets or sets the header layout.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public Layout? Header { get; set; }

	/// <summary>
	/// Gets or sets the footer layout.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public Layout? Footer { get; set; }

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return Layout.Render(logEvent);
	}

	/// <inheritdoc />
	protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		Layout.Render(logEvent, target);
	}
}

using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Only outputs the inner layout when exception has been defined for log message.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/OnException-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/OnException-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("onexception")]
[ThreadAgnostic]
public sealed class OnExceptionLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// If <see cref="P:NLog.LogEventInfo.Exception" /> is not found, print this layout.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Condition Options" order="10" />
	public Layout Else { get; set; } = Layout.Empty;

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (logEvent.Exception != null)
		{
			base.Inner?.Render(logEvent, builder);
		}
		else
		{
			Else?.Render(logEvent, builder);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		return text;
	}
}

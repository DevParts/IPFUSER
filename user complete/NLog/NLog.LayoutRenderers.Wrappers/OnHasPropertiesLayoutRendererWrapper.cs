using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Outputs alternative layout when the inner layout produces empty result.
/// </summary>
/// <example>
/// ${onhasproperties:, Properties\: ${all-event-properties}}
/// </example>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/OnHasProperties-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/OnHasProperties-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("onhasproperties")]
[ThreadAgnostic]
public sealed class OnHasPropertiesLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// If <see cref="P:NLog.LogEventInfo.HasProperties" /> is not found, print this layout.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Condition Options" order="10" />
	public Layout Else { get; set; } = Layout.Empty;

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (logEvent.HasProperties)
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

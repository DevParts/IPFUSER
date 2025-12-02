using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Base class for <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />s which wrapping other <see cref="T:NLog.LayoutRenderers.LayoutRenderer" />s.
///
/// This has the <see cref="P:NLog.LayoutRenderers.Wrappers.WrapperLayoutRendererBase.Inner" /> property (which is default) and can be used to wrap.
/// </summary>
/// <example>
/// ${uppercase:${level}} //[DefaultParameter]
/// ${uppercase:Inner=${level}}
/// </example>
public abstract class WrapperLayoutRendererBase : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the wrapped layout.
	///
	/// [DefaultParameter] so Inner: is not required if it's the first
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public Layout Inner { get; set; } = Layout.Empty;

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		Inner?.Initialize(base.LoggingConfiguration);
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		int length = builder.Length;
		try
		{
			RenderInnerAndTransform(logEvent, builder, length);
		}
		catch
		{
			builder.Length = length;
			throw;
		}
	}

	/// <summary>
	/// Appends the rendered output from <see cref="P:NLog.LayoutRenderers.Wrappers.WrapperLayoutRendererBase.Inner" />-layout and transforms the added output (when necessary)
	/// </summary>
	/// <param name="logEvent">Logging event.</param>
	/// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="orgLength">Start position for any necessary transformation of <see cref="T:System.Text.StringBuilder" />.</param>
	protected virtual void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		string text = RenderInner(logEvent);
		builder.Append(Transform(logEvent, text));
	}

	/// <summary>
	/// Transforms the output of another layout.
	/// </summary>
	/// <param name="logEvent">Logging event.</param>
	/// <param name="text">Output to be transform.</param>
	/// <returns>Transformed text.</returns>
	protected virtual string Transform(LogEventInfo logEvent, string text)
	{
		return Transform(text);
	}

	/// <summary>
	/// Transforms the output of another layout.
	/// </summary>
	/// <param name="text">Output to be transform.</param>
	/// <returns>Transformed text.</returns>
	protected abstract string Transform(string text);

	/// <summary>
	/// Renders the inner layout contents.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>Contents of inner layout.</returns>
	protected virtual string RenderInner(LogEventInfo logEvent)
	{
		return Inner?.Render(logEvent) ?? string.Empty;
	}
}

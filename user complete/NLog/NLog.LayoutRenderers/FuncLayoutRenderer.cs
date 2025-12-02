using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// A layout renderer which could have different behavior per instance by using a <see cref="T:System.Func`1" />.
/// </summary>
public class FuncLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	private readonly Func<LogEventInfo, LoggingConfiguration?, object> _renderMethod;

	/// <summary>
	/// Name used in config without ${}. E.g. "test" could be used as "${test}".
	/// </summary>
	public string LayoutRendererName { get; }

	/// <summary>
	/// Method that renders the layout.
	/// </summary>
	[Obsolete("Public API-property was a mistake. Marked obsolete with NLog v6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Func<LogEventInfo, LoggingConfiguration?, object> RenderMethod => _renderMethod;

	/// <summary>
	/// Format string for conversion from object to string.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.FuncLayoutRenderer" /> class.
	/// </summary>
	/// <param name="layoutRendererName">Name without ${}.</param>
	protected FuncLayoutRenderer(string layoutRendererName)
	{
		LayoutRendererName = layoutRendererName;
		_renderMethod = (LogEventInfo evt, LoggingConfiguration? cfg) => string.Empty;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.FuncLayoutRenderer" /> class.
	/// </summary>
	/// <param name="layoutRendererName">Name without ${}.</param>
	/// <param name="renderMethod">Method that renders the layout.</param>
	public FuncLayoutRenderer(string layoutRendererName, Func<LogEventInfo, LoggingConfiguration?, object> renderMethod)
	{
		_renderMethod = Guard.ThrowIfNull(renderMethod, "renderMethod");
		LayoutRendererName = layoutRendererName;
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		object value = RenderValue(logEvent);
		AppendFormattedValue(builder, logEvent, value, Format, Culture);
	}

	string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		if (!"@".Equals(Format))
		{
			return FormatHelper.TryFormatToString(RenderValue(logEvent), Format, GetFormatProvider(logEvent, Culture));
		}
		return null;
	}

	/// <summary>
	/// Render the value for this log event
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	/// <returns>The value.</returns>
	protected virtual object? RenderValue(LogEventInfo logEvent)
	{
		return _renderMethod(logEvent, base.LoggingConfiguration);
	}
}

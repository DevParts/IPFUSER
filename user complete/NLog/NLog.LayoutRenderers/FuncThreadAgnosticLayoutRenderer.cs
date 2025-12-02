using System;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// A layout renderer which could have different behavior per instance by using a <see cref="T:System.Func`1" />.
/// </summary>
[ThreadAgnostic]
internal sealed class FuncThreadAgnosticLayoutRenderer : FuncLayoutRenderer, IRawValue
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.FuncThreadAgnosticLayoutRenderer" /> class.
	/// </summary>
	/// <param name="layoutRendererName">Name without ${}.</param>
	/// <param name="renderMethod">Method that renders the layout.</param>
	public FuncThreadAgnosticLayoutRenderer(string layoutRendererName, Func<LogEventInfo, LoggingConfiguration?, object> renderMethod)
		: base(layoutRendererName, renderMethod)
	{
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		value = RenderValue(logEvent);
		return true;
	}
}

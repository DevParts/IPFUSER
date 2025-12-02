using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Render the non-raw value of an object.
/// </summary>
/// <remarks>For performance and/or full (formatted) control of the output.</remarks>
[LayoutRenderer("norawvalue")]
[AmbientProperty("NoRawValue")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class NoRawValueLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating whether to disable the IRawValue-interface
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool NoRawValue { get; set; } = true;

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner?.Render(logEvent, builder);
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}
}

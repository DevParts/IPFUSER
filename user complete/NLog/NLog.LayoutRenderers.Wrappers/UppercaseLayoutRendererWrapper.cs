using System;
using System.Globalization;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Converts the result of another layout output to upper case.
/// </summary>
/// <example>
/// ${uppercase:${level}} //[DefaultParameter]
/// ${uppercase:Inner=${level}}
/// ${level:uppercase} // [AmbientProperty]
/// </example>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Uppercase-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Uppercase-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("uppercase")]
[AmbientProperty("Uppercase")]
[AmbientProperty("ToUpper")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class UppercaseLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating whether upper case conversion should be applied.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool Uppercase { get; set; } = true;

	/// <summary>
	/// Same as <see cref="P:NLog.LayoutRenderers.Wrappers.UppercaseLayoutRendererWrapper.Uppercase" />-property, so it can be used as ambient property.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <example>
	/// ${level:toupper}
	/// </example>
	/// <docgen category="Layout Options" order="10" />
	public bool ToUpper
	{
		get
		{
			return Uppercase;
		}
		set
		{
			Uppercase = value;
		}
	}

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		base.Inner.Render(logEvent, builder);
		if (Uppercase && builder.Length > orgLength)
		{
			TransformToUpperCase(builder, logEvent, orgLength);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	private void TransformToUpperCase(StringBuilder target, LogEventInfo logEvent, int startPos)
	{
		CultureInfo culture = GetCulture(logEvent, Culture);
		for (int i = startPos; i < target.Length; i++)
		{
			target[i] = char.ToUpper(target[i], culture);
		}
	}
}

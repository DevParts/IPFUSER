using System;
using System.Globalization;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Converts the result of another layout output to lower case.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Lowercase-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Lowercase-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("lowercase")]
[AmbientProperty("Lowercase")]
[AmbientProperty("ToLower")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public sealed class LowercaseLayoutRendererWrapper : WrapperLayoutRendererBase
{
	/// <summary>
	/// Gets or sets a value indicating whether lower case conversion should be applied.
	/// </summary>
	/// <value>A value of <see langword="true" /> if lower case conversion should be applied; otherwise, <see langword="false" />.</value>
	/// <docgen category="Layout Options" order="10" />
	public bool Lowercase { get; set; } = true;

	/// <summary>
	/// Same as <see cref="P:NLog.LayoutRenderers.Wrappers.LowercaseLayoutRendererWrapper.Lowercase" />-property, so it can be used as ambient property.
	/// </summary>
	/// <example>
	/// ${level:tolower}
	/// </example>
	/// <docgen category="Layout Options" order="10" />
	public bool ToLower
	{
		get
		{
			return Lowercase;
		}
		set
		{
			Lowercase = value;
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
		base.Inner?.Render(logEvent, builder);
		if (Lowercase && builder.Length > orgLength)
		{
			TransformToLowerCase(builder, logEvent, orgLength);
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	private void TransformToLowerCase(StringBuilder target, LogEventInfo logEvent, int startPos)
	{
		CultureInfo culture = GetCulture(logEvent, Culture);
		for (int i = startPos; i < target.Length; i++)
		{
			target[i] = char.ToLower(target[i], culture);
		}
	}
}

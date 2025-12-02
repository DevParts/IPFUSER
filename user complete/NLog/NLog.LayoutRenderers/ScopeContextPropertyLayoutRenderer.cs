using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// Renders specified property-item from <see cref="T:NLog.ScopeContext" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ScopeProperty-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ScopeProperty-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("scopeproperty")]
[LayoutRenderer("mdc")]
[LayoutRenderer("mdlc")]
public sealed class ScopeContextPropertyLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	/// <summary>
	/// Gets or sets the name of the item.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Item { get; set; } = string.Empty;

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

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		if (string.IsNullOrEmpty(Item))
		{
			throw new NLogConfigurationException("ScopeProperty-LayoutRenderer Item-property must be assigned. Lookup blank value not supported.");
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		object value = GetValue();
		AppendFormattedValue(builder, logEvent, value, Format, Culture);
	}

	string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		if (!"@".Equals(Format))
		{
			return FormatHelper.TryFormatToString(GetValue(), Format, GetFormatProvider(logEvent, Culture));
		}
		return null;
	}

	private object? GetValue()
	{
		ScopeContext.TryGetProperty(Item, out object value);
		return value;
	}
}

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// Render a Global Diagnostics Context item. See <see cref="T:NLog.GlobalDiagnosticsContext" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Gdc-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Gdc-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("gdc")]
[ThreadAgnostic]
public class GdcLayoutRenderer : LayoutRenderer, IRawValue, IStringValueRenderer
{
	private sealed class CachedLookup
	{
		internal readonly object CachedDictionary;

		internal readonly object? CachedItemValue;

		public CachedLookup(object cachedDictionary, object? cachedItemValue)
		{
			CachedDictionary = cachedDictionary;
			CachedItemValue = cachedItemValue;
		}
	}

	private CachedLookup _cachedLookup = new CachedLookup(string.Empty, null);

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
			throw new NLogConfigurationException("Gdc-LayoutRenderer Item-property must be assigned. Lookup blank value not supported.");
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		object value = GetValue();
		if (value != null || !string.IsNullOrEmpty(Format))
		{
			AppendFormattedValue(builder, logEvent, value, Format, Culture);
		}
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		value = GetValue();
		return true;
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
		CachedLookup cachedLookup = _cachedLookup;
		Dictionary<string, object> readOnlyDict = GlobalDiagnosticsContext.GetReadOnlyDict();
		if (cachedLookup.CachedDictionary == readOnlyDict)
		{
			return cachedLookup.CachedItemValue;
		}
		readOnlyDict.TryGetValue(Item, out var value);
		_cachedLookup = new CachedLookup(readOnlyDict, value);
		return value;
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;
using NLog.MessageTemplates;

namespace NLog.LayoutRenderers;

/// <summary>
/// Log event context data.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/All-Event-Properties-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/All-Event-Properties-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("all-event-properties")]
[ThreadAgnostic]
[ThreadAgnosticImmutable]
public class AllEventPropertiesLayoutRenderer : LayoutRenderer
{
	private string _format;

	private string? _beforeKey;

	private string? _afterKey;

	private string? _afterValue;

	private string _separator = ", ";

	private string _separatorOriginal = ", ";

	private static readonly LayoutRenderer _disableThreadAgnostic = new FuncLayoutRenderer(string.Empty, (LogEventInfo evt, LoggingConfiguration? cfg) => string.Empty);

	/// <summary>
	/// Gets or sets string that will be used to separate key/value pairs.
	/// </summary>
	/// <remarks>Default: <c>, </c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string Separator
	{
		get
		{
			return _separatorOriginal ?? _separator;
		}
		set
		{
			_separatorOriginal = value;
			_separator = SimpleLayout.Evaluate(value, base.LoggingConfiguration, null, false);
		}
	}

	/// <summary>
	/// Gets or sets whether empty property-values should be included in the output.
	/// </summary>
	/// <remarks>Default: <see langword="false" /> . Empty value is either null or empty string</remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeEmptyValues { get; set; }

	/// <summary>
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.ScopeContext" /> properties-dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeScopeProperties { get; set; }

	/// <summary>
	/// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public ISet<string> Exclude { get; set; }

	/// <summary>
	/// Disables <see cref="T:NLog.Config.ThreadAgnosticAttribute" /> to capture ScopeContext-properties from active thread context
	/// </summary>
	public LayoutRenderer? DisableThreadAgnostic
	{
		get
		{
			if (!IncludeScopeProperties)
			{
				return null;
			}
			return _disableThreadAgnostic;
		}
	}

	/// <summary>
	/// Gets or sets how key/value pairs will be formatted.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public string Format
	{
		get
		{
			return _format;
		}
		set
		{
			if (string.IsNullOrEmpty(value) || value.IndexOf("[key]", StringComparison.Ordinal) < 0)
			{
				throw new ArgumentException("Invalid format: [key] placeholder is missing.");
			}
			if (value.IndexOf("[value]", StringComparison.Ordinal) < 0)
			{
				throw new ArgumentException("Invalid format: [value] placeholder is missing.");
			}
			_format = value;
			string[] array = _format.Split(new string[2] { "[key]", "[value]" }, StringSplitOptions.None);
			if (array.Length == 3)
			{
				_beforeKey = array[0];
				_afterKey = array[1];
				_afterValue = array[2];
			}
			else
			{
				_beforeKey = null;
				_afterKey = null;
				_afterValue = null;
			}
		}
	}

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.AllEventPropertiesLayoutRenderer" /> class.
	/// </summary>
	public AllEventPropertiesLayoutRenderer()
	{
		_format = (Format = "[key]=[value]");
		Exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		if (_separatorOriginal != null)
		{
			_separator = SimpleLayout.Evaluate(_separatorOriginal, base.LoggingConfiguration);
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		if (!logEvent.HasProperties && !IncludeScopeProperties)
		{
			return;
		}
		IFormatProvider formatProvider = GetFormatProvider(logEvent, Culture);
		ISet<string> exclude = Exclude;
		bool checkForExclude = exclude != null && exclude.Count > 0;
		bool nonStandardFormat = _beforeKey == null || _afterKey == null || _afterValue == null;
		bool includeSeparator = false;
		if (logEvent.HasProperties)
		{
			using PropertiesDictionary.PropertyDictionaryEnumerator propertyDictionaryEnumerator = logEvent.CreatePropertiesInternal().GetPropertyEnumerator();
			while (propertyDictionaryEnumerator.MoveNext())
			{
				MessageTemplateParameter currentParameter = propertyDictionaryEnumerator.CurrentParameter;
				if (AppendProperty(builder, currentParameter.Name, currentParameter.Value, currentParameter.Format, formatProvider, includeSeparator, checkForExclude, nonStandardFormat))
				{
					includeSeparator = true;
				}
			}
		}
		if (!IncludeScopeProperties)
		{
			return;
		}
		using ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = ScopeContext.GetAllPropertiesEnumerator();
		while (scopeContextPropertyEnumerator.MoveNext())
		{
			KeyValuePair<string, object> current = scopeContextPropertyEnumerator.Current;
			if (AppendProperty(builder, current.Key, current.Value, null, formatProvider, includeSeparator, checkForExclude, nonStandardFormat))
			{
				includeSeparator = true;
			}
		}
	}

	private bool AppendProperty(StringBuilder builder, object propertyKey, object? propertyValue, string? propertyFormat, IFormatProvider? formatProvider, bool includeSeparator, bool checkForExclude, bool nonStandardFormat)
	{
		if (!IncludeEmptyValues && StringHelpers.IsNullOrEmptyString(propertyValue))
		{
			return false;
		}
		if (checkForExclude && Exclude.Contains((propertyKey as string) ?? string.Empty))
		{
			return false;
		}
		if (includeSeparator)
		{
			builder.Append(_separator);
		}
		if (nonStandardFormat)
		{
			string newValue = Convert.ToString(propertyKey, formatProvider);
			string newValue2 = Convert.ToString(propertyValue, formatProvider);
			string value = Format.Replace("[key]", newValue).Replace("[value]", newValue2);
			builder.Append(value);
		}
		else
		{
			builder.Append(_beforeKey);
			builder.AppendFormattedValue(propertyKey, null, formatProvider, base.ValueFormatter);
			builder.Append(_afterKey);
			builder.AppendFormattedValue(propertyValue, propertyFormat, formatProvider, base.ValueFormatter);
			builder.Append(_afterValue);
		}
		return true;
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.MessageTemplates;
using NLog.Targets;

namespace NLog.Layouts;

/// <summary>
/// A specialized layout that renders JSON-formatted events.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/JsonLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/JsonLayout">Documentation on NLog Wiki</seealso>
[Layout("JsonLayout")]
[ThreadAgnostic]
public class JsonLayout : Layout
{
	private sealed class LimitRecursionJsonConvert : IJsonConverter
	{
		private readonly IJsonConverter _converter;

		private readonly DefaultJsonSerializer? _serializer;

		private readonly JsonSerializeOptions _serializerOptions;

		public LimitRecursionJsonConvert(int maxRecursionLimit, bool suppressSpaces, IJsonConverter converter)
		{
			_converter = converter;
			_serializer = converter as DefaultJsonSerializer;
			_serializerOptions = new JsonSerializeOptions
			{
				MaxRecursionLimit = Math.Max(0, maxRecursionLimit),
				SuppressSpaces = suppressSpaces,
				SanitizeDictionaryKeys = true
			};
		}

		public bool SerializeObject(object? value, StringBuilder builder)
		{
			if (_serializer != null)
			{
				return _serializer.SerializeObject(value, builder, _serializerOptions);
			}
			return _converter.SerializeObject(value, builder);
		}

		public bool SerializeObjectNoLimit(object? value, StringBuilder builder)
		{
			return _converter.SerializeObject(value, builder);
		}
	}

	private const int SpacesPerIndent = 2;

	private Layout[]? _precalculateLayouts;

	private LimitRecursionJsonConvert? _jsonConverter;

	private IValueFormatter? _valueFormatter;

	private readonly List<JsonAttribute> _attributes = new List<JsonAttribute>();

	private bool _suppressSpaces = true;

	private bool? _renderEmptyObject;

	private bool _indentJson;

	private bool? _includeScopeProperties;

	private bool? _includeMdc;

	private bool? _includeMdlc;

	private string _beginJsonMessage = "{\"";

	private string _completeJsonMessage = "}";

	private string _beginJsonPropertyName = ",\"";

	private string _completeJsonPropertyName = "\":";

	private LimitRecursionJsonConvert JsonConverter => _jsonConverter ?? (_jsonConverter = new LimitRecursionJsonConvert(MaxRecursionLimit, SuppressSpaces, ResolveService<IJsonConverter>()));

	private IValueFormatter ValueFormatter => _valueFormatter ?? (_valueFormatter = ResolveService<IValueFormatter>());

	/// <summary>
	/// Gets the array of attributes' configurations.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(JsonAttribute), "attribute")]
	public IList<JsonAttribute> Attributes => _attributes;

	/// <summary>
	/// Gets or sets the option to suppress the extra spaces in the output json.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool SuppressSpaces
	{
		get
		{
			return _suppressSpaces;
		}
		set
		{
			if (_suppressSpaces != value)
			{
				_suppressSpaces = value;
				RefreshJsonDelimiters();
			}
		}
	}

	/// <summary>
	/// Gets or sets the option to render the empty object value {}
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool RenderEmptyObject
	{
		get
		{
			return _renderEmptyObject ?? true;
		}
		set
		{
			_renderEmptyObject = value;
		}
	}

	/// <summary>
	/// Auto indent and create new lines
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool IndentJson
	{
		get
		{
			return _indentJson;
		}
		set
		{
			if (_indentJson != value)
			{
				_indentJson = value;
				if (_indentJson)
				{
					_suppressSpaces = false;
				}
				RefreshJsonDelimiters();
			}
		}
	}

	/// <summary>
	/// Gets or sets the option to include all properties from the log event (as JSON)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeEventProperties { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to include contents of the <see cref="T:NLog.GlobalDiagnosticsContext" /> dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeGdc { get; set; }

	/// <summary>
	/// Gets or sets whether to include the contents of the <see cref="T:NLog.ScopeContext" /> dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeScopeProperties
	{
		get
		{
			bool? includeScopeProperties = _includeScopeProperties;
			if (!includeScopeProperties.HasValue)
			{
				if (_includeMdlc != true)
				{
					return _includeMdc == true;
				}
				return true;
			}
			return includeScopeProperties == true;
		}
		set
		{
			_includeScopeProperties = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Layouts.JsonLayout.IncludeEventProperties" /> with NLog v5.
	///
	/// Gets or sets the option to include all properties from the log event (as JSON)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeEventProperties. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeAllProperties
	{
		get
		{
			return IncludeEventProperties;
		}
		set
		{
			IncludeEventProperties = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Layouts.JsonLayout.IncludeScopeProperties" /> with NLog v5.
	///
	/// Gets or sets a value indicating whether to include contents of the <see cref="T:NLog.MappedDiagnosticsContext" /> dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeMdc
	{
		get
		{
			return _includeMdc == true;
		}
		set
		{
			_includeMdc = value;
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Layouts.JsonLayout.IncludeScopeProperties" /> with NLog v5.
	///
	/// Gets or sets a value indicating whether to include contents of the <see cref="T:NLog.MappedDiagnosticsLogicalContext" /> dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[Obsolete("Replaced by IncludeScopeProperties. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IncludeMdlc
	{
		get
		{
			return _includeMdlc == true;
		}
		set
		{
			_includeMdlc = value;
		}
	}

	/// <summary>
	/// Gets or sets the option to exclude null/empty properties from the log event (as JSON)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool ExcludeEmptyProperties { get; set; }

	/// <summary>
	/// List of property names to exclude when <see cref="P:NLog.Layouts.JsonLayout.IncludeAllProperties" /> is true
	/// </summary>
	/// <docgen category="Layout Options" order="100" />
	public ISet<string> ExcludeProperties { get; set; }

	/// <summary>
	/// How far should the JSON serializer follow object references before backing off
	/// </summary>
	/// <remarks>Default: <see langword="1" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public int MaxRecursionLimit { get; set; } = 1;

	/// <summary>
	/// Should forward slashes be escaped? If <see langword="true" />, / will be converted to \/
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	[Obsolete("Marked obsolete with NLog 5.5. Should never escape forward slash")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool EscapeForwardSlash { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.JsonLayout" /> class.
	/// </summary>
	public JsonLayout()
	{
		ExcludeProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		base.InitializeLayout();
		if (IncludeScopeProperties)
		{
			base.ThreadAgnostic = false;
		}
		if (IncludeEventProperties)
		{
			base.ThreadAgnosticImmutable = true;
		}
		_precalculateLayouts = ((IncludeScopeProperties || IncludeEventProperties) ? null : ResolveLayoutPrecalculation(Attributes.Select((JsonAttribute atr) => atr.Layout)));
		foreach (JsonAttribute attribute in _attributes)
		{
			if (string.IsNullOrEmpty(attribute.Name))
			{
				throw new NLogConfigurationException("JsonLayout: Contains invalid JsonAttribute with unassigned Name-property");
			}
			if (!attribute.Encode && attribute.Layout is JsonLayout jsonLayout)
			{
				if (!attribute.IncludeEmptyValue && !jsonLayout._renderEmptyObject.HasValue)
				{
					jsonLayout.RenderEmptyObject = false;
				}
				if (!SuppressSpaces || IndentJson)
				{
					jsonLayout.SuppressSpaces = false;
				}
			}
		}
	}

	/// <inheritdoc />
	protected override void CloseLayout()
	{
		_jsonConverter = null;
		_valueFormatter = null;
		_precalculateLayouts = null;
		base.CloseLayout();
	}

	internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		PrecalculateBuilderInternal(logEvent, target, _precalculateLayouts);
	}

	/// <inheritdoc />
	protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		int length = target.Length;
		RenderJsonFormattedMessage(logEvent, target);
		if (target.Length == length && RenderEmptyObject)
		{
			target.Append(SuppressSpaces ? "{}" : "{ }");
		}
	}

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return RenderAllocateBuilder(logEvent);
	}

	private void RenderJsonFormattedMessage(LogEventInfo logEvent, StringBuilder sb)
	{
		int length = sb.Length;
		foreach (JsonAttribute attribute in _attributes)
		{
			int length2 = sb.Length;
			if (!RenderAppendJsonPropertyValue(attribute, logEvent, sb, length2 == length))
			{
				sb.Length = length2;
			}
		}
		if (IncludeGdc)
		{
			ICollection<string> names = GlobalDiagnosticsContext.GetNames();
			if (names.Count > 0)
			{
				foreach (string item in names)
				{
					if (!string.IsNullOrEmpty(item))
					{
						object propertyValue = GlobalDiagnosticsContext.GetObject(item);
						AppendJsonPropertyValue(item, propertyValue, sb, sb.Length == length);
					}
				}
			}
		}
		if (IncludeScopeProperties)
		{
			bool flag = ExcludeProperties.Count > 0;
			using ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = ScopeContext.GetAllPropertiesEnumerator();
			while (scopeContextPropertyEnumerator.MoveNext())
			{
				KeyValuePair<string, object> current3 = scopeContextPropertyEnumerator.Current;
				if (!string.IsNullOrEmpty(current3.Key) && (!flag || !ExcludeProperties.Contains(current3.Key)))
				{
					AppendJsonPropertyValue(current3.Key, current3.Value, sb, sb.Length == length);
				}
			}
		}
		if (IncludeEventProperties && logEvent.HasProperties)
		{
			bool flag2 = ExcludeProperties.Count > 0;
			using PropertiesDictionary.PropertyDictionaryEnumerator propertyDictionaryEnumerator = logEvent.CreatePropertiesInternal().GetPropertyEnumerator();
			while (propertyDictionaryEnumerator.MoveNext())
			{
				MessageTemplateParameter currentParameter = propertyDictionaryEnumerator.CurrentParameter;
				if (!string.IsNullOrEmpty(currentParameter.Name) && (!flag2 || !ExcludeProperties.Contains(currentParameter.Name)))
				{
					AppendJsonPropertyValue(currentParameter.Name, currentParameter.Value, currentParameter.Format, logEvent.FormatProvider, currentParameter.CaptureType, sb, sb.Length == length);
				}
			}
		}
		if (sb.Length > length)
		{
			sb.Append(_completeJsonMessage);
		}
	}

	private void BeginJsonProperty(StringBuilder sb, string propName, bool beginJsonMessage, bool ensureStringEscape)
	{
		sb.Append(beginJsonMessage ? _beginJsonMessage : _beginJsonPropertyName);
		if (ensureStringEscape)
		{
			DefaultJsonSerializer.AppendStringEscape(sb, propName, escapeUnicode: false);
		}
		else
		{
			sb.Append(propName);
		}
		sb.Append(_completeJsonPropertyName);
	}

	private void RefreshJsonDelimiters()
	{
		if (IndentJson)
		{
			_beginJsonMessage = new StringBuilder().Append('{').AppendLine().Append(' ', 2)
				.Append('"')
				.ToString();
		}
		else
		{
			_beginJsonMessage = (SuppressSpaces ? "{\"" : "{ \"");
		}
		if (IndentJson)
		{
			_completeJsonMessage = new StringBuilder().AppendLine().Append('}').ToString()
				.ToString();
		}
		else
		{
			_completeJsonMessage = (SuppressSpaces ? "}" : " }");
		}
		if (IndentJson)
		{
			_beginJsonPropertyName = new StringBuilder().Append(',').AppendLine().Append(' ', 2)
				.Append('"')
				.ToString();
		}
		else
		{
			_beginJsonPropertyName = (SuppressSpaces ? ",\"" : ", \"");
		}
		_completeJsonPropertyName = (SuppressSpaces ? "\":" : "\": ");
	}

	private void AppendJsonPropertyValue(string propName, object? propertyValue, StringBuilder sb, bool beginJsonMessage)
	{
		if (!ExcludeEmptyProperties || (propertyValue != null && propertyValue != string.Empty))
		{
			int length = sb.Length;
			BeginJsonProperty(sb, propName, beginJsonMessage, ensureStringEscape: true);
			if (!JsonConverter.SerializeObject(propertyValue, sb))
			{
				sb.Length = length;
			}
			else if (ExcludeEmptyProperties && sb[sb.Length - 1] == '"' && sb[sb.Length - 2] == '"' && sb[sb.Length - 3] != '\\')
			{
				sb.Length = length;
			}
		}
	}

	private void AppendJsonPropertyValue(string propName, object? propertyValue, string? format, IFormatProvider? formatProvider, CaptureType captureType, StringBuilder sb, bool beginJsonMessage)
	{
		if (captureType == CaptureType.Serialize && MaxRecursionLimit <= 1)
		{
			if (!ExcludeEmptyProperties || propertyValue != null)
			{
				int length = sb.Length;
				BeginJsonProperty(sb, propName, beginJsonMessage, ensureStringEscape: true);
				if (!JsonConverter.SerializeObjectNoLimit(propertyValue, sb))
				{
					sb.Length = length;
				}
			}
		}
		else if (captureType == CaptureType.Stringify)
		{
			if (!ExcludeEmptyProperties || !StringHelpers.IsNullOrEmptyString(propertyValue))
			{
				BeginJsonProperty(sb, propName, beginJsonMessage, ensureStringEscape: true);
				int length2 = sb.Length;
				sb.Append('"');
				ValueFormatter.FormatValue(propertyValue, format, captureType, formatProvider, sb);
				sb.Append('"');
				PerformJsonEscapeIfNeeded(sb, length2);
			}
		}
		else
		{
			AppendJsonPropertyValue(propName, propertyValue, sb, beginJsonMessage);
		}
	}

	private static void PerformJsonEscapeIfNeeded(StringBuilder sb, int valueStart)
	{
		int length = sb.Length;
		if (length - valueStart <= 2)
		{
			return;
		}
		for (int i = valueStart + 1; i < length - 1; i++)
		{
			if (DefaultJsonSerializer.RequiresJsonEscape(sb[i], escapeUnicode: false))
			{
				string text = sb.ToString(valueStart + 1, sb.Length - valueStart - 2);
				sb.Length = valueStart;
				sb.Append('"');
				DefaultJsonSerializer.AppendStringEscape(sb, text, escapeUnicode: false);
				sb.Append('"');
				break;
			}
		}
	}

	private bool RenderAppendJsonPropertyValue(JsonAttribute attrib, LogEventInfo logEvent, StringBuilder sb, bool beginJsonMessage)
	{
		BeginJsonProperty(sb, attrib.Name, beginJsonMessage, ensureStringEscape: false);
		if (!attrib.RenderAppendJsonValue(logEvent, JsonConverter, sb))
		{
			return false;
		}
		return true;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (_attributes.Count > 0)
		{
			return ToStringWithNestedItems(_attributes, (JsonAttribute a) => a.Name + "=" + a.Layout);
		}
		if (IncludeEventProperties)
		{
			return GetType().Name + ": IncludeEventProperties=true";
		}
		return GetType().Name;
	}
}

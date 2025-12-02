using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.LayoutRenderers.Wrappers;
using NLog.MessageTemplates;
using NLog.Targets;

namespace NLog.Layouts;

/// <summary>
/// A specialized layout that renders XML-formatted events.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/XmlLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/XmlLayout">Documentation on NLog Wiki</seealso>
public abstract class XmlElementBase : Layout
{
	private Layout[]? _precalculateLayouts;

	private const string DefaultPropertyName = "property";

	private const string DefaultPropertyKeyAttribute = "key";

	private const string DefaultCollectionItemName = "item";

	private string _elementNameInternal = string.Empty;

	/// <summary>
	/// Value inside the XML element
	/// </summary>
	/// <remarks>Upgrade to private protected when using C# 7.2 </remarks>
	internal readonly XmlEncodeLayoutRendererWrapper LayoutWrapper = new XmlEncodeLayoutRendererWrapper();

	private readonly List<XmlElement> _elements = new List<XmlElement>();

	private readonly List<XmlAttribute> _attributes = new List<XmlAttribute>();

	private bool? _includeScopeProperties;

	private bool? _includeMdc;

	private bool? _includeMdlc;

	private string _propertiesElementName = "property";

	private bool _propertiesElementNameHasFormat;

	private ObjectReflectionCache? _objectReflectionCache;

	private static readonly IEqualityComparer<object> _referenceEqualsComparer = SingleItemOptimizedHashSet<object>.ReferenceEqualityComparer.Default;

	private const int MaxXmlLength = 524288;

	/// <summary>
	/// Name of the XML element
	/// </summary>
	internal string ElementNameInternal
	{
		get
		{
			return _elementNameInternal;
		}
		set
		{
			_elementNameInternal = XmlHelper.XmlConvertToElementName(value?.Trim() ?? string.Empty);
		}
	}

	/// <summary>
	/// Auto indent and create new lines
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool IndentXml { get; set; }

	/// <summary>
	/// Gets the array of xml 'elements' configurations.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(XmlElement), "element")]
	public IList<XmlElement> Elements => _elements;

	/// <summary>
	/// Gets the array of 'attributes' configurations for the element
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(XmlAttribute), "attribute")]
	public IList<XmlAttribute> Attributes => _attributes;

	/// <summary>
	/// Gets the collection of context properties that should be included with the other properties.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	[ArrayParameter(typeof(TargetPropertyWithContext), "contextproperty")]
	public List<TargetPropertyWithContext>? ContextProperties { get; set; }

	/// <summary>
	/// Gets or sets whether empty XML-element should be included in the output.
	/// </summary>
	/// <remarks>Default: <see langword="false" /> . Empty value is either null or empty string</remarks>
	/// <docgen category="Layout Output" order="10" />
	public bool IncludeEmptyValue { get; set; }

	/// <summary>
	/// Gets or sets the option to include all properties from the log event (as XML)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Output" order="10" />
	public bool IncludeEventProperties { get; set; }

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
	/// Obsolete and replaced by <see cref="P:NLog.Layouts.XmlElementBase.IncludeScopeProperties" /> with NLog v5.
	///
	/// Gets or sets a value indicating whether to include contents of the <see cref="T:NLog.MappedDiagnosticsContext" /> dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
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
	/// Obsolete and replaced by <see cref="P:NLog.Layouts.XmlElementBase.IncludeScopeProperties" /> with NLog v5.
	///
	/// Gets or sets a value indicating whether to include contents of the <see cref="T:NLog.MappedDiagnosticsLogicalContext" /> dictionary.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
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
	/// Obsolete and replaced by <see cref="P:NLog.Layouts.XmlElementBase.IncludeEventProperties" /> with NLog v5.
	///
	/// Gets or sets the option to include all properties from the log event (as XML)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="100" />
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
	/// List of property names to exclude when <see cref="P:NLog.Layouts.XmlElementBase.IncludeEventProperties" /> is <see langword="true" />
	/// </summary>
	/// <docgen category="Layout Options" order="50" />
	public ISet<string> ExcludeProperties { get; set; }

	/// <summary>
	/// XML element name to use when rendering properties
	/// </summary>
	/// <remarks>
	/// Support string-format where {0} means property-key-name
	///
	/// Skips closing element tag when having configured <see cref="P:NLog.Layouts.XmlElementBase.PropertiesElementValueAttribute" />
	/// </remarks>
	/// <docgen category="Layout Options" order="50" />
	public string PropertiesElementName
	{
		get
		{
			return _propertiesElementName;
		}
		set
		{
			_propertiesElementName = value;
			_propertiesElementNameHasFormat = value != null && value.IndexOf('{') >= 0;
			if (!_propertiesElementNameHasFormat)
			{
				_propertiesElementName = XmlHelper.XmlConvertToElementName(value?.Trim() ?? string.Empty);
			}
		}
	}

	/// <summary>
	/// XML attribute name to use when rendering property-key
	///
	/// When null (or empty) then key-attribute is not included
	/// </summary>
	/// <remarks>Default: <c>key</c> . Newlines in attribute-value will be replaced with <c>
	/// </c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string PropertiesElementKeyAttribute { get; set; } = "key";

	/// <summary>
	/// XML attribute name to use when rendering property-value
	///
	/// When null (or empty) then value-attribute is not included and
	/// value is formatted as XML-element-value.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /> . Newlines in attribute-value will be replaced with <c>
	/// </c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string PropertiesElementValueAttribute { get; set; } = string.Empty;

	/// <summary>
	/// XML element name to use for rendering IList-collections items
	/// </summary>
	/// <remarks>Default: <c>item</c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string PropertiesCollectionItemName { get; set; } = "item";

	/// <summary>
	/// How far should the XML serializer follow object references before backing off
	/// </summary>
	/// <remarks>Default: <see langword="1" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public int MaxRecursionLimit { get; set; } = 1;

	private ObjectReflectionCache ObjectReflectionCache => _objectReflectionCache ?? (_objectReflectionCache = new ObjectReflectionCache(base.LoggingConfiguration.GetServiceProvider()));

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlElementBase" /> class.
	/// </summary>
	/// <param name="elementName">The name of the top XML node</param>
	/// <param name="elementValue">The value of the top XML node</param>
	protected XmlElementBase(string elementName, Layout elementValue)
	{
		ElementNameInternal = elementName;
		LayoutWrapper.Inner = elementValue;
		ExcludeProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		base.InitializeLayout();
		if (string.IsNullOrEmpty(ElementNameInternal))
		{
			throw new NLogConfigurationException("XmlLayout Name-property must be assigned. Name is required for valid XML element.");
		}
		if (IncludeScopeProperties)
		{
			base.ThreadAgnostic = false;
		}
		if (IncludeEventProperties)
		{
			base.ThreadAgnosticImmutable = true;
		}
		if (_attributes.Count > 0)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (XmlAttribute attribute in _attributes)
			{
				if (string.IsNullOrEmpty(attribute.Name))
				{
					throw new NLogConfigurationException("XmlElement(Name=" + ElementNameInternal + "): Contains invalid XmlAttribute with unassigned Name-property");
				}
				if (hashSet.Contains(attribute.Name))
				{
					InternalLogger.Warn("XmlElement(ElementName={0}): Contains duplicate XmlAttribute(Name={1}) (Invalid xml)", ElementNameInternal, attribute.Name);
				}
				else
				{
					hashSet.Add(attribute.Name);
				}
			}
		}
		if (ContextProperties != null)
		{
			foreach (TargetPropertyWithContext contextProperty in ContextProperties)
			{
				if (string.IsNullOrEmpty(contextProperty.Name))
				{
					throw new NLogConfigurationException("XmlElement(Name=" + ElementNameInternal + "): Contains invalid ContextProperty with unassigned Name-property");
				}
			}
		}
		Layout[] second = ((LayoutWrapper.Inner == null) ? ArrayHelper.Empty<Layout>() : new Layout[1] { LayoutWrapper.Inner });
		_precalculateLayouts = ((IncludeEventProperties || IncludeScopeProperties) ? null : ResolveLayoutPrecalculation(_attributes.Select((XmlAttribute atr) => atr.Layout).Concat<Layout>(from elm in _elements
			where elm.Layout != null
			select elm.Layout).Concat<Layout>(ContextProperties?.Select((TargetPropertyWithContext ctx) => ctx.Layout) ?? Enumerable.Empty<Layout>())
			.Concat(second)));
	}

	/// <inheritdoc />
	protected override void CloseLayout()
	{
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
		RenderXmlFormattedMessage(logEvent, target);
		if (target.Length == length && IncludeEmptyValue && !string.IsNullOrEmpty(ElementNameInternal))
		{
			RenderSelfClosingElement(target, ElementNameInternal);
		}
	}

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return RenderAllocateBuilder(logEvent);
	}

	private void RenderXmlFormattedMessage(LogEventInfo logEvent, StringBuilder sb)
	{
		int length = sb.Length;
		if (!string.IsNullOrEmpty(ElementNameInternal))
		{
			foreach (XmlAttribute attribute in _attributes)
			{
				int length2 = sb.Length;
				if (!RenderAppendXmlAttributeValue(attribute, logEvent, sb, sb.Length == length))
				{
					sb.Length = length2;
				}
			}
			if (sb.Length != length)
			{
				if (!HasNestedXmlElements(logEvent))
				{
					sb.Append("/>");
					return;
				}
				sb.Append('>');
			}
			if (LayoutWrapper.Inner != null)
			{
				int length3 = sb.Length;
				if (sb.Length == length)
				{
					RenderStartElement(sb, ElementNameInternal);
				}
				int length4 = sb.Length;
				LayoutWrapper.RenderAppendBuilder(logEvent, sb);
				if (length4 == sb.Length && !IncludeEmptyValue)
				{
					sb.Length = length3;
				}
			}
			if (IndentXml && sb.Length != length)
			{
				sb.AppendLine();
			}
		}
		foreach (XmlElement element in _elements)
		{
			int length5 = sb.Length;
			if (!RenderAppendXmlElementValue(element, logEvent, sb, sb.Length == length))
			{
				sb.Length = length5;
			}
		}
		AppendLogEventXmlProperties(logEvent, sb, length);
		if (sb.Length > length && !string.IsNullOrEmpty(ElementNameInternal))
		{
			EndXmlDocument(sb, ElementNameInternal);
		}
	}

	private bool HasNestedXmlElements(LogEventInfo logEvent)
	{
		Layout inner = LayoutWrapper.Inner;
		if (inner != null && inner != Layout.Empty)
		{
			return true;
		}
		if (_elements.Count > 0)
		{
			return true;
		}
		List<TargetPropertyWithContext>? contextProperties = ContextProperties;
		if (contextProperties != null && contextProperties.Count > 0)
		{
			return true;
		}
		if (IncludeScopeProperties)
		{
			return true;
		}
		if (IncludeEventProperties && logEvent.HasProperties)
		{
			return true;
		}
		return false;
	}

	private void AppendLogEventXmlProperties(LogEventInfo logEventInfo, StringBuilder sb, int orgLength)
	{
		if (ContextProperties != null)
		{
			foreach (TargetPropertyWithContext contextProperty in ContextProperties)
			{
				object obj = contextProperty.RenderValue(logEventInfo);
				if (contextProperty.IncludeEmptyValue || !StringHelpers.IsNullOrEmptyString(obj))
				{
					AppendXmlPropertyValue(contextProperty.Name, obj, sb, orgLength);
				}
			}
		}
		if (IncludeScopeProperties)
		{
			bool flag = ExcludeProperties.Count > 0;
			using ScopeContextPropertyEnumerator<object> scopeContextPropertyEnumerator = ScopeContext.GetAllPropertiesEnumerator();
			while (scopeContextPropertyEnumerator.MoveNext())
			{
				KeyValuePair<string, object> current2 = scopeContextPropertyEnumerator.Current;
				if (!string.IsNullOrEmpty(current2.Key) && (!flag || !ExcludeProperties.Contains(current2.Key)))
				{
					AppendXmlPropertyValue(current2.Key, current2.Value, sb, orgLength);
				}
			}
		}
		if (IncludeEventProperties)
		{
			AppendLogEventProperties(logEventInfo, sb, orgLength);
		}
	}

	private void AppendLogEventProperties(LogEventInfo logEventInfo, StringBuilder sb, int orgLength)
	{
		if (!logEventInfo.HasProperties)
		{
			return;
		}
		bool flag = ExcludeProperties.Count > 0;
		using PropertiesDictionary.PropertyDictionaryEnumerator propertyDictionaryEnumerator = logEventInfo.CreatePropertiesInternal().GetPropertyEnumerator();
		while (propertyDictionaryEnumerator.MoveNext())
		{
			MessageTemplateParameter currentParameter = propertyDictionaryEnumerator.CurrentParameter;
			if (!string.IsNullOrEmpty(currentParameter.Name) && (!flag || !ExcludeProperties.Contains(currentParameter.Name)))
			{
				object obj = currentParameter.Value;
				if (!string.IsNullOrEmpty(currentParameter.Format) && obj is IFormattable formattable)
				{
					obj = formattable.ToString(currentParameter.Format, CultureInfo.InvariantCulture);
				}
				else if (currentParameter.CaptureType == CaptureType.Stringify)
				{
					obj = Convert.ToString(currentParameter.Value ?? string.Empty, CultureInfo.InvariantCulture);
				}
				AppendXmlPropertyObjectValue(currentParameter.Name, obj, sb, orgLength, default(SingleItemOptimizedHashSet<object>), 0);
			}
		}
	}

	private bool AppendXmlPropertyObjectValue(string propName, object? propertyValue, StringBuilder sb, int orgLength, SingleItemOptimizedHashSet<object> objectsInPath, int depth, bool ignorePropertiesElementName = false)
	{
		if (propertyValue is IConvertible convertible)
		{
			TypeCode typeCode = convertible.GetTypeCode();
			if (typeCode != TypeCode.Object)
			{
				string xmlValueString = XmlHelper.XmlConvertToString(convertible, typeCode, safeConversion: true);
				AppendXmlPropertyStringValue(propName, xmlValueString, sb, orgLength, ignoreValue: false, ignorePropertiesElementName);
				return true;
			}
		}
		else if (propertyValue == null)
		{
			string xmlValueString2 = XmlHelper.XmlConvertToString(null, TypeCode.Empty, safeConversion: true);
			AppendXmlPropertyStringValue(propName, xmlValueString2, sb, orgLength, ignoreValue: false, ignorePropertiesElementName);
			return true;
		}
		if (sb.Length > 524288)
		{
			return false;
		}
		int num = ((objectsInPath.Count == 0) ? depth : (depth + 1));
		if (num > MaxRecursionLimit)
		{
			return false;
		}
		if (objectsInPath.Contains(propertyValue))
		{
			return false;
		}
		if (MaxRecursionLimit == 0 || (num == MaxRecursionLimit && !(propertyValue is IEnumerable)))
		{
			string xmlValueString3 = XmlHelper.XmlConvertToStringSafe(propertyValue);
			AppendXmlPropertyStringValue(propName, xmlValueString3, sb, orgLength, ignoreValue: false, ignorePropertiesElementName);
			return true;
		}
		if (propertyValue is IDictionary dictionary)
		{
			using (StartCollectionScope(ref objectsInPath, dictionary))
			{
				AppendXmlDictionaryObject(propName, dictionary, sb, orgLength, objectsInPath, num, ignorePropertiesElementName);
			}
		}
		else if (propertyValue is IEnumerable enumerable)
		{
			if (ObjectReflectionCache.TryLookupExpandoObject(propertyValue, out var objectPropertyList))
			{
				using (new SingleItemOptimizedHashSet<object>.SingleItemScopedInsert(propertyValue, ref objectsInPath, forceHashSet: false, _referenceEqualsComparer))
				{
					AppendXmlObjectPropertyValues(propName, ref objectPropertyList, sb, orgLength, ref objectsInPath, num, ignorePropertiesElementName);
				}
			}
			else
			{
				using (StartCollectionScope(ref objectsInPath, enumerable))
				{
					AppendXmlCollectionObject(propName, enumerable, sb, orgLength, objectsInPath, num, ignorePropertiesElementName);
				}
			}
		}
		else
		{
			using (new SingleItemOptimizedHashSet<object>.SingleItemScopedInsert(propertyValue, ref objectsInPath, forceHashSet: false, _referenceEqualsComparer))
			{
				ObjectReflectionCache.ObjectPropertyList propertyValues = ObjectReflectionCache.LookupObjectProperties(propertyValue);
				AppendXmlObjectPropertyValues(propName, ref propertyValues, sb, orgLength, ref objectsInPath, num, ignorePropertiesElementName);
			}
		}
		return true;
	}

	private static SingleItemOptimizedHashSet<object>.SingleItemScopedInsert StartCollectionScope(ref SingleItemOptimizedHashSet<object> objectsInPath, object value)
	{
		return new SingleItemOptimizedHashSet<object>.SingleItemScopedInsert(value, ref objectsInPath, forceHashSet: true, _referenceEqualsComparer);
	}

	private void AppendXmlCollectionObject(string propName, IEnumerable collection, StringBuilder sb, int orgLength, SingleItemOptimizedHashSet<object> objectsInPath, int depth, bool ignorePropertiesElementName)
	{
		string text = AppendXmlPropertyValue(propName, string.Empty, sb, orgLength, ignoreValue: true);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		foreach (object item in collection)
		{
			int length = sb.Length;
			if (length > 524288)
			{
				break;
			}
			if (!AppendXmlPropertyObjectValue(PropertiesCollectionItemName, item, sb, orgLength, objectsInPath, depth, ignorePropertiesElementName: true))
			{
				sb.Length = length;
			}
		}
		AppendClosingPropertyTag(text, sb, ignorePropertiesElementName);
	}

	private void AppendXmlDictionaryObject(string propName, IDictionary dictionary, StringBuilder sb, int orgLength, SingleItemOptimizedHashSet<object> objectsInPath, int depth, bool ignorePropertiesElementName)
	{
		string text = AppendXmlPropertyValue(propName, string.Empty, sb, orgLength, ignoreValue: true, ignorePropertiesElementName);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		foreach (DictionaryEntry item in new DictionaryEntryEnumerable(dictionary))
		{
			int length = sb.Length;
			if (length > 524288)
			{
				break;
			}
			string text2 = item.Key?.ToString() ?? string.Empty;
			if (!string.IsNullOrEmpty(text2) && !AppendXmlPropertyObjectValue(text2, item.Value, sb, orgLength, objectsInPath, depth))
			{
				sb.Length = length;
			}
		}
		AppendClosingPropertyTag(text, sb, ignorePropertiesElementName);
	}

	private void AppendXmlObjectPropertyValues(string propName, ref ObjectReflectionCache.ObjectPropertyList propertyValues, StringBuilder sb, int orgLength, ref SingleItemOptimizedHashSet<object> objectsInPath, int depth, bool ignorePropertiesElementName = false)
	{
		if (propertyValues.IsSimpleValue)
		{
			AppendXmlPropertyValue(propName, propertyValues.ObjectValue, sb, orgLength, ignoreValue: false, ignorePropertiesElementName);
			return;
		}
		string text = AppendXmlPropertyValue(propName, string.Empty, sb, orgLength, ignoreValue: true, ignorePropertiesElementName);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		foreach (ObjectReflectionCache.ObjectPropertyList.PropertyValue propertyValue in propertyValues)
		{
			int length = sb.Length;
			if (length > 524288)
			{
				break;
			}
			if (!string.IsNullOrEmpty(propertyValue.Name) && (IncludeEmptyValue || !StringHelpers.IsNullOrEmptyString(propertyValue.Value)))
			{
				TypeCode typeCode = propertyValue.TypeCode;
				if (typeCode != TypeCode.Object)
				{
					string xmlValueString = XmlHelper.XmlConvertToString((IConvertible)propertyValue.Value, typeCode, safeConversion: true);
					AppendXmlPropertyStringValue(propertyValue.Name, xmlValueString, sb, orgLength, ignoreValue: false, ignorePropertiesElementName);
				}
				else if (!AppendXmlPropertyObjectValue(propertyValue.Name, propertyValue.Value, sb, orgLength, objectsInPath, depth))
				{
					sb.Length = length;
				}
			}
		}
		AppendClosingPropertyTag(text, sb, ignorePropertiesElementName);
	}

	private string AppendXmlPropertyValue(string propName, object? propertyValue, StringBuilder sb, int orgLength, bool ignoreValue = false, bool ignorePropertiesElementName = false)
	{
		string xmlValueString = (ignoreValue ? string.Empty : XmlHelper.XmlConvertToStringSafe(propertyValue));
		return AppendXmlPropertyStringValue(propName, xmlValueString, sb, orgLength, ignoreValue, ignorePropertiesElementName);
	}

	private string AppendXmlPropertyStringValue(string propName, string xmlValueString, StringBuilder sb, int orgLength, bool ignoreValue = false, bool ignorePropertiesElementName = false)
	{
		if (string.IsNullOrEmpty(PropertiesElementName))
		{
			return string.Empty;
		}
		propName = propName?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(propName))
		{
			return string.Empty;
		}
		if (sb.Length == orgLength && !string.IsNullOrEmpty(ElementNameInternal))
		{
			BeginXmlDocument(sb, ElementNameInternal);
		}
		if (IndentXml && !string.IsNullOrEmpty(ElementNameInternal))
		{
			sb.Append("  ");
		}
		sb.Append('<');
		string text;
		if (ignorePropertiesElementName)
		{
			text = XmlHelper.XmlConvertToElementName(propName);
			sb.Append(text);
		}
		else
		{
			if (_propertiesElementNameHasFormat)
			{
				text = XmlHelper.XmlConvertToElementName(propName);
				sb.AppendFormat(CultureInfo.InvariantCulture, PropertiesElementName, text);
			}
			else
			{
				text = PropertiesElementName;
				sb.Append(PropertiesElementName);
			}
			RenderAttribute(sb, PropertiesElementKeyAttribute, propName);
		}
		if (ignoreValue)
		{
			sb.Append('>');
			if (IndentXml)
			{
				sb.AppendLine();
			}
		}
		else if (RenderAttribute(sb, PropertiesElementValueAttribute, xmlValueString))
		{
			sb.Append("/>");
			if (IndentXml)
			{
				sb.AppendLine();
			}
		}
		else
		{
			sb.Append('>');
			XmlHelper.EscapeXmlString(xmlValueString, xmlEncodeNewlines: false, sb);
			AppendClosingPropertyTag(text, sb, ignorePropertiesElementName);
		}
		return text;
	}

	private void AppendClosingPropertyTag(string propNameElement, StringBuilder sb, bool ignorePropertiesElementName = false)
	{
		sb.Append("</");
		if (ignorePropertiesElementName)
		{
			sb.Append(propNameElement);
		}
		else
		{
			sb.AppendFormat(CultureInfo.InvariantCulture, PropertiesElementName, propNameElement);
		}
		sb.Append('>');
		if (IndentXml)
		{
			sb.AppendLine();
		}
	}

	/// <summary>
	/// write attribute, only if <paramref name="attributeName" /> is not empty
	/// </summary>
	/// <param name="sb"></param>
	/// <param name="attributeName"></param>
	/// <param name="value"></param>
	/// <returns>rendered</returns>
	private static bool RenderAttribute(StringBuilder sb, string attributeName, string value)
	{
		if (!string.IsNullOrEmpty(attributeName))
		{
			sb.Append(' ');
			sb.Append(attributeName);
			sb.Append("=\"");
			XmlHelper.EscapeXmlString(value, xmlEncodeNewlines: true, sb);
			sb.Append('"');
			return true;
		}
		return false;
	}

	private bool RenderAppendXmlElementValue(XmlElementBase xmlElement, LogEventInfo logEvent, StringBuilder sb, bool beginXmlDocument)
	{
		if (string.IsNullOrEmpty(xmlElement.ElementNameInternal))
		{
			return false;
		}
		if (beginXmlDocument && !string.IsNullOrEmpty(ElementNameInternal))
		{
			BeginXmlDocument(sb, ElementNameInternal);
		}
		if (IndentXml && !string.IsNullOrEmpty(ElementNameInternal))
		{
			sb.Append("  ");
		}
		int length = sb.Length;
		xmlElement.Render(logEvent, sb);
		if (sb.Length == length && !xmlElement.IncludeEmptyValue)
		{
			return false;
		}
		if (IndentXml)
		{
			sb.AppendLine();
		}
		return true;
	}

	private bool RenderAppendXmlAttributeValue(XmlAttribute xmlAttribute, LogEventInfo logEvent, StringBuilder sb, bool beginXmlDocument)
	{
		string name = xmlAttribute.Name;
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		if (beginXmlDocument)
		{
			sb.Append('<');
			sb.Append(ElementNameInternal);
		}
		sb.Append(' ');
		sb.Append(name);
		sb.Append("=\"");
		if (!xmlAttribute.RenderAppendXmlValue(logEvent, sb))
		{
			return false;
		}
		sb.Append('"');
		return true;
	}

	private void BeginXmlDocument(StringBuilder sb, string elementName)
	{
		RenderStartElement(sb, elementName);
		if (IndentXml)
		{
			sb.AppendLine();
		}
	}

	private void EndXmlDocument(StringBuilder sb, string elementName)
	{
		RenderEndElement(sb, elementName);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (_elements.Count > 0)
		{
			return ToStringWithNestedItems(_elements, (XmlElement l) => (!string.IsNullOrEmpty(l.ElementNameInternal)) ? ("TagName=" + l.ElementNameInternal) : l.ToString());
		}
		if (!string.IsNullOrEmpty(ElementNameInternal))
		{
			return ToStringWithNestedItems(new XmlElementBase[1] { this }, (XmlElementBase l) => "TagName=" + l.ElementNameInternal);
		}
		if (_attributes.Count > 0)
		{
			return ToStringWithNestedItems(_attributes, (XmlAttribute a) => "Attribute=" + a.Name);
		}
		if (ContextProperties != null && ContextProperties.Count > 0)
		{
			return ToStringWithNestedItems(ContextProperties, (TargetPropertyWithContext n) => "Property=" + n.Name);
		}
		return GetType().Name;
	}

	private static void RenderSelfClosingElement(StringBuilder target, string elementName)
	{
		target.Append('<');
		target.Append(elementName);
		target.Append("/>");
	}

	private static void RenderStartElement(StringBuilder sb, string elementName)
	{
		sb.Append('<');
		sb.Append(elementName);
		sb.Append('>');
	}

	private static void RenderEndElement(StringBuilder sb, string elementName)
	{
		sb.Append("</");
		sb.Append(elementName);
		sb.Append('>');
	}
}

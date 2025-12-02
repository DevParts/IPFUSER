using System;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.Layouts;

/// <summary>
/// XML attribute.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/XmlLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/XmlLayout">Documentation on NLog Wiki</seealso>
[NLogConfigurationItem]
public class XmlAttribute
{
	private readonly ValueTypeLayoutInfo _layoutInfo = new ValueTypeLayoutInfo();

	private string _name = string.Empty;

	private bool _includeEmptyValue;

	/// <summary>
	/// Gets or sets the name of the attribute.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="1" />
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = XmlHelper.XmlConvertToElementName(value?.Trim() ?? string.Empty);
		}
	}

	/// <summary>
	/// Gets or sets the layout used for rendering the attribute value.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout Layout
	{
		get
		{
			return _layoutInfo.Layout;
		}
		set
		{
			_layoutInfo.Layout = value;
		}
	}

	/// <summary>
	/// Gets or sets the result value type, for conversion of layout rendering output
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public Type? ValueType
	{
		get
		{
			return _layoutInfo.ValueType;
		}
		set
		{
			_layoutInfo.ValueType = value;
		}
	}

	/// <summary>
	/// Gets or sets the fallback value when result value is not available
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public Layout? DefaultValue
	{
		get
		{
			return _layoutInfo.DefaultValue;
		}
		set
		{
			_layoutInfo.DefaultValue = value;
		}
	}

	/// <summary>
	/// Gets or sets whether output should be encoded with Xml-string escaping, or be treated as valid xml-attribute-value
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool Encode { get; set; }

	/// <summary>
	/// Gets or sets whether empty attribute value should be included in the output.
	/// </summary>
	/// <remarks>Default: <see langword="false" /> . Empty value is either null or empty string</remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool IncludeEmptyValue
	{
		get
		{
			return _includeEmptyValue;
		}
		set
		{
			_includeEmptyValue = value;
			_layoutInfo.ForceDefaultValueNull = !value;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlAttribute" /> class.
	/// </summary>
	public XmlAttribute()
		: this(string.Empty, NLog.Layouts.Layout.Empty, encode: true)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlAttribute" /> class.
	/// </summary>
	/// <param name="name">The name of the attribute.</param>
	/// <param name="layout">The layout of the attribute's value.</param>
	public XmlAttribute(string name, Layout layout)
		: this(name, layout, encode: true)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlAttribute" /> class.
	/// </summary>
	/// <param name="name">The name of the attribute.</param>
	/// <param name="layout">The layout of the attribute's value.</param>
	/// <param name="encode">Encode value with xml-encode</param>
	public XmlAttribute(string name, Layout layout, bool encode)
	{
		Name = name;
		Layout = layout;
		Encode = encode;
		IncludeEmptyValue = false;
	}

	internal bool RenderAppendXmlValue(LogEventInfo logEvent, StringBuilder builder)
	{
		if ((object)ValueType == null)
		{
			int length = builder.Length;
			Layout?.Render(logEvent, builder);
			if (!IncludeEmptyValue && builder.Length <= length)
			{
				return false;
			}
			if (Encode)
			{
				XmlHelper.PerformXmlEscapeWhenNeeded(builder, length, xmlEncodeNewlines: true);
			}
		}
		else
		{
			object obj = _layoutInfo.RenderValue(logEvent);
			if (!IncludeEmptyValue && StringHelpers.IsNullOrEmptyString(obj))
			{
				return false;
			}
			IConvertible convertible = obj as IConvertible;
			TypeCode typeCode = (TypeCode)(((int?)convertible?.GetTypeCode()) ?? ((obj != null) ? 1 : 0));
			if (typeCode != TypeCode.Object)
			{
				string value = XmlHelper.XmlConvertToString(convertible, typeCode, safeConversion: true);
				builder.Append(value);
			}
			else
			{
				string value2 = XmlHelper.XmlConvertToStringSafe(obj);
				builder.Append(value2);
			}
		}
		return true;
	}
}

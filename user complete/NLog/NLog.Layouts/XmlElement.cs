using NLog.Config;

namespace NLog.Layouts;

/// <summary>
/// A XML Element
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/XmlLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/XmlLayout">Documentation on NLog Wiki</seealso>
[ThreadAgnostic]
public class XmlElement : XmlElementBase
{
	private const string DefaultElementName = "item";

	/// <summary>
	/// Name of the element
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <c>item</c></remarks>
	/// <docgen category="Layout Options" order="1" />
	public string Name
	{
		get
		{
			return base.ElementNameInternal;
		}
		set
		{
			base.ElementNameInternal = value;
		}
	}

	/// <summary>
	/// Value inside the element
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout Value
	{
		get
		{
			return LayoutWrapper.Inner;
		}
		set
		{
			LayoutWrapper.Inner = value;
		}
	}

	/// <summary>
	/// Gets or sets the layout used for rendering the XML-element InnerText.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout Layout
	{
		get
		{
			return Value;
		}
		set
		{
			Value = value;
		}
	}

	/// <summary>
	/// Gets or sets whether output should be encoded with XML-string escaping, or be treated as valid xml-element-value
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool Encode
	{
		get
		{
			return LayoutWrapper.XmlEncode;
		}
		set
		{
			LayoutWrapper.XmlEncode = value;
		}
	}

	/// <summary>
	/// Gets or sets whether output should be wrapped using CDATA section instead of XML-string escaping
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	public bool CDataEncode
	{
		get
		{
			return LayoutWrapper.CDataEncode;
		}
		set
		{
			LayoutWrapper.CDataEncode = value;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlElement" /> class.
	/// </summary>
	public XmlElement()
		: this("item", NLog.Layouts.Layout.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlElement" /> class.
	/// </summary>
	public XmlElement(string elementName, Layout elementValue)
		: base(elementName, elementValue)
	{
	}
}

using NLog.Config;

namespace NLog.Layouts;

/// <summary>
/// A specialized layout that renders XML-formatted events.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/XmlLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/XmlLayout">Documentation on NLog Wiki</seealso>
[Layout("XmlLayout")]
[ThreadAgnostic]
public class XmlLayout : XmlElementBase
{
	private const string DefaultRootElementName = "logevent";

	/// <summary>
	/// Name of the root XML element
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <c>logevent</c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string ElementName
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
	/// Value inside the root XML element
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout ElementValue
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
	/// Determines whether or not this attribute will be Xml encoded.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool ElementEncode
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
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlLayout" /> class.
	/// </summary>
	public XmlLayout()
		: this("logevent", Layout.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.XmlLayout" /> class.
	/// </summary>
	/// <param name="elementName">The name of the top XML node</param>
	/// <param name="elementValue">The value of the top XML node</param>
	public XmlLayout(string elementName, Layout elementValue)
		: base(elementName, elementValue)
	{
	}
}

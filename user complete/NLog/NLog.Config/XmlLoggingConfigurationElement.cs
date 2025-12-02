using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Represents simple XML element with case-insensitive attribute semantics.
/// </summary>
[Obsolete("Instead use TextReader as input. Marked obsolete with NLog 6.0")]
internal sealed class XmlLoggingConfigurationElement : ILoggingConfigurationElement
{
	/// <summary>
	/// Gets the element name.
	/// </summary>
	public string LocalName { get; }

	/// <summary>
	/// Gets the dictionary of attribute values.
	/// </summary>
	public IList<KeyValuePair<string, string?>> AttributeValues { get; }

	/// <summary>
	/// Gets the collection of child elements.
	/// </summary>
	public IList<XmlLoggingConfigurationElement> Children { get; }

	/// <summary>
	/// Gets the value of the element.
	/// </summary>
	public string? Value { get; }

	public string Name => LocalName;

	public IEnumerable<KeyValuePair<string, string?>> Values
	{
		get
		{
			for (int i = 0; i < Children.Count; i++)
			{
				if (SingleValueElement(Children[i]))
				{
					return AttributeValues.Concat<KeyValuePair<string, string>>(from item in Children
						where SingleValueElement(item)
						select new KeyValuePair<string, string>(item.Name, item.Value ?? string.Empty));
				}
			}
			return AttributeValues;
		}
	}

	IEnumerable<ILoggingConfigurationElement> ILoggingConfigurationElement.Children
	{
		get
		{
			for (int i = 0; i < Children.Count; i++)
			{
				if (!SingleValueElement(Children[i]))
				{
					return Children.Where((XmlLoggingConfigurationElement item) => !SingleValueElement(item)).Cast<ILoggingConfigurationElement>();
				}
			}
			return ArrayHelper.Empty<ILoggingConfigurationElement>();
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfigurationElement" /> class.
	/// </summary>
	/// <param name="reader">The reader to initialize element from.</param>
	public XmlLoggingConfigurationElement(XmlReader reader)
		: this(reader, nestedElement: false)
	{
	}

	public XmlLoggingConfigurationElement(XmlReader reader, bool nestedElement)
	{
		AttributeValues = ParseAttributes(reader, nestedElement);
		LocalName = reader.LocalName;
		Children = ParseChildren(reader, nestedElement, out string innerText);
		Value = innerText;
	}

	private static bool SingleValueElement(XmlLoggingConfigurationElement child)
	{
		if (child.Children.Count == 0 && child.AttributeValues.Count == 0)
		{
			return child.Value != null;
		}
		return false;
	}

	/// <summary>
	/// Asserts that the name of the element is among specified element names.
	/// </summary>
	/// <param name="allowedNames">The allowed names.</param>
	public void AssertName(params string[] allowedNames)
	{
		foreach (string value in allowedNames)
		{
			if (LocalName.Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
		}
		throw new InvalidOperationException("Assertion failed. Expected element name '" + string.Join("|", allowedNames) + "', actual: '" + LocalName + "'.");
	}

	private static IList<XmlLoggingConfigurationElement> ParseChildren(XmlReader reader, bool nestedElement, out string? innerText)
	{
		IList<XmlLoggingConfigurationElement> list = null;
		innerText = null;
		if (!reader.IsEmptyElement)
		{
			while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.CDATA || reader.NodeType == XmlNodeType.Text)
				{
					innerText += reader.Value;
				}
				else if (reader.NodeType == XmlNodeType.Element)
				{
					list = list ?? new List<XmlLoggingConfigurationElement>();
					bool nestedElement2 = nestedElement || !string.Equals(reader.LocalName, "nlog", StringComparison.OrdinalIgnoreCase);
					list.Add(new XmlLoggingConfigurationElement(reader, nestedElement2));
				}
			}
		}
		return list ?? ArrayHelper.Empty<XmlLoggingConfigurationElement>();
	}

	private static IList<KeyValuePair<string, string?>> ParseAttributes(XmlReader reader, bool nestedElement)
	{
		IList<KeyValuePair<string, string>> list = null;
		if (reader.MoveToFirstAttribute())
		{
			do
			{
				if (nestedElement || !IsSpecialXmlAttribute(reader))
				{
					list = list ?? new List<KeyValuePair<string, string>>();
					list.Add(new KeyValuePair<string, string>(reader.LocalName, reader.Value));
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}
		return list ?? ArrayHelper.Empty<KeyValuePair<string, string>>();
	}

	/// <summary>
	/// Special attribute we could ignore
	/// </summary>
	private static bool IsSpecialXmlAttribute(XmlReader reader)
	{
		string localName = reader.LocalName;
		if (localName != null && localName.Equals("xmlns", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		string localName2 = reader.LocalName;
		if (localName2 != null && localName2.Equals("schemaLocation", StringComparison.OrdinalIgnoreCase) && !StringHelpers.IsNullOrWhiteSpace(reader.Prefix))
		{
			return true;
		}
		string prefix = reader.Prefix;
		if (prefix != null && prefix.Equals("xsi", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		string prefix2 = reader.Prefix;
		if (prefix2 != null && prefix2.Equals("xmlns", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return Name;
	}
}

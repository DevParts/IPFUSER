using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Internal;

namespace NLog.Config;

internal sealed class XmlParserConfigurationElement : ILoggingConfigurationElement
{
	/// <summary>
	/// Gets the element name.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// Gets the value of the element.
	/// </summary>
	public string? Value { get; private set; }

	/// <summary>
	/// Gets the dictionary of attribute values.
	/// </summary>
	public IList<KeyValuePair<string, string?>> AttributeValues { get; }

	/// <summary>
	/// Gets the collection of child elements.
	/// </summary>
	public IList<XmlParserConfigurationElement> Children { get; }

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
					return Children.Where((XmlParserConfigurationElement item) => !SingleValueElement(item)).Cast<ILoggingConfigurationElement>();
				}
			}
			return ArrayHelper.Empty<ILoggingConfigurationElement>();
		}
	}

	public XmlParserConfigurationElement(XmlParser.XmlParserElement xmlElement)
		: this(xmlElement, nestedElement: false)
	{
	}

	public XmlParserConfigurationElement(XmlParser.XmlParserElement xmlElement, bool nestedElement)
	{
		int num = xmlElement.Name.IndexOf(':');
		Name = ((num >= 0) ? xmlElement.Name.Substring(num + 1) : xmlElement.Name);
		Value = xmlElement.InnerText;
		AttributeValues = ParseAttributes(xmlElement, nestedElement);
		Children = ParseChildren(xmlElement, nestedElement);
	}

	private static bool SingleValueElement(XmlParserConfigurationElement child)
	{
		if (child.Children.Count == 0 && child.AttributeValues.Count == 0)
		{
			return child.Value != null;
		}
		return false;
	}

	private static IList<KeyValuePair<string, string>> ParseAttributes(XmlParser.XmlParserElement xmlElement, bool nestedElement)
	{
		IList<KeyValuePair<string, string>> attributes = xmlElement.Attributes;
		if (attributes != null && attributes.Count > 0)
		{
			if (!nestedElement)
			{
				for (int num = attributes.Count - 1; num >= 0; num--)
				{
					if (IsSpecialXmlRootAttribute(attributes[num].Key))
					{
						attributes.RemoveAt(num);
					}
				}
			}
			for (int i = 0; i < attributes.Count; i++)
			{
				int num2 = attributes[i].Key.IndexOf(':');
				if (num2 >= 0)
				{
					attributes[i] = new KeyValuePair<string, string>(attributes[i].Key.Substring(num2 + 1), attributes[i].Value);
				}
			}
		}
		return attributes ?? ArrayHelper.Empty<KeyValuePair<string, string>>();
	}

	private static IList<XmlParserConfigurationElement> ParseChildren(XmlParser.XmlParserElement xmlElement, bool nestedElement)
	{
		IList<XmlParser.XmlParserElement> children = xmlElement.Children;
		if (children == null || children.Count == 0)
		{
			return ArrayHelper.Empty<XmlParserConfigurationElement>();
		}
		List<XmlParserConfigurationElement> list = new List<XmlParserConfigurationElement>();
		for (int i = 0; i < children.Count; i++)
		{
			XmlParser.XmlParserElement xmlParserElement = children[i];
			bool nestedElement2 = nestedElement || !string.Equals(xmlParserElement.Name, "nlog", StringComparison.OrdinalIgnoreCase);
			list.Add(new XmlParserConfigurationElement(xmlParserElement, nestedElement2));
		}
		return list;
	}

	/// <summary>
	/// Special attribute we could ignore
	/// </summary>
	private static bool IsSpecialXmlRootAttribute(string attributeName)
	{
		if (attributeName != null && attributeName.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (attributeName != null && attributeName.IndexOf(":xmlns", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}
		if (attributeName != null && attributeName.StartsWith("schemaLocation", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (attributeName != null && attributeName.IndexOf(":schemaLocation", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}
		if (attributeName != null && attributeName.StartsWith("xsi:", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}
}

using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlUtility
{
	private XmlUtility()
	{
	}

	public static bool SelectChildrenByName(XmlTextReader reader, string strName)
	{
		if (!GoDownOneLevel(reader))
		{
			return false;
		}
		return SelectElementByName(reader, strName);
	}

	public static bool SelectChildrenByAttribute(XmlTextReader reader, string strAttribute, string strValue)
	{
		if (!GoDownOneLevel(reader))
		{
			return false;
		}
		int depth = reader.Depth;
		do
		{
			if (reader.NodeType == XmlNodeType.Element && strValue == reader[strAttribute])
			{
				return true;
			}
			reader.Skip();
		}
		while (!reader.EOF && depth == reader.Depth);
		return false;
	}

	public static bool SelectNextSibling(XmlTextReader reader)
	{
		int depth = reader.Depth;
		reader.Skip();
		while (depth == reader.Depth && !reader.EOF)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				return true;
			}
			reader.Skip();
		}
		return depth == reader.Depth;
	}

	public static bool SelectNextElementOnLevel(XmlTextReader reader)
	{
		int depth = reader.Depth;
		reader.Skip();
		while (depth >= reader.Depth && !reader.EOF)
		{
			if (reader.NodeType == XmlNodeType.Element && depth == reader.Depth)
			{
				return true;
			}
			reader.Skip();
		}
		return false;
	}

	public static bool SelectElementByName(XmlTextReader reader, string strName)
	{
		int depth = reader.Depth;
		do
		{
			if (reader.NodeType == XmlNodeType.Element && strName == reader.Name)
			{
				return true;
			}
			reader.Skip();
		}
		while (!reader.EOF && depth == reader.Depth);
		return false;
	}

	public static bool GetFirstElementOnLevel(XmlTextReader reader)
	{
		int depth = reader.Depth;
		do
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				return true;
			}
		}
		while (reader.Read() && depth == reader.Depth);
		return false;
	}

	public static bool GoDownOneLevel(XmlTextReader reader)
	{
		int num = reader.Depth + 1;
		while (num != reader.Depth || reader.NodeType != XmlNodeType.Element)
		{
			if (!reader.Read() || num < reader.Depth || num > reader.Depth + 1)
			{
				return false;
			}
			if (reader.NodeType == XmlNodeType.Element && num - 1 == reader.Depth)
			{
				return false;
			}
		}
		return true;
	}

	public static bool GoUpOneLevel(XmlTextReader reader)
	{
		int num = reader.Depth - 1;
		while (num != reader.Depth || reader.NodeType != XmlNodeType.Element)
		{
			if (!reader.Read() || num > reader.Depth || num < reader.Depth - 1)
			{
				return false;
			}
		}
		return true;
	}

	public static bool SelectNextElement(XmlTextReader reader)
	{
		while (reader.Read())
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				return true;
			}
		}
		return false;
	}

	public static bool SelectNextElement(XmlTextReader reader, string strName)
	{
		if (SelectNextElement(reader))
		{
			return IsElement(reader, strName);
		}
		return false;
	}

	public static bool IsElement(XmlTextReader reader, string strName)
	{
		if (!reader.EOF && reader.NodeType == XmlNodeType.Element)
		{
			return reader.Name == strName;
		}
		return false;
	}
}

using System.Collections.Specialized;
using System.Xml;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessXmlToList : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		StringCollection stringCollection = new StringCollection();
		string triggeredString = GetTriggeredString(dp, 0);
		if (string.IsNullOrEmpty(triggeredString))
		{
			return stringCollection;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(triggeredString);
		XmlNode xmlNode = xmlDocument.ChildNodes[0];
		if (xmlNode != null)
		{
			foreach (XmlNode childNode in xmlNode.ChildNodes)
			{
				XmlAttribute xmlAttribute = childNode.Attributes[0];
				if (xmlAttribute != null)
				{
					stringCollection.Add(xmlAttribute.Value);
				}
			}
		}
		return stringCollection;
	}
}

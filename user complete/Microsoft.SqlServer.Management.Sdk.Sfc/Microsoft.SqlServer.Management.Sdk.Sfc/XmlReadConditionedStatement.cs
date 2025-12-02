using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadConditionedStatement : XmlReadRepeated
{
	public string Sql => GetTextOfElement();

	public StringCollection Fields => XmlRead.GetFields(base.Reader["fields"]);

	public XmlReadMultipleLink MultipleLink
	{
		get
		{
			if (XmlUtility.IsElement(base.Reader, "link_multiple"))
			{
				return new XmlReadMultipleLink(this);
			}
			return null;
		}
	}

	public XmlReadConditionedStatement(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

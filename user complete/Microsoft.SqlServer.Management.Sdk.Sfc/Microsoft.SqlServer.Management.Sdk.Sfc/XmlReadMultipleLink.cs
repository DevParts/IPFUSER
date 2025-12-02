using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadMultipleLink : XmlRead
{
	public string No => base.Reader["no"];

	public string Expression => GetAliasString(base.Reader["expression"]);

	public XmlReadLinkFields LinkFields
	{
		get
		{
			if (!XmlUtility.SelectNextElement(base.Reader))
			{
				return null;
			}
			if (XmlUtility.IsElement(base.Reader, "link_field"))
			{
				return new XmlReadLinkFields(this);
			}
			return null;
		}
	}

	public XmlReadMultipleLink(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

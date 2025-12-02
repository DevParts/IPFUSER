using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadMultipleLink : XmlRead
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

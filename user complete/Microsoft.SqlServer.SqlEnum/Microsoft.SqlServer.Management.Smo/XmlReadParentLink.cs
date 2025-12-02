using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadParentLink : XmlRead
{
	public XmlReadSimpleParentLink SimpleParentLink
	{
		get
		{
			if (!XmlUtility.SelectNextElement(base.Reader))
			{
				return null;
			}
			if (XmlUtility.IsElement(base.Reader, "link"))
			{
				return new XmlReadSimpleParentLink(this);
			}
			return null;
		}
	}

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

	public XmlReadParentLink(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadParentLink : XmlRead
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

using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlRequestParentSelect : XmlRead
{
	public XmlRequestParentSelectField Field
	{
		get
		{
			if (!XmlUtility.SelectNextElement(base.Reader))
			{
				return null;
			}
			if (XmlUtility.IsElement(base.Reader, "field"))
			{
				return new XmlRequestParentSelectField(this);
			}
			return null;
		}
	}

	public XmlRequestParentSelect(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

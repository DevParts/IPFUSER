using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlRequestParentSelect : XmlRead
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

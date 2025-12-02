using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadRepeated : XmlRead
{
	public XmlReadRepeated(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public XmlReadRepeated()
	{
	}

	public virtual bool Next()
	{
		base.Closed = true;
		return XmlUtility.SelectNextSibling(base.Reader);
	}

	public bool Next(string elemName)
	{
		base.Closed = true;
		if (!XmlUtility.SelectNextElement(base.Reader))
		{
			return false;
		}
		return IsElementWithCheckVersion(elemName);
	}
}

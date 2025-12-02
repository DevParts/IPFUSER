using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadRepeated : XmlRead
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

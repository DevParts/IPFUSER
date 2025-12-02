using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadSimpleParentLink : XmlReadRepeated
{
	public string Local => base.Reader["local"];

	public string Parent => base.Reader["parent"];

	public XmlReadSimpleParentLink(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		if (base.Next() && XmlUtility.IsElement(base.Reader, "link"))
		{
			return true;
		}
		return false;
	}
}

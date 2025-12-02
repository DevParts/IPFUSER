using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadSimpleParentLink : XmlReadRepeated
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

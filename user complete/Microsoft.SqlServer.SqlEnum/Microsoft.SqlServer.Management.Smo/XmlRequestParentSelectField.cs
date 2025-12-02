using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlRequestParentSelectField : XmlReadRepeated
{
	public string Name => base.Reader["name"];

	public XmlRequestParentSelectField(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		if (base.Next() && XmlUtility.IsElement(base.Reader, "field"))
		{
			return true;
		}
		return false;
	}
}

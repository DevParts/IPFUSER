using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlRequestParentSelectField : XmlReadRepeated
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

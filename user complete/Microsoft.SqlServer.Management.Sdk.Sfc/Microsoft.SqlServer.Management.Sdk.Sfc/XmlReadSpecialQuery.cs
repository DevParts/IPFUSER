using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadSpecialQuery : XmlRead
{
	public string Database => base.Reader["database"];

	public string Query => base.Reader["query"];

	public XmlReadSpecialQuery(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

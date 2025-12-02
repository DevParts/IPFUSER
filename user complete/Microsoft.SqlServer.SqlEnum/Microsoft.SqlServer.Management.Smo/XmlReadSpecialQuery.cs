using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadSpecialQuery : XmlRead
{
	public string Database => base.Reader["database"];

	public string Query => base.Reader["query"];

	public string Hint => base.Reader["hint"];

	public XmlReadSpecialQuery(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

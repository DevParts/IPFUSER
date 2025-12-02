using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadOrderByRedirect : XmlReadRepeated
{
	public string Field => base.Reader["field"];

	public StringCollection RedirectFields => XmlRead.GetFields(base.Reader["redirect_fields"]);

	public XmlReadOrderByRedirect(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		return Next("redirect_orderby");
	}
}

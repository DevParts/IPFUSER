using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadInclude : XmlRead
{
	public string File => base.Reader["file"];

	public string TableAlias => GetAliasString(base.Reader["alias"]);

	public StringCollection RequestedFields => XmlRead.GetFields(base.Reader["for"]);

	public StringCollection ROAfterCreation => XmlRead.GetFields(base.Reader["ro_after_creation"]);

	public XmlReadInclude(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadInclude : XmlRead
{
	public string File => base.Reader["file"];

	public string TableAlias => GetAliasString(base.Reader["alias"]);

	public StringCollection RequestedFields => XmlRead.GetFields(base.Reader["for"]);

	public XmlReadInclude(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

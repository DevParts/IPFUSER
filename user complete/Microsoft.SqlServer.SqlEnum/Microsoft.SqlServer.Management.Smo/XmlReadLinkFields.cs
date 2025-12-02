using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadLinkFields : XmlReadRepeated
{
	public LinkFieldType Type => base.Reader["type"] switch
	{
		"parent" => LinkFieldType.Parent, 
		"local" => LinkFieldType.Local, 
		"computed" => LinkFieldType.Computed, 
		"filter" => LinkFieldType.Filter, 
		_ => LinkFieldType.Computed, 
	};

	public string Field => base.Reader["field"];

	public string DefaultValue => base.Reader["default_value"];

	public XmlReadLinkFields(XmlRead xmlReader)
		: base(xmlReader)
	{
	}
}

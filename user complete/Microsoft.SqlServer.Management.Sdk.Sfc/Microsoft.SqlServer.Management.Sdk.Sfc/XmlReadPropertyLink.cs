using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadPropertyLink : XmlReadRepeated
{
	public StringCollection Fields => XmlRead.GetFields(base.Reader["fields"]);

	public string Table => GetAliasString(base.Reader["table"]);

	public string TableAlias => GetAliasString(base.Reader["alias"]);

	public string InnerJoin => GetAliasString(base.Reader["join"]);

	public bool ExpressionIsForTableName => null != base.Reader["expression_is_for_table_name"];

	public string LeftJoin => GetAliasString(base.Reader["left_join"]);

	public string Filter => GetAliasString(GetTextOfElement());

	public XmlReadMultipleLink MultipleLink
	{
		get
		{
			if (XmlUtility.IsElement(base.Reader, "link_multiple"))
			{
				return new XmlReadMultipleLink(this);
			}
			return null;
		}
	}

	public XmlReadPropertyLink(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		return Next("property_link");
	}
}

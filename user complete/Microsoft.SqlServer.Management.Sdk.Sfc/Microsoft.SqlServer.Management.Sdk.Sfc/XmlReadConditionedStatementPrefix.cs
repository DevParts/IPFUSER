using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadConditionedStatementPrefix : XmlReadConditionedStatement
{
	public XmlReadConditionedStatementPrefix(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		return Next("prefix");
	}
}

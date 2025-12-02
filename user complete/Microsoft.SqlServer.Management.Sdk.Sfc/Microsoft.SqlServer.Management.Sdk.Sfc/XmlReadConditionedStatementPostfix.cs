using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadConditionedStatementPostfix : XmlReadConditionedStatement
{
	public XmlReadConditionedStatementPostfix(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		return Next("postfix");
	}
}

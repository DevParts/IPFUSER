using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadConditionedStatementPostfix : XmlReadConditionedStatement
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

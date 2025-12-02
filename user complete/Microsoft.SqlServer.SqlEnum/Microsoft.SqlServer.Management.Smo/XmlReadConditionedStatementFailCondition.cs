using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadConditionedStatementFailCondition : XmlReadConditionedStatement
{
	public XmlReadConditionedStatementFailCondition(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		return Next("fail_condition");
	}
}

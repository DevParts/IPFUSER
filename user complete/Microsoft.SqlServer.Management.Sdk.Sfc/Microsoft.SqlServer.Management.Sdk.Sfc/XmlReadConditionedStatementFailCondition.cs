using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadConditionedStatementFailCondition : XmlReadConditionedStatement
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

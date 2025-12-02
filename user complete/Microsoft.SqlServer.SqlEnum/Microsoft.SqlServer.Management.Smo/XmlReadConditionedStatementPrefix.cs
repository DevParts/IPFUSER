using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class XmlReadConditionedStatementPrefix : XmlReadConditionedStatement
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

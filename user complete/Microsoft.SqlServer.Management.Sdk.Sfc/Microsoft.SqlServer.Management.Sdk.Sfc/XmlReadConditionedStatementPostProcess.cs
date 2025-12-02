using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[ComVisible(false)]
public class XmlReadConditionedStatementPostProcess : XmlReadRepeated
{
	public string ClassName => base.Reader["class_name"];

	public StringCollection Fields => XmlRead.GetFields(base.Reader["fields"]);

	public StringCollection TriggeredFields => XmlRead.GetFields(base.Reader["triggered_fields"]);

	public XmlReadConditionedStatementPostProcess(XmlRead xmlReader)
		: base(xmlReader)
	{
	}

	public override bool Next()
	{
		return Next("post_process");
	}
}

using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessCreateSqlSecureString : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		object triggeredObject = GetTriggeredObject(dp, 0);
		return new SqlSecureString(triggeredObject.ToString());
	}
}

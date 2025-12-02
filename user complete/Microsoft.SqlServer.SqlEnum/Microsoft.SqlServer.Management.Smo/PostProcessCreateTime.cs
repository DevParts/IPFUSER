using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessCreateTime : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		int triggeredInt = GetTriggeredInt32(dp, 0);
		if (triggeredInt > 0)
		{
			data = new TimeSpan(triggeredInt / 10000, triggeredInt / 100 % 100, triggeredInt % 100);
		}
		return data;
	}
}

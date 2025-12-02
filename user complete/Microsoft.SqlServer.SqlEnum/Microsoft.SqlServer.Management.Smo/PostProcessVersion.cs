using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessVersion : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		return ConvertToValidVersion(GetTriggeredInt32(dp, 0), GetTriggeredInt32(dp, 1), GetTriggeredInt32(dp, 2), GetTriggeredInt32(dp, 3));
	}

	internal static Version ConvertToValidVersion(int major, int minor, int build, int revision)
	{
		return new Version((-1 != major) ? major : 0, (-1 != minor) ? minor : 0, (-1 != build) ? build : 0, (-1 != revision) ? revision : 0);
	}
}

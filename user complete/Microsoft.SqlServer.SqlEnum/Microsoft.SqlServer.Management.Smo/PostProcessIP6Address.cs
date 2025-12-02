using System.Net;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessIP6Address : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (IsNull(data))
		{
			return data;
		}
		_ = (string)data;
		return IPAddress.Parse((string)data);
	}
}

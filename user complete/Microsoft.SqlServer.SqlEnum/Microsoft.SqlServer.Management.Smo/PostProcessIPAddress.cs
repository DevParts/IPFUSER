using System.Globalization;
using System.Net;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessIPAddress : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (IsNull(data))
		{
			return IPAddress.None;
		}
		string text = (string)data;
		long num = 0L;
		string[] array = text.Split('.');
		if (array.Length != 4)
		{
			return IPAddress.None;
		}
		int num2 = 0;
		string[] array2 = array;
		foreach (string s in array2)
		{
			long num3 = int.Parse(s, CultureInfo.InvariantCulture);
			num += num3 * (1 << num2++ * 8);
		}
		return new IPAddress(num);
	}
}

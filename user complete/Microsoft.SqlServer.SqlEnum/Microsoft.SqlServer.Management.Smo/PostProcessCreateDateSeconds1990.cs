using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessCreateDateSeconds1990 : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		int num = 0;
		object triggeredObject = GetTriggeredObject(dp, 0);
		if (!IsNull(triggeredObject))
		{
			num = (int)triggeredObject;
		}
		if (num <= 0)
		{
			return DateTime.MinValue;
		}
		DateTime dateTime = new DateTime(1990, 1, 1);
		TimeSpan value = new TimeSpan(num / 3600, num % 3600 / 60, num % 60);
		data = dateTime.Add(value);
		return data;
	}
}

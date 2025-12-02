using System;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessCreateDateTime : PostProcess
{
	protected object GetDateTime(object oDate, object oTime)
	{
		if (IsNull(oDate))
		{
			return DBNull.Value;
		}
		int num = (int)oDate;
		if (IsNull(oTime))
		{
			return DBNull.Value;
		}
		int num2 = (int)oTime;
		if (num <= 0 || num2 < 0)
		{
			return DBNull.Value;
		}
		return new DateTime(num / 10000, num / 100 % 100, num % 100, num2 / 10000, num2 / 100 % 100, num2 % 100);
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		return GetDateTime(GetTriggeredObject(dp, 0), GetTriggeredObject(dp, 1));
	}
}

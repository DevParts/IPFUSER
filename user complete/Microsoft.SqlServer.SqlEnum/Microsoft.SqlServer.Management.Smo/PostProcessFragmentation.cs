using System;
using System.Collections;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessFragmentation : PostProcess
{
	private bool calledUseDB;

	private bool isInitDataRow;

	private string[] contigData;

	internal PostProcessFragmentation()
	{
		calledUseDB = true;
		isInitDataRow = false;
		contigData = null;
	}

	private string GetSql(DataProvider dp)
	{
		int triggeredInt = GetTriggeredInt32(dp, 0);
		int triggeredInt2 = GetTriggeredInt32(dp, 1);
		string triggeredString = GetTriggeredString(dp, 2);
		if (calledUseDB)
		{
			return string.Format(CultureInfo.InvariantCulture, "USE [{2}] DBCC SHOWCONTIG({0}, {1})", new object[3]
			{
				triggeredInt2,
				triggeredInt,
				Util.EscapeString(triggeredString, ']')
			});
		}
		calledUseDB = false;
		return string.Format(CultureInfo.InvariantCulture, "DBCC SHOWCONTIG({0}, {1})", new object[2] { triggeredInt2, triggeredInt });
	}

	private void InitRowData(DataProvider dp)
	{
		if (isInitDataRow)
		{
			return;
		}
		isInitDataRow = true;
		contigData = new string[9];
		ArrayList arrayList = ExecuteSql.ExecuteImmediateGetMessage(GetSql(dp), base.ConnectionInfo);
		foreach (SqlInfoMessageEventArgs item in arrayList)
		{
			int num = 0;
			for (int i = 0; i < item.Errors.Count; i++)
			{
				if (num >= contigData.Length)
				{
					break;
				}
				if (item.Errors[i].Number != 7945)
				{
					continue;
				}
				do
				{
					SqlError sqlError = item.Errors[i];
					int num2 = sqlError.Message.LastIndexOf("..:", sqlError.Message.Length - 1, StringComparison.Ordinal);
					if (num2 < 0)
					{
						break;
					}
					num2 += 3;
					int num3 = sqlError.Message.LastIndexOf('%');
					if (num3 <= num2)
					{
						num3 = sqlError.Message.Length;
					}
					contigData[num++] = sqlError.Message.Substring(num2, num3 - num2);
					i++;
				}
				while (i < item.Errors.Count && num < contigData.Length);
			}
			if (num >= contigData.Length)
			{
				return;
			}
		}
		contigData = null;
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		InitRowData(dp);
		if (contigData != null)
		{
			switch (name)
			{
			case "Pages":
				return long.Parse(contigData[0], CultureInfo.InvariantCulture);
			case "Extents":
				return int.Parse(contigData[1], CultureInfo.InvariantCulture);
			case "ExtentSwitches":
				return int.Parse(contigData[2], CultureInfo.InvariantCulture);
			case "ScanDensity":
				return double.Parse(contigData[4], CultureInfo.InvariantCulture);
			case "LogicalFragmentation":
				return double.Parse(contigData[5], CultureInfo.InvariantCulture);
			case "ExtentFragmentation":
				return double.Parse(contigData[6], CultureInfo.InvariantCulture);
			case "AverageFreeBytes":
				return double.Parse(contigData[7], CultureInfo.InvariantCulture);
			case "AveragePageDensity":
				return double.Parse(contigData[8], CultureInfo.InvariantCulture);
			}
		}
		return null;
	}

	public override void CleanRowData()
	{
		contigData = null;
		isInitDataRow = false;
	}
}

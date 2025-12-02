using System.Data;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessStatisticStream : PostProcessCreateDateTime
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		string triggeredString = GetTriggeredString(dp, 0);
		string triggeredString2 = GetTriggeredString(dp, 1);
		DataTable dataTable = ExecuteSql.ExecuteWithResults("DBCC SHOW_STATISTICS(" + Util.MakeSqlString(triggeredString2) + ", " + Util.MakeSqlString(triggeredString) + ") WITH STATS_STREAM", base.ConnectionInfo);
		if (dataTable.Rows.Count > 0)
		{
			return dataTable.Rows[0]["Stats_Stream"];
		}
		return data;
	}
}

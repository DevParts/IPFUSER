using System.Data;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessFileProperties : PostProcess
{
	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		DataTable dataTable = null;
		string triggeredString = GetTriggeredString(dp, 0);
		int triggeredInt = GetTriggeredInt32(dp, 1);
		if (dataTable == null)
		{
			dataTable = ExecuteSql.ExecuteWithResults(string.Format(CultureInfo.InvariantCulture, "  CREATE TABLE #tempspace (value BIGINT) \r\n IF (OBJECT_ID(N'msdb.sys.sp_getVolumeFreeSpace',  N'P')) is null \r\n INSERT INTO #tempspace VALUES(-1) ELSE \r\n INSERT INTO #tempspace EXEC msdb.sys.sp_getVolumeFreeSpace {0},{1} \r\n SELECT TOP 1 value  as [freebytes] from #tempspace   as [freebytes] \r\n DROP TABLE #tempspace \r\n", new object[2]
			{
				Util.MakeSqlString(triggeredString),
				triggeredInt
			}), base.ConnectionInfo);
		}
		if (dataTable.Rows.Count > 0)
		{
			return (long)dataTable.Rows[0]["freebytes"];
		}
		return data;
	}
}

using System;
using System.Data;
using System.Globalization;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessFile : PostProcess
{
	private bool firstTime = true;

	private float usedSpace;

	private float availableSpace;

	protected override bool SupportDataReader => false;

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (firstTime)
		{
			ExecuteQuery(data, dp);
			firstTime = false;
		}
		return name switch
		{
			"UsedSpace" => usedSpace, 
			"AvailableSpace" => availableSpace, 
			_ => data, 
		};
	}

	public override void CleanRowData()
	{
		firstTime = true;
	}

	private void ExecuteQuery(object data, DataProvider dp)
	{
		string text = string.Format(CultureInfo.InvariantCulture, "use [{0}];", new object[1] { Util.EscapeString(GetTriggeredString(dp, 2), ']') });
		string format = ((!ExecuteSql.IsContainedAuthentication(base.ConnectionInfo)) ? "select \r\nCAST(CASE s.type WHEN 2 THEN s.size * CONVERT(float,8) ELSE dfs.allocated_extent_page_count*convert(float,8) END AS float) AS [UsedSpace],\r\nCASE s.type WHEN 2 THEN 0 ELSE <msparam>{0}</msparam> - dfs.allocated_extent_page_count*convert(float,8) END AS [AvailableSpace] \r\nfrom \r\nsys.filegroups AS g\r\ninner join sys.database_files AS s on ((s.type = 2 or s.type = 0) and (s.drop_lsn IS NULL)) AND (s.data_space_id=g.data_space_id)\r\nleft outer join sys.dm_db_file_space_usage as dfs ON dfs.database_id = db_id() AND dfs.file_id = s.file_id\r\nwhere \r\ns.name = <msparam>{1}</msparam> and g.data_space_id = <msparam>{2}</msparam>" : "create table #tmpspc (Fileid int, FileGroup int, TotalExtents int, UsedExtents int, Name sysname, FileName nchar(520))\r\ninsert #tmpspc EXEC ('dbcc showfilestats');\r\n\r\nSELECT\r\nCAST(CASE s.type WHEN 2 THEN s.size * CONVERT(float,8) ELSE tspc.UsedExtents*convert(float,64) END AS float) AS [UsedSpace],\r\nCASE s.type WHEN 2 THEN 0 ELSE <msparam>{0}</msparam> - tspc.UsedExtents*convert(float,64) END AS [AvailableSpace]\r\nFROM\r\nsys.filegroups AS g\r\nINNER JOIN sys.database_files AS s ON ((s.type = 2 or s.type = 0) and (s.drop_lsn IS NULL)) AND (s.data_space_id=g.data_space_id)\r\nLEFT OUTER JOIN #tmpspc tspc ON tspc.Fileid = s.file_id\r\nwhere \r\ns.name = <msparam>{1}</msparam> and g.data_space_id = <msparam>{2}</msparam>;\r\n\r\ndrop table #tmpspc;");
		DataTable dataTable = ExecuteSql.ExecuteWithResults(text + string.Format(CultureInfo.InvariantCulture, format, new object[3]
		{
			GetTriggeredObject(dp, 3),
			GetTriggeredString(dp, 0),
			GetTriggeredInt32(dp, 1)
		}), base.ConnectionInfo);
		usedSpace = Convert.ToSingle(dataTable.Rows[0]["UsedSpace"], CultureInfo.InvariantCulture);
		availableSpace = Convert.ToSingle(dataTable.Rows[0]["AvailableSpace"], CultureInfo.InvariantCulture);
	}
}

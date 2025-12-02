using System;
using System.Data;
using System.Globalization;
using System.Text;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessTable : PostProcess
{
	private const string rowCount = "RowCount";

	private DataRow rowResults;

	private string databaseName;

	private string schemaName;

	private string tableName;

	private string query;

	public PostProcessTable()
	{
		rowResults = null;
		databaseName = string.Empty;
		schemaName = string.Empty;
		tableName = string.Empty;
		query = string.Empty;
	}

	private void GetRowResults(DataProvider dp)
	{
		if (rowResults == null)
		{
			databaseName = GetTriggeredString(dp, 0);
			schemaName = GetTriggeredString(dp, 1);
			tableName = GetTriggeredString(dp, 2);
			DataTable dataTable = null;
			BuildQuery();
			dataTable = ExecuteSql.ExecuteWithResults(query, base.ConnectionInfo, databaseName, poolConnection: false);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rowResults = dataTable.Rows[0];
			}
		}
	}

	public override void CleanRowData()
	{
		rowResults = null;
	}

	private void BuildQuery()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StatementBuilder statementBuilder = new StatementBuilder();
		if (GetIsFieldHit("RowCount"))
		{
			string value = string.Format(CultureInfo.InvariantCulture, "(CASE WHEN (tbl.is_memory_optimized=0) \r\n                            THEN ISNULL((SELECT SUM (spart.rows) FROM sys.partitions spart WHERE spart.object_id = tbl.object_id AND spart.index_id < 2), 0)\r\n                            ELSE ISNULL((SELECT COUNT(*) FROM [{0}].[{1}]), 0) END)", new object[2]
			{
				Util.EscapeString(schemaName, ']'),
				Util.EscapeString(tableName, ']')
			});
			statementBuilder.AddProperty("RowCount", value);
		}
		stringBuilder.Append(statementBuilder.SqlStatement);
		string value2 = string.Format(CultureInfo.InvariantCulture, "FROM sys.tables tbl WHERE SCHEMA_NAME(tbl.schema_id)=N'{0}' AND tbl.name=N'{1}'", new object[2]
		{
			Util.EscapeString(schemaName, '\''),
			Util.EscapeString(tableName, '\'')
		});
		stringBuilder.Append(value2);
		query = stringBuilder.ToString();
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		GetRowResults(dp);
		string text;
		if (rowResults == null)
		{
			data = DBNull.Value;
		}
		else if ((text = name) != null && text == "RowCount")
		{
			data = Convert.ToInt64(rowResults["RowCount"]);
		}
		return data;
	}
}

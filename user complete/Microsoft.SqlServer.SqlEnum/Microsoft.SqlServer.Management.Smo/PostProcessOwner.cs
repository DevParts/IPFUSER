using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessOwner : PostProcess
{
	private string uSid;

	private bool firstTime = true;

	private string ownerName;

	protected override bool SupportDataReader => false;

	private PostProcessOwner()
	{
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (firstTime)
		{
			ownerName = ExecuteQuery(data, dp);
			firstTime = false;
		}
		string text;
		if ((text = name) != null && text == "Owner")
		{
			return ownerName;
		}
		return data;
	}

	private string ExecuteQuery(object data, DataProvider dp)
	{
		uSid = GetTriggeredString(dp, 0);
		string value = string.Format(CultureInfo.InvariantCulture, "DECLARE @sid varbinary(max) \r\n                DECLARE @name varchar(max)\r\n                SET @sid = {0}\r\n                set @name = CAST(@sid as UNIQUEIDENTIFIER)\r\n                select @name = name from sys.database_principals WITH (NOLOCK) where sid=@sid\r\n                IF @@ROWCOUNT = 0\r\n                BEGIN\r\n                  select @name = name from sys.sql_logins WITH (NOLOCK) where sid=@sid\r\n                END\r\n                select @name", new object[1] { uSid });
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(value);
		DataTable dataTable;
		try
		{
			dataTable = ExecuteSql.ExecuteWithResults(stringCollection, base.ConnectionInfo, "master");
		}
		catch (ExecutionFailureException)
		{
			return string.Empty;
		}
		catch (ConnectionFailureException)
		{
			return string.Empty;
		}
		if (dataTable.Rows.Count == 0)
		{
			return string.Empty;
		}
		return dataTable.Rows[0][0].ToString();
	}

	public override void CleanRowData()
	{
		firstTime = true;
	}
}

using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessUser : PostProcess
{
	private string uSid;

	private bool firstTime = true;

	private string str;

	protected override bool SupportDataReader => false;

	private PostProcessUser()
	{
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (firstTime)
		{
			str = ExecuteQuery(data, dp);
			firstTime = false;
		}
		switch (name)
		{
		case "Login":
		case "Owner":
			return str;
		case "UserType":
			if (string.IsNullOrEmpty(str))
			{
				return 3;
			}
			return 0;
		default:
			return data;
		}
	}

	private string ExecuteQuery(object data, DataProvider dp)
	{
		uSid = GetTriggeredString(dp, 0);
		if (string.IsNullOrEmpty(uSid) || uSid.Equals("0x00", StringComparison.InvariantCultureIgnoreCase) || uSid.Equals("0x01", StringComparison.InvariantCultureIgnoreCase))
		{
			return string.Empty;
		}
		string value = string.Format(CultureInfo.InvariantCulture, "Select name from sys.sql_logins WITH (NOLOCK) where sid={0}", new object[1] { uSid });
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

using System;
using System.Data;
using System.Data.Sql;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[ComVisible(false)]
internal class AvailableSQLServers : EnumObject
{
	public override ResultType[] ResultTypes => new ResultType[1] { ResultType.DataTable };

	private AvailableSQLServers()
	{
		AddProperty(new ObjectProperty
		{
			Name = "Name",
			Type = "System.String",
			ReadOnly = true,
			Usage = (ObjectPropertyUsages.Filter | ObjectPropertyUsages.Request)
		});
		AddProperty(new ObjectProperty
		{
			Name = "Server",
			Type = "System.String",
			ReadOnly = true,
			Usage = ObjectPropertyUsages.Request
		});
		AddProperty(new ObjectProperty
		{
			Name = "Instance",
			Type = "System.String",
			ReadOnly = true,
			Usage = ObjectPropertyUsages.Request
		});
		AddProperty(new ObjectProperty
		{
			Name = "IsClustered",
			Type = "System.Boolean",
			ReadOnly = true,
			Usage = ObjectPropertyUsages.Request
		});
		AddProperty(new ObjectProperty
		{
			Name = "Version",
			Type = "System.String",
			ReadOnly = true,
			Usage = ObjectPropertyUsages.Request
		});
		AddProperty(new ObjectProperty
		{
			Name = "IsLocal",
			Type = "System.Boolean",
			ReadOnly = true,
			Usage = (ObjectPropertyUsages.Filter | ObjectPropertyUsages.Request)
		});
	}

	public override EnumResult GetData(EnumResult res)
	{
		DataTable dataSources = SqlDataSourceEnumerator.Instance.GetDataSources();
		DataTable dataTable = new DataTable();
		dataSources.Locale = CultureInfo.InvariantCulture;
		string[] fields = base.Request.Fields;
		foreach (string text in fields)
		{
			ObjectProperty property = GetProperty(text, ObjectPropertyUsages.Request);
			dataTable.Columns.Add(text, Type.GetType(property.Type));
		}
		string fixedStringProperty = GetFixedStringProperty("Name", removeEscape: true);
		FilterNodeFunction filterNodeFunction = base.FixedProperties["IsLocal"] as FilterNodeFunction;
		bool flag = false;
		bool flag2 = false;
		if (filterNodeFunction != null)
		{
			if (filterNodeFunction.FunctionType == FilterNodeFunction.Type.True)
			{
				flag2 = true;
				flag = true;
			}
			else if (filterNodeFunction.FunctionType == FilterNodeFunction.Type.False)
			{
				flag = true;
			}
		}
		foreach (DataRow row in dataSources.Rows)
		{
			string text2 = row["InstanceName"].ToString();
			if (text2 != null && (text2.Length <= 0 || "MSSQLSERVER" == text2))
			{
				text2 = null;
			}
			string text3 = row["ServerName"].ToString() + ((text2 != null) ? ("\\" + text2) : "");
			if (fixedStringProperty != null && string.Compare(text3, fixedStringProperty, StringComparison.OrdinalIgnoreCase) != 0)
			{
				continue;
			}
			bool flag3 = 0 == string.Compare(row["ServerName"].ToString(), System.Environment.MachineName, StringComparison.OrdinalIgnoreCase);
			if (flag && flag2 != flag3)
			{
				continue;
			}
			DataRow dataRow2 = dataTable.NewRow();
			string[] fields2 = base.Request.Fields;
			foreach (string text4 in fields2)
			{
				switch (text4)
				{
				case "Name":
					dataRow2[text4] = text3;
					break;
				case "IsLocal":
					dataRow2[text4] = flag3;
					break;
				case "Server":
					dataRow2[text4] = row["ServerName"];
					break;
				case "Instance":
					dataRow2[text4] = text2;
					break;
				case "IsClustered":
					dataRow2[text4] = !("No" == row["IsClustered"].ToString());
					break;
				case "Version":
					dataRow2[text4] = row["Version"];
					break;
				}
			}
			dataTable.Rows.Add(dataRow2);
		}
		return new EnumResult(dataTable, ResultType.DataTable);
	}
}

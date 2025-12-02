using System;
using System.Collections.Specialized;
using System.Data;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

public class ConfigurationBase
{
	internal Server m_server;

	internal DataTable m_table;

	public static string UrnSuffix => "Configuration";

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent => m_server;

	internal DataTable ConfigDataTable => m_table;

	internal ConfigurationBase(Server server)
	{
		m_server = server;
	}

	internal void PopulateDataTable()
	{
		Request req = new Request("Server/Configuration");
		new Enumerator();
		m_table = m_server.ExecutionManager.GetEnumeratorData(req);
		m_table.Columns.Add("Value", Type.GetType("System.Int32"));
		m_table.Columns.Add("Updated", Type.GetType("System.Boolean"));
		foreach (DataRow row in m_table.Rows)
		{
			row["Value"] = row["ConfigValue"];
			row["Updated"] = false;
		}
	}

	internal object GetConfigProperty(int iNumber, string sColumnName)
	{
		if (ConfigDataTable == null)
		{
			PopulateDataTable();
		}
		object result = null;
		foreach (DataRow row in ConfigDataTable.Rows)
		{
			if (iNumber == (int)row["Number"])
			{
				result = row[sColumnName];
				break;
			}
		}
		return result;
	}

	internal bool SetConfigProperty(int iNumber, int iValue)
	{
		if (ConfigDataTable == null)
		{
			PopulateDataTable();
		}
		bool result = false;
		foreach (DataRow row in ConfigDataTable.Rows)
		{
			if (iNumber == (int)row["Number"])
			{
				row["Value"] = iValue;
				row["Updated"] = true;
				result = true;
				break;
			}
		}
		return result;
	}

	private bool IsRowChanged(DataRow row)
	{
		return (bool)row["Updated"];
	}

	private void CleanRow(DataRow row)
	{
		row["Updated"] = false;
		row["ConfigValue"] = row["Value"];
		if ((bool)row["Dynamic"])
		{
			row["RunValue"] = row["Value"];
		}
	}

	private bool ShowAdvancedOptionsIsSet()
	{
		return 0 != (int)GetConfigProperty(518, "RunValue");
	}

	private string GetSchema()
	{
		if (m_server.ServerVersion.Major >= 9)
		{
			return "sys";
		}
		return "master.dbo";
	}

	private void ScriptAlterWithStatistics(StringCollection configStrings, ref bool bHasChangedOptions, ref bool bHasAdvancedOptions, ref bool bShowAdvancedOptionsModified)
	{
		foreach (DataRow row in ConfigDataTable.Rows)
		{
			if (IsRowChanged(row))
			{
				bHasChangedOptions = true;
				if ((bool)row["Advanced"])
				{
					bHasAdvancedOptions = true;
				}
				if (518 == (int)row["Number"])
				{
					bShowAdvancedOptionsModified = true;
				}
				configStrings.Add(string.Format(SmoApplication.DefaultCulture, "EXEC {2}.sp_configure N'{0}', N'{1}'", new object[3]
				{
					SqlSmoObject.SqlString((string)row["Name"]),
					row["Value"].ToString(),
					GetSchema()
				}));
			}
		}
	}

	public void Refresh()
	{
		m_table = null;
	}

	internal void DoAlter(bool overrideValueChecking)
	{
		if (ConfigDataTable == null)
		{
			return;
		}
		bool flag = ShowAdvancedOptionsIsSet();
		bool bHasChangedOptions = false;
		bool bHasAdvancedOptions = false;
		bool bShowAdvancedOptionsModified = false;
		bool flag2 = false;
		StringCollection stringCollection = new StringCollection();
		ScriptAlterWithStatistics(stringCollection, ref bHasChangedOptions, ref bHasAdvancedOptions, ref bShowAdvancedOptionsModified);
		if (stringCollection.Count <= 0)
		{
			return;
		}
		if (!flag && bHasAdvancedOptions)
		{
			m_server.ExecutionManager.ExecuteNonQuery("EXEC " + GetSchema() + ".sp_configure N'show advanced options', N'1'  RECONFIGURE WITH OVERRIDE");
			flag2 = true;
		}
		try
		{
			stringCollection.Add(overrideValueChecking ? "RECONFIGURE WITH OVERRIDE" : "RECONFIGURE");
			m_server.ExecutionManager.ExecuteNonQuery(stringCollection);
			if (bShowAdvancedOptionsModified)
			{
				flag2 = false;
			}
		}
		finally
		{
			if (flag2)
			{
				m_server.ExecutionManager.ExecuteNonQuery("EXEC " + GetSchema() + ".sp_configure N'show advanced options', N'0'  RECONFIGURE WITH OVERRIDE");
			}
		}
	}

	internal void CleanObject()
	{
		if (ConfigDataTable == null)
		{
			return;
		}
		foreach (DataRow row in ConfigDataTable.Rows)
		{
			CleanRow(row);
		}
	}

	internal void ScriptAlter(StringCollection query, ScriptingPreferences sp, bool overrideValueChecking)
	{
		DoAlter(overrideValueChecking);
	}

	public void Alter()
	{
		Alter(overrideValueChecking: false);
	}

	public void Alter(bool overrideValueChecking)
	{
		try
		{
			DoAlter(overrideValueChecking);
			if (!m_server.ExecutionManager.Recording)
			{
				CleanObject();
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, ex);
		}
	}
}

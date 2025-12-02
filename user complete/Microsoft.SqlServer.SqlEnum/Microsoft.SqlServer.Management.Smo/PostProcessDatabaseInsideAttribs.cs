using System;
using System.Data;
using System.Globalization;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessDatabaseInsideAttribs : PostProcessWithRowCaching
{
	private double bytesPerPage;

	private string databaseName = string.Empty;

	private string sqlQuery;

	private string defaultdataPath = string.Empty;

	protected override bool SupportDataReader => false;

	private double BytesPerPage
	{
		get
		{
			if (0.0 == bytesPerPage)
			{
				DataTable dataTable = ExecuteSql.ExecuteWithResults("select convert(float,low/1024.) from master.dbo.spt_values where number = 1 and type = 'E'", base.ConnectionInfo);
				bytesPerPage = (double)dataTable.Rows[0][0];
			}
			return bytesPerPage;
		}
	}

	private string DefaultDataPath
	{
		get
		{
			if (string.IsNullOrEmpty(defaultdataPath))
			{
				DataTable dataTable = ExecuteSql.ExecuteWithResults("select SERVERPROPERTY('instancedefaultdatapath')", base.ConnectionInfo);
				defaultdataPath = (string)dataTable.Rows[0][0];
				defaultdataPath = defaultdataPath.Remove(defaultdataPath.Length - 1, 1);
			}
			return defaultdataPath;
		}
	}

	protected override string SqlQuery
	{
		get
		{
			if (sqlQuery == null)
			{
				sqlQuery = ((ExecuteSql.GetServerVersion(base.ConnectionInfo).Major < 9) ? BuildSqlStatementLess90() : BuildSqlStatementMoreEqual90());
				sqlQuery = string.Format(CultureInfo.InvariantCulture, sqlQuery, new object[1] { Util.EscapeString(databaseName, '\'') });
			}
			return sqlQuery;
		}
	}

	public override void CleanRowData()
	{
		if (!GetIsFieldHit("HasMemoryOptimizedObjects") || HitFieldsCount() != 1 || ExecuteSql.GetDatabaseEngineType(base.ConnectionInfo) == DatabaseEngineType.SqlAzureDatabase)
		{
			base.CleanRowData();
		}
	}

	private void BuildCommonSql(StatementBuilder sb)
	{
		if (GetIsFieldHit("IsDbAccessAdmin"))
		{
			sb.AddProperty("IsDbAccessAdmin", "is_member(N'db_accessadmin')");
		}
		if (GetIsFieldHit("IsDbBackupOperator"))
		{
			sb.AddProperty("IsDbBackupOperator", "is_member(N'db_backupoperator')");
		}
		if (GetIsFieldHit("IsDbDatareader"))
		{
			sb.AddProperty("IsDbDatareader", "is_member(N'db_datareader')");
		}
		if (GetIsFieldHit("IsDbDatawriter"))
		{
			sb.AddProperty("IsDbDatawriter", "is_member(N'db_datawriter')");
		}
		if (GetIsFieldHit("IsDbOwner"))
		{
			sb.AddProperty("IsDbOwner", "is_member(N'db_owner')");
		}
		if (GetIsFieldHit("IsDbSecurityAdmin"))
		{
			sb.AddProperty("IsDbSecurityAdmin", "is_member(N'db_securityadmin')");
		}
		if (GetIsFieldHit("IsDbDdlAdmin"))
		{
			sb.AddProperty("IsDbDdlAdmin", "is_member(N'db_ddladmin')");
		}
		if (GetIsFieldHit("IsDbDenyDatareader"))
		{
			sb.AddProperty("IsDbDenyDatareader", "is_member(N'db_denydatareader')");
		}
		if (GetIsFieldHit("IsDbDenyDatawriter"))
		{
			sb.AddProperty("IsDbDenyDatawriter", "is_member(N'db_denydatawriter')");
		}
		if (GetIsFieldHit("DboLogin"))
		{
			sb.AddProperty("DboLogin", "is_member(N'db_owner')");
		}
		if (GetIsFieldHit("UserName"))
		{
			sb.AddProperty("UserName", "user_name()");
		}
	}

	private string BuildSqlStatementMoreEqual90()
	{
		StatementBuilder statementBuilder = new StatementBuilder();
		BuildCommonSql(statementBuilder);
		if (GetIsFieldHit("SpaceAvailable") || GetIsFieldHit("Size"))
		{
			statementBuilder.AddProperty("DbSize", "(SELECT SUM(CAST(df.size as float)) FROM sys.database_files AS df WHERE df.type in ( 0, 2, 4 ) )");
		}
		if (GetIsFieldHit("SpaceAvailable"))
		{
			statementBuilder.AddProperty("SpaceUsed", "(SUM(a.total_pages) + (SELECT ISNULL(SUM(CAST(df.size as bigint)), 0) FROM sys.database_files AS df WHERE df.type = 2 ))");
		}
		if (GetIsFieldHit("Size"))
		{
			statementBuilder.AddProperty("LogSize", "(SELECT SUM(CAST(df.size as float)) FROM sys.database_files AS df WHERE df.type in (1, 3))");
		}
		if (GetIsFieldHit("HasMemoryOptimizedObjects"))
		{
			DatabaseEngineType databaseEngineType = ExecuteSql.GetDatabaseEngineType(base.ConnectionInfo);
			if (HitFieldsCount() > 1 || databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					statementBuilder.AddProperty("HasMemoryOptimizedObjects", "(SELECT databasepropertyex(db_name(db_id()),'IsXTPSupported'))");
				}
				else
				{
					statementBuilder.AddProperty("HasMemoryOptimizedObjects", "ISNULL((select top 1 1 from sys.filegroups FG where FG.[type] = 'FX'), 0)");
				}
			}
			else
			{
				statementBuilder.AddProperty(null, "db.name as HasMemoryOptimizedObjects from master.sys.master_files mf join master.sys.databases db on mf.database_id = db.database_id where mf.[type] = 2");
			}
		}
		if (GetIsFieldHit("MemoryAllocatedToMemoryOptimizedObjectsInKB"))
		{
			statementBuilder.AddProperty("MemoryAllocatedToMemoryOptimizedObjectsInKB", "isnull((select convert(decimal(18,2),(sum(tms.memory_allocated_for_table_kb) + sum(tms.memory_allocated_for_indexes_kb))) \r\n                                                                                from [sys].[dm_db_xtp_table_memory_stats] tms), 0.00)");
		}
		if (GetIsFieldHit("MemoryUsedByMemoryOptimizedObjectsInKB"))
		{
			statementBuilder.AddProperty("MemoryUsedByMemoryOptimizedObjectsInKB", "isnull((select convert(decimal(18,2),(sum(tms.memory_used_by_table_kb) + sum(tms.memory_used_by_indexes_kb))) \r\n                                                                           from [sys].[dm_db_xtp_table_memory_stats] tms), 0.00)");
		}
		if (GetIsFieldHit("HasFileInCloud"))
		{
			statementBuilder.AddProperty("HasFileInCloud", "ISNULL ((select top 1 1 from sys.database_files\r\n                                                    where state = 0 and physical_name like 'https%' collate SQL_Latin1_General_CP1_CI_AS), 0)");
		}
		if (GetIsFieldHit("DataSpaceUsage") || GetIsFieldHit("IndexSpaceUsage"))
		{
			statementBuilder.AddProperty("DataSpaceUsage", "SUM(CASE When it.internal_type IN (202,204,207,211,212,213,214,215,216) Then 0 When a.type <> 1 Then a.used_pages\tWhen p.index_id < 2 Then a.data_pages\tElse 0\tEND)");
		}
		if (GetIsFieldHit("IndexSpaceUsage"))
		{
			statementBuilder.AddProperty("IndexSpaceTotal", "SUM(a.used_pages)");
		}
		if (GetIsFieldHit("IsMailHost"))
		{
			statementBuilder.AddProperty("IsMailHost", "(select count(1) from sys.services where name ='InternalMailService')");
		}
		if (GetIsFieldHit("DefaultSchema"))
		{
			statementBuilder.AddProperty("DefaultSchema", "(select schema_name())");
		}
		if (GetIsFieldHit("DataSpaceUsage") || GetIsFieldHit("IndexSpaceUsage") || GetIsFieldHit("SpaceAvailable"))
		{
			statementBuilder.AddFrom("sys.partitions p join sys.allocation_units a on p.partition_id = a.container_id left join sys.internal_tables it on p.object_id = it.object_id");
		}
		if (GetIsFieldHit("DefaultFileGroup"))
		{
			statementBuilder.AddProperty("DefaultFileGroup", "(select top 1 ds.name from sys.data_spaces as ds where ds.is_default = 1 and ds.type = 'FG' )");
		}
		if (GetIsFieldHit("IsManagementDataWarehouse"))
		{
			statementBuilder.AddProperty("IsManagementDataWarehouse", "(select count(1) from sys.extended_properties where name = 'Microsoft_DataCollector_MDW_Version')");
		}
		if (GetIsFieldHit("DefaultFileStreamFileGroup"))
		{
			statementBuilder.AddProperty("DefaultFileStreamFileGroup", "(select case when t1.c1 > 0 then t1.c2 else N'' end from (select top 1 count(*) c1, min(ds.name) c2 from sys.data_spaces as ds where ds.is_default = 1 and ds.type = 'FD') t1)");
		}
		if (GetIsFieldHit("PrimaryFilePath") && ExecuteSql.IsContainedAuthentication(base.ConnectionInfo))
		{
			statementBuilder.AddProperty("PrimaryFilePath", "(select ISNULL(df.physical_name, N'') from sys.database_files as df where df.data_space_id = 1 and df.file_id = 1)");
		}
		if (GetIsFieldHit("MaxDop"))
		{
			statementBuilder.AddProperty("MaxDop", "(select value from sys.database_scoped_configurations as dsc where dsc.name = 'MAXDOP')");
		}
		if (GetIsFieldHit("MaxDopForSecondary"))
		{
			statementBuilder.AddProperty("MaxDopForSecondary", "(select value_for_secondary from sys.database_scoped_configurations as dsc where dsc.name = 'MAXDOP')");
		}
		if (GetIsFieldHit("LegacyCardinalityEstimation"))
		{
			statementBuilder.AddProperty("LegacyCardinalityEstimation", "(select value from sys.database_scoped_configurations as dsc where dsc.name = 'LEGACY_CARDINALITY_ESTIMATION')");
		}
		if (GetIsFieldHit("LegacyCardinalityEstimationForSecondary"))
		{
			statementBuilder.AddProperty("LegacyCardinalityEstimationForSecondary", "(select ISNULL(value_for_secondary, 2) from sys.database_scoped_configurations as dsc where dsc.name = 'LEGACY_CARDINALITY_ESTIMATION')");
		}
		if (GetIsFieldHit("ParameterSniffing"))
		{
			statementBuilder.AddProperty("ParameterSniffing", "(select value from sys.database_scoped_configurations as dsc where dsc.name = 'PARAMETER_SNIFFING')");
		}
		if (GetIsFieldHit("ParameterSniffingForSecondary"))
		{
			statementBuilder.AddProperty("ParameterSniffingForSecondary", "(select ISNULL(value_for_secondary, 2) from sys.database_scoped_configurations as dsc where dsc.name = 'PARAMETER_SNIFFING')");
		}
		if (GetIsFieldHit("QueryOptimizerHotfixes"))
		{
			statementBuilder.AddProperty("QueryOptimizerHotfixes", "(select value from sys.database_scoped_configurations as dsc where dsc.name = 'QUERY_OPTIMIZER_HOTFIXES')");
		}
		if (GetIsFieldHit("QueryOptimizerHotfixesForSecondary"))
		{
			statementBuilder.AddProperty("QueryOptimizerHotfixesForSecondary", "(select ISNULL(value_for_secondary, 2) from sys.database_scoped_configurations as dsc where dsc.name = 'QUERY_OPTIMIZER_HOTFIXES')");
		}
		AddDbChaining(statementBuilder);
		return statementBuilder.SqlStatement;
	}

	private string BuildSqlStatementLess90()
	{
		StatementBuilder statementBuilder = new StatementBuilder();
		BuildCommonSql(statementBuilder);
		if (GetIsFieldHit("SpaceAvailable") || GetIsFieldHit("Size"))
		{
			statementBuilder.AddProperty("DbSize", "(select sum(convert(float,size)) from dbo.sysfiles where (status & 64 = 0))");
		}
		if (GetIsFieldHit("SpaceAvailable"))
		{
			statementBuilder.AddProperty("SpaceUsed", "(select sum(convert(float,reserved)) from dbo.sysindexes where indid in (0, 1, 255))");
		}
		if (GetIsFieldHit("Size"))
		{
			statementBuilder.AddProperty("LogSize", "(select sum(convert(float,size)) from dbo.sysfiles where (status & 64 <> 0))");
		}
		if (GetIsFieldHit("DataSpaceUsage") || GetIsFieldHit("IndexSpaceUsage"))
		{
			statementBuilder.AddProperty("DataSpaceUsage", "((select sum(convert(float,dpages)) from dbo.sysindexes where indid < 2) + (select isnull(sum(convert(float,used)), 0) from dbo.sysindexes where indid = 255))");
		}
		if (GetIsFieldHit("IndexSpaceUsage"))
		{
			statementBuilder.AddProperty("IndexSpaceTotal", "(select sum(convert(float,used)) from dbo.sysindexes where indid in (0, 1, 255))");
		}
		if (GetIsFieldHit("DefaultSchema"))
		{
			statementBuilder.AddProperty("DefaultSchema", "user_name()");
		}
		if (GetIsFieldHit("DefaultFileGroup"))
		{
			statementBuilder.AddProperty("DefaultFileGroup", "(select top 1 fg.groupname from dbo.sysfilegroups as fg where fg.status & 0x10 <> 0)");
		}
		ServerVersion serverVersion = ExecuteSql.GetServerVersion(base.ConnectionInfo);
		if (serverVersion.Major >= 8 && serverVersion.BuildNumber >= 760)
		{
			AddDbChaining(statementBuilder);
		}
		return statementBuilder.SqlStatement;
	}

	private void AddDbChaining(StatementBuilder sb)
	{
		if (GetIsFieldHit("DatabaseOwnershipChaining"))
		{
			sb.AddPrefix("create table #tmpdbchaining( name sysname , dbc sysname )");
			ServerVersion serverVersion = ExecuteSql.GetServerVersion(base.ConnectionInfo);
			if (serverVersion.Major < 9)
			{
				sb.AddPrefix("insert into #tmpdbchaining exec dbo.sp_dboption N'{0}', 'db chaining'\n");
			}
			else
			{
				sb.AddPrefix("insert into #tmpdbchaining SELECT 'db chaining' AS 'OptionName', CASE WHEN (SELECT is_db_chaining_on FROM sys.databases WHERE name=N'{0}') = 1 THEN 'ON' ELSE 'OFF' END AS 'CurrentSetting'\n");
			}
			sb.AddPrefix("declare @DBChaining bit\nset @DBChaining = null\nselect @DBChaining = case LOWER(dbc) when 'off' then 0 else 1 end from #tmpdbchaining");
			sb.AddProperty("DatabaseOwnershipChaining", "@DBChaining");
			sb.AddPostfix("drop table #tmpdbchaining");
		}
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		ServerVersion serverVersion = ExecuteSql.GetServerVersion(base.ConnectionInfo);
		databaseName = GetTriggeredString(dp, 0);
		GetCachedRowResultsForDatabase(dp, databaseName);
		if (rowResults == null && string.Compare(name, "PrimaryFilePath", StringComparison.OrdinalIgnoreCase) != 0)
		{
			data = DBNull.Value;
		}
		else
		{
			switch (name)
			{
			case "SpaceAvailable":
			{
				double num = 0.0;
				if (!IsNull(data))
				{
					num = (double)data;
				}
				data = ((serverVersion.Major >= 9) ? ((object)(((double)rowResults[0]["DbSize"] - (double)(long)rowResults[0]["SpaceUsed"]) * BytesPerPage + num)) : ((object)(((double)rowResults[0]["DbSize"] - (double)rowResults[0]["SpaceUsed"]) * BytesPerPage + num)));
				if ((double)data < 0.0)
				{
					data = 0.0;
				}
				break;
			}
			case "Size":
			{
				double num2 = 0.0;
				if (rowResults[0]["LogSize"] is double)
				{
					num2 = (double)rowResults[0]["LogSize"];
				}
				data = ((double)rowResults[0]["DbSize"] + num2) * BytesPerPage / 1024.0;
				break;
			}
			case "DataSpaceUsage":
				data = ((serverVersion.Major >= 9) ? ((object)((double)(long)rowResults[0]["DataSpaceUsage"] * BytesPerPage)) : ((object)((double)rowResults[0]["DataSpaceUsage"] * BytesPerPage)));
				break;
			case "HasMemoryOptimizedObjects":
			{
				if (HitFieldsCount() > 1 || ExecuteSql.GetDatabaseEngineType(base.ConnectionInfo) == DatabaseEngineType.SqlAzureDatabase)
				{
					data = Convert.ToBoolean(rowResults[0]["HasMemoryOptimizedObjects"]);
					break;
				}
				bool value = false;
				for (int i = 0; i < rowResults.Count; i++)
				{
					if (rowResults[i]["HasMemoryOptimizedObjects"].ToString() == databaseName)
					{
						value = true;
						break;
					}
				}
				data = Convert.ToBoolean(value);
				break;
			}
			case "MemoryAllocatedToMemoryOptimizedObjectsInKB":
				data = (decimal)rowResults[0]["MemoryAllocatedToMemoryOptimizedObjectsInKB"];
				break;
			case "MemoryUsedByMemoryOptimizedObjectsInKB":
				data = (decimal)rowResults[0]["MemoryUsedByMemoryOptimizedObjectsInKB"];
				break;
			case "HasFileInCloud":
				data = Convert.ToBoolean(rowResults[0]["HasFileInCloud"]);
				break;
			case "IndexSpaceUsage":
				data = ((serverVersion.Major >= 9) ? ((object)((double)((long)rowResults[0]["IndexSpaceTotal"] - (long)rowResults[0]["DataSpaceUsage"]) * BytesPerPage)) : ((object)(((double)rowResults[0]["IndexSpaceTotal"] - (double)rowResults[0]["DataSpaceUsage"]) * BytesPerPage)));
				break;
			case "UserName":
			case "IsMailHost":
			case "DboLogin":
			case "DefaultSchema":
			case "IsDbOwner":
			case "IsDbDdlAdmin":
			case "IsDbDatareader":
			case "IsDbDatawriter":
			case "IsDbAccessAdmin":
			case "IsDbSecurityAdmin":
			case "IsDbBackupOperator":
			case "IsDbDenyDatareader":
			case "IsDbDenyDatawriter":
			case "DefaultFileGroup":
			case "MaxDop":
			case "MaxDopForSecondary":
			case "LegacyCardinalityEstimation":
			case "LegacyCardinalityEstimationForSecondary":
			case "ParameterSniffing":
			case "ParameterSniffingForSecondary":
			case "QueryOptimizerHotfixes":
			case "QueryOptimizerHotfixesForSecondary":
				data = rowResults[0][name];
				break;
			case "DefaultFileStreamFileGroup":
				if (serverVersion.Major >= 10)
				{
					data = rowResults[0]["DefaultFileStreamFileGroup"];
				}
				break;
			case "DatabaseOwnershipChaining":
				if (serverVersion.Major >= 8 && serverVersion.BuildNumber >= 760)
				{
					data = rowResults[0]["DatabaseOwnershipChaining"];
				}
				break;
			case "IsManagementDataWarehouse":
				if (serverVersion.Major >= 10)
				{
					data = rowResults[0]["IsManagementDataWarehouse"];
				}
				break;
			case "PrimaryFilePath":
				if (serverVersion.Major < 9)
				{
					if (IsNull(data))
					{
						return data;
					}
					data = GetPath((string)data);
					break;
				}
				if (ExecuteSql.IsContainedAuthentication(base.ConnectionInfo))
				{
					data = rowResults[0]["PrimaryFilePath"];
				}
				if (IsNull(data))
				{
					return string.Empty;
				}
				data = GetPath((string)data);
				break;
			}
		}
		return data;
	}

	private string GetPath(string sFullName)
	{
		if (sFullName == null || sFullName.Length == 0)
		{
			return string.Empty;
		}
		string result = string.Empty;
		try
		{
			result = PathWrapper.GetDirectoryName(sFullName);
		}
		catch (ArgumentException ex)
		{
			TraceHelper.LogExCatch(ex);
		}
		return result;
	}
}

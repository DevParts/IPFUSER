using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

internal class PostProcessAutoCloseProperties : PostProcessWithRowCaching
{
	protected override bool SupportDataReader => false;

	protected override string SqlQuery
	{
		get
		{
			string text = null;
			ServerVersion serverVersion = ExecuteSql.GetServerVersion(base.ConnectionInfo);
			if (serverVersion.Major >= 12)
			{
				return "SELECT dtb.collation_name AS [Collation], CAST(DATABASEPROPERTYEX(dtb.name, 'Version') AS int) AS [Version], dtb.compatibility_level AS [CompatibilityLevel], CAST(CHARINDEX(N'_CS_', dtb.collation_name) AS bit) AS [CaseSensitive], dtb.target_recovery_time_in_seconds AS [TargetRecoveryTime], dtb.delayed_durability AS [DelayedDurability] FROM master.sys.databases AS dtb where name = db_name()";
			}
			if (serverVersion.Major >= 11)
			{
				return "SELECT dtb.collation_name AS [Collation], CAST(DATABASEPROPERTYEX(dtb.name, 'Version') AS int) AS [Version], dtb.compatibility_level AS [CompatibilityLevel], CAST(CHARINDEX(N'_CS_', dtb.collation_name) AS bit) AS [CaseSensitive], dtb.target_recovery_time_in_seconds AS [TargetRecoveryTime] FROM master.sys.databases AS dtb where name = db_name()";
			}
			if (serverVersion.Major >= 9)
			{
				return "SELECT dtb.collation_name AS [Collation], CAST(DATABASEPROPERTYEX(dtb.name, 'Version') AS int) AS [Version], dtb.compatibility_level AS [CompatibilityLevel], CAST(CHARINDEX(N'_CS_', dtb.collation_name) AS bit) AS [CaseSensitive] FROM master.sys.databases AS dtb where name = db_name()";
			}
			return "SELECT CAST(DATABASEPROPERTYEX(dtb.name, 'Collation') AS sysname) AS [Collation], CAST(DATABASEPROPERTYEX(dtb.name, 'Version') AS int) AS [Version], dtb.cmptlevel AS [CompatibilityLevel], CAST(CHARINDEX(N'_CS_', CAST(DATABASEPROPERTYEX(dtb.name, 'Collation') AS nvarchar(255))) AS bit) AS [CaseSensitive] FROM master.dbo.sysdatabases AS dtb where name = db_name()";
		}
	}

	public override object GetColumnData(string name, object data, DataProvider dp)
	{
		if (IsNull(data))
		{
			GetCachedRowResultsForDatabase(dp, GetTriggeredString(dp, 0));
			data = ((rowResults == null) ? data : rowResults[0][name]);
		}
		return data;
	}
}

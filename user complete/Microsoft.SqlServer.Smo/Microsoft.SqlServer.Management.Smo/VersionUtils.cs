using System.ComponentModel;
using Microsoft.SqlServer.Management.Common;

namespace Microsoft.SqlServer.Management.Smo;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class VersionUtils
{
	internal static ServerVersion HighestKnownServerVersion => Sql150ServerVersion;

	internal static ServerVersion Sql110ServerVersion => new ServerVersion(11, 0);

	internal static ServerVersion Sql120ServerVersion => new ServerVersion(12, 0);

	internal static ServerVersion Sql130ServerVersion => new ServerVersion(13, 0);

	internal static ServerVersion Sql140ServerVersion => new ServerVersion(14, 0);

	internal static ServerVersion Sql150ServerVersion => new ServerVersion(15, 0);

	internal static SqlServerVersionInternal Sql110TargetServerVersion => SqlServerVersionInternal.Version110;

	internal static SqlServerVersionInternal Sql120TargetServerVersion => SqlServerVersionInternal.Version120;

	internal static SqlServerVersionInternal Sql130TargetServerVersion => SqlServerVersionInternal.Version130;

	internal static SqlServerVersionInternal Sql140TargetServerVersion => SqlServerVersionInternal.Version140;

	internal static SqlServerVersionInternal Sql150TargetServerVersion => SqlServerVersionInternal.Version150;

	internal static bool IsSql14OrLater(SqlServerVersionInternal targetServerVersion, ServerVersion currentServerVersion)
	{
		if (targetServerVersion >= SqlServerVersionInternal.Version140)
		{
			return currentServerVersion.Major >= 14;
		}
		return false;
	}

	public static bool IsSql14OrLater(ServerVersion currentServerVersion)
	{
		return IsSql14OrLater(SqlServerVersionInternal.Version140, currentServerVersion);
	}

	internal static bool IsTargetServerVersionSQl14OrLater(SqlServerVersionInternal targetServerVersion)
	{
		return targetServerVersion >= SqlServerVersionInternal.Version140;
	}

	internal static bool IsSql13OrLater(SqlServerVersionInternal targetServerVersion, ServerVersion currentServerVersion)
	{
		if (targetServerVersion >= SqlServerVersionInternal.Version130)
		{
			return currentServerVersion.Major >= 13;
		}
		return false;
	}

	public static bool IsSql13OrLater(ServerVersion currentServerVersion)
	{
		return IsSql13OrLater(SqlServerVersionInternal.Version130, currentServerVersion);
	}

	internal static bool IsTargetServerVersionSQl13OrLater(SqlServerVersionInternal targetServerVersion)
	{
		return targetServerVersion >= SqlServerVersionInternal.Version130;
	}

	internal static bool IsSql12OrLater(SqlServerVersionInternal targetServerVersion, ServerVersion currentServerVersion)
	{
		if (targetServerVersion >= SqlServerVersionInternal.Version120)
		{
			return currentServerVersion.Major >= 12;
		}
		return false;
	}

	public static bool IsSql12OrLater(ServerVersion currentServerVersion)
	{
		return IsSql12OrLater(SqlServerVersionInternal.Version120, currentServerVersion);
	}

	internal static bool IsTargetServerVersionSQl12OrLater(SqlServerVersionInternal targetServerVersion)
	{
		return targetServerVersion >= SqlServerVersionInternal.Version120;
	}

	internal static bool IsSql11OrLater(SqlServerVersionInternal targetServerVersion, ServerVersion currentServerVersion)
	{
		if (targetServerVersion >= SqlServerVersionInternal.Version110)
		{
			return currentServerVersion.Major >= 11;
		}
		return false;
	}

	public static bool IsSql11OrLater(ServerVersion currentServerVersion)
	{
		return IsSql11OrLater(SqlServerVersionInternal.Version110, currentServerVersion);
	}

	internal static bool IsTargetServerVersionSQl11OrLater(SqlServerVersionInternal targetServerVersion)
	{
		return targetServerVersion >= SqlServerVersionInternal.Version110;
	}

	internal static bool IsSql13Azure12OrLater(DatabaseEngineType currentDatabaseEngineType, ServerVersion currentServerVersion, ScriptingPreferences sp)
	{
		if (sp != null)
		{
			if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (!IsTargetServerVersionSQl12OrLater(sp.TargetServerVersionInternal))
				{
					return false;
				}
			}
			else if (!IsTargetServerVersionSQl13OrLater(sp.TargetServerVersionInternal))
			{
				return false;
			}
		}
		if (currentDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return IsSql12OrLater(currentServerVersion);
		}
		return IsSql13OrLater(currentServerVersion);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool IsSql13Azure12OrLater(DatabaseEngineType currentDatabaseEngineType, ServerVersion currentServerVersion)
	{
		return IsSql13Azure12OrLater(currentDatabaseEngineType, currentServerVersion, null);
	}
}

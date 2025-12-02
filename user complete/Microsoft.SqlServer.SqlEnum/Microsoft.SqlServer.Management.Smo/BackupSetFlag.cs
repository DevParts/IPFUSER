namespace Microsoft.SqlServer.Management.Smo;

public enum BackupSetFlag
{
	MinimalLogData = 1,
	WithSnapshot = 2,
	ReadOnlyDatabase = 4,
	SingleUserModeDatabase = 8
}

namespace Microsoft.SqlServer.Management.Smo;

public enum BackupSetType
{
	Database = 1,
	Differential = 2,
	Incremental = 2,
	Log = 3,
	FileOrFileGroup = 4,
	FileOrFileGroupDifferential = 5
}

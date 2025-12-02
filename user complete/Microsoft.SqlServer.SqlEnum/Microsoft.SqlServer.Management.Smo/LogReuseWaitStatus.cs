namespace Microsoft.SqlServer.Management.Smo;

public enum LogReuseWaitStatus
{
	Nothing,
	Checkpoint,
	LogBackup,
	BackupOrRestore,
	Transaction,
	Mirroring,
	Replication,
	SnapshotCreation,
	LogScan,
	Other
}

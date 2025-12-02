namespace Microsoft.SqlServer.Management.Smo;

public enum CatalogPopulationStatus
{
	Idle,
	CrawlinProgress,
	Paused,
	Throttled,
	Recovering,
	Shutdown,
	Incremental,
	UpdatingIndex,
	DiskFullPause,
	Notification
}

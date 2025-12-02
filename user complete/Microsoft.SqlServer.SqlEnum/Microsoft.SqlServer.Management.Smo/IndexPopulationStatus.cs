namespace Microsoft.SqlServer.Management.Smo;

public enum IndexPopulationStatus
{
	None,
	Full,
	Incremental,
	Manual,
	Background,
	PausedOrThrottled
}

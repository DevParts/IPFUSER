namespace Microsoft.SqlServer.Management.Smo;

public enum HadrManagerStatus
{
	[LocDisplayName("hmsPendingCommunication")]
	PendingCommunication,
	[LocDisplayName("hmsRunning")]
	Running,
	[LocDisplayName("hmsFailed")]
	Failed
}

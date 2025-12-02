namespace Microsoft.SqlServer.Management.Smo;

public enum InstanceState
{
	Unknown = 0,
	Online = 1,
	OnlinePending = 3,
	Offline = 16,
	OfflinePending = 48
}

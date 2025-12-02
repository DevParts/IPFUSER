namespace Microsoft.SqlServer.Management.Smo;

public enum RemoteDataArchiveMigrationState
{
	Disabled,
	PausedOutbound,
	PausedInbound,
	Outbound,
	Inbound,
	Paused
}

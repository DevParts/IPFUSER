using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(MirroringStatusConverter))]
public enum MirroringStatus
{
	[LocDisplayName("msNone")]
	None,
	[LocDisplayName("msSuspended")]
	Suspended,
	[LocDisplayName("msDisconnected")]
	Disconnected,
	[LocDisplayName("msSynchronizing")]
	Synchronizing,
	[LocDisplayName("msPendingFailover")]
	PendingFailover,
	[LocDisplayName("msSynchronized")]
	Synchronized
}

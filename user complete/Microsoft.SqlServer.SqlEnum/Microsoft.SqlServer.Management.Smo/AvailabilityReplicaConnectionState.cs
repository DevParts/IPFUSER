using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaConnectionStateConverter))]
public enum AvailabilityReplicaConnectionState
{
	[LocDisplayName("arcsDisconnected")]
	Disconnected,
	[LocDisplayName("arcsConnected")]
	Connected,
	[LocDisplayName("Unknown")]
	Unknown
}

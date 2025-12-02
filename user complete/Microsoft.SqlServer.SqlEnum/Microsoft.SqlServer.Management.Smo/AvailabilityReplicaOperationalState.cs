using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaOperationalStateConverter))]
public enum AvailabilityReplicaOperationalState
{
	[LocDisplayName("arosPendingFailover")]
	PendingFailover,
	[LocDisplayName("arosPending")]
	Pending,
	[LocDisplayName("arosOnline")]
	Online,
	[LocDisplayName("arosOffline")]
	Offline,
	[LocDisplayName("arosFailed")]
	Failed,
	[LocDisplayName("arosFailedNoQuorum")]
	FailedNoQuorum,
	[LocDisplayName("Unknown")]
	Unknown
}

using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaRollupRecoveryStateConverter))]
public enum AvailabilityReplicaRollupRecoveryState
{
	[LocDisplayName("arrhInProgress")]
	InProgress,
	[LocDisplayName("arrhOnline")]
	Online,
	[LocDisplayName("Unknown")]
	Unknown
}

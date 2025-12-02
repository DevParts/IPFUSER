using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaRollupSynchronizationStateConverter))]
public enum AvailabilityReplicaRollupSynchronizationState
{
	[LocDisplayName("arshNotSynchronizing")]
	NotSynchronizing,
	[LocDisplayName("arshSynchronizing")]
	Synchronizing,
	[LocDisplayName("arshSynchronized")]
	Synchronized,
	[LocDisplayName("Unknown")]
	Unknown
}

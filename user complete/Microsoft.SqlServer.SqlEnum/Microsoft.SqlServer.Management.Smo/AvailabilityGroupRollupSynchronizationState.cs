using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityGroupRollupSynchronizationStateConverter))]
public enum AvailabilityGroupRollupSynchronizationState
{
	[LocDisplayName("agshNoneSynchronizing")]
	NoneSynchronizing,
	[LocDisplayName("agshPartiallySynchronizing")]
	PartiallySynchronizing,
	[LocDisplayName("agshAllSynchronizing")]
	AllSynchronizing,
	[LocDisplayName("agshAllSynchronized")]
	AllSynchronized,
	[LocDisplayName("Unknown")]
	Unknown
}

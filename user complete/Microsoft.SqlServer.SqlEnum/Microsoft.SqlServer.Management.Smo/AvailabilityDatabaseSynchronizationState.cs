using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityDatabaseSynchronizationStateConverter))]
public enum AvailabilityDatabaseSynchronizationState
{
	[LocDisplayName("adssNotSynchronizing")]
	NotSynchronizing,
	[LocDisplayName("adssSynchronizing")]
	Synchronizing,
	[LocDisplayName("adssSynchronized")]
	Synchronized,
	[LocDisplayName("adssReverting")]
	Reverting,
	[LocDisplayName("adssInitializing")]
	Initializing
}

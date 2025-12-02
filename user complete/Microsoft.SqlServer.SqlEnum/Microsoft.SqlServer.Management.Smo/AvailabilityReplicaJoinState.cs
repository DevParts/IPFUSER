using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaJoinStateConverter))]
public enum AvailabilityReplicaJoinState
{
	[LocDisplayName("arjsNotJoined")]
	NotJoined = 0,
	[LocDisplayName("arjsJoinedStandaloneInstance")]
	JoinedStandaloneInstance = 1,
	[LocDisplayName("arjsJoinedFailoverClusterInstance")]
	JoinedFailoverClusterInstance = 2,
	[LocDisplayName("Unknown")]
	Unknown = 99
}

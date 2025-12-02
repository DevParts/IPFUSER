using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaRoleConverter))]
public enum AvailabilityReplicaRole
{
	[LocDisplayName("arrResolving")]
	Resolving,
	[LocDisplayName("arrPrimary")]
	Primary,
	[LocDisplayName("arrSecondary")]
	Secondary,
	[LocDisplayName("Unknown")]
	Unknown
}

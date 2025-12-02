using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaConnectionModeInSecondaryRoleConverter))]
public enum AvailabilityReplicaConnectionModeInSecondaryRole
{
	[TsqlSyntaxString("NO")]
	[LocDisplayName("cmsrNoConnections")]
	AllowNoConnections,
	[LocDisplayName("cmsrReadIntentConnectionsOnly")]
	[TsqlSyntaxString("READ_ONLY")]
	AllowReadIntentConnectionsOnly,
	[LocDisplayName("cmsrAllConnections")]
	[TsqlSyntaxString("ALL")]
	AllowAllConnections,
	[LocDisplayName("Unknown")]
	[Browsable(false)]
	Unknown
}

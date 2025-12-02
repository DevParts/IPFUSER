using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaFailoverModeConverter))]
public enum AvailabilityReplicaFailoverMode
{
	[LocDisplayName("arfmAutomatic")]
	[TsqlSyntaxString("AUTOMATIC")]
	Automatic,
	[TsqlSyntaxString("MANUAL")]
	[LocDisplayName("arfmManual")]
	Manual,
	[LocDisplayName("arfmExternal")]
	[TsqlSyntaxString("EXTERNAL")]
	External,
	[Browsable(false)]
	[LocDisplayName("Unknown")]
	Unknown
}

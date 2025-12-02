using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityReplicaSeedingModeConverter))]
public enum AvailabilityReplicaSeedingMode
{
	[LocDisplayName("seedingModeAutomatic")]
	[TsqlSyntaxString("AUTOMATIC")]
	Automatic,
	[TsqlSyntaxString("MANUAL")]
	[LocDisplayName("seedingModeManual")]
	Manual
}

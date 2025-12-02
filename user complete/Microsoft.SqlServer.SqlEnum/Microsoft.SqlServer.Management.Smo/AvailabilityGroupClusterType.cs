using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AvailabilityGroupClusterTypeConverter))]
public enum AvailabilityGroupClusterType
{
	[LocDisplayName("agctWsfc")]
	Wsfc,
	[LocDisplayName("agctNone")]
	None,
	[LocDisplayName("agctExternal")]
	External
}

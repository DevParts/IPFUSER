using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(QueryStoreSizeBasedCleanupModeConverter))]
public enum QueryStoreSizeBasedCleanupMode
{
	[LocDisplayName("Off")]
	Off,
	[LocDisplayName("Auto")]
	Auto
}

using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(QueryStoreCaptureModeConverter))]
public enum QueryStoreCaptureMode
{
	[LocDisplayName("All")]
	All = 1,
	[LocDisplayName("Auto")]
	Auto,
	[LocDisplayName("None")]
	None
}

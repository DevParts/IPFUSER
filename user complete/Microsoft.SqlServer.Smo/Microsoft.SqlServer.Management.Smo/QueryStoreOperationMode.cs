using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(QueryStoreOperationModeConverter))]
public enum QueryStoreOperationMode
{
	[LocDisplayName("Off")]
	Off,
	[LocDisplayName("ReadOnly")]
	ReadOnly,
	[LocDisplayName("ReadWrite")]
	ReadWrite,
	[LocDisplayName("Error")]
	Error
}

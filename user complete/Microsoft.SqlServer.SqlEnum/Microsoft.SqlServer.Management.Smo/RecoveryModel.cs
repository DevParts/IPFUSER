using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(RecoveryModelConverter))]
public enum RecoveryModel
{
	[LocDisplayName("rmSimple")]
	Simple = 3,
	[LocDisplayName("rmBulkLogged")]
	BulkLogged = 2,
	[LocDisplayName("rmFull")]
	Full = 1
}

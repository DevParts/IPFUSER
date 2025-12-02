using System.ComponentModel;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[TypeConverter(typeof(LocalizableEnumConverter))]
public enum SqlServerVersion
{
	[DisplayNameKey("ServerShiloh")]
	Version80 = 1,
	[DisplayNameKey("ServerYukon")]
	Version90,
	[DisplayNameKey("ServerKatmai")]
	Version100,
	[DisplayNameKey("ServerKilimanjaro")]
	Version105,
	[DisplayNameKey("ServerDenali")]
	Version110,
	[DisplayNameKey("ServerSQL14")]
	Version120,
	[DisplayNameKey("ServerSQL15")]
	Version130,
	[DisplayNameKey("ServerSQL2017")]
	Version140,
	[DisplayNameKey("ServerSQLv150")]
	Version150
}

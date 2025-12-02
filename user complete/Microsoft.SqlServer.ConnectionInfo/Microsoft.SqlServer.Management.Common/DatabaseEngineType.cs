using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Common;

[TypeConverter(typeof(CommonLocalizableEnumConverter))]
[CommonLocalizedPropertyResources("Microsoft.SqlServer.Management.Common.StringConnectionInfo")]
public enum DatabaseEngineType
{
	Unknown,
	[CommonDisplayNameKey("Standalone")]
	Standalone,
	[CommonDisplayNameKey("SqlAzureDatabase")]
	SqlAzureDatabase
}

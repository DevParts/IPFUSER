using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Common;

[TypeConverter(typeof(CommonLocalizableEnumConverter))]
[CommonLocalizedPropertyResources("Microsoft.SqlServer.Management.Common.StringConnectionInfo")]
public enum DatabaseEngineEdition
{
	Unknown,
	[CommonDisplayNameKey("PersonalEdition")]
	Personal,
	[CommonDisplayNameKey("StandardEdition")]
	Standard,
	[CommonDisplayNameKey("EnterpriseEdition")]
	Enterprise,
	[CommonDisplayNameKey("ExpressEdition")]
	Express,
	[CommonDisplayNameKey("SqlAzureDatabaseEdition")]
	SqlDatabase,
	[CommonDisplayNameKey("SqlDataWarehouseEdition")]
	SqlDataWarehouse,
	[CommonDisplayNameKey("StretchEdition")]
	SqlStretchDatabase,
	[CommonDisplayNameKey("SqlManagedInstanceEdition")]
	SqlManagedInstance
}

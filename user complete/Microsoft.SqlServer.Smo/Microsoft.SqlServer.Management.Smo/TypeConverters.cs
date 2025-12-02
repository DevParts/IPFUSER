using System.ComponentModel;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

public static class TypeConverters
{
	public static readonly TypeConverter SqlServerVersionTypeConverter = SmoManagementUtil.GetTypeConverter(typeof(SqlServerVersion));
}

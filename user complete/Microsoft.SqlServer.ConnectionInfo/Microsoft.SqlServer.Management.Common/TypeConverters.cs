using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Common;

public static class TypeConverters
{
	public static readonly TypeConverter DatabaseEngineEditionTypeConverter = TypeDescriptor.GetConverter(typeof(DatabaseEngineEdition));

	public static readonly TypeConverter DatabaseEngineTypeTypeConverter = TypeDescriptor.GetConverter(typeof(DatabaseEngineType));
}

using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(ContainmentTypeConverter))]
public enum ContainmentType
{
	[LocDisplayName("ctNone")]
	None,
	[LocDisplayName("ctPartial")]
	Partial
}

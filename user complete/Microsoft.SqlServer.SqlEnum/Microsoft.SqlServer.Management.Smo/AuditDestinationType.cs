using System.ComponentModel;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(AuditDestinationTypeConverter))]
public enum AuditDestinationType
{
	[TsqlSyntaxString("File")]
	[LocDisplayName("FileDest")]
	File = 0,
	[LocDisplayName("SecurityLogDest")]
	[TsqlSyntaxString("SecurityLog")]
	SecurityLog = 1,
	[TsqlSyntaxString("ApplicationLog")]
	[LocDisplayName("ApplicationLogDest")]
	ApplicationLog = 2,
	[LocDisplayName("UrlDest")]
	[TsqlSyntaxString("Url")]
	Url = 3,
	[TsqlSyntaxString("Unknown")]
	[LocDisplayName("UnknownDest")]
	Unknown = 100
}

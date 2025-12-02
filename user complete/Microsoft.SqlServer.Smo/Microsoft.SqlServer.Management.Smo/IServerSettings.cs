using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(LocalizableTypeConverter))]
[RootFacet(typeof(Server))]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[CLSCompliant(false)]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[DisplayNameKey("IServerSettings_Name")]
[DisplayDescriptionKey("IServerSettings_Desc")]
public interface IServerSettings : IDmfFacet
{
	[DisplayDescriptionKey("Server_AuditLevelDesc")]
	[DisplayNameKey("Server_AuditLevelName")]
	AuditLevel AuditLevel { get; set; }

	[DisplayNameKey("Server_BackupDirectoryName")]
	[DisplayDescriptionKey("Server_BackupDirectoryDesc")]
	string BackupDirectory { get; set; }

	[DisplayDescriptionKey("Server_DefaultFileDesc")]
	[DisplayNameKey("Server_DefaultFileName")]
	string DefaultFile { get; set; }

	[DisplayNameKey("Server_DefaultLogName")]
	[DisplayDescriptionKey("Server_DefaultLogDesc")]
	string DefaultLog { get; set; }

	[DisplayNameKey("Server_LoginModeName")]
	[DisplayDescriptionKey("Server_LoginModeDesc")]
	ServerLoginMode LoginMode { get; }

	[DisplayNameKey("Server_MailProfileName")]
	[DisplayDescriptionKey("Server_MailProfileDesc")]
	string MailProfile { get; set; }

	[DisplayDescriptionKey("Server_NumberOfLogFilesDesc")]
	[DisplayNameKey("Server_NumberOfLogFilesName")]
	int NumberOfLogFiles { get; set; }

	[DisplayNameKey("Server_PerfMonModeName")]
	[DisplayDescriptionKey("Server_PerfMonModeDesc")]
	PerfMonMode PerfMonMode { get; set; }

	[DisplayNameKey("Server_TapeLoadWaitTimeName")]
	[DisplayDescriptionKey("Server_TapeLoadWaitTimeDesc")]
	int TapeLoadWaitTime { get; set; }
}

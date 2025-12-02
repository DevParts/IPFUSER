using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[DisplayNameKey("IServerSetupFacet_Name")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[DisplayDescriptionKey("IServerSetupFacet_Desc")]
public interface IServerSetupFacet : IDmfFacet
{
	[DisplayDescriptionKey("Server_ServiceNameDesc")]
	[DisplayNameKey("Server_ServiceNameName")]
	string ServiceName { get; }

	[DisplayDescriptionKey("Server_ServiceAccountDesc")]
	[DisplayNameKey("Server_ServiceAccountName")]
	string EngineServiceAccount { get; }

	[DisplayNameKey("Server_ServiceStartModeName")]
	[DisplayDescriptionKey("Server_ServiceStartModeDesc")]
	ServiceStartMode ServiceStartMode { get; }

	[DisplayDescriptionKey("Server_InstanceNameDesc")]
	[DisplayNameKey("Server_InstanceNameName")]
	string InstanceName { get; }

	[DisplayNameKey("IServerSetupFacet_ServiceInstanceIdSuffixName")]
	[DisplayDescriptionKey("IServerSetupFacet_ServiceInstanceIdSuffixDesc")]
	string ServiceInstanceIdSuffix { get; }

	[DisplayDescriptionKey("Server_FilestreamLevelDesc")]
	[DisplayNameKey("Server_FilestreamLevelName")]
	FileStreamEffectiveLevel FilestreamLevel { get; }

	[DisplayNameKey("Server_FilestreamShareNameName")]
	[DisplayDescriptionKey("Server_FilestreamShareNameDesc")]
	string FilestreamShareName { get; }

	[DisplayDescriptionKey("IServerConfigurationFacet_UserInstancesEnabledDesc")]
	[DisplayNameKey("IServerConfigurationFacet_UserInstancesEnabledName")]
	bool UserInstancesEnabled { get; }

	[DisplayNameKey("Server_CollationName")]
	[DisplayDescriptionKey("Server_CollationDesc")]
	string Collation { get; }

	[DisplayNameKey("Server_SqlDomainGroupName")]
	[DisplayDescriptionKey("Server_SqlDomainGroupDesc")]
	string SqlDomainGroup { get; }

	[DisplayDescriptionKey("IServerSetupFacet_WindowsUsersAndGroupsInSysadminRoleDesc")]
	[DisplayNameKey("IServerSetupFacet_WindowsUsersAndGroupsInSysadminRoleName")]
	string[] WindowsUsersAndGroupsInSysadminRole { get; }

	[DisplayDescriptionKey("Server_LoginModeDesc")]
	[DisplayNameKey("Server_LoginModeName")]
	ServerLoginMode LoginMode { get; }

	[DisplayDescriptionKey("Server_InstallDataDirectoryDesc")]
	[DisplayNameKey("Server_InstallDataDirectoryName")]
	string InstallDataDirectory { get; }

	[DisplayNameKey("Server_BackupDirectoryName")]
	[DisplayDescriptionKey("Server_BackupDirectoryDesc")]
	string BackupDirectory { get; }

	[DisplayNameKey("Server_DefaultFileName")]
	[DisplayDescriptionKey("Server_DefaultFileDesc")]
	string DefaultFile { get; }

	[DisplayNameKey("Server_DefaultLogName")]
	[DisplayDescriptionKey("Server_DefaultLogDesc")]
	string DefaultLog { get; }

	[DisplayDescriptionKey("IServerSetupFacet_TempdbPrimaryFilePathDesc")]
	[DisplayNameKey("IServerSetupFacet_TempdbPrimaryFilePathName")]
	string TempdbPrimaryFilePath { get; }

	[DisplayDescriptionKey("IServerSetupFacet_TempdbLogPathDesc")]
	[DisplayNameKey("IServerSetupFacet_TempdbLogPathName")]
	string TempdbLogPath { get; }

	[DisplayNameKey("JobServer_ServiceStartModeName")]
	[DisplayDescriptionKey("JobServer_ServiceStartModeDesc")]
	ServiceStartMode AgentStartMode { get; }

	[DisplayNameKey("JobServer_ServiceAccountName")]
	[DisplayDescriptionKey("JobServer_ServiceAccountDesc")]
	string AgentServiceAccount { get; }

	[DisplayNameKey("JobServer_AgentDomainGroupName")]
	[DisplayDescriptionKey("JobServer_AgentDomainGroupDesc")]
	string AgentDomainGroup { get; }

	[DisplayDescriptionKey("Server_NamedPipesEnabledDesc")]
	[DisplayNameKey("Server_NamedPipesEnabledName")]
	bool NamedPipesEnabled { get; }

	[DisplayDescriptionKey("Server_TcpEnabledDesc")]
	[DisplayNameKey("Server_TcpEnabledName")]
	bool TcpEnabled { get; }

	[DisplayDescriptionKey("Server_InstallSharedDirectoryDesc")]
	[DisplayNameKey("Server_InstallSharedDirectoryName")]
	string InstallSharedDirectory { get; }

	[DisplayNameKey("Server_BrowserStartModeName")]
	[DisplayDescriptionKey("Server_InstallSharedDirectoryDesc")]
	ServiceStartMode BrowserStartMode { get; }

	[DisplayDescriptionKey("Server_BrowserServiceAccountDesc")]
	[DisplayNameKey("Server_BrowserServiceAccountName")]
	string BrowserServiceAccount { get; }
}

using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[RootFacet(typeof(Server))]
[CLSCompliant(false)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[DisplayNameKey("IServerInformation_Name")]
[DisplayDescriptionKey("IServerInformation_Desc")]
public interface IServerInformation : IDmfFacet
{
	[DisplayNameKey("Server_CollationName")]
	[DisplayDescriptionKey("Server_CollationDesc")]
	string Collation { get; }

	[DisplayNameKey("Server_EditionName")]
	[DisplayDescriptionKey("Server_EditionDesc")]
	string Edition { get; }

	[DisplayNameKey("Server_Name")]
	[DisplayDescriptionKey("Server_Desc")]
	string ErrorLogPath { get; }

	[DisplayDescriptionKey("Server_IsCaseSensitiveDesc")]
	[DisplayNameKey("Server_IsCaseSensitiveName")]
	bool IsCaseSensitive { get; }

	[DisplayNameKey("Server_IsClusteredName")]
	[DisplayDescriptionKey("Server_IsClusteredDesc")]
	bool IsClustered { get; }

	[DisplayNameKey("Server_IsFullTextInstalledName")]
	[DisplayDescriptionKey("Server_IsFullTextInstalledDesc")]
	bool IsFullTextInstalled { get; }

	[DisplayDescriptionKey("Server_IsPolyBaseInstalledDesc")]
	[DisplayNameKey("Server_IsPolyBaseInstalledName")]
	bool IsPolyBaseInstalled { get; }

	[DisplayNameKey("Server_IsSingleUserName")]
	[DisplayDescriptionKey("Server_IsSingleUserDesc")]
	bool IsSingleUser { get; }

	[DisplayNameKey("Server_LanguageName")]
	[DisplayDescriptionKey("Server_LanguageDesc")]
	string Language { get; }

	[DisplayDescriptionKey("Server_MasterDBLogPathDesc")]
	[DisplayNameKey("Server_MasterDBLogPathName")]
	string MasterDBLogPath { get; }

	[DisplayNameKey("Server_MasterDBPathName")]
	[DisplayDescriptionKey("Server_MasterDBPathDesc")]
	string MasterDBPath { get; }

	[DisplayNameKey("Server_MaxPrecisionName")]
	[DisplayDescriptionKey("Server_MaxPrecisionDesc")]
	byte MaxPrecision { get; }

	[DisplayDescriptionKey("Server_NetNameDesc")]
	[DisplayNameKey("Server_NetNameName")]
	string NetName { get; }

	[DisplayDescriptionKey("Server_OSVersionDesc")]
	[DisplayNameKey("Server_OSVersionName")]
	string OSVersion { get; }

	[DisplayDescriptionKey("Server_PhysicalMemoryDesc")]
	[DisplayNameKey("Server_PhysicalMemoryName")]
	int PhysicalMemory { get; }

	[DisplayNameKey("Server_PlatformName")]
	[DisplayDescriptionKey("Server_PlatformDesc")]
	string Platform { get; }

	[DisplayDescriptionKey("Server_ProcessorsDesc")]
	[DisplayNameKey("Server_ProcessorsName")]
	int Processors { get; }

	[DisplayDescriptionKey("Server_ProductDesc")]
	[DisplayNameKey("Server_ProductName")]
	string Product { get; }

	[DisplayDescriptionKey("Server_ProductLevelDesc")]
	[DisplayNameKey("Server_ProductLevelName")]
	string ProductLevel { get; }

	[DisplayDescriptionKey("Server_RootDirectoryDesc")]
	[DisplayNameKey("Server_RootDirectoryName")]
	string RootDirectory { get; }

	[DisplayNameKey("Server_VersionStringName")]
	[DisplayDescriptionKey("Server_VersionStringDesc")]
	string VersionString { get; }

	[DisplayNameKey("Server_EngineEditionName")]
	[DisplayDescriptionKey("Server_EngineEditionDesc")]
	Edition EngineEdition { get; }

	[DisplayDescriptionKey("Server_VersionMajorDesc")]
	[DisplayNameKey("Server_VersionMajorName")]
	int VersionMajor { get; }

	[DisplayNameKey("Server_VersionMinorName")]
	[DisplayDescriptionKey("Server_VersionMinorDesc")]
	int VersionMinor { get; }

	[DisplayDescriptionKey("Server_BuildClrVersionStringDesc")]
	[DisplayNameKey("Server_BuildClrVersionStringName")]
	string BuildClrVersionString { get; }

	[DisplayDescriptionKey("Server_BuildNumberDesc")]
	[DisplayNameKey("Server_BuildNumberName")]
	int BuildNumber { get; }

	[DisplayNameKey("Server_CollationIDName")]
	[DisplayDescriptionKey("Server_CollationIDDesc")]
	int CollationID { get; }

	[DisplayNameKey("Server_ComparisonStyleName")]
	[DisplayDescriptionKey("Server_ComparisonStyleDesc")]
	int ComparisonStyle { get; }

	[DisplayDescriptionKey("Server_ComputerNamePhysicalNetBIOSDesc")]
	[DisplayNameKey("Server_ComputerNamePhysicalNetBIOSName")]
	string ComputerNamePhysicalNetBIOS { get; }

	[DisplayDescriptionKey("Server_ResourceLastUpdateDateTimeDesc")]
	[DisplayNameKey("Server_ResourceLastUpdateDateTimeName")]
	DateTime ResourceLastUpdateDateTime { get; }

	[DisplayDescriptionKey("Server_ResourceVersionStringDesc")]
	[DisplayNameKey("Server_ResourceVersionStringName")]
	string ResourceVersionString { get; }

	[DisplayDescriptionKey("Server_SqlCharSetDesc")]
	[DisplayNameKey("Server_SqlCharSetName")]
	short SqlCharSet { get; }

	[DisplayNameKey("Server_SqlCharSetNameName")]
	[DisplayDescriptionKey("Server_SqlCharSetNameDesc")]
	string SqlCharSetName { get; }

	[DisplayNameKey("Server_SqlSortOrderName")]
	[DisplayDescriptionKey("Server_SqlSortOrderDesc")]
	short SqlSortOrder { get; }

	[DisplayDescriptionKey("Server_SqlSortOrderNameDesc")]
	[DisplayNameKey("Server_SqlSortOrderNameName")]
	string SqlSortOrderName { get; }

	[DisplayNameKey("Server_IsHadrEnabledName")]
	[DisplayDescriptionKey("Server_IsHadrEnabledDesc")]
	bool IsHadrEnabled { get; }

	[DisplayNameKey("Server_IsXTPSupported")]
	[DisplayDescriptionKey("Server_IsXTPSupportedDesc")]
	bool IsXTPSupported { get; }
}

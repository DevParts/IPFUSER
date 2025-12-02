using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayNameKey("IServerSelection_Name")]
[DisplayDescriptionKey("IServerSelection_Desc")]
[CLSCompliant(false)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
public interface IServerSelectionFacet : IDmfFacet
{
	[DisplayDescriptionKey("Server_BuildNumberDesc")]
	[DisplayNameKey("Server_BuildNumberName")]
	int BuildNumber { get; }

	[DisplayDescriptionKey("Server_CollationDesc")]
	[DisplayNameKey("Server_CollationName")]
	string Collation { get; }

	[DisplayNameKey("Server_EditionName")]
	[DisplayDescriptionKey("Server_EditionDesc")]
	string Edition { get; }

	[DisplayDescriptionKey("Server_IsCaseSensitiveDesc")]
	[DisplayNameKey("Server_IsCaseSensitiveName")]
	bool IsCaseSensitive { get; }

	[DisplayDescriptionKey("Server_LanguageDesc")]
	[DisplayNameKey("Server_LanguageName")]
	string Language { get; }

	[DisplayDescriptionKey("Server_NamedPipesEnabledDesc")]
	[DisplayNameKey("Server_NamedPipesEnabledName")]
	bool NamedPipesEnabled { get; }

	[DisplayNameKey("Server_OSVersionName")]
	[DisplayDescriptionKey("Server_OSVersionDesc")]
	string OSVersion { get; }

	[DisplayDescriptionKey("Server_PlatformDesc")]
	[DisplayNameKey("Server_PlatformName")]
	string Platform { get; }

	[DisplayNameKey("Server_TcpIpProtocolEnabledName")]
	[DisplayDescriptionKey("Server_TcpEnabledDesc")]
	bool TcpEnabled { get; }

	[DisplayNameKey("Server_VersionMajorName")]
	[DisplayDescriptionKey("Server_VersionMajorDesc")]
	int VersionMajor { get; }

	[DisplayNameKey("Server_VersionMinorName")]
	[DisplayDescriptionKey("Server_VersionMinorDesc")]
	int VersionMinor { get; }
}

using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayDescriptionKey("IDatabaseSecurityFacet_Desc")]
[DisplayNameKey("IDatabaseSecurityFacet_Name")]
[CLSCompliant(false)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
public interface IDatabaseSecurityFacet : IDmfFacet
{
	[DisplayDescriptionKey("Database_TrustworthyDesc")]
	[DisplayNameKey("Database_TrustworthyName")]
	bool Trustworthy { get; set; }

	[DisplayNameKey("IDatabaseSecurityFacet_IsOwnerSysadminName")]
	[DisplayDescriptionKey("IDatabaseSecurityFacet_IsOwnerSysadminDesc")]
	bool IsOwnerSysadmin { get; }
}

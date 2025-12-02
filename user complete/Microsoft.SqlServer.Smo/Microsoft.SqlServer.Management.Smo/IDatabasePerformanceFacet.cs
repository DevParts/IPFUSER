using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayNameKey("IDatabasePerformanceFacet_Name")]
[DisplayDescriptionKey("IDatabasePerformanceFacet_Desc")]
[CLSCompliant(false)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public interface IDatabasePerformanceFacet : IDmfFacet
{
	[DisplayDescriptionKey("Database_AutoCloseDesc")]
	[DisplayNameKey("Database_AutoCloseName")]
	bool AutoClose { get; set; }

	[DisplayDescriptionKey("Database_AutoShrinkDesc")]
	[DisplayNameKey("Database_AutoShrinkName")]
	bool AutoShrink { get; set; }

	[DisplayNameKey("Database_SizeName")]
	[DisplayDescriptionKey("Database_SizeDesc")]
	double Size { get; }

	[DisplayDescriptionKey("IDatabasePerformanceFacet_DataAndLogFilesOnSeparateLogicalVolumesDesc")]
	[DisplayNameKey("IDatabasePerformanceFacet_DataAndLogFilesOnSeparateLogicalVolumesName")]
	bool DataAndLogFilesOnSeparateLogicalVolumes { get; }

	[DisplayNameKey("IDatabasePerformanceFacet_CollationMatchesModelOrMasterName")]
	[DisplayDescriptionKey("IDatabasePerformanceFacet_CollationMatchesModelOrMasterDesc")]
	bool CollationMatchesModelOrMaster { get; }

	[DisplayNameKey("Database_IsSystemObjectName")]
	[DisplayDescriptionKey("Database_IsSystemObjectDesc")]
	bool IsSystemObject { get; }

	[DisplayDescriptionKey("Database_StatusDesc")]
	[DisplayNameKey("Database_StatusName")]
	DatabaseStatus Status { get; }
}

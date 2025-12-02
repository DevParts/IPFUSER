using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[DisplayNameKey("IDatabaseMaintenanceFacet_Name")]
[DisplayDescriptionKey("IDatabaseMaintenanceFacet_Desc")]
[CLSCompliant(false)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
public interface IDatabaseMaintenanceFacet : IDmfFacet
{
	[DisplayNameKey("Database_RecoveryModelName")]
	[DisplayDescriptionKey("Database_RecoveryModelDesc")]
	RecoveryModel RecoveryModel { get; set; }

	[DisplayNameKey("Database_ReadOnlyName")]
	[DisplayDescriptionKey("Database_ReadOnlyDesc")]
	bool ReadOnly { get; set; }

	[DisplayDescriptionKey("Database_PageVerifyDesc")]
	[DisplayNameKey("Database_PageVerifyName")]
	PageVerify PageVerify { get; set; }

	[DisplayDescriptionKey("Database_StatusDesc")]
	[DisplayNameKey("Database_StatusName")]
	DatabaseStatus Status { get; }

	[DisplayNameKey("Database_LastBackupDateName")]
	[DisplayDescriptionKey("Database_LastBackupDateDesc")]
	DateTime LastBackupDate { get; }

	[DisplayNameKey("Database_LastLogBackupDateName")]
	[DisplayDescriptionKey("Database_LastLogBackupDateDesc")]
	DateTime LastLogBackupDate { get; }

	[DisplayNameKey("IDatabaseMaintenanceFacet_DataAndBackupOnSeparateLogicalVolumesName")]
	[DisplayDescriptionKey("IDatabaseMaintenanceFacet_DataAndBackupOnSeparateLogicalVolumesDesc")]
	bool DataAndBackupOnSeparateLogicalVolumes { get; }

	[DisplayDescriptionKey("Database_TargetRecoveryTimeDesc")]
	[DisplayNameKey("Database_TargetRecoveryTimeName")]
	int TargetRecoveryTime { get; set; }

	[DisplayDescriptionKey("Database_DelayedDurabilityDesc")]
	[DisplayNameKey("Database_DelayedDurabilityName")]
	DelayedDurability DelayedDurability { get; set; }
}

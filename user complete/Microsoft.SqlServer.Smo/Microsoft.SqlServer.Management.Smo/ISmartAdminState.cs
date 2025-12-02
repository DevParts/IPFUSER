using System.ComponentModel;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[DisplayNameKey("SmartAdminStateName")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[DisplayDescriptionKey("SmartAdminStateDesc")]
[TypeConverter(typeof(LocalizableTypeConverter))]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.FacetSR")]
public interface ISmartAdminState : IDmfFacet, IRefreshable
{
	[DisplayDescriptionKey("SmartAdminState_IsMasterSwitchEnabledDesc")]
	[DisplayNameKey("SmartAdminState_IsMasterSwitchEnabledName")]
	bool IsMasterSwitchEnabled { get; }

	[DisplayNameKey("SmartAdminState_IsBackupEnabledName")]
	[DisplayDescriptionKey("SmartAdminState_IsBackupEnabledDesc")]
	bool IsBackupEnabled { get; }

	[DisplayDescriptionKey("SmartAdminState_NumberOfStorageConnectivityErrorsDesc")]
	[DisplayNameKey("SmartAdminState_NumberOfStorageConnectivityErrorsName")]
	int NumberOfStorageConnectivityErrors { get; }

	[DisplayDescriptionKey("SmartAdminState_NumberOfSqlErrorsDesc")]
	[DisplayNameKey("SmartAdminState_NumberOfSqlErrorsName")]
	int NumberOfSqlErrors { get; }

	[DisplayDescriptionKey("SmartAdminState_NumberOfInvalidCredentialErrorsDesc")]
	[DisplayNameKey("SmartAdminState_NumberOfInvalidCredentialErrorsName")]
	int NumberOfInvalidCredentialErrors { get; }

	[DisplayNameKey("SmartAdminState_NumberOfOtherErrorsName")]
	[DisplayDescriptionKey("SmartAdminState_NumberOfOtherErrorsDesc")]
	int NumberOfOtherErrors { get; }

	[DisplayNameKey("SmartAdminState_NumberOfCorruptedOrDeletedBackupsName")]
	[DisplayDescriptionKey("SmartAdminState_NumberOfCorruptedOrDeletedBackupsDesc")]
	int NumberOfCorruptedOrDeletedBackups { get; }

	[DisplayNameKey("SmartAdminState_NumberOfBackupLoopsName")]
	[DisplayDescriptionKey("SmartAdminState_NumberOfBackupLoopsDesc")]
	int NumberOfBackupLoops { get; }

	[DisplayNameKey("SmartAdminState_NumberOfRetentionLoopsName")]
	[DisplayDescriptionKey("SmartAdminState_NumberOfRetentionLoopsDesc")]
	int NumberOfRetentionLoops { get; }
}

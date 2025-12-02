using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[CLSCompliant(false)]
[TypeConverter(typeof(LocalizableTypeConverter))]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[LocalizedPropertyResources("Microsoft.SqlServer.Management.Smo.LocalizableResources")]
[DisplayDescriptionKey("IDatabaseOptions_Desc")]
[StateChangeEvent("CREATE_DATABASE", "DATABASE")]
[StateChangeEvent("ALTER_DATABASE", "DATABASE")]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "DATABASE")]
[DisplayNameKey("IDatabaseOptions_Name")]
public interface IDatabaseOptions : IDmfFacet
{
	[DisplayDescriptionKey("Database_AnsiNullDefaultDesc")]
	[DisplayNameKey("Database_AnsiNullDefaultName")]
	bool AnsiNullDefault { get; set; }

	[DisplayDescriptionKey("Database_AnsiNullsEnabledDesc")]
	[DisplayNameKey("Database_AnsiNullsEnabledName")]
	bool AnsiNullsEnabled { get; set; }

	[DisplayDescriptionKey("Database_AnsiPaddingEnabledDesc")]
	[DisplayNameKey("Database_AnsiPaddingEnabledName")]
	bool AnsiPaddingEnabled { get; set; }

	[DisplayDescriptionKey("Database_AnsiWarningsEnabledDesc")]
	[DisplayNameKey("Database_AnsiWarningsEnabledName")]
	bool AnsiWarningsEnabled { get; set; }

	[DisplayDescriptionKey("Database_ArithmeticAbortEnabledDesc")]
	[DisplayNameKey("Database_ArithmeticAbortEnabledName")]
	bool ArithmeticAbortEnabled { get; set; }

	[DisplayNameKey("Database_AutoCloseName")]
	[DisplayDescriptionKey("Database_AutoCloseDesc")]
	bool AutoClose { get; set; }

	[DisplayDescriptionKey("Database_AutoCreateStatisticsEnabledDesc")]
	[DisplayNameKey("Database_AutoCreateStatisticsEnabledName")]
	bool AutoCreateStatisticsEnabled { get; set; }

	[DisplayNameKey("Database_AutoCreateIncrementalStatisticsEnabledName")]
	[DisplayDescriptionKey("Database_AutoCreateIncrementalStatisticsEnabledDesc")]
	bool AutoCreateIncrementalStatisticsEnabled { get; set; }

	[DisplayNameKey("Database_AutoShrinkName")]
	[DisplayDescriptionKey("Database_AutoShrinkDesc")]
	bool AutoShrink { get; set; }

	[DisplayDescriptionKey("Database_AutoUpdateStatisticsAsyncDesc")]
	[DisplayNameKey("Database_AutoUpdateStatisticsAsyncName")]
	bool AutoUpdateStatisticsAsync { get; set; }

	[DisplayNameKey("Database_AutoUpdateStatisticsEnabledName")]
	[DisplayDescriptionKey("Database_AutoUpdateStatisticsEnabledDesc")]
	bool AutoUpdateStatisticsEnabled { get; set; }

	[DisplayNameKey("Database_BrokerEnabledName")]
	[DisplayDescriptionKey("Database_BrokerEnabledDesc")]
	bool BrokerEnabled { get; set; }

	[DisplayDescriptionKey("Database_ChangeTrackingAutoCleanUpDesc")]
	[DisplayNameKey("Database_ChangeTrackingAutoCleanUpName")]
	bool ChangeTrackingAutoCleanUp { get; set; }

	[DisplayNameKey("Database_ChangeTrackingEnabledName")]
	[DisplayDescriptionKey("Database_ChangeTrackingEnabledDesc")]
	bool ChangeTrackingEnabled { get; set; }

	[DisplayDescriptionKey("Database_ChangeTrackingRetentionPeriodDesc")]
	[DisplayNameKey("Database_ChangeTrackingRetentionPeriodName")]
	int ChangeTrackingRetentionPeriod { get; set; }

	[DisplayDescriptionKey("Database_ChangeTrackingRetentionPeriodUnitsDesc")]
	[DisplayNameKey("Database_ChangeTrackingRetentionPeriodUnitsName")]
	RetentionPeriodUnits ChangeTrackingRetentionPeriodUnits { get; set; }

	[DisplayDescriptionKey("Database_CloseCursorsOnCommitEnabledDesc")]
	[DisplayNameKey("Database_CloseCursorsOnCommitEnabledName")]
	bool CloseCursorsOnCommitEnabled { get; set; }

	[DisplayNameKey("Database_CollationName")]
	[DisplayDescriptionKey("Database_CollationDesc")]
	string Collation { get; set; }

	[DisplayDescriptionKey("Database_CompatibilityLevelDesc")]
	[DisplayNameKey("Database_CompatibilityLevelName")]
	CompatibilityLevel CompatibilityLevel { get; set; }

	[DisplayNameKey("Database_ConcatenateNullYieldsNullName")]
	[DisplayDescriptionKey("Database_ConcatenateNullYieldsNullDesc")]
	bool ConcatenateNullYieldsNull { get; set; }

	[DisplayNameKey("Database_CreateDateName")]
	[DisplayDescriptionKey("Database_CreateDateDesc")]
	DateTime CreateDate { get; }

	[DisplayNameKey("Database_DatabaseOwnershipChainingName")]
	[DisplayDescriptionKey("Database_DatabaseOwnershipChainingDesc")]
	bool DatabaseOwnershipChaining { get; set; }

	[DisplayDescriptionKey("Database_DatabaseSnapshotBaseNameDesc")]
	[DisplayNameKey("Database_DatabaseSnapshotBaseNameName")]
	string DatabaseSnapshotBaseName { get; }

	[DisplayDescriptionKey("Database_DateCorrelationOptimizationDesc")]
	[DisplayNameKey("Database_DateCorrelationOptimizationName")]
	bool DateCorrelationOptimization { get; set; }

	[DisplayNameKey("Database_DefaultFileGroupName")]
	[DisplayDescriptionKey("Database_DefaultFileGroupDesc")]
	string DefaultFileGroup { get; }

	[DisplayNameKey("Database_DefaultFileStreamFileGroupName")]
	[DisplayDescriptionKey("Database_DefaultFileStreamFileGroupDesc")]
	string DefaultFileStreamFileGroup { get; }

	[DisplayDescriptionKey("Database_EncryptionEnabledDesc")]
	[DisplayNameKey("Database_EncryptionEnabledName")]
	bool EncryptionEnabled { get; set; }

	[DisplayDescriptionKey("Database_HonorBrokerPriorityDesc")]
	[DisplayNameKey("Database_HonorBrokerPriorityName")]
	bool HonorBrokerPriority { get; set; }

	[DisplayNameKey("Database_IDName")]
	[DisplayDescriptionKey("Database_IDDesc")]
	int ID { get; }

	[DisplayDescriptionKey("Database_IsParameterizationForcedDesc")]
	[DisplayNameKey("Database_IsParameterizationForcedName")]
	bool IsParameterizationForced { get; set; }

	[DisplayDescriptionKey("Database_IsReadCommittedSnapshotOnDesc")]
	[DisplayNameKey("Database_IsReadCommittedSnapshotOnName")]
	bool IsReadCommittedSnapshotOn { get; set; }

	[DisplayNameKey("Database_IsSystemObjectName")]
	[DisplayDescriptionKey("Database_IsSystemObjectDesc")]
	bool IsSystemObject { get; }

	[DisplayNameKey("Database_IsUpdateableName")]
	[DisplayDescriptionKey("Database_IsUpdateableDesc")]
	bool IsUpdateable { get; }

	[DisplayDescriptionKey("Database_LocalCursorsDefaultDesc")]
	[DisplayNameKey("Database_LocalCursorsDefaultName")]
	bool LocalCursorsDefault { get; set; }

	[DisplayNameKey("NamedSmoObject_NameName")]
	[DisplayDescriptionKey("NamedSmoObject_NameDesc")]
	string Name { get; }

	[DisplayDescriptionKey("Database_OwnerDesc")]
	[DisplayNameKey("Database_OwnerName")]
	string Owner { get; }

	[DisplayDescriptionKey("Database_NumericRoundAbortEnabledDesc")]
	[DisplayNameKey("Database_NumericRoundAbortEnabledName")]
	bool NumericRoundAbortEnabled { get; set; }

	[DisplayNameKey("Database_MirroringTimeoutName")]
	[DisplayDescriptionKey("Database_MirroringTimeoutDesc")]
	int MirroringTimeout { get; set; }

	[DisplayDescriptionKey("Database_PageVerifyDesc")]
	[DisplayNameKey("Database_PageVerifyName")]
	PageVerify PageVerify { get; set; }

	[DisplayDescriptionKey("Database_PrimaryFilePathDesc")]
	[DisplayNameKey("Database_PrimaryFilePathName")]
	string PrimaryFilePath { get; }

	[DisplayNameKey("Database_QuotedIdentifiersEnabledName")]
	[DisplayDescriptionKey("Database_QuotedIdentifiersEnabledDesc")]
	bool QuotedIdentifiersEnabled { get; set; }

	[DisplayDescriptionKey("Database_ReadOnlyDesc")]
	[DisplayNameKey("Database_ReadOnlyName")]
	bool ReadOnly { get; set; }

	[DisplayDescriptionKey("Database_RecoveryModelDesc")]
	[DisplayNameKey("Database_RecoveryModelName")]
	RecoveryModel RecoveryModel { get; set; }

	[DisplayDescriptionKey("Database_RecursiveTriggersEnabledDesc")]
	[DisplayNameKey("Database_RecursiveTriggersEnabledName")]
	bool RecursiveTriggersEnabled { get; set; }

	[DisplayNameKey("Database_RemoteDataArchiveEnabledName")]
	[DisplayDescriptionKey("Database_RemoteDataArchiveEnabledDesc")]
	bool RemoteDataArchiveEnabled { get; set; }

	[DisplayNameKey("Database_RemoteDataArchiveEndpointName")]
	[DisplayDescriptionKey("Database_RemoteDataArchiveEndpointDesc")]
	string RemoteDataArchiveEndpoint { get; set; }

	[DisplayNameKey("Database_RemoteDataArchiveLinkedServerName")]
	[DisplayDescriptionKey("Database_RemoteDataArchiveLinkedServerDesc")]
	string RemoteDataArchiveLinkedServer { get; }

	[DisplayDescriptionKey("Database_RemoteDatabaseNameDesc")]
	[DisplayNameKey("Database_RemoteDatabaseNameName")]
	string RemoteDatabaseName { get; }

	[DisplayDescriptionKey("Database_RemoteDataArchiveUseFederatedServiceAccountDesc")]
	[DisplayNameKey("Database_RemoteDataArchiveUseFederatedServiceAccount")]
	bool RemoteDataArchiveUseFederatedServiceAccount { get; }

	[DisplayNameKey("Database_RemoteDataArchiveCredentialName")]
	[DisplayDescriptionKey("Database_RemoteDataArchiveCredentialDesc")]
	string RemoteDataArchiveCredential { get; }

	[DisplayNameKey("Database_TrustworthyName")]
	[DisplayDescriptionKey("Database_TrustworthyDesc")]
	bool Trustworthy { get; set; }

	[DisplayDescriptionKey("Database_UserAccessDesc")]
	[DisplayNameKey("Database_UserAccessName")]
	DatabaseUserAccess UserAccess { get; set; }

	[DisplayDescriptionKey("Database_TargetRecoveryTimeDesc")]
	[DisplayNameKey("Database_TargetRecoveryTimeName")]
	int TargetRecoveryTime { get; set; }

	[DisplayDescriptionKey("Database_DelayedDurabilityDesc")]
	[DisplayNameKey("Database_DelayedDurabilityName")]
	DelayedDurability DelayedDurability { get; set; }
}

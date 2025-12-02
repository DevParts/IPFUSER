using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Broker;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
public sealed class Database : ScriptNameObjectBase, ISfcSupportsDesignMode, IPropertyDataDispatch, ICreatable, IAlterable, IDroppable, IDropIfExists, ISafeRenamable, IRenamable, IExtendedProperties, IScriptable, IDatabaseOptions, IDmfFacet
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 32, 58, 89, 98, 98, 112, 118, 132, 132, 133 };

		private static int[] cloudVersionCount = new int[3] { 56, 56, 84 };

		private static int sqlDwPropertyCount = 64;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[64]
		{
			new StaticMetadata("AnsiNullDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPaddingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarningsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ArithmeticAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateIncrementalStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoShrink", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsAsync", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AzureEdition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("AzureServiceObjective", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CaseSensitive", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("CloseCursorsOnCommitEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Collation", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("CompatibilityLevel", expensive: true, readOnly: false, typeof(CompatibilityLevel)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DatabaseOwnershipChaining", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseSnapshotBaseName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateCorrelationOptimization", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DboLogin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("DefaultFileGroup", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptionEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("HonorBrokerPriority", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsAccessible", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDatabaseSnapshot", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDatabaseSnapshotBase", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbAccessAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbBackupOperator", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDatareader", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDatawriter", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDdlAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDenyDatareader", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDenyDatawriter", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbManager", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbOwner", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbSecurityAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsLoginManager", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsMirroringEnabled", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsParameterizationForced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsReadCommittedSnapshotOn", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSqlDw", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUpdateable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LastGoodCheckDbTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("MaxSizeInBytes", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("NumericRoundAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifiersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RecursiveTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReplicationOptions", expensive: false, readOnly: true, typeof(ReplicationOptions)),
			new StaticMetadata("ServiceBrokerGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("Size", expensive: false, readOnly: true, typeof(double)),
			new StaticMetadata("SnapshotIsolationState", expensive: false, readOnly: true, typeof(SnapshotIsolationState)),
			new StaticMetadata("SpaceAvailable", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("Status", expensive: false, readOnly: true, typeof(DatabaseStatus)),
			new StaticMetadata("Trustworthy", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserAccess", expensive: false, readOnly: false, typeof(DatabaseUserAccess)),
			new StaticMetadata("UserName", expensive: true, readOnly: true, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[84]
		{
			new StaticMetadata("AnsiNullDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPaddingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarningsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ArithmeticAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoShrink", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsAsync", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AzureEdition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CaseSensitive", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("CloseCursorsOnCommitEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Collation", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("CompatibilityLevel", expensive: true, readOnly: false, typeof(CompatibilityLevel)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DatabaseOwnershipChaining", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseSnapshotBaseName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateCorrelationOptimization", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DboLogin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("DefaultSchema", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDatabaseSnapshot", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDatabaseSnapshotBase", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbAccessAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbBackupOperator", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDatareader", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDatawriter", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDdlAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDenyDatareader", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDenyDatawriter", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbManager", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbOwner", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbSecurityAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsLoginManager", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsParameterizationForced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsReadCommittedSnapshotOn", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSqlDw", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUpdateable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MaxSizeInBytes", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("NumericRoundAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifiersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RecursiveTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReplicationOptions", expensive: false, readOnly: true, typeof(ReplicationOptions)),
			new StaticMetadata("ServiceBrokerGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("Size", expensive: false, readOnly: true, typeof(double)),
			new StaticMetadata("SnapshotIsolationState", expensive: false, readOnly: true, typeof(SnapshotIsolationState)),
			new StaticMetadata("SpaceAvailable", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("Status", expensive: false, readOnly: true, typeof(DatabaseStatus)),
			new StaticMetadata("Trustworthy", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserAccess", expensive: false, readOnly: false, typeof(DatabaseUserAccess)),
			new StaticMetadata("UserName", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("AutoCreateIncrementalStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AzureServiceObjective", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CatalogCollation", expensive: false, readOnly: false, typeof(CatalogCollationType)),
			new StaticMetadata("ChangeTrackingAutoCleanUp", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ChangeTrackingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ChangeTrackingRetentionPeriod", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ChangeTrackingRetentionPeriodUnits", expensive: false, readOnly: false, typeof(RetentionPeriodUnits)),
			new StaticMetadata("DefaultFileGroup", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("DefaultFullTextCatalog", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptionEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("HasFileInCloud", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasMemoryOptimizedObjects", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HonorBrokerPriority", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsAccessible", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsMirroringEnabled", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSqlDwEdition", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LastGoodCheckDbTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("LegacyCardinalityEstimation", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("LegacyCardinalityEstimationForSecondary", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("MaxDop", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("MaxDopForSecondary", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("MemoryAllocatedToMemoryOptimizedObjectsInKB", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("MemoryUsedByMemoryOptimizedObjectsInKB", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("ParameterSniffing", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("ParameterSniffingForSecondary", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("QueryOptimizerHotfixes", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("QueryOptimizerHotfixesForSecondary", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("TemporalHistoryRetentionEnabled", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[133]
		{
			new StaticMetadata("ActiveConnections", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("AutoClose", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoShrink", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CompatibilityLevel", expensive: true, readOnly: false, typeof(CompatibilityLevel)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataSpaceUsage", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("DboLogin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("DefaultFileGroup", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IndexSpaceUsage", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("IsAccessible", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbAccessAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbBackupOperator", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDatareader", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDatawriter", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDdlAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDenyDatareader", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbDenyDatawriter", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbOwner", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDbSecurityAdmin", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsMirroringEnabled", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSqlDw", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("PrimaryFilePath", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("ReplicationOptions", expensive: false, readOnly: true, typeof(ReplicationOptions)),
			new StaticMetadata("Size", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("SpaceAvailable", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("Status", expensive: false, readOnly: true, typeof(DatabaseStatus)),
			new StaticMetadata("UserName", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("AnsiNullDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPaddingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarningsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ArithmeticAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CaseSensitive", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("CloseCursorsOnCommitEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Collation", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseOwnershipChaining", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("IsUpdateable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LastBackupDate", expensive: true, readOnly: true, typeof(DateTime)),
			new StaticMetadata("LastDifferentialBackupDate", expensive: true, readOnly: true, typeof(DateTime)),
			new StaticMetadata("LastGoodCheckDbTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("LastLogBackupDate", expensive: true, readOnly: true, typeof(DateTime)),
			new StaticMetadata("LocalCursorsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericRoundAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PageVerify", expensive: false, readOnly: false, typeof(PageVerify)),
			new StaticMetadata("QuotedIdentifiersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RecoveryModel", expensive: false, readOnly: false, typeof(RecoveryModel)),
			new StaticMetadata("RecursiveTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserAccess", expensive: false, readOnly: false, typeof(DatabaseUserAccess)),
			new StaticMetadata("Version", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("AutoUpdateStatisticsAsync", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BrokerEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("DatabaseSnapshotBaseName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateCorrelationOptimization", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DefaultFullTextCatalog", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("HasFullBackup", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDatabaseSnapshot", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsDatabaseSnapshotBase", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsMailHost", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IsParameterizationForced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsReadCommittedSnapshotOn", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsVarDecimalStorageFormatEnabled", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("LogReuseWaitStatus", expensive: false, readOnly: true, typeof(LogReuseWaitStatus)),
			new StaticMetadata("MirroringFailoverLogSequenceNumber", expensive: true, readOnly: true, typeof(decimal)),
			new StaticMetadata("MirroringID", expensive: true, readOnly: true, typeof(Guid)),
			new StaticMetadata("MirroringPartner", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("MirroringPartnerInstance", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("MirroringRedoQueueMaxSize", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("MirroringRole", expensive: true, readOnly: true, typeof(MirroringRole)),
			new StaticMetadata("MirroringRoleSequence", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("MirroringSafetyLevel", expensive: true, readOnly: false, typeof(MirroringSafetyLevel)),
			new StaticMetadata("MirroringSafetySequence", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("MirroringStatus", expensive: true, readOnly: true, typeof(MirroringStatus)),
			new StaticMetadata("MirroringTimeout", expensive: true, readOnly: false, typeof(int)),
			new StaticMetadata("MirroringWitness", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("MirroringWitnessStatus", expensive: true, readOnly: true, typeof(MirroringWitnessStatus)),
			new StaticMetadata("RecoveryForkGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("ServiceBrokerGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("SnapshotIsolationState", expensive: false, readOnly: true, typeof(SnapshotIsolationState)),
			new StaticMetadata("Trustworthy", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ChangeTrackingAutoCleanUp", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ChangeTrackingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ChangeTrackingRetentionPeriod", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ChangeTrackingRetentionPeriodUnits", expensive: false, readOnly: false, typeof(RetentionPeriodUnits)),
			new StaticMetadata("DefaultFileStreamFileGroup", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("EncryptionEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("HonorBrokerPriority", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsManagementDataWarehouse", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("AvailabilityDatabaseSynchronizationState", expensive: true, readOnly: true, typeof(AvailabilityDatabaseSynchronizationState)),
			new StaticMetadata("AvailabilityGroupName", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("ContainmentType", expensive: false, readOnly: false, typeof(ContainmentType)),
			new StaticMetadata("DefaultFullTextLanguageLcid", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("DefaultFullTextLanguageName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultLanguageLcid", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("DefaultLanguageName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FilestreamDirectoryName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FilestreamNonTransactedAccess", expensive: false, readOnly: false, typeof(FilestreamNonTransactedAccessType)),
			new StaticMetadata("HasDatabaseEncryptionKey", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("NestedTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("TargetRecoveryTime", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("TransformNoiseWords", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("TwoDigitYearCutoff", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("AutoCreateIncrementalStatisticsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DelayedDurability", expensive: false, readOnly: false, typeof(DelayedDurability)),
			new StaticMetadata("HasFileInCloud", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasMemoryOptimizedObjects", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("MemoryAllocatedToMemoryOptimizedObjectsInKB", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("MemoryUsedByMemoryOptimizedObjectsInKB", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("LegacyCardinalityEstimation", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("LegacyCardinalityEstimationForSecondary", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("MaxDop", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("MaxDopForSecondary", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ParameterSniffing", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("ParameterSniffingForSecondary", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("QueryOptimizerHotfixes", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("QueryOptimizerHotfixesForSecondary", expensive: false, readOnly: false, typeof(DatabaseScopedConfigurationOnOff)),
			new StaticMetadata("RemoteDataArchiveCredential", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteDataArchiveEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RemoteDataArchiveEndpoint", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteDataArchiveLinkedServer", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteDataArchiveUseFederatedServiceAccount", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("RemoteDatabaseName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("CatalogCollation", expensive: false, readOnly: false, typeof(CatalogCollationType))
		};

		public override int Count
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return sqlDwPropertyCount;
					}
					int num = ((currentVersionIndex < cloudVersionCount.Length) ? currentVersionIndex : (cloudVersionCount.Length - 1));
					return cloudVersionCount[num];
				}
				int num2 = ((currentVersionIndex < versionCount.Length) ? currentVersionIndex : (versionCount.Length - 1));
				return versionCount[num2];
			}
		}

		protected override int[] VersionCount
		{
			get
			{
				if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
				{
					if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
					{
						return new int[1] { sqlDwPropertyCount };
					}
					return cloudVersionCount;
				}
				return versionCount;
			}
		}

		internal PropertyMetadataProvider(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
			: base(version, databaseEngineType, databaseEngineEdition)
		{
		}

		public override int PropertyNameToIDLookup(string propertyName)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return propertyName switch
					{
						"AnsiNullDefault" => 0, 
						"AnsiNullsEnabled" => 1, 
						"AnsiPaddingEnabled" => 2, 
						"AnsiWarningsEnabled" => 3, 
						"ArithmeticAbortEnabled" => 4, 
						"AutoCreateIncrementalStatisticsEnabled" => 5, 
						"AutoCreateStatisticsEnabled" => 6, 
						"AutoShrink" => 7, 
						"AutoUpdateStatisticsAsync" => 8, 
						"AutoUpdateStatisticsEnabled" => 9, 
						"AzureEdition" => 10, 
						"AzureServiceObjective" => 11, 
						"CaseSensitive" => 12, 
						"CloseCursorsOnCommitEnabled" => 13, 
						"Collation" => 14, 
						"CompatibilityLevel" => 15, 
						"ConcatenateNullYieldsNull" => 16, 
						"CreateDate" => 17, 
						"DatabaseOwnershipChaining" => 18, 
						"DatabaseSnapshotBaseName" => 19, 
						"DateCorrelationOptimization" => 20, 
						"DboLogin" => 21, 
						"DefaultFileGroup" => 22, 
						"DefaultSchema" => 23, 
						"EncryptionEnabled" => 24, 
						"HonorBrokerPriority" => 25, 
						"ID" => 26, 
						"IsAccessible" => 27, 
						"IsDatabaseSnapshot" => 28, 
						"IsDatabaseSnapshotBase" => 29, 
						"IsDbAccessAdmin" => 30, 
						"IsDbBackupOperator" => 31, 
						"IsDbDatareader" => 32, 
						"IsDbDatawriter" => 33, 
						"IsDbDdlAdmin" => 34, 
						"IsDbDenyDatareader" => 35, 
						"IsDbDenyDatawriter" => 36, 
						"IsDbManager" => 37, 
						"IsDbOwner" => 38, 
						"IsDbSecurityAdmin" => 39, 
						"IsFullTextEnabled" => 40, 
						"IsLoginManager" => 41, 
						"IsMirroringEnabled" => 42, 
						"IsParameterizationForced" => 43, 
						"IsReadCommittedSnapshotOn" => 44, 
						"IsSqlDw" => 45, 
						"IsSystemObject" => 46, 
						"IsUpdateable" => 47, 
						"LastGoodCheckDbTime" => 48, 
						"MaxSizeInBytes" => 49, 
						"NumericRoundAbortEnabled" => 50, 
						"Owner" => 51, 
						"QuotedIdentifiersEnabled" => 52, 
						"ReadOnly" => 53, 
						"RecursiveTriggersEnabled" => 54, 
						"ReplicationOptions" => 55, 
						"ServiceBrokerGuid" => 56, 
						"Size" => 57, 
						"SnapshotIsolationState" => 58, 
						"SpaceAvailable" => 59, 
						"Status" => 60, 
						"Trustworthy" => 61, 
						"UserAccess" => 62, 
						"UserName" => 63, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AnsiNullDefault" => 0, 
					"AnsiNullsEnabled" => 1, 
					"AnsiPaddingEnabled" => 2, 
					"AnsiWarningsEnabled" => 3, 
					"ArithmeticAbortEnabled" => 4, 
					"AutoCreateStatisticsEnabled" => 5, 
					"AutoShrink" => 6, 
					"AutoUpdateStatisticsAsync" => 7, 
					"AutoUpdateStatisticsEnabled" => 8, 
					"AzureEdition" => 9, 
					"CaseSensitive" => 10, 
					"CloseCursorsOnCommitEnabled" => 11, 
					"Collation" => 12, 
					"CompatibilityLevel" => 13, 
					"ConcatenateNullYieldsNull" => 14, 
					"CreateDate" => 15, 
					"DatabaseOwnershipChaining" => 16, 
					"DatabaseSnapshotBaseName" => 17, 
					"DateCorrelationOptimization" => 18, 
					"DboLogin" => 19, 
					"DefaultSchema" => 20, 
					"ID" => 21, 
					"IsDatabaseSnapshot" => 22, 
					"IsDatabaseSnapshotBase" => 23, 
					"IsDbAccessAdmin" => 24, 
					"IsDbBackupOperator" => 25, 
					"IsDbDatareader" => 26, 
					"IsDbDatawriter" => 27, 
					"IsDbDdlAdmin" => 28, 
					"IsDbDenyDatareader" => 29, 
					"IsDbDenyDatawriter" => 30, 
					"IsDbManager" => 31, 
					"IsDbOwner" => 32, 
					"IsDbSecurityAdmin" => 33, 
					"IsFullTextEnabled" => 34, 
					"IsLoginManager" => 35, 
					"IsParameterizationForced" => 36, 
					"IsReadCommittedSnapshotOn" => 37, 
					"IsSqlDw" => 38, 
					"IsSystemObject" => 39, 
					"IsUpdateable" => 40, 
					"MaxSizeInBytes" => 41, 
					"NumericRoundAbortEnabled" => 42, 
					"Owner" => 43, 
					"QuotedIdentifiersEnabled" => 44, 
					"ReadOnly" => 45, 
					"RecursiveTriggersEnabled" => 46, 
					"ReplicationOptions" => 47, 
					"ServiceBrokerGuid" => 48, 
					"Size" => 49, 
					"SnapshotIsolationState" => 50, 
					"SpaceAvailable" => 51, 
					"Status" => 52, 
					"Trustworthy" => 53, 
					"UserAccess" => 54, 
					"UserName" => 55, 
					"AutoCreateIncrementalStatisticsEnabled" => 56, 
					"AzureServiceObjective" => 57, 
					"CatalogCollation" => 58, 
					"ChangeTrackingAutoCleanUp" => 59, 
					"ChangeTrackingEnabled" => 60, 
					"ChangeTrackingRetentionPeriod" => 61, 
					"ChangeTrackingRetentionPeriodUnits" => 62, 
					"DefaultFileGroup" => 63, 
					"DefaultFullTextCatalog" => 64, 
					"EncryptionEnabled" => 65, 
					"HasFileInCloud" => 66, 
					"HasMemoryOptimizedObjects" => 67, 
					"HonorBrokerPriority" => 68, 
					"IsAccessible" => 69, 
					"IsMirroringEnabled" => 70, 
					"IsSqlDwEdition" => 71, 
					"LastGoodCheckDbTime" => 72, 
					"LegacyCardinalityEstimation" => 73, 
					"LegacyCardinalityEstimationForSecondary" => 74, 
					"MaxDop" => 75, 
					"MaxDopForSecondary" => 76, 
					"MemoryAllocatedToMemoryOptimizedObjectsInKB" => 77, 
					"MemoryUsedByMemoryOptimizedObjectsInKB" => 78, 
					"ParameterSniffing" => 79, 
					"ParameterSniffingForSecondary" => 80, 
					"QueryOptimizerHotfixes" => 81, 
					"QueryOptimizerHotfixesForSecondary" => 82, 
					"TemporalHistoryRetentionEnabled" => 83, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ActiveConnections" => 0, 
				"AutoClose" => 1, 
				"AutoShrink" => 2, 
				"CompatibilityLevel" => 3, 
				"CreateDate" => 4, 
				"DataSpaceUsage" => 5, 
				"DboLogin" => 6, 
				"DefaultFileGroup" => 7, 
				"DefaultSchema" => 8, 
				"ID" => 9, 
				"IndexSpaceUsage" => 10, 
				"IsAccessible" => 11, 
				"IsDbAccessAdmin" => 12, 
				"IsDbBackupOperator" => 13, 
				"IsDbDatareader" => 14, 
				"IsDbDatawriter" => 15, 
				"IsDbDdlAdmin" => 16, 
				"IsDbDenyDatareader" => 17, 
				"IsDbDenyDatawriter" => 18, 
				"IsDbOwner" => 19, 
				"IsDbSecurityAdmin" => 20, 
				"IsFullTextEnabled" => 21, 
				"IsMirroringEnabled" => 22, 
				"IsSqlDw" => 23, 
				"IsSystemObject" => 24, 
				"Owner" => 25, 
				"PrimaryFilePath" => 26, 
				"ReplicationOptions" => 27, 
				"Size" => 28, 
				"SpaceAvailable" => 29, 
				"Status" => 30, 
				"UserName" => 31, 
				"AnsiNullDefault" => 32, 
				"AnsiNullsEnabled" => 33, 
				"AnsiPaddingEnabled" => 34, 
				"AnsiWarningsEnabled" => 35, 
				"ArithmeticAbortEnabled" => 36, 
				"AutoCreateStatisticsEnabled" => 37, 
				"AutoUpdateStatisticsEnabled" => 38, 
				"CaseSensitive" => 39, 
				"CloseCursorsOnCommitEnabled" => 40, 
				"Collation" => 41, 
				"ConcatenateNullYieldsNull" => 42, 
				"DatabaseOwnershipChaining" => 43, 
				"IsUpdateable" => 44, 
				"LastBackupDate" => 45, 
				"LastDifferentialBackupDate" => 46, 
				"LastGoodCheckDbTime" => 47, 
				"LastLogBackupDate" => 48, 
				"LocalCursorsDefault" => 49, 
				"NumericRoundAbortEnabled" => 50, 
				"PageVerify" => 51, 
				"QuotedIdentifiersEnabled" => 52, 
				"ReadOnly" => 53, 
				"RecoveryModel" => 54, 
				"RecursiveTriggersEnabled" => 55, 
				"UserAccess" => 56, 
				"Version" => 57, 
				"AutoUpdateStatisticsAsync" => 58, 
				"BrokerEnabled" => 59, 
				"DatabaseGuid" => 60, 
				"DatabaseSnapshotBaseName" => 61, 
				"DateCorrelationOptimization" => 62, 
				"DefaultFullTextCatalog" => 63, 
				"HasFullBackup" => 64, 
				"IsDatabaseSnapshot" => 65, 
				"IsDatabaseSnapshotBase" => 66, 
				"IsMailHost" => 67, 
				"IsParameterizationForced" => 68, 
				"IsReadCommittedSnapshotOn" => 69, 
				"IsVarDecimalStorageFormatEnabled" => 70, 
				"LogReuseWaitStatus" => 71, 
				"MirroringFailoverLogSequenceNumber" => 72, 
				"MirroringID" => 73, 
				"MirroringPartner" => 74, 
				"MirroringPartnerInstance" => 75, 
				"MirroringRedoQueueMaxSize" => 76, 
				"MirroringRole" => 77, 
				"MirroringRoleSequence" => 78, 
				"MirroringSafetyLevel" => 79, 
				"MirroringSafetySequence" => 80, 
				"MirroringStatus" => 81, 
				"MirroringTimeout" => 82, 
				"MirroringWitness" => 83, 
				"MirroringWitnessStatus" => 84, 
				"RecoveryForkGuid" => 85, 
				"ServiceBrokerGuid" => 86, 
				"SnapshotIsolationState" => 87, 
				"Trustworthy" => 88, 
				"ChangeTrackingAutoCleanUp" => 89, 
				"ChangeTrackingEnabled" => 90, 
				"ChangeTrackingRetentionPeriod" => 91, 
				"ChangeTrackingRetentionPeriodUnits" => 92, 
				"DefaultFileStreamFileGroup" => 93, 
				"EncryptionEnabled" => 94, 
				"HonorBrokerPriority" => 95, 
				"IsManagementDataWarehouse" => 96, 
				"PolicyHealthState" => 97, 
				"AvailabilityDatabaseSynchronizationState" => 98, 
				"AvailabilityGroupName" => 99, 
				"ContainmentType" => 100, 
				"DefaultFullTextLanguageLcid" => 101, 
				"DefaultFullTextLanguageName" => 102, 
				"DefaultLanguageLcid" => 103, 
				"DefaultLanguageName" => 104, 
				"FilestreamDirectoryName" => 105, 
				"FilestreamNonTransactedAccess" => 106, 
				"HasDatabaseEncryptionKey" => 107, 
				"NestedTriggersEnabled" => 108, 
				"TargetRecoveryTime" => 109, 
				"TransformNoiseWords" => 110, 
				"TwoDigitYearCutoff" => 111, 
				"AutoCreateIncrementalStatisticsEnabled" => 112, 
				"DelayedDurability" => 113, 
				"HasFileInCloud" => 114, 
				"HasMemoryOptimizedObjects" => 115, 
				"MemoryAllocatedToMemoryOptimizedObjectsInKB" => 116, 
				"MemoryUsedByMemoryOptimizedObjectsInKB" => 117, 
				"LegacyCardinalityEstimation" => 118, 
				"LegacyCardinalityEstimationForSecondary" => 119, 
				"MaxDop" => 120, 
				"MaxDopForSecondary" => 121, 
				"ParameterSniffing" => 122, 
				"ParameterSniffingForSecondary" => 123, 
				"QueryOptimizerHotfixes" => 124, 
				"QueryOptimizerHotfixesForSecondary" => 125, 
				"RemoteDataArchiveCredential" => 126, 
				"RemoteDataArchiveEnabled" => 127, 
				"RemoteDataArchiveEndpoint" => 128, 
				"RemoteDataArchiveLinkedServer" => 129, 
				"RemoteDataArchiveUseFederatedServiceAccount" => 130, 
				"RemoteDatabaseName" => 131, 
				"CatalogCollation" => 132, 
				_ => -1, 
			};
		}

		internal new static int[] GetVersionArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return new int[1] { sqlDwPropertyCount };
				}
				return cloudVersionCount;
			}
			return versionCount;
		}

		public override StaticMetadata GetStaticMetadata(int id)
		{
			if (base.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (base.DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata[id];
				}
				return cloudStaticMetadata[id];
			}
			return staticMetadata[id];
		}

		internal new static StaticMetadata[] GetStaticMetadataArray(DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
		{
			if (databaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				if (databaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
				{
					return sqlDwStaticMetadata;
				}
				return cloudStaticMetadata;
			}
			return staticMetadata;
		}
	}

	private sealed class XSchemaProps
	{
		private AvailabilityDatabaseSynchronizationState _AvailabilityDatabaseSynchronizationState;

		private string _AvailabilityGroupName;

		private string _Collation;

		private string _DatabaseSnapshotBaseName;

		private string _DefaultSchema;

		private DateTime _LastDifferentialBackupDate;

		private DateTime _LastGoodCheckDbTime;

		internal AvailabilityDatabaseSynchronizationState AvailabilityDatabaseSynchronizationState
		{
			get
			{
				return _AvailabilityDatabaseSynchronizationState;
			}
			set
			{
				_AvailabilityDatabaseSynchronizationState = value;
			}
		}

		internal string AvailabilityGroupName
		{
			get
			{
				return _AvailabilityGroupName;
			}
			set
			{
				_AvailabilityGroupName = value;
			}
		}

		internal string Collation
		{
			get
			{
				return _Collation;
			}
			set
			{
				_Collation = value;
			}
		}

		internal string DatabaseSnapshotBaseName
		{
			get
			{
				return _DatabaseSnapshotBaseName;
			}
			set
			{
				_DatabaseSnapshotBaseName = value;
			}
		}

		internal string DefaultSchema
		{
			get
			{
				return _DefaultSchema;
			}
			set
			{
				_DefaultSchema = value;
			}
		}

		internal DateTime LastDifferentialBackupDate
		{
			get
			{
				return _LastDifferentialBackupDate;
			}
			set
			{
				_LastDifferentialBackupDate = value;
			}
		}

		internal DateTime LastGoodCheckDbTime
		{
			get
			{
				return _LastGoodCheckDbTime;
			}
			set
			{
				_LastGoodCheckDbTime = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private int _ActiveConnections;

		private bool _AnsiNullDefault;

		private bool _AnsiNullsEnabled;

		private bool _AnsiPaddingEnabled;

		private bool _AnsiWarningsEnabled;

		private bool _ArithmeticAbortEnabled;

		private bool _AutoClose;

		private bool _AutoCreateIncrementalStatisticsEnabled;

		private bool _AutoCreateStatisticsEnabled;

		private bool _AutoShrink;

		private bool _AutoUpdateStatisticsAsync;

		private bool _AutoUpdateStatisticsEnabled;

		private bool _BrokerEnabled;

		private bool _CaseSensitive;

		private CatalogCollationType _CatalogCollation;

		private bool _ChangeTrackingAutoCleanUp;

		private bool _ChangeTrackingEnabled;

		private int _ChangeTrackingRetentionPeriod;

		private RetentionPeriodUnits _ChangeTrackingRetentionPeriodUnits;

		private bool _CloseCursorsOnCommitEnabled;

		private CompatibilityLevel _CompatibilityLevel;

		private bool _ConcatenateNullYieldsNull;

		private ContainmentType _ContainmentType;

		private DateTime _CreateDate;

		private Guid _DatabaseGuid;

		private bool _DatabaseOwnershipChaining;

		private double _DataSpaceUsage;

		private bool _DateCorrelationOptimization;

		private bool _DboLogin;

		private string _DefaultFileGroup;

		private string _DefaultFileStreamFileGroup;

		private string _DefaultFullTextCatalog;

		private int _DefaultFullTextLanguageLcid;

		private string _DefaultFullTextLanguageName;

		private int _DefaultLanguageLcid;

		private string _DefaultLanguageName;

		private DelayedDurability _DelayedDurability;

		private bool _EncryptionEnabled;

		private string _FilestreamDirectoryName;

		private FilestreamNonTransactedAccessType _FilestreamNonTransactedAccess;

		private bool _HasDatabaseEncryptionKey;

		private bool _HasFileInCloud;

		private bool _HasFullBackup;

		private bool _HasMemoryOptimizedObjects;

		private bool _HonorBrokerPriority;

		private int _ID;

		private double _IndexSpaceUsage;

		private bool _IsAccessible;

		private bool _IsDatabaseSnapshot;

		private bool _IsDatabaseSnapshotBase;

		private bool _IsDbAccessAdmin;

		private bool _IsDbBackupOperator;

		private bool _IsDbDatareader;

		private bool _IsDbDatawriter;

		private bool _IsDbDdlAdmin;

		private bool _IsDbDenyDatareader;

		private bool _IsDbDenyDatawriter;

		private bool _IsDbOwner;

		private bool _IsDbSecurityAdmin;

		private bool _IsFullTextEnabled;

		private bool _IsMailHost;

		private bool _IsManagementDataWarehouse;

		private bool _IsMirroringEnabled;

		private bool _IsParameterizationForced;

		private bool _IsReadCommittedSnapshotOn;

		private bool _IsSqlDw;

		private bool _IsSystemObject;

		private bool _IsUpdateable;

		private bool _IsVarDecimalStorageFormatEnabled;

		private DateTime _LastBackupDate;

		private DateTime _LastLogBackupDate;

		private DatabaseScopedConfigurationOnOff _LegacyCardinalityEstimation;

		private DatabaseScopedConfigurationOnOff _LegacyCardinalityEstimationForSecondary;

		private bool _LocalCursorsDefault;

		private LogReuseWaitStatus _LogReuseWaitStatus;

		private int _MaxDop;

		private int _MaxDopForSecondary;

		private double _MemoryAllocatedToMemoryOptimizedObjectsInKB;

		private double _MemoryUsedByMemoryOptimizedObjectsInKB;

		private decimal _MirroringFailoverLogSequenceNumber;

		private Guid _MirroringID;

		private string _MirroringPartner;

		private string _MirroringPartnerInstance;

		private int _MirroringRedoQueueMaxSize;

		private MirroringRole _MirroringRole;

		private int _MirroringRoleSequence;

		private MirroringSafetyLevel _MirroringSafetyLevel;

		private int _MirroringSafetySequence;

		private MirroringStatus _MirroringStatus;

		private int _MirroringTimeout;

		private string _MirroringWitness;

		private MirroringWitnessStatus _MirroringWitnessStatus;

		private bool _NestedTriggersEnabled;

		private bool _NumericRoundAbortEnabled;

		private string _Owner;

		private PageVerify _PageVerify;

		private DatabaseScopedConfigurationOnOff _ParameterSniffing;

		private DatabaseScopedConfigurationOnOff _ParameterSniffingForSecondary;

		private PolicyHealthState _PolicyHealthState;

		private string _PrimaryFilePath;

		private DatabaseScopedConfigurationOnOff _QueryOptimizerHotfixes;

		private DatabaseScopedConfigurationOnOff _QueryOptimizerHotfixesForSecondary;

		private bool _QuotedIdentifiersEnabled;

		private bool _ReadOnly;

		private Guid _RecoveryForkGuid;

		private RecoveryModel _RecoveryModel;

		private bool _RecursiveTriggersEnabled;

		private string _RemoteDataArchiveCredential;

		private bool _RemoteDataArchiveEnabled;

		private string _RemoteDataArchiveEndpoint;

		private string _RemoteDataArchiveLinkedServer;

		private bool _RemoteDataArchiveUseFederatedServiceAccount;

		private string _RemoteDatabaseName;

		private ReplicationOptions _ReplicationOptions;

		private Guid _ServiceBrokerGuid;

		private double _Size;

		private SnapshotIsolationState _SnapshotIsolationState;

		private double _SpaceAvailable;

		private DatabaseStatus _Status;

		private int _TargetRecoveryTime;

		private bool _TransformNoiseWords;

		private bool _Trustworthy;

		private int _TwoDigitYearCutoff;

		private DatabaseUserAccess _UserAccess;

		private string _UserName;

		private int _Version;

		private string _AzureEdition;

		private string _AzureServiceObjective;

		private bool _IsDbManager;

		private bool _IsLoginManager;

		private bool _IsSqlDwEdition;

		private double _MaxSizeInBytes;

		private bool _TemporalHistoryRetentionEnabled;

		internal int ActiveConnections
		{
			get
			{
				return _ActiveConnections;
			}
			set
			{
				_ActiveConnections = value;
			}
		}

		internal bool AnsiNullDefault
		{
			get
			{
				return _AnsiNullDefault;
			}
			set
			{
				_AnsiNullDefault = value;
			}
		}

		internal bool AnsiNullsEnabled
		{
			get
			{
				return _AnsiNullsEnabled;
			}
			set
			{
				_AnsiNullsEnabled = value;
			}
		}

		internal bool AnsiPaddingEnabled
		{
			get
			{
				return _AnsiPaddingEnabled;
			}
			set
			{
				_AnsiPaddingEnabled = value;
			}
		}

		internal bool AnsiWarningsEnabled
		{
			get
			{
				return _AnsiWarningsEnabled;
			}
			set
			{
				_AnsiWarningsEnabled = value;
			}
		}

		internal bool ArithmeticAbortEnabled
		{
			get
			{
				return _ArithmeticAbortEnabled;
			}
			set
			{
				_ArithmeticAbortEnabled = value;
			}
		}

		internal bool AutoClose
		{
			get
			{
				return _AutoClose;
			}
			set
			{
				_AutoClose = value;
			}
		}

		internal bool AutoCreateIncrementalStatisticsEnabled
		{
			get
			{
				return _AutoCreateIncrementalStatisticsEnabled;
			}
			set
			{
				_AutoCreateIncrementalStatisticsEnabled = value;
			}
		}

		internal bool AutoCreateStatisticsEnabled
		{
			get
			{
				return _AutoCreateStatisticsEnabled;
			}
			set
			{
				_AutoCreateStatisticsEnabled = value;
			}
		}

		internal bool AutoShrink
		{
			get
			{
				return _AutoShrink;
			}
			set
			{
				_AutoShrink = value;
			}
		}

		internal bool AutoUpdateStatisticsAsync
		{
			get
			{
				return _AutoUpdateStatisticsAsync;
			}
			set
			{
				_AutoUpdateStatisticsAsync = value;
			}
		}

		internal bool AutoUpdateStatisticsEnabled
		{
			get
			{
				return _AutoUpdateStatisticsEnabled;
			}
			set
			{
				_AutoUpdateStatisticsEnabled = value;
			}
		}

		internal bool BrokerEnabled
		{
			get
			{
				return _BrokerEnabled;
			}
			set
			{
				_BrokerEnabled = value;
			}
		}

		internal bool CaseSensitive
		{
			get
			{
				return _CaseSensitive;
			}
			set
			{
				_CaseSensitive = value;
			}
		}

		internal CatalogCollationType CatalogCollation
		{
			get
			{
				return _CatalogCollation;
			}
			set
			{
				_CatalogCollation = value;
			}
		}

		internal bool ChangeTrackingAutoCleanUp
		{
			get
			{
				return _ChangeTrackingAutoCleanUp;
			}
			set
			{
				_ChangeTrackingAutoCleanUp = value;
			}
		}

		internal bool ChangeTrackingEnabled
		{
			get
			{
				return _ChangeTrackingEnabled;
			}
			set
			{
				_ChangeTrackingEnabled = value;
			}
		}

		internal int ChangeTrackingRetentionPeriod
		{
			get
			{
				return _ChangeTrackingRetentionPeriod;
			}
			set
			{
				_ChangeTrackingRetentionPeriod = value;
			}
		}

		internal RetentionPeriodUnits ChangeTrackingRetentionPeriodUnits
		{
			get
			{
				return _ChangeTrackingRetentionPeriodUnits;
			}
			set
			{
				_ChangeTrackingRetentionPeriodUnits = value;
			}
		}

		internal bool CloseCursorsOnCommitEnabled
		{
			get
			{
				return _CloseCursorsOnCommitEnabled;
			}
			set
			{
				_CloseCursorsOnCommitEnabled = value;
			}
		}

		internal CompatibilityLevel CompatibilityLevel
		{
			get
			{
				return _CompatibilityLevel;
			}
			set
			{
				_CompatibilityLevel = value;
			}
		}

		internal bool ConcatenateNullYieldsNull
		{
			get
			{
				return _ConcatenateNullYieldsNull;
			}
			set
			{
				_ConcatenateNullYieldsNull = value;
			}
		}

		internal ContainmentType ContainmentType
		{
			get
			{
				return _ContainmentType;
			}
			set
			{
				_ContainmentType = value;
			}
		}

		internal DateTime CreateDate
		{
			get
			{
				return _CreateDate;
			}
			set
			{
				_CreateDate = value;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return _DatabaseGuid;
			}
			set
			{
				_DatabaseGuid = value;
			}
		}

		internal bool DatabaseOwnershipChaining
		{
			get
			{
				return _DatabaseOwnershipChaining;
			}
			set
			{
				_DatabaseOwnershipChaining = value;
			}
		}

		internal double DataSpaceUsage
		{
			get
			{
				return _DataSpaceUsage;
			}
			set
			{
				_DataSpaceUsage = value;
			}
		}

		internal bool DateCorrelationOptimization
		{
			get
			{
				return _DateCorrelationOptimization;
			}
			set
			{
				_DateCorrelationOptimization = value;
			}
		}

		internal bool DboLogin
		{
			get
			{
				return _DboLogin;
			}
			set
			{
				_DboLogin = value;
			}
		}

		internal string DefaultFileGroup
		{
			get
			{
				return _DefaultFileGroup;
			}
			set
			{
				_DefaultFileGroup = value;
			}
		}

		internal string DefaultFileStreamFileGroup
		{
			get
			{
				return _DefaultFileStreamFileGroup;
			}
			set
			{
				_DefaultFileStreamFileGroup = value;
			}
		}

		internal string DefaultFullTextCatalog
		{
			get
			{
				return _DefaultFullTextCatalog;
			}
			set
			{
				_DefaultFullTextCatalog = value;
			}
		}

		internal int DefaultFullTextLanguageLcid
		{
			get
			{
				return _DefaultFullTextLanguageLcid;
			}
			set
			{
				_DefaultFullTextLanguageLcid = value;
			}
		}

		internal string DefaultFullTextLanguageName
		{
			get
			{
				return _DefaultFullTextLanguageName;
			}
			set
			{
				_DefaultFullTextLanguageName = value;
			}
		}

		internal int DefaultLanguageLcid
		{
			get
			{
				return _DefaultLanguageLcid;
			}
			set
			{
				_DefaultLanguageLcid = value;
			}
		}

		internal string DefaultLanguageName
		{
			get
			{
				return _DefaultLanguageName;
			}
			set
			{
				_DefaultLanguageName = value;
			}
		}

		internal DelayedDurability DelayedDurability
		{
			get
			{
				return _DelayedDurability;
			}
			set
			{
				_DelayedDurability = value;
			}
		}

		internal bool EncryptionEnabled
		{
			get
			{
				return _EncryptionEnabled;
			}
			set
			{
				_EncryptionEnabled = value;
			}
		}

		internal string FilestreamDirectoryName
		{
			get
			{
				return _FilestreamDirectoryName;
			}
			set
			{
				_FilestreamDirectoryName = value;
			}
		}

		internal FilestreamNonTransactedAccessType FilestreamNonTransactedAccess
		{
			get
			{
				return _FilestreamNonTransactedAccess;
			}
			set
			{
				_FilestreamNonTransactedAccess = value;
			}
		}

		internal bool HasDatabaseEncryptionKey
		{
			get
			{
				return _HasDatabaseEncryptionKey;
			}
			set
			{
				_HasDatabaseEncryptionKey = value;
			}
		}

		internal bool HasFileInCloud
		{
			get
			{
				return _HasFileInCloud;
			}
			set
			{
				_HasFileInCloud = value;
			}
		}

		internal bool HasFullBackup
		{
			get
			{
				return _HasFullBackup;
			}
			set
			{
				_HasFullBackup = value;
			}
		}

		internal bool HasMemoryOptimizedObjects
		{
			get
			{
				return _HasMemoryOptimizedObjects;
			}
			set
			{
				_HasMemoryOptimizedObjects = value;
			}
		}

		internal bool HonorBrokerPriority
		{
			get
			{
				return _HonorBrokerPriority;
			}
			set
			{
				_HonorBrokerPriority = value;
			}
		}

		internal int ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		internal double IndexSpaceUsage
		{
			get
			{
				return _IndexSpaceUsage;
			}
			set
			{
				_IndexSpaceUsage = value;
			}
		}

		internal bool IsAccessible
		{
			get
			{
				return _IsAccessible;
			}
			set
			{
				_IsAccessible = value;
			}
		}

		internal bool IsDatabaseSnapshot
		{
			get
			{
				return _IsDatabaseSnapshot;
			}
			set
			{
				_IsDatabaseSnapshot = value;
			}
		}

		internal bool IsDatabaseSnapshotBase
		{
			get
			{
				return _IsDatabaseSnapshotBase;
			}
			set
			{
				_IsDatabaseSnapshotBase = value;
			}
		}

		internal bool IsDbAccessAdmin
		{
			get
			{
				return _IsDbAccessAdmin;
			}
			set
			{
				_IsDbAccessAdmin = value;
			}
		}

		internal bool IsDbBackupOperator
		{
			get
			{
				return _IsDbBackupOperator;
			}
			set
			{
				_IsDbBackupOperator = value;
			}
		}

		internal bool IsDbDatareader
		{
			get
			{
				return _IsDbDatareader;
			}
			set
			{
				_IsDbDatareader = value;
			}
		}

		internal bool IsDbDatawriter
		{
			get
			{
				return _IsDbDatawriter;
			}
			set
			{
				_IsDbDatawriter = value;
			}
		}

		internal bool IsDbDdlAdmin
		{
			get
			{
				return _IsDbDdlAdmin;
			}
			set
			{
				_IsDbDdlAdmin = value;
			}
		}

		internal bool IsDbDenyDatareader
		{
			get
			{
				return _IsDbDenyDatareader;
			}
			set
			{
				_IsDbDenyDatareader = value;
			}
		}

		internal bool IsDbDenyDatawriter
		{
			get
			{
				return _IsDbDenyDatawriter;
			}
			set
			{
				_IsDbDenyDatawriter = value;
			}
		}

		internal bool IsDbOwner
		{
			get
			{
				return _IsDbOwner;
			}
			set
			{
				_IsDbOwner = value;
			}
		}

		internal bool IsDbSecurityAdmin
		{
			get
			{
				return _IsDbSecurityAdmin;
			}
			set
			{
				_IsDbSecurityAdmin = value;
			}
		}

		internal bool IsFullTextEnabled
		{
			get
			{
				return _IsFullTextEnabled;
			}
			set
			{
				_IsFullTextEnabled = value;
			}
		}

		internal bool IsMailHost
		{
			get
			{
				return _IsMailHost;
			}
			set
			{
				_IsMailHost = value;
			}
		}

		internal bool IsManagementDataWarehouse
		{
			get
			{
				return _IsManagementDataWarehouse;
			}
			set
			{
				_IsManagementDataWarehouse = value;
			}
		}

		internal bool IsMirroringEnabled
		{
			get
			{
				return _IsMirroringEnabled;
			}
			set
			{
				_IsMirroringEnabled = value;
			}
		}

		internal bool IsParameterizationForced
		{
			get
			{
				return _IsParameterizationForced;
			}
			set
			{
				_IsParameterizationForced = value;
			}
		}

		internal bool IsReadCommittedSnapshotOn
		{
			get
			{
				return _IsReadCommittedSnapshotOn;
			}
			set
			{
				_IsReadCommittedSnapshotOn = value;
			}
		}

		internal bool IsSqlDw
		{
			get
			{
				return _IsSqlDw;
			}
			set
			{
				_IsSqlDw = value;
			}
		}

		internal bool IsSystemObject
		{
			get
			{
				return _IsSystemObject;
			}
			set
			{
				_IsSystemObject = value;
			}
		}

		internal bool IsUpdateable
		{
			get
			{
				return _IsUpdateable;
			}
			set
			{
				_IsUpdateable = value;
			}
		}

		internal bool IsVarDecimalStorageFormatEnabled
		{
			get
			{
				return _IsVarDecimalStorageFormatEnabled;
			}
			set
			{
				_IsVarDecimalStorageFormatEnabled = value;
			}
		}

		internal DateTime LastBackupDate
		{
			get
			{
				return _LastBackupDate;
			}
			set
			{
				_LastBackupDate = value;
			}
		}

		internal DateTime LastLogBackupDate
		{
			get
			{
				return _LastLogBackupDate;
			}
			set
			{
				_LastLogBackupDate = value;
			}
		}

		internal DatabaseScopedConfigurationOnOff LegacyCardinalityEstimation
		{
			get
			{
				return _LegacyCardinalityEstimation;
			}
			set
			{
				_LegacyCardinalityEstimation = value;
			}
		}

		internal DatabaseScopedConfigurationOnOff LegacyCardinalityEstimationForSecondary
		{
			get
			{
				return _LegacyCardinalityEstimationForSecondary;
			}
			set
			{
				_LegacyCardinalityEstimationForSecondary = value;
			}
		}

		internal bool LocalCursorsDefault
		{
			get
			{
				return _LocalCursorsDefault;
			}
			set
			{
				_LocalCursorsDefault = value;
			}
		}

		internal LogReuseWaitStatus LogReuseWaitStatus
		{
			get
			{
				return _LogReuseWaitStatus;
			}
			set
			{
				_LogReuseWaitStatus = value;
			}
		}

		internal int MaxDop
		{
			get
			{
				return _MaxDop;
			}
			set
			{
				_MaxDop = value;
			}
		}

		internal int MaxDopForSecondary
		{
			get
			{
				return _MaxDopForSecondary;
			}
			set
			{
				_MaxDopForSecondary = value;
			}
		}

		internal double MemoryAllocatedToMemoryOptimizedObjectsInKB
		{
			get
			{
				return _MemoryAllocatedToMemoryOptimizedObjectsInKB;
			}
			set
			{
				_MemoryAllocatedToMemoryOptimizedObjectsInKB = value;
			}
		}

		internal double MemoryUsedByMemoryOptimizedObjectsInKB
		{
			get
			{
				return _MemoryUsedByMemoryOptimizedObjectsInKB;
			}
			set
			{
				_MemoryUsedByMemoryOptimizedObjectsInKB = value;
			}
		}

		internal decimal MirroringFailoverLogSequenceNumber
		{
			get
			{
				return _MirroringFailoverLogSequenceNumber;
			}
			set
			{
				_MirroringFailoverLogSequenceNumber = value;
			}
		}

		internal Guid MirroringID
		{
			get
			{
				return _MirroringID;
			}
			set
			{
				_MirroringID = value;
			}
		}

		internal string MirroringPartner
		{
			get
			{
				return _MirroringPartner;
			}
			set
			{
				_MirroringPartner = value;
			}
		}

		internal string MirroringPartnerInstance
		{
			get
			{
				return _MirroringPartnerInstance;
			}
			set
			{
				_MirroringPartnerInstance = value;
			}
		}

		internal int MirroringRedoQueueMaxSize
		{
			get
			{
				return _MirroringRedoQueueMaxSize;
			}
			set
			{
				_MirroringRedoQueueMaxSize = value;
			}
		}

		internal MirroringRole MirroringRole
		{
			get
			{
				return _MirroringRole;
			}
			set
			{
				_MirroringRole = value;
			}
		}

		internal int MirroringRoleSequence
		{
			get
			{
				return _MirroringRoleSequence;
			}
			set
			{
				_MirroringRoleSequence = value;
			}
		}

		internal MirroringSafetyLevel MirroringSafetyLevel
		{
			get
			{
				return _MirroringSafetyLevel;
			}
			set
			{
				_MirroringSafetyLevel = value;
			}
		}

		internal int MirroringSafetySequence
		{
			get
			{
				return _MirroringSafetySequence;
			}
			set
			{
				_MirroringSafetySequence = value;
			}
		}

		internal MirroringStatus MirroringStatus
		{
			get
			{
				return _MirroringStatus;
			}
			set
			{
				_MirroringStatus = value;
			}
		}

		internal int MirroringTimeout
		{
			get
			{
				return _MirroringTimeout;
			}
			set
			{
				_MirroringTimeout = value;
			}
		}

		internal string MirroringWitness
		{
			get
			{
				return _MirroringWitness;
			}
			set
			{
				_MirroringWitness = value;
			}
		}

		internal MirroringWitnessStatus MirroringWitnessStatus
		{
			get
			{
				return _MirroringWitnessStatus;
			}
			set
			{
				_MirroringWitnessStatus = value;
			}
		}

		internal bool NestedTriggersEnabled
		{
			get
			{
				return _NestedTriggersEnabled;
			}
			set
			{
				_NestedTriggersEnabled = value;
			}
		}

		internal bool NumericRoundAbortEnabled
		{
			get
			{
				return _NumericRoundAbortEnabled;
			}
			set
			{
				_NumericRoundAbortEnabled = value;
			}
		}

		internal string Owner
		{
			get
			{
				return _Owner;
			}
			set
			{
				_Owner = value;
			}
		}

		internal PageVerify PageVerify
		{
			get
			{
				return _PageVerify;
			}
			set
			{
				_PageVerify = value;
			}
		}

		internal DatabaseScopedConfigurationOnOff ParameterSniffing
		{
			get
			{
				return _ParameterSniffing;
			}
			set
			{
				_ParameterSniffing = value;
			}
		}

		internal DatabaseScopedConfigurationOnOff ParameterSniffingForSecondary
		{
			get
			{
				return _ParameterSniffingForSecondary;
			}
			set
			{
				_ParameterSniffingForSecondary = value;
			}
		}

		internal PolicyHealthState PolicyHealthState
		{
			get
			{
				return _PolicyHealthState;
			}
			set
			{
				_PolicyHealthState = value;
			}
		}

		internal string PrimaryFilePath
		{
			get
			{
				return _PrimaryFilePath;
			}
			set
			{
				_PrimaryFilePath = value;
			}
		}

		internal DatabaseScopedConfigurationOnOff QueryOptimizerHotfixes
		{
			get
			{
				return _QueryOptimizerHotfixes;
			}
			set
			{
				_QueryOptimizerHotfixes = value;
			}
		}

		internal DatabaseScopedConfigurationOnOff QueryOptimizerHotfixesForSecondary
		{
			get
			{
				return _QueryOptimizerHotfixesForSecondary;
			}
			set
			{
				_QueryOptimizerHotfixesForSecondary = value;
			}
		}

		internal bool QuotedIdentifiersEnabled
		{
			get
			{
				return _QuotedIdentifiersEnabled;
			}
			set
			{
				_QuotedIdentifiersEnabled = value;
			}
		}

		internal bool ReadOnly
		{
			get
			{
				return _ReadOnly;
			}
			set
			{
				_ReadOnly = value;
			}
		}

		internal Guid RecoveryForkGuid
		{
			get
			{
				return _RecoveryForkGuid;
			}
			set
			{
				_RecoveryForkGuid = value;
			}
		}

		internal RecoveryModel RecoveryModel
		{
			get
			{
				return _RecoveryModel;
			}
			set
			{
				_RecoveryModel = value;
			}
		}

		internal bool RecursiveTriggersEnabled
		{
			get
			{
				return _RecursiveTriggersEnabled;
			}
			set
			{
				_RecursiveTriggersEnabled = value;
			}
		}

		internal string RemoteDataArchiveCredential
		{
			get
			{
				return _RemoteDataArchiveCredential;
			}
			set
			{
				_RemoteDataArchiveCredential = value;
			}
		}

		internal bool RemoteDataArchiveEnabled
		{
			get
			{
				return _RemoteDataArchiveEnabled;
			}
			set
			{
				_RemoteDataArchiveEnabled = value;
			}
		}

		internal string RemoteDataArchiveEndpoint
		{
			get
			{
				return _RemoteDataArchiveEndpoint;
			}
			set
			{
				_RemoteDataArchiveEndpoint = value;
			}
		}

		internal string RemoteDataArchiveLinkedServer
		{
			get
			{
				return _RemoteDataArchiveLinkedServer;
			}
			set
			{
				_RemoteDataArchiveLinkedServer = value;
			}
		}

		internal bool RemoteDataArchiveUseFederatedServiceAccount
		{
			get
			{
				return _RemoteDataArchiveUseFederatedServiceAccount;
			}
			set
			{
				_RemoteDataArchiveUseFederatedServiceAccount = value;
			}
		}

		internal string RemoteDatabaseName
		{
			get
			{
				return _RemoteDatabaseName;
			}
			set
			{
				_RemoteDatabaseName = value;
			}
		}

		internal ReplicationOptions ReplicationOptions
		{
			get
			{
				return _ReplicationOptions;
			}
			set
			{
				_ReplicationOptions = value;
			}
		}

		internal Guid ServiceBrokerGuid
		{
			get
			{
				return _ServiceBrokerGuid;
			}
			set
			{
				_ServiceBrokerGuid = value;
			}
		}

		internal double Size
		{
			get
			{
				return _Size;
			}
			set
			{
				_Size = value;
			}
		}

		internal SnapshotIsolationState SnapshotIsolationState
		{
			get
			{
				return _SnapshotIsolationState;
			}
			set
			{
				_SnapshotIsolationState = value;
			}
		}

		internal double SpaceAvailable
		{
			get
			{
				return _SpaceAvailable;
			}
			set
			{
				_SpaceAvailable = value;
			}
		}

		internal DatabaseStatus Status
		{
			get
			{
				return _Status;
			}
			set
			{
				_Status = value;
			}
		}

		internal int TargetRecoveryTime
		{
			get
			{
				return _TargetRecoveryTime;
			}
			set
			{
				_TargetRecoveryTime = value;
			}
		}

		internal bool TransformNoiseWords
		{
			get
			{
				return _TransformNoiseWords;
			}
			set
			{
				_TransformNoiseWords = value;
			}
		}

		internal bool Trustworthy
		{
			get
			{
				return _Trustworthy;
			}
			set
			{
				_Trustworthy = value;
			}
		}

		internal int TwoDigitYearCutoff
		{
			get
			{
				return _TwoDigitYearCutoff;
			}
			set
			{
				_TwoDigitYearCutoff = value;
			}
		}

		internal DatabaseUserAccess UserAccess
		{
			get
			{
				return _UserAccess;
			}
			set
			{
				_UserAccess = value;
			}
		}

		internal string UserName
		{
			get
			{
				return _UserName;
			}
			set
			{
				_UserName = value;
			}
		}

		internal int Version
		{
			get
			{
				return _Version;
			}
			set
			{
				_Version = value;
			}
		}

		internal string AzureEdition
		{
			get
			{
				return _AzureEdition;
			}
			set
			{
				_AzureEdition = value;
			}
		}

		internal string AzureServiceObjective
		{
			get
			{
				return _AzureServiceObjective;
			}
			set
			{
				_AzureServiceObjective = value;
			}
		}

		internal bool IsDbManager
		{
			get
			{
				return _IsDbManager;
			}
			set
			{
				_IsDbManager = value;
			}
		}

		internal bool IsLoginManager
		{
			get
			{
				return _IsLoginManager;
			}
			set
			{
				_IsLoginManager = value;
			}
		}

		internal bool IsSqlDwEdition
		{
			get
			{
				return _IsSqlDwEdition;
			}
			set
			{
				_IsSqlDwEdition = value;
			}
		}

		internal double MaxSizeInBytes
		{
			get
			{
				return _MaxSizeInBytes;
			}
			set
			{
				_MaxSizeInBytes = value;
			}
		}

		internal bool TemporalHistoryRetentionEnabled
		{
			get
			{
				return _TemporalHistoryRetentionEnabled;
			}
			set
			{
				_TemporalHistoryRetentionEnabled = value;
			}
		}
	}

	internal enum SupportedEngineType
	{
		Standalone = 1
	}

	private class UrnInfo
	{
		public string UrnType;

		public bool HasSchema;

		public bool HasName;

		public DatabaseObjectTypes DatabaseObjectTypes;

		public int VersionMajor;

		public int VersionMinor;

		public int SupportedEngineTypes = 1;

		public UrnInfo(string urnType, bool hasSchema, bool hasName, DatabaseObjectTypes databaseObjectType, int versionMajor)
		{
			UrnType = urnType;
			HasSchema = hasSchema;
			HasName = hasName;
			DatabaseObjectTypes = databaseObjectType;
			VersionMajor = versionMajor;
		}

		public UrnInfo(string urnType, bool hasSchema, bool hasName, int supportedEngineTypes, DatabaseObjectTypes databaseObjectType, int versionMajor)
		{
			UrnType = urnType;
			HasSchema = hasSchema;
			HasName = hasName;
			DatabaseObjectTypes = databaseObjectType;
			VersionMajor = versionMajor;
			SupportedEngineTypes = supportedEngineTypes;
		}

		public UrnInfo(string urnType, bool hasSchema, bool hasName, DatabaseObjectTypes databaseObjectType, int versionMajor, int versionMinor)
			: this(urnType, hasSchema, hasName, databaseObjectType, versionMajor)
		{
			VersionMinor = versionMinor;
		}
	}

	internal class OptionTerminationStatement
	{
		private TimeSpan m_time;

		private TerminationClause m_clause;

		internal OptionTerminationStatement(TimeSpan time)
		{
			m_time = time;
		}

		internal OptionTerminationStatement(TerminationClause clause)
		{
			m_time = TimeSpan.Zero;
			m_clause = clause;
		}

		internal string GetTerminationScript()
		{
			if (TimeSpan.Zero != m_time)
			{
				return string.Format(SmoApplication.DefaultCulture, "WITH ROLLBACK AFTER {0} SECONDS", new object[1] { m_time.Seconds });
			}
			if (m_clause == TerminationClause.FailOnOpenTransactions)
			{
				return "WITH NO_WAIT";
			}
			return "WITH ROLLBACK IMMEDIATE";
		}
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	private DatabaseEvents events;

	private DatabaseEngineEdition? m_edition = null;

	private object syncRoot = new object();

	private ExecutionManager m_dbExecutionManager;

	private OptionTerminationStatement optionTerminationStatement;

	private bool bForAttach;

	private bool isDefaultLanguageModified;

	private bool isDefaultFulltextLanguageModified;

	private DatabaseOptions m_DatabaseOptions;

	private QueryStoreOptions m_QueryStoreOptions;

	private SynonymCollection m_Synonyms;

	private SequenceCollection m_Sequences;

	private TableCollection m_Tables;

	private DatabaseScopedCredentialCollection m_DatabaseScopedCredentials;

	private StoredProcedureCollection m_StoredProcedures;

	private SqlAssemblyCollection m_SqlAssemblies;

	private ExternalLibraryCollection m_ExternalLibraries;

	private UserDefinedTypeCollection m_UserDefinedTypes;

	private UserDefinedAggregateCollection m_UserDefinedAggregates;

	private FullTextCatalogCollection m_FullTextCatalogs;

	private FullTextStopListCollection m_FullTextStopLists;

	private SearchPropertyListCollection m_SearchPropertyLists;

	private SecurityPolicyCollection m_SecurityPolicies;

	private DatabaseScopedConfigurationCollection m_DatabaseScopedConfigurations;

	private ExternalDataSourceCollection m_ExternalDataSources;

	private ExternalFileFormatCollection m_ExternalFileFormats;

	private CertificateCollection certificateCollection;

	private ColumnMasterKeyCollection m_ColumnMasterKeys;

	private ColumnEncryptionKeyCollection m_ColumnEncryptionKeys;

	private SymmetricKeyCollection symmetricKeyCollection;

	private AsymmetricKeyCollection asymmetricKeyCollection;

	private DatabaseEncryptionKey m_DatabaseEncryptionKey;

	internal bool databaseEncryptionKeyInitialized;

	private ExtendedStoredProcedureCollection m_ExtendedStoredProcedures;

	private UserDefinedFunctionCollection m_UserDefinedFunctions;

	private ViewCollection m_Views;

	private UserCollection m_Users;

	private DatabaseAuditSpecificationCollection databaseAuditSpecifications;

	private SchemaCollection m_Schemas;

	private DatabaseRoleCollection m_Roles;

	private ApplicationRoleCollection m_ApplcicationRoles;

	private BackupSetCollection m_BackupSets;

	private LogFileCollection m_LogFiles;

	private FileGroupCollection m_FileGroups;

	private PlanGuideCollection m_PlanGuides;

	private DefaultCollection m_Defaults;

	private RuleCollection m_Rules;

	private UserDefinedDataTypeCollection m_UserDefinedDataTypes;

	private UserDefinedTableTypeCollection m_UserDefinedTableTypes;

	private XmlSchemaCollectionCollection m_XmlSchemaCollections;

	private PartitionFunctionCollection m_PartitionFunctions;

	private PartitionSchemeCollection m_PartitionSchemes;

	private DatabaseActiveDirectory databaseActiveDirectory;

	private MasterKey masterKey;

	internal bool masterKeyInitialized;

	private DatabaseDdlTriggerCollection databaseDdlTriggerCollection;

	private DefaultLanguage defaultLanguageObj;

	private DefaultLanguage defaultFullTextLanguageObj;

	private ServiceBroker m_ServiceBroker;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	private XSchemaProps XSchema
	{
		get
		{
			if (_XSchema == null)
			{
				_XSchema = new XSchemaProps();
			}
			return _XSchema;
		}
	}

	private XRuntimeProps XRuntime
	{
		get
		{
			if (_XRuntime == null)
			{
				_XRuntime = new XRuntimeProps();
			}
			return _XRuntime;
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int ActiveConnections => (int)base.Properties.GetValueWithNullReplacement("ActiveConnections");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool AnsiNullDefault
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullDefault");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design, "false")]
	public bool AnsiNullsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design, "false")]
	public bool AnsiPaddingEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiPaddingEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiPaddingEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool AnsiWarningsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiWarningsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiWarningsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool ArithmeticAbortEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ArithmeticAbortEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ArithmeticAbortEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.Deploy)]
	public bool AutoClose
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutoClose");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutoClose", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool AutoCreateIncrementalStatisticsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutoCreateIncrementalStatisticsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutoCreateIncrementalStatisticsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool AutoCreateStatisticsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutoCreateStatisticsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutoCreateStatisticsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool AutoShrink
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutoShrink");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutoShrink", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool AutoUpdateStatisticsAsync
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutoUpdateStatisticsAsync");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutoUpdateStatisticsAsync", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool AutoUpdateStatisticsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutoUpdateStatisticsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutoUpdateStatisticsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public AvailabilityDatabaseSynchronizationState AvailabilityDatabaseSynchronizationState => (AvailabilityDatabaseSynchronizationState)base.Properties.GetValueWithNullReplacement("AvailabilityDatabaseSynchronizationState");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string AvailabilityGroupName => (string)base.Properties.GetValueWithNullReplacement("AvailabilityGroupName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool BrokerEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("BrokerEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BrokerEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool CaseSensitive => (bool)base.Properties.GetValueWithNullReplacement("CaseSensitive");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool ChangeTrackingAutoCleanUp
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ChangeTrackingAutoCleanUp");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ChangeTrackingAutoCleanUp", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool ChangeTrackingEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ChangeTrackingEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ChangeTrackingEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public int ChangeTrackingRetentionPeriod
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ChangeTrackingRetentionPeriod");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ChangeTrackingRetentionPeriod", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public RetentionPeriodUnits ChangeTrackingRetentionPeriodUnits
	{
		get
		{
			return (RetentionPeriodUnits)base.Properties.GetValueWithNullReplacement("ChangeTrackingRetentionPeriodUnits");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ChangeTrackingRetentionPeriodUnits", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool CloseCursorsOnCommitEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("CloseCursorsOnCommitEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CloseCursorsOnCommitEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design, "SQL_Latin1_General_CP1_CI_AS")]
	public string Collation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Collation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Collation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public CompatibilityLevel CompatibilityLevel
	{
		get
		{
			return (CompatibilityLevel)base.Properties.GetValueWithNullReplacement("CompatibilityLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CompatibilityLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool ConcatenateNullYieldsNull
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ConcatenateNullYieldsNull");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ConcatenateNullYieldsNull", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ContainmentType ContainmentType
	{
		get
		{
			return (ContainmentType)base.Properties.GetValueWithNullReplacement("ContainmentType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ContainmentType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid DatabaseGuid => (Guid)base.Properties.GetValueWithNullReplacement("DatabaseGuid");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DatabaseSnapshotBaseName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DatabaseSnapshotBaseName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DatabaseSnapshotBaseName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double DataSpaceUsage => (double)base.Properties.GetValueWithNullReplacement("DataSpaceUsage");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool DateCorrelationOptimization
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DateCorrelationOptimization");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DateCorrelationOptimization", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool DboLogin => (bool)base.Properties.GetValueWithNullReplacement("DboLogin");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DefaultFileGroup => (string)base.Properties.GetValueWithNullReplacement("DefaultFileGroup");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string DefaultFileStreamFileGroup => (string)base.Properties.GetValueWithNullReplacement("DefaultFileStreamFileGroup");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string DefaultFullTextCatalog
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultFullTextCatalog");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultFullTextCatalog", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string DefaultSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DelayedDurability DelayedDurability
	{
		get
		{
			return (DelayedDurability)base.Properties.GetValueWithNullReplacement("DelayedDurability");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DelayedDurability", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool EncryptionEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("EncryptionEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptionEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FilestreamDirectoryName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FilestreamDirectoryName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FilestreamDirectoryName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public FilestreamNonTransactedAccessType FilestreamNonTransactedAccess
	{
		get
		{
			return (FilestreamNonTransactedAccessType)base.Properties.GetValueWithNullReplacement("FilestreamNonTransactedAccess");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FilestreamNonTransactedAccess", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasFileInCloud => (bool)base.Properties.GetValueWithNullReplacement("HasFileInCloud");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasMemoryOptimizedObjects => (bool)base.Properties.GetValueWithNullReplacement("HasMemoryOptimizedObjects");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HonorBrokerPriority
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("HonorBrokerPriority");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HonorBrokerPriority", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double IndexSpaceUsage => (double)base.Properties.GetValueWithNullReplacement("IndexSpaceUsage");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsAccessible => (bool)base.Properties.GetValueWithNullReplacement("IsAccessible");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDatabaseSnapshot => (bool)base.Properties.GetValueWithNullReplacement("IsDatabaseSnapshot");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDatabaseSnapshotBase => (bool)base.Properties.GetValueWithNullReplacement("IsDatabaseSnapshotBase");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbAccessAdmin => (bool)base.Properties.GetValueWithNullReplacement("IsDbAccessAdmin");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbBackupOperator => (bool)base.Properties.GetValueWithNullReplacement("IsDbBackupOperator");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbDatareader => (bool)base.Properties.GetValueWithNullReplacement("IsDbDatareader");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbDatawriter => (bool)base.Properties.GetValueWithNullReplacement("IsDbDatawriter");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbDdlAdmin => (bool)base.Properties.GetValueWithNullReplacement("IsDbDdlAdmin");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbDenyDatareader => (bool)base.Properties.GetValueWithNullReplacement("IsDbDenyDatareader");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbDenyDatawriter => (bool)base.Properties.GetValueWithNullReplacement("IsDbDenyDatawriter");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbOwner => (bool)base.Properties.GetValueWithNullReplacement("IsDbOwner");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbSecurityAdmin => (bool)base.Properties.GetValueWithNullReplacement("IsDbSecurityAdmin");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFullTextEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsFullTextEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsFullTextEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool IsMailHost => (bool)base.Properties.GetValueWithNullReplacement("IsMailHost");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool IsManagementDataWarehouse => (bool)base.Properties.GetValueWithNullReplacement("IsManagementDataWarehouse");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsMirroringEnabled => (bool)base.Properties.GetValueWithNullReplacement("IsMirroringEnabled");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool IsParameterizationForced
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsParameterizationForced");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsParameterizationForced", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool IsReadCommittedSnapshotOn
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsReadCommittedSnapshotOn");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsReadCommittedSnapshotOn", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSqlDw
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSqlDw");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSqlDw", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsUpdateable => (bool)base.Properties.GetValueWithNullReplacement("IsUpdateable");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public DateTime LastBackupDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastBackupDate");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public DateTime LastDifferentialBackupDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastDifferentialBackupDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime LastGoodCheckDbTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastGoodCheckDbTime");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public DateTime LastLogBackupDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastLogBackupDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.Design)]
	public bool LocalCursorsDefault
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("LocalCursorsDefault");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LocalCursorsDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public LogReuseWaitStatus LogReuseWaitStatus => (LogReuseWaitStatus)base.Properties.GetValueWithNullReplacement("LogReuseWaitStatus");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double MemoryAllocatedToMemoryOptimizedObjectsInKB => (double)base.Properties.GetValueWithNullReplacement("MemoryAllocatedToMemoryOptimizedObjectsInKB");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double MemoryUsedByMemoryOptimizedObjectsInKB => (double)base.Properties.GetValueWithNullReplacement("MemoryUsedByMemoryOptimizedObjectsInKB");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public decimal MirroringFailoverLogSequenceNumber => (decimal)base.Properties.GetValueWithNullReplacement("MirroringFailoverLogSequenceNumber");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public Guid MirroringID => (Guid)base.Properties.GetValueWithNullReplacement("MirroringID");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string MirroringPartner
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MirroringPartner");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MirroringPartner", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string MirroringPartnerInstance => (string)base.Properties.GetValueWithNullReplacement("MirroringPartnerInstance");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int MirroringRedoQueueMaxSize => (int)base.Properties.GetValueWithNullReplacement("MirroringRedoQueueMaxSize");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int MirroringRoleSequence => (int)base.Properties.GetValueWithNullReplacement("MirroringRoleSequence");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public MirroringSafetyLevel MirroringSafetyLevel
	{
		get
		{
			return (MirroringSafetyLevel)base.Properties.GetValueWithNullReplacement("MirroringSafetyLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MirroringSafetyLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int MirroringSafetySequence => (int)base.Properties.GetValueWithNullReplacement("MirroringSafetySequence");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public MirroringStatus MirroringStatus => (MirroringStatus)base.Properties.GetValueWithNullReplacement("MirroringStatus");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int MirroringTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MirroringTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MirroringTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string MirroringWitness
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MirroringWitness");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MirroringWitness", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public MirroringWitnessStatus MirroringWitnessStatus => (MirroringWitnessStatus)base.Properties.GetValueWithNullReplacement("MirroringWitnessStatus");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool NestedTriggersEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NestedTriggersEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NestedTriggersEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool NumericRoundAbortEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NumericRoundAbortEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumericRoundAbortEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[CLSCompliant(false)]
	[SfcReference(typeof(Login), "Server[@Name = '{0}']/Login[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "Owner" })]
	public string Owner => (string)base.Properties.GetValueWithNullReplacement("Owner");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.Design)]
	public PageVerify PageVerify
	{
		get
		{
			return (PageVerify)base.Properties.GetValueWithNullReplacement("PageVerify");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PageVerify", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string PrimaryFilePath => (string)base.Properties.GetValueWithNullReplacement("PrimaryFilePath");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool QuotedIdentifiersEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("QuotedIdentifiersEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QuotedIdentifiersEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool ReadOnly
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ReadOnly");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReadOnly", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid RecoveryForkGuid => (Guid)base.Properties.GetValueWithNullReplacement("RecoveryForkGuid");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.Deploy)]
	public RecoveryModel RecoveryModel
	{
		get
		{
			return (RecoveryModel)base.Properties.GetValueWithNullReplacement("RecoveryModel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RecoveryModel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool RecursiveTriggersEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RecursiveTriggersEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RecursiveTriggersEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string RemoteDataArchiveCredential
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveCredential");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveCredential", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool RemoteDataArchiveEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string RemoteDataArchiveEndpoint
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveEndpoint");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveEndpoint", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string RemoteDataArchiveLinkedServer
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveLinkedServer");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveLinkedServer", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public bool RemoteDataArchiveUseFederatedServiceAccount
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveUseFederatedServiceAccount");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveUseFederatedServiceAccount", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string RemoteDatabaseName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteDatabaseName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDatabaseName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ReplicationOptions ReplicationOptions => (ReplicationOptions)base.Properties.GetValueWithNullReplacement("ReplicationOptions");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public Guid ServiceBrokerGuid => (Guid)base.Properties.GetValueWithNullReplacement("ServiceBrokerGuid");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double Size => (double)base.Properties.GetValueWithNullReplacement("Size");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SnapshotIsolationState SnapshotIsolationState => (SnapshotIsolationState)base.Properties.GetValueWithNullReplacement("SnapshotIsolationState");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double SpaceAvailable => (double)base.Properties.GetValueWithNullReplacement("SpaceAvailable");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseStatus Status => (DatabaseStatus)base.Properties.GetValueWithNullReplacement("Status");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int TargetRecoveryTime
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("TargetRecoveryTime");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TargetRecoveryTime", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool TransformNoiseWords
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("TransformNoiseWords");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TransformNoiseWords", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Trustworthy
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Trustworthy");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Trustworthy", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int TwoDigitYearCutoff
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("TwoDigitYearCutoff");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TwoDigitYearCutoff", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public DatabaseUserAccess UserAccess
	{
		get
		{
			return (DatabaseUserAccess)base.Properties.GetValueWithNullReplacement("UserAccess");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UserAccess", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string UserName => (string)base.Properties.GetValueWithNullReplacement("UserName");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int Version => (int)base.Properties.GetValueWithNullReplacement("Version");

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public string AzureEdition
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AzureEdition");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AzureEdition", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public string AzureServiceObjective
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AzureServiceObjective");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AzureServiceObjective", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDbManager => (bool)base.Properties.GetValueWithNullReplacement("IsDbManager");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsLoginManager => (bool)base.Properties.GetValueWithNullReplacement("IsLoginManager");

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSqlDwEdition => (bool)base.Properties.GetValueWithNullReplacement("IsSqlDwEdition");

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public double MaxSizeInBytes
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("MaxSizeInBytes");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaxSizeInBytes", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public bool TemporalHistoryRetentionEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("TemporalHistoryRetentionEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TemporalHistoryRetentionEnabled", value);
		}
	}

	public DatabaseEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new DatabaseEvents(this);
			}
			return events;
		}
	}

	internal ExecutionManager DatabaseExecutionManager
	{
		get
		{
			if (m_dbExecutionManager == null)
			{
				lock (syncRoot)
				{
					if (m_dbExecutionManager == null)
					{
						ServerConnection connectionContext = GetServerObject().ConnectionContext;
						bool poolConnection = !connectionContext.NonPooledConnection;
						m_dbExecutionManager = new ExecutionManager(GetServerObject().ConnectionContext.GetDatabaseConnection(Name, poolConnection))
						{
							Parent = this
						};
					}
				}
			}
			return m_dbExecutionManager;
		}
	}

	public override ExecutionManager ExecutionManager
	{
		get
		{
			if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				return DatabaseExecutionManager;
			}
			return base.ExecutionManager;
		}
	}

	public override DatabaseEngineType DatabaseEngineType => GetServerObject()?.ExecutionManager.GetDatabaseEngineType() ?? DatabaseEngineType.Standalone;

	public override DatabaseEngineEdition DatabaseEngineEdition
	{
		get
		{
			if (m_edition.HasValue)
			{
				DatabaseEngineEdition? edition = m_edition;
				if (edition.GetValueOrDefault() != DatabaseEngineEdition.Unknown || !edition.HasValue || base.State != SqlSmoState.Existing)
				{
					goto IL_0097;
				}
			}
			try
			{
				m_edition = ExecutionManager.GetDatabaseEngineEdition();
			}
			catch (ConnectionFailureException ex)
			{
				if (!(ex.InnerException is SqlException ex2) || (ex2.Number != 40892 && ((base.State != SqlSmoState.Creating && base.State != SqlSmoState.Pending) || ex2.Number != 4060)))
				{
					throw;
				}
				m_edition = DatabaseEngineEdition.Unknown;
			}
			goto IL_0097;
			IL_0097:
			return m_edition ?? DatabaseEngineEdition.Unknown;
		}
	}

	public static string UrnSuffix => "Database";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	public bool WarnOnRename => true;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool DatabaseOwnershipChaining
	{
		get
		{
			ThrowIfBelowVersion80SP3();
			return (bool)base.Properties["DatabaseOwnershipChaining"].Value;
		}
		set
		{
			ThrowIfBelowVersion80SP3();
			base.Properties.Get("DatabaseOwnershipChaining").Value = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.SqlAzureDatabase)]
	public CatalogCollationType CatalogCollation
	{
		get
		{
			return (CatalogCollationType)base.Properties.GetValueWithNullReplacement("CatalogCollation");
		}
		set
		{
			if (CatalogCollationType.ContainedDatabaseFixedCollation == value)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(ExceptionTemplatesImpl.CantSetContainedDatabaseCatalogCollation));
			}
			base.Properties.SetValueWithConsistencyCheck("CatalogCollation", value);
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public DatabaseOptions DatabaseOptions
	{
		get
		{
			CheckObjectStateImpl(throwIfNotCreated: false);
			if (m_DatabaseOptions == null)
			{
				m_DatabaseOptions = new DatabaseOptions(this, new ObjectKeyBase(), base.State);
			}
			return m_DatabaseOptions;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public QueryStoreOptions QueryStoreOptions
	{
		get
		{
			CheckObjectStateImpl(throwIfNotCreated: false);
			this.ThrowIfNotSupported(typeof(QueryStoreOptions));
			if (m_QueryStoreOptions == null)
			{
				m_QueryStoreOptions = new QueryStoreOptions(this, new ObjectKeyBase(), base.State);
			}
			return m_QueryStoreOptions;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Synonym), SfcObjectFlags.Design)]
	public SynonymCollection Synonyms
	{
		get
		{
			CheckObjectState();
			if (m_Synonyms == null)
			{
				m_Synonyms = new SynonymCollection(this);
			}
			return m_Synonyms;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Sequence))]
	public SequenceCollection Sequences
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(Sequence));
			if (m_Sequences == null)
			{
				m_Sequences = new SequenceCollection(this);
			}
			return m_Sequences;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Table), SfcObjectFlags.Design)]
	public TableCollection Tables
	{
		get
		{
			CheckObjectState();
			if (m_Tables == null)
			{
				m_Tables = new TableCollection(this);
			}
			return m_Tables;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(DatabaseScopedCredential))]
	public DatabaseScopedCredentialCollection DatabaseScopedCredentials
	{
		get
		{
			this.ThrowIfNotSupported(typeof(DatabaseScopedCredential));
			CheckObjectState();
			if (m_DatabaseScopedCredentials == null)
			{
				m_DatabaseScopedCredentials = new DatabaseScopedCredentialCollection(this);
			}
			return m_DatabaseScopedCredentials;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(StoredProcedure), SfcObjectFlags.Design)]
	public StoredProcedureCollection StoredProcedures
	{
		get
		{
			CheckObjectState();
			if (m_StoredProcedures == null)
			{
				m_StoredProcedures = new StoredProcedureCollection(this);
			}
			return m_StoredProcedures;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(SqlAssembly))]
	public SqlAssemblyCollection Assemblies
	{
		get
		{
			this.ThrowIfNotSupported(typeof(SqlAssembly));
			CheckObjectState();
			if (m_SqlAssemblies == null)
			{
				m_SqlAssemblies = new SqlAssemblyCollection(this);
			}
			return m_SqlAssemblies;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ExternalLibrary))]
	public ExternalLibraryCollection ExternalLibraries
	{
		get
		{
			this.ThrowIfNotSupported(typeof(ExternalLibrary));
			CheckObjectState();
			if (m_ExternalLibraries == null)
			{
				m_ExternalLibraries = new ExternalLibraryCollection(this);
			}
			return m_ExternalLibraries;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedType))]
	public UserDefinedTypeCollection UserDefinedTypes
	{
		get
		{
			CheckObjectState();
			if (m_UserDefinedTypes == null)
			{
				m_UserDefinedTypes = new UserDefinedTypeCollection(this);
			}
			return m_UserDefinedTypes;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedAggregate))]
	public UserDefinedAggregateCollection UserDefinedAggregates
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(UserDefinedAggregate));
			if (m_UserDefinedAggregates == null)
			{
				m_UserDefinedAggregates = new UserDefinedAggregateCollection(this);
			}
			return m_UserDefinedAggregates;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(FullTextCatalog))]
	public FullTextCatalogCollection FullTextCatalogs
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(FullTextCatalog));
			if (m_FullTextCatalogs == null)
			{
				m_FullTextCatalogs = new FullTextCatalogCollection(this);
			}
			return m_FullTextCatalogs;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(FullTextStopList))]
	public FullTextStopListCollection FullTextStopLists
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(FullTextStopList));
			if (m_FullTextStopLists == null)
			{
				m_FullTextStopLists = new FullTextStopListCollection(this);
			}
			return m_FullTextStopLists;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(SearchPropertyList))]
	public SearchPropertyListCollection SearchPropertyLists
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(SearchPropertyList));
			if (m_SearchPropertyLists == null)
			{
				m_SearchPropertyLists = new SearchPropertyListCollection(this);
			}
			return m_SearchPropertyLists;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(SecurityPolicy))]
	public SecurityPolicyCollection SecurityPolicies
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(SecurityPolicy));
			if (m_SecurityPolicies == null)
			{
				m_SecurityPolicies = new SecurityPolicyCollection(this);
			}
			return m_SecurityPolicies;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(DatabaseScopedConfiguration))]
	public DatabaseScopedConfigurationCollection DatabaseScopedConfigurations
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(DatabaseScopedConfiguration));
			if (m_DatabaseScopedConfigurations == null)
			{
				m_DatabaseScopedConfigurations = new DatabaseScopedConfigurationCollection(this);
			}
			return m_DatabaseScopedConfigurations;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ExternalDataSource))]
	public ExternalDataSourceCollection ExternalDataSources
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(ExternalDataSource));
			if (m_ExternalDataSources == null)
			{
				m_ExternalDataSources = new ExternalDataSourceCollection(this);
			}
			return m_ExternalDataSources;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ExternalFileFormat))]
	public ExternalFileFormatCollection ExternalFileFormats
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(ExternalFileFormat));
			if (m_ExternalFileFormats == null)
			{
				m_ExternalFileFormats = new ExternalFileFormatCollection(this);
			}
			return m_ExternalFileFormats;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Certificate))]
	public CertificateCollection Certificates
	{
		get
		{
			this.ThrowIfNotSupported(typeof(Certificate));
			CheckObjectState();
			if (certificateCollection == null)
			{
				certificateCollection = new CertificateCollection(this);
			}
			return certificateCollection;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ColumnMasterKey))]
	public ColumnMasterKeyCollection ColumnMasterKeys
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(ColumnMasterKey));
			if (m_ColumnMasterKeys == null)
			{
				m_ColumnMasterKeys = new ColumnMasterKeyCollection(this);
			}
			return m_ColumnMasterKeys;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ColumnEncryptionKey))]
	public ColumnEncryptionKeyCollection ColumnEncryptionKeys
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(ColumnEncryptionKey));
			if (m_ColumnEncryptionKeys == null)
			{
				m_ColumnEncryptionKeys = new ColumnEncryptionKeyCollection(this);
			}
			return m_ColumnEncryptionKeys;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(SymmetricKey))]
	public SymmetricKeyCollection SymmetricKeys
	{
		get
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState();
			if (symmetricKeyCollection == null)
			{
				symmetricKeyCollection = new SymmetricKeyCollection(this);
			}
			return symmetricKeyCollection;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(AsymmetricKey))]
	public AsymmetricKeyCollection AsymmetricKeys
	{
		get
		{
			this.ThrowIfNotSupported(typeof(AsymmetricKey));
			CheckObjectState();
			if (asymmetricKeyCollection == null)
			{
				asymmetricKeyCollection = new AsymmetricKeyCollection(this);
			}
			return asymmetricKeyCollection;
		}
	}

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public DatabaseEncryptionKey DatabaseEncryptionKey
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(DatabaseEncryptionKey));
			if (!databaseEncryptionKeyInitialized)
			{
				m_DatabaseEncryptionKey = InitializeDatabaseEncryptionKey();
				databaseEncryptionKeyInitialized = true;
			}
			return m_DatabaseEncryptionKey;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedStoredProcedure))]
	public ExtendedStoredProcedureCollection ExtendedStoredProcedures
	{
		get
		{
			CheckObjectState();
			if (m_ExtendedStoredProcedures == null)
			{
				m_ExtendedStoredProcedures = new ExtendedStoredProcedureCollection(this);
			}
			return m_ExtendedStoredProcedures;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedFunction), SfcObjectFlags.Design)]
	public UserDefinedFunctionCollection UserDefinedFunctions
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_UserDefinedFunctions == null)
			{
				m_UserDefinedFunctions = new UserDefinedFunctionCollection(this);
			}
			return m_UserDefinedFunctions;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(View), SfcObjectFlags.Design)]
	public ViewCollection Views
	{
		get
		{
			CheckObjectState();
			if (m_Views == null)
			{
				m_Views = new ViewCollection(this);
			}
			return m_Views;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(User), SfcObjectFlags.Design)]
	public UserCollection Users
	{
		get
		{
			CheckObjectState();
			if (m_Users == null)
			{
				m_Users = new UserCollection(this);
			}
			return m_Users;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(DatabaseAuditSpecification))]
	public DatabaseAuditSpecificationCollection DatabaseAuditSpecifications
	{
		get
		{
			this.ThrowIfNotSupported(typeof(DatabaseAuditSpecification));
			CheckObjectState();
			if (databaseAuditSpecifications == null)
			{
				databaseAuditSpecifications = new DatabaseAuditSpecificationCollection(this);
			}
			return databaseAuditSpecifications;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Schema), SfcObjectFlags.Design)]
	public SchemaCollection Schemas
	{
		get
		{
			CheckObjectState();
			if (m_Schemas == null)
			{
				m_Schemas = new SchemaCollection(this);
			}
			return m_Schemas;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(DatabaseRole), SfcObjectFlags.Design)]
	public DatabaseRoleCollection Roles
	{
		get
		{
			CheckObjectState();
			if (m_Roles == null)
			{
				m_Roles = new DatabaseRoleCollection(this);
			}
			return m_Roles;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ApplicationRole))]
	public ApplicationRoleCollection ApplicationRoles
	{
		get
		{
			CheckObjectState();
			if (m_ApplcicationRoles == null)
			{
				m_ApplcicationRoles = new ApplicationRoleCollection(this);
			}
			return m_ApplcicationRoles;
		}
	}

	internal BackupSetCollection BackupSets
	{
		get
		{
			CheckObjectState();
			if (m_BackupSets == null)
			{
				m_BackupSets = new BackupSetCollection(this);
			}
			return m_BackupSets;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(LogFile))]
	public LogFileCollection LogFiles
	{
		get
		{
			CheckObjectState();
			if (m_LogFiles == null)
			{
				m_LogFiles = new LogFileCollection(this);
			}
			return m_LogFiles;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(FileGroup))]
	public FileGroupCollection FileGroups
	{
		get
		{
			CheckObjectState();
			if (m_FileGroups == null)
			{
				m_FileGroups = new FileGroupCollection(this);
			}
			return m_FileGroups;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(PlanGuide))]
	public PlanGuideCollection PlanGuides
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PlanGuide));
			if (m_PlanGuides == null)
			{
				m_PlanGuides = new PlanGuideCollection(this);
			}
			return m_PlanGuides;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Default))]
	public DefaultCollection Defaults
	{
		get
		{
			CheckObjectState();
			if (m_Defaults == null)
			{
				m_Defaults = new DefaultCollection(this);
			}
			return m_Defaults;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(Rule))]
	public RuleCollection Rules
	{
		get
		{
			CheckObjectState();
			if (m_Rules == null)
			{
				m_Rules = new RuleCollection(this);
			}
			return m_Rules;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedDataType), SfcObjectFlags.Design)]
	public UserDefinedDataTypeCollection UserDefinedDataTypes
	{
		get
		{
			CheckObjectState();
			if (m_UserDefinedDataTypes == null)
			{
				m_UserDefinedDataTypes = new UserDefinedDataTypeCollection(this);
			}
			return m_UserDefinedDataTypes;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(UserDefinedTableType), SfcObjectFlags.Design)]
	public UserDefinedTableTypeCollection UserDefinedTableTypes
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(UserDefinedTableType));
			if (m_UserDefinedTableTypes == null)
			{
				m_UserDefinedTableTypes = new UserDefinedTableTypeCollection(this);
			}
			return m_UserDefinedTableTypes;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(XmlSchemaCollection))]
	public XmlSchemaCollectionCollection XmlSchemaCollections
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(XmlSchemaCollection));
			if (m_XmlSchemaCollections == null)
			{
				m_XmlSchemaCollections = new XmlSchemaCollectionCollection(this);
			}
			return m_XmlSchemaCollections;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(PartitionFunction))]
	public PartitionFunctionCollection PartitionFunctions
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PartitionFunction));
			if (m_PartitionFunctions == null)
			{
				m_PartitionFunctions = new PartitionFunctionCollection(this);
			}
			return m_PartitionFunctions;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(PartitionScheme))]
	public PartitionSchemeCollection PartitionSchemes
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PartitionScheme));
			if (m_PartitionSchemes == null)
			{
				m_PartitionSchemes = new PartitionSchemeCollection(this);
			}
			return m_PartitionSchemes;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	[Obsolete]
	public DatabaseActiveDirectory ActiveDirectory
	{
		get
		{
			ThrowIfAboveVersion100();
			this.ThrowIfNotSupported(typeof(DatabaseActiveDirectory));
			if (databaseActiveDirectory == null)
			{
				databaseActiveDirectory = new DatabaseActiveDirectory(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return databaseActiveDirectory;
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.ZeroToOne)]
	public MasterKey MasterKey
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(MasterKey));
			if (!masterKeyInitialized && !base.IsDesignMode)
			{
				masterKey = InitializeMasterKey();
				masterKeyInitialized = true;
			}
			return masterKey;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(DatabaseDdlTrigger))]
	public DatabaseDdlTriggerCollection Triggers
	{
		get
		{
			this.ThrowIfNotSupported(typeof(DatabaseDdlTrigger));
			if (databaseDdlTriggerCollection == null)
			{
				databaseDdlTriggerCollection = new DatabaseDdlTriggerCollection(this);
			}
			return databaseDdlTriggerCollection;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DefaultLanguage DefaultLanguage
	{
		get
		{
			ThrowIfCloudProp("DefaultLanguage");
			ThrowIfBelowVersion110Prop("DefaultLanguage");
			if (defaultLanguageObj == null)
			{
				defaultLanguageObj = new DefaultLanguage(this, "DefaultLanguage");
			}
			return defaultLanguageObj;
		}
		internal set
		{
			ThrowIfCloudProp("DefaultLanguage");
			ThrowIfBelowVersion110Prop("DefaultLanguage");
			if (value.IsProperlyInitialized())
			{
				defaultLanguageObj = value;
			}
			else
			{
				defaultLanguageObj = value.Copy(this, "DefaultLanguage");
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DefaultLanguage DefaultFullTextLanguage
	{
		get
		{
			ThrowIfCloudProp("DefaultFullTextLanguage");
			ThrowIfBelowVersion110Prop("DefaultFullTextLanguage");
			if (defaultFullTextLanguageObj == null)
			{
				defaultFullTextLanguageObj = new DefaultLanguage(this, "DefaultFullTextLanguage");
			}
			return defaultFullTextLanguageObj;
		}
		internal set
		{
			ThrowIfCloudProp("DefaultFullTextLanguage");
			ThrowIfBelowVersion110Prop("DefaultFullTextLanguage");
			if (value.IsProperlyInitialized())
			{
				defaultFullTextLanguageObj = value;
			}
			else
			{
				defaultFullTextLanguageObj = value.Copy(this, "DefaultFullTextLanguage");
			}
		}
	}

	[SfcObject(SfcObjectRelationship.Object, SfcObjectCardinality.One)]
	public ServiceBroker ServiceBroker
	{
		get
		{
			this.ThrowIfNotSupported(typeof(ServiceBroker));
			if (m_ServiceBroker == null)
			{
				m_ServiceBroker = new ServiceBroker(this, new ObjectKeyBase(), SqlSmoState.Existing);
			}
			return m_ServiceBroker;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int MaxDop
	{
		get
		{
			ThrowIfPropertyNotSupported("MaxDop");
			return Convert.ToInt32(DatabaseScopedConfigurations["MAXDOP"].Value);
		}
		set
		{
			ThrowIfPropertyNotSupported("MaxDop");
			DatabaseScopedConfigurations["MAXDOP"].Value = value.ToString();
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int? MaxDopForSecondary
	{
		get
		{
			ThrowIfPropertyNotSupported("MaxDopForSecondary");
			if (string.Compare(DatabaseScopedConfigurations["MAXDOP"].ValueForSecondary, "PRIMARY", ignoreCase: true) == 0)
			{
				return null;
			}
			return Convert.ToInt32(DatabaseScopedConfigurations["MAXDOP"].ValueForSecondary);
		}
		set
		{
			ThrowIfPropertyNotSupported("MaxDopForSecondary");
			DatabaseScopedConfigurations["MAXDOP"].ValueForSecondary = ((!value.HasValue) ? "PRIMARY" : value.ToString());
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseScopedConfigurationOnOff LegacyCardinalityEstimation
	{
		get
		{
			ThrowIfPropertyNotSupported("LegacyCardinalityEstimation");
			return (DatabaseScopedConfigurationOnOff)Enum.Parse(typeof(DatabaseScopedConfigurationOnOff), DatabaseScopedConfigurations["Legacy_Cardinality_Estimation"].Value, ignoreCase: true);
		}
		set
		{
			ThrowIfPropertyNotSupported("LegacyCardinalityEstimation");
			DatabaseScopedConfigurations["Legacy_Cardinality_Estimation"].Value = value.ToString();
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseScopedConfigurationOnOff LegacyCardinalityEstimationForSecondary
	{
		get
		{
			ThrowIfPropertyNotSupported("LegacyCardinalityEstimationForSecondary");
			return (DatabaseScopedConfigurationOnOff)Enum.Parse(typeof(DatabaseScopedConfigurationOnOff), DatabaseScopedConfigurations["Legacy_Cardinality_Estimation"].ValueForSecondary, ignoreCase: true);
		}
		set
		{
			ThrowIfPropertyNotSupported("LegacyCardinalityEstimationForSecondary");
			DatabaseScopedConfigurations["Legacy_Cardinality_Estimation"].ValueForSecondary = value.ToString();
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseScopedConfigurationOnOff ParameterSniffing
	{
		get
		{
			ThrowIfPropertyNotSupported("ParameterSniffing");
			return (DatabaseScopedConfigurationOnOff)Enum.Parse(typeof(DatabaseScopedConfigurationOnOff), DatabaseScopedConfigurations["Parameter_Sniffing"].Value, ignoreCase: true);
		}
		set
		{
			ThrowIfPropertyNotSupported("ParameterSniffing");
			DatabaseScopedConfigurations["Parameter_Sniffing"].Value = value.ToString();
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseScopedConfigurationOnOff ParameterSniffingForSecondary
	{
		get
		{
			ThrowIfPropertyNotSupported("ParameterSniffingForSecondary");
			return (DatabaseScopedConfigurationOnOff)Enum.Parse(typeof(DatabaseScopedConfigurationOnOff), DatabaseScopedConfigurations["Parameter_Sniffing"].ValueForSecondary, ignoreCase: true);
		}
		set
		{
			ThrowIfPropertyNotSupported("ParameterSniffingForSecondary");
			DatabaseScopedConfigurations["Parameter_Sniffing"].ValueForSecondary = value.ToString();
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseScopedConfigurationOnOff QueryOptimizerHotfixes
	{
		get
		{
			ThrowIfPropertyNotSupported("QueryOptimizerHotfixes");
			return (DatabaseScopedConfigurationOnOff)Enum.Parse(typeof(DatabaseScopedConfigurationOnOff), DatabaseScopedConfigurations["Query_Optimizer_Hotfixes"].Value, ignoreCase: true);
		}
		set
		{
			ThrowIfPropertyNotSupported("QueryOptimizerHotfixes");
			DatabaseScopedConfigurations["Query_Optimizer_Hotfixes"].Value = value.ToString();
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseScopedConfigurationOnOff QueryOptimizerHotfixesForSecondary
	{
		get
		{
			ThrowIfPropertyNotSupported("QueryOptimizerHotfixesForSecondary");
			return (DatabaseScopedConfigurationOnOff)Enum.Parse(typeof(DatabaseScopedConfigurationOnOff), DatabaseScopedConfigurations["Query_Optimizer_Hotfixes"].ValueForSecondary, ignoreCase: true);
		}
		set
		{
			ThrowIfPropertyNotSupported("QueryOptimizerHotfixesForSecondary");
			DatabaseScopedConfigurations["Query_Optimizer_Hotfixes"].ValueForSecondary = value.ToString();
		}
	}

	public bool IsVarDecimalStorageFormatSupported
	{
		get
		{
			Version version = new Version(9, 0, 3003);
			Version version2 = new Version(Parent.ConnectionContext.ServerVersion.Major, Parent.ConnectionContext.ServerVersion.Minor, Parent.ConnectionContext.ServerVersion.BuildNumber);
			if (base.IsDesignMode)
			{
				return version2 > version;
			}
			if (version2 > version)
			{
				if (Parent.Information.EngineEdition != Edition.EnterpriseOrDeveloper)
				{
					return Parent.Information.EngineEdition == Edition.SqlManagedInstance;
				}
				return true;
			}
			return false;
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.Deploy)]
	public bool IsVarDecimalStorageFormatEnabled
	{
		get
		{
			return IsVarDecimalStorageFormatSupported && (bool)base.Properties.GetValueWithNullReplacement("IsVarDecimalStorageFormatEnabled");
		}
		set
		{
			if (!IsVarDecimalStorageFormatSupported)
			{
				throw new PropertyNotAvailableException(ExceptionTemplatesImpl.ReasonPropertyIsNotSupportedOnCurrentServerVersion);
			}
			base.Properties.SetValueWithConsistencyCheck("IsVarDecimalStorageFormatEnabled", value);
		}
	}

	public Database()
	{
	}

	public Database(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	object IPropertyDataDispatch.GetPropertyValue(int index)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				return index switch
				{
					0 => XRuntime.AnsiNullDefault, 
					1 => XRuntime.AnsiNullsEnabled, 
					2 => XRuntime.AnsiPaddingEnabled, 
					3 => XRuntime.AnsiWarningsEnabled, 
					4 => XRuntime.ArithmeticAbortEnabled, 
					5 => XRuntime.AutoCreateIncrementalStatisticsEnabled, 
					6 => XRuntime.AutoCreateStatisticsEnabled, 
					7 => XRuntime.AutoShrink, 
					8 => XRuntime.AutoUpdateStatisticsAsync, 
					9 => XRuntime.AutoUpdateStatisticsEnabled, 
					10 => XRuntime.AzureEdition, 
					11 => XRuntime.AzureServiceObjective, 
					12 => XRuntime.CaseSensitive, 
					13 => XRuntime.CloseCursorsOnCommitEnabled, 
					14 => XSchema.Collation, 
					15 => XRuntime.CompatibilityLevel, 
					16 => XRuntime.ConcatenateNullYieldsNull, 
					17 => XRuntime.CreateDate, 
					18 => XRuntime.DatabaseOwnershipChaining, 
					19 => XSchema.DatabaseSnapshotBaseName, 
					20 => XRuntime.DateCorrelationOptimization, 
					21 => XRuntime.DboLogin, 
					22 => XRuntime.DefaultFileGroup, 
					23 => XSchema.DefaultSchema, 
					24 => XRuntime.EncryptionEnabled, 
					25 => XRuntime.HonorBrokerPriority, 
					26 => XRuntime.ID, 
					27 => XRuntime.IsAccessible, 
					28 => XRuntime.IsDatabaseSnapshot, 
					29 => XRuntime.IsDatabaseSnapshotBase, 
					30 => XRuntime.IsDbAccessAdmin, 
					31 => XRuntime.IsDbBackupOperator, 
					32 => XRuntime.IsDbDatareader, 
					33 => XRuntime.IsDbDatawriter, 
					34 => XRuntime.IsDbDdlAdmin, 
					35 => XRuntime.IsDbDenyDatareader, 
					36 => XRuntime.IsDbDenyDatawriter, 
					37 => XRuntime.IsDbManager, 
					38 => XRuntime.IsDbOwner, 
					39 => XRuntime.IsDbSecurityAdmin, 
					40 => XRuntime.IsFullTextEnabled, 
					41 => XRuntime.IsLoginManager, 
					42 => XRuntime.IsMirroringEnabled, 
					43 => XRuntime.IsParameterizationForced, 
					44 => XRuntime.IsReadCommittedSnapshotOn, 
					45 => XRuntime.IsSqlDw, 
					46 => XRuntime.IsSystemObject, 
					47 => XRuntime.IsUpdateable, 
					48 => XSchema.LastGoodCheckDbTime, 
					49 => XRuntime.MaxSizeInBytes, 
					50 => XRuntime.NumericRoundAbortEnabled, 
					51 => XRuntime.Owner, 
					52 => XRuntime.QuotedIdentifiersEnabled, 
					53 => XRuntime.ReadOnly, 
					54 => XRuntime.RecursiveTriggersEnabled, 
					55 => XRuntime.ReplicationOptions, 
					56 => XRuntime.ServiceBrokerGuid, 
					57 => XRuntime.Size, 
					58 => XRuntime.SnapshotIsolationState, 
					59 => XRuntime.SpaceAvailable, 
					60 => XRuntime.Status, 
					61 => XRuntime.Trustworthy, 
					62 => XRuntime.UserAccess, 
					63 => XRuntime.UserName, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			return index switch
			{
				0 => XRuntime.AnsiNullDefault, 
				1 => XRuntime.AnsiNullsEnabled, 
				2 => XRuntime.AnsiPaddingEnabled, 
				3 => XRuntime.AnsiWarningsEnabled, 
				4 => XRuntime.ArithmeticAbortEnabled, 
				56 => XRuntime.AutoCreateIncrementalStatisticsEnabled, 
				5 => XRuntime.AutoCreateStatisticsEnabled, 
				6 => XRuntime.AutoShrink, 
				7 => XRuntime.AutoUpdateStatisticsAsync, 
				8 => XRuntime.AutoUpdateStatisticsEnabled, 
				9 => XRuntime.AzureEdition, 
				57 => XRuntime.AzureServiceObjective, 
				10 => XRuntime.CaseSensitive, 
				58 => XRuntime.CatalogCollation, 
				59 => XRuntime.ChangeTrackingAutoCleanUp, 
				60 => XRuntime.ChangeTrackingEnabled, 
				61 => XRuntime.ChangeTrackingRetentionPeriod, 
				62 => XRuntime.ChangeTrackingRetentionPeriodUnits, 
				11 => XRuntime.CloseCursorsOnCommitEnabled, 
				12 => XSchema.Collation, 
				13 => XRuntime.CompatibilityLevel, 
				14 => XRuntime.ConcatenateNullYieldsNull, 
				15 => XRuntime.CreateDate, 
				16 => XRuntime.DatabaseOwnershipChaining, 
				17 => XSchema.DatabaseSnapshotBaseName, 
				18 => XRuntime.DateCorrelationOptimization, 
				19 => XRuntime.DboLogin, 
				63 => XRuntime.DefaultFileGroup, 
				64 => XRuntime.DefaultFullTextCatalog, 
				20 => XSchema.DefaultSchema, 
				65 => XRuntime.EncryptionEnabled, 
				66 => XRuntime.HasFileInCloud, 
				67 => XRuntime.HasMemoryOptimizedObjects, 
				68 => XRuntime.HonorBrokerPriority, 
				21 => XRuntime.ID, 
				69 => XRuntime.IsAccessible, 
				22 => XRuntime.IsDatabaseSnapshot, 
				23 => XRuntime.IsDatabaseSnapshotBase, 
				24 => XRuntime.IsDbAccessAdmin, 
				25 => XRuntime.IsDbBackupOperator, 
				26 => XRuntime.IsDbDatareader, 
				27 => XRuntime.IsDbDatawriter, 
				28 => XRuntime.IsDbDdlAdmin, 
				29 => XRuntime.IsDbDenyDatareader, 
				30 => XRuntime.IsDbDenyDatawriter, 
				31 => XRuntime.IsDbManager, 
				32 => XRuntime.IsDbOwner, 
				33 => XRuntime.IsDbSecurityAdmin, 
				34 => XRuntime.IsFullTextEnabled, 
				35 => XRuntime.IsLoginManager, 
				70 => XRuntime.IsMirroringEnabled, 
				36 => XRuntime.IsParameterizationForced, 
				37 => XRuntime.IsReadCommittedSnapshotOn, 
				38 => XRuntime.IsSqlDw, 
				71 => XRuntime.IsSqlDwEdition, 
				39 => XRuntime.IsSystemObject, 
				40 => XRuntime.IsUpdateable, 
				72 => XSchema.LastGoodCheckDbTime, 
				73 => XRuntime.LegacyCardinalityEstimation, 
				74 => XRuntime.LegacyCardinalityEstimationForSecondary, 
				75 => XRuntime.MaxDop, 
				76 => XRuntime.MaxDopForSecondary, 
				41 => XRuntime.MaxSizeInBytes, 
				77 => XRuntime.MemoryAllocatedToMemoryOptimizedObjectsInKB, 
				78 => XRuntime.MemoryUsedByMemoryOptimizedObjectsInKB, 
				42 => XRuntime.NumericRoundAbortEnabled, 
				43 => XRuntime.Owner, 
				79 => XRuntime.ParameterSniffing, 
				80 => XRuntime.ParameterSniffingForSecondary, 
				81 => XRuntime.QueryOptimizerHotfixes, 
				82 => XRuntime.QueryOptimizerHotfixesForSecondary, 
				44 => XRuntime.QuotedIdentifiersEnabled, 
				45 => XRuntime.ReadOnly, 
				46 => XRuntime.RecursiveTriggersEnabled, 
				47 => XRuntime.ReplicationOptions, 
				48 => XRuntime.ServiceBrokerGuid, 
				49 => XRuntime.Size, 
				50 => XRuntime.SnapshotIsolationState, 
				51 => XRuntime.SpaceAvailable, 
				52 => XRuntime.Status, 
				83 => XRuntime.TemporalHistoryRetentionEnabled, 
				53 => XRuntime.Trustworthy, 
				54 => XRuntime.UserAccess, 
				55 => XRuntime.UserName, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			0 => XRuntime.ActiveConnections, 
			32 => XRuntime.AnsiNullDefault, 
			33 => XRuntime.AnsiNullsEnabled, 
			34 => XRuntime.AnsiPaddingEnabled, 
			35 => XRuntime.AnsiWarningsEnabled, 
			36 => XRuntime.ArithmeticAbortEnabled, 
			1 => XRuntime.AutoClose, 
			112 => XRuntime.AutoCreateIncrementalStatisticsEnabled, 
			37 => XRuntime.AutoCreateStatisticsEnabled, 
			2 => XRuntime.AutoShrink, 
			58 => XRuntime.AutoUpdateStatisticsAsync, 
			38 => XRuntime.AutoUpdateStatisticsEnabled, 
			98 => XSchema.AvailabilityDatabaseSynchronizationState, 
			99 => XSchema.AvailabilityGroupName, 
			59 => XRuntime.BrokerEnabled, 
			39 => XRuntime.CaseSensitive, 
			132 => XRuntime.CatalogCollation, 
			89 => XRuntime.ChangeTrackingAutoCleanUp, 
			90 => XRuntime.ChangeTrackingEnabled, 
			91 => XRuntime.ChangeTrackingRetentionPeriod, 
			92 => XRuntime.ChangeTrackingRetentionPeriodUnits, 
			40 => XRuntime.CloseCursorsOnCommitEnabled, 
			41 => XSchema.Collation, 
			3 => XRuntime.CompatibilityLevel, 
			42 => XRuntime.ConcatenateNullYieldsNull, 
			100 => XRuntime.ContainmentType, 
			4 => XRuntime.CreateDate, 
			60 => XRuntime.DatabaseGuid, 
			43 => XRuntime.DatabaseOwnershipChaining, 
			61 => XSchema.DatabaseSnapshotBaseName, 
			5 => XRuntime.DataSpaceUsage, 
			62 => XRuntime.DateCorrelationOptimization, 
			6 => XRuntime.DboLogin, 
			7 => XRuntime.DefaultFileGroup, 
			93 => XRuntime.DefaultFileStreamFileGroup, 
			63 => XRuntime.DefaultFullTextCatalog, 
			101 => XRuntime.DefaultFullTextLanguageLcid, 
			102 => XRuntime.DefaultFullTextLanguageName, 
			103 => XRuntime.DefaultLanguageLcid, 
			104 => XRuntime.DefaultLanguageName, 
			8 => XSchema.DefaultSchema, 
			113 => XRuntime.DelayedDurability, 
			94 => XRuntime.EncryptionEnabled, 
			105 => XRuntime.FilestreamDirectoryName, 
			106 => XRuntime.FilestreamNonTransactedAccess, 
			107 => XRuntime.HasDatabaseEncryptionKey, 
			114 => XRuntime.HasFileInCloud, 
			64 => XRuntime.HasFullBackup, 
			115 => XRuntime.HasMemoryOptimizedObjects, 
			95 => XRuntime.HonorBrokerPriority, 
			9 => XRuntime.ID, 
			10 => XRuntime.IndexSpaceUsage, 
			11 => XRuntime.IsAccessible, 
			65 => XRuntime.IsDatabaseSnapshot, 
			66 => XRuntime.IsDatabaseSnapshotBase, 
			12 => XRuntime.IsDbAccessAdmin, 
			13 => XRuntime.IsDbBackupOperator, 
			14 => XRuntime.IsDbDatareader, 
			15 => XRuntime.IsDbDatawriter, 
			16 => XRuntime.IsDbDdlAdmin, 
			17 => XRuntime.IsDbDenyDatareader, 
			18 => XRuntime.IsDbDenyDatawriter, 
			19 => XRuntime.IsDbOwner, 
			20 => XRuntime.IsDbSecurityAdmin, 
			21 => XRuntime.IsFullTextEnabled, 
			67 => XRuntime.IsMailHost, 
			96 => XRuntime.IsManagementDataWarehouse, 
			22 => XRuntime.IsMirroringEnabled, 
			68 => XRuntime.IsParameterizationForced, 
			69 => XRuntime.IsReadCommittedSnapshotOn, 
			23 => XRuntime.IsSqlDw, 
			24 => XRuntime.IsSystemObject, 
			44 => XRuntime.IsUpdateable, 
			70 => XRuntime.IsVarDecimalStorageFormatEnabled, 
			45 => XRuntime.LastBackupDate, 
			46 => XSchema.LastDifferentialBackupDate, 
			47 => XSchema.LastGoodCheckDbTime, 
			48 => XRuntime.LastLogBackupDate, 
			118 => XRuntime.LegacyCardinalityEstimation, 
			119 => XRuntime.LegacyCardinalityEstimationForSecondary, 
			49 => XRuntime.LocalCursorsDefault, 
			71 => XRuntime.LogReuseWaitStatus, 
			120 => XRuntime.MaxDop, 
			121 => XRuntime.MaxDopForSecondary, 
			116 => XRuntime.MemoryAllocatedToMemoryOptimizedObjectsInKB, 
			117 => XRuntime.MemoryUsedByMemoryOptimizedObjectsInKB, 
			72 => XRuntime.MirroringFailoverLogSequenceNumber, 
			73 => XRuntime.MirroringID, 
			74 => XRuntime.MirroringPartner, 
			75 => XRuntime.MirroringPartnerInstance, 
			76 => XRuntime.MirroringRedoQueueMaxSize, 
			77 => XRuntime.MirroringRole, 
			78 => XRuntime.MirroringRoleSequence, 
			79 => XRuntime.MirroringSafetyLevel, 
			80 => XRuntime.MirroringSafetySequence, 
			81 => XRuntime.MirroringStatus, 
			82 => XRuntime.MirroringTimeout, 
			83 => XRuntime.MirroringWitness, 
			84 => XRuntime.MirroringWitnessStatus, 
			108 => XRuntime.NestedTriggersEnabled, 
			50 => XRuntime.NumericRoundAbortEnabled, 
			25 => XRuntime.Owner, 
			51 => XRuntime.PageVerify, 
			122 => XRuntime.ParameterSniffing, 
			123 => XRuntime.ParameterSniffingForSecondary, 
			97 => XRuntime.PolicyHealthState, 
			26 => XRuntime.PrimaryFilePath, 
			124 => XRuntime.QueryOptimizerHotfixes, 
			125 => XRuntime.QueryOptimizerHotfixesForSecondary, 
			52 => XRuntime.QuotedIdentifiersEnabled, 
			53 => XRuntime.ReadOnly, 
			85 => XRuntime.RecoveryForkGuid, 
			54 => XRuntime.RecoveryModel, 
			55 => XRuntime.RecursiveTriggersEnabled, 
			126 => XRuntime.RemoteDataArchiveCredential, 
			127 => XRuntime.RemoteDataArchiveEnabled, 
			128 => XRuntime.RemoteDataArchiveEndpoint, 
			129 => XRuntime.RemoteDataArchiveLinkedServer, 
			130 => XRuntime.RemoteDataArchiveUseFederatedServiceAccount, 
			131 => XRuntime.RemoteDatabaseName, 
			27 => XRuntime.ReplicationOptions, 
			86 => XRuntime.ServiceBrokerGuid, 
			28 => XRuntime.Size, 
			87 => XRuntime.SnapshotIsolationState, 
			29 => XRuntime.SpaceAvailable, 
			30 => XRuntime.Status, 
			109 => XRuntime.TargetRecoveryTime, 
			110 => XRuntime.TransformNoiseWords, 
			88 => XRuntime.Trustworthy, 
			111 => XRuntime.TwoDigitYearCutoff, 
			56 => XRuntime.UserAccess, 
			31 => XRuntime.UserName, 
			57 => XRuntime.Version, 
			_ => throw new IndexOutOfRangeException(), 
		};
	}

	void IPropertyDataDispatch.SetPropertyValue(int index, object value)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				switch (index)
				{
				case 0:
					XRuntime.AnsiNullDefault = (bool)value;
					break;
				case 1:
					XRuntime.AnsiNullsEnabled = (bool)value;
					break;
				case 2:
					XRuntime.AnsiPaddingEnabled = (bool)value;
					break;
				case 3:
					XRuntime.AnsiWarningsEnabled = (bool)value;
					break;
				case 4:
					XRuntime.ArithmeticAbortEnabled = (bool)value;
					break;
				case 5:
					XRuntime.AutoCreateIncrementalStatisticsEnabled = (bool)value;
					break;
				case 6:
					XRuntime.AutoCreateStatisticsEnabled = (bool)value;
					break;
				case 7:
					XRuntime.AutoShrink = (bool)value;
					break;
				case 8:
					XRuntime.AutoUpdateStatisticsAsync = (bool)value;
					break;
				case 9:
					XRuntime.AutoUpdateStatisticsEnabled = (bool)value;
					break;
				case 10:
					XRuntime.AzureEdition = (string)value;
					break;
				case 11:
					XRuntime.AzureServiceObjective = (string)value;
					break;
				case 12:
					XRuntime.CaseSensitive = (bool)value;
					break;
				case 13:
					XRuntime.CloseCursorsOnCommitEnabled = (bool)value;
					break;
				case 14:
					XSchema.Collation = (string)value;
					break;
				case 15:
					XRuntime.CompatibilityLevel = (CompatibilityLevel)value;
					break;
				case 16:
					XRuntime.ConcatenateNullYieldsNull = (bool)value;
					break;
				case 17:
					XRuntime.CreateDate = (DateTime)value;
					break;
				case 18:
					XRuntime.DatabaseOwnershipChaining = (bool)value;
					break;
				case 19:
					XSchema.DatabaseSnapshotBaseName = (string)value;
					break;
				case 20:
					XRuntime.DateCorrelationOptimization = (bool)value;
					break;
				case 21:
					XRuntime.DboLogin = (bool)value;
					break;
				case 22:
					XRuntime.DefaultFileGroup = (string)value;
					break;
				case 23:
					XSchema.DefaultSchema = (string)value;
					break;
				case 24:
					XRuntime.EncryptionEnabled = (bool)value;
					break;
				case 25:
					XRuntime.HonorBrokerPriority = (bool)value;
					break;
				case 26:
					XRuntime.ID = (int)value;
					break;
				case 27:
					XRuntime.IsAccessible = (bool)value;
					break;
				case 28:
					XRuntime.IsDatabaseSnapshot = (bool)value;
					break;
				case 29:
					XRuntime.IsDatabaseSnapshotBase = (bool)value;
					break;
				case 30:
					XRuntime.IsDbAccessAdmin = (bool)value;
					break;
				case 31:
					XRuntime.IsDbBackupOperator = (bool)value;
					break;
				case 32:
					XRuntime.IsDbDatareader = (bool)value;
					break;
				case 33:
					XRuntime.IsDbDatawriter = (bool)value;
					break;
				case 34:
					XRuntime.IsDbDdlAdmin = (bool)value;
					break;
				case 35:
					XRuntime.IsDbDenyDatareader = (bool)value;
					break;
				case 36:
					XRuntime.IsDbDenyDatawriter = (bool)value;
					break;
				case 37:
					XRuntime.IsDbManager = (bool)value;
					break;
				case 38:
					XRuntime.IsDbOwner = (bool)value;
					break;
				case 39:
					XRuntime.IsDbSecurityAdmin = (bool)value;
					break;
				case 40:
					XRuntime.IsFullTextEnabled = (bool)value;
					break;
				case 41:
					XRuntime.IsLoginManager = (bool)value;
					break;
				case 42:
					XRuntime.IsMirroringEnabled = (bool)value;
					break;
				case 43:
					XRuntime.IsParameterizationForced = (bool)value;
					break;
				case 44:
					XRuntime.IsReadCommittedSnapshotOn = (bool)value;
					break;
				case 45:
					XRuntime.IsSqlDw = (bool)value;
					break;
				case 46:
					XRuntime.IsSystemObject = (bool)value;
					break;
				case 47:
					XRuntime.IsUpdateable = (bool)value;
					break;
				case 48:
					XSchema.LastGoodCheckDbTime = (DateTime)value;
					break;
				case 49:
					XRuntime.MaxSizeInBytes = (double)value;
					break;
				case 50:
					XRuntime.NumericRoundAbortEnabled = (bool)value;
					break;
				case 51:
					XRuntime.Owner = (string)value;
					break;
				case 52:
					XRuntime.QuotedIdentifiersEnabled = (bool)value;
					break;
				case 53:
					XRuntime.ReadOnly = (bool)value;
					break;
				case 54:
					XRuntime.RecursiveTriggersEnabled = (bool)value;
					break;
				case 55:
					XRuntime.ReplicationOptions = (ReplicationOptions)value;
					break;
				case 56:
					XRuntime.ServiceBrokerGuid = (Guid)value;
					break;
				case 57:
					XRuntime.Size = (double)value;
					break;
				case 58:
					XRuntime.SnapshotIsolationState = (SnapshotIsolationState)value;
					break;
				case 59:
					XRuntime.SpaceAvailable = (double)value;
					break;
				case 60:
					XRuntime.Status = (DatabaseStatus)value;
					break;
				case 61:
					XRuntime.Trustworthy = (bool)value;
					break;
				case 62:
					XRuntime.UserAccess = (DatabaseUserAccess)value;
					break;
				case 63:
					XRuntime.UserName = (string)value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return;
			}
			switch (index)
			{
			case 0:
				XRuntime.AnsiNullDefault = (bool)value;
				break;
			case 1:
				XRuntime.AnsiNullsEnabled = (bool)value;
				break;
			case 2:
				XRuntime.AnsiPaddingEnabled = (bool)value;
				break;
			case 3:
				XRuntime.AnsiWarningsEnabled = (bool)value;
				break;
			case 4:
				XRuntime.ArithmeticAbortEnabled = (bool)value;
				break;
			case 56:
				XRuntime.AutoCreateIncrementalStatisticsEnabled = (bool)value;
				break;
			case 5:
				XRuntime.AutoCreateStatisticsEnabled = (bool)value;
				break;
			case 6:
				XRuntime.AutoShrink = (bool)value;
				break;
			case 7:
				XRuntime.AutoUpdateStatisticsAsync = (bool)value;
				break;
			case 8:
				XRuntime.AutoUpdateStatisticsEnabled = (bool)value;
				break;
			case 9:
				XRuntime.AzureEdition = (string)value;
				break;
			case 57:
				XRuntime.AzureServiceObjective = (string)value;
				break;
			case 10:
				XRuntime.CaseSensitive = (bool)value;
				break;
			case 58:
				XRuntime.CatalogCollation = (CatalogCollationType)value;
				break;
			case 59:
				XRuntime.ChangeTrackingAutoCleanUp = (bool)value;
				break;
			case 60:
				XRuntime.ChangeTrackingEnabled = (bool)value;
				break;
			case 61:
				XRuntime.ChangeTrackingRetentionPeriod = (int)value;
				break;
			case 62:
				XRuntime.ChangeTrackingRetentionPeriodUnits = (RetentionPeriodUnits)value;
				break;
			case 11:
				XRuntime.CloseCursorsOnCommitEnabled = (bool)value;
				break;
			case 12:
				XSchema.Collation = (string)value;
				break;
			case 13:
				XRuntime.CompatibilityLevel = (CompatibilityLevel)value;
				break;
			case 14:
				XRuntime.ConcatenateNullYieldsNull = (bool)value;
				break;
			case 15:
				XRuntime.CreateDate = (DateTime)value;
				break;
			case 16:
				XRuntime.DatabaseOwnershipChaining = (bool)value;
				break;
			case 17:
				XSchema.DatabaseSnapshotBaseName = (string)value;
				break;
			case 18:
				XRuntime.DateCorrelationOptimization = (bool)value;
				break;
			case 19:
				XRuntime.DboLogin = (bool)value;
				break;
			case 63:
				XRuntime.DefaultFileGroup = (string)value;
				break;
			case 64:
				XRuntime.DefaultFullTextCatalog = (string)value;
				break;
			case 20:
				XSchema.DefaultSchema = (string)value;
				break;
			case 65:
				XRuntime.EncryptionEnabled = (bool)value;
				break;
			case 66:
				XRuntime.HasFileInCloud = (bool)value;
				break;
			case 67:
				XRuntime.HasMemoryOptimizedObjects = (bool)value;
				break;
			case 68:
				XRuntime.HonorBrokerPriority = (bool)value;
				break;
			case 21:
				XRuntime.ID = (int)value;
				break;
			case 69:
				XRuntime.IsAccessible = (bool)value;
				break;
			case 22:
				XRuntime.IsDatabaseSnapshot = (bool)value;
				break;
			case 23:
				XRuntime.IsDatabaseSnapshotBase = (bool)value;
				break;
			case 24:
				XRuntime.IsDbAccessAdmin = (bool)value;
				break;
			case 25:
				XRuntime.IsDbBackupOperator = (bool)value;
				break;
			case 26:
				XRuntime.IsDbDatareader = (bool)value;
				break;
			case 27:
				XRuntime.IsDbDatawriter = (bool)value;
				break;
			case 28:
				XRuntime.IsDbDdlAdmin = (bool)value;
				break;
			case 29:
				XRuntime.IsDbDenyDatareader = (bool)value;
				break;
			case 30:
				XRuntime.IsDbDenyDatawriter = (bool)value;
				break;
			case 31:
				XRuntime.IsDbManager = (bool)value;
				break;
			case 32:
				XRuntime.IsDbOwner = (bool)value;
				break;
			case 33:
				XRuntime.IsDbSecurityAdmin = (bool)value;
				break;
			case 34:
				XRuntime.IsFullTextEnabled = (bool)value;
				break;
			case 35:
				XRuntime.IsLoginManager = (bool)value;
				break;
			case 70:
				XRuntime.IsMirroringEnabled = (bool)value;
				break;
			case 36:
				XRuntime.IsParameterizationForced = (bool)value;
				break;
			case 37:
				XRuntime.IsReadCommittedSnapshotOn = (bool)value;
				break;
			case 38:
				XRuntime.IsSqlDw = (bool)value;
				break;
			case 71:
				XRuntime.IsSqlDwEdition = (bool)value;
				break;
			case 39:
				XRuntime.IsSystemObject = (bool)value;
				break;
			case 40:
				XRuntime.IsUpdateable = (bool)value;
				break;
			case 72:
				XSchema.LastGoodCheckDbTime = (DateTime)value;
				break;
			case 73:
				XRuntime.LegacyCardinalityEstimation = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 74:
				XRuntime.LegacyCardinalityEstimationForSecondary = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 75:
				XRuntime.MaxDop = (int)value;
				break;
			case 76:
				XRuntime.MaxDopForSecondary = (int)value;
				break;
			case 41:
				XRuntime.MaxSizeInBytes = (double)value;
				break;
			case 77:
				XRuntime.MemoryAllocatedToMemoryOptimizedObjectsInKB = (double)value;
				break;
			case 78:
				XRuntime.MemoryUsedByMemoryOptimizedObjectsInKB = (double)value;
				break;
			case 42:
				XRuntime.NumericRoundAbortEnabled = (bool)value;
				break;
			case 43:
				XRuntime.Owner = (string)value;
				break;
			case 79:
				XRuntime.ParameterSniffing = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 80:
				XRuntime.ParameterSniffingForSecondary = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 81:
				XRuntime.QueryOptimizerHotfixes = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 82:
				XRuntime.QueryOptimizerHotfixesForSecondary = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 44:
				XRuntime.QuotedIdentifiersEnabled = (bool)value;
				break;
			case 45:
				XRuntime.ReadOnly = (bool)value;
				break;
			case 46:
				XRuntime.RecursiveTriggersEnabled = (bool)value;
				break;
			case 47:
				XRuntime.ReplicationOptions = (ReplicationOptions)value;
				break;
			case 48:
				XRuntime.ServiceBrokerGuid = (Guid)value;
				break;
			case 49:
				XRuntime.Size = (double)value;
				break;
			case 50:
				XRuntime.SnapshotIsolationState = (SnapshotIsolationState)value;
				break;
			case 51:
				XRuntime.SpaceAvailable = (double)value;
				break;
			case 52:
				XRuntime.Status = (DatabaseStatus)value;
				break;
			case 83:
				XRuntime.TemporalHistoryRetentionEnabled = (bool)value;
				break;
			case 53:
				XRuntime.Trustworthy = (bool)value;
				break;
			case 54:
				XRuntime.UserAccess = (DatabaseUserAccess)value;
				break;
			case 55:
				XRuntime.UserName = (string)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 0:
				XRuntime.ActiveConnections = (int)value;
				break;
			case 32:
				XRuntime.AnsiNullDefault = (bool)value;
				break;
			case 33:
				XRuntime.AnsiNullsEnabled = (bool)value;
				break;
			case 34:
				XRuntime.AnsiPaddingEnabled = (bool)value;
				break;
			case 35:
				XRuntime.AnsiWarningsEnabled = (bool)value;
				break;
			case 36:
				XRuntime.ArithmeticAbortEnabled = (bool)value;
				break;
			case 1:
				XRuntime.AutoClose = (bool)value;
				break;
			case 112:
				XRuntime.AutoCreateIncrementalStatisticsEnabled = (bool)value;
				break;
			case 37:
				XRuntime.AutoCreateStatisticsEnabled = (bool)value;
				break;
			case 2:
				XRuntime.AutoShrink = (bool)value;
				break;
			case 58:
				XRuntime.AutoUpdateStatisticsAsync = (bool)value;
				break;
			case 38:
				XRuntime.AutoUpdateStatisticsEnabled = (bool)value;
				break;
			case 98:
				XSchema.AvailabilityDatabaseSynchronizationState = (AvailabilityDatabaseSynchronizationState)value;
				break;
			case 99:
				XSchema.AvailabilityGroupName = (string)value;
				break;
			case 59:
				XRuntime.BrokerEnabled = (bool)value;
				break;
			case 39:
				XRuntime.CaseSensitive = (bool)value;
				break;
			case 132:
				XRuntime.CatalogCollation = (CatalogCollationType)value;
				break;
			case 89:
				XRuntime.ChangeTrackingAutoCleanUp = (bool)value;
				break;
			case 90:
				XRuntime.ChangeTrackingEnabled = (bool)value;
				break;
			case 91:
				XRuntime.ChangeTrackingRetentionPeriod = (int)value;
				break;
			case 92:
				XRuntime.ChangeTrackingRetentionPeriodUnits = (RetentionPeriodUnits)value;
				break;
			case 40:
				XRuntime.CloseCursorsOnCommitEnabled = (bool)value;
				break;
			case 41:
				XSchema.Collation = (string)value;
				break;
			case 3:
				XRuntime.CompatibilityLevel = (CompatibilityLevel)value;
				break;
			case 42:
				XRuntime.ConcatenateNullYieldsNull = (bool)value;
				break;
			case 100:
				XRuntime.ContainmentType = (ContainmentType)value;
				break;
			case 4:
				XRuntime.CreateDate = (DateTime)value;
				break;
			case 60:
				XRuntime.DatabaseGuid = (Guid)value;
				break;
			case 43:
				XRuntime.DatabaseOwnershipChaining = (bool)value;
				break;
			case 61:
				XSchema.DatabaseSnapshotBaseName = (string)value;
				break;
			case 5:
				XRuntime.DataSpaceUsage = (double)value;
				break;
			case 62:
				XRuntime.DateCorrelationOptimization = (bool)value;
				break;
			case 6:
				XRuntime.DboLogin = (bool)value;
				break;
			case 7:
				XRuntime.DefaultFileGroup = (string)value;
				break;
			case 93:
				XRuntime.DefaultFileStreamFileGroup = (string)value;
				break;
			case 63:
				XRuntime.DefaultFullTextCatalog = (string)value;
				break;
			case 101:
				XRuntime.DefaultFullTextLanguageLcid = (int)value;
				break;
			case 102:
				XRuntime.DefaultFullTextLanguageName = (string)value;
				break;
			case 103:
				XRuntime.DefaultLanguageLcid = (int)value;
				break;
			case 104:
				XRuntime.DefaultLanguageName = (string)value;
				break;
			case 8:
				XSchema.DefaultSchema = (string)value;
				break;
			case 113:
				XRuntime.DelayedDurability = (DelayedDurability)value;
				break;
			case 94:
				XRuntime.EncryptionEnabled = (bool)value;
				break;
			case 105:
				XRuntime.FilestreamDirectoryName = (string)value;
				break;
			case 106:
				XRuntime.FilestreamNonTransactedAccess = (FilestreamNonTransactedAccessType)value;
				break;
			case 107:
				XRuntime.HasDatabaseEncryptionKey = (bool)value;
				break;
			case 114:
				XRuntime.HasFileInCloud = (bool)value;
				break;
			case 64:
				XRuntime.HasFullBackup = (bool)value;
				break;
			case 115:
				XRuntime.HasMemoryOptimizedObjects = (bool)value;
				break;
			case 95:
				XRuntime.HonorBrokerPriority = (bool)value;
				break;
			case 9:
				XRuntime.ID = (int)value;
				break;
			case 10:
				XRuntime.IndexSpaceUsage = (double)value;
				break;
			case 11:
				XRuntime.IsAccessible = (bool)value;
				break;
			case 65:
				XRuntime.IsDatabaseSnapshot = (bool)value;
				break;
			case 66:
				XRuntime.IsDatabaseSnapshotBase = (bool)value;
				break;
			case 12:
				XRuntime.IsDbAccessAdmin = (bool)value;
				break;
			case 13:
				XRuntime.IsDbBackupOperator = (bool)value;
				break;
			case 14:
				XRuntime.IsDbDatareader = (bool)value;
				break;
			case 15:
				XRuntime.IsDbDatawriter = (bool)value;
				break;
			case 16:
				XRuntime.IsDbDdlAdmin = (bool)value;
				break;
			case 17:
				XRuntime.IsDbDenyDatareader = (bool)value;
				break;
			case 18:
				XRuntime.IsDbDenyDatawriter = (bool)value;
				break;
			case 19:
				XRuntime.IsDbOwner = (bool)value;
				break;
			case 20:
				XRuntime.IsDbSecurityAdmin = (bool)value;
				break;
			case 21:
				XRuntime.IsFullTextEnabled = (bool)value;
				break;
			case 67:
				XRuntime.IsMailHost = (bool)value;
				break;
			case 96:
				XRuntime.IsManagementDataWarehouse = (bool)value;
				break;
			case 22:
				XRuntime.IsMirroringEnabled = (bool)value;
				break;
			case 68:
				XRuntime.IsParameterizationForced = (bool)value;
				break;
			case 69:
				XRuntime.IsReadCommittedSnapshotOn = (bool)value;
				break;
			case 23:
				XRuntime.IsSqlDw = (bool)value;
				break;
			case 24:
				XRuntime.IsSystemObject = (bool)value;
				break;
			case 44:
				XRuntime.IsUpdateable = (bool)value;
				break;
			case 70:
				XRuntime.IsVarDecimalStorageFormatEnabled = (bool)value;
				break;
			case 45:
				XRuntime.LastBackupDate = (DateTime)value;
				break;
			case 46:
				XSchema.LastDifferentialBackupDate = (DateTime)value;
				break;
			case 47:
				XSchema.LastGoodCheckDbTime = (DateTime)value;
				break;
			case 48:
				XRuntime.LastLogBackupDate = (DateTime)value;
				break;
			case 118:
				XRuntime.LegacyCardinalityEstimation = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 119:
				XRuntime.LegacyCardinalityEstimationForSecondary = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 49:
				XRuntime.LocalCursorsDefault = (bool)value;
				break;
			case 71:
				XRuntime.LogReuseWaitStatus = (LogReuseWaitStatus)value;
				break;
			case 120:
				XRuntime.MaxDop = (int)value;
				break;
			case 121:
				XRuntime.MaxDopForSecondary = (int)value;
				break;
			case 116:
				XRuntime.MemoryAllocatedToMemoryOptimizedObjectsInKB = (double)value;
				break;
			case 117:
				XRuntime.MemoryUsedByMemoryOptimizedObjectsInKB = (double)value;
				break;
			case 72:
				XRuntime.MirroringFailoverLogSequenceNumber = (decimal)value;
				break;
			case 73:
				XRuntime.MirroringID = (Guid)value;
				break;
			case 74:
				XRuntime.MirroringPartner = (string)value;
				break;
			case 75:
				XRuntime.MirroringPartnerInstance = (string)value;
				break;
			case 76:
				XRuntime.MirroringRedoQueueMaxSize = (int)value;
				break;
			case 77:
				XRuntime.MirroringRole = (MirroringRole)value;
				break;
			case 78:
				XRuntime.MirroringRoleSequence = (int)value;
				break;
			case 79:
				XRuntime.MirroringSafetyLevel = (MirroringSafetyLevel)value;
				break;
			case 80:
				XRuntime.MirroringSafetySequence = (int)value;
				break;
			case 81:
				XRuntime.MirroringStatus = (MirroringStatus)value;
				break;
			case 82:
				XRuntime.MirroringTimeout = (int)value;
				break;
			case 83:
				XRuntime.MirroringWitness = (string)value;
				break;
			case 84:
				XRuntime.MirroringWitnessStatus = (MirroringWitnessStatus)value;
				break;
			case 108:
				XRuntime.NestedTriggersEnabled = (bool)value;
				break;
			case 50:
				XRuntime.NumericRoundAbortEnabled = (bool)value;
				break;
			case 25:
				XRuntime.Owner = (string)value;
				break;
			case 51:
				XRuntime.PageVerify = (PageVerify)value;
				break;
			case 122:
				XRuntime.ParameterSniffing = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 123:
				XRuntime.ParameterSniffingForSecondary = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 97:
				XRuntime.PolicyHealthState = (PolicyHealthState)value;
				break;
			case 26:
				XRuntime.PrimaryFilePath = (string)value;
				break;
			case 124:
				XRuntime.QueryOptimizerHotfixes = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 125:
				XRuntime.QueryOptimizerHotfixesForSecondary = (DatabaseScopedConfigurationOnOff)value;
				break;
			case 52:
				XRuntime.QuotedIdentifiersEnabled = (bool)value;
				break;
			case 53:
				XRuntime.ReadOnly = (bool)value;
				break;
			case 85:
				XRuntime.RecoveryForkGuid = (Guid)value;
				break;
			case 54:
				XRuntime.RecoveryModel = (RecoveryModel)value;
				break;
			case 55:
				XRuntime.RecursiveTriggersEnabled = (bool)value;
				break;
			case 126:
				XRuntime.RemoteDataArchiveCredential = (string)value;
				break;
			case 127:
				XRuntime.RemoteDataArchiveEnabled = (bool)value;
				break;
			case 128:
				XRuntime.RemoteDataArchiveEndpoint = (string)value;
				break;
			case 129:
				XRuntime.RemoteDataArchiveLinkedServer = (string)value;
				break;
			case 130:
				XRuntime.RemoteDataArchiveUseFederatedServiceAccount = (bool)value;
				break;
			case 131:
				XRuntime.RemoteDatabaseName = (string)value;
				break;
			case 27:
				XRuntime.ReplicationOptions = (ReplicationOptions)value;
				break;
			case 86:
				XRuntime.ServiceBrokerGuid = (Guid)value;
				break;
			case 28:
				XRuntime.Size = (double)value;
				break;
			case 87:
				XRuntime.SnapshotIsolationState = (SnapshotIsolationState)value;
				break;
			case 29:
				XRuntime.SpaceAvailable = (double)value;
				break;
			case 30:
				XRuntime.Status = (DatabaseStatus)value;
				break;
			case 109:
				XRuntime.TargetRecoveryTime = (int)value;
				break;
			case 110:
				XRuntime.TransformNoiseWords = (bool)value;
				break;
			case 88:
				XRuntime.Trustworthy = (bool)value;
				break;
			case 111:
				XRuntime.TwoDigitYearCutoff = (int)value;
				break;
			case 56:
				XRuntime.UserAccess = (DatabaseUserAccess)value;
				break;
			case 31:
				XRuntime.UserName = (string)value;
				break;
			case 57:
				XRuntime.Version = (int)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[4] { "CatalogCollation", "DatabaseSnapshotBaseName", "RemoteDataArchiveLinkedServer", "RemoteDatabaseName" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"AnsiNullsEnabled" => false, 
			"AnsiPaddingEnabled" => false, 
			"Collation" => "SQL_Latin1_General_CP1_CI_AS", 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	public void Deny(DatabasePermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(DatabasePermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(DatabasePermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(DatabasePermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(DatabasePermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(DatabasePermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(DatabasePermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(DatabasePermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(DatabasePermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(DatabasePermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(DatabasePermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(DatabasePermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(DatabasePermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(DatabasePermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(DatabasePermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(DatabasePermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public DatabasePermissionInfo[] EnumDatabasePermissions()
	{
		return (DatabasePermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Database, this, null, null);
	}

	public DatabasePermissionInfo[] EnumDatabasePermissions(string granteeName)
	{
		return (DatabasePermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Database, this, granteeName, null);
	}

	public DatabasePermissionInfo[] EnumDatabasePermissions(DatabasePermissionSet permissions)
	{
		return (DatabasePermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Database, this, null, permissions);
	}

	public DatabasePermissionInfo[] EnumDatabasePermissions(string granteeName, DatabasePermissionSet permissions)
	{
		return (DatabasePermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Database, this, granteeName, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumAllPermissions(this, granteeName, permissions);
	}

	internal Database(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_LogFiles = null;
		m_FileGroups = null;
		m_PlanGuides = null;
		m_Tables = null;
		m_StoredProcedures = null;
		m_ExtendedStoredProcedures = null;
		m_UserDefinedFunctions = null;
		m_Views = null;
		m_Users = null;
		m_Roles = null;
		m_Defaults = null;
		m_Rules = null;
		m_UserDefinedDataTypes = null;
		m_UserDefinedTableTypes = null;
		m_ServiceBroker = null;
		m_PartitionFunctions = null;
		m_PartitionSchemes = null;
		m_SqlAssemblies = null;
		m_UserDefinedTypes = null;
		m_UserDefinedAggregates = null;
		m_FullTextCatalogs = null;
		m_DatabaseEncryptionKey = null;
		databaseAuditSpecifications = null;
		m_FullTextStopLists = null;
		m_SearchPropertyLists = null;
		m_SecurityPolicies = null;
		m_ExternalDataSources = null;
		m_ExternalFileFormats = null;
		m_ColumnMasterKeys = null;
		m_ColumnEncryptionKeys = null;
		m_DatabaseScopedCredentials = null;
		m_DatabaseScopedConfigurations = null;
	}

	public Database(Server server, string name, DatabaseEngineEdition edition)
		: this(server, name)
	{
		m_edition = edition;
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (!(prop.Name == "CompatibilityLevel"))
		{
			return;
		}
		bool flag = false;
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			flag = (CompatibilityLevel)value >= CompatibilityLevel.Version100;
		}
		else
		{
			switch (base.ServerVersion.Major)
			{
			case 8:
				if ((CompatibilityLevel)value <= CompatibilityLevel.Version80)
				{
					flag = true;
				}
				break;
			case 9:
				if ((CompatibilityLevel)value <= CompatibilityLevel.Version90)
				{
					flag = true;
				}
				break;
			case 10:
				if ((CompatibilityLevel)value <= CompatibilityLevel.Version100)
				{
					flag = true;
				}
				break;
			case 11:
				if ((CompatibilityLevel)value >= CompatibilityLevel.Version90 && (CompatibilityLevel)value <= CompatibilityLevel.Version110)
				{
					flag = true;
				}
				break;
			case 12:
			{
				CompatibilityLevel compatibilityLevel = CompatibilityLevel.Version120;
				if ((CompatibilityLevel)value >= CompatibilityLevel.Version100 && (CompatibilityLevel)value <= compatibilityLevel)
				{
					flag = true;
				}
				break;
			}
			case 13:
				if ((CompatibilityLevel)value >= CompatibilityLevel.Version100 && (CompatibilityLevel)value <= CompatibilityLevel.Version130)
				{
					flag = true;
				}
				break;
			default:
				if (base.ServerVersion.Major >= 14 && (CompatibilityLevel)value >= CompatibilityLevel.Version100)
				{
					flag = true;
				}
				break;
			}
		}
		if (!flag)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "CompatibilityLevel", value.ToString(), GetSqlServerVersionName()));
		}
	}

	public void Create(bool forAttach)
	{
		bForAttach = forAttach;
		try
		{
			CreateImpl();
		}
		finally
		{
			bForAttach = false;
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		ContainmentRelatedValidation(sp);
		StringBuilder stringBuilder = new StringBuilder();
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		string text2 = string.Empty;
		bool flag = false;
		StringCollection stringCollection = new StringCollection();
		bool flag2 = DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType;
		bool suppressDirtyCheck = sp.SuppressDirtyCheck;
		bool flag3 = !flag2 && sp.TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance;
		if (IsSupportedProperty("DatabaseSnapshotBaseName"))
		{
			text2 = GetPropValueOptional("DatabaseSnapshotBaseName", string.Empty);
			flag = text2.Length > 0;
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (SqlServerVersionInternal.Version90 <= sp.TargetServerVersionInternal)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE90, new object[2]
				{
					"NOT",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE80, new object[2]
				{
					"NOT",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		if (flag2)
		{
			ScriptCreateForCloud(stringBuilder, sp, text);
			if (sp.TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				createQuery.Add(stringBuilder.ToString());
				return;
			}
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE DATABASE {0}", new object[1] { text });
		}
		if (IsSupportedProperty("ContainmentType", sp))
		{
			Property propertyOptional = GetPropertyOptional("ContainmentType");
			if (propertyOptional.Value != null)
			{
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(" CONTAINMENT = ");
				switch ((ContainmentType)propertyOptional.Value)
				{
				case ContainmentType.None:
					stringBuilder.Append("NONE");
					break;
				case ContainmentType.Partial:
					stringBuilder.Append("PARTIAL");
					break;
				default:
					throw new WrongPropertyValueException(base.Properties.Get("ContainmentType"));
				}
				stringBuilder.Append(sp.NewLine);
			}
		}
		if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			if (sp.Storage.FileGroup && FileGroups.Count > 0 && !flag3)
			{
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				GetFileGroupsScript(stringBuilder2, flag, stringCollection, sp);
				if (stringBuilder2.Length > 0)
				{
					stringBuilder.Append(" ON ");
					stringBuilder.Append(stringBuilder2.ToString());
				}
			}
			if (!flag && sp.Storage.FileGroup && LogFiles.Count > 0 && !flag3)
			{
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(" LOG ON ");
				GetLogFilesScript(sp, stringBuilder);
			}
		}
		if (!flag)
		{
			if (!flag2 && sp.IncludeScripts.Collation && base.ServerVersion.Major > 7 && SqlServerVersionInternal.Version80 <= sp.TargetServerVersionInternal)
			{
				Property property = ((base.State == SqlSmoState.Creating) ? base.Properties.Get("Collation") : base.Properties["Collation"]);
				if (property.Value != null && (suppressDirtyCheck || property.Dirty))
				{
					CheckCollation((string)property.Value, sp);
					stringBuilder.Append(Globals.newline);
					stringBuilder.Append(" COLLATE ");
					stringBuilder.Append((string)property.Value);
				}
			}
			if (bForAttach)
			{
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(" FOR ATTACH");
			}
			else if (!flag2 && IsSupportedProperty("CatalogCollation", sp) && !flag3)
			{
				Property propertyOptional2 = GetPropertyOptional("CatalogCollation");
				if (propertyOptional2 != null && propertyOptional2.Value != null && (CatalogCollationType)propertyOptional2.Value != CatalogCollationType.ContainedDatabaseFixedCollation)
				{
					TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(CatalogCollationType));
					string value = typeConverter.ConvertToInvariantString(propertyOptional2.Value);
					stringBuilder.Append(Globals.newline);
					stringBuilder.Append(" WITH CATALOG_COLLATION = ");
					stringBuilder.Append(value);
				}
			}
		}
		else
		{
			if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.SupportedOnlyOn90).SetHelpContext("SupportedOnlyOn90");
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " AS SNAPSHOT OF [{0}]", new object[1] { SqlSmoObject.SqlBraket(text2) });
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
		FileGroup fileGroup = null;
		if (flag3)
		{
			FileGroup fileGroup2 = m_FileGroups["PRIMARY"];
			foreach (FileGroup fileGroup3 in FileGroups)
			{
				if (fileGroup3 == fileGroup2 || fileGroup3.FileGroupType == FileGroupType.MemoryOptimizedDataFileGroup)
				{
					continue;
				}
				if (base.State == SqlSmoState.Existing)
				{
					fileGroup3.Initialize(allProperties: true);
				}
				fileGroup3.ScriptCreate(createQuery, sp);
				foreach (DataFile file in fileGroup3.Files)
				{
					file.ScriptCreate(createQuery, sp);
				}
				if (fileGroup3.IsDefault)
				{
					fileGroup = fileGroup3;
				}
			}
		}
		StringEnumerator enumerator3 = stringCollection.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				string current = enumerator3.Current;
				createQuery.Add(current);
			}
		}
		finally
		{
			if (enumerator3 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && base.ServerVersion.Major >= 13 && SqlServerVersionInternal.Version130 <= sp.TargetServerVersionInternal)
		{
			GetAutoGrowFilesScript(createQuery, sp);
		}
		if (!flag)
		{
			if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70)
			{
				ScriptDbProps70Comp(createQuery, sp);
			}
			else
			{
				ScriptDbProps80Comp(createQuery, sp, flag2);
			}
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			bool forCreate = true;
			ScriptVardecimalCompression(createQuery, sp, forCreate);
		}
		if (IsSupportedProperty("ChangeTrackingEnabled", sp) && sp.Data.ChangeTracking)
		{
			ScriptChangeTracking(createQuery, sp);
		}
		if (IsSupportedProperty("RemoteDataArchiveEnabled", sp))
		{
			ScriptRemoteDataArchive(createQuery, sp);
		}
		if (sp.IncludeScripts.Owner)
		{
			ScriptChangeOwner(createQuery, sp);
		}
		if (IsSupportedProperty("EncryptionEnabled", sp))
		{
			if (!flag2 && m_DatabaseEncryptionKey != null && (!IsDEKInitializedWithoutAnyPropertiesSet() || base.Properties.Get("EncryptionEnabled").Dirty))
			{
				AddUseDb(createQuery, sp);
				DatabaseEncryptionKey.ScriptCreateInternal(createQuery, sp);
			}
			Property property2 = base.Properties.Get("EncryptionEnabled");
			if (property2.Value != null && (bool)property2.Value)
			{
				StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE {0}", new object[1] { FormatFullNameForScripting(sp) });
				stringBuilder3.AppendFormat(SmoApplication.DefaultCulture, " SET ENCRYPTION {0}", new object[1] { ((bool)property2.Value) ? "ON" : "OFF" });
				createQuery.Add(stringBuilder3.ToString());
			}
		}
		ScriptCreateUsersIfRequired(createQuery, sp);
		if (this.IsSupportedObject<QueryStoreOptions>(sp) && base.State != SqlSmoState.Creating)
		{
			QueryStoreOptions.ScriptCreate(createQuery, sp);
		}
		StringCollection stringCollection2 = new StringCollection();
		ScriptDbScopedConfigurations(stringCollection2, sp);
		if (stringCollection2.Count != 0)
		{
			if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType)
			{
				SmoUtility.EncodeStringCollectionAsComment(stringCollection2, LocalizableResources.DatabaseScopedConfiguration_CreateScriptOnAzureDesc);
			}
			createQuery.AddCollection(stringCollection2);
		}
		if (fileGroup != null)
		{
			createQuery.AddCollection(GetDefaultFileGroupScript(sp, fileGroup.Name));
		}
	}

	private void ScriptDbScopedConfigurations(StringCollection query, ScriptingPreferences sp)
	{
		if (!this.IsSupportedObject<DatabaseScopedConfiguration>(sp))
		{
			return;
		}
		string[] source = new string[4] { "MAXDOP", "LEGACY_CARDINALITY_ESTIMATION", "PARAMETER_SNIFFING", "QUERY_OPTIMIZER_HOTFIXES" };
		StringCollection stringCollection = new StringCollection();
		for (int i = 0; i < DatabaseScopedConfigurations.Count; i++)
		{
			DatabaseScopedConfiguration databaseScopedConfiguration = DatabaseScopedConfigurations[i];
			bool flag = base.State == SqlSmoState.Existing && databaseScopedConfiguration.State == SqlSmoState.Existing && databaseScopedConfiguration.IsSupportedProperty("IsValueDefault") && databaseScopedConfiguration.IsValueDefault;
			Property propertyObject = databaseScopedConfiguration.Properties.GetPropertyObject("Value");
			if ((propertyObject.Dirty || (!sp.ScriptForAlter && !flag)) && databaseScopedConfiguration.Value != string.Empty)
			{
				int num = databaseScopedConfiguration.Value.IndexOf('\0');
				if (num >= 0)
				{
					databaseScopedConfiguration.Value = databaseScopedConfiguration.Value.Substring(0, num);
				}
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE SCOPED CONFIGURATION SET {0} = {1};", new object[2]
				{
					databaseScopedConfiguration.Name.ToUpper(CultureInfo.InvariantCulture),
					databaseScopedConfiguration.Value
				});
				stringCollection.Add(stringBuilder.ToString());
			}
			propertyObject = databaseScopedConfiguration.Properties.GetPropertyObject("ValueForSecondary");
			string text = (string.IsNullOrEmpty(databaseScopedConfiguration.ValueForSecondary) ? "PRIMARY" : databaseScopedConfiguration.ValueForSecondary);
			bool flag2 = source.Contains(databaseScopedConfiguration.Name.ToUpper()) && !text.Equals("PRIMARY", StringComparison.OrdinalIgnoreCase);
			if ((!string.IsNullOrEmpty(databaseScopedConfiguration.ValueForSecondary) && propertyObject.Dirty) || (!sp.ScriptForAlter && flag2))
			{
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET {0} = {1};", new object[2]
				{
					databaseScopedConfiguration.Name.ToUpper(CultureInfo.InvariantCulture),
					databaseScopedConfiguration.ValueForSecondary
				});
				stringCollection.Add(stringBuilder2.ToString());
			}
		}
		if (stringCollection.Count != 0)
		{
			AddUseDb(query, sp);
			query.AddCollection(stringCollection);
		}
	}

	private void ScriptCreateUsersIfRequired(StringCollection createQuery, ScriptingPreferences sp)
	{
		if (!sp.IncludeScripts.Permissions)
		{
			return;
		}
		StringCollection stringCollection = new StringCollection();
		foreach (User user in Users)
		{
			if (!user.IsSystemObject && user.Login.Length > 0)
			{
				user.ScriptCreateInternal(stringCollection, sp);
			}
		}
		if (stringCollection.Count <= 0)
		{
			return;
		}
		AddUseDb(createQuery, sp);
		StringEnumerator enumerator2 = stringCollection.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current = enumerator2.Current;
				createQuery.Add(current);
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void ScriptCreateForCloud(StringBuilder sbStatement, ScriptingPreferences sp, string scriptName)
	{
		if (IsSupportedProperty("IsSqlDw", sp))
		{
			Property propertyOptional = GetPropertyOptional("IsSqlDw");
			bool flag = base.State == SqlSmoState.Creating || base.Properties.Contains("AzureEdition");
			object obj = (flag ? GetPropertyOptional("MaxSizeInBytes").Value : null);
			object obj2 = (flag ? GetPropertyOptional("AzureEdition").Value : null);
			object obj3 = (flag ? GetPropertyOptional("AzureServiceObjective").Value : null);
			Property propertyOptional2 = GetPropertyOptional("Collation");
			Property property = (IsSupportedProperty("CatalogCollation", sp) ? GetPropertyOptional("CatalogCollation") : null);
			if (propertyOptional.Value != null && (bool)propertyOptional.Value && (obj2 == null || obj3 == null))
			{
				throw new ArgumentException(ExceptionTemplatesImpl.SqlDwCreateRequiredParameterMissing);
			}
			string text = string.Format(SmoApplication.DefaultCulture, "CREATE DATABASE {0} ", new object[1] { scriptName });
			if (sp.IncludeScripts.Collation && propertyOptional2.Value != null)
			{
				text += $"COLLATE {propertyOptional2.Value} ";
			}
			ScriptStringBuilder scriptStringBuilder = new ScriptStringBuilder(text);
			if (sp.TargetEngineIsAzureSqlDw())
			{
				scriptStringBuilder.SetParameter("EDITION", "DataWarehouse");
			}
			else if (obj2 != null && DatabaseEngineEdition == sp.TargetDatabaseEngineEdition)
			{
				scriptStringBuilder.SetParameter("EDITION", obj2.ToString());
			}
			if (DatabaseEngineEdition == sp.TargetDatabaseEngineEdition)
			{
				if (obj3 != null)
				{
					scriptStringBuilder.SetParameter("SERVICE_OBJECTIVE", obj3.ToString());
				}
				if (obj != null)
				{
					scriptStringBuilder.SetParameter("MAXSIZE", GetMaxSizeString((double)obj), ParameterValueFormat.NotString);
				}
			}
			else if (sp.TargetEngineIsAzureSqlDw())
			{
				scriptStringBuilder.SetParameter("SERVICE_OBJECTIVE", "DW100");
			}
			sbStatement.Append(scriptStringBuilder.ToString(scriptSemiColon: false));
			if (property != null && property.Value != null)
			{
				TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(CatalogCollationType));
				string arg = typeConverter.ConvertToInvariantString(property.Value);
				sbStatement.AppendFormat(" WITH CATALOG_COLLATION = {0}", arg);
			}
			sbStatement.AppendFormat(";{0}", sp.NewLine);
		}
		else
		{
			sbStatement.AppendFormat(SmoApplication.DefaultCulture, "CREATE DATABASE {0} ", new object[1] { scriptName });
		}
	}

	private static string GetMaxSizeString(double maxSizeInBytes)
	{
		double num = maxSizeInBytes;
		if (num < 1073741824.0)
		{
			return string.Format(SmoApplication.DefaultCulture, "{0} MB", new object[1] { num / 1024.0 / 1024.0 });
		}
		return string.Format(SmoApplication.DefaultCulture, "{0} GB", new object[1] { num / 1024.0 / 1024.0 / 1024.0 });
	}

	private void ScriptAlterForCloud(StringCollection sbStatement, ScriptingPreferences sp)
	{
		Property property = base.Properties.Get("AzureEdition");
		Property property2 = base.Properties.Get("MaxSizeInBytes");
		Property property3 = base.Properties.Get("AzureServiceObjective");
		Property property4 = base.Properties.Get("TemporalHistoryRetentionEnabled");
		IEnumerable<Property> enumerable = new Property[3] { property, property2, property3 }.Where((Property prop) => prop.Value != null && (prop.Dirty || !sp.ForDirectExecution));
		if (enumerable.Any())
		{
			ScriptStringBuilder scriptStringBuilder = new ScriptStringBuilder(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} MODIFY", new object[1] { FormatFullNameForScripting(sp) }));
			foreach (Property item in enumerable)
			{
				switch (item.Name.ToLowerInvariant())
				{
				case "azureedition":
					scriptStringBuilder.SetParameter("EDITION", item.Value.ToString());
					break;
				case "azureserviceobjective":
					scriptStringBuilder.SetParameter("SERVICE_OBJECTIVE", item.Value.ToString());
					break;
				case "maxsizeinbytes":
					scriptStringBuilder.SetParameter("MAXSIZE", GetMaxSizeString((double)item.Value), ParameterValueFormat.NotString);
					break;
				}
			}
			sbStatement.Add(scriptStringBuilder.ToString());
		}
		if (property4.Value != null && (property4.Dirty || !sp.ForDirectExecution))
		{
			string value = string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET TEMPORAL_HISTORY_RETENTION {1}", new object[2]
			{
				FormatFullNameForScripting(sp),
				((bool)property4.Value) ? "ON" : "OFF"
			});
			sbStatement.Add(value);
		}
	}

	private void ScriptDbProps70Comp(StringCollection query, ScriptingPreferences sp)
	{
		_ = sp.SuppressDirtyCheck;
		string text = FormatFullNameForScripting(sp, nameIsIndentifier: false);
		if (IsSupportedProperty("IsFullTextEnabled", sp))
		{
			Property property = base.Properties.Get("IsFullTextEnabled");
			if (property.Value != null && (property.Dirty || !sp.ScriptForAlter))
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')){0}begin{0}EXEC {1}.[dbo].[sp_fulltext_database] @action = '{2}'{0}end", new object[3]
				{
					Globals.newline,
					FormatFullNameForScripting(sp),
					((bool)property.Value) ? "enable" : "disable"
				}));
			}
		}
		Property property2 = base.Properties.Get("CompatibilityLevel");
		if (property2.Value != null && (property2.Dirty || !sp.ScriptForAlter) && (CompatibilityLevel)property2.Value <= CompatibilityLevel.Version70)
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_dbcmptlevel @dbname={0}, @new_cmptlevel={1}", new object[2]
			{
				text,
				Enum.Format(typeof(CompatibilityLevel), (CompatibilityLevel)property2.Value, "d")
			}));
		}
	}

	private void ScriptDbProps80Comp(StringCollection query, ScriptingPreferences sp, bool isAzureDb)
	{
		AddCompatibilityLevel(query, sp);
		if (sp.ScriptForAlter && !isAzureDb)
		{
			Property property = base.Properties.Get("Collation");
			if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
			{
				CheckCollation((string)property.Value, sp);
				query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} COLLATE {1}", new object[2]
				{
					FormatFullNameForScripting(sp),
					(string)property.Value
				}));
			}
		}
		if (!isAzureDb && IsSupportedProperty("IsFullTextEnabled", sp))
		{
			Property property2 = base.Properties.Get("IsFullTextEnabled");
			if (property2.Value != null && (property2.Dirty || !sp.ScriptForAlter) && (sp.TargetServerVersion >= SqlServerVersion.Version90 || sp.ScriptForAlter || (bool)property2.Value))
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')){0}begin{0}EXEC {1}.[dbo].[sp_fulltext_database] @action = '{2}'{0}end", new object[3]
				{
					Globals.newline,
					FormatFullNameForScripting(sp),
					((bool)property2.Value) ? "enable" : "disable"
				}));
			}
		}
		ScriptDbOptionsProps(query, sp, isAzureDb);
	}

	private void AddCompatibilityLevel(StringCollection query, ScriptingPreferences sp)
	{
		Property property = base.Properties.Get("CompatibilityLevel");
		if (property.Value == null || (!property.Dirty && sp.ScriptForAlter))
		{
			return;
		}
		bool flag = sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && sp.TargetServerVersionInternal == SqlServerVersionInternal.Version120;
		bool flag2 = (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version150 || flag) && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version150;
		bool flag3 = (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version140 || flag) && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version140;
		bool flag4 = (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version130 || flag) && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version130;
		bool flag5 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version120 && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version120;
		bool flag6 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version110 && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version110;
		bool flag7 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version105 && (CompatibilityLevel)property.Value <= (CompatibilityLevel)105;
		bool flag8 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version100 && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version100;
		bool flag9 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90 && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version90;
		bool flag10 = sp.TargetServerVersionInternal == SqlServerVersionInternal.Version80 && (CompatibilityLevel)property.Value <= CompatibilityLevel.Version80;
		bool flag11 = flag9 || flag10;
		bool flag12 = flag7 || flag8 || flag6 || flag5 || flag4 || flag3 || flag2;
		if (sp.ScriptForAlter || flag12 || flag11)
		{
			CompatibilityLevel compatibilityLevel = UpgradeCompatibilityValueIfRequired(sp, (CompatibilityLevel)property.Value);
			if (flag12)
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET COMPATIBILITY_LEVEL = {1}", new object[2]
				{
					FormatFullNameForScripting(sp),
					Enum.Format(typeof(CompatibilityLevel), compatibilityLevel, "d")
				}));
			}
			else if (flag11)
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_dbcmptlevel @dbname={0}, @new_cmptlevel={1}", new object[2]
				{
					FormatFullNameForScripting(sp, nameIsIndentifier: false),
					Enum.Format(typeof(CompatibilityLevel), compatibilityLevel, "d")
				}));
			}
		}
	}

	private CompatibilityLevel UpgradeCompatibilityValueIfRequired(ScriptingPreferences sp, CompatibilityLevel compatibilityLevel)
	{
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version110 && compatibilityLevel <= CompatibilityLevel.Version80)
		{
			return CompatibilityLevel.Version90;
		}
		if ((sp.TargetServerVersionInternal == SqlServerVersionInternal.Version105 || sp.TargetServerVersionInternal == SqlServerVersionInternal.Version100) && compatibilityLevel <= CompatibilityLevel.Version70)
		{
			return CompatibilityLevel.Version80;
		}
		if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90 && compatibilityLevel <= CompatibilityLevel.Version65)
		{
			return CompatibilityLevel.Version70;
		}
		return compatibilityLevel;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			return null;
		}
		bool flag = action != PropagateAction.Create;
		if (base.ServerVersion.Major >= 9)
		{
			if (action == PropagateAction.Alter)
			{
				return new PropagateInfo[6]
				{
					new PropagateInfo(m_ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix),
					new PropagateInfo(databaseDdlTriggerCollection, flag, flag),
					new PropagateInfo(m_FileGroups, flag, flag),
					new PropagateInfo(m_LogFiles, flag, flag),
					new PropagateInfo(m_FullTextCatalogs, flag, flag),
					new PropagateInfo((base.ServerVersion.Major < 10) ? null : m_FullTextStopLists, flag, flag)
				};
			}
			bool flag2 = false;
			if (IsSupportedProperty("IsDatabaseSnapshot"))
			{
				flag2 = GetPropValueOptional("IsDatabaseSnapshot", defaultValue: false);
			}
			return new PropagateInfo[5]
			{
				new PropagateInfo(ExtendedProperties, !flag2, ExtendedProperty.UrnSuffix),
				new PropagateInfo(FileGroups, flag, flag),
				new PropagateInfo(LogFiles, flag, flag),
				new PropagateInfo(m_FullTextCatalogs, flag, flag),
				new PropagateInfo((base.ServerVersion.Major < 10) ? null : m_FullTextStopLists, flag, flag)
			};
		}
		if (base.ServerVersion.Major >= 8)
		{
			return new PropagateInfo[3]
			{
				new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix),
				new PropagateInfo(FileGroups, flag, flag),
				new PropagateInfo(LogFiles, flag, flag)
			};
		}
		return new PropagateInfo[2]
		{
			new PropagateInfo(FileGroups, flag, flag),
			new PropagateInfo(LogFiles, flag, flag)
		};
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		AddScriptPermissions(query, PermissionWorker.PermissionEnumKind.Database, sp);
	}

	public void SetOffline()
	{
		SetOfflineImpl(offline: true);
	}

	public void SetOnline()
	{
		SetOfflineImpl(offline: false);
	}

	public void SetSnapshotIsolation(bool enabled)
	{
		try
		{
			CheckObjectState();
			if (base.State == SqlSmoState.Creating)
			{
				throw new InvalidSmoOperationException("SetSnapshotIsolation", base.State);
			}
			if (base.ServerVersion.Major < 9)
			{
				throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION {1}", new object[2]
			{
				SqlSmoObject.SqlBraket(Name),
				enabled ? "ON" : "OFF"
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetSnapshotIsolation, this, ex);
		}
	}

	private void SetOfflineImpl(bool offline)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(Scripts.USEMASTER);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET  {1}", new object[2]
			{
				SqlSmoObject.SqlBraket(Name),
				offline ? "OFFLINE" : "ONLINE"
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullDatabaseEvent())
			{
				SmoApplication.eventsSingleton.CallDatabaseEvent(Parent, new DatabaseEventArgs(base.Urn, this, Name, offline ? DatabaseEventType.Offline : DatabaseEventType.Online));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetOffline, this, ex);
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void ScriptOwnerForShiloh(StringBuilder sb, ScriptingPreferences sp, string newOwner)
	{
		sb.AppendFormat(SmoApplication.DefaultCulture, "EXEC {0}.dbo.sp_changedbowner @loginame = {1}, @map = false", new object[2]
		{
			FormatFullNameForScripting(sp),
			SqlSmoObject.MakeSqlString(newOwner)
		});
	}

	public void SetOwner(string loginName)
	{
		CheckObjectState(throwIfNotCreated: true);
		SetOwnerImpl(loginName, dropExistingUser: false);
	}

	public void SetOwner(string loginName, bool dropExistingUser)
	{
		CheckObjectState(throwIfNotCreated: true);
		SetOwnerImpl(loginName, dropExistingUser);
	}

	private void SetOwnerImpl(string loginName, bool dropExistingUser)
	{
		try
		{
			if (loginName == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("loginName"));
			}
			if (base.IsDesignMode)
			{
				Property property = base.Properties.Get("Owner");
				property.SetValue(loginName);
				property.SetRetrieved(retrieved: true);
				return;
			}
			StringCollection stringCollection = new StringCollection();
			InsertUseDb(0, stringCollection, DatabaseEngineType);
			if (dropExistingUser)
			{
				Login login = ((Server)base.ParentColl.ParentInstance).Logins[loginName];
				if (login == null)
				{
					throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(ExceptionTemplatesImpl.InvalidLogin(loginName)));
				}
				User user = Users.Cast<User>().FirstOrDefault((User u) => u.Login.Equals(loginName));
				if (user != null && !user.Name.Equals("dbo", StringComparison.Ordinal))
				{
					user.ScriptDrop(stringCollection, GetScriptingPreferencesForCreate());
				}
			}
			ScriptChangeOwner(stringCollection, loginName);
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetOwner, this, ex);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	protected override void CleanObject()
	{
		base.CleanObject();
		if (isDefaultLanguageModified)
		{
			Property property = base.Properties.Get("DefaultLanguageName");
			Property property2 = base.Properties.Get("DefaultLanguageLcid");
			property.SetRetrieved(retrieved: false);
			property2.SetRetrieved(retrieved: false);
		}
		if (isDefaultFulltextLanguageModified)
		{
			Property property3 = base.Properties.Get("DefaultFullTextLanguageName");
			Property property4 = base.Properties.Get("DefaultFullTextLanguageLcid");
			property3.SetRetrieved(retrieved: false);
			property4.SetRetrieved(retrieved: false);
		}
		if (isDefaultLanguageModified || isDefaultFulltextLanguageModified)
		{
			propertyBagState = PropertyBagState.Lazy;
		}
		isDefaultLanguageModified = false;
		isDefaultFulltextLanguageModified = false;
	}

	public void Alter(TerminationClause terminationClause)
	{
		try
		{
			optionTerminationStatement = new OptionTerminationStatement(terminationClause);
			AlterImpl();
		}
		finally
		{
			optionTerminationStatement = null;
		}
	}

	public void Alter(TimeSpan transactionTerminationTime)
	{
		if (transactionTerminationTime.Seconds < 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.TimeoutMustBePositive);
		}
		try
		{
			optionTerminationStatement = new OptionTerminationStatement(transactionTerminationTime);
			AlterImpl();
		}
		finally
		{
			optionTerminationStatement = null;
		}
	}

	private void ScriptAutoCreateStatistics(StringCollection queries, ScriptingPreferences sp)
	{
		Property property = base.Properties.Get("AutoCreateStatisticsEnabled");
		Property property2 = null;
		if (IsSupportedProperty("AutoCreateIncrementalStatisticsEnabled"))
		{
			property2 = base.Properties.Get("AutoCreateIncrementalStatisticsEnabled");
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = !property.IsNull && property.Dirty;
		if (flag)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { ((bool)property.Value) ? Globals.On : Globals.Off });
		}
		if (!property.IsNull && (bool)property.Value && property2 != null && !property2.IsNull && property2.Dirty && !SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			if (!flag)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { Globals.On });
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "INCREMENTAL = {0}", new object[1] { ((bool)property2.Value) ? Globals.On : Globals.Off });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET AUTO_CREATE_STATISTICS " + stringBuilder.ToString(), new object[1] { FormatFullNameForScripting(sp) }));
		}
	}

	private void ScriptChangeTracking(StringCollection queries, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && !VersionUtils.IsSql12OrLater(sp.TargetServerVersionInternal, base.ServerVersion))
		{
			return;
		}
		Property property = base.Properties.Get("ChangeTrackingEnabled");
		Property property2 = base.Properties.Get("ChangeTrackingRetentionPeriod");
		Property property3 = base.Properties.Get("ChangeTrackingRetentionPeriodUnits");
		Property property4 = base.Properties.Get("ChangeTrackingAutoCleanUp");
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!property.IsNull)
		{
			flag = (bool)property.Value;
			if ((property.Dirty && sp.ScriptForAlter) || (flag && !sp.ScriptForAlter))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "= {0} ", new object[1] { flag ? Globals.On : Globals.Off });
			}
		}
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (!property2.IsNull && (property2.Dirty || !sp.ScriptForAlter) && (sp.ForDirectExecution || (int)property2.Value != 0))
		{
			flag2 = true;
		}
		if (!property3.IsNull && (property3.Dirty || !sp.ScriptForAlter) && (RetentionPeriodUnits)property3.Value != RetentionPeriodUnits.None)
		{
			flag3 = true;
		}
		if (flag2 && flag3)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CHANGE_RETENTION = {0} {1}", new object[2]
			{
				(int)property2.Value,
				property3.Value.ToString().ToUpperInvariant()
			});
			flag4 = true;
		}
		else if (flag2 || flag3)
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.MissingChangeTrackingParameters);
		}
		if (!property4.IsNull && (property4.Dirty || !sp.ScriptForAlter))
		{
			bool flag5 = (bool)property4.Value;
			if (flag5 || flag)
			{
				if (flag4)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
				}
				else
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
					flag4 = true;
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "AUTO_CLEANUP = {0}", new object[1] { flag5 ? "ON" : "OFF" });
			}
		}
		if (flag4)
		{
			if (!flag)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ChangeTrackingException);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET CHANGE_TRACKING " + stringBuilder.ToString(), new object[1] { FormatFullNameForScripting(sp) }));
		}
	}

	private void ScriptMirroringOptions(StringCollection queries, ScriptingPreferences sp)
	{
		if (!IsSupportedProperty("MirroringPartner", sp))
		{
			return;
		}
		Property property = base.Properties.Get("MirroringPartner");
		if (property.Dirty)
		{
			string text = (string)property.Value;
			if (text != null && text.Length > 0)
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET PARTNER = {1}", new object[2]
				{
					FormatFullNameForScripting(sp),
					SqlSmoObject.MakeSqlString(text)
				}));
			}
		}
		property = base.Properties.Get("MirroringWitness");
		if (property.Dirty)
		{
			string text2 = (string)property.Value;
			if (text2 != null && text2.Length > 0)
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET WITNESS = {1}", new object[2]
				{
					FormatFullNameForScripting(sp),
					SqlSmoObject.MakeSqlString(text2)
				}));
			}
		}
		property = base.Properties.Get("MirroringSafetyLevel");
		if (!property.Dirty)
		{
			return;
		}
		object value = property.Value;
		if (value != null)
		{
			MirroringSafetyLevel mirroringSafetyLevel = (MirroringSafetyLevel)value;
			string text3 = null;
			text3 = mirroringSafetyLevel switch
			{
				MirroringSafetyLevel.Full => "FULL", 
				MirroringSafetyLevel.Off => "OFF", 
				_ => throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration(mirroringSafetyLevel.ToString())), 
			};
			if (text3 != null)
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET SAFETY {1}", new object[2]
				{
					FormatFullNameForScripting(sp),
					text3
				}));
			}
		}
	}

	private void ScriptRemoteDataArchive(StringCollection queries, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			return;
		}
		Property property = base.Properties.Get("RemoteDataArchiveEnabled");
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!property.IsNull)
		{
			flag = (bool)property.Value;
			if ((property.Dirty && sp.ScriptForAlter) || (flag && !sp.ScriptForAlter))
			{
				Property property2 = base.Properties.Get("RemoteDataArchiveEndpoint");
				Property property3 = base.Properties.Get("RemoteDataArchiveUseFederatedServiceAccount");
				Property property4 = base.Properties.Get("RemoteDataArchiveCredential");
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "= {0} ", new object[1] { flag ? Globals.On : Globals.Off });
				if ((((property2.Dirty || property4.Dirty) && sp.ScriptForAlter) || !sp.ScriptForAlter) && flag)
				{
					string value = ((property2.Value == null) ? string.Empty : property2.Value.ToString());
					if (string.IsNullOrEmpty(value))
					{
						throw new ArgumentException(ExceptionTemplatesImpl.RemoteServerEndpointRequired);
					}
					bool flag2 = !property3.IsNull && (bool)property3.Value;
					if (!flag2)
					{
						string value2 = ((property4.Value == null) ? string.Empty : property4.Value.ToString());
						if (string.IsNullOrEmpty(value2))
						{
							throw new ArgumentException(ExceptionTemplatesImpl.DatabaseScopedCredentialsRequired);
						}
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "SERVER = N'{0}'", new object[1] { Util.EscapeString(property2.Value.ToString(), '\'') });
					if (flag2)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", FEDERATED_SERVICE_ACCOUNT = ON");
					}
					else
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", CREDENTIAL = [{0}]", new object[1] { SqlSmoObject.SqlBraket(property4.Value.ToString()) });
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
				}
			}
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET REMOTE_DATA_ARCHIVE {1}", new object[2]
			{
				FormatFullNameForScripting(sp),
				stringBuilder.ToString()
			}));
		}
	}

	private void ContainmentRelatedValidation(ScriptingPreferences sp)
	{
		if (!IsSupportedProperty("ContainmentType"))
		{
			return;
		}
		if (GetPropValueOptional("ContainmentType", ContainmentType.None) == ContainmentType.None)
		{
			Property propertyOptional = GetPropertyOptional("DefaultFullTextLanguageLcid");
			Property propertyOptional2 = GetPropertyOptional("DefaultFullTextLanguageName");
			Property propertyOptional3 = GetPropertyOptional("DefaultLanguageLcid");
			Property propertyOptional4 = GetPropertyOptional("DefaultLanguageName");
			Property propertyOptional5 = GetPropertyOptional("NestedTriggersEnabled");
			Property propertyOptional6 = GetPropertyOptional("TransformNoiseWords");
			Property propertyOptional7 = GetPropertyOptional("TwoDigitYearCutoff");
			if ((propertyOptional.Dirty && (int)propertyOptional.Value >= 0) || (propertyOptional2.Dirty && !string.IsNullOrEmpty(propertyOptional2.Value.ToString())) || (propertyOptional3.Dirty && (int)propertyOptional3.Value >= 0) || (propertyOptional4.Dirty && !string.IsNullOrEmpty(propertyOptional4.Value.ToString())) || propertyOptional5.Dirty || propertyOptional6.Dirty || propertyOptional7.Dirty)
			{
				throw new SmoException(string.Format(CultureInfo.CurrentCulture, ExceptionTemplatesImpl.FollowingPropertiesCanBeSetOnlyWithContainmentEnabled, "DefaultFullTextLanguage", "DefaultLanguage", propertyOptional5.Name, propertyOptional6.Name, propertyOptional7.Name));
			}
		}
		else
		{
			SqlSmoObject.ThrowIfCloud(sp.TargetDatabaseEngineType);
			SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
			DefaultFullTextLanguage.VerifyBothLcidAndNameNotDirty(isLanguageValueNoneAllowed: false);
			DefaultLanguage.VerifyBothLcidAndNameNotDirty(isLanguageValueNoneAllowed: false);
		}
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		ContainmentRelatedValidation(sp);
		if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			ScriptAlterForCloud(alterQuery, sp);
		}
		ScriptAlterContainmentDDL(sp, alterQuery);
		if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70)
		{
			ScriptDbProps70Comp(alterQuery, sp);
		}
		else
		{
			ScriptDbProps80Comp(alterQuery, sp, DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType);
			if (IsSupportedProperty("MirroringPartner", sp))
			{
				ScriptMirroringOptions(alterQuery, sp);
			}
		}
		if (IsSupportedProperty("EncryptionEnabled", sp))
		{
			if (IsSupportedProperty("DatabaseEncryptionKey"))
			{
				bool databaseContext = sp.IncludeScripts.DatabaseContext;
				sp.IncludeScripts.DatabaseContext = true;
				try
				{
					if (IsDatabaseEncryptionKeyPresent())
					{
						DatabaseEncryptionKey.Alter();
					}
					else if (databaseEncryptionKeyInitialized && (!IsDEKInitializedWithoutAnyPropertiesSet() || base.Properties.Get("EncryptionEnabled").Dirty))
					{
						DatabaseEncryptionKey.Create();
					}
				}
				finally
				{
					sp.IncludeScripts.DatabaseContext = databaseContext;
				}
			}
			Property property = base.Properties.Get("EncryptionEnabled");
			if (property.Value != null && property.Dirty)
			{
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE {0}", new object[1] { FormatFullNameForScripting(sp) });
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " SET ENCRYPTION {0}", new object[1] { ((bool)property.Value) ? "ON" : "OFF" });
				alterQuery.Add(stringBuilder.ToString());
			}
		}
		if (IsSupportedProperty("DefaultFullTextCatalog", sp))
		{
			Property property2 = base.Properties.Get("DefaultFullTextCatalog");
			if (property2.Value != null && (property2.Dirty || sp.ScriptForCreateDrop) && ((string)property2.Value).Length > 0)
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT CATALOG [{0}] AS DEFAULT", new object[1] { SqlSmoObject.SqlBraket((string)property2.Value) }));
			}
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			bool forCreate = false;
			ScriptVardecimalCompression(alterQuery, sp, forCreate);
		}
		if (sp.Data.ChangeTracking && IsSupportedProperty("ChangeTrackingEnabled", sp))
		{
			ScriptChangeTracking(alterQuery, sp);
		}
		if (IsSupportedProperty("RemoteDataArchiveEnabled", sp))
		{
			ScriptRemoteDataArchive(alterQuery, sp);
		}
		if (this.IsSupportedObject<QueryStoreOptions>(sp))
		{
			QueryStoreOptions.ScriptAlter(alterQuery, sp);
		}
		ScriptDbScopedConfigurations(alterQuery, sp);
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130 && DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType)
		{
			if (SqlServerVersionInternal.Version90 <= sp.TargetServerVersionInternal)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE90, new object[2]
				{
					"",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE80, new object[2]
				{
					"",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP DATABASE {0}{1}", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130 && DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType) ? "IF EXISTS " : string.Empty,
			FormatFullNameForScripting(sp)
		});
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} MODIFY NAME = {1}", new object[2]
		{
			SqlSmoObject.MakeSqlBraket(Name),
			SqlSmoObject.MakeSqlBraket(newName)
		}));
	}

	protected override void PostCreate()
	{
		PostAlterAndCreate();
		base.PostCreate();
	}

	protected override void PostAlter()
	{
		PostAlterAndCreate();
		base.PostAlter();
	}

	private void PostAlterAndCreate()
	{
		CheckObjectState();
		SetComparerToNullIfRequired();
		isDefaultLanguageModified = IsDefaultLanguageDirty();
		isDefaultFulltextLanguageModified = IsDefaultFullTextLanguageDirty();
	}

	private bool IsDefaultLanguageDirty()
	{
		if (IsSupportedProperty("DefaultLanguageLcid"))
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("DefaultLanguageLcid");
			stringCollection.Add("DefaultLanguageName");
			return base.Properties.ArePropertiesDirty(stringCollection);
		}
		return false;
	}

	private bool IsDefaultFullTextLanguageDirty()
	{
		if (IsSupportedProperty("DefaultFullTextLanguageLcid"))
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("DefaultFullTextLanguageLcid");
			stringCollection.Add("DefaultFullTextLanguageName");
			return base.Properties.ArePropertiesDirty(stringCollection);
		}
		return false;
	}

	private void SetComparerToNullIfRequired()
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("Collation");
		if (IsSupportedProperty("ContainmentType"))
		{
			stringCollection.Add("ContainmentType");
		}
		if (base.Properties.ArePropertiesDirty(stringCollection))
		{
			m_comparer = null;
		}
	}

	public void Checkpoint()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("CHECKPOINT");
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Checkpoint, this, ex);
		}
	}

	internal string GetUseDbStatement(string databaseName)
	{
		return string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(databaseName) });
	}

	internal void AddUseDb(StringCollection col, ScriptingPreferences sp)
	{
		InsertUseDb(col.Count, col, sp.TargetDatabaseEngineType);
	}

	internal void AddUseDb(StringCollection col)
	{
		col.Add(GetUseDbStatement(Name));
	}

	private void InsertUseDb(int index, StringCollection col, DatabaseEngineType targetEngineType)
	{
		if (DatabaseEngineType.SqlAzureDatabase != targetEngineType)
		{
			col.Insert(index, GetUseDbStatement(Name));
		}
	}

	public void ExecuteNonQuery(string sqlCommand)
	{
		if (sqlCommand == null)
		{
			throw new ArgumentNullException("sqlCommand");
		}
		ExecuteNonQuery(sqlCommand, ExecutionTypes.Default);
	}

	public void ExecuteNonQuery(string sqlCommand, ExecutionTypes executionType)
	{
		if (sqlCommand == null)
		{
			throw new ArgumentNullException("sqlCommand");
		}
		CheckObjectState();
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(sqlCommand);
		ExecuteNonQuery(stringCollection, executionType);
	}

	public void ExecuteNonQuery(StringCollection sqlCommands)
	{
		if (sqlCommands == null)
		{
			throw new ArgumentNullException("sqlCommands");
		}
		ExecuteNonQuery(sqlCommands, ExecutionTypes.Default);
	}

	public void ExecuteNonQuery(StringCollection sqlCommands, ExecutionTypes executionType)
	{
		if (sqlCommands == null)
		{
			throw new ArgumentNullException("sqlCommands");
		}
		try
		{
			CheckObjectState();
			InsertUseDb(0, sqlCommands, DatabaseEngineType);
			ExecutionManager.ExecuteNonQuery(sqlCommands, executionType);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExecuteNonQuery, this, ex);
		}
	}

	public DataSet ExecuteWithResults(StringCollection sqlCommands)
	{
		if (sqlCommands == null)
		{
			throw new ArgumentNullException("sqlCommands");
		}
		try
		{
			CheckObjectState();
			InsertUseDb(0, sqlCommands, DatabaseEngineType);
			return ExecutionManager.ExecuteWithResults(sqlCommands);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExecuteWithResults, this, ex);
		}
	}

	public DataSet ExecuteWithResults(string sqlCommand)
	{
		if (sqlCommand == null)
		{
			throw new ArgumentNullException("sqlCommand");
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(sqlCommand);
		return ExecuteWithResults(stringCollection);
	}

	public bool IsMember(string groupOrRole)
	{
		try
		{
			CheckObjectState();
			bool result = false;
			string sqlCommand = "IF IS_MEMBER('" + groupOrRole + "') = 1 BEGIN SELECT 1 END ELSE BEGIN SELECT 0 END";
			DataSet dataSet = ExecuteWithResults(sqlCommand);
			DataTable dataTable = dataSet.Tables[0];
			DataRow dataRow = dataTable.Rows[0];
			DataColumn column = dataTable.Columns[0];
			if (dataRow[column].ToString() == "1")
			{
				result = true;
			}
			dataSet.Dispose();
			return result;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.IsMember, this, ex);
		}
	}

	internal void GetAutoGrowFilesScript(StringCollection query, ScriptingPreferences sp)
	{
		string text = FormatFullNameForScripting(sp);
		foreach (FileGroup fileGroup in m_FileGroups)
		{
			if (!fileGroup.IsFileStream && fileGroup.AutogrowAllFiles && fileGroup.Files.Count > 0)
			{
				string text2 = string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} MODIFY FILEGROUP [{1}] AUTOGROW_ALL_FILES", new object[2]
				{
					text,
					SqlSmoObject.SqlBraket(fileGroup.Name)
				});
				query.Add(text2.ToString());
			}
		}
	}

	internal void GetFileGroupsScript(StringBuilder query, bool databaseIsView, StringCollection emptyfgs, ScriptingPreferences sp)
	{
		FileGroup fileGroup = m_FileGroups["PRIMARY"];
		if (fileGroup == null)
		{
			throw new PropertyNotSetException("Primary file");
		}
		if (base.State == SqlSmoState.Existing)
		{
			fileGroup.Initialize(allProperties: true);
		}
		fileGroup.ScriptDdl(sp, query, databaseIsView);
		foreach (FileGroup fileGroup2 in m_FileGroups)
		{
			if (fileGroup2 != fileGroup)
			{
				if (base.State == SqlSmoState.Existing)
				{
					fileGroup2.Initialize(allProperties: true);
				}
				if (fileGroup2.Files.Count > 0)
				{
					query.Append(Globals.commaspace);
					query.Append(Globals.newline);
					fileGroup2.ScriptDdl(sp, query, databaseIsView);
				}
				else
				{
					fileGroup2.ScriptCreateInternal(emptyfgs, sp);
				}
			}
		}
	}

	internal void GetLogFilesScript(ScriptingPreferences sp, StringBuilder query)
	{
		int num = 0;
		foreach (LogFile logFile in m_LogFiles)
		{
			if (num++ > 0)
			{
				query.Append(Globals.commaspace);
			}
			query.Append(Globals.newline);
			if (base.State == SqlSmoState.Existing)
			{
				logFile.Initialize(allProperties: true);
			}
			FileGroup.GetFileScriptWithCheck(sp, logFile, query, databaseIsView: false);
		}
	}

	public void ChangeMirroringState(MirroringOption mirroringOption)
	{
		try
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			string text = null;
			string text2 = "PARTNER";
			bool flag = false;
			switch (mirroringOption)
			{
			case MirroringOption.Off:
				text = "OFF";
				break;
			case MirroringOption.Suspend:
				text = "SUSPEND";
				break;
			case MirroringOption.Resume:
				text = "RESUME";
				break;
			case MirroringOption.RemoveWitness:
				text2 = "WITNESS";
				text = "OFF";
				break;
			case MirroringOption.Failover:
				text = "FAILOVER";
				flag = true;
				break;
			case MirroringOption.ForceFailoverAndAllowDataLoss:
				text = "FORCE_SERVICE_ALLOW_DATA_LOSS ";
				break;
			}
			if (text != null)
			{
				string text3 = string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET {1} {2}", new object[3]
				{
					FormatFullNameForScripting(new ScriptingPreferences()),
					text2,
					text
				});
				if (flag)
				{
					text3 = Scripts.USEMASTER + ";" + text3;
				}
				ExecutionManager.ExecuteNonQuery(text3);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangeMirroringState, this, ex);
		}
	}

	public void DropBackupHistory()
	{
		try
		{
			CheckObjectState();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropBackupHistory, this, ex);
		}
		Server parent = Parent;
		parent.DeleteBackupHistory(Name);
	}

	private DatabaseEncryptionKey InitializeDatabaseEncryptionKey()
	{
		SqlSmoState state = ((!IsDatabaseEncryptionKeyPresent()) ? SqlSmoState.Creating : SqlSmoState.Existing);
		return new DatabaseEncryptionKey(this, new ObjectKeyBase(), state);
	}

	private bool IsDatabaseEncryptionKeyPresent()
	{
		bool result = false;
		if (!base.IsDesignMode)
		{
			try
			{
				string query = string.Format(SmoApplication.DefaultCulture, "select create_date from sys.dm_database_encryption_keys where database_id = DB_ID({0})", new object[1] { SqlSmoObject.MakeSqlString(Name) });
				DataTable dataTable = ExecuteSql.ExecuteWithResults(query, ExecutionManager.ConnectionContext);
				result = dataTable.Rows.Count != 0;
			}
			catch (ExecutionFailureException ex)
			{
				if (!(ex.InnerException is SqlException ex2) || (ex2.Number != 300 && ex2.Number != 262))
				{
					throw;
				}
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Trace("Database SMO Object", ex.Message);
			}
		}
		return result;
	}

	private bool IsDEKInitializedWithoutAnyPropertiesSet()
	{
		if (m_DatabaseEncryptionKey.State == SqlSmoState.Creating)
		{
			return !m_DatabaseEncryptionKey.InternalIsObjectDirty;
		}
		return false;
	}

	internal bool DoesMasterKeyAlreadyExist()
	{
		return masterKey != null;
	}

	internal void SetRefMasterKey(MasterKey mk)
	{
		masterKey = mk;
	}

	internal void SetNullRefMasterKey()
	{
		masterKey = null;
	}

	private MasterKey InitializeMasterKey()
	{
		Request request = new Request(string.Concat(base.Urn, "/", MasterKey.UrnSuffix));
		request.Fields = new string[1] { "CreateDate" };
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
		if (1 != enumeratorData.Rows.Count)
		{
			return null;
		}
		return new MasterKey(this, new ObjectKeyBase(), SqlSmoState.Existing);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_ColumnEncryptionKeys != null)
		{
			m_ColumnEncryptionKeys.MarkAllDropped();
		}
		if (m_ColumnMasterKeys != null)
		{
			m_ColumnMasterKeys.MarkAllDropped();
		}
		if (m_LogFiles != null)
		{
			m_LogFiles.MarkAllDropped();
		}
		if (m_FileGroups != null)
		{
			m_FileGroups.MarkAllDropped();
		}
		if (m_Tables != null)
		{
			m_Tables.MarkAllDropped();
		}
		if (m_StoredProcedures != null)
		{
			m_StoredProcedures.MarkAllDropped();
		}
		if (m_ExtendedStoredProcedures != null)
		{
			m_ExtendedStoredProcedures.MarkAllDropped();
		}
		if (m_Views != null)
		{
			m_Views.MarkAllDropped();
		}
		if (m_Users != null)
		{
			m_Users.MarkAllDropped();
		}
		if (m_Roles != null)
		{
			m_Roles.MarkAllDropped();
		}
		if (m_Defaults != null)
		{
			m_Defaults.MarkAllDropped();
		}
		if (m_Rules != null)
		{
			m_Rules.MarkAllDropped();
		}
		if (m_UserDefinedDataTypes != null)
		{
			m_UserDefinedDataTypes.MarkAllDropped();
		}
		if (m_UserDefinedTableTypes != null)
		{
			m_UserDefinedTableTypes.MarkAllDropped();
		}
		if (m_PartitionFunctions != null)
		{
			m_PartitionFunctions.MarkAllDropped();
		}
		if (m_PartitionSchemes != null)
		{
			m_PartitionSchemes.MarkAllDropped();
		}
		if (m_SqlAssemblies != null)
		{
			m_SqlAssemblies.MarkAllDropped();
		}
		if (m_UserDefinedTypes != null)
		{
			m_UserDefinedTypes.MarkAllDropped();
		}
		if (m_UserDefinedAggregates != null)
		{
			m_UserDefinedAggregates.MarkAllDropped();
		}
		if (m_FullTextCatalogs != null)
		{
			m_FullTextCatalogs.MarkAllDropped();
		}
		if (m_FullTextStopLists != null)
		{
			m_FullTextStopLists.MarkAllDropped();
		}
		if (m_SearchPropertyLists != null)
		{
			m_SearchPropertyLists.MarkAllDropped();
		}
		if (m_SecurityPolicies != null)
		{
			m_SecurityPolicies.MarkAllDropped();
		}
		if (m_ExternalDataSources != null)
		{
			m_ExternalDataSources.MarkAllDropped();
		}
		if (m_ExternalFileFormats != null)
		{
			m_ExternalFileFormats.MarkAllDropped();
		}
		if (m_XmlSchemaCollections != null)
		{
			m_XmlSchemaCollections.MarkAllDropped();
		}
		if (databaseDdlTriggerCollection != null)
		{
			databaseDdlTriggerCollection.MarkAllDropped();
		}
		if (m_PlanGuides != null)
		{
			m_PlanGuides.MarkAllDropped();
		}
		if (m_DatabaseEncryptionKey != null)
		{
			m_DatabaseEncryptionKey.MarkDroppedInternal();
		}
		if (databaseAuditSpecifications != null)
		{
			databaseAuditSpecifications.MarkAllDropped();
		}
	}

	private DataTable EnumGeneric(Urn urn)
	{
		return ExecutionManager.GetEnumeratorData(new Request(urn));
	}

	public DataTable EnumLocks(int processId)
	{
		CheckObjectState();
		return EnumGeneric(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/Lock[@RequestorSpid={0}]", new object[1] { processId }));
	}

	public DataTable EnumLocks()
	{
		CheckObjectState();
		return EnumGeneric(base.Urn.Value + "/Lock");
	}

	public DataTable EnumLoginMappings()
	{
		CheckObjectState();
		Request request = new Request(base.Urn.Value + "/User[@Login != '']", new string[2] { "Name", "Login" });
		request.PropertyAlias = new PropertyAlias(new string[2] { "UserName", "LoginName" });
		return ExecutionManager.GetEnumeratorData(request);
	}

	public DataTable EnumWindowsGroups()
	{
		CheckObjectState();
		return EnumGeneric(string.Format(SmoApplication.DefaultCulture, "{0}/User[@LoginType={1}]", new object[2]
		{
			base.Urn.Value,
			1
		}));
	}

	public DataTable EnumWindowsGroups(string groupName)
	{
		CheckObjectState();
		if (groupName == null)
		{
			return EnumWindowsGroups();
		}
		return EnumGeneric(string.Format(SmoApplication.DefaultCulture, "{0}/User[@LoginType={2} and @Name='{1}']", new object[3]
		{
			base.Urn.Value,
			Urn.EscapeString(groupName),
			1
		}));
	}

	public StringCollection CheckAllocations(RepairType repairType)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append("DBCC CHECKALLOC");
			switch (repairType)
			{
			case RepairType.Fast:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_FAST) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.Rebuild:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_REBUILD) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.AllowDataLoss:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_ALLOW_DATA_LOSS) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			}
			stringBuilder.Append(" WITH NO_INFOMSGS");
			stringCollection.Add(stringBuilder.ToString());
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckAllocations, this, ex);
		}
	}

	public StringCollection CheckAllocationsDataOnly()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKALLOC(N'{0}', NOINDEX)", new object[1] { SqlSmoObject.SqlString(Name) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckAllocations, this, ex);
		}
	}

	public StringCollection CheckCatalog()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			stringCollection.Add("DBCC CHECKCATALOG");
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckCatalog, this, ex);
		}
	}

	public void CheckIdentityValues()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			foreach (Table table in Tables)
			{
				bool flag = false;
				foreach (Column column in table.Columns)
				{
					flag |= column.Identity;
				}
				if (flag)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKIDENT(N'{0}', NORESEED)", new object[1] { SqlSmoObject.SqlString(table.FullQualifiedName) }));
				}
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckIdentityValues, this, ex);
		}
	}

	public StringCollection CheckTables(RepairType repairType)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append("DBCC CHECKDB");
			switch (repairType)
			{
			case RepairType.Fast:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_FAST) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.Rebuild:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_REBUILD) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.AllowDataLoss:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_ALLOW_DATA_LOSS) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.None:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}') ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			}
			stringBuilder.Append(" WITH NO_INFOMSGS");
			stringCollection.Add(stringBuilder.ToString());
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckTables, this, ex);
		}
	}

	public StringCollection CheckTables(RepairType repairType, RepairOptions repairOptions, RepairStructure repairStructure, long? maxDOP = null)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.Append("DBCC CHECKDB");
			switch (repairType)
			{
			case RepairType.Fast:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_FAST) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.Rebuild:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_REBUILD) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.AllowDataLoss:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}', REPAIR_ALLOW_DATA_LOSS) ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			case RepairType.None:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "(N'{0}') ", new object[1] { SqlSmoObject.SqlString(Name) });
				break;
			}
			string text = GenerateRepairOptionsScript(repairOptions, repairStructure, maxDOP);
			if (text.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " WITH {0} ", new object[1] { text });
			}
			else
			{
				stringBuilder.Append(" WITH NO_INFOMSGS");
			}
			stringCollection.Add(stringBuilder.ToString());
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckTables, this, ex);
		}
	}

	public StringCollection CheckTables(RepairType repairType, RepairStructure repairStructure)
	{
		return CheckTables(repairType, RepairOptions.None, repairStructure);
	}

	public StringCollection CheckTables(RepairType repairType, RepairOptions repairOptions)
	{
		return CheckTables(repairType, repairOptions, RepairStructure.None);
	}

	public StringCollection CheckTablesDataOnly()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKDB(N'{0}', NOINDEX)", new object[1] { SqlSmoObject.SqlString(Name) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckTables, this, ex);
		}
	}

	public StringCollection CheckTablesDataOnly(RepairOptions repairOptions)
	{
		return CheckTablesDataOnly(repairOptions, RepairStructure.None);
	}

	public StringCollection CheckTablesDataOnly(RepairStructure repairStructure)
	{
		return CheckTablesDataOnly(RepairOptions.None, repairStructure);
	}

	public StringCollection CheckTablesDataOnly(RepairOptions repairOptions, RepairStructure repairStructure, long? maxDOP = null)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DBCC CHECKDB(N'{0}', NOINDEX)", new object[1] { SqlSmoObject.SqlString(Name) });
			string text = GenerateRepairOptionsScript(repairOptions, repairStructure, maxDOP);
			if (text.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " WITH {0} ", new object[1] { text });
			}
			else
			{
				stringBuilder.Append(" WITH NO_INFOMSGS");
			}
			stringCollection.Add(stringBuilder.ToString());
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckTables, this, ex);
		}
	}

	private string GenerateRepairOptionsScript(RepairOptions repairOptions, RepairStructure repairStructure, long? maxDOP = null)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if ((repairOptions & RepairOptions.AllErrorMessages) == RepairOptions.AllErrorMessages)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ALL_ERRORMSGS ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
		}
		if ((repairOptions & RepairOptions.ExtendedLogicalChecks) == RepairOptions.ExtendedLogicalChecks)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " EXTENDED_LOGICAL_CHECKS ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
		}
		if ((repairOptions & RepairOptions.NoInformationMessages) == RepairOptions.NoInformationMessages)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " NO_INFOMSGS ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
		}
		if ((repairOptions & RepairOptions.TableLock) == RepairOptions.TableLock)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TABLOCK ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
		}
		if ((repairOptions & RepairOptions.EstimateOnly) == RepairOptions.EstimateOnly)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " ESTIMATEONLY ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
		}
		if (maxDOP.HasValue)
		{
			ThrowIfBelowVersion130Prop("MAXDOP");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " MAXDOP = {0} ", new object[1] { maxDOP.Value });
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
		}
		switch (repairStructure)
		{
		case RepairStructure.PhysicalOnly:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " PHYSICAL_ONLY ");
			break;
		case RepairStructure.DataPurity:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " DATA_PURITY ");
			break;
		default:
			if (stringBuilder.ToString().EndsWith(Globals.comma))
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			break;
		}
		return stringBuilder.ToString();
	}

	public void Shrink(int percentFreeSpace, ShrinkMethod shrinkMethod)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			string empty = string.Empty;
			empty = shrinkMethod switch
			{
				ShrinkMethod.Default => " )", 
				ShrinkMethod.NoTruncate => ", NOTRUNCATE)", 
				ShrinkMethod.TruncateOnly => ", TRUNCATEONLY)", 
				ShrinkMethod.EmptyFile => throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(ExceptionTemplatesImpl.InvalidShrinkMethod(ShrinkMethod.EmptyFile.ToString()))), 
				_ => throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(ExceptionTemplatesImpl.UnknownShrinkType)), 
			};
			if (percentFreeSpace <= 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DBCC SHRINKDATABASE(N'{0}'{1}", new object[2]
				{
					SqlSmoObject.SqlString(Name),
					empty
				});
			}
			else
			{
				if (percentFreeSpace > 100)
				{
					percentFreeSpace = 100;
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DBCC SHRINKDATABASE(N'{0}', {1}{2}", new object[3]
				{
					SqlSmoObject.SqlString(Name),
					percentFreeSpace,
					empty
				});
			}
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Shrink, this, ex);
		}
	}

	public void RecalculateSpaceUsage()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			stringCollection.Add("DBCC UPDATEUSAGE(0) WITH NO_INFOMSGS");
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RecalculateSpaceUsage, ex);
		}
	}

	public void PrefetchObjects()
	{
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.IncludeScripts.ExtendedProperties = true;
		PrefetchTables(scriptingPreferences);
		PrefetchViews(scriptingPreferences);
		PrefetchPartitionSchemes(scriptingPreferences);
		PrefetchPartitionFunctions(scriptingPreferences);
		PrefetchOtherObjects(scriptingPreferences);
		PrefetchScriptingOnlyChildren(scriptingPreferences);
	}

	public void PrefetchObjects(Type objectType)
	{
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
		scriptingPreferences.IncludeScripts.ExtendedProperties = true;
		PrefetchObjectsImpl(objectType, scriptingPreferences);
	}

	internal void PrefetchObjects(Type objectType, ScriptingPreferences scriptingPreferences)
	{
		ScriptingPreferences scriptingPreferences2 = (ScriptingPreferences)scriptingPreferences.Clone();
		scriptingPreferences2.IncludeScripts.ExtendedProperties = true;
		PrefetchObjectsImpl(objectType, scriptingPreferences2);
	}

	public void PrefetchObjects(Type objectType, ScriptingOptions scriptingOptions)
	{
		PrefetchObjects(objectType, scriptingOptions.GetScriptingPreferences());
	}

	private void PrefetchObjectsImpl(Type objectType, ScriptingPreferences scriptingPreferences)
	{
		try
		{
			if (null == objectType)
			{
				throw new ArgumentNullException("objectType");
			}
			if (scriptingPreferences == null)
			{
				throw new ArgumentNullException("scriptingPreferences");
			}
			CheckObjectState();
			if (base.State == SqlSmoState.Creating)
			{
				return;
			}
			if (objectType.Equals(typeof(Table)))
			{
				PrefetchTables(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(View)))
			{
				PrefetchViews(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(StoredProcedure)))
			{
				PrefetchStoredProcedures(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(User)))
			{
				PrefetchUsers(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(Default)))
			{
				PrefetchDefaults(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(Rule)))
			{
				PrefetchRules(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(UserDefinedFunction)))
			{
				PrefetchUserDefinedFunctions(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(ExtendedStoredProcedure)))
			{
				PrefetchExtendedStoredProcedures(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(UserDefinedType)))
			{
				PrefetchUserDefinedTypes(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(UserDefinedTableType)))
			{
				PrefetchUserDefinedTableTypes(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(UserDefinedAggregate)))
			{
				PrefetchUserDefinedAggregates(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(PartitionScheme)))
			{
				PrefetchPartitionSchemes(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(PartitionFunction)))
			{
				PrefetchPartitionFunctions(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(XmlSchemaCollection)))
			{
				PrefetchXmlSchemaCollections(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(SqlAssembly)))
			{
				PrefetchSqlAssemblies(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(Schema)))
			{
				PrefetchSchemas(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(DatabaseRole)))
			{
				PrefetchDatabaseRoles(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(UserDefinedDataType)))
			{
				PrefetchUDDT(scriptingPreferences);
				return;
			}
			if (objectType.Equals(typeof(Sequence)))
			{
				PrefetchSequences(scriptingPreferences);
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.InvalidType(objectType.FullName));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PrefetchObjects, this, ex);
		}
	}

	internal void PrefetchStoredProcedures(ScriptingPreferences options)
	{
		InitChildLevel("StoredProcedure", options, forScripting: true);
		InitChildLevel("StoredProcedure[@IsSystemObject=false()]/Param", options, forScripting: true);
		if (this.IsSupportedObject<NumberedStoredProcedure>(options))
		{
			InitChildLevel("StoredProcedure/Numbered", options, forScripting: true);
		}
		if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
		{
			InitChildLevel("StoredProcedure/ExtendedProperty", options, forScripting: true);
			InitChildLevel("StoredProcedure[@IsSystemObject=false()]/Param/ExtendedProperty", options, forScripting: true);
		}
		if (options.IncludeScripts.Permissions)
		{
			InitChildLevel("StoredProcedure/Permission", options, forScripting: true);
		}
	}

	internal void PrefetchUsers(ScriptingPreferences options)
	{
		InitChildLevel("User", options, forScripting: true);
		if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
		{
			InitChildLevel("User/ExtendedProperty", options, forScripting: true);
		}
		if (options.IncludeScripts.Permissions && base.ServerVersion.Major > 8)
		{
			InitChildLevel("User/Permission", options, forScripting: true);
		}
	}

	internal void PrefetchScriptingOnlyChildren(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<DatabaseScopedConfiguration>(options))
		{
			InitChildLevel("DatabaseScopedConfiguration", options, forScripting: true);
		}
	}

	internal void PrefetchDatabaseRoles(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<DatabaseRole>(options))
		{
			InitChildLevel("Role", options, forScripting: true);
			if (options.IncludeScripts.Permissions && base.ServerVersion.Major > 8)
			{
				InitChildLevel("Role/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchDefaults(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<Default>(options))
		{
			InitChildLevel("Default", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("Default/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchRules(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<Rule>(options))
		{
			InitChildLevel("Rule", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("Rule/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchExternalLibraries(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<ExternalLibrary>(options))
		{
			InitChildLevel("ExternalLibrary", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("ExternalLibrary/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchUserDefinedFunctions(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<UserDefinedFunction>(options))
		{
			InitChildLevel("UserDefinedFunction", options, forScripting: true);
			InitChildLevel("UserDefinedFunction/Param", options, forScripting: true);
			InitChildLevel("UserDefinedFunction/Check", options, forScripting: true);
			InitChildLevel("UserDefinedFunction/Column", options, forScripting: true);
			InitChildLevel("UserDefinedFunction/Column/Default", options, forScripting: true);
			InitChildLevel("UserDefinedFunction/Index", options, forScripting: true);
			InitChildLevel("UserDefinedFunction/Index/IndexedColumn", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("UserDefinedFunction/ExtendedProperty", options, forScripting: true);
				InitChildLevel("UserDefinedFunction/Param/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("UserDefinedFunction/Permission", options, forScripting: true);
				InitChildLevel("UserDefinedFunction/Column/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchUserDefinedAggregates(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<UserDefinedAggregate>(options))
		{
			InitChildLevel("UserDefinedAggregate", options, forScripting: true);
			InitChildLevel("UserDefinedAggregate/Param", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("UserDefinedAggregate/ExtendedProperty", options, forScripting: true);
				InitChildLevel("UserDefinedAggregate/Param/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("UserDefinedAggregate/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchExtendedStoredProcedures(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<ExtendedStoredProcedure>(options))
		{
			InitChildLevel("ExtendedStoredProcedure", options, forScripting: true);
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("ExtendedStoredProcedure/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchSequences(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<Sequence>(options))
		{
			InitChildLevel("Sequence", options, forScripting: true);
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("Sequence/Permission", options, forScripting: true);
			}
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("Sequence/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchTables(ScriptingPreferences options)
	{
		PrefetchTables(options, "Table");
	}

	internal void PrefetchTables(ScriptingPreferences options, string tableFilter)
	{
		PrefetchUDDT(options);
		PrefetchObjects(options, EnumerateTableFiltersForPrefetch(tableFilter, options));
	}

	internal void PrefetchViews(ScriptingPreferences options)
	{
		PrefetchViews(options, "View");
	}

	internal void PrefetchViews(ScriptingPreferences options, string viewFilter)
	{
		PrefetchObjects(options, EnumerateViewFiltersForPrefetch(viewFilter, options));
	}

	private void PrefetchObjects(ScriptingPreferences options, IEnumerable<string> filters)
	{
		foreach (string filter in filters)
		{
			InitChildLevel(filter, options, forScripting: true);
		}
	}

	internal IEnumerable<string> EnumerateTableFiltersForPrefetch(string tableFilter, ScriptingPreferences options)
	{
		yield return tableFilter;
		if (this.IsSupportedObject<Column>(options))
		{
			yield return tableFilter + "/Column";
		}
		if (this.IsSupportedObject<Default>(options))
		{
			yield return tableFilter + "/Column/Default";
		}
		if (this.IsSupportedObject<Check>(options))
		{
			yield return tableFilter + "/Check";
		}
		if (this.IsSupportedObject<ForeignKey>(options))
		{
			yield return tableFilter + "/ForeignKey";
			yield return tableFilter + "/ForeignKey/Column";
		}
		if (this.IsSupportedObject<Trigger>(options))
		{
			yield return tableFilter + "/Trigger";
		}
		if (this.IsSupportedObject<Index>(options))
		{
			yield return tableFilter + "/Index";
			yield return tableFilter + "/Index/IndexedColumn";
		}
		if (this.IsSupportedObject<FullTextIndex>(options))
		{
			yield return tableFilter + "/FullTextIndex";
			yield return tableFilter + "/FullTextIndex/FullTextIndexColumn";
		}
		if (this.IsSupportedObject<Statistic>(options))
		{
			yield return tableFilter + "/Statistic";
			yield return tableFilter + "/Statistic/Column";
		}
		if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
		{
			yield return tableFilter + "/ExtendedProperty";
			if (this.IsSupportedObject<Column>(options))
			{
				yield return tableFilter + "/Column/ExtendedProperty";
			}
			if (this.IsSupportedObject<Default>(options))
			{
				yield return tableFilter + "/Column/Default/ExtendedProperty";
			}
			if (this.IsSupportedObject<Check>(options))
			{
				yield return tableFilter + "/Check/ExtendedProperty";
			}
			if (this.IsSupportedObject<ForeignKey>(options))
			{
				yield return tableFilter + "/ForeignKey/ExtendedProperty";
			}
			if (this.IsSupportedObject<Trigger>(options))
			{
				yield return tableFilter + "/Trigger/ExtendedProperty";
			}
			if (this.IsSupportedObject<Index>(options))
			{
				yield return tableFilter + "/Index/ExtendedProperty";
			}
		}
		if (options.IncludeScripts.Permissions)
		{
			yield return tableFilter + "/Permission";
			if (this.IsSupportedObject<Column>(options))
			{
				yield return tableFilter + "/Column/Permission";
			}
		}
	}

	internal IEnumerable<string> EnumerateViewFiltersForPrefetch(string viewFilter, ScriptingPreferences options)
	{
		yield return viewFilter;
		if (this.IsSupportedObject<Column>(options))
		{
			yield return viewFilter + "/Column";
			if (options.IncludeScripts.Permissions)
			{
				yield return viewFilter + "/Column/Permission";
			}
		}
		if (this.IsSupportedObject<Trigger>(options))
		{
			yield return viewFilter + "/Trigger";
		}
		if (this.IsSupportedObject<Index>(options))
		{
			yield return viewFilter + "/Index";
			yield return viewFilter + "/Index/IndexedColumn";
		}
		if (this.IsSupportedObject<Statistic>(options))
		{
			yield return viewFilter + "/Statistic";
			yield return viewFilter + "/Statistic/Column";
		}
		if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
		{
			yield return viewFilter + "/ExtendedProperty";
			if (this.IsSupportedObject<Column>(options))
			{
				yield return viewFilter + "/Column/ExtendedProperty";
			}
			if (this.IsSupportedObject<Trigger>(options))
			{
				yield return viewFilter + "/Trigger/ExtendedProperty";
			}
			if (this.IsSupportedObject<Index>(options))
			{
				yield return viewFilter + "/Index/ExtendedProperty";
			}
		}
		if (options.IncludeScripts.Permissions)
		{
			yield return viewFilter + "/Permission";
		}
		if (base.ServerVersion.Major > 8 && this.IsSupportedObject<FullTextIndex>(options))
		{
			yield return viewFilter + "/FullTextIndex";
			yield return viewFilter + "/FullTextIndex/FullTextIndexColumn";
		}
	}

	internal void PrefetchUDDT(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<UserDefinedDataType>(options))
		{
			Parent.SystemDataTypes.GetEnumerator();
			InitChildLevel("UserDefinedDataType", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("UserDefinedDataType/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions && base.ServerVersion.Major > 8)
			{
				InitChildLevel("UserDefinedDataType/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchUserDefinedTableTypes(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<UserDefinedTableType>(options))
		{
			InitChildLevel("UserDefinedTableType", options, forScripting: true);
			InitChildLevel("UserDefinedTableType/Column", options, forScripting: true);
			InitChildLevel("UserDefinedTableType/Index", options, forScripting: true);
			InitChildLevel("UserDefinedTableType/Index/IndexedColumn", options, forScripting: true);
			InitChildLevel("UserDefinedTableType/Check", options, forScripting: true);
			InitChildLevel("UserDefinedTableType/Permission", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("UserDefinedTableType/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchUserDefinedTypes(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<UserDefinedType>(options))
		{
			InitChildLevel("UserDefinedType", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("UserDefinedType/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("UserDefinedType/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchPartitionSchemes(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<PartitionScheme>(options))
		{
			InitChildLevel("PartitionScheme", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("PartitionScheme/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchPartitionFunctions(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<PartitionFunction>(options))
		{
			InitChildLevel("PartitionFunction", options, forScripting: true);
			InitChildLevel("PartitionFunction/PartitionFunctionParameter", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("PartitionFunction/ExtendedProperty", options, forScripting: true);
			}
		}
	}

	internal void PrefetchSchemas(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<Schema>(options))
		{
			InitChildLevel("Schema", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("Schema/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("Schema/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchXmlSchemaCollections(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<XmlSchemaCollection>(options))
		{
			InitChildLevel("XmlSchemaCollection", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("XmlSchemaCollection/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("XmlSchemaCollection/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchSqlAssemblies(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<SqlAssembly>(options))
		{
			InitChildLevel("SqlAssembly", options, forScripting: true);
			InitChildLevel("SqlAssembly[@IsSystemObject=false()]/SqlAssemblyFile", options, forScripting: true);
			if (options.IncludeScripts.ExtendedProperties && this.IsSupportedObject<ExtendedProperty>(options))
			{
				InitChildLevel("SqlAssembly/ExtendedProperty", options, forScripting: true);
			}
			if (options.IncludeScripts.Permissions)
			{
				InitChildLevel("SqlAssembly/Permission", options, forScripting: true);
			}
		}
	}

	internal void PrefetchDatabaseScopedCredentials(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<DatabaseScopedCredential>(options))
		{
			InitChildLevel("DatabaseScopedCredential", options, forScripting: true);
		}
	}

	internal void PrefetchExternalFileFormats(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<ExternalFileFormat>(options))
		{
			InitChildLevel("ExternalFileFormat", options, forScripting: true);
		}
	}

	internal void PrefetchExternalDataSources(ScriptingPreferences options)
	{
		if (this.IsSupportedObject<ExternalDataSource>(options))
		{
			InitChildLevel("ExternalDataSource", options, forScripting: true);
		}
	}

	internal void PrefetchOtherObjects(ScriptingPreferences options)
	{
		PrefetchStoredProcedures(options);
		PrefetchUsers(options);
		PrefetchDatabaseRoles(options);
		PrefetchDefaults(options);
		PrefetchRules(options);
		PrefetchUserDefinedFunctions(options);
		PrefetchExtendedStoredProcedures(options);
		PrefetchUserDefinedTypes(options);
		PrefetchExternalLibraries(options);
		PrefetchUserDefinedTableTypes(options);
		PrefetchUserDefinedAggregates(options);
		PrefetchXmlSchemaCollections(options);
		PrefetchSqlAssemblies(options);
		PrefetchSchemas(options);
		PrefetchDatabaseScopedCredentials(options);
		PrefetchExternalDataSources(options);
		PrefetchExternalFileFormats(options);
	}

	internal override void PreInitChildLevel()
	{
		InitializeStringComparer();
	}

	public DataTable EnumTransactions()
	{
		try
		{
			return EnumTransactions(TransactionTypes.Both);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumTransactions, this, ex);
		}
	}

	public DataTable EnumTransactions(TransactionTypes transactionType)
	{
		try
		{
			CheckObjectState();
			if (base.State == SqlSmoState.Creating)
			{
				throw new InvalidSmoOperationException("EnumTransactions", base.State);
			}
			if (base.ServerVersion.Major < 9)
			{
				throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
			}
			return ExecutionManager.GetEnumeratorData(new Request("Server/Transaction" + GetTranFilterExpr(transactionType)));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumTransactions, this, ex);
		}
	}

	public int GetTransactionCount()
	{
		try
		{
			return GetTransactionCount(TransactionTypes.Both);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetTransactionCount, this, ex);
		}
	}

	public int GetTransactionCount(TransactionTypes transactionType)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion90();
			DataTable dataTable = EnumTransactions(transactionType);
			return dataTable.Rows.Count;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetTransactionCount, this, ex);
		}
	}

	private string GetTranFilterExpr(TransactionTypes tt)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[@DatabaseID = " + ID);
		switch (tt)
		{
		case TransactionTypes.Versioned:
			stringBuilder.Append(" and @IsVersioned = true()");
			break;
		case TransactionTypes.UnVersioned:
			stringBuilder.Append(" and @IsVersioned = false()");
			break;
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public void RemoveFullTextCatalogs()
	{
		try
		{
			this.ThrowIfNotSupported(typeof(FullTextCatalog));
			CheckObjectState();
			if (FullTextCatalogs.Count <= 0)
			{
				return;
			}
			StringCollection stringCollection = new StringCollection();
			AddUseDb(stringCollection);
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			scriptingPreferences.SetTargetServerInfo(this);
			foreach (FullTextCatalog fullTextCatalog in FullTextCatalogs)
			{
				if (fullTextCatalog.State != SqlSmoState.Creating)
				{
					fullTextCatalog.ScriptDropInternal(stringCollection, scriptingPreferences);
				}
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
			FullTextCatalogs.Refresh();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveFullTextCatalogs, this, ex);
		}
	}

	public void SetDefaultFullTextCatalog(string catalog)
	{
		try
		{
			ThrowIfBelowVersion90();
			this.ThrowIfNotSupported(typeof(FullTextCatalog));
			CheckObjectState();
			if (catalog == null)
			{
				throw new ArgumentNullException("catalog");
			}
			if (catalog.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("catalog", "string"));
			}
			StringCollection stringCollection = new StringCollection();
			AddUseDb(stringCollection);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT CATALOG [{0}] AS DEFAULT", new object[1] { SqlSmoObject.SqlBraket(catalog) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				Property property = base.Properties.Get("DefaultFullTextCatalog");
				property.SetValue(catalog);
				property.SetRetrieved(retrieved: true);
				property = FullTextCatalogs[catalog].Properties.Get("IsDefault");
				property.SetValue(true);
				property.SetRetrieved(retrieved: true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDefaultFullTextCatalog, this, ex);
		}
	}

	public void SetDefaultFileGroup(string fileGroupName)
	{
		try
		{
			if (fileGroupName == null)
			{
				throw new ArgumentNullException("fileGroupName");
			}
			if (fileGroupName.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("fileGroupName", "string"));
			}
			ScriptingPreferences sp = new ScriptingPreferences(this);
			StringCollection defaultFileGroupScript = GetDefaultFileGroupScript(sp, fileGroupName);
			ExecutionManager.ExecuteNonQuery(defaultFileGroupScript);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDefaultFileGroup, this, ex);
		}
	}

	private StringCollection GetDefaultFileGroupScript(ScriptingPreferences sp, string dataSpaceName)
	{
		StringCollection stringCollection = new StringCollection();
		if (sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version70)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.SupportedOnlyOn80).SetHelpContext("SupportedOnlyOn80");
		}
		if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version80)
		{
			AddUseDb(stringCollection, sp);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "IF NOT EXISTS (SELECT groupname FROM dbo.sysfilegroups WHERE (status & 0x10) != 0 AND groupname = {2}) ALTER DATABASE {0} MODIFY FILEGROUP [{1}] DEFAULT", new object[3]
			{
				FormatFullNameForScripting(sp),
				SqlSmoObject.SqlBraket(dataSpaceName),
				SqlSmoObject.MakeSqlString(dataSpaceName)
			}));
		}
		else
		{
			AddUseDb(stringCollection, sp);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = {2}) ALTER DATABASE {0} MODIFY FILEGROUP [{1}] DEFAULT", new object[3]
			{
				FormatFullNameForScripting(sp),
				SqlSmoObject.SqlBraket(dataSpaceName),
				SqlSmoObject.MakeSqlString(dataSpaceName)
			}));
		}
		return stringCollection;
	}

	public void SetDefaultFileStreamFileGroup(string fileGroupName)
	{
		try
		{
			if (fileGroupName == null)
			{
				throw new ArgumentNullException("fileGroupName");
			}
			if (fileGroupName.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("fileGroupName", "string"));
			}
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			if (scriptingPreferences.TargetServerVersionInternal < SqlServerVersionInternal.Version100)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.SupportedOnlyOn100).SetHelpContext("SupportedOnlyOn100");
			}
			StringCollection defaultFileGroupScript = GetDefaultFileGroupScript(scriptingPreferences, fileGroupName);
			ExecutionManager.ExecuteNonQuery(defaultFileGroupScript);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetDefaultFileStreamFileGroup, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		masterKeyInitialized = false;
		databaseEncryptionKeyInitialized = false;
		m_edition = null;
	}

	public DataTable EnumBackupSets()
	{
		Request req = new Request($"Server/BackupSet[@DatabaseName='{Name}']");
		return ExecutionManager.GetEnumeratorData(req);
	}

	public DataTable EnumBackupSetFiles(int backupSetID)
	{
		Request req = new Request($"Server/BackupSet[@DatabaseName='{Name}' and @ID='{backupSetID}']/File");
		return ExecutionManager.GetEnumeratorData(req);
	}

	public DataTable EnumBackupSetFiles()
	{
		Request req = new Request($"Server/BackupSet[@DatabaseName='{Name}']/File");
		return ExecutionManager.GetEnumeratorData(req);
	}

	public DataTable EnumCandidateKeys()
	{
		try
		{
			CheckObjectState();
			Request request = new Request(base.Urn.Value + "/Table/Index[@IndexKeyType > 0]", new string[1] { "Name" });
			request.ParentPropertiesRequests = new PropertiesRequest[1]
			{
				new PropertiesRequest(new string[1] { "Name" })
			};
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
			enumeratorData.Columns[0].Caption = "candidate_table";
			enumeratorData.Columns[1].Caption = "candidate_key";
			return enumeratorData;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumCandidateKeys, this, ex);
		}
	}

	public void UpdateIndexStatistics()
	{
		try
		{
			if (Name == "tempdb")
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.FailedOperationExceptionText3("UpdateIndexStatistics", ExceptionTemplatesImpl.Database, "tempdb", ExceptionTemplatesImpl.FailedOperationMessageNotSupportedTempdb));
			}
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendLine("SET ARITHABORT ON");
			stringBuilder.AppendLine("SET CONCAT_NULL_YIELDS_NULL ON");
			stringBuilder.AppendLine("SET QUOTED_IDENTIFIER ON");
			stringBuilder.AppendLine("SET ANSI_NULLS ON");
			stringBuilder.AppendLine("SET ANSI_PADDING ON");
			stringBuilder.AppendLine("SET ANSI_WARNINGS ON");
			stringBuilder.AppendLine("SET NUMERIC_ROUNDABORT OFF");
			stringCollection.Add(stringBuilder.ToString());
			Server serverObject = GetServerObject();
			StringCollection defaultInitFields = serverObject.GetDefaultInitFields(typeof(Table), DatabaseEngineEdition);
			try
			{
				StringCollection defaultInitFields2 = serverObject.GetDefaultInitFields(typeof(Table), DatabaseEngineEdition);
				defaultInitFields2.Add("IsSystemObject");
				serverObject.SetDefaultInitFields(typeof(Table), defaultInitFields2, DatabaseEngineEdition);
				foreach (Table table in Tables)
				{
					if (base.ServerVersion.Major > 8 || !table.IsSystemObject)
					{
						string value = string.Format(SmoApplication.DefaultCulture, "UPDATE STATISTICS {0}.{1}", new object[2]
						{
							SqlSmoObject.MakeSqlBraket(table.Schema),
							SqlSmoObject.MakeSqlBraket(table.Name)
						});
						stringCollection.Add(value);
					}
				}
			}
			finally
			{
				serverObject.SetDefaultInitFields(typeof(Table), defaultInitFields, DatabaseEngineEdition);
			}
			if (base.ServerVersion.Major > 7)
			{
				Request request = new Request();
				request.Urn = string.Concat(base.Urn, "/View/Statistic[@IsFromIndexCreation = true()]");
				request.Fields = new string[1] { "ID" };
				request.ParentPropertiesRequests = new PropertiesRequest[1]
				{
					new PropertiesRequest()
				};
				request.ParentPropertiesRequests[0].Fields = new string[2] { "Schema", "Name" };
				request.ParentPropertiesRequests[0].OrderByList = new OrderBy[2]
				{
					new OrderBy("Schema", OrderBy.Direction.Asc),
					new OrderBy("Name", OrderBy.Direction.Asc)
				};
				DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
				if (enumeratorData.Rows.Count > 0)
				{
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert("View_Schema" == enumeratorData.Columns[0].Caption);
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert("View_Name" == enumeratorData.Columns[1].Caption);
					string text = string.Empty;
					string text2 = string.Empty;
					foreach (DataRow row in enumeratorData.Rows)
					{
						if (!(text == (string)row[0]) || !(text2 == (string)row[1]))
						{
							text = (string)row[0];
							text2 = (string)row[1];
							stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "UPDATE STATISTICS {0}.{1}", new object[2]
							{
								SqlSmoObject.MakeSqlBraket(text),
								SqlSmoObject.MakeSqlBraket(text2)
							}));
						}
					}
				}
			}
			ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateIndexStatistics, this, ex);
		}
	}

	public UrnCollection EnumMatchingSPs(string description, bool includeSystem)
	{
		if (description == null)
		{
			throw new ArgumentNullException("description");
		}
		try
		{
			CheckObjectState();
			Request request = new Request(base.Urn.Value + "/StoredProcedure" + (includeSystem ? "" : "[@IsSystemObject = false()]") + "/Text[contains(@Text, '" + Urn.EscapeString(description) + "')]", new string[0]);
			request.ParentPropertiesRequests = new PropertiesRequest[1]
			{
				new PropertiesRequest(new string[1] { "Urn" })
			};
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
			UrnCollection urnCollection = new UrnCollection();
			foreach (DataRow row in enumeratorData.Rows)
			{
				Urn urn = new Urn((string)row[0]);
				if (!urnCollection.Contains(urn))
				{
					urnCollection.Add(urn);
				}
			}
			return urnCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMatchingSPs, this, ex);
		}
	}

	public UrnCollection EnumMatchingSPs(string description)
	{
		return EnumMatchingSPs(description, includeSystem: false);
	}

	public DataTable EnumObjects(DatabaseObjectTypes types, SortOrder order)
	{
		try
		{
			SortedList sortedList = new SortedList();
			sortedList[DatabaseObjectTypes.ApplicationRole] = new UrnInfo("ApplicationRole", hasSchema: false, hasName: true, DatabaseObjectTypes.ApplicationRole, 7);
			sortedList[DatabaseObjectTypes.ServiceBroker] = new UrnInfo("ServiceBroker", hasSchema: false, hasName: false, 1, DatabaseObjectTypes.ServiceBroker, 9);
			sortedList[DatabaseObjectTypes.Default] = new UrnInfo("Default", hasSchema: false, hasName: true, DatabaseObjectTypes.Default, 7);
			sortedList[DatabaseObjectTypes.ExtendedStoredProcedure] = new UrnInfo("ExtendedStoredProcedure", hasSchema: true, hasName: true, DatabaseObjectTypes.ExtendedStoredProcedure, 7);
			sortedList[DatabaseObjectTypes.FullTextCatalog] = new UrnInfo("FullTextCatalog", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.FullTextCatalog, 7);
			sortedList[DatabaseObjectTypes.MessageType] = new UrnInfo("ServiceBroker/MessageType", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.MessageType, 9);
			sortedList[DatabaseObjectTypes.PartitionFunction] = new UrnInfo("PartitionFunction", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.PartitionFunction, 9);
			sortedList[DatabaseObjectTypes.PartitionScheme] = new UrnInfo("PartitionScheme", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.PartitionScheme, 9);
			sortedList[DatabaseObjectTypes.DatabaseRole] = new UrnInfo("Role", hasSchema: false, hasName: true, DatabaseObjectTypes.DatabaseRole, 7);
			sortedList[DatabaseObjectTypes.RemoteServiceBinding] = new UrnInfo("ServiceBroker/RemoteServiceBinding", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.RemoteServiceBinding, 9);
			sortedList[DatabaseObjectTypes.Rule] = new UrnInfo("Rule", hasSchema: false, hasName: true, DatabaseObjectTypes.Rule, 7);
			sortedList[DatabaseObjectTypes.Schema] = new UrnInfo("Schema", hasSchema: false, hasName: true, DatabaseObjectTypes.Schema, 9);
			sortedList[DatabaseObjectTypes.ServiceContract] = new UrnInfo("ServiceBroker/ServiceContract", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.ServiceContract, 9);
			sortedList[DatabaseObjectTypes.ServiceQueue] = new UrnInfo("ServiceBroker/ServiceQueue", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.ServiceQueue, 9);
			sortedList[DatabaseObjectTypes.ServiceRoute] = new UrnInfo("ServiceBroker/ServiceRoute", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.ServiceRoute, 9);
			sortedList[DatabaseObjectTypes.SqlAssembly] = new UrnInfo("SqlAssembly", hasSchema: false, hasName: true, DatabaseObjectTypes.SqlAssembly, 9);
			sortedList[DatabaseObjectTypes.StoredProcedure] = new UrnInfo("StoredProcedure", hasSchema: true, hasName: true, DatabaseObjectTypes.StoredProcedure, 7);
			sortedList[DatabaseObjectTypes.Synonym] = new UrnInfo("Synonym", hasSchema: false, hasName: true, DatabaseObjectTypes.Synonym, 9);
			sortedList[DatabaseObjectTypes.Sequence] = new UrnInfo("Sequence", hasSchema: false, hasName: true, DatabaseObjectTypes.Sequence, 11);
			sortedList[DatabaseObjectTypes.Table] = new UrnInfo("Table", hasSchema: true, hasName: true, DatabaseObjectTypes.Table, 7);
			sortedList[DatabaseObjectTypes.User] = new UrnInfo("User", hasSchema: false, hasName: true, DatabaseObjectTypes.User, 7);
			sortedList[DatabaseObjectTypes.UserDefinedAggregate] = new UrnInfo("UserDefinedAggregate", hasSchema: true, hasName: true, 1, DatabaseObjectTypes.UserDefinedAggregate, 9);
			sortedList[DatabaseObjectTypes.UserDefinedDataType] = new UrnInfo("UserDefinedDataType", hasSchema: false, hasName: true, DatabaseObjectTypes.UserDefinedDataType, 7);
			sortedList[DatabaseObjectTypes.UserDefinedTableTypes] = new UrnInfo("UserDefinedTableType", hasSchema: true, hasName: true, 1, DatabaseObjectTypes.UserDefinedTableTypes, 9);
			sortedList[DatabaseObjectTypes.UserDefinedFunction] = new UrnInfo("UserDefinedFunction", hasSchema: true, hasName: true, DatabaseObjectTypes.UserDefinedFunction, 8);
			sortedList[DatabaseObjectTypes.UserDefinedType] = new UrnInfo("UserDefinedType", hasSchema: false, hasName: true, DatabaseObjectTypes.UserDefinedType, 9);
			sortedList[DatabaseObjectTypes.View] = new UrnInfo("View", hasSchema: true, hasName: true, DatabaseObjectTypes.View, 7);
			sortedList[DatabaseObjectTypes.XmlSchemaCollection] = new UrnInfo("XmlSchemaCollection", hasSchema: false, hasName: true, DatabaseObjectTypes.XmlSchemaCollection, 9);
			sortedList[DatabaseObjectTypes.Certificate] = new UrnInfo("Certificate", hasSchema: false, hasName: true, DatabaseObjectTypes.Certificate, 9);
			sortedList[DatabaseObjectTypes.SymmetricKey] = new UrnInfo("SymmetricKey", hasSchema: false, hasName: true, DatabaseObjectTypes.SymmetricKey, 9);
			sortedList[DatabaseObjectTypes.AsymmetricKey] = new UrnInfo("AsymmetricKey", hasSchema: false, hasName: true, DatabaseObjectTypes.AsymmetricKey, 9);
			sortedList[DatabaseObjectTypes.PlanGuide] = new UrnInfo("PlanGuide", hasSchema: false, hasName: true, DatabaseObjectTypes.PlanGuide, 9);
			sortedList[DatabaseObjectTypes.DatabaseEncryptionKey] = new UrnInfo("DatabaseEncryptionKey", hasSchema: false, hasName: false, 1, DatabaseObjectTypes.DatabaseEncryptionKey, 10);
			sortedList[DatabaseObjectTypes.DatabaseAuditSpecification] = new UrnInfo("DatabaseAuditSpecification", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.DatabaseAuditSpecification, 10);
			sortedList[DatabaseObjectTypes.FullTextStopList] = new UrnInfo("FullTextStopList", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.FullTextStopList, 10);
			sortedList[DatabaseObjectTypes.SearchPropertyList] = new UrnInfo("SearchPropertyList", hasSchema: false, hasName: true, 1, DatabaseObjectTypes.SearchPropertyList, 10);
			sortedList[DatabaseObjectTypes.SecurityPolicy] = new UrnInfo("SecurityPolicy", hasSchema: true, hasName: true, DatabaseObjectTypes.SecurityPolicy, 13);
			sortedList[DatabaseObjectTypes.ExternalDataSource] = new UrnInfo("ExternalDataSource", hasSchema: false, hasName: true, DatabaseObjectTypes.ExternalDataSource, 13);
			sortedList[DatabaseObjectTypes.ExternalFileFormat] = new UrnInfo("ExternalFileFormat", hasSchema: false, hasName: true, DatabaseObjectTypes.ExternalFileFormat, 13);
			sortedList[DatabaseObjectTypes.DatabaseScopedCredential] = new UrnInfo("DatabaseScopedCredential", hasSchema: false, hasName: true, DatabaseObjectTypes.DatabaseScopedCredential, 13);
			sortedList[DatabaseObjectTypes.DatabaseScopedConfiguration] = new UrnInfo("DatabaseScopedConfiguration", hasSchema: false, hasName: true, DatabaseObjectTypes.DatabaseScopedConfiguration, 13);
			StringCollection stringCollection = new StringCollection();
			string[] array = new string[1] { "Urn" };
			string[] array2 = new string[2] { "Name", "Urn" };
			string[] array3 = new string[3] { "Schema", "Name", "Urn" };
			int major = base.ServerVersion.Major;
			int minor = base.ServerVersion.Minor;
			SupportedEngineType supportedEngineType = SupportedEngineType.Standalone;
			foreach (DatabaseObjectTypes key in sortedList.Keys)
			{
				if ((key & types) != key)
				{
					continue;
				}
				UrnInfo urnInfo = (UrnInfo)sortedList[key];
				if (urnInfo.VersionMajor <= major && (urnInfo.VersionMajor != major || urnInfo.VersionMinor <= minor) && ((uint)urnInfo.SupportedEngineTypes & (uint)supportedEngineType) == (uint)supportedEngineType)
				{
					string[] fields = (urnInfo.HasSchema ? array3 : (urnInfo.HasName ? array2 : array));
					Request request = new Request(string.Concat(base.Urn, "/", urnInfo.UrnType), fields);
					request.ResultType = ResultType.Reserved2;
					ServerInformation connectionInfo = new ServerInformation(ExecutionManager.GetServerVersion(), ExecutionManager.GetProductVersion(), ExecutionManager.GetDatabaseEngineType(), ExecutionManager.GetDatabaseEngineEdition());
					SqlEnumResult sqlEnumResult = (SqlEnumResult)Enumerator.GetData(connectionInfo, request);
					StatementBuilder statementBuilder = sqlEnumResult.StatementBuilder;
					statementBuilder.AddProperty("DatabaseObjectTypes", "N'" + key.ToString() + "'");
					if (!urnInfo.HasSchema)
					{
						statementBuilder.AddProperty("Schema", "''");
					}
					if (!urnInfo.HasName)
					{
						statementBuilder.AddProperty("Name", "''");
					}
					stringCollection.Add(statementBuilder.GetSqlNoPrefixPostfix());
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("create table #t([DatabaseObjectTypes] nvarchar(100), [Schema] sysname, [Name] sysname, [Urn] nvarchar(2000))\n");
			for (int i = 0; i < stringCollection.Count; i++)
			{
				stringBuilder.Append("\ninsert #t\n");
				stringBuilder.Append(stringCollection[i]);
			}
			stringBuilder.Replace("\n + ", "\n'" + Urn.EscapeString(base.Urn) + "' + ");
			stringBuilder.Append("\nselect [DatabaseObjectTypes], [Schema], [Name], [Urn] from #t");
			stringBuilder.Append(" ORDER BY ");
			switch (order)
			{
			case SortOrder.Name:
				stringBuilder.Append("Name");
				break;
			case SortOrder.Schema:
				stringBuilder.Append("Schema");
				break;
			case SortOrder.Type:
				stringBuilder.Append("DatabaseObjectTypes");
				break;
			default:
				stringBuilder.Append("Urn");
				break;
			}
			stringBuilder.Append("\ndrop table #t");
			return ExecuteWithResults(stringBuilder.ToString()).Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumObjects, this, ex);
		}
	}

	public DataTable EnumObjects()
	{
		return EnumObjects(DatabaseObjectTypes.All, SortOrder.Type);
	}

	public DataTable EnumObjects(DatabaseObjectTypes types)
	{
		return EnumObjects(types, SortOrder.Type);
	}

	public void TruncateLog()
	{
		if (base.ServerVersion.Major >= 10)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.SupportedOnlyBelow100).SetHelpContext("SupportedOnlyBelow100");
		}
		try
		{
			CheckObjectState();
			string cmd = string.Format(SmoApplication.DefaultCulture, "BACKUP LOG {0} WITH TRUNCATE_ONLY ", new object[1] { FormatFullNameForScripting(new ScriptingPreferences()) });
			ExecutionManager.ExecuteNonQuery(cmd);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.TruncateLog, this, ex);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[5] { "CatalogCollation", "Collation", "ContainmentType", "DatabaseSnapshotBaseName", "DefaultSchema" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	internal static string[] GetScriptFields2(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode, ScriptingPreferences sp)
	{
		string[] fields = new string[3] { "CompatibilityLevel", "IsMirroringEnabled", "IsVarDecimalStorageFormatEnabled" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	private void ScriptVardecimalCompression(StringCollection query, ScriptingPreferences sp, bool forCreate)
	{
		if (IsSupportedProperty("IsVarDecimalStorageFormatEnabled", sp) && (sp.ForDirectExecution || !sp.OldOptions.NoVardecimal) && IsVarDecimalStorageFormatSupported)
		{
			Property property = base.Properties.Get("IsVarDecimalStorageFormatEnabled");
			if (property.Dirty || (forCreate && base.State == SqlSmoState.Existing && IsVarDecimalStorageFormatEnabled))
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sys.sp_db_vardecimal_storage_format N'{0}', N'{1}'", new object[2]
				{
					Util.EscapeString(Name, '\''),
					IsVarDecimalStorageFormatEnabled ? "ON" : "OFF"
				}));
			}
		}
	}

	public void EnableAllPlanGuides()
	{
		EnableDisableDropAllPlanGuides("ENABLE ALL");
	}

	public void DisableAllPlanGuides()
	{
		EnableDisableDropAllPlanGuides("DISABLE ALL");
	}

	public void DropAllPlanGuides()
	{
		EnableDisableDropAllPlanGuides("DROP ALL");
	}

	private void EnableDisableDropAllPlanGuides(string action)
	{
		try
		{
			CheckObjectStateImpl(throwIfNotCreated: true);
			this.ThrowIfNotSupported(typeof(PlanGuideType));
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SP_CONTROLPLANGUIDE, new object[1] { SqlSmoObject.MakeSqlString(action) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (m_PlanGuides != null)
			{
				m_PlanGuides.Refresh();
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PlanGuide, this, ex);
		}
	}

	public bool ValidateAllPlanGuides()
	{
		DataTable errorInfo;
		return ValidateAllPlanGuides(out errorInfo);
	}

	public bool ValidateAllPlanGuides(out DataTable errorInfo)
	{
		try
		{
			CheckObjectStateImpl(throwIfNotCreated: true);
			ThrowIfBelowVersion100();
			new StringBuilder();
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SELECT name, msgnum, severity, state, message FROM sys.plan_guides CROSS APPLY sys.fn_validate_plan_guide(plan_guide_id)"));
			errorInfo = ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
			bool result = false;
			if (errorInfo.Rows.Count == 0)
			{
				result = true;
			}
			return result;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PlanGuide, this, ex);
		}
	}

	public void ReauthorizeRemoteDataArchiveConnection(string credentialName, bool withCopy = true)
	{
		try
		{
			CheckObjectState();
			ThrowIfPropertyNotSupported("RemoteDataArchiveEnabled");
			if (string.IsNullOrEmpty(credentialName))
			{
				throw new ArgumentNullException("credentialName");
			}
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SP_RDA_REAUTHORIZE_DB, new object[2]
			{
				SqlSmoObject.MakeSqlString(credentialName),
				withCopy ? 1 : 0
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ReauthorizeRemoteDataArchive, this, ex);
		}
	}

	public IEnumerable<RemoteDataArchiveMigrationStatusReport> GetRemoteDataArchiveMigrationStatusReports(DateTime migrationStartTime, int statusReportCount, string tableName = null)
	{
		try
		{
			CheckObjectState();
			ThrowIfPropertyNotSupported("RemoteDataArchiveEnabled");
			StringCollection stringCollection = new StringCollection();
			if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
			}
			StatementBuilder statementBuilder = new StatementBuilder();
			statementBuilder.AddFields("dbs.name as database_name");
			statementBuilder.AddFields("tabs.name as table_name");
			statementBuilder.AddFields("rdams.migrated_rows");
			statementBuilder.AddFields("rdams.start_time_utc");
			statementBuilder.AddFields("rdams.end_time_utc");
			statementBuilder.AddFields("rdams.error_number");
			statementBuilder.AddFields("rdams.error_severity");
			statementBuilder.AddFields("rdams.error_state");
			statementBuilder.AddFields("msgs.text as details");
			statementBuilder.TopN = statusReportCount;
			statementBuilder.AddFrom("sys.dm_db_rda_migration_status rdams");
			statementBuilder.AddJoin("INNER JOIN sys.databases dbs ON rdams.database_id = dbs.database_id");
			statementBuilder.AddJoin("INNER JOIN sys.tables tabs ON rdams.table_id = tabs.object_id");
			statementBuilder.AddJoin("LEFT OUTER JOIN sys.messages msgs ON rdams.error_number = msgs.message_id");
			if (string.IsNullOrEmpty(tableName))
			{
				statementBuilder.AddWhere(string.Format(CultureInfo.InvariantCulture, "start_time_utc > '{0}'", new object[1] { Urn.EscapeString(migrationStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")) }));
			}
			else
			{
				statementBuilder.AddWhere(string.Format(CultureInfo.InvariantCulture, "start_time_utc > '{0}' AND tabs.name = '{1}'", new object[2]
				{
					Urn.EscapeString(migrationStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")),
					Urn.EscapeString(tableName)
				}));
			}
			stringCollection.Add(statementBuilder.SqlStatement);
			DataSet dataSet = ExecutionManager.ExecuteWithResults(stringCollection);
			IList<RemoteDataArchiveMigrationStatusReport> list = null;
			if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
			{
				list = new List<RemoteDataArchiveMigrationStatusReport>();
				DataTable dataTable = dataSet.Tables[0];
				foreach (DataRow row in dataTable.Rows)
				{
					int result;
					int? errorNumber = ((row["error_number"] != null) ? (int.TryParse(row["error_number"].ToString(), out result) ? new int?(result) : ((int?)null)) : ((int?)null));
					int? errorSeverity = ((row["error_severity"] != null) ? (int.TryParse(row["error_severity"].ToString(), out result) ? new int?(result) : ((int?)null)) : ((int?)null));
					int? errorState = ((row["error_state"] != null) ? (int.TryParse(row["error_state"].ToString(), out result) ? new int?(result) : ((int?)null)) : ((int?)null));
					string details = ((row["details"] != null) ? row["details"].ToString() : string.Empty);
					RemoteDataArchiveMigrationStatusReport item = new RemoteDataArchiveMigrationStatusReport((string)row["database_name"], (string)row["table_name"], (long)row["migrated_rows"], (DateTime)row["start_time_utc"], (DateTime)row["end_time_utc"], errorNumber, errorSeverity, errorState, details);
					list.Add(item);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetRemoteDataArchiveMigrationStatusReports, this, ex);
		}
	}

	public RemoteDatabaseMigrationStatistics GetRemoteDatabaseMigrationStatistics()
	{
		try
		{
			CheckObjectState();
			ThrowIfPropertyNotSupported("RemoteDataArchiveEnabled");
			if (RemoteDataArchiveEnabled)
			{
				StringCollection stringCollection = new StringCollection();
				if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Name) }));
				}
				stringCollection.Add("exec sp_spaceused @mode = 'REMOTE_ONLY', @oneresultset = 1");
				DataSet dataSet = ExecutionManager.ExecuteWithResults(stringCollection);
				double remoteDatabaseSizeInMB = 0.0;
				if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					string text = dataTable.Rows[0]["database_size"].ToString();
					if (text.ToUpperInvariant().IndexOf("MB") > -1)
					{
						string text2 = text.Substring(0, text.ToUpperInvariant().IndexOf("MB"));
						remoteDatabaseSizeInMB = double.Parse(text2.Trim());
					}
				}
				return new RemoteDatabaseMigrationStatistics(remoteDatabaseSizeInMB);
			}
			return null;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetRemoteDatabaseMigrationStatistics, this, ex);
		}
	}

	public void InitFileGroupFiles()
	{
		InitChildLevel("FileGroup", null, forScripting: false);
		InitChildLevel("FileGroup/File", null, forScripting: false);
	}

	public void InitTableColumns()
	{
		InitChildLevel("Table", null, forScripting: false);
		InitChildLevel("Table/Column", null, forScripting: false);
	}

	private void ScriptDbOptionsProps(StringCollection query, ScriptingPreferences sp, bool isAzureDb)
	{
		bool flag = sp.TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlManagedInstance;
		ScriptAlterPropBool("AnsiNullDefault", "ANSI_NULL_DEFAULT", sp, query);
		ScriptAlterPropBool("AnsiNullsEnabled", "ANSI_NULLS", sp, query);
		ScriptAlterPropBool("AnsiPaddingEnabled", "ANSI_PADDING", sp, query);
		ScriptAlterPropBool("AnsiWarningsEnabled", "ANSI_WARNINGS", sp, query);
		ScriptAlterPropBool("ArithmeticAbortEnabled", "ARITHABORT", sp, query);
		if (IsSupportedProperty("AutoClose", sp) && !flag)
		{
			ScriptAlterPropBool("AutoClose", "AUTO_CLOSE", sp, query);
		}
		ScriptAlterPropBool("AutoShrink", "AUTO_SHRINK", sp, query);
		ScriptAutoCreateStatistics(query, sp);
		ScriptAlterPropBool("AutoUpdateStatisticsEnabled", "AUTO_UPDATE_STATISTICS", sp, query);
		ScriptAlterPropBool("CloseCursorsOnCommitEnabled", "CURSOR_CLOSE_ON_COMMIT", sp, query);
		if (IsSupportedProperty("LocalCursorsDefault", sp) && !isAzureDb)
		{
			ScriptAlterPropBool("LocalCursorsDefault", "CURSOR_DEFAULT ", sp, query, "LOCAL", "GLOBAL");
		}
		ScriptAlterPropBool("ConcatenateNullYieldsNull", "CONCAT_NULL_YIELDS_NULL", sp, query);
		ScriptAlterPropBool("NumericRoundAbortEnabled", "NUMERIC_ROUNDABORT", sp, query);
		ScriptAlterPropBool("QuotedIdentifiersEnabled", "QUOTED_IDENTIFIER", sp, query);
		ScriptAlterPropBool("RecursiveTriggersEnabled", "RECURSIVE_TRIGGERS", sp, query);
		if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			if (IsSupportedProperty("BrokerEnabled", sp) && !flag)
			{
				ScriptAlterPropBool("BrokerEnabled", string.Empty, sp, query, "ENABLE_BROKER", "DISABLE_BROKER");
			}
			ScriptAlterPropBool("AutoUpdateStatisticsAsync", "AUTO_UPDATE_STATISTICS_ASYNC", sp, query);
			if (!flag && !isAzureDb)
			{
				ScriptAlterPropBool("DateCorrelationOptimization", "DATE_CORRELATION_OPTIMIZATION", sp, query);
			}
			if (!isAzureDb)
			{
				ScriptAlterPropBool("Trustworthy", "TRUSTWORTHY", sp, query);
			}
			if (IsSupportedProperty("SnapshotIsolationState", sp))
			{
				ScriptSnapshotIsolationState(sp, query);
			}
			ScriptAlterPropBool("IsParameterizationForced", "PARAMETERIZATION", sp, query, "FORCED", "SIMPLE");
			ScriptAlterPropBool("IsReadCommittedSnapshotOn", "READ_COMMITTED_SNAPSHOT", sp, query);
			if (IsSupportedProperty("MirroringPartner", sp) && GetPropValueOptional("IsMirroringEnabled", defaultValue: false))
			{
				Property property = base.Properties.Get("MirroringTimeout");
				if (property.Value != null && (property.Dirty || !sp.ForDirectExecution))
				{
					query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET PARTNER TIMEOUT {1} {2}", new object[3]
					{
						FormatFullNameForScripting(sp),
						Convert.ToInt32(property.Value, SmoApplication.DefaultCulture),
						(optionTerminationStatement != null) ? optionTerminationStatement.GetTerminationScript() : ""
					}));
				}
			}
		}
		if (IsSupportedProperty("HonorBrokerPriority", sp) && !isAzureDb && !flag)
		{
			ScriptAlterPropBool("HonorBrokerPriority", "HONOR_BROKER_PRIORITY", sp, query);
		}
		if (!flag && (sp.ScriptForAlter || sp.ForDirectExecution))
		{
			Property property2 = base.Properties.Get("ReadOnly");
			if (property2.Value != null)
			{
				ScriptAlterPropReadonly(query, sp, (bool)property2.Value);
			}
		}
		if (IsSupportedProperty("RecoveryModel", sp) && !flag)
		{
			Property property3 = base.Properties.Get("RecoveryModel");
			if (property3.Value != null && (property3.Dirty || !sp.ForDirectExecution))
			{
				RecoveryModel recoveryModel = (RecoveryModel)property3.Value;
				string empty = string.Empty;
				ScriptAlterPropBool("RecoveryModel", "RECOVERY", sp, query, recoveryModel switch
				{
					RecoveryModel.Full => "FULL", 
					RecoveryModel.Simple => "SIMPLE", 
					RecoveryModel.BulkLogged => "BULK_LOGGED", 
					_ => throw new SmoException(ExceptionTemplatesImpl.UnknownRecoveryModel(recoveryModel.ToString())), 
				});
			}
		}
		Property property4 = base.Properties.Get("UserAccess");
		if (property4.Value != null && (property4.Dirty || !sp.ForDirectExecution) && !flag)
		{
			DatabaseUserAccess databaseUserAccess = (DatabaseUserAccess)property4.Value;
			string val = "MULTI_USER";
			switch (databaseUserAccess)
			{
			case DatabaseUserAccess.Single:
				val = "SINGLE_USER";
				break;
			case DatabaseUserAccess.Restricted:
				val = "RESTRICTED_USER";
				break;
			case DatabaseUserAccess.Multiple:
				val = "MULTI_USER";
				break;
			}
			ScriptAlterPropBool("UserAccess", "", sp, query, val);
		}
		if (!flag)
		{
			ScriptPageVerify(sp, query);
		}
		if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
		{
			Property property5 = base.Properties.Get("DatabaseOwnershipChaining");
			if (property5.Value != null && (property5.Dirty || !sp.ForDirectExecution))
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "if ( ((@@microsoftversion / power(2, 24) = 8) and (@@microsoftversion & 0xffff >= 760)) or \n\t\t(@@microsoftversion / power(2, 24) >= 9) )begin \n\texec dbo.sp_dboption @dbname =  {0}, @optname = 'db chaining', @optvalue = '{1}'\n end", new object[2]
				{
					FormatFullNameForScripting(sp, nameIsIndentifier: false),
					((bool)property5.Value) ? "ON" : "OFF"
				}));
			}
		}
		else if (!isAzureDb)
		{
			ScriptAlterPropBool("DatabaseOwnershipChaining", "DB_CHAINING", sp, query);
		}
		if (IsSupportedProperty("ContainmentType", sp) && !flag && GetPropValueOptional("ContainmentType", ContainmentType.None) != ContainmentType.None)
		{
			AddDefaultLanguageOption("DefaultFullTextLanguageName", "DefaultFullTextLanguageLcid", "DEFAULT_FULLTEXT_LANGUAGE", sp, query);
			AddDefaultLanguageOption("DefaultLanguageName", "DefaultLanguageLcid", "DEFAULT_LANGUAGE", sp, query);
			ScriptAlterPropBool("NestedTriggersEnabled", "NESTED_TRIGGERS", sp, query, useEqualityOperator: true);
			ScriptAlterPropBool("TransformNoiseWords", "TRANSFORM_NOISE_WORDS", sp, query, useEqualityOperator: true);
			ScriptAlterPropBool("TwoDigitYearCutoff", "TWO_DIGIT_YEAR_CUTOFF", sp, query, Convert.ToString(GetPropValueOptional("TwoDigitYearCutoff"), SmoApplication.DefaultCulture), useEqualityOperator: true);
		}
		if (!flag)
		{
			ScriptAlterFileStreamProp(sp, query);
		}
		if (IsSupportedProperty("TargetRecoveryTime", sp) && !flag)
		{
			object propValueOptionalAllowNull = GetPropValueOptionalAllowNull("TargetRecoveryTime");
			if (propValueOptionalAllowNull != null)
			{
				if ((int)propValueOptionalAllowNull < 0)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.TargetRecoveryTimeNotNegative);
				}
				string val2 = Convert.ToString(propValueOptionalAllowNull, SmoApplication.DefaultCulture) + " SECONDS";
				ScriptAlterPropBool("TargetRecoveryTime", "TARGET_RECOVERY_TIME", sp, query, val2, useEqualityOperator: true);
			}
		}
		if (IsSupportedProperty("DelayedDurability", sp) && !flag)
		{
			object propValueOptionalAllowNull2 = GetPropValueOptionalAllowNull("DelayedDurability");
			if (propValueOptionalAllowNull2 != null)
			{
				string val3 = Convert.ToString(propValueOptionalAllowNull2, SmoApplication.DefaultCulture).ToUpperInvariant();
				ScriptAlterPropBool("DelayedDurability", "DELAYED_DURABILITY", sp, query, val3, useEqualityOperator: true);
			}
		}
	}

	private void ScriptAlterFileStreamProp(ScriptingPreferences sp, StringCollection query)
	{
		if (!IsSupportedProperty("FilestreamDirectoryName", sp))
		{
			return;
		}
		Property property = base.Properties.Get("FilestreamNonTransactedAccess");
		StringBuilder stringBuilder = new StringBuilder();
		if (property.Value != null)
		{
			FilestreamNonTransactedAccessType filestreamNonTransactedAccessType = (FilestreamNonTransactedAccessType)property.Value;
			if (property.Dirty || !sp.ForDirectExecution)
			{
				string text = null;
				switch (filestreamNonTransactedAccessType)
				{
				case FilestreamNonTransactedAccessType.Off:
					text = "OFF";
					break;
				case FilestreamNonTransactedAccessType.ReadOnly:
					text = "READ_ONLY";
					break;
				case FilestreamNonTransactedAccessType.Full:
					text = "FULL";
					break;
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "NON_TRANSACTED_ACCESS = {0}{1}", new object[2]
				{
					text,
					Globals.commaspace
				});
			}
		}
		Property property2 = base.Properties.Get("FilestreamDirectoryName");
		if ((property2.Dirty || !sp.ForDirectExecution) && !string.IsNullOrEmpty((string)property2.Value))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DIRECTORY_NAME = {0}{1}", new object[2]
			{
				SqlSmoObject.MakeSqlString((string)property2.Value),
				Globals.commaspace
			});
		}
		if (!string.IsNullOrEmpty(stringBuilder.ToString()))
		{
			stringBuilder.Remove(stringBuilder.ToString().Length - 2, 2);
			query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET FILESTREAM( {1} ) {2}", new object[3]
			{
				FormatFullNameForScripting(sp),
				stringBuilder.ToString(),
				(optionTerminationStatement != null) ? optionTerminationStatement.GetTerminationScript() : ""
			}));
		}
	}

	private void AddDefaultLanguageOption(string nameProperty, string lcidProperty, string optname, ScriptingPreferences sp, StringCollection query)
	{
		string text = Convert.ToString(GetPropValueOptional(nameProperty), CultureInfo.InvariantCulture);
		if (!GetPropertyOptional(nameProperty).Dirty || string.IsNullOrEmpty(text) || !ScriptAlterPropBool(nameProperty, optname, sp, query, SqlSmoObject.MakeSqlBraket(text), useEqualityOperator: true))
		{
			int? propValueOptional = GetPropValueOptional<int>(lcidProperty);
			if (propValueOptional >= 0)
			{
				ScriptAlterPropBool(lcidProperty, optname, sp, query, Convert.ToString(propValueOptional, CultureInfo.InvariantCulture), useEqualityOperator: true);
			}
		}
	}

	internal void ScriptAlterPropReadonly(StringCollection query, ScriptingPreferences sp, bool readonlyMode)
	{
		if (IsSupportedProperty("ReadOnly") && sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse && sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlManagedInstance)
		{
			ScriptAlterPropBool("ReadOnly", "", sp, query, readonlyMode ? "READ_ONLY" : "READ_WRITE");
		}
	}

	internal void Encryption(bool encryptionEnabled)
	{
		this.ThrowIfNotSupported(typeof(DatabaseEncryptionKey));
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE {0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " SET ENCRYPTION {0}", new object[1] { encryptionEnabled ? "ON" : "OFF" });
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	public void EnableEncryption(bool isEnabled)
	{
		Encryption(isEnabled);
	}

	private PageVerify GetPageVerify(ScriptingPreferences sp)
	{
		return GetPropValueOptional("PageVerify", PageVerify.None);
	}

	private void ScriptAlterContainmentDDL(ScriptingPreferences sp, StringCollection queries)
	{
		if (IsSupportedProperty("ContainmentType", sp))
		{
			switch (GetPropValueOptional("ContainmentType", ContainmentType.None))
			{
			case ContainmentType.None:
				ScriptAlterPropBool("ContainmentType", "CONTAINMENT", sp, queries, "NONE", useEqualityOperator: true);
				break;
			case ContainmentType.Partial:
				ScriptAlterPropBool("ContainmentType", "CONTAINMENT", sp, queries, "PARTIAL", useEqualityOperator: true);
				break;
			default:
				throw new WrongPropertyValueException(base.Properties.Get("ContainmentType"));
			}
		}
	}

	private void ScriptPageVerify(ScriptingPreferences sp, StringCollection queries)
	{
		if (!IsSupportedProperty("PageVerify", sp))
		{
			return;
		}
		if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
		{
			FormatFullNameForScripting(sp, nameIsIndentifier: false);
			switch (GetPageVerify(sp))
			{
			case PageVerify.TornPageDetection:
				ScriptAlterPropBool("PageVerify", "TORN_PAGE_DETECTION", sp, queries, "ON");
				return;
			case PageVerify.None:
				ScriptAlterPropBool("PageVerify", "TORN_PAGE_DETECTION", sp, queries, "OFF");
				return;
			}
			if (sp.ScriptForCreateDrop)
			{
				throw new WrongPropertyValueException(base.Properties.Get("PageVerify"));
			}
			return;
		}
		Property property = base.Properties.Get("PageVerify");
		if (property.Value == null || (!property.Dirty && sp.ForDirectExecution))
		{
			return;
		}
		string val = string.Empty;
		switch (GetPageVerify(sp))
		{
		case PageVerify.TornPageDetection:
			val = "TORN_PAGE_DETECTION ";
			break;
		case PageVerify.Checksum:
			val = "CHECKSUM ";
			break;
		case PageVerify.None:
			val = "NONE ";
			break;
		default:
			if (sp.ScriptForCreateDrop)
			{
				throw new WrongPropertyValueException(property);
			}
			break;
		}
		ScriptAlterPropBool("PageVerify", "PAGE_VERIFY", sp, queries, val);
	}

	private SnapshotIsolationState GetSnapshotIsolationState(ScriptingPreferences sp)
	{
		return (SnapshotIsolationState)GetPropValueOptional("SnapshotIsolationState");
	}

	private void ScriptSnapshotIsolationState(ScriptingPreferences sp, StringCollection queries)
	{
		Property property = base.Properties.Get("SnapshotIsolationState");
		if (property.Value == null || (!property.Dirty && sp.ForDirectExecution))
		{
			return;
		}
		string val = string.Empty;
		switch (GetSnapshotIsolationState(sp))
		{
		case SnapshotIsolationState.Enabled:
			val = "ON";
			break;
		case SnapshotIsolationState.Disabled:
			val = "OFF";
			break;
		default:
			if (sp.ScriptForCreateDrop)
			{
				throw new WrongPropertyValueException(property);
			}
			break;
		}
		ScriptAlterPropBool("SnapshotIsolationState", "ALLOW_SNAPSHOT_ISOLATION", sp, queries, val);
	}

	private void ScriptAlterPropBool(string propname, string optname, ScriptingPreferences sp, StringCollection queries)
	{
		ScriptAlterPropBool(propname, optname, sp, queries, useEqualityOperator: false);
	}

	private void ScriptAlterPropBool(string propname, string optname, ScriptingPreferences sp, StringCollection queries, bool useEqualityOperator)
	{
		ScriptAlterPropBool(propname, optname, sp, queries, "ON", "OFF", useEqualityOperator);
	}

	private void ScriptAlterPropBool(string propname, string optname, ScriptingPreferences sp, StringCollection queries, string scriptTrue, string scriptFalse)
	{
		ScriptAlterPropBool(propname, optname, sp, queries, scriptTrue, scriptFalse, useEqualityOperator: false);
	}

	private void ScriptAlterPropBool(string propname, string optname, ScriptingPreferences sp, StringCollection queries, string scriptTrue, string scriptFalse, bool useEqualityOperator)
	{
		Property property = ((base.State == SqlSmoState.Creating || base.IsDesignMode) ? base.Properties.Get(propname) : base.Properties[propname]);
		if (property.Value != null && (property.Dirty || !sp.ForDirectExecution))
		{
			ScriptAlterPropBool(propname, optname, sp, queries, ((bool)property.Value) ? scriptTrue : scriptFalse, useEqualityOperator);
		}
	}

	private void ScriptAlterPropBool(string propname, string optname, ScriptingPreferences sp, StringCollection queries, string val)
	{
		ScriptAlterPropBool(propname, optname, sp, queries, val, useEqualityOperator: false);
	}

	private bool ScriptAlterPropBool(string propname, string optname, ScriptingPreferences sp, StringCollection queries, string val, bool useEqualityOperator)
	{
		bool result = false;
		Property property = ((base.State == SqlSmoState.Creating) ? base.Properties.Get(propname) : base.Properties[propname]);
		if (property.Value != null && (property.Dirty || !sp.ForDirectExecution))
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE {0} SET {1} {2}{3} {4}", FormatFullNameForScripting(sp), optname, useEqualityOperator ? "= " : string.Empty, val, (optionTerminationStatement != null) ? optionTerminationStatement.GetTerminationScript() : ""));
			result = true;
		}
		return result;
	}

	public bool IsLocalPrimaryReplica()
	{
		bool result = false;
		Server serverObject = GetServerObject();
		if (base.ServerVersion.Major < 11)
		{
			return result;
		}
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType || !serverObject.IsHadrEnabled)
		{
			return result;
		}
		if (string.IsNullOrEmpty(AvailabilityGroupName))
		{
			return result;
		}
		AvailabilityGroup availabilityGroup = serverObject.AvailabilityGroups[AvailabilityGroupName];
		if (availabilityGroup == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("AvailabilityGroup"));
		}
		string primaryReplicaServerName = availabilityGroup.PrimaryReplicaServerName;
		return NetCoreHelpers.StringCompare(GetServerName(), primaryReplicaServerName, ignoreCase: true, CultureInfo.InvariantCulture) == 0;
	}
}

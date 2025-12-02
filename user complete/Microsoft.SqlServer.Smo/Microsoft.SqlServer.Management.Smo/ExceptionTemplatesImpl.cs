using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.Management.Smo;

[CompilerGenerated]
internal class ExceptionTemplatesImpl
{
	[CompilerGenerated]
	public class Keys
	{
		public const string InvalidPasswordHash = "InvalidPasswordHash";

		public const string LoginPropertyNotSet = "LoginPropertyNotSet";

		public const string LoginEnable = "LoginEnable";

		public const string LoginDisable = "LoginDisable";

		public const string AddCredential = "AddCredential";

		public const string DropCredential = "DropCredential";

		public const string EnterServerRoleName = "EnterServerRoleName";

		public const string ServerRoleOwnerNameEmpty = "ServerRoleOwnerNameEmpty";

		public const string CreateAlterNotSupported = "CreateAlterNotSupported";

		public const string EnterSequenceName = "EnterSequenceName";

		public const string EnterMinValue = "EnterMinValue";

		public const string EnterMaxValue = "EnterMaxValue";

		public const string EnterIncrementValue = "EnterIncrementValue";

		public const string EnterStartValue = "EnterStartValue";

		public const string InvalidSequenceValue = "InvalidSequenceValue";

		public const string UpgradeDll = "UpgradeDll";

		public const string ProviderEnable = "ProviderEnable";

		public const string ProviderDisable = "ProviderDisable";

		public const string SourceTypeShouldBeProvider = "SourceTypeShouldBeProvider";

		public const string CannotSetPrivilege = "CannotSetPrivilege";

		public const string CyclicalForeignKeys = "CyclicalForeignKeys";

		public const string UsersWithoutLoginsDownLevel = "UsersWithoutLoginsDownLevel";

		public const string EncryptedUserDefinedFunctionsDownlevel = "EncryptedUserDefinedFunctionsDownlevel";

		public const string EncryptedStoredProcedureDownlevel = "EncryptedStoredProcedureDownlevel";

		public const string EncryptedViewsFunctionsDownlevel = "EncryptedViewsFunctionsDownlevel";

		public const string SchemaDownlevel = "SchemaDownlevel";

		public const string UserDefinedAggregatesDownlevel = "UserDefinedAggregatesDownlevel";

		public const string XmlSchemaCollectionDownlevel = "XmlSchemaCollectionDownlevel";

		public const string SynonymDownlevel = "SynonymDownlevel";

		public const string SequenceDownlevel = "SequenceDownlevel";

		public const string SecurityPolicyDownlevel = "SecurityPolicyDownlevel";

		public const string ExternalDataSourceDownlevel = "ExternalDataSourceDownlevel";

		public const string ColumnEncryptionKeyDownlevel = "ColumnEncryptionKeyDownlevel";

		public const string ExternalFileFormatDownlevel = "ExternalFileFormatDownlevel";

		public const string ColumnMasterKeyDownlevel = "ColumnMasterKeyDownlevel";

		public const string DatabaseScopedCredentialDownlevel = "DatabaseScopedCredentialDownlevel";

		public const string UserDefinedTableDownlevel = "UserDefinedTableDownlevel";

		public const string DdlTriggerDownlevel = "DdlTriggerDownlevel";

		public const string ClrUserDefinedFunctionDownlevel = "ClrUserDefinedFunctionDownlevel";

		public const string ClrStoredProcedureDownlevel = "ClrStoredProcedureDownlevel";

		public const string AssemblyDownlevel = "AssemblyDownlevel";

		public const string ColumnEncryptionKeyNoValues = "ColumnEncryptionKeyNoValues";

		public const string SecurityPolicyNoPredicates = "SecurityPolicyNoPredicates";

		public const string NullParameterTable = "NullParameterTable";

		public const string ComputedColumnDownlevelContraint = "ComputedColumnDownlevelContraint";

		public const string NullParameterScriptingOptions = "NullParameterScriptingOptions";

		public const string DataScriptingUnsupportedDataTypeException = "DataScriptingUnsupportedDataTypeException";

		public const string StoredProcedureDownlevelExecutionContext = "StoredProcedureDownlevelExecutionContext";

		public const string UserDefinedFunctionDownlevelExecutionContext = "UserDefinedFunctionDownlevelExecutionContext";

		public const string TriggerDownlevelExecutionContext = "TriggerDownlevelExecutionContext";

		public const string UnsupportedColumnCollation = "UnsupportedColumnCollation";

		public const string UnsupportedDatabaseCollation = "UnsupportedDatabaseCollation";

		public const string UnsupportedColumnType = "UnsupportedColumnType";

		public const string UnsupportedColumnTypeOnEngineType = "UnsupportedColumnTypeOnEngineType";

		public const string CollectionNotAvailable = "CollectionNotAvailable";

		public const string CreateOrAlterDownlevel = "CreateOrAlterDownlevel";

		public const string CreateOrAlterNotSupported = "CreateOrAlterNotSupported";

		public const string TriggerNotSupported = "TriggerNotSupported";

		public const string CannotEnableViewTrigger = "CannotEnableViewTrigger";

		public const string ReplicationOptionNotSupportedForCloud = "ReplicationOptionNotSupportedForCloud";

		public const string UnknownRecoveryModel = "UnknownRecoveryModel";

		public const string UnknownUserAccess = "UnknownUserAccess";

		public const string CannotRenameObject = "CannotRenameObject";

		public const string PropertyNotSet = "PropertyNotSet";

		public const string ColumnAlreadyHasDefault = "ColumnAlreadyHasDefault";

		public const string ColumnHasNoDefault = "ColumnHasNoDefault";

		public const string NoSparseOnColumnSet = "NoSparseOnColumnSet";

		public const string NoSparseOnComputed = "NoSparseOnComputed";

		public const string NoColumnSetOnComputed = "NoColumnSetOnComputed";

		public const string NoGeneratedAlwaysColumnsOnNonTables = "NoGeneratedAlwaysColumnsOnNonTables";

		public const string NoSparseOrColumnSetOnTemporalColumns = "NoSparseOrColumnSetOnTemporalColumns";

		public const string ComputedTemporalColumns = "ComputedTemporalColumns";

		public const string IdentityTemporalColumns = "IdentityTemporalColumns";

		public const string NullableTemporalColumns = "NullableTemporalColumns";

		public const string InvalidAlwaysEncryptedPropertyValues = "InvalidAlwaysEncryptedPropertyValues";

		public const string NoMemoryOptimizedTemporalTables = "NoMemoryOptimizedTemporalTables";

		public const string SystemVersionedTableWithoutPeriod = "SystemVersionedTableWithoutPeriod";

		public const string HistoryTableWithoutSystemVersioning = "HistoryTableWithoutSystemVersioning";

		public const string NoTemporalFileTables = "NoTemporalFileTables";

		public const string NoAddingPeriodOnDroppedTable = "NoAddingPeriodOnDroppedTable";

		public const string CannotHaveMultiplePeriods = "CannotHaveMultiplePeriods";

		public const string NoDroppingPeriodOnDroppedTable = "NoDroppingPeriodOnDroppedTable";

		public const string NoDroppingPeriodOnNotYetCreatedTable = "NoDroppingPeriodOnNotYetCreatedTable";

		public const string CannotDropNonExistingPeriod = "CannotDropNonExistingPeriod";

		public const string MustProvideExistingColumn = "MustProvideExistingColumn";

		public const string PeriodMustHaveDifferentColumns = "PeriodMustHaveDifferentColumns";

		public const string PeriodStartColumnMustBeGeneratedAlways = "PeriodStartColumnMustBeGeneratedAlways";

		public const string PeriodEndColumnMustBeGeneratedAlways = "PeriodEndColumnMustBeGeneratedAlways";

		public const string PeriodInvalidDataType = "PeriodInvalidDataType";

		public const string InvalidPeriodColumnName = "InvalidPeriodColumnName";

		public const string BothHistoryTableNameAndSchemaMustBeProvided = "BothHistoryTableNameAndSchemaMustBeProvided";

		public const string NoHiddenColumnsOnNonGeneratedAlwaysColumns = "NoHiddenColumnsOnNonGeneratedAlwaysColumns";

		public const string InvalidHistoryRetentionPeriodSpecification = "InvalidHistoryRetentionPeriodSpecification";

		public const string InvalidHistoryRetentionPeriodUnitSpecification = "InvalidHistoryRetentionPeriodUnitSpecification";

		public const string NoDataMaskingOnNonTables = "NoDataMaskingOnNonTables";

		public const string InvalidMaskingFunctionFormat = "InvalidMaskingFunctionFormat";

		public const string MaskingFunctionOnWrongType = "MaskingFunctionOnWrongType";

		public const string NoDataMaskingOnColumnSet = "NoDataMaskingOnColumnSet";

		public const string NoDataMaskingOnTemporalColumns = "NoDataMaskingOnTemporalColumns";

		public const string NoDataMaskingOnComputedColumns = "NoDataMaskingOnComputedColumns";

		public const string NoDataMaskingOnFileStreamColumns = "NoDataMaskingOnFileStreamColumns";

		public const string NoDataMaskingOnEncryptedColumns = "NoDataMaskingOnEncryptedColumns";

		public const string MismatchingServerName = "MismatchingServerName";

		public const string MissingBackupDevices = "MissingBackupDevices";

		public const string MissingBackupDeviceType = "MissingBackupDeviceType";

		public const string MismatchingNumberOfMirrors = "MismatchingNumberOfMirrors";

		public const string BackupToPipesNotSupported = "BackupToPipesNotSupported";

		public const string PipeDeviceNotSupported = "PipeDeviceNotSupported";

		public const string BackupToUrlNotSupported = "BackupToUrlNotSupported";

		public const string CredentialNotSupportedError = "CredentialNotSupportedError";

		public const string BackupEncryptionNotSupported = "BackupEncryptionNotSupported";

		public const string LoginHasUser = "LoginHasUser";

		public const string LoginHasAlias = "LoginHasAlias";

		public const string InvalidLogin = "InvalidLogin";

		public const string UnknownShrinkType = "UnknownShrinkType";

		public const string InvalidShrinkMethod = "InvalidShrinkMethod";

		public const string CannotChangePrimary = "CannotChangePrimary";

		public const string OnlyOnePrimaryFile = "OnlyOnePrimaryFile";

		public const string MustSpecifyGrowth = "MustSpecifyGrowth";

		public const string WrongPercentageGrowth = "WrongPercentageGrowth";

		public const string WrongSize = "WrongSize";

		public const string InvalidSizeFileStream = "InvalidSizeFileStream";

		public const string InvalidMaxSizeFileStream = "InvalidMaxSizeFileStream";

		public const string InvalidGrowthFileStream = "InvalidGrowthFileStream";

		public const string PrimaryAlreadyDefault = "PrimaryAlreadyDefault";

		public const string ColumnNotVarbinaryMax = "ColumnNotVarbinaryMax";

		public const string NotClusteredIndex = "NotClusteredIndex";

		public const string NotNonClusteredIndex = "NotNonClusteredIndex";

		public const string UnsupportedFileGroupType = "UnsupportedFileGroupType";

		public const string ObjectRefsNonexCol = "ObjectRefsNonexCol";

		public const string OrderHintRefsNonexCol = "OrderHintRefsNonexCol";

		public const string CannotCopyPartition = "CannotCopyPartition";

		public const string PartitionNumberStartOutOfRange = "PartitionNumberStartOutOfRange";

		public const string VarDecimalAndDataCompressionConflict = "VarDecimalAndDataCompressionConflict";

		public const string RebuildHeapOnClusteredIndexError = "RebuildHeapOnClusteredIndexError";

		public const string RebuildHeapError = "RebuildHeapError";

		public const string PartitionSchemeNotAssignedError = "PartitionSchemeNotAssignedError";

		public const string CannotAddObject = "CannotAddObject";

		public const string UnknownObjectType = "UnknownObjectType";

		public const string TooFewFiles = "TooFewFiles";

		public const string UnknownProperty = "UnknownProperty";

		public const string ObjectNotUnderServer = "ObjectNotUnderServer";

		public const string UnknownChild = "UnknownChild";

		public const string UnknownChildCollection = "UnknownChildCollection";

		public const string CantCreateType = "CantCreateType";

		public const string ObjectAlreadyExists = "ObjectAlreadyExists";

		public const string NoSqlGen = "NoSqlGen";

		public const string InvalidType = "InvalidType";

		public const string WrongUrn = "WrongUrn";

		public const string NoDepForSysObjects = "NoDepForSysObjects";

		public const string UrnMissing = "UrnMissing";

		public const string InvalidGranteeList = "InvalidGranteeList";

		public const string DatabaseAlreadyExists = "DatabaseAlreadyExists";

		public const string ObjectWithNoChildren = "ObjectWithNoChildren";

		public const string ColumnBeforeNotExisting = "ColumnBeforeNotExisting";

		public const string PropNotModifiable = "PropNotModifiable";

		public const string UnsupportedLoginMode = "UnsupportedLoginMode";

		public const string CannotSetDefInitFlds = "CannotSetDefInitFlds";

		public const string ReasonTextIsEncrypted = "ReasonTextIsEncrypted";

		public const string ReasonPropertyIsNotSupportedOnCurrentServerVersion = "ReasonPropertyIsNotSupportedOnCurrentServerVersion";

		public const string ObjectWithMoreChildren = "ObjectWithMoreChildren";

		public const string WrongHybridIPAddresses = "WrongHybridIPAddresses";

		public const string WrongDHCPv6IPAddress = "WrongDHCPv6IPAddress";

		public const string WrongMultiDHCPIPAddresses = "WrongMultiDHCPIPAddresses";

		public const string GetDHCPAddressFailed = "GetDHCPAddressFailed";

		public const string CannotAddDHCPIPAddress = "CannotAddDHCPIPAddress";

		public const string SearchPropertyListNameNotValid = "SearchPropertyListNameNotValid";

		public const string SearchPropertyNameNotValid = "SearchPropertyNameNotValid";

		public const string SearchPropertyIntIDNotValid = "SearchPropertyIntIDNotValid";

		public const string SearchPropertySetGuidNotValid = "SearchPropertySetGuidNotValid";

		public const string SearchPropertyDescriptionNotValid = "SearchPropertyDescriptionNotValid";

		public const string SearchPropertyGuidIntIdNotValid = "SearchPropertyGuidIntIdNotValid";

		public const string NullOrEmptyParameterSourceDatabaseName = "NullOrEmptyParameterSourceDatabaseName";

		public const string NullOrEmptyParameterSourceSearchPropertyListName = "NullOrEmptyParameterSourceSearchPropertyListName";

		public const string EmptySourceSearchPropertyListName = "EmptySourceSearchPropertyListName";

		public const string SearchPropertyListNameAllWhiteSpaces = "SearchPropertyListNameAllWhiteSpaces";

		public const string EmptyRestorePlan = "EmptyRestorePlan";

		public const string MultipleDatabaseSelectedToRestore = "MultipleDatabaseSelectedToRestore";

		public const string ConflictWithNoRecovery = "ConflictWithNoRecovery";

		public const string BackupsOfDifferentDb = "BackupsOfDifferentDb";

		public const string FullBackupShouldBeFirst = "FullBackupShouldBeFirst";

		public const string BackupsNotInSequence = "BackupsNotInSequence";

		public const string NoFullBackupSelected = "NoFullBackupSelected";

		public const string WrongDiffbackup = "WrongDiffbackup";

		public const string DiffBackupNotCompatible = "DiffBackupNotCompatible";

		public const string WrongTLogbackup = "WrongTLogbackup";

		public const string TailLogBackupDeviceNull = "TailLogBackupDeviceNull";

		public const string OnlyLastRestoreWithNoRecovery = "OnlyLastRestoreWithNoRecovery";

		public const string UnsupportedServerVersion = "UnsupportedServerVersion";

		public const string BackupMediaSetNotComplete = "BackupMediaSetNotComplete";

		public const string BackupMediaSetEmpty = "BackupMediaSetEmpty";

		public const string GetMediaSetGuid = "GetMediaSetGuid";

		public const string MediaNotPartOfSet = "MediaNotPartOfSet";

		public const string UnableToReadDevice = "UnableToReadDevice";

		public const string FileGroupNotSupported = "FileGroupNotSupported";

		public const string InvalidDatabaseState = "InvalidDatabaseState";

		public const string PageRestoreOnlyForFullRecovery = "PageRestoreOnlyForFullRecovery";

		public const string BackupFileAlreadyExists = "BackupFileAlreadyExists";

		public const string CloseConnectionsFailed = "CloseConnectionsFailed";

		public const string BackupTailLog = "BackupTailLog";

		public const string Restoring = "Restoring";

		public const string BackupFileNotFound = "BackupFileNotFound";

		public const string TailLog = "TailLog";

		public const string UnableToCreateRestoreSequence = "UnableToCreateRestoreSequence";

		public const string UnableToCreatePageRestoreSequence = "UnableToCreatePageRestoreSequence";

		public const string UnableToCreatePlanTakeTLogBackup = "UnableToCreatePlanTakeTLogBackup";

		public const string OperationCancelledByUser = "OperationCancelledByUser";

		public const string CannotRestoreFileBootPage = "CannotRestoreFileBootPage";

		public const string CannotRestoreDatabaseBootPage = "CannotRestoreDatabaseBootPage";

		public const string DuplicateSuspectPage = "DuplicateSuspectPage";

		public const string InvalidSuspectpage = "InvalidSuspectpage";

		public const string InvalidPathChildCollectionNotFound = "InvalidPathChildCollectionNotFound";

		public const string InvalidPathChildSingletonNotFound = "InvalidPathChildSingletonNotFound";

		public const string SqlInnerException = "SqlInnerException";

		public const string InnerException = "InnerException";

		public const string InnerWmiException = "InnerWmiException";

		public const string UnknownError = "UnknownError";

		public const string WMIException = "WMIException";

		public const string CallingInitChildLevelWithWrongUrn = "CallingInitChildLevelWithWrongUrn";

		public const string CallingInitQueryUrnsWithWrongUrn = "CallingInitQueryUrnsWithWrongUrn";

		public const string UnsupportedObjectQueryUrn = "UnsupportedObjectQueryUrn";

		public const string UnsupportedBackupDeviceType = "UnsupportedBackupDeviceType";

		public const string UnableToRetrieveBackupHistory = "UnableToRetrieveBackupHistory";

		public const string UnsupportedVersion = "UnsupportedVersion";

		public const string UnsupportedCompatLevelException = "UnsupportedCompatLevelException";

		public const string ObjectDoesNotExist = "ObjectDoesNotExist";

		public const string UnsupportedFeature = "UnsupportedFeature";

		public const string OneFilePageSupported = "OneFilePageSupported";

		public const string NoObjectWithoutColumns = "NoObjectWithoutColumns";

		public const string ConflictingScriptingOptions = "ConflictingScriptingOptions";

		public const string InvalidScriptingOutput = "InvalidScriptingOutput";

		public const string ScriptDataNotSupportedByThisMethod = "ScriptDataNotSupportedByThisMethod";

		public const string ForeignKeyCycleInObjects = "ForeignKeyCycleInObjects";

		public const string NotSupportedForCloudVersion = "NotSupportedForCloudVersion";

		public const string NotSupportedForSqlDw = "NotSupportedForSqlDw";

		public const string NotSupportedForSqlDb = "NotSupportedForSqlDb";

		public const string NotSupportedOnStandalone = "NotSupportedOnStandalone";

		public const string NotSupportedOnStandaloneWithDetails = "NotSupportedOnStandaloneWithDetails";

		public const string NotSupportedOnCloud = "NotSupportedOnCloud";

		public const string NotSupportedOnCloudWithDetails = "NotSupportedOnCloudWithDetails";

		public const string InvalidScriptingOptions = "InvalidScriptingOptions";

		public const string ScriptingNotSupportedForSqlDw = "ScriptingNotSupportedForSqlDw";

		public const string ScriptingNotSupportedForSqlDb = "ScriptingNotSupportedForSqlDb";

		public const string ScriptingNotSupportedOnStandalone = "ScriptingNotSupportedOnStandalone";

		public const string ScriptingNotSupportedOnCloud = "ScriptingNotSupportedOnCloud";

		public const string SupportedOnlyBelow110 = "SupportedOnlyBelow110";

		public const string SupportedOnlyBelow100 = "SupportedOnlyBelow100";

		public const string SupportedOnlyBelow90 = "SupportedOnlyBelow90";

		public const string SupportedOnlyOn150 = "SupportedOnlyOn150";

		public const string SupportedOnlyOn140 = "SupportedOnlyOn140";

		public const string SupportedOnlyOn130 = "SupportedOnlyOn130";

		public const string SupportedOnlyOn120 = "SupportedOnlyOn120";

		public const string SupportedOnlyOn110 = "SupportedOnlyOn110";

		public const string SupportedOnlyOn105 = "SupportedOnlyOn105";

		public const string SupportedOnlyOn100 = "SupportedOnlyOn100";

		public const string SupportedOnlyOn90 = "SupportedOnlyOn90";

		public const string SupportedOnlyOn80SP3 = "SupportedOnlyOn80SP3";

		public const string SupportedOnlyOn80 = "SupportedOnlyOn80";

		public const string NotSupportedForVersionEarlierThan110 = "NotSupportedForVersionEarlierThan110";

		public const string NotSupportedForVersionEarlierThan120 = "NotSupportedForVersionEarlierThan120";

		public const string NotSupportedForVersionEarlierThan130 = "NotSupportedForVersionEarlierThan130";

		public const string NotSupportedForVersionEarlierThan140 = "NotSupportedForVersionEarlierThan140";

		public const string NotSupportedForVersionEarlierThan150 = "NotSupportedForVersionEarlierThan150";

		public const string PropertySupportedOnlyOn110SP1 = "PropertySupportedOnlyOn110SP1";

		public const string PropertySupportedOnlyOn110 = "PropertySupportedOnlyOn110";

		public const string PropertyNotSupportedForCloudVersion = "PropertyNotSupportedForCloudVersion";

		public const string PropertyNotSupportedOnStandalone = "PropertyNotSupportedOnStandalone";

		public const string PropertyNotSupportedOnCloud = "PropertyNotSupportedOnCloud";

		public const string PropertyNotSupportedWithDetails = "PropertyNotSupportedWithDetails";

		public const string PropertyValueSupportedOnlyOn110 = "PropertyValueSupportedOnlyOn110";

		public const string PropertyValueNotSupportedForSqlDw = "PropertyValueNotSupportedForSqlDw";

		public const string PropertyValueNotSupportedForSqlDb = "PropertyValueNotSupportedForSqlDb";

		public const string WrongPropertyValueException = "WrongPropertyValueException";

		public const string InvalidPropertyValueForVersion = "InvalidPropertyValueForVersion";

		public const string PropertyCannotBeSetForVersion = "PropertyCannotBeSetForVersion";

		public const string EmptyInputParam = "EmptyInputParam";

		public const string MustSpecifyOneParameter = "MustSpecifyOneParameter";

		public const string MutuallyExclusiveProperties = "MutuallyExclusiveProperties";

		public const string NeedPSParams = "NeedPSParams";

		public const string IndexMustBeClustered = "IndexMustBeClustered";

		public const string ParentMustExist = "ParentMustExist";

		public const string ViewCannotHaveKeys = "ViewCannotHaveKeys";

		public const string InvalidAcctName = "InvalidAcctName";

		public const string OperationOnlyInPendingState = "OperationOnlyInPendingState";

		public const string OperationNotInPendingState = "OperationNotInPendingState";

		public const string OperationNotInPendingState1 = "OperationNotInPendingState1";

		public const string OperationNotInPendingState2 = "OperationNotInPendingState2";

		public const string OperationNotInPendingState3 = "OperationNotInPendingState3";

		public const string RemoteServerEndpointRequired = "RemoteServerEndpointRequired";

		public const string DatabaseScopedCredentialsRequired = "DatabaseScopedCredentialsRequired";

		public const string UnsupportedDatabaseScopedConfiguration = "UnsupportedDatabaseScopedConfiguration";

		public const string MissingBoundingParameters = "MissingBoundingParameters";

		public const string InvalidNonGeometryParameters = "InvalidNonGeometryParameters";

		public const string UnsupportedNonSpatialParameters = "UnsupportedNonSpatialParameters";

		public const string NotSpatialIndexOnView = "NotSpatialIndexOnView";

		public const string OneColumnInSpatialIndex = "OneColumnInSpatialIndex";

		public const string NoSpatialIndexUnique = "NoSpatialIndexUnique";

		public const string NoSpatialIndexClustered = "NoSpatialIndexClustered";

		public const string LowPriorityCannotBeSetForDrop = "LowPriorityCannotBeSetForDrop";

		public const string SpatialAutoGridDownlevel = "SpatialAutoGridDownlevel";

		public const string NoAutoGridWithGrids = "NoAutoGridWithGrids";

		public const string OnlyHashIndexIsSupportedInAlter = "OnlyHashIndexIsSupportedInAlter";

		public const string NotXmlIndexOnView = "NotXmlIndexOnView";

		public const string OneColumnInXmlIndex = "OneColumnInXmlIndex";

		public const string BucketCountForHashIndex = "BucketCountForHashIndex";

		public const string ScriptMemoryOptimizedIndex = "ScriptMemoryOptimizedIndex";

		public const string HashIndexTableDependency = "HashIndexTableDependency";

		public const string TableMemoryOptimizedIndexDependency = "TableMemoryOptimizedIndexDependency";

		public const string TableSqlDwIndexRestrictions = "TableSqlDwIndexRestrictions";

		public const string TableSqlDwIndexTypeRestrictions = "TableSqlDwIndexTypeRestrictions";

		public const string UnexpectedIndexTypeDetected = "UnexpectedIndexTypeDetected";

		public const string NoXmlIndexUnique = "NoXmlIndexUnique";

		public const string NoXmlIndexClustered = "NoXmlIndexClustered";

		public const string UnsupportedNonXmlParameters = "UnsupportedNonXmlParameters";

		public const string CantSetDefaultFalse = "CantSetDefaultFalse";

		public const string TimeoutMustBePositive = "TimeoutMustBePositive";

		public const string NoPropertyChangeForDotNet = "NoPropertyChangeForDotNet";

		public const string ClrNotSupported = "ClrNotSupported";

		public const string InvalidPropertyNumberRange = "InvalidPropertyNumberRange";

		public const string ChangeTrackingException = "ChangeTrackingException";

		public const string MissingChangeTrackingParameters = "MissingChangeTrackingParameters";

		public const string TrackColumnsException = "TrackColumnsException";

		public const string InvalidCollation = "InvalidCollation";

		public const string UnsupportedCollation = "UnsupportedCollation";

		public const string ViewColumnsCannotBeModified = "ViewColumnsCannotBeModified";

		public const string NeedToPassObject = "NeedToPassObject";

		public const string InexistentDir = "InexistentDir";

		public const string CantCreateTempFile = "CantCreateTempFile";

		public const string UnknownFilter = "UnknownFilter";

		public const string SystemMessageReadOnly = "SystemMessageReadOnly";

		public const string MessageIDTooSmall = "MessageIDTooSmall";

		public const string DataTypeUnsupported = "DataTypeUnsupported";

		public const string CantSetTypeName = "CantSetTypeName";

		public const string CantSetTypeSchema = "CantSetTypeSchema";

		public const string UnknownSqlDataType = "UnknownSqlDataType";

		public const string NeedExistingObjForDataType = "NeedExistingObjForDataType";

		public const string NoPendingObjForDataType = "NoPendingObjForDataType";

		public const string CantSetContainedDatabaseCatalogCollation = "CantSetContainedDatabaseCatalogCollation";

		public const string ViewFragInfoNotInV7 = "ViewFragInfoNotInV7";

		public const string InvalidOptionForVersion = "InvalidOptionForVersion";

		public const string InvalidParamForVersion = "InvalidParamForVersion";

		public const string UnknownLanguageId = "UnknownLanguageId";

		public const string UnknownEnumeration = "UnknownEnumeration";

		public const string UnknownEnumerationWithValue = "UnknownEnumerationWithValue";

		public const string MissingConfigVariable = "MissingConfigVariable";

		public const string DtsNotInstalled = "DtsNotInstalled";

		public const string NSNotInstalled = "NSNotInstalled";

		public const string InvalidNSOperation = "InvalidNSOperation";

		public const string NSNotConnectedToServer = "NSNotConnectedToServer";

		public const string InvalidThreadPoolSize = "InvalidThreadPoolSize";

		public const string InvalidQuantumLimit = "InvalidQuantumLimit";

		public const string InvalidQuantumDuration = "InvalidQuantumDuration";

		public const string InvalidThrottle = "InvalidThrottle";

		public const string InvalidFailuresBeforeLoggingEvent = "InvalidFailuresBeforeLoggingEvent";

		public const string InvalidFailuresBeforeAbort = "InvalidFailuresBeforeAbort";

		public const string InvalidMulticastRecipientLimit = "InvalidMulticastRecipientLimit";

		public const string InvalidNotificationClassBatchSize = "InvalidNotificationClassBatchSize";

		public const string ThreadPoolSizeNotValidForEdition = "ThreadPoolSizeNotValidForEdition";

		public const string StartTimeGreaterThanADay = "StartTimeGreaterThanADay";

		public const string InvalidPropertySetForExistingObject = "InvalidPropertySetForExistingObject";

		public const string InvalidEncryptArgumentsAndArgumentKey = "InvalidEncryptArgumentsAndArgumentKey";

		public const string InvalidSerializerAdapterFound = "InvalidSerializerAdapterFound";

		public const string InvalidConversionError = "InvalidConversionError";

		public const string RequiredChildMissingFromParent = "RequiredChildMissingFromParent";

		public const string NoPermissions = "NoPermissions";

		public const string WrongParent = "WrongParent";

		public const string VerifyFailed = "VerifyFailed";

		public const string VerifyFailed0 = "VerifyFailed0";

		public const string PrimaryFgMustHaveFiles = "PrimaryFgMustHaveFiles";

		public const string NeedToSetParent = "NeedToSetParent";

		public const string InvalidVersion = "InvalidVersion";

		public const string ErrorInCreatingState = "ErrorInCreatingState";

		public const string ConflictingSwitches = "ConflictingSwitches";

		public const string BackupFailed = "BackupFailed";

		public const string RestoreFailed = "RestoreFailed";

		public const string BadCompatLevel = "BadCompatLevel";

		public const string SetPasswordError = "SetPasswordError";

		public const string PassPhraseAndIdentityNotSpecified = "PassPhraseAndIdentityNotSpecified";

		public const string PassPhraseNotSpecified = "PassPhraseNotSpecified";

		public const string NotEnoughRights = "NotEnoughRights";

		public const string SqlDwCreateRequiredParameterMissing = "SqlDwCreateRequiredParameterMissing";

		public const string CannotAlterKeyWithProvider = "CannotAlterKeyWithProvider";

		public const string InvalidAlgorithm = "InvalidAlgorithm";

		public const string AlterQueryStorePropertyForRestoringDatabase = "AlterQueryStorePropertyForRestoringDatabase";

		public const string IncludeHeader = "IncludeHeader";

		public const string NoUrnSuffix = "NoUrnSuffix";

		public const string FullPropertyBag = "FullPropertyBag";

		public const string MultipleRowsForUrn = "MultipleRowsForUrn";

		public const string GetParentFailed = "GetParentFailed";

		public const string ParentNull = "ParentNull";

		public const string CouldNotFindKey = "CouldNotFindKey";

		public const string UnsupportedUrnFilter = "UnsupportedUrnFilter";

		public const string UnsupportedUrnAttrib = "UnsupportedUrnAttrib";

		public const string InvalidFileInformationData = "InvalidFileInformationData";

		public const string MappingObjectIdMissing = "MappingObjectIdMissing";

		public const string EmptyMapping = "EmptyMapping";

		public const string ExecutionContextPrincipalIsNotSpecified = "ExecutionContextPrincipalIsNotSpecified";

		public const string MissingBrokerEndpoint = "MissingBrokerEndpoint";

		public const string CannotEnableSoapEndpoints = "CannotEnableSoapEndpoints";

		public const string ServiceError0 = "ServiceError0";

		public const string ServiceError1 = "ServiceError1";

		public const string ServiceError2 = "ServiceError2";

		public const string ServiceError3 = "ServiceError3";

		public const string ServiceError4 = "ServiceError4";

		public const string ServiceError5 = "ServiceError5";

		public const string ServiceError6 = "ServiceError6";

		public const string ServiceError7 = "ServiceError7";

		public const string ServiceError8 = "ServiceError8";

		public const string ServiceError9 = "ServiceError9";

		public const string ServiceError10 = "ServiceError10";

		public const string ServiceError11 = "ServiceError11";

		public const string ServiceError12 = "ServiceError12";

		public const string ServiceError13 = "ServiceError13";

		public const string ServiceError14 = "ServiceError14";

		public const string ServiceError15 = "ServiceError15";

		public const string ServiceError16 = "ServiceError16";

		public const string ServiceError17 = "ServiceError17";

		public const string ServiceError18 = "ServiceError18";

		public const string ServiceError19 = "ServiceError19";

		public const string ServiceError20 = "ServiceError20";

		public const string ServiceError21 = "ServiceError21";

		public const string ServiceError22 = "ServiceError22";

		public const string ServiceError23 = "ServiceError23";

		public const string ServiceError24 = "ServiceError24";

		public const string UnknownCategoryName = "UnknownCategoryName";

		public const string UnknownCategoryType = "UnknownCategoryType";

		public const string UnknownOperator = "UnknownOperator";

		public const string UnsupportedFeatureSqlAgent = "UnsupportedFeatureSqlAgent";

		public const string UnsupportedFeatureSqlMail = "UnsupportedFeatureSqlMail";

		public const string UnsupportedFeatureServiceBroker = "UnsupportedFeatureServiceBroker";

		public const string UnsupportedFeatureFullText = "UnsupportedFeatureFullText";

		public const string UnsupportedFeatureResourceGovernor = "UnsupportedFeatureResourceGovernor";

		public const string UnsupportedFeatureSmartAdmin = "UnsupportedFeatureSmartAdmin";

		public const string InvalidServerUrn = "InvalidServerUrn";

		public const string InvalidUrn = "InvalidUrn";

		public const string WMIProviderNotInstalled = "WMIProviderNotInstalled";

		public const string PropertyCannotBeChangedAfterConnection = "PropertyCannotBeChangedAfterConnection";

		public const string CouldNotFindManagementObject = "CouldNotFindManagementObject";

		public const string CannotSubscribe = "CannotSubscribe";

		public const string CannotStartSubscription = "CannotStartSubscription";

		public const string NotSupportedNotification = "NotSupportedNotification";

		public const string InnerRegSvrException = "InnerRegSvrException";

		public const string SqlServerTypeName = "SqlServerTypeName";

		public const string MissingObjectExceptionText = "MissingObjectExceptionText";

		public const string PropertyNotSetExceptionText = "PropertyNotSetExceptionText";

		public const string MissingObjectNameExceptionText = "MissingObjectNameExceptionText";

		public const string WrongPropertyValueExceptionText = "WrongPropertyValueExceptionText";

		public const string PropertyTypeMismatchExceptionText = "PropertyTypeMismatchExceptionText";

		public const string MissingPropertyExceptionText = "MissingPropertyExceptionText";

		public const string UnknownPropertyExceptionText = "UnknownPropertyExceptionText";

		public const string PropertyReadOnlyExceptionText = "PropertyReadOnlyExceptionText";

		public const string InvalidSmoOperationExceptionText = "InvalidSmoOperationExceptionText";

		public const string PropertyCannotBeRetrievedExceptionText = "PropertyCannotBeRetrievedExceptionText";

		public const string ObjectDroppedExceptionText = "ObjectDroppedExceptionText";

		public const string UnsupportedObjectNameExceptionText = "UnsupportedObjectNameExceptionText";

		public const string FailedtoInitialize = "FailedtoInitialize";

		public const string PropertyMustBeSpecifiedInUrn = "PropertyMustBeSpecifiedInUrn";

		public const string InvalidScanType = "InvalidScanType";

		public const string ColumnsMustBeSpecified = "ColumnsMustBeSpecified";

		public const string PasswdModiOnlyForStandardLogin = "PasswdModiOnlyForStandardLogin";

		public const string DenyLoginModiNotForStandardLogin = "DenyLoginModiNotForStandardLogin";

		public const string CloudSidNotApplicableOnStandalone = "CloudSidNotApplicableOnStandalone";

		public const string CannotCreateExtendedPropertyWithoutSchema = "CannotCreateExtendedPropertyWithoutSchema";

		public const string InvalidSchema = "InvalidSchema";

		public const string FailedToChangeSchema = "FailedToChangeSchema";

		public const string InvalidUrnServerLevel = "InvalidUrnServerLevel";

		public const string ServerLevelMustBePresent = "ServerLevelMustBePresent";

		public const string TempTablesNotSupported = "TempTablesNotSupported";

		public const string PlanGuideNameCannotStartWithHash = "PlanGuideNameCannotStartWithHash";

		public const string PropertiesNotValidException = "PropertiesNotValidException";

		public const string TypeSchemaMustBeDbo = "TypeSchemaMustBeDbo";

		public const string OperationInProgress = "OperationInProgress";

		public const string UnsupportedPermission = "UnsupportedPermission";

		public const string InvalidVersionSmoOperation = "InvalidVersionSmoOperation";

		public const string ContainmentNotSupported = "ContainmentNotSupported";

		public const string AweEnabledNotSupported = "AweEnabledNotSupported";

		public const string PasswordError = "PasswordError";

		public const string MediaPasswordError = "MediaPasswordError";

		public const string CannotChangePassword = "CannotChangePassword";

		public const string CannotChangePasswordForUser = "CannotChangePasswordForUser";

		public const string PasswordOnlyForDatabaseAuthenticatedNonWindowsUser = "PasswordOnlyForDatabaseAuthenticatedNonWindowsUser";

		public const string DefaultLanguageOnlyForDatabaseAuthenticatedUser = "DefaultLanguageOnlyForDatabaseAuthenticatedUser";

		public const string CannotCopyPasswordToUser = "CannotCopyPasswordToUser";

		public const string FollowingPropertiesCanBeSetOnlyWithContainmentEnabled = "FollowingPropertiesCanBeSetOnlyWithContainmentEnabled";

		public const string OperationNotSupportedWhenPartOfAUDF = "OperationNotSupportedWhenPartOfAUDF";

		public const string FailedToWriteProperty = "FailedToWriteProperty";

		public const string ReasonIntextMode = "ReasonIntextMode";

		public const string ReasonNotIntextMode = "ReasonNotIntextMode";

		public const string SyntaxErrorInTextHeader = "SyntaxErrorInTextHeader";

		public const string IncorrectTextHeader = "IncorrectTextHeader";

		public const string ScriptHeaderTypeNotSupported = "ScriptHeaderTypeNotSupported";

		public const string CollectionCannotBeModified = "CollectionCannotBeModified";

		public const string KeyOptionsIncorrect = "KeyOptionsIncorrect";

		public const string PropertyIsInvalidInUrn = "PropertyIsInvalidInUrn";

		public const string TableOrViewParentForUpdateStatistics = "TableOrViewParentForUpdateStatistics";

		public const string ReasonObjectAlreadyCreated = "ReasonObjectAlreadyCreated";

		public const string PropertyAvailable = "PropertyAvailable";

		public const string CannotReadProperty = "CannotReadProperty";

		public const string CannotWriteProperty = "CannotWriteProperty";

		public const string CannotAccessProperty = "CannotAccessProperty";

		public const string PropertyNotAvailableToWrite = "PropertyNotAvailableToWrite";

		public const string ServerPropertyMustBeSetForOlap = "ServerPropertyMustBeSetForOlap";

		public const string InvalidInstanceName = "InvalidInstanceName";

		public const string TransferCtorErr = "TransferCtorErr";

		public const string UnsupportedVersionException = "UnsupportedVersionException";

		public const string UnsupportedEngineTypeException = "UnsupportedEngineTypeException";

		public const string UnsupportedEngineEditionException = "UnsupportedEngineEditionException";

		public const string CantScriptObject = "CantScriptObject";

		public const string PropertyNotValidException = "PropertyNotValidException";

		public const string TargetRecoveryTimeNotNegative = "TargetRecoveryTimeNotNegative";

		public const string CannotEnableUDTTTrigger = "CannotEnableUDTTTrigger";

		public const string NotIndexOnUDTT = "NotIndexOnUDTT";

		public const string NotXmlIndexOnUDTT = "NotXmlIndexOnUDTT";

		public const string NotFullTextIndexOnUDTT = "NotFullTextIndexOnUDTT";

		public const string UDTTColumnsCannotBeModified = "UDTTColumnsCannotBeModified";

		public const string UDTTCannotBeModified = "UDTTCannotBeModified";

		public const string UDTTIndexCannotBeModified = "UDTTIndexCannotBeModified";

		public const string NotFragInfoOnUDTT = "NotFragInfoOnUDTT";

		public const string NotStatisticsOnUDTT = "NotStatisticsOnUDTT";

		public const string NotTriggersOnUDTT = "NotTriggersOnUDTT";

		public const string NotCheckIndexOnUDTT = "NotCheckIndexOnUDTT";

		public const string UDTTChecksCannotBeModified = "UDTTChecksCannotBeModified";

		public const string OperationNotSupportedWhenPartOfUDTT = "OperationNotSupportedWhenPartOfUDTT";

		public const string AddAuditSpecificationDetail = "AddAuditSpecificationDetail";

		public const string RemoveAuditSpecificationDetail = "RemoveAuditSpecificationDetail";

		public const string EnterAuditName = "EnterAuditName";

		public const string EnterOwner = "EnterOwner";

		public const string EnterFilePath = "EnterFilePath";

		public const string EnterName = "EnterName";

		public const string EnterServerAudit = "EnterServerAudit";

		public const string EnterStoragePath = "EnterStoragePath";

		public const string ReserveDiskSpaceNotAllowedWhenMaxFileSizeIsUnlimited = "ReserveDiskSpaceNotAllowedWhenMaxFileSizeIsUnlimited";

		public const string EnterServerCertificate = "EnterServerCertificate";

		public const string EnterServerAsymmetricKey = "EnterServerAsymmetricKey";

		public const string CertificateNotBackedUp = "CertificateNotBackedUp";

		public const string FailedOperationExceptionText = "FailedOperationExceptionText";

		public const string FailedOperationExceptionText2 = "FailedOperationExceptionText2";

		public const string FailedOperationExceptionText3 = "FailedOperationExceptionText3";

		public const string FailedOperationExceptionTextColl = "FailedOperationExceptionTextColl";

		public const string FailedOperationExceptionTextScript = "FailedOperationExceptionTextScript";

		public const string FailedOperationMessageNotSupportedTempdb = "FailedOperationMessageNotSupportedTempdb";

		public const string InvalidOperationInDisconnectedMode = "InvalidOperationInDisconnectedMode";

		public const string CannotCreateAvailabilityGroupWithoutCurrentIntance = "CannotCreateAvailabilityGroupWithoutCurrentIntance";

		public const string JoinAvailabilityGroupFailed = "JoinAvailabilityGroupFailed";

		public const string ForceFailoverFailed = "ForceFailoverFailed";

		public const string ManualFailoverFailed = "ManualFailoverFailed";

		public const string DatabaseJoinAvailabilityGroupFailed = "DatabaseJoinAvailabilityGroupFailed";

		public const string DatabaseJoinAvailabilityGroupInvalidGroupName = "DatabaseJoinAvailabilityGroupInvalidGroupName";

		public const string DatabaseLeaveAvailabilityGroupFailed = "DatabaseLeaveAvailabilityGroupFailed";

		public const string SuspendDataMovementFailed = "SuspendDataMovementFailed";

		public const string ResumeDataMovementFailed = "ResumeDataMovementFailed";

		public const string EnumClusterMemberState = "EnumClusterMemberState";

		public const string EnumReplicaClusterNodes = "EnumReplicaClusterNodes";

		public const string EnumClusterSubnets = "EnumClusterSubnets";

		public const string RestartListenerFailed = "RestartListenerFailed";

		public const string PropertyCannotBeRetrievedFromSecondary = "PropertyCannotBeRetrievedFromSecondary";

		public const string GrantAGCreateDatabasePrivilegeFailed = "GrantAGCreateDatabasePrivilegeFailed";

		public const string RevokeAGCreateDatabasePrivilegeFailed = "RevokeAGCreateDatabasePrivilegeFailed";

		public const string ReadOnlyRoutingListContainsEmptyReplicaName = "ReadOnlyRoutingListContainsEmptyReplicaName";

		public const string CannotUpdateBothReadOnlyRoutingLists = "CannotUpdateBothReadOnlyRoutingLists";

		public const string SetAccount = "SetAccount";

		public const string Create = "Create";

		public const string Alter = "Alter";

		public const string CreateOrAlter = "CreateOrAlter";

		public const string Drop = "Drop";

		public const string Rename = "Rename";

		public const string Script = "Script";

		public const string Grant = "Grant";

		public const string Revoke = "Revoke";

		public const string Deny = "Deny";

		public const string GrantWithGrant = "GrantWithGrant";

		public const string Bind = "Bind";

		public const string Unbind = "Unbind";

		public const string AddDefaultConstraint = "AddDefaultConstraint";

		public const string TestMailProfile = "TestMailProfile";

		public const string TestNetSend = "TestNetSend";

		public const string SetState = "SetState";

		public const string DropAndMove = "DropAndMove";

		public const string GetSmoObject = "GetSmoObject";

		public const string AdvancedProperties = "AdvancedProperties";

		public const string AddCollection = "AddCollection";

		public const string RemoveCollection = "RemoveCollection";

		public const string This = "This";

		public const string Abort = "Abort";

		public const string AddMember = "AddMember";

		public const string DropMember = "DropMember";

		public const string EnumMembers = "EnumMembers";

		public const string EnumPermissions = "EnumPermissions";

		public const string EnumContainingRoles = "EnumContainingRoles";

		public const string EnumAgentProxyAccounts = "EnumAgentProxyAccounts";

		public const string AddMemberServer = "AddMemberServer";

		public const string EnumMemberServers = "EnumMemberServers";

		public const string RemoveMemberServer = "RemoveMemberServer";

		public const string ResetOccurrenceCount = "ResetOccurrenceCount";

		public const string AddNotification = "AddNotification";

		public const string RemoveNotification = "RemoveNotification";

		public const string UpdateNotification = "UpdateNotification";

		public const string EnumNotifications = "EnumNotifications";

		public const string ApplyToTargetServer = "ApplyToTargetServer";

		public const string ApplyToTargetServerGroup = "ApplyToTargetServerGroup";

		public const string EnumAlerts = "EnumAlerts";

		public const string EnumHistory = "EnumHistory";

		public const string EnumJobStepOutputLogs = "EnumJobStepOutputLogs";

		public const string EnumTargetServers = "EnumTargetServers";

		public const string EnumJobSteps = "EnumJobSteps";

		public const string Invoke = "Invoke";

		public const string PurgeHistory = "PurgeHistory";

		public const string AddSharedSchedule = "AddSharedSchedule";

		public const string RemoveSharedSchedule = "RemoveSharedSchedule";

		public const string RemoveAllJobSchedules = "RemoveAllJobSchedules";

		public const string RemoveAllJobSteps = "RemoveAllJobSteps";

		public const string RemoveFromTargetServer = "RemoveFromTargetServer";

		public const string RemoveFromTargetServerGroup = "RemoveFromTargetServerGroup";

		public const string Start = "Start";

		public const string Stop = "Stop";

		public const string SetPassword = "SetPassword";

		public const string ChangePassword = "ChangePassword";

		public const string MakeContained = "MakeContained";

		public const string EnumDatabaseMappings = "EnumDatabaseMappings";

		public const string GetDatabaseUser = "GetDatabaseUser";

		public const string AddToRole = "AddToRole";

		public const string AttachDatabase = "AttachDatabase";

		public const string DetachDatabase = "DetachDatabase";

		public const string EnumCollations = "EnumCollations";

		public const string EnumPerformanceCounters = "EnumPerformanceCounters";

		public const string EnumErrorLogs = "EnumErrorLogs";

		public const string EnumDatabaseMirrorWitnessRoles = "EnumDatabaseMirrorWitnessRoles";

		public const string ReadErrorLog = "ReadErrorLog";

		public const string KillDatabase = "KillDatabase";

		public const string KillProcess = "KillProcess";

		public const string GetActiveDBConnectionCount = "GetActiveDBConnectionCount";

		public const string DropAllActiveDBConnections = "DropAllActiveDBConnections";

		public const string EnumDirectories = "EnumDirectories";

		public const string EnumLocks = "EnumLocks";

		public const string AddPrivateKey = "AddPrivateKey";

		public const string ExportCertificate = "ExportCertificate";

		public const string ChangePrivateKeyPassword = "ChangePrivateKeyPassword";

		public const string RemovePrivateKey = "RemovePrivateKey";

		public const string AddKeyEncryption = "AddKeyEncryption";

		public const string DropKeyEncryption = "DropKeyEncryption";

		public const string SymmetricKeyOpen = "SymmetricKeyOpen";

		public const string SymmetricKeyClose = "SymmetricKeyClose";

		public const string EnumLogins = "EnumLogins";

		public const string EnumWindowsDomainGroups = "EnumWindowsDomainGroups";

		public const string EnumProcesses = "EnumProcesses";

		public const string EnumStartupProcedures = "EnumStartupProcedures";

		public const string EnumWindowsUserInfo = "EnumWindowsUserInfo";

		public const string EnumWindowsGroupInfo = "EnumWindowsGroupInfo";

		public const string EnumAvailableMedia = "EnumAvailableMedia";

		public const string EnumServerAttributes = "EnumServerAttributes";

		public const string DeleteBackupHistory = "DeleteBackupHistory";

		public const string Refresh = "Refresh";

		public const string EnumBoundColumns = "EnumBoundColumns";

		public const string EnumBoundDataTypes = "EnumBoundDataTypes";

		public const string CheckAllocations = "CheckAllocations";

		public const string CheckCatalog = "CheckCatalog";

		public const string CheckIdentityValues = "CheckIdentityValues";

		public const string CheckTables = "CheckTables";

		public const string CheckTable = "CheckTable";

		public const string Shrink = "Shrink";

		public const string RecalculateSpaceUsage = "RecalculateSpaceUsage";

		public const string PrefetchObjects = "PrefetchObjects";

		public const string EnumTransactions = "EnumTransactions";

		public const string GetTransactionCount = "GetTransactionCount";

		public const string ImportXmlSchema = "ImportXmlSchema";

		public const string ExtendXmlSchema = "ExtendXmlSchema";

		public const string RemoveFullTextCatalogs = "RemoveFullTextCatalogs";

		public const string SetDefaultFullTextCatalog = "SetDefaultFullTextCatalog";

		public const string SetDefaultFileGroup = "SetDefaultFileGroup";

		public const string SetDefaultFileStreamFileGroup = "SetDefaultFileStreamFileGroup";

		public const string CheckFileGroup = "CheckFileGroup";

		public const string CheckIndex = "CheckIndex";

		public const string Checkpoint = "Checkpoint";

		public const string Cleanup = "Cleanup";

		public const string CannotReadProp = "CannotReadProp";

		public const string UpdateLanguageResources = "UpdateLanguageResources";

		public const string CatalogUpgradeOptions = "CatalogUpgradeOptions";

		public const string EnumLanguages = "EnumLanguages";

		public const string EnumSemanticLanguages = "EnumSemanticLanguages";

		public const string ClearHostLoginAccount = "ClearHostLoginAccount";

		public const string SetProxyAccount = "SetProxyAccount";

		public const string ClearProxyAccount = "ClearProxyAccount";

		public const string SetMsxAccount = "SetMsxAccount";

		public const string ClearMsxAccount = "ClearMsxAccount";

		public const string CycleErrorLog = "CycleErrorLog";

		public const string EnumJobHistory = "EnumJobHistory";

		public const string EnumJobs = "EnumJobs";

		public const string MoreThanOneProxyAccountIsNotSupported = "MoreThanOneProxyAccountIsNotSupported";

		public const string AddSubSystems = "AddSubSystems";

		public const string DeleteJobStepLogs = "DeleteJobStepLogs";

		public const string RemoveSubSystems = "RemoveSubSystems";

		public const string EnumSubSystems = "EnumSubSystems";

		public const string AddMailAccountToProfile = "AddMailAccountToProfile";

		public const string RemoveMailAccountFromProfile = "RemoveMailAccountFromProfile";

		public const string EnumMailAccountsForProfile = "EnumMailAccountsForProfile";

		public const string AddPrincipalToMailProfile = "AddPrincipalToMailProfile";

		public const string RemovePrincipalFromMailProfile = "RemovePrincipalFromMailProfile";

		public const string EnumPrincipalsForMailProfile = "EnumPrincipalsForMailProfile";

		public const string AddLoginToProxyAccount = "AddLoginToProxyAccount";

		public const string RemoveLoginFromProxyAccount = "RemoveLoginFromProxyAccount";

		public const string EnumLoginsOfProxyAccount = "EnumLoginsOfProxyAccount";

		public const string AddServerRoleToProxyAccount = "AddServerRoleToProxyAccount";

		public const string RemoveServerRoleFromProxyAccount = "RemoveServerRoleFromProxyAccount";

		public const string EnumServerRolesOfProxyAccount = "EnumServerRolesOfProxyAccount";

		public const string AddMSDBRoleToProxyAccount = "AddMSDBRoleToProxyAccount";

		public const string RemoveMSDBRoleFromProxyAccount = "RemoveMSDBRoleFromProxyAccount";

		public const string EnumMSDBRolesOfProxyAccount = "EnumMSDBRolesOfProxyAccount";

		public const string MsxDefect = "MsxDefect";

		public const string MsxEnlist = "MsxEnlist";

		public const string PurgeJobHistory = "PurgeJobHistory";

		public const string ReassignJobsByLogin = "ReassignJobsByLogin";

		public const string CreateRestorePlan = "CreateRestorePlan";

		public const string DropJobsByLogin = "DropJobsByLogin";

		public const string StartMonitor = "StartMonitor";

		public const string StopMonitor = "StopMonitor";

		public const string EnumProxies = "EnumProxies";

		public const string DropJobsByServer = "DropJobsByServer";

		public const string CompareUrn = "CompareUrn";

		public const string Disable = "Disable";

		public const string DisableAllIndexes = "DisableAllIndexes";

		public const string EnableAllIndexes = "EnableAllIndexes";

		public const string DiscoverDependencies = "DiscoverDependencies";

		public const string DropBackupHistory = "DropBackupHistory";

		public const string ChangeMirroringState = "ChangeMirroringState";

		public const string IsMember = "IsMember";

		public const string Recreate = "Recreate";

		public const string Enable = "Enable";

		public const string EnumColumns = "EnumColumns";

		public const string EnumForeignKeys = "EnumForeignKeys";

		public const string EnumIndexes = "EnumIndexes";

		public const string EnumFragmentation = "EnumFragmentation";

		public const string EnumReferences = "EnumReferences";

		public const string SetOffline = "SetOffline";

		public const string SetEncryption = "SetEncryption";

		public const string SetOwner = "SetOwner";

		public const string StartPopulation = "StartPopulation";

		public const string StopPopulation = "StopPopulation";

		public const string Rebuild = "Rebuild";

		public const string Reorganize = "Reorganize";

		public const string UpdateStatistics = "UpdateStatistics";

		public const string SetHostLoginAccount = "SetHostLoginAccount";

		public const string SetMailServerAccount = "SetMailServerAccount";

		public const string SetMailServerPassword = "SetMailServerPassword";

		public const string EnumLastStatisticsUpdates = "EnumLastStatisticsUpdates";

		public const string RebuildIndexes = "RebuildIndexes";

		public const string ResumeIndexes = "ResumeIndexes";

		public const string AbortIndexes = "AbortIndexes";

		public const string PauseIndexes = "PauseIndexes";

		public const string ReCompileReferences = "ReCompileReferences";

		public const string TruncateData = "TruncateData";

		public const string TruncateLog = "TruncateLog";

		public const string TruncatePartitionsNotSupported = "TruncatePartitionsNotSupported";

		public const string SwitchPartition = "SwitchPartition";

		public const string MergeHashPartition = "MergeHashPartition";

		public const string MergeRangePartition = "MergeRangePartition";

		public const string SplitHashPartition = "SplitHashPartition";

		public const string SplitRangePartition = "SplitRangePartition";

		public const string GetRangeValues = "GetRangeValues";

		public const string ResetNextUsed = "ResetNextUsed";

		public const string GetFileGroups = "GetFileGroups";

		public const string GetDefaultInitFields = "GetDefaultInitFields";

		public const string SetDefaultInitFields = "SetDefaultInitFields";

		public const string GetPropertyNames = "GetPropertyNames";

		public const string SetParent = "SetParent";

		public const string InitObject = "InitObject";

		public const string SetName = "SetName";

		public const string SetNamespace = "SetNamespace";

		public const string SetSchema = "SetSchema";

		public const string ExecuteNonQuery = "ExecuteNonQuery";

		public const string SetSnapshotIsolation = "SetSnapshotIsolation";

		public const string AlterDatabaseScopedConfiguration = "AlterDatabaseScopedConfiguration";

		public const string EnumNamespaces = "EnumNamespaces";

		public const string EnumTypes = "EnumTypes";

		public const string AddSchemaDocument = "AddSchemaDocument";

		public const string ScriptTransfer = "ScriptTransfer";

		public const string SetIdentityPhrase = "SetIdentityPhrase";

		public const string SetEncryptionOptions = "SetEncryptionOptions";

		public const string EnumStatistics = "EnumStatistics";

		public const string GetJobByID = "GetJobByID";

		public const string RemoveJobByID = "RemoveJobByID";

		public const string RemoveJobsByLogin = "RemoveJobsByLogin";

		public const string EnumCandidateKeys = "EnumCandidateKeys";

		public const string ExecuteWithResults = "ExecuteWithResults";

		public const string UpdateIndexStatistics = "UpdateIndexStatistics";

		public const string EnumMatchingSPs = "EnumMatchingSPs";

		public const string EnumObjects = "EnumObjects";

		public const string ReadBackupHeader = "ReadBackupHeader";

		public const string ReadMediaHeader = "ReadMediaHeader";

		public const string DetachedDatabaseInfo = "DetachedDatabaseInfo";

		public const string IsDetachedPrimaryFile = "IsDetachedPrimaryFile";

		public const string IsWindowsGroupMember = "IsWindowsGroupMember";

		public const string EnumDetachedDatabaseFiles = "EnumDetachedDatabaseFiles";

		public const string EnumDetachedLogFiles = "EnumDetachedLogFiles";

		public const string ServerEnumMembers = "ServerEnumMembers";

		public const string Contains = "Contains";

		public const string PingSqlServerVersion = "PingSqlServerVersion";

		public const string SetServiceAccount = "SetServiceAccount";

		public const string ChangeServicePassword = "ChangeServicePassword";

		public const string ServerActiveDirectoryRegister = "ServerActiveDirectoryRegister";

		public const string ServerActiveDirectoryUpdateRegistration = "ServerActiveDirectoryUpdateRegistration";

		public const string ServerActiveDirectoryUnregister = "ServerActiveDirectoryUnregister";

		public const string DatabaseActiveDirectoryRegister = "DatabaseActiveDirectoryRegister";

		public const string DatabaseActiveDirectoryUpdateRegistration = "DatabaseActiveDirectoryUpdateRegistration";

		public const string DatabaseActiveDirectoryUnregister = "DatabaseActiveDirectoryUnregister";

		public const string RecoverMasterKey = "RecoverMasterKey";

		public const string RegenerateMasterKey = "RegenerateMasterKey";

		public const string ImportMasterKey = "ImportMasterKey";

		public const string ExportMasterKey = "ExportMasterKey";

		public const string ChangeAcctMasterKey = "ChangeAcctMasterKey";

		public const string AddEncryptionMasterKey = "AddEncryptionMasterKey";

		public const string DropEncryptionMasterKey = "DropEncryptionMasterKey";

		public const string Close = "Close";

		public const string Open = "Open";

		public const string EnumKeyEncryptions = "EnumKeyEncryptions";

		public const string Compare = "Compare";

		public const string Insert = "Insert";

		public const string AddRange = "AddRange";

		public const string SetRange = "SetRange";

		public const string AddDevice = "AddDevice";

		public const string SetMirrors = "SetMirrors";

		public const string SetDatabase = "SetDatabase";

		public const string SqlManagement = "SqlManagement";

		public const string EnumEncryptionAlgorithms = "EnumEncryptionAlgorithms";

		public const string EnumProviderKeys = "EnumProviderKeys";

		public const string SetIpAddress = "SetIpAddress";

		public const string SetSubnetIp = "SetSubnetIp";

		public const string SetSubnetMask = "SetSubnetMask";

		public const string GetDHCPAddress = "GetDHCPAddress";

		public const string GetQueryStoreOptions = "GetQueryStoreOptions";

		public const string SetQueryStoreOptions = "SetQueryStoreOptions";

		public const string InvalidQueryStoreOptions = "InvalidQueryStoreOptions";

		public const string ReauthorizeRemoteDataArchive = "ReauthorizeRemoteDataArchive";

		public const string GetRemoteDataArchiveMigrationStatusReports = "GetRemoteDataArchiveMigrationStatusReports";

		public const string GetRemoteDatabaseMigrationStatistics = "GetRemoteDatabaseMigrationStatistics";

		public const string GetRemoteTableMigrationStatistics = "GetRemoteTableMigrationStatistics";

		public const string Table = "Table";

		public const string FileTable = "FileTable";

		public const string View = "View";

		public const string Server = "Server";

		public const string Database = "Database";

		public const string ExtendedProperty = "ExtendedProperty";

		public const string DatabaseOptions = "DatabaseOptions";

		public const string Synonym = "Synonym";

		public const string Sequence = "Sequence";

		public const string FullTextIndex = "FullTextIndex";

		public const string FullTextIndexColumn = "FullTextIndexColumn";

		public const string Check = "Check";

		public const string ForeignKey = "ForeignKey";

		public const string ForeignKeyColumn = "ForeignKeyColumn";

		public const string PartitionSchemeParameter = "PartitionSchemeParameter";

		public const string PlanGuide = "PlanGuide";

		public const string Trigger = "Trigger";

		public const string Index = "Index";

		public const string BrokerPriority = "BrokerPriority";

		public const string IndexedColumn = "IndexedColumn";

		public const string Statistic = "Statistic";

		public const string StatisticColumn = "StatisticColumn";

		public const string Column = "Column";

		public const string DefaultConstraint = "DefaultConstraint";

		public const string StoredProcedure = "StoredProcedure";

		public const string StoredProcedureParameter = "StoredProcedureParameter";

		public const string SqlAssembly = "SqlAssembly";

		public const string SqlAssemblyFile = "SqlAssemblyFile";

		public const string UserDefinedType = "UserDefinedType";

		public const string UserDefinedAggregate = "UserDefinedAggregate";

		public const string UserDefinedAggregateParameter = "UserDefinedAggregateParameter";

		public const string FullTextCatalog = "FullTextCatalog";

		public const string FullTextStopList = "FullTextStopList";

		public const string SearchPropertyList = "SearchPropertyList";

		public const string ExtendedStoredProcedure = "ExtendedStoredProcedure";

		public const string UserDefinedFunction = "UserDefinedFunction";

		public const string UserDefinedFunctionParameter = "UserDefinedFunctionParameter";

		public const string User = "User";

		public const string Schema = "Schema";

		public const string DatabaseRole = "DatabaseRole";

		public const string ApplicationRole = "ApplicationRole";

		public const string LogFile = "LogFile";

		public const string FileGroup = "FileGroup";

		public const string DataFile = "DataFile";

		public const string Default = "Default";

		public const string Rule = "Rule";

		public const string UserDefinedDataType = "UserDefinedDataType";

		public const string UserDefinedTableType = "UserDefinedTableType";

		public const string PartitionFunction = "PartitionFunction";

		public const string PartitionScheme = "PartitionScheme";

		public const string DatabaseActiveDirectory = "DatabaseActiveDirectory";

		public const string Language = "Language";

		public const string Login = "Login";

		public const string ServerRole = "ServerRole";

		public const string LinkedServer = "LinkedServer";

		public const string LinkedServerLogin = "LinkedServerLogin";

		public const string SystemDataType = "SystemDataType";

		public const string JobServer = "JobServer";

		public const string Category = "Category";

		public const string AlertSystem = "AlertSystem";

		public const string Alert = "Alert";

		public const string Operator = "Operator";

		public const string TargetServer = "TargetServer";

		public const string TargetServerGroup = "TargetServerGroup";

		public const string Job = "Job";

		public const string JobStep = "JobStep";

		public const string JobSchedule = "JobSchedule";

		public const string Settings = "Settings";

		public const string Information = "Information";

		public const string UserOptions = "UserOptions";

		public const string BackupDevice = "BackupDevice";

		public const string FullTextService = "FullTextService";

		public const string ServerActiveDirectory = "ServerActiveDirectory";

		public const string HttpEndpoint = "HttpEndpoint";

		public const string SoapConfiguration = "SoapConfiguration";

		public const string SoapMethod = "SoapMethod";

		public const string ServerAlias = "ServerAlias";

		public const string PhysicalPartition = "PhysicalPartition";

		public const string Audit = "Audit";

		public const string ServerAuditSpecification = "ServerAuditSpecification";

		public const string DatabaseAuditSpecification = "DatabaseAuditSpecification";

		public const string ManagedComputer = "ManagedComputer";

		public const string Service = "Service";

		public const string XmlSchemaCollection = "XmlSchemaCollection";

		public const string DatabaseEncryptionKey = "DatabaseEncryptionKey";

		public const string Restore = "Restore";

		public const string RestoreAsync = "RestoreAsync";

		public const string EnumAvailableSqlServers = "EnumAvailableSqlServers";

		public const string GetDataType = "GetDataType";

		public const string SetDataType = "SetDataType";

		public const string Backup = "Backup";

		public const string AvailabilityGroup = "AvailabilityGroup";

		public const string AvailabilityReplica = "AvailabilityReplica";

		public const string AvailabilityDatabase = "AvailabilityDatabase";

		public const string AvailabilityGroupListener = "AvailabilityGroupListener";

		public const string AvailabilityGroupListenerIPAddress = "AvailabilityGroupListenerIPAddress";

		public const string SecurityPolicy = "SecurityPolicy";

		public const string SecurityPredicate = "SecurityPredicate";

		public const string ExternalDataSource = "ExternalDataSource";

		public const string ExternalFileFormat = "ExternalFileFormat";

		public const string ColumnMasterKey = "ColumnMasterKey";

		public const string Win32Error = "Win32Error";

		public const string SmoSQLCLRUnAvailable = "SmoSQLCLRUnAvailable";

		public const string MoveToPool = "MoveToPool";

		public const string ResourcePoolNotExist = "ResourcePoolNotExist";

		public const string CannotMoveToInternalResourcePool = "CannotMoveToInternalResourcePool";

		public const string CannotMoveToSamePool = "CannotMoveToSamePool";

		public const string AffinityTypeCannotBeSet = "AffinityTypeCannotBeSet";

		public const string AffinityValueCannotBeSet = "AffinityValueCannotBeSet";

		public const string NoCPUAffinitized = "NoCPUAffinitized";

		public const string WrongIndexRangeProvidedCPU = "WrongIndexRangeProvidedCPU";

		public const string WrongIndexRangeProvidedNuma = "WrongIndexRangeProvidedNuma";

		public const string HoleInIndexRangeProvidedCPU = "HoleInIndexRangeProvidedCPU";

		public const string HoleInIndexRangeProvidedNumaNode = "HoleInIndexRangeProvidedNumaNode";

		public const string WrongIndexRangeProvidedScheduler = "WrongIndexRangeProvidedScheduler";

		public const string ResourceGovernorPoolMissing = "ResourceGovernorPoolMissing";

		public const string CannotSwitchDesignModeOff = "CannotSwitchDesignModeOff";

		public const string ServerVersionNotSpecified = "ServerVersionNotSpecified";

		public const string PropertyNotSetInDesignMode = "PropertyNotSetInDesignMode";

		public const string PropertyNotAvailableInDesignMode = "PropertyNotAvailableInDesignMode";

		public const string OperationNotAvailableInDesignMode = "OperationNotAvailableInDesignMode";

		public const string PropertyNotFound = "PropertyNotFound";

		public const string OnlyDesignModeSupported = "OnlyDesignModeSupported";

		public const string OnlySmoObjectsSupported = "OnlySmoObjectsSupported";

		public const string RootNotFound = "RootNotFound";

		public const string UnknownDomain = "UnknownDomain";

		public const string FileWritingException = "FileWritingException";

		public const string InvalideFileName = "InvalideFileName";

		public const string FolderPathNotFound = "FolderPathNotFound";

		public const string FilePerObjectUrnMissingName = "FilePerObjectUrnMissingName";

		public const string OrderingCycleDetected = "OrderingCycleDetected";

		public const string IncorrectEndpointProtocol = "IncorrectEndpointProtocol";

		public const string FileTableNotSupportedOnTargetEngine = "FileTableNotSupportedOnTargetEngine";

		public const string FileTableCannotHaveUserColumns = "FileTableCannotHaveUserColumns";

		public const string PropertyOnlySupportedForFileTable = "PropertyOnlySupportedForFileTable";

		public const string TableNotFileTable = "TableNotFileTable";

		public const string NamespaceNotEnabled = "NamespaceNotEnabled";

		public const string TableNotExternalTable = "TableNotExternalTable";

		public const string PropertyOnlySupportedForExternalTable = "PropertyOnlySupportedForExternalTable";

		public const string ConflictingExternalTableProperties = "ConflictingExternalTableProperties";

		public const string ExternalTableCannotContainChecks = "ExternalTableCannotContainChecks";

		public const string ExternalTableCannotContainForeignKeys = "ExternalTableCannotContainForeignKeys";

		public const string ExternalTableCannotContainPartitionSchemeParameters = "ExternalTableCannotContainPartitionSchemeParameters";

		public const string ExternalTableCannotContainTriggers = "ExternalTableCannotContainTriggers";

		public const string ExternalTableCannotContainIndexes = "ExternalTableCannotContainIndexes";

		public const string ExternalTableCannotContainPhysicalPartitions = "ExternalTableCannotContainPhysicalPartitions";

		public const string IdentityColumnForExternalTable = "IdentityColumnForExternalTable";

		public const string TruncateOperationNotSupportedOnExternalTables = "TruncateOperationNotSupportedOnExternalTables";

		public const string ChangeTrackingNotSupportedOnExternalTables = "ChangeTrackingNotSupportedOnExternalTables";

		public const string ShardingColumnNotSupportedWithNonShardedDistribution = "ShardingColumnNotSupportedWithNonShardedDistribution";

		public const string ShardingColumnNotSpecifiedForShardedDistribution = "ShardingColumnNotSpecifiedForShardedDistribution";

		public const string ShardingColumnNotAddedToTable = "ShardingColumnNotAddedToTable";

		public const string DependentPropertyMissing = "DependentPropertyMissing";

		public const string ColumnStoreCompressionNotSettable = "ColumnStoreCompressionNotSettable";

		public const string NoIndexUnique = "NoIndexUnique";

		public const string NoIndexIgnoreDupKey = "NoIndexIgnoreDupKey";

		public const string NoIndexClustered = "NoIndexClustered";

		public const string InvaildColumnStoreIndexOption = "InvaildColumnStoreIndexOption";

		public const string IndexOnTableView = "IndexOnTableView";

		public const string IncludedColumnNotSupported = "IncludedColumnNotSupported";

		public const string ConflictingIndexProperties = "ConflictingIndexProperties";

		public const string SelectiveXmlIndexDoesNotSupportReorganize = "SelectiveXmlIndexDoesNotSupportReorganize";

		public const string MoreThenOneXmlDefaultNamespace = "MoreThenOneXmlDefaultNamespace";

		public const string SecondarySelectiveXmlIndexModify = "SecondarySelectiveXmlIndexModify";

		public const string UnsupportedPropertyForSXI = "UnsupportedPropertyForSXI";

		public const string InvaildSXIOption = "InvaildSXIOption";

		public const string UnsupportedValueForSXI = "UnsupportedValueForSXI";

		public const string InvalidUpgradeToCCIIndexType = "InvalidUpgradeToCCIIndexType";

		public const string PropertyValidOnlyForColumnStoreIndexes = "PropertyValidOnlyForColumnStoreIndexes";

		public const string InvalidCompressionDelayValue = "InvalidCompressionDelayValue";

		public const string ExecutingScript = "ExecutingScript";

		public const string TransferDataException = "TransferDataException";

		public const string StartingDataTransfer = "StartingDataTransfer";

		public const string CompletedDataTransfer = "CompletedDataTransfer";

		public const string ColumnCollationIncompatible = "ColumnCollationIncompatible";

		public const string ConflictingExternalFileFormatProperties = "ConflictingExternalFileFormatProperties";

		public const string UnsupportedPropertyForAlter = "UnsupportedPropertyForAlter";

		public const string UnsupportedResourceManagerLocationProperty = "UnsupportedResourceManagerLocationProperty";

		public const string UnsupportedParamForDataSourceType = "UnsupportedParamForDataSourceType";

		public const string AlterNotSupportedForRelationalTypes = "AlterNotSupportedForRelationalTypes";

		public const string InvalidIndexSpecifiedForModifyingTextToCreateOrAlter = "InvalidIndexSpecifiedForModifyingTextToCreateOrAlter";

		public const string InvalidTextForModifyingToCreateOrAlter = "InvalidTextForModifyingToCreateOrAlter";

		public const string ExpectedGraphColumnNotFound = "ExpectedGraphColumnNotFound";

		private static ResourceManager resourceManager = new ResourceManager(typeof(ExceptionTemplatesImpl).FullName, typeof(ExceptionTemplatesImpl).Module.Assembly);

		private static CultureInfo _culture = null;

		public static CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				_culture = value;
			}
		}

		private Keys()
		{
		}

		public static string GetString(string key)
		{
			return resourceManager.GetString(key, _culture);
		}

		public static string GetString(string key, object arg0)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[1] { arg0 });
		}

		public static string GetString(string key, object arg0, object arg1)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[2] { arg0, arg1 });
		}

		public static string GetString(string key, object arg0, object arg1, object arg2)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), new object[3] { arg0, arg1, arg2 });
		}

		public static string GetString(string key, object arg0, object arg1, object arg2, object arg3)
		{
			return string.Format(CultureInfo.CurrentCulture, resourceManager.GetString(key, _culture), arg0, arg1, arg2, arg3);
		}
	}

	public static CultureInfo Culture
	{
		get
		{
			return Keys.Culture;
		}
		set
		{
			Keys.Culture = value;
		}
	}

	public static string InvalidPasswordHash => Keys.GetString("InvalidPasswordHash");

	public static string LoginPropertyNotSet => Keys.GetString("LoginPropertyNotSet");

	public static string LoginEnable => Keys.GetString("LoginEnable");

	public static string LoginDisable => Keys.GetString("LoginDisable");

	public static string AddCredential => Keys.GetString("AddCredential");

	public static string DropCredential => Keys.GetString("DropCredential");

	public static string EnterServerRoleName => Keys.GetString("EnterServerRoleName");

	public static string ServerRoleOwnerNameEmpty => Keys.GetString("ServerRoleOwnerNameEmpty");

	public static string CreateAlterNotSupported => Keys.GetString("CreateAlterNotSupported");

	public static string EnterSequenceName => Keys.GetString("EnterSequenceName");

	public static string EnterMinValue => Keys.GetString("EnterMinValue");

	public static string EnterMaxValue => Keys.GetString("EnterMaxValue");

	public static string EnterIncrementValue => Keys.GetString("EnterIncrementValue");

	public static string EnterStartValue => Keys.GetString("EnterStartValue");

	public static string UpgradeDll => Keys.GetString("UpgradeDll");

	public static string ProviderEnable => Keys.GetString("ProviderEnable");

	public static string ProviderDisable => Keys.GetString("ProviderDisable");

	public static string SourceTypeShouldBeProvider => Keys.GetString("SourceTypeShouldBeProvider");

	public static string CyclicalForeignKeys => Keys.GetString("CyclicalForeignKeys");

	public static string NullParameterTable => Keys.GetString("NullParameterTable");

	public static string NullParameterScriptingOptions => Keys.GetString("NullParameterScriptingOptions");

	public static string CannotEnableViewTrigger => Keys.GetString("CannotEnableViewTrigger");

	public static string ReplicationOptionNotSupportedForCloud => Keys.GetString("ReplicationOptionNotSupportedForCloud");

	public static string NoSparseOnColumnSet => Keys.GetString("NoSparseOnColumnSet");

	public static string NoSparseOnComputed => Keys.GetString("NoSparseOnComputed");

	public static string NoColumnSetOnComputed => Keys.GetString("NoColumnSetOnComputed");

	public static string NoGeneratedAlwaysColumnsOnNonTables => Keys.GetString("NoGeneratedAlwaysColumnsOnNonTables");

	public static string NoSparseOrColumnSetOnTemporalColumns => Keys.GetString("NoSparseOrColumnSetOnTemporalColumns");

	public static string ComputedTemporalColumns => Keys.GetString("ComputedTemporalColumns");

	public static string IdentityTemporalColumns => Keys.GetString("IdentityTemporalColumns");

	public static string NullableTemporalColumns => Keys.GetString("NullableTemporalColumns");

	public static string InvalidAlwaysEncryptedPropertyValues => Keys.GetString("InvalidAlwaysEncryptedPropertyValues");

	public static string NoMemoryOptimizedTemporalTables => Keys.GetString("NoMemoryOptimizedTemporalTables");

	public static string SystemVersionedTableWithoutPeriod => Keys.GetString("SystemVersionedTableWithoutPeriod");

	public static string HistoryTableWithoutSystemVersioning => Keys.GetString("HistoryTableWithoutSystemVersioning");

	public static string NoTemporalFileTables => Keys.GetString("NoTemporalFileTables");

	public static string NoAddingPeriodOnDroppedTable => Keys.GetString("NoAddingPeriodOnDroppedTable");

	public static string CannotHaveMultiplePeriods => Keys.GetString("CannotHaveMultiplePeriods");

	public static string NoDroppingPeriodOnDroppedTable => Keys.GetString("NoDroppingPeriodOnDroppedTable");

	public static string NoDroppingPeriodOnNotYetCreatedTable => Keys.GetString("NoDroppingPeriodOnNotYetCreatedTable");

	public static string CannotDropNonExistingPeriod => Keys.GetString("CannotDropNonExistingPeriod");

	public static string MustProvideExistingColumn => Keys.GetString("MustProvideExistingColumn");

	public static string PeriodMustHaveDifferentColumns => Keys.GetString("PeriodMustHaveDifferentColumns");

	public static string PeriodStartColumnMustBeGeneratedAlways => Keys.GetString("PeriodStartColumnMustBeGeneratedAlways");

	public static string PeriodEndColumnMustBeGeneratedAlways => Keys.GetString("PeriodEndColumnMustBeGeneratedAlways");

	public static string PeriodInvalidDataType => Keys.GetString("PeriodInvalidDataType");

	public static string InvalidPeriodColumnName => Keys.GetString("InvalidPeriodColumnName");

	public static string BothHistoryTableNameAndSchemaMustBeProvided => Keys.GetString("BothHistoryTableNameAndSchemaMustBeProvided");

	public static string NoHiddenColumnsOnNonGeneratedAlwaysColumns => Keys.GetString("NoHiddenColumnsOnNonGeneratedAlwaysColumns");

	public static string InvalidHistoryRetentionPeriodSpecification => Keys.GetString("InvalidHistoryRetentionPeriodSpecification");

	public static string InvalidHistoryRetentionPeriodUnitSpecification => Keys.GetString("InvalidHistoryRetentionPeriodUnitSpecification");

	public static string NoDataMaskingOnNonTables => Keys.GetString("NoDataMaskingOnNonTables");

	public static string InvalidMaskingFunctionFormat => Keys.GetString("InvalidMaskingFunctionFormat");

	public static string MaskingFunctionOnWrongType => Keys.GetString("MaskingFunctionOnWrongType");

	public static string NoDataMaskingOnColumnSet => Keys.GetString("NoDataMaskingOnColumnSet");

	public static string NoDataMaskingOnTemporalColumns => Keys.GetString("NoDataMaskingOnTemporalColumns");

	public static string NoDataMaskingOnComputedColumns => Keys.GetString("NoDataMaskingOnComputedColumns");

	public static string NoDataMaskingOnFileStreamColumns => Keys.GetString("NoDataMaskingOnFileStreamColumns");

	public static string NoDataMaskingOnEncryptedColumns => Keys.GetString("NoDataMaskingOnEncryptedColumns");

	public static string MissingBackupDevices => Keys.GetString("MissingBackupDevices");

	public static string MissingBackupDeviceType => Keys.GetString("MissingBackupDeviceType");

	public static string PipeDeviceNotSupported => Keys.GetString("PipeDeviceNotSupported");

	public static string BackupEncryptionNotSupported => Keys.GetString("BackupEncryptionNotSupported");

	public static string UnknownShrinkType => Keys.GetString("UnknownShrinkType");

	public static string CannotChangePrimary => Keys.GetString("CannotChangePrimary");

	public static string OnlyOnePrimaryFile => Keys.GetString("OnlyOnePrimaryFile");

	public static string MustSpecifyGrowth => Keys.GetString("MustSpecifyGrowth");

	public static string WrongPercentageGrowth => Keys.GetString("WrongPercentageGrowth");

	public static string WrongSize => Keys.GetString("WrongSize");

	public static string InvalidSizeFileStream => Keys.GetString("InvalidSizeFileStream");

	public static string InvalidMaxSizeFileStream => Keys.GetString("InvalidMaxSizeFileStream");

	public static string InvalidGrowthFileStream => Keys.GetString("InvalidGrowthFileStream");

	public static string PrimaryAlreadyDefault => Keys.GetString("PrimaryAlreadyDefault");

	public static string ColumnNotVarbinaryMax => Keys.GetString("ColumnNotVarbinaryMax");

	public static string NotClusteredIndex => Keys.GetString("NotClusteredIndex");

	public static string NotNonClusteredIndex => Keys.GetString("NotNonClusteredIndex");

	public static string UnsupportedFileGroupType => Keys.GetString("UnsupportedFileGroupType");

	public static string VarDecimalAndDataCompressionConflict => Keys.GetString("VarDecimalAndDataCompressionConflict");

	public static string RebuildHeapOnClusteredIndexError => Keys.GetString("RebuildHeapOnClusteredIndexError");

	public static string TooFewFiles => Keys.GetString("TooFewFiles");

	public static string UnknownChild => Keys.GetString("UnknownChild");

	public static string InvalidGranteeList => Keys.GetString("InvalidGranteeList");

	public static string DatabaseAlreadyExists => Keys.GetString("DatabaseAlreadyExists");

	public static string ReasonTextIsEncrypted => Keys.GetString("ReasonTextIsEncrypted");

	public static string ReasonPropertyIsNotSupportedOnCurrentServerVersion => Keys.GetString("ReasonPropertyIsNotSupportedOnCurrentServerVersion");

	public static string SearchPropertyIntIDNotValid => Keys.GetString("SearchPropertyIntIDNotValid");

	public static string SearchPropertySetGuidNotValid => Keys.GetString("SearchPropertySetGuidNotValid");

	public static string SearchPropertyGuidIntIdNotValid => Keys.GetString("SearchPropertyGuidIntIdNotValid");

	public static string NullOrEmptyParameterSourceDatabaseName => Keys.GetString("NullOrEmptyParameterSourceDatabaseName");

	public static string NullOrEmptyParameterSourceSearchPropertyListName => Keys.GetString("NullOrEmptyParameterSourceSearchPropertyListName");

	public static string EmptySourceSearchPropertyListName => Keys.GetString("EmptySourceSearchPropertyListName");

	public static string SearchPropertyListNameAllWhiteSpaces => Keys.GetString("SearchPropertyListNameAllWhiteSpaces");

	public static string EmptyRestorePlan => Keys.GetString("EmptyRestorePlan");

	public static string MultipleDatabaseSelectedToRestore => Keys.GetString("MultipleDatabaseSelectedToRestore");

	public static string ConflictWithNoRecovery => Keys.GetString("ConflictWithNoRecovery");

	public static string BackupsOfDifferentDb => Keys.GetString("BackupsOfDifferentDb");

	public static string FullBackupShouldBeFirst => Keys.GetString("FullBackupShouldBeFirst");

	public static string BackupsNotInSequence => Keys.GetString("BackupsNotInSequence");

	public static string NoFullBackupSelected => Keys.GetString("NoFullBackupSelected");

	public static string WrongDiffbackup => Keys.GetString("WrongDiffbackup");

	public static string DiffBackupNotCompatible => Keys.GetString("DiffBackupNotCompatible");

	public static string WrongTLogbackup => Keys.GetString("WrongTLogbackup");

	public static string TailLogBackupDeviceNull => Keys.GetString("TailLogBackupDeviceNull");

	public static string OnlyLastRestoreWithNoRecovery => Keys.GetString("OnlyLastRestoreWithNoRecovery");

	public static string UnsupportedServerVersion => Keys.GetString("UnsupportedServerVersion");

	public static string BackupMediaSetEmpty => Keys.GetString("BackupMediaSetEmpty");

	public static string GetMediaSetGuid => Keys.GetString("GetMediaSetGuid");

	public static string MediaNotPartOfSet => Keys.GetString("MediaNotPartOfSet");

	public static string FileGroupNotSupported => Keys.GetString("FileGroupNotSupported");

	public static string InvalidDatabaseState => Keys.GetString("InvalidDatabaseState");

	public static string PageRestoreOnlyForFullRecovery => Keys.GetString("PageRestoreOnlyForFullRecovery");

	public static string CloseConnectionsFailed => Keys.GetString("CloseConnectionsFailed");

	public static string BackupTailLog => Keys.GetString("BackupTailLog");

	public static string TailLog => Keys.GetString("TailLog");

	public static string UnableToCreateRestoreSequence => Keys.GetString("UnableToCreateRestoreSequence");

	public static string UnableToCreatePageRestoreSequence => Keys.GetString("UnableToCreatePageRestoreSequence");

	public static string UnableToCreatePlanTakeTLogBackup => Keys.GetString("UnableToCreatePlanTakeTLogBackup");

	public static string OperationCancelledByUser => Keys.GetString("OperationCancelledByUser");

	public static string InvalidSuspectpage => Keys.GetString("InvalidSuspectpage");

	public static string SqlInnerException => Keys.GetString("SqlInnerException");

	public static string InnerException => Keys.GetString("InnerException");

	public static string InnerWmiException => Keys.GetString("InnerWmiException");

	public static string UnknownError => Keys.GetString("UnknownError");

	public static string UnableToRetrieveBackupHistory => Keys.GetString("UnableToRetrieveBackupHistory");

	public static string OneFilePageSupported => Keys.GetString("OneFilePageSupported");

	public static string ScriptDataNotSupportedByThisMethod => Keys.GetString("ScriptDataNotSupportedByThisMethod");

	public static string ForeignKeyCycleInObjects => Keys.GetString("ForeignKeyCycleInObjects");

	public static string NotSupportedOnStandalone => Keys.GetString("NotSupportedOnStandalone");

	public static string NotSupportedOnCloud => Keys.GetString("NotSupportedOnCloud");

	public static string InvalidScriptingOptions => Keys.GetString("InvalidScriptingOptions");

	public static string SupportedOnlyBelow110 => Keys.GetString("SupportedOnlyBelow110");

	public static string SupportedOnlyBelow100 => Keys.GetString("SupportedOnlyBelow100");

	public static string SupportedOnlyBelow90 => Keys.GetString("SupportedOnlyBelow90");

	public static string SupportedOnlyOn150 => Keys.GetString("SupportedOnlyOn150");

	public static string SupportedOnlyOn140 => Keys.GetString("SupportedOnlyOn140");

	public static string SupportedOnlyOn130 => Keys.GetString("SupportedOnlyOn130");

	public static string SupportedOnlyOn120 => Keys.GetString("SupportedOnlyOn120");

	public static string SupportedOnlyOn110 => Keys.GetString("SupportedOnlyOn110");

	public static string SupportedOnlyOn105 => Keys.GetString("SupportedOnlyOn105");

	public static string SupportedOnlyOn100 => Keys.GetString("SupportedOnlyOn100");

	public static string SupportedOnlyOn90 => Keys.GetString("SupportedOnlyOn90");

	public static string SupportedOnlyOn80SP3 => Keys.GetString("SupportedOnlyOn80SP3");

	public static string SupportedOnlyOn80 => Keys.GetString("SupportedOnlyOn80");

	public static string NotSupportedForVersionEarlierThan110 => Keys.GetString("NotSupportedForVersionEarlierThan110");

	public static string NotSupportedForVersionEarlierThan120 => Keys.GetString("NotSupportedForVersionEarlierThan120");

	public static string NotSupportedForVersionEarlierThan130 => Keys.GetString("NotSupportedForVersionEarlierThan130");

	public static string NotSupportedForVersionEarlierThan140 => Keys.GetString("NotSupportedForVersionEarlierThan140");

	public static string NotSupportedForVersionEarlierThan150 => Keys.GetString("NotSupportedForVersionEarlierThan150");

	public static string MustSpecifyOneParameter => Keys.GetString("MustSpecifyOneParameter");

	public static string NeedPSParams => Keys.GetString("NeedPSParams");

	public static string ViewCannotHaveKeys => Keys.GetString("ViewCannotHaveKeys");

	public static string InvalidAcctName => Keys.GetString("InvalidAcctName");

	public static string OperationOnlyInPendingState => Keys.GetString("OperationOnlyInPendingState");

	public static string OperationNotInPendingState => Keys.GetString("OperationNotInPendingState");

	public static string RemoteServerEndpointRequired => Keys.GetString("RemoteServerEndpointRequired");

	public static string DatabaseScopedCredentialsRequired => Keys.GetString("DatabaseScopedCredentialsRequired");

	public static string MissingBoundingParameters => Keys.GetString("MissingBoundingParameters");

	public static string InvalidNonGeometryParameters => Keys.GetString("InvalidNonGeometryParameters");

	public static string UnsupportedNonSpatialParameters => Keys.GetString("UnsupportedNonSpatialParameters");

	public static string NotSpatialIndexOnView => Keys.GetString("NotSpatialIndexOnView");

	public static string OneColumnInSpatialIndex => Keys.GetString("OneColumnInSpatialIndex");

	public static string NoSpatialIndexUnique => Keys.GetString("NoSpatialIndexUnique");

	public static string NoSpatialIndexClustered => Keys.GetString("NoSpatialIndexClustered");

	public static string LowPriorityCannotBeSetForDrop => Keys.GetString("LowPriorityCannotBeSetForDrop");

	public static string OnlyHashIndexIsSupportedInAlter => Keys.GetString("OnlyHashIndexIsSupportedInAlter");

	public static string NotXmlIndexOnView => Keys.GetString("NotXmlIndexOnView");

	public static string OneColumnInXmlIndex => Keys.GetString("OneColumnInXmlIndex");

	public static string BucketCountForHashIndex => Keys.GetString("BucketCountForHashIndex");

	public static string ScriptMemoryOptimizedIndex => Keys.GetString("ScriptMemoryOptimizedIndex");

	public static string HashIndexTableDependency => Keys.GetString("HashIndexTableDependency");

	public static string TableMemoryOptimizedIndexDependency => Keys.GetString("TableMemoryOptimizedIndexDependency");

	public static string TableSqlDwIndexRestrictions => Keys.GetString("TableSqlDwIndexRestrictions");

	public static string NoXmlIndexUnique => Keys.GetString("NoXmlIndexUnique");

	public static string NoXmlIndexClustered => Keys.GetString("NoXmlIndexClustered");

	public static string UnsupportedNonXmlParameters => Keys.GetString("UnsupportedNonXmlParameters");

	public static string CantSetDefaultFalse => Keys.GetString("CantSetDefaultFalse");

	public static string TimeoutMustBePositive => Keys.GetString("TimeoutMustBePositive");

	public static string ChangeTrackingException => Keys.GetString("ChangeTrackingException");

	public static string MissingChangeTrackingParameters => Keys.GetString("MissingChangeTrackingParameters");

	public static string TrackColumnsException => Keys.GetString("TrackColumnsException");

	public static string ViewColumnsCannotBeModified => Keys.GetString("ViewColumnsCannotBeModified");

	public static string SystemMessageReadOnly => Keys.GetString("SystemMessageReadOnly");

	public static string CantSetContainedDatabaseCatalogCollation => Keys.GetString("CantSetContainedDatabaseCatalogCollation");

	public static string ViewFragInfoNotInV7 => Keys.GetString("ViewFragInfoNotInV7");

	public static string DtsNotInstalled => Keys.GetString("DtsNotInstalled");

	public static string NSNotInstalled => Keys.GetString("NSNotInstalled");

	public static string InvalidNSOperation => Keys.GetString("InvalidNSOperation");

	public static string NSNotConnectedToServer => Keys.GetString("NSNotConnectedToServer");

	public static string InvalidThreadPoolSize => Keys.GetString("InvalidThreadPoolSize");

	public static string InvalidQuantumLimit => Keys.GetString("InvalidQuantumLimit");

	public static string InvalidQuantumDuration => Keys.GetString("InvalidQuantumDuration");

	public static string InvalidThrottle => Keys.GetString("InvalidThrottle");

	public static string InvalidFailuresBeforeLoggingEvent => Keys.GetString("InvalidFailuresBeforeLoggingEvent");

	public static string InvalidFailuresBeforeAbort => Keys.GetString("InvalidFailuresBeforeAbort");

	public static string InvalidMulticastRecipientLimit => Keys.GetString("InvalidMulticastRecipientLimit");

	public static string InvalidNotificationClassBatchSize => Keys.GetString("InvalidNotificationClassBatchSize");

	public static string StartTimeGreaterThanADay => Keys.GetString("StartTimeGreaterThanADay");

	public static string NoPermissions => Keys.GetString("NoPermissions");

	public static string PrimaryFgMustHaveFiles => Keys.GetString("PrimaryFgMustHaveFiles");

	public static string NeedToSetParent => Keys.GetString("NeedToSetParent");

	public static string ErrorInCreatingState => Keys.GetString("ErrorInCreatingState");

	public static string BackupFailed => Keys.GetString("BackupFailed");

	public static string RestoreFailed => Keys.GetString("RestoreFailed");

	public static string SetPasswordError => Keys.GetString("SetPasswordError");

	public static string PassPhraseAndIdentityNotSpecified => Keys.GetString("PassPhraseAndIdentityNotSpecified");

	public static string PassPhraseNotSpecified => Keys.GetString("PassPhraseNotSpecified");

	public static string NotEnoughRights => Keys.GetString("NotEnoughRights");

	public static string SqlDwCreateRequiredParameterMissing => Keys.GetString("SqlDwCreateRequiredParameterMissing");

	public static string CannotAlterKeyWithProvider => Keys.GetString("CannotAlterKeyWithProvider");

	public static string AlterQueryStorePropertyForRestoringDatabase => Keys.GetString("AlterQueryStorePropertyForRestoringDatabase");

	public static string NoUrnSuffix => Keys.GetString("NoUrnSuffix");

	public static string GetParentFailed => Keys.GetString("GetParentFailed");

	public static string ParentNull => Keys.GetString("ParentNull");

	public static string InvalidFileInformationData => Keys.GetString("InvalidFileInformationData");

	public static string ExecutionContextPrincipalIsNotSpecified => Keys.GetString("ExecutionContextPrincipalIsNotSpecified");

	public static string MissingBrokerEndpoint => Keys.GetString("MissingBrokerEndpoint");

	public static string CannotEnableSoapEndpoints => Keys.GetString("CannotEnableSoapEndpoints");

	public static string ServiceError0 => Keys.GetString("ServiceError0");

	public static string ServiceError1 => Keys.GetString("ServiceError1");

	public static string ServiceError2 => Keys.GetString("ServiceError2");

	public static string ServiceError3 => Keys.GetString("ServiceError3");

	public static string ServiceError4 => Keys.GetString("ServiceError4");

	public static string ServiceError5 => Keys.GetString("ServiceError5");

	public static string ServiceError6 => Keys.GetString("ServiceError6");

	public static string ServiceError7 => Keys.GetString("ServiceError7");

	public static string ServiceError8 => Keys.GetString("ServiceError8");

	public static string ServiceError9 => Keys.GetString("ServiceError9");

	public static string ServiceError10 => Keys.GetString("ServiceError10");

	public static string ServiceError11 => Keys.GetString("ServiceError11");

	public static string ServiceError12 => Keys.GetString("ServiceError12");

	public static string ServiceError13 => Keys.GetString("ServiceError13");

	public static string ServiceError14 => Keys.GetString("ServiceError14");

	public static string ServiceError15 => Keys.GetString("ServiceError15");

	public static string ServiceError16 => Keys.GetString("ServiceError16");

	public static string ServiceError17 => Keys.GetString("ServiceError17");

	public static string ServiceError18 => Keys.GetString("ServiceError18");

	public static string ServiceError19 => Keys.GetString("ServiceError19");

	public static string ServiceError20 => Keys.GetString("ServiceError20");

	public static string ServiceError21 => Keys.GetString("ServiceError21");

	public static string ServiceError22 => Keys.GetString("ServiceError22");

	public static string ServiceError23 => Keys.GetString("ServiceError23");

	public static string ServiceError24 => Keys.GetString("ServiceError24");

	public static string UnsupportedFeatureSqlAgent => Keys.GetString("UnsupportedFeatureSqlAgent");

	public static string UnsupportedFeatureSqlMail => Keys.GetString("UnsupportedFeatureSqlMail");

	public static string UnsupportedFeatureServiceBroker => Keys.GetString("UnsupportedFeatureServiceBroker");

	public static string UnsupportedFeatureFullText => Keys.GetString("UnsupportedFeatureFullText");

	public static string UnsupportedFeatureResourceGovernor => Keys.GetString("UnsupportedFeatureResourceGovernor");

	public static string UnsupportedFeatureSmartAdmin => Keys.GetString("UnsupportedFeatureSmartAdmin");

	public static string CannotSubscribe => Keys.GetString("CannotSubscribe");

	public static string CannotStartSubscription => Keys.GetString("CannotStartSubscription");

	public static string InnerRegSvrException => Keys.GetString("InnerRegSvrException");

	public static string SqlServerTypeName => Keys.GetString("SqlServerTypeName");

	public static string ColumnsMustBeSpecified => Keys.GetString("ColumnsMustBeSpecified");

	public static string PasswdModiOnlyForStandardLogin => Keys.GetString("PasswdModiOnlyForStandardLogin");

	public static string DenyLoginModiNotForStandardLogin => Keys.GetString("DenyLoginModiNotForStandardLogin");

	public static string CloudSidNotApplicableOnStandalone => Keys.GetString("CloudSidNotApplicableOnStandalone");

	public static string CannotCreateExtendedPropertyWithoutSchema => Keys.GetString("CannotCreateExtendedPropertyWithoutSchema");

	public static string InvalidSchema => Keys.GetString("InvalidSchema");

	public static string FailedToChangeSchema => Keys.GetString("FailedToChangeSchema");

	public static string InvalidUrnServerLevel => Keys.GetString("InvalidUrnServerLevel");

	public static string ServerLevelMustBePresent => Keys.GetString("ServerLevelMustBePresent");

	public static string OperationInProgress => Keys.GetString("OperationInProgress");

	public static string PasswordError => Keys.GetString("PasswordError");

	public static string MediaPasswordError => Keys.GetString("MediaPasswordError");

	public static string CannotChangePassword => Keys.GetString("CannotChangePassword");

	public static string CannotChangePasswordForUser => Keys.GetString("CannotChangePasswordForUser");

	public static string PasswordOnlyForDatabaseAuthenticatedNonWindowsUser => Keys.GetString("PasswordOnlyForDatabaseAuthenticatedNonWindowsUser");

	public static string DefaultLanguageOnlyForDatabaseAuthenticatedUser => Keys.GetString("DefaultLanguageOnlyForDatabaseAuthenticatedUser");

	public static string CannotCopyPasswordToUser => Keys.GetString("CannotCopyPasswordToUser");

	public static string FollowingPropertiesCanBeSetOnlyWithContainmentEnabled => Keys.GetString("FollowingPropertiesCanBeSetOnlyWithContainmentEnabled");

	public static string OperationNotSupportedWhenPartOfAUDF => Keys.GetString("OperationNotSupportedWhenPartOfAUDF");

	public static string ReasonIntextMode => Keys.GetString("ReasonIntextMode");

	public static string ReasonNotIntextMode => Keys.GetString("ReasonNotIntextMode");

	public static string CollectionCannotBeModified => Keys.GetString("CollectionCannotBeModified");

	public static string KeyOptionsIncorrect => Keys.GetString("KeyOptionsIncorrect");

	public static string TableOrViewParentForUpdateStatistics => Keys.GetString("TableOrViewParentForUpdateStatistics");

	public static string PropertyAvailable => Keys.GetString("PropertyAvailable");

	public static string ServerPropertyMustBeSetForOlap => Keys.GetString("ServerPropertyMustBeSetForOlap");

	public static string InvalidInstanceName => Keys.GetString("InvalidInstanceName");

	public static string TransferCtorErr => Keys.GetString("TransferCtorErr");

	public static string UnsupportedVersionException => Keys.GetString("UnsupportedVersionException");

	public static string UnsupportedEngineTypeException => Keys.GetString("UnsupportedEngineTypeException");

	public static string UnsupportedEngineEditionException => Keys.GetString("UnsupportedEngineEditionException");

	public static string TargetRecoveryTimeNotNegative => Keys.GetString("TargetRecoveryTimeNotNegative");

	public static string CannotEnableUDTTTrigger => Keys.GetString("CannotEnableUDTTTrigger");

	public static string NotIndexOnUDTT => Keys.GetString("NotIndexOnUDTT");

	public static string NotXmlIndexOnUDTT => Keys.GetString("NotXmlIndexOnUDTT");

	public static string NotFullTextIndexOnUDTT => Keys.GetString("NotFullTextIndexOnUDTT");

	public static string UDTTColumnsCannotBeModified => Keys.GetString("UDTTColumnsCannotBeModified");

	public static string UDTTCannotBeModified => Keys.GetString("UDTTCannotBeModified");

	public static string UDTTIndexCannotBeModified => Keys.GetString("UDTTIndexCannotBeModified");

	public static string NotFragInfoOnUDTT => Keys.GetString("NotFragInfoOnUDTT");

	public static string NotStatisticsOnUDTT => Keys.GetString("NotStatisticsOnUDTT");

	public static string NotTriggersOnUDTT => Keys.GetString("NotTriggersOnUDTT");

	public static string NotCheckIndexOnUDTT => Keys.GetString("NotCheckIndexOnUDTT");

	public static string UDTTChecksCannotBeModified => Keys.GetString("UDTTChecksCannotBeModified");

	public static string OperationNotSupportedWhenPartOfUDTT => Keys.GetString("OperationNotSupportedWhenPartOfUDTT");

	public static string AddAuditSpecificationDetail => Keys.GetString("AddAuditSpecificationDetail");

	public static string RemoveAuditSpecificationDetail => Keys.GetString("RemoveAuditSpecificationDetail");

	public static string EnterAuditName => Keys.GetString("EnterAuditName");

	public static string EnterOwner => Keys.GetString("EnterOwner");

	public static string EnterFilePath => Keys.GetString("EnterFilePath");

	public static string EnterName => Keys.GetString("EnterName");

	public static string EnterServerAudit => Keys.GetString("EnterServerAudit");

	public static string EnterStoragePath => Keys.GetString("EnterStoragePath");

	public static string ReserveDiskSpaceNotAllowedWhenMaxFileSizeIsUnlimited => Keys.GetString("ReserveDiskSpaceNotAllowedWhenMaxFileSizeIsUnlimited");

	public static string EnterServerCertificate => Keys.GetString("EnterServerCertificate");

	public static string EnterServerAsymmetricKey => Keys.GetString("EnterServerAsymmetricKey");

	public static string CertificateNotBackedUp => Keys.GetString("CertificateNotBackedUp");

	public static string FailedOperationMessageNotSupportedTempdb => Keys.GetString("FailedOperationMessageNotSupportedTempdb");

	public static string InvalidOperationInDisconnectedMode => Keys.GetString("InvalidOperationInDisconnectedMode");

	public static string ReadOnlyRoutingListContainsEmptyReplicaName => Keys.GetString("ReadOnlyRoutingListContainsEmptyReplicaName");

	public static string CannotUpdateBothReadOnlyRoutingLists => Keys.GetString("CannotUpdateBothReadOnlyRoutingLists");

	public static string SetAccount => Keys.GetString("SetAccount");

	public static string Create => Keys.GetString("Create");

	public static string Alter => Keys.GetString("Alter");

	public static string CreateOrAlter => Keys.GetString("CreateOrAlter");

	public static string Drop => Keys.GetString("Drop");

	public static string Rename => Keys.GetString("Rename");

	public static string Script => Keys.GetString("Script");

	public static string Grant => Keys.GetString("Grant");

	public static string Revoke => Keys.GetString("Revoke");

	public static string Deny => Keys.GetString("Deny");

	public static string GrantWithGrant => Keys.GetString("GrantWithGrant");

	public static string Bind => Keys.GetString("Bind");

	public static string Unbind => Keys.GetString("Unbind");

	public static string AddDefaultConstraint => Keys.GetString("AddDefaultConstraint");

	public static string TestMailProfile => Keys.GetString("TestMailProfile");

	public static string TestNetSend => Keys.GetString("TestNetSend");

	public static string SetState => Keys.GetString("SetState");

	public static string DropAndMove => Keys.GetString("DropAndMove");

	public static string GetSmoObject => Keys.GetString("GetSmoObject");

	public static string AdvancedProperties => Keys.GetString("AdvancedProperties");

	public static string AddCollection => Keys.GetString("AddCollection");

	public static string RemoveCollection => Keys.GetString("RemoveCollection");

	public static string This => Keys.GetString("This");

	public static string Abort => Keys.GetString("Abort");

	public static string AddMember => Keys.GetString("AddMember");

	public static string DropMember => Keys.GetString("DropMember");

	public static string EnumMembers => Keys.GetString("EnumMembers");

	public static string EnumPermissions => Keys.GetString("EnumPermissions");

	public static string EnumContainingRoles => Keys.GetString("EnumContainingRoles");

	public static string EnumAgentProxyAccounts => Keys.GetString("EnumAgentProxyAccounts");

	public static string AddMemberServer => Keys.GetString("AddMemberServer");

	public static string EnumMemberServers => Keys.GetString("EnumMemberServers");

	public static string RemoveMemberServer => Keys.GetString("RemoveMemberServer");

	public static string ResetOccurrenceCount => Keys.GetString("ResetOccurrenceCount");

	public static string AddNotification => Keys.GetString("AddNotification");

	public static string RemoveNotification => Keys.GetString("RemoveNotification");

	public static string UpdateNotification => Keys.GetString("UpdateNotification");

	public static string EnumNotifications => Keys.GetString("EnumNotifications");

	public static string ApplyToTargetServer => Keys.GetString("ApplyToTargetServer");

	public static string ApplyToTargetServerGroup => Keys.GetString("ApplyToTargetServerGroup");

	public static string EnumAlerts => Keys.GetString("EnumAlerts");

	public static string EnumHistory => Keys.GetString("EnumHistory");

	public static string EnumJobStepOutputLogs => Keys.GetString("EnumJobStepOutputLogs");

	public static string EnumTargetServers => Keys.GetString("EnumTargetServers");

	public static string EnumJobSteps => Keys.GetString("EnumJobSteps");

	public static string Invoke => Keys.GetString("Invoke");

	public static string PurgeHistory => Keys.GetString("PurgeHistory");

	public static string AddSharedSchedule => Keys.GetString("AddSharedSchedule");

	public static string RemoveSharedSchedule => Keys.GetString("RemoveSharedSchedule");

	public static string RemoveAllJobSchedules => Keys.GetString("RemoveAllJobSchedules");

	public static string RemoveAllJobSteps => Keys.GetString("RemoveAllJobSteps");

	public static string RemoveFromTargetServer => Keys.GetString("RemoveFromTargetServer");

	public static string RemoveFromTargetServerGroup => Keys.GetString("RemoveFromTargetServerGroup");

	public static string Start => Keys.GetString("Start");

	public static string Stop => Keys.GetString("Stop");

	public static string SetPassword => Keys.GetString("SetPassword");

	public static string ChangePassword => Keys.GetString("ChangePassword");

	public static string MakeContained => Keys.GetString("MakeContained");

	public static string EnumDatabaseMappings => Keys.GetString("EnumDatabaseMappings");

	public static string GetDatabaseUser => Keys.GetString("GetDatabaseUser");

	public static string AddToRole => Keys.GetString("AddToRole");

	public static string AttachDatabase => Keys.GetString("AttachDatabase");

	public static string DetachDatabase => Keys.GetString("DetachDatabase");

	public static string EnumCollations => Keys.GetString("EnumCollations");

	public static string EnumPerformanceCounters => Keys.GetString("EnumPerformanceCounters");

	public static string EnumErrorLogs => Keys.GetString("EnumErrorLogs");

	public static string EnumDatabaseMirrorWitnessRoles => Keys.GetString("EnumDatabaseMirrorWitnessRoles");

	public static string ReadErrorLog => Keys.GetString("ReadErrorLog");

	public static string KillDatabase => Keys.GetString("KillDatabase");

	public static string KillProcess => Keys.GetString("KillProcess");

	public static string GetActiveDBConnectionCount => Keys.GetString("GetActiveDBConnectionCount");

	public static string DropAllActiveDBConnections => Keys.GetString("DropAllActiveDBConnections");

	public static string EnumDirectories => Keys.GetString("EnumDirectories");

	public static string EnumLocks => Keys.GetString("EnumLocks");

	public static string AddPrivateKey => Keys.GetString("AddPrivateKey");

	public static string ExportCertificate => Keys.GetString("ExportCertificate");

	public static string ChangePrivateKeyPassword => Keys.GetString("ChangePrivateKeyPassword");

	public static string RemovePrivateKey => Keys.GetString("RemovePrivateKey");

	public static string AddKeyEncryption => Keys.GetString("AddKeyEncryption");

	public static string DropKeyEncryption => Keys.GetString("DropKeyEncryption");

	public static string SymmetricKeyOpen => Keys.GetString("SymmetricKeyOpen");

	public static string SymmetricKeyClose => Keys.GetString("SymmetricKeyClose");

	public static string EnumLogins => Keys.GetString("EnumLogins");

	public static string EnumWindowsDomainGroups => Keys.GetString("EnumWindowsDomainGroups");

	public static string EnumProcesses => Keys.GetString("EnumProcesses");

	public static string EnumStartupProcedures => Keys.GetString("EnumStartupProcedures");

	public static string EnumWindowsUserInfo => Keys.GetString("EnumWindowsUserInfo");

	public static string EnumWindowsGroupInfo => Keys.GetString("EnumWindowsGroupInfo");

	public static string EnumAvailableMedia => Keys.GetString("EnumAvailableMedia");

	public static string EnumServerAttributes => Keys.GetString("EnumServerAttributes");

	public static string DeleteBackupHistory => Keys.GetString("DeleteBackupHistory");

	public static string Refresh => Keys.GetString("Refresh");

	public static string EnumBoundColumns => Keys.GetString("EnumBoundColumns");

	public static string EnumBoundDataTypes => Keys.GetString("EnumBoundDataTypes");

	public static string CheckAllocations => Keys.GetString("CheckAllocations");

	public static string CheckCatalog => Keys.GetString("CheckCatalog");

	public static string CheckIdentityValues => Keys.GetString("CheckIdentityValues");

	public static string CheckTables => Keys.GetString("CheckTables");

	public static string CheckTable => Keys.GetString("CheckTable");

	public static string Shrink => Keys.GetString("Shrink");

	public static string RecalculateSpaceUsage => Keys.GetString("RecalculateSpaceUsage");

	public static string PrefetchObjects => Keys.GetString("PrefetchObjects");

	public static string EnumTransactions => Keys.GetString("EnumTransactions");

	public static string GetTransactionCount => Keys.GetString("GetTransactionCount");

	public static string ImportXmlSchema => Keys.GetString("ImportXmlSchema");

	public static string ExtendXmlSchema => Keys.GetString("ExtendXmlSchema");

	public static string RemoveFullTextCatalogs => Keys.GetString("RemoveFullTextCatalogs");

	public static string SetDefaultFullTextCatalog => Keys.GetString("SetDefaultFullTextCatalog");

	public static string SetDefaultFileGroup => Keys.GetString("SetDefaultFileGroup");

	public static string SetDefaultFileStreamFileGroup => Keys.GetString("SetDefaultFileStreamFileGroup");

	public static string CheckFileGroup => Keys.GetString("CheckFileGroup");

	public static string CheckIndex => Keys.GetString("CheckIndex");

	public static string Checkpoint => Keys.GetString("Checkpoint");

	public static string Cleanup => Keys.GetString("Cleanup");

	public static string CannotReadProp => Keys.GetString("CannotReadProp");

	public static string UpdateLanguageResources => Keys.GetString("UpdateLanguageResources");

	public static string CatalogUpgradeOptions => Keys.GetString("CatalogUpgradeOptions");

	public static string EnumLanguages => Keys.GetString("EnumLanguages");

	public static string EnumSemanticLanguages => Keys.GetString("EnumSemanticLanguages");

	public static string ClearHostLoginAccount => Keys.GetString("ClearHostLoginAccount");

	public static string SetProxyAccount => Keys.GetString("SetProxyAccount");

	public static string ClearProxyAccount => Keys.GetString("ClearProxyAccount");

	public static string SetMsxAccount => Keys.GetString("SetMsxAccount");

	public static string ClearMsxAccount => Keys.GetString("ClearMsxAccount");

	public static string CycleErrorLog => Keys.GetString("CycleErrorLog");

	public static string EnumJobHistory => Keys.GetString("EnumJobHistory");

	public static string EnumJobs => Keys.GetString("EnumJobs");

	public static string MoreThanOneProxyAccountIsNotSupported => Keys.GetString("MoreThanOneProxyAccountIsNotSupported");

	public static string AddSubSystems => Keys.GetString("AddSubSystems");

	public static string DeleteJobStepLogs => Keys.GetString("DeleteJobStepLogs");

	public static string RemoveSubSystems => Keys.GetString("RemoveSubSystems");

	public static string EnumSubSystems => Keys.GetString("EnumSubSystems");

	public static string AddMailAccountToProfile => Keys.GetString("AddMailAccountToProfile");

	public static string RemoveMailAccountFromProfile => Keys.GetString("RemoveMailAccountFromProfile");

	public static string EnumMailAccountsForProfile => Keys.GetString("EnumMailAccountsForProfile");

	public static string AddPrincipalToMailProfile => Keys.GetString("AddPrincipalToMailProfile");

	public static string RemovePrincipalFromMailProfile => Keys.GetString("RemovePrincipalFromMailProfile");

	public static string EnumPrincipalsForMailProfile => Keys.GetString("EnumPrincipalsForMailProfile");

	public static string AddLoginToProxyAccount => Keys.GetString("AddLoginToProxyAccount");

	public static string RemoveLoginFromProxyAccount => Keys.GetString("RemoveLoginFromProxyAccount");

	public static string EnumLoginsOfProxyAccount => Keys.GetString("EnumLoginsOfProxyAccount");

	public static string AddServerRoleToProxyAccount => Keys.GetString("AddServerRoleToProxyAccount");

	public static string RemoveServerRoleFromProxyAccount => Keys.GetString("RemoveServerRoleFromProxyAccount");

	public static string EnumServerRolesOfProxyAccount => Keys.GetString("EnumServerRolesOfProxyAccount");

	public static string AddMSDBRoleToProxyAccount => Keys.GetString("AddMSDBRoleToProxyAccount");

	public static string RemoveMSDBRoleFromProxyAccount => Keys.GetString("RemoveMSDBRoleFromProxyAccount");

	public static string EnumMSDBRolesOfProxyAccount => Keys.GetString("EnumMSDBRolesOfProxyAccount");

	public static string MsxDefect => Keys.GetString("MsxDefect");

	public static string MsxEnlist => Keys.GetString("MsxEnlist");

	public static string PurgeJobHistory => Keys.GetString("PurgeJobHistory");

	public static string ReassignJobsByLogin => Keys.GetString("ReassignJobsByLogin");

	public static string CreateRestorePlan => Keys.GetString("CreateRestorePlan");

	public static string DropJobsByLogin => Keys.GetString("DropJobsByLogin");

	public static string StartMonitor => Keys.GetString("StartMonitor");

	public static string StopMonitor => Keys.GetString("StopMonitor");

	public static string EnumProxies => Keys.GetString("EnumProxies");

	public static string DropJobsByServer => Keys.GetString("DropJobsByServer");

	public static string CompareUrn => Keys.GetString("CompareUrn");

	public static string Disable => Keys.GetString("Disable");

	public static string DisableAllIndexes => Keys.GetString("DisableAllIndexes");

	public static string EnableAllIndexes => Keys.GetString("EnableAllIndexes");

	public static string DiscoverDependencies => Keys.GetString("DiscoverDependencies");

	public static string DropBackupHistory => Keys.GetString("DropBackupHistory");

	public static string ChangeMirroringState => Keys.GetString("ChangeMirroringState");

	public static string IsMember => Keys.GetString("IsMember");

	public static string Recreate => Keys.GetString("Recreate");

	public static string Enable => Keys.GetString("Enable");

	public static string EnumColumns => Keys.GetString("EnumColumns");

	public static string EnumForeignKeys => Keys.GetString("EnumForeignKeys");

	public static string EnumIndexes => Keys.GetString("EnumIndexes");

	public static string EnumFragmentation => Keys.GetString("EnumFragmentation");

	public static string EnumReferences => Keys.GetString("EnumReferences");

	public static string SetOffline => Keys.GetString("SetOffline");

	public static string SetEncryption => Keys.GetString("SetEncryption");

	public static string SetOwner => Keys.GetString("SetOwner");

	public static string StartPopulation => Keys.GetString("StartPopulation");

	public static string StopPopulation => Keys.GetString("StopPopulation");

	public static string Rebuild => Keys.GetString("Rebuild");

	public static string Reorganize => Keys.GetString("Reorganize");

	public static string UpdateStatistics => Keys.GetString("UpdateStatistics");

	public static string SetHostLoginAccount => Keys.GetString("SetHostLoginAccount");

	public static string SetMailServerAccount => Keys.GetString("SetMailServerAccount");

	public static string SetMailServerPassword => Keys.GetString("SetMailServerPassword");

	public static string EnumLastStatisticsUpdates => Keys.GetString("EnumLastStatisticsUpdates");

	public static string RebuildIndexes => Keys.GetString("RebuildIndexes");

	public static string ResumeIndexes => Keys.GetString("ResumeIndexes");

	public static string AbortIndexes => Keys.GetString("AbortIndexes");

	public static string PauseIndexes => Keys.GetString("PauseIndexes");

	public static string ReCompileReferences => Keys.GetString("ReCompileReferences");

	public static string TruncateData => Keys.GetString("TruncateData");

	public static string TruncateLog => Keys.GetString("TruncateLog");

	public static string TruncatePartitionsNotSupported => Keys.GetString("TruncatePartitionsNotSupported");

	public static string SwitchPartition => Keys.GetString("SwitchPartition");

	public static string MergeHashPartition => Keys.GetString("MergeHashPartition");

	public static string MergeRangePartition => Keys.GetString("MergeRangePartition");

	public static string SplitHashPartition => Keys.GetString("SplitHashPartition");

	public static string SplitRangePartition => Keys.GetString("SplitRangePartition");

	public static string GetRangeValues => Keys.GetString("GetRangeValues");

	public static string ResetNextUsed => Keys.GetString("ResetNextUsed");

	public static string GetFileGroups => Keys.GetString("GetFileGroups");

	public static string GetDefaultInitFields => Keys.GetString("GetDefaultInitFields");

	public static string SetDefaultInitFields => Keys.GetString("SetDefaultInitFields");

	public static string GetPropertyNames => Keys.GetString("GetPropertyNames");

	public static string SetParent => Keys.GetString("SetParent");

	public static string InitObject => Keys.GetString("InitObject");

	public static string SetName => Keys.GetString("SetName");

	public static string SetNamespace => Keys.GetString("SetNamespace");

	public static string SetSchema => Keys.GetString("SetSchema");

	public static string ExecuteNonQuery => Keys.GetString("ExecuteNonQuery");

	public static string SetSnapshotIsolation => Keys.GetString("SetSnapshotIsolation");

	public static string AlterDatabaseScopedConfiguration => Keys.GetString("AlterDatabaseScopedConfiguration");

	public static string EnumNamespaces => Keys.GetString("EnumNamespaces");

	public static string EnumTypes => Keys.GetString("EnumTypes");

	public static string AddSchemaDocument => Keys.GetString("AddSchemaDocument");

	public static string ScriptTransfer => Keys.GetString("ScriptTransfer");

	public static string SetIdentityPhrase => Keys.GetString("SetIdentityPhrase");

	public static string SetEncryptionOptions => Keys.GetString("SetEncryptionOptions");

	public static string EnumStatistics => Keys.GetString("EnumStatistics");

	public static string GetJobByID => Keys.GetString("GetJobByID");

	public static string RemoveJobByID => Keys.GetString("RemoveJobByID");

	public static string RemoveJobsByLogin => Keys.GetString("RemoveJobsByLogin");

	public static string EnumCandidateKeys => Keys.GetString("EnumCandidateKeys");

	public static string ExecuteWithResults => Keys.GetString("ExecuteWithResults");

	public static string UpdateIndexStatistics => Keys.GetString("UpdateIndexStatistics");

	public static string EnumMatchingSPs => Keys.GetString("EnumMatchingSPs");

	public static string EnumObjects => Keys.GetString("EnumObjects");

	public static string ReadBackupHeader => Keys.GetString("ReadBackupHeader");

	public static string ReadMediaHeader => Keys.GetString("ReadMediaHeader");

	public static string DetachedDatabaseInfo => Keys.GetString("DetachedDatabaseInfo");

	public static string IsDetachedPrimaryFile => Keys.GetString("IsDetachedPrimaryFile");

	public static string IsWindowsGroupMember => Keys.GetString("IsWindowsGroupMember");

	public static string EnumDetachedDatabaseFiles => Keys.GetString("EnumDetachedDatabaseFiles");

	public static string EnumDetachedLogFiles => Keys.GetString("EnumDetachedLogFiles");

	public static string ServerEnumMembers => Keys.GetString("ServerEnumMembers");

	public static string Contains => Keys.GetString("Contains");

	public static string PingSqlServerVersion => Keys.GetString("PingSqlServerVersion");

	public static string SetServiceAccount => Keys.GetString("SetServiceAccount");

	public static string ChangeServicePassword => Keys.GetString("ChangeServicePassword");

	public static string ServerActiveDirectoryRegister => Keys.GetString("ServerActiveDirectoryRegister");

	public static string ServerActiveDirectoryUpdateRegistration => Keys.GetString("ServerActiveDirectoryUpdateRegistration");

	public static string ServerActiveDirectoryUnregister => Keys.GetString("ServerActiveDirectoryUnregister");

	public static string DatabaseActiveDirectoryRegister => Keys.GetString("DatabaseActiveDirectoryRegister");

	public static string DatabaseActiveDirectoryUpdateRegistration => Keys.GetString("DatabaseActiveDirectoryUpdateRegistration");

	public static string DatabaseActiveDirectoryUnregister => Keys.GetString("DatabaseActiveDirectoryUnregister");

	public static string RecoverMasterKey => Keys.GetString("RecoverMasterKey");

	public static string RegenerateMasterKey => Keys.GetString("RegenerateMasterKey");

	public static string ImportMasterKey => Keys.GetString("ImportMasterKey");

	public static string ExportMasterKey => Keys.GetString("ExportMasterKey");

	public static string ChangeAcctMasterKey => Keys.GetString("ChangeAcctMasterKey");

	public static string AddEncryptionMasterKey => Keys.GetString("AddEncryptionMasterKey");

	public static string DropEncryptionMasterKey => Keys.GetString("DropEncryptionMasterKey");

	public static string Close => Keys.GetString("Close");

	public static string Open => Keys.GetString("Open");

	public static string EnumKeyEncryptions => Keys.GetString("EnumKeyEncryptions");

	public static string Compare => Keys.GetString("Compare");

	public static string Insert => Keys.GetString("Insert");

	public static string AddRange => Keys.GetString("AddRange");

	public static string SetRange => Keys.GetString("SetRange");

	public static string AddDevice => Keys.GetString("AddDevice");

	public static string SetMirrors => Keys.GetString("SetMirrors");

	public static string SetDatabase => Keys.GetString("SetDatabase");

	public static string SqlManagement => Keys.GetString("SqlManagement");

	public static string EnumEncryptionAlgorithms => Keys.GetString("EnumEncryptionAlgorithms");

	public static string EnumProviderKeys => Keys.GetString("EnumProviderKeys");

	public static string SetIpAddress => Keys.GetString("SetIpAddress");

	public static string SetSubnetIp => Keys.GetString("SetSubnetIp");

	public static string SetSubnetMask => Keys.GetString("SetSubnetMask");

	public static string GetDHCPAddress => Keys.GetString("GetDHCPAddress");

	public static string GetQueryStoreOptions => Keys.GetString("GetQueryStoreOptions");

	public static string SetQueryStoreOptions => Keys.GetString("SetQueryStoreOptions");

	public static string InvalidQueryStoreOptions => Keys.GetString("InvalidQueryStoreOptions");

	public static string ReauthorizeRemoteDataArchive => Keys.GetString("ReauthorizeRemoteDataArchive");

	public static string GetRemoteDataArchiveMigrationStatusReports => Keys.GetString("GetRemoteDataArchiveMigrationStatusReports");

	public static string GetRemoteDatabaseMigrationStatistics => Keys.GetString("GetRemoteDatabaseMigrationStatistics");

	public static string GetRemoteTableMigrationStatistics => Keys.GetString("GetRemoteTableMigrationStatistics");

	public static string Table => Keys.GetString("Table");

	public static string FileTable => Keys.GetString("FileTable");

	public static string View => Keys.GetString("View");

	public static string Server => Keys.GetString("Server");

	public static string Database => Keys.GetString("Database");

	public static string ExtendedProperty => Keys.GetString("ExtendedProperty");

	public static string DatabaseOptions => Keys.GetString("DatabaseOptions");

	public static string Synonym => Keys.GetString("Synonym");

	public static string Sequence => Keys.GetString("Sequence");

	public static string FullTextIndex => Keys.GetString("FullTextIndex");

	public static string FullTextIndexColumn => Keys.GetString("FullTextIndexColumn");

	public static string Check => Keys.GetString("Check");

	public static string ForeignKey => Keys.GetString("ForeignKey");

	public static string ForeignKeyColumn => Keys.GetString("ForeignKeyColumn");

	public static string PartitionSchemeParameter => Keys.GetString("PartitionSchemeParameter");

	public static string PlanGuide => Keys.GetString("PlanGuide");

	public static string Trigger => Keys.GetString("Trigger");

	public static string Index => Keys.GetString("Index");

	public static string BrokerPriority => Keys.GetString("BrokerPriority");

	public static string IndexedColumn => Keys.GetString("IndexedColumn");

	public static string Statistic => Keys.GetString("Statistic");

	public static string StatisticColumn => Keys.GetString("StatisticColumn");

	public static string Column => Keys.GetString("Column");

	public static string DefaultConstraint => Keys.GetString("DefaultConstraint");

	public static string StoredProcedure => Keys.GetString("StoredProcedure");

	public static string StoredProcedureParameter => Keys.GetString("StoredProcedureParameter");

	public static string SqlAssembly => Keys.GetString("SqlAssembly");

	public static string SqlAssemblyFile => Keys.GetString("SqlAssemblyFile");

	public static string UserDefinedType => Keys.GetString("UserDefinedType");

	public static string UserDefinedAggregate => Keys.GetString("UserDefinedAggregate");

	public static string UserDefinedAggregateParameter => Keys.GetString("UserDefinedAggregateParameter");

	public static string FullTextCatalog => Keys.GetString("FullTextCatalog");

	public static string FullTextStopList => Keys.GetString("FullTextStopList");

	public static string SearchPropertyList => Keys.GetString("SearchPropertyList");

	public static string ExtendedStoredProcedure => Keys.GetString("ExtendedStoredProcedure");

	public static string UserDefinedFunction => Keys.GetString("UserDefinedFunction");

	public static string UserDefinedFunctionParameter => Keys.GetString("UserDefinedFunctionParameter");

	public static string User => Keys.GetString("User");

	public static string Schema => Keys.GetString("Schema");

	public static string DatabaseRole => Keys.GetString("DatabaseRole");

	public static string ApplicationRole => Keys.GetString("ApplicationRole");

	public static string LogFile => Keys.GetString("LogFile");

	public static string FileGroup => Keys.GetString("FileGroup");

	public static string DataFile => Keys.GetString("DataFile");

	public static string Default => Keys.GetString("Default");

	public static string Rule => Keys.GetString("Rule");

	public static string UserDefinedDataType => Keys.GetString("UserDefinedDataType");

	public static string UserDefinedTableType => Keys.GetString("UserDefinedTableType");

	public static string PartitionFunction => Keys.GetString("PartitionFunction");

	public static string PartitionScheme => Keys.GetString("PartitionScheme");

	public static string DatabaseActiveDirectory => Keys.GetString("DatabaseActiveDirectory");

	public static string Language => Keys.GetString("Language");

	public static string Login => Keys.GetString("Login");

	public static string ServerRole => Keys.GetString("ServerRole");

	public static string LinkedServer => Keys.GetString("LinkedServer");

	public static string LinkedServerLogin => Keys.GetString("LinkedServerLogin");

	public static string SystemDataType => Keys.GetString("SystemDataType");

	public static string JobServer => Keys.GetString("JobServer");

	public static string Category => Keys.GetString("Category");

	public static string AlertSystem => Keys.GetString("AlertSystem");

	public static string Alert => Keys.GetString("Alert");

	public static string Operator => Keys.GetString("Operator");

	public static string TargetServer => Keys.GetString("TargetServer");

	public static string TargetServerGroup => Keys.GetString("TargetServerGroup");

	public static string Job => Keys.GetString("Job");

	public static string JobStep => Keys.GetString("JobStep");

	public static string JobSchedule => Keys.GetString("JobSchedule");

	public static string Settings => Keys.GetString("Settings");

	public static string Information => Keys.GetString("Information");

	public static string UserOptions => Keys.GetString("UserOptions");

	public static string BackupDevice => Keys.GetString("BackupDevice");

	public static string FullTextService => Keys.GetString("FullTextService");

	public static string ServerActiveDirectory => Keys.GetString("ServerActiveDirectory");

	public static string HttpEndpoint => Keys.GetString("HttpEndpoint");

	public static string SoapConfiguration => Keys.GetString("SoapConfiguration");

	public static string SoapMethod => Keys.GetString("SoapMethod");

	public static string ServerAlias => Keys.GetString("ServerAlias");

	public static string PhysicalPartition => Keys.GetString("PhysicalPartition");

	public static string Audit => Keys.GetString("Audit");

	public static string ServerAuditSpecification => Keys.GetString("ServerAuditSpecification");

	public static string DatabaseAuditSpecification => Keys.GetString("DatabaseAuditSpecification");

	public static string ManagedComputer => Keys.GetString("ManagedComputer");

	public static string Service => Keys.GetString("Service");

	public static string XmlSchemaCollection => Keys.GetString("XmlSchemaCollection");

	public static string DatabaseEncryptionKey => Keys.GetString("DatabaseEncryptionKey");

	public static string Restore => Keys.GetString("Restore");

	public static string RestoreAsync => Keys.GetString("RestoreAsync");

	public static string EnumAvailableSqlServers => Keys.GetString("EnumAvailableSqlServers");

	public static string GetDataType => Keys.GetString("GetDataType");

	public static string SetDataType => Keys.GetString("SetDataType");

	public static string Backup => Keys.GetString("Backup");

	public static string AvailabilityGroup => Keys.GetString("AvailabilityGroup");

	public static string AvailabilityReplica => Keys.GetString("AvailabilityReplica");

	public static string AvailabilityDatabase => Keys.GetString("AvailabilityDatabase");

	public static string AvailabilityGroupListener => Keys.GetString("AvailabilityGroupListener");

	public static string AvailabilityGroupListenerIPAddress => Keys.GetString("AvailabilityGroupListenerIPAddress");

	public static string SecurityPolicy => Keys.GetString("SecurityPolicy");

	public static string SecurityPredicate => Keys.GetString("SecurityPredicate");

	public static string ExternalDataSource => Keys.GetString("ExternalDataSource");

	public static string ExternalFileFormat => Keys.GetString("ExternalFileFormat");

	public static string ColumnMasterKey => Keys.GetString("ColumnMasterKey");

	public static string SmoSQLCLRUnAvailable => Keys.GetString("SmoSQLCLRUnAvailable");

	public static string MoveToPool => Keys.GetString("MoveToPool");

	public static string ResourcePoolNotExist => Keys.GetString("ResourcePoolNotExist");

	public static string CannotMoveToInternalResourcePool => Keys.GetString("CannotMoveToInternalResourcePool");

	public static string CannotMoveToSamePool => Keys.GetString("CannotMoveToSamePool");

	public static string AffinityTypeCannotBeSet => Keys.GetString("AffinityTypeCannotBeSet");

	public static string AffinityValueCannotBeSet => Keys.GetString("AffinityValueCannotBeSet");

	public static string NoCPUAffinitized => Keys.GetString("NoCPUAffinitized");

	public static string ResourceGovernorPoolMissing => Keys.GetString("ResourceGovernorPoolMissing");

	public static string CannotSwitchDesignModeOff => Keys.GetString("CannotSwitchDesignModeOff");

	public static string ServerVersionNotSpecified => Keys.GetString("ServerVersionNotSpecified");

	public static string OperationNotAvailableInDesignMode => Keys.GetString("OperationNotAvailableInDesignMode");

	public static string OnlyDesignModeSupported => Keys.GetString("OnlyDesignModeSupported");

	public static string OnlySmoObjectsSupported => Keys.GetString("OnlySmoObjectsSupported");

	public static string RootNotFound => Keys.GetString("RootNotFound");

	public static string FileWritingException => Keys.GetString("FileWritingException");

	public static string InvalideFileName => Keys.GetString("InvalideFileName");

	public static string FolderPathNotFound => Keys.GetString("FolderPathNotFound");

	public static string FilePerObjectUrnMissingName => Keys.GetString("FilePerObjectUrnMissingName");

	public static string OrderingCycleDetected => Keys.GetString("OrderingCycleDetected");

	public static string IncorrectEndpointProtocol => Keys.GetString("IncorrectEndpointProtocol");

	public static string FileTableCannotHaveUserColumns => Keys.GetString("FileTableCannotHaveUserColumns");

	public static string ExternalTableCannotContainChecks => Keys.GetString("ExternalTableCannotContainChecks");

	public static string ExternalTableCannotContainForeignKeys => Keys.GetString("ExternalTableCannotContainForeignKeys");

	public static string ExternalTableCannotContainPartitionSchemeParameters => Keys.GetString("ExternalTableCannotContainPartitionSchemeParameters");

	public static string ExternalTableCannotContainTriggers => Keys.GetString("ExternalTableCannotContainTriggers");

	public static string ExternalTableCannotContainIndexes => Keys.GetString("ExternalTableCannotContainIndexes");

	public static string ExternalTableCannotContainPhysicalPartitions => Keys.GetString("ExternalTableCannotContainPhysicalPartitions");

	public static string IdentityColumnForExternalTable => Keys.GetString("IdentityColumnForExternalTable");

	public static string TruncateOperationNotSupportedOnExternalTables => Keys.GetString("TruncateOperationNotSupportedOnExternalTables");

	public static string ChangeTrackingNotSupportedOnExternalTables => Keys.GetString("ChangeTrackingNotSupportedOnExternalTables");

	public static string ColumnStoreCompressionNotSettable => Keys.GetString("ColumnStoreCompressionNotSettable");

	public static string NoIndexUnique => Keys.GetString("NoIndexUnique");

	public static string NoIndexIgnoreDupKey => Keys.GetString("NoIndexIgnoreDupKey");

	public static string NoIndexClustered => Keys.GetString("NoIndexClustered");

	public static string InvaildColumnStoreIndexOption => Keys.GetString("InvaildColumnStoreIndexOption");

	public static string IndexOnTableView => Keys.GetString("IndexOnTableView");

	public static string IncludedColumnNotSupported => Keys.GetString("IncludedColumnNotSupported");

	public static string ConflictingIndexProperties => Keys.GetString("ConflictingIndexProperties");

	public static string SelectiveXmlIndexDoesNotSupportReorganize => Keys.GetString("SelectiveXmlIndexDoesNotSupportReorganize");

	public static string MoreThenOneXmlDefaultNamespace => Keys.GetString("MoreThenOneXmlDefaultNamespace");

	public static string SecondarySelectiveXmlIndexModify => Keys.GetString("SecondarySelectiveXmlIndexModify");

	public static string InvaildSXIOption => Keys.GetString("InvaildSXIOption");

	public static string InvalidUpgradeToCCIIndexType => Keys.GetString("InvalidUpgradeToCCIIndexType");

	public static string PropertyValidOnlyForColumnStoreIndexes => Keys.GetString("PropertyValidOnlyForColumnStoreIndexes");

	public static string InvalidCompressionDelayValue => Keys.GetString("InvalidCompressionDelayValue");

	public static string TransferDataException => Keys.GetString("TransferDataException");

	public static string ColumnCollationIncompatible => Keys.GetString("ColumnCollationIncompatible");

	public static string ConflictingExternalFileFormatProperties => Keys.GetString("ConflictingExternalFileFormatProperties");

	public static string UnsupportedPropertyForAlter => Keys.GetString("UnsupportedPropertyForAlter");

	public static string UnsupportedResourceManagerLocationProperty => Keys.GetString("UnsupportedResourceManagerLocationProperty");

	public static string UnsupportedParamForDataSourceType => Keys.GetString("UnsupportedParamForDataSourceType");

	public static string AlterNotSupportedForRelationalTypes => Keys.GetString("AlterNotSupportedForRelationalTypes");

	public static string InvalidTextForModifyingToCreateOrAlter => Keys.GetString("InvalidTextForModifyingToCreateOrAlter");

	public static string ExpectedGraphColumnNotFound => Keys.GetString("ExpectedGraphColumnNotFound");

	protected ExceptionTemplatesImpl()
	{
	}

	public static string InvalidSequenceValue(string propertyName)
	{
		return Keys.GetString("InvalidSequenceValue", propertyName);
	}

	public static string CannotSetPrivilege(string privName)
	{
		return Keys.GetString("CannotSetPrivilege", privName);
	}

	public static string UsersWithoutLoginsDownLevel(string targetVersion)
	{
		return Keys.GetString("UsersWithoutLoginsDownLevel", targetVersion);
	}

	public static string EncryptedUserDefinedFunctionsDownlevel(string udf, string targetVersion)
	{
		return Keys.GetString("EncryptedUserDefinedFunctionsDownlevel", udf, targetVersion);
	}

	public static string EncryptedStoredProcedureDownlevel(string sproc, string targetVersion)
	{
		return Keys.GetString("EncryptedStoredProcedureDownlevel", sproc, targetVersion);
	}

	public static string EncryptedViewsFunctionsDownlevel(string view, string targetVersion)
	{
		return Keys.GetString("EncryptedViewsFunctionsDownlevel", view, targetVersion);
	}

	public static string SchemaDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("SchemaDownlevel", objectName, targetVersion);
	}

	public static string UserDefinedAggregatesDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("UserDefinedAggregatesDownlevel", objectName, targetVersion);
	}

	public static string XmlSchemaCollectionDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("XmlSchemaCollectionDownlevel", objectName, targetVersion);
	}

	public static string SynonymDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("SynonymDownlevel", objectName, targetVersion);
	}

	public static string SequenceDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("SequenceDownlevel", objectName, targetVersion);
	}

	public static string SecurityPolicyDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("SecurityPolicyDownlevel", objectName, targetVersion);
	}

	public static string ExternalDataSourceDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("ExternalDataSourceDownlevel", objectName, targetVersion);
	}

	public static string ColumnEncryptionKeyDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("ColumnEncryptionKeyDownlevel", objectName, targetVersion);
	}

	public static string ExternalFileFormatDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("ExternalFileFormatDownlevel", objectName, targetVersion);
	}

	public static string ColumnMasterKeyDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("ColumnMasterKeyDownlevel", objectName, targetVersion);
	}

	public static string DatabaseScopedCredentialDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("DatabaseScopedCredentialDownlevel", objectName, targetVersion);
	}

	public static string UserDefinedTableDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("UserDefinedTableDownlevel", objectName, targetVersion);
	}

	public static string DdlTriggerDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("DdlTriggerDownlevel", objectName, targetVersion);
	}

	public static string ClrUserDefinedFunctionDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("ClrUserDefinedFunctionDownlevel", objectName, targetVersion);
	}

	public static string ClrStoredProcedureDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("ClrStoredProcedureDownlevel", objectName, targetVersion);
	}

	public static string AssemblyDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("AssemblyDownlevel", objectName, targetVersion);
	}

	public static string ColumnEncryptionKeyNoValues(string objectName)
	{
		return Keys.GetString("ColumnEncryptionKeyNoValues", objectName);
	}

	public static string SecurityPolicyNoPredicates(string objectName)
	{
		return Keys.GetString("SecurityPolicyNoPredicates", objectName);
	}

	public static string ComputedColumnDownlevelContraint(string objectName, string targetVersion)
	{
		return Keys.GetString("ComputedColumnDownlevelContraint", objectName, targetVersion);
	}

	public static string DataScriptingUnsupportedDataTypeException(string tableName, string columnName, string dataType)
	{
		return Keys.GetString("DataScriptingUnsupportedDataTypeException", tableName, columnName, dataType);
	}

	public static string StoredProcedureDownlevelExecutionContext(string objectName, string executionPrincipal, string targetVersion)
	{
		return Keys.GetString("StoredProcedureDownlevelExecutionContext", objectName, executionPrincipal, targetVersion);
	}

	public static string UserDefinedFunctionDownlevelExecutionContext(string objectName, string executionPrincipal, string targetVersion)
	{
		return Keys.GetString("UserDefinedFunctionDownlevelExecutionContext", objectName, executionPrincipal, targetVersion);
	}

	public static string TriggerDownlevelExecutionContext(string objectName, string executionPrincipal, string targetVersion)
	{
		return Keys.GetString("TriggerDownlevelExecutionContext", objectName, executionPrincipal, targetVersion);
	}

	public static string UnsupportedColumnCollation(string columnName, string parentName, string collationName, string targetVersion)
	{
		return Keys.GetString("UnsupportedColumnCollation", columnName, parentName, collationName, targetVersion);
	}

	public static string UnsupportedDatabaseCollation(string collationName, string targetVersion)
	{
		return Keys.GetString("UnsupportedDatabaseCollation", collationName, targetVersion);
	}

	public static string UnsupportedColumnType(string parentName, string columnName, string columnDataType, string targetVersion)
	{
		return Keys.GetString("UnsupportedColumnType", parentName, columnName, columnDataType, targetVersion);
	}

	public static string UnsupportedColumnTypeOnEngineType(string parentName, string columnName, string columnDataType, string engineType)
	{
		return Keys.GetString("UnsupportedColumnTypeOnEngineType", parentName, columnName, columnDataType, engineType);
	}

	public static string CollectionNotAvailable(string objectname, string serverversion)
	{
		return Keys.GetString("CollectionNotAvailable", objectname, serverversion);
	}

	public static string CreateOrAlterDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("CreateOrAlterDownlevel", objectName, targetVersion);
	}

	public static string CreateOrAlterNotSupported(string objectName)
	{
		return Keys.GetString("CreateOrAlterNotSupported", objectName);
	}

	public static string TriggerNotSupported(string serverversion)
	{
		return Keys.GetString("TriggerNotSupported", serverversion);
	}

	public static string UnknownRecoveryModel(string modelname)
	{
		return Keys.GetString("UnknownRecoveryModel", modelname);
	}

	public static string UnknownUserAccess(string modelname)
	{
		return Keys.GetString("UnknownUserAccess", modelname);
	}

	public static string CannotRenameObject(string objectname, string serverversion)
	{
		return Keys.GetString("CannotRenameObject", objectname, serverversion);
	}

	public static string PropertyNotSet(string propname, string objtype)
	{
		return Keys.GetString("PropertyNotSet", propname, objtype);
	}

	public static string ColumnAlreadyHasDefault(string name)
	{
		return Keys.GetString("ColumnAlreadyHasDefault", name);
	}

	public static string ColumnHasNoDefault(string colname, string defname)
	{
		return Keys.GetString("ColumnHasNoDefault", colname, defname);
	}

	public static string MismatchingServerName(string srvname, string urnsrvname)
	{
		return Keys.GetString("MismatchingServerName", srvname, urnsrvname);
	}

	public static string MismatchingNumberOfMirrors(int i1, int i2)
	{
		return Keys.GetString("MismatchingNumberOfMirrors", i1, i2);
	}

	public static string BackupToPipesNotSupported(string serverversion)
	{
		return Keys.GetString("BackupToPipesNotSupported", serverversion);
	}

	public static string BackupToUrlNotSupported(string serverVersion, string supportedVersion)
	{
		return Keys.GetString("BackupToUrlNotSupported", serverVersion, supportedVersion);
	}

	public static string CredentialNotSupportedError(string credential, string serverVersion, string supportedVersion)
	{
		return Keys.GetString("CredentialNotSupportedError", credential, serverVersion, supportedVersion);
	}

	public static string LoginHasUser(string name, string dbname)
	{
		return Keys.GetString("LoginHasUser", name, dbname);
	}

	public static string LoginHasAlias(string name)
	{
		return Keys.GetString("LoginHasAlias", name);
	}

	public static string InvalidLogin(string loginname)
	{
		return Keys.GetString("InvalidLogin", loginname);
	}

	public static string InvalidShrinkMethod(string name)
	{
		return Keys.GetString("InvalidShrinkMethod", name);
	}

	public static string ObjectRefsNonexCol(string objname, string key, string col)
	{
		return Keys.GetString("ObjectRefsNonexCol", objname, key, col);
	}

	public static string OrderHintRefsNonexCol(string key, string col)
	{
		return Keys.GetString("OrderHintRefsNonexCol", key, col);
	}

	public static string CannotCopyPartition(int startPartition, int endPartition)
	{
		return Keys.GetString("CannotCopyPartition", startPartition, endPartition);
	}

	public static string PartitionNumberStartOutOfRange(int maxvalue)
	{
		return Keys.GetString("PartitionNumberStartOutOfRange", maxvalue);
	}

	public static string RebuildHeapError(string msg)
	{
		return Keys.GetString("RebuildHeapError", msg);
	}

	public static string PartitionSchemeNotAssignedError(string msg)
	{
		return Keys.GetString("PartitionSchemeNotAssignedError", msg);
	}

	public static string CannotAddObject(string typename, string objectName)
	{
		return Keys.GetString("CannotAddObject", typename, objectName);
	}

	public static string UnknownObjectType(string name)
	{
		return Keys.GetString("UnknownObjectType", name);
	}

	public static string UnknownProperty(string name, string typename)
	{
		return Keys.GetString("UnknownProperty", name, typename);
	}

	public static string ObjectNotUnderServer(string name)
	{
		return Keys.GetString("ObjectNotUnderServer", name);
	}

	public static string UnknownChildCollection(string objtypename, string childname)
	{
		return Keys.GetString("UnknownChildCollection", objtypename, childname);
	}

	public static string CantCreateType(string name)
	{
		return Keys.GetString("CantCreateType", name);
	}

	public static string ObjectAlreadyExists(string type, string name)
	{
		return Keys.GetString("ObjectAlreadyExists", type, name);
	}

	public static string NoSqlGen(string objname)
	{
		return Keys.GetString("NoSqlGen", objname);
	}

	public static string InvalidType(string typename)
	{
		return Keys.GetString("InvalidType", typename);
	}

	public static string WrongUrn(string objectType, string urn)
	{
		return Keys.GetString("WrongUrn", objectType, urn);
	}

	public static string NoDepForSysObjects(string objUrn)
	{
		return Keys.GetString("NoDepForSysObjects", objUrn);
	}

	public static string UrnMissing(string urn)
	{
		return Keys.GetString("UrnMissing", urn);
	}

	public static string ObjectWithNoChildren(string objectName, string childName)
	{
		return Keys.GetString("ObjectWithNoChildren", objectName, childName);
	}

	public static string ColumnBeforeNotExisting(string colname)
	{
		return Keys.GetString("ColumnBeforeNotExisting", colname);
	}

	public static string PropNotModifiable(string propName, string objectType)
	{
		return Keys.GetString("PropNotModifiable", propName, objectType);
	}

	public static string UnsupportedLoginMode(string loginmode)
	{
		return Keys.GetString("UnsupportedLoginMode", loginmode);
	}

	public static string CannotSetDefInitFlds(string typename)
	{
		return Keys.GetString("CannotSetDefInitFlds", typename);
	}

	public static string ObjectWithMoreChildren(string objectName, string childName)
	{
		return Keys.GetString("ObjectWithMoreChildren", objectName, childName);
	}

	public static string WrongHybridIPAddresses(string objectName)
	{
		return Keys.GetString("WrongHybridIPAddresses", objectName);
	}

	public static string WrongDHCPv6IPAddress(string objectName)
	{
		return Keys.GetString("WrongDHCPv6IPAddress", objectName);
	}

	public static string WrongMultiDHCPIPAddresses(string objectName)
	{
		return Keys.GetString("WrongMultiDHCPIPAddresses", objectName);
	}

	public static string GetDHCPAddressFailed(string listenerName, int count)
	{
		return Keys.GetString("GetDHCPAddressFailed", listenerName, count);
	}

	public static string CannotAddDHCPIPAddress(string objectName, string propName)
	{
		return Keys.GetString("CannotAddDHCPIPAddress", objectName, propName);
	}

	public static string SearchPropertyListNameNotValid(int maxLength)
	{
		return Keys.GetString("SearchPropertyListNameNotValid", maxLength);
	}

	public static string SearchPropertyNameNotValid(int maxLength)
	{
		return Keys.GetString("SearchPropertyNameNotValid", maxLength);
	}

	public static string SearchPropertyDescriptionNotValid(int maxLength)
	{
		return Keys.GetString("SearchPropertyDescriptionNotValid", maxLength);
	}

	public static string BackupMediaSetNotComplete(string files, int familyCount, int missing)
	{
		return Keys.GetString("BackupMediaSetNotComplete", files, familyCount, missing);
	}

	public static string UnableToReadDevice(string backupDeviceName)
	{
		return Keys.GetString("UnableToReadDevice", backupDeviceName);
	}

	public static string BackupFileAlreadyExists(string backupFile)
	{
		return Keys.GetString("BackupFileAlreadyExists", backupFile);
	}

	public static string Restoring(string backupset)
	{
		return Keys.GetString("Restoring", backupset);
	}

	public static string BackupFileNotFound(string file)
	{
		return Keys.GetString("BackupFileNotFound", file);
	}

	public static string CannotRestoreFileBootPage(int fileid, long pageid)
	{
		return Keys.GetString("CannotRestoreFileBootPage", fileid, pageid);
	}

	public static string CannotRestoreDatabaseBootPage(int fileid, long pageid)
	{
		return Keys.GetString("CannotRestoreDatabaseBootPage", fileid, pageid);
	}

	public static string DuplicateSuspectPage(int fileId, long pageId)
	{
		return Keys.GetString("DuplicateSuspectPage", fileId, pageId);
	}

	public static string InvalidPathChildCollectionNotFound(string child, string parent)
	{
		return Keys.GetString("InvalidPathChildCollectionNotFound", child, parent);
	}

	public static string InvalidPathChildSingletonNotFound(string child, string parent)
	{
		return Keys.GetString("InvalidPathChildSingletonNotFound", child, parent);
	}

	public static string WMIException(string errorCode)
	{
		return Keys.GetString("WMIException", errorCode);
	}

	public static string CallingInitChildLevelWithWrongUrn(string urn)
	{
		return Keys.GetString("CallingInitChildLevelWithWrongUrn", urn);
	}

	public static string CallingInitQueryUrnsWithWrongUrn(string urn)
	{
		return Keys.GetString("CallingInitQueryUrnsWithWrongUrn", urn);
	}

	public static string UnsupportedObjectQueryUrn(string urn)
	{
		return Keys.GetString("UnsupportedObjectQueryUrn", urn);
	}

	public static string UnsupportedBackupDeviceType(string name)
	{
		return Keys.GetString("UnsupportedBackupDeviceType", name);
	}

	public static string UnsupportedVersion(string version)
	{
		return Keys.GetString("UnsupportedVersion", version);
	}

	public static string UnsupportedCompatLevelException(long targetLevel, long minLevel)
	{
		return Keys.GetString("UnsupportedCompatLevelException", targetLevel, minLevel);
	}

	public static string ObjectDoesNotExist(string type, string name)
	{
		return Keys.GetString("ObjectDoesNotExist", type, name);
	}

	public static string UnsupportedFeature(string type)
	{
		return Keys.GetString("UnsupportedFeature", type);
	}

	public static string NoObjectWithoutColumns(string objName)
	{
		return Keys.GetString("NoObjectWithoutColumns", objName);
	}

	public static string ConflictingScriptingOptions(string opt1, string opt2)
	{
		return Keys.GetString("ConflictingScriptingOptions", opt1, opt2);
	}

	public static string InvalidScriptingOutput(string opt1, string opt2)
	{
		return Keys.GetString("InvalidScriptingOutput", opt1, opt2);
	}

	public static string NotSupportedForCloudVersion(string type, string version)
	{
		return Keys.GetString("NotSupportedForCloudVersion", type, version);
	}

	public static string NotSupportedForSqlDw(string type)
	{
		return Keys.GetString("NotSupportedForSqlDw", type);
	}

	public static string NotSupportedForSqlDb(string type)
	{
		return Keys.GetString("NotSupportedForSqlDb", type);
	}

	public static string NotSupportedOnStandaloneWithDetails(string type)
	{
		return Keys.GetString("NotSupportedOnStandaloneWithDetails", type);
	}

	public static string NotSupportedOnCloudWithDetails(string type)
	{
		return Keys.GetString("NotSupportedOnCloudWithDetails", type);
	}

	public static string ScriptingNotSupportedForSqlDw(string type)
	{
		return Keys.GetString("ScriptingNotSupportedForSqlDw", type);
	}

	public static string ScriptingNotSupportedForSqlDb(string type)
	{
		return Keys.GetString("ScriptingNotSupportedForSqlDb", type);
	}

	public static string ScriptingNotSupportedOnStandalone(string type)
	{
		return Keys.GetString("ScriptingNotSupportedOnStandalone", type);
	}

	public static string ScriptingNotSupportedOnCloud(string type)
	{
		return Keys.GetString("ScriptingNotSupportedOnCloud", type);
	}

	public static string PropertySupportedOnlyOn110SP1(string propertyName)
	{
		return Keys.GetString("PropertySupportedOnlyOn110SP1", propertyName);
	}

	public static string PropertySupportedOnlyOn110(string propertyName)
	{
		return Keys.GetString("PropertySupportedOnlyOn110", propertyName);
	}

	public static string PropertyNotSupportedForCloudVersion(string propertyName, string version)
	{
		return Keys.GetString("PropertyNotSupportedForCloudVersion", propertyName, version);
	}

	public static string PropertyNotSupportedOnStandalone(string propertyName)
	{
		return Keys.GetString("PropertyNotSupportedOnStandalone", propertyName);
	}

	public static string PropertyNotSupportedOnCloud(string propertyName)
	{
		return Keys.GetString("PropertyNotSupportedOnCloud", propertyName);
	}

	public static string PropertyNotSupportedWithDetails(string propertyName, string engineType, string version, string engineEdition)
	{
		return Keys.GetString("PropertyNotSupportedWithDetails", propertyName, engineType, version, engineEdition);
	}

	public static string PropertyValueSupportedOnlyOn110(string propertyName, string propertyValue)
	{
		return Keys.GetString("PropertyValueSupportedOnlyOn110", propertyName, propertyValue);
	}

	public static string PropertyValueNotSupportedForSqlDw(string propertyName, string propertyValue)
	{
		return Keys.GetString("PropertyValueNotSupportedForSqlDw", propertyName, propertyValue);
	}

	public static string PropertyValueNotSupportedForSqlDb(string propertyName, string propertyValue)
	{
		return Keys.GetString("PropertyValueNotSupportedForSqlDb", propertyName, propertyValue);
	}

	public static string WrongPropertyValueException(string propertyName, string propertyValue)
	{
		return Keys.GetString("WrongPropertyValueException", propertyName, propertyValue);
	}

	public static string InvalidPropertyValueForVersion(string typeName, string propertyName, string propertyValue, string sqlVersionName)
	{
		return Keys.GetString("InvalidPropertyValueForVersion", typeName, propertyName, propertyValue, sqlVersionName);
	}

	public static string PropertyCannotBeSetForVersion(string propertyName, string objectType, string version)
	{
		return Keys.GetString("PropertyCannotBeSetForVersion", propertyName, objectType, version);
	}

	public static string EmptyInputParam(string paramName, string paramKind)
	{
		return Keys.GetString("EmptyInputParam", paramName, paramKind);
	}

	public static string MutuallyExclusiveProperties(string prop1, string prop2)
	{
		return Keys.GetString("MutuallyExclusiveProperties", prop1, prop2);
	}

	public static string IndexMustBeClustered(string tableName, string indexName)
	{
		return Keys.GetString("IndexMustBeClustered", tableName, indexName);
	}

	public static string ParentMustExist(string objectType, string objectName)
	{
		return Keys.GetString("ParentMustExist", objectType, objectName);
	}

	public static string OperationNotInPendingState1(string prop1)
	{
		return Keys.GetString("OperationNotInPendingState1", prop1);
	}

	public static string OperationNotInPendingState2(string prop1, string prop2)
	{
		return Keys.GetString("OperationNotInPendingState2", prop1, prop2);
	}

	public static string OperationNotInPendingState3(string prop1, string prop2, string prop3)
	{
		return Keys.GetString("OperationNotInPendingState3", prop1, prop2, prop3);
	}

	public static string UnsupportedDatabaseScopedConfiguration(string configName)
	{
		return Keys.GetString("UnsupportedDatabaseScopedConfiguration", configName);
	}

	public static string SpatialAutoGridDownlevel(string objectName, string targetVersion)
	{
		return Keys.GetString("SpatialAutoGridDownlevel", objectName, targetVersion);
	}

	public static string NoAutoGridWithGrids(string objectName)
	{
		return Keys.GetString("NoAutoGridWithGrids", objectName);
	}

	public static string TableSqlDwIndexTypeRestrictions(string indexType)
	{
		return Keys.GetString("TableSqlDwIndexTypeRestrictions", indexType);
	}

	public static string UnexpectedIndexTypeDetected(string indexType)
	{
		return Keys.GetString("UnexpectedIndexTypeDetected", indexType);
	}

	public static string NoPropertyChangeForDotNet(string propertyName)
	{
		return Keys.GetString("NoPropertyChangeForDotNet", propertyName);
	}

	public static string ClrNotSupported(string propName, string version)
	{
		return Keys.GetString("ClrNotSupported", propName, version);
	}

	public static string InvalidPropertyNumberRange(string propName, string numberMin, string numberMax)
	{
		return Keys.GetString("InvalidPropertyNumberRange", propName, numberMin, numberMax);
	}

	public static string InvalidCollation(string name)
	{
		return Keys.GetString("InvalidCollation", name);
	}

	public static string UnsupportedCollation(string name, string version)
	{
		return Keys.GetString("UnsupportedCollation", name, version);
	}

	public static string NeedToPassObject(string objtype)
	{
		return Keys.GetString("NeedToPassObject", objtype);
	}

	public static string InexistentDir(string dir)
	{
		return Keys.GetString("InexistentDir", dir);
	}

	public static string CantCreateTempFile(string dir)
	{
		return Keys.GetString("CantCreateTempFile", dir);
	}

	public static string UnknownFilter(string filter)
	{
		return Keys.GetString("UnknownFilter", filter);
	}

	public static string MessageIDTooSmall(int id)
	{
		return Keys.GetString("MessageIDTooSmall", id);
	}

	public static string DataTypeUnsupported(string dt)
	{
		return Keys.GetString("DataTypeUnsupported", dt);
	}

	public static string CantSetTypeName(string type)
	{
		return Keys.GetString("CantSetTypeName", type);
	}

	public static string CantSetTypeSchema(string type)
	{
		return Keys.GetString("CantSetTypeSchema", type);
	}

	public static string UnknownSqlDataType(string type)
	{
		return Keys.GetString("UnknownSqlDataType", type);
	}

	public static string NeedExistingObjForDataType(string objName)
	{
		return Keys.GetString("NeedExistingObjForDataType", objName);
	}

	public static string NoPendingObjForDataType(string state)
	{
		return Keys.GetString("NoPendingObjForDataType", state);
	}

	public static string InvalidOptionForVersion(string method, string optionName, string SqlVersionName)
	{
		return Keys.GetString("InvalidOptionForVersion", method, optionName, SqlVersionName);
	}

	public static string InvalidParamForVersion(string method, string paramName, string SqlVersionName)
	{
		return Keys.GetString("InvalidParamForVersion", method, paramName, SqlVersionName);
	}

	public static string UnknownLanguageId(string langid)
	{
		return Keys.GetString("UnknownLanguageId", langid);
	}

	public static string UnknownEnumeration(string type)
	{
		return Keys.GetString("UnknownEnumeration", type);
	}

	public static string UnknownEnumerationWithValue(string type, object value)
	{
		return Keys.GetString("UnknownEnumerationWithValue", type, value);
	}

	public static string MissingConfigVariable(string fnName, string varName)
	{
		return Keys.GetString("MissingConfigVariable", fnName, varName);
	}

	public static string ThreadPoolSizeNotValidForEdition(string componentName, int threadPoolSize)
	{
		return Keys.GetString("ThreadPoolSizeNotValidForEdition", componentName, threadPoolSize);
	}

	public static string InvalidPropertySetForExistingObject(string propertyName)
	{
		return Keys.GetString("InvalidPropertySetForExistingObject", propertyName);
	}

	public static string InvalidEncryptArgumentsAndArgumentKey(string EncryptArguments, string ArgumentKey)
	{
		return Keys.GetString("InvalidEncryptArgumentsAndArgumentKey", EncryptArguments, ArgumentKey);
	}

	public static string InvalidSerializerAdapterFound(string name, string expectedType, string actualType)
	{
		return Keys.GetString("InvalidSerializerAdapterFound", name, expectedType, actualType);
	}

	public static string InvalidConversionError(string name, string input, string type)
	{
		return Keys.GetString("InvalidConversionError", name, input, type);
	}

	public static string RequiredChildMissingFromParent(string requiredChildType, string parentName, string parentType)
	{
		return Keys.GetString("RequiredChildMissingFromParent", requiredChildType, parentName, parentType);
	}

	public static string WrongParent(string objectName)
	{
		return Keys.GetString("WrongParent", objectName);
	}

	public static string VerifyFailed(string database, string backupType)
	{
		return Keys.GetString("VerifyFailed", database, backupType);
	}

	public static string VerifyFailed0(string database)
	{
		return Keys.GetString("VerifyFailed0", database);
	}

	public static string InvalidVersion(string version)
	{
		return Keys.GetString("InvalidVersion", version);
	}

	public static string ConflictingSwitches(string prop1, string prop2)
	{
		return Keys.GetString("ConflictingSwitches", prop1, prop2);
	}

	public static string BadCompatLevel(string level)
	{
		return Keys.GetString("BadCompatLevel", level);
	}

	public static string InvalidAlgorithm(string parent, string argument)
	{
		return Keys.GetString("InvalidAlgorithm", parent, argument);
	}

	public static string IncludeHeader(string objectType, string name, string dateString)
	{
		return Keys.GetString("IncludeHeader", objectType, name, dateString);
	}

	public static string FullPropertyBag(string propName)
	{
		return Keys.GetString("FullPropertyBag", propName);
	}

	public static string MultipleRowsForUrn(string urnName)
	{
		return Keys.GetString("MultipleRowsForUrn", urnName);
	}

	public static string CouldNotFindKey(string keyName)
	{
		return Keys.GetString("CouldNotFindKey", keyName);
	}

	public static string UnsupportedUrnFilter(string attrib, string functionType)
	{
		return Keys.GetString("UnsupportedUrnFilter", attrib, functionType);
	}

	public static string UnsupportedUrnAttrib(string attrib)
	{
		return Keys.GetString("UnsupportedUrnAttrib", attrib);
	}

	public static string MappingObjectIdMissing(string typename, int id)
	{
		return Keys.GetString("MappingObjectIdMissing", typename, id);
	}

	public static string EmptyMapping(string parent, string mappingname)
	{
		return Keys.GetString("EmptyMapping", parent, mappingname);
	}

	public static string UnknownCategoryName(string name)
	{
		return Keys.GetString("UnknownCategoryName", name);
	}

	public static string UnknownCategoryType(string typename)
	{
		return Keys.GetString("UnknownCategoryType", typename);
	}

	public static string UnknownOperator(string name)
	{
		return Keys.GetString("UnknownOperator", name);
	}

	public static string InvalidServerUrn(string serverName)
	{
		return Keys.GetString("InvalidServerUrn", serverName);
	}

	public static string InvalidUrn(string type)
	{
		return Keys.GetString("InvalidUrn", type);
	}

	public static string WMIProviderNotInstalled(string machineName)
	{
		return Keys.GetString("WMIProviderNotInstalled", machineName);
	}

	public static string PropertyCannotBeChangedAfterConnection(string propertyName)
	{
		return Keys.GetString("PropertyCannotBeChangedAfterConnection", propertyName);
	}

	public static string CouldNotFindManagementObject(string type, string name)
	{
		return Keys.GetString("CouldNotFindManagementObject", type, name);
	}

	public static string NotSupportedNotification(string className, string eventType)
	{
		return Keys.GetString("NotSupportedNotification", className, eventType);
	}

	public static string MissingObjectExceptionText(string parentName, string objectName, string serverVersion)
	{
		return Keys.GetString("MissingObjectExceptionText", parentName, objectName, serverVersion);
	}

	public static string PropertyNotSetExceptionText(string propertyName)
	{
		return Keys.GetString("PropertyNotSetExceptionText", propertyName);
	}

	public static string MissingObjectNameExceptionText(string parentName, string objectType, string objectName)
	{
		return Keys.GetString("MissingObjectNameExceptionText", parentName, objectType, objectName);
	}

	public static string WrongPropertyValueExceptionText(string propName, string propValue)
	{
		return Keys.GetString("WrongPropertyValueExceptionText", propName, propValue);
	}

	public static string PropertyTypeMismatchExceptionText(string propname, string received, string expected)
	{
		return Keys.GetString("PropertyTypeMismatchExceptionText", propname, received, expected);
	}

	public static string MissingPropertyExceptionText(string propertyName, string serverVersion)
	{
		return Keys.GetString("MissingPropertyExceptionText", propertyName, serverVersion);
	}

	public static string UnknownPropertyExceptionText(string propertyName)
	{
		return Keys.GetString("UnknownPropertyExceptionText", propertyName);
	}

	public static string PropertyReadOnlyExceptionText(string name)
	{
		return Keys.GetString("PropertyReadOnlyExceptionText", name);
	}

	public static string InvalidSmoOperationExceptionText(string opName, string state)
	{
		return Keys.GetString("InvalidSmoOperationExceptionText", opName, state);
	}

	public static string PropertyCannotBeRetrievedExceptionText(string objType, string propname, string objName)
	{
		return Keys.GetString("PropertyCannotBeRetrievedExceptionText", objType, propname, objName);
	}

	public static string ObjectDroppedExceptionText(string type, string name)
	{
		return Keys.GetString("ObjectDroppedExceptionText", type, name);
	}

	public static string UnsupportedObjectNameExceptionText(string objectType)
	{
		return Keys.GetString("UnsupportedObjectNameExceptionText", objectType);
	}

	public static string FailedtoInitialize(string urn)
	{
		return Keys.GetString("FailedtoInitialize", urn);
	}

	public static string PropertyMustBeSpecifiedInUrn(string propName, string nodeType)
	{
		return Keys.GetString("PropertyMustBeSpecifiedInUrn", propName, nodeType);
	}

	public static string InvalidScanType(string scanType)
	{
		return Keys.GetString("InvalidScanType", scanType);
	}

	public static string TempTablesNotSupported(string tableName)
	{
		return Keys.GetString("TempTablesNotSupported", tableName);
	}

	public static string PlanGuideNameCannotStartWithHash(string planGuideName)
	{
		return Keys.GetString("PlanGuideNameCannotStartWithHash", planGuideName);
	}

	public static string PropertiesNotValidException(string propNames, string propName, string propValue)
	{
		return Keys.GetString("PropertiesNotValidException", propNames, propName, propValue);
	}

	public static string TypeSchemaMustBeDbo(string prop, string value)
	{
		return Keys.GetString("TypeSchemaMustBeDbo", prop, value);
	}

	public static string UnsupportedPermission(string permName)
	{
		return Keys.GetString("UnsupportedPermission", permName);
	}

	public static string InvalidVersionSmoOperation(string version)
	{
		return Keys.GetString("InvalidVersionSmoOperation", version);
	}

	public static string ContainmentNotSupported(string version)
	{
		return Keys.GetString("ContainmentNotSupported", version);
	}

	public static string AweEnabledNotSupported(string version)
	{
		return Keys.GetString("AweEnabledNotSupported", version);
	}

	public static string FailedToWriteProperty(string propName, string objectType, string objectName, string reason)
	{
		return Keys.GetString("FailedToWriteProperty", propName, objectType, objectName, reason);
	}

	public static string SyntaxErrorInTextHeader(string objectType, string objectName)
	{
		return Keys.GetString("SyntaxErrorInTextHeader", objectType, objectName);
	}

	public static string IncorrectTextHeader(string objectType, string objectName, string propNameSmall, string propName)
	{
		return Keys.GetString("IncorrectTextHeader", objectType, objectName, propNameSmall, propName);
	}

	public static string ScriptHeaderTypeNotSupported(string scriptHeaderType, string objectType, string objectName)
	{
		return Keys.GetString("ScriptHeaderTypeNotSupported", scriptHeaderType, objectType, objectName);
	}

	public static string PropertyIsInvalidInUrn(string propName, string nodeType)
	{
		return Keys.GetString("PropertyIsInvalidInUrn", propName, nodeType);
	}

	public static string ReasonObjectAlreadyCreated(string objName)
	{
		return Keys.GetString("ReasonObjectAlreadyCreated", objName);
	}

	public static string CannotReadProperty(string propName)
	{
		return Keys.GetString("CannotReadProperty", propName);
	}

	public static string CannotWriteProperty(string propName)
	{
		return Keys.GetString("CannotWriteProperty", propName);
	}

	public static string CannotAccessProperty(string propName)
	{
		return Keys.GetString("CannotAccessProperty", propName);
	}

	public static string PropertyNotAvailableToWrite(string propName, string version)
	{
		return Keys.GetString("PropertyNotAvailableToWrite", propName, version);
	}

	public static string CantScriptObject(string urn)
	{
		return Keys.GetString("CantScriptObject", urn);
	}

	public static string PropertyNotValidException(string propName1, string propName2, string propValue)
	{
		return Keys.GetString("PropertyNotValidException", propName1, propName2, propValue);
	}

	public static string FailedOperationExceptionText(string opName, string objType, string objName)
	{
		return Keys.GetString("FailedOperationExceptionText", opName, objType, objName);
	}

	public static string FailedOperationExceptionText2(string opName)
	{
		return Keys.GetString("FailedOperationExceptionText2", opName);
	}

	public static string FailedOperationExceptionText3(string opName, string objType, string objName, string Reason)
	{
		return Keys.GetString("FailedOperationExceptionText3", opName, objType, objName, Reason);
	}

	public static string FailedOperationExceptionTextColl(string opName, string coll, string objType, string parent)
	{
		return Keys.GetString("FailedOperationExceptionTextColl", opName, coll, objType, parent);
	}

	public static string FailedOperationExceptionTextScript(string objType, string objName)
	{
		return Keys.GetString("FailedOperationExceptionTextScript", objType, objName);
	}

	public static string CannotCreateAvailabilityGroupWithoutCurrentIntance(string instanceName, string agName)
	{
		return Keys.GetString("CannotCreateAvailabilityGroupWithoutCurrentIntance", instanceName, agName);
	}

	public static string JoinAvailabilityGroupFailed(string replicaName, string agName)
	{
		return Keys.GetString("JoinAvailabilityGroupFailed", replicaName, agName);
	}

	public static string ForceFailoverFailed(string serverName, string agName)
	{
		return Keys.GetString("ForceFailoverFailed", serverName, agName);
	}

	public static string ManualFailoverFailed(string serverName, string agName)
	{
		return Keys.GetString("ManualFailoverFailed", serverName, agName);
	}

	public static string DatabaseJoinAvailabilityGroupFailed(string replicaName, string agName, string dbName)
	{
		return Keys.GetString("DatabaseJoinAvailabilityGroupFailed", replicaName, agName, dbName);
	}

	public static string DatabaseJoinAvailabilityGroupInvalidGroupName(string agName, string parentAgName, string dbName)
	{
		return Keys.GetString("DatabaseJoinAvailabilityGroupInvalidGroupName", agName, parentAgName, dbName);
	}

	public static string DatabaseLeaveAvailabilityGroupFailed(string replicaName, string agName, string dbName)
	{
		return Keys.GetString("DatabaseLeaveAvailabilityGroupFailed", replicaName, agName, dbName);
	}

	public static string SuspendDataMovementFailed(string replicaName, string agName, string dbName)
	{
		return Keys.GetString("SuspendDataMovementFailed", replicaName, agName, dbName);
	}

	public static string ResumeDataMovementFailed(string replicaName, string agName, string dbName)
	{
		return Keys.GetString("ResumeDataMovementFailed", replicaName, agName, dbName);
	}

	public static string EnumClusterMemberState(string serverName)
	{
		return Keys.GetString("EnumClusterMemberState", serverName);
	}

	public static string EnumReplicaClusterNodes(string serverName)
	{
		return Keys.GetString("EnumReplicaClusterNodes", serverName);
	}

	public static string EnumClusterSubnets(string serverName)
	{
		return Keys.GetString("EnumClusterSubnets", serverName);
	}

	public static string RestartListenerFailed(string listenerName, string agName)
	{
		return Keys.GetString("RestartListenerFailed", listenerName, agName);
	}

	public static string PropertyCannotBeRetrievedFromSecondary(string propname)
	{
		return Keys.GetString("PropertyCannotBeRetrievedFromSecondary", propname);
	}

	public static string GrantAGCreateDatabasePrivilegeFailed(string serverName, string agName)
	{
		return Keys.GetString("GrantAGCreateDatabasePrivilegeFailed", serverName, agName);
	}

	public static string RevokeAGCreateDatabasePrivilegeFailed(string serverName, string agName)
	{
		return Keys.GetString("RevokeAGCreateDatabasePrivilegeFailed", serverName, agName);
	}

	public static string Win32Error(string code)
	{
		return Keys.GetString("Win32Error", code);
	}

	public static string WrongIndexRangeProvidedCPU(int startIndex, int endIndex)
	{
		return Keys.GetString("WrongIndexRangeProvidedCPU", startIndex, endIndex);
	}

	public static string WrongIndexRangeProvidedNuma(int startIndex, int endIndex)
	{
		return Keys.GetString("WrongIndexRangeProvidedNuma", startIndex, endIndex);
	}

	public static string HoleInIndexRangeProvidedCPU(int index)
	{
		return Keys.GetString("HoleInIndexRangeProvidedCPU", index);
	}

	public static string HoleInIndexRangeProvidedNumaNode(int index)
	{
		return Keys.GetString("HoleInIndexRangeProvidedNumaNode", index);
	}

	public static string WrongIndexRangeProvidedScheduler(int startIndex, int endIndex)
	{
		return Keys.GetString("WrongIndexRangeProvidedScheduler", startIndex, endIndex);
	}

	public static string PropertyNotSetInDesignMode(string name)
	{
		return Keys.GetString("PropertyNotSetInDesignMode", name);
	}

	public static string PropertyNotAvailableInDesignMode(string name)
	{
		return Keys.GetString("PropertyNotAvailableInDesignMode", name);
	}

	public static string PropertyNotFound(string propertyName, string typeName)
	{
		return Keys.GetString("PropertyNotFound", propertyName, typeName);
	}

	public static string UnknownDomain(string name)
	{
		return Keys.GetString("UnknownDomain", name);
	}

	public static string FileTableNotSupportedOnTargetEngine(string targetVersion)
	{
		return Keys.GetString("FileTableNotSupportedOnTargetEngine", targetVersion);
	}

	public static string PropertyOnlySupportedForFileTable(string propName)
	{
		return Keys.GetString("PropertyOnlySupportedForFileTable", propName);
	}

	public static string TableNotFileTable(string name)
	{
		return Keys.GetString("TableNotFileTable", name);
	}

	public static string NamespaceNotEnabled(string name)
	{
		return Keys.GetString("NamespaceNotEnabled", name);
	}

	public static string TableNotExternalTable(string name)
	{
		return Keys.GetString("TableNotExternalTable", name);
	}

	public static string PropertyOnlySupportedForExternalTable(string propName)
	{
		return Keys.GetString("PropertyOnlySupportedForExternalTable", propName);
	}

	public static string ConflictingExternalTableProperties(string propName, string propValue, string confPropName, string confPropValue)
	{
		return Keys.GetString("ConflictingExternalTableProperties", propName, propValue, confPropName, confPropValue);
	}

	public static string ShardingColumnNotSupportedWithNonShardedDistribution(string shardingColPropName, string distributionName)
	{
		return Keys.GetString("ShardingColumnNotSupportedWithNonShardedDistribution", shardingColPropName, distributionName);
	}

	public static string ShardingColumnNotSpecifiedForShardedDistribution(string shardingColPropName)
	{
		return Keys.GetString("ShardingColumnNotSpecifiedForShardedDistribution", shardingColPropName);
	}

	public static string ShardingColumnNotAddedToTable(string shardingColName)
	{
		return Keys.GetString("ShardingColumnNotAddedToTable", shardingColName);
	}

	public static string DependentPropertyMissing(string propName, string dependentPropName)
	{
		return Keys.GetString("DependentPropertyMissing", propName, dependentPropName);
	}

	public static string UnsupportedPropertyForSXI(string propertyName)
	{
		return Keys.GetString("UnsupportedPropertyForSXI", propertyName);
	}

	public static string UnsupportedValueForSXI(string value, string propertyName, string suggestion)
	{
		return Keys.GetString("UnsupportedValueForSXI", value, propertyName, suggestion);
	}

	public static string ExecutingScript(string statement)
	{
		return Keys.GetString("ExecutingScript", statement);
	}

	public static string StartingDataTransfer(string tableName)
	{
		return Keys.GetString("StartingDataTransfer", tableName);
	}

	public static string CompletedDataTransfer(string tableName)
	{
		return Keys.GetString("CompletedDataTransfer", tableName);
	}

	public static string InvalidIndexSpecifiedForModifyingTextToCreateOrAlter(int index, int startIndex, int endIndex)
	{
		return Keys.GetString("InvalidIndexSpecifiedForModifyingTextToCreateOrAlter", index, startIndex, endIndex);
	}
}

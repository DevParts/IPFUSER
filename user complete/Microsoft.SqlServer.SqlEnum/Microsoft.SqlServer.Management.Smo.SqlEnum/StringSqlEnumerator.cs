using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.Management.Smo.SqlEnum;

[CompilerGenerated]
internal class StringSqlEnumerator
{
	[CompilerGenerated]
	public class Keys
	{
		public const string IncorrectVersionTag = "IncorrectVersionTag";

		public const string ObjectNotSupportedOnSqlDw = "ObjectNotSupportedOnSqlDw";

		public const string InvalidAttributeValue = "InvalidAttributeValue";

		public const string NullVersionOnLoadingCfgFile = "NullVersionOnLoadingCfgFile";

		public const string EnumObjectTagNotFound = "EnumObjectTagNotFound";

		public const string InvalidConnectionType = "InvalidConnectionType";

		public const string OnlyPathOrFullName = "OnlyPathOrFullName";

		public const string FileNameMustHavePath = "FileNameMustHavePath";

		public const string DatabaseNameMustBeSpecified = "DatabaseNameMustBeSpecified";

		public const string FailedToLoadResFile = "FailedToLoadResFile";

		public const string UnsupportedTypeDepDiscovery = "UnsupportedTypeDepDiscovery";

		public const string QueryNotSupportedPostProcess = "QueryNotSupportedPostProcess";

		public const string FailedToLoadAssembly = "FailedToLoadAssembly";

		public const string FailedToCreateUrn = "FailedToCreateUrn";

		public const string UnknownOperator = "UnknownOperator";

		public const string PropMustBeSpecified = "PropMustBeSpecified";

		public const string InvalidUrnForDepends = "InvalidUrnForDepends";

		public const string TooManyDbLevels = "TooManyDbLevels";

		public const string CouldNotInstantiateObj = "CouldNotInstantiateObj";

		public const string NotDerivedFrom = "NotDerivedFrom";

		public const string UnknownType = "UnknownType";

		public const string InvalidConfigurationFile = "InvalidConfigurationFile";

		public const string MissingSection = "MissingSection";

		public const string NotDbObject = "NotDbObject";

		public const string NotSingleDb = "NotSingleDb";

		public const string NoClassNamePostProcess = "NoClassNamePostProcess";

		public const string InvalidVersion = "InvalidVersion";

		public const string InvalidSqlServer = "InvalidSqlServer";

		public const string DatabaseNameMustBeSpecifiedinTheUrn = "DatabaseNameMustBeSpecifiedinTheUrn";

		public const string CouldNotGetInfoFromDependencyRow = "CouldNotGetInfoFromDependencyRow";

		public const string SqlServer90Name = "SqlServer90Name";

		public const string SqlServer80Name = "SqlServer80Name";

		public const string SmoSQLCLRUnAvailable = "SmoSQLCLRUnAvailable";

		public const string UnknownPermissionType = "UnknownPermissionType";

		public const string UnknownPermissionCode = "UnknownPermissionCode";

		public const string Executing = "Executing";

		public const string WaitingForWorkerThread = "WaitingForWorkerThread";

		public const string BetweenRetries = "BetweenRetries";

		public const string Idle = "Idle";

		public const string Suspended = "Suspended";

		public const string WaitingForStepToFinish = "WaitingForStepToFinish";

		public const string PerformingCompletionAction = "PerformingCompletionAction";

		public const string Unknown = "Unknown";

		public const string ctNone = "ctNone";

		public const string ctPartial = "ctPartial";

		public const string rmFull = "rmFull";

		public const string rmBulkLogged = "rmBulkLogged";

		public const string rmSimple = "rmSimple";

		public const string msNone = "msNone";

		public const string msSuspended = "msSuspended";

		public const string msDisconnected = "msDisconnected";

		public const string msSynchronizing = "msSynchronizing";

		public const string msPendingFailover = "msPendingFailover";

		public const string msSynchronized = "msSynchronized";

		public const string agshNoneSynchronizing = "agshNoneSynchronizing";

		public const string agshPartiallySynchronizing = "agshPartiallySynchronizing";

		public const string agshAllSynchronizing = "agshAllSynchronizing";

		public const string agshAllSynchronized = "agshAllSynchronized";

		public const string arosPendingFailover = "arosPendingFailover";

		public const string arosPending = "arosPending";

		public const string arosOnline = "arosOnline";

		public const string arosOffline = "arosOffline";

		public const string arosFailed = "arosFailed";

		public const string arosFailedNoQuorum = "arosFailedNoQuorum";

		public const string arrhInProgress = "arrhInProgress";

		public const string arrhOnline = "arrhOnline";

		public const string arshNotSynchronizing = "arshNotSynchronizing";

		public const string arshSynchronizing = "arshSynchronizing";

		public const string arshSynchronized = "arshSynchronized";

		public const string arrUninitialized = "arrUninitialized";

		public const string arrResolving = "arrResolving";

		public const string arrSecondary = "arrSecondary";

		public const string arrPrimary = "arrPrimary";

		public const string arcsDisconnected = "arcsDisconnected";

		public const string arcsConnected = "arcsConnected";

		public const string hmsPendingCommunication = "hmsPendingCommunication";

		public const string hmsRunning = "hmsRunning";

		public const string hmsFailed = "hmsFailed";

		public const string cqtNodeMajority = "cqtNodeMajority";

		public const string cqtNodeAndDiskMajority = "cqtNodeAndDiskMajority";

		public const string cqtNodeAndFileshareMajority = "cqtNodeAndFileshareMajority";

		public const string cqtDiskOnly = "cqtDiskOnly";

		public const string cqtNotApplicable = "cqtNotApplicable";

		public const string cqsUnknownQuorumState = "cqsUnknownQuorumState";

		public const string cqsNormalQuorum = "cqsNormalQuorum";

		public const string cqsForcedQuorum = "cqsForcedQuorum";

		public const string cqsNotApplicable = "cqsNotApplicable";

		public const string cmtNode = "cmtNode";

		public const string cmtDiskWitness = "cmtDiskWitness";

		public const string cmtFileshareWitness = "cmtFileshareWitness";

		public const string cmsOffline = "cmsOffline";

		public const string cmsPartiallyOnline = "cmsPartiallyOnline";

		public const string cmsOnline = "cmsOnline";

		public const string cmsUnknown = "cmsUnknown";

		public const string replicaReadModeNoConnections = "replicaReadModeNoConnections";

		public const string replicaReadModeReadIntentConnectionsOnly = "replicaReadModeReadIntentConnectionsOnly";

		public const string replicaReadModeAllConnections = "replicaReadModeAllConnections";

		public const string cmprReadWriteConnections = "cmprReadWriteConnections";

		public const string cmprAllConnections = "cmprAllConnections";

		public const string cmsrNoConnections = "cmsrNoConnections";

		public const string cmsrReadIntentConnectionsOnly = "cmsrReadIntentConnectionsOnly";

		public const string cmsrAllConnections = "cmsrAllConnections";

		public const string seedingModeAutomatic = "seedingModeAutomatic";

		public const string seedingModeManual = "seedingModeManual";

		public const string aramSynchronousCommit = "aramSynchronousCommit";

		public const string aramAsynchronousCommit = "aramAsynchronousCommit";

		public const string aramConfigurationOnly = "aramConfigurationOnly";

		public const string arfmAutomatic = "arfmAutomatic";

		public const string arfmManual = "arfmManual";

		public const string arfmExternal = "arfmExternal";

		public const string arjsNotJoined = "arjsNotJoined";

		public const string arjsJoinedStandaloneInstance = "arjsJoinedStandaloneInstance";

		public const string arjsJoinedFailoverClusterInstance = "arjsJoinedFailoverClusterInstance";

		public const string adssNotSynchronizing = "adssNotSynchronizing";

		public const string adssSynchronizing = "adssSynchronizing";

		public const string adssSynchronized = "adssSynchronized";

		public const string adssReverting = "adssReverting";

		public const string adssInitializing = "adssInitializing";

		public const string drsrSuspendFromUser = "drsrSuspendFromUser";

		public const string drsrSuspendFromPartner = "drsrSuspendFromPartner";

		public const string drsrSuspendFromRedo = "drsrSuspendFromRedo";

		public const string drsrSuspendFromApply = "drsrSuspendFromApply";

		public const string drsrSuspendFromCapture = "drsrSuspendFromCapture";

		public const string drsrSuspendFromRestart = "drsrSuspendFromRestart";

		public const string drsrSuspendFromUndo = "drsrSuspendFromUndo";

		public const string drsrNotApplicable = "drsrNotApplicable";

		public const string agabpPrimary = "agabpPrimary";

		public const string agabpSecondaryOnly = "agabpSecondaryOnly";

		public const string agabpSecondary = "agabpSecondary";

		public const string agabpNone = "agabpNone";

		public const string agfcOnServerDown = "agfcOnServerDown";

		public const string agfcOnServerUnresponsive = "agfcOnServerUnresponsive";

		public const string agfcOnCriticalServerErrors = "agfcOnCriticalServerErrors";

		public const string agfcOnModerateServerErrors = "agfcOnModerateServerErrors";

		public const string agfcOnAnyQualifiedFailureCondition = "agfcOnAnyQualifiedFailureCondition";

		public const string aglipOffline = "aglipOffline";

		public const string aglipOnline = "aglipOnline";

		public const string aglipOnlinePending = "aglipOnlinePending";

		public const string agliFailure = "agliFailure";

		public const string agliUnknown = "agliUnknown";

		public const string agctExternal = "agctExternal";

		public const string agctNone = "agctNone";

		public const string agctWsfc = "agctWsfc";

		public const string fgtRowsFileGroup = "fgtRowsFileGroup";

		public const string fgtFileStreamDataFileGroup = "fgtFileStreamDataFileGroup";

		public const string fgtMemoryOptimizedDataFileGroup = "fgtMemoryOptimizedDataFileGroup";

		public const string securityPredicateTypeFilter = "securityPredicateTypeFilter";

		public const string securityPredicateTypeBlock = "securityPredicateTypeBlock";

		public const string securityPredicateOperationAll = "securityPredicateOperationAll";

		public const string securityPredicateOperationAfterInsert = "securityPredicateOperationAfterInsert";

		public const string securityPredicateOperationAfterUpdate = "securityPredicateOperationAfterUpdate";

		public const string securityPredicateOperationBeforeUpdate = "securityPredicateOperationBeforeUpdate";

		public const string securityPredicateOperationBeforeDelete = "securityPredicateOperationBeforeDelete";

		public const string Clustered = "Clustered";

		public const string NonClustered = "NonClustered";

		public const string PrimaryXml = "PrimaryXml";

		public const string SecondaryXml = "SecondaryXml";

		public const string Spatial = "Spatial";

		public const string NonClusteredColumnStore = "NonClusteredColumnStore";

		public const string NonClusteredHash = "NonClusteredHash";

		public const string SelectiveXml = "SelectiveXml";

		public const string SecondarySelectiveXml = "SecondarySelectiveXml";

		public const string ClusteredColumnStore = "ClusteredColumnStore";

		public const string Heap = "Heap";

		public const string TransactSql = "TransactSql";

		public const string ActiveScripting = "ActiveScripting";

		public const string CmdExec = "CmdExec";

		public const string AnalysisCommand = "AnalysisCommand";

		public const string AnalysisQuery = "AnalysisQuery";

		public const string ReplDistribution = "ReplDistribution";

		public const string ReplMerge = "ReplMerge";

		public const string ReplQueueReader = "ReplQueueReader";

		public const string ReplSnapshot = "ReplSnapshot";

		public const string ReplLogReader = "ReplLogReader";

		public const string SSIS = "SSIS";

		public const string PowerShell = "PowerShell";

		public const string dbCatalogCollationDatabaseDefault = "dbCatalogCollationDatabaseDefault";

		public const string dbCatalogCollationContained = "dbCatalogCollationContained";

		public const string dbCatalogCollationSQL_Latin1_General_CP1_CI_AS = "dbCatalogCollationSQL_Latin1_General_CP1_CI_AS";

		public const string UnknownDest = "UnknownDest";

		public const string FileDest = "FileDest";

		public const string SecurityLogDest = "SecurityLogDest";

		public const string ApplicationLogDest = "ApplicationLogDest";

		public const string UrlDest = "UrlDest";

		public const string Off = "Off";

		public const string ReadOnly = "ReadOnly";

		public const string ReadWrite = "ReadWrite";

		public const string Error = "Error";

		public const string All = "All";

		public const string Auto = "Auto";

		public const string None = "None";

		private static ResourceManager resourceManager = new ResourceManager(typeof(StringSqlEnumerator).FullName, typeof(StringSqlEnumerator).Module.Assembly);

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

	public static string ObjectNotSupportedOnSqlDw => Keys.GetString("ObjectNotSupportedOnSqlDw");

	public static string InvalidAttributeValue => Keys.GetString("InvalidAttributeValue");

	public static string NullVersionOnLoadingCfgFile => Keys.GetString("NullVersionOnLoadingCfgFile");

	public static string EnumObjectTagNotFound => Keys.GetString("EnumObjectTagNotFound");

	public static string InvalidConnectionType => Keys.GetString("InvalidConnectionType");

	public static string OnlyPathOrFullName => Keys.GetString("OnlyPathOrFullName");

	public static string FileNameMustHavePath => Keys.GetString("FileNameMustHavePath");

	public static string DatabaseNameMustBeSpecified => Keys.GetString("DatabaseNameMustBeSpecified");

	public static string UnknownOperator => Keys.GetString("UnknownOperator");

	public static string TooManyDbLevels => Keys.GetString("TooManyDbLevels");

	public static string InvalidConfigurationFile => Keys.GetString("InvalidConfigurationFile");

	public static string NotDbObject => Keys.GetString("NotDbObject");

	public static string NotSingleDb => Keys.GetString("NotSingleDb");

	public static string NoClassNamePostProcess => Keys.GetString("NoClassNamePostProcess");

	public static string SqlServer90Name => Keys.GetString("SqlServer90Name");

	public static string SqlServer80Name => Keys.GetString("SqlServer80Name");

	public static string SmoSQLCLRUnAvailable => Keys.GetString("SmoSQLCLRUnAvailable");

	public static string Executing => Keys.GetString("Executing");

	public static string WaitingForWorkerThread => Keys.GetString("WaitingForWorkerThread");

	public static string BetweenRetries => Keys.GetString("BetweenRetries");

	public static string Idle => Keys.GetString("Idle");

	public static string Suspended => Keys.GetString("Suspended");

	public static string WaitingForStepToFinish => Keys.GetString("WaitingForStepToFinish");

	public static string PerformingCompletionAction => Keys.GetString("PerformingCompletionAction");

	public static string Unknown => Keys.GetString("Unknown");

	public static string ctNone => Keys.GetString("ctNone");

	public static string ctPartial => Keys.GetString("ctPartial");

	public static string rmFull => Keys.GetString("rmFull");

	public static string rmBulkLogged => Keys.GetString("rmBulkLogged");

	public static string rmSimple => Keys.GetString("rmSimple");

	public static string msNone => Keys.GetString("msNone");

	public static string msSuspended => Keys.GetString("msSuspended");

	public static string msDisconnected => Keys.GetString("msDisconnected");

	public static string msSynchronizing => Keys.GetString("msSynchronizing");

	public static string msPendingFailover => Keys.GetString("msPendingFailover");

	public static string msSynchronized => Keys.GetString("msSynchronized");

	public static string agshNoneSynchronizing => Keys.GetString("agshNoneSynchronizing");

	public static string agshPartiallySynchronizing => Keys.GetString("agshPartiallySynchronizing");

	public static string agshAllSynchronizing => Keys.GetString("agshAllSynchronizing");

	public static string agshAllSynchronized => Keys.GetString("agshAllSynchronized");

	public static string arosPendingFailover => Keys.GetString("arosPendingFailover");

	public static string arosPending => Keys.GetString("arosPending");

	public static string arosOnline => Keys.GetString("arosOnline");

	public static string arosOffline => Keys.GetString("arosOffline");

	public static string arosFailed => Keys.GetString("arosFailed");

	public static string arosFailedNoQuorum => Keys.GetString("arosFailedNoQuorum");

	public static string arrhInProgress => Keys.GetString("arrhInProgress");

	public static string arrhOnline => Keys.GetString("arrhOnline");

	public static string arshNotSynchronizing => Keys.GetString("arshNotSynchronizing");

	public static string arshSynchronizing => Keys.GetString("arshSynchronizing");

	public static string arshSynchronized => Keys.GetString("arshSynchronized");

	public static string arrUninitialized => Keys.GetString("arrUninitialized");

	public static string arrResolving => Keys.GetString("arrResolving");

	public static string arrSecondary => Keys.GetString("arrSecondary");

	public static string arrPrimary => Keys.GetString("arrPrimary");

	public static string arcsDisconnected => Keys.GetString("arcsDisconnected");

	public static string arcsConnected => Keys.GetString("arcsConnected");

	public static string hmsPendingCommunication => Keys.GetString("hmsPendingCommunication");

	public static string hmsRunning => Keys.GetString("hmsRunning");

	public static string hmsFailed => Keys.GetString("hmsFailed");

	public static string cqtNodeMajority => Keys.GetString("cqtNodeMajority");

	public static string cqtNodeAndDiskMajority => Keys.GetString("cqtNodeAndDiskMajority");

	public static string cqtNodeAndFileshareMajority => Keys.GetString("cqtNodeAndFileshareMajority");

	public static string cqtDiskOnly => Keys.GetString("cqtDiskOnly");

	public static string cqtNotApplicable => Keys.GetString("cqtNotApplicable");

	public static string cqsUnknownQuorumState => Keys.GetString("cqsUnknownQuorumState");

	public static string cqsNormalQuorum => Keys.GetString("cqsNormalQuorum");

	public static string cqsForcedQuorum => Keys.GetString("cqsForcedQuorum");

	public static string cqsNotApplicable => Keys.GetString("cqsNotApplicable");

	public static string cmtNode => Keys.GetString("cmtNode");

	public static string cmtDiskWitness => Keys.GetString("cmtDiskWitness");

	public static string cmtFileshareWitness => Keys.GetString("cmtFileshareWitness");

	public static string cmsOffline => Keys.GetString("cmsOffline");

	public static string cmsPartiallyOnline => Keys.GetString("cmsPartiallyOnline");

	public static string cmsOnline => Keys.GetString("cmsOnline");

	public static string cmsUnknown => Keys.GetString("cmsUnknown");

	public static string replicaReadModeNoConnections => Keys.GetString("replicaReadModeNoConnections");

	public static string replicaReadModeReadIntentConnectionsOnly => Keys.GetString("replicaReadModeReadIntentConnectionsOnly");

	public static string replicaReadModeAllConnections => Keys.GetString("replicaReadModeAllConnections");

	public static string cmprReadWriteConnections => Keys.GetString("cmprReadWriteConnections");

	public static string cmprAllConnections => Keys.GetString("cmprAllConnections");

	public static string cmsrNoConnections => Keys.GetString("cmsrNoConnections");

	public static string cmsrReadIntentConnectionsOnly => Keys.GetString("cmsrReadIntentConnectionsOnly");

	public static string cmsrAllConnections => Keys.GetString("cmsrAllConnections");

	public static string seedingModeAutomatic => Keys.GetString("seedingModeAutomatic");

	public static string seedingModeManual => Keys.GetString("seedingModeManual");

	public static string aramSynchronousCommit => Keys.GetString("aramSynchronousCommit");

	public static string aramAsynchronousCommit => Keys.GetString("aramAsynchronousCommit");

	public static string aramConfigurationOnly => Keys.GetString("aramConfigurationOnly");

	public static string arfmAutomatic => Keys.GetString("arfmAutomatic");

	public static string arfmManual => Keys.GetString("arfmManual");

	public static string arfmExternal => Keys.GetString("arfmExternal");

	public static string arjsNotJoined => Keys.GetString("arjsNotJoined");

	public static string arjsJoinedStandaloneInstance => Keys.GetString("arjsJoinedStandaloneInstance");

	public static string arjsJoinedFailoverClusterInstance => Keys.GetString("arjsJoinedFailoverClusterInstance");

	public static string adssNotSynchronizing => Keys.GetString("adssNotSynchronizing");

	public static string adssSynchronizing => Keys.GetString("adssSynchronizing");

	public static string adssSynchronized => Keys.GetString("adssSynchronized");

	public static string adssReverting => Keys.GetString("adssReverting");

	public static string adssInitializing => Keys.GetString("adssInitializing");

	public static string drsrSuspendFromUser => Keys.GetString("drsrSuspendFromUser");

	public static string drsrSuspendFromPartner => Keys.GetString("drsrSuspendFromPartner");

	public static string drsrSuspendFromRedo => Keys.GetString("drsrSuspendFromRedo");

	public static string drsrSuspendFromApply => Keys.GetString("drsrSuspendFromApply");

	public static string drsrSuspendFromCapture => Keys.GetString("drsrSuspendFromCapture");

	public static string drsrSuspendFromRestart => Keys.GetString("drsrSuspendFromRestart");

	public static string drsrSuspendFromUndo => Keys.GetString("drsrSuspendFromUndo");

	public static string drsrNotApplicable => Keys.GetString("drsrNotApplicable");

	public static string agabpPrimary => Keys.GetString("agabpPrimary");

	public static string agabpSecondaryOnly => Keys.GetString("agabpSecondaryOnly");

	public static string agabpSecondary => Keys.GetString("agabpSecondary");

	public static string agabpNone => Keys.GetString("agabpNone");

	public static string agfcOnServerDown => Keys.GetString("agfcOnServerDown");

	public static string agfcOnServerUnresponsive => Keys.GetString("agfcOnServerUnresponsive");

	public static string agfcOnCriticalServerErrors => Keys.GetString("agfcOnCriticalServerErrors");

	public static string agfcOnModerateServerErrors => Keys.GetString("agfcOnModerateServerErrors");

	public static string agfcOnAnyQualifiedFailureCondition => Keys.GetString("agfcOnAnyQualifiedFailureCondition");

	public static string aglipOffline => Keys.GetString("aglipOffline");

	public static string aglipOnline => Keys.GetString("aglipOnline");

	public static string aglipOnlinePending => Keys.GetString("aglipOnlinePending");

	public static string agliFailure => Keys.GetString("agliFailure");

	public static string agliUnknown => Keys.GetString("agliUnknown");

	public static string agctExternal => Keys.GetString("agctExternal");

	public static string agctNone => Keys.GetString("agctNone");

	public static string agctWsfc => Keys.GetString("agctWsfc");

	public static string fgtRowsFileGroup => Keys.GetString("fgtRowsFileGroup");

	public static string fgtFileStreamDataFileGroup => Keys.GetString("fgtFileStreamDataFileGroup");

	public static string fgtMemoryOptimizedDataFileGroup => Keys.GetString("fgtMemoryOptimizedDataFileGroup");

	public static string securityPredicateTypeFilter => Keys.GetString("securityPredicateTypeFilter");

	public static string securityPredicateTypeBlock => Keys.GetString("securityPredicateTypeBlock");

	public static string securityPredicateOperationAll => Keys.GetString("securityPredicateOperationAll");

	public static string securityPredicateOperationAfterInsert => Keys.GetString("securityPredicateOperationAfterInsert");

	public static string securityPredicateOperationAfterUpdate => Keys.GetString("securityPredicateOperationAfterUpdate");

	public static string securityPredicateOperationBeforeUpdate => Keys.GetString("securityPredicateOperationBeforeUpdate");

	public static string securityPredicateOperationBeforeDelete => Keys.GetString("securityPredicateOperationBeforeDelete");

	public static string Clustered => Keys.GetString("Clustered");

	public static string NonClustered => Keys.GetString("NonClustered");

	public static string PrimaryXml => Keys.GetString("PrimaryXml");

	public static string SecondaryXml => Keys.GetString("SecondaryXml");

	public static string Spatial => Keys.GetString("Spatial");

	public static string NonClusteredColumnStore => Keys.GetString("NonClusteredColumnStore");

	public static string NonClusteredHash => Keys.GetString("NonClusteredHash");

	public static string SelectiveXml => Keys.GetString("SelectiveXml");

	public static string SecondarySelectiveXml => Keys.GetString("SecondarySelectiveXml");

	public static string ClusteredColumnStore => Keys.GetString("ClusteredColumnStore");

	public static string Heap => Keys.GetString("Heap");

	public static string TransactSql => Keys.GetString("TransactSql");

	public static string ActiveScripting => Keys.GetString("ActiveScripting");

	public static string CmdExec => Keys.GetString("CmdExec");

	public static string AnalysisCommand => Keys.GetString("AnalysisCommand");

	public static string AnalysisQuery => Keys.GetString("AnalysisQuery");

	public static string ReplDistribution => Keys.GetString("ReplDistribution");

	public static string ReplMerge => Keys.GetString("ReplMerge");

	public static string ReplQueueReader => Keys.GetString("ReplQueueReader");

	public static string ReplSnapshot => Keys.GetString("ReplSnapshot");

	public static string ReplLogReader => Keys.GetString("ReplLogReader");

	public static string SSIS => Keys.GetString("SSIS");

	public static string PowerShell => Keys.GetString("PowerShell");

	public static string dbCatalogCollationDatabaseDefault => Keys.GetString("dbCatalogCollationDatabaseDefault");

	public static string dbCatalogCollationContained => Keys.GetString("dbCatalogCollationContained");

	public static string dbCatalogCollationSQL_Latin1_General_CP1_CI_AS => Keys.GetString("dbCatalogCollationSQL_Latin1_General_CP1_CI_AS");

	public static string UnknownDest => Keys.GetString("UnknownDest");

	public static string FileDest => Keys.GetString("FileDest");

	public static string SecurityLogDest => Keys.GetString("SecurityLogDest");

	public static string ApplicationLogDest => Keys.GetString("ApplicationLogDest");

	public static string UrlDest => Keys.GetString("UrlDest");

	public static string Off => Keys.GetString("Off");

	public static string ReadOnly => Keys.GetString("ReadOnly");

	public static string ReadWrite => Keys.GetString("ReadWrite");

	public static string Error => Keys.GetString("Error");

	public static string All => Keys.GetString("All");

	public static string Auto => Keys.GetString("Auto");

	public static string None => Keys.GetString("None");

	protected StringSqlEnumerator()
	{
	}

	public static string IncorrectVersionTag(string elemContent)
	{
		return Keys.GetString("IncorrectVersionTag", elemContent);
	}

	public static string FailedToLoadResFile(string fileName)
	{
		return Keys.GetString("FailedToLoadResFile", fileName);
	}

	public static string UnsupportedTypeDepDiscovery(string objType, string suppList)
	{
		return Keys.GetString("UnsupportedTypeDepDiscovery", objType, suppList);
	}

	public static string QueryNotSupportedPostProcess(string propList)
	{
		return Keys.GetString("QueryNotSupportedPostProcess", propList);
	}

	public static string FailedToLoadAssembly(string assembly)
	{
		return Keys.GetString("FailedToLoadAssembly", assembly);
	}

	public static string FailedToCreateUrn(string objCode)
	{
		return Keys.GetString("FailedToCreateUrn", objCode);
	}

	public static string PropMustBeSpecified(string prop, string obj)
	{
		return Keys.GetString("PropMustBeSpecified", prop, obj);
	}

	public static string InvalidUrnForDepends(string urn)
	{
		return Keys.GetString("InvalidUrnForDepends", urn);
	}

	public static string CouldNotInstantiateObj(string objType)
	{
		return Keys.GetString("CouldNotInstantiateObj", objType);
	}

	public static string NotDerivedFrom(string objType, string objName)
	{
		return Keys.GetString("NotDerivedFrom", objType, objName);
	}

	public static string UnknownType(string type)
	{
		return Keys.GetString("UnknownType", type);
	}

	public static string MissingSection(string section)
	{
		return Keys.GetString("MissingSection", section);
	}

	public static string InvalidVersion(string version)
	{
		return Keys.GetString("InvalidVersion", version);
	}

	public static string InvalidSqlServer(string productName)
	{
		return Keys.GetString("InvalidSqlServer", productName);
	}

	public static string DatabaseNameMustBeSpecifiedinTheUrn(string urn)
	{
		return Keys.GetString("DatabaseNameMustBeSpecifiedinTheUrn", urn);
	}

	public static string CouldNotGetInfoFromDependencyRow(string rowInformation)
	{
		return Keys.GetString("CouldNotGetInfoFromDependencyRow", rowInformation);
	}

	public static string UnknownPermissionType(string permissionType)
	{
		return Keys.GetString("UnknownPermissionType", permissionType);
	}

	public static string UnknownPermissionCode(int code)
	{
		return Keys.GetString("UnknownPermissionCode", code);
	}
}

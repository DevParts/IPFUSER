using System;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;

namespace Microsoft.SqlServer.Management.Smo;

[TypeConverter(typeof(LocalizableTypeConverter))]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[StateChangeEvent("AUDIT_SERVER_OPERATION_EVENT", "SERVER")]
[CLSCompliant(false)]
public interface IServerConfigurationFacet : IDmfFacet
{
	bool ContainmentEnabled { get; set; }

	bool AdHocRemoteQueriesEnabled { get; set; }

	int AffinityMask { get; set; }

	int Affinity64Mask { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int AffinityIOMask { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int Affinity64IOMask { get; set; }

	bool AgentXPsEnabled { get; set; }

	bool AllowUpdates { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool AweEnabled { get; set; }

	int BlockedProcessThreshold { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool C2AuditTracingEnabled { get; set; }

	bool ClrIntegrationEnabled { get; set; }

	bool CommonCriteriaComplianceEnabled { get; }

	int CostThresholdForParallelism { get; set; }

	bool CrossDBOwnershipChainingEnabled { get; set; }

	int CursorThreshold { get; set; }

	bool DatabaseMailEnabled { get; set; }

	bool DefaultTraceEnabled { get; set; }

	int DefaultFullTextLanguage { get; set; }

	int DefaultLanguage { get; set; }

	bool DisallowResultsFromTriggers { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int FillFactor { get; set; }

	int FullTextCrawlBandwidthMin { get; set; }

	int FullTextCrawlBandwidthMax { get; set; }

	int FullTextNotifyBandwidthMin { get; set; }

	int FullTextNotifyBandwidthMax { get; set; }

	int FullTextCrawlRangeMax { get; set; }

	InDoubtTransactionResolutionType InDoubtTransactionResolution { get; set; }

	int IndexCreateMemory { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool LightweightPoolingEnabled { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int DynamicLocks { get; set; }

	int MaxDegreeOfParallelism { get; set; }

	int MaxServerMemory { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int MaxWorkerThreads { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int MediaRetention { get; set; }

	int MinMemoryPerQuery { get; set; }

	int MinServerMemory { get; set; }

	bool NestedTriggersEnabled { get; set; }

	int NetworkPacketSize { get; set; }

	bool OleAutomationEnabled { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int OpenObjects { get; set; }

	bool PrecomputeRank { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool PriorityBoost { get; set; }

	int ProtocolHandlerTimeout { get; set; }

	int QueryGovernorCostLimit { get; set; }

	int QueryWait { get; set; }

	int RecoveryInterval { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool RemoteAccessEnabled { get; set; }

	bool RemoteDacEnabled { get; set; }

	int RemoteLoginTimeout { get; set; }

	bool RemoteProcTransEnabled { get; set; }

	int RemoteQueryTimeout { get; set; }

	int ReplicationMaxTextSize { get; set; }

	bool ReplicationXPsEnabled { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool ScanForStartupProcedures { get; set; }

	bool ServerTriggerRecursionEnabled { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	bool SetWorkingSetSize { get; set; }

	bool ShowAdvancedOptions { get; set; }

	bool SmoAndDmoXPsEnabled { get; set; }

	bool SqlMailEnabled { get; set; }

	bool TransformNoiseWords { get; set; }

	int TwoDigitYearCutoff { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	int UserConnections { get; set; }

	int UserInstanceTimeout { get; set; }

	bool UserInstancesEnabled { get; set; }

	int UserOptions { get; set; }

	bool WebAssistantEnabled { get; set; }

	bool XPCmdShellEnabled { get; set; }

	bool DefaultBackupCompressionEnabled { get; set; }

	bool ExtensibleKeyManagementEnabled { get; set; }

	[PostConfigurationAction(/*Could not decode attribute arguments.*/)]
	FilestreamAccessLevelType FilestreamAccessLevel { get; set; }

	bool OptimizeAdhocWorkloads { get; set; }

	bool RemoteDataArchiveEnabled { get; set; }
}

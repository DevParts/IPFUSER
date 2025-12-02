using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf.Common;

namespace Microsoft.SqlServer.Management.Smo;

public abstract class ServerAdapterBase : IAlterable, IRefreshable
{
	private Server wrappedObject;

	protected Server Server => wrappedObject;

	public AuditLevel LoginAuditLevel
	{
		get
		{
			return Server.AuditLevel;
		}
		set
		{
			Server.AuditLevel = value;
		}
	}

	public ServerLoginMode LoginMode => Server.LoginMode;

	public string ServiceName => Server.ServiceName;

	public ServiceStartMode ServiceStartMode => Server.ServiceStartMode;

	public string InstanceName => Server.InstanceName;

	public FileStreamEffectiveLevel FilestreamLevel => Server.FilestreamLevel;

	public string FilestreamShareName => Server.FilestreamShareName;

	public string Collation => Server.Collation;

	public string SqlDomainGroup => Server.SqlDomainGroup;

	public string InstallDataDirectory => Server.InstallDataDirectory;

	public string BackupDirectory => Server.BackupDirectory;

	public string DefaultFile => Server.DefaultFile;

	public string DefaultLog => Server.DefaultLog;

	public bool NamedPipesEnabled => Server.NamedPipesEnabled;

	public bool TcpEnabled => Server.TcpEnabled;

	public string InstallSharedDirectory => Server.InstallSharedDirectory;

	public ServiceStartMode BrowserStartMode => Server.BrowserStartMode;

	public string BrowserServiceAccount => Server.BrowserServiceAccount;

	public bool ContainmentEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.ContainmentEnabled.RunValue);
		}
		set
		{
			Server.Configuration.ContainmentEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool AdHocRemoteQueriesEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.AdHocDistributedQueriesEnabled.RunValue);
		}
		set
		{
			Server.Configuration.AdHocDistributedQueriesEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int AffinityMask
	{
		get
		{
			return Server.Configuration.AffinityMask.RunValue;
		}
		set
		{
			Server.Configuration.AffinityMask.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int Affinity64Mask
	{
		get
		{
			return Server.Configuration.Affinity64Mask.RunValue;
		}
		set
		{
			Server.Configuration.Affinity64Mask.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int AffinityIOMask
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("AffinityIOMask", Server.Configuration.AffinityIOMask.ConfigValue, Server.Configuration.AffinityIOMask.RunValue);
			return Server.Configuration.AffinityIOMask.RunValue;
		}
		set
		{
			Server.Configuration.AffinityIOMask.ConfigValue = value;
		}
	}

	public int Affinity64IOMask
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("AffinityIOMask", Server.Configuration.AffinityIOMask.ConfigValue, Server.Configuration.AffinityIOMask.RunValue);
			return Server.Configuration.Affinity64IOMask.RunValue;
		}
		set
		{
			Server.Configuration.Affinity64IOMask.ConfigValue = value;
		}
	}

	public bool AgentXPsEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.AgentXPsEnabled.RunValue);
		}
		set
		{
			Server.Configuration.AgentXPsEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool AllowUpdates
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.AllowUpdates.RunValue);
		}
		set
		{
			Server.Configuration.AllowUpdates.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool AweEnabled
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("AweEnabled", Convert.ToBoolean(Server.Configuration.AweEnabled.ConfigValue), Convert.ToBoolean(Server.Configuration.AweEnabled.RunValue));
			return Convert.ToBoolean(Server.Configuration.AweEnabled.RunValue);
		}
		set
		{
			Server.Configuration.AweEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool C2AuditTracingEnabled
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("C2AuditTracingEnabled", Convert.ToBoolean(Server.Configuration.C2AuditMode.ConfigValue), Convert.ToBoolean(Server.Configuration.C2AuditMode.RunValue));
			return Convert.ToBoolean(Server.Configuration.C2AuditMode.RunValue);
		}
		set
		{
			Server.Configuration.C2AuditMode.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int BlockedProcessThreshold
	{
		get
		{
			return Server.Configuration.BlockedProcessThreshold.RunValue;
		}
		set
		{
			Server.Configuration.BlockedProcessThreshold.ConfigValue = value;
		}
	}

	public bool DefaultBackupCompressionEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.DefaultBackupCompression.RunValue);
		}
		set
		{
			Server.Configuration.DefaultBackupCompression.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool ClrIntegrationEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.IsSqlClrEnabled.RunValue);
		}
		set
		{
			Server.Configuration.IsSqlClrEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool CommonCriteriaComplianceEnabled => Convert.ToBoolean(Server.Configuration.CommonCriteriaComplianceEnabled.RunValue);

	public int CostThresholdForParallelism
	{
		get
		{
			return Server.Configuration.CostThresholdForParallelism.RunValue;
		}
		set
		{
			Server.Configuration.CostThresholdForParallelism.ConfigValue = value;
		}
	}

	public bool CrossDBOwnershipChainingEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.CrossDBOwnershipChaining.RunValue);
		}
		set
		{
			Server.Configuration.CrossDBOwnershipChaining.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int CursorThreshold
	{
		get
		{
			return Server.Configuration.CursorThreshold.RunValue;
		}
		set
		{
			Server.Configuration.CursorThreshold.ConfigValue = value;
		}
	}

	public bool ExtensibleKeyManagementEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.ExtensibleKeyManagementEnabled.RunValue);
		}
		set
		{
			Server.Configuration.ExtensibleKeyManagementEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool DatabaseMailEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.DatabaseMailEnabled.RunValue);
		}
		set
		{
			Server.Configuration.DatabaseMailEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool DefaultTraceEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.DefaultTraceEnabled.RunValue);
		}
		set
		{
			Server.Configuration.DefaultTraceEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int DefaultFullTextLanguage
	{
		get
		{
			return Server.Configuration.DefaultFullTextLanguage.RunValue;
		}
		set
		{
			Server.Configuration.DefaultFullTextLanguage.ConfigValue = value;
		}
	}

	public int DefaultLanguage
	{
		get
		{
			return Server.Configuration.DefaultLanguage.RunValue;
		}
		set
		{
			Server.Configuration.DefaultLanguage.ConfigValue = value;
		}
	}

	public bool DisallowResultsFromTriggers
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.DisallowResultsFromTriggers.RunValue);
		}
		set
		{
			Server.Configuration.DisallowResultsFromTriggers.ConfigValue = Convert.ToInt32(value);
		}
	}

	public FilestreamAccessLevelType FilestreamAccessLevel
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("FilestreamAccessLevel", Server.Configuration.FillFactor.ConfigValue, Server.Configuration.FillFactor.RunValue);
			return (FilestreamAccessLevelType)Server.Configuration.FilestreamAccessLevel.RunValue;
		}
		set
		{
			Server.Configuration.FilestreamAccessLevel.ConfigValue = (int)value;
		}
	}

	public bool OptimizeAdhocWorkloads
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.OptimizeAdhocWorkloads.RunValue);
		}
		set
		{
			Server.Configuration.OptimizeAdhocWorkloads.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int FillFactor
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("FillFactor", Server.Configuration.FillFactor.ConfigValue, Server.Configuration.FillFactor.RunValue);
			return Server.Configuration.FillFactor.RunValue;
		}
		set
		{
			Server.Configuration.FillFactor.ConfigValue = value;
		}
	}

	public int FullTextCrawlBandwidthMin
	{
		get
		{
			return Server.Configuration.FullTextCrawlBandwidthMin.RunValue;
		}
		set
		{
			Server.Configuration.FullTextCrawlBandwidthMin.ConfigValue = value;
		}
	}

	public int FullTextCrawlBandwidthMax
	{
		get
		{
			return Server.Configuration.FullTextCrawlBandwidthMax.RunValue;
		}
		set
		{
			Server.Configuration.FullTextCrawlBandwidthMax.ConfigValue = value;
		}
	}

	public int FullTextNotifyBandwidthMin
	{
		get
		{
			return Server.Configuration.FullTextNotifyBandwidthMin.RunValue;
		}
		set
		{
			Server.Configuration.FullTextNotifyBandwidthMin.ConfigValue = value;
		}
	}

	public int FullTextNotifyBandwidthMax
	{
		get
		{
			return Server.Configuration.FullTextNotifyBandwidthMax.RunValue;
		}
		set
		{
			Server.Configuration.FullTextNotifyBandwidthMax.ConfigValue = value;
		}
	}

	public int FullTextCrawlRangeMax
	{
		get
		{
			return Server.Configuration.FullTextCrawlRangeMax.RunValue;
		}
		set
		{
			Server.Configuration.FullTextCrawlRangeMax.ConfigValue = value;
		}
	}

	public InDoubtTransactionResolutionType InDoubtTransactionResolution
	{
		get
		{
			return (InDoubtTransactionResolutionType)Server.Configuration.InDoubtTransactionResolution.RunValue;
		}
		set
		{
			Server.Configuration.InDoubtTransactionResolution.ConfigValue = (int)value;
		}
	}

	public int IndexCreateMemory
	{
		get
		{
			return Server.Configuration.IndexCreateMemory.RunValue;
		}
		set
		{
			Server.Configuration.IndexCreateMemory.ConfigValue = value;
		}
	}

	public bool LightweightPoolingEnabled
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("LightweightPoolingEnabled", Convert.ToBoolean(Server.Configuration.LightweightPooling.ConfigValue), Convert.ToBoolean(Server.Configuration.LightweightPooling.RunValue));
			return Convert.ToBoolean(Server.Configuration.LightweightPooling.RunValue);
		}
		set
		{
			Server.Configuration.LightweightPooling.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int DynamicLocks
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("DynamicLocks", Server.Configuration.Locks.ConfigValue, Server.Configuration.Locks.RunValue);
			return Server.Configuration.Locks.RunValue;
		}
		set
		{
			Server.Configuration.Locks.ConfigValue = value;
		}
	}

	public int MaxDegreeOfParallelism
	{
		get
		{
			return Server.Configuration.MaxDegreeOfParallelism.RunValue;
		}
		set
		{
			Server.Configuration.MaxDegreeOfParallelism.ConfigValue = value;
		}
	}

	public int MaxServerMemory
	{
		get
		{
			return Server.Configuration.MaxServerMemory.RunValue;
		}
		set
		{
			Server.Configuration.MaxServerMemory.ConfigValue = value;
		}
	}

	public int MaxWorkerThreads
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("MaxWorkerThreads", Server.Configuration.MaxWorkerThreads.ConfigValue, Server.Configuration.MaxWorkerThreads.RunValue);
			return Server.Configuration.MaxWorkerThreads.RunValue;
		}
		set
		{
			Server.Configuration.MaxWorkerThreads.ConfigValue = value;
		}
	}

	public int MediaRetention
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("MediaRetention", Server.Configuration.MediaRetention.ConfigValue, Server.Configuration.MediaRetention.RunValue);
			return Server.Configuration.MediaRetention.RunValue;
		}
		set
		{
			Server.Configuration.MediaRetention.ConfigValue = value;
		}
	}

	public int MinMemoryPerQuery
	{
		get
		{
			return Server.Configuration.MinMemoryPerQuery.RunValue;
		}
		set
		{
			Server.Configuration.MinMemoryPerQuery.ConfigValue = value;
		}
	}

	public int MinServerMemory
	{
		get
		{
			return Server.Configuration.MinServerMemory.RunValue;
		}
		set
		{
			Server.Configuration.MinServerMemory.ConfigValue = value;
		}
	}

	public bool NestedTriggersEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.NestedTriggers.RunValue);
		}
		set
		{
			Server.Configuration.NestedTriggers.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int NetworkPacketSize
	{
		get
		{
			return Server.Configuration.NetworkPacketSize.RunValue;
		}
		set
		{
			Server.Configuration.NetworkPacketSize.ConfigValue = value;
		}
	}

	public bool OleAutomationEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.OleAutomationProceduresEnabled.RunValue);
		}
		set
		{
			Server.Configuration.OleAutomationProceduresEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int OpenObjects
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("OpenObjects", Server.Configuration.OpenObjects.ConfigValue, Server.Configuration.OpenObjects.RunValue);
			return Server.Configuration.OpenObjects.RunValue;
		}
		set
		{
			Server.Configuration.OpenObjects.ConfigValue = value;
		}
	}

	public bool PrecomputeRank
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.PrecomputeRank.RunValue);
		}
		set
		{
			Server.Configuration.PrecomputeRank.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool PriorityBoost
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("PriorityBoost", Convert.ToBoolean(Server.Configuration.PriorityBoost.ConfigValue), Convert.ToBoolean(Server.Configuration.PriorityBoost.RunValue));
			return Convert.ToBoolean(Server.Configuration.PriorityBoost.RunValue);
		}
		set
		{
			Server.Configuration.PriorityBoost.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int ProtocolHandlerTimeout
	{
		get
		{
			return Server.Configuration.ProtocolHandlerTimeout.RunValue;
		}
		set
		{
			Server.Configuration.ProtocolHandlerTimeout.ConfigValue = value;
		}
	}

	public int QueryGovernorCostLimit
	{
		get
		{
			return Server.Configuration.QueryGovernorCostLimit.RunValue;
		}
		set
		{
			Server.Configuration.QueryGovernorCostLimit.ConfigValue = value;
		}
	}

	public int QueryWait
	{
		get
		{
			return Server.Configuration.QueryWait.RunValue;
		}
		set
		{
			Server.Configuration.QueryWait.ConfigValue = value;
		}
	}

	public int RecoveryInterval
	{
		get
		{
			return Server.Configuration.RecoveryInterval.RunValue;
		}
		set
		{
			Server.Configuration.RecoveryInterval.ConfigValue = value;
		}
	}

	public bool RemoteAccessEnabled
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("RemoteAccessEnabled", Convert.ToBoolean(Server.Configuration.RemoteAccess.ConfigValue), Convert.ToBoolean(Server.Configuration.RemoteAccess.RunValue));
			return Convert.ToBoolean(Server.Configuration.RemoteAccess.RunValue);
		}
		set
		{
			Server.Configuration.RemoteAccess.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool RemoteDacEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.RemoteDacConnectionsEnabled.RunValue);
		}
		set
		{
			Server.Configuration.RemoteDacConnectionsEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int RemoteLoginTimeout
	{
		get
		{
			return Server.Configuration.RemoteLoginTimeout.RunValue;
		}
		set
		{
			Server.Configuration.RemoteLoginTimeout.ConfigValue = value;
		}
	}

	public bool RemoteProcTransEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.RemoteProcTrans.RunValue);
		}
		set
		{
			Server.Configuration.RemoteProcTrans.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int RemoteQueryTimeout
	{
		get
		{
			return Server.Configuration.RemoteQueryTimeout.RunValue;
		}
		set
		{
			Server.Configuration.RemoteQueryTimeout.ConfigValue = value;
		}
	}

	public int ReplicationMaxTextSize
	{
		get
		{
			return Server.Configuration.ReplicationMaxTextSize.RunValue;
		}
		set
		{
			Server.Configuration.ReplicationMaxTextSize.ConfigValue = value;
		}
	}

	public bool ReplicationXPsEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.ReplicationXPsEnabled.RunValue);
		}
		set
		{
			Server.Configuration.ReplicationXPsEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool ScanForStartupProcedures
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("ScanForStartupProcedures", Convert.ToBoolean(Server.Configuration.ScanForStartupProcedures.ConfigValue), Convert.ToBoolean(Server.Configuration.ScanForStartupProcedures.RunValue));
			return Convert.ToBoolean(Server.Configuration.ScanForStartupProcedures.RunValue);
		}
		set
		{
			Server.Configuration.ScanForStartupProcedures.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool ServerTriggerRecursionEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.ServerTriggerRecursionEnabled.RunValue);
		}
		set
		{
			Server.Configuration.ServerTriggerRecursionEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool SetWorkingSetSize
	{
		get
		{
			Utils.CheckConfigurationProperty<bool>("SetWorkingSetSize", Convert.ToBoolean(Server.Configuration.SetWorkingSetSize.ConfigValue), Convert.ToBoolean(Server.Configuration.SetWorkingSetSize.RunValue));
			return Convert.ToBoolean(Server.Configuration.SetWorkingSetSize.RunValue);
		}
		set
		{
			Server.Configuration.SetWorkingSetSize.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool ShowAdvancedOptions
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.ShowAdvancedOptions.RunValue);
		}
		set
		{
			Server.Configuration.ShowAdvancedOptions.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool SmoAndDmoXPsEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.SmoAndDmoXPsEnabled.RunValue);
		}
		set
		{
			Server.Configuration.SmoAndDmoXPsEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool SqlMailEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.SqlMailXPsEnabled.RunValue);
		}
		set
		{
			Server.Configuration.SqlMailXPsEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool TransformNoiseWords
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.TransformNoiseWords.RunValue);
		}
		set
		{
			Server.Configuration.TransformNoiseWords.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int TwoDigitYearCutoff
	{
		get
		{
			return Server.Configuration.TwoDigitYearCutoff.RunValue;
		}
		set
		{
			Server.Configuration.TwoDigitYearCutoff.ConfigValue = value;
		}
	}

	public int UserConnections
	{
		get
		{
			Utils.CheckConfigurationProperty<int>("UserConnections", Server.Configuration.UserConnections.ConfigValue, Server.Configuration.UserConnections.RunValue);
			return Server.Configuration.UserConnections.RunValue;
		}
		set
		{
			Server.Configuration.UserConnections.ConfigValue = value;
		}
	}

	public int UserInstanceTimeout
	{
		get
		{
			return Server.Configuration.UserInstanceTimeout.RunValue;
		}
		set
		{
			Server.Configuration.UserInstanceTimeout.ConfigValue = value;
		}
	}

	public bool UserInstancesEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.UserInstancesEnabled.RunValue);
		}
		set
		{
			Server.Configuration.UserInstancesEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public int UserOptions
	{
		get
		{
			return Server.Configuration.UserOptions.RunValue;
		}
		set
		{
			Server.Configuration.UserOptions.ConfigValue = value;
		}
	}

	public bool WebAssistantEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.WebXPsEnabled.RunValue);
		}
		set
		{
			Server.Configuration.WebXPsEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool XPCmdShellEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.XPCmdShellEnabled.RunValue);
		}
		set
		{
			Server.Configuration.XPCmdShellEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public bool RemoteDataArchiveEnabled
	{
		get
		{
			return Convert.ToBoolean(Server.Configuration.RemoteDataArchiveEnabled.RunValue);
		}
		set
		{
			Server.Configuration.RemoteDataArchiveEnabled.ConfigValue = Convert.ToInt32(value);
		}
	}

	public ServerAdapterBase(Server obj)
	{
		wrappedObject = obj;
	}

	public virtual void Refresh()
	{
		Server.Refresh();
	}

	public virtual void Alter()
	{
		Server.Alter(overrideValueChecking: true);
	}
}

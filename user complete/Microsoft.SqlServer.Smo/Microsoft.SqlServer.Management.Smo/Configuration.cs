using System;

namespace Microsoft.SqlServer.Management.Smo;

public class Configuration : ConfigurationBase
{
	private ConfigPropertyCollection m_prop;

	public ConfigPropertyCollection Properties
	{
		get
		{
			if (m_prop == null)
			{
				m_prop = new ConfigPropertyCollection(this);
			}
			return m_prop;
		}
	}

	public ConfigProperty ContainmentEnabled
	{
		get
		{
			if (VersionUtils.IsSql11OrLater(base.Parent.ServerVersion))
			{
				return new ConfigProperty(this, 16393);
			}
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.ContainmentNotSupported(base.Parent.ServerVersion.ToString()));
		}
	}

	public ConfigProperty RecoveryInterval => new ConfigProperty(this, 101);

	public ConfigProperty AllowUpdates => new ConfigProperty(this, 102);

	public ConfigProperty UserConnections => new ConfigProperty(this, 103);

	public ConfigProperty Locks => new ConfigProperty(this, 106);

	public ConfigProperty OpenObjects => new ConfigProperty(this, 107);

	public ConfigProperty FillFactor => new ConfigProperty(this, 109);

	public ConfigProperty NestedTriggers => new ConfigProperty(this, 115);

	public ConfigProperty RemoteAccess => new ConfigProperty(this, 117);

	public ConfigProperty DefaultLanguage => new ConfigProperty(this, 124);

	public ConfigProperty DefaultTraceEnabled => new ConfigProperty(this, 1568);

	public ConfigProperty CrossDBOwnershipChaining => new ConfigProperty(this, 400);

	public ConfigProperty MaxWorkerThreads => new ConfigProperty(this, 503);

	public ConfigProperty NetworkPacketSize => new ConfigProperty(this, 505);

	public ConfigProperty ShowAdvancedOptions => new ConfigProperty(this, 518);

	public ConfigProperty RemoteProcTrans => new ConfigProperty(this, 542);

	public ConfigProperty C2AuditMode => new ConfigProperty(this, 544);

	public ConfigProperty DefaultFullTextLanguage => new ConfigProperty(this, 1126);

	public ConfigProperty TwoDigitYearCutoff => new ConfigProperty(this, 1127);

	public ConfigProperty IndexCreateMemory => new ConfigProperty(this, 1505);

	public ConfigProperty PriorityBoost => new ConfigProperty(this, 1517);

	public ConfigProperty RemoteLoginTimeout => new ConfigProperty(this, 1519);

	public ConfigProperty RemoteQueryTimeout => new ConfigProperty(this, 1520);

	public ConfigProperty CursorThreshold => new ConfigProperty(this, 1531);

	public ConfigProperty SetWorkingSetSize => new ConfigProperty(this, 1532);

	public ConfigProperty UserOptions => new ConfigProperty(this, 1534);

	public ConfigProperty AffinityMask => new ConfigProperty(this, 1535);

	public ConfigProperty ReplicationMaxTextSize => new ConfigProperty(this, 1536);

	public ConfigProperty MediaRetention => new ConfigProperty(this, 1537);

	public ConfigProperty CostThresholdForParallelism => new ConfigProperty(this, 1538);

	public ConfigProperty MaxDegreeOfParallelism => new ConfigProperty(this, 1539);

	public ConfigProperty MinMemoryPerQuery => new ConfigProperty(this, 1540);

	public ConfigProperty QueryWait => new ConfigProperty(this, 1541);

	public ConfigProperty MinServerMemory => new ConfigProperty(this, 1543);

	public ConfigProperty MaxServerMemory => new ConfigProperty(this, 1544);

	public ConfigProperty QueryGovernorCostLimit => new ConfigProperty(this, 1545);

	public ConfigProperty LightweightPooling => new ConfigProperty(this, 1546);

	public ConfigProperty ScanForStartupProcedures => new ConfigProperty(this, 1547);

	[Obsolete]
	public ConfigProperty AweEnabled
	{
		get
		{
			if (!VersionUtils.IsSql11OrLater(base.Parent.ServerVersion))
			{
				return new ConfigProperty(this, 1548);
			}
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.AweEnabledNotSupported(base.Parent.ServerVersion.Major.ToString(SmoApplication.DefaultCulture)));
		}
	}

	public ConfigProperty Affinity64Mask => new ConfigProperty(this, 1549);

	public ConfigProperty AffinityIOMask => new ConfigProperty(this, 1550);

	public ConfigProperty TransformNoiseWords => new ConfigProperty(this, 1555);

	public ConfigProperty PrecomputeRank => new ConfigProperty(this, 1556);

	public ConfigProperty ProtocolHandlerTimeout => new ConfigProperty(this, 1557);

	public ConfigProperty IsSqlClrEnabled => new ConfigProperty(this, 1562);

	public ConfigProperty RemoteDacConnectionsEnabled => new ConfigProperty(this, 1576);

	public ConfigProperty CommonCriteriaComplianceEnabled => new ConfigProperty(this, 1577);

	public ConfigProperty ExtensibleKeyManagementEnabled => new ConfigProperty(this, 1578);

	public ConfigProperty DefaultBackupCompression => new ConfigProperty(this, 1579);

	public ConfigProperty FilestreamAccessLevel => new ConfigProperty(this, 1580);

	public ConfigProperty OptimizeAdhocWorkloads => new ConfigProperty(this, 1581);

	public ConfigProperty DefaultBackupChecksum => new ConfigProperty(this, 1584);

	public ConfigProperty AgentXPsEnabled => new ConfigProperty(this, 16384);

	public ConfigProperty AdHocDistributedQueriesEnabled => new ConfigProperty(this, 16391);

	public ConfigProperty XPCmdShellEnabled => new ConfigProperty(this, 16390);

	public ConfigProperty SmoAndDmoXPsEnabled => new ConfigProperty(this, 16387);

	public ConfigProperty SqlMailXPsEnabled => new ConfigProperty(this, 16385);

	public ConfigProperty DatabaseMailEnabled => new ConfigProperty(this, 16386);

	public ConfigProperty OleAutomationProceduresEnabled => new ConfigProperty(this, 16388);

	public ConfigProperty ReplicationXPsEnabled => new ConfigProperty(this, 16392);

	public ConfigProperty WebXPsEnabled
	{
		get
		{
			Version version = base.Parent.Version;
			if (version.Major < 10)
			{
				return new ConfigProperty(this, 16389);
			}
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(version.Major.ToString()));
		}
	}

	public ConfigProperty BlockedProcessThreshold => new ConfigProperty(this, 1569);

	public ConfigProperty Affinity64IOMask => new ConfigProperty(this, 1551);

	public ConfigProperty DisallowResultsFromTriggers => new ConfigProperty(this, 114);

	public ConfigProperty FullTextCrawlBandwidthMin => new ConfigProperty(this, 1566);

	public ConfigProperty FullTextCrawlBandwidthMax => new ConfigProperty(this, 1567);

	public ConfigProperty FullTextNotifyBandwidthMin => new ConfigProperty(this, 1564);

	public ConfigProperty FullTextNotifyBandwidthMax => new ConfigProperty(this, 1565);

	public ConfigProperty InDoubtTransactionResolution => new ConfigProperty(this, 1570);

	public ConfigProperty FullTextCrawlRangeMax => new ConfigProperty(this, 1563);

	public ConfigProperty ServerTriggerRecursionEnabled => new ConfigProperty(this, 116);

	public ConfigProperty UserInstanceTimeout
	{
		get
		{
			if (base.Parent.IsExpressSku())
			{
				return new ConfigProperty(this, 1573);
			}
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.UnsupportedFeature("UserInstanceTimeout"));
		}
	}

	public ConfigProperty UserInstancesEnabled
	{
		get
		{
			if (base.Parent.IsExpressSku())
			{
				return new ConfigProperty(this, 1575);
			}
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.UnsupportedFeature("UserInstancesEnabled"));
		}
	}

	public ConfigProperty RemoteDataArchiveEnabled => new ConfigProperty(this, 16396);

	internal Configuration(Server server)
		: base(server)
	{
	}
}

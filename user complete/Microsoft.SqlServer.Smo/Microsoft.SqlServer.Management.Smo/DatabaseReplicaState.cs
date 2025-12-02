using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
public sealed class DatabaseReplicaState : SqlSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 37, 37, 37, 37, 37 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

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
					return -1;
				}
				return -1;
			}
			return propertyName switch
			{
				"AvailabilityDatabaseName" => 0, 
				"AvailabilityDateabaseId" => 1, 
				"AvailabilityGroupId" => 2, 
				"AvailabilityGroupName" => 3, 
				"AvailabilityReplicaId" => 4, 
				"AvailabilityReplicaServerName" => 5, 
				"DatabaseId" => 6, 
				"EndOfLogLSN" => 7, 
				"EstimatedDataLoss" => 8, 
				"EstimatedRecoveryTime" => 9, 
				"FileStreamSendRate" => 10, 
				"IsFailoverReady" => 11, 
				"IsJoined" => 12, 
				"IsLocal" => 13, 
				"IsSuspended" => 14, 
				"LastCommitLSN" => 15, 
				"LastCommitTime" => 16, 
				"LastHardenedLSN" => 17, 
				"LastHardenedTime" => 18, 
				"LastReceivedLSN" => 19, 
				"LastReceivedTime" => 20, 
				"LastRedoneLSN" => 21, 
				"LastRedoneTime" => 22, 
				"LastSentLSN" => 23, 
				"LastSentTime" => 24, 
				"LogSendQueueSize" => 25, 
				"LogSendRate" => 26, 
				"PolicyHealthState" => 27, 
				"RecoveryLSN" => 28, 
				"RedoQueueSize" => 29, 
				"RedoRate" => 30, 
				"ReplicaAvailabilityMode" => 31, 
				"ReplicaRole" => 32, 
				"SuspendReason" => 33, 
				"SynchronizationPerformance" => 34, 
				"SynchronizationState" => 35, 
				"TruncationLSN" => 36, 
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

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[37]
			{
				new StaticMetadata("AvailabilityDatabaseName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("AvailabilityDateabaseId", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("AvailabilityGroupId", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("AvailabilityGroupName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("AvailabilityReplicaId", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("AvailabilityReplicaServerName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("DatabaseId", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("EndOfLogLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("EstimatedDataLoss", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("EstimatedRecoveryTime", expensive: false, readOnly: true, typeof(double)),
				new StaticMetadata("FileStreamSendRate", expensive: false, readOnly: true, typeof(long)),
				new StaticMetadata("IsFailoverReady", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsJoined", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsLocal", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsSuspended", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("LastCommitLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("LastCommitTime", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastHardenedLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("LastHardenedTime", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastReceivedLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("LastReceivedTime", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastRedoneLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("LastRedoneTime", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LastSentLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("LastSentTime", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LogSendQueueSize", expensive: false, readOnly: true, typeof(long)),
				new StaticMetadata("LogSendRate", expensive: false, readOnly: true, typeof(long)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("RecoveryLSN", expensive: false, readOnly: true, typeof(decimal)),
				new StaticMetadata("RedoQueueSize", expensive: false, readOnly: true, typeof(long)),
				new StaticMetadata("RedoRate", expensive: false, readOnly: true, typeof(long)),
				new StaticMetadata("ReplicaAvailabilityMode", expensive: false, readOnly: false, typeof(AvailabilityReplicaAvailabilityMode)),
				new StaticMetadata("ReplicaRole", expensive: false, readOnly: true, typeof(AvailabilityReplicaRole)),
				new StaticMetadata("SuspendReason", expensive: false, readOnly: true, typeof(DatabaseReplicaSuspendReason)),
				new StaticMetadata("SynchronizationPerformance", expensive: false, readOnly: true, typeof(double)),
				new StaticMetadata("SynchronizationState", expensive: false, readOnly: true, typeof(AvailabilityDatabaseSynchronizationState)),
				new StaticMetadata("TruncationLSN", expensive: false, readOnly: true, typeof(decimal))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public AvailabilityGroup Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as AvailabilityGroup;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid AvailabilityDateabaseId => (Guid)base.Properties.GetValueWithNullReplacement("AvailabilityDateabaseId");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid AvailabilityGroupId => (Guid)base.Properties.GetValueWithNullReplacement("AvailabilityGroupId");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string AvailabilityGroupName => (string)base.Properties.GetValueWithNullReplacement("AvailabilityGroupName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid AvailabilityReplicaId => (Guid)base.Properties.GetValueWithNullReplacement("AvailabilityReplicaId");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int DatabaseId => (int)base.Properties.GetValueWithNullReplacement("DatabaseId");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal EndOfLogLSN => (decimal)base.Properties.GetValueWithNullReplacement("EndOfLogLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int EstimatedDataLoss => (int)base.Properties.GetValueWithNullReplacement("EstimatedDataLoss");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public double EstimatedRecoveryTime => (double)base.Properties.GetValueWithNullReplacement("EstimatedRecoveryTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public long FileStreamSendRate => (long)base.Properties.GetValueWithNullReplacement("FileStreamSendRate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsFailoverReady => (bool)base.Properties.GetValueWithNullReplacement("IsFailoverReady");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsJoined => (bool)base.Properties.GetValueWithNullReplacement("IsJoined");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsLocal => (bool)base.Properties.GetValueWithNullReplacement("IsLocal");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSuspended => (bool)base.Properties.GetValueWithNullReplacement("IsSuspended");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal LastCommitLSN => (decimal)base.Properties.GetValueWithNullReplacement("LastCommitLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastCommitTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastCommitTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal LastHardenedLSN => (decimal)base.Properties.GetValueWithNullReplacement("LastHardenedLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastHardenedTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastHardenedTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal LastReceivedLSN => (decimal)base.Properties.GetValueWithNullReplacement("LastReceivedLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastReceivedTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastReceivedTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal LastRedoneLSN => (decimal)base.Properties.GetValueWithNullReplacement("LastRedoneLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastRedoneTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastRedoneTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal LastSentLSN => (decimal)base.Properties.GetValueWithNullReplacement("LastSentLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastSentTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastSentTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public long LogSendQueueSize => (long)base.Properties.GetValueWithNullReplacement("LogSendQueueSize");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public long LogSendRate => (long)base.Properties.GetValueWithNullReplacement("LogSendRate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal RecoveryLSN => (decimal)base.Properties.GetValueWithNullReplacement("RecoveryLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public long RedoQueueSize => (long)base.Properties.GetValueWithNullReplacement("RedoQueueSize");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public long RedoRate => (long)base.Properties.GetValueWithNullReplacement("RedoRate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaAvailabilityMode ReplicaAvailabilityMode
	{
		get
		{
			return (AvailabilityReplicaAvailabilityMode)base.Properties.GetValueWithNullReplacement("ReplicaAvailabilityMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReplicaAvailabilityMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityReplicaRole ReplicaRole => (AvailabilityReplicaRole)base.Properties.GetValueWithNullReplacement("ReplicaRole");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DatabaseReplicaSuspendReason SuspendReason => (DatabaseReplicaSuspendReason)base.Properties.GetValueWithNullReplacement("SuspendReason");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public double SynchronizationPerformance => (double)base.Properties.GetValueWithNullReplacement("SynchronizationPerformance");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AvailabilityDatabaseSynchronizationState SynchronizationState => (AvailabilityDatabaseSynchronizationState)base.Properties.GetValueWithNullReplacement("SynchronizationState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public decimal TruncationLSN => (decimal)base.Properties.GetValueWithNullReplacement("TruncationLSN");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	[SfcKey(0)]
	public string AvailabilityReplicaServerName => (string)base.Properties.GetValueWithNullReplacement("AvailabilityReplicaServerName");

	[SfcKey(1)]
	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string AvailabilityDatabaseName => (string)base.Properties.GetValueWithNullReplacement("AvailabilityDatabaseName");

	public static string UrnSuffix => "DatabaseReplicaState";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal DatabaseReplicaState(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}
}

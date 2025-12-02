using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class TargetServer : AgentObjectBase
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 };

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
				"EnlistDate" => 0, 
				"ID" => 1, 
				"LastPollDate" => 2, 
				"LocalTime" => 3, 
				"Location" => 4, 
				"PendingInstructions" => 5, 
				"PollingInterval" => 6, 
				"Status" => 7, 
				"TimeZoneAdjustment" => 8, 
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
			staticMetadata = new StaticMetadata[9]
			{
				new StaticMetadata("EnlistDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("LastPollDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("LocalTime", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("Location", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("PendingInstructions", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("PollingInterval", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("Status", expensive: false, readOnly: true, typeof(TargetServerStatus)),
				new StaticMetadata("TimeZoneAdjustment", expensive: false, readOnly: true, typeof(int))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public JobServer Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as JobServer;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime EnlistDate => (DateTime)base.Properties.GetValueWithNullReplacement("EnlistDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastPollDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastPollDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LocalTime => (DateTime)base.Properties.GetValueWithNullReplacement("LocalTime");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Location => (string)base.Properties.GetValueWithNullReplacement("Location");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int PendingInstructions => (int)base.Properties.GetValueWithNullReplacement("PendingInstructions");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int PollingInterval => (int)base.Properties.GetValueWithNullReplacement("PollingInterval");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public TargetServerStatus Status => (TargetServerStatus)base.Properties.GetValueWithNullReplacement("Status");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int TimeZoneAdjustment => (int)base.Properties.GetValueWithNullReplacement("TimeZoneAdjustment");

	public static string UrnSuffix => "TargetServer";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal TargetServer(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}
}

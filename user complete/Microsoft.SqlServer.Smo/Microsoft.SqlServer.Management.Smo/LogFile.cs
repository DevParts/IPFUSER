using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class LogFile : DatabaseFile, ISfcSupportsDesignMode, ICreatable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 7, 7, 15, 16, 17, 17, 17, 17, 17, 17 };

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
				"FileName" => 0, 
				"Growth" => 1, 
				"GrowthType" => 2, 
				"ID" => 3, 
				"MaxSize" => 4, 
				"Size" => 5, 
				"UsedSpace" => 6, 
				"BytesReadFromDisk" => 7, 
				"BytesWrittenToDisk" => 8, 
				"IsOffline" => 9, 
				"IsReadOnly" => 10, 
				"IsReadOnlyMedia" => 11, 
				"IsSparse" => 12, 
				"NumberOfDiskReads" => 13, 
				"NumberOfDiskWrites" => 14, 
				"PolicyHealthState" => 15, 
				"VolumeFreeSpace" => 16, 
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
			staticMetadata = new StaticMetadata[17]
			{
				new StaticMetadata("FileName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Growth", expensive: false, readOnly: false, typeof(double)),
				new StaticMetadata("GrowthType", expensive: false, readOnly: false, typeof(FileGrowthType)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("MaxSize", expensive: false, readOnly: false, typeof(double)),
				new StaticMetadata("Size", expensive: false, readOnly: false, typeof(double)),
				new StaticMetadata("UsedSpace", expensive: true, readOnly: true, typeof(double)),
				new StaticMetadata("BytesReadFromDisk", expensive: true, readOnly: true, typeof(long)),
				new StaticMetadata("BytesWrittenToDisk", expensive: true, readOnly: true, typeof(long)),
				new StaticMetadata("IsOffline", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsReadOnly", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsReadOnlyMedia", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsSparse", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("NumberOfDiskReads", expensive: true, readOnly: true, typeof(long)),
				new StaticMetadata("NumberOfDiskWrites", expensive: true, readOnly: true, typeof(long)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("VolumeFreeSpace", expensive: true, readOnly: true, typeof(long))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Database;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public long BytesReadFromDisk => (long)base.Properties.GetValueWithNullReplacement("BytesReadFromDisk");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public long BytesWrittenToDisk => (long)base.Properties.GetValueWithNullReplacement("BytesWrittenToDisk");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string FileName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public double Growth
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("Growth");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Growth", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public FileGrowthType GrowthType
	{
		get
		{
			return (FileGrowthType)base.Properties.GetValueWithNullReplacement("GrowthType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("GrowthType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsOffline => (bool)base.Properties.GetValueWithNullReplacement("IsOffline");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsReadOnly => (bool)base.Properties.GetValueWithNullReplacement("IsReadOnly");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsReadOnlyMedia => (bool)base.Properties.GetValueWithNullReplacement("IsReadOnlyMedia");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSparse => (bool)base.Properties.GetValueWithNullReplacement("IsSparse");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public double MaxSize
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("MaxSize");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaxSize", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public long NumberOfDiskReads => (long)base.Properties.GetValueWithNullReplacement("NumberOfDiskReads");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public long NumberOfDiskWrites => (long)base.Properties.GetValueWithNullReplacement("NumberOfDiskWrites");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public double Size
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("Size");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Size", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double UsedSpace => (double)base.Properties.GetValueWithNullReplacement("UsedSpace");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public long VolumeFreeSpace => (long)base.Properties.GetValueWithNullReplacement("VolumeFreeSpace");

	public static string UrnSuffix => "LogFile";

	public LogFile()
	{
	}

	public LogFile(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "FileName" };
	}

	internal LogFile(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public LogFile(Database database, string name, string fileName)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
		FileName = fileName;
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		bool suppressDirtyCheck = sp.SuppressDirtyCheck;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] ADD LOG FILE ", new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) });
		if (ScriptDdl(sp, stringBuilder, suppressDirtyCheck, scriptCreate: true))
		{
			createQuery.Add(stringBuilder.ToString());
		}
	}
}

using System;
using System.Collections.Generic;
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
[SfcElementType("File")]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class DataFile : DatabaseFile, ISfcSupportsDesignMode, ICreatable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 9, 17, 18, 19, 19, 19, 19, 19, 19 };

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
				"AvailableSpace" => 0, 
				"FileName" => 1, 
				"Growth" => 2, 
				"GrowthType" => 3, 
				"ID" => 4, 
				"IsPrimaryFile" => 5, 
				"MaxSize" => 6, 
				"Size" => 7, 
				"UsedSpace" => 8, 
				"BytesReadFromDisk" => 9, 
				"BytesWrittenToDisk" => 10, 
				"IsOffline" => 11, 
				"IsReadOnly" => 12, 
				"IsReadOnlyMedia" => 13, 
				"IsSparse" => 14, 
				"NumberOfDiskReads" => 15, 
				"NumberOfDiskWrites" => 16, 
				"PolicyHealthState" => 17, 
				"VolumeFreeSpace" => 18, 
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
			staticMetadata = new StaticMetadata[19]
			{
				new StaticMetadata("AvailableSpace", expensive: true, readOnly: true, typeof(double)),
				new StaticMetadata("FileName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Growth", expensive: false, readOnly: false, typeof(double)),
				new StaticMetadata("GrowthType", expensive: false, readOnly: false, typeof(FileGrowthType)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsPrimaryFile", expensive: false, readOnly: false, typeof(bool)),
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
	public FileGroup Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as FileGroup;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double AvailableSpace => (double)base.Properties.GetValueWithNullReplacement("AvailableSpace");

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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public bool IsPrimaryFile
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsPrimaryFile");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsPrimaryFile", value);
		}
	}

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

	public static string UrnSuffix => "File";

	public DataFile()
	{
	}

	public DataFile(FileGroup fileGroup, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = fileGroup;
		InitializeDefaults();
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[2] { "FileName", "IsPrimaryFile" };
	}

	internal DataFile(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public DataFile(FileGroup fileGroup, string name, string fileName)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = fileGroup;
		FileName = fileName;
	}

	private void InitializeDefaults()
	{
		IsPrimaryFile = false;
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		bool suppressDirtyCheck = sp.SuppressDirtyCheck;
		StringBuilder stringBuilder = new StringBuilder();
		string internalName = base.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName;
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] ADD FILE ", new object[1] { SqlSmoObject.SqlBraket(internalName) });
		if (ScriptDdl(sp, stringBuilder, suppressDirtyCheck, scriptCreate: true))
		{
			if (internalName.ToLower(SmoApplication.DefaultCulture) != "tempdb")
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TO FILEGROUP [{0}]", new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) });
			}
			createQuery.Add(stringBuilder.ToString());
		}
	}

	internal static void Validate_set_IsPrimaryFile(Property prop, object newValue)
	{
		DataFile dataFile = (DataFile)prop.Parent.m_parent;
		if (dataFile.State != SqlSmoState.Creating)
		{
			throw new PropertyReadOnlyException(prop.Name);
		}
		if (string.Compare(dataFile.Parent.Name, "PRIMARY", StringComparison.Ordinal) != 0 && (bool)newValue)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotChangePrimary);
		}
		if (!(bool)newValue)
		{
			return;
		}
		SmoInternalStorage internalStorage = ((DataFileCollection)dataFile.ParentColl).InternalStorage;
		foreach (DataFile item in internalStorage)
		{
			Property property = item.Properties.Get("IsPrimaryFile");
			if (dataFile != item && property.Value != null && (bool)property.Value)
			{
				throw new SmoException(ExceptionTemplatesImpl.OnlyOnePrimaryFile);
			}
		}
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "IsPrimaryFile")
		{
			Validate_set_IsPrimaryFile(prop, value);
		}
	}

	public void SetOffline()
	{
		try
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILE (NAME=N'{1}', OFFLINE)", new object[2]
			{
				SqlSmoObject.SqlBraket(Parent.Parent.Name),
				SqlSmoObject.SqlString(Name)
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetOffline, this, ex);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[1] { "IsPrimaryFile" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(Database.PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

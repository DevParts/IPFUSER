using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[PhysicalFacet]
public sealed class FileGroup : NamedSmoObject, ISfcSupportsDesignMode, ICreatable, IAlterable, IDroppable, IRenamable, IMarkForDrop
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 4, 4, 6, 7, 7, 7, 7, 8, 8, 8 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 6 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("FileGroupType", expensive: false, readOnly: false, typeof(FileGroupType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsFileStream", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Size", expensive: false, readOnly: false, typeof(double))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[8]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Size", expensive: false, readOnly: true, typeof(double)),
			new StaticMetadata("FileGroupType", expensive: false, readOnly: false, typeof(FileGroupType)),
			new StaticMetadata("IsFileStream", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("AutogrowAllFiles", expensive: false, readOnly: false, typeof(bool))
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
					return -1;
				}
				return propertyName switch
				{
					"FileGroupType" => 0, 
					"ID" => 1, 
					"IsDefault" => 2, 
					"IsFileStream" => 3, 
					"ReadOnly" => 4, 
					"Size" => 5, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"IsDefault" => 1, 
				"ReadOnly" => 2, 
				"Size" => 3, 
				"FileGroupType" => 4, 
				"IsFileStream" => 5, 
				"PolicyHealthState" => 6, 
				"AutogrowAllFiles" => 7, 
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

	private DataFileCollection m_Files;

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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AutogrowAllFiles
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AutogrowAllFiles");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AutogrowAllFiles", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public FileGroupType FileGroupType
	{
		get
		{
			return (FileGroupType)base.Properties.GetValueWithNullReplacement("FileGroupType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileGroupType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDefault
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDefault");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFileStream
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsFileStream");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsFileStream", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double Size => (double)base.Properties.GetValueWithNullReplacement("Size");

	public static string UrnSuffix => "FileGroup";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(DataFile))]
	public DataFileCollection Files
	{
		get
		{
			CheckObjectState();
			if (m_Files == null)
			{
				m_Files = new DataFileCollection(this);
			}
			return m_Files;
		}
	}

	public FileGroup()
	{
	}

	public FileGroup(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
		InitializeDefaults();
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[2] { "FileGroupType", "IsFileStream" };
	}

	internal FileGroup(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_Files = null;
	}

	internal override void ValidateName(string name)
	{
		base.ValidateName(name);
		if (name.Length == 0)
		{
			throw new UnsupportedObjectNameException(ExceptionTemplatesImpl.UnsupportedObjectNameExceptionText(ExceptionTemplatesImpl.FileGroup));
		}
	}

	public FileGroup(Database database, string name, FileGroupType fileGroupType)
		: this(database, name)
	{
		FileGroupType = fileGroupType;
		IsFileStream = fileGroupType == FileGroupType.FileStreamDataFileGroup;
		if (IsSupportedProperty("AutogrowAllFiles"))
		{
			AutogrowAllFiles = false;
		}
	}

	private void InitializeDefaults()
	{
		FileGroupType = FileGroupType.RowsFileGroup;
		IsDefault = false;
		IsFileStream = false;
		if (IsSupportedProperty("AutogrowAllFiles"))
		{
			AutogrowAllFiles = false;
		}
	}

	public FileGroup(Database database, string name, bool isFileStream)
		: this(database, name, isFileStream ? FileGroupType.FileStreamDataFileGroup : FileGroupType.RowsFileGroup)
	{
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_Files != null)
		{
			m_Files.MarkAllDropped();
		}
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		if (base.ServerVersion.Major == 7)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotRenameObject("FileGroup", base.ServerVersion.ToString()));
		}
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] MODIFY FILEGROUP [{1}]  NAME = [{2}]", new object[3]
		{
			SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName),
			SqlSmoObject.SqlBraket(Name),
			SqlSmoObject.SqlBraket(newName)
		}));
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string internalName = base.ParentColl.ParentInstance.InternalName;
		string name = Name;
		int num = 0;
		Property property = base.Properties.Get("ReadOnly");
		if (property.Dirty && property.Value != null)
		{
			bool flag = (bool)property.Value;
			stringBuilder.Length = 0;
			string format = "declare @readonly bit{5}SELECT @readonly=convert(bit, (status & 0x08)) FROM sysfilegroups WHERE groupname=N'{0}'{5}if(@readonly={1}){5}\tALTER DATABASE [{2}] MODIFY FILEGROUP [{3}] {4}";
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, format, SqlSmoObject.SqlString(name), (!flag) ? 1 : 0, SqlSmoObject.SqlBraket(internalName), SqlSmoObject.SqlBraket(name), flag ? "READONLY" : "READWRITE", sp.NewLine);
			alterQuery.Add(stringBuilder.ToString());
			num++;
		}
		if (IsSupportedProperty("AutogrowAllFiles", sp))
		{
			property = base.Properties.Get("AutogrowAllFiles");
			if (property.Dirty && property.Value != null)
			{
				bool flag2 = (bool)property.Value;
				stringBuilder.Length = 0;
				string format2 = "declare @autogrow bit\r\nSELECT @autogrow=convert(bit, is_autogrow_all_files) FROM sys.filegroups WHERE name=N'{0}'\r\nif(@autogrow={1})\r\n\tALTER DATABASE [{2}] MODIFY FILEGROUP [{3}] {4}";
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, format2, SqlSmoObject.SqlString(name), (!flag2) ? 1 : 0, SqlSmoObject.SqlBraket(internalName), SqlSmoObject.SqlBraket(name), flag2 ? "AUTOGROW_ALL_FILES" : "AUTOGROW_SINGLE_FILE");
				alterQuery.Add(stringBuilder.ToString());
				num++;
			}
		}
		property = base.Properties.Get("IsDefault");
		if (property.Dirty && property.Value != null)
		{
			if (!(bool)property.Value)
			{
				throw new SmoException(ExceptionTemplatesImpl.CantSetDefaultFalse);
			}
			stringBuilder.Length = 0;
			string format3 = "declare @isdefault bit{3}SELECT @isdefault=convert(bit, (status & 0x10)) FROM sysfilegroups WHERE groupname=N'{0}'{3}if(@isdefault=0){3}\tALTER DATABASE [{1}] MODIFY FILEGROUP [{2}] DEFAULT";
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, format3, SqlSmoObject.SqlString(name), SqlSmoObject.SqlBraket(internalName), SqlSmoObject.SqlBraket(name), sp.NewLine);
			alterQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(internalName) }));
			alterQuery.Add(stringBuilder.ToString());
			num++;
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		_ = sp.SuppressDirtyCheck;
		StringBuilder stringBuilder = new StringBuilder();
		Database database = (Database)base.ParentColl.ParentInstance;
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER DATABASE {0} ADD FILEGROUP [{1}]", new object[2]
		{
			database.FormatFullNameForScripting(sp),
			SqlSmoObject.SqlBraket(Name)
		});
		switch (FileGroupType)
		{
		case FileGroupType.FileStreamDataFileGroup:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " CONTAINS FILESTREAM ");
			break;
		case FileGroupType.MemoryOptimizedDataFileGroup:
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " CONTAINS MEMORY_OPTIMIZED_DATA ");
			break;
		}
		createQuery.Add(stringBuilder.ToString());
	}

	public void Drop()
	{
		DropImpl();
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		for (int i = 0; i < Files.Count; i++)
		{
			DataFile dataFile = Files[i];
			dataFile.ScriptDropInternal(dropQuery, sp);
		}
		dropQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] REMOVE FILEGROUP [{1}]", new object[2]
		{
			SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName),
			SqlSmoObject.SqlBraket(Name)
		}));
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	internal void ScriptDdl(ScriptingPreferences sp, StringBuilder ddl)
	{
		ScriptDdl(sp, ddl, databaseIsView: false);
	}

	private void ScriptFileGroupFiles(ScriptingPreferences sp, StringBuilder ddl, bool databaseIsView)
	{
		bool flag = false;
		foreach (DataFile file in Files)
		{
			if (file.GetPropValueOptional("IsPrimaryFile", defaultValue: false))
			{
				ddl.Append(Globals.newline);
				GetFileScriptWithCheck(sp, file, ddl, databaseIsView);
				flag = true;
			}
		}
		foreach (DataFile file2 in Files)
		{
			if (!file2.GetPropValueOptional("IsPrimaryFile", defaultValue: false))
			{
				if (flag)
				{
					ddl.Append(Globals.comma + Globals.newline);
				}
				else
				{
					ddl.Append(Globals.newline);
				}
				GetFileScriptWithCheck(sp, file2, ddl, databaseIsView);
				flag = true;
			}
		}
	}

	private void ScriptPrimaryFileGroup(ScriptingPreferences sp, StringBuilder ddl, bool databaseIsView)
	{
		if (Files.Count == 0)
		{
			if (base.State == SqlSmoState.Existing)
			{
				throw new SmoException(ExceptionTemplatesImpl.NotEnoughRights);
			}
			throw new SmoException(ExceptionTemplatesImpl.PrimaryFgMustHaveFiles);
		}
		if (!databaseIsView)
		{
			ddl.Append(" PRIMARY ");
		}
		if (base.Properties.Get("IsDefault").Value != null && base.Properties["IsDefault"].Dirty && IsDefault && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			throw new SmoException(ExceptionTemplatesImpl.PrimaryAlreadyDefault);
		}
	}

	internal void ScriptDdl(ScriptingPreferences sp, StringBuilder ddl, bool databaseIsView)
	{
		string name = Name;
		base.Properties.Get("IsDefault");
		if (string.Compare(name, "PRIMARY", StringComparison.Ordinal) == 0)
		{
			ScriptPrimaryFileGroup(sp, ddl, databaseIsView);
		}
		else if (!databaseIsView)
		{
			string text = string.Empty;
			string text2 = (IsDefault ? " DEFAULT" : string.Empty);
			string text3 = string.Format(SmoApplication.DefaultCulture, " FILEGROUP [{0}] ", new object[1] { SqlSmoObject.SqlBraket(name) });
			switch (FileGroupType)
			{
			case FileGroupType.FileStreamDataFileGroup:
				text = "CONTAINS FILESTREAM ";
				break;
			case FileGroupType.MemoryOptimizedDataFileGroup:
				text = "CONTAINS MEMORY_OPTIMIZED_DATA ";
				break;
			}
			ddl.Append(text3 + text + text2);
		}
		ScriptFileGroupFiles(sp, ddl, databaseIsView);
	}

	internal static void GetFileScriptWithCheck(ScriptingPreferences sp, DatabaseFile df, StringBuilder ddl, bool databaseIsView)
	{
		if (!df.ScriptDdl(sp, ddl, bSuppressDirtyCheck: true, scriptCreate: true, databaseIsView ? "FileName" : ""))
		{
			throw new SmoException(ExceptionTemplatesImpl.NoSqlGen(df.Urn.ToString()));
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo(Files, (base.ParentColl.ParentInstance.State != SqlSmoState.Creating || action != PropagateAction.Create) && base.State != SqlSmoState.ToBeDropped)
		};
	}

	public StringCollection CheckFileGroup()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKFILEGROUP( N'{0}' ) WITH NO_INFOMSGS", new object[1] { SqlSmoObject.SqlString(Name) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckFileGroup, this, ex);
		}
	}

	public StringCollection CheckFileGroupDataOnly()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKFILEGROUP( N'{0}', NOINDEX ) WITH NO_INFOMSGS", new object[1] { SqlSmoObject.SqlString(Name) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckFileGroup, this, ex);
		}
	}

	private void AddObjects(string partialUrn, ArrayList list)
	{
		string text = base.ParentColl.ParentInstance.Urn;
		string text2 = string.Format(SmoApplication.DefaultCulture, "[@FileGroup='{0}']", new object[1] { Urn.EscapeString(Name) });
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(new Request(text + partialUrn + text2, new string[1] { "Urn" }));
		foreach (DataRow row in enumeratorData.Rows)
		{
			list.Add((string)row[0]);
		}
	}

	public SqlSmoObject[] EnumObjects()
	{
		CheckObjectState();
		ArrayList arrayList = new ArrayList();
		AddObjects("/Table", arrayList);
		AddObjects("/Table/Index", arrayList);
		AddObjects("/View/Index", arrayList);
		AddObjects("/Table/Statistic", arrayList);
		SqlSmoObject[] array = new SqlSmoObject[arrayList.Count];
		int num = 0;
		Server serverObject = GetServerObject();
		foreach (object item in arrayList)
		{
			array[num++] = serverObject.GetSmoObject((string)item);
		}
		return array;
	}
}

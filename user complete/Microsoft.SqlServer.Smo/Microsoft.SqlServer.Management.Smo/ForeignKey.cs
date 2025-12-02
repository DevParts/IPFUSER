using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class ForeignKey : ScriptNameObjectBase, ISfcSupportsDesignMode, IPropertyDataDispatch, ICreatable, IDroppable, IDropIfExists, IMarkForDrop, IAlterable, IRenamable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 11, 11, 12, 12, 12, 13, 14, 14, 14, 14 };

		private static int[] cloudVersionCount = new int[3] { 12, 12, 13 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[13]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DeleteAction", expensive: false, readOnly: false, typeof(ForeignKeyAction)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsChecked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReferencedKey", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ReferencedTable", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ReferencedTableSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UpdateAction", expensive: false, readOnly: false, typeof(ForeignKeyAction)),
			new StaticMetadata("IsMemoryOptimized", expensive: true, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[14]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DeleteAction", expensive: false, readOnly: false, typeof(ForeignKeyAction)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsChecked", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReferencedKey", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ReferencedTable", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ReferencedTableSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UpdateAction", expensive: false, readOnly: false, typeof(ForeignKeyAction)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("IsFileTableDefined", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsMemoryOptimized", expensive: true, readOnly: false, typeof(bool))
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
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"DeleteAction" => 2, 
					"ID" => 3, 
					"IsChecked" => 4, 
					"IsEnabled" => 5, 
					"IsSystemNamed" => 6, 
					"NotForReplication" => 7, 
					"ReferencedKey" => 8, 
					"ReferencedTable" => 9, 
					"ReferencedTableSchema" => 10, 
					"UpdateAction" => 11, 
					"IsMemoryOptimized" => 12, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DeleteAction" => 1, 
				"ID" => 2, 
				"IsChecked" => 3, 
				"IsEnabled" => 4, 
				"IsSystemNamed" => 5, 
				"NotForReplication" => 6, 
				"ReferencedKey" => 7, 
				"ReferencedTable" => 8, 
				"ReferencedTableSchema" => 9, 
				"UpdateAction" => 10, 
				"DateLastModified" => 11, 
				"IsFileTableDefined" => 12, 
				"IsMemoryOptimized" => 13, 
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

	private sealed class XSchemaProps
	{
		private ForeignKeyAction _DeleteAction;

		private bool _IsChecked;

		private bool _IsEnabled;

		private bool _IsSystemNamed;

		private bool _NotForReplication;

		private string _ReferencedTable;

		private string _ReferencedTableSchema;

		private ForeignKeyAction _UpdateAction;

		internal ForeignKeyAction DeleteAction
		{
			get
			{
				return _DeleteAction;
			}
			set
			{
				_DeleteAction = value;
			}
		}

		internal bool IsChecked
		{
			get
			{
				return _IsChecked;
			}
			set
			{
				_IsChecked = value;
			}
		}

		internal bool IsEnabled
		{
			get
			{
				return _IsEnabled;
			}
			set
			{
				_IsEnabled = value;
			}
		}

		internal bool IsSystemNamed
		{
			get
			{
				return _IsSystemNamed;
			}
			set
			{
				_IsSystemNamed = value;
			}
		}

		internal bool NotForReplication
		{
			get
			{
				return _NotForReplication;
			}
			set
			{
				_NotForReplication = value;
			}
		}

		internal string ReferencedTable
		{
			get
			{
				return _ReferencedTable;
			}
			set
			{
				_ReferencedTable = value;
			}
		}

		internal string ReferencedTableSchema
		{
			get
			{
				return _ReferencedTableSchema;
			}
			set
			{
				_ReferencedTableSchema = value;
			}
		}

		internal ForeignKeyAction UpdateAction
		{
			get
			{
				return _UpdateAction;
			}
			set
			{
				_UpdateAction = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private DateTime _CreateDate;

		private DateTime _DateLastModified;

		private int _ID;

		private bool _IsFileTableDefined;

		private bool _IsMemoryOptimized;

		private string _ReferencedKey;

		internal DateTime CreateDate
		{
			get
			{
				return _CreateDate;
			}
			set
			{
				_CreateDate = value;
			}
		}

		internal DateTime DateLastModified
		{
			get
			{
				return _DateLastModified;
			}
			set
			{
				_DateLastModified = value;
			}
		}

		internal int ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		internal bool IsFileTableDefined
		{
			get
			{
				return _IsFileTableDefined;
			}
			set
			{
				_IsFileTableDefined = value;
			}
		}

		internal bool IsMemoryOptimized
		{
			get
			{
				return _IsMemoryOptimized;
			}
			set
			{
				_IsMemoryOptimized = value;
			}
		}

		internal string ReferencedKey
		{
			get
			{
				return _ReferencedKey;
			}
			set
			{
				_ReferencedKey = value;
			}
		}
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	private ForeignKeyColumnCollection m_Columns;

	private string m_sReferencedTable = string.Empty;

	private string m_sReferencedTableSchema = string.Empty;

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	public Table Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Table;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	private XSchemaProps XSchema
	{
		get
		{
			if (_XSchema == null)
			{
				_XSchema = new XSchemaProps();
			}
			return _XSchema;
		}
	}

	private XRuntimeProps XRuntime
	{
		get
		{
			if (_XRuntime == null)
			{
				_XRuntime = new XRuntimeProps();
			}
			return _XRuntime;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public ForeignKeyAction DeleteAction
	{
		get
		{
			return (ForeignKeyAction)base.Properties.GetValueWithNullReplacement("DeleteAction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DeleteAction", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsChecked
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsChecked");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsChecked", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsFileTableDefined => (bool)base.Properties.GetValueWithNullReplacement("IsFileTableDefined");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsMemoryOptimized
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsMemoryOptimized");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsMemoryOptimized", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool NotForReplication
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NotForReplication");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NotForReplication", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ReferencedKey => (string)base.Properties.GetValueWithNullReplacement("ReferencedKey");

	[CLSCompliant(false)]
	[SfcReference(typeof(Table), "Server[@Name='{0}']/Database[@Name='{1}']/Table[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "ReferencedTable", "ReferencedTableSchema" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string ReferencedTable
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ReferencedTable");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReferencedTable", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string ReferencedTableSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ReferencedTableSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReferencedTableSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public ForeignKeyAction UpdateAction
	{
		get
		{
			return (ForeignKeyAction)base.Properties.GetValueWithNullReplacement("UpdateAction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UpdateAction", value);
		}
	}

	public static string UrnSuffix => "ForeignKey";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public override string Name
	{
		get
		{
			if (base.IsDesignMode && GetIsSystemNamed() && base.State == SqlSmoState.Creating)
			{
				return null;
			}
			return base.Name;
		}
		set
		{
			base.Name = value;
			if (base.ParentColl != null)
			{
				SetIsSystemNamed(flag: false);
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemNamed
	{
		get
		{
			if (base.ParentColl != null && base.IsDesignMode && base.State != SqlSmoState.Existing)
			{
				throw new PropertyNotSetException("IsSystemNamed");
			}
			return (bool)base.Properties.GetValueWithNullReplacement("IsSystemNamed");
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(ForeignKeyColumn), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design)]
	public ForeignKeyColumnCollection Columns
	{
		get
		{
			CheckObjectState();
			if (m_Columns == null)
			{
				m_Columns = new ForeignKeyColumnCollection(this);
			}
			return m_Columns;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ScriptReferencedTable
	{
		get
		{
			CheckObjectState();
			return m_sReferencedTable;
		}
		set
		{
			CheckObjectState();
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("ScriptReferencedTable"));
			}
			m_sReferencedTable = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ScriptReferencedTableSchema
	{
		get
		{
			CheckObjectState();
			return m_sReferencedTableSchema;
		}
		set
		{
			CheckObjectState();
			if (value == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("ScriptReferencedTableSchema"));
			}
			m_sReferencedTableSchema = value;
		}
	}

	public ForeignKey()
	{
	}

	public ForeignKey(Table table, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = table;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	object IPropertyDataDispatch.GetPropertyValue(int index)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				throw new IndexOutOfRangeException();
			}
			return index switch
			{
				0 => XRuntime.CreateDate, 
				1 => XRuntime.DateLastModified, 
				2 => XSchema.DeleteAction, 
				3 => XRuntime.ID, 
				4 => XSchema.IsChecked, 
				5 => XSchema.IsEnabled, 
				12 => XRuntime.IsMemoryOptimized, 
				6 => XSchema.IsSystemNamed, 
				7 => XSchema.NotForReplication, 
				8 => XRuntime.ReferencedKey, 
				9 => XSchema.ReferencedTable, 
				10 => XSchema.ReferencedTableSchema, 
				11 => XSchema.UpdateAction, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			0 => XRuntime.CreateDate, 
			11 => XRuntime.DateLastModified, 
			1 => XSchema.DeleteAction, 
			2 => XRuntime.ID, 
			3 => XSchema.IsChecked, 
			4 => XSchema.IsEnabled, 
			12 => XRuntime.IsFileTableDefined, 
			13 => XRuntime.IsMemoryOptimized, 
			5 => XSchema.IsSystemNamed, 
			6 => XSchema.NotForReplication, 
			7 => XRuntime.ReferencedKey, 
			8 => XSchema.ReferencedTable, 
			9 => XSchema.ReferencedTableSchema, 
			10 => XSchema.UpdateAction, 
			_ => throw new IndexOutOfRangeException(), 
		};
	}

	void IPropertyDataDispatch.SetPropertyValue(int index, object value)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				throw new IndexOutOfRangeException();
			}
			switch (index)
			{
			case 0:
				XRuntime.CreateDate = (DateTime)value;
				break;
			case 1:
				XRuntime.DateLastModified = (DateTime)value;
				break;
			case 2:
				XSchema.DeleteAction = (ForeignKeyAction)value;
				break;
			case 3:
				XRuntime.ID = (int)value;
				break;
			case 4:
				XSchema.IsChecked = (bool)value;
				break;
			case 5:
				XSchema.IsEnabled = (bool)value;
				break;
			case 12:
				XRuntime.IsMemoryOptimized = (bool)value;
				break;
			case 6:
				XSchema.IsSystemNamed = (bool)value;
				break;
			case 7:
				XSchema.NotForReplication = (bool)value;
				break;
			case 8:
				XRuntime.ReferencedKey = (string)value;
				break;
			case 9:
				XSchema.ReferencedTable = (string)value;
				break;
			case 10:
				XSchema.ReferencedTableSchema = (string)value;
				break;
			case 11:
				XSchema.UpdateAction = (ForeignKeyAction)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 0:
				XRuntime.CreateDate = (DateTime)value;
				break;
			case 11:
				XRuntime.DateLastModified = (DateTime)value;
				break;
			case 1:
				XSchema.DeleteAction = (ForeignKeyAction)value;
				break;
			case 2:
				XRuntime.ID = (int)value;
				break;
			case 3:
				XSchema.IsChecked = (bool)value;
				break;
			case 4:
				XSchema.IsEnabled = (bool)value;
				break;
			case 12:
				XRuntime.IsFileTableDefined = (bool)value;
				break;
			case 13:
				XRuntime.IsMemoryOptimized = (bool)value;
				break;
			case 5:
				XSchema.IsSystemNamed = (bool)value;
				break;
			case 6:
				XSchema.NotForReplication = (bool)value;
				break;
			case 7:
				XRuntime.ReferencedKey = (string)value;
				break;
			case 8:
				XSchema.ReferencedTable = (string)value;
				break;
			case 9:
				XSchema.ReferencedTableSchema = (string)value;
				break;
			case 10:
				XSchema.UpdateAction = (ForeignKeyAction)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[5] { "DeleteAction", "IsChecked", "IsMemoryOptimized", "NotForReplication", "UpdateAction" };
	}

	internal ForeignKey(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override void UpdateObjectState()
	{
		if (base.State == SqlSmoState.Pending && base.ParentColl != null && (!key.IsNull || base.IsDesignMode))
		{
			SetState(SqlSmoState.Creating);
			if (key.IsNull)
			{
				AutoGenerateName();
			}
			else
			{
				SetIsSystemNamed(flag: false);
			}
		}
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_Columns != null)
		{
			m_Columns.MarkAllDropped();
		}
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ConstraintScriptCreate(ScriptDdlBody(sp), createQuery, sp);
	}

	internal string ScriptDdlBody(ScriptingPreferences sp)
	{
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		AddConstraintName(stringBuilder, sp);
		stringBuilder.Append("FOREIGN KEY");
		stringBuilder.Append(Globals.LParen);
		if (Columns.Count == 0)
		{
			throw new SmoException(ExceptionTemplatesImpl.NoObjectWithoutColumns("ForeignKey"));
		}
		bool flag = true;
		foreach (ForeignKeyColumn column2 in Columns)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(Globals.commaspace);
			}
			Column column = null;
			if (typeof(Table).Equals(base.ParentColl.ParentInstance.GetType()))
			{
				column = ((Table)base.ParentColl.ParentInstance).Columns[column2.Name];
				if (column == null)
				{
					throw new SmoException(ExceptionTemplatesImpl.ObjectRefsNonexCol(UrnSuffix, Name, "[" + SqlSmoObject.SqlStringBraket(base.ParentColl.ParentInstance.InternalName) + "].[" + SqlSmoObject.SqlStringBraket(column2.Name) + "]"));
				}
				if (column.IgnoreForScripting)
				{
					return "";
				}
			}
			if (column != null && column.ScriptName.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(column.ScriptName) });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(column2.Name) });
			}
		}
		stringBuilder.Append(Globals.RParen);
		stringBuilder.Append(sp.NewLine);
		string text = "";
		string text2 = "";
		string text3 = "";
		text2 = ((sp.ForDirectExecution || ScriptReferencedTable.Length <= 0) ? ((string)GetPropValue("ReferencedTable")) : ScriptReferencedTable);
		if (!sp.ForDirectExecution && (sp.IncludeScripts.SchemaQualifyForeignKeysReferences || sp.IncludeScripts.SchemaQualify) && ScriptReferencedTableSchema.Length > 0)
		{
			text3 = ScriptReferencedTableSchema;
		}
		else if ((!sp.ForDirectExecution && (sp.IncludeScripts.SchemaQualifyForeignKeysReferences || sp.IncludeScripts.SchemaQualify)) || sp.ForDirectExecution)
		{
			string text4 = (string)GetPropValueOptional("ReferencedTableSchema");
			if (text4 != null)
			{
				text3 = text4;
			}
		}
		if (text3.Length > 0)
		{
			text = string.Format(SmoApplication.DefaultCulture, "[{0}].", new object[1] { SqlSmoObject.SqlBraket(text3) });
		}
		text += string.Format(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(text2) });
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "REFERENCES {0} (", new object[1] { text });
		flag = true;
		foreach (ForeignKeyColumn column3 in Columns)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(Globals.commaspace);
			}
			stringBuilder.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(column3.ReferencedColumn));
		}
		stringBuilder.Append(Globals.RParen);
		AddReferentioalAction(sp, stringBuilder, "UpdateAction", "ON UPDATE");
		AddReferentioalAction(sp, stringBuilder, "DeleteAction", "ON DELETE");
		if (IsSupportedProperty("NotForReplication", sp))
		{
			Property property = base.Properties.Get("NotForReplication");
			if (property.Value != null && (bool)property.Value && !SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
			{
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append("NOT FOR REPLICATION ");
			}
		}
		return stringBuilder.ToString();
	}

	internal override string GetScriptIncludeExists(ScriptingPreferences sp, string tableName, bool forCreate)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("N'");
		if (sp.IncludeScripts.SchemaQualify)
		{
			string schema = Parent.GetSchema(sp);
			if (schema.Length > 0)
			{
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket(SqlSmoObject.SqlString(schema)));
				stringBuilder.Append(Globals.Dot);
			}
		}
		stringBuilder.Append(SqlSmoObject.SqlString(FormatFullNameForScripting(sp)));
		stringBuilder.Append("'");
		if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80)
		{
			return string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FOREIGN_KEY90, new object[3]
			{
				forCreate ? "NOT" : "",
				stringBuilder.ToString(),
				SqlSmoObject.MakeSqlString(tableName)
			});
		}
		return string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FOREIGN_KEY80, new object[2]
		{
			forCreate ? "NOT" : "",
			stringBuilder.ToString()
		});
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	private void AddReferentioalAction(ScriptingPreferences sp, StringBuilder sb, string propertyName, string action)
	{
		switch (GetPropValueOptional(propertyName, ForeignKeyAction.NoAction))
		{
		case ForeignKeyAction.Cascade:
			sb.Append(sp.NewLine);
			sb.Append(action);
			sb.Append(" CASCADE");
			break;
		case ForeignKeyAction.SetNull:
			if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
			{
				SqlSmoObject.Trace("Entering ForeignKeyBase.AddReferentioalAction( " + sp.TargetServerVersionInternal.ToString() + ")");
				throw new WrongPropertyValueException(base.Properties.Get(propertyName));
			}
			sb.Append(sp.NewLine);
			sb.Append(action);
			sb.Append(" SET NULL");
			break;
		case ForeignKeyAction.SetDefault:
			if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
			{
				throw new WrongPropertyValueException(base.Properties.Get(propertyName));
			}
			sb.Append(sp.NewLine);
			sb.Append(action);
			sb.Append(" SET DEFAULT");
			break;
		default:
			throw new WrongPropertyValueException(base.Properties.Get(propertyName));
		case ForeignKeyAction.NoAction:
			break;
		}
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		TableViewBase tableViewBase = (TableViewBase)base.ParentColl.ParentInstance;
		string text = tableViewBase.FormatFullNameForScripting(sp);
		string text2 = FormatFullNameForScripting(sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = VersionUtils.IsTargetServerVersionSQl13OrLater(sp.TargetServerVersionInternal);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (flag)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
				{
					"",
					SqlSmoObject.SqlString(text)
				});
			}
			else
			{
				stringBuilder.Append(GetScriptIncludeExists(sp, text, forCreate: false));
			}
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP CONSTRAINT {1}{2}", new object[3]
		{
			text,
			(sp.IncludeScripts.ExistenceCheck && flag) ? "IF EXISTS " : string.Empty,
			text2
		});
		queries.Add(stringBuilder.ToString());
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ConstraintScriptAlter(alterQuery, sp);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		string schema = ((SchemaObjectKey)base.ParentColl.ParentInstance.key).Schema;
		string s = string.Format(SmoApplication.DefaultCulture, "{0}.{1}", new object[2]
		{
			SqlSmoObject.MakeSqlBraket(schema),
			FullQualifiedName
		});
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_rename N'{0}', N'{1}', N'OBJECT'", new object[2]
		{
			SqlSmoObject.SqlString(s),
			SqlSmoObject.SqlString(newName)
		}));
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
			};
		}
		return null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[9] { "IsSystemNamed", "DeleteAction", "UpdateAction", "ReferencedTable", "ReferencedTableSchema", "IsChecked", "IsEnabled", "NotForReplication", "IsFileTableDefined" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class IndexedXmlPath : NamedSmoObject, ISfcSupportsDesignMode, IPropertyDataDispatch, IMarkForDrop
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 18, 18, 18, 18, 18 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 18 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[18]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsNode", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSingleton", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsXQueryMaxlengthInferred", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXQueryTypeInferred", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Path", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PathType", expensive: false, readOnly: false, typeof(IndexedXmlPathType)),
			new StaticMetadata("PathTypeDesc", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SqlTypeCollationName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlComponentID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("XQueryMaxLength", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("XQueryTypeDescription", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[18]
		{
			new StaticMetadata("DataType", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataTypeSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsNode", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSingleton", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsXQueryMaxlengthInferred", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXQueryTypeInferred", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("Path", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PathType", expensive: false, readOnly: false, typeof(IndexedXmlPathType)),
			new StaticMetadata("PathTypeDesc", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SqlTypeCollationName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SystemType", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("XmlComponentID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("XQueryMaxLength", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("XQueryTypeDescription", expensive: false, readOnly: false, typeof(string))
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
					"DataType" => 0, 
					"DataTypeSchema" => 1, 
					"ID" => 2, 
					"IsNode" => 3, 
					"IsSingleton" => 4, 
					"IsXQueryMaxlengthInferred" => 5, 
					"IsXQueryTypeInferred" => 6, 
					"Length" => 7, 
					"NumericPrecision" => 8, 
					"NumericScale" => 9, 
					"Path" => 10, 
					"PathType" => 11, 
					"PathTypeDesc" => 12, 
					"SqlTypeCollationName" => 13, 
					"SystemType" => 14, 
					"XmlComponentID" => 15, 
					"XQueryMaxLength" => 16, 
					"XQueryTypeDescription" => 17, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"DataType" => 0, 
				"DataTypeSchema" => 1, 
				"ID" => 2, 
				"IsNode" => 3, 
				"IsSingleton" => 4, 
				"IsXQueryMaxlengthInferred" => 5, 
				"IsXQueryTypeInferred" => 6, 
				"Length" => 7, 
				"NumericPrecision" => 8, 
				"NumericScale" => 9, 
				"Path" => 10, 
				"PathType" => 11, 
				"PathTypeDesc" => 12, 
				"SqlTypeCollationName" => 13, 
				"SystemType" => 14, 
				"XmlComponentID" => 15, 
				"XQueryMaxLength" => 16, 
				"XQueryTypeDescription" => 17, 
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
		private string _DataType;

		private string _DataTypeSchema;

		private bool _IsNode;

		private bool _IsSingleton;

		private bool _IsXQueryMaxlengthInferred;

		private bool _IsXQueryTypeInferred;

		private int _Length;

		private int _NumericPrecision;

		private int _NumericScale;

		private string _Path;

		private IndexedXmlPathType _PathType;

		private string _PathTypeDesc;

		private string _SqlTypeCollationName;

		private string _SystemType;

		private int _XmlComponentID;

		private int _XQueryMaxLength;

		private string _XQueryTypeDescription;

		internal string DataType
		{
			get
			{
				return _DataType;
			}
			set
			{
				_DataType = value;
			}
		}

		internal string DataTypeSchema
		{
			get
			{
				return _DataTypeSchema;
			}
			set
			{
				_DataTypeSchema = value;
			}
		}

		internal bool IsNode
		{
			get
			{
				return _IsNode;
			}
			set
			{
				_IsNode = value;
			}
		}

		internal bool IsSingleton
		{
			get
			{
				return _IsSingleton;
			}
			set
			{
				_IsSingleton = value;
			}
		}

		internal bool IsXQueryMaxlengthInferred
		{
			get
			{
				return _IsXQueryMaxlengthInferred;
			}
			set
			{
				_IsXQueryMaxlengthInferred = value;
			}
		}

		internal bool IsXQueryTypeInferred
		{
			get
			{
				return _IsXQueryTypeInferred;
			}
			set
			{
				_IsXQueryTypeInferred = value;
			}
		}

		internal int Length
		{
			get
			{
				return _Length;
			}
			set
			{
				_Length = value;
			}
		}

		internal int NumericPrecision
		{
			get
			{
				return _NumericPrecision;
			}
			set
			{
				_NumericPrecision = value;
			}
		}

		internal int NumericScale
		{
			get
			{
				return _NumericScale;
			}
			set
			{
				_NumericScale = value;
			}
		}

		internal string Path
		{
			get
			{
				return _Path;
			}
			set
			{
				_Path = value;
			}
		}

		internal IndexedXmlPathType PathType
		{
			get
			{
				return _PathType;
			}
			set
			{
				_PathType = value;
			}
		}

		internal string PathTypeDesc
		{
			get
			{
				return _PathTypeDesc;
			}
			set
			{
				_PathTypeDesc = value;
			}
		}

		internal string SqlTypeCollationName
		{
			get
			{
				return _SqlTypeCollationName;
			}
			set
			{
				_SqlTypeCollationName = value;
			}
		}

		internal string SystemType
		{
			get
			{
				return _SystemType;
			}
			set
			{
				_SystemType = value;
			}
		}

		internal int XmlComponentID
		{
			get
			{
				return _XmlComponentID;
			}
			set
			{
				_XmlComponentID = value;
			}
		}

		internal int XQueryMaxLength
		{
			get
			{
				return _XQueryMaxLength;
			}
			set
			{
				_XQueryMaxLength = value;
			}
		}

		internal string XQueryTypeDescription
		{
			get
			{
				return _XQueryTypeDescription;
			}
			set
			{
				_XQueryTypeDescription = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private int _ID;

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
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	private DataType dataType;

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	public Index Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Index;
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
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsNode
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsNode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsNode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSingleton
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSingleton");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSingleton", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsXQueryMaxlengthInferred => (bool)base.Properties.GetValueWithNullReplacement("IsXQueryMaxlengthInferred");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsXQueryTypeInferred => (bool)base.Properties.GetValueWithNullReplacement("IsXQueryTypeInferred");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Path
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Path");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Path", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "XQuery")]
	public IndexedXmlPathType PathType
	{
		get
		{
			return (IndexedXmlPathType)base.Properties.GetValueWithNullReplacement("PathType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PathType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string PathTypeDesc
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PathTypeDesc");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PathTypeDesc", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SqlTypeCollationName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("SqlTypeCollationName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SqlTypeCollationName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int XmlComponentID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("XmlComponentID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("XmlComponentID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int XQueryMaxLength
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("XQueryMaxLength");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("XQueryMaxLength", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string XQueryTypeDescription
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("XQueryTypeDescription");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("XQueryTypeDescription", value);
		}
	}

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public override string Name
	{
		get
		{
			return base.Name;
		}
		set
		{
			base.Name = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DataType DataType
	{
		get
		{
			return GetDataType(ref dataType);
		}
		set
		{
			SetDataType(ref dataType, value);
		}
	}

	public static string UrnSuffix => "IndexedXmlPath";

	public IndexedXmlPath()
	{
	}

	public IndexedXmlPath(Index index, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = index;
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
				0 => XSchema.DataType, 
				1 => XSchema.DataTypeSchema, 
				2 => XRuntime.ID, 
				3 => XSchema.IsNode, 
				4 => XSchema.IsSingleton, 
				5 => XSchema.IsXQueryMaxlengthInferred, 
				6 => XSchema.IsXQueryTypeInferred, 
				7 => XSchema.Length, 
				8 => XSchema.NumericPrecision, 
				9 => XSchema.NumericScale, 
				10 => XSchema.Path, 
				11 => XSchema.PathType, 
				12 => XSchema.PathTypeDesc, 
				13 => XSchema.SqlTypeCollationName, 
				14 => XSchema.SystemType, 
				15 => XSchema.XmlComponentID, 
				16 => XSchema.XQueryMaxLength, 
				17 => XSchema.XQueryTypeDescription, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			0 => XSchema.DataType, 
			1 => XSchema.DataTypeSchema, 
			2 => XRuntime.ID, 
			3 => XSchema.IsNode, 
			4 => XSchema.IsSingleton, 
			5 => XSchema.IsXQueryMaxlengthInferred, 
			6 => XSchema.IsXQueryTypeInferred, 
			7 => XSchema.Length, 
			8 => XSchema.NumericPrecision, 
			9 => XSchema.NumericScale, 
			10 => XSchema.Path, 
			11 => XSchema.PathType, 
			12 => XSchema.PathTypeDesc, 
			13 => XSchema.SqlTypeCollationName, 
			14 => XSchema.SystemType, 
			15 => XSchema.XmlComponentID, 
			16 => XSchema.XQueryMaxLength, 
			17 => XSchema.XQueryTypeDescription, 
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
				XSchema.DataType = (string)value;
				break;
			case 1:
				XSchema.DataTypeSchema = (string)value;
				break;
			case 2:
				XRuntime.ID = (int)value;
				break;
			case 3:
				XSchema.IsNode = (bool)value;
				break;
			case 4:
				XSchema.IsSingleton = (bool)value;
				break;
			case 5:
				XSchema.IsXQueryMaxlengthInferred = (bool)value;
				break;
			case 6:
				XSchema.IsXQueryTypeInferred = (bool)value;
				break;
			case 7:
				XSchema.Length = (int)value;
				break;
			case 8:
				XSchema.NumericPrecision = (int)value;
				break;
			case 9:
				XSchema.NumericScale = (int)value;
				break;
			case 10:
				XSchema.Path = (string)value;
				break;
			case 11:
				XSchema.PathType = (IndexedXmlPathType)value;
				break;
			case 12:
				XSchema.PathTypeDesc = (string)value;
				break;
			case 13:
				XSchema.SqlTypeCollationName = (string)value;
				break;
			case 14:
				XSchema.SystemType = (string)value;
				break;
			case 15:
				XSchema.XmlComponentID = (int)value;
				break;
			case 16:
				XSchema.XQueryMaxLength = (int)value;
				break;
			case 17:
				XSchema.XQueryTypeDescription = (string)value;
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
				XSchema.DataType = (string)value;
				break;
			case 1:
				XSchema.DataTypeSchema = (string)value;
				break;
			case 2:
				XRuntime.ID = (int)value;
				break;
			case 3:
				XSchema.IsNode = (bool)value;
				break;
			case 4:
				XSchema.IsSingleton = (bool)value;
				break;
			case 5:
				XSchema.IsXQueryMaxlengthInferred = (bool)value;
				break;
			case 6:
				XSchema.IsXQueryTypeInferred = (bool)value;
				break;
			case 7:
				XSchema.Length = (int)value;
				break;
			case 8:
				XSchema.NumericPrecision = (int)value;
				break;
			case 9:
				XSchema.NumericScale = (int)value;
				break;
			case 10:
				XSchema.Path = (string)value;
				break;
			case 11:
				XSchema.PathType = (IndexedXmlPathType)value;
				break;
			case 12:
				XSchema.PathTypeDesc = (string)value;
				break;
			case 13:
				XSchema.SqlTypeCollationName = (string)value;
				break;
			case 14:
				XSchema.SystemType = (string)value;
				break;
			case 15:
				XSchema.XmlComponentID = (int)value;
				break;
			case 16:
				XSchema.XQueryMaxLength = (int)value;
				break;
			case 17:
				XSchema.XQueryTypeDescription = (string)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[15]
		{
			"DataType", "DataTypeSchema", "IsNode", "IsSingleton", "Length", "NumericPrecision", "NumericScale", "Path", "PathType", "PathTypeDesc",
			"SqlTypeCollationName", "SystemType", "XmlComponentID", "XQueryMaxLength", "XQueryTypeDescription"
		};
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "PathType")
		{
			return IndexedXmlPathType.XQuery;
		}
		return base.GetPropertyDefaultValue(propname);
	}

	internal IndexedXmlPath(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[1] { "Path" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

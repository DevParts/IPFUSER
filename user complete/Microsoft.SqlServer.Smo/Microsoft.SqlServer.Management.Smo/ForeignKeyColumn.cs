using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("Column")]
public sealed class ForeignKeyColumn : NamedSmoObject, ISfcSupportsDesignMode, IPropertyDataDispatch
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

		private static int[] cloudVersionCount = new int[3] { 2, 2, 2 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[2]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ReferencedColumn", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[2]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ReferencedColumn", expensive: false, readOnly: false, typeof(string))
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
					"ID" => 0, 
					"ReferencedColumn" => 1, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"ReferencedColumn" => 1, 
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
		private string _ReferencedColumn;

		internal string ReferencedColumn
		{
			get
			{
				return _ReferencedColumn;
			}
			set
			{
				_ReferencedColumn = value;
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

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	public ForeignKey Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ForeignKey;
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string ReferencedColumn
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ReferencedColumn");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReferencedColumn", value);
		}
	}

	public static string UrnSuffix => "Column";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(0)]
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

	public ForeignKeyColumn()
	{
	}

	public ForeignKeyColumn(ForeignKey foreignKey, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = foreignKey;
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
				0 => XRuntime.ID, 
				1 => XSchema.ReferencedColumn, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			0 => XRuntime.ID, 
			1 => XSchema.ReferencedColumn, 
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
				XRuntime.ID = (int)value;
				break;
			case 1:
				XSchema.ReferencedColumn = (string)value;
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
				XRuntime.ID = (int)value;
				break;
			case 1:
				XSchema.ReferencedColumn = (string)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal ForeignKeyColumn(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ForeignKeyColumn(ForeignKey foreignKey, string name, string referencedColumn)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = foreignKey;
		ReferencedColumn = referencedColumn;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[1] { "ReferencedColumn" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

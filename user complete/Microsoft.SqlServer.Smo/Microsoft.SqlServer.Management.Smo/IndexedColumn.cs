using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class IndexedColumn : NamedSmoObject, ISfcSupportsDesignMode, IPropertyDataDispatch
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 2, 3, 4, 4, 4, 4, 4, 4, 4, 4 };

		private static int[] cloudVersionCount = new int[3] { 4, 4, 4 };

		private static int sqlDwPropertyCount = 3;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("Descending", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsIncluded", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("Descending", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsComputed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsIncluded", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsComputed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Descending", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsIncluded", expensive: false, readOnly: false, typeof(bool))
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
					return propertyName switch
					{
						"Descending" => 0, 
						"ID" => 1, 
						"IsIncluded" => 2, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"Descending" => 0, 
					"ID" => 1, 
					"IsComputed" => 2, 
					"IsIncluded" => 3, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"IsComputed" => 1, 
				"Descending" => 2, 
				"IsIncluded" => 3, 
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
		private bool _Descending;

		private bool _IsComputed;

		private bool _IsIncluded;

		internal bool Descending
		{
			get
			{
				return _Descending;
			}
			set
			{
				_Descending = value;
			}
		}

		internal bool IsComputed
		{
			get
			{
				return _IsComputed;
			}
			set
			{
				_IsComputed = value;
			}
		}

		internal bool IsIncluded
		{
			get
			{
				return _IsIncluded;
			}
			set
			{
				_IsIncluded = value;
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

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design | SfcObjectFlags.Deploy)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool Descending
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Descending");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Descending", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsComputed => (bool)base.Properties.GetValueWithNullReplacement("IsComputed");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool IsIncluded
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsIncluded");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsIncluded", value);
		}
	}

	public static string UrnSuffix => "IndexedColumn";

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

	public IndexedColumn()
	{
	}

	public IndexedColumn(Index index, string name)
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
				return index switch
				{
					0 => XSchema.Descending, 
					1 => XRuntime.ID, 
					2 => XSchema.IsIncluded, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			return index switch
			{
				0 => XSchema.Descending, 
				1 => XRuntime.ID, 
				2 => XSchema.IsComputed, 
				3 => XSchema.IsIncluded, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			2 => XSchema.Descending, 
			0 => XRuntime.ID, 
			1 => XSchema.IsComputed, 
			3 => XSchema.IsIncluded, 
			_ => throw new IndexOutOfRangeException(), 
		};
	}

	void IPropertyDataDispatch.SetPropertyValue(int index, object value)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			if (DatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse)
			{
				switch (index)
				{
				case 0:
					XSchema.Descending = (bool)value;
					break;
				case 1:
					XRuntime.ID = (int)value;
					break;
				case 2:
					XSchema.IsIncluded = (bool)value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return;
			}
			switch (index)
			{
			case 0:
				XSchema.Descending = (bool)value;
				break;
			case 1:
				XRuntime.ID = (int)value;
				break;
			case 2:
				XSchema.IsComputed = (bool)value;
				break;
			case 3:
				XSchema.IsIncluded = (bool)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 2:
				XSchema.Descending = (bool)value;
				break;
			case 0:
				XRuntime.ID = (int)value;
				break;
			case 1:
				XSchema.IsComputed = (bool)value;
				break;
			case 3:
				XSchema.IsIncluded = (bool)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal IndexedColumn(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public IndexedColumn(Index index, string name, bool descending)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = index;
		Descending = descending;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[3] { "IsIncluded", "Descending", "IsComputed" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class IndexedXmlPathNamespace : NamedSmoObject, ISfcSupportsDesignMode, IPropertyDataDispatch
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 2, 2, 2, 2, 2 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 2 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[2]
		{
			new StaticMetadata("IsDefaultUri", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Uri", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[2]
		{
			new StaticMetadata("IsDefaultUri", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Uri", expensive: false, readOnly: false, typeof(string))
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
					"IsDefaultUri" => 0, 
					"Uri" => 1, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"IsDefaultUri" => 0, 
				"Uri" => 1, 
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
		private bool _IsDefaultUri;

		private string _Uri;

		internal bool IsDefaultUri
		{
			get
			{
				return _IsDefaultUri;
			}
			set
			{
				_IsDefaultUri = value;
			}
		}

		internal string Uri
		{
			get
			{
				return _Uri;
			}
			set
			{
				_Uri = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDefaultUri
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDefaultUri");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDefaultUri", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Uri
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Uri");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Uri", value);
		}
	}

	public static string UrnSuffix => "IndexedXmlPathNamespace";

	public IndexedXmlPathNamespace()
	{
	}

	public IndexedXmlPathNamespace(Index index, string name)
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
				0 => XSchema.IsDefaultUri, 
				1 => XSchema.Uri, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			0 => XSchema.IsDefaultUri, 
			1 => XSchema.Uri, 
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
				XSchema.IsDefaultUri = (bool)value;
				break;
			case 1:
				XSchema.Uri = (string)value;
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
				XSchema.IsDefaultUri = (bool)value;
				break;
			case 1:
				XSchema.Uri = (string)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[2] { "IsDefaultUri", "Uri" };
	}

	internal IndexedXmlPathNamespace(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[1] { "IsDefaultUri" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

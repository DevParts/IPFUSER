using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class PartitionFunctionParameter : ScriptNameObjectBase, ISfcSupportsDesignMode
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 5, 5, 5, 5, 5, 5, 5, 5 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 5 };

		private static int sqlDwPropertyCount = 5;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("Collation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("Collation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("Collation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Length", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericPrecision", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("NumericScale", expensive: false, readOnly: false, typeof(int))
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
						"Collation" => 0, 
						"ID" => 1, 
						"Length" => 2, 
						"NumericPrecision" => 3, 
						"NumericScale" => 4, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"Collation" => 0, 
					"ID" => 1, 
					"Length" => 2, 
					"NumericPrecision" => 3, 
					"NumericScale" => 4, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"Collation" => 0, 
				"ID" => 1, 
				"Length" => 2, 
				"NumericPrecision" => 3, 
				"NumericScale" => 4, 
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

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Collation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Collation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Collation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public int Length
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("Length");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Length", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public int NumericPrecision
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("NumericPrecision");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumericPrecision", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public int NumericScale
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("NumericScale");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NumericScale", value);
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public PartitionFunction Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as PartitionFunction;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	public static string UrnSuffix => "PartitionFunctionParameter";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal PartitionFunctionParameter(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public PartitionFunctionParameter()
	{
	}

	public PartitionFunctionParameter(PartitionFunction partitionFunction)
	{
		Parent = partitionFunction;
	}

	public PartitionFunctionParameter(PartitionFunction partitionFunction, DataType dataType)
	{
		string sqlName = dataType.GetSqlName(dataType.sqlDataType);
		ValidateName(sqlName);
		key = new SimpleObjectKey(sqlName);
		Parent = partitionFunction;
		if (UserDefinedDataType.TypeAllowsLength(dataType.Name, partitionFunction.StringComparer))
		{
			Length = dataType.MaximumLength;
		}
		else if (UserDefinedDataType.TypeAllowsPrecisionScale(dataType.Name, partitionFunction.StringComparer))
		{
			NumericPrecision = dataType.NumericPrecision;
			NumericScale = dataType.NumericScale;
		}
		else if (UserDefinedDataType.TypeAllowsScale(dataType.Name, partitionFunction.StringComparer))
		{
			NumericScale = dataType.NumericScale;
		}
		else if (DataType.IsTypeFloatStateCreating(dataType.Name, partitionFunction))
		{
			NumericPrecision = dataType.NumericPrecision;
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[3] { "Length", "NumericPrecision", "NumericScale" };
	}
}

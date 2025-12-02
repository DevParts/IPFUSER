using System;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class PhysicalPartition : SqlSmoObject, IPropertyDataDispatch
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 5, 6, 6, 6, 6, 6, 6, 6 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 6 };

		private static int sqlDwPropertyCount = 5;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("DataCompression", expensive: false, readOnly: false, typeof(DataCompressionType)),
			new StaticMetadata("PartitionNumber", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("RangeType", expensive: false, readOnly: false, typeof(RangeType)),
			new StaticMetadata("RightBoundaryValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("RowCount", expensive: false, readOnly: true, typeof(double))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("DataCompression", expensive: false, readOnly: false, typeof(DataCompressionType)),
			new StaticMetadata("FileGroupName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionNumber", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("RangeType", expensive: false, readOnly: false, typeof(RangeType)),
			new StaticMetadata("RightBoundaryValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("RowCount", expensive: false, readOnly: true, typeof(double))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("FileGroupName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionNumber", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("RangeType", expensive: false, readOnly: false, typeof(RangeType)),
			new StaticMetadata("RightBoundaryValue", expensive: false, readOnly: false, typeof(object)),
			new StaticMetadata("RowCount", expensive: false, readOnly: true, typeof(double)),
			new StaticMetadata("DataCompression", expensive: false, readOnly: false, typeof(DataCompressionType))
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
						"DataCompression" => 0, 
						"PartitionNumber" => 1, 
						"RangeType" => 2, 
						"RightBoundaryValue" => 3, 
						"RowCount" => 4, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"DataCompression" => 0, 
					"FileGroupName" => 1, 
					"PartitionNumber" => 2, 
					"RangeType" => 3, 
					"RightBoundaryValue" => 4, 
					"RowCount" => 5, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"FileGroupName" => 0, 
				"PartitionNumber" => 1, 
				"RangeType" => 2, 
				"RightBoundaryValue" => 3, 
				"RowCount" => 4, 
				"DataCompression" => 5, 
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
		private DataCompressionType _DataCompression;

		private string _FileGroupName;

		private int _PartitionNumber;

		private object _RightBoundaryValue;

		private double _RowCount;

		internal DataCompressionType DataCompression
		{
			get
			{
				return _DataCompression;
			}
			set
			{
				_DataCompression = value;
			}
		}

		internal string FileGroupName
		{
			get
			{
				return _FileGroupName;
			}
			set
			{
				_FileGroupName = value;
			}
		}

		internal int PartitionNumber
		{
			get
			{
				return _PartitionNumber;
			}
			set
			{
				_PartitionNumber = value;
			}
		}

		internal object RightBoundaryValue
		{
			get
			{
				return _RightBoundaryValue;
			}
			set
			{
				_RightBoundaryValue = value;
			}
		}

		internal double RowCount
		{
			get
			{
				return _RowCount;
			}
			set
			{
				_RowCount = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private RangeType _RangeType;

		internal RangeType RangeType
		{
			get
			{
				return _RangeType;
			}
			set
			{
				_RangeType = value;
			}
		}
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	[SfcParent("Index")]
	[SfcObject(SfcObjectRelationship.ParentObject)]
	[SfcParent("Table")]
	public SqlSmoObject Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance;
		}
	}

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
	public string FileGroupName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileGroupName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileGroupName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int PartitionNumber
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("PartitionNumber");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PartitionNumber", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double RowCount => (double)base.Properties.GetValueWithNullReplacement("RowCount");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public object RightBoundaryValue
	{
		get
		{
			return base.Properties.GetValueWithNullReplacement("RightBoundaryValue", throwOnNullValue: false, useDefaultOnMissingValue: false);
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RightBoundaryValue", value, allowNull: true);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public RangeType RangeType
	{
		get
		{
			try
			{
				return (RangeType)base.Properties.GetValueWithNullReplacement("RangeType");
			}
			catch (PropertyCannotBeRetrievedException)
			{
				return RangeType.None;
			}
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RangeType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DataCompressionType DataCompression
	{
		get
		{
			this.ThrowIfNotSupported(typeof(PhysicalPartition));
			if (base.ServerVersion.Major < 10)
			{
				return DataCompressionType.None;
			}
			return (DataCompressionType)base.Properties.GetValueWithNullReplacement("DataCompression");
		}
		set
		{
			ThrowIfBelowVersion100();
			if (value == DataCompressionType.ColumnStore || value == DataCompressionType.ColumnStoreArchive)
			{
				ThrowIfBelowVersion120();
			}
			base.Properties.SetValueWithConsistencyCheck("DataCompression", value);
		}
	}

	public static string UrnSuffix => "PhysicalPartition";

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
					0 => XSchema.DataCompression, 
					1 => XSchema.PartitionNumber, 
					2 => XRuntime.RangeType, 
					3 => XSchema.RightBoundaryValue, 
					4 => XSchema.RowCount, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			return index switch
			{
				0 => XSchema.DataCompression, 
				1 => XSchema.FileGroupName, 
				2 => XSchema.PartitionNumber, 
				3 => XRuntime.RangeType, 
				4 => XSchema.RightBoundaryValue, 
				5 => XSchema.RowCount, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			5 => XSchema.DataCompression, 
			0 => XSchema.FileGroupName, 
			1 => XSchema.PartitionNumber, 
			2 => XRuntime.RangeType, 
			3 => XSchema.RightBoundaryValue, 
			4 => XSchema.RowCount, 
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
					XSchema.DataCompression = (DataCompressionType)value;
					break;
				case 1:
					XSchema.PartitionNumber = (int)value;
					break;
				case 2:
					XRuntime.RangeType = (RangeType)value;
					break;
				case 3:
					XSchema.RightBoundaryValue = value;
					break;
				case 4:
					XSchema.RowCount = (double)value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return;
			}
			switch (index)
			{
			case 0:
				XSchema.DataCompression = (DataCompressionType)value;
				break;
			case 1:
				XSchema.FileGroupName = (string)value;
				break;
			case 2:
				XSchema.PartitionNumber = (int)value;
				break;
			case 3:
				XRuntime.RangeType = (RangeType)value;
				break;
			case 4:
				XSchema.RightBoundaryValue = value;
				break;
			case 5:
				XSchema.RowCount = (double)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 5:
				XSchema.DataCompression = (DataCompressionType)value;
				break;
			case 0:
				XSchema.FileGroupName = (string)value;
				break;
			case 1:
				XSchema.PartitionNumber = (int)value;
				break;
			case 2:
				XRuntime.RangeType = (RangeType)value;
				break;
			case 3:
				XSchema.RightBoundaryValue = value;
				break;
			case 4:
				XSchema.RowCount = (double)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal PhysicalPartition(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	private void Init()
	{
		DataCompression = DataCompressionType.None;
		PartitionNumber = 1;
		RightBoundaryValue = null;
		FileGroupName = string.Empty;
		RangeType = RangeType.None;
	}

	public PhysicalPartition(SqlSmoObject parent, int partitionNumber, DataCompressionType dataCompressionType)
	{
		SetParentImpl(parent);
		Init();
		PartitionNumber = partitionNumber;
		if (base.ServerVersion.Major >= 10)
		{
			DataCompression = dataCompressionType;
		}
		key = new PartitionNumberedObjectKey((short)PartitionNumber);
	}

	public PhysicalPartition()
	{
		Init();
		key = new PartitionNumberedObjectKey((short)PartitionNumber);
	}

	public PhysicalPartition(SqlSmoObject parent, int partitionNumber)
	{
		SetParentImpl(parent);
		Init();
		PartitionNumber = partitionNumber;
		key = new PartitionNumberedObjectKey((short)PartitionNumber);
	}

	internal PhysicalPartition(PhysicalPartition physicalPartition)
	{
		SetParentImpl(physicalPartition.Parent);
		DataCompression = physicalPartition.DataCompression;
		PartitionNumber = physicalPartition.PartitionNumber;
		RightBoundaryValue = physicalPartition.RightBoundaryValue;
		FileGroupName = physicalPartition.FileGroupName;
		RangeType = physicalPartition.RangeType;
	}

	internal bool Compare(PhysicalPartition physicalPartition)
	{
		if (DataCompression != physicalPartition.DataCompression)
		{
			return false;
		}
		if (PartitionNumber != physicalPartition.PartitionNumber)
		{
			return false;
		}
		if (RangeType != physicalPartition.RangeType)
		{
			return false;
		}
		return 0 == string.Compare(FileGroupName, physicalPartition.FileGroupName, StringComparison.Ordinal);
	}

	internal bool IsDirty(string property)
	{
		return base.Properties.IsDirty(base.Properties.LookupID(property, PropertyAccessPurpose.Read));
	}
}

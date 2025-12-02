using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Index : ScriptNameObjectBase, ISfcSupportsDesignMode, IPropertyDataDispatch, ICreatable, IDroppable, IDropIfExists, IMarkForDrop, IAlterable, IRenamable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 17, 17, 23, 40, 40, 42, 44, 45, 46, 46 };

		private static int[] cloudVersionCount = new int[3] { 30, 30, 43 };

		private static int sqlDwPropertyCount = 23;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[23]
		{
			new StaticMetadata("DisallowPageLocks", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DisallowRowLocks", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("FileGroup", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("FillFactor", expensive: false, readOnly: false, typeof(byte)),
			new StaticMetadata("FilterDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasCompressedPartitions", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasFilter", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSparseColumn", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IgnoreDuplicateKeys", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IndexKeyType", expensive: false, readOnly: false, typeof(IndexKeyType)),
			new StaticMetadata("IndexType", expensive: false, readOnly: false, typeof(IndexType)),
			new StaticMetadata("IsClustered", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextKey", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPartitioned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUnique", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsXmlIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NoAutomaticRecomputation", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PadIndex", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PartitionScheme", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[43]
		{
			new StaticMetadata("BoundingBoxXMax", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("BoundingBoxXMin", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("BoundingBoxYMax", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("BoundingBoxYMin", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("CellsPerObject", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("DisallowPageLocks", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DisallowRowLocks", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("FillFactor", expensive: false, readOnly: false, typeof(byte)),
			new StaticMetadata("FilterDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasFilter", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSparseColumn", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IgnoreDuplicateKeys", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IndexKeyType", expensive: false, readOnly: false, typeof(IndexKeyType)),
			new StaticMetadata("IndexType", expensive: false, readOnly: false, typeof(IndexType)),
			new StaticMetadata("IsClustered", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsFullTextKey", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSpatialIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUnique", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsXmlIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Level1Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("Level2Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("Level3Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("Level4Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("NoAutomaticRecomputation", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PadIndex", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SpatialIndexType", expensive: false, readOnly: false, typeof(SpatialIndexType)),
			new StaticMetadata("BucketCount", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("CompressionDelay", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("FileGroup", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("FileStreamFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileStreamPartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasCompressedPartitions", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("IndexedXmlPathName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsMemoryOptimized", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPartitioned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ParentXmlIndex", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ResumableOperationState", expensive: false, readOnly: true, typeof(ResumableOperationStateType)),
			new StaticMetadata("SecondaryXmlIndexType", expensive: false, readOnly: false, typeof(SecondaryXmlIndexType))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[46]
		{
			new StaticMetadata("DisallowPageLocks", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DisallowRowLocks", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("FileGroup", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("FillFactor", expensive: false, readOnly: false, typeof(byte)),
			new StaticMetadata("HasSparseColumn", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IgnoreDuplicateKeys", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IndexKeyType", expensive: false, readOnly: false, typeof(IndexKeyType)),
			new StaticMetadata("IndexType", expensive: false, readOnly: false, typeof(IndexType)),
			new StaticMetadata("IsClustered", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsFullTextKey", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemNamed", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUnique", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NoAutomaticRecomputation", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PadIndex", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SpaceUsed", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsPartitioned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsXmlIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ParentXmlIndex", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("SecondaryXmlIndexType", expensive: false, readOnly: false, typeof(SecondaryXmlIndexType)),
			new StaticMetadata("BoundingBoxXMax", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("BoundingBoxXMin", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("BoundingBoxYMax", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("BoundingBoxYMin", expensive: false, readOnly: false, typeof(double)),
			new StaticMetadata("CellsPerObject", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("FileStreamFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileStreamPartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FilterDefinition", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasCompressedPartitions", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasFilter", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSpatialIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Level1Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("Level2Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("Level3Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("Level4Grid", expensive: false, readOnly: false, typeof(SpatialGeoLevelSize)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("SpatialIndexType", expensive: false, readOnly: false, typeof(SpatialIndexType)),
			new StaticMetadata("IndexedXmlPathName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsFileTableDefined", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("BucketCount", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("IsMemoryOptimized", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("CompressionDelay", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ResumableOperationState", expensive: false, readOnly: true, typeof(ResumableOperationStateType))
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
						"DisallowPageLocks" => 0, 
						"DisallowRowLocks" => 1, 
						"FileGroup" => 2, 
						"FillFactor" => 3, 
						"FilterDefinition" => 4, 
						"HasCompressedPartitions" => 5, 
						"HasFilter" => 6, 
						"HasSparseColumn" => 7, 
						"ID" => 8, 
						"IgnoreDuplicateKeys" => 9, 
						"IndexKeyType" => 10, 
						"IndexType" => 11, 
						"IsClustered" => 12, 
						"IsDisabled" => 13, 
						"IsFullTextKey" => 14, 
						"IsPartitioned" => 15, 
						"IsSystemNamed" => 16, 
						"IsSystemObject" => 17, 
						"IsUnique" => 18, 
						"IsXmlIndex" => 19, 
						"NoAutomaticRecomputation" => 20, 
						"PadIndex" => 21, 
						"PartitionScheme" => 22, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"BoundingBoxXMax" => 0, 
					"BoundingBoxXMin" => 1, 
					"BoundingBoxYMax" => 2, 
					"BoundingBoxYMin" => 3, 
					"CellsPerObject" => 4, 
					"DisallowPageLocks" => 5, 
					"DisallowRowLocks" => 6, 
					"FillFactor" => 7, 
					"FilterDefinition" => 8, 
					"HasFilter" => 9, 
					"HasSparseColumn" => 10, 
					"ID" => 11, 
					"IgnoreDuplicateKeys" => 12, 
					"IndexKeyType" => 13, 
					"IndexType" => 14, 
					"IsClustered" => 15, 
					"IsDisabled" => 16, 
					"IsFullTextKey" => 17, 
					"IsSpatialIndex" => 18, 
					"IsSystemNamed" => 19, 
					"IsSystemObject" => 20, 
					"IsUnique" => 21, 
					"IsXmlIndex" => 22, 
					"Level1Grid" => 23, 
					"Level2Grid" => 24, 
					"Level3Grid" => 25, 
					"Level4Grid" => 26, 
					"NoAutomaticRecomputation" => 27, 
					"PadIndex" => 28, 
					"SpatialIndexType" => 29, 
					"BucketCount" => 30, 
					"CompressionDelay" => 31, 
					"FileGroup" => 32, 
					"FileStreamFileGroup" => 33, 
					"FileStreamPartitionScheme" => 34, 
					"HasCompressedPartitions" => 35, 
					"IndexedXmlPathName" => 36, 
					"IsMemoryOptimized" => 37, 
					"IsPartitioned" => 38, 
					"ParentXmlIndex" => 39, 
					"PartitionScheme" => 40, 
					"ResumableOperationState" => 41, 
					"SecondaryXmlIndexType" => 42, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"DisallowPageLocks" => 0, 
				"DisallowRowLocks" => 1, 
				"FileGroup" => 2, 
				"FillFactor" => 3, 
				"HasSparseColumn" => 4, 
				"ID" => 5, 
				"IgnoreDuplicateKeys" => 6, 
				"IndexKeyType" => 7, 
				"IndexType" => 8, 
				"IsClustered" => 9, 
				"IsFullTextKey" => 10, 
				"IsSystemNamed" => 11, 
				"IsSystemObject" => 12, 
				"IsUnique" => 13, 
				"NoAutomaticRecomputation" => 14, 
				"PadIndex" => 15, 
				"SpaceUsed" => 16, 
				"IsDisabled" => 17, 
				"IsPartitioned" => 18, 
				"IsXmlIndex" => 19, 
				"ParentXmlIndex" => 20, 
				"PartitionScheme" => 21, 
				"SecondaryXmlIndexType" => 22, 
				"BoundingBoxXMax" => 23, 
				"BoundingBoxXMin" => 24, 
				"BoundingBoxYMax" => 25, 
				"BoundingBoxYMin" => 26, 
				"CellsPerObject" => 27, 
				"FileStreamFileGroup" => 28, 
				"FileStreamPartitionScheme" => 29, 
				"FilterDefinition" => 30, 
				"HasCompressedPartitions" => 31, 
				"HasFilter" => 32, 
				"IsSpatialIndex" => 33, 
				"Level1Grid" => 34, 
				"Level2Grid" => 35, 
				"Level3Grid" => 36, 
				"Level4Grid" => 37, 
				"PolicyHealthState" => 38, 
				"SpatialIndexType" => 39, 
				"IndexedXmlPathName" => 40, 
				"IsFileTableDefined" => 41, 
				"BucketCount" => 42, 
				"IsMemoryOptimized" => 43, 
				"CompressionDelay" => 44, 
				"ResumableOperationState" => 45, 
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
		private double _BoundingBoxXMax;

		private double _BoundingBoxXMin;

		private double _BoundingBoxYMax;

		private double _BoundingBoxYMin;

		private int _CellsPerObject;

		private string _FileGroup;

		private string _FileStreamFileGroup;

		private string _FileStreamPartitionScheme;

		private byte _FillFactor;

		private bool _IgnoreDuplicateKeys;

		private string _IndexedXmlPathName;

		private IndexKeyType _IndexKeyType;

		private IndexType _IndexType;

		private bool _IsClustered;

		private bool _IsSpatialIndex;

		private bool _IsSystemNamed;

		private bool _IsUnique;

		private bool _IsXmlIndex;

		private SpatialGeoLevelSize _Level1Grid;

		private SpatialGeoLevelSize _Level2Grid;

		private SpatialGeoLevelSize _Level3Grid;

		private SpatialGeoLevelSize _Level4Grid;

		private bool _PadIndex;

		private string _PartitionScheme;

		private ResumableOperationStateType _ResumableOperationState;

		private SecondaryXmlIndexType _SecondaryXmlIndexType;

		private SpatialIndexType _SpatialIndexType;

		internal double BoundingBoxXMax
		{
			get
			{
				return _BoundingBoxXMax;
			}
			set
			{
				_BoundingBoxXMax = value;
			}
		}

		internal double BoundingBoxXMin
		{
			get
			{
				return _BoundingBoxXMin;
			}
			set
			{
				_BoundingBoxXMin = value;
			}
		}

		internal double BoundingBoxYMax
		{
			get
			{
				return _BoundingBoxYMax;
			}
			set
			{
				_BoundingBoxYMax = value;
			}
		}

		internal double BoundingBoxYMin
		{
			get
			{
				return _BoundingBoxYMin;
			}
			set
			{
				_BoundingBoxYMin = value;
			}
		}

		internal int CellsPerObject
		{
			get
			{
				return _CellsPerObject;
			}
			set
			{
				_CellsPerObject = value;
			}
		}

		internal string FileGroup
		{
			get
			{
				return _FileGroup;
			}
			set
			{
				_FileGroup = value;
			}
		}

		internal string FileStreamFileGroup
		{
			get
			{
				return _FileStreamFileGroup;
			}
			set
			{
				_FileStreamFileGroup = value;
			}
		}

		internal string FileStreamPartitionScheme
		{
			get
			{
				return _FileStreamPartitionScheme;
			}
			set
			{
				_FileStreamPartitionScheme = value;
			}
		}

		internal byte FillFactor
		{
			get
			{
				return _FillFactor;
			}
			set
			{
				_FillFactor = value;
			}
		}

		internal bool IgnoreDuplicateKeys
		{
			get
			{
				return _IgnoreDuplicateKeys;
			}
			set
			{
				_IgnoreDuplicateKeys = value;
			}
		}

		internal string IndexedXmlPathName
		{
			get
			{
				return _IndexedXmlPathName;
			}
			set
			{
				_IndexedXmlPathName = value;
			}
		}

		internal IndexKeyType IndexKeyType
		{
			get
			{
				return _IndexKeyType;
			}
			set
			{
				_IndexKeyType = value;
			}
		}

		internal IndexType IndexType
		{
			get
			{
				return _IndexType;
			}
			set
			{
				_IndexType = value;
			}
		}

		internal bool IsClustered
		{
			get
			{
				return _IsClustered;
			}
			set
			{
				_IsClustered = value;
			}
		}

		internal bool IsSpatialIndex
		{
			get
			{
				return _IsSpatialIndex;
			}
			set
			{
				_IsSpatialIndex = value;
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

		internal bool IsUnique
		{
			get
			{
				return _IsUnique;
			}
			set
			{
				_IsUnique = value;
			}
		}

		internal bool IsXmlIndex
		{
			get
			{
				return _IsXmlIndex;
			}
			set
			{
				_IsXmlIndex = value;
			}
		}

		internal SpatialGeoLevelSize Level1Grid
		{
			get
			{
				return _Level1Grid;
			}
			set
			{
				_Level1Grid = value;
			}
		}

		internal SpatialGeoLevelSize Level2Grid
		{
			get
			{
				return _Level2Grid;
			}
			set
			{
				_Level2Grid = value;
			}
		}

		internal SpatialGeoLevelSize Level3Grid
		{
			get
			{
				return _Level3Grid;
			}
			set
			{
				_Level3Grid = value;
			}
		}

		internal SpatialGeoLevelSize Level4Grid
		{
			get
			{
				return _Level4Grid;
			}
			set
			{
				_Level4Grid = value;
			}
		}

		internal bool PadIndex
		{
			get
			{
				return _PadIndex;
			}
			set
			{
				_PadIndex = value;
			}
		}

		internal string PartitionScheme
		{
			get
			{
				return _PartitionScheme;
			}
			set
			{
				_PartitionScheme = value;
			}
		}

		internal ResumableOperationStateType ResumableOperationState
		{
			get
			{
				return _ResumableOperationState;
			}
			set
			{
				_ResumableOperationState = value;
			}
		}

		internal SecondaryXmlIndexType SecondaryXmlIndexType
		{
			get
			{
				return _SecondaryXmlIndexType;
			}
			set
			{
				_SecondaryXmlIndexType = value;
			}
		}

		internal SpatialIndexType SpatialIndexType
		{
			get
			{
				return _SpatialIndexType;
			}
			set
			{
				_SpatialIndexType = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private int _BucketCount;

		private int _CompressionDelay;

		private bool _DisallowPageLocks;

		private bool _DisallowRowLocks;

		private string _FilterDefinition;

		private bool _HasCompressedPartitions;

		private bool _HasFilter;

		private bool _HasSparseColumn;

		private int _ID;

		private bool _IsDisabled;

		private bool _IsFileTableDefined;

		private bool _IsFullTextKey;

		private bool _IsMemoryOptimized;

		private bool _IsPartitioned;

		private bool _IsSystemObject;

		private bool _NoAutomaticRecomputation;

		private string _ParentXmlIndex;

		private PolicyHealthState _PolicyHealthState;

		private double _SpaceUsed;

		internal int BucketCount
		{
			get
			{
				return _BucketCount;
			}
			set
			{
				_BucketCount = value;
			}
		}

		internal int CompressionDelay
		{
			get
			{
				return _CompressionDelay;
			}
			set
			{
				_CompressionDelay = value;
			}
		}

		internal bool DisallowPageLocks
		{
			get
			{
				return _DisallowPageLocks;
			}
			set
			{
				_DisallowPageLocks = value;
			}
		}

		internal bool DisallowRowLocks
		{
			get
			{
				return _DisallowRowLocks;
			}
			set
			{
				_DisallowRowLocks = value;
			}
		}

		internal string FilterDefinition
		{
			get
			{
				return _FilterDefinition;
			}
			set
			{
				_FilterDefinition = value;
			}
		}

		internal bool HasCompressedPartitions
		{
			get
			{
				return _HasCompressedPartitions;
			}
			set
			{
				_HasCompressedPartitions = value;
			}
		}

		internal bool HasFilter
		{
			get
			{
				return _HasFilter;
			}
			set
			{
				_HasFilter = value;
			}
		}

		internal bool HasSparseColumn
		{
			get
			{
				return _HasSparseColumn;
			}
			set
			{
				_HasSparseColumn = value;
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

		internal bool IsDisabled
		{
			get
			{
				return _IsDisabled;
			}
			set
			{
				_IsDisabled = value;
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

		internal bool IsFullTextKey
		{
			get
			{
				return _IsFullTextKey;
			}
			set
			{
				_IsFullTextKey = value;
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

		internal bool IsPartitioned
		{
			get
			{
				return _IsPartitioned;
			}
			set
			{
				_IsPartitioned = value;
			}
		}

		internal bool IsSystemObject
		{
			get
			{
				return _IsSystemObject;
			}
			set
			{
				_IsSystemObject = value;
			}
		}

		internal bool NoAutomaticRecomputation
		{
			get
			{
				return _NoAutomaticRecomputation;
			}
			set
			{
				_NoAutomaticRecomputation = value;
			}
		}

		internal string ParentXmlIndex
		{
			get
			{
				return _ParentXmlIndex;
			}
			set
			{
				_ParentXmlIndex = value;
			}
		}

		internal PolicyHealthState PolicyHealthState
		{
			get
			{
				return _PolicyHealthState;
			}
			set
			{
				_PolicyHealthState = value;
			}
		}

		internal double SpaceUsed
		{
			get
			{
				return _SpaceUsed;
			}
			set
			{
				_SpaceUsed = value;
			}
		}
	}

	private delegate bool CheckColumnDataType(string datatype);

	private abstract class IndexScripter
	{
		protected ScriptSchemaObjectBase parent;

		protected Index index;

		protected ScriptingPreferences preferences;

		protected ColumnCollection columns;

		public bool TableCreate { get; set; }

		public bool? IsClustered
		{
			get
			{
				if (index.GetPropValueOptional<IndexType>("IndexType").HasValue && (!index.GetPropertyOptional("IsClustered").Dirty || index.GetPropertyOptional("IndexType").Dirty))
				{
					IndexType? propValueOptional = index.GetPropValueOptional<IndexType>("IndexType");
					return propValueOptional.GetValueOrDefault() == IndexType.ClusteredIndex && propValueOptional.HasValue;
				}
				return index.GetPropValueOptional<bool>("IsClustered");
			}
		}

		protected virtual bool IsIncludedColumnSupported => false;

		public IndexScripter(Index index, ScriptingPreferences sp)
		{
			this.index = index;
			parent = (ScriptSchemaObjectBase)index.ParentColl.ParentInstance;
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != parent, "parent == null");
			preferences = sp;
			TableCreate = false;
			if (parent is TableViewTableTypeBase)
			{
				columns = ((TableViewTableTypeBase)parent).Columns;
			}
			else if (parent is UserDefinedFunction)
			{
				columns = ((UserDefinedFunction)parent).Columns;
			}
			else
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition: false, "Invalid parent");
			}
		}

		protected abstract void Validate();

		protected void CheckConflictingProperties()
		{
			if (index.GetPropValueOptional<IndexType>("IndexType").HasValue && index.GetPropertyOptional("IndexType").Dirty && index.GetPropValueOptional<bool>("IsClustered").HasValue && index.GetPropertyOptional("IsClustered").Dirty && index.GetPropValueOptional<IndexType>("IndexType").Value == IndexType.ClusteredIndex != index.GetPropValueOptional<bool>("IsClustered"))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingIndexProperties, "IsClustered", index.GetPropValueOptional<bool>("IsClustered").ToString(), "IndexType", index.GetPropValueOptional<IndexType>("IndexType").ToString()));
			}
		}

		protected void CheckProperty<T>(string propertyName, T defaultValue, Exception exception)
		{
			if (index.IsSupportedProperty(propertyName))
			{
				Property property = index.Properties.Get(propertyName);
				if (!property.IsNull && !EqualityComparer<T>.Default.Equals((T)property.Value, defaultValue))
				{
					throw exception;
				}
			}
		}

		protected void CheckProperty<T>(T propertyValue, T defaultValue, Exception exception)
		{
			if (propertyValue != null && !EqualityComparer<T>.Default.Equals(propertyValue, defaultValue))
			{
				throw exception;
			}
		}

		protected void CheckSpatialProperties()
		{
			if (index.IsSupportedProperty("IsSpatialIndex"))
			{
				Exception exception = new SmoException(ExceptionTemplatesImpl.UnsupportedNonSpatialParameters);
				CheckProperty("SpatialIndexType", SpatialIndexType.None, exception);
				CheckProperty("BoundingBoxXMin", 0.0, exception);
				CheckProperty("BoundingBoxYMin", 0.0, exception);
				CheckProperty("BoundingBoxXMax", 0.0, exception);
				CheckProperty("BoundingBoxYMax", 0.0, exception);
				CheckProperty("CellsPerObject", 0, exception);
				CheckProperty("Level1Grid", SpatialGeoLevelSize.None, exception);
				CheckProperty("Level2Grid", SpatialGeoLevelSize.None, exception);
				CheckProperty("Level3Grid", SpatialGeoLevelSize.None, exception);
				CheckProperty("Level4Grid", SpatialGeoLevelSize.None, exception);
			}
		}

		protected void CheckXmlProperties()
		{
			if (index.IsSupportedProperty("IsXmlIndex"))
			{
				Exception exception = new SmoException(ExceptionTemplatesImpl.UnsupportedNonXmlParameters);
				CheckProperty("ParentXmlIndex", string.Empty, exception);
				CheckProperty("SecondaryXmlIndexType", SecondaryXmlIndexType.None, exception);
			}
		}

		protected void CheckNonClusteredProperties()
		{
			if (index.IsSupportedProperty("FilterDefinition"))
			{
				Exception exception = new SmoException(ExceptionTemplatesImpl.NotNonClusteredIndex);
				CheckProperty("FilterDefinition", string.Empty, exception);
			}
		}

		protected void CheckClusteredProperties()
		{
			if (index.IsSupportedProperty("FileStreamFileGroup"))
			{
				Exception exception = new SmoException(ExceptionTemplatesImpl.NotClusteredIndex);
				CheckProperty("FileStreamFileGroup", string.Empty, exception);
				CheckProperty("FileStreamPartitionScheme", string.Empty, exception);
			}
		}

		protected virtual void CheckRegularIndexProperties()
		{
			CheckProperty("IsClustered", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexClustered));
			CheckProperty("IsUnique", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexUnique));
			CheckProperty("IgnoreDuplicateKeys", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexIgnoreDupKey));
		}

		protected void CheckConstraintProperties()
		{
			CheckProperty("IndexKeyType", IndexKeyType.None, new WrongPropertyValueException(index.Properties.Get("IndexKeyType")));
			if (!(parent is TableViewBase) && !(parent is UserDefinedTableType))
			{
				throw new SmoException(ExceptionTemplatesImpl.IndexOnTableView);
			}
			if (index.IsDesignMode && index.GetIsSystemNamed())
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.PropertyNotSet("Name", typeof(Index).Name));
			}
		}

		public virtual string GetCreateScript()
		{
			Validate();
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptIndexHeader(stringBuilder);
			ScriptColumns(stringBuilder);
			ScriptIndexDetails(stringBuilder);
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptIndexOptions(stringBuilder2);
			ScriptDistribution(stringBuilder2);
			if (stringBuilder2.Length > 0)
			{
				stringBuilder2.Length -= Globals.commaspace.Length;
				stringBuilder.Append("WITH ");
				if (preferences.TargetServerVersionInternal != SqlServerVersionInternal.Version80)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0})", new object[1] { stringBuilder2.ToString() });
				}
				else
				{
					stringBuilder.Append(stringBuilder2.ToString());
				}
			}
			ScriptIndexStorage(stringBuilder);
			return stringBuilder.ToString();
		}

		protected virtual void ScriptIndexHeader(StringBuilder sb)
		{
			if (!index.IsSqlDwIndex || !TableCreate)
			{
				index.ScriptIncludeHeaders(sb, preferences, UrnSuffix);
				ScriptExistenceCheck(sb, not: true);
			}
			ScriptCreateHeaderDdl(sb);
		}

		protected virtual void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			throw new NotImplementedException();
		}

		protected virtual void ScriptIndexStorage(StringBuilder sb)
		{
			index.GenerateDataSpaceScript(sb, preferences);
		}

		protected void ScriptFileStream(StringBuilder sb)
		{
			if (index.IsSupportedProperty("FileStreamFileGroup", preferences))
			{
				index.GenerateDataSpaceFileStreamScript(sb, preferences, alterTable: false);
			}
		}

		protected virtual void ScriptIndexDetails(StringBuilder sb)
		{
		}

		protected virtual void ScriptIndexOptions(StringBuilder sb)
		{
			ScriptIndexOptions(sb, forRebuild: false, -1);
		}

		private void ScriptIndexOptions(StringBuilder sb, bool forRebuild, int rebuildPartitionNumber)
		{
			if (!preferences.TargetEngineIsAzureStretchDb() && !preferences.TargetEngineIsAzureSqlDw())
			{
				if (!forRebuild || rebuildPartitionNumber == -1)
				{
					if (preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
					{
						ScriptIndexOption(sb, "PAD_INDEX", GetOnOffValue(index.GetPropValueOptional<bool>("PadIndex")));
					}
					ScriptIndexOption(sb, "STATISTICS_NORECOMPUTE", GetOnOffValue(index.GetPropValueOptional<bool>("NoAutomaticRecomputation")));
				}
				if (preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					ScriptIndexOption(sb, "SORT_IN_TEMPDB", GetOnOffValue(index.sortInTempdb));
				}
				if ((!forRebuild || rebuildPartitionNumber == -1) && index.GetPropValueOptional("IsUnique", defaultValue: false) && index.GetPropValueOptional("IndexKeyType", IndexKeyType.None) == IndexKeyType.None)
				{
					ScriptIndexOption(sb, "IGNORE_DUP_KEY", GetOnOffValue(index.GetPropValueOptional<bool>("IgnoreDuplicateKeys")));
				}
			}
			if (!forRebuild && (!index.IsSqlDwIndex || !TableCreate))
			{
				ScriptIndexOption(sb, "DROP_EXISTING", GetOnOffValue(index.dropExistingIndex));
			}
			if (preferences.TargetEngineIsAzureStretchDb() || preferences.TargetEngineIsAzureSqlDw())
			{
				return;
			}
			if (((forRebuild && preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version140) || (preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version150 && !TableCreate && !preferences.ScriptForAlter) || (index.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && !TableCreate && !preferences.ScriptForAlter)) && (VersionUtils.IsSql14OrLater(index.ServerVersion) || index.DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase) && index.ResumableIndexOperation)
			{
				ScriptIndexOption(sb, "RESUMABLE", GetOnOffValue(index.ResumableIndexOperation));
				if (index.ResumableMaxDuration != 0)
				{
					ScriptIndexOption(sb, "MAX_DURATION", index.ResumableMaxDuration + " MINUTES");
				}
			}
			if (preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				if (!forRebuild || rebuildPartitionNumber == -1)
				{
					ScriptIndexOptionOnline(sb, forRebuild);
					if (preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
					{
						ScriptIndexOption(sb, "ALLOW_ROW_LOCKS", GetOnOffValue(RevertMeaning(index.GetPropValueOptional<bool>("DisallowRowLocks"))));
						ScriptIndexOption(sb, "ALLOW_PAGE_LOCKS", GetOnOffValue(RevertMeaning(index.GetPropValueOptional<bool>("DisallowPageLocks"))));
					}
				}
				else if (VersionUtils.IsSql12OrLater(index.ServerVersion))
				{
					ScriptIndexOptionOnline(sb, forRebuild);
				}
				if (index.ServerVersion.Major >= 9 && index.MaximumDegreeOfParallelism > 0)
				{
					ScriptIndexOption(sb, "MAXDOP", index.MaximumDegreeOfParallelism);
				}
			}
			if (!forRebuild || rebuildPartitionNumber == -1)
			{
				ScriptFillFactor(sb);
			}
		}

		protected void ScriptIndexOptionOnline(StringBuilder sb)
		{
			ScriptIndexOptionOnline(sb, forRebuild: false);
		}

		protected void ScriptIndexOptionOnline(StringBuilder sb, bool forRebuild)
		{
			if (!preferences.TargetEngineIsAzureStretchDb())
			{
				StringBuilder stringBuilder = new StringBuilder(GetOnOffValue(index.onlineIndexOperation));
				ScriptWaitAtLowPriorityIndexOption(stringBuilder, forRebuild);
				ScriptIndexOption(sb, "ONLINE", stringBuilder.ToString());
			}
		}

		protected void ScriptWaitAtLowPriorityIndexOption(StringBuilder sb, bool forRebuild)
		{
			if (VersionUtils.IsSql12OrLater(index.ServerVersion) && forRebuild && index.onlineIndexOperation)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, " (WAIT_AT_LOW_PRIORITY (MAX_DURATION = {0} MINUTES, ABORT_AFTER_WAIT = {1}))", new object[2]
				{
					index.lowPriorityMaxDuration,
					index.LowPriorityAbortAfterWait.ToString().ToUpper()
				});
			}
		}

		protected void ScriptWaitAtLowPriorityIndexOptionForDropAndResume(StringBuilder sb)
		{
			if (VersionUtils.IsSql14OrLater(index.ServerVersion) && index.LowPriorityMaxDuration != 0)
			{
				if (index.IsClustered && !index.OnlineIndexOperation)
				{
					throw new SmoException(ExceptionTemplatesImpl.LowPriorityCannotBeSetForDrop);
				}
				AbortAfterWaitConverter abortAfterWaitConverter = new AbortAfterWaitConverter();
				sb.AppendFormat(SmoApplication.DefaultCulture, "WAIT_AT_LOW_PRIORITY (MAX_DURATION = {0} MINUTES, ABORT_AFTER_WAIT = {1})", new object[2]
				{
					index.LowPriorityMaxDuration,
					abortAfterWaitConverter.ConvertToInvariantString(index.LowPriorityAbortAfterWait)
				});
				sb.Append(Globals.commaspace);
			}
		}

		protected void ScriptFillFactor(StringBuilder sb)
		{
			if (!SqlSmoObject.IsCloudAtSrcOrDest(index.DatabaseEngineType, preferences.TargetDatabaseEngineType))
			{
				Property property = index.Properties.Get("FillFactor");
				if (property.Value != null && !(parent is UserDefinedTableType) && (byte)property.Value != 0)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "FILLFACTOR = {0}", new object[1] { property.Value });
					sb.Append(Globals.commaspace);
				}
			}
		}

		protected bool? RevertMeaning(bool? propvalue)
		{
			if (propvalue.HasValue)
			{
				return !propvalue.Value;
			}
			return null;
		}

		protected string GetOnOffValue(bool? propValue)
		{
			if (propValue.HasValue)
			{
				if (!propValue.Value)
				{
					if (preferences.TargetServerVersionInternal != SqlServerVersionInternal.Version80)
					{
						return "OFF";
					}
					return null;
				}
				return "ON";
			}
			return null;
		}

		protected void ScriptIndexOption(StringBuilder sb, string optname, object propValue)
		{
			bool flag = preferences.TargetServerVersionInternal != SqlServerVersionInternal.Version80;
			if (propValue != null)
			{
				if (flag)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "{0} = {1}", new object[2] { optname, propValue });
				}
				else
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { optname });
				}
				sb.Append(Globals.commaspace);
			}
		}

		protected virtual void ScriptColumns(StringBuilder sb)
		{
			sb.Append(preferences.NewLine);
			if (index.IsSqlDwIndex && TableCreate)
			{
				sb.Append(Globals.tab);
			}
			sb.Append(Globals.LParen);
			sb.Append(preferences.NewLine);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (IndexedColumn indexedColumn in index.IndexedColumns)
			{
				Column column = columns[indexedColumn.Name];
				if (num > 0)
				{
					if (column == null)
					{
						throw new InvalidSmoOperationException(ExceptionTemplatesImpl.ExpectedGraphColumnNotFound);
					}
					if (column.GetPropValueOptional("GraphType", GraphType.None) != GraphType.GraphToId && column.GraphType != GraphType.GraphFromId)
					{
						throw new InvalidSmoOperationException(ExceptionTemplatesImpl.ExpectedGraphColumnNotFound);
					}
					num--;
					continue;
				}
				if (column != null)
				{
					GraphType propValueIfSupportedWithThrowOnTarget = column.GetPropValueIfSupportedWithThrowOnTarget("GraphType", GraphType.None, preferences);
					if (propValueIfSupportedWithThrowOnTarget == GraphType.GraphId && column.Parent is Table table)
					{
						stringBuilder.Append(Globals.tab);
						stringBuilder.Append(table.IsNode ? "$node_id" : "$edge_id");
						stringBuilder.Append(Globals.comma);
						stringBuilder.Append(preferences.NewLine);
						continue;
					}
					switch (propValueIfSupportedWithThrowOnTarget)
					{
					case GraphType.GraphToObjId:
						stringBuilder.Append(Globals.tab);
						stringBuilder.Append("$to_id");
						stringBuilder.Append(Globals.comma);
						stringBuilder.Append(preferences.NewLine);
						num = 1;
						continue;
					case GraphType.GraphFromObjId:
						stringBuilder.Append(Globals.tab);
						stringBuilder.Append("$from_id");
						stringBuilder.Append(Globals.comma);
						stringBuilder.Append(preferences.NewLine);
						num = 1;
						continue;
					}
				}
				if (ScriptColumn(indexedColumn, stringBuilder))
				{
					stringBuilder.Append(Globals.comma);
					stringBuilder.Append(preferences.NewLine);
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length -= preferences.NewLine.Length + Globals.comma.Length;
				sb.Append(stringBuilder.ToString());
				sb.Append(preferences.NewLine);
				if (index.IsSqlDwIndex && TableCreate)
				{
					sb.Append(Globals.tab);
				}
				sb.Append(Globals.RParen);
				return;
			}
			throw new SmoException(ExceptionTemplatesImpl.NoObjectWithoutColumns("Index"));
		}

		protected virtual bool ScriptColumn(IndexedColumn col, StringBuilder sb)
		{
			if (index.ServerVersion.Major > 9 && col.GetPropValueOptional("IsIncluded", defaultValue: false) && !IsIncludedColumnSupported)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.IncludedColumnNotSupported);
			}
			if (col.IsSupportedProperty("IsComputed"))
			{
				index.m_bIsOnComputed = ((!index.m_bIsOnComputed) ? col.GetPropValueOptional("IsComputed", defaultValue: false) : index.m_bIsOnComputed);
			}
			Column column = columns[col.Name];
			sb.Append(Globals.tab);
			if (index.IsSqlDwIndex && TableCreate)
			{
				sb.Append(Globals.tab);
			}
			if (column != null)
			{
				if (column.IsGraphComputedColumn() || IsGraphPseudoColumn(col.Name))
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { column.GetName(preferences) });
				}
				else
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(column.GetName(preferences)) });
				}
				index.isOnColumnWithAnsiPadding |= column.GetPropValueOptional("AnsiPaddingStatus", defaultValue: false);
			}
			else if (IsGraphPseudoColumn(col.Name))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { col.Name });
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(col.Name) });
			}
			return true;
		}

		protected void ScriptColumnOrder(IndexedColumn col, StringBuilder sb)
		{
			bool? propValueOptional = col.GetPropValueOptional<bool>("Descending");
			if (propValueOptional.HasValue)
			{
				sb.Append(propValueOptional.Value ? " DESC" : " ASC");
			}
		}

		protected void ScriptCompression(StringBuilder sb)
		{
			if (preferences.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse && preferences.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlStretchDatabase && index.IsSupportedProperty("HasCompressedPartitions", preferences) && preferences.Storage.DataCompression && index.IsCompressionCodeRequired(bAlter: false))
			{
				Server serverObject = index.GetServerObject();
				string[] fields = new string[2] { "PartitionNumber", "DataCompression" };
				serverObject.SetDefaultInitFields(typeof(PhysicalPartition), index.DatabaseEngineEdition, fields);
				string compressionCode = index.PhysicalPartitions.GetCompressionCode(isOnAlter: false, isOnTable: false, preferences);
				if (!string.IsNullOrEmpty(compressionCode))
				{
					sb.Append(index.PhysicalPartitions.GetCompressionCode(isOnAlter: false, isOnTable: false, preferences));
					sb.Append(Globals.commaspace);
				}
			}
		}

		protected void ScriptDistribution(StringBuilder sb)
		{
		}

		protected void ScriptFilter(StringBuilder sb)
		{
			if (index.IsSupportedProperty("FilterDefinition", preferences))
			{
				string text = index.Properties.Get("FilterDefinition").Value as string;
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
				{
					sb.Append(preferences.NewLine);
					sb.AppendFormat(SmoApplication.DefaultCulture, "WHERE {0}", new object[1] { text.Trim() });
					sb.Append(preferences.NewLine);
				}
			}
		}

		public string GetDropScript()
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			index.ScriptIncludeHeaders(stringBuilder, preferences, UrnSuffix);
			if (preferences.TargetServerVersionInternal < SqlServerVersionInternal.Version130 || index.IsMemoryOptimizedIndex)
			{
				ScriptExistenceCheck(stringBuilder, not: false);
			}
			else if (this is ConstraintScripter && preferences.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
				{
					"",
					SqlSmoObject.SqlString(parent.FormatFullNameForScripting(preferences))
				});
				stringBuilder.Append(preferences.NewLine);
			}
			ScriptDropHeaderDdl(stringBuilder);
			if (preferences.TargetServerVersionInternal > SqlServerVersionInternal.Version80)
			{
				ScriptDropOptions(stringBuilder);
			}
			return stringBuilder.ToString();
		}

		protected virtual void ScriptDropOptions(StringBuilder sb)
		{
		}

		protected virtual void ScriptDropHeaderDdl(StringBuilder sb)
		{
			if (preferences.TargetServerVersionInternal <= SqlServerVersionInternal.Version80)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "DROP INDEX {0}.{1}", new object[2]
				{
					parent.FormatFullNameForScripting(preferences),
					index.FormatFullNameForScripting(preferences)
				});
			}
			else if (index.IsMemoryOptimizedIndex)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP INDEX {1}", new object[2]
				{
					parent.FormatFullNameForScripting(preferences),
					index.FormatFullNameForScripting(preferences)
				});
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "DROP INDEX {0}{1} ON {2}", new object[3]
				{
					(preferences.IncludeScripts.ExistenceCheck && preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
					index.FormatFullNameForScripting(preferences),
					parent.FormatFullNameForScripting(preferences)
				});
			}
		}

		public string GetRebuildScript(bool allIndexes, int rebuildPartitionNumber)
		{
			if (index.ServerVersion.Major < 9 || (parent is View && ((View)parent).Parent.CompatibilityLevel < CompatibilityLevel.Version90))
			{
				return Rebuild80(allIndexes);
			}
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (allIndexes)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX ALL ON {0} REBUILD", new object[1] { parent.FullQualifiedName });
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				if (rebuildPartitionNumber != -1)
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "PARTITION = {0}", new object[1] { rebuildPartitionNumber });
				}
				else if (preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version100)
				{
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "PARTITION = ALL");
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX [{0}] ON {1} REBUILD {2}", new object[3]
				{
					SqlSmoObject.SqlBraket(index.Name),
					parent.FullQualifiedName,
					stringBuilder2.ToString()
				});
			}
			StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptIndexRebuildOptions(stringBuilder3, rebuildPartitionNumber);
			if (!allIndexes && rebuildPartitionNumber == -1)
			{
				ScriptDistribution(stringBuilder3);
			}
			if (stringBuilder3.Length > 0)
			{
				stringBuilder3.Length -= Globals.commaspace.Length;
				stringBuilder.Append(" WITH ");
				if (preferences.TargetServerVersionInternal != SqlServerVersionInternal.Version80)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0})", new object[1] { stringBuilder3.ToString() });
				}
				else
				{
					stringBuilder.Append(stringBuilder3.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		protected virtual void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
			ScriptIndexOptions(withClause, forRebuild: true, rebuildPartitionNumber);
		}

		private string Rebuild80(bool allIndexes)
		{
			if (allIndexes)
			{
				return string.Format(SmoApplication.DefaultCulture, "DBCC DBREINDEX(N'{0}')", new object[1] { SqlSmoObject.SqlString(parent.FullQualifiedName) });
			}
			return string.Format(SmoApplication.DefaultCulture, "DBCC DBREINDEX(N'{0}', N'{1}', {2})", new object[3]
			{
				SqlSmoObject.SqlString(parent.FullQualifiedName),
				SqlSmoObject.SqlString(index.Name),
				(byte)index.Properties["FillFactor"].Value
			});
		}

		public string GetResumeScript()
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX [{0}] ON {1} RESUME", new object[2]
			{
				SqlSmoObject.SqlBraket(index.Name),
				parent.FullQualifiedName
			});
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptIndexResumeOptions(stringBuilder2);
			if (stringBuilder2.Length > 0)
			{
				stringBuilder2.Length -= Globals.commaspace.Length;
				stringBuilder.Append(" WITH ");
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0})", new object[1] { stringBuilder2.ToString() });
			}
			return stringBuilder.ToString();
		}

		private void ScriptIndexResumeOptions(StringBuilder sb)
		{
			if (index.MaximumDegreeOfParallelism > 0)
			{
				ScriptIndexOption(sb, "MAXDOP", index.MaximumDegreeOfParallelism);
			}
			if (index.ResumableMaxDuration != 0)
			{
				ScriptIndexOption(sb, "MAX_DURATION", index.ResumableMaxDuration + " MINUTES");
			}
			ScriptWaitAtLowPriorityIndexOptionForDropAndResume(sb);
		}

		public string GetAbortOrPauseScript(bool isAbort)
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX [{0}] ON {1} {2}", new object[3]
			{
				SqlSmoObject.SqlBraket(index.Name),
				parent.FullQualifiedName,
				isAbort ? "ABORT" : "PAUSE"
			});
			return stringBuilder.ToString();
		}

		public string GetAlterScript90(bool allIndexes)
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptSetIndexOptions(stringBuilder);
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length -= Globals.commaspace.Length;
				return string.Format(SmoApplication.DefaultCulture, "ALTER INDEX {0} ON {1} SET ( {2} )", new object[3]
				{
					allIndexes ? "ALL" : index.FullQualifiedName,
					parent.FullQualifiedName,
					stringBuilder.ToString()
				});
			}
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptAlterDetails(stringBuilder2);
			if (this is HashIndexScripter)
			{
				return string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ALTER INDEX {1} {2}", new object[3]
				{
					parent.FormatFullNameForScripting(preferences),
					index.FormatFullNameForScripting(preferences),
					stringBuilder2.ToString()
				});
			}
			if (stringBuilder2.Length > 0)
			{
				return string.Format(SmoApplication.DefaultCulture, "ALTER INDEX {0} ON {1} {2}", new object[3]
				{
					allIndexes ? "ALL" : index.FullQualifiedName,
					parent.FullQualifiedName,
					stringBuilder2.ToString()
				});
			}
			return string.Empty;
		}

		protected virtual void ScriptAlterDetails(StringBuilder sb)
		{
		}

		protected virtual void ScriptSetIndexOptions(StringBuilder setClause)
		{
			if (!preferences.TargetEngineIsAzureStretchDb())
			{
				ScriptIndexOption(setClause, "ALLOW_ROW_LOCKS", GetOnOffValue(RevertMeaning(GetDirtyPropValueOptional<bool>("DisallowRowLocks"))));
				ScriptIndexOption(setClause, "ALLOW_PAGE_LOCKS", GetOnOffValue(RevertMeaning(GetDirtyPropValueOptional<bool>("DisallowPageLocks"))));
				ScriptIndexOption(setClause, "STATISTICS_NORECOMPUTE", GetOnOffValue(GetDirtyPropValueOptional<bool>("NoAutomaticRecomputation")));
				ScriptIndexOption(setClause, "IGNORE_DUP_KEY", GetOnOffValue(GetDirtyPropValueOptional<bool>("IgnoreDuplicateKeys")));
			}
		}

		protected T? GetDirtyPropValueOptional<T>(string propName) where T : struct
		{
			if (index.Properties.Get(propName).Dirty)
			{
				return index.GetPropValueOptional<T>(propName);
			}
			return null;
		}

		protected void ScriptExistenceCheck(StringBuilder sb, bool not)
		{
			if (preferences.IncludeScripts.ExistenceCheck)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, (preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_INDEX90 : Scripts.INCLUDE_EXISTS_INDEX80, new object[3]
				{
					not ? "NOT" : string.Empty,
					SqlSmoObject.SqlString(parent.FormatFullNameForScripting(preferences)),
					index.FormatFullNameForScripting(preferences, nameIsIndentifier: false)
				});
				sb.Append(preferences.NewLine);
			}
		}

		public static IndexScripter GetIndexScripterForCreate(Index index, ScriptingPreferences sp)
		{
			index.isOnColumnWithAnsiPadding = false;
			if (index.IsMemoryOptimizedIndex)
			{
				return GetMemoryOptimizedIndexScripter(index, sp);
			}
			if (index.IsSqlDwIndex)
			{
				return GetSqlDwIndexScripter(index, sp);
			}
			if ((!index.GetPropValueOptional<IndexType>("IndexType").HasValue && (index.State == SqlSmoState.Creating || index.IsDesignMode)) || (index.GetPropValueOptional<IndexType>("IndexType").Value.Equals(IndexType.NonClusteredColumnStoreIndex) && index.GetPropertyOptional("IsClustered").Dirty && !index.GetPropertyOptional("IndexType").Dirty))
			{
				return GetCreatingIndexScripterForCreate(index, sp);
			}
			return GetExistingIndexScripterForCreate(index, sp);
		}

		private static IndexScripter GetCreatingIndexScripterForCreate(Index index, ScriptingPreferences sp)
		{
			ColumnCollection columnCollection = (index.Parent.Urn.Type.Equals(UserDefinedFunction.UrnSuffix) ? ((UserDefinedFunction)index.Parent).Columns : ((TableViewTableTypeBase)index.Parent).Columns);
			foreach (IndexedColumn indexedColumn in index.IndexedColumns)
			{
				Column column = columnCollection[indexedColumn.Name];
				if (column == null)
				{
					if (index.Parent.State != SqlSmoState.Creating || !(index.Parent is View))
					{
						ScriptSchemaObjectBase scriptSchemaObjectBase = index.Parent as TableViewTableTypeBase;
						throw new SmoException(ExceptionTemplatesImpl.ObjectRefsNonexCol(UrnSuffix, index.Name, scriptSchemaObjectBase.FullQualifiedName + ".[" + SqlSmoObject.SqlStringBraket(indexedColumn.Name) + "]"));
					}
					continue;
				}
				if (column.IgnoreForScripting)
				{
					index.IgnoreForScripting = true;
					return null;
				}
				if (indexedColumn.IsSupportedProperty("IsIncluded") && !indexedColumn.GetPropValueOptional("IsIncluded", defaultValue: false) && column.DataType.SqlDataType == SqlDataType.Xml)
				{
					if (!string.IsNullOrEmpty(index.GetPropValueOptional("ParentXmlIndex", string.Empty).ToString()))
					{
						return new SecondaryXmlIndexScripter(index, sp);
					}
					return new PrimaryXmlIndexScripter(index, sp);
				}
				if (indexedColumn.IsSupportedProperty("IsIncluded") && !indexedColumn.GetPropValueOptional("IsIncluded", defaultValue: false) && index.IsSpatialColumn(column))
				{
					return new SpatialIndexScripter(index, sp);
				}
			}
			if (GetSupportedPropertyValue(index, "IndexKeyType", IndexKeyType.None) != IndexKeyType.None && !index.dropExistingIndex && !index.IsMemoryOptimizedIndex)
			{
				return new ConstraintScripter(index, sp);
			}
			if (GetSupportedPropertyValue(index, "IsClustered", defaultValue: false))
			{
				return new ClusteredRegularIndexScripter(index, sp);
			}
			return new NonClusteredRegularIndexScripter(index, sp);
		}

		private static IndexScripter GetExistingIndexScripterForCreate(Index index, ScriptingPreferences sp)
		{
			if (GetSupportedPropertyValue(index, "IndexKeyType", IndexKeyType.None) != IndexKeyType.None && !index.dropExistingIndex && !index.IsMemoryOptimizedIndex)
			{
				return new ConstraintScripter(index, sp);
			}
			return GetIndexScripter(index, sp);
		}

		public static IndexScripter GetIndexScripterForDrop(Index index, ScriptingPreferences sp)
		{
			Property property = index.Properties.Get("IndexKeyType");
			IndexKeyType indexKeyType = index.GetPropValueOptional("IndexKeyType", IndexKeyType.None);
			if (property.Dirty && index.State != SqlSmoState.Creating)
			{
				indexKeyType = (IndexKeyType)index.GetRealValue(property, index.oldIndexKeyTypeValue);
			}
			if (indexKeyType != IndexKeyType.None)
			{
				return new ConstraintScripter(index, sp);
			}
			if (GetSupportedPropertyValue(index, "IndexType", IndexType.NonClusteredIndex) == IndexType.ClusteredColumnStoreIndex)
			{
				return new ClusteredColumnstoreIndexScripter(index, sp);
			}
			if (GetSupportedPropertyValue(index, "IndexType", IndexType.NonClusteredIndex) == IndexType.ClusteredIndex || GetSupportedPropertyValue(index, "IsClustered", defaultValue: false))
			{
				return new ClusteredRegularIndexScripter(index, sp);
			}
			return new NonClusteredRegularIndexScripter(index, sp);
		}

		public static IndexScripter GetMemoryOptimizedIndexScripter(Index index, ScriptingPreferences sp)
		{
			return index.InferredIndexType switch
			{
				IndexType.NonClusteredIndex => new RangeIndexScripter(index, sp), 
				IndexType.NonClusteredHashIndex => new HashIndexScripter(index, sp), 
				IndexType.ClusteredColumnStoreIndex => new ClusteredColumnstoreIndexScripter(index, sp), 
				_ => throw new InvalidSmoOperationException(ExceptionTemplatesImpl.TableMemoryOptimizedIndexDependency), 
			};
		}

		public static IndexScripter GetSqlDwIndexScripter(Index index, ScriptingPreferences sp)
		{
			return index.InferredIndexType switch
			{
				IndexType.ClusteredColumnStoreIndex => new ClusteredColumnstoreIndexScripter(index, sp), 
				IndexType.ClusteredIndex => new ClusteredRegularIndexScripter(index, sp), 
				IndexType.NonClusteredIndex => new NonClusteredRegularIndexScripter(index, sp), 
				_ => throw new InvalidSmoOperationException(ExceptionTemplatesImpl.TableSqlDwIndexTypeRestrictions(index.InferredIndexType.ToString())), 
			};
		}

		public static IndexScripter GetIndexScripter(Index index, ScriptingPreferences sp)
		{
			if (index.IsMemoryOptimizedIndex)
			{
				return GetMemoryOptimizedIndexScripter(index, sp);
			}
			if (index.IsSqlDwIndex)
			{
				return GetSqlDwIndexScripter(index, sp);
			}
			switch (index.InferredIndexType)
			{
			case IndexType.ClusteredIndex:
				if (index.Parent is UserDefinedTableType)
				{
					return new UserDefinedTableTypeIndexScripter(index, sp);
				}
				return new ClusteredRegularIndexScripter(index, sp);
			case IndexType.NonClusteredIndex:
				if (index.Parent is UserDefinedTableType)
				{
					return new UserDefinedTableTypeIndexScripter(index, sp);
				}
				return new NonClusteredRegularIndexScripter(index, sp);
			case IndexType.PrimaryXmlIndex:
				return new PrimaryXmlIndexScripter(index, sp);
			case IndexType.SecondaryXmlIndex:
				return new SecondaryXmlIndexScripter(index, sp);
			case IndexType.SpatialIndex:
				return new SpatialIndexScripter(index, sp);
			case IndexType.NonClusteredColumnStoreIndex:
				return new NonClusteredColumnStoreIndexScripter(index, sp);
			case IndexType.NonClusteredHashIndex:
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.HashIndexTableDependency);
			case IndexType.SelectiveXmlIndex:
				return new SelectiveXMLIndexScripter(index, sp);
			case IndexType.SecondarySelectiveXmlIndex:
				return new SecondarySelectiveXMLIndexScripter(index, sp);
			case IndexType.ClusteredColumnStoreIndex:
				return new ClusteredColumnstoreIndexScripter(index, sp);
			default:
				throw new WrongPropertyValueException(index.Properties["IndexType"]);
			}
		}

		public static IndexScripter GetIndexScripterForAlter(Index index, ScriptingPreferences sp)
		{
			if (GetSupportedPropertyValue(index, "IndexType", IndexType.NonClusteredIndex) == IndexType.NonClusteredColumnStoreIndex)
			{
				return new NonClusteredColumnStoreIndexScripter(index, sp);
			}
			if (GetSupportedPropertyValue(index, "IndexType", IndexType.ClusteredIndex) == IndexType.ClusteredColumnStoreIndex)
			{
				return new ClusteredColumnstoreIndexScripter(index, sp);
			}
			if (GetSupportedPropertyValue(index, "IndexType", IndexType.NonClusteredIndex) == IndexType.SelectiveXmlIndex)
			{
				return new SelectiveXMLIndexScripter(index, sp);
			}
			if (index.IsMemoryOptimizedIndex)
			{
				return GetIndexScripter(index, sp);
			}
			return new NonClusteredRegularIndexScripter(index, sp);
		}

		private static T GetSupportedPropertyValue<T>(Index index, string propertyName, T defaultValue)
		{
			if (index.IsSupportedProperty(propertyName))
			{
				return index.GetPropValueOptional(propertyName, defaultValue);
			}
			return defaultValue;
		}
	}

	private class ConstraintScripter : IndexScripter
	{
		public ConstraintScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			bool? isClustered = base.IsClustered;
			int num = ((IndexKeyType.DriPrimaryKey == index.GetIndexKeyType()) ? 1 : 0);
			if ((((int?)isClustered) ?? num) == 0)
			{
				CheckClusteredProperties();
			}
			CheckConflictingProperties();
			CheckNonClusteredProperties();
			CheckXmlProperties();
			CheckSpatialProperties();
		}

		protected override void ScriptIndexHeader(StringBuilder sb)
		{
			if (!base.TableCreate)
			{
				index.ScriptIncludeHeaders(sb, preferences, UrnSuffix);
				ScriptExistenceCheck(sb, not: true);
				sb.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ADD ", new object[1] { parent.FormatFullNameForScripting(preferences) });
			}
			if (parent is Table)
			{
				index.AddConstraintName(sb, preferences);
			}
			sb.Append((IndexKeyType.DriPrimaryKey == index.GetIndexKeyType()) ? "PRIMARY KEY " : "UNIQUE ");
			if (base.IsClustered.HasValue)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, base.IsClustered.Value ? "CLUSTERED " : "NONCLUSTERED ");
			}
		}

		protected override bool ScriptColumn(IndexedColumn col, StringBuilder sb)
		{
			base.ScriptColumn(col, sb);
			ScriptColumnOrder(col, sb);
			return true;
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
			if (preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && !preferences.TargetEngineIsAzureStretchDb())
			{
				if (parent is UserDefinedTableType)
				{
					ScriptIndexOption(sb, "IGNORE_DUP_KEY", GetOnOffValue(index.GetPropValueOptional<bool>("IgnoreDuplicateKeys")));
					return;
				}
				bool flag = parent is Table && !base.TableCreate;
				if (preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					ScriptIndexOption(sb, "PAD_INDEX", GetOnOffValue(index.GetPropValueOptional<bool>("PadIndex")));
				}
				ScriptIndexOption(sb, "STATISTICS_NORECOMPUTE", GetOnOffValue(index.GetPropValueOptional<bool>("NoAutomaticRecomputation")));
				if (flag && preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					ScriptIndexOption(sb, "SORT_IN_TEMPDB", GetOnOffValue(index.sortInTempdb));
				}
				ScriptIndexOption(sb, "IGNORE_DUP_KEY", GetOnOffValue(index.GetPropValueOptional<bool>("IgnoreDuplicateKeys")));
				if (flag)
				{
					ScriptIndexOptionOnline(sb);
				}
				if (preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					ScriptIndexOption(sb, "ALLOW_ROW_LOCKS", GetOnOffValue(RevertMeaning(index.GetPropValueOptional<bool>("DisallowRowLocks"))));
					ScriptIndexOption(sb, "ALLOW_PAGE_LOCKS", GetOnOffValue(RevertMeaning(index.GetPropValueOptional<bool>("DisallowPageLocks"))));
				}
				if (flag && index.ServerVersion.Major >= 9 && index.MaximumDegreeOfParallelism > 0)
				{
					ScriptIndexOption(sb, "MAXDOP", index.MaximumDegreeOfParallelism);
				}
				ScriptFillFactor(sb);
			}
			else
			{
				ScriptFillFactor(sb);
			}
			ScriptCompression(sb);
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
			base.ScriptIndexStorage(sb);
			bool? isClustered = base.IsClustered;
			bool num = index.GetIndexKeyType() != IndexKeyType.DriUniqueKey;
			if (isClustered ?? num)
			{
				ScriptFileStream(sb);
			}
		}

		protected override void ScriptDropHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP CONSTRAINT {1}{2}", new object[3]
			{
				parent.FormatFullNameForScripting(preferences),
				(preferences.IncludeScripts.ExistenceCheck && preferences.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
				index.FormatFullNameForScripting(preferences)
			});
		}

		protected override void ScriptDropOptions(StringBuilder sb)
		{
			if (preferences.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && (index.ServerVersion.Major < 12 || preferences.TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (base.IsClustered == true)
			{
				ScriptIndexOptionOnline(stringBuilder);
				if (index.MaximumDegreeOfParallelism > 0)
				{
					ScriptIndexOption(stringBuilder, "MAXDOP", index.MaximumDegreeOfParallelism);
				}
			}
			ScriptWaitAtLowPriorityIndexOptionForDropAndResume(stringBuilder);
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length -= Globals.commaspace.Length;
				sb.AppendFormat(SmoApplication.DefaultCulture, " WITH ( {0} )", new object[1] { stringBuilder.ToString() });
			}
		}
	}

	private class RegularIndexScripter : IndexScripter
	{
		public bool IsUnique => index.GetPropValueOptional("IsUnique", defaultValue: false);

		public RegularIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			if (!index.dropExistingIndex)
			{
				CheckConstraintProperties();
			}
			CheckConflictingProperties();
			CheckXmlProperties();
			CheckSpatialProperties();
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			if (index.IsSqlDwIndex && base.TableCreate)
			{
				if (!(base.IsClustered ?? false))
				{
					throw new InvalidSmoOperationException(ExceptionTemplatesImpl.UnexpectedIndexTypeDetected(index.InferredIndexType.ToString()));
				}
				sb.Append(Globals.tab);
				TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(IndexType));
				sb.AppendFormat(SmoApplication.DefaultCulture, typeConverter.ConvertToInvariantString(IndexType.ClusteredIndex));
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE {0}{1} INDEX {2} ON {3}", IsUnique ? "UNIQUE " : string.Empty, (base.IsClustered ?? false) ? "CLUSTERED" : "NONCLUSTERED", index.FormatFullNameForScripting(preferences), parent.FormatFullNameForScripting(preferences));
			}
		}

		protected override bool ScriptColumn(IndexedColumn col, StringBuilder sb)
		{
			base.ScriptColumn(col, sb);
			ScriptColumnOrder(col, sb);
			return true;
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
			base.ScriptIndexOptions(sb);
			ScriptCompression(sb);
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
			base.ScriptIndexRebuildOptions(withClause, rebuildPartitionNumber);
			if (rebuildPartitionNumber != -1 && index.PhysicalPartitions.IsDataCompressionStateDirty(rebuildPartitionNumber))
			{
				withClause.Append(index.PhysicalPartitions.GetCompressionCode(rebuildPartitionNumber));
				withClause.Append(Globals.commaspace);
			}
			else
			{
				ScriptCompression(withClause);
			}
		}
	}

	private class NonClusteredRegularIndexScripter : RegularIndexScripter
	{
		private List<IndexedColumn> includedColumns;

		protected override bool IsIncludedColumnSupported => true;

		public NonClusteredRegularIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
			includedColumns = new List<IndexedColumn>();
		}

		protected override void Validate()
		{
			base.Validate();
			CheckClusteredProperties();
		}

		protected override bool ScriptColumn(IndexedColumn col, StringBuilder sb)
		{
			if (index.ServerVersion.Major < 9 || !col.GetPropValueOptional("IsIncluded", defaultValue: false))
			{
				return base.ScriptColumn(col, sb);
			}
			includedColumns.Add(col);
			return false;
		}

		protected override void ScriptIndexDetails(StringBuilder sb)
		{
			ScriptIncludedColumns(sb, includedColumns);
			ScriptFilter(sb);
		}

		private void ScriptIncludedColumns(StringBuilder sb, List<IndexedColumn> includedColumns)
		{
			if (0 >= includedColumns.Count || preferences.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
			{
				return;
			}
			sb.Append(preferences.NewLine);
			sb.Append("INCLUDE ( ");
			foreach (IndexedColumn includedColumn in includedColumns)
			{
				index.m_bIsOnComputed = ((!index.m_bIsOnComputed) ? includedColumn.GetPropValueOptional("IsComputed", defaultValue: false) : index.m_bIsOnComputed);
				Column column = (Column)columns.NoFaultLookup(new SimpleObjectKey(includedColumn.Name));
				sb.Append(Globals.tab);
				if (column != null)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(column.GetName(preferences)) });
					index.isOnColumnWithAnsiPadding |= column.GetPropValueOptional("AnsiPaddingStatus", defaultValue: false);
				}
				else
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(includedColumn.Name) });
				}
				sb.Append(Globals.comma);
				sb.Append(preferences.NewLine);
			}
			sb.Length -= preferences.NewLine.Length + Globals.comma.Length;
			sb.Append(") ");
		}

		protected override void ScriptDropOptions(StringBuilder sb)
		{
			if (preferences.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase || (index.ServerVersion.Major >= 12 && preferences.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse))
			{
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				ScriptWaitAtLowPriorityIndexOptionForDropAndResume(stringBuilder);
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Length -= Globals.commaspace.Length;
					sb.AppendFormat(SmoApplication.DefaultCulture, " WITH ( {0} )", new object[1] { stringBuilder.ToString() });
				}
			}
		}
	}

	private class ClusteredRegularIndexScripter : RegularIndexScripter
	{
		private string dataSpaceName;

		private StringCollection partitionSchemeParameters;

		public string DataSpaceName
		{
			get
			{
				return dataSpaceName;
			}
			set
			{
				dataSpaceName = value;
			}
		}

		public StringCollection PartitionSchemeParameters
		{
			get
			{
				return partitionSchemeParameters;
			}
			set
			{
				partitionSchemeParameters = value;
			}
		}

		public ClusteredRegularIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			base.Validate();
			CheckNonClusteredProperties();
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
			base.ScriptIndexStorage(sb);
			ScriptFileStream(sb);
		}

		protected override void ScriptDropOptions(StringBuilder sb)
		{
			if (preferences.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && (index.ServerVersion.Major < 12 || preferences.TargetDatabaseEngineEdition == DatabaseEngineEdition.SqlDataWarehouse))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			ScriptIndexOptionOnline(stringBuilder);
			if (index.MaximumDegreeOfParallelism > 0)
			{
				ScriptIndexOption(stringBuilder, "MAXDOP", index.MaximumDegreeOfParallelism);
			}
			if (DataSpaceName != null)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "MOVE TO [{0}]", new object[1] { SqlSmoObject.SqlBraket(DataSpaceName) });
				if (PartitionSchemeParameters != null)
				{
					stringBuilder.Append("(");
					int num = 0;
					StringEnumerator enumerator = PartitionSchemeParameters.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							if (0 < num++)
							{
								stringBuilder.Append(Globals.commaspace);
							}
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "[{0}]", new object[1] { SqlSmoObject.SqlBraket(current) });
						}
					}
					finally
					{
						if (enumerator is IDisposable disposable)
						{
							disposable.Dispose();
						}
					}
					stringBuilder.Append(")");
				}
				stringBuilder.Append(Globals.commaspace);
			}
			ScriptWaitAtLowPriorityIndexOptionForDropAndResume(stringBuilder);
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Length -= Globals.commaspace.Length;
				sb.AppendFormat(SmoApplication.DefaultCulture, " WITH ( {0} )", new object[1] { stringBuilder.ToString() });
			}
		}
	}

	private class XmlIndexScripter : IndexScripter
	{
		public XmlIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
			index.xmlOrSpatialIndex = true;
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion90(preferences.TargetServerVersionInternal);
			index.ThrowIfCompatibilityLevelBelow90();
			if (preferences.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				SqlSmoObject.ThrowIfBelowVersion120(preferences.TargetServerVersionInternal);
			}
			if (parent is View)
			{
				throw new SmoException(ExceptionTemplatesImpl.NotXmlIndexOnView);
			}
			if (index.IndexedColumns.Count != 1)
			{
				throw new SmoException(ExceptionTemplatesImpl.OneColumnInXmlIndex);
			}
			CheckConstraintProperties();
			CheckRegularIndexProperties();
			CheckClusteredProperties();
			CheckNonClusteredProperties();
			CheckSpatialProperties();
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
		}
	}

	private class PrimaryXmlIndexScripter : XmlIndexScripter
	{
		public PrimaryXmlIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			base.Validate();
			if (!string.IsNullOrEmpty(index.GetPropValueOptional("ParentXmlIndex", string.Empty)))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingIndexProperties, "ParentXmlIndex", index.GetPropValueOptional("ParentXmlIndex").ToString(), "IndexType", index.GetPropValueOptional<IndexType>("IndexType").ToString()));
			}
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE PRIMARY XML INDEX {0} ON {1}", new object[2]
			{
				index.FormatFullNameForScripting(preferences),
				parent.FormatFullNameForScripting(preferences)
			});
		}
	}

	private class SecondaryXmlIndexScripter : XmlIndexScripter
	{
		public string ParentXmlIndex => (string)index.Properties["ParentXmlIndex"].Value;

		public SecondaryXmlIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			base.Validate();
			if (string.IsNullOrEmpty(index.GetPropValueOptional("ParentXmlIndex", string.Empty)))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingIndexProperties, "ParentXmlIndex", string.Empty, "IndexType", index.GetPropValueOptional<IndexType>("IndexType").ToString()));
			}
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE XML INDEX {0} ON {1}", new object[2]
			{
				index.FormatFullNameForScripting(preferences),
				parent.FormatFullNameForScripting(preferences)
			});
		}

		protected override void ScriptIndexDetails(StringBuilder sb)
		{
			sb.Append(preferences.NewLine);
			sb.AppendFormat(SmoApplication.DefaultCulture, "USING XML INDEX {0} ", new object[1] { SqlSmoObject.MakeSqlBraket(ParentXmlIndex) });
			switch ((SecondaryXmlIndexType)index.GetPropValue("SecondaryXmlIndexType"))
			{
			case SecondaryXmlIndexType.Path:
				sb.Append("FOR PATH ");
				break;
			case SecondaryXmlIndexType.Value:
				sb.Append("FOR VALUE ");
				break;
			case SecondaryXmlIndexType.Property:
				sb.Append("FOR PROPERTY ");
				break;
			default:
				throw new WrongPropertyValueException(index.Properties.Get("SecondaryXmlIndexType"));
			}
		}
	}

	private class SpatialIndexScripter : IndexScripter
	{
		private Property spatialIndexType;

		private Property xMin;

		private Property yMin;

		private Property xMax;

		private Property yMax;

		private Property level1;

		private Property level2;

		private Property level3;

		private Property level4;

		private Property cellsPerObject;

		public SpatialIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
			index.xmlOrSpatialIndex = true;
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion100(preferences.TargetServerVersionInternal);
			index.ThrowIfCompatibilityLevelBelow100();
			if (parent is View)
			{
				throw new SmoException(ExceptionTemplatesImpl.NotSpatialIndexOnView);
			}
			if (index.IndexedColumns.Count != 1)
			{
				throw new SmoException(ExceptionTemplatesImpl.OneColumnInSpatialIndex);
			}
			CheckConstraintProperties();
			CheckRegularIndexProperties();
			CheckClusteredProperties();
			CheckNonClusteredProperties();
			CheckXmlProperties();
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE SPATIAL INDEX {0} ON {1}", new object[2]
			{
				index.FormatFullNameForScripting(preferences),
				parent.FormatFullNameForScripting(preferences)
			});
		}

		protected override void ScriptIndexDetails(StringBuilder sb)
		{
			spatialIndexType = index.Properties.Get("SpatialIndexType");
			xMin = index.Properties.Get("BoundingBoxXMin");
			yMin = index.Properties.Get("BoundingBoxYMin");
			xMax = index.Properties.Get("BoundingBoxXMax");
			yMax = index.Properties.Get("BoundingBoxYMax");
			level1 = index.Properties.Get("Level1Grid");
			level2 = index.Properties.Get("Level2Grid");
			level3 = index.Properties.Get("Level3Grid");
			level4 = index.Properties.Get("Level4Grid");
			cellsPerObject = index.Properties.Get("CellsPerObject");
			if (spatialIndexType.Value != null && (spatialIndexType.Dirty || !preferences.ScriptForAlter) && (SpatialIndexType)spatialIndexType.Value != SpatialIndexType.None)
			{
				string text = string.Empty;
				switch ((SpatialIndexType)spatialIndexType.Value)
				{
				case SpatialIndexType.GeometryGrid:
					text = " GEOMETRY_GRID ";
					break;
				case SpatialIndexType.GeographyGrid:
					text = " GEOGRAPHY_GRID ";
					break;
				case SpatialIndexType.GeometryAutoGrid:
					SqlSmoObject.ThrowIfBelowVersion110(preferences.TargetServerVersionInternal, ExceptionTemplatesImpl.SpatialAutoGridDownlevel(index.FormatFullNameForScripting(preferences, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(preferences)));
					text = " GEOMETRY_AUTO_GRID ";
					break;
				case SpatialIndexType.GeographyAutoGrid:
					SqlSmoObject.ThrowIfBelowVersion110(preferences.TargetServerVersionInternal, ExceptionTemplatesImpl.SpatialAutoGridDownlevel(index.FormatFullNameForScripting(preferences, nameIsIndentifier: true), SqlSmoObject.GetSqlServerName(preferences)));
					text = " GEOGRAPHY_AUTO_GRID ";
					break;
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "USING {0}", new object[1] { text });
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.newline);
			}
		}

		private void ScriptSpatialIndexOptions(StringBuilder sb)
		{
			SpatialIndexType spatialIndexType = SpatialIndexType.GeometryGrid;
			if (this.spatialIndexType.Value != null)
			{
				spatialIndexType = (SpatialIndexType)this.spatialIndexType.Value;
			}
			if (spatialIndexType == SpatialIndexType.GeometryGrid || spatialIndexType == SpatialIndexType.GeometryAutoGrid)
			{
				if (xMin.IsNull || yMin.IsNull || xMax.IsNull || yMax.IsNull)
				{
					throw new SmoException(ExceptionTemplatesImpl.MissingBoundingParameters);
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, "BOUNDING_BOX =");
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
				sb.AppendFormat(SmoApplication.DefaultCulture, ((double)xMin.Value).ToString(SmoApplication.DefaultCulture));
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				sb.AppendFormat(SmoApplication.DefaultCulture, ((double)yMin.Value).ToString(SmoApplication.DefaultCulture));
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				sb.AppendFormat(SmoApplication.DefaultCulture, ((double)xMax.Value).ToString(SmoApplication.DefaultCulture));
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				sb.AppendFormat(SmoApplication.DefaultCulture, ((double)yMax.Value).ToString(SmoApplication.DefaultCulture));
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
			else if (!xMin.IsNull && !yMin.IsNull && !xMax.IsNull && !yMax.IsNull && (double)xMin.Value != 0.0 && (double)xMax.Value != 0.0 && (double)yMin.Value != 0.0 && (double)yMax.Value != 0.0)
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidNonGeometryParameters);
			}
			if (spatialIndexType != SpatialIndexType.GeometryAutoGrid && spatialIndexType != SpatialIndexType.GeographyAutoGrid)
			{
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				ScriptGridClause(stringBuilder);
				if (stringBuilder.Length > 0)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "GRIDS =");
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
					sb.AppendFormat(SmoApplication.DefaultCulture, stringBuilder.ToString());
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				}
			}
			else if ((!level1.IsNull && (SpatialGeoLevelSize)level1.Value != SpatialGeoLevelSize.None) || (!level2.IsNull && (SpatialGeoLevelSize)level2.Value != SpatialGeoLevelSize.None) || (!level3.IsNull && (SpatialGeoLevelSize)level3.Value != SpatialGeoLevelSize.None) || (!level4.IsNull && (SpatialGeoLevelSize)level4.Value != SpatialGeoLevelSize.None))
			{
				throw new SmoException(ExceptionTemplatesImpl.NoAutoGridWithGrids(index.FormatFullNameForScripting(preferences, nameIsIndentifier: true)));
			}
			if (cellsPerObject.Value != null && (cellsPerObject.Dirty || !preferences.ScriptForAlter) && (preferences.ForDirectExecution || (int)cellsPerObject.Value != 0))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.newline);
				sb.AppendFormat(SmoApplication.DefaultCulture, "CELLS_PER_OBJECT = {0}", new object[1] { cellsPerObject.Value.ToString() });
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
			}
		}

		private void ScriptGridClause(StringBuilder gridsClause)
		{
			ScriptLevel(gridsClause, level1, "LEVEL_1");
			ScriptLevel(gridsClause, level2, "LEVEL_2");
			ScriptLevel(gridsClause, level3, "LEVEL_3");
			ScriptLevel(gridsClause, level4, "LEVEL_4");
			if (gridsClause.ToString().EndsWith(Globals.comma, StringComparison.Ordinal))
			{
				gridsClause.Remove(gridsClause.Length - 1, 1);
			}
		}

		private void ScriptLevel(StringBuilder gridsClause, Property level, string levelString)
		{
			if (level.Value != null && (level.Dirty || !preferences.ScriptForAlter) && (SpatialGeoLevelSize)level.Value != SpatialGeoLevelSize.None)
			{
				gridsClause.AppendFormat(SmoApplication.DefaultCulture, "{0} = {1}", new object[2]
				{
					levelString,
					level.Value.ToString().ToUpperInvariant()
				});
				gridsClause.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
			}
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
			ScriptSpatialIndexOptions(sb);
			base.ScriptIndexOptions(sb);
			ScriptCompression(sb);
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
			base.ScriptIndexRebuildOptions(withClause, rebuildPartitionNumber);
			if (rebuildPartitionNumber != -1 && index.PhysicalPartitions.IsDataCompressionStateDirty(rebuildPartitionNumber))
			{
				withClause.Append(index.PhysicalPartitions.GetCompressionCode(rebuildPartitionNumber));
				withClause.Append(Globals.commaspace);
			}
			else
			{
				ScriptCompression(withClause);
			}
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
			if (index.IsSupportedProperty("FileGroup", preferences) && !string.IsNullOrEmpty(index.GetPropValueOptional("FileGroup", string.Empty)))
			{
				base.ScriptIndexStorage(sb);
			}
		}
	}

	private abstract class ColumnstoreIndexScripter : IndexScripter
	{
		public ColumnstoreIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected virtual void ScriptIndexOptions(StringBuilder sb, bool forRebuild, int rebuildPartitionNumber)
		{
			if (!forRebuild)
			{
				ScriptIndexOption(sb, "DROP_EXISTING", GetOnOffValue(index.dropExistingIndex));
				ScriptCompressionDelay(sb);
			}
			if (index.MaximumDegreeOfParallelism > 0)
			{
				ScriptIndexOption(sb, "MAXDOP", index.MaximumDegreeOfParallelism);
			}
			if (forRebuild && rebuildPartitionNumber != -1 && index.PhysicalPartitions.IsDataCompressionStateDirty(rebuildPartitionNumber))
			{
				sb.Append(index.PhysicalPartitions.GetCompressionCode(rebuildPartitionNumber));
				sb.Append(Globals.commaspace);
			}
			else
			{
				ScriptCompression(sb);
			}
		}

		protected override void ScriptSetIndexOptions(StringBuilder setClause)
		{
			if (!preferences.TargetEngineIsAzureStretchDb())
			{
				ScriptCompressionDelay(setClause);
			}
		}

		protected void ScriptCompressionDelay(StringBuilder sb)
		{
			if (index.IsSupportedProperty("CompressionDelay", preferences))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "COMPRESSION_DELAY = {0}", new object[1] { index.CompressionDelay });
				sb.Append(Globals.commaspace);
			}
		}

		protected void CheckInvalidOptions()
		{
			if (!index.IsMemoryOptimizedIndex)
			{
				Exception exception = new SmoException(ExceptionTemplatesImpl.InvaildColumnStoreIndexOption);
				CheckProperty("PadIndex", defaultValue: false, exception);
				CheckProperty("NoAutomaticRecomputation", defaultValue: false, exception);
				CheckProperty(index.sortInTempdb, defaultValue: false, exception);
				CheckProperty(index.onlineIndexOperation, defaultValue: false, exception);
				CheckProperty("DisallowRowLocks", defaultValue: true, exception);
				CheckProperty("DisallowPageLocks", defaultValue: true, exception);
				CheckProperty("FillFactor", (byte)0, exception);
			}
		}
	}

	private class NonClusteredColumnStoreIndexScripter : ColumnstoreIndexScripter
	{
		protected override bool IsIncludedColumnSupported => true;

		public NonClusteredColumnStoreIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion110(preferences.TargetServerVersionInternal);
			CheckConstraintProperties();
			CheckRegularIndexProperties();
			CheckClusteredProperties();
			CheckXmlProperties();
			CheckSpatialProperties();
			CheckInvalidOptions();
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE NONCLUSTERED COLUMNSTORE INDEX {0} ON {1}", new object[2]
			{
				index.FormatFullNameForScripting(preferences),
				parent.FormatFullNameForScripting(preferences)
			});
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
			ScriptIndexOptions(sb, forRebuild: false, -1);
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
			ScriptIndexOptions(withClause, forRebuild: true, rebuildPartitionNumber);
		}

		protected override void ScriptIndexDetails(StringBuilder sb)
		{
			ScriptFilter(sb);
		}
	}

	private class HashIndexScripter : IndexScripter
	{
		protected override bool IsIncludedColumnSupported => false;

		public HashIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void CheckRegularIndexProperties()
		{
			CheckProperty("IsClustered", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexClustered));
			CheckProperty("IgnoreDuplicateKeys", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexIgnoreDupKey));
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion120(preferences.TargetServerVersionInternal);
			CheckRequiredProperties();
			CheckConflictingProperties();
			if (index.GetIndexKeyType() == IndexKeyType.None)
			{
				CheckRegularIndexProperties();
				CheckNonClusteredProperties();
				CheckXmlProperties();
				CheckSpatialProperties();
			}
		}

		private void CheckRequiredProperties()
		{
			if (!index.IsSupportedProperty("BucketCount") || !index.GetPropValueOptional<int>("BucketCount").HasValue)
			{
				throw new SmoException(ExceptionTemplatesImpl.BucketCountForHashIndex);
			}
		}

		protected override void ScriptAlterDetails(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "REBUILD WITH (BUCKET_COUNT = {0})", new object[1] { index.GetPropValueOptional<int>("BucketCount").Value.ToString() });
		}

		protected override void ScriptIndexHeader(StringBuilder sb)
		{
			ScriptCreateHeaderDdl(sb);
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			if (!base.TableCreate)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ADD ", new object[1] { parent.FormatFullNameForScripting(preferences) });
			}
			IndexKeyType indexKeyType = index.GetIndexKeyType();
			if (IndexKeyType.DriPrimaryKey == indexKeyType || IndexKeyType.DriUniqueKey == indexKeyType)
			{
				if (parent is Table)
				{
					index.AddConstraintName(sb, preferences);
				}
				sb.Append(Globals.space);
				sb.Append((IndexKeyType.DriPrimaryKey == indexKeyType) ? Scripts.PRIMARY_KEY : Scripts.UNIQUE);
				sb.Append(Globals.space);
			}
			else
			{
				sb.AppendFormat(Scripts.INDEX_NAME, SqlSmoObject.SqlBraket(index.Name));
				sb.Append(Globals.space);
				if (index.GetPropValueOptional("IsUnique", defaultValue: false))
				{
					sb.Append(Scripts.UNIQUE);
					sb.Append(Globals.space);
				}
			}
			sb.Append(Scripts.HASH);
			sb.Append(Globals.space);
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
			sb.Append(Globals.space);
			sb.AppendFormat(Scripts.WITH_BUCKET_COUNT, index.BucketCount);
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
		}

		protected override void ScriptSetIndexOptions(StringBuilder setClause)
		{
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
		}
	}

	private class RangeIndexScripter : IndexScripter
	{
		protected override bool IsIncludedColumnSupported => false;

		public RangeIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void CheckRegularIndexProperties()
		{
			CheckProperty("IsClustered", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexClustered));
			CheckProperty("IgnoreDuplicateKeys", defaultValue: false, new SmoException(ExceptionTemplatesImpl.NoIndexIgnoreDupKey));
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion120(preferences.TargetServerVersionInternal);
			CheckConflictingProperties();
			if (index.GetIndexKeyType() == IndexKeyType.None)
			{
				CheckRegularIndexProperties();
				CheckNonClusteredProperties();
				CheckXmlProperties();
				CheckSpatialProperties();
			}
		}

		protected override void ScriptIndexHeader(StringBuilder sb)
		{
			ScriptCreateHeaderDdl(sb);
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			if (!base.TableCreate)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ADD ", new object[1] { parent.FormatFullNameForScripting(preferences) });
			}
			IndexKeyType indexKeyType = index.GetIndexKeyType();
			if (IndexKeyType.DriPrimaryKey == indexKeyType || IndexKeyType.DriUniqueKey == indexKeyType)
			{
				if (parent is Table)
				{
					index.AddConstraintName(sb, preferences);
				}
				sb.Append(Globals.space);
				sb.Append((IndexKeyType.DriPrimaryKey == indexKeyType) ? Scripts.PRIMARY_KEY : Scripts.UNIQUE);
				sb.Append(Globals.space);
			}
			else
			{
				sb.AppendFormat(Scripts.INDEX_NAME, SqlSmoObject.SqlBraket(index.Name));
				sb.Append(Globals.space);
				if (index.GetPropValueOptional("IsUnique", defaultValue: false))
				{
					sb.Append(Scripts.UNIQUE);
					sb.Append(Globals.space);
				}
			}
			sb.Append(Scripts.NONCLUSTERED);
			sb.Append(Globals.space);
		}

		protected override bool ScriptColumn(IndexedColumn col, StringBuilder sb)
		{
			base.ScriptColumn(col, sb);
			ScriptColumnOrder(col, sb);
			return true;
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
		}

		protected override void ScriptSetIndexOptions(StringBuilder setClause)
		{
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
		}
	}

	private class UserDefinedTableTypeIndexScripter : IndexScripter
	{
		public UserDefinedTableTypeIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion130(preferences.TargetServerVersionInternal);
			CheckConflictingProperties();
		}

		protected override void ScriptIndexHeader(StringBuilder sb)
		{
			ScriptCreateHeaderDdl(sb);
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(Scripts.INDEX_NAME, SqlSmoObject.SqlBraket(index.Name));
			sb.Append(Globals.space);
			sb.Append(index.IsClustered ? Scripts.CLUSTERED : Scripts.NONCLUSTERED);
			sb.Append(Globals.space);
		}

		protected override bool ScriptColumn(IndexedColumn col, StringBuilder sb)
		{
			base.ScriptColumn(col, sb);
			ScriptColumnOrder(col, sb);
			return true;
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
		}

		protected override void ScriptSetIndexOptions(StringBuilder setClause)
		{
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
		}
	}

	private class SelectiveXMLIndexScripter : IndexScripter
	{
		public SelectiveXMLIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
			index.xmlOrSpatialIndex = true;
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion110(preferences.TargetServerVersionInternal);
			if (!string.IsNullOrEmpty(index.GetPropValueOptional("ParentXmlIndex", string.Empty)))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingIndexProperties, "ParentXmlIndex", string.Empty, "IndexType", index.GetPropValueOptional<IndexType>("IndexType").ToString()));
			}
			CheckConstraintProperties();
			CheckRegularIndexProperties();
			CheckClusteredProperties();
			CheckNonClusteredProperties();
			CheckSpatialProperties();
			CheckInvalidOptions();
			bool flag = false;
			for (int i = 0; i < index.IndexedXmlPathNamespaces.Count; i++)
			{
				if (index.IndexedXmlPathNamespaces[i].GetPropValueOptional("IsDefaultUri", defaultValue: false))
				{
					if (flag)
					{
						throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.MoreThenOneXmlDefaultNamespace, new object[1] { index.Name }));
					}
					flag = true;
				}
			}
		}

		private void CheckInvalidOptions()
		{
			Exception exception = new SmoException(ExceptionTemplatesImpl.InvaildSXIOption);
			CheckProperty(index.OnlineIndexOperation, defaultValue: false, exception);
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE SELECTIVE XML INDEX {0} ON {1}", new object[2]
			{
				index.FormatFullNameForScripting(preferences),
				parent.FormatFullNameForScripting(preferences)
			});
		}

		protected override void ScriptAlterDetails(StringBuilder sb)
		{
			if (index.IndexedXmlPathNamespaces.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				stringBuilder.Append(preferences.NewLine);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WITH XMLNAMESPACES");
				stringBuilder.Append(preferences.NewLine);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
				stringBuilder.Append(preferences.NewLine);
				for (int i = 0; i < index.IndexedXmlPathNamespaces.Count; i++)
				{
					IndexedXmlPathNamespace indexedXmlPathNamespace = index.IndexedXmlPathNamespaces[i];
					if (indexedXmlPathNamespace.State == SqlSmoState.Creating)
					{
						if (flag)
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
							stringBuilder.Append(preferences.NewLine);
						}
						flag = true;
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "'{0}' as {1}", new object[2] { indexedXmlPathNamespace.Uri, indexedXmlPathNamespace.Name });
					}
				}
				if (flag)
				{
					stringBuilder.Append(preferences.NewLine);
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
					stringBuilder.Append(preferences.NewLine);
					sb.AppendFormat(SmoApplication.DefaultCulture, stringBuilder.ToString());
				}
			}
			if (index.IndexedXmlPaths.Count <= 0)
			{
				return;
			}
			bool flag2 = false;
			sb.AppendFormat(SmoApplication.DefaultCulture, "FOR (");
			sb.Append(preferences.NewLine);
			for (int j = 0; j < index.IndexedXmlPaths.Count; j++)
			{
				IndexedXmlPath indexedXmlPath = index.IndexedXmlPaths[j];
				if (indexedXmlPath.State == SqlSmoState.Creating || indexedXmlPath.State == SqlSmoState.ToBeDropped)
				{
					if (flag2)
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
						sb.Append(preferences.NewLine);
					}
					flag2 = true;
					if (indexedXmlPath.State == SqlSmoState.Creating)
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, "ADD ");
						ScriptSelectiveIndexPath(indexedXmlPath, sb);
					}
					else
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, "REMOVE {0}", new object[1] { SqlSmoObject.MakeSqlBraket(indexedXmlPath.Name) });
					}
				}
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, ")");
		}

		protected override void ScriptIndexDetails(StringBuilder sb)
		{
			int count = index.IndexedXmlPathNamespaces.Count;
			if (count > 0)
			{
				sb.Append(preferences.NewLine);
				sb.AppendFormat(SmoApplication.DefaultCulture, "WITH XMLNAMESPACES");
				sb.Append(preferences.NewLine);
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
				sb.Append(preferences.NewLine);
				for (int i = 0; i < count; i++)
				{
					IndexedXmlPathNamespace indexedXmlPathNamespace = index.IndexedXmlPathNamespaces[i];
					if (indexedXmlPathNamespace.GetPropValueOptional("IsDefaultUri", defaultValue: false))
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, "DEFAULT '{0}'", new object[1] { indexedXmlPathNamespace.Uri });
					}
					else
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, "'{0}' as {1}", new object[2] { indexedXmlPathNamespace.Uri, indexedXmlPathNamespace.Name });
					}
					if (i != count - 1)
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, Globals.comma);
					}
					sb.Append(preferences.NewLine);
				}
				sb.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
				sb.Append(preferences.NewLine);
			}
			sb.Append(preferences.NewLine);
			sb.AppendFormat(SmoApplication.DefaultCulture, "FOR");
			sb.Append(preferences.NewLine);
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
			sb.Append(preferences.NewLine);
			for (int j = 0; j < index.IndexedXmlPaths.Count; j++)
			{
				IndexedXmlPath path = index.IndexedXmlPaths[j];
				ScriptSelectiveIndexPath(path, sb);
				if (j != index.IndexedXmlPaths.Count - 1)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, Globals.commaspace);
				}
				sb.Append(preferences.NewLine);
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
			sb.Append(preferences.NewLine);
		}

		protected void ScriptSelectiveIndexPath(IndexedXmlPath path, StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0} = '{1}'", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(path.Name),
				path.Path
			});
			if (!path.GetPropValueOptional<IndexedXmlPathType>("PathType").HasValue)
			{
				path.PathType = IndexedXmlPathType.XQuery;
			}
			if (path.PathType == IndexedXmlPathType.XQuery)
			{
				if (path.GetPropValueOptional<bool>("IsNode").HasValue && path.IsNode)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, " as XQUERY 'node()'");
				}
				else if (!string.IsNullOrEmpty(path.GetPropValueOptional("XQueryTypeDescription", string.Empty)))
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, " as XQUERY '{0}'", new object[1] { path.XQueryTypeDescription });
					if (path.GetPropValueOptional<int>("XQueryMaxLength").HasValue && path.XQueryMaxLength > 0)
					{
						sb.AppendFormat(SmoApplication.DefaultCulture, " MAXLENGTH ({0})", new object[1] { path.XQueryMaxLength });
					}
				}
				if (path.GetPropValueOptional<bool>("IsSingleton").HasValue && path.IsSingleton)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, " SINGLETON");
				}
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, " as SQL ");
				UserDefinedDataType.AppendScriptTypeDefinition(sb, preferences, path, path.DataType.SqlDataType);
				if (path.GetPropValueOptional<bool>("IsSingleton").HasValue && path.IsSingleton)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, " SINGLETON ");
				}
			}
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
		}
	}

	private class SecondarySelectiveXMLIndexScripter : IndexScripter
	{
		public string ParentXmlIndex => (string)index.Properties["ParentXmlIndex"].Value;

		public SecondarySelectiveXMLIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
			index.xmlOrSpatialIndex = true;
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion110(preferences.TargetServerVersionInternal);
			if (string.IsNullOrEmpty(index.GetPropValueOptional("ParentXmlIndex", string.Empty)))
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.ConflictingIndexProperties, "ParentXmlIndex", string.Empty, "IndexType", index.GetPropValueOptional<IndexType>("IndexType").ToString()));
			}
			CheckConstraintProperties();
			CheckRegularIndexProperties();
			CheckClusteredProperties();
			CheckNonClusteredProperties();
			CheckSpatialProperties();
			CheckInvalidOptions();
		}

		private void CheckInvalidOptions()
		{
			Exception exception = new SmoException(ExceptionTemplatesImpl.InvaildSXIOption);
			CheckProperty(index.OnlineIndexOperation, defaultValue: false, exception);
		}

		protected override void ScriptIndexDetails(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "USING XML INDEX {0} ", new object[1] { SqlSmoObject.MakeSqlBraket(ParentXmlIndex) });
			sb.AppendFormat(SmoApplication.DefaultCulture, "FOR (");
			sb.Append(preferences.NewLine);
			sb.AppendFormat(SmoApplication.DefaultCulture, SqlSmoObject.MakeSqlBraket(index.IndexedXmlPathName));
			sb.Append(preferences.NewLine);
			sb.AppendFormat(SmoApplication.DefaultCulture, ") ");
			sb.Append(preferences.NewLine);
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE XML INDEX {0} ON {1}", new object[2]
			{
				index.FormatFullNameForScripting(preferences),
				parent.FormatFullNameForScripting(preferences)
			});
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
		}
	}

	private class ClusteredColumnstoreIndexScripter : ColumnstoreIndexScripter
	{
		protected override bool IsIncludedColumnSupported => true;

		public ClusteredColumnstoreIndexScripter(Index index, ScriptingPreferences sp)
			: base(index, sp)
		{
		}

		protected override void Validate()
		{
			SqlSmoObject.ThrowIfBelowVersion120(preferences.TargetServerVersionInternal);
			CheckConstraintProperties();
			CheckClusteredProperties();
			CheckNonClusteredProperties();
			CheckXmlProperties();
			CheckSpatialProperties();
			CheckInvalidOptions();
		}

		protected override void ScriptCreateHeaderDdl(StringBuilder sb)
		{
			if (index.IsMemoryOptimizedIndex)
			{
				if (base.TableCreate)
				{
					sb.AppendFormat(SmoApplication.DefaultCulture, "INDEX {0} CLUSTERED COLUMNSTORE ", new object[1] { index.FormatFullNameForScripting(preferences) });
				}
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "CREATE CLUSTERED COLUMNSTORE INDEX {0} ON {1} ", new object[2]
				{
					index.FormatFullNameForScripting(preferences),
					parent.FormatFullNameForScripting(preferences)
				});
			}
		}

		protected override void ScriptIndexOptions(StringBuilder sb)
		{
			ScriptIndexOptions(sb, forRebuild: false, -1);
		}

		protected override void ScriptIndexRebuildOptions(StringBuilder withClause, int rebuildPartitionNumber)
		{
			ScriptIndexOptions(withClause, forRebuild: true, rebuildPartitionNumber);
		}

		protected override void ScriptIndexOptions(StringBuilder sb, bool forRebuild, int rebuildPartitionNumber)
		{
			if (index.IsMemoryOptimizedIndex)
			{
				if (base.TableCreate)
				{
					ScriptCompressionDelay(sb);
				}
			}
			else
			{
				base.ScriptIndexOptions(sb, forRebuild, rebuildPartitionNumber);
			}
		}

		protected override void ScriptColumns(StringBuilder sb)
		{
		}

		protected override void ScriptIndexStorage(StringBuilder sb)
		{
			if (!index.IsMemoryOptimizedIndex)
			{
				base.ScriptIndexStorage(sb);
			}
		}
	}

	private const double m_boundingBoxDef = 0.0;

	private const int m_cellsPerObjectDef = 0;

	private const byte fillFactorDef = 0;

	private const int m_minCompressionDelay = 0;

	private const int m_maxCompressionDelay = 10080;

	private const int m_hkMinCompressionDelay = 60;

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	private IndexEvents events;

	private bool m_bIsOnComputed;

	private bool xmlOrSpatialIndex;

	private bool isOnColumnWithAnsiPadding;

	private IndexedColumnCollection m_IndexedColumns;

	private IndexedXmlPathCollection m_IndexedXmlPaths;

	private IndexedXmlPathNamespaceCollection m_IndexedXmlPathNamespaces;

	private PartitionSchemeParameterCollection m_PartitionSchemeParameters;

	private PhysicalPartitionCollection m_PhysicalPartitions;

	private bool affectAllIndexes;

	private int optimizePartitionNumber = -1;

	internal bool dropExistingIndex;

	private bool sortInTempdb;

	private bool onlineIndexOperation;

	private bool resumableIndexOperation;

	private int resumableMaxDuration;

	private int lowPriorityMaxDuration;

	private AbortAfterWait lowPriorityAbortAfterWait;

	private int maximumDegreeOfParallelism = -1;

	private bool compactLargeObjects = true;

	private bool compressAllRowGroups;

	internal object oldIndexKeyTypeValue;

	[SfcParent("View")]
	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	[SfcParent("Table")]
	[SfcParent("UserDefinedTableType")]
	[SfcParent("UserDefinedFunction")]
	public SqlSmoObject Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance;
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
	public double BoundingBoxXMax
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("BoundingBoxXMax");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BoundingBoxXMax", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double BoundingBoxXMin
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("BoundingBoxXMin");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BoundingBoxXMin", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double BoundingBoxYMax
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("BoundingBoxYMax");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BoundingBoxYMax", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double BoundingBoxYMin
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("BoundingBoxYMin");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BoundingBoxYMin", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int BucketCount
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("BucketCount");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BucketCount", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int CellsPerObject
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("CellsPerObject");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CellsPerObject", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool DisallowPageLocks
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DisallowPageLocks");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DisallowPageLocks", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool DisallowRowLocks
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DisallowRowLocks");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DisallowRowLocks", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FileGroup
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileGroup", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FileStreamFileGroup
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileStreamFileGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileStreamFileGroup", value);
		}
	}

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(PartitionScheme), "Server[@Name='{0}']/Database[@Name='{1}']/PartitionScheme[@Name='{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "FileStreamPartitionScheme" })]
	public string FileStreamPartitionScheme
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileStreamPartitionScheme");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileStreamPartitionScheme", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public byte FillFactor
	{
		get
		{
			return (byte)base.Properties.GetValueWithNullReplacement("FillFactor");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FillFactor", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public string FilterDefinition
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FilterDefinition");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FilterDefinition", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasCompressedPartitions => (bool)base.Properties.GetValueWithNullReplacement("HasCompressedPartitions");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasFilter => (bool)base.Properties.GetValueWithNullReplacement("HasFilter");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasSparseColumn => (bool)base.Properties.GetValueWithNullReplacement("HasSparseColumn");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool IgnoreDuplicateKeys
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IgnoreDuplicateKeys");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IgnoreDuplicateKeys", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string IndexedXmlPathName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("IndexedXmlPathName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IndexedXmlPathName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public IndexKeyType IndexKeyType
	{
		get
		{
			return (IndexKeyType)base.Properties.GetValueWithNullReplacement("IndexKeyType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IndexKeyType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public IndexType IndexType
	{
		get
		{
			return (IndexType)base.Properties.GetValueWithNullReplacement("IndexType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IndexType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool IsClustered
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsClustered");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsClustered", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDisabled => (bool)base.Properties.GetValueWithNullReplacement("IsDisabled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsFileTableDefined => (bool)base.Properties.GetValueWithNullReplacement("IsFileTableDefined");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFullTextKey
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsFullTextKey");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsFullTextKey", value);
		}
	}

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsPartitioned => (bool)base.Properties.GetValueWithNullReplacement("IsPartitioned");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSpatialIndex => (bool)base.Properties.GetValueWithNullReplacement("IsSpatialIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool IsUnique
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsUnique");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsUnique", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsXmlIndex => (bool)base.Properties.GetValueWithNullReplacement("IsXmlIndex");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SpatialGeoLevelSize Level1Grid
	{
		get
		{
			return (SpatialGeoLevelSize)base.Properties.GetValueWithNullReplacement("Level1Grid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Level1Grid", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SpatialGeoLevelSize Level2Grid
	{
		get
		{
			return (SpatialGeoLevelSize)base.Properties.GetValueWithNullReplacement("Level2Grid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Level2Grid", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SpatialGeoLevelSize Level3Grid
	{
		get
		{
			return (SpatialGeoLevelSize)base.Properties.GetValueWithNullReplacement("Level3Grid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Level3Grid", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SpatialGeoLevelSize Level4Grid
	{
		get
		{
			return (SpatialGeoLevelSize)base.Properties.GetValueWithNullReplacement("Level4Grid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Level4Grid", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool NoAutomaticRecomputation
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NoAutomaticRecomputation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NoAutomaticRecomputation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public bool PadIndex
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("PadIndex");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PadIndex", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ParentXmlIndex
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ParentXmlIndex");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ParentXmlIndex", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(PartitionScheme), "Server[@Name='{0}']/Database[@Name='{1}']/PartitionScheme[@Name='{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "PartitionScheme" })]
	[CLSCompliant(false)]
	public string PartitionScheme
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("PartitionScheme");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PartitionScheme", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ResumableOperationStateType ResumableOperationState => (ResumableOperationStateType)base.Properties.GetValueWithNullReplacement("ResumableOperationState");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SecondaryXmlIndexType SecondaryXmlIndexType
	{
		get
		{
			return (SecondaryXmlIndexType)base.Properties.GetValueWithNullReplacement("SecondaryXmlIndexType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SecondaryXmlIndexType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double SpaceUsed => (double)base.Properties.GetValueWithNullReplacement("SpaceUsed");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SpatialIndexType SpatialIndexType
	{
		get
		{
			return (SpatialIndexType)base.Properties.GetValueWithNullReplacement("SpatialIndexType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SpatialIndexType", value);
		}
	}

	public IndexEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new IndexEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Index";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	[SfcKey(0)]
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(IndexedColumn), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design | SfcObjectFlags.Deploy)]
	public IndexedColumnCollection IndexedColumns
	{
		get
		{
			CheckObjectState();
			if (m_IndexedColumns == null)
			{
				m_IndexedColumns = new IndexedColumnCollection(this);
				if (base.State == SqlSmoState.Existing)
				{
					m_IndexedColumns.LockCollection(ExceptionTemplatesImpl.ReasonObjectAlreadyCreated(UrnSuffix));
				}
			}
			return m_IndexedColumns;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(IndexedXmlPath), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design | SfcObjectFlags.Deploy)]
	public IndexedXmlPathCollection IndexedXmlPaths
	{
		get
		{
			Version version = new Version(11, 0, 2813);
			Version version2 = new Version(base.ServerVersion.Major, base.ServerVersion.Minor, base.ServerVersion.BuildNumber);
			if (version2 < version)
			{
				throw new UnknownPropertyException(ExceptionTemplatesImpl.PropertySupportedOnlyOn110SP1("IndexedXmlPaths"));
			}
			CheckObjectState();
			if (m_IndexedXmlPaths == null)
			{
				m_IndexedXmlPaths = new IndexedXmlPathCollection(this);
			}
			return m_IndexedXmlPaths;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(IndexedXmlPathNamespace), SfcObjectFlags.NaturalOrder | SfcObjectFlags.Design | SfcObjectFlags.Deploy)]
	public IndexedXmlPathNamespaceCollection IndexedXmlPathNamespaces
	{
		get
		{
			Version version = new Version(11, 0, 2813);
			Version version2 = new Version(base.ServerVersion.Major, base.ServerVersion.Minor, base.ServerVersion.BuildNumber);
			if (version2 < version)
			{
				throw new UnknownPropertyException(ExceptionTemplatesImpl.PropertySupportedOnlyOn110SP1("IndexedXmlPathNamespaces"));
			}
			CheckObjectState();
			if (m_IndexedXmlPathNamespaces == null)
			{
				m_IndexedXmlPathNamespaces = new IndexedXmlPathNamespaceCollection(this);
			}
			return m_IndexedXmlPathNamespaces;
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(PartitionSchemeParameter))]
	[SfcInvalidForType(typeof(UserDefinedFunction))]
	public PartitionSchemeParameterCollection PartitionSchemeParameters
	{
		get
		{
			if (base.ParentColl.ParentInstance is UserDefinedTableType)
			{
				return null;
			}
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PartitionSchemeParameter));
			if (m_PartitionSchemeParameters == null)
			{
				m_PartitionSchemeParameters = new PartitionSchemeParameterCollection(this);
			}
			return m_PartitionSchemeParameters;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(PhysicalPartition))]
	public PhysicalPartitionCollection PhysicalPartitions
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PhysicalPartition));
			if (Parent is UserDefinedTableType)
			{
				return null;
			}
			if (m_PhysicalPartitions == null)
			{
				m_PhysicalPartitions = new PhysicalPartitionCollection(this);
			}
			return m_PhysicalPartitions;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.Design | SfcPropertyFlags.Deploy)]
	public int CompressionDelay
	{
		get
		{
			CheckObjectState();
			ThrowIfPropertyNotSupported("CompressionDelay");
			if (InferredIndexType != IndexType.ClusteredColumnStoreIndex && InferredIndexType != IndexType.NonClusteredColumnStoreIndex)
			{
				throw new InvalidSmoOperationException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.PropertyValidOnlyForColumnStoreIndexes, new object[1] { "CompressionDelay" }));
			}
			return (int)base.Properties.GetValueWithNullReplacement("CompressionDelay", throwOnNullValue: false, useDefaultOnMissingValue: true);
		}
		set
		{
			CheckObjectState();
			ThrowIfPropertyNotSupported("CompressionDelay");
			if (InferredIndexType != IndexType.ClusteredColumnStoreIndex && InferredIndexType != IndexType.NonClusteredColumnStoreIndex)
			{
				throw new InvalidSmoOperationException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.PropertyValidOnlyForColumnStoreIndexes, new object[1] { "CompressionDelay" }));
			}
			if (value >= 0 && value <= 10080 && (!IsMemoryOptimizedIndex || value >= 60 || value == 0))
			{
				base.Properties.SetValueWithConsistencyCheck("CompressionDelay", value);
				return;
			}
			throw new InvalidSmoOperationException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.InvalidCompressionDelayValue, value, 0, 10080, 60, 0));
		}
	}

	internal IndexType InferredIndexType
	{
		get
		{
			if (!GetPropValueOptional<IndexType>("IndexType").HasValue && (base.State == SqlSmoState.Creating || base.IsDesignMode))
			{
				IndexKeyType propValueOptional = GetPropValueOptional("IndexKeyType", IndexKeyType.None);
				bool? flag = GetPropValueOptional<bool>("IsClustered");
				if (!flag.HasValue && propValueOptional == IndexKeyType.DriPrimaryKey)
				{
					flag = true;
				}
				if (flag.HasValue && flag.Value)
				{
					return IndexType.ClusteredIndex;
				}
				if (HasXmlColumn(throwIfNotSet: false))
				{
					if (!string.IsNullOrEmpty(GetPropValueOptional("ParentXmlIndex", string.Empty).ToString()))
					{
						return IndexType.SecondaryXmlIndex;
					}
					return IndexType.PrimaryXmlIndex;
				}
				if (HasSpatialColumn(throwIfNotSet: false))
				{
					return IndexType.SpatialIndex;
				}
				return IndexType.NonClusteredIndex;
			}
			return (IndexType)base.Properties["IndexType"].Value;
		}
	}

	public bool DropExistingIndex
	{
		get
		{
			return dropExistingIndex;
		}
		set
		{
			dropExistingIndex = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SortInTempdb
	{
		get
		{
			CheckObjectState();
			return sortInTempdb;
		}
		set
		{
			CheckObjectState();
			sortInTempdb = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool OnlineIndexOperation
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			return onlineIndexOperation;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			onlineIndexOperation = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ResumableIndexOperation
	{
		get
		{
			CheckObjectState();
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				ThrowIfBelowVersion140Prop("ResumableIndexOperation");
			}
			return resumableIndexOperation;
		}
		set
		{
			CheckObjectState();
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				ThrowIfBelowVersion140Prop("ResumableIndexOperation");
			}
			resumableIndexOperation = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ResumableMaxDuration
	{
		get
		{
			CheckObjectState();
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				ThrowIfBelowVersion140Prop("ResumableMaxDuration");
			}
			return resumableMaxDuration;
		}
		set
		{
			CheckObjectState();
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				ThrowIfBelowVersion140Prop("ResumableMaxDuration");
			}
			resumableMaxDuration = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int LowPriorityMaxDuration
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			return lowPriorityMaxDuration;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			lowPriorityMaxDuration = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public AbortAfterWait LowPriorityAbortAfterWait
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			return lowPriorityAbortAfterWait;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			lowPriorityAbortAfterWait = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumDegreeOfParallelism
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			return maximumDegreeOfParallelism;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			maximumDegreeOfParallelism = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool CompactLargeObjects
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			return compactLargeObjects;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion90();
			compactLargeObjects = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool CompressAllRowGroups
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion130();
			return compressAllRowGroups;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion130();
			if (IndexType != IndexType.ClusteredColumnStoreIndex && IndexType != IndexType.NonClusteredColumnStoreIndex)
			{
				throw new InvalidSmoOperationException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.PropertyValidOnlyForColumnStoreIndexes, new object[1] { "CompressAllRowGroups" }));
			}
			compressAllRowGroups = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsIndexOnTable
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance is Table;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsIndexOnComputed
	{
		get
		{
			CheckObjectState();
			if (!(base.ParentColl.ParentInstance is Table table))
			{
				return false;
			}
			foreach (IndexedColumn indexedColumn in IndexedColumns)
			{
				Column column = table.Columns[indexedColumn.Name];
				if (column != null && column.Computed)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsOnlineRebuildSupported
	{
		get
		{
			if (base.ServerVersion.Major < 9 || GetServerObject().Information.EngineEdition != Edition.EnterpriseOrDeveloper || !(Parent is TableViewBase))
			{
				return false;
			}
			IndexType inferredIndexType = InferredIndexType;
			TableViewBase tableViewBase = (TableViewBase)Parent;
			switch (inferredIndexType)
			{
			case IndexType.ClusteredIndex:
				foreach (Column column2 in tableViewBase.Columns)
				{
					if ((column2.DataType.SqlDataType == SqlDataType.UserDefinedDataType && IsLargeObject(DataType.SqlToEnum(column2.GetPropValueOptional("SystemType").ToString()))) || IsLargeObject(column2.DataType.SqlDataType))
					{
						return false;
					}
				}
				break;
			case IndexType.NonClusteredIndex:
				if (IndexKeyType != IndexKeyType.None)
				{
					break;
				}
				foreach (IndexedColumn indexedColumn in IndexedColumns)
				{
					if (indexedColumn.IsIncluded)
					{
						Column column = tableViewBase.Columns[indexedColumn.Name];
						if ((column.DataType.SqlDataType == SqlDataType.UserDefinedDataType && IsLargeObject(DataType.SqlToEnum(column.GetPropValueOptional("SystemType").ToString()))) || IsLargeObject(column.DataType.SqlDataType))
						{
							return false;
						}
					}
				}
				break;
			case IndexType.PrimaryXmlIndex:
			case IndexType.SecondaryXmlIndex:
			case IndexType.SpatialIndex:
			case IndexType.NonClusteredColumnStoreIndex:
			case IndexType.ClusteredColumnStoreIndex:
				return false;
			}
			return true;
		}
	}

	internal bool IsMemoryOptimizedIndex
	{
		get
		{
			bool result = false;
			if (Parent.IsSupportedProperty("IsMemoryOptimized") && Parent.GetPropValueOptional("IsMemoryOptimized", defaultValue: false) && (InferredIndexType == IndexType.NonClusteredHashIndex || InferredIndexType == IndexType.NonClusteredIndex || InferredIndexType == IndexType.ClusteredColumnStoreIndex))
			{
				result = true;
			}
			return result;
		}
	}

	internal bool IsSqlDwIndex
	{
		get
		{
			bool result = false;
			if (Parent.IsSupportedProperty("DwTableDistribution") && Parent.GetPropValueOptional<DwTableDistributionType>("DwTableDistribution").HasValue)
			{
				DwTableDistributionType? propValueOptional = Parent.GetPropValueOptional<DwTableDistributionType>("DwTableDistribution");
				if ((propValueOptional.GetValueOrDefault() != DwTableDistributionType.Undefined || !propValueOptional.HasValue) && Parent.GetPropValueOptional<DwTableDistributionType>("DwTableDistribution") != DwTableDistributionType.None)
				{
					result = true;
				}
			}
			return result;
		}
	}

	public Index()
	{
	}

	public Index(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
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
					0 => XRuntime.DisallowPageLocks, 
					1 => XRuntime.DisallowRowLocks, 
					2 => XSchema.FileGroup, 
					3 => XSchema.FillFactor, 
					4 => XRuntime.FilterDefinition, 
					5 => XRuntime.HasCompressedPartitions, 
					6 => XRuntime.HasFilter, 
					7 => XRuntime.HasSparseColumn, 
					8 => XRuntime.ID, 
					9 => XSchema.IgnoreDuplicateKeys, 
					10 => XSchema.IndexKeyType, 
					11 => XSchema.IndexType, 
					12 => XSchema.IsClustered, 
					13 => XRuntime.IsDisabled, 
					14 => XRuntime.IsFullTextKey, 
					15 => XRuntime.IsPartitioned, 
					16 => XSchema.IsSystemNamed, 
					17 => XRuntime.IsSystemObject, 
					18 => XSchema.IsUnique, 
					19 => XSchema.IsXmlIndex, 
					20 => XRuntime.NoAutomaticRecomputation, 
					21 => XSchema.PadIndex, 
					22 => XSchema.PartitionScheme, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			return index switch
			{
				0 => XSchema.BoundingBoxXMax, 
				1 => XSchema.BoundingBoxXMin, 
				2 => XSchema.BoundingBoxYMax, 
				3 => XSchema.BoundingBoxYMin, 
				30 => XRuntime.BucketCount, 
				4 => XSchema.CellsPerObject, 
				31 => XRuntime.CompressionDelay, 
				5 => XRuntime.DisallowPageLocks, 
				6 => XRuntime.DisallowRowLocks, 
				32 => XSchema.FileGroup, 
				33 => XSchema.FileStreamFileGroup, 
				34 => XSchema.FileStreamPartitionScheme, 
				7 => XSchema.FillFactor, 
				8 => XRuntime.FilterDefinition, 
				35 => XRuntime.HasCompressedPartitions, 
				9 => XRuntime.HasFilter, 
				10 => XRuntime.HasSparseColumn, 
				11 => XRuntime.ID, 
				12 => XSchema.IgnoreDuplicateKeys, 
				36 => XSchema.IndexedXmlPathName, 
				13 => XSchema.IndexKeyType, 
				14 => XSchema.IndexType, 
				15 => XSchema.IsClustered, 
				16 => XRuntime.IsDisabled, 
				17 => XRuntime.IsFullTextKey, 
				37 => XRuntime.IsMemoryOptimized, 
				38 => XRuntime.IsPartitioned, 
				18 => XSchema.IsSpatialIndex, 
				19 => XSchema.IsSystemNamed, 
				20 => XRuntime.IsSystemObject, 
				21 => XSchema.IsUnique, 
				22 => XSchema.IsXmlIndex, 
				23 => XSchema.Level1Grid, 
				24 => XSchema.Level2Grid, 
				25 => XSchema.Level3Grid, 
				26 => XSchema.Level4Grid, 
				27 => XRuntime.NoAutomaticRecomputation, 
				28 => XSchema.PadIndex, 
				39 => XRuntime.ParentXmlIndex, 
				40 => XSchema.PartitionScheme, 
				41 => XSchema.ResumableOperationState, 
				42 => XSchema.SecondaryXmlIndexType, 
				29 => XSchema.SpatialIndexType, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			23 => XSchema.BoundingBoxXMax, 
			24 => XSchema.BoundingBoxXMin, 
			25 => XSchema.BoundingBoxYMax, 
			26 => XSchema.BoundingBoxYMin, 
			42 => XRuntime.BucketCount, 
			27 => XSchema.CellsPerObject, 
			44 => XRuntime.CompressionDelay, 
			0 => XRuntime.DisallowPageLocks, 
			1 => XRuntime.DisallowRowLocks, 
			2 => XSchema.FileGroup, 
			28 => XSchema.FileStreamFileGroup, 
			29 => XSchema.FileStreamPartitionScheme, 
			3 => XSchema.FillFactor, 
			30 => XRuntime.FilterDefinition, 
			31 => XRuntime.HasCompressedPartitions, 
			32 => XRuntime.HasFilter, 
			4 => XRuntime.HasSparseColumn, 
			5 => XRuntime.ID, 
			6 => XSchema.IgnoreDuplicateKeys, 
			40 => XSchema.IndexedXmlPathName, 
			7 => XSchema.IndexKeyType, 
			8 => XSchema.IndexType, 
			9 => XSchema.IsClustered, 
			17 => XRuntime.IsDisabled, 
			41 => XRuntime.IsFileTableDefined, 
			10 => XRuntime.IsFullTextKey, 
			43 => XRuntime.IsMemoryOptimized, 
			18 => XRuntime.IsPartitioned, 
			33 => XSchema.IsSpatialIndex, 
			11 => XSchema.IsSystemNamed, 
			12 => XRuntime.IsSystemObject, 
			13 => XSchema.IsUnique, 
			19 => XSchema.IsXmlIndex, 
			34 => XSchema.Level1Grid, 
			35 => XSchema.Level2Grid, 
			36 => XSchema.Level3Grid, 
			37 => XSchema.Level4Grid, 
			14 => XRuntime.NoAutomaticRecomputation, 
			15 => XSchema.PadIndex, 
			20 => XRuntime.ParentXmlIndex, 
			21 => XSchema.PartitionScheme, 
			38 => XRuntime.PolicyHealthState, 
			45 => XSchema.ResumableOperationState, 
			22 => XSchema.SecondaryXmlIndexType, 
			16 => XRuntime.SpaceUsed, 
			39 => XSchema.SpatialIndexType, 
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
					XRuntime.DisallowPageLocks = (bool)value;
					break;
				case 1:
					XRuntime.DisallowRowLocks = (bool)value;
					break;
				case 2:
					XSchema.FileGroup = (string)value;
					break;
				case 3:
					XSchema.FillFactor = (byte)value;
					break;
				case 4:
					XRuntime.FilterDefinition = (string)value;
					break;
				case 5:
					XRuntime.HasCompressedPartitions = (bool)value;
					break;
				case 6:
					XRuntime.HasFilter = (bool)value;
					break;
				case 7:
					XRuntime.HasSparseColumn = (bool)value;
					break;
				case 8:
					XRuntime.ID = (int)value;
					break;
				case 9:
					XSchema.IgnoreDuplicateKeys = (bool)value;
					break;
				case 10:
					XSchema.IndexKeyType = (IndexKeyType)value;
					break;
				case 11:
					XSchema.IndexType = (IndexType)value;
					break;
				case 12:
					XSchema.IsClustered = (bool)value;
					break;
				case 13:
					XRuntime.IsDisabled = (bool)value;
					break;
				case 14:
					XRuntime.IsFullTextKey = (bool)value;
					break;
				case 15:
					XRuntime.IsPartitioned = (bool)value;
					break;
				case 16:
					XSchema.IsSystemNamed = (bool)value;
					break;
				case 17:
					XRuntime.IsSystemObject = (bool)value;
					break;
				case 18:
					XSchema.IsUnique = (bool)value;
					break;
				case 19:
					XSchema.IsXmlIndex = (bool)value;
					break;
				case 20:
					XRuntime.NoAutomaticRecomputation = (bool)value;
					break;
				case 21:
					XSchema.PadIndex = (bool)value;
					break;
				case 22:
					XSchema.PartitionScheme = (string)value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return;
			}
			switch (index)
			{
			case 0:
				XSchema.BoundingBoxXMax = (double)value;
				break;
			case 1:
				XSchema.BoundingBoxXMin = (double)value;
				break;
			case 2:
				XSchema.BoundingBoxYMax = (double)value;
				break;
			case 3:
				XSchema.BoundingBoxYMin = (double)value;
				break;
			case 30:
				XRuntime.BucketCount = (int)value;
				break;
			case 4:
				XSchema.CellsPerObject = (int)value;
				break;
			case 31:
				XRuntime.CompressionDelay = (int)value;
				break;
			case 5:
				XRuntime.DisallowPageLocks = (bool)value;
				break;
			case 6:
				XRuntime.DisallowRowLocks = (bool)value;
				break;
			case 32:
				XSchema.FileGroup = (string)value;
				break;
			case 33:
				XSchema.FileStreamFileGroup = (string)value;
				break;
			case 34:
				XSchema.FileStreamPartitionScheme = (string)value;
				break;
			case 7:
				XSchema.FillFactor = (byte)value;
				break;
			case 8:
				XRuntime.FilterDefinition = (string)value;
				break;
			case 35:
				XRuntime.HasCompressedPartitions = (bool)value;
				break;
			case 9:
				XRuntime.HasFilter = (bool)value;
				break;
			case 10:
				XRuntime.HasSparseColumn = (bool)value;
				break;
			case 11:
				XRuntime.ID = (int)value;
				break;
			case 12:
				XSchema.IgnoreDuplicateKeys = (bool)value;
				break;
			case 36:
				XSchema.IndexedXmlPathName = (string)value;
				break;
			case 13:
				XSchema.IndexKeyType = (IndexKeyType)value;
				break;
			case 14:
				XSchema.IndexType = (IndexType)value;
				break;
			case 15:
				XSchema.IsClustered = (bool)value;
				break;
			case 16:
				XRuntime.IsDisabled = (bool)value;
				break;
			case 17:
				XRuntime.IsFullTextKey = (bool)value;
				break;
			case 37:
				XRuntime.IsMemoryOptimized = (bool)value;
				break;
			case 38:
				XRuntime.IsPartitioned = (bool)value;
				break;
			case 18:
				XSchema.IsSpatialIndex = (bool)value;
				break;
			case 19:
				XSchema.IsSystemNamed = (bool)value;
				break;
			case 20:
				XRuntime.IsSystemObject = (bool)value;
				break;
			case 21:
				XSchema.IsUnique = (bool)value;
				break;
			case 22:
				XSchema.IsXmlIndex = (bool)value;
				break;
			case 23:
				XSchema.Level1Grid = (SpatialGeoLevelSize)value;
				break;
			case 24:
				XSchema.Level2Grid = (SpatialGeoLevelSize)value;
				break;
			case 25:
				XSchema.Level3Grid = (SpatialGeoLevelSize)value;
				break;
			case 26:
				XSchema.Level4Grid = (SpatialGeoLevelSize)value;
				break;
			case 27:
				XRuntime.NoAutomaticRecomputation = (bool)value;
				break;
			case 28:
				XSchema.PadIndex = (bool)value;
				break;
			case 39:
				XRuntime.ParentXmlIndex = (string)value;
				break;
			case 40:
				XSchema.PartitionScheme = (string)value;
				break;
			case 41:
				XSchema.ResumableOperationState = (ResumableOperationStateType)value;
				break;
			case 42:
				XSchema.SecondaryXmlIndexType = (SecondaryXmlIndexType)value;
				break;
			case 29:
				XSchema.SpatialIndexType = (SpatialIndexType)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 23:
				XSchema.BoundingBoxXMax = (double)value;
				break;
			case 24:
				XSchema.BoundingBoxXMin = (double)value;
				break;
			case 25:
				XSchema.BoundingBoxYMax = (double)value;
				break;
			case 26:
				XSchema.BoundingBoxYMin = (double)value;
				break;
			case 42:
				XRuntime.BucketCount = (int)value;
				break;
			case 27:
				XSchema.CellsPerObject = (int)value;
				break;
			case 44:
				XRuntime.CompressionDelay = (int)value;
				break;
			case 0:
				XRuntime.DisallowPageLocks = (bool)value;
				break;
			case 1:
				XRuntime.DisallowRowLocks = (bool)value;
				break;
			case 2:
				XSchema.FileGroup = (string)value;
				break;
			case 28:
				XSchema.FileStreamFileGroup = (string)value;
				break;
			case 29:
				XSchema.FileStreamPartitionScheme = (string)value;
				break;
			case 3:
				XSchema.FillFactor = (byte)value;
				break;
			case 30:
				XRuntime.FilterDefinition = (string)value;
				break;
			case 31:
				XRuntime.HasCompressedPartitions = (bool)value;
				break;
			case 32:
				XRuntime.HasFilter = (bool)value;
				break;
			case 4:
				XRuntime.HasSparseColumn = (bool)value;
				break;
			case 5:
				XRuntime.ID = (int)value;
				break;
			case 6:
				XSchema.IgnoreDuplicateKeys = (bool)value;
				break;
			case 40:
				XSchema.IndexedXmlPathName = (string)value;
				break;
			case 7:
				XSchema.IndexKeyType = (IndexKeyType)value;
				break;
			case 8:
				XSchema.IndexType = (IndexType)value;
				break;
			case 9:
				XSchema.IsClustered = (bool)value;
				break;
			case 17:
				XRuntime.IsDisabled = (bool)value;
				break;
			case 41:
				XRuntime.IsFileTableDefined = (bool)value;
				break;
			case 10:
				XRuntime.IsFullTextKey = (bool)value;
				break;
			case 43:
				XRuntime.IsMemoryOptimized = (bool)value;
				break;
			case 18:
				XRuntime.IsPartitioned = (bool)value;
				break;
			case 33:
				XSchema.IsSpatialIndex = (bool)value;
				break;
			case 11:
				XSchema.IsSystemNamed = (bool)value;
				break;
			case 12:
				XRuntime.IsSystemObject = (bool)value;
				break;
			case 13:
				XSchema.IsUnique = (bool)value;
				break;
			case 19:
				XSchema.IsXmlIndex = (bool)value;
				break;
			case 34:
				XSchema.Level1Grid = (SpatialGeoLevelSize)value;
				break;
			case 35:
				XSchema.Level2Grid = (SpatialGeoLevelSize)value;
				break;
			case 36:
				XSchema.Level3Grid = (SpatialGeoLevelSize)value;
				break;
			case 37:
				XSchema.Level4Grid = (SpatialGeoLevelSize)value;
				break;
			case 14:
				XRuntime.NoAutomaticRecomputation = (bool)value;
				break;
			case 15:
				XSchema.PadIndex = (bool)value;
				break;
			case 20:
				XRuntime.ParentXmlIndex = (string)value;
				break;
			case 21:
				XSchema.PartitionScheme = (string)value;
				break;
			case 38:
				XRuntime.PolicyHealthState = (PolicyHealthState)value;
				break;
			case 45:
				XSchema.ResumableOperationState = (ResumableOperationStateType)value;
				break;
			case 22:
				XSchema.SecondaryXmlIndexType = (SecondaryXmlIndexType)value;
				break;
			case 16:
				XRuntime.SpaceUsed = (double)value;
				break;
			case 39:
				XSchema.SpatialIndexType = (SpatialIndexType)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[26]
		{
			"BoundingBoxXMax", "BoundingBoxXMin", "BoundingBoxYMax", "BoundingBoxYMin", "BucketCount", "CellsPerObject", "FileGroup", "FileStreamFileGroup", "FileStreamPartitionScheme", "FillFactor",
			"FilterDefinition", "IndexedXmlPathName", "IndexKeyType", "IndexType", "IsClustered", "IsMemoryOptimized", "IsUnique", "Level1Grid", "Level2Grid", "Level3Grid",
			"Level4Grid", "PadIndex", "ParentXmlIndex", "PartitionScheme", "SecondaryXmlIndexType", "SpatialIndexType"
		};
	}

	internal Index(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_IndexedColumns = null;
		m_IndexedXmlPaths = null;
		m_IndexedXmlPathNamespaces = null;
		m_ExtendedProperties = null;
		m_PartitionSchemeParameters = null;
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		if (propname == "CompressionDelay")
		{
			return 0;
		}
		return base.GetPropertyDefaultValue(propname);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_IndexedColumns != null)
		{
			m_IndexedColumns.MarkAllDropped();
		}
		if (m_IndexedXmlPaths != null)
		{
			m_IndexedXmlPaths.MarkAllDropped();
		}
		if (m_IndexedXmlPathNamespaces != null)
		{
			m_IndexedXmlPathNamespaces.MarkAllDropped();
		}
		if (m_ExtendedProperties != null)
		{
			m_ExtendedProperties.MarkAllDropped();
		}
		if (m_PartitionSchemeParameters != null)
		{
			m_PartitionSchemeParameters.MarkAllDropped();
		}
	}

	internal bool IsDirty(string property)
	{
		return base.Properties.IsDirty(base.Properties.LookupID(property, PropertyAccessPurpose.Read));
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

	public void Create()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		CreateImpl();
		((TableViewBase)base.ParentColl.ParentInstance).Statistics.MarkOutOfSync();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		CheckIndexTypeAllowsModification(sp);
		ScriptDdl(queries, sp, notEmbedded: true);
	}

	private void CheckIndexTypeAllowsModification(ScriptingPreferences sp = null)
	{
		if (GetPropValueOptionalAllowNull("IndexType") != null && IsMemoryOptimizedIndex)
		{
			if (sp == null)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.ScriptMemoryOptimizedIndex);
			}
			if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.SupportedOnlyOn130);
			}
		}
	}

	protected override void PostCreate()
	{
		if (Parent is Table table)
		{
			bool propValueIfSupported = table.GetPropValueIfSupported("IsNode", defaultValue: false);
			bool propValueIfSupported2 = table.GetPropValueIfSupported("IsEdge", defaultValue: false);
			if (propValueIfSupported || propValueIfSupported2)
			{
				m_IndexedColumns.Refresh();
				List<IndexedColumn> list = new List<IndexedColumn>();
				foreach (IndexedColumn indexedColumn in m_IndexedColumns)
				{
					if (IsGraphPseudoColumn(indexedColumn.Name))
					{
						list.Add(indexedColumn);
					}
				}
				foreach (IndexedColumn item in list)
				{
					m_IndexedColumns.Remove(item);
				}
			}
		}
		m_IndexedColumns.LockCollection(ExceptionTemplatesImpl.ReasonObjectAlreadyCreated(UrnSuffix));
		if (!ExecutionManager.Recording && base.IsDesignMode && key.IsNull && GetIsSystemNamed())
		{
			AutoGenerateName();
		}
	}

	internal override void ScriptDdl(StringCollection queries, ScriptingPreferences sp)
	{
		if (sp.IncludeScripts.DatabaseContext)
		{
			AddDatabaseContext(queries, sp);
		}
		ScriptDdl(queries, sp, notEmbedded: false);
	}

	internal void ScriptDdl(StringCollection queries, ScriptingPreferences sp, bool notEmbedded)
	{
		ScriptDdl(queries, sp, notEmbedded, createStatement: false);
	}

	internal void ScriptDdl(StringCollection queries, ScriptingPreferences sp, bool notEmbedded, bool createStatement)
	{
		CheckObjectState();
		InitializeKeepDirtyValues();
		StringBuilder stringBuilder = new StringBuilder();
		StringCollection stringCollection = new StringCollection();
		string dDL = GetDDL(sp, notEmbedded, createStatement);
		if (dDL.Length > 0)
		{
			_ = (ScriptSchemaObjectBase)base.ParentColl.ParentInstance;
			if (!createStatement)
			{
				AddSetOptionsForIndex(stringCollection);
				if (stringCollection.Count > 0)
				{
					StringEnumerator enumerator = stringCollection.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							string current = enumerator.Current;
							stringBuilder.Append(current);
							stringBuilder.Append(sp.NewLine);
						}
					}
					finally
					{
						if (enumerator is IDisposable disposable)
						{
							disposable.Dispose();
						}
					}
					stringCollection.Clear();
					queries.Add(stringBuilder.ToString());
				}
			}
			queries.Add(dDL);
			if (sp.ScriptForCreateDrop && (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70 || sp.TargetServerVersionInternal == SqlServerVersionInternal.Version80))
			{
				ScriptSpIndexoptions(queries, sp);
			}
		}
		if (ScriptConstraintWithName(sp) && base.ServerVersion.Major > 8 && sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80 && !dropExistingIndex && properties.Get("IsDisabled").Value != null && (bool)properties.Get("IsDisabled").Value)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER INDEX [{0}] ON {1} DISABLE", new object[2]
			{
				SqlSmoObject.SqlBraket(Name),
				Parent.FullQualifiedName
			}));
		}
	}

	private IndexKeyType GetIndexKeyType()
	{
		bool flag = null != base.Properties.Get("IndexKeyType").Value;
		IndexKeyType result = IndexKeyType.None;
		if (flag)
		{
			result = (IndexKeyType)base.Properties["IndexKeyType"].Value;
		}
		return result;
	}

	private string GetDDL(ScriptingPreferences sp, bool creating)
	{
		return GetDDL(sp, creating, tableCreate: false);
	}

	private string GetDDL(ScriptingPreferences sp, bool creating, bool tableCreate)
	{
		IndexScripter indexScripterForCreate = IndexScripter.GetIndexScripterForCreate(this, sp);
		if (indexScripterForCreate != null)
		{
			if (indexScripterForCreate is ConstraintScripter || indexScripterForCreate is RangeIndexScripter || indexScripterForCreate is HashIndexScripter || indexScripterForCreate is ClusteredColumnstoreIndexScripter || indexScripterForCreate is ClusteredRegularIndexScripter || indexScripterForCreate is UserDefinedTableTypeIndexScripter)
			{
				indexScripterForCreate.TableCreate = tableCreate;
			}
			return indexScripterForCreate.GetCreateScript();
		}
		return string.Empty;
	}

	private bool IsSpatialColumn(Column colBase)
	{
		if (colBase.DataType.SqlDataType != SqlDataType.Geometry)
		{
			return colBase.DataType.SqlDataType == SqlDataType.Geography;
		}
		return true;
	}

	private bool IsCompressionCodeRequired(bool bAlter)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(base.ServerVersion.Major >= 10);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(base.State == SqlSmoState.Existing || base.State == SqlSmoState.Creating);
		if (base.State == SqlSmoState.Creating)
		{
			if (m_PhysicalPartitions != null)
			{
				return PhysicalPartitions.IsCompressionCodeRequired(bAlter);
			}
			return false;
		}
		if (HasCompressedPartitions)
		{
			return true;
		}
		if (m_PhysicalPartitions != null && PhysicalPartitions.IsCollectionDirty())
		{
			return PhysicalPartitions.IsCompressionCodeRequired(bAlter);
		}
		return false;
	}

	internal bool HasXmlColumn(bool throwIfNotSet)
	{
		if (base.ServerVersion.Major < 9 || DatabaseEngineType != DatabaseEngineType.Standalone)
		{
			return false;
		}
		IndexType? propValueOptional = GetPropValueOptional<IndexType>("IndexType");
		if (propValueOptional.HasValue)
		{
			if (propValueOptional.Value != IndexType.PrimaryXmlIndex)
			{
				return propValueOptional.Value == IndexType.SecondaryXmlIndex;
			}
			return true;
		}
		return CheckColumnsDataType(throwIfNotSet, IsColumnXmlDataType);
	}

	internal bool HasSpatialColumn(bool throwIfNotSet)
	{
		if (base.ServerVersion.Major < 10 || DatabaseEngineType != DatabaseEngineType.Standalone)
		{
			return false;
		}
		IndexType? propValueOptional = GetPropValueOptional<IndexType>("IndexType");
		if (propValueOptional.HasValue)
		{
			return propValueOptional.Value == IndexType.SpatialIndex;
		}
		return CheckColumnsDataType(throwIfNotSet, IsColumnSpatialDataType);
	}

	private bool CheckColumnsDataType(bool throwIfNotSet, CheckColumnDataType checkDataType)
	{
		ColumnCollection columnCollection = null;
		TableViewTableTypeBase tableViewTableTypeBase = base.ParentColl.ParentInstance as TableViewTableTypeBase;
		ScriptSchemaObjectBase scriptSchemaObjectBase = null;
		if (tableViewTableTypeBase != null)
		{
			scriptSchemaObjectBase = tableViewTableTypeBase;
			columnCollection = tableViewTableTypeBase.Columns;
			if (scriptSchemaObjectBase is View && scriptSchemaObjectBase.State == SqlSmoState.Creating)
			{
				return false;
			}
		}
		if (base.ParentColl.ParentInstance is UserDefinedFunction userDefinedFunction)
		{
			scriptSchemaObjectBase = userDefinedFunction;
			columnCollection = userDefinedFunction.Columns;
		}
		foreach (IndexedColumn indexedColumn in IndexedColumns)
		{
			object propValueOptional = indexedColumn.GetPropValueOptional("IsIncluded");
			if ((propValueOptional == null || !(bool)propValueOptional) && !IsGraphPseudoColumn(indexedColumn.Name))
			{
				Column column = columnCollection[indexedColumn.Name];
				if (column == null)
				{
					throw new SmoException(ExceptionTemplatesImpl.ObjectRefsNonexCol(UrnSuffix, Name, scriptSchemaObjectBase.FullQualifiedName + ".[" + SqlSmoObject.SqlStringBraket(indexedColumn.Name) + "]"));
				}
				string datatype = ((!throwIfNotSet) ? (column.GetPropValueOptional("DataType") as string) : (column.GetPropValue("DataType") as string));
				if (checkDataType(datatype))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool IsColumnXmlDataType(string dataType)
	{
		if (dataType != null)
		{
			return dataType.ToLower(SmoApplication.DefaultCulture) == "xml";
		}
		return false;
	}

	private static bool IsColumnSpatialDataType(string dataType)
	{
		if (dataType != null)
		{
			if (!(dataType.ToLower(SmoApplication.DefaultCulture) == "geometry"))
			{
				return dataType.ToLower(SmoApplication.DefaultCulture) == "geography";
			}
			return true;
		}
		return false;
	}

	private static bool IsGraphPseudoColumn(string name)
	{
		switch (name)
		{
		default:
			return name == "$to_id";
		case "$node_id":
		case "$edge_id":
		case "$from_id":
			return true;
		}
	}

	public void Drop()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		DropImpl();
		((TableViewBase)base.ParentColl.ParentInstance).Statistics.RemoveObject(new SimpleObjectKey(Name));
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		CheckIndexTypeAllowsModification(sp);
		CheckObjectState();
		IndexScripter indexScripterForDrop = IndexScripter.GetIndexScripterForDrop(this, sp);
		queries.Add(indexScripterForDrop.GetDropScript());
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Drop, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		MarkForDropImpl(dropOnAlter);
	}

	private void CheckUnsupportedSXI(bool checkPrimarySXI, bool checkSecondarySXI, string operation, string reason)
	{
		IndexType? propValueOptional = GetPropValueOptional<IndexType>("IndexType");
		if (propValueOptional.HasValue && ((checkPrimarySXI && propValueOptional.Value == IndexType.SelectiveXmlIndex) || (checkSecondarySXI && propValueOptional.Value == IndexType.SecondarySelectiveXmlIndex)))
		{
			throw new FailedOperationException(operation, this, null, reason);
		}
	}

	public void Alter()
	{
		CheckUnsupportedSXI(checkPrimarySXI: false, checkSecondarySXI: true, ExceptionTemplatesImpl.Alter, ExceptionTemplatesImpl.SecondarySelectiveXmlIndexModify);
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		AlterImpl();
	}

	public void Alter(IndexOperation operation)
	{
		CheckUnsupportedSXI(checkPrimarySXI: false, checkSecondarySXI: true, ExceptionTemplatesImpl.Alter, ExceptionTemplatesImpl.SecondarySelectiveXmlIndexModify);
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		switch (operation)
		{
		case IndexOperation.Rebuild:
			Rebuild();
			break;
		case IndexOperation.Reorganize:
			ThrowIfBelowVersion80();
			Reorganize();
			break;
		case IndexOperation.Disable:
			ThrowIfBelowVersion90();
			Disable();
			break;
		}
	}

	public void AlterAllIndexes()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckIndexTypeAllowsModification();
		ThrowIfBelowVersion90();
		try
		{
			affectAllIndexes = true;
			Alter();
		}
		finally
		{
			affectAllIndexes = false;
		}
	}

	private void ScriptSpIndexoptions(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptSchemaObjectBase scriptSchemaObjectBase = (ScriptSchemaObjectBase)base.ParentColl.ParentInstance;
		Property property = base.Properties.Get("DisallowRowLocks");
		if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_indexoption @IndexNamePattern = N'{0}.[{1}]', @OptionName = 'disallowrowlocks', @OptionValue = '{2}'", new object[3]
			{
				SqlSmoObject.SqlString(scriptSchemaObjectBase.FullQualifiedName),
				SqlSmoObject.SqlStringBraket(Name),
				((bool)property.Value) ? "on" : "off"
			}));
		}
		property = base.Properties.Get("DisallowPageLocks");
		if (property.Value != null && (property.Dirty || sp.ScriptForCreateDrop))
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_indexoption @IndexNamePattern = N'{0}.[{1}]', @OptionName = 'disallowpagelocks', @OptionValue = '{2}'", new object[3]
			{
				SqlSmoObject.SqlString(scriptSchemaObjectBase.FullQualifiedName),
				SqlSmoObject.SqlStringBraket(Name),
				((bool)property.Value) ? "on" : "off"
			}));
		}
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (base.ParentColl.ParentInstance is UserDefinedFunction)
		{
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfAUDF);
		}
		CheckIndexTypeAllowsModification(sp);
		if (IsMemoryOptimizedIndex)
		{
			if (!IsObjectDirty())
			{
				return;
			}
			if (GetPropValueOptional<IndexType>("IndexType").Value != IndexType.NonClusteredHashIndex)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.OnlyHashIndexIsSupportedInAlter);
			}
		}
		if (sp.TargetServerVersionInternal <= SqlServerVersionInternal.Version80)
		{
			ScriptSpIndexoptions(alterQuery, sp);
			return;
		}
		IndexScripter indexScripterForAlter = IndexScripter.GetIndexScripterForAlter(this, sp);
		string alterScript = indexScripterForAlter.GetAlterScript90(affectAllIndexes);
		if (!string.IsNullOrEmpty(alterScript))
		{
			alterQuery.Add(alterScript);
		}
	}

	public void Rebuild(int partitionNumber)
	{
		optimizePartitionNumber = partitionNumber;
		try
		{
			if (base.ParentColl.ParentInstance is UserDefinedTableType)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.RebuildIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
			}
			CheckObjectState(throwIfNotCreated: true);
			if (partitionNumber != -1)
			{
				this.ThrowIfNotSupported(typeof(PhysicalPartition));
			}
			RebuildImpl(allIndexes: false);
		}
		finally
		{
			optimizePartitionNumber = -1;
		}
	}

	public void Rebuild()
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RebuildIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		RebuildImpl(allIndexes: false);
	}

	public void Resume()
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ResumeIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		ResumeImpl();
	}

	private void ResumeImpl()
	{
		ThrowIfPropertyNotSupported("ResumableOperationState");
		try
		{
			StringCollection stringCollection = new StringCollection();
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			scriptingPreferences.ScriptForCreateDrop = true;
			GetContextDB().AddUseDb(stringCollection, scriptingPreferences);
			IndexScripter indexScripter = IndexScripter.GetIndexScripter(this, scriptingPreferences);
			stringCollection.Add(indexScripter.GetResumeScript());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ResumeIndexes, this, ex);
		}
	}

	public void Abort()
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.AbortIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		AbortImpl();
	}

	private void AbortImpl()
	{
		ThrowIfPropertyNotSupported("ResumableOperationState");
		try
		{
			StringCollection stringCollection = new StringCollection();
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			scriptingPreferences.ScriptForCreateDrop = true;
			GetContextDB().AddUseDb(stringCollection, scriptingPreferences);
			IndexScripter indexScripter = IndexScripter.GetIndexScripter(this, scriptingPreferences);
			stringCollection.Add(indexScripter.GetAbortOrPauseScript(isAbort: true));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AbortIndexes, this, ex);
		}
	}

	public void Pause()
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.PauseIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		PauseImpl();
	}

	private void PauseImpl()
	{
		ThrowIfPropertyNotSupported("ResumableOperationState");
		try
		{
			StringCollection stringCollection = new StringCollection();
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			scriptingPreferences.ScriptForCreateDrop = true;
			GetContextDB().AddUseDb(stringCollection, scriptingPreferences);
			IndexScripter indexScripter = IndexScripter.GetIndexScripter(this, scriptingPreferences);
			stringCollection.Add(indexScripter.GetAbortOrPauseScript(isAbort: false));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.PauseIndexes, this, ex);
		}
	}

	public void RebuildAllIndexes()
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RebuildIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		RebuildImpl(allIndexes: true);
	}

	public void UpgradeToClusteredColumnStoreIndex()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RebuildIndexes, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		if (InferredIndexType != IndexType.ClusteredIndex)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, null, ExceptionTemplatesImpl.InvalidUpgradeToCCIIndexType);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion120();
			UpgradeToCCI120Impl();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RebuildIndexes, this, ex);
		}
	}

	internal bool AddSetOptionsForIndex(StringCollection queries)
	{
		if (((m_bIsOnComputed || xmlOrSpatialIndex) && GetIndexKeyType() == IndexKeyType.None) || Parent is View)
		{
			queries.Add("SET ARITHABORT ON");
			queries.Add("SET CONCAT_NULL_YIELDS_NULL ON");
			queries.Add("SET QUOTED_IDENTIFIER ON");
			queries.Add("SET ANSI_NULLS ON");
			queries.Add("SET ANSI_PADDING ON");
			queries.Add("SET ANSI_WARNINGS ON");
			queries.Add("SET NUMERIC_ROUNDABORT OFF");
			return true;
		}
		if ((m_bIsOnComputed && GetIndexKeyType() != IndexKeyType.None) || isOnColumnWithAnsiPadding)
		{
			queries.Add("SET ANSI_PADDING ON");
		}
		return false;
	}

	private void RebuildImpl(bool allIndexes)
	{
		IndexScripter indexScripter;
		try
		{
			StringCollection stringCollection = new StringCollection();
			_ = (ScriptSchemaObjectBase)base.ParentColl.ParentInstance;
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			scriptingPreferences.ScriptForCreateDrop = true;
			GetContextDB().AddUseDb(stringCollection, scriptingPreferences);
			if ((base.ServerVersion.Major < 9 || (Parent is View && ((View)Parent).Parent.CompatibilityLevel < CompatibilityLevel.Version90)) && Parent is TableViewTableTypeBase tableViewTableTypeBase)
			{
				foreach (Index index in tableViewTableTypeBase.Indexes)
				{
					if (index.AddSetOptionsForIndex(stringCollection))
					{
						break;
					}
				}
			}
			indexScripter = IndexScripter.GetIndexScripter(this, scriptingPreferences);
			stringCollection.Add(indexScripter.GetRebuildScript(allIndexes, optimizePartitionNumber));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Rebuild, this, ex);
		}
		if (base.ServerVersion.Major >= 9 && m_PhysicalPartitions != null && (indexScripter is RegularIndexScripter || (indexScripter is SpatialIndexScripter && base.ServerVersion.Major >= 11)) && !ExecutionManager.Recording)
		{
			if (optimizePartitionNumber == -1)
			{
				PhysicalPartitions.Reset();
			}
			else
			{
				PhysicalPartitions.Reset(optimizePartitionNumber);
			}
		}
	}

	public void Rename(string newname)
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Rename, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		Table.CheckTableName(newname);
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_rename N'{0}.{1}', N'{2}', N'INDEX'", new object[3]
		{
			SqlSmoObject.SqlString(Parent.FullQualifiedName),
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public void Reorganize(int partitionNumber = -1)
	{
		try
		{
			if (partitionNumber != -1)
			{
				this.ThrowIfNotSupported(typeof(PhysicalPartition));
				optimizePartitionNumber = partitionNumber;
			}
			ReorganizeGeneralImpl(allIndexes: false);
		}
		finally
		{
			optimizePartitionNumber = -1;
		}
	}

	public void ReorganizeAllIndexes()
	{
		ReorganizeGeneralImpl(allIndexes: true);
	}

	private void ReorganizeGeneralImpl(bool allIndexes)
	{
		CheckIndexTypeAllowsModification();
		CheckUnsupportedSXI(checkPrimarySXI: true, checkSecondarySXI: true, ExceptionTemplatesImpl.Reorganize, ExceptionTemplatesImpl.SelectiveXmlIndexDoesNotSupportReorganize);
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Reorganize, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion80();
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.SetTargetServerInfo(this);
			if (base.ServerVersion.Major == 8 || scriptingPreferences.TargetServerVersionInternal == SqlServerVersionInternal.Version80 || scriptingPreferences.TargetServerVersionInternal == SqlServerVersionInternal.Version70)
			{
				Reorganize80Impl(allIndexes);
			}
			else
			{
				AlterIndexReorganizeImpl(allIndexes);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Reorganize, this, ex);
		}
	}

	private void Reorganize80Impl(bool allIndexes)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		TableViewTableTypeBase tableViewTableTypeBase = (TableViewTableTypeBase)base.ParentColl.ParentInstance;
		if (allIndexes)
		{
			foreach (Index index in ((TableViewTableTypeBase)base.ParentColl.ParentInstance).Indexes)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC INDEXDEFRAG( N'{0}', N'{1}', N'{2}' )", new object[3]
				{
					SqlSmoObject.SqlString(GetDBName()),
					SqlSmoObject.SqlString(tableViewTableTypeBase.FullQualifiedName),
					SqlSmoObject.SqlString(index.Name)
				}));
			}
		}
		else
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC INDEXDEFRAG( N'{0}', N'{1}', N'{2}' )", new object[3]
			{
				SqlSmoObject.SqlString(GetDBName()),
				SqlSmoObject.SqlString(tableViewTableTypeBase.FullQualifiedName),
				SqlSmoObject.SqlString(Name)
			}));
		}
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	private void AlterIndexReorganizeImpl(bool allIndexes)
	{
		if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase && base.ServerVersion.Major < 12)
		{
			throw new SmoException(ExceptionTemplatesImpl.PropertyNotSupportedForCloudVersion("REORGANIZE", base.ServerVersion.Major.ToString()));
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		ScriptSchemaObjectBase scriptSchemaObjectBase = (ScriptSchemaObjectBase)base.ParentColl.ParentInstance;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (allIndexes)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX ALL ON {0} REORGANIZE", new object[1] { scriptSchemaObjectBase.FullQualifiedName });
		}
		else
		{
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (optimizePartitionNumber != -1)
			{
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "PARTITION = {0}", new object[1] { optimizePartitionNumber });
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX [{0}] ON {1} REORGANIZE {2}", new object[3]
			{
				SqlSmoObject.SqlBraket(Name),
				scriptSchemaObjectBase.FullQualifiedName,
				stringBuilder2.ToString()
			});
		}
		StringBuilder stringBuilder3 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		int optCount = 0;
		ScriptAlterPropNonBag(CompactLargeObjects, "LOB_COMPACTION", base.ServerVersion, stringBuilder3, ref optCount);
		if (base.ServerVersion.Major > 12 && (InferredIndexType == IndexType.ClusteredColumnStoreIndex || InferredIndexType == IndexType.NonClusteredColumnStoreIndex))
		{
			ScriptAlterPropNonBag(CompressAllRowGroups, "COMPRESS_ALL_ROW_GROUPS", base.ServerVersion, stringBuilder3, ref optCount);
		}
		if (0 < stringBuilder3.Length)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " WITH ( {0} )", new object[1] { stringBuilder3.ToString() });
		}
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	private void UpgradeToCCI120Impl()
	{
		ScriptSchemaObjectBase parent = (ScriptSchemaObjectBase)base.ParentColl.ParentInstance;
		Index index = new Index(parent, Name);
		index.IndexType = IndexType.ClusteredColumnStoreIndex;
		index.dropExistingIndex = true;
		index.Create();
		((TableViewBase)base.ParentColl.ParentInstance).Refresh();
	}

	private void ScriptAlterPropNonBag(bool propValue, string optname, ServerVersion serverVersion, StringBuilder statement, ref int optCount)
	{
		if (serverVersion.Major == 7 || serverVersion.Major == 8)
		{
			if (propValue)
			{
				if (0 < optCount++)
				{
					statement.Append(Globals.commaspace);
				}
				statement.Append(optname);
			}
		}
		else
		{
			if (0 < optCount++)
			{
				statement.Append(Globals.commaspace);
			}
			statement.AppendFormat(SmoApplication.DefaultCulture, "{0} = {1}", new object[2]
			{
				optname,
				propValue ? "ON" : "OFF"
			});
		}
	}

	public void Disable()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Disable, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckIndexTypeAllowsModification();
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion90();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			ScriptSchemaObjectBase scriptSchemaObjectBase = (ScriptSchemaObjectBase)base.ParentColl.ParentInstance;
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER INDEX [{0}] ON {1} DISABLE", new object[2]
			{
				SqlSmoObject.SqlBraket(Name),
				scriptSchemaObjectBase.FullQualifiedName
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (base.ServerVersion.Major > 9 && PhysicalPartitions != null && !ExecutionManager.Recording)
			{
				IndexType inferredIndexType = InferredIndexType;
				if (inferredIndexType == IndexType.NonClusteredIndex || inferredIndexType == IndexType.NonClusteredColumnStoreIndex)
				{
					PhysicalPartitions.Refresh();
				}
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Disable, this, ex);
		}
	}

	public void Enable(IndexEnableAction action)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Enable, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion90();
			switch (action)
			{
			case IndexEnableAction.Rebuild:
				Rebuild();
				break;
			case IndexEnableAction.Recreate:
				Recreate();
				break;
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Enable, this, ex);
		}
	}

	public void Recreate()
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Recreate, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		bool flag = dropExistingIndex;
		try
		{
			dropExistingIndex = true;
			string dBName = GetDBName();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(dBName) }));
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = false;
			scriptingPreferences.SetTargetServerInfo(this);
			ScriptCreate(stringCollection, scriptingPreferences);
			if (stringCollection.Count > 1)
			{
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Recreate, this, ex);
		}
		finally
		{
			dropExistingIndex = flag;
		}
	}

	public StringCollection CheckIndex()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckIndex, this, null, ExceptionTemplatesImpl.NotCheckIndexOnUDTT);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName) }));
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			string s = ((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FormatFullNameForScripting(scriptingPreferences);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKTABLE (N'{0}', {1}) WITH NO_INFOMSGS", new object[2]
			{
				SqlSmoObject.SqlString(s),
				(int)base.Properties["ID"].Value
			}));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckIndex, this, ex);
		}
	}

	public DataTable CheckIndexWithResult()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckIndex, this, null, ExceptionTemplatesImpl.NotCheckIndexOnUDTT);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName) }));
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			string s = ((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FormatFullNameForScripting(scriptingPreferences);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKTABLE (N'{0}', {1}) WITH TABLERESULTS, NO_INFOMSGS", new object[2]
			{
				SqlSmoObject.SqlString(s),
				(int)base.Properties["ID"].Value
			}));
			DataSet dataSet = ExecutionManager.ExecuteWithResults(stringCollection);
			if (dataSet.Tables.Count > 0)
			{
				return dataSet.Tables[0];
			}
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			return dataTable;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckIndex, this, ex);
		}
	}

	public DataSet EnumStatistics()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumStatistics, this, null, ExceptionTemplatesImpl.NotStatisticsOnUDTT);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.ParentColl.ParentInstance.InternalName) }));
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			string s = ((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FormatFullNameForScripting(scriptingPreferences);
			_ = (Database)base.ParentColl.ParentInstance.ParentColl.ParentInstance;
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC SHOW_STATISTICS ({0}, {1})", new object[2]
			{
				SqlSmoObject.MakeSqlString(s),
				SqlSmoObject.MakeSqlString(Name)
			}));
			return ExecutionManager.ExecuteWithResults(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumStatistics, this, ex);
		}
	}

	public void RecalculateSpaceUsage()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.RecalculateSpaceUsage, this, null, ExceptionTemplatesImpl.NotFragInfoOnUDTT);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			SqlSmoObject parentInstance = base.ParentColl.ParentInstance;
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(parentInstance.ParentColl.ParentInstance.InternalName) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC UPDATEUSAGE(0, N'[{0}].[{1}]', {2}) WITH NO_INFOMSGS", new object[3]
			{
				SqlSmoObject.SqlString(((ScriptSchemaObjectBase)parentInstance).Schema),
				SqlSmoObject.SqlString(parentInstance.InternalName),
				(int)base.Properties["ID"].Value
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RecalculateSpaceUsage, this, ex);
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		Version version = new Version(base.ServerVersion.Major, base.ServerVersion.Minor, base.ServerVersion.BuildNumber);
		Version version2 = new Version(11, 0, 2813);
		bool flag = version >= version2;
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[5]
			{
				new PropagateInfo((base.ServerVersion.Major < 10) ? null : m_PhysicalPartitions, bWithScript: false, bPropagateScriptToChildLevel: false),
				new PropagateInfo(IndexedColumns, bWithScript: false, bPropagateScriptToChildLevel: false),
				new PropagateInfo(flag ? IndexedXmlPaths : null, bWithScript: false, bPropagateScriptToChildLevel: false),
				new PropagateInfo(flag ? IndexedXmlPathNamespaces : null, bWithScript: false, bPropagateScriptToChildLevel: false),
				new PropagateInfo((base.ServerVersion.Major < 8 || Parent is UserDefinedTableType) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
			};
		}
		return new PropagateInfo[1]
		{
			new PropagateInfo(IndexedColumns, bWithScript: false)
		};
	}

	public void UpdateStatistics()
	{
		UpdateStatistics(StatisticsScanType.Default, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsScanType scanType)
	{
		UpdateStatistics(scanType, 0, recompute: true);
	}

	public void UpdateStatistics(StatisticsScanType scanType, int sampleValue)
	{
		UpdateStatistics(scanType, sampleValue, recompute: true);
	}

	public void UpdateStatistics(StatisticsScanType scanType, int sampleValue, bool recompute)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateStatistics, this, null, ExceptionTemplatesImpl.NotStatisticsOnUDTT);
		}
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			string fullQualifiedName = ((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FullQualifiedName;
			ExecutionManager.ExecuteNonQuery(Statistic.UpdateStatistics(SqlSmoObject.MakeSqlBraket(GetDBName()), fullQualifiedName, SqlSmoObject.MakeSqlBraket(Name), scanType, StatisticsTarget.Index, !recompute, sampleValue));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.UpdateStatistics, this, ex);
		}
	}

	public DataTable EnumFragmentation()
	{
		return EnumFragmentation(FragmentationOption.Fast);
	}

	public DataTable EnumFragmentation(FragmentationOption fragmentationOption)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumFragmentation, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		try
		{
			CheckObjectState();
			string text = string.Format(SmoApplication.DefaultCulture, "{0}/{1}", new object[2]
			{
				base.Urn.Value,
				GetFragOptionString(fragmentationOption)
			});
			Request request = new Request(text);
			request.ParentPropertiesRequests = new PropertiesRequest[1];
			PropertiesRequest propertiesRequest = new PropertiesRequest();
			propertiesRequest.Fields = new string[2] { "Name", "ID" };
			propertiesRequest.OrderByList = new OrderBy[1]
			{
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			request.ParentPropertiesRequests[0] = propertiesRequest;
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumFragmentation, this, ex);
		}
	}

	public DataTable EnumFragmentation(FragmentationOption fragmentationOption, int partitionNumber)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumFragmentation, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		try
		{
			CheckObjectState();
			if (base.ServerVersion.Major < 9)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidParamForVersion("EnumFragmentation", "partitionNumber", GetSqlServerVersionName())).SetHelpContext("InvalidParamForVersion");
			}
			string text = string.Format(SmoApplication.DefaultCulture, "{0}/{1}[@PartitionNumber={2}]", new object[3]
			{
				base.Urn.Value,
				GetFragOptionString(fragmentationOption),
				partitionNumber
			});
			Request request = new Request(text);
			request.ParentPropertiesRequests = new PropertiesRequest[1];
			PropertiesRequest propertiesRequest = new PropertiesRequest();
			propertiesRequest.Fields = new string[2] { "Name", "ID" };
			propertiesRequest.OrderByList = new OrderBy[1]
			{
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			request.ParentPropertiesRequests[0] = propertiesRequest;
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumFragmentation, this, ex);
		}
	}

	public void DropAndMove(string partitionScheme, StringCollection partitionSchemeParameters)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		ThrowIfBelowVersion90();
		if (partitionScheme == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, new ArgumentNullException("partitionScheme"));
		}
		if (partitionScheme.Length == 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, null, ExceptionTemplatesImpl.EmptyInputParam("partitionScheme", "string"));
		}
		if (partitionSchemeParameters == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, new ArgumentNullException("partitionSchemeParameters"));
		}
		if (partitionSchemeParameters.Count == 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, null, ExceptionTemplatesImpl.EmptyInputParam("partitionSchemeParameters", "Collection"));
		}
		DropAndMoveImpl(partitionScheme, partitionSchemeParameters);
	}

	public void DropAndMove(string fileGroup)
	{
		CheckIndexTypeAllowsModification();
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, null, ExceptionTemplatesImpl.UDTTIndexCannotBeModified);
		}
		CheckObjectState(throwIfNotCreated: true);
		ThrowIfBelowVersion90();
		if (fileGroup == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, new ArgumentNullException("fileGroup"));
		}
		if (fileGroup.Length == 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, null, ExceptionTemplatesImpl.EmptyInputParam("fileGroup", "string"));
		}
		DropAndMoveImpl(fileGroup, null);
	}

	private void DropAndMoveImpl(string dataSpaceName, StringCollection partitionSchemeParameters)
	{
		try
		{
			DropAndMoveImplWorker(dataSpaceName, partitionSchemeParameters);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropAndMove, this, ex);
		}
	}

	private void DropAndMoveImplWorker(string dataSpaceName, StringCollection partitionSchemeParameters)
	{
		if (!(bool)base.Properties["IsClustered"].Value)
		{
			throw new SmoException(ExceptionTemplatesImpl.IndexMustBeClustered(((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FullQualifiedName, FullQualifiedName));
		}
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
		scriptingPreferences.ScriptForCreateDrop = true;
		scriptingPreferences.IncludeScripts.ExistenceCheck = false;
		scriptingPreferences.IncludeScripts.Header = false;
		ClusteredRegularIndexScripter clusteredRegularIndexScripter = new ClusteredRegularIndexScripter(this, scriptingPreferences);
		clusteredRegularIndexScripter.DataSpaceName = dataSpaceName;
		clusteredRegularIndexScripter.PartitionSchemeParameters = partitionSchemeParameters;
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		stringCollection.Add(clusteredRegularIndexScripter.GetDropScript());
		ExecutionManager.ExecuteNonQuery(stringCollection);
		if (!ExecutionManager.Recording)
		{
			if (base.ParentColl != null)
			{
				base.ParentColl.RemoveObject(new SimpleObjectKey(Name));
			}
			MarkDropped();
		}
	}

	public StringCollection Script()
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		if (base.ParentColl.ParentInstance is UserDefinedTableType)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Script, this, null, ExceptionTemplatesImpl.OperationNotSupportedWhenPartOfUDTT);
		}
		return ScriptImpl(scriptingOptions);
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "IndexKeyType" && !prop.Dirty)
		{
			oldIndexKeyTypeValue = prop.Value;
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		oldIndexKeyTypeValue = null;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[34]
		{
			"BoundingBoxXMax", "BoundingBoxXMin", "BoundingBoxYMax", "BoundingBoxYMin", "BucketCount", "CellsPerObject", "CompressionDelay", "DisallowPageLocks", "DisallowRowLocks", "FileGroup",
			"FileStreamFileGroup", "FileStreamPartitionScheme", "FillFactor", "FilterDefinition", "IgnoreDuplicateKeys", "IndexedXmlPathName", "IndexKeyType", "IndexType", "IsClustered", "IsDisabled",
			"IsFileTableDefined", "IsSystemNamed", "IsSystemObject", "IsUnique", "Level1Grid", "Level2Grid", "Level3Grid", "Level4Grid", "NoAutomaticRecomputation", "PadIndex",
			"ParentXmlIndex", "PartitionScheme", "SecondaryXmlIndexType", "SpatialIndexType"
		};
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	internal static string[] GetScriptFields2(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode, ScriptingPreferences sp)
	{
		if ((parentType.Name == "Table" || parentType.Name == "View") && version.Major > 9 && sp.TargetServerVersionInternal > SqlServerVersionInternal.Version90 && sp.Storage.DataCompression)
		{
			return new string[1] { "HasCompressedPartitions" };
		}
		return new string[0];
	}

	internal static string[] GetRebuildFields(ServerVersion version, DatabaseEngineType databaseEngineType)
	{
		return GetRebuildFields(version, databaseEngineType, DatabaseEngineEdition.Unknown);
	}

	internal static string[] GetRebuildFields(ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition)
	{
		string[] fields = new string[14]
		{
			"IsSystemNamed", "IndexKeyType", "IndexType", "IsUnique", "IsClustered", "IsDisabled", "IgnoreDuplicateKeys", "FillFactor", "PadIndex", "DisallowRowLocks",
			"DisallowPageLocks", "NoAutomaticRecomputation", "HasCompressedPartitions", "IsSystemObject"
		};
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	internal static bool IsLargeObject(SqlDataType dataType)
	{
		if (dataType != SqlDataType.Image && dataType != SqlDataType.Text && dataType != SqlDataType.NText && dataType != SqlDataType.VarCharMax && dataType != SqlDataType.NVarCharMax && dataType != SqlDataType.VarBinaryMax && dataType != SqlDataType.Xml && dataType != SqlDataType.Geometry)
		{
			return dataType == SqlDataType.Geography;
		}
		return true;
	}
}

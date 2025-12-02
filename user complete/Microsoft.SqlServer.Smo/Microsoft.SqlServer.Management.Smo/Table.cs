using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
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
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class Table : TableViewBase, ISfcSupportsDesignMode, IColumnPermission, IObjectPermission, IPropertyDataDispatch, ICreatable, IAlterable, IDroppable, IDropIfExists, IRenamable, ITableOptions, IDmfFacet
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 17, 26, 36, 43, 43, 47, 50, 74, 74, 74 };

		private static int[] cloudVersionCount = new int[3] { 17, 17, 62 };

		private static int sqlDwPropertyCount = 42;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[42]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataSourceName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("Durability", expensive: false, readOnly: false, typeof(DurabilityType)),
			new StaticMetadata("DwTableDistribution", expensive: false, readOnly: false, typeof(DwTableDistributionType)),
			new StaticMetadata("FakeSystemTable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("FileFormatName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("FileGroup", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("HasClusteredColumnStoreIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasCompressedPartitions", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasHeapIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredColumnStoreIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasPrimaryClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSparseColumn", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSpatialData", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasXmlData", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasXmlIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsEdge", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsExternal", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsIndexable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsMemoryOptimized", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsNode", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPartitioned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Location", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("LockEscalation", expensive: false, readOnly: false, typeof(LockEscalationType)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RejectedRowLocation", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RejectSampleValue", expensive: true, readOnly: false, typeof(double)),
			new StaticMetadata("RejectType", expensive: true, readOnly: false, typeof(ExternalTableRejectType)),
			new StaticMetadata("RejectValue", expensive: true, readOnly: false, typeof(double)),
			new StaticMetadata("RowCount", expensive: true, readOnly: true, typeof(long)),
			new StaticMetadata("TemporalType", expensive: false, readOnly: true, typeof(TableTemporalType)),
			new StaticMetadata("TextFileGroup", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[62]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("HasAfterTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasDeleteTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsertTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsteadOfTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSparseColumn", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasUpdateTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsIndexable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("LockEscalation", expensive: false, readOnly: false, typeof(LockEscalationType)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Replicated", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("RowCount", expensive: true, readOnly: true, typeof(long)),
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ChangeTrackingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DataSourceName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("Durability", expensive: false, readOnly: false, typeof(DurabilityType)),
			new StaticMetadata("ExternalTableDistribution", expensive: true, readOnly: false, typeof(ExternalTableDistributionType)),
			new StaticMetadata("FakeSystemTable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("FileGroup", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("FileStreamFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileStreamPartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileTableDirectoryName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileTableNameColumnCollation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileTableNamespaceEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("HasClusteredColumnStoreIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasCompressedPartitions", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("HasHeapIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredColumnStoreIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasPrimaryClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSpatialData", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSystemTimePeriod", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasXmlData", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasXmlIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HistoryRetentionPeriod", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("HistoryRetentionPeriodUnit", expensive: false, readOnly: false, typeof(TemporalHistoryRetentionPeriodUnit)),
			new StaticMetadata("HistoryTableID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("HistoryTableName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HistoryTableSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsEdge", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsExternal", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsFileTable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsMemoryOptimized", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsNode", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsPartitioned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemVersioned", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RemoteObjectName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteSchemaName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("ShardingColumnName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("SystemTimePeriodEndColumn", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SystemTimePeriodStartColumn", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("TemporalType", expensive: false, readOnly: true, typeof(TableTemporalType)),
			new StaticMetadata("TextFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("TrackColumnsUpdatedEnabled", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[74]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DataSpaceUsed", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("FakeSystemTable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("FileGroup", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("HasClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasPrimaryClusteredIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSparseColumn", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IndexSpaceUsed", expensive: true, readOnly: true, typeof(double)),
			new StaticMetadata("IsEdge", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsNode", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Replicated", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("RowCount", expensive: true, readOnly: true, typeof(long)),
			new StaticMetadata("TextFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("HasAfterTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasDeleteTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsertTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasInsteadOfTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasUpdateTrigger", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsIndexable", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("HasHeapIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasNonClusteredColumnStoreIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasSpatialData", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasXmlData", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HasXmlIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsPartitioned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsVarDecimalStorageFormatEnabled", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("PartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ChangeTrackingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("FileStreamFileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileStreamPartitionScheme", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasCompressedPartitions", expensive: true, readOnly: true, typeof(bool)),
			new StaticMetadata("LockEscalation", expensive: false, readOnly: false, typeof(LockEscalationType)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("TrackColumnsUpdatedEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("FileTableDirectoryName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileTableNameColumnCollation", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FileTableNamespaceEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsFileTable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Durability", expensive: false, readOnly: false, typeof(DurabilityType)),
			new StaticMetadata("HasClusteredColumnStoreIndex", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsMemoryOptimized", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DataSourceName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("ExternalTableDistribution", expensive: true, readOnly: false, typeof(ExternalTableDistributionType)),
			new StaticMetadata("FileFormatName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("HasSystemTimePeriod", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("HistoryTableID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("HistoryTableName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HistoryTableSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsExternal", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemVersioned", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Location", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RejectSampleValue", expensive: true, readOnly: false, typeof(double)),
			new StaticMetadata("RejectType", expensive: true, readOnly: false, typeof(ExternalTableRejectType)),
			new StaticMetadata("RejectValue", expensive: true, readOnly: false, typeof(double)),
			new StaticMetadata("RemoteDataArchiveDataMigrationState", expensive: false, readOnly: false, typeof(RemoteDataArchiveMigrationState)),
			new StaticMetadata("RemoteDataArchiveEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RemoteDataArchiveFilterPredicate", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteObjectName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteSchemaName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteTableName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("RemoteTableProvisioned", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ShardingColumnName", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("SystemTimePeriodEndColumn", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("SystemTimePeriodStartColumn", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("TemporalType", expensive: false, readOnly: true, typeof(TableTemporalType))
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
						"AnsiNullsStatus" => 0, 
						"CreateDate" => 1, 
						"DataSourceName" => 2, 
						"DateLastModified" => 3, 
						"Durability" => 4, 
						"DwTableDistribution" => 5, 
						"FakeSystemTable" => 6, 
						"FileFormatName" => 7, 
						"FileGroup" => 8, 
						"HasClusteredColumnStoreIndex" => 9, 
						"HasClusteredIndex" => 10, 
						"HasCompressedPartitions" => 11, 
						"HasHeapIndex" => 12, 
						"HasIndex" => 13, 
						"HasNonClusteredColumnStoreIndex" => 14, 
						"HasNonClusteredIndex" => 15, 
						"HasPrimaryClusteredIndex" => 16, 
						"HasSparseColumn" => 17, 
						"HasSpatialData" => 18, 
						"HasXmlData" => 19, 
						"HasXmlIndex" => 20, 
						"ID" => 21, 
						"IsEdge" => 22, 
						"IsExternal" => 23, 
						"IsIndexable" => 24, 
						"IsMemoryOptimized" => 25, 
						"IsNode" => 26, 
						"IsPartitioned" => 27, 
						"IsSchemaOwned" => 28, 
						"IsSystemObject" => 29, 
						"Location" => 30, 
						"LockEscalation" => 31, 
						"Owner" => 32, 
						"PartitionScheme" => 33, 
						"QuotedIdentifierStatus" => 34, 
						"RejectedRowLocation" => 35, 
						"RejectSampleValue" => 36, 
						"RejectType" => 37, 
						"RejectValue" => 38, 
						"RowCount" => 39, 
						"TemporalType" => 40, 
						"TextFileGroup" => 41, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"HasAfterTrigger" => 2, 
					"HasDeleteTrigger" => 3, 
					"HasIndex" => 4, 
					"HasInsertTrigger" => 5, 
					"HasInsteadOfTrigger" => 6, 
					"HasSparseColumn" => 7, 
					"HasUpdateTrigger" => 8, 
					"ID" => 9, 
					"IsIndexable" => 10, 
					"IsSchemaOwned" => 11, 
					"IsSystemObject" => 12, 
					"LockEscalation" => 13, 
					"Owner" => 14, 
					"Replicated" => 15, 
					"RowCount" => 16, 
					"AnsiNullsStatus" => 17, 
					"ChangeTrackingEnabled" => 18, 
					"DataSourceName" => 19, 
					"Durability" => 20, 
					"ExternalTableDistribution" => 21, 
					"FakeSystemTable" => 22, 
					"FileGroup" => 23, 
					"FileStreamFileGroup" => 24, 
					"FileStreamPartitionScheme" => 25, 
					"FileTableDirectoryName" => 26, 
					"FileTableNameColumnCollation" => 27, 
					"FileTableNamespaceEnabled" => 28, 
					"HasClusteredColumnStoreIndex" => 29, 
					"HasClusteredIndex" => 30, 
					"HasCompressedPartitions" => 31, 
					"HasHeapIndex" => 32, 
					"HasNonClusteredColumnStoreIndex" => 33, 
					"HasNonClusteredIndex" => 34, 
					"HasPrimaryClusteredIndex" => 35, 
					"HasSpatialData" => 36, 
					"HasSystemTimePeriod" => 37, 
					"HasXmlData" => 38, 
					"HasXmlIndex" => 39, 
					"HistoryRetentionPeriod" => 40, 
					"HistoryRetentionPeriodUnit" => 41, 
					"HistoryTableID" => 42, 
					"HistoryTableName" => 43, 
					"HistoryTableSchema" => 44, 
					"IsEdge" => 45, 
					"IsExternal" => 46, 
					"IsFileTable" => 47, 
					"IsMemoryOptimized" => 48, 
					"IsNode" => 49, 
					"IsPartitioned" => 50, 
					"IsSystemVersioned" => 51, 
					"PartitionScheme" => 52, 
					"QuotedIdentifierStatus" => 53, 
					"RemoteObjectName" => 54, 
					"RemoteSchemaName" => 55, 
					"ShardingColumnName" => 56, 
					"SystemTimePeriodEndColumn" => 57, 
					"SystemTimePeriodStartColumn" => 58, 
					"TemporalType" => 59, 
					"TextFileGroup" => 60, 
					"TrackColumnsUpdatedEnabled" => 61, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DataSpaceUsed" => 1, 
				"FakeSystemTable" => 2, 
				"FileGroup" => 3, 
				"HasClusteredIndex" => 4, 
				"HasNonClusteredIndex" => 5, 
				"HasPrimaryClusteredIndex" => 6, 
				"HasSparseColumn" => 7, 
				"ID" => 8, 
				"IndexSpaceUsed" => 9, 
				"IsEdge" => 10, 
				"IsNode" => 11, 
				"IsSystemObject" => 12, 
				"Owner" => 13, 
				"Replicated" => 14, 
				"RowCount" => 15, 
				"TextFileGroup" => 16, 
				"AnsiNullsStatus" => 17, 
				"HasAfterTrigger" => 18, 
				"HasDeleteTrigger" => 19, 
				"HasIndex" => 20, 
				"HasInsertTrigger" => 21, 
				"HasInsteadOfTrigger" => 22, 
				"HasUpdateTrigger" => 23, 
				"IsIndexable" => 24, 
				"QuotedIdentifierStatus" => 25, 
				"DateLastModified" => 26, 
				"HasHeapIndex" => 27, 
				"HasNonClusteredColumnStoreIndex" => 28, 
				"HasSpatialData" => 29, 
				"HasXmlData" => 30, 
				"HasXmlIndex" => 31, 
				"IsPartitioned" => 32, 
				"IsSchemaOwned" => 33, 
				"IsVarDecimalStorageFormatEnabled" => 34, 
				"PartitionScheme" => 35, 
				"ChangeTrackingEnabled" => 36, 
				"FileStreamFileGroup" => 37, 
				"FileStreamPartitionScheme" => 38, 
				"HasCompressedPartitions" => 39, 
				"LockEscalation" => 40, 
				"PolicyHealthState" => 41, 
				"TrackColumnsUpdatedEnabled" => 42, 
				"FileTableDirectoryName" => 43, 
				"FileTableNameColumnCollation" => 44, 
				"FileTableNamespaceEnabled" => 45, 
				"IsFileTable" => 46, 
				"Durability" => 47, 
				"HasClusteredColumnStoreIndex" => 48, 
				"IsMemoryOptimized" => 49, 
				"DataSourceName" => 50, 
				"ExternalTableDistribution" => 51, 
				"FileFormatName" => 52, 
				"HasSystemTimePeriod" => 53, 
				"HistoryTableID" => 54, 
				"HistoryTableName" => 55, 
				"HistoryTableSchema" => 56, 
				"IsExternal" => 57, 
				"IsSystemVersioned" => 58, 
				"Location" => 59, 
				"RejectSampleValue" => 60, 
				"RejectType" => 61, 
				"RejectValue" => 62, 
				"RemoteDataArchiveDataMigrationState" => 63, 
				"RemoteDataArchiveEnabled" => 64, 
				"RemoteDataArchiveFilterPredicate" => 65, 
				"RemoteObjectName" => 66, 
				"RemoteSchemaName" => 67, 
				"RemoteTableName" => 68, 
				"RemoteTableProvisioned" => 69, 
				"ShardingColumnName" => 70, 
				"SystemTimePeriodEndColumn" => 71, 
				"SystemTimePeriodStartColumn" => 72, 
				"TemporalType" => 73, 
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
		private bool _AnsiNullsStatus;

		private string _DataSourceName;

		private ExternalTableDistributionType _ExternalTableDistribution;

		private string _FileFormatName;

		private string _FileGroup;

		private string _FileStreamFileGroup;

		private string _FileStreamPartitionScheme;

		private int _ID;

		private bool _IsExternal;

		private bool _IsPartitioned;

		private bool _IsSystemObject;

		private string _Location;

		private string _PartitionScheme;

		private bool _QuotedIdentifierStatus;

		private double _RejectSampleValue;

		private ExternalTableRejectType _RejectType;

		private double _RejectValue;

		private string _RemoteObjectName;

		private string _RemoteSchemaName;

		private string _ShardingColumnName;

		private string _TextFileGroup;

		private DwTableDistributionType _DwTableDistribution;

		private string _RejectedRowLocation;

		internal bool AnsiNullsStatus
		{
			get
			{
				return _AnsiNullsStatus;
			}
			set
			{
				_AnsiNullsStatus = value;
			}
		}

		internal string DataSourceName
		{
			get
			{
				return _DataSourceName;
			}
			set
			{
				_DataSourceName = value;
			}
		}

		internal ExternalTableDistributionType ExternalTableDistribution
		{
			get
			{
				return _ExternalTableDistribution;
			}
			set
			{
				_ExternalTableDistribution = value;
			}
		}

		internal string FileFormatName
		{
			get
			{
				return _FileFormatName;
			}
			set
			{
				_FileFormatName = value;
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

		internal bool IsExternal
		{
			get
			{
				return _IsExternal;
			}
			set
			{
				_IsExternal = value;
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

		internal string Location
		{
			get
			{
				return _Location;
			}
			set
			{
				_Location = value;
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

		internal bool QuotedIdentifierStatus
		{
			get
			{
				return _QuotedIdentifierStatus;
			}
			set
			{
				_QuotedIdentifierStatus = value;
			}
		}

		internal double RejectSampleValue
		{
			get
			{
				return _RejectSampleValue;
			}
			set
			{
				_RejectSampleValue = value;
			}
		}

		internal ExternalTableRejectType RejectType
		{
			get
			{
				return _RejectType;
			}
			set
			{
				_RejectType = value;
			}
		}

		internal double RejectValue
		{
			get
			{
				return _RejectValue;
			}
			set
			{
				_RejectValue = value;
			}
		}

		internal string RemoteObjectName
		{
			get
			{
				return _RemoteObjectName;
			}
			set
			{
				_RemoteObjectName = value;
			}
		}

		internal string RemoteSchemaName
		{
			get
			{
				return _RemoteSchemaName;
			}
			set
			{
				_RemoteSchemaName = value;
			}
		}

		internal string ShardingColumnName
		{
			get
			{
				return _ShardingColumnName;
			}
			set
			{
				_ShardingColumnName = value;
			}
		}

		internal string TextFileGroup
		{
			get
			{
				return _TextFileGroup;
			}
			set
			{
				_TextFileGroup = value;
			}
		}

		internal DwTableDistributionType DwTableDistribution
		{
			get
			{
				return _DwTableDistribution;
			}
			set
			{
				_DwTableDistribution = value;
			}
		}

		internal string RejectedRowLocation
		{
			get
			{
				return _RejectedRowLocation;
			}
			set
			{
				_RejectedRowLocation = value;
			}
		}
	}

	private sealed class XRuntimeProps
	{
		private bool _ChangeTrackingEnabled;

		private DateTime _CreateDate;

		private double _DataSpaceUsed;

		private DateTime _DateLastModified;

		private DurabilityType _Durability;

		private bool _FakeSystemTable;

		private string _FileTableDirectoryName;

		private string _FileTableNameColumnCollation;

		private bool _FileTableNamespaceEnabled;

		private bool _HasAfterTrigger;

		private bool _HasClusteredColumnStoreIndex;

		private bool _HasClusteredIndex;

		private bool _HasCompressedPartitions;

		private bool _HasDeleteTrigger;

		private bool _HasHeapIndex;

		private bool _HasIndex;

		private bool _HasInsertTrigger;

		private bool _HasInsteadOfTrigger;

		private bool _HasNonClusteredColumnStoreIndex;

		private bool _HasNonClusteredIndex;

		private bool _HasPrimaryClusteredIndex;

		private bool _HasSparseColumn;

		private bool _HasSpatialData;

		private bool _HasSystemTimePeriod;

		private bool _HasUpdateTrigger;

		private bool _HasXmlData;

		private bool _HasXmlIndex;

		private int _HistoryTableID;

		private string _HistoryTableName;

		private string _HistoryTableSchema;

		private double _IndexSpaceUsed;

		private bool _IsEdge;

		private bool _IsFileTable;

		private bool _IsIndexable;

		private bool _IsMemoryOptimized;

		private bool _IsNode;

		private bool _IsSchemaOwned;

		private bool _IsSystemVersioned;

		private bool _IsVarDecimalStorageFormatEnabled;

		private LockEscalationType _LockEscalation;

		private string _Owner;

		private PolicyHealthState _PolicyHealthState;

		private RemoteDataArchiveMigrationState _RemoteDataArchiveDataMigrationState;

		private bool _RemoteDataArchiveEnabled;

		private string _RemoteDataArchiveFilterPredicate;

		private string _RemoteTableName;

		private bool _RemoteTableProvisioned;

		private bool _Replicated;

		private long _RowCount;

		private string _SystemTimePeriodEndColumn;

		private string _SystemTimePeriodStartColumn;

		private TableTemporalType _TemporalType;

		private bool _TrackColumnsUpdatedEnabled;

		private int _HistoryRetentionPeriod;

		private TemporalHistoryRetentionPeriodUnit _HistoryRetentionPeriodUnit;

		internal bool ChangeTrackingEnabled
		{
			get
			{
				return _ChangeTrackingEnabled;
			}
			set
			{
				_ChangeTrackingEnabled = value;
			}
		}

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

		internal double DataSpaceUsed
		{
			get
			{
				return _DataSpaceUsed;
			}
			set
			{
				_DataSpaceUsed = value;
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

		internal DurabilityType Durability
		{
			get
			{
				return _Durability;
			}
			set
			{
				_Durability = value;
			}
		}

		internal bool FakeSystemTable
		{
			get
			{
				return _FakeSystemTable;
			}
			set
			{
				_FakeSystemTable = value;
			}
		}

		internal string FileTableDirectoryName
		{
			get
			{
				return _FileTableDirectoryName;
			}
			set
			{
				_FileTableDirectoryName = value;
			}
		}

		internal string FileTableNameColumnCollation
		{
			get
			{
				return _FileTableNameColumnCollation;
			}
			set
			{
				_FileTableNameColumnCollation = value;
			}
		}

		internal bool FileTableNamespaceEnabled
		{
			get
			{
				return _FileTableNamespaceEnabled;
			}
			set
			{
				_FileTableNamespaceEnabled = value;
			}
		}

		internal bool HasAfterTrigger
		{
			get
			{
				return _HasAfterTrigger;
			}
			set
			{
				_HasAfterTrigger = value;
			}
		}

		internal bool HasClusteredColumnStoreIndex
		{
			get
			{
				return _HasClusteredColumnStoreIndex;
			}
			set
			{
				_HasClusteredColumnStoreIndex = value;
			}
		}

		internal bool HasClusteredIndex
		{
			get
			{
				return _HasClusteredIndex;
			}
			set
			{
				_HasClusteredIndex = value;
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

		internal bool HasDeleteTrigger
		{
			get
			{
				return _HasDeleteTrigger;
			}
			set
			{
				_HasDeleteTrigger = value;
			}
		}

		internal bool HasHeapIndex
		{
			get
			{
				return _HasHeapIndex;
			}
			set
			{
				_HasHeapIndex = value;
			}
		}

		internal bool HasIndex
		{
			get
			{
				return _HasIndex;
			}
			set
			{
				_HasIndex = value;
			}
		}

		internal bool HasInsertTrigger
		{
			get
			{
				return _HasInsertTrigger;
			}
			set
			{
				_HasInsertTrigger = value;
			}
		}

		internal bool HasInsteadOfTrigger
		{
			get
			{
				return _HasInsteadOfTrigger;
			}
			set
			{
				_HasInsteadOfTrigger = value;
			}
		}

		internal bool HasNonClusteredColumnStoreIndex
		{
			get
			{
				return _HasNonClusteredColumnStoreIndex;
			}
			set
			{
				_HasNonClusteredColumnStoreIndex = value;
			}
		}

		internal bool HasNonClusteredIndex
		{
			get
			{
				return _HasNonClusteredIndex;
			}
			set
			{
				_HasNonClusteredIndex = value;
			}
		}

		internal bool HasPrimaryClusteredIndex
		{
			get
			{
				return _HasPrimaryClusteredIndex;
			}
			set
			{
				_HasPrimaryClusteredIndex = value;
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

		internal bool HasSpatialData
		{
			get
			{
				return _HasSpatialData;
			}
			set
			{
				_HasSpatialData = value;
			}
		}

		internal bool HasSystemTimePeriod
		{
			get
			{
				return _HasSystemTimePeriod;
			}
			set
			{
				_HasSystemTimePeriod = value;
			}
		}

		internal bool HasUpdateTrigger
		{
			get
			{
				return _HasUpdateTrigger;
			}
			set
			{
				_HasUpdateTrigger = value;
			}
		}

		internal bool HasXmlData
		{
			get
			{
				return _HasXmlData;
			}
			set
			{
				_HasXmlData = value;
			}
		}

		internal bool HasXmlIndex
		{
			get
			{
				return _HasXmlIndex;
			}
			set
			{
				_HasXmlIndex = value;
			}
		}

		internal int HistoryTableID
		{
			get
			{
				return _HistoryTableID;
			}
			set
			{
				_HistoryTableID = value;
			}
		}

		internal string HistoryTableName
		{
			get
			{
				return _HistoryTableName;
			}
			set
			{
				_HistoryTableName = value;
			}
		}

		internal string HistoryTableSchema
		{
			get
			{
				return _HistoryTableSchema;
			}
			set
			{
				_HistoryTableSchema = value;
			}
		}

		internal double IndexSpaceUsed
		{
			get
			{
				return _IndexSpaceUsed;
			}
			set
			{
				_IndexSpaceUsed = value;
			}
		}

		internal bool IsEdge
		{
			get
			{
				return _IsEdge;
			}
			set
			{
				_IsEdge = value;
			}
		}

		internal bool IsFileTable
		{
			get
			{
				return _IsFileTable;
			}
			set
			{
				_IsFileTable = value;
			}
		}

		internal bool IsIndexable
		{
			get
			{
				return _IsIndexable;
			}
			set
			{
				_IsIndexable = value;
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

		internal bool IsSchemaOwned
		{
			get
			{
				return _IsSchemaOwned;
			}
			set
			{
				_IsSchemaOwned = value;
			}
		}

		internal bool IsSystemVersioned
		{
			get
			{
				return _IsSystemVersioned;
			}
			set
			{
				_IsSystemVersioned = value;
			}
		}

		internal bool IsVarDecimalStorageFormatEnabled
		{
			get
			{
				return _IsVarDecimalStorageFormatEnabled;
			}
			set
			{
				_IsVarDecimalStorageFormatEnabled = value;
			}
		}

		internal LockEscalationType LockEscalation
		{
			get
			{
				return _LockEscalation;
			}
			set
			{
				_LockEscalation = value;
			}
		}

		internal string Owner
		{
			get
			{
				return _Owner;
			}
			set
			{
				_Owner = value;
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

		internal RemoteDataArchiveMigrationState RemoteDataArchiveDataMigrationState
		{
			get
			{
				return _RemoteDataArchiveDataMigrationState;
			}
			set
			{
				_RemoteDataArchiveDataMigrationState = value;
			}
		}

		internal bool RemoteDataArchiveEnabled
		{
			get
			{
				return _RemoteDataArchiveEnabled;
			}
			set
			{
				_RemoteDataArchiveEnabled = value;
			}
		}

		internal string RemoteDataArchiveFilterPredicate
		{
			get
			{
				return _RemoteDataArchiveFilterPredicate;
			}
			set
			{
				_RemoteDataArchiveFilterPredicate = value;
			}
		}

		internal string RemoteTableName
		{
			get
			{
				return _RemoteTableName;
			}
			set
			{
				_RemoteTableName = value;
			}
		}

		internal bool RemoteTableProvisioned
		{
			get
			{
				return _RemoteTableProvisioned;
			}
			set
			{
				_RemoteTableProvisioned = value;
			}
		}

		internal bool Replicated
		{
			get
			{
				return _Replicated;
			}
			set
			{
				_Replicated = value;
			}
		}

		internal long RowCount
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

		internal string SystemTimePeriodEndColumn
		{
			get
			{
				return _SystemTimePeriodEndColumn;
			}
			set
			{
				_SystemTimePeriodEndColumn = value;
			}
		}

		internal string SystemTimePeriodStartColumn
		{
			get
			{
				return _SystemTimePeriodStartColumn;
			}
			set
			{
				_SystemTimePeriodStartColumn = value;
			}
		}

		internal TableTemporalType TemporalType
		{
			get
			{
				return _TemporalType;
			}
			set
			{
				_TemporalType = value;
			}
		}

		internal bool TrackColumnsUpdatedEnabled
		{
			get
			{
				return _TrackColumnsUpdatedEnabled;
			}
			set
			{
				_TrackColumnsUpdatedEnabled = value;
			}
		}

		internal int HistoryRetentionPeriod
		{
			get
			{
				return _HistoryRetentionPeriod;
			}
			set
			{
				_HistoryRetentionPeriod = value;
			}
		}

		internal TemporalHistoryRetentionPeriodUnit HistoryRetentionPeriodUnit
		{
			get
			{
				return _HistoryRetentionPeriodUnit;
			}
			set
			{
				_HistoryRetentionPeriodUnit = value;
			}
		}
	}

	private struct SystemTimePeriodInfo
	{
		public string m_StartColumnName;

		public string m_EndColumnName;

		public bool m_MarkedForCreate;

		public bool m_MarkedForDrop;

		public void MarkForCreate(string start, string end)
		{
			m_StartColumnName = start;
			m_EndColumnName = end;
			m_MarkedForCreate = true;
		}

		public void Reset()
		{
			m_StartColumnName = string.Empty;
			m_EndColumnName = string.Empty;
			m_MarkedForCreate = false;
		}

		public void MarkForDrop(bool drop)
		{
			m_MarkedForDrop = drop;
		}
	}

	private XSchemaProps _XSchema;

	private XRuntimeProps _XRuntime;

	private TableEvents events;

	private CheckCollection m_Checks;

	private ResumableIndexCollection m_ResumableIndexes;

	private bool m_OnlineHeapOperation;

	private int m_lowPriorityMaxDuration;

	private bool m_DataConsistencyCheckForSystemVersionedTable = true;

	private SystemTimePeriodInfo m_systemTimePeriodInfo = default(SystemTimePeriodInfo);

	private AbortAfterWait m_lowPriorityAbortAfterWait;

	private int m_MaximumDegreeOfParallelism = -1;

	private ForeignKeyCollection m_ForeignKeys;

	private PhysicalPartitionCollection m_PhysicalPartitions;

	private PartitionSchemeParameterCollection m_PartitionSchemeParameters;

	private List<Index> indexPropagationList;

	private List<SqlSmoObject> embeddedForeignKeyChecksList;

	private int rebuildPartitionNumber = -1;

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
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

	internal override UserPermissionCollection Permissions => GetUserPermissions();

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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool AnsiNullsStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullsStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullsStatus", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool ChangeTrackingEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ChangeTrackingEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ChangeTrackingEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[CLSCompliant(false)]
	[SfcReference(typeof(ExternalDataSource), "Server[@Name = '{0}']/Database[@Name = '{1}']/ExternalDataSource[@Name='{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "ExternalDataSource" })]
	public string DataSourceName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DataSourceName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataSourceName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double DataSpaceUsed => (double)base.Properties.GetValueWithNullReplacement("DataSpaceUsed");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DurabilityType Durability
	{
		get
		{
			return (DurabilityType)base.Properties.GetValueWithNullReplacement("Durability");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Durability", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ExternalTableDistributionType ExternalTableDistribution
	{
		get
		{
			return (ExternalTableDistributionType)base.Properties.GetValueWithNullReplacement("ExternalTableDistribution");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExternalTableDistribution", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool FakeSystemTable => (bool)base.Properties.GetValueWithNullReplacement("FakeSystemTable");

	[SfcReference(typeof(ExternalFileFormat), "Server[@Name = '{0}']/Database[@Name = '{1}']/ExternalFileFormat[@Name='{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "ExternalFileFormat" })]
	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	[CLSCompliant(false)]
	public string FileFormatName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileFormatName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileFormatName", value);
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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
	[SfcReference(typeof(PartitionScheme), "Server[@Name='{0}']/Database[@Name='{1}']/PartitionScheme[@Name='{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "FileStreamPartitionScheme" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FileTableDirectoryName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileTableDirectoryName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileTableDirectoryName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FileTableNameColumnCollation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileTableNameColumnCollation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileTableNameColumnCollation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool FileTableNamespaceEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("FileTableNamespaceEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileTableNamespaceEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasAfterTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasAfterTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasClusteredColumnStoreIndex => (bool)base.Properties.GetValueWithNullReplacement("HasClusteredColumnStoreIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasClusteredIndex => (bool)base.Properties.GetValueWithNullReplacement("HasClusteredIndex");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "false")]
	public bool HasCompressedPartitions => (bool)base.Properties.GetValueWithNullReplacement("HasCompressedPartitions");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasDeleteTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasDeleteTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasHeapIndex => (bool)base.Properties.GetValueWithNullReplacement("HasHeapIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasIndex => (bool)base.Properties.GetValueWithNullReplacement("HasIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasInsertTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasInsertTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasInsteadOfTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasInsteadOfTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasNonClusteredColumnStoreIndex => (bool)base.Properties.GetValueWithNullReplacement("HasNonClusteredColumnStoreIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasNonClusteredIndex => (bool)base.Properties.GetValueWithNullReplacement("HasNonClusteredIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasPrimaryClusteredIndex => (bool)base.Properties.GetValueWithNullReplacement("HasPrimaryClusteredIndex");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasSparseColumn => (bool)base.Properties.GetValueWithNullReplacement("HasSparseColumn");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasSpatialData => (bool)base.Properties.GetValueWithNullReplacement("HasSpatialData");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasSystemTimePeriod => (bool)base.Properties.GetValueWithNullReplacement("HasSystemTimePeriod");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasUpdateTrigger => (bool)base.Properties.GetValueWithNullReplacement("HasUpdateTrigger");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasXmlData => (bool)base.Properties.GetValueWithNullReplacement("HasXmlData");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasXmlIndex => (bool)base.Properties.GetValueWithNullReplacement("HasXmlIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int HistoryTableID => (int)base.Properties.GetValueWithNullReplacement("HistoryTableID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string HistoryTableName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("HistoryTableName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HistoryTableName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string HistoryTableSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("HistoryTableSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HistoryTableSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double IndexSpaceUsed => (double)base.Properties.GetValueWithNullReplacement("IndexSpaceUsed");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsExternal
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsExternal");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsExternal", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsFileTable
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsFileTable");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsFileTable", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsIndexable => (bool)base.Properties.GetValueWithNullReplacement("IsIndexable");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemVersioned
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSystemVersioned");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSystemVersioned", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Location
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Location");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Location", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public LockEscalationType LockEscalation
	{
		get
		{
			return (LockEscalationType)base.Properties.GetValueWithNullReplacement("LockEscalation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LockEscalation", value);
		}
	}

	[CLSCompliant(false)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	public string Owner
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Owner");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Owner", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[CLSCompliant(false)]
	[SfcReference(typeof(PartitionScheme), "Server[@Name='{0}']/Database[@Name='{1}']/PartitionScheme[@Name='{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "PartitionScheme" })]
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool QuotedIdentifierStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("QuotedIdentifierStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QuotedIdentifierStatus", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double RejectSampleValue
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("RejectSampleValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RejectSampleValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public ExternalTableRejectType RejectType
	{
		get
		{
			return (ExternalTableRejectType)base.Properties.GetValueWithNullReplacement("RejectType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RejectType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public double RejectValue
	{
		get
		{
			return (double)base.Properties.GetValueWithNullReplacement("RejectValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RejectValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public RemoteDataArchiveMigrationState RemoteDataArchiveDataMigrationState
	{
		get
		{
			return (RemoteDataArchiveMigrationState)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveDataMigrationState");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveDataMigrationState", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool RemoteDataArchiveEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string RemoteDataArchiveFilterPredicate
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteDataArchiveFilterPredicate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteDataArchiveFilterPredicate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string RemoteObjectName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteObjectName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteObjectName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string RemoteSchemaName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteSchemaName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteSchemaName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string RemoteTableName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RemoteTableName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteTableName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public bool RemoteTableProvisioned
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RemoteTableProvisioned");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RemoteTableProvisioned", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool Replicated => (bool)base.Properties.GetValueWithNullReplacement("Replicated");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public long RowCount => (long)base.Properties.GetValueWithNullReplacement("RowCount");

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ShardingColumnName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ShardingColumnName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ShardingColumnName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SystemTimePeriodEndColumn => (string)base.Properties.GetValueWithNullReplacement("SystemTimePeriodEndColumn");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SystemTimePeriodStartColumn => (string)base.Properties.GetValueWithNullReplacement("SystemTimePeriodStartColumn");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public TableTemporalType TemporalType => (TableTemporalType)base.Properties.GetValueWithNullReplacement("TemporalType");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TextFileGroup
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("TextFileGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TextFileGroup", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool TrackColumnsUpdatedEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("TrackColumnsUpdatedEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("TrackColumnsUpdatedEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public int HistoryRetentionPeriod
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("HistoryRetentionPeriod");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HistoryRetentionPeriod", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public TemporalHistoryRetentionPeriodUnit HistoryRetentionPeriodUnit
	{
		get
		{
			return (TemporalHistoryRetentionPeriodUnit)base.Properties.GetValueWithNullReplacement("HistoryRetentionPeriodUnit");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("HistoryRetentionPeriodUnit", value);
		}
	}

	[SfcProperty]
	public DwTableDistributionType DwTableDistribution
	{
		get
		{
			return (DwTableDistributionType)base.Properties.GetValueWithNullReplacement("DwTableDistribution");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DwTableDistribution", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.ReadOnlyAfterCreation)]
	public string RejectedRowLocation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RejectedRowLocation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RejectedRowLocation", value);
		}
	}

	public TableEvents Events
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
				events = new TableEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Table";

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Index), SfcObjectFlags.Design | SfcObjectFlags.Deploy)]
	public override IndexCollection Indexes => base.Indexes;

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Check), SfcObjectFlags.Design)]
	public CheckCollection Checks
	{
		get
		{
			CheckObjectState();
			if (m_Checks == null)
			{
				m_Checks = new CheckCollection(this);
			}
			return m_Checks;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Check), SfcObjectFlags.Design)]
	public ResumableIndexCollection ResumableIndexes
	{
		get
		{
			CheckObjectState();
			if (m_ResumableIndexes == null)
			{
				m_ResumableIndexes = new ResumableIndexCollection(this);
			}
			return m_ResumableIndexes;
		}
	}

	public bool OnlineHeapOperation
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion100();
			return m_OnlineHeapOperation;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion100();
			m_OnlineHeapOperation = value;
		}
	}

	public int LowPriorityMaxDuration
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			return m_lowPriorityMaxDuration;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			m_lowPriorityMaxDuration = value;
		}
	}

	public bool DataConsistencyCheck
	{
		get
		{
			CheckObjectState();
			if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				ThrowIfCloudAndVersionBelow12("DataConsistencyCheck");
			}
			else
			{
				ThrowIfBelowVersion130();
			}
			return m_DataConsistencyCheckForSystemVersionedTable;
		}
		set
		{
			CheckObjectState();
			if (DatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
			{
				ThrowIfCloudAndVersionBelow12("DataConsistencyCheck");
			}
			else
			{
				ThrowIfBelowVersion130();
			}
			m_DataConsistencyCheckForSystemVersionedTable = value;
		}
	}

	public AbortAfterWait LowPriorityAbortAfterWait
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			return m_lowPriorityAbortAfterWait;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion120();
			m_lowPriorityAbortAfterWait = value;
		}
	}

	public int MaximumDegreeOfParallelism
	{
		get
		{
			CheckObjectState();
			ThrowIfBelowVersion100();
			return m_MaximumDegreeOfParallelism;
		}
		set
		{
			CheckObjectState();
			ThrowIfBelowVersion100();
			m_MaximumDegreeOfParallelism = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsNode
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsNode");
		}
		set
		{
			ThrowIfBelowVersion140Prop("IsNode");
			base.Properties.SetValueWithConsistencyCheck("IsNode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsEdge
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEdge");
		}
		set
		{
			ThrowIfBelowVersion140Prop("IsNode");
			base.Properties.SetValueWithConsistencyCheck("IsEdge", value);
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ForeignKey), SfcObjectFlags.Design)]
	public ForeignKeyCollection ForeignKeys
	{
		get
		{
			CheckObjectState();
			if (m_ForeignKeys == null)
			{
				m_ForeignKeys = new ForeignKeyCollection(this);
			}
			return m_ForeignKeys;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.OneToAny, typeof(PhysicalPartition))]
	public PhysicalPartitionCollection PhysicalPartitions
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PhysicalPartition));
			if (m_PhysicalPartitions == null)
			{
				m_PhysicalPartitions = new PhysicalPartitionCollection(this);
			}
			return m_PhysicalPartitions;
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(PartitionSchemeParameter))]
	public PartitionSchemeParameterCollection PartitionSchemeParameters
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(PartitionSchemeParameter));
			if (m_PartitionSchemeParameters == null)
			{
				m_PartitionSchemeParameters = new PartitionSchemeParameterCollection(this);
				if (base.State == SqlSmoState.Existing)
				{
					m_PartitionSchemeParameters.LockCollection(ExceptionTemplatesImpl.ReasonObjectAlreadyCreated(UrnSuffix));
				}
			}
			return m_PartitionSchemeParameters;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public double RowCountAsDouble => Convert.ToDouble(base.Properties["RowCount"].Value, SmoApplication.DefaultCulture);

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool IsVarDecimalStorageFormatEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsVarDecimalStorageFormatEnabled");
		}
		set
		{
			if (!Parent.IsVarDecimalStorageFormatSupported)
			{
				throw new PropertyNotAvailableException(ExceptionTemplatesImpl.ReasonPropertyIsNotSupportedOnCurrentServerVersion);
			}
			base.Properties.SetValueWithConsistencyCheck("IsVarDecimalStorageFormatEnabled", value);
		}
	}

	public Table()
	{
	}

	public Table(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public Table(Database database, string name, string schema)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, schema);
		Parent = database;
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
					0 => XSchema.AnsiNullsStatus, 
					1 => XRuntime.CreateDate, 
					2 => XSchema.DataSourceName, 
					3 => XRuntime.DateLastModified, 
					4 => XRuntime.Durability, 
					5 => XSchema.DwTableDistribution, 
					6 => XRuntime.FakeSystemTable, 
					7 => XSchema.FileFormatName, 
					8 => XSchema.FileGroup, 
					9 => XRuntime.HasClusteredColumnStoreIndex, 
					10 => XRuntime.HasClusteredIndex, 
					11 => XRuntime.HasCompressedPartitions, 
					12 => XRuntime.HasHeapIndex, 
					13 => XRuntime.HasIndex, 
					14 => XRuntime.HasNonClusteredColumnStoreIndex, 
					15 => XRuntime.HasNonClusteredIndex, 
					16 => XRuntime.HasPrimaryClusteredIndex, 
					17 => XRuntime.HasSparseColumn, 
					18 => XRuntime.HasSpatialData, 
					19 => XRuntime.HasXmlData, 
					20 => XRuntime.HasXmlIndex, 
					21 => XSchema.ID, 
					22 => XRuntime.IsEdge, 
					23 => XSchema.IsExternal, 
					24 => XRuntime.IsIndexable, 
					25 => XRuntime.IsMemoryOptimized, 
					26 => XRuntime.IsNode, 
					27 => XSchema.IsPartitioned, 
					28 => XRuntime.IsSchemaOwned, 
					29 => XSchema.IsSystemObject, 
					30 => XSchema.Location, 
					31 => XRuntime.LockEscalation, 
					32 => XRuntime.Owner, 
					33 => XSchema.PartitionScheme, 
					34 => XSchema.QuotedIdentifierStatus, 
					35 => XSchema.RejectedRowLocation, 
					36 => XSchema.RejectSampleValue, 
					37 => XSchema.RejectType, 
					38 => XSchema.RejectValue, 
					39 => XRuntime.RowCount, 
					40 => XRuntime.TemporalType, 
					41 => XSchema.TextFileGroup, 
					_ => throw new IndexOutOfRangeException(), 
				};
			}
			return index switch
			{
				17 => XSchema.AnsiNullsStatus, 
				18 => XRuntime.ChangeTrackingEnabled, 
				0 => XRuntime.CreateDate, 
				19 => XSchema.DataSourceName, 
				1 => XRuntime.DateLastModified, 
				20 => XRuntime.Durability, 
				21 => XSchema.ExternalTableDistribution, 
				22 => XRuntime.FakeSystemTable, 
				23 => XSchema.FileGroup, 
				24 => XSchema.FileStreamFileGroup, 
				25 => XSchema.FileStreamPartitionScheme, 
				26 => XRuntime.FileTableDirectoryName, 
				27 => XRuntime.FileTableNameColumnCollation, 
				28 => XRuntime.FileTableNamespaceEnabled, 
				2 => XRuntime.HasAfterTrigger, 
				29 => XRuntime.HasClusteredColumnStoreIndex, 
				30 => XRuntime.HasClusteredIndex, 
				31 => XRuntime.HasCompressedPartitions, 
				3 => XRuntime.HasDeleteTrigger, 
				32 => XRuntime.HasHeapIndex, 
				4 => XRuntime.HasIndex, 
				5 => XRuntime.HasInsertTrigger, 
				6 => XRuntime.HasInsteadOfTrigger, 
				33 => XRuntime.HasNonClusteredColumnStoreIndex, 
				34 => XRuntime.HasNonClusteredIndex, 
				35 => XRuntime.HasPrimaryClusteredIndex, 
				7 => XRuntime.HasSparseColumn, 
				36 => XRuntime.HasSpatialData, 
				37 => XRuntime.HasSystemTimePeriod, 
				8 => XRuntime.HasUpdateTrigger, 
				38 => XRuntime.HasXmlData, 
				39 => XRuntime.HasXmlIndex, 
				40 => XRuntime.HistoryRetentionPeriod, 
				41 => XRuntime.HistoryRetentionPeriodUnit, 
				42 => XRuntime.HistoryTableID, 
				43 => XRuntime.HistoryTableName, 
				44 => XRuntime.HistoryTableSchema, 
				9 => XSchema.ID, 
				45 => XRuntime.IsEdge, 
				46 => XSchema.IsExternal, 
				47 => XRuntime.IsFileTable, 
				10 => XRuntime.IsIndexable, 
				48 => XRuntime.IsMemoryOptimized, 
				49 => XRuntime.IsNode, 
				50 => XSchema.IsPartitioned, 
				11 => XRuntime.IsSchemaOwned, 
				12 => XSchema.IsSystemObject, 
				51 => XRuntime.IsSystemVersioned, 
				13 => XRuntime.LockEscalation, 
				14 => XRuntime.Owner, 
				52 => XSchema.PartitionScheme, 
				53 => XSchema.QuotedIdentifierStatus, 
				54 => XSchema.RemoteObjectName, 
				55 => XSchema.RemoteSchemaName, 
				15 => XRuntime.Replicated, 
				16 => XRuntime.RowCount, 
				56 => XSchema.ShardingColumnName, 
				57 => XRuntime.SystemTimePeriodEndColumn, 
				58 => XRuntime.SystemTimePeriodStartColumn, 
				59 => XRuntime.TemporalType, 
				60 => XSchema.TextFileGroup, 
				61 => XRuntime.TrackColumnsUpdatedEnabled, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		return index switch
		{
			17 => XSchema.AnsiNullsStatus, 
			36 => XRuntime.ChangeTrackingEnabled, 
			0 => XRuntime.CreateDate, 
			50 => XSchema.DataSourceName, 
			1 => XRuntime.DataSpaceUsed, 
			26 => XRuntime.DateLastModified, 
			47 => XRuntime.Durability, 
			51 => XSchema.ExternalTableDistribution, 
			2 => XRuntime.FakeSystemTable, 
			52 => XSchema.FileFormatName, 
			3 => XSchema.FileGroup, 
			37 => XSchema.FileStreamFileGroup, 
			38 => XSchema.FileStreamPartitionScheme, 
			43 => XRuntime.FileTableDirectoryName, 
			44 => XRuntime.FileTableNameColumnCollation, 
			45 => XRuntime.FileTableNamespaceEnabled, 
			18 => XRuntime.HasAfterTrigger, 
			48 => XRuntime.HasClusteredColumnStoreIndex, 
			4 => XRuntime.HasClusteredIndex, 
			39 => XRuntime.HasCompressedPartitions, 
			19 => XRuntime.HasDeleteTrigger, 
			27 => XRuntime.HasHeapIndex, 
			20 => XRuntime.HasIndex, 
			21 => XRuntime.HasInsertTrigger, 
			22 => XRuntime.HasInsteadOfTrigger, 
			28 => XRuntime.HasNonClusteredColumnStoreIndex, 
			5 => XRuntime.HasNonClusteredIndex, 
			6 => XRuntime.HasPrimaryClusteredIndex, 
			7 => XRuntime.HasSparseColumn, 
			29 => XRuntime.HasSpatialData, 
			53 => XRuntime.HasSystemTimePeriod, 
			23 => XRuntime.HasUpdateTrigger, 
			30 => XRuntime.HasXmlData, 
			31 => XRuntime.HasXmlIndex, 
			54 => XRuntime.HistoryTableID, 
			55 => XRuntime.HistoryTableName, 
			56 => XRuntime.HistoryTableSchema, 
			8 => XSchema.ID, 
			9 => XRuntime.IndexSpaceUsed, 
			10 => XRuntime.IsEdge, 
			57 => XSchema.IsExternal, 
			46 => XRuntime.IsFileTable, 
			24 => XRuntime.IsIndexable, 
			49 => XRuntime.IsMemoryOptimized, 
			11 => XRuntime.IsNode, 
			32 => XSchema.IsPartitioned, 
			33 => XRuntime.IsSchemaOwned, 
			12 => XSchema.IsSystemObject, 
			58 => XRuntime.IsSystemVersioned, 
			34 => XRuntime.IsVarDecimalStorageFormatEnabled, 
			59 => XSchema.Location, 
			40 => XRuntime.LockEscalation, 
			13 => XRuntime.Owner, 
			35 => XSchema.PartitionScheme, 
			41 => XRuntime.PolicyHealthState, 
			25 => XSchema.QuotedIdentifierStatus, 
			60 => XSchema.RejectSampleValue, 
			61 => XSchema.RejectType, 
			62 => XSchema.RejectValue, 
			63 => XRuntime.RemoteDataArchiveDataMigrationState, 
			64 => XRuntime.RemoteDataArchiveEnabled, 
			65 => XRuntime.RemoteDataArchiveFilterPredicate, 
			66 => XSchema.RemoteObjectName, 
			67 => XSchema.RemoteSchemaName, 
			68 => XRuntime.RemoteTableName, 
			69 => XRuntime.RemoteTableProvisioned, 
			14 => XRuntime.Replicated, 
			15 => XRuntime.RowCount, 
			70 => XSchema.ShardingColumnName, 
			71 => XRuntime.SystemTimePeriodEndColumn, 
			72 => XRuntime.SystemTimePeriodStartColumn, 
			73 => XRuntime.TemporalType, 
			16 => XSchema.TextFileGroup, 
			42 => XRuntime.TrackColumnsUpdatedEnabled, 
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
					XSchema.AnsiNullsStatus = (bool)value;
					break;
				case 1:
					XRuntime.CreateDate = (DateTime)value;
					break;
				case 2:
					XSchema.DataSourceName = (string)value;
					break;
				case 3:
					XRuntime.DateLastModified = (DateTime)value;
					break;
				case 4:
					XRuntime.Durability = (DurabilityType)value;
					break;
				case 5:
					XSchema.DwTableDistribution = (DwTableDistributionType)value;
					break;
				case 6:
					XRuntime.FakeSystemTable = (bool)value;
					break;
				case 7:
					XSchema.FileFormatName = (string)value;
					break;
				case 8:
					XSchema.FileGroup = (string)value;
					break;
				case 9:
					XRuntime.HasClusteredColumnStoreIndex = (bool)value;
					break;
				case 10:
					XRuntime.HasClusteredIndex = (bool)value;
					break;
				case 11:
					XRuntime.HasCompressedPartitions = (bool)value;
					break;
				case 12:
					XRuntime.HasHeapIndex = (bool)value;
					break;
				case 13:
					XRuntime.HasIndex = (bool)value;
					break;
				case 14:
					XRuntime.HasNonClusteredColumnStoreIndex = (bool)value;
					break;
				case 15:
					XRuntime.HasNonClusteredIndex = (bool)value;
					break;
				case 16:
					XRuntime.HasPrimaryClusteredIndex = (bool)value;
					break;
				case 17:
					XRuntime.HasSparseColumn = (bool)value;
					break;
				case 18:
					XRuntime.HasSpatialData = (bool)value;
					break;
				case 19:
					XRuntime.HasXmlData = (bool)value;
					break;
				case 20:
					XRuntime.HasXmlIndex = (bool)value;
					break;
				case 21:
					XSchema.ID = (int)value;
					break;
				case 22:
					XRuntime.IsEdge = (bool)value;
					break;
				case 23:
					XSchema.IsExternal = (bool)value;
					break;
				case 24:
					XRuntime.IsIndexable = (bool)value;
					break;
				case 25:
					XRuntime.IsMemoryOptimized = (bool)value;
					break;
				case 26:
					XRuntime.IsNode = (bool)value;
					break;
				case 27:
					XSchema.IsPartitioned = (bool)value;
					break;
				case 28:
					XRuntime.IsSchemaOwned = (bool)value;
					break;
				case 29:
					XSchema.IsSystemObject = (bool)value;
					break;
				case 30:
					XSchema.Location = (string)value;
					break;
				case 31:
					XRuntime.LockEscalation = (LockEscalationType)value;
					break;
				case 32:
					XRuntime.Owner = (string)value;
					break;
				case 33:
					XSchema.PartitionScheme = (string)value;
					break;
				case 34:
					XSchema.QuotedIdentifierStatus = (bool)value;
					break;
				case 35:
					XSchema.RejectedRowLocation = (string)value;
					break;
				case 36:
					XSchema.RejectSampleValue = (double)value;
					break;
				case 37:
					XSchema.RejectType = (ExternalTableRejectType)value;
					break;
				case 38:
					XSchema.RejectValue = (double)value;
					break;
				case 39:
					XRuntime.RowCount = (long)value;
					break;
				case 40:
					XRuntime.TemporalType = (TableTemporalType)value;
					break;
				case 41:
					XSchema.TextFileGroup = (string)value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return;
			}
			switch (index)
			{
			case 17:
				XSchema.AnsiNullsStatus = (bool)value;
				break;
			case 18:
				XRuntime.ChangeTrackingEnabled = (bool)value;
				break;
			case 0:
				XRuntime.CreateDate = (DateTime)value;
				break;
			case 19:
				XSchema.DataSourceName = (string)value;
				break;
			case 1:
				XRuntime.DateLastModified = (DateTime)value;
				break;
			case 20:
				XRuntime.Durability = (DurabilityType)value;
				break;
			case 21:
				XSchema.ExternalTableDistribution = (ExternalTableDistributionType)value;
				break;
			case 22:
				XRuntime.FakeSystemTable = (bool)value;
				break;
			case 23:
				XSchema.FileGroup = (string)value;
				break;
			case 24:
				XSchema.FileStreamFileGroup = (string)value;
				break;
			case 25:
				XSchema.FileStreamPartitionScheme = (string)value;
				break;
			case 26:
				XRuntime.FileTableDirectoryName = (string)value;
				break;
			case 27:
				XRuntime.FileTableNameColumnCollation = (string)value;
				break;
			case 28:
				XRuntime.FileTableNamespaceEnabled = (bool)value;
				break;
			case 2:
				XRuntime.HasAfterTrigger = (bool)value;
				break;
			case 29:
				XRuntime.HasClusteredColumnStoreIndex = (bool)value;
				break;
			case 30:
				XRuntime.HasClusteredIndex = (bool)value;
				break;
			case 31:
				XRuntime.HasCompressedPartitions = (bool)value;
				break;
			case 3:
				XRuntime.HasDeleteTrigger = (bool)value;
				break;
			case 32:
				XRuntime.HasHeapIndex = (bool)value;
				break;
			case 4:
				XRuntime.HasIndex = (bool)value;
				break;
			case 5:
				XRuntime.HasInsertTrigger = (bool)value;
				break;
			case 6:
				XRuntime.HasInsteadOfTrigger = (bool)value;
				break;
			case 33:
				XRuntime.HasNonClusteredColumnStoreIndex = (bool)value;
				break;
			case 34:
				XRuntime.HasNonClusteredIndex = (bool)value;
				break;
			case 35:
				XRuntime.HasPrimaryClusteredIndex = (bool)value;
				break;
			case 7:
				XRuntime.HasSparseColumn = (bool)value;
				break;
			case 36:
				XRuntime.HasSpatialData = (bool)value;
				break;
			case 37:
				XRuntime.HasSystemTimePeriod = (bool)value;
				break;
			case 8:
				XRuntime.HasUpdateTrigger = (bool)value;
				break;
			case 38:
				XRuntime.HasXmlData = (bool)value;
				break;
			case 39:
				XRuntime.HasXmlIndex = (bool)value;
				break;
			case 40:
				XRuntime.HistoryRetentionPeriod = (int)value;
				break;
			case 41:
				XRuntime.HistoryRetentionPeriodUnit = (TemporalHistoryRetentionPeriodUnit)value;
				break;
			case 42:
				XRuntime.HistoryTableID = (int)value;
				break;
			case 43:
				XRuntime.HistoryTableName = (string)value;
				break;
			case 44:
				XRuntime.HistoryTableSchema = (string)value;
				break;
			case 9:
				XSchema.ID = (int)value;
				break;
			case 45:
				XRuntime.IsEdge = (bool)value;
				break;
			case 46:
				XSchema.IsExternal = (bool)value;
				break;
			case 47:
				XRuntime.IsFileTable = (bool)value;
				break;
			case 10:
				XRuntime.IsIndexable = (bool)value;
				break;
			case 48:
				XRuntime.IsMemoryOptimized = (bool)value;
				break;
			case 49:
				XRuntime.IsNode = (bool)value;
				break;
			case 50:
				XSchema.IsPartitioned = (bool)value;
				break;
			case 11:
				XRuntime.IsSchemaOwned = (bool)value;
				break;
			case 12:
				XSchema.IsSystemObject = (bool)value;
				break;
			case 51:
				XRuntime.IsSystemVersioned = (bool)value;
				break;
			case 13:
				XRuntime.LockEscalation = (LockEscalationType)value;
				break;
			case 14:
				XRuntime.Owner = (string)value;
				break;
			case 52:
				XSchema.PartitionScheme = (string)value;
				break;
			case 53:
				XSchema.QuotedIdentifierStatus = (bool)value;
				break;
			case 54:
				XSchema.RemoteObjectName = (string)value;
				break;
			case 55:
				XSchema.RemoteSchemaName = (string)value;
				break;
			case 15:
				XRuntime.Replicated = (bool)value;
				break;
			case 16:
				XRuntime.RowCount = (long)value;
				break;
			case 56:
				XSchema.ShardingColumnName = (string)value;
				break;
			case 57:
				XRuntime.SystemTimePeriodEndColumn = (string)value;
				break;
			case 58:
				XRuntime.SystemTimePeriodStartColumn = (string)value;
				break;
			case 59:
				XRuntime.TemporalType = (TableTemporalType)value;
				break;
			case 60:
				XSchema.TextFileGroup = (string)value;
				break;
			case 61:
				XRuntime.TrackColumnsUpdatedEnabled = (bool)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
		else
		{
			switch (index)
			{
			case 17:
				XSchema.AnsiNullsStatus = (bool)value;
				break;
			case 36:
				XRuntime.ChangeTrackingEnabled = (bool)value;
				break;
			case 0:
				XRuntime.CreateDate = (DateTime)value;
				break;
			case 50:
				XSchema.DataSourceName = (string)value;
				break;
			case 1:
				XRuntime.DataSpaceUsed = (double)value;
				break;
			case 26:
				XRuntime.DateLastModified = (DateTime)value;
				break;
			case 47:
				XRuntime.Durability = (DurabilityType)value;
				break;
			case 51:
				XSchema.ExternalTableDistribution = (ExternalTableDistributionType)value;
				break;
			case 2:
				XRuntime.FakeSystemTable = (bool)value;
				break;
			case 52:
				XSchema.FileFormatName = (string)value;
				break;
			case 3:
				XSchema.FileGroup = (string)value;
				break;
			case 37:
				XSchema.FileStreamFileGroup = (string)value;
				break;
			case 38:
				XSchema.FileStreamPartitionScheme = (string)value;
				break;
			case 43:
				XRuntime.FileTableDirectoryName = (string)value;
				break;
			case 44:
				XRuntime.FileTableNameColumnCollation = (string)value;
				break;
			case 45:
				XRuntime.FileTableNamespaceEnabled = (bool)value;
				break;
			case 18:
				XRuntime.HasAfterTrigger = (bool)value;
				break;
			case 48:
				XRuntime.HasClusteredColumnStoreIndex = (bool)value;
				break;
			case 4:
				XRuntime.HasClusteredIndex = (bool)value;
				break;
			case 39:
				XRuntime.HasCompressedPartitions = (bool)value;
				break;
			case 19:
				XRuntime.HasDeleteTrigger = (bool)value;
				break;
			case 27:
				XRuntime.HasHeapIndex = (bool)value;
				break;
			case 20:
				XRuntime.HasIndex = (bool)value;
				break;
			case 21:
				XRuntime.HasInsertTrigger = (bool)value;
				break;
			case 22:
				XRuntime.HasInsteadOfTrigger = (bool)value;
				break;
			case 28:
				XRuntime.HasNonClusteredColumnStoreIndex = (bool)value;
				break;
			case 5:
				XRuntime.HasNonClusteredIndex = (bool)value;
				break;
			case 6:
				XRuntime.HasPrimaryClusteredIndex = (bool)value;
				break;
			case 7:
				XRuntime.HasSparseColumn = (bool)value;
				break;
			case 29:
				XRuntime.HasSpatialData = (bool)value;
				break;
			case 53:
				XRuntime.HasSystemTimePeriod = (bool)value;
				break;
			case 23:
				XRuntime.HasUpdateTrigger = (bool)value;
				break;
			case 30:
				XRuntime.HasXmlData = (bool)value;
				break;
			case 31:
				XRuntime.HasXmlIndex = (bool)value;
				break;
			case 54:
				XRuntime.HistoryTableID = (int)value;
				break;
			case 55:
				XRuntime.HistoryTableName = (string)value;
				break;
			case 56:
				XRuntime.HistoryTableSchema = (string)value;
				break;
			case 8:
				XSchema.ID = (int)value;
				break;
			case 9:
				XRuntime.IndexSpaceUsed = (double)value;
				break;
			case 10:
				XRuntime.IsEdge = (bool)value;
				break;
			case 57:
				XSchema.IsExternal = (bool)value;
				break;
			case 46:
				XRuntime.IsFileTable = (bool)value;
				break;
			case 24:
				XRuntime.IsIndexable = (bool)value;
				break;
			case 49:
				XRuntime.IsMemoryOptimized = (bool)value;
				break;
			case 11:
				XRuntime.IsNode = (bool)value;
				break;
			case 32:
				XSchema.IsPartitioned = (bool)value;
				break;
			case 33:
				XRuntime.IsSchemaOwned = (bool)value;
				break;
			case 12:
				XSchema.IsSystemObject = (bool)value;
				break;
			case 58:
				XRuntime.IsSystemVersioned = (bool)value;
				break;
			case 34:
				XRuntime.IsVarDecimalStorageFormatEnabled = (bool)value;
				break;
			case 59:
				XSchema.Location = (string)value;
				break;
			case 40:
				XRuntime.LockEscalation = (LockEscalationType)value;
				break;
			case 13:
				XRuntime.Owner = (string)value;
				break;
			case 35:
				XSchema.PartitionScheme = (string)value;
				break;
			case 41:
				XRuntime.PolicyHealthState = (PolicyHealthState)value;
				break;
			case 25:
				XSchema.QuotedIdentifierStatus = (bool)value;
				break;
			case 60:
				XSchema.RejectSampleValue = (double)value;
				break;
			case 61:
				XSchema.RejectType = (ExternalTableRejectType)value;
				break;
			case 62:
				XSchema.RejectValue = (double)value;
				break;
			case 63:
				XRuntime.RemoteDataArchiveDataMigrationState = (RemoteDataArchiveMigrationState)value;
				break;
			case 64:
				XRuntime.RemoteDataArchiveEnabled = (bool)value;
				break;
			case 65:
				XRuntime.RemoteDataArchiveFilterPredicate = (string)value;
				break;
			case 66:
				XSchema.RemoteObjectName = (string)value;
				break;
			case 67:
				XSchema.RemoteSchemaName = (string)value;
				break;
			case 68:
				XRuntime.RemoteTableName = (string)value;
				break;
			case 69:
				XRuntime.RemoteTableProvisioned = (bool)value;
				break;
			case 14:
				XRuntime.Replicated = (bool)value;
				break;
			case 15:
				XRuntime.RowCount = (long)value;
				break;
			case 70:
				XSchema.ShardingColumnName = (string)value;
				break;
			case 71:
				XRuntime.SystemTimePeriodEndColumn = (string)value;
				break;
			case 72:
				XRuntime.SystemTimePeriodStartColumn = (string)value;
				break;
			case 73:
				XRuntime.TemporalType = (TableTemporalType)value;
				break;
			case 16:
				XSchema.TextFileGroup = (string)value;
				break;
			case 42:
				XRuntime.TrackColumnsUpdatedEnabled = (bool)value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[14]
		{
			"AnsiNullsStatus", "Durability", "FileGroup", "FileTableNameColumnCollation", "IsEdge", "IsFileTable", "IsMemoryOptimized", "IsNode", "PartitionScheme", "QuotedIdentifierStatus",
			"RemoteTableName", "RemoteTableProvisioned", "TextFileGroup", "RejectedRowLocation"
		};
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "HasCompressedPartitions")
		{
			return false;
		}
		return base.GetPropertyDefaultValue(propname);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, permissions);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, columnNames, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, columnNames, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, columnNames, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, columnNames, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, string[] columnNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, columnNames, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, null, null);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumColumnPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Column, this, granteeName, permissions);
	}

	internal Table(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		Init();
	}

	public void ChangeSchema(string newSchema)
	{
		CheckObjectState();
		ChangeSchema(newSchema, bCheckExisting: true);
	}

	private void Init()
	{
		m_Checks = null;
		m_ForeignKeys = null;
		m_PartitionSchemeParameters = null;
		m_PhysicalPartitions = null;
	}

	public string GetFileTableNamespacePath()
	{
		if (!IsFileTable)
		{
			throw new InvalidOperationException(ExceptionTemplatesImpl.TableNotFileTable(Name));
		}
		if (!FileTableNamespaceEnabled)
		{
			throw new InvalidOperationException(ExceptionTemplatesImpl.NamespaceNotEnabled(Name));
		}
		string netName = GetServerObject().NetName;
		string filestreamShareName = GetServerObject().FilestreamShareName;
		string filestreamDirectoryName = Parent.FilestreamDirectoryName;
		string fileTableDirectoryName = FileTableDirectoryName;
		string path = string.Format(CultureInfo.InvariantCulture, "\\\\{0}\\{1}\\{2}\\{3}", netName, filestreamShareName, filestreamDirectoryName, fileTableDirectoryName);
		return Path.GetFullPath(path);
	}

	internal bool IsDirty(string property)
	{
		return base.Properties.IsDirty(base.Properties.LookupID(property, PropertyAccessPurpose.Read));
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ValidateName(string name)
	{
		base.ValidateName(name);
		CheckTableName(name);
	}

	internal void AddToIndexPropagationList(Index i)
	{
		if (indexPropagationList == null)
		{
			indexPropagationList = new List<Index>();
		}
		indexPropagationList.Add(i);
	}

	internal void AddToEmbeddedForeignKeyChecksList(SqlSmoObject fkck)
	{
		if (embeddedForeignKeyChecksList == null)
		{
			embeddedForeignKeyChecksList = new List<SqlSmoObject>();
		}
		embeddedForeignKeyChecksList.Add(fkck);
	}

	internal IEnumerable<string> ScriptDataInternal(ScriptingPreferences sp)
	{
		return new DataScriptCollection(this, sp);
	}

	internal StringCollection ScriptDropData(ScriptingPreferences sp)
	{
		string value = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0}", new object[1] { FormatFullNameForScripting(sp) });
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(value);
		return stringCollection;
	}

	public IEnumerable<string> EnumScript()
	{
		return EnumScriptImpl(new ScriptingPreferences(this));
	}

	public IEnumerable<string> EnumScript(ScriptingOptions scriptingOptions)
	{
		if (scriptingOptions == null)
		{
			throw new ArgumentNullException("scriptingOptions");
		}
		Scripter scripter = new Scripter(GetServerObject());
		if (!scriptingOptions.GetScriptingPreferences().TargetVersionAndDatabaseEngineTypeDirty)
		{
			scriptingOptions.SetTargetServerInfo(this, forced: false);
		}
		scripter.Options = scriptingOptions;
		return scripter.EnumScript(this);
	}

	public void InitPhysicalPartitions()
	{
		InitChildLevel("PhysicalPartition", new ScriptingPreferences(), forScripting: false);
	}

	public void InitIndexes()
	{
		InitChildLevel("Index", new ScriptingPreferences(), forScripting: false);
	}

	public void InitColumns()
	{
		InitChildLevel("Column", new ScriptingPreferences(), forScripting: false);
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (sp == null)
		{
			throw new ArgumentNullException("Scripting preferences cannot be null.");
		}
		this.ThrowIfNotSupported(GetType(), sp);
		ValidateExternalTable();
		ValidateIndexes();
		bool flag = false;
		if (IsSupportedProperty("HasSystemTimePeriod"))
		{
			ValidateSystemTimeTemporal();
			flag = ((base.State != SqlSmoState.Existing) ? m_systemTimePeriodInfo.m_MarkedForCreate : HasSystemTimePeriod);
		}
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.OldOptions.PrimaryObject)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				Server serverObject = GetServerObject();
				flag4 = serverObject.UserOptions.AnsiPadding;
			}
			if (Parent.State == SqlSmoState.Existing && base.ServerVersion.Major > 7)
			{
				flag4 = Parent.DatabaseOptions.AnsiPaddingEnabled;
			}
			if (SqlServerVersionInternal.Version80 <= sp.TargetServerVersionInternal && base.ServerVersion.Major > 7)
			{
				if (null != base.Properties.Get("AnsiNullsStatus").Value)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_ANSI_NULLS, new object[1] { ((bool)base.Properties["AnsiNullsStatus"].Value) ? Globals.On : Globals.Off });
					stringCollection.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
				}
				if (null != base.Properties.Get("QuotedIdentifierStatus").Value)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_QUOTED_IDENTIFIER, new object[1] { ((bool)base.Properties["QuotedIdentifierStatus"].Value) ? Globals.On : Globals.Off });
					stringCollection.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
				}
			}
			bool? flag5 = GetTableAnsiPadded();
			bool flag6 = false;
			if (indexPropagationList != null)
			{
				foreach (Index indexPropagation in indexPropagationList)
				{
					foreach (IndexedColumn indexedColumn in indexPropagation.IndexedColumns)
					{
						if (indexedColumn.State != SqlSmoState.Creating && indexedColumn.IsComputed)
						{
							flag6 = true;
							break;
						}
					}
					if (flag6)
					{
						break;
					}
				}
			}
			if (flag6)
			{
				flag5 = true;
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SET ARITHABORT ON"));
			}
			if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && sp.IncludeScripts.AnsiPadding && flag5.HasValue)
			{
				if (flag5 == true)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SET ANSI_PADDING ON"));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SET ANSI_PADDING OFF"));
				}
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
					{
						"NOT",
						SqlSmoObject.SqlString(text)
					});
				}
				else
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE80, new object[2]
					{
						"NOT",
						SqlSmoObject.SqlString(text)
					});
				}
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(Scripts.BEGIN);
				stringBuilder.Append(sp.NewLine);
			}
			bool flag7 = CheckIsExternalTable();
			if (!flag7)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE TABLE {0}", new object[1] { text });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE EXTERNAL TABLE {0}", new object[1] { text });
			}
			bool propValueIfSupportedWithThrowOnTarget = GetPropValueIfSupportedWithThrowOnTarget("IsEdge", defaultValue: false, sp);
			bool flag8 = CheckIsSqlDwTable();
			bool flag9 = CheckIsMemoryOptimizedTable();
			bool flag10 = false;
			if (IsSupportedProperty("IsSystemVersioned", sp))
			{
				flag10 = GetPropValueOptional("IsSystemVersioned", defaultValue: false);
			}
			string text2 = string.Empty;
			if (IsSupportedProperty("HistoryTableName", sp))
			{
				text2 = GetPropValueOptional("HistoryTableName", string.Empty);
			}
			string text3 = string.Empty;
			if (IsSupportedProperty("HistoryTableSchema", sp))
			{
				text3 = GetPropValueOptional("HistoryTableSchema", string.Empty);
			}
			if (flag10 && !flag)
			{
				throw new SmoException(ExceptionTemplatesImpl.SystemVersionedTableWithoutPeriod);
			}
			if (!flag10)
			{
				if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text2.Trim()))
				{
					throw new SmoException(ExceptionTemplatesImpl.HistoryTableWithoutSystemVersioning);
				}
				if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text3.Trim()))
				{
					throw new SmoException(ExceptionTemplatesImpl.HistoryTableWithoutSystemVersioning);
				}
			}
			bool flag11 = false;
			if (!flag9 && !flag7 && !flag8 && IsSupportedProperty("IsFileTable") && GetPropValueOptional("IsFileTable", defaultValue: false))
			{
				if (!IsSupportedProperty("IsFileTable", sp))
				{
					throw new SmoException(ExceptionTemplatesImpl.FileTableNotSupportedOnTargetEngine(SqlSmoObject.GetSqlServerName(sp)));
				}
				if (base.Columns.Count > 0 && base.State == SqlSmoState.Creating)
				{
					throw new SmoException(ExceptionTemplatesImpl.FileTableCannotHaveUserColumns);
				}
				if (flag10 || flag)
				{
					throw new SmoException(ExceptionTemplatesImpl.NoTemporalFileTables);
				}
				flag11 = true;
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " AS FILETABLE");
				GetFileTableCreationScript(sp, stringBuilder);
			}
			if (!flag11)
			{
				if (base.Columns.Count < 1 && !propValueIfSupportedWithThrowOnTarget)
				{
					throw new SmoException(ExceptionTemplatesImpl.ObjectWithNoChildren("Table", "Column"));
				}
				if (sp.IncludeScripts.AnsiPadding && HasMultiplePaddings())
				{
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(flag5.HasValue);
					GetTableCreationScriptWithAnsiPadding(sp, stringBuilder, flag5.Value);
				}
				else if (flag7)
				{
					GetExternalTableCreationScript(sp, stringBuilder);
				}
				else if (flag9)
				{
					GetMemoryOptimizedTableCreationScript(sp, stringBuilder);
				}
				else if (flag8)
				{
					GetSqlDwTableCreationScript(sp, stringBuilder);
				}
				else
				{
					GetTableCreationScript(sp, stringBuilder);
				}
			}
			stringBuilder.Append(sp.NewLine);
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.Append(Scripts.END);
			}
			stringCollection.Add(stringBuilder.ToString());
			stringBuilder.Length = 0;
			bool forCreate = true;
			ScriptVardecimalCompression(stringCollection, sp, forCreate);
			if (IsSupportedProperty("ChangeTrackingEnabled", sp) && sp.Data.ChangeTracking)
			{
				if (GetPropValueOptional("ChangeTrackingEnabled", defaultValue: false) && CheckIsExternalTable())
				{
					throw new SmoException(ExceptionTemplatesImpl.ChangeTrackingNotSupportedOnExternalTables);
				}
				ScriptChangeTracking(stringCollection, sp);
			}
			if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && sp.IncludeScripts.AnsiPadding && flag5.HasValue)
			{
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "SET ANSI_PADDING {0}", new object[1] { flag4 ? "ON" : "OFF" }));
			}
			if (sp.Data.OptimizerData && base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && !flag9 && !flag8)
			{
				ScriptUpdateStatistics(sp, stringCollection, stringBuilder);
			}
			if (!sp.IncludeScripts.Data)
			{
				ScriptBindings(stringCollection, sp);
			}
			if (IsSupportedProperty("LockEscalation", sp))
			{
				ScriptLockEscalationSettings(stringCollection, sp);
			}
			ScriptAlterFileTableProp(stringCollection, sp);
			if (!flag9 && sp.IncludeScripts.Owner)
			{
				ScriptOwner(stringCollection, sp);
			}
			if (IsSupportedProperty("RemoteDataArchiveEnabled", sp))
			{
				ScriptRemoteDataArchive(stringCollection, sp);
			}
		}
		indexPropagationList = null;
		embeddedForeignKeyChecksList = null;
		StringEnumerator enumerator3 = stringCollection.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				string current2 = enumerator3.Current;
				queries.Add(current2);
			}
		}
		finally
		{
			if (enumerator3 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void GetMemoryOptimizedTableCreationScript(ScriptingPreferences sp, StringBuilder sb)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		ThrowIfPropertyNotSupported("IsMemoryOptimized", sp);
		sb.Append(sp.NewLine);
		ScriptColumns(sp, sb, base.Columns);
		if (IsSupportedProperty("HasSystemTimePeriod", sp))
		{
			ScriptPeriodForSystemTime(sb);
		}
		sb.Append(Globals.comma);
		sb.Append(sp.NewLine);
		GenerateMemoryOptimizedIndexes(sb, sp, Indexes);
		sb.Append(sp.NewLine);
		sb.Append(Globals.RParen);
		string text = GenerateSystemVersioningWithClauseContent(sp);
		if (!string.IsNullOrEmpty(text))
		{
			sb.AppendFormat(Scripts.WITH_MEMORY_OPTIMIZED_AND_DURABILITY_AND_TEMPORAL_SYSTEM_VERSIONING, (DurabilityTypeMap)Durability, text);
		}
		else
		{
			sb.AppendFormat(Scripts.WITH_MEMORY_OPTIMIZED_AND_DURABILITY, (DurabilityTypeMap)Durability);
		}
	}

	private void GetExternalTableCreationScript(ScriptingPreferences sp, StringBuilder sb)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		ThrowIfPropertyNotSupported("IsExternal", sp);
		ValidateExternalTableRequiredStringProperty("DataSourceName", sp);
		string s = GetPropertyOptional("DataSourceName").Value.ToString();
		sb.Append(sp.NewLine);
		ScriptColumns(sp, sb, base.Columns);
		sb.Append(sp.NewLine);
		sb.Append(Globals.RParen);
		sb.Append(sp.NewLine);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.EXTERNAL_DATASOURCE_NAME, new object[1] { SqlSmoObject.MakeSqlBraket(s) });
		ValidateExternalTableOptionalProperties(sp);
		ProcessExternalTableOptionalProperties(stringBuilder, sp);
		if (stringBuilder.Length > 0)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "WITH ({0})", new object[1] { stringBuilder.ToString() });
		}
	}

	private void GetSqlDwTableCreationScript(ScriptingPreferences sp, StringBuilder sb)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		ThrowIfPropertyNotSupported("DwTableDistribution", sp);
		sb.Append(sp.NewLine);
		ScriptColumns(sp, sb, base.Columns);
		sb.Append(sp.NewLine);
		sb.Append(Globals.RParen);
		sb.Append(sp.NewLine);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ProcessSqlDwTableProperties(stringBuilder, sp);
		if (stringBuilder.Length > 0)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "WITH");
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.newline);
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.newline);
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { stringBuilder.ToString() });
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.newline);
			sb.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
		}
	}

	private void ScriptUpdateStatistics(ScriptingPreferences sp, StringCollection scqueries, StringBuilder sb)
	{
		bool flag = false;
		foreach (Index index in Indexes)
		{
			if (index.GetPropValueOptional("IsClustered", defaultValue: false))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		string query = string.Format(SmoApplication.DefaultCulture, "DBCC SHOW_STATISTICS(N'{0}.{1}') WITH STATS_STREAM", new object[2]
		{
			SqlSmoObject.SqlString(Parent.FullQualifiedName),
			SqlSmoObject.SqlString(FullQualifiedName)
		});
		DataSet dataSet = null;
		SqlExecutionModes sqlExecutionModes = ExecutionManager.ConnectionContext.SqlExecutionModes;
		ExecutionManager.ConnectionContext.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
		try
		{
			dataSet = ExecutionManager.ExecuteWithResults(query);
		}
		finally
		{
			ExecutionManager.ConnectionContext.SqlExecutionModes = sqlExecutionModes;
		}
		if (dataSet.Tables.Count <= 0)
		{
			return;
		}
		DataTable dataTable = dataSet.Tables[0];
		if (dataTable.Rows.Count > 0)
		{
			object obj = dataTable.Rows[0]["Rows"];
			object obj2 = dataTable.Rows[0]["Data Pages"];
			if (obj != null && !(obj is DBNull) && obj2 != null && !(obj2 is DBNull))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "UPDATE STATISTICS {0} WITH ROWCOUNT = {1}, PAGECOUNT = {2}", new object[3]
				{
					FormatFullNameForScripting(sp),
					Convert.ToInt64(obj, SmoApplication.DefaultCulture).ToString(SmoApplication.DefaultCulture),
					Convert.ToInt64(obj2, SmoApplication.DefaultCulture).ToString(SmoApplication.DefaultCulture)
				});
				scqueries.Add(sb.ToString());
				sb.Length = 0;
			}
		}
	}

	internal void ScriptBindings(StringCollection scqueries, ScriptingPreferences sp)
	{
		if (!sp.OldOptions.Bindings)
		{
			return;
		}
		foreach (Column column in base.Columns)
		{
			column.ScriptDefaultAndRuleBinding(scqueries, sp);
		}
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

	private void GetTableCreationScript(ScriptingPreferences sp, StringBuilder sb)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		bool propValueIfSupportedWithThrowOnTarget = GetPropValueIfSupportedWithThrowOnTarget("IsEdge", defaultValue: false, sp);
		ColumnCollection columns = base.Columns;
		object indexes;
		if (!sp.ForDirectExecution)
		{
			ICollection collection = indexPropagationList;
			indexes = collection;
		}
		else
		{
			indexes = Indexes;
		}
		ScriptTableInternal(sp, sb, columns, (ICollection)indexes, propValueIfSupportedWithThrowOnTarget);
		if (IsSupportedProperty("HasSystemTimePeriod", sp))
		{
			ScriptPeriodForSystemTime(sb);
		}
		ScriptChecksAndForeignKeys(sp, sb);
		if (ShouldEmitColumnListParenthesis(propValueIfSupportedWithThrowOnTarget, base.Columns))
		{
			sb.Append(sp.NewLine);
			sb.Append(Globals.RParen);
		}
		GenerateGraphScript(sb, sp);
		if (sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse)
		{
			GenerateDataSpaceScript(sb, sp);
			GenerateTextFileGroupScript(sb, sp);
		}
		if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			GenerateDataSpaceFileStreamScript(sb, sp, alterTable: false);
		}
		if (sp.TargetEngineIsAzureStretchDb())
		{
			GenerateStretchHeapWithClause(sb, sp);
		}
		else
		{
			GenerateWithOptionScript(sb, sp);
		}
	}

	private void ScriptPeriodForSystemTime(StringBuilder sb)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(IsSupportedProperty("HasSystemTimePeriod"));
		string empty = string.Empty;
		string empty2 = string.Empty;
		bool flag = false;
		flag = GetPropValueOptional("HasSystemTimePeriod", defaultValue: false);
		if (m_systemTimePeriodInfo.m_MarkedForCreate)
		{
			empty = m_systemTimePeriodInfo.m_StartColumnName;
			empty2 = m_systemTimePeriodInfo.m_EndColumnName;
		}
		else
		{
			if (!flag)
			{
				return;
			}
			empty = SystemTimePeriodStartColumn;
			empty2 = SystemTimePeriodEndColumn;
		}
		empty = Util.EscapeString(empty, ']');
		empty2 = Util.EscapeString(empty2, ']');
		sb.Append(Globals.comma);
		sb.AppendLine();
		sb.Append(Globals.tab);
		sb.AppendFormat(SmoApplication.DefaultCulture, "PERIOD FOR SYSTEM_TIME ([{0}], [{1}])", new object[2] { empty, empty2 });
	}

	private void GetFileTableCreationScript(ScriptingPreferences sp, StringBuilder sb)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		GenerateDataSpaceScript(sb, sp);
		GenerateTextFileGroupScript(sb, sp);
		GenerateDataSpaceFileStreamScript(sb, sp, alterTable: false);
		GenerateWithOptionScript(sb, sp);
	}

	private void ScriptChecksAndForeignKeys(ScriptingPreferences sp, StringBuilder sb)
	{
		if (embeddedForeignKeyChecksList == null)
		{
			return;
		}
		foreach (SqlSmoObject embeddedForeignKeyChecks in embeddedForeignKeyChecksList)
		{
			sb.Append(Globals.comma);
			sb.Append(sp.NewLine);
			sb.Append(embeddedForeignKeyChecks.Urn.Type.Equals(ForeignKey.UrnSuffix) ? ((ForeignKey)embeddedForeignKeyChecks).ScriptDdlBody(sp) : ((Check)embeddedForeignKeyChecks).ScriptDdlBody(sp));
		}
	}

	private bool HasClusteredPrimaryOrUniqueKey(ScriptingPreferences sp)
	{
		if (base.State != SqlSmoState.Existing)
		{
			return false;
		}
		if (!HasClusteredIndex)
		{
			return false;
		}
		if (indexPropagationList == null || indexPropagationList.Count == 0)
		{
			return false;
		}
		Index index = null;
		foreach (Index indexPropagation in indexPropagationList)
		{
			if (indexPropagation.IsClustered)
			{
				index = indexPropagation;
				break;
			}
		}
		if (index == null)
		{
			return false;
		}
		if (!sp.ScriptForCreateDrop && index.IgnoreForScripting)
		{
			return false;
		}
		if (IndexKeyType.DriPrimaryKey != index.IndexKeyType)
		{
			return IndexKeyType.DriUniqueKey == index.IndexKeyType;
		}
		return true;
	}

	private void GetTableCreationScriptWithAnsiPadding(ScriptingPreferences sp, StringBuilder sb, bool initialPadding)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		StringCollection stringCollection = new StringCollection();
		bool flag = true;
		IEnumerator enumerator = base.Columns.GetEnumerator();
		bool condition = false;
		bool propValueIfSupportedWithThrowOnTarget = GetPropValueIfSupportedWithThrowOnTarget("IsEdge", defaultValue: false, sp);
		if (ShouldEmitColumnListParenthesis(propValueIfSupportedWithThrowOnTarget, base.Columns))
		{
			sb.Append(Globals.LParen);
			sb.Append(sp.NewLine);
		}
		while (enumerator.MoveNext())
		{
			Column column = enumerator.Current as Column;
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != column);
			if (sp.ScriptForCreateDrop || !column.IgnoreForScripting)
			{
				bool? columnPadding = GetColumnPadding(column);
				if (columnPadding.HasValue && columnPadding != initialPadding)
				{
					condition = true;
					break;
				}
				column.ScriptDdlInternal(stringCollection, sp);
				if (flag)
				{
					flag = false;
				}
				else
				{
					sb.Append(Globals.comma);
					sb.Append(sp.NewLine);
				}
				sb.Append(Globals.tab);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(stringCollection.Count == 1);
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
		if (ShouldEmitColumnListParenthesis(propValueIfSupportedWithThrowOnTarget, base.Columns))
		{
			sb.Append(sp.NewLine);
			sb.Append(Globals.RParen);
		}
		GenerateGraphScript(sb, sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(condition);
		GenerateDataSpaceScript(sb, sp);
		if (HasTextimageColumn(sp) && sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse && sp.Storage.FileGroup && base.Properties.Get("TextFileGroup").Value != null)
		{
			string text = (string)base.Properties["TextFileGroup"].Value;
			if (0 < text.Length)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, " TEXTIMAGE_ON [{0}]", new object[1] { SqlSmoObject.SqlBraket(text) });
			}
		}
		if (IsSupportedProperty("FileStreamPartitionScheme", sp))
		{
			GenerateDataSpaceFileStreamScript(sb, sp, alterTable: false);
		}
		GenerateWithOptionScript(sb, sp);
		bool flag2 = initialPadding;
		do
		{
			Column column2 = enumerator.Current as Column;
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != column2);
			if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType)
			{
				bool? columnPadding2 = GetColumnPadding(column2);
				if (columnPadding2.HasValue && columnPadding2 != flag2)
				{
					flag2 = columnPadding2.Value;
					sb.Append(Globals.newline);
					sb.Append("SET ANSI_PADDING ");
					sb.Append(flag2 ? "ON" : "OFF");
				}
			}
			stringCollection.Clear();
			column2.ScriptCreateInternal(stringCollection, sp, skipPropagateScript: true);
			StringEnumerator enumerator2 = stringCollection.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					string current = enumerator2.Current;
					sb.Append(Globals.newline);
					sb.Append(current);
				}
			}
			finally
			{
				if (enumerator2 is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			stringCollection.Clear();
		}
		while (enumerator.MoveNext());
		object indexes;
		if (!sp.ForDirectExecution)
		{
			ICollection collection = indexPropagationList;
			indexes = collection;
		}
		else
		{
			indexes = Indexes;
		}
		GeneratePkUkInCreateTable(sb, sp, (ICollection)indexes, embedded: false);
	}

	private void GenerateWithOptionScript(StringBuilder sb, ScriptingPreferences sp)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlDataWarehouse && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10 && !sp.TargetEngineIsAzureStretchDb() && sp.Storage.DataCompression && !HasClusteredPrimaryOrUniqueKey(sp) && IsCompressionCodeRequired(bAlter: false))
		{
			stringBuilder.Append(PhysicalPartitions.GetCompressionCode(isOnAlter: false, isOnTable: true, sp));
		}
		if (IsSupportedProperty("IsFileTable"))
		{
			bool propValueOptional = GetPropValueOptional("IsFileTable", defaultValue: false);
			string propValueOptional2 = GetPropValueOptional("FileTableDirectoryName", string.Empty);
			if (!string.IsNullOrEmpty(propValueOptional2))
			{
				if (!propValueOptional)
				{
					throw new SmoException(ExceptionTemplatesImpl.PropertyOnlySupportedForFileTable("FileTableDirectoryName"));
				}
				if (!string.IsNullOrEmpty(stringBuilder.ToString()))
				{
					stringBuilder.Append(Globals.commaspace);
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FILETABLE_DIRECTORY = {0}", new object[1] { SqlSmoObject.MakeSqlString(propValueOptional2) });
			}
			string propValueOptional3 = GetPropValueOptional("FileTableNameColumnCollation", string.Empty);
			if (!string.IsNullOrEmpty(propValueOptional3))
			{
				if (!propValueOptional)
				{
					throw new SmoException(ExceptionTemplatesImpl.PropertyOnlySupportedForFileTable("FileTableNameColumnCollation"));
				}
				if (!string.IsNullOrEmpty(stringBuilder.ToString()))
				{
					stringBuilder.Append(Globals.commaspace);
				}
				CheckCollation(propValueOptional3, sp);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FILETABLE_COLLATE_FILENAME = {0}", new object[1] { propValueOptional3 });
			}
			if (propValueOptional && FetchFileTableIndexNames(out var pkIndex, out var stream_idIndex, out var fullpathIndex, sp.Table.SystemNamesForConstraints))
			{
				if (!string.IsNullOrEmpty(pkIndex))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(Globals.commaspace);
						stringBuilder.Append(Globals.newline);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FILETABLE_PRIMARY_KEY_CONSTRAINT_NAME=[{0}]", new object[1] { pkIndex });
				}
				if (!string.IsNullOrEmpty(stream_idIndex))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(Globals.commaspace);
						stringBuilder.Append(Globals.newline);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FILETABLE_STREAMID_UNIQUE_CONSTRAINT_NAME=[{0}]", new object[1] { stream_idIndex });
				}
				if (!string.IsNullOrEmpty(fullpathIndex))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(Globals.commaspace);
						stringBuilder.Append(Globals.newline);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FILETABLE_FULLPATH_UNIQUE_CONSTRAINT_NAME=[{0}]", new object[1] { fullpathIndex });
				}
			}
		}
		string text = GenerateSystemVersioningWithClauseContent(sp);
		if (!string.IsNullOrEmpty(text))
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Append(Globals.commaspace);
				stringBuilder.Append(Globals.newline);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, text);
		}
		if (!string.IsNullOrEmpty(stringBuilder.ToString()))
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0}WITH{0}({0}{1}{0})", new object[2]
			{
				sp.NewLine,
				stringBuilder.ToString()
			});
		}
	}

	private string GenerateSystemVersioningWithClauseContent(ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (IsSupportedProperty("IsSystemVersioned", sp) && GetPropValueOptional("IsSystemVersioned", defaultValue: false))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "SYSTEM_VERSIONING = ON");
			string propValueOptional = GetPropValueOptional("HistoryTableName", string.Empty);
			string propValueOptional2 = GetPropValueOptional("HistoryTableSchema", string.Empty);
			if (string.IsNullOrEmpty(propValueOptional) != string.IsNullOrEmpty(propValueOptional2))
			{
				throw new SmoException(ExceptionTemplatesImpl.BothHistoryTableNameAndSchemaMustBeProvided);
			}
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			if (!string.IsNullOrEmpty(propValueOptional) && !string.IsNullOrEmpty(propValueOptional2))
			{
				text = string.Format(SmoApplication.DefaultCulture, "HISTORY_TABLE = {0}.{1}", new object[2]
				{
					SqlSmoObject.MakeSqlBraket(propValueOptional2),
					SqlSmoObject.MakeSqlBraket(propValueOptional)
				});
			}
			if (!m_DataConsistencyCheckForSystemVersionedTable)
			{
				text2 = "DATA_CONSISTENCY_CHECK = OFF";
			}
			if (IsSupportedProperty("HistoryRetentionPeriod"))
			{
				int propValueOptional3 = GetPropValueOptional("HistoryRetentionPeriod", -1);
				TemporalHistoryRetentionPeriodUnit propValueOptional4 = GetPropValueOptional("HistoryRetentionPeriodUnit", TemporalHistoryRetentionPeriodUnit.Undefined);
				if (propValueOptional3 > 0 && propValueOptional4 != TemporalHistoryRetentionPeriodUnit.Undefined && propValueOptional4 != TemporalHistoryRetentionPeriodUnit.Infinite)
				{
					TemporalHistoryRetentionPeriodUnitTypeConverter temporalHistoryRetentionPeriodUnitTypeConverter = new TemporalHistoryRetentionPeriodUnitTypeConverter();
					string text4 = temporalHistoryRetentionPeriodUnitTypeConverter.ConvertToInvariantString(propValueOptional4);
					text3 = string.Format(SmoApplication.DefaultCulture, "HISTORY_RETENTION_PERIOD = {0} {1}", new object[2] { propValueOptional3, text4 });
				}
			}
			bool flag = !string.IsNullOrEmpty(text);
			bool flag2 = !string.IsNullOrEmpty(text2);
			bool flag3 = !string.IsNullOrEmpty(text3);
			if (flag || flag2 || flag3)
			{
				stringBuilder.Append(" ( ");
				if (flag)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { text });
					if (flag2 || flag3)
					{
						stringBuilder.Append(" , ");
					}
				}
				if (flag2)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { text2 });
					if (flag3)
					{
						stringBuilder.Append(" , ");
					}
				}
				if (flag3)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { text3 });
				}
				stringBuilder.Append(" )");
			}
		}
		return stringBuilder.ToString();
	}

	private void GenerateSystemVersioningWithClause(StringBuilder sb, ScriptingPreferences sp)
	{
		if (!sp.TargetEngineIsAzureStretchDb())
		{
			string text = GenerateSystemVersioningWithClauseContent(sp);
			if (!string.IsNullOrEmpty(text))
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, "{0}WITH{0}({0}{1}{0})", new object[2] { sp.NewLine, text });
			}
		}
	}

	private void GenerateStretchHeapWithClause(StringBuilder sb, ScriptingPreferences sp)
	{
		if (sp.TargetEngineIsAzureStretchDb())
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, "{0}WITH (HEAP)", new object[1] { sp.NewLine });
		}
	}

	private bool FetchFileTableIndexNames(out string pkIndex, out string stream_idIndex, out string fullpathIndex, bool systemNameAllowed)
	{
		pkIndex = (stream_idIndex = (fullpathIndex = string.Empty));
		foreach (Index index in Indexes)
		{
			if (VerifyIndexType(index, IndexType.ClusteredColumnStoreIndex))
			{
				continue;
			}
			string name = index.IndexedColumns[0].Name;
			if (string.Compare(name, "path_locator", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (systemNameAllowed || !index.IsSystemNamed)
				{
					pkIndex = index.Name;
				}
			}
			else if (string.Compare(name, "stream_id", StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (systemNameAllowed || !index.IsSystemNamed)
				{
					stream_idIndex = index.Name;
				}
			}
			else if (string.Compare(name, "parent_path_locator", StringComparison.OrdinalIgnoreCase) == 0 && (systemNameAllowed || !index.IsSystemNamed))
			{
				fullpathIndex = index.Name;
			}
		}
		if (string.IsNullOrEmpty(pkIndex) && string.IsNullOrEmpty(stream_idIndex) && string.IsNullOrEmpty(fullpathIndex))
		{
			return false;
		}
		return true;
	}

	private void GenerateTextFileGroupScript(StringBuilder sb, ScriptingPreferences sp)
	{
		bool flag = false;
		if (IsSupportedProperty("IsPartitioned", sp))
		{
			flag = GetPropValueOptional("IsPartitioned", defaultValue: false);
		}
		if (HasTextimageColumn(sp) && !flag && sp.Storage.FileGroup && base.Properties.Get("TextFileGroup").Value != null)
		{
			string text = (string)base.Properties["TextFileGroup"].Value;
			if (0 < text.Length)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, " TEXTIMAGE_ON [{0}]", new object[1] { SqlSmoObject.SqlBraket(text) });
			}
		}
	}

	private void GenerateGraphScript(StringBuilder sb, ScriptingPreferences sp)
	{
		if (!sp.TargetEngineIsAzureSqlDw() && (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version140 || sp.TargetDatabaseEngineType != DatabaseEngineType.Standalone))
		{
			bool propValueOptional = GetPropValueOptional("IsNode", defaultValue: false);
			bool propValueOptional2 = GetPropValueOptional("IsEdge", defaultValue: false);
			if (propValueOptional)
			{
				sb.Append(sp.NewLine);
				sb.Append(Scripts.AS_NODE);
			}
			if (propValueOptional2)
			{
				sb.Append(sp.NewLine);
				sb.Append(Scripts.AS_EDGE);
			}
		}
	}

	protected override void PostCreate()
	{
		if (m_PartitionSchemeParameters != null)
		{
			m_PartitionSchemeParameters.LockCollection(ExceptionTemplatesImpl.ReasonObjectAlreadyCreated(UrnSuffix));
		}
		m_systemTimePeriodInfo.Reset();
		m_systemTimePeriodInfo.MarkForDrop(drop: false);
		bool propValueIfSupported = GetPropValueIfSupported("IsNode", defaultValue: false);
		bool propValueIfSupported2 = GetPropValueIfSupported("IsEdge", defaultValue: false);
		if (propValueIfSupported || propValueIfSupported2)
		{
			base.Columns.Clear();
			base.Columns.Refresh();
		}
	}

	private void ProcessExternalTableOptionalProperties(StringBuilder script, ScriptingPreferences sp)
	{
		ValidateLocationProperties("Location", "LOCATION = {0}", sp, script);
		ValidateOptionalProperty("FileFormatName", "FILE_FORMAT = {0}", new List<string>(), script, sp, bracketize: true);
		ValidateOptionalProperty("RejectType", "REJECT_TYPE = {0}", new List<ExternalTableRejectType>(), script, sp, bracketize: false, SmoManagementUtil.GetTypeConverter(typeof(ExternalTableRejectType)));
		ValidateOptionalProperty("RejectValue", "REJECT_VALUE = {0}", new List<double>(), script, sp);
		ValidateOptionalProperty("RejectSampleValue", "REJECT_SAMPLE_VALUE = {0}", new List<double> { -1.0 }, script, sp);
		ValidateLocationProperties("RejectedRowLocation", "REJECTED_ROW_LOCATION = {0}", sp, script);
		if (IsSupportedProperty("ExternalTableDistribution"))
		{
			Property propertyOptional = GetPropertyOptional("ExternalTableDistribution");
			TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(ExternalTableDistributionType));
			if (!propertyOptional.IsNull)
			{
				ExternalTableDistributionType externalTableDistributionType = (ExternalTableDistributionType)propertyOptional.Value;
				switch (externalTableDistributionType)
				{
				case ExternalTableDistributionType.Sharded:
				{
					Property propertyOptional2 = GetPropertyOptional("ShardingColumnName");
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(!propertyOptional2.IsNull);
					string propertyValue = $"{typeConverter.ConvertToInvariantString(externalTableDistributionType)}({SqlSmoObject.MakeSqlBraket(Convert.ToString(propertyOptional2.Value, SmoApplication.DefaultCulture))})";
					AddPropertyToScript(propertyValue, "DISTRIBUTION = {0}", script);
					break;
				}
				case ExternalTableDistributionType.Replicated:
				case ExternalTableDistributionType.RoundRobin:
					AddPropertyToScript(typeConverter.ConvertToInvariantString(externalTableDistributionType), "DISTRIBUTION = {0}", script);
					break;
				default:
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(externalTableDistributionType == ExternalTableDistributionType.None);
					break;
				}
			}
		}
		if (IsSupportedProperty("RemoteSchemaName", sp))
		{
			Property propertyOptional3 = GetPropertyOptional("RemoteSchemaName");
			if (!propertyOptional3.IsNull && !string.IsNullOrEmpty(propertyOptional3.Value.ToString()))
			{
				AddPropertyToScript(Util.MakeSqlString(propertyOptional3.Value.ToString()), "SCHEMA_NAME = {0}", script);
			}
		}
		if (IsSupportedProperty("RemoteObjectName", sp))
		{
			Property propertyOptional4 = GetPropertyOptional("RemoteObjectName");
			if (!propertyOptional4.IsNull && !string.IsNullOrEmpty(propertyOptional4.Value.ToString()))
			{
				AddPropertyToScript(Util.MakeSqlString(propertyOptional4.Value.ToString()), "OBJECT_NAME = {0}", script);
			}
		}
	}

	private void ValidateLocationProperties(string locationPropertyName, string sqlString, ScriptingPreferences sp, StringBuilder script)
	{
		if (IsSupportedProperty(locationPropertyName, sp))
		{
			Property propertyOptional = GetPropertyOptional(locationPropertyName);
			if (!propertyOptional.IsNull && !string.IsNullOrEmpty(propertyOptional.Value.ToString()))
			{
				AddPropertyToScript(Util.MakeSqlString(propertyOptional.Value.ToString()), sqlString, script);
			}
		}
	}

	private void ProcessSqlDwTableProperties(StringBuilder script, ScriptingPreferences sp)
	{
		if (IsSupportedProperty("DwTableDistribution"))
		{
			Property propertyOptional = GetPropertyOptional("DwTableDistribution");
			TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(DwTableDistributionType));
			if (!propertyOptional.IsNull)
			{
				DwTableDistributionType dwTableDistributionType = (DwTableDistributionType)propertyOptional.Value;
				switch (dwTableDistributionType)
				{
				case DwTableDistributionType.Hash:
				{
					string s = string.Empty;
					foreach (Column column3 in base.Columns)
					{
						if (column3.GetPropValueOptional("IsDistributedColumn", defaultValue: false))
						{
							s = column3.GetPropValueOptional("DistributionColumnName", string.Empty);
							break;
						}
					}
					string propertyValue = $"{typeConverter.ConvertToInvariantString(dwTableDistributionType)} ( {SqlSmoObject.MakeSqlBraket(s)} )";
					AddPropertyToScript(propertyValue, "DISTRIBUTION = {0}", script);
					break;
				}
				case DwTableDistributionType.Replicate:
				case DwTableDistributionType.RoundRobin:
					AddPropertyToScript(typeConverter.ConvertToInvariantString(dwTableDistributionType), "DISTRIBUTION = {0}", script);
					break;
				default:
					Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(dwTableDistributionType == DwTableDistributionType.None);
					break;
				}
			}
			else if (propertyOptional.IsNull && base.State == SqlSmoState.Creating)
			{
				AddPropertyToScript(typeConverter.ConvertToInvariantString(DwTableDistributionType.RoundRobin), "DISTRIBUTION = {0}", script);
			}
			script.Insert(0, Globals.tab);
		}
		if (IsSupportedProperty("HasClusteredColumnStoreIndex") && IsSupportedProperty("HasClusteredIndex") && IsSupportedProperty("HasHeapIndex"))
		{
			Property propertyOptional2 = GetPropertyOptional("HasClusteredColumnStoreIndex");
			Property propertyOptional3 = GetPropertyOptional("HasClusteredIndex");
			Property propertyOptional4 = GetPropertyOptional("HasHeapIndex");
			TypeConverter typeConverter2 = SmoManagementUtil.GetTypeConverter(typeof(IndexType));
			if (!propertyOptional2.IsNull || !propertyOptional3.IsNull || !propertyOptional4.IsNull)
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(Indexes != null);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(Indexes.Count > 0);
				if (!propertyOptional3.IsNull && (bool)propertyOptional3.Value)
				{
					if (!propertyOptional2.IsNull && (bool)propertyOptional2.Value)
					{
						AddPropertyToScript(typeConverter2.ConvertToInvariantString(IndexType.ClusteredColumnStoreIndex), "{0}", script, formatted: true);
					}
					else
					{
						if (script.Length > 0)
						{
							script.Append(", ");
						}
						ScriptSqlDwClusteredIndexes(script, sp, Indexes);
					}
				}
				else if (!propertyOptional4.IsNull && (bool)propertyOptional4.Value)
				{
					AddPropertyToScript(typeConverter2.ConvertToInvariantString(IndexType.HeapIndex), "{0}", script, formatted: true);
				}
			}
			else if (propertyOptional2.IsNull && propertyOptional3.IsNull && propertyOptional4.IsNull && base.State == SqlSmoState.Creating)
			{
				AddPropertyToScript(typeConverter2.ConvertToInvariantString(IndexType.ClusteredColumnStoreIndex), "{0}", script, formatted: true);
			}
		}
		bool flag = false;
		if (!IsSupportedProperty("IsPartitioned", sp) || !GetPropValueOptional("IsPartitioned", defaultValue: false) || PartitionSchemeParameters == null)
		{
			return;
		}
		script.Append(Globals.comma);
		script.Append(Globals.newline);
		script.Append(Globals.tab);
		script.AppendFormat(SmoApplication.DefaultCulture, "PARTITION");
		script.AppendFormat(Globals.newline);
		script.AppendFormat(Globals.tab);
		script.AppendFormat(Globals.LParen);
		int num = 0;
		foreach (PartitionSchemeParameter partitionSchemeParameter in PartitionSchemeParameters)
		{
			string name = partitionSchemeParameter.Name;
			Column column2 = base.Columns[name];
			if (column2 == null && base.ParentColl.ParentInstance.State != SqlSmoState.Creating)
			{
				string propValueOptional = GetPropValueOptional("PartitionScheme", string.Empty);
				throw new SmoException(ExceptionTemplatesImpl.ObjectRefsNonexCol(UrnSuffix, propValueOptional, ToString() + "." + SqlSmoObject.MakeSqlBraket(name)));
			}
			if (num > 0)
			{
				script.Append(Globals.comma);
			}
			script.Append(Globals.newline);
			script.Append(Globals.tab);
			script.Append(Globals.tab);
			script.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(column2.Name) });
			num++;
		}
		script.Append(Globals.space);
		script.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { "RANGE" });
		script.Append(Globals.space);
		if (PhysicalPartitions == null || PhysicalPartitions.Count <= 0)
		{
			return;
		}
		TypeConverter typeConverter3 = SmoManagementUtil.GetTypeConverter(typeof(RangeType));
		int num2 = 0;
		RangeType rangeType = (RangeType)PhysicalPartitions[num2].GetPropValueOptional("RangeType");
		if (rangeType != RangeType.None)
		{
			script.AppendFormat(SmoApplication.DefaultCulture, "{0} ", new object[1] { (rangeType == RangeType.Left) ? typeConverter3.ConvertToInvariantString(RangeType.Left) : typeConverter3.ConvertToInvariantString(RangeType.Right) });
		}
		script.AppendFormat(SmoApplication.DefaultCulture, "{0} ", new object[1] { "FOR VALUES" });
		script.Append(Globals.LParen);
		foreach (PhysicalPartition physicalPartition in PhysicalPartitions)
		{
			object propValueOptionalAllowNull = physicalPartition.GetPropValueOptionalAllowNull("RightBoundaryValue");
			if (propValueOptionalAllowNull != null)
			{
				if (num2 > 0)
				{
					script.Append(Globals.commaspace);
				}
				script.AppendFormat(SmoApplication.DefaultCulture, FormatSqlVariant(propValueOptionalAllowNull));
			}
			num2++;
		}
		script.Append(Globals.RParen);
		script.Append(Globals.newline);
		script.Append(Globals.tab);
		script.Append(Globals.RParen);
	}

	private void ValidateOptionalProperty<T>(string propertyName, string sqlString, List<T> defaultValues, StringBuilder fileFormatOptions, ScriptingPreferences sp, bool bracketize = false, TypeConverter typeConverter = null, bool isNullable = false)
	{
		if (!IsSupportedProperty(propertyName, sp))
		{
			return;
		}
		Property propertyOptional = GetPropertyOptional(propertyName);
		if (!propertyOptional.IsNull && !IsPropertyDefaultValue(propertyOptional, (T)propertyOptional.Value, defaultValues))
		{
			string empty = string.Empty;
			empty = ((typeConverter != null) ? typeConverter.ConvertToInvariantString(propertyOptional.Value) : Convert.ToString(propertyOptional.Value, SmoApplication.DefaultCulture));
			if (bracketize)
			{
				empty = SqlSmoObject.MakeSqlBraket(empty);
			}
			AddPropertyToScript(empty, sqlString, fileFormatOptions);
		}
	}

	private void AddPropertyToScript(string propertyValue, string sqlString, StringBuilder script, bool formatted = false)
	{
		if (script.Length > 0)
		{
			script.Append(Globals.comma);
			if (formatted)
			{
				script.Append(Globals.newline);
				script.Append(Globals.tab);
			}
		}
		script.AppendFormat(SmoApplication.DefaultCulture, sqlString, new object[1] { propertyValue });
	}

	private bool IsPropertyDefaultValue<T>(Property prop, T value, List<T> defaultValues)
	{
		if (!prop.IsNull)
		{
			foreach (T defaultValue in defaultValues)
			{
				if (EqualityComparer<T>.Default.Equals((T)prop.Value, defaultValue))
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool? GetTableAnsiPadded()
	{
		if (CheckIsMemoryOptimizedTable())
		{
			return true;
		}
		foreach (Column column in base.Columns)
		{
			bool? columnPadding = GetColumnPadding(column);
			if (columnPadding.HasValue)
			{
				return columnPadding;
			}
		}
		return null;
	}

	private bool? GetColumnPadding(Column c)
	{
		if (IsPaddingType(c))
		{
			return c.GetPropValueOptional<bool>("AnsiPaddingStatus");
		}
		if (c.GetPropValueOptional("Computed", defaultValue: false) && base.ServerVersion.Major >= 9 && c.GetPropValueOptional("IsPersisted", defaultValue: false))
		{
			return true;
		}
		return null;
	}

	internal bool HasTextimageColumn(ScriptingPreferences sp)
	{
		foreach (Column column in base.Columns)
		{
			if ((sp.ScriptForCreateDrop || !column.IgnoreForScripting) && (column.DataType.SqlDataType == SqlDataType.NText || column.DataType.SqlDataType == SqlDataType.Text || column.DataType.SqlDataType == SqlDataType.Image || column.DataType.SqlDataType == SqlDataType.VarCharMax || column.DataType.SqlDataType == SqlDataType.NVarCharMax || column.DataType.SqlDataType == SqlDataType.VarBinaryMax || column.DataType.SqlDataType == SqlDataType.Xml || column.DataType.SqlDataType == SqlDataType.Geometry || column.DataType.SqlDataType == SqlDataType.Geography || column.DataType.SqlDataType == SqlDataType.UserDefinedType || column.DataType.SqlDataType == SqlDataType.UserDefinedDataType))
			{
				return true;
			}
		}
		return false;
	}

	internal bool HasMultiplePaddings()
	{
		bool? flag = null;
		foreach (Column column in base.Columns)
		{
			if (!IsPaddingType(column))
			{
				continue;
			}
			if (!flag.HasValue)
			{
				flag = column.GetPropValueOptional<bool>("AnsiPaddingStatus");
				continue;
			}
			bool? propValueOptional = column.GetPropValueOptional<bool>("AnsiPaddingStatus");
			if (!propValueOptional.HasValue || flag == propValueOptional)
			{
				continue;
			}
			return true;
		}
		return false;
	}

	internal bool IsPaddingType(Column col)
	{
		switch (col.DataType.SqlDataType)
		{
		case SqlDataType.Binary:
		case SqlDataType.Char:
		case SqlDataType.VarBinary:
		case SqlDataType.VarBinaryMax:
		case SqlDataType.VarChar:
		case SqlDataType.VarCharMax:
			return true;
		case SqlDataType.UserDefinedDataType:
			switch (col.GetPropValueOptional("SystemType", string.Empty))
			{
			case "char":
			case "varchar":
			case "binary":
			case "varbinary":
				return true;
			}
			break;
		}
		return false;
	}

	internal static void ScriptTableInternal(ScriptingPreferences sp, StringBuilder sb, ColumnCollection columns, ICollection indexes, bool isEdgeTable = false)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sp);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != sb);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != columns);
		ScriptColumns(sp, sb, columns, isEdgeTable);
		GeneratePkUkInCreateTable(sb, sp, indexes, embedded: true);
	}

	private static void ScriptColumns(ScriptingPreferences sp, StringBuilder sb, ColumnCollection columns, bool isEdgeTable = false)
	{
		StringCollection stringCollection = new StringCollection();
		if (ShouldEmitColumnListParenthesis(isEdgeTable, columns))
		{
			sb.Append(Globals.LParen);
			sb.Append(sp.NewLine);
		}
		bool flag = true;
		foreach (Column column in columns)
		{
			if (!column.IsGraphInternalColumn() && !column.IsGraphComputedColumn() && (sp.ScriptForCreateDrop || !column.IgnoreForScripting))
			{
				column.ScriptDdlInternal(stringCollection, sp);
				if (flag)
				{
					flag = false;
				}
				else
				{
					sb.Append(Globals.comma);
					sb.Append(sp.NewLine);
				}
				sb.Append(Globals.tab);
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
	}

	private static bool ShouldEmitColumnListParenthesis(bool isEdgeTable, ColumnCollection columns)
	{
		if (!isEdgeTable)
		{
			return true;
		}
		foreach (Column column in columns)
		{
			if (column.GetPropValueOptional("GraphType", GraphType.None) == GraphType.None)
			{
				return true;
			}
		}
		return false;
	}

	private static void GenerateMemoryOptimizedIndexes(StringBuilder sb, ScriptingPreferences sp, ICollection indexes)
	{
		if (indexes == null)
		{
			return;
		}
		StringCollection stringCollection = new StringCollection();
		bool flag = true;
		foreach (Index index in indexes)
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(index.IsMemoryOptimizedIndex);
			if (flag)
			{
				flag = false;
			}
			else
			{
				sb.Append(Globals.comma);
			}
			if (sp.ScriptForCreateDrop || !index.IgnoreForScripting)
			{
				index.ScriptDdl(stringCollection, sp, notEmbedded: true, createStatement: true);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(stringCollection.Count != 0);
				sb.Append(sp.NewLine);
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
	}

	private static void ScriptSqlDwClusteredIndexes(StringBuilder sb, ScriptingPreferences sp, ICollection indexes)
	{
		if (indexes == null)
		{
			return;
		}
		StringCollection stringCollection = new StringCollection();
		bool flag = true;
		foreach (Index index in indexes)
		{
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(index.IndexType == IndexType.ClusteredIndex);
			if (flag)
			{
				flag = false;
			}
			else
			{
				sb.Append(Globals.comma);
			}
			if (sp.ScriptForCreateDrop || !index.IgnoreForScripting)
			{
				index.ScriptDdl(stringCollection, sp, notEmbedded: true, createStatement: true);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(stringCollection.Count == 1);
				sb.Append(sp.NewLine);
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
	}

	private static void GeneratePkUkInCreateTable(StringBuilder sb, ScriptingPreferences sp, ICollection indexes, bool embedded)
	{
		if (indexes == null)
		{
			return;
		}
		StringCollection stringCollection = new StringCollection();
		foreach (Index index3 in indexes)
		{
			if (IsColumnstoreIndex(index3) || IndexKeyType.DriPrimaryKey != (IndexKeyType)index3.GetPropValue("IndexKeyType"))
			{
				continue;
			}
			if (sp.ScriptForCreateDrop || !index3.IgnoreForScripting)
			{
				index3.ScriptDdl(stringCollection, sp, !embedded, createStatement: true);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(stringCollection.Count != 0);
				if (embedded)
				{
					sb.Append(Globals.comma);
				}
				sb.Append(sp.NewLine);
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
			break;
		}
		foreach (Index index4 in indexes)
		{
			if (!IsColumnstoreIndex(index4) && IndexKeyType.DriUniqueKey == index4.IndexKeyType && (sp.TargetServerVersion != SqlServerVersion.Version80 || !index4.IgnoreDuplicateKeys) && (sp.ScriptForCreateDrop || !index4.IgnoreForScripting))
			{
				index4.ScriptDdl(stringCollection, sp, !embedded, createStatement: true);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(stringCollection.Count != 0);
				if (embedded)
				{
					sb.Append(Globals.comma);
				}
				sb.Append(sp.NewLine);
				sb.Append(stringCollection[0]);
				stringCollection.Clear();
			}
		}
	}

	private static bool IsColumnstoreIndex(Index index)
	{
		if (!VerifyIndexType(index, IndexType.ClusteredColumnStoreIndex))
		{
			return VerifyIndexType(index, IndexType.NonClusteredColumnStoreIndex);
		}
		return true;
	}

	private static bool VerifyIndexType(Index index, IndexType expectedIndexType)
	{
		if (index.GetPropValueOptional<IndexType>("IndexType").HasValue)
		{
			return expectedIndexType.Equals(index.GetPropValueOptional<IndexType>("IndexType").Value);
		}
		return false;
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (IsSupportedProperty("IsSystemVersioned", sp) && IsSystemVersioned)
		{
			IsSystemVersioned = false;
			ScriptSystemVersioning(queries, sp);
			IsSystemVersioned = true;
		}
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
				{
					"",
					SqlSmoObject.SqlString(text)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE80, new object[2]
				{
					"",
					SqlSmoObject.SqlString(text)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		if (!GetPropValueIfSupportedWithThrowOnTarget("IsExternal", defaultValue: false, sp))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP TABLE {0}{1}", new object[2]
			{
				(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
				text
			});
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP EXTERNAL TABLE {0}", new object[1] { text });
		}
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
		SetSchemaOwned();
	}

	public void Rebuild(int partitionNumber)
	{
		rebuildPartitionNumber = partitionNumber;
		try
		{
			Rebuild();
		}
		finally
		{
			rebuildPartitionNumber = -1;
		}
	}

	private StringCollection InsertUseDbIfNeeded(StringCollection queries)
	{
		if (DatabaseEngineType == DatabaseEngineType.Standalone)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		}
		return queries;
	}

	public void Rebuild()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			int count = InsertUseDbIfNeeded(stringCollection).Count;
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			scriptingPreferences.SetTargetServerInfo(this);
			if (-1 != rebuildPartitionNumber)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = false;
				if (VersionUtils.IsSql12OrLater(base.ServerVersion) && OnlineHeapOperation)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ONLINE = ON");
					ScriptWaitAtLowPriorityIndexOption(stringBuilder);
					flag = true;
				}
				if (PhysicalPartitions.IsDataCompressionStateDirty(rebuildPartitionNumber))
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD PARTITION = {1} WITH({2}{3}{4})", FormatFullNameForScripting(scriptingPreferences), rebuildPartitionNumber, PhysicalPartitions.GetCompressionCode(rebuildPartitionNumber), flag ? (", " + stringBuilder.ToString()) : string.Empty, (MaximumDegreeOfParallelism > 0) ? (", MAXDOP = " + MaximumDegreeOfParallelism) : string.Empty));
				}
				else if (!flag)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD PARTITION = {1} {2}", new object[3]
					{
						FormatFullNameForScripting(scriptingPreferences),
						rebuildPartitionNumber,
						(MaximumDegreeOfParallelism > 0) ? ("WITH (MAXDOP = " + MaximumDegreeOfParallelism + ")") : string.Empty
					}));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD PARTITION = {1} WITH ({2}{3})", FormatFullNameForScripting(scriptingPreferences), rebuildPartitionNumber, stringBuilder.ToString(), (MaximumDegreeOfParallelism > 0) ? (", MAXDOP = " + MaximumDegreeOfParallelism) : string.Empty));
				}
			}
			else
			{
				GenerateDataCompressionAlterScript(stringCollection, scriptingPreferences);
			}
			if (stringCollection.Count == count)
			{
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				ScriptRebuildOptions(stringBuilder2, scriptingPreferences);
				if (stringBuilder2.Length > 0)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD WITH ({1})", new object[2]
					{
						FormatFullNameForScripting(scriptingPreferences),
						stringBuilder2.ToString()
					}));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD", new object[1] { FormatFullNameForScripting(scriptingPreferences) }));
				}
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RebuildHeapError(ex.Message), this, ex);
		}
		if (!ExecutionManager.Recording && PhysicalPartitions != null)
		{
			if (rebuildPartitionNumber == -1)
			{
				PhysicalPartitions.Reset();
			}
			else
			{
				PhysicalPartitions.Reset(rebuildPartitionNumber);
			}
		}
	}

	private void ScriptRebuildOptions(StringBuilder rebuildOptions, ScriptingPreferences sp)
	{
		bool flag = false;
		if (OnlineHeapOperation)
		{
			rebuildOptions.AppendFormat(SmoApplication.DefaultCulture, "ONLINE = ON");
			ScriptWaitAtLowPriorityIndexOption(rebuildOptions);
			flag = true;
		}
		if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && MaximumDegreeOfParallelism > 0)
		{
			if (flag)
			{
				rebuildOptions.Append(Globals.commaspace);
			}
			rebuildOptions.AppendFormat(SmoApplication.DefaultCulture, "MAXDOP = {0}", new object[1] { MaximumDegreeOfParallelism });
			flag = true;
		}
	}

	private void ScriptWaitAtLowPriorityIndexOption(StringBuilder options)
	{
		if (VersionUtils.IsSql12OrLater(base.ServerVersion))
		{
			options.AppendFormat(SmoApplication.DefaultCulture, " (WAIT_AT_LOW_PRIORITY (MAX_DURATION = {0} MINUTES, ABORT_AFTER_WAIT = {1}))", new object[2]
			{
				LowPriorityMaxDuration,
				LowPriorityAbortAfterWait.ToString().ToUpper()
			});
		}
	}

	private void GenerateDataCompressionAlterScript(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (PhysicalPartitions.IsCollectionDirty())
		{
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			string compressionCode = PhysicalPartitions.GetCompressionCode(isOnAlter: true, isOnTable: true, sp);
			ScriptRebuildOptions(stringBuilder, sp);
			if (stringBuilder.Length > 0)
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD {1}{2}WITH {2}({3}, {4}{2})", FormatFullNameForScripting(sp), "PARTITION = ALL", sp.NewLine, stringBuilder.ToString(), compressionCode));
			}
			else
			{
				alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} REBUILD {1}{2}WITH {2}({3}{2})", FormatFullNameForScripting(sp), "PARTITION = ALL", sp.NewLine, compressionCode));
			}
		}
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (sp == null)
		{
			throw new ArgumentNullException("Scripting preferences cannot be null.");
		}
		this.ThrowIfNotSupported(GetType(), sp);
		ValidateIndexes();
		bool forCreate = false;
		ScriptVardecimalCompression(alterQuery, sp, forCreate);
		if (IsSupportedProperty("ChangeTrackingEnabled", sp) && sp.Data.ChangeTracking)
		{
			ScriptChangeTracking(alterQuery, sp);
		}
		if (IsSupportedProperty("IsSystemVersioned", sp))
		{
			ScriptSystemVersioning(alterQuery, sp);
		}
		if (IsSupportedProperty("TemporalType", sp))
		{
			ScriptSystemTimePeriodForAlter(alterQuery, sp);
		}
		if (IsSupportedProperty("FileStreamFileGroup", sp))
		{
			Property property = base.Properties.Get("FileStreamFileGroup");
			Property property2 = base.Properties.Get("FileStreamPartitionScheme");
			if (property.Dirty || property2.Dirty)
			{
				if (property.Dirty && property2.Dirty)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.MutuallyExclusiveProperties("FileStreamPartitionScheme", "FileStreamFileGroup"));
				}
				string text = FormatFullNameForScripting(sp);
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ", new object[1] { text });
				GenerateDataSpaceFileStreamScript(stringBuilder, sp, alterTable: true);
				alterQuery.Add(stringBuilder.ToString());
			}
		}
		if (IsSupportedProperty("LockEscalation", sp))
		{
			ScriptLockEscalationSettings(alterQuery, sp);
		}
		ScriptAlterFileTableProp(alterQuery, sp);
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(alterQuery, sp);
		}
		if (IsSupportedProperty("RemoteDataArchiveEnabled", sp))
		{
			ScriptRemoteDataArchive(alterQuery, sp);
		}
	}

	protected override void PostAlter()
	{
		base.PostAlter();
		m_systemTimePeriodInfo.Reset();
		m_systemTimePeriodInfo.MarkForDrop(drop: false);
	}

	public void AlterWithNoCheck()
	{
		try
		{
			AlterImplInit(out var alterQuery, out var sp);
			sp.Table.ConstraintsWithNoCheck = true;
			ScriptAlterInternal(alterQuery, sp);
			bool forCreate = false;
			ScriptVardecimalCompression(alterQuery, sp, forCreate);
			AlterImplFinish(alterQuery, sp);
			SetSchemaOwned();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, ex);
		}
	}

	public void Rename(string newname)
	{
		CheckTableName(newname);
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_rename @objname = N'{0}', @newname = N'{1}', @objtype = N'OBJECT'", new object[2]
		{
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_Checks != null)
		{
			m_Checks.MarkAllDropped();
		}
		if (m_ForeignKeys != null)
		{
			m_ForeignKeys.MarkAllDropped();
		}
		if (m_PartitionSchemeParameters != null)
		{
			m_PartitionSchemeParameters.MarkAllDropped();
		}
		if (m_PhysicalPartitions != null)
		{
			m_PhysicalPartitions.MarkAllDropped();
		}
	}

	public StringCollection CheckIdentityValue()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			InsertUseDbIfNeeded(stringCollection);
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			FormatFullNameForScripting(scriptingPreferences);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKIDENT(N'{0}')", new object[1] { SqlSmoObject.SqlString(FullQualifiedName) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckIdentityValues, this, ex);
		}
	}

	public StringCollection CheckTable()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			InsertUseDbIfNeeded(stringCollection);
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			string s = FormatFullNameForScripting(scriptingPreferences);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKTABLE (N'{0}') WITH NO_INFOMSGS", new object[1] { SqlSmoObject.SqlString(s) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckTable, this, ex);
		}
	}

	public StringCollection CheckTableDataOnly()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			InsertUseDbIfNeeded(stringCollection);
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			scriptingPreferences.ScriptForCreateDrop = true;
			string s = FormatFullNameForScripting(scriptingPreferences);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC CHECKTABLE (N'{0}', NOINDEX)", new object[1] { SqlSmoObject.SqlString(s) }));
			return ExecutionManager.ExecuteNonQueryWithMessage(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.CheckTable, this, ex);
		}
	}

	public DataTable EnumLastStatisticsUpdates()
	{
		try
		{
			CheckObjectState();
			return EnumLastStatisticsUpdates(null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLastStatisticsUpdates, this, ex);
		}
	}

	public DataTable EnumLastStatisticsUpdates(string statname)
	{
		try
		{
			CheckObjectState();
			Request request = new Request(base.Urn.Value + string.Format(SmoApplication.DefaultCulture, "/Statistic"));
			if (statname != null)
			{
				request.Urn.Value += string.Format(SmoApplication.DefaultCulture, "[@Name='{0}']", new object[1] { Urn.EscapeString(statname) });
			}
			request.Fields = new string[2] { "Name", "LastUpdated" };
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLastStatisticsUpdates, this, ex);
		}
	}

	public DataTable EnumForeignKeys()
	{
		try
		{
			Request request = new Request(string.Concat(base.ParentColl.ParentInstance.Urn, string.Format(SmoApplication.DefaultCulture, "/Table/ForeignKey[@ReferencedTable='{0}']", new object[1] { Urn.EscapeString(Name) })), new string[1] { "Name" });
			request.OrderByList = new OrderBy[1]
			{
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			request.ParentPropertiesRequests = new PropertiesRequest[1];
			PropertiesRequest propertiesRequest = new PropertiesRequest();
			propertiesRequest.Fields = new string[2] { "Schema", "Name" };
			propertiesRequest.OrderByList = new OrderBy[2]
			{
				new OrderBy("Schema", OrderBy.Direction.Asc),
				new OrderBy("Name", OrderBy.Direction.Asc)
			};
			request.ParentPropertiesRequests[0] = propertiesRequest;
			return ExecutionManager.GetEnumeratorData(request);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumForeignKeys, this, ex);
		}
	}

	public void RebuildIndexes(int fillFactor)
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			InsertUseDbIfNeeded(stringCollection);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC DBREINDEX(N'{0}', N'', {1})", new object[2]
			{
				SqlSmoObject.SqlString(FullQualifiedName),
				fillFactor
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RebuildIndexes, this, ex);
		}
	}

	public void RecalculateSpaceUsage()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			InsertUseDbIfNeeded(stringCollection);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "DBCC UPDATEUSAGE(0, N'[{0}].[{1}]') WITH NO_INFOMSGS", new object[2]
			{
				SqlSmoObject.SqlString(Schema),
				SqlSmoObject.SqlString(Name)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RecalculateSpaceUsage, this, ex);
		}
	}

	public void TruncateData()
	{
		if (CheckIsExternalTable())
		{
			throw new SmoException(ExceptionTemplatesImpl.TruncateOperationNotSupportedOnExternalTables);
		}
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			InsertUseDbIfNeeded(stringCollection);
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "TRUNCATE TABLE [{0}].[{1}]", new object[2]
			{
				SqlSmoObject.SqlBraket(Schema),
				SqlSmoObject.SqlBraket(Name)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.TruncateData, this, ex);
		}
	}

	public void TruncateData(int partitionNumber)
	{
		if (VersionUtils.IsSql12OrLater(base.ServerVersion))
		{
			if (CheckIsExternalTable())
			{
				throw new SmoException(ExceptionTemplatesImpl.TruncateOperationNotSupportedOnExternalTables);
			}
			try
			{
				CheckObjectState();
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "TRUNCATE TABLE [{0}].[{1}] WITH (PARTITIONS ({2}))", new object[3]
				{
					SqlSmoObject.SqlBraket(Schema),
					SqlSmoObject.SqlBraket(Name),
					partitionNumber
				}));
				ExecutionManager.ExecuteNonQuery(stringCollection);
				return;
			}
			catch (Exception ex)
			{
				SqlSmoObject.FilterException(ex);
				throw new FailedOperationException(ExceptionTemplatesImpl.TruncateData, this, ex);
			}
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.TruncatePartitionsNotSupported);
	}

	public void DisableAllIndexes()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion90();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER INDEX ALL ON {0} DISABLE", new object[1] { FullQualifiedName }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DisableAllIndexes, this, ex);
		}
	}

	public void EnableAllIndexes(IndexEnableAction action)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			ThrowIfBelowVersion90();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
			if (action == IndexEnableAction.Rebuild)
			{
				StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				if (base.ServerVersion.Major >= 10)
				{
					ScriptRebuildOptions(stringBuilder, new ScriptingPreferences(this));
				}
				if (stringBuilder.Length > 0)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER INDEX ALL ON {0} REBUILD WITH ({1})", new object[2]
					{
						FullQualifiedName,
						stringBuilder.ToString()
					}));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER INDEX ALL ON {0} REBUILD", new object[1] { FullQualifiedName }));
				}
			}
			else
			{
				string dBName = GetDBName();
				StringCollection stringCollection2 = new StringCollection();
				stringCollection2.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(dBName) }));
				ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
				scriptingPreferences.ScriptForCreateDrop = false;
				scriptingPreferences.SetTargetServerInfo(this);
				foreach (Index index3 in Indexes)
				{
					bool dropExistingIndex = index3.dropExistingIndex;
					index3.dropExistingIndex = true;
					try
					{
						index3.ScriptCreateInternal(stringCollection2, scriptingPreferences);
					}
					finally
					{
						index3.dropExistingIndex = dropExistingIndex;
					}
				}
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
			foreach (Index index4 in Indexes)
			{
				index4.Refresh();
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnableAllIndexes, this, ex);
		}
	}

	public void AddPeriodForSystemTime(string periodStartColumn, string periodEndColumn, bool addPeriod)
	{
		if (!IsSupportedProperty("HasSystemTimePeriod"))
		{
			throw new SmoException(ExceptionTemplatesImpl.ReasonPropertyIsNotSupportedOnCurrentServerVersion);
		}
		if (base.State == SqlSmoState.Dropped || base.State == SqlSmoState.ToBeDropped)
		{
			throw new SmoException(ExceptionTemplatesImpl.NoAddingPeriodOnDroppedTable);
		}
		if (base.State == SqlSmoState.Existing && HasSystemTimePeriod)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotHaveMultiplePeriods);
		}
		if (addPeriod)
		{
			if (string.IsNullOrEmpty(periodStartColumn) || string.IsNullOrEmpty(periodEndColumn))
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidPeriodColumnName);
			}
			m_systemTimePeriodInfo.MarkForCreate(periodStartColumn, periodEndColumn);
		}
		else
		{
			m_systemTimePeriodInfo.Reset();
		}
	}

	public void DropPeriodForSystemTime()
	{
		if (!IsSupportedProperty("HasSystemTimePeriod"))
		{
			throw new SmoException(ExceptionTemplatesImpl.ReasonPropertyIsNotSupportedOnCurrentServerVersion);
		}
		if (base.State == SqlSmoState.Dropped || base.State == SqlSmoState.ToBeDropped)
		{
			throw new SmoException(ExceptionTemplatesImpl.NoDroppingPeriodOnDroppedTable);
		}
		if (base.State == SqlSmoState.Creating)
		{
			throw new SmoException(ExceptionTemplatesImpl.NoDroppingPeriodOnNotYetCreatedTable);
		}
		if (base.State == SqlSmoState.Existing && !HasSystemTimePeriod)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotDropNonExistingPeriod);
		}
		m_systemTimePeriodInfo.MarkForDrop(drop: true);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return GetPropagateInfoImpl(action, forDiscovery: false);
	}

	private PropagateInfo[] GetPropagateInfoImpl(PropagateAction action, bool forDiscovery)
	{
		bool bWithScript = action != PropagateAction.Create;
		ArrayList arrayList = new ArrayList();
		arrayList.Add(new PropagateInfo((base.ServerVersion.Major < 10) ? null : m_PhysicalPartitions, bWithScript: false, bPropagateScriptToChildLevel: false));
		arrayList.Add(new PropagateInfo(base.Columns, bWithScript, bPropagateScriptToChildLevel: true));
		arrayList.Add(new PropagateInfo(base.Statistics, bWithScript: true, Statistic.UrnSuffix));
		if (forDiscovery)
		{
			indexPropagationList = null;
			embeddedForeignKeyChecksList = null;
			new IndexPropagateInfo(Indexes).PropagateInfo(arrayList);
		}
		else
		{
			arrayList.Add(new PropagateInfo(Indexes, bWithScript: true, Index.UrnSuffix));
		}
		if (DatabaseEngineEdition.SqlDataWarehouse != Parent.DatabaseEngineEdition)
		{
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType || base.ServerVersion.Major >= 12)
			{
				arrayList.Add(new PropagateInfo((base.ServerVersion.Major < 8) ? null : base.ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix));
				if (Parent.IsSupportedProperty("IsFullTextEnabled") && base.FullTextIndex != null)
				{
					arrayList.Add(new PropagateInfo(base.FullTextIndex, bWithScript: true, FullTextIndex.UrnSuffix));
				}
			}
			arrayList.Add(new PropagateInfo(base.Triggers, bWithScript: true, Trigger.UrnSuffix));
			if (!IsSupportedProperty("IsFileTable") || !GetPropValueOptional("IsFileTable", defaultValue: false))
			{
				arrayList.Add(new PropagateInfo(ForeignKeys, bWithScript: true, ForeignKey.UrnSuffix));
				arrayList.Add(new PropagateInfo(Checks, bWithScript: true, Check.UrnSuffix));
			}
			else
			{
				List<ForeignKey> list = new List<ForeignKey>();
				foreach (ForeignKey foreignKey in ForeignKeys)
				{
					if (!foreignKey.IsFileTableDefined)
					{
						list.Add(foreignKey);
					}
				}
				arrayList.Add(new PropagateInfo(list, bWithScript: true, ForeignKey.UrnSuffix));
				List<Check> list2 = new List<Check>();
				foreach (Check check in Checks)
				{
					if (!check.IsFileTableDefined)
					{
						list2.Add(check);
					}
				}
				arrayList.Add(new PropagateInfo(list2, bWithScript: true, Check.UrnSuffix));
			}
		}
		PropagateInfo[] array = new PropagateInfo[arrayList.Count];
		arrayList.CopyTo(array, 0);
		return array;
	}

	internal override PropagateInfo[] GetPropagateInfoForDiscovery(PropagateAction action)
	{
		return GetPropagateInfoImpl(action, forDiscovery: true);
	}

	public void SwitchPartition(int sourcePartitionNumber, Table targetTable, int targetPartitionNumber)
	{
		CheckObjectState(throwIfNotCreated: true);
		SwitchPartitionImpl(sourcePartitionNumber, targetTable, targetPartitionNumber);
	}

	public void SwitchPartition(int sourcePartitionNumber, Table targetTable)
	{
		CheckObjectState(throwIfNotCreated: true);
		SwitchPartitionImpl(sourcePartitionNumber, targetTable, -1);
	}

	public void SwitchPartition(Table targetTable, int targetPartitionNumber)
	{
		CheckObjectState(throwIfNotCreated: true);
		SwitchPartitionImpl(-1, targetTable, targetPartitionNumber);
	}

	public void SwitchPartition(Table targetTable)
	{
		CheckObjectState(throwIfNotCreated: true);
		SwitchPartitionImpl(-1, targetTable, -1);
	}

	public RemoteTableMigrationStatistics GetRemoteTableMigrationStatistics()
	{
		try
		{
			CheckObjectState();
			ThrowIfPropertyNotSupported("RemoteDataArchiveEnabled");
			if (RemoteDataArchiveEnabled && RemoteTableProvisioned)
			{
				StringCollection stringCollection = new StringCollection();
				if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
				}
				stringCollection.Add(string.Format(CultureInfo.InvariantCulture, "exec sp_spaceused @objname = N'[{0}].[{1}]', @mode = 'REMOTE_ONLY', @oneresultset = 1", new object[2]
				{
					Urn.EscapeString(Schema),
					Urn.EscapeString(Name)
				}));
				DataSet dataSet = ExecutionManager.ExecuteWithResults(stringCollection);
				double sizeInKB = 0.0;
				long rowCount = 0L;
				if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
				{
					string text = dataSet.Tables[0].Rows[0]["rows"].ToString();
					rowCount = long.Parse(text.Trim());
					string text2 = dataSet.Tables[0].Rows[0]["data"].ToString();
					if (text2.ToUpperInvariant().IndexOf("KB") > -1)
					{
						string text3 = text2.Substring(0, text2.ToUpperInvariant().IndexOf("KB"));
						sizeInKB = double.Parse(text3.Trim());
					}
				}
				return new RemoteTableMigrationStatistics(sizeInKB, rowCount);
			}
			return null;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetRemoteTableMigrationStatistics, this, ex);
		}
	}

	private void SwitchPartitionImpl(int sourcePartitionNumber, Table targetTable, int targetPartitionNumber)
	{
		try
		{
			if (targetTable == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("targetTable"));
			}
			CheckObjectState(throwIfNotCreated: true);
			if (Parent.Parent.ConnectionContext.SqlExecutionModes != SqlExecutionModes.CaptureSql)
			{
				targetTable.CheckObjectState(throwIfNotCreated: true);
			}
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE [{0}].{1} SWITCH ", new object[2]
			{
				SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName),
				FullQualifiedName
			});
			if (0 <= sourcePartitionNumber)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "PARTITION {0}", new object[1] { sourcePartitionNumber });
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " TO [{0}].{1}", new object[2]
			{
				SqlSmoObject.SqlBraket(targetTable.Parent.Name),
				targetTable.FullQualifiedName
			});
			if (0 <= targetPartitionNumber)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "PARTITION {0}", new object[1] { targetPartitionNumber });
			}
			if (VersionUtils.IsSql12OrLater(base.ServerVersion))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " WITH");
				ScriptWaitAtLowPriorityIndexOption(stringBuilder);
			}
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SwitchPartition, this, ex);
		}
	}

	private void ValidateExternalTableOptionalProperties(ScriptingPreferences sp)
	{
		bool flag = true;
		if (sp.TargetDatabaseEngineType == DatabaseEngineType.SqlAzureDatabase)
		{
			flag = false;
		}
		if (flag)
		{
			ValidateExternalTableRequiredStringProperty("Location", sp);
			ValidateExternalTableRequiredStringProperty("FileFormatName", sp);
			if (!IsSupportedProperty("RejectType", sp))
			{
				return;
			}
			Property propertyOptional = GetPropertyOptional("RejectType");
			switch ((!propertyOptional.IsNull) ? ((ExternalTableRejectType)propertyOptional.Value) : ExternalTableRejectType.Value)
			{
			case ExternalTableRejectType.Value:
				if (IsSupportedProperty("RejectSampleValue", sp))
				{
					Property propertyOptional3 = GetPropertyOptional("RejectSampleValue");
					if (!propertyOptional3.IsNull && (double)propertyOptional3.Value != -1.0)
					{
						throw new SmoException(ExceptionTemplatesImpl.ConflictingExternalTableProperties(propertyOptional3.Name, propertyOptional3.Value.ToString(), propertyOptional.Name, propertyOptional.Value.ToString()));
					}
				}
				break;
			case ExternalTableRejectType.Percentage:
				if (IsSupportedProperty("RejectSampleValue", sp))
				{
					Property propertyOptional2 = GetPropertyOptional("RejectSampleValue");
					if (propertyOptional2.IsNull || (double)propertyOptional2.Value == -1.0)
					{
						throw new PropertyNotSetException("RejectSampleValue");
					}
				}
				break;
			default:
				throw new WrongPropertyValueException(propertyOptional);
			}
		}
		else
		{
			if (!IsSupportedProperty("ExternalTableDistribution"))
			{
				return;
			}
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(IsSupportedProperty("ShardingColumnName"));
			Property propertyOptional4 = GetPropertyOptional("ExternalTableDistribution");
			Property propertyOptional5 = GetPropertyOptional("ShardingColumnName");
			if (!propertyOptional4.IsNull)
			{
				switch ((ExternalTableDistributionType)propertyOptional4.Value)
				{
				case ExternalTableDistributionType.Sharded:
				{
					if (propertyOptional5.IsNull || string.IsNullOrEmpty(propertyOptional5.Value.ToString()))
					{
						throw new SmoException(ExceptionTemplatesImpl.ShardingColumnNotSpecifiedForShardedDistribution(propertyOptional5.Name));
					}
					string text = propertyOptional5.Value.ToString();
					if (!base.Columns.Contains(text))
					{
						throw new SmoException(ExceptionTemplatesImpl.ShardingColumnNotAddedToTable(text));
					}
					break;
				}
				case ExternalTableDistributionType.Replicated:
				case ExternalTableDistributionType.RoundRobin:
				case ExternalTableDistributionType.None:
					if (!propertyOptional5.IsNull && !string.IsNullOrEmpty(propertyOptional5.Value.ToString()))
					{
						throw new SmoException(ExceptionTemplatesImpl.ShardingColumnNotSupportedWithNonShardedDistribution(propertyOptional5.Name, propertyOptional4.Value.ToString()));
					}
					break;
				default:
					throw new WrongPropertyValueException(propertyOptional4);
				}
			}
			else if (!propertyOptional5.IsNull && !string.IsNullOrEmpty(propertyOptional5.Value.ToString()))
			{
				throw new SmoException(ExceptionTemplatesImpl.ConflictingExternalTableProperties(propertyOptional5.Name, propertyOptional5.Value.ToString(), propertyOptional4.Name, propertyOptional4.Value.ToString()));
			}
			if (IsSupportedProperty("RemoteSchemaName"))
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(IsSupportedProperty("RemoteObjectName"));
				Property propertyOptional6 = GetPropertyOptional("RemoteSchemaName");
				Property propertyOptional7 = GetPropertyOptional("RemoteObjectName");
				bool flag2 = !propertyOptional6.IsNull && !string.IsNullOrEmpty(propertyOptional6.Value.ToString());
				bool flag3 = !propertyOptional7.IsNull && !string.IsNullOrEmpty(propertyOptional7.Value.ToString());
				if ((flag2 && !flag3) || (!flag2 && flag3))
				{
					throw new SmoException(ExceptionTemplatesImpl.DependentPropertyMissing(propertyOptional6.Name, propertyOptional7.Name));
				}
			}
		}
	}

	private bool CheckIsExternalTable()
	{
		return GetPropValueIfSupported("IsExternal", defaultValue: false);
	}

	private bool CheckIsSqlDwTable()
	{
		bool result = false;
		if (IsSupportedProperty("DwTableDistribution"))
		{
			result = GetPropValueOptional<DwTableDistributionType>("DwTableDistribution").HasValue;
		}
		return result;
	}

	private bool CheckIsMemoryOptimizedTable()
	{
		return GetPropValueIfSupported("IsMemoryOptimized", defaultValue: false);
	}

	internal static void CheckTableName(string tableName)
	{
		if (tableName.StartsWith("#", StringComparison.Ordinal))
		{
			throw new WrongPropertyValueException(ExceptionTemplatesImpl.TempTablesNotSupported(tableName));
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[47]
		{
			"AnsiNullsStatus", "ChangeTrackingEnabled", "DataSourceName", "DwTableDistribution", "Durability", "ExternalTableDistribution", "FileFormatName", "FileGroup", "FileStreamFileGroup", "FileStreamPartitionScheme",
			"FileTableDirectoryName", "FileTableNameColumnCollation", "FileTableNamespaceEnabled", "HasClusteredIndex", "HasClusteredColumnStoreIndex", "HasHeapIndex", "HasSystemTimePeriod", "HistoryTableName", "HistoryTableSchema", "ID",
			"IsEdge", "IsExternal", "IsFileTable", "IsMemoryOptimized", "IsNode", "IsSchemaOwned", "IsSystemObject", "IsSystemVersioned", "IsPartitioned", "IsVarDecimalStorageFormatEnabled",
			"Location", "LockEscalation", "Owner", "PartitionScheme", "RejectedRowLocation", "RejectSampleValue", "RejectType", "RejectValue", "QuotedIdentifierStatus", "RemoteObjectName",
			"RemoteSchemaName", "ShardingColumnName", "SystemTimePeriodEndColumn", "SystemTimePeriodStartColumn", "TemporalType", "TextFileGroup", "TrackColumnsUpdatedEnabled"
		};
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	internal static string[] GetScriptFields2(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode, ScriptingPreferences sp)
	{
		if (version.Major > 9 && sp.TargetServerVersionInternal > SqlServerVersionInternal.Version90 && sp.Storage.DataCompression)
		{
			return new string[1] { "HasCompressedPartitions" };
		}
		return new string[0];
	}

	private void ScriptVardecimalCompression(StringCollection query, ScriptingPreferences sp, bool forCreate)
	{
		if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && (sp.ForDirectExecution || !sp.OldOptions.NoVardecimal) && Parent.IsVarDecimalStorageFormatSupported)
		{
			Property property = base.Properties.Get("IsVarDecimalStorageFormatEnabled");
			if (property.Dirty || (forCreate && base.State == SqlSmoState.Existing && IsVarDecimalStorageFormatEnabled))
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sys.sp_tableoption N'{0}', N'vardecimal storage format', N'{1}'", new object[2]
				{
					FormatFullNameForScripting(sp),
					IsVarDecimalStorageFormatEnabled ? "ON" : "OFF"
				}));
			}
		}
	}

	private void ScriptAlterFileTableProp(StringCollection query, ScriptingPreferences sp)
	{
		if (!IsSupportedProperty("IsFileTable"))
		{
			return;
		}
		bool propValueOptional = GetPropValueOptional("IsFileTable", defaultValue: false);
		Property propertyOptional = GetPropertyOptional("FileTableNamespaceEnabled");
		Property propertyOptional2 = GetPropertyOptional("FileTableDirectoryName");
		if (propertyOptional2.Dirty || propertyOptional.Dirty)
		{
			if (!IsSupportedProperty("IsFileTable", sp))
			{
				throw new SmoException(ExceptionTemplatesImpl.FileTableNotSupportedOnTargetEngine(SqlSmoObject.GetSqlServerName(sp)));
			}
			if (!propValueOptional)
			{
				throw new SmoException(ExceptionTemplatesImpl.PropertyOnlySupportedForFileTable(propertyOptional2.Dirty ? "FileTableDirectoryName" : "FileTableNamespaceEnabled"));
			}
		}
		if (!string.IsNullOrEmpty(propertyOptional2.Value as string) && sp.ScriptForAlter && (propertyOptional2.Dirty || !sp.ForDirectExecution))
		{
			string value = string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0}{1}SET( FILETABLE_DIRECTORY = {2})", new object[3]
			{
				FormatFullNameForScripting(sp),
				Globals.newline,
				SqlSmoObject.MakeSqlString((string)propertyOptional2.Value)
			});
			query.Add(value);
		}
		if (propValueOptional && propertyOptional.Value != null && (propertyOptional.Dirty || (!sp.ForDirectExecution && !(bool)propertyOptional.Value)))
		{
			string value2 = string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0}{1}{2} FILETABLE_NAMESPACE", new object[3]
			{
				FormatFullNameForScripting(sp),
				Globals.newline,
				((bool)propertyOptional.Value) ? Scripts.ENABLE : Scripts.DISABLE
			});
			query.Add(value2);
		}
	}

	private void ScriptChangeTracking(StringCollection query, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && !VersionUtils.IsSql12OrLater(sp.TargetServerVersionInternal, base.ServerVersion))
		{
			return;
		}
		Property property = base.Properties.Get("ChangeTrackingEnabled");
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!property.IsNull)
		{
			flag = (bool)property.Value;
			if ((property.Dirty && sp.ScriptForAlter) || (flag && !sp.ScriptForAlter))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} {1} CHANGE_TRACKING ", new object[2]
				{
					FormatFullNameForScripting(sp),
					((bool)property.Value) ? "ENABLE" : "DISABLE"
				});
			}
		}
		Property property2 = base.Properties.Get("TrackColumnsUpdatedEnabled");
		if (!property2.IsNull && (property2.Dirty || !sp.ScriptForAlter))
		{
			if (!flag)
			{
				if ((bool)property2.Value)
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.TrackColumnsException);
				}
			}
			else
			{
				if (stringBuilder.Length == 0)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} ENABLE CHANGE_TRACKING ", new object[1] { FormatFullNameForScripting(sp) });
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WITH(TRACK_COLUMNS_UPDATED = {0})", new object[1] { ((bool)property2.Value) ? "ON" : "OFF" });
			}
		}
		if (stringBuilder.Length > 0)
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, stringBuilder.ToString()));
		}
	}

	private void ScriptSystemVersioning(StringCollection query, ScriptingPreferences sp)
	{
		Property property = base.Properties.Get("IsSystemVersioned");
		Property property2 = base.Properties.Get("HistoryTableName");
		Property property3 = base.Properties.Get("HistoryTableSchema");
		Property property4 = null;
		Property property5 = null;
		bool flag = false;
		bool flag2 = false;
		if (IsSupportedProperty("HistoryRetentionPeriod") && IsSupportedProperty("HistoryRetentionPeriodUnit"))
		{
			property4 = base.Properties.Get("HistoryRetentionPeriod");
			property5 = base.Properties.Get("HistoryRetentionPeriodUnit");
			flag = property4.Dirty;
			flag2 = property5.Dirty;
		}
		bool flag3 = false;
		string text = string.Empty;
		string s = string.Empty;
		if (property.IsNull)
		{
			return;
		}
		flag3 = (bool)property.Value;
		if (!property.Dirty && !flag && !flag2)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text2 = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text2, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE90, new object[2]
				{
					"",
					SqlSmoObject.SqlString(text2)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_TABLE80, new object[2]
				{
					"",
					SqlSmoObject.SqlString(text2)
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		ScriptStringBuilder scriptStringBuilder = new ScriptStringBuilder(string.Format("SYSTEM_VERSIONING = {0}", flag3 ? "ON" : "OFF"));
		if (flag3)
		{
			if (!property2.IsNull)
			{
				text = property2.Value.ToString();
			}
			if (!property3.IsNull)
			{
				s = property3.Value.ToString();
			}
			if (property2.IsNull != property3.IsNull)
			{
				throw new SmoException(ExceptionTemplatesImpl.BothHistoryTableNameAndSchemaMustBeProvided);
			}
			if (!string.IsNullOrEmpty(text))
			{
				string arg = SqlSmoObject.MakeSqlBraket(s);
				string arg2 = SqlSmoObject.MakeSqlBraket(text);
				scriptStringBuilder.SetParameter("HISTORY_TABLE", $"{arg}.{arg2}", ParameterValueFormat.NotString);
			}
			if (!m_DataConsistencyCheckForSystemVersionedTable)
			{
				scriptStringBuilder.SetParameter("DATA_CONSISTENCY_CHECK", "OFF", ParameterValueFormat.NotString);
			}
			bool flag4 = property4 == null || property4.IsNull;
			bool flag5 = property5 == null || property5.IsNull;
			int num = ((!flag4) ? ((int)property4.Value) : 0);
			TemporalHistoryRetentionPeriodUnit temporalHistoryRetentionPeriodUnit = (flag5 ? TemporalHistoryRetentionPeriodUnit.Undefined : ((TemporalHistoryRetentionPeriodUnit)property5.Value));
			if ((!flag4 || !flag5) && (num != 0 || temporalHistoryRetentionPeriodUnit != TemporalHistoryRetentionPeriodUnit.Undefined))
			{
				switch (temporalHistoryRetentionPeriodUnit)
				{
				case TemporalHistoryRetentionPeriodUnit.Undefined:
					throw new SmoException(ExceptionTemplatesImpl.InvalidHistoryRetentionPeriodUnitSpecification);
				case TemporalHistoryRetentionPeriodUnit.Infinite:
					if (num != -1)
					{
						throw new SmoException(ExceptionTemplatesImpl.InvalidHistoryRetentionPeriodSpecification);
					}
					break;
				default:
				{
					if (num < 1)
					{
						throw new SmoException(ExceptionTemplatesImpl.InvalidHistoryRetentionPeriodSpecification);
					}
					TemporalHistoryRetentionPeriodUnitTypeConverter temporalHistoryRetentionPeriodUnitTypeConverter = new TemporalHistoryRetentionPeriodUnitTypeConverter();
					string text3 = temporalHistoryRetentionPeriodUnitTypeConverter.ConvertToInvariantString(temporalHistoryRetentionPeriodUnit);
					scriptStringBuilder.SetParameter("HISTORY_RETENTION_PERIOD", string.Format(SmoApplication.DefaultCulture, "{0} {1}", new object[2]
					{
						num.ToString(),
						text3
					}), ParameterValueFormat.NotString);
					break;
				}
				}
			}
		}
		stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} SET ( {1} )", new object[2]
		{
			FormatFullNameForScripting(sp),
			scriptStringBuilder.ToString(scriptSemiColon: false)
		}));
		query.Add(stringBuilder.ToString());
	}

	private void ScriptSystemTimePeriodForAlter(StringCollection query, ScriptingPreferences sp)
	{
		ValidateSystemTimeTemporal();
		if (HasSystemTimePeriod)
		{
			if (!m_systemTimePeriodInfo.m_MarkedForCreate && m_systemTimePeriodInfo.m_MarkedForDrop)
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} DROP PERIOD FOR SYSTEM_TIME", new object[1] { FormatFullNameForScripting(sp) }));
			}
		}
		else if (m_systemTimePeriodInfo.m_MarkedForCreate)
		{
			query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} ADD PERIOD FOR SYSTEM_TIME ( [{1}], [{2}] )", new object[3]
			{
				FormatFullNameForScripting(sp),
				Util.EscapeString(m_systemTimePeriodInfo.m_StartColumnName, ']'),
				Util.EscapeString(m_systemTimePeriodInfo.m_EndColumnName, ']')
			}));
		}
	}

	private void ScriptRemoteDataArchive(StringCollection queries, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			return;
		}
		Property property = base.Properties.Get("RemoteDataArchiveEnabled");
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (!property.IsNull)
		{
			flag = (bool)property.Value;
			Property property2 = base.Properties.Get("RemoteDataArchiveDataMigrationState");
			Property property3 = base.Properties.Get("RemoteDataArchiveFilterPredicate");
			if (((property.Dirty || property2.Dirty || property3.Dirty) && sp.ScriptForAlter) || (flag && !sp.ScriptForAlter))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.LParen);
				RemoteDataArchiveMigrationState remoteDataArchiveMigrationState = (RemoteDataArchiveMigrationState)property2.Value;
				string text;
				switch (remoteDataArchiveMigrationState)
				{
				case RemoteDataArchiveMigrationState.PausedOutbound:
				case RemoteDataArchiveMigrationState.PausedInbound:
					text = "PAUSED";
					break;
				case RemoteDataArchiveMigrationState.Outbound:
					text = "OUTBOUND";
					break;
				case RemoteDataArchiveMigrationState.Inbound:
					text = "INBOUND";
					break;
				default:
					text = "PAUSED";
					break;
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "MIGRATION_STATE = {0}", new object[1] { text });
				if (!property3.IsNull && (remoteDataArchiveMigrationState == RemoteDataArchiveMigrationState.Outbound || remoteDataArchiveMigrationState == RemoteDataArchiveMigrationState.PausedOutbound))
				{
					string text2 = (string)property3.Value;
					if (!string.IsNullOrEmpty(text2))
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", FILTER_PREDICATE = {0}", new object[1] { text2 });
					}
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Globals.RParen);
			}
		}
		if (stringBuilder.Length > 0)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} {1}(REMOTE_DATA_ARCHIVE = {2} {3})", FormatFullNameForScripting(sp), Scripts.SET, flag ? Globals.On : Scripts.OFF_WITHOUT_DATA_RECOVERY, stringBuilder.ToString()));
		}
	}

	internal void ScriptLockGranularity(StringCollection scqueries, LockEscalationType lockStatus, ScriptingPreferences sp, bool scriptTableGranularity)
	{
		if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER TABLE {0} SET ", new object[1] { FormatFullNameForScripting(sp) });
		stringBuilder.Append(Globals.LParen);
		if (Enum.IsDefined(typeof(LockEscalationType), lockStatus))
		{
			switch (lockStatus)
			{
			case LockEscalationType.Auto:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "LOCK_ESCALATION = {0}", new object[1] { "AUTO" });
				break;
			case LockEscalationType.Disable:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "LOCK_ESCALATION = {0}", new object[1] { "DISABLE" });
				break;
			case LockEscalationType.Table:
				if (scriptTableGranularity)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "LOCK_ESCALATION = {0}", new object[1] { "TABLE" });
				}
				else
				{
					stringBuilder.Length = 0;
				}
				break;
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(Globals.RParen);
				scqueries.Add(string.Format(SmoApplication.DefaultCulture, stringBuilder.ToString()));
			}
			return;
		}
		throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("LockEscalationType"));
	}

	internal void ScriptLockEscalationSettings(StringCollection scqueries, ScriptingPreferences sp)
	{
		Property property = properties.Get("LockEscalation");
		LockEscalationType lockEscalationType = LockEscalationType.Table;
		if (!property.IsNull)
		{
			lockEscalationType = (LockEscalationType)property.Value;
			if (property.Dirty)
			{
				ScriptLockGranularity(scqueries, lockEscalationType, sp, scriptTableGranularity: true);
			}
			else if (!sp.ScriptForAlter)
			{
				ScriptLockGranularity(scqueries, lockEscalationType, sp, scriptTableGranularity: false);
			}
		}
	}

	internal override void ScriptCreateInternal(StringCollection query, ScriptingPreferences sp)
	{
		indexPropagationList = null;
		embeddedForeignKeyChecksList = null;
		ScriptMaker scriptMaker = new ScriptMaker(GetServerObject());
		scriptMaker.Preferences = sp;
		StringCollection stringCollection = scriptMaker.Script(new SqlSmoObject[1] { this });
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				query.Add(current);
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void ValidateExternalTable()
	{
		if (CheckIsExternalTable() && base.State == SqlSmoState.Creating)
		{
			if (Checks.Count > 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.ExternalTableCannotContainChecks);
			}
			if (ForeignKeys.Count > 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.ExternalTableCannotContainForeignKeys);
			}
			if (PartitionSchemeParameters.Count > 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.ExternalTableCannotContainPartitionSchemeParameters);
			}
			if (base.Triggers.Count > 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.ExternalTableCannotContainTriggers);
			}
			if (Indexes.Count > 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.ExternalTableCannotContainIndexes);
			}
			if (PhysicalPartitions.Count > 0)
			{
				throw new SmoException(ExceptionTemplatesImpl.ExternalTableCannotContainPhysicalPartitions);
			}
		}
	}

	private void ValidateIndexes()
	{
		bool flag = CheckIsMemoryOptimizedTable();
		bool flag2 = CheckIsSqlDwTable();
		foreach (Index index in Indexes)
		{
			IndexType indexType;
			try
			{
				indexType = index.IndexType;
			}
			catch
			{
				indexType = index.InferredIndexType;
			}
			if (indexType == IndexType.NonClusteredHashIndex && !flag)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.HashIndexTableDependency);
			}
			if (flag && indexType != IndexType.NonClusteredHashIndex && indexType != IndexType.NonClusteredIndex && indexType != IndexType.ClusteredColumnStoreIndex)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.TableMemoryOptimizedIndexDependency);
			}
			if (flag2 && indexType != IndexType.HeapIndex && indexType != IndexType.ClusteredColumnStoreIndex && indexType != IndexType.ClusteredIndex && indexType != IndexType.NonClusteredIndex)
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.TableSqlDwIndexRestrictions);
			}
		}
	}

	private void ValidateExternalTableRequiredStringProperty(string propertyName, ScriptingPreferences sp)
	{
		if (IsSupportedProperty(propertyName, sp))
		{
			Property propertyOptional = GetPropertyOptional(propertyName);
			if (propertyOptional.IsNull)
			{
				throw new ArgumentNullException(propertyName);
			}
			if (string.IsNullOrEmpty(propertyOptional.Value.ToString()))
			{
				throw new PropertyNotSetException(propertyName);
			}
			if (!CheckIsExternalTable())
			{
				throw new SmoException(ExceptionTemplatesImpl.PropertyOnlySupportedForExternalTable(propertyName));
			}
		}
	}

	private void ValidateSystemTimeTemporal()
	{
		if (!m_systemTimePeriodInfo.m_MarkedForCreate)
		{
			return;
		}
		Column column;
		if ((column = base.Columns[m_systemTimePeriodInfo.m_StartColumnName]) == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.MustProvideExistingColumn);
		}
		Column column2;
		if ((column2 = base.Columns[m_systemTimePeriodInfo.m_EndColumnName]) == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.MustProvideExistingColumn);
		}
		if (column == column2)
		{
			throw new SmoException(ExceptionTemplatesImpl.PeriodMustHaveDifferentColumns);
		}
		if (column.DataType.SqlDataType != SqlDataType.DateTime2)
		{
			throw new SmoException(ExceptionTemplatesImpl.PeriodInvalidDataType);
		}
		if (column2.DataType.SqlDataType != SqlDataType.DateTime2)
		{
			throw new SmoException(ExceptionTemplatesImpl.PeriodInvalidDataType);
		}
		if (base.State == SqlSmoState.Creating)
		{
			if (column.GetPropValueOptional("GeneratedAlwaysType", GeneratedAlwaysType.None) != GeneratedAlwaysType.AsRowStart)
			{
				throw new SmoException(ExceptionTemplatesImpl.PeriodStartColumnMustBeGeneratedAlways);
			}
			if (column2.GetPropValueOptional("GeneratedAlwaysType", GeneratedAlwaysType.None) != GeneratedAlwaysType.AsRowEnd)
			{
				throw new SmoException(ExceptionTemplatesImpl.PeriodEndColumnMustBeGeneratedAlways);
			}
		}
	}
}

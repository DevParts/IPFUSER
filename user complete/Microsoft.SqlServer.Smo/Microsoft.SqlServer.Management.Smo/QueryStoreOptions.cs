using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("QueryStoreOptions")]
public sealed class QueryStoreOptions : SqlSmoObject, ISfcSupportsDesignMode, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 10, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 10 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("ActualState", expensive: false, readOnly: true, typeof(QueryStoreOperationMode)),
			new StaticMetadata("CurrentStorageSizeInMB", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("DataFlushIntervalInSeconds", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("DesiredState", expensive: false, readOnly: false, typeof(QueryStoreOperationMode)),
			new StaticMetadata("MaxStorageSizeInMB", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("QueryCaptureMode", expensive: false, readOnly: false, typeof(QueryStoreCaptureMode)),
			new StaticMetadata("ReadOnlyReason", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SizeBasedCleanupMode", expensive: false, readOnly: false, typeof(QueryStoreSizeBasedCleanupMode)),
			new StaticMetadata("StaleQueryThresholdInDays", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("StatisticsCollectionIntervalInMinutes", expensive: false, readOnly: false, typeof(long))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("ActualState", expensive: false, readOnly: true, typeof(QueryStoreOperationMode)),
			new StaticMetadata("CurrentStorageSizeInMB", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("DataFlushIntervalInSeconds", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("DesiredState", expensive: false, readOnly: false, typeof(QueryStoreOperationMode)),
			new StaticMetadata("MaxStorageSizeInMB", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("QueryCaptureMode", expensive: false, readOnly: false, typeof(QueryStoreCaptureMode)),
			new StaticMetadata("ReadOnlyReason", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SizeBasedCleanupMode", expensive: false, readOnly: false, typeof(QueryStoreSizeBasedCleanupMode)),
			new StaticMetadata("StaleQueryThresholdInDays", expensive: false, readOnly: false, typeof(long)),
			new StaticMetadata("StatisticsCollectionIntervalInMinutes", expensive: false, readOnly: false, typeof(long))
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
					"ActualState" => 0, 
					"CurrentStorageSizeInMB" => 1, 
					"DataFlushIntervalInSeconds" => 2, 
					"DesiredState" => 3, 
					"MaxStorageSizeInMB" => 4, 
					"QueryCaptureMode" => 5, 
					"ReadOnlyReason" => 6, 
					"SizeBasedCleanupMode" => 7, 
					"StaleQueryThresholdInDays" => 8, 
					"StatisticsCollectionIntervalInMinutes" => 9, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ActualState" => 0, 
				"CurrentStorageSizeInMB" => 1, 
				"DataFlushIntervalInSeconds" => 2, 
				"DesiredState" => 3, 
				"MaxStorageSizeInMB" => 4, 
				"QueryCaptureMode" => 5, 
				"ReadOnlyReason" => 6, 
				"SizeBasedCleanupMode" => 7, 
				"StaleQueryThresholdInDays" => 8, 
				"StatisticsCollectionIntervalInMinutes" => 9, 
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
	public QueryStoreOperationMode ActualState => (QueryStoreOperationMode)base.Properties.GetValueWithNullReplacement("ActualState");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public long CurrentStorageSizeInMB
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("CurrentStorageSizeInMB");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CurrentStorageSizeInMB", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public long DataFlushIntervalInSeconds
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("DataFlushIntervalInSeconds");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataFlushIntervalInSeconds", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public QueryStoreOperationMode DesiredState
	{
		get
		{
			return (QueryStoreOperationMode)base.Properties.GetValueWithNullReplacement("DesiredState");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DesiredState", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public long MaxStorageSizeInMB
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("MaxStorageSizeInMB");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaxStorageSizeInMB", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public QueryStoreCaptureMode QueryCaptureMode
	{
		get
		{
			return (QueryStoreCaptureMode)base.Properties.GetValueWithNullReplacement("QueryCaptureMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QueryCaptureMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ReadOnlyReason
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ReadOnlyReason");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReadOnlyReason", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public QueryStoreSizeBasedCleanupMode SizeBasedCleanupMode
	{
		get
		{
			return (QueryStoreSizeBasedCleanupMode)base.Properties.GetValueWithNullReplacement("SizeBasedCleanupMode");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("SizeBasedCleanupMode", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public long StaleQueryThresholdInDays
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("StaleQueryThresholdInDays");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StaleQueryThresholdInDays", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public long StatisticsCollectionIntervalInMinutes
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("StatisticsCollectionIntervalInMinutes");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StatisticsCollectionIntervalInMinutes", value);
		}
	}

	internal static string UrnSuffix => "QueryStoreOptions";

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent => singletonParent as Database;

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal QueryStoreOptions(Database parentdb, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentdb;
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		if (Parent.Status == DatabaseStatus.Restoring)
		{
			Property property = base.Properties.Get("DesiredState");
			Property property2 = base.Properties.Get("ActualState");
			Property property3 = base.Properties.Get("StaleQueryThresholdInDays");
			Property property4 = base.Properties.Get("DataFlushIntervalInSeconds");
			Property property5 = base.Properties.Get("StatisticsCollectionIntervalInMinutes");
			Property property6 = base.Properties.Get("MaxStorageSizeInMB");
			Property property7 = base.Properties.Get("QueryCaptureMode");
			Property property8 = base.Properties.Get("SizeBasedCleanupMode");
			if ((property != null && property.Dirty) || (property2 != null && property2.Dirty) || (property3 != null && property3.Dirty) || (property4 != null && property4.Dirty) || (property5 != null && property5.Dirty) || (property6 != null && property6.Dirty) || (property7 != null && property7.Dirty) || (property8 != null && property8.Dirty))
			{
				throw new InvalidSmoOperationException(ExceptionTemplatesImpl.AlterQueryStorePropertyForRestoringDatabase);
			}
		}
		else
		{
			ScriptQueryStoreOptions(query, sp, scriptAll: false);
		}
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		ScriptQueryStoreOptions(query, sp, scriptAll: true);
	}

	private void ScriptQueryStoreOptions(StringCollection query, ScriptingPreferences sp, bool scriptAll)
	{
		CheckObjectState();
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		Property propertyOptional = GetPropertyOptional("DesiredState");
		if (propertyOptional.Value == null)
		{
			return;
		}
		Property propertyOptional2 = GetPropertyOptional("ActualState");
		Property propertyOptional3 = GetPropertyOptional("StaleQueryThresholdInDays");
		Property propertyOptional4 = GetPropertyOptional("DataFlushIntervalInSeconds");
		Property propertyOptional5 = GetPropertyOptional("StatisticsCollectionIntervalInMinutes");
		Property propertyOptional6 = GetPropertyOptional("MaxStorageSizeInMB");
		Property propertyOptional7 = GetPropertyOptional("QueryCaptureMode");
		Property propertyOptional8 = GetPropertyOptional("SizeBasedCleanupMode");
		QueryStoreOperationMode queryStoreOperationMode = (QueryStoreOperationMode)propertyOptional.Value;
		QueryStoreOperationMode queryStoreOperationMode2 = ((!propertyOptional2.IsNull) ? ((QueryStoreOperationMode)propertyOptional2.Value) : QueryStoreOperationMode.Off);
		if (scriptAll || (propertyOptional.Dirty && queryStoreOperationMode == QueryStoreOperationMode.Off != (queryStoreOperationMode2 == QueryStoreOperationMode.Off)))
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET QUERY_STORE = {1}", new object[2]
			{
				SqlSmoObject.SqlBraket(Parent.Name),
				(queryStoreOperationMode != QueryStoreOperationMode.Off) ? "ON" : "OFF"
			}));
			query.Add(stringBuilder.ToString());
		}
		if (queryStoreOperationMode == QueryStoreOperationMode.Off)
		{
			return;
		}
		stringBuilder = new StringBuilder();
		if (scriptAll || propertyOptional.Dirty || propertyOptional3.Dirty || propertyOptional4.Dirty || propertyOptional5.Dirty || propertyOptional6.Dirty || propertyOptional7.Dirty || propertyOptional8.Dirty)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET QUERY_STORE (", new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "OPERATION_MODE = {0}, ", new object[1] { queryStoreOperationMode.ToSyntaxString() }));
			if (scriptAll || propertyOptional3.Dirty)
			{
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = {0}), ", new object[1] { (long)propertyOptional3.Value }));
			}
			if (scriptAll || propertyOptional4.Dirty)
			{
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "DATA_FLUSH_INTERVAL_SECONDS = {0}, ", new object[1] { (long)propertyOptional4.Value }));
			}
			if (scriptAll || propertyOptional5.Dirty)
			{
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "INTERVAL_LENGTH_MINUTES = {0}, ", new object[1] { (long)propertyOptional5.Value }));
			}
			if (scriptAll || propertyOptional6.Dirty)
			{
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "MAX_STORAGE_SIZE_MB = {0}, ", new object[1] { (long)propertyOptional6.Value }));
			}
			if (scriptAll || propertyOptional7.Dirty)
			{
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "QUERY_CAPTURE_MODE = {0}, ", new object[1] { ((QueryStoreCaptureMode)propertyOptional7.Value).ToSyntaxString() }));
			}
			if (scriptAll || propertyOptional8.Dirty)
			{
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "SIZE_BASED_CLEANUP_MODE = {0}, ", new object[1] { ((QueryStoreSizeBasedCleanupMode)propertyOptional8.Value).ToSyntaxString() }));
			}
			stringBuilder.Length -= 2;
			stringBuilder.Append(")");
		}
		query.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void PurgeQueryStoreData()
	{
		try
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(QueryStoreOptions));
			Parent.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET QUERY_STORE CLEAR ALL", new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetQueryStoreOptions, Parent, ex);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[8] { "ActualState", "DataFlushIntervalInSeconds", "DesiredState", "MaxStorageSizeInMB", "QueryCaptureMode", "SizeBasedCleanupMode", "StaleQueryThresholdInDays", "StatisticsCollectionIntervalInMinutes" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

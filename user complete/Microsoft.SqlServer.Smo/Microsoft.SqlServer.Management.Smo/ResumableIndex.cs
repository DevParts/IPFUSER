using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class ResumableIndex : NamedSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 10 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("LastPauseTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("MaxDOP", expensive: false, readOnly: false, typeof(short)),
			new StaticMetadata("PageCount", expensive: false, readOnly: true, typeof(long)),
			new StaticMetadata("PartitionNumber", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PercentComplete", expensive: false, readOnly: true, typeof(double)),
			new StaticMetadata("ResumableOperationState", expensive: false, readOnly: true, typeof(ResumableOperationStateType)),
			new StaticMetadata("SqlText", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("StartTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("TotalExecutionTime", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("LastPauseTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("MaxDOP", expensive: false, readOnly: false, typeof(short)),
			new StaticMetadata("PageCount", expensive: false, readOnly: true, typeof(long)),
			new StaticMetadata("PartitionNumber", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PercentComplete", expensive: false, readOnly: true, typeof(double)),
			new StaticMetadata("ResumableOperationState", expensive: false, readOnly: true, typeof(ResumableOperationStateType)),
			new StaticMetadata("SqlText", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("StartTime", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("TotalExecutionTime", expensive: false, readOnly: true, typeof(int))
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
					"LastPauseTime" => 1, 
					"MaxDOP" => 2, 
					"PageCount" => 3, 
					"PartitionNumber" => 4, 
					"PercentComplete" => 5, 
					"ResumableOperationState" => 6, 
					"SqlText" => 7, 
					"StartTime" => 8, 
					"TotalExecutionTime" => 9, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"LastPauseTime" => 1, 
				"MaxDOP" => 2, 
				"PageCount" => 3, 
				"PartitionNumber" => 4, 
				"PercentComplete" => 5, 
				"ResumableOperationState" => 6, 
				"SqlText" => 7, 
				"StartTime" => 8, 
				"TotalExecutionTime" => 9, 
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

	private int resumableMaxDuration;

	private int lowPriorityMaxDuration;

	private AbortAfterWait lowPriorityAbortAfterWait;

	[SfcParent("View")]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime LastPauseTime => (DateTime)base.Properties.GetValueWithNullReplacement("LastPauseTime");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short MaxDOP
	{
		get
		{
			return (short)base.Properties.GetValueWithNullReplacement("MaxDOP");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaxDOP", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public long PageCount => (long)base.Properties.GetValueWithNullReplacement("PageCount");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int PartitionNumber => (int)base.Properties.GetValueWithNullReplacement("PartitionNumber");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public double PercentComplete => (double)base.Properties.GetValueWithNullReplacement("PercentComplete");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ResumableOperationStateType ResumableOperationState => (ResumableOperationStateType)base.Properties.GetValueWithNullReplacement("ResumableOperationState");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string SqlText => (string)base.Properties.GetValueWithNullReplacement("SqlText");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime StartTime => (DateTime)base.Properties.GetValueWithNullReplacement("StartTime");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int TotalExecutionTime => (int)base.Properties.GetValueWithNullReplacement("TotalExecutionTime");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ResumableMaxDuration
	{
		get
		{
			return resumableMaxDuration;
		}
		set
		{
			resumableMaxDuration = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int LowPriorityMaxDuration
	{
		get
		{
			return lowPriorityMaxDuration;
		}
		set
		{
			lowPriorityMaxDuration = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public AbortAfterWait LowPriorityAbortAfterWait
	{
		get
		{
			return lowPriorityAbortAfterWait;
		}
		set
		{
			lowPriorityAbortAfterWait = value;
		}
	}

	public static string UrnSuffix => "ResumableIndex";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ResumableIndex(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Abort()
	{
		PauseOrAbort(isAbort: true);
	}

	public void Pause()
	{
		PauseOrAbort(isAbort: false);
	}

	public void Resume()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			scriptingPreferences.ScriptForCreateDrop = true;
			GetContextDB().AddUseDb(stringCollection, scriptingPreferences);
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX {0} {1} {2} RESUME", new object[3]
			{
				SqlSmoObject.MakeSqlBraket(Name),
				Globals.On,
				Parent.FullQualifiedName
			});
			if (ResumableMaxDuration != 0 || LowPriorityAbortAfterWait != AbortAfterWait.None || MaxDOP != 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " {0} (MAXDOP = {1}", new object[2]
				{
					Globals.With,
					MaxDOP
				});
				if (ResumableMaxDuration != 0)
				{
					stringBuilder.Append(Globals.commaspace);
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "MAX_DURATION = {0} MINUTES", new object[1] { ResumableMaxDuration });
				}
				if (LowPriorityAbortAfterWait != AbortAfterWait.None)
				{
					AbortAfterWaitConverter abortAfterWaitConverter = new AbortAfterWaitConverter();
					stringBuilder.Append(Globals.commaspace);
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WAIT_AT_LOW_PRIORITY (MAX_DURATION = {0} MINUTES, ABORT_AFTER_WAIT = {1})", new object[2]
					{
						LowPriorityMaxDuration,
						abortAfterWaitConverter.ConvertToInvariantString(LowPriorityAbortAfterWait)
					});
				}
				stringBuilder.Append(Globals.RParen);
			}
			stringBuilder.Append(";");
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ResumeIndexes, this, ex);
		}
	}

	private void PauseOrAbort(bool isAbort)
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
			scriptingPreferences.ScriptForCreateDrop = true;
			GetContextDB().AddUseDb(stringCollection, scriptingPreferences);
			StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER INDEX {0} ON {1} {2}", new object[3]
			{
				SqlSmoObject.MakeSqlBraket(Name),
				Parent.FullQualifiedName,
				isAbort ? "ABORT" : "PAUSE"
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			if (isAbort)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.AbortIndexes, this, ex);
			}
			throw new FailedOperationException(ExceptionTemplatesImpl.PauseIndexes, this, ex);
		}
	}
}

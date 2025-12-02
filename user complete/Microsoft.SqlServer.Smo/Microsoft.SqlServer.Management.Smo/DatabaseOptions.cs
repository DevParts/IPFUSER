using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[SfcElementType("Option")]
public sealed class DatabaseOptions : SqlSmoObject, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 13, 20, 28, 28, 28, 28, 29, 29, 29, 29 };

		private static int[] cloudVersionCount = new int[3] { 26, 26, 26 };

		private static int sqlDwPropertyCount = 26;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[26]
		{
			new StaticMetadata("AnsiNullDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPaddingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarningsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ArithmeticAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoClose", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatistics", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoShrink", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatistics", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsAsync", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BrokerEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CloseCursorsOnCommitEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseOwnershipChaining", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DateCorrelationOptimization", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsParameterizationForced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("LocalCursorsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericRoundAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PageVerify", expensive: false, readOnly: false, typeof(PageVerify)),
			new StaticMetadata("QuotedIdentifiersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RecoveryModel", expensive: false, readOnly: false, typeof(RecoveryModel)),
			new StaticMetadata("RecursiveTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SnapshotIsolationState", expensive: false, readOnly: true, typeof(SnapshotIsolationState)),
			new StaticMetadata("Trustworthy", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserAccess", expensive: false, readOnly: false, typeof(DatabaseUserAccess))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[26]
		{
			new StaticMetadata("AnsiNullDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiPaddingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarningsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ArithmeticAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoClose", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatistics", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoShrink", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatistics", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsAsync", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BrokerEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CloseCursorsOnCommitEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseOwnershipChaining", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DateCorrelationOptimization", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsParameterizationForced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("LocalCursorsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericRoundAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PageVerify", expensive: false, readOnly: false, typeof(PageVerify)),
			new StaticMetadata("QuotedIdentifiersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RecoveryModel", expensive: false, readOnly: false, typeof(RecoveryModel)),
			new StaticMetadata("RecursiveTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("SnapshotIsolationState", expensive: false, readOnly: true, typeof(SnapshotIsolationState)),
			new StaticMetadata("Trustworthy", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserAccess", expensive: false, readOnly: false, typeof(DatabaseUserAccess))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[29]
		{
			new StaticMetadata("AnsiNullDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiNullsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AnsiWarningsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoClose", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatistics", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoShrink", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatistics", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("CloseCursorsOnCommitEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("LocalCursorsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifiersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("RecoveryModel", expensive: false, readOnly: false, typeof(RecoveryModel)),
			new StaticMetadata("RecursiveTriggersEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UserAccess", expensive: false, readOnly: false, typeof(DatabaseUserAccess)),
			new StaticMetadata("AnsiPaddingEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ArithmeticAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ConcatenateNullYieldsNull", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DatabaseOwnershipChaining", expensive: true, readOnly: false, typeof(bool)),
			new StaticMetadata("NumericRoundAbortEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PageVerify", expensive: false, readOnly: false, typeof(PageVerify)),
			new StaticMetadata("ReadOnly", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoUpdateStatisticsAsync", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BrokerEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DateCorrelationOptimization", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsParameterizationForced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MirroringRedoQueueMaxSize", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("MirroringTimeout", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("SnapshotIsolationState", expensive: false, readOnly: true, typeof(SnapshotIsolationState)),
			new StaticMetadata("Trustworthy", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AutoCreateStatisticsIncremental", expensive: false, readOnly: false, typeof(bool))
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
						"AnsiNullDefault" => 0, 
						"AnsiNullsEnabled" => 1, 
						"AnsiPaddingEnabled" => 2, 
						"AnsiWarningsEnabled" => 3, 
						"ArithmeticAbortEnabled" => 4, 
						"AutoClose" => 5, 
						"AutoCreateStatistics" => 6, 
						"AutoShrink" => 7, 
						"AutoUpdateStatistics" => 8, 
						"AutoUpdateStatisticsAsync" => 9, 
						"BrokerEnabled" => 10, 
						"CloseCursorsOnCommitEnabled" => 11, 
						"ConcatenateNullYieldsNull" => 12, 
						"DatabaseOwnershipChaining" => 13, 
						"DateCorrelationOptimization" => 14, 
						"IsParameterizationForced" => 15, 
						"LocalCursorsDefault" => 16, 
						"NumericRoundAbortEnabled" => 17, 
						"PageVerify" => 18, 
						"QuotedIdentifiersEnabled" => 19, 
						"ReadOnly" => 20, 
						"RecoveryModel" => 21, 
						"RecursiveTriggersEnabled" => 22, 
						"SnapshotIsolationState" => 23, 
						"Trustworthy" => 24, 
						"UserAccess" => 25, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"AnsiNullDefault" => 0, 
					"AnsiNullsEnabled" => 1, 
					"AnsiPaddingEnabled" => 2, 
					"AnsiWarningsEnabled" => 3, 
					"ArithmeticAbortEnabled" => 4, 
					"AutoClose" => 5, 
					"AutoCreateStatistics" => 6, 
					"AutoShrink" => 7, 
					"AutoUpdateStatistics" => 8, 
					"AutoUpdateStatisticsAsync" => 9, 
					"BrokerEnabled" => 10, 
					"CloseCursorsOnCommitEnabled" => 11, 
					"ConcatenateNullYieldsNull" => 12, 
					"DatabaseOwnershipChaining" => 13, 
					"DateCorrelationOptimization" => 14, 
					"IsParameterizationForced" => 15, 
					"LocalCursorsDefault" => 16, 
					"NumericRoundAbortEnabled" => 17, 
					"PageVerify" => 18, 
					"QuotedIdentifiersEnabled" => 19, 
					"ReadOnly" => 20, 
					"RecoveryModel" => 21, 
					"RecursiveTriggersEnabled" => 22, 
					"SnapshotIsolationState" => 23, 
					"Trustworthy" => 24, 
					"UserAccess" => 25, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiNullDefault" => 0, 
				"AnsiNullsEnabled" => 1, 
				"AnsiWarningsEnabled" => 2, 
				"AutoClose" => 3, 
				"AutoCreateStatistics" => 4, 
				"AutoShrink" => 5, 
				"AutoUpdateStatistics" => 6, 
				"CloseCursorsOnCommitEnabled" => 7, 
				"LocalCursorsDefault" => 8, 
				"QuotedIdentifiersEnabled" => 9, 
				"RecoveryModel" => 10, 
				"RecursiveTriggersEnabled" => 11, 
				"UserAccess" => 12, 
				"AnsiPaddingEnabled" => 13, 
				"ArithmeticAbortEnabled" => 14, 
				"ConcatenateNullYieldsNull" => 15, 
				"DatabaseOwnershipChaining" => 16, 
				"NumericRoundAbortEnabled" => 17, 
				"PageVerify" => 18, 
				"ReadOnly" => 19, 
				"AutoUpdateStatisticsAsync" => 20, 
				"BrokerEnabled" => 21, 
				"DateCorrelationOptimization" => 22, 
				"IsParameterizationForced" => 23, 
				"MirroringRedoQueueMaxSize" => 24, 
				"MirroringTimeout" => 25, 
				"SnapshotIsolationState" => 26, 
				"Trustworthy" => 27, 
				"AutoCreateStatisticsIncremental" => 28, 
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

	internal class OptionTerminationStatement
	{
		private TimeSpan m_time;

		private TerminationClause m_clause;

		internal OptionTerminationStatement(TimeSpan time)
		{
			m_time = time;
		}

		internal OptionTerminationStatement(TerminationClause clause)
		{
			m_time = TimeSpan.Zero;
			m_clause = clause;
		}

		internal string GetTerminationScript()
		{
			if (TimeSpan.Zero != m_time)
			{
				return string.Format(SmoApplication.DefaultCulture, "WITH ROLLBACK AFTER {0} SECONDS", new object[1] { m_time.Seconds });
			}
			if (m_clause == TerminationClause.FailOnOpenTransactions)
			{
				return "WITH NO_WAIT";
			}
			return "WITH ROLLBACK IMMEDIATE";
		}
	}

	private OptionTerminationStatement m_OptionTerminationStatement;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent => singletonParent as Database;

	public new SqlPropertyCollection Properties => Parent.Properties;

	public static string UrnSuffix => "Option";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool DatabaseOwnershipChaining
	{
		get
		{
			ThrowIfBelowVersion80SP3();
			return (bool)Parent.Properties["DatabaseOwnershipChaining"].Value;
		}
		set
		{
			ThrowIfBelowVersion80SP3();
			Parent.Properties.Get("DatabaseOwnershipChaining").Value = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AnsiNullDefault
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AnsiNullDefault");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AnsiNullDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AnsiNullsEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AnsiNullsEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AnsiNullsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AnsiPaddingEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AnsiPaddingEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AnsiPaddingEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AnsiWarningsEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AnsiWarningsEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AnsiWarningsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ArithmeticAbortEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("ArithmeticAbortEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("ArithmeticAbortEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AutoClose
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AutoClose");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AutoClose", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AutoCreateStatistics
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AutoCreateStatisticsEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AutoCreateStatisticsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AutoCreateStatisticsIncremental
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AutoCreateIncrementalStatisticsEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AutoCreateIncrementalStatisticsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AutoShrink
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AutoShrink");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AutoShrink", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AutoUpdateStatistics
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AutoUpdateStatisticsEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AutoUpdateStatisticsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AutoUpdateStatisticsAsync
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("AutoUpdateStatisticsAsync");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("AutoUpdateStatisticsAsync", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool BrokerEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("BrokerEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("BrokerEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool CloseCursorsOnCommitEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("CloseCursorsOnCommitEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("CloseCursorsOnCommitEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ConcatenateNullYieldsNull
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("ConcatenateNullYieldsNull");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("ConcatenateNullYieldsNull", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool DateCorrelationOptimization
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("DateCorrelationOptimization");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("DateCorrelationOptimization", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsParameterizationForced
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("IsParameterizationForced");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("IsParameterizationForced", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool LocalCursorsDefault
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("LocalCursorsDefault");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("LocalCursorsDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MirroringRedoQueueMaxSize => (int)Parent.Properties.GetValueWithNullReplacement("MirroringRedoQueueMaxSize");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MirroringTimeout
	{
		get
		{
			return (int)Parent.Properties.GetValueWithNullReplacement("MirroringTimeout");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("MirroringTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool NumericRoundAbortEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("NumericRoundAbortEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("NumericRoundAbortEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public PageVerify PageVerify
	{
		get
		{
			return (PageVerify)Parent.Properties.GetValueWithNullReplacement("PageVerify");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("PageVerify", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool QuotedIdentifiersEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("QuotedIdentifiersEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("QuotedIdentifiersEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ReadOnly
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("ReadOnly");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("ReadOnly", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public RecoveryModel RecoveryModel
	{
		get
		{
			return (RecoveryModel)Parent.Properties.GetValueWithNullReplacement("RecoveryModel");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("RecoveryModel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool RecursiveTriggersEnabled
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("RecursiveTriggersEnabled");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("RecursiveTriggersEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public SnapshotIsolationState SnapshotIsolationState => (SnapshotIsolationState)Parent.Properties.GetValueWithNullReplacement("SnapshotIsolationState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Trustworthy
	{
		get
		{
			return (bool)Parent.Properties.GetValueWithNullReplacement("Trustworthy");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("Trustworthy", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DatabaseUserAccess UserAccess
	{
		get
		{
			return (DatabaseUserAccess)Parent.Properties.GetValueWithNullReplacement("UserAccess");
		}
		set
		{
			Parent.Properties.SetValueWithConsistencyCheck("UserAccess", value);
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"AnsiNullsEnabled" => false, 
			"AnsiPaddingEnabled" => false, 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	internal void SetOptionTerminationStatement(OptionTerminationStatement optionTerminationStatement)
	{
		m_OptionTerminationStatement = optionTerminationStatement;
	}

	internal DatabaseOptions(Database parentdb, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentdb;
		SetServerObject(parentdb.GetServerObject());
	}

	internal DatabaseOptions()
	{
	}

	public override void Refresh()
	{
		base.Refresh();
		Parent.Refresh();
	}

	protected internal override string GetDBName()
	{
		return Parent.InternalName;
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		Parent.ScriptAlterInternal(query, sp);
	}

	public void SetSnapshotIsolation(bool enabled)
	{
		try
		{
			CheckObjectState();
			if (base.State == SqlSmoState.Creating)
			{
				throw new InvalidSmoOperationException("SetSnapshotIsolation", base.State);
			}
			if (base.ServerVersion.Major < 9)
			{
				throw new SmoException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION {1}", new object[2]
			{
				SqlSmoObject.SqlBraket(Parent.Name),
				enabled ? "ON" : "OFF"
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SetSnapshotIsolation, (Database)singletonParent, ex);
		}
	}

	public void Alter()
	{
		Parent.Alter();
	}

	public void Alter(TerminationClause terminationClause)
	{
		BuildOptionTerminationStatement(terminationClause);
		Alter();
	}

	public void Alter(TimeSpan transactionTerminationTime)
	{
		BuildOptionTerminationStatement(transactionTerminationTime);
		Alter();
	}

	internal void BuildOptionTerminationStatement(TerminationClause terminationClause)
	{
		m_OptionTerminationStatement = new OptionTerminationStatement(terminationClause);
	}

	internal void BuildOptionTerminationStatement(TimeSpan transactionTerminationTime)
	{
		if (transactionTerminationTime.Seconds < 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.TimeoutMustBePositive);
		}
		m_OptionTerminationStatement = new OptionTerminationStatement(transactionTerminationTime);
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}
}

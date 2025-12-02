using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
[StateChangeEvent("CREATE_WORKLOAD_GROUP", "WORKLOADGROUP", "WORKLOAD GROUP")]
[StateChangeEvent("ALTER_WORKLOAD_GROUP", "WORKLOADGROUP", "WORKLOAD GROUP")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class WorkloadGroup : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 9, 9, 9, 9, 10, 10, 10 };

		private static int[] cloudVersionCount;

		private static int sqlDwPropertyCount;

		internal static StaticMetadata[] sqlDwStaticMetadata;

		internal static StaticMetadata[] cloudStaticMetadata;

		internal static StaticMetadata[] staticMetadata;

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
				return -1;
			}
			return propertyName switch
			{
				"GroupMaximumRequests" => 0, 
				"ID" => 1, 
				"Importance" => 2, 
				"IsSystemObject" => 3, 
				"MaximumDegreeOfParallelism" => 4, 
				"PolicyHealthState" => 5, 
				"RequestMaximumCpuTimeInSeconds" => 6, 
				"RequestMaximumMemoryGrantPercentage" => 7, 
				"RequestMemoryGrantTimeoutInSeconds" => 8, 
				"ExternalResourcePoolName" => 9, 
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

		static PropertyMetadataProvider()
		{
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[10]
			{
				new StaticMetadata("GroupMaximumRequests", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("Importance", expensive: false, readOnly: false, typeof(WorkloadGroupImportance)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MaximumDegreeOfParallelism", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("RequestMaximumCpuTimeInSeconds", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("RequestMaximumMemoryGrantPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("RequestMemoryGrantTimeoutInSeconds", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ExternalResourcePoolName", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	private const string InternalPoolName = "internal";

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ResourcePool Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ResourcePool;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ExternalResourcePoolName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ExternalResourcePoolName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExternalResourcePoolName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int GroupMaximumRequests
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("GroupMaximumRequests");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("GroupMaximumRequests", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public WorkloadGroupImportance Importance
	{
		get
		{
			return (WorkloadGroupImportance)base.Properties.GetValueWithNullReplacement("Importance");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Importance", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumDegreeOfParallelism
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumDegreeOfParallelism");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumDegreeOfParallelism", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RequestMaximumCpuTimeInSeconds
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RequestMaximumCpuTimeInSeconds");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RequestMaximumCpuTimeInSeconds", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RequestMaximumMemoryGrantPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RequestMaximumMemoryGrantPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RequestMaximumMemoryGrantPercentage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RequestMemoryGrantTimeoutInSeconds
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RequestMemoryGrantTimeoutInSeconds");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RequestMemoryGrantTimeoutInSeconds", value);
		}
	}

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	public static string UrnSuffix => "WorkloadGroup";

	public WorkloadGroup()
	{
	}

	public WorkloadGroup(ResourcePool resourcePool, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = resourcePool;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal WorkloadGroup(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_comparer = ((ResourcePool)parentColl.ParentInstance).Parent.Parent.StringComparer;
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Alter()
	{
		AlterImpl();
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void MoveToPool(string poolName)
	{
		CheckObjectState(throwIfNotCreated: true);
		MoveToPoolImpl(poolName);
	}

	public StringCollection ScriptMoveToPool(string poolName)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER WORKLOAD GROUP {0} USING {1}", new object[2]
		{
			SqlSmoObject.MakeSqlBraket(SqlSmoObject.SqlString(Name)),
			SqlSmoObject.MakeSqlBraket(SqlSmoObject.SqlString(poolName))
		}));
		return stringCollection;
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_RG_WORKLOADGROUP, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE WORKLOAD GROUP {0}", new object[1] { FormatFullNameForScripting(sp) });
		int count = 0;
		GetAllParams(stringBuilder, sp, ref count);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " USING {0}", new object[1] { SqlSmoObject.MakeSqlBraket(Parent.Name) });
		if (IsSupportedProperty("ExternalResourcePoolName"))
		{
			int count2 = 0;
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetParameter(stringBuilder2, sp, "ExternalResourcePoolName", "{0}", ref count2);
			if (0 < count2)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", EXTERNAL {0}", new object[1] { SqlSmoObject.MakeSqlBraket(stringBuilder2.ToString()) });
			}
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		queries.Add(stringBuilder.ToString());
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER WORKLOAD GROUP {0}", new object[1] { FormatFullNameForScripting(sp) });
		int count = 0;
		GetAllParams(stringBuilder, sp, ref count);
		int count2 = 0;
		if (IsSupportedProperty("ExternalResourcePoolName"))
		{
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			GetParameter(stringBuilder2, sp, "ExternalResourcePoolName", "{0}", ref count2);
			if (0 < count2)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " USING EXTERNAL {0}", new object[1] { SqlSmoObject.MakeSqlBraket(stringBuilder2.ToString()) });
			}
		}
		if (0 < count || 0 < count2)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_RG_WORKLOADGROUP, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP WORKLOAD GROUP {0}", new object[1] { FormatFullNameForScripting(sp) });
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		queries.Add(stringBuilder.ToString());
	}

	protected override void TouchImpl()
	{
		GroupMaximumRequests = GroupMaximumRequests;
		Importance = Importance;
		RequestMaximumCpuTimeInSeconds = RequestMaximumCpuTimeInSeconds;
		RequestMaximumMemoryGrantPercentage = RequestMaximumMemoryGrantPercentage;
		RequestMemoryGrantTimeoutInSeconds = RequestMemoryGrantTimeoutInSeconds;
		MaximumDegreeOfParallelism = MaximumDegreeOfParallelism;
	}

	private void InitComparer()
	{
		m_comparer = Parent.Parent.Parent.StringComparer;
	}

	private void GetAllParams(StringBuilder sb, ScriptingPreferences sp, ref int count)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		GetParameter(stringBuilder, sp, "GroupMaximumRequests", "group_max_requests={0}", ref count);
		GetParameter(stringBuilder, sp, "Importance", "importance={0}", ref count);
		GetParameter(stringBuilder, sp, "RequestMaximumCpuTimeInSeconds", "request_max_cpu_time_sec={0}", ref count);
		GetParameter(stringBuilder, sp, "RequestMaximumMemoryGrantPercentage", "request_max_memory_grant_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "RequestMemoryGrantTimeoutInSeconds", "request_memory_grant_timeout_sec={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumDegreeOfParallelism", "max_dop={0}", ref count);
		if (0 < count)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, " WITH({0})", new object[1] { stringBuilder.ToString() });
		}
	}

	private void MoveToPoolImpl(string poolName)
	{
		try
		{
			string name = Name;
			if (poolName == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("poolName"));
			}
			if (m_comparer == null)
			{
				InitComparer();
			}
			if (m_comparer.Compare(poolName, "internal") == 0)
			{
				string message = string.Format(CultureInfo.InvariantCulture, ExceptionTemplatesImpl.CannotMoveToInternalResourcePool, new object[1] { name });
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(message, "poolName"));
			}
			ResourcePool resourcePool = Parent.Parent.ResourcePools[poolName];
			if (resourcePool == null)
			{
				string message2 = string.Format(CultureInfo.InvariantCulture, ExceptionTemplatesImpl.ResourcePoolNotExist, new object[1] { poolName });
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(message2, "poolName"));
			}
			if (m_comparer.Compare(poolName, Parent.Name) == 0)
			{
				string message3 = string.Format(CultureInfo.InvariantCulture, ExceptionTemplatesImpl.CannotMoveToSamePool, new object[1] { poolName });
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentException(message3, "poolName"));
			}
			StringCollection queries = ScriptMoveToPool(poolName);
			ExecutionManager.ExecuteNonQuery(queries);
			SetState(SqlSmoState.Pending);
			Parent = resourcePool;
			SetState(SqlSmoState.Existing);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.MoveToPool, this, ex);
		}
	}

	internal static WorkloadGroup GetWorkloadGroup(Server server, string groupName)
	{
		string path = string.Format(SmoApplication.DefaultCulture, string.Concat(server.Urn, "/ResourceGovernor/ResourcePool/WorkloadGroup[@Name='{0}']"), new object[1] { Urn.EscapeString(groupName) });
		SfcObjectQuery sfcObjectQuery = new SfcObjectQuery(server);
		IEnumerable enumerable = sfcObjectQuery.ExecuteIterator(new SfcQueryExpression(path), null, null);
		WorkloadGroup workloadGroup = null;
		IEnumerator enumerator = enumerable.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				workloadGroup = current as WorkloadGroup;
			}
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		if (workloadGroup == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.CouldNotFindManagementObject("WorkloadGroup", groupName));
		}
		return workloadGroup;
	}
}

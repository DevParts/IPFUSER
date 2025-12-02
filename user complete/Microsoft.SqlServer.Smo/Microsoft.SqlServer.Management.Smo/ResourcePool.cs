using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
[StateChangeEvent("CREATE_RESOURCE_POOL", "RESOURCEPOOL", "RESOURCE POOL")]
[StateChangeEvent("ALTER_RESOURCE_POOL", "RESOURCEPOOL", "RESOURCE POOL")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class ResourcePool : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 7, 7, 8, 10, 10, 10, 10 };

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
				"ID" => 0, 
				"IsSystemObject" => 1, 
				"MaximumCpuPercentage" => 2, 
				"MaximumMemoryPercentage" => 3, 
				"MinimumCpuPercentage" => 4, 
				"MinimumMemoryPercentage" => 5, 
				"PolicyHealthState" => 6, 
				"CapCpuPercentage" => 7, 
				"MaximumIopsPerVolume" => 8, 
				"MinimumIopsPerVolume" => 9, 
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
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MaximumCpuPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumMemoryPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MinimumCpuPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MinimumMemoryPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("CapCpuPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumIopsPerVolume", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MinimumIopsPerVolume", expensive: false, readOnly: false, typeof(int))
			};
		}
	}

	private WorkloadGroupCollection m_WorkloadGroups;

	private ResourcePoolAffinityInfo affinityInfo;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public ResourceGovernor Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ResourceGovernor;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int CapCpuPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("CapCpuPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CapCpuPercentage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumCpuPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumCpuPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumCpuPercentage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumIopsPerVolume
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumIopsPerVolume");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumIopsPerVolume", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumMemoryPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumMemoryPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumMemoryPercentage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MinimumCpuPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MinimumCpuPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MinimumCpuPercentage", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MinimumIopsPerVolume
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MinimumIopsPerVolume");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MinimumIopsPerVolume", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MinimumMemoryPercentage
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MinimumMemoryPercentage");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MinimumMemoryPercentage", value);
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(WorkloadGroup))]
	public WorkloadGroupCollection WorkloadGroups
	{
		get
		{
			CheckObjectState();
			if (m_WorkloadGroups == null)
			{
				m_WorkloadGroups = new WorkloadGroupCollection(this);
			}
			return m_WorkloadGroups;
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

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public ResourcePoolAffinityInfo ResourcePoolAffinityInfo
	{
		get
		{
			if (affinityInfo == null)
			{
				affinityInfo = new ResourcePoolAffinityInfo(this);
			}
			return affinityInfo;
		}
	}

	public static string UrnSuffix => "ResourcePool";

	public ResourcePool()
	{
	}

	public ResourcePool(ResourceGovernor resourceGovernor, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = resourceGovernor;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ResourcePool(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		m_comparer = ((ResourceGovernor)parentColl.ParentInstance).Parent.StringComparer;
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
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

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_RG_RESOUREPOOL, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE RESOURCE POOL {0}", new object[1] { FormatFullNameForScripting(sp) });
		int count = 0;
		GetAllParams(stringBuilder, sp, ref count);
		stringBuilder.Append(sp.NewLine);
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
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER RESOURCE POOL {0}", new object[1] { FormatFullNameForScripting(sp) });
		int count = 0;
		GetAllParams(stringBuilder, sp, ref count);
		stringBuilder.Append(sp.NewLine);
		if (0 < count)
		{
			queries.Add(stringBuilder.ToString());
		}
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_RG_RESOUREPOOL, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP RESOURCE POOL {0}", new object[1] { FormatFullNameForScripting(sp) });
		stringBuilder.Append(sp.NewLine);
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
		MinimumCpuPercentage = MinimumCpuPercentage;
		MaximumCpuPercentage = MaximumCpuPercentage;
		MinimumMemoryPercentage = MinimumMemoryPercentage;
		MaximumMemoryPercentage = MaximumMemoryPercentage;
		CapCpuPercentage = CapCpuPercentage;
		MinimumIopsPerVolume = MinimumIopsPerVolume;
		MaximumIopsPerVolume = MaximumIopsPerVolume;
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_WorkloadGroups != null)
		{
			m_WorkloadGroups.MarkAllDropped();
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		if (affinityInfo != null)
		{
			affinityInfo.Refresh();
		}
	}

	private void GetAllParams(StringBuilder sb, ScriptingPreferences sp, ref int count)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		GetParameter(stringBuilder, sp, "MinimumCpuPercentage", "min_cpu_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumCpuPercentage", "max_cpu_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "MinimumMemoryPercentage", "min_memory_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumMemoryPercentage", "max_memory_percent={0}", ref count);
		if (IsSupportedProperty("CapCpuPercentage"))
		{
			GetParameter(stringBuilder, sp, "CapCpuPercentage", "cap_cpu_percent={0}", ref count);
			StringCollection stringCollection = ResourcePoolAffinityInfo.DoAlterInternal(sp);
			if (stringCollection != null && stringCollection.Count > 0)
			{
				if (count++ > 0)
				{
					stringBuilder.Append(Globals.commaspace);
					stringBuilder.Append(Globals.newline);
					stringBuilder.Append(Globals.tab);
					stringBuilder.Append(Globals.tab);
				}
				StringEnumerator enumerator = stringCollection.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						stringBuilder.AppendLine(current);
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
		}
		if (IsSupportedProperty("MinimumIopsPerVolume") && IsSupportedProperty("MaximumIopsPerVolume"))
		{
			GetParameter(stringBuilder, sp, "MinimumIopsPerVolume", "min_iops_per_volume={0}", ref count);
			GetParameter(stringBuilder, sp, "MaximumIopsPerVolume", "max_iops_per_volume={0}", ref count);
		}
		if (count > 0)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, " WITH({0})", new object[1] { stringBuilder.ToString() });
		}
	}
}

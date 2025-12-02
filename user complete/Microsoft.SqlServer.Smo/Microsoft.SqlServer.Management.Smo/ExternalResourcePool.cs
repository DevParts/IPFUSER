using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class ExternalResourcePool : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 6, 6, 6 };

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
				"MaximumProcesses" => 4, 
				"PolicyHealthState" => 5, 
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
			staticMetadata = new StaticMetadata[6]
			{
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MaximumCpuPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumMemoryPercentage", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumProcesses", expensive: false, readOnly: false, typeof(long)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private ExternalResourcePoolAffinityInfo affinityInfo;

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
	public long MaximumProcesses
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("MaximumProcesses");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumProcesses", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	[SfcObject(SfcObjectRelationship.ChildObject, SfcObjectCardinality.One)]
	public ExternalResourcePoolAffinityInfo ExternalResourcePoolAffinityInfo
	{
		get
		{
			if (affinityInfo == null)
			{
				affinityInfo = new ExternalResourcePoolAffinityInfo(this);
			}
			return affinityInfo;
		}
	}

	public static string UrnSuffix => "ExternalResourcePool";

	public ExternalResourcePool()
	{
	}

	public ExternalResourcePool(ResourceGovernor resourceGovernor, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = resourceGovernor;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ExternalResourcePool(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
		this.ThrowIfNotSupported(typeof(ExternalResourcePool), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_RG_EXTERNALRESOUREPOOL, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE EXTERNAL RESOURCE POOL {0}", new object[1] { FormatFullNameForScripting(sp) });
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
		this.ThrowIfNotSupported(typeof(ExternalResourcePool), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER EXTERNAL RESOURCE POOL {0}", new object[1] { FormatFullNameForScripting(sp) });
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
		this.ThrowIfNotSupported(typeof(ExternalResourcePool), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_RG_EXTERNALRESOUREPOOL, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP EXTERNAL RESOURCE POOL {0}", new object[1] { FormatFullNameForScripting(sp) });
		stringBuilder.Append(sp.NewLine);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		queries.Add(stringBuilder.ToString());
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
		GetParameter(stringBuilder, sp, "MaximumCpuPercentage", "max_cpu_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumMemoryPercentage", "max_memory_percent={0}", ref count);
		GetParameter(stringBuilder, sp, "MaximumProcesses", "max_processes={0}", ref count);
		StringCollection stringCollection = ExternalResourcePoolAffinityInfo.DoAlterInternal(sp);
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
		if (count > 0)
		{
			sb.AppendFormat(SmoApplication.DefaultCulture, " WITH ({0})", new object[1] { stringBuilder.ToString() });
		}
	}
}

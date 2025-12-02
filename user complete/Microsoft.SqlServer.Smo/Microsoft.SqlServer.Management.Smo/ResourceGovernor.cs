using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class ResourceGovernor : SqlSmoObject, ISfcSupportsDesignMode, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 4, 4, 4, 5, 5, 5, 5 };

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
				"ClassifierFunction" => 0, 
				"Enabled" => 1, 
				"PolicyHealthState" => 2, 
				"ReconfigurePending" => 3, 
				"MaxOutstandingIOPerVolume" => 4, 
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
			staticMetadata = new StaticMetadata[5]
			{
				new StaticMetadata("ClassifierFunction", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Enabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("ReconfigurePending", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MaxOutstandingIOPerVolume", expensive: false, readOnly: false, typeof(int))
			};
		}
	}

	private ResourcePoolCollection m_ResourcePools;

	private ExternalResourcePoolCollection m_ExternalResourcePools;

	internal object oldEnabledValue;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ClassifierFunction
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ClassifierFunction");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ClassifierFunction", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Enabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Enabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Enabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaxOutstandingIOPerVolume
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaxOutstandingIOPerVolume");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaxOutstandingIOPerVolume", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool ReconfigurePending => (bool)base.Properties.GetValueWithNullReplacement("ReconfigurePending");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
		internal set
		{
			SetParentImpl(value);
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ResourcePool))]
	public ResourcePoolCollection ResourcePools
	{
		get
		{
			CheckObjectState();
			if (m_ResourcePools == null)
			{
				m_ResourcePools = new ResourcePoolCollection(this);
			}
			return m_ResourcePools;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(ExternalResourcePool))]
	public ExternalResourcePoolCollection ExternalResourcePools
	{
		get
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(ExternalResourcePool));
			if (m_ExternalResourcePools == null)
			{
				m_ExternalResourcePools = new ExternalResourcePoolCollection(this);
			}
			return m_ExternalResourcePools;
		}
	}

	public static string UrnSuffix => "ResourceGovernor";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "ReconfigurePending")
		{
			return false;
		}
		return base.GetPropertyDefaultValue(propname);
	}

	public ResourceGovernor()
	{
	}

	internal ResourceGovernor(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
		m_comparer = parentsrv.StringComparer;
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "Enabled" && !prop.Dirty)
		{
			oldEnabledValue = prop.Value;
		}
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

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	private void ScriptProperties(StringCollection queries, ScriptingPreferences sp)
	{
		bool flag = false;
		new StringBuilder(Globals.INIT_BUFFER_SIZE);
		Property property = base.Properties.Get("ClassifierFunction");
		if (property.Dirty || !sp.ScriptForAlter)
		{
			string text = "NULL";
			if (property.Value != null && !string.IsNullOrEmpty(property.Value.ToString()))
			{
				text = SqlSmoObject.SqlString(property.Value.ToString());
			}
			queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER RESOURCE GOVERNOR WITH (CLASSIFIER_FUNCTION = {0});", new object[1] { text }));
			flag = true;
		}
		if (IsSupportedProperty("MaxOutstandingIOPerVolume", sp))
		{
			Property property2 = base.Properties.Get("MaxOutstandingIOPerVolume");
			if (property2 != null && (property2.Dirty || !sp.ScriptForAlter))
			{
				string text2 = "DEFAULT";
				if (property2.Value != null && (int)property2.Value != 0)
				{
					text2 = property2.Value.ToString();
				}
				queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER RESOURCE GOVERNOR WITH (MAX_OUTSTANDING_IO_PER_VOLUME = {0});", new object[1] { text2 }));
				flag = true;
			}
		}
		Property property3 = base.Properties.Get("ReconfigurePending");
		if (null != property3 && property3.Value != null && (bool)property3.Value)
		{
			flag = true;
		}
		Property property4 = base.Properties.Get("Enabled");
		bool flag2 = false;
		if (property4.Value != null)
		{
			bool flag3 = (bool)property4.Value;
			if (HasEnabledPropertyChanged(flag3) || !sp.ScriptForAlter)
			{
				if (flag3)
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
			}
		}
		if (flag)
		{
			queries.Add(Scripts.RESOURCE_GOVERNOR_RECONFIGURE);
		}
		if (flag2)
		{
			queries.Add("ALTER RESOURCE GOVERNOR DISABLE;");
		}
	}

	private bool HasEnabledPropertyChanged(bool newValue)
	{
		bool result = true;
		if (oldEnabledValue != null)
		{
			result = ((newValue != (bool)oldEnabledValue) ? true : false);
		}
		return result;
	}
}

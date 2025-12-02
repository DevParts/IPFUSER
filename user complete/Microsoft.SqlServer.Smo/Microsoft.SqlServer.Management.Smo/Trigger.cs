using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class Trigger : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, ICreateOrAlterable, IAlterable, IDroppable, IDropIfExists, IMarkForDrop, IExtendedProperties, IScriptable, ITextObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 18, 18, 24, 25, 25, 25, 25, 27, 27, 27 };

		private static int[] cloudVersionCount = new int[3] { 21, 21, 24 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[24]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("Delete", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DeleteOrder", expensive: false, readOnly: false, typeof(ActivationOrder)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("Insert", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("InsertOrder", expensive: false, readOnly: false, typeof(ActivationOrder)),
			new StaticMetadata("InsteadOf", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("Update", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UpdateOrder", expensive: false, readOnly: false, typeof(ActivationOrder)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[27]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("Delete", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("DeleteOrder", expensive: false, readOnly: false, typeof(ActivationOrder)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("Insert", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("InsertOrder", expensive: false, readOnly: false, typeof(ActivationOrder)),
			new StaticMetadata("InsteadOf", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("Update", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("UpdateOrder", expensive: false, readOnly: false, typeof(ActivationOrder)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ExecutionContext)),
			new StaticMetadata("ExecutionContextPrincipal", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("IsNativelyCompiled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSchemaBound", expensive: false, readOnly: false, typeof(bool))
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
					"AnsiNullsStatus" => 0, 
					"BodyStartIndex" => 1, 
					"CreateDate" => 2, 
					"DateLastModified" => 3, 
					"Delete" => 4, 
					"DeleteOrder" => 5, 
					"ExecutionContext" => 6, 
					"ExecutionContextPrincipal" => 7, 
					"ID" => 8, 
					"ImplementationType" => 9, 
					"Insert" => 10, 
					"InsertOrder" => 11, 
					"InsteadOf" => 12, 
					"IsEnabled" => 13, 
					"IsEncrypted" => 14, 
					"IsSystemObject" => 15, 
					"NotForReplication" => 16, 
					"QuotedIdentifierStatus" => 17, 
					"Text" => 18, 
					"Update" => 19, 
					"UpdateOrder" => 20, 
					"AssemblyName" => 21, 
					"ClassName" => 22, 
					"MethodName" => 23, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiNullsStatus" => 0, 
				"BodyStartIndex" => 1, 
				"CreateDate" => 2, 
				"Delete" => 3, 
				"DeleteOrder" => 4, 
				"ID" => 5, 
				"ImplementationType" => 6, 
				"Insert" => 7, 
				"InsertOrder" => 8, 
				"InsteadOf" => 9, 
				"IsEnabled" => 10, 
				"IsEncrypted" => 11, 
				"IsSystemObject" => 12, 
				"NotForReplication" => 13, 
				"QuotedIdentifierStatus" => 14, 
				"Text" => 15, 
				"Update" => 16, 
				"UpdateOrder" => 17, 
				"AssemblyName" => 18, 
				"ClassName" => 19, 
				"DateLastModified" => 20, 
				"ExecutionContext" => 21, 
				"ExecutionContextPrincipal" => 22, 
				"MethodName" => 23, 
				"PolicyHealthState" => 24, 
				"IsNativelyCompiled" => 25, 
				"IsSchemaBound" => 26, 
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

	private TriggerEvents events;

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
	[SfcParent("Table")]
	[SfcParent("View")]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[CLSCompliant(false)]
	[SfcReference(typeof(SqlAssembly), "Server[@Name = '{0}']/Database[@Name = '{1}']/SqlAssembly[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "AssemblyName" })]
	public string AssemblyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AssemblyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AssemblyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string ClassName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ClassName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ClassName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Delete
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Delete");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Delete", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public ActivationOrder DeleteOrder
	{
		get
		{
			return (ActivationOrder)base.Properties.GetValueWithNullReplacement("DeleteOrder");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DeleteOrder", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public ExecutionContext ExecutionContext
	{
		get
		{
			return (ExecutionContext)base.Properties.GetValueWithNullReplacement("ExecutionContext");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContext", value);
		}
	}

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.Parent.ConnectionContext.TrueName", "Parent.Parent.Name", "ExecutionContextPrincipal" })]
	public string ExecutionContextPrincipal
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ExecutionContextPrincipal");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContextPrincipal", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public ImplementationType ImplementationType
	{
		get
		{
			return (ImplementationType)base.Properties.GetValueWithNullReplacement("ImplementationType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ImplementationType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Insert
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Insert");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Insert", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public ActivationOrder InsertOrder
	{
		get
		{
			return (ActivationOrder)base.Properties.GetValueWithNullReplacement("InsertOrder");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("InsertOrder", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool InsteadOf
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("InsteadOf");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("InsteadOf", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool IsEncrypted
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEncrypted");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEncrypted", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsNativelyCompiled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsNativelyCompiled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsNativelyCompiled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSchemaBound
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsSchemaBound");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsSchemaBound", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string MethodName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MethodName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MethodName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool NotForReplication
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NotForReplication");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NotForReplication", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool Update
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Update");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Update", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public ActivationOrder UpdateOrder
	{
		get
		{
			return (ActivationOrder)base.Properties.GetValueWithNullReplacement("UpdateOrder");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UpdateOrder", value);
		}
	}

	public TriggerEvents Events
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
				events = new TriggerEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Trigger";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string TextBody
	{
		get
		{
			CheckObjectState();
			return GetTextBody();
		}
		set
		{
			CheckObjectState();
			SetTextBody(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string TextHeader
	{
		get
		{
			CheckObjectState();
			return GetTextHeader(forAlter: false);
		}
		set
		{
			CheckObjectState();
			SetTextHeader(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool TextMode
	{
		get
		{
			CheckObjectState();
			return GetTextMode();
		}
		set
		{
			CheckObjectState();
			SetTextMode(value, null);
		}
	}

	public Trigger()
	{
	}

	public Trigger(SqlSmoObject parent, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal Trigger(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState eState)
		: base(parentColl, key, eState)
	{
	}

	internal override string[] GetNonAlterableProperties()
	{
		if (base.ParentColl.ParentInstance is Table)
		{
			return new string[0];
		}
		return new string[1] { "InsteadOf" };
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptDdl(queries, sp);
	}

	internal override void ScriptDdl(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptTrigger(queries, sp, ScriptHeaderType.ScriptHeaderForCreate);
	}

	public void CreateOrAlter()
	{
		CreateOrAlterImpl();
	}

	internal override void ScriptCreateOrAlter(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		GetInternalDDL(queries, sp, ScriptHeaderType.ScriptHeaderForCreateOrAlter);
		GetExternalDDL(queries, sp, ScriptHeaderType.ScriptHeaderForCreateOrAlter);
	}

	private string GetIfNotExistString(bool forCreate, ScriptingPreferences sp)
	{
		return string.Format(format: (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_TRIGGER80 : Scripts.INCLUDE_EXISTS_TRIGGER90, provider: SmoApplication.DefaultCulture, args: new object[2]
		{
			forCreate ? "NOT" : "",
			SqlSmoObject.SqlString(FormatFullNameForScripting(sp))
		});
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
		string text = FormatFullNameForScripting(sp);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendLine(GetIfNotExistString(forCreate: false, sp));
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP TRIGGER {0}{1}", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty())
		{
			ScriptTrigger(alterQuery, sp, ScriptHeaderType.ScriptHeaderForAlter);
		}
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
	}

	internal override string FormatFullNameForScripting(ScriptingPreferences sp)
	{
		CheckObjectState();
		string text = string.Empty;
		if (sp.IncludeScripts.SchemaQualify)
		{
			string schema = ((TableViewBase)base.ParentColl.ParentInstance).GetSchema(sp);
			if (schema.Length > 0)
			{
				text = SqlSmoObject.MakeSqlBraket(schema);
				text += Globals.Dot;
			}
		}
		return text + base.FormatFullNameForScripting(sp);
	}

	private bool ShouldScriptBodyAtAlter()
	{
		if (GetIsTextDirty())
		{
			return true;
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add("AnsiNullsStatus");
		stringCollection.Add("QuotedIdentifierStatus");
		stringCollection.Add("InsteadOf");
		stringCollection.Add("IsEncrypted");
		stringCollection.Add("Insert");
		stringCollection.Add("Delete");
		stringCollection.Add("Update");
		stringCollection.Add("NotForReplication");
		stringCollection.Add("ImplementationType");
		if (base.ServerVersion.Major >= 9)
		{
			stringCollection.Add("ExecutionContext");
			stringCollection.Add("ExecutionContextPrincipal");
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				stringCollection.Add("AssemblyName");
				stringCollection.Add("ClassName");
				stringCollection.Add("MethodName");
			}
		}
		if (base.Properties.ArePropertiesDirty(stringCollection))
		{
			return true;
		}
		return false;
	}

	private bool GetInsteafOfValue(ScriptingPreferences sp)
	{
		object obj = null;
		if (base.State == SqlSmoState.Creating || base.IsDesignMode)
		{
			obj = base.Properties.Get("InsteadOf").Value;
		}
		else if (sp.TargetServerVersionInternal != SqlServerVersionInternal.Version70)
		{
			obj = base.Properties["InsteadOf"].Value;
		}
		if (obj == null)
		{
			return false;
		}
		return (bool)obj;
	}

	private void ScriptTrigger(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		if (IsCreate(scriptHeaderType) || ShouldScriptBodyAtAlter())
		{
			GetInternalDDL(queries, sp, scriptHeaderType);
		}
		GetExternalDDL(queries, sp, scriptHeaderType);
	}

	private void GetInternalDDL(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		bool flag = IsCreate(scriptHeaderType);
		bool insteafOfValue = GetInsteafOfValue(sp);
		if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70 && insteafOfValue)
		{
			throw new SmoException(ExceptionTemplatesImpl.TriggerNotSupported(sp.TargetServerVersionInternal.ToString()));
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag2 = false;
		bool flag3 = false;
		string text = FormatFullNameForScripting(sp);
		if (!sp.OldOptions.DdlHeaderOnly && !sp.OldOptions.DdlBodyOnly)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			flag2 = null != base.Properties.Get("AnsiNullsStatus").Value;
			flag3 = null != base.Properties.Get("QuotedIdentifierStatus").Value;
			if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				Server server = (Server)base.ParentColl.ParentInstance.ParentColl.ParentInstance.ParentColl.ParentInstance;
				_ = server.UserOptions.AnsiNulls;
				_ = server.UserOptions.QuotedIdentifier;
			}
			if (flag2)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_ANSI_NULLS, new object[1] { ((bool)base.Properties["AnsiNullsStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			if (flag3)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.SET_QUOTED_IDENTIFIER, new object[1] { ((bool)base.Properties["QuotedIdentifierStatus"].Value) ? Globals.On : Globals.Off });
				queries.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
		}
		bool flag4 = true;
		Property property;
		if ((property = base.Properties.Get("ImplementationType")).Value != null && ImplementationType.SqlClr == (ImplementationType)property.Value)
		{
			if (base.ServerVersion.Major < 9)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.ClrNotSupported("ImplementationType", base.ServerVersion.ToString()));
			}
			flag4 = false;
			if (base.Properties.Get("Text").Dirty)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.NoPropertyChangeForDotNet("TextBody"));
			}
		}
		if (!TextMode)
		{
			bool flag5 = flag && sp.IncludeScripts.ExistenceCheck;
			StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
			if (!sp.OldOptions.DdlBodyOnly)
			{
				if (flag5)
				{
					stringBuilder.AppendLine(GetIfNotExistString(forCreate: true, sp));
					stringBuilder.AppendLine("EXECUTE dbo.sp_executesql N'");
				}
				switch (scriptHeaderType)
				{
				case ScriptHeaderType.ScriptHeaderForCreate:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TRIGGER {1}", new object[2]
					{
						Scripts.CREATE,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForAlter:
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TRIGGER {1}", new object[2]
					{
						Scripts.ALTER,
						text
					});
					break;
				case ScriptHeaderType.ScriptHeaderForCreateOrAlter:
					SqlSmoObject.ThrowIfCreateOrAlterUnsupported(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateOrAlterDownlevel("Trigger", SqlSmoObject.GetSqlServerName(sp)));
					stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "{0} TRIGGER {1}", new object[2]
					{
						Scripts.CREATE_OR_ALTER,
						text
					});
					break;
				default:
					throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration(scriptHeaderType.ToString()));
				}
				stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, " ON {0} ", new object[1] { ((TableViewBase)base.ParentColl.ParentInstance).FormatFullNameForScripting(sp) });
				bool needsComma = false;
				if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && flag4)
				{
					if (base.ServerVersion.Major >= 13 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130)
					{
						if (IsSupportedProperty("IsNativelyCompiled", sp))
						{
							AppendWithOption(stringBuilder2, "IsNativelyCompiled", Scripts.NATIVELY_COMPILED, ref needsComma);
						}
						if (IsSupportedProperty("IsSchemaBound", sp))
						{
							AppendWithOption(stringBuilder2, "IsSchemaBound", Scripts.SP_SCHEMABINDING, ref needsComma);
						}
					}
					AppendWithOption(stringBuilder2, "IsEncrypted", "ENCRYPTION", ref needsComma);
				}
				if (base.ServerVersion.Major >= 9 && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
				{
					AddScriptExecuteAs(stringBuilder2, sp, base.Properties, ref needsComma);
				}
				if (insteafOfValue)
				{
					stringBuilder2.Append(" INSTEAD OF ");
				}
				else if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version70)
				{
					stringBuilder2.Append(" FOR ");
				}
				else
				{
					stringBuilder2.Append(" AFTER ");
				}
				int num = 0;
				if (GetPropValueOptional("Insert", defaultValue: false))
				{
					stringBuilder2.Append(" INSERT");
					num++;
				}
				if (GetPropValueOptional("Delete", defaultValue: false))
				{
					if (num++ > 0)
					{
						stringBuilder2.Append(Globals.comma);
					}
					stringBuilder2.Append(" DELETE");
				}
				if (GetPropValueOptional("Update", defaultValue: false))
				{
					if (num++ > 0)
					{
						stringBuilder2.Append(Globals.comma);
					}
					stringBuilder2.Append(" UPDATE");
				}
				if (num == 0)
				{
					throw new PropertyNotSetException("Insert or Update or Delete");
				}
				if (IsSupportedProperty("NotForReplication", sp))
				{
					object value = base.Properties.Get("NotForReplication").Value;
					if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) && value != null && (bool)value)
					{
						stringBuilder2.Append(" NOT FOR REPLICATION ");
					}
				}
				stringBuilder2.Append(" AS ");
				stringBuilder2.Append(sp.NewLine);
			}
			if (!sp.OldOptions.DdlHeaderOnly)
			{
				if (flag4)
				{
					stringBuilder2.Append(GetTextBody(forScripting: true));
				}
				else if (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
				{
					stringBuilder2.Append("EXTERNAL NAME ");
					string text2 = (string)GetPropValue("AssemblyName");
					if (string.Empty == text2)
					{
						throw new PropertyNotSetException("AssemblyName");
					}
					stringBuilder2.AppendFormat("[{0}]", SqlSmoObject.SqlBraket(text2));
					text2 = (string)GetPropValue("ClassName");
					if (string.Empty == text2)
					{
						throw new PropertyNotSetException("ClassName");
					}
					stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text2));
					text2 = (string)GetPropValue("MethodName");
					if (string.Empty == text2)
					{
						throw new PropertyNotSetException(text2);
					}
					stringBuilder2.AppendFormat(".[{0}]", SqlSmoObject.SqlBraket(text2));
				}
			}
			if (flag5)
			{
				stringBuilder.Append(SqlSmoObject.SqlString(stringBuilder2.ToString()));
				if (!sp.OldOptions.DdlBodyOnly)
				{
					stringBuilder.Append(sp.NewLine);
					stringBuilder.Append("'");
				}
			}
			else
			{
				stringBuilder.Append(stringBuilder2.ToString());
			}
		}
		else
		{
			if (base.State == SqlSmoState.Existing && IsSupportedProperty("NotForReplication", sp))
			{
				object propValueOptional = GetPropValueOptional("NotForReplication");
				if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType && propValueOptional != null && (bool)propValueOptional)
				{
					throw new WrongPropertyValueException(string.Format(CultureInfo.CurrentCulture, ExceptionTemplatesImpl.ReplicationOptionNotSupportedForCloud, new object[1] { "NOT FOR REPLICATION" }));
				}
			}
			string textForScript = GetTextForScript(sp, new string[1] { "trigger" }, forceCheckNameAndManipulateIfRequired: true, scriptHeaderType);
			if (flag && sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendLine(GetIfNotExistString(forCreate: true, sp));
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_executesql @statement = {0} ", new object[1] { SqlSmoObject.MakeSqlString(textForScript) });
			}
			else
			{
				stringBuilder.Append(textForScript);
			}
		}
		queries.Add(stringBuilder.ToString());
		stringBuilder.Length = 0;
	}

	private void GetExternalDDL(StringCollection queries, ScriptingPreferences sp, ScriptHeaderType scriptHeaderType)
	{
		new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool insteafOfValue = GetInsteafOfValue(sp);
		if (sp.OldOptions.DdlHeaderOnly || sp.OldOptions.DdlBodyOnly)
		{
			return;
		}
		Property propertyOptional = GetPropertyOptional("IsEnabled");
		if (propertyOptional.Value != null && (propertyOptional.Dirty || sp.ScriptForCreateDrop || IsCreate(scriptHeaderType)))
		{
			if (base.ParentColl.ParentInstance is Table)
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "ALTER TABLE {0} {1} TRIGGER [{2}]", new object[3]
				{
					((ScriptSchemaObjectBase)base.ParentColl.ParentInstance).FullQualifiedName,
					((bool)propertyOptional.Value) ? "ENABLE" : "DISABLE",
					SqlSmoObject.SqlBraket(Name)
				}));
			}
			else if (sp.ScriptForCreateDrop)
			{
				throw new SmoException(ExceptionTemplatesImpl.CannotEnableViewTrigger);
			}
		}
		if (insteafOfValue)
		{
			return;
		}
		string[] array = new string[3] { "Delete", "Insert", "Update" };
		foreach (string text in array)
		{
			if (!GetPropValueOptional(text, defaultValue: false))
			{
				continue;
			}
			Property property = base.Properties.Get(text + "Order");
			if (property.Value != null)
			{
				ActivationOrder activationOrder = (ActivationOrder)property.Value;
				if (activationOrder != ActivationOrder.None || property.Dirty)
				{
					queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_settriggerorder @triggername=N'{0}', @order=N'{1}', @stmttype=N'{2}'", new object[3]
					{
						SqlSmoObject.SqlString(FormatFullNameForScripting(sp)),
						activationOrder,
						text.ToUpper(SmoApplication.DefaultCulture)
					}));
				}
			}
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
		{
			return null;
		}
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public void ReCompileReferences()
	{
		ReCompile(Name, string.Empty);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public string ScriptHeader(bool forAlter)
	{
		CheckObjectState();
		return GetTextHeader(forAlter);
	}

	public string ScriptHeader(ScriptHeaderType scriptHeaderType)
	{
		CheckObjectState();
		return GetTextHeader(scriptHeaderType);
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		switch (prop.Name)
		{
		case "InsteadOf":
		case "IsEncrypted":
		case "NotForReplication":
		case "Insert":
		case "InsertOrder":
		case "Update":
		case "UpdateOrder":
		case "Delete":
		case "DeleteOrder":
			ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
			break;
		}
	}

	protected override void PostCreate()
	{
		if (TextMode && !CheckTextModeSupport())
		{
			TextMode = false;
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		if (defaultTextMode)
		{
			string[] fields = new string[11]
			{
				"InsteadOf", "Insert", "Delete", "Update", "ImplementationType", "AnsiNullsStatus", "QuotedIdentifierStatus", "IsSystemObject", "DeleteOrder", "InsertOrder",
				"UpdateOrder"
			};
			List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
			supportedScriptFields.Add("Text");
			return supportedScriptFields.ToArray();
		}
		string[] fields2 = new string[16]
		{
			"AssemblyName", "ClassName", "MethodName", "InsteadOf", "Insert", "Delete", "Update", "ImplementationType", "IsEncrypted", "NotForReplication",
			"DeleteOrder", "InsertOrder", "UpdateOrder", "AnsiNullsStatus", "QuotedIdentifierStatus", "IsSystemObject"
		};
		List<string> supportedScriptFields2 = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields2, version, databaseEngineType, databaseEngineEdition);
		supportedScriptFields2.Add("Text");
		return supportedScriptFields2.ToArray();
	}
}

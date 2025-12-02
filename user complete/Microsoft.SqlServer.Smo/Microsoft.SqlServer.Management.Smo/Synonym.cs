using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[PhysicalFacet]
public sealed class Synonym : ScriptSchemaObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IExtendedProperties, IScriptable, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 10, 11, 11, 11, 11, 11, 11, 11 };

		private static int[] cloudVersionCount = new int[3] { 9, 9, 9 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[9]
		{
			new StaticMetadata("BaseDatabase", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseObject", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseType", expensive: false, readOnly: true, typeof(SynonymBaseType)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("BaseDatabase", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseObject", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseServer", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BaseType", expensive: false, readOnly: true, typeof(SynonymBaseType)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
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
					"BaseDatabase" => 0, 
					"BaseObject" => 1, 
					"BaseSchema" => 2, 
					"BaseType" => 3, 
					"CreateDate" => 4, 
					"DateLastModified" => 5, 
					"ID" => 6, 
					"IsSchemaOwned" => 7, 
					"Owner" => 8, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"BaseDatabase" => 0, 
				"BaseObject" => 1, 
				"BaseSchema" => 2, 
				"BaseServer" => 3, 
				"BaseType" => 4, 
				"CreateDate" => 5, 
				"DateLastModified" => 6, 
				"ID" => 7, 
				"IsSchemaOwned" => 8, 
				"Owner" => 9, 
				"PolicyHealthState" => 10, 
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

	private SynonymEvents events;

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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string BaseDatabase
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BaseDatabase");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BaseDatabase", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(View), "Server[@Name = '{0}']/Database[@Name = '{1}']/View[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "BaseObject", "BaseSchema" })]
	[CLSCompliant(false)]
	[SfcReference(typeof(UserDefinedFunction), "Server[@Name = '{0}']/Database[@Name = '{1}']/UserDefinedFunction[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "BaseObject", "BaseSchema" })]
	[SfcReference(typeof(StoredProcedure), "Server[@Name = '{0}']/Database[@Name = '{1}']/StoredProcedure[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "BaseObject", "BaseSchema" })]
	[SfcReference(typeof(Table), "Server[@Name = '{0}']/Database[@Name = '{1}']/Table[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "BaseObject", "BaseSchema" })]
	[SfcReference(typeof(UserDefinedAggregate), "Server[@Name = '{0}']/Database[@Name = '{1}']/UserDefinedAggregate[@Name='{2}' and @Schema='{3}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "BaseObject", "BaseSchema" })]
	public string BaseObject
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BaseObject");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BaseObject", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design, "dbo")]
	public string BaseSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BaseSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BaseSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.Deploy)]
	public string BaseServer
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("BaseServer");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BaseServer", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SynonymBaseType BaseType => (SynonymBaseType)base.Properties.GetValueWithNullReplacement("BaseType");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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

	public SynonymEvents Events
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
				events = new SynonymEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Synonym";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(1)]
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

	[SfcKey(0)]
	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	public override string Schema
	{
		get
		{
			return base.Schema;
		}
		set
		{
			base.Schema = value;
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

	public Synonym()
	{
	}

	public Synonym(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public Synonym(Database database, string name, string schema)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, schema);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[4] { "BaseDatabase", "BaseObject", "BaseSchema", "BaseServer" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "BaseSchema")
		{
			return "dbo";
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

	private void Init(Database database, string schema, string name, string baseServer, string baseDatabase, string baseSchema, string baseObject)
	{
		Name = name;
		Parent = database;
		if (schema != null)
		{
			ChangeSchema(schema, bCheckExisting: false);
		}
		BaseServer = baseServer;
		BaseDatabase = baseDatabase;
		BaseSchema = baseSchema;
		BaseObject = baseObject;
	}

	public Synonym(Database database, string name, string schema, string baseServer, string baseDatabase, string baseSchema, string baseObject)
	{
		Init(database, schema, name, baseServer, baseDatabase, baseSchema, baseObject);
	}

	public Synonym(Database database, string name, string baseServer, string baseDatabase, string baseSchema, string baseObject)
	{
		Init(database, null, name, baseServer, baseDatabase, baseSchema, baseObject);
	}

	internal Synonym(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("Synonym", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SYNONYM, new object[3]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false),
				SqlSmoObject.MakeSqlString(GetSchema(sp))
			}));
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append("DROP SYNONYM " + ((sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty) + FormatFullNameForScripting(sp));
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
		SetSchemaOwned();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("Synonym", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SYNONYM, new object[3]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false),
				SqlSmoObject.MakeSqlString(GetSchema(sp))
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE SYNONYM {0} FOR ", new object[1] { FormatFullNameForScripting(sp) });
		string text = string.Empty;
		if (IsSupportedProperty("BaseServer", sp) && !SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			text = (string)GetPropValueOptional("BaseServer");
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket(text));
				stringBuilder.Append(Globals.Dot);
			}
		}
		string text2 = (string)GetPropValueOptional("BaseDatabase");
		if (!string.IsNullOrEmpty(text2))
		{
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(text2));
			stringBuilder.Append(Globals.Dot);
		}
		else if (!string.IsNullOrEmpty(text))
		{
			stringBuilder.Append(Globals.Dot);
		}
		string text3 = (string)GetPropValueOptional("BaseSchema");
		if (!string.IsNullOrEmpty(text3))
		{
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(text3));
			stringBuilder.Append(Globals.Dot);
		}
		else if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
		{
			stringBuilder.Append(Globals.Dot);
		}
		string text4 = (string)GetPropValue("BaseObject");
		if (!string.IsNullOrEmpty(text4))
		{
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(text4));
		}
		createQuery.Add(stringBuilder.ToString());
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(createQuery, sp);
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Alter()
	{
		AlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(alterQuery, sp);
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
}

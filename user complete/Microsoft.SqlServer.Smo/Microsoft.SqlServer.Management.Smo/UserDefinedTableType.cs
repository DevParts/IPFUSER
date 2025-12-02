using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
public sealed class UserDefinedTableType : TableViewTableTypeBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IAlterable, IRenamable, IDroppable, IDropIfExists, IScriptable, IExtendedProperties
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 10, 10, 10, 11, 11, 11, 11 };

		private static int[] cloudVersionCount = new int[3] { 9, 9, 10 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUserDefined", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MaxLength", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsMemoryOptimized", expensive: false, readOnly: false, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("Collation", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSchemaOwned", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsUserDefined", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("MaxLength", expensive: false, readOnly: true, typeof(short)),
			new StaticMetadata("Nullable", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("IsMemoryOptimized", expensive: false, readOnly: false, typeof(bool))
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
					"Collation" => 0, 
					"CreateDate" => 1, 
					"DateLastModified" => 2, 
					"ID" => 3, 
					"IsSchemaOwned" => 4, 
					"IsUserDefined" => 5, 
					"MaxLength" => 6, 
					"Nullable" => 7, 
					"Owner" => 8, 
					"IsMemoryOptimized" => 9, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"Collation" => 0, 
				"CreateDate" => 1, 
				"DateLastModified" => 2, 
				"ID" => 3, 
				"IsSchemaOwned" => 4, 
				"IsUserDefined" => 5, 
				"MaxLength" => 6, 
				"Nullable" => 7, 
				"Owner" => 8, 
				"PolicyHealthState" => 9, 
				"IsMemoryOptimized" => 10, 
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

	private CheckCollection m_checks;

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Collation => (string)base.Properties.GetValueWithNullReplacement("Collation");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsSchemaOwned => (bool)base.Properties.GetValueWithNullReplacement("IsSchemaOwned");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsUserDefined
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsUserDefined");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsUserDefined", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public short MaxLength => (short)base.Properties.GetValueWithNullReplacement("MaxLength");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool Nullable
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Nullable");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Nullable", value);
		}
	}

	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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

	public static string UrnSuffix => "UserDefinedTableType";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(Schema), typeof(SchemaCustomResolver), "Resolve", new string[] { })]
	[CLSCompliant(false)]
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

	[SfcKey(1)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(Check), SfcObjectFlags.Design)]
	public CheckCollection Checks
	{
		get
		{
			CheckObjectState();
			if (m_checks == null)
			{
				m_checks = new CheckCollection(this);
			}
			return m_checks;
		}
	}

	public UserDefinedTableType()
	{
	}

	public UserDefinedTableType(Database database, string name)
	{
		ValidateName(name);
		key = new SchemaObjectKey(name, null);
		Parent = database;
	}

	public UserDefinedTableType(Database database, string name, string schema)
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
		return new string[3] { "IsMemoryOptimized", "IsUserDefined", "Nullable" };
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

	internal UserDefinedTableType(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		CreateImpl();
		SetSchemaOwned();
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		bool bWithScript = action != PropagateAction.Create;
		ArrayList arrayList = new ArrayList();
		arrayList.Add(new PropagateInfo(base.Columns, bWithScript));
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType || base.ServerVersion.Major >= 12)
		{
			arrayList.Add(new PropagateInfo(base.ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix));
		}
		if (action == PropagateAction.Create)
		{
			arrayList.Add(new PropagateInfo(Indexes, bWithScript));
			arrayList.Add(new PropagateInfo(Checks, bWithScript));
		}
		PropagateInfo[] array = new PropagateInfo[arrayList.Count];
		arrayList.CopyTo(array, 0);
		return array;
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		StringCollection stringCollection2 = new StringCollection();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDDT90, new object[3]
			{
				"NOT",
				SqlSmoObject.SqlString((ScriptName.Length > 0) ? ScriptName : Name),
				SqlSmoObject.SqlString((ScriptName.Length > 0) ? ScriptName : Schema)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE TYPE {0} AS TABLE", new object[1] { text });
		if (base.Columns.Count < 1)
		{
			throw new SmoException(ExceptionTemplatesImpl.ObjectWithNoChildren("UserDefinedTableType", "Column"));
		}
		stringBuilder.Append(Globals.LParen);
		stringBuilder.Append(sp.NewLine);
		bool flag = true;
		stringCollection2.Clear();
		foreach (Column column in base.Columns)
		{
			column.ScriptDdlInternal(stringCollection2, sp);
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(Globals.comma);
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append(Globals.tab);
			stringBuilder.Append(stringCollection2[0]);
			stringCollection2.Clear();
		}
		foreach (Index index in Indexes)
		{
			IndexKeyType? propValueOptional = index.GetPropValueOptional<IndexKeyType>("IndexKeyType");
			IndexType? propValueOptional2 = index.GetPropValueOptional<IndexType>("IndexType");
			if ((propValueOptional.HasValue && propValueOptional.Value != IndexKeyType.None) || (propValueOptional2.HasValue && (propValueOptional2.Value == IndexType.ClusteredIndex || propValueOptional2.Value == IndexType.NonClusteredIndex)) || index.IsMemoryOptimizedIndex)
			{
				index.ScriptDdl(stringCollection2, sp, notEmbedded: false, createStatement: true);
				stringBuilder.Append(Globals.comma);
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(Globals.tab);
				stringBuilder.Append(stringCollection2[0]);
				stringCollection2.Clear();
				continue;
			}
			throw new InvalidSmoOperationException(ExceptionTemplatesImpl.NotIndexOnUDTT);
		}
		foreach (Check check in Checks)
		{
			stringBuilder.Append(Globals.comma);
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Globals.tab);
			stringBuilder.Append("CHECK ");
			string value = (string)check.GetPropValue("Text");
			stringBuilder.Append(Globals.LParen);
			stringBuilder.Append(value);
			stringBuilder.Append(Globals.RParen);
		}
		stringBuilder.Append(sp.NewLine);
		stringBuilder.Append(Globals.RParen);
		if (IsSupportedProperty("IsMemoryOptimized", sp) && GetPropValueOptional("IsMemoryOptimized", defaultValue: false))
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.AppendFormat(Scripts.WITH_MEMORY_OPTIMIZED);
		}
		stringCollection.Add(stringBuilder.ToString());
		StringEnumerator enumerator4 = stringCollection.GetEnumerator();
		try
		{
			while (enumerator4.MoveNext())
			{
				string current = enumerator4.Current;
				query.Add(current);
			}
		}
		finally
		{
			if (enumerator4 is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		if (sp.IncludeScripts.Owner && (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType || sp.TargetServerVersion == SqlServerVersion.Version120) && sp.IncludeScripts.Owner)
		{
			ScriptOwner(query, sp);
		}
	}

	protected override void PostCreate()
	{
		Indexes.Refresh();
		Checks.Refresh();
	}

	public void Alter()
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		AlterImpl();
		SetSchemaOwned();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (sp.IncludeScripts.Owner)
		{
			ScriptOwner(query, sp);
		}
	}

	public void Rename(string newname)
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "EXEC sp_rename @objname=N'{0}', @newname=N'{1}', @objtype=N'USERDATATYPE'", new object[2]
		{
			SqlSmoObject.SqlString(FullQualifiedName),
			SqlSmoObject.SqlString(newName)
		}));
	}

	public void Drop()
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		DropImpl();
	}

	public void DropIfExists()
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_UDDT90, new object[3]
			{
				"",
				SqlSmoObject.SqlString((ScriptName.Length > 0) ? ScriptName : Name),
				SqlSmoObject.SqlString((ScriptName.Length > 0) ? ScriptName : Schema)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP TYPE {0}{1}", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text
		});
		dropQuery.Add(stringBuilder.ToString());
	}

	public new StringCollection Script()
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		return ScriptImpl();
	}

	public new StringCollection Script(ScriptingOptions so)
	{
		this.ThrowIfNotSupported(typeof(UserDefinedTableType));
		return ScriptImpl(so);
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[4] { "ID", "Owner", "IsMemoryOptimized", "IsSchemaOwned" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

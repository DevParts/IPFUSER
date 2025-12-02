using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "ROLE")]
[StateChangeEvent("CREATE_ROLE", "ROLE")]
[StateChangeEvent("ALTER_ROLE", "ROLE")]
[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElementType("Role")]
public sealed class DatabaseRole : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IAlterable, IRenamable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 5, 5, 5, 6, 6, 6, 6, 6, 6, 6 };

		private static int[] cloudVersionCount = new int[3] { 5, 5, 5 };

		private static int sqlDwPropertyCount = 5;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsFixedRole", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsFixedRole", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsFixedRole", expensive: false, readOnly: true, typeof(bool)),
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
					return propertyName switch
					{
						"CreateDate" => 0, 
						"DateLastModified" => 1, 
						"ID" => 2, 
						"IsFixedRole" => 3, 
						"Owner" => 4, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"ID" => 2, 
					"IsFixedRole" => 3, 
					"Owner" => 4, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"ID" => 2, 
				"IsFixedRole" => 3, 
				"Owner" => 4, 
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
	}

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
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public bool IsFixedRole => (bool)base.Properties.GetValueWithNullReplacement("IsFixedRole");

	public static string UrnSuffix => "Role";

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[CLSCompliant(false)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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
			ThrowIfBelowVersion90();
			base.Properties.SetValueWithConsistencyCheck("Owner", value);
		}
	}

	public DatabaseRole()
	{
	}

	public DatabaseRole(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
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

	internal DatabaseRole(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
		StringBuilder stringBuilder = new StringBuilder(string.Format(SmoApplication.DefaultCulture, Scripts.DECLARE_ROLE_MEMEBER, new object[1] { SqlSmoObject.EscapeString(Name, '\'') }));
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.Append((sp.TargetServerVersion >= SqlServerVersion.Version90) ? Scripts.INCLUDE_EXISTS_ROLE_MEMBERS90 : Scripts.INCLUDE_EXISTS_ROLE_MEMBERS80);
		}
		if (sp.TargetServerVersion >= SqlServerVersion.Version90)
		{
			stringBuilder.Append(Scripts.IS_DBROLE_FIXED_OR_PUBLIC_90);
		}
		if (Parent.GetPropValueOptional("IsSqlDw", defaultValue: false))
		{
			stringBuilder.AppendFormat(Scripts.DROP_DATABASEROLE_MEMBERS_DW, Guid.NewGuid().ToString("N"));
		}
		else
		{
			stringBuilder.Append(VersionUtils.IsTargetServerVersionSQl11OrLater(sp.TargetServerVersionInternal) ? Scripts.DROP_DATABASEROLE_MEMBERS_110 : ((sp.TargetServerVersion >= SqlServerVersion.Version90) ? Scripts.DROP_DATABASEROLE_MEMBERS_90 : Scripts.DROP_DATABASEROLE_MEMBERS_80));
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("DatabaseRole", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_DBROLE80 : Scripts.INCLUDE_EXISTS_DBROLE90, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			}));
			stringBuilder.Append(sp.NewLine);
		}
		if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
		{
			stringBuilder.Append("EXEC dbo.sp_droprole @rolename = " + FormatFullNameForScripting(sp, nameIsIndentifier: false));
		}
		else
		{
			stringBuilder.Append("DROP ROLE " + ((sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty) + FormatFullNameForScripting(sp, nameIsIndentifier: true));
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
	}

	private void CreateDdl(StringBuilder sb, ScriptingPreferences sp, string owner)
	{
		if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
		{
			sb.Append("EXEC dbo.sp_addrole @rolename = " + FormatFullNameForScripting(sp, nameIsIndentifier: false));
			if (sp.IncludeScripts.Owner && owner != null && owner.Length > 0)
			{
				sb.Append(", @ownername = " + SqlSmoObject.MakeSqlString(owner));
			}
		}
		else
		{
			sb.Append("CREATE ROLE " + FormatFullNameForScripting(sp, nameIsIndentifier: true));
			if (sp.IncludeScripts.Owner && owner != null && owner.Length > 0)
			{
				sb.Append(" AUTHORIZATION " + SqlSmoObject.MakeSqlBraket(owner));
			}
		}
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		if (string.Compare(Name, "public", StringComparison.OrdinalIgnoreCase) != 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader("DatabaseRole", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_DBROLE80 : Scripts.INCLUDE_EXISTS_DBROLE90, new object[2]
				{
					"NOT",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
				stringBuilder.Append(sp.NewLine);
			}
			CreateDdl(stringBuilder, sp, (string)base.Properties.Get("Owner").Value);
			createQuery.Add(stringBuilder.ToString());
			if (!sp.ScriptForCreateDrop && sp.IncludeScripts.Associations && !base.IsDesignMode)
			{
				ScriptAssociations(createQuery, sp);
			}
		}
	}

	internal override void ScriptAssociations(StringCollection createQuery, ScriptingPreferences sp)
	{
		StringCollection stringCollection = EnumRoles();
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				createQuery.Add(ScriptAddToRole(current, sp));
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

	public void AddMember(string name)
	{
		try
		{
			CheckObjectState();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!CompareAccToDbCollation(Name, "public"))
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
				if (VersionUtils.IsSql11OrLater(base.ServerVersion) && DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER ROLE {0} ADD MEMBER {1}", new object[2]
					{
						SqlSmoObject.MakeSqlBraket(InternalName),
						SqlSmoObject.MakeSqlBraket(name)
					}));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SP_ADDDBROLEMEMBER, new object[2]
					{
						SqlSmoObject.SqlString(InternalName),
						SqlSmoObject.SqlString(name)
					}));
				}
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddMember, this, ex);
		}
	}

	public void DropMember(string name)
	{
		try
		{
			CheckObjectState();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!CompareAccToDbCollation(Name, "public"))
			{
				StringCollection stringCollection = new StringCollection();
				stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(base.ParentColl.ParentInstance.InternalName) }));
				if (VersionUtils.IsSql11OrLater(base.ServerVersion))
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER ROLE {0} DROP MEMBER {1}", new object[2]
					{
						SqlSmoObject.MakeSqlBraket(InternalName),
						SqlSmoObject.MakeSqlBraket(name)
					}));
				}
				else
				{
					stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.SP_DROPDBROLEMEMBER, new object[2]
					{
						SqlSmoObject.SqlString(InternalName),
						SqlSmoObject.SqlString(name)
					}));
				}
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropMember, this, ex);
		}
	}

	public StringCollection EnumMembers()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			Request req = new Request(string.Concat(base.Urn, "/Member"));
			foreach (DataRow row in ExecutionManager.GetEnumeratorData(req).Rows)
			{
				stringCollection.Add(Convert.ToString(row["Name"], SmoApplication.DefaultCulture));
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMembers, this, ex);
		}
	}

	private string ScriptAddToRole(string role, ScriptingPreferences sp)
	{
		if (VersionUtils.IsTargetServerVersionSQl11OrLater(sp.TargetServerVersionInternal) && sp.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return string.Format(SmoApplication.DefaultCulture, "ALTER ROLE {0} ADD MEMBER {1}", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(role),
				SqlSmoObject.MakeSqlBraket(Name)
			});
		}
		string text = ((sp == null) ? SqlSmoObject.MakeSqlString(Name) : FormatFullNameForScripting(sp, nameIsIndentifier: false));
		string text2 = ((sp != null) ? ((sp.TargetServerVersion < SqlServerVersion.Version90) ? "dbo" : "sys") : ((base.ServerVersion.Major < 9) ? "dbo" : "sys"));
		return string.Format(SmoApplication.DefaultCulture, "EXEC {0}.sp_addrolemember @rolename = {1}, @membername = {2}", new object[3]
		{
			text2,
			SqlSmoObject.MakeSqlString(role),
			text
		});
	}

	private void AddToRole(string role)
	{
		CheckObjectState();
		if (role != null)
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			stringCollection.Add(ScriptAddToRole(role, null));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
	}

	private bool CompareAccToDbCollation(object obj1, object obj2)
	{
		IComparer comparerFromCollation = Parent.GetComparerFromCollation(Parent.Collation);
		if (comparerFromCollation.Compare(obj1, obj2) == 0)
		{
			return true;
		}
		return false;
	}

	public StringCollection EnumRoles()
	{
		CheckObjectState();
		StringCollection stringCollection = new StringCollection();
		StringCollection stringCollection2 = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder();
		AddDatabaseContext(stringCollection2);
		if (base.ServerVersion.Major >= 9)
		{
			stringBuilder.AppendLine("SELECT p1.name FROM sys.database_role_members as r ");
			stringBuilder.AppendLine("JOIN sys.database_principals as p1 on p1.principal_id = r.role_principal_id ");
			stringBuilder.AppendLine("JOIN sys.database_principals as p2 on p2.principal_id = r.member_principal_id ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WHERE p2.name = {0}", new object[1] { SqlSmoObject.MakeSqlString(Name) });
		}
		else
		{
			stringBuilder.AppendLine("SELECT g.name FROM sysusers u, sysusers g, sysmembers m ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WHERE u.name = {0} AND u.uid = m.memberuid AND g.uid = m.groupuid AND u.issqlrole = 1 ", new object[1] { SqlSmoObject.MakeSqlString(Name) });
		}
		stringCollection2.Add(stringBuilder.ToString());
		DataTable dataTable = ExecutionManager.ExecuteWithResults(stringCollection2).Tables[0];
		if (dataTable.Rows.Count == 0)
		{
			return stringCollection;
		}
		foreach (DataRow row in dataTable.Rows)
		{
			stringCollection.Add(Convert.ToString(row[0], SmoApplication.DefaultCulture));
		}
		return stringCollection;
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		if (base.ServerVersion.Major < 9)
		{
			throw new InvalidVersionSmoOperationException(base.ServerVersion);
		}
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER ROLE {0} WITH NAME={1}", new object[2]
		{
			FormatFullNameForScripting(new ScriptingPreferences()),
			SqlSmoObject.MakeSqlBraket(newName)
		}));
	}

	public DataTable EnumAgentProxyAccounts()
	{
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_enum_login_for_proxy @name={0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
		stringCollection.Add(stringBuilder.ToString());
		return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		ScriptChangeOwner(alterQuery, sp);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo((base.ServerVersion.Major <= 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
			};
		}
		return null;
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80 && base.ServerVersion.Major > 8)
		{
			base.AddScriptPermission(query, sp);
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[3] { "IsFixedRole", "ID", "Owner" };
	}
}

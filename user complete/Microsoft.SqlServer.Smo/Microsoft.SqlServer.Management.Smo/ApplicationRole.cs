using System;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("CREATE_APPLICATION_ROLE", "APPLICATION ROLE")]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[StateChangeEvent("ALTER_APPLICATION_ROLE", "APPLICATION ROLE")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
public sealed class ApplicationRole : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, IAlterable, IDroppable, IDropIfExists, IRenamable, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 3, 3, 4, 5, 5, 5, 5, 5, 5, 5 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 4 };

		private static int sqlDwPropertyCount = 4;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
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
						"DefaultSchema" => 2, 
						"ID" => 3, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"DefaultSchema" => 2, 
					"ID" => 3, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"ID" => 2, 
				"DefaultSchema" => 3, 
				"PolicyHealthState" => 4, 
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

	private ApplicationRoleEvents events;

	private SqlSecureString password;

	[SfcObject(SfcObjectRelationship.ParentObject)]
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
	[SfcReference(typeof(Schema), "Server[@Name = '{0}']/Database[@Name = '{1}']/Schema[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "DefaultSchema" })]
	[CLSCompliant(false)]
	public string DefaultSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	public ApplicationRoleEvents Events
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
				events = new ApplicationRoleEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "ApplicationRole";

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

	public ApplicationRole()
	{
	}

	public ApplicationRole(Database database, string name)
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

	internal ApplicationRole(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (Parent.Parent.Information.Version.Major >= 9)
		{
			base.AddScriptPermission(query, sp);
		}
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
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ApplicationRole", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? Scripts.INCLUDE_EXISTS_APPROLE80 : Scripts.INCLUDE_EXISTS_APPROLE90, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			}));
			stringBuilder.Append(sp.NewLine);
		}
		if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
		{
			stringBuilder.Append("EXEC dbo.sp_dropapprole @rolename = " + FormatFullNameForScripting(sp, nameIsIndentifier: false));
		}
		else
		{
			stringBuilder.Append("DROP APPLICATION ROLE  " + FormatFullNameForScripting(sp, nameIsIndentifier: true));
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create(string password)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("password"));
		}
		Create(new SqlSecureString(password));
	}

	public void Create(SecureString password)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("password"));
		}
		this.password = password;
		CreateImpl();
	}

	private void ScriptDdl(StringBuilder sb, ScriptingPreferences sp)
	{
		if (sp.ScriptForCreateDrop && password == null)
		{
			throw new PropertyNotSetException("password");
		}
		if (!sp.ScriptForCreateDrop)
		{
			SecurityUtils.ScriptRandomPwd(sb);
		}
		if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
		{
			if (sp.IncludeScripts.ExistenceCheck)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_APPROLE80, new object[2]
				{
					"NOT",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
				sb.Append(sp.NewLine);
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_addapprole @rolename = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
			if (!sp.ScriptForCreateDrop)
			{
				sb.Append(", @password = @randomPwd");
				return;
			}
			sb.AppendFormat(SmoApplication.DefaultCulture, ", @password = {0}", new object[1] { SqlSmoObject.MakeSqlString((string)password) });
			return;
		}
		if (!sp.ForDirectExecution)
		{
			sb.Append("declare @statement nvarchar(4000)");
			sb.Append(Globals.newline);
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_APPROLE90, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE APPLICATION ROLE {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: true) });
		StringBuilder stringBuilder2 = new StringBuilder();
		string propValueOptional = GetPropValueOptional("DefaultSchema", string.Empty);
		if (propValueOptional.Length > 0)
		{
			stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "DEFAULT_SCHEMA = {0}, ", new object[1] { SqlSmoObject.MakeSqlBraket(propValueOptional) });
		}
		if (sp.ForDirectExecution)
		{
			stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "PASSWORD = {0}", new object[1] { SqlSmoObject.MakeSqlString((string)password) });
		}
		if (stringBuilder2.Length > 0)
		{
			stringBuilder.Append(" WITH ");
			stringBuilder.Append(stringBuilder2.ToString());
		}
		if (sp.ForDirectExecution)
		{
			sb.Append(stringBuilder.ToString());
			return;
		}
		sb.AppendFormat(SmoApplication.DefaultCulture, "select @statement = N'{0}' + N'PASSWORD = N' + QUOTENAME(@randomPwd,'''')", new object[1] { SqlSmoObject.SqlString(stringBuilder.ToString()) });
		sb.Append(Globals.newline);
		sb.Append("EXEC dbo.sp_executesql @statement");
		sb.Append(Globals.newline);
	}

	protected override void PostCreate()
	{
		password = null;
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ApplicationRole", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		ScriptDdl(stringBuilder, sp);
		createQuery.Add(stringBuilder.ToString());
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
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (SqlServerVersionInternal.Version90 > sp.TargetServerVersionInternal)
		{
			throw new InvalidVersionSmoOperationException(base.ServerVersion);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER APPLICATION ROLE {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: true) });
		string propValueOptional = GetPropValueOptional("DefaultSchema", string.Empty);
		if (propValueOptional.Length > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " WITH DEFAULT_SCHEMA = {0}", new object[1] { SqlSmoObject.MakeSqlBraket(propValueOptional) });
		}
		query.Add(stringBuilder.ToString());
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER APPLICATION ROLE {0} WITH NAME={1}", new object[2]
		{
			FormatFullNameForScripting(new ScriptingPreferences()),
			SqlSmoObject.MakeSqlBraket(newName)
		}));
	}

	public void ChangePassword(string password)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, new ArgumentNullException("password"));
		}
		ChangePassword(new SqlSecureString(password));
	}

	public void ChangePassword(SecureString password)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, new ArgumentNullException("password"));
		}
		if (base.ServerVersion.Major < 9)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, new InvalidVersionSmoOperationException(base.ServerVersion));
		}
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER APPLICATION ROLE {0} WITH PASSWORD={1}", new object[2]
			{
				FormatFullNameForScripting(new ScriptingPreferences()),
				SqlSmoObject.MakeSqlString((string)(SqlSecureString)password)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major <= 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}
}

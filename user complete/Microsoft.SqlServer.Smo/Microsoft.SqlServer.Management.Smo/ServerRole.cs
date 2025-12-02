using System;
using System.Collections.Specialized;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElementType("Role")]
[SfcElement(SfcElementFlags.Standalone)]
[StateChangeEvent("CREATE_SERVER_ROLE", "SERVER ROLE")]
[StateChangeEvent("ALTER_SERVER_ROLE", "SERVER ROLE")]
[StateChangeEvent("ALTER_AUTHORIZATION_SERVER", "SERVER ROLE")]
public sealed class ServerRole : ScriptNameObjectBase, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IAlterable, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 1, 1, 2, 2, 2, 7, 7, 7, 7, 7 };

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
				"Description" => 0, 
				"ID" => 1, 
				"DateCreated" => 2, 
				"DateModified" => 3, 
				"IsFixedRole" => 4, 
				"Owner" => 5, 
				"PolicyHealthState" => 6, 
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
			staticMetadata = new StaticMetadata[7]
			{
				new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("DateCreated", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DateModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("IsFixedRole", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private ServerRoleEvents events;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateCreated => (DateTime)base.Properties.GetValueWithNullReplacement("DateCreated");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsFixedRole => (bool)base.Properties.GetValueWithNullReplacement("IsFixedRole");

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	public ServerRoleEvents Events
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
				events = new ServerRoleEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Role";

	public ServerRole()
	{
	}

	public ServerRole(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
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

	internal ServerRole(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
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

	public void Rename(string newname)
	{
		RenameImpl(newname);
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

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		ThrowIfSourceOrDestBelowVersion110(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateAlterNotSupported);
		Property property = base.Properties.Get("Owner");
		if (property.Dirty && string.IsNullOrEmpty(property.Value as string))
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ServerRoleOwnerNameEmpty);
		}
		ScriptChangeOwner(alterQuery, sp);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		ThrowIfSourceOrDestBelowVersion110(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateAlterNotSupported);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER SERVER ROLE {0} WITH NAME={1}", new object[2]
		{
			FormatFullNameForScripting(sp),
			SqlSmoObject.MakeSqlBraket(newName)
		}));
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		ThrowIfSourceOrDestBelowVersion110(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateAlterNotSupported);
		StringBuilder stringBuilder = new StringBuilder(string.Format(SmoApplication.DefaultCulture, Scripts.DECLARE_ROLE_MEMEBER, new object[1] { SqlSmoObject.EscapeString(Name, '\'') }));
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(Scripts.INCLUDE_EXISTS_SERVER_ROLE_MEMBERS);
		}
		stringBuilder.Append(Scripts.IS_SERVER_ROLE_FIXED_OR_PUBLIC);
		stringBuilder.Append(Scripts.DROP_SERVER_ROLE_MEMBERS);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ServerRole", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SERVER_ROLE, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			}));
		}
		stringBuilder.AppendLine();
		stringBuilder.Append("DROP SERVER ROLE " + FormatFullNameForScripting(sp, nameIsIndentifier: true));
		dropQuery.Add(stringBuilder.ToString());
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		ThrowIfSourceOrDestBelowVersion110(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.CreateAlterNotSupported);
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ServerRole", FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SERVER_ROLE, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append("CREATE SERVER ROLE " + FormatFullNameForScripting(sp, nameIsIndentifier: true));
		Property property = base.Properties.Get("Owner");
		if (property.Dirty && property.Value as string == string.Empty)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ServerRoleOwnerNameEmpty);
		}
		if (property.Value != null && (property.Dirty || sp.IncludeScripts.Owner))
		{
			stringBuilder.Append(" AUTHORIZATION " + SqlSmoObject.MakeSqlBraket(property.Value.ToString()));
		}
		createQuery.Add(stringBuilder.ToString());
	}

	internal override void ScriptAssociations(StringCollection rolesCmd, ScriptingPreferences sp)
	{
		ThrowIfSourceOrDestBelowVersion110(sp.TargetServerVersionInternal);
		StringCollection stringCollection = EnumServerRoleMemberships();
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				rolesCmd.Add(ScriptAddMembershipToRole(current));
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

	private string ScriptAddMembershipToRole(string role)
	{
		ThrowIfBelowVersion110();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER ROLE {0} ADD MEMBER {1}", new object[2]
		{
			SqlSmoObject.MakeSqlBraket(role),
			SqlSmoObject.MakeSqlBraket(Name)
		});
		return stringBuilder.ToString();
	}

	private string ScriptDropMembershipFromRole(string role)
	{
		ThrowIfBelowVersion110();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER ROLE {0} DROP MEMBER {1}", new object[2]
		{
			SqlSmoObject.MakeSqlBraket(role),
			SqlSmoObject.MakeSqlBraket(Name)
		});
		return stringBuilder.ToString();
	}

	public StringCollection EnumContainingRoleNames()
	{
		StringCollection stringCollection = new StringCollection();
		try
		{
			ThrowIfBelowVersion110();
			CheckObjectState();
			if (IsFixedRole || ID == 2)
			{
				return stringCollection;
			}
			StringBuilder stringBuilder = new StringBuilder(base.Urn.Parent);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "/Role/Member[@Name='{0}']", new object[1] { Urn.EscapeString(Name) });
			Request request = new Request(stringBuilder.ToString(), new string[0]);
			request.ParentPropertiesRequests = new PropertiesRequest[1]
			{
				new PropertiesRequest(new string[1] { "Name" })
			};
			DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
			if (enumeratorData.Rows.Count == 0)
			{
				return stringCollection;
			}
			foreach (DataRow row in enumeratorData.Rows)
			{
				stringCollection.Add(Convert.ToString(row[0], SmoApplication.DefaultCulture));
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumContainingRoles, this, ex);
		}
	}

	public void AddMember(string memberName)
	{
		try
		{
			CheckObjectState();
			if (memberName == null)
			{
				throw new ArgumentNullException("memberName");
			}
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			if (VersionUtils.IsSql11OrLater(base.ServerVersion))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER ROLE {0} ADD MEMBER {1}", new object[2]
				{
					SqlSmoObject.MakeSqlBraket(Name),
					SqlSmoObject.MakeSqlBraket(memberName)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master..sp_addsrvrolemember @loginame = N'{0}', @rolename = N'{1}'", new object[2]
				{
					SqlSmoObject.SqlString(memberName),
					SqlSmoObject.SqlString(InternalName)
				});
			}
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddMember, this, ex);
		}
	}

	public void DropMember(string memberName)
	{
		try
		{
			CheckObjectState();
			if (memberName == null)
			{
				throw new ArgumentNullException("memberName");
			}
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			if (VersionUtils.IsSql11OrLater(base.ServerVersion))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER ROLE {0} DROP MEMBER {1}", new object[2]
				{
					SqlSmoObject.MakeSqlBraket(Name),
					SqlSmoObject.MakeSqlBraket(memberName)
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master..sp_dropsrvrolemember @loginame = N'{0}', @rolename = N'{1}'", new object[2]
				{
					SqlSmoObject.SqlString(memberName),
					SqlSmoObject.SqlString(InternalName)
				});
			}
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropMember, this, ex);
		}
	}

	[Obsolete]
	public StringCollection EnumServerRoleMembers()
	{
		return EnumMemberNames();
	}

	public StringCollection EnumMemberNames()
	{
		CheckObjectState();
		try
		{
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

	internal StringCollection EnumServerRoleMemberships()
	{
		CheckObjectState();
		try
		{
			StringCollection stringCollection = new StringCollection();
			Request request = new Request(string.Format(SmoApplication.DefaultCulture, "Server[@Name='{0}']/Role", new object[1] { Urn.EscapeString(GetServerName()) }));
			request.Fields = new string[1] { "Name" };
			foreach (DataRow row in ExecutionManager.GetEnumeratorData(request).Rows)
			{
				request = new Request(string.Format(SmoApplication.DefaultCulture, "Server[@Name='{0}']/Role[@Name='{1}']/Member[@Name='{2}']", new object[3]
				{
					Urn.EscapeString(GetServerName()),
					Urn.EscapeString((string)row["Name"]),
					Urn.EscapeString(Name)
				}));
				request.Fields = new string[1] { "Name" };
				if (ExecutionManager.GetEnumeratorData(request).Rows.Count != 0)
				{
					stringCollection.Add((string)row["Name"]);
				}
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMembers, this, ex);
		}
	}

	public void AddMembershipToRole(string roleName)
	{
		ThrowIfBelowVersion110();
		CheckObjectState();
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(ScriptAddMembershipToRole(roleName));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddMember, this, ex);
		}
	}

	public void DropMembershipFromRole(string roleName)
	{
		ThrowIfBelowVersion110();
		CheckObjectState();
		try
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(ScriptDropMembershipFromRole(roleName));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropMember, this, ex);
		}
	}

	[Obsolete]
	public DataTable EnumServerRolePermissions()
	{
		try
		{
			CheckObjectState();
			if (!IsFixedRole)
			{
				return new DataTable();
			}
			string query = string.Format(SmoApplication.DefaultCulture, "EXEC master..sp_srvrolepermission @srvrolename = N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
			DataSet dataSet = ExecutionManager.ExecuteWithResults(query);
			if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count == 0)
			{
				return new DataTable();
			}
			return dataSet.Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPermissions, this, ex);
		}
	}

	public DataTable EnumAgentProxyAccounts()
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_enum_login_for_proxy @name={0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
			stringCollection.Add(stringBuilder.ToString());
			return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumAgentProxyAccounts, this, ex);
		}
	}
}

using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Agent;

public sealed class ProxyAccount : AgentObjectBase, IAlterable, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 6, 6, 6, 6, 6, 6, 6, 6 };

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
				"CredentialID" => 0, 
				"CredentialIdentity" => 1, 
				"CredentialName" => 2, 
				"Description" => 3, 
				"ID" => 4, 
				"IsEnabled" => 5, 
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
				new StaticMetadata("CredentialID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("CredentialIdentity", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("CredentialName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public JobServer Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as JobServer;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int CredentialID => (int)base.Properties.GetValueWithNullReplacement("CredentialID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string CredentialIdentity => (string)base.Properties.GetValueWithNullReplacement("CredentialIdentity");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string CredentialName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("CredentialName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CredentialName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Description
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Description");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Description", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	public static string UrnSuffix => "ProxyAccount";

	public ProxyAccount()
	{
	}

	public ProxyAccount(JobServer jobServer, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = jobServer;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	private void Init(JobServer jobServer, string proxyName, string credentialName, bool enabled, string description)
	{
		ValidateName(proxyName);
		key = new SimpleObjectKey(proxyName);
		Parent = jobServer;
		m_comparer = jobServer.Parent.Databases["msdb"].StringComparer;
		CredentialName = credentialName;
		IsEnabled = enabled;
		Description = description;
	}

	public ProxyAccount(JobServer jobServer, string proxyName, string credentialName, bool enabled, string description)
	{
		Init(jobServer, proxyName, credentialName, enabled, description);
	}

	public ProxyAccount(JobServer jobServer, string proxyName, string credentialName, bool enabled)
	{
		Init(jobServer, proxyName, credentialName, enabled, " ");
	}

	public ProxyAccount(JobServer jobServer, string proxyName, string credentialName)
	{
		Init(jobServer, proxyName, credentialName, enabled: true, " ");
	}

	internal ProxyAccount(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_PROXY, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_add_proxy @proxy_name=N'{0}',", new object[1] { SqlSmoObject.SqlString(Name) });
		string s = base.Properties["CredentialName"].Value as string;
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@credential_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(s) });
		int count = 2;
		GetBoolParameter(stringBuilder, sp, "IsEnabled", "@enabled={0}", ref count);
		GetStringParameter(stringBuilder, sp, "Description", "@description=N'{0}'", ref count);
		queries.Add(stringBuilder.ToString());
		if (base.State == SqlSmoState.Creating)
		{
			return;
		}
		DataTable dataTable = EnumSubSystems();
		foreach (DataRow row in dataTable.Rows)
		{
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_proxy_to_subsystem @proxy_name={0}, @subsystem_id={1}", new object[2]
			{
				SqlSmoObject.MakeSqlString(Name),
				Enum.Format(typeof(AgentSubSystem), Enum.Parse(typeof(AgentSubSystem), row["Name"].ToString(), ignoreCase: true), "d")
			});
			queries.Add(stringBuilder.ToString());
		}
		dataTable = EnumLogins();
		foreach (DataRow row2 in dataTable.Rows)
		{
			stringBuilder.Length = 0;
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(!string.IsNullOrEmpty(row2["Name"].ToString()), "Invalid login name");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_login_to_proxy @proxy_name={0}, @login_name={1}", new object[2]
			{
				SqlSmoObject.MakeSqlString(Name),
				SqlSmoObject.MakeSqlString(row2["Name"].ToString())
			});
			queries.Add(stringBuilder.ToString());
		}
		dataTable = EnumServerRoles();
		foreach (DataRow row3 in dataTable.Rows)
		{
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_login_to_proxy @proxy_name={0}, @fixed_server_role={1}", new object[2]
			{
				SqlSmoObject.MakeSqlString(Name),
				SqlSmoObject.MakeSqlString(row3["Name"].ToString())
			});
			queries.Add(stringBuilder.ToString());
		}
		dataTable = EnumMsdbRoles();
		foreach (DataRow row4 in dataTable.Rows)
		{
			stringBuilder.Length = 0;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_login_to_proxy @proxy_name={0}, @msdb_role={1}", new object[2]
			{
				SqlSmoObject.MakeSqlString(Name),
				SqlSmoObject.MakeSqlString(row4["Name"].ToString())
			});
			queries.Add(stringBuilder.ToString());
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_proxy @proxy_name=N'{0}',", new object[1] { SqlSmoObject.SqlString(Name) });
		string s = base.Properties["CredentialName"].Value as string;
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@credential_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(s) });
		int count = 1;
		GetBoolParameter(stringBuilder, sp, "IsEnabled", "@enabled={0}", ref count);
		GetStringParameter(stringBuilder, sp, "Description", "@description=N'{0}'", ref count);
		queries.Add(stringBuilder.ToString());
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
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AGENT_PROXY, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_delete_proxy @proxy_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		queries.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_update_proxy @proxy_name=N'{0}', @new_name=N'{1}'", new object[2]
		{
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(newName)
		});
		queries.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void AddSubSystem(AgentSubSystem subSystem)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_proxy_to_subsystem @proxy_name=N'{0}', @subsystem_id={1}", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				Enum.Format(typeof(AgentSubSystem), subSystem, "d")
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddSubSystems, this, ex);
		}
	}

	public void RemoveSubSystem(AgentSubSystem subSystem)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_revoke_proxy_from_subsystem @proxy_name=N'{0}', @subsystem_id={1}", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				Enum.Format(typeof(AgentSubSystem), subSystem, "d")
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveSubSystems, this, ex);
		}
	}

	public DataTable EnumSubSystems()
	{
		try
		{
			ThrowIfBelowVersion90();
			Request req = new Request(string.Concat(base.Urn, "/AgentSubSystem"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumSubSystems, this, ex);
		}
	}

	public void AddLogin(string loginName)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_login_to_proxy @proxy_name=N'{0}', @login_name={1}", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.MakeSqlString(loginName)
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddLoginToProxyAccount, this, ex);
		}
	}

	private void RemovePrincipal(string principalName, string exceptionString)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_revoke_login_from_proxy @proxy_name=N'{0}', @name={1}", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.MakeSqlString(principalName)
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(exceptionString, this, ex);
		}
	}

	public void RemoveLogin(string loginName)
	{
		RemovePrincipal(loginName, ExceptionTemplatesImpl.RemoveLoginFromProxyAccount);
	}

	public DataTable EnumLogins()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/ProxyAccountPrincipal[@Flag=0]")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLoginsOfProxyAccount, this, ex);
		}
	}

	public void AddServerRole(string serverRoleName)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_login_to_proxy @proxy_name={0}, @fixed_server_role={1}", new object[2]
			{
				SqlSmoObject.MakeSqlString(Name),
				SqlSmoObject.MakeSqlString(serverRoleName)
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddServerRoleToProxyAccount, this, ex);
		}
	}

	public void RemoveServerRole(string serverRoleName)
	{
		RemovePrincipal(serverRoleName, ExceptionTemplatesImpl.RemoveServerRoleFromProxyAccount);
	}

	public DataTable EnumServerRoles()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/ProxyAccountPrincipal[@Flag=1]")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumServerRolesOfProxyAccount, this, ex);
		}
	}

	public void AddMsdbRole(string msdbRoleName)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_grant_login_to_proxy @proxy_name={0}, @msdb_role={1}", new object[2]
			{
				SqlSmoObject.MakeSqlString(Name),
				SqlSmoObject.MakeSqlString(msdbRoleName)
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddMSDBRoleToProxyAccount, this, ex);
		}
	}

	public void RemoveMsdbRole(string msdbRoleName)
	{
		RemovePrincipal(msdbRoleName, ExceptionTemplatesImpl.RemoveMSDBRoleFromProxyAccount);
	}

	public DataTable EnumMsdbRoles()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/ProxyAccountPrincipal[@Flag=2]")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMSDBRolesOfProxyAccount, this, ex);
		}
	}

	public void Reassign(string targetProxyAccountName)
	{
		ThrowIfBelowVersion110();
		CheckObjectState(throwIfNotCreated: true);
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_reassign_proxy @current_proxy_name={0}, @target_proxy_name={1}", new object[2]
		{
			SqlSmoObject.MakeSqlString(Name),
			SqlSmoObject.MakeSqlString(targetProxyAccountName)
		});
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}
}

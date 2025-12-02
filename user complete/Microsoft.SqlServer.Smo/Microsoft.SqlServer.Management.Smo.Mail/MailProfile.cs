using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Mail;

public sealed class MailProfile : ScriptNameObjectBase, IAlterable, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 2, 2, 2, 2, 2, 2, 2, 2 };

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
			staticMetadata = new StaticMetadata[2]
			{
				new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
			};
		}
	}

	private bool forceDeleteForActiveProfiles = true;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public SqlMail Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as SqlMail;
		}
		set
		{
			SetParentImpl(value);
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

	public static string UrnSuffix => "MailProfile";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ForceDeleteForActiveProfiles
	{
		get
		{
			return forceDeleteForActiveProfiles;
		}
		set
		{
			forceDeleteForActiveProfiles = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsBusyProfile
	{
		get
		{
			string query = string.Format(CultureInfo.InvariantCulture, "use msdb\r\nSELECT COUNT(*) from sysmail_unsentitems WHERE sysmail_unsentitems.profile_id = {0}", new object[1] { ID });
			return (int)ExecutionManager.ExecuteScalar(query) != 0;
		}
	}

	internal int ProfileIDInternal => (int)base.Properties["ID"].Value;

	public MailProfile()
	{
	}

	public MailProfile(SqlMail sqlMail, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = sqlMail;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public MailProfile(SqlMail parent, string name, string description)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		base.Properties.Get("Description").Value = description;
	}

	internal MailProfile(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
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
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, SqlSmoObject.SqlBraket(Name), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_MAIL_PROFILE, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("BEGIN");
			stringBuilder.Append(sp.NewLine);
		}
		string name = GetName(sp);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_add_profile_sp @profile_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(name) });
		int count = 1;
		GetStringParameter(stringBuilder, sp, "Description", "@description=N'{0}'", ref count);
		queries.Add(stringBuilder.ToString());
		if (!sp.ScriptForCreateDrop && sp.Mail.Accounts)
		{
			DataTable dataTable = EnumAccounts();
			foreach (DataRow row in dataTable.Rows)
			{
				stringBuilder = ScriptAddAccount(name, Convert.ToString(row["AccountName"], CultureInfo.InvariantCulture), Convert.ToInt32(row["SequenceNumber"], CultureInfo.InvariantCulture));
				queries.Add(stringBuilder.ToString());
			}
		}
		if (!sp.ScriptForCreateDrop && sp.Mail.Principals)
		{
			DataTable dataTable2 = EnumPrincipals();
			foreach (DataRow row2 in dataTable2.Rows)
			{
				stringBuilder = ScriptAddPrincipal(name, Convert.ToString(row2["PrincipalName"], CultureInfo.InvariantCulture), Convert.ToBoolean(row2["IsDefault"], CultureInfo.InvariantCulture));
				queries.Add(stringBuilder.ToString());
			}
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			queries.Add(sp.NewLine + "END" + sp.NewLine);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_update_profile_sp @profile_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
		int count = 1;
		GetStringParameter(stringBuilder, sp, "Description", "@description=N'{0}'", ref count);
		if (1 < count)
		{
			queries.Add(stringBuilder.ToString());
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

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, SqlSmoObject.SqlBraket(GetName(sp)), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_MAIL_PROFILE, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_delete_profile_sp @profile_name=N'{0}', @force_delete={1}", new object[2]
		{
			SqlSmoObject.SqlString(Name),
			forceDeleteForActiveProfiles
		});
		queries.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_update_profile_sp @profile_id={0}, @profile_name=N'{1}'", new object[2]
		{
			ProfileIDInternal,
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

	public void AddAccount(string accountName, int sequenceNumber)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringBuilder stringBuilder = ScriptAddAccount(Name, accountName, sequenceNumber);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddMailAccountToProfile, this, ex);
		}
	}

	private StringBuilder ScriptAddAccount(string profileName, string accountName, int sequenceNumber)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_add_profileaccount_sp @profile_name=N'{0}', @account_name=N'{1}', @sequence_number={2}", new object[3]
		{
			SqlSmoObject.SqlString(profileName),
			SqlSmoObject.SqlString(accountName),
			sequenceNumber
		});
		return stringBuilder;
	}

	public void RemoveAccount(string accountName)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_delete_profileaccount_sp @profile_name=N'{0}', @account_name=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(accountName)
			});
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemoveMailAccountFromProfile, this, ex);
		}
	}

	public DataTable EnumAccounts()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			Request req = new Request(string.Concat(base.Urn, "/MailProfileAccount"));
			return ExecutionManager.GetEnumeratorData(req);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMailAccountsForProfile, this, ex);
		}
	}

	public void AddPrincipal(string principalName)
	{
		AddPrincipal(principalName, isDefaultProfile: true);
	}

	public void AddPrincipal(string principalName, bool isDefaultProfile)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = ScriptAddPrincipal(Name, principalName, isDefaultProfile);
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddPrincipalToMailProfile, this, ex);
		}
	}

	private StringBuilder ScriptAddPrincipal(string profileName, string principalName, bool isDefaultProfile)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_add_principalprofile_sp @principal_name=N'{0}', @profile_name=N'{1}', @is_default={2}", new object[3]
		{
			SqlSmoObject.SqlString(principalName),
			SqlSmoObject.SqlString(profileName),
			isDefaultProfile ? 1 : 0
		});
		return stringBuilder;
	}

	public void RemovePrincipal(string principalName)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_delete_principalprofile_sp @principal_name=N'{0}', @profile_name=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(principalName),
				SqlSmoObject.SqlString(Name)
			});
			stringCollection.Add(stringBuilder.ToString());
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemovePrincipalFromMailProfile, this, ex);
		}
	}

	public DataTable EnumPrincipals()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/MailProfilePrincipal")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumPrincipalsForMailProfile, this, ex);
		}
	}
}

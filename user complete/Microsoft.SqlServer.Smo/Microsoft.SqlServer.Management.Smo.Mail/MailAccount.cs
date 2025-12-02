using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo.Mail;

public sealed class MailAccount : ScriptNameObjectBase, IAlterable, ICreatable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 5, 5, 5, 5, 5, 5, 5, 5 };

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
				"DisplayName" => 1, 
				"EmailAddress" => 2, 
				"ID" => 3, 
				"ReplyToAddress" => 4, 
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
				new StaticMetadata("Description", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DisplayName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EmailAddress", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("ReplyToAddress", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	private MailServerCollection mailServers;

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
	public string DisplayName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DisplayName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DisplayName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string EmailAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EmailAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EmailAddress", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ReplyToAddress
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ReplyToAddress");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReplyToAddress", value);
		}
	}

	public static string UrnSuffix => "MailAccount";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsBusyAccount
	{
		get
		{
			string query = string.Format(CultureInfo.InvariantCulture, "USE msdb\r\nSELECT COUNT(*) FROM dbo.sysmail_unsentitems,sysmail_profileaccount,sysmail_account WHERE sysmail_unsentitems.profile_id = sysmail_profileaccount.profile_id AND sysmail_profileaccount.account_id = sysmail_account.account_id AND sysmail_account.account_id = {0}", new object[1] { ID });
			return (int)ExecutionManager.ExecuteScalar(query) != 0;
		}
	}

	[SfcObject(SfcContainerRelationship.ObjectContainer, SfcContainerCardinality.ZeroToAny, typeof(MailServer))]
	public MailServerCollection MailServers
	{
		get
		{
			if (mailServers == null)
			{
				mailServers = new MailServerCollection(this);
			}
			return mailServers;
		}
	}

	public MailAccount()
	{
	}

	public MailAccount(SqlMail sqlMail, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = sqlMail;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public MailAccount(SqlMail parent, string name, string description)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		base.Properties.Get("Description").Value = description;
	}

	public MailAccount(SqlMail parent, string name, string description, string displayName, string emailAddress)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		base.Properties.Get("Description").Value = description;
		base.Properties.Get("DisplayName").Value = displayName;
		base.Properties.Get("EmailAddress").Value = emailAddress;
	}

	internal MailAccount(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	public string[] GetAccountProfileNames()
	{
		string query = string.Format(CultureInfo.InvariantCulture, "SELECT p.[name]\r\n                 FROM [msdb].[dbo].[sysmail_profile] as p\r\n                 JOIN\t[msdb].[dbo].[sysmail_profileaccount] as pa\r\n                 ON p.profile_id = pa.profile_id\r\n                 WHERE pa.account_id = {0}", new object[1] { ID });
		DataSet dataSet = ExecutionManager.ExecuteWithResults(query);
		string[] array = new string[dataSet.Tables[0].Rows.Count];
		for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
		{
			array[i] = dataSet.Tables[0].Rows[i][0] as string;
		}
		return array;
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
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_MAIL_ACCOUNT, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_add_account_sp @account_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(GetName(sp)) });
		int count = 1;
		GetStringParameter(stringBuilder, sp, "EmailAddress", "@email_address=N'{0}'", ref count, throwIfNotSet: true);
		GetStringParameter(stringBuilder, sp, "DisplayName", "@display_name=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "ReplyToAddress", "@replyto_address=N'{0}'", ref count);
		GetStringParameter(stringBuilder, sp, "Description", "@description=N'{0}'", ref count);
		queries.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		MailServer mailServer = MailServers[0];
		mailServer.ScriptMailServer(queries, sp);
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
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, SqlSmoObject.SqlBraket(Name), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_MAIL_ACCOUNT, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_delete_account_sp @account_name=N'{0}'", new object[1] { SqlSmoObject.SqlString(GetName(sp)) });
		queries.Add(stringBuilder.ToString());
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		MailServer mailServer = MailServers[0];
		mailServer.ScriptMailServer(queries, new ScriptingPreferences(), newName, null);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}
}

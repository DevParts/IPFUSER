using System;
using System.Collections.Specialized;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo.Mail;

public sealed class MailServer : ScriptNameObjectBase, IRenamable, IAlterable, IScriptable
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
				"EnableSsl" => 0, 
				"Port" => 1, 
				"ServerType" => 2, 
				"UseDefaultCredentials" => 3, 
				"UserName" => 4, 
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
				new StaticMetadata("EnableSsl", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Port", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ServerType", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("UseDefaultCredentials", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("UserName", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	private SqlSecureString password = string.Empty;

	private bool noCredentialChange;

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public MailAccount Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as MailAccount;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool EnableSsl
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("EnableSsl");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EnableSsl", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int Port
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("Port");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Port", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ServerType => (string)base.Properties.GetValueWithNullReplacement("ServerType");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool UseDefaultCredentials
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("UseDefaultCredentials");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UseDefaultCredentials", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string UserName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("UserName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UserName", value);
		}
	}

	public static string UrnSuffix => "MailServer";

	internal SqlSecureString Password => password;

	public bool NoCredentialChange
	{
		get
		{
			return noCredentialChange;
		}
		set
		{
			noCredentialChange = value;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal MailServer(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void SetAccount(string userName, string password)
	{
		SetAccount(userName, (password != null) ? new SqlSecureString(password) : null);
	}

	public void SetAccount(string userName, SecureString password)
	{
		if (userName == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetAccount, this, new ArgumentNullException("userName"));
		}
		if (userName.Length == 0)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.SetAccount, this, new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("userName", "string")));
		}
		base.Properties.Get("UserName").Value = userName;
		this.password = password;
		SetAccountPasswordInternal(setAccount: true);
	}

	public void SetPassword(string password)
	{
		SetPassword((password != null) ? new SqlSecureString(password) : null);
	}

	public void SetPassword(SecureString password)
	{
		this.password = password;
		SetAccountPasswordInternal(setAccount: false);
	}

	private void SetAccountPasswordInternal(bool setAccount)
	{
		try
		{
			StringCollection queries = new StringCollection();
			ScriptMailServer(queries, new ScriptingPreferences());
			ExecutionManager.ExecuteNonQuery(queries);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(setAccount ? ExceptionTemplatesImpl.SetMailServerAccount : ExceptionTemplatesImpl.SetMailServerPassword, this, ex);
		}
	}

	internal void ScriptMailServer(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptMailServer(queries, sp, null, null);
	}

	internal void ScriptMailServer(StringCollection queries, ScriptingPreferences sp, string newAccountName, string newServerName)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		Initialize(allProperties: true);
		MailAccount mailAccount = base.ParentColl.ParentInstance as MailAccount;
		mailAccount.Initialize(allProperties: true);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sysmail_update_account_sp @account_name=N'{0}', @description=N'{1}', @email_address=N'{2}', @display_name=N'{3}', @replyto_address=N'{4}', @mailserver_name=N'{5}', @mailserver_type=N'{6}', @port={7}, @username=N'{8}', @password=N'{9}', @use_default_credentials={10}, @enable_ssl={11}", (newAccountName == null) ? SqlSmoObject.SqlString(mailAccount.GetName(sp)) : SqlSmoObject.SqlString(newAccountName), SqlSmoObject.SqlString(mailAccount.Description), SqlSmoObject.SqlString(mailAccount.EmailAddress), SqlSmoObject.SqlString(mailAccount.DisplayName), SqlSmoObject.SqlString(mailAccount.ReplyToAddress), (newServerName == null) ? SqlSmoObject.SqlString(GetName(sp)) : SqlSmoObject.SqlString(newServerName), SqlSmoObject.SqlString((string)base.Properties.Get("ServerType").Value), (int)base.Properties.Get("Port").Value, SqlSmoObject.SqlString((string)base.Properties.Get("UserName").Value), SqlSmoObject.SqlString((string)Password), ((bool)base.Properties.Get("UseDefaultCredentials").Value) ? "1" : "0", ((bool)base.Properties.Get("EnableSsl").Value) ? "1" : "0");
		if (NoCredentialChange)
		{
			bool flag = true;
			Version version = new Version(9, 0, 4230);
			Version version2 = (Version)base.ServerVersion;
			if (base.ServerVersion.Major == 9 && version2 < version)
			{
				flag = false;
			}
			else if (base.ServerVersion.Major == 10)
			{
				Version version3 = new Version(10, 0, 1815);
				Version version4 = new Version(10, 0, 2531);
				Version version5 = new Version(10, 0, 2732);
				if (version2 <= version3)
				{
					flag = false;
				}
				else if (version2 < version4)
				{
					flag = true;
				}
				else if (version2 <= version5)
				{
					flag = false;
				}
			}
			if (flag)
			{
				stringBuilder.Append(", @no_credential_change=1");
			}
		}
		if (newAccountName != null)
		{
			stringBuilder.AppendFormat(", @account_id={0}", mailAccount.ID);
		}
		queries.Add(stringBuilder.ToString());
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, SqlSmoObject.SqlBraket(Name), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
			queries.Add(stringBuilder.ToString());
		}
		ScriptMailServer(queries, sp);
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptMailServer(queries, sp);
	}

	public void Rename(string newName)
	{
		RenameImpl(newName);
	}

	internal override void ScriptRename(StringCollection queries, ScriptingPreferences sp, string newName)
	{
		ScriptMailServer(queries, new ScriptingPreferences(), null, newName);
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

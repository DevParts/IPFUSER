using System;
using System.Collections.Specialized;
using System.Data;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
public sealed class Credential : NamedSmoObject, ISfcSupportsDesignMode, ICreatable, IAlterable, IDroppable, IDropIfExists
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 4, 7, 7, 7, 7, 7, 7, 7 };

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
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"ID" => 2, 
				"Identity" => 3, 
				"MappedClassType" => 4, 
				"PolicyHealthState" => 5, 
				"ProviderName" => 6, 
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
				new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("MappedClassType", expensive: false, readOnly: false, typeof(MappedClassType)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("ProviderName", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	private SqlSecureString secret;

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

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Identity
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Identity");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Identity", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "None")]
	public MappedClassType MappedClassType
	{
		get
		{
			return (MappedClassType)base.Properties.GetValueWithNullReplacement("MappedClassType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MappedClassType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "string.empty")]
	[SfcReference(typeof(CryptographicProvider), "Server[@Name = '{0}']/CryptographicProvider[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "ProviderName" })]
	[CLSCompliant(false)]
	public string ProviderName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ProviderName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ProviderName", value);
		}
	}

	public static string UrnSuffix => "Credential";

	public Credential()
	{
	}

	public Credential(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"MappedClassType" => MappedClassType.None, 
			"ProviderName" => "string.empty", 
			_ => base.GetPropertyDefaultValue(propname), 
		};
	}

	internal Credential(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[2] { "ProviderName", "MappedClassType" };
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Create(string identity)
	{
		Create(identity, (string)null);
	}

	public void Create(string identity, string secret)
	{
		Create(identity, (secret != null) ? new SqlSecureString(secret) : null);
	}

	public void Create(string identity, SecureString secret)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		if (identity.Length == 0)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("identity", "string"));
		}
		Identity = identity;
		this.secret = secret;
		Create();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (sp.IncludeScripts.Header)
		{
			queries.Add(ExceptionTemplates.IncludeHeader(UrnSuffix, FullQualifiedName, DateTime.Now.ToString(GetDbCulture())));
		}
		queries.Add(CreateAlterScript(create: true, sp));
	}

	public void Alter()
	{
		AlterImpl();
	}

	public void Alter(string identity)
	{
		Alter(identity, (string)null);
	}

	public void Alter(string identity, string secret)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		if (identity.Length == 0)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("identity", "string"));
		}
		Identity = identity;
		this.secret = ((secret != null) ? new SqlSecureString(secret) : null);
		Alter();
	}

	public void Alter(string identity, SecureString secret)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		if (identity.Length == 0)
		{
			throw new ArgumentException(ExceptionTemplatesImpl.EmptyInputParam("identity", "string"));
		}
		Identity = identity;
		this.secret = secret;
		Alter();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		if (IsObjectDirty())
		{
			queries.Add(CreateAlterScript(create: false, sp));
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
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_CREDENTIAL, "", ID);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP CREDENTIAL {0}", new object[1] { FullQualifiedName });
		queries.Add(stringBuilder.ToString());
	}

	public StringCollection EnumLogins()
	{
		CheckObjectState();
		try
		{
			StringCollection stringCollection = new StringCollection();
			if (base.IsDesignMode)
			{
				foreach (Login login in Parent.Logins)
				{
					if (login.EnumCredentials().Contains(Name))
					{
						stringCollection.Add(login.Name);
					}
				}
			}
			else
			{
				DataTable dataTable;
				if (base.ServerVersion.Major < 10)
				{
					string text = string.Format(SmoApplication.DefaultCulture, Parent.Urn.Value + "/Login[@Credential = '{0}']", new object[1] { Urn.EscapeString(Name) });
					Request req = new Request(text, new string[1] { "Name" });
					dataTable = ExecutionManager.GetEnumeratorData(req);
				}
				else
				{
					string query = string.Format(SmoApplication.DefaultCulture, "select name from sys.server_principal_credentials as c join sys.server_principals as p on p.principal_id = c.principal_id where c.credential_id = {0}", new object[1] { ID.ToString() });
					dataTable = ExecutionManager.ExecuteWithResults(query).Tables[0];
				}
				foreach (DataRow row in dataTable.Rows)
				{
					string value = row[0].ToString();
					if (!stringCollection.Contains(value))
					{
						stringCollection.Add(value);
					}
				}
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumLogins, this, ex);
		}
	}

	private string CreateAlterScript(bool create, ScriptingPreferences sp)
	{
		string text = (string)GetPropValue("Identity");
		if (text == null || text.Length == 0)
		{
			throw new PropertyNotSetException("Identity");
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (create)
		{
			ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		}
		if (create && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_CREDENTIAL, "NOT", ID);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} CREDENTIAL {1} WITH IDENTITY = N'{2}'", new object[3]
		{
			create ? "CREATE" : "ALTER",
			FullQualifiedName,
			SqlSmoObject.SqlString(Identity)
		});
		if (secret != null)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", SECRET = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)secret) });
			secret = null;
		}
		if (create && base.ServerVersion.Major >= 10)
		{
			Property property = base.Properties.Get("MappedClassType");
			if (property.Dirty && (MappedClassType)property.Value == MappedClassType.CryptographicProvider)
			{
				string text2 = (string)GetPropValue("ProviderName");
				if (string.IsNullOrEmpty(text2))
				{
					throw new PropertyNotSetException("ProviderName");
				}
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FOR CRYPTOGRAPHIC PROVIDER {0}", new object[1] { SqlSmoObject.MakeSqlBraket(text2) });
			}
		}
		return stringBuilder.ToString();
	}
}

using System;
using System.Collections.Specialized;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class DatabaseScopedCredential : NamedSmoObject, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 4, 4, 4 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 4 };

		private static int sqlDwPropertyCount = 4;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Identity", expensive: false, readOnly: false, typeof(string))
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
						"Identity" => 3, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"ID" => 2, 
					"Identity" => 3, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"ID" => 2, 
				"Identity" => 3, 
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

	private SqlSecureString secret;

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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	public static string UrnSuffix => "DatabaseScopedCredential";

	public DatabaseScopedCredential()
	{
	}

	public DatabaseScopedCredential(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal DatabaseScopedCredential(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
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
		if (Util.IsNullOrWhiteSpace(identity))
		{
			throw new ArgumentNullException("identity");
		}
		Create(identity, (!Util.IsNullOrWhiteSpace(secret)) ? new SqlSecureString(secret) : null);
	}

	public void Create(string identity, SecureString secret)
	{
		if (Util.IsNullOrWhiteSpace(identity))
		{
			throw new ArgumentNullException("identity");
		}
		Identity = identity;
		this.secret = secret;
		Create();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		queries.Add(CreateAlterScript(queries, create: true, sp));
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
		if (Util.IsNullOrWhiteSpace(identity))
		{
			throw new ArgumentNullException("identity");
		}
		Identity = identity;
		this.secret = ((!Util.IsNullOrWhiteSpace(secret)) ? new SqlSecureString(secret) : null);
		Alter();
	}

	public void Alter(string identity, SecureString secret)
	{
		if (Util.IsNullOrWhiteSpace(identity))
		{
			throw new ArgumentNullException("identity");
		}
		Identity = identity;
		this.secret = secret;
		Alter();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty())
		{
			queries.Add(CreateAlterScript(queries, create: false, sp));
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
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_DATABASESCOPEDCREDENTIAL, "", ID);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP DATABASE SCOPED CREDENTIAL {0}", new object[1] { FormatFullNameForScripting(sp) });
		queries.Add(stringBuilder.ToString());
	}

	private string CreateAlterScript(StringCollection queries, bool create, ScriptingPreferences sp)
	{
		string value = (string)GetPropValue("Identity");
		if (Util.IsNullOrWhiteSpace(value))
		{
			throw new PropertyNotSetException("Identity");
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (create)
		{
			ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		}
		if (sp.IncludeScripts.DatabaseContext)
		{
			AddDatabaseContext(queries, sp);
		}
		if (create && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_DATABASESCOPEDCREDENTIAL, "NOT", ID);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0} DATABASE SCOPED CREDENTIAL {1} WITH IDENTITY = N'{2}'", new object[3]
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
		return stringBuilder.ToString();
	}
}

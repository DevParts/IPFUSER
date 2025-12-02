using System;
using System.Collections.Specialized;
using System.Data;
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
public sealed class LinkedServer : NamedSmoObject, ISfcSupportsDesignMode, ICreatable, IDroppable, IDropIfExists, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 15, 20, 21, 23, 23, 23, 23, 23, 23, 23 };

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
				"Catalog" => 0, 
				"CollationCompatible" => 1, 
				"DataAccess" => 2, 
				"DataSource" => 3, 
				"DistPublisher" => 4, 
				"Distributor" => 5, 
				"ID" => 6, 
				"Location" => 7, 
				"ProductName" => 8, 
				"ProviderName" => 9, 
				"ProviderString" => 10, 
				"Publisher" => 11, 
				"Rpc" => 12, 
				"RpcOut" => 13, 
				"Subscriber" => 14, 
				"CollationName" => 15, 
				"ConnectTimeout" => 16, 
				"LazySchemaValidation" => 17, 
				"QueryTimeout" => 18, 
				"UseRemoteCollation" => 19, 
				"DateLastModified" => 20, 
				"IsPromotionofDistributedTransactionsForRPCEnabled" => 21, 
				"PolicyHealthState" => 22, 
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
			staticMetadata = new StaticMetadata[23]
			{
				new StaticMetadata("Catalog", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("CollationCompatible", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("DataAccess", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("DataSource", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("DistPublisher", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Distributor", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("Location", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ProductName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ProviderName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ProviderString", expensive: false, readOnly: false, typeof(SqlSecureString)),
				new StaticMetadata("Publisher", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Rpc", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("RpcOut", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Subscriber", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("CollationName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ConnectTimeout", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("LazySchemaValidation", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("QueryTimeout", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("UseRemoteCollation", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("IsPromotionofDistributedTransactionsForRPCEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	private bool dropLogins = true;

	private LinkedServerLoginCollection m_LinkedServerLogins;

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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string Catalog
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Catalog");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Catalog", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool CollationCompatible
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("CollationCompatible");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CollationCompatible", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string CollationName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("CollationName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CollationName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ConnectTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ConnectTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ConnectTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool DataAccess
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DataAccess");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataAccess", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string DataSource
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DataSource");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataSource", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool DistPublisher
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DistPublisher");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DistPublisher", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Distributor
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Distributor");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Distributor", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsPromotionofDistributedTransactionsForRPCEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsPromotionofDistributedTransactionsForRPCEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsPromotionofDistributedTransactionsForRPCEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool LazySchemaValidation
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("LazySchemaValidation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LazySchemaValidation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string Location
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Location");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Location", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public string ProductName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ProductName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ProductName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Publisher
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Publisher");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Publisher", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int QueryTimeout
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("QueryTimeout");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QueryTimeout", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Rpc
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Rpc");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Rpc", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool RpcOut
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("RpcOut");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RpcOut", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Subscriber
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Subscriber");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Subscriber", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool UseRemoteCollation
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("UseRemoteCollation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UseRemoteCollation", value);
		}
	}

	public static string UrnSuffix => "LinkedServer";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	[DmfIgnoreProperty]
	public string ProviderString
	{
		get
		{
			SqlSecureString sqlSecureString = (SqlSecureString)base.Properties.GetValueWithNullReplacement("ProviderString");
			return sqlSecureString.ToString();
		}
		set
		{
			SqlSecureString value2 = new SqlSecureString(value);
			base.Properties.SetValueWithConsistencyCheck("ProviderString", value2);
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(LinkedServerLogin))]
	public LinkedServerLoginCollection LinkedServerLogins
	{
		get
		{
			if (m_LinkedServerLogins == null)
			{
				m_LinkedServerLogins = new LinkedServerLoginCollection(this);
			}
			return m_LinkedServerLogins;
		}
	}

	public LinkedServer()
	{
	}

	public LinkedServer(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[6] { "Catalog", "DataSource", "Location", "ProductName", "ProviderName", "ProviderString" };
	}

	internal LinkedServer(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			ScriptIncludeIfNotExists(stringBuilder, sp, "NOT");
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("BEGIN").Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_addlinkedserver @server = N'{0}'", new object[1] { SqlSmoObject.SqlString(InternalName) });
		int count = 1;
		string text = (string)GetPropValueOptionalAllowNull("ProductName");
		string text2 = (string)GetPropValueOptionalAllowNull("ProviderName");
		if (text == "SQL Server")
		{
			GetStringParam(stringBuilder, sp, "ProductName", "srvproduct", ref count);
		}
		else if (text2 != null)
		{
			GetStringParamCompulsory(stringBuilder, sp, "ProductName", "srvproduct", ref count);
			GetStringParamCompulsory(stringBuilder, sp, "ProviderName", "provider", ref count);
			GetStringParam(stringBuilder, sp, "DataSource", "datasrc", ref count);
			GetStringParam(stringBuilder, sp, "Location", "location", ref count);
			GetStringParam(stringBuilder, sp, "ProviderString", "provstr", ref count);
			GetStringParam(stringBuilder, sp, "Catalog", "catalog", ref count);
		}
		stringBuilder.Append(sp.NewLine);
		if (LinkedServerLogins.Count > 0)
		{
			stringBuilder.Append(" /* For security reasons the linked server remote logins password is changed with ######## */" + sp.NewLine);
			string text3 = SqlSmoObject.SqlString(InternalName);
			foreach (LinkedServerLogin linkedServerLogin in LinkedServerLogins)
			{
				string text4 = SqlSmoObject.SqlString(linkedServerLogin.Name);
				text4 = (string.IsNullOrEmpty(text4) ? "NULL" : ("N'" + text4 + "'"));
				string text5 = null;
				object propValueOptional = linkedServerLogin.GetPropValueOptional("Impersonate");
				if (propValueOptional != null && propValueOptional.ToString().Length > 0)
				{
					text5 = SqlSmoObject.SqlString(propValueOptional.ToString());
				}
				text5 = (string.IsNullOrEmpty(text5) ? "NULL" : ("N'" + text5 + "'"));
				string text6 = null;
				object propValueOptional2 = linkedServerLogin.GetPropValueOptional("RemoteUser");
				if (propValueOptional2 != null && propValueOptional2.ToString().Length > 0)
				{
					text6 = SqlSmoObject.SqlString(propValueOptional2.ToString());
				}
				text6 = (string.IsNullOrEmpty(text6) ? "NULL" : ("N'" + text6 + "'"));
				string text7 = null;
				text7 = ((!(text6 == "NULL")) ? "'########'" : "NULL");
				string value = string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_addlinkedsrvlogin @rmtsrvname=N'{0}',@useself={1},@locallogin={2},@rmtuser={3},@rmtpassword={4}", text3, text5, text4, text6, text7);
				stringBuilder.Append(value).Append(sp.NewLine);
			}
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append("END");
		}
		query.Add(stringBuilder.ToString());
		ScriptAlter(query, sp);
	}

	private void GetStringParam(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropName, ref int count)
	{
		object propValueOptional = GetPropValueOptional(propName);
		if (propValueOptional != null && propValueOptional.ToString().Length > 0)
		{
			if (count++ > 0)
			{
				buffer.Append(Globals.commaspace);
			}
			buffer.AppendFormat(SmoApplication.DefaultCulture, "@{0}=N'{1}'", new object[2]
			{
				sqlPropName,
				SqlSmoObject.SqlString(propValueOptional.ToString())
			});
		}
	}

	private void GetStringParamCompulsory(StringBuilder buffer, ScriptingPreferences sp, string propName, string sqlPropName, ref int count)
	{
		object propValueOptional = GetPropValueOptional(propName);
		string s = ((propValueOptional != null) ? propValueOptional.ToString() : string.Empty);
		if (count++ > 0)
		{
			buffer.Append(Globals.commaspace);
		}
		buffer.AppendFormat(SmoApplication.DefaultCulture, "@{0}=N'{1}'", new object[2]
		{
			sqlPropName,
			SqlSmoObject.SqlString(s)
		});
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		GetStringOption(query, sp, "CollationCompatible", "collation compatible");
		GetStringOption(query, sp, "DataAccess", "data access");
		GetStringOption(query, sp, "Distributor", "dist");
		GetStringOption(query, sp, "Publisher", "pub");
		GetStringOption(query, sp, "Rpc", "rpc");
		GetStringOption(query, sp, "RpcOut", "rpc out");
		GetStringOption(query, sp, "Subscriber", "sub");
		if (SqlServerVersionInternal.Version80 == sp.TargetServerVersionInternal)
		{
			GetStringOption(query, sp, "DistPublisher", "dpub");
		}
		if (SqlServerVersionInternal.Version80 <= sp.TargetServerVersionInternal)
		{
			GetStringOption(query, sp, "ConnectTimeout", "connect timeout");
			GetStringOption(query, sp, "CollationName", "collation name");
			GetStringOption(query, sp, "LazySchemaValidation", "lazy schema validation");
			GetStringOption(query, sp, "QueryTimeout", "query timeout");
			GetStringOption(query, sp, "UseRemoteCollation", "use remote collation");
		}
		if (base.ServerVersion.Major >= 10 && SqlServerVersionInternal.Version100 <= sp.TargetServerVersionInternal)
		{
			GetStringOption(query, sp, "IsPromotionofDistributedTransactionsForRPCEnabled", "remote proc transaction promotion");
		}
	}

	private void GetStringOption(StringCollection queries, ScriptingPreferences sp, string propName, string optionName)
	{
		Property property = base.Properties.Get(propName);
		if (property.Value == null || (!property.Dirty && sp.ScriptForAlter))
		{
			return;
		}
		string text = string.Empty;
		if (property.Value != null)
		{
			text = property.Value.ToString();
			if (property.Value is bool)
			{
				text = text.ToLower(SmoApplication.DefaultCulture);
			}
		}
		text = ((text.Length != 0) ? SqlSmoObject.MakeSqlString(text) : "null");
		queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_serveroption @server=N'{0}', @optname=N'{1}', @optvalue={2}", new object[3]
		{
			SqlSmoObject.SqlString(InternalName),
			optionName,
			text
		}));
	}

	public void Drop()
	{
		dropLogins = false;
		DropImpl();
	}

	public void DropIfExists()
	{
		dropLogins = false;
		DropImpl(isDropIfExists: true);
	}

	public void Drop(bool dropDependentLogins)
	{
		dropLogins = dropDependentLogins;
		DropImpl();
	}

	public void DropIfExists(bool dropDependentLogins)
	{
		dropLogins = dropDependentLogins;
		DropImpl(isDropIfExists: true);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		ScriptIncludeIfNotExists(stringBuilder, sp, string.Empty);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_dropserver @server=N'{0}'", new object[1] { SqlSmoObject.SqlString(InternalName) });
		if (dropLogins)
		{
			stringBuilder.Append(", @droplogins='droplogins'");
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	protected override void PostDrop()
	{
		if (!ExecutionManager.Recording && dropLogins && m_LinkedServerLogins != null)
		{
			m_LinkedServerLogins.MarkAllDropped();
			m_LinkedServerLogins = null;
		}
	}

	private void ScriptIncludeIfNotExists(StringBuilder sb, ScriptingPreferences sp, string predicate)
	{
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LINKED_SERVER90, new object[2]
				{
					predicate,
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
			else
			{
				sb.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LINKED_SERVER80, new object[2]
				{
					predicate,
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (m_LinkedServerLogins != null)
		{
			m_LinkedServerLogins.MarkAllDropped();
		}
	}

	public DataTable EnumColumns()
	{
		return EnumColumns(null, null, null, null);
	}

	public DataTable EnumColumns(string tableName)
	{
		return EnumColumns(tableName, null, null, null);
	}

	public DataTable EnumColumns(string tableName, string schemaName)
	{
		return EnumColumns(tableName, schemaName, null, null);
	}

	public DataTable EnumColumns(string tableName, string schemaName, string databaseName)
	{
		return EnumColumns(tableName, schemaName, databaseName, null);
	}

	public DataTable EnumColumns(string tableName, string schemaName, string databaseName, string columnName)
	{
		try
		{
			StringCollection stringCollection = new StringCollection();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_columns_ex @table_server = N'{0}'", new object[1] { SqlSmoObject.SqlString(InternalName) });
			if (tableName != null && tableName.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @table_name = N'{0}'", new object[1] { SqlSmoObject.SqlString(tableName) });
			}
			if (schemaName != null && schemaName.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @table_schema = N'{0}'", new object[1] { SqlSmoObject.SqlString(schemaName) });
			}
			if (databaseName != null && databaseName.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @table_catalog = N'{0}'", new object[1] { SqlSmoObject.SqlString(databaseName) });
			}
			if (columnName != null && columnName.Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @column_name = N'{0}'", new object[1] { SqlSmoObject.SqlString(columnName) });
			}
			stringBuilder.Append(", @ODBCVer = 3");
			stringCollection.Add(stringBuilder.ToString());
			return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumColumns, this, ex);
		}
	}

	public DataTable EnumTables()
	{
		return EnumTables(null, null, null, LinkedTableType.Default);
	}

	public DataTable EnumTables(string tableName)
	{
		return EnumTables(tableName, null, null, LinkedTableType.Default);
	}

	public DataTable EnumTables(string tableName, string schemaName)
	{
		return EnumTables(tableName, schemaName, null, LinkedTableType.Default);
	}

	public DataTable EnumTables(string tableName, string schemaName, string databaseName)
	{
		return EnumTables(tableName, schemaName, databaseName, LinkedTableType.Default);
	}

	public DataTable EnumTables(string tableName, string schemaName, string databaseName, LinkedTableType tableType)
	{
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_tables_ex @table_server = N'{0}'", new object[1] { SqlSmoObject.SqlString(InternalName) });
		if (tableName != null && tableName.Length > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @table_name = N'{0}'", new object[1] { SqlSmoObject.SqlString(tableName) });
		}
		if (schemaName != null && schemaName.Length > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @table_schema = N'{0}'", new object[1] { SqlSmoObject.SqlString(schemaName) });
		}
		if (databaseName != null && databaseName.Length > 0)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @table_catalog = N'{0}'", new object[1] { SqlSmoObject.SqlString(databaseName) });
		}
		stringBuilder.Append(", @table_type = ");
		switch (tableType)
		{
		case LinkedTableType.GlobalTemporary:
			InternalAdd(stringBuilder, "GLOBAL TEMPORARY");
			break;
		case LinkedTableType.LocalTemporary:
			InternalAdd(stringBuilder, "LOCAL TEMPORARY");
			break;
		case LinkedTableType.Alias:
			InternalAdd(stringBuilder, "ALIAS");
			break;
		case LinkedTableType.SystemTable:
			InternalAdd(stringBuilder, "SYSTEM TABLE");
			break;
		case LinkedTableType.SystemView:
			InternalAdd(stringBuilder, "SYSTEM VIEW");
			break;
		case LinkedTableType.Table:
			InternalAdd(stringBuilder, "TABLE");
			break;
		case LinkedTableType.View:
			InternalAdd(stringBuilder, "VIEW");
			break;
		case LinkedTableType.Default:
			stringBuilder.Append("NULL");
			break;
		}
		stringCollection.Add(stringBuilder.ToString());
		return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
	}

	private void InternalAdd(StringBuilder stmt, string optname)
	{
		stmt.AppendFormat(SmoApplication.DefaultCulture, "'''{0}'''", new object[1] { optname });
	}

	public void TestConnection()
	{
		int major = base.ServerVersion.Major;
		if (major >= 9)
		{
			string text = SqlSmoObject.SqlString(InternalName);
			string cmd = "EXEC sp_testlinkedserver N'" + text + "'";
			ExecutionManager.ExecuteNonQuery(cmd);
			return;
		}
		throw new InvalidVersionSmoOperationException(base.ServerVersion);
	}
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[StateChangeEvent("CREATE_COLUMN_MASTER_KEY", "COLUMN_MASTER_KEY", "COLUMN_MASTER_KEY")]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[PhysicalFacet]
public sealed class ColumnMasterKey : ScriptNameObjectBase, ICreatable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 5, 5, 7 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 5 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("KeyPath", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("KeyStoreProviderName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[7]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("KeyPath", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("KeyStoreProviderName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("AllowEnclaveComputations", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Signature", expensive: false, readOnly: false, typeof(byte[]))
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
					return -1;
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"ID" => 2, 
					"KeyPath" => 3, 
					"KeyStoreProviderName" => 4, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"ID" => 2, 
				"KeyPath" => 3, 
				"KeyStoreProviderName" => 4, 
				"AllowEnclaveComputations" => 5, 
				"Signature" => 6, 
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

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public bool AllowEnclaveComputations
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AllowEnclaveComputations");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AllowEnclaveComputations", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DateLastModified", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string KeyPath
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("KeyPath");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("KeyPath", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string KeyStoreProviderName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("KeyStoreProviderName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("KeyStoreProviderName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public byte[] Signature
	{
		get
		{
			return (byte[])base.Properties.GetValueWithNullReplacement("Signature");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Signature", value);
		}
	}

	public static string UrnSuffix => "ColumnMasterKey";

	internal static string ParentType => "DATABASE";

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	internal string SignatureAsSqlBinaryString
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(2 + Signature.Length * 2);
			stringBuilder.Append("0x");
			byte[] signature = Signature;
			foreach (byte b in signature)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}
	}

	public ColumnMasterKey()
	{
	}

	public ColumnMasterKey(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[7] { "AllowEnclaveComputations", "CreateDate", "DateLastModified", "ID", "KeyPath", "KeyStoreProviderName", "Signature" };
	}

	internal ColumnMasterKey(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ColumnMasterKey(Database parent, string name, string keyStoreProviderName, string keyPath, bool allowEnclaveComputations, byte[] signature)
		: this(parent, name, keyStoreProviderName, keyPath)
	{
		if (!IsSupportedProperty("AllowEnclaveComputations") && allowEnclaveComputations)
		{
			throw new InvalidVersionSmoOperationException(ExceptionTemplatesImpl.PropertyCannotBeSetForVersion("AllowEnclaveComputations", "ColumnMasterKey", parent.Version.ToString()));
		}
		if (!IsSupportedProperty("Signature") && signature != null)
		{
			throw new InvalidVersionSmoOperationException(ExceptionTemplatesImpl.PropertyCannotBeSetForVersion("Signature", "ColumnMasterKey", parent.Version.ToString()));
		}
		AllowEnclaveComputations = allowEnclaveComputations;
		if (allowEnclaveComputations)
		{
			Signature = signature;
		}
	}

	public ColumnMasterKey(Database parent, string name, string keyStoreProviderName, string keyPath)
	{
		Parent = parent;
		Name = name;
		KeyStoreProviderName = keyStoreProviderName;
		KeyPath = keyPath;
		if (IsSupportedProperty("AllowEnclaveComputations"))
		{
			AllowEnclaveComputations = false;
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
		string text = FormatFullNameForScripting(sp);
		this.ThrowIfNotSupported(GetType(), ExceptionTemplatesImpl.ColumnMasterKeyDownlevel(text, SqlSmoObject.GetSqlServerName(sp)), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ColumnMasterKey", text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_COLUMN_MASTER_KEY, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			}));
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append("DROP COLUMN MASTER KEY " + text);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		string text = FormatFullNameForScripting(sp);
		this.ThrowIfNotSupported(GetType(), ExceptionTemplatesImpl.ColumnMasterKeyDownlevel(text, SqlSmoObject.GetSqlServerName(sp)), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ColumnMasterKey", text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_COLUMN_MASTER_KEY, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		ScriptStringBuilder scriptStringBuilder = new ScriptStringBuilder(string.Format(SmoApplication.DefaultCulture, "CREATE COLUMN MASTER KEY {0}{1}WITH{1}", new object[2] { text, sp.NewLine }), sp);
		scriptStringBuilder.SetParameter("KEY_STORE_PROVIDER_NAME", KeyStoreProviderName, ParameterValueFormat.NVarCharString);
		scriptStringBuilder.SetParameter("KEY_PATH", KeyPath, ParameterValueFormat.NVarCharString);
		if (IsSupportedProperty("AllowEnclaveComputations", sp) && AllowEnclaveComputations)
		{
			List<IScriptStringBuilderParameter> list = new List<IScriptStringBuilderParameter>();
			list.Add(new ScriptStringBuilderParameter("SIGNATURE", SignatureAsSqlBinaryString, ParameterValueFormat.NotString));
			scriptStringBuilder.SetParameter("ENCLAVE_COMPUTATIONS", list);
		}
		stringBuilder.Append(scriptStringBuilder.ToString(scriptSemiColon: false, pretty: true));
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
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

	public IList<ColumnEncryptionKeyValue> GetColumnEncryptionKeyValuesEncrypted()
	{
		IList<ColumnEncryptionKeyValue> list = new List<ColumnEncryptionKeyValue>();
		string sqlCommand = "select [cek].[name] [cekName]\r\n                                    from [sys].[column_encryption_keys] [cek]\r\n                                    join [sys].[column_encryption_key_values] [cekv]\r\n                                    on [cek].[column_encryption_key_id] = [cekv].[column_encryption_key_id]\r\n                                    where [column_master_key_id] = " + ID;
		DataSet dataSet = Parent.ExecuteWithResults(sqlCommand);
		Parent.ColumnEncryptionKeys.Refresh();
		foreach (DataRow row in dataSet.Tables[0].Rows)
		{
			foreach (ColumnEncryptionKeyValue columnEncryptionKeyValue in Parent.ColumnEncryptionKeys[(string)row["cekName"]].ColumnEncryptionKeyValues)
			{
				if (columnEncryptionKeyValue.ColumnMasterKeyID == ID)
				{
					list.Add(columnEncryptionKeyValue);
					break;
				}
			}
		}
		return list;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[4] { "KeyStoreProviderName", "KeyPath", "AllowEnclaveComputations", "Signature" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

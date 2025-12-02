using System;
using System.Collections;
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
[StateChangeEvent("ALTER_COLUMN_ENCRYPTION_KEY", "COLUMN_ENCRYPTION_KEY", "COLUMN_ENCRYPTION_KEY")]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[PhysicalFacet]
[StateChangeEvent("CREATE_COLUMN_ENCRYPTION_KEY", "COLUMN_ENCRYPTION_KEY", "COLUMN_ENCRYPTION_KEY")]
public sealed class ColumnEncryptionKey : ScriptNameObjectBase, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 3, 3, 3 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 3 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int))
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
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"ID" => 2, 
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

	private ColumnEncryptionKeyValueCollection m_cekValues;

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

	public static string UrnSuffix => "ColumnEncryptionKey";

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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ColumnEncryptionKeyValue))]
	public ColumnEncryptionKeyValueCollection ColumnEncryptionKeyValues
	{
		get
		{
			if (m_cekValues == null)
			{
				m_cekValues = new ColumnEncryptionKeyValueCollection(this);
			}
			return m_cekValues;
		}
	}

	public ColumnEncryptionKey()
	{
	}

	public ColumnEncryptionKey(Database database, string name)
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
		return new string[3] { "CreateDate", "DateLastModified", "ID" };
	}

	internal ColumnEncryptionKey(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
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
		this.ThrowIfNotSupported(GetType(), ExceptionTemplatesImpl.ColumnEncryptionKeyDownlevel(text, SqlSmoObject.GetSqlServerName(sp)), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ColumnEncryptionKey", text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_COLUMN_ENCRYPTION_KEY, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP COLUMN ENCRYPTION KEY {0}", new object[1] { text });
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
		if (ColumnEncryptionKeyValues.Count == 0)
		{
			throw new InvalidOperationException(ExceptionTemplatesImpl.ColumnEncryptionKeyNoValues(FullQualifiedName));
		}
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		string text = FormatFullNameForScripting(sp);
		this.ThrowIfNotSupported(GetType(), ExceptionTemplatesImpl.ColumnEncryptionKeyDownlevel(text, SqlSmoObject.GetSqlServerName(sp)), sp);
		StringBuilder stringBuilder = new StringBuilder();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ColumnEncryptionKey", text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_COLUMN_ENCRYPTION_KEY, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE COLUMN ENCRYPTION KEY {0}", new object[1] { text });
		stringBuilder.Append(sp.NewLine);
		stringBuilder.Append("WITH VALUES");
		stringBuilder.Append(sp.NewLine);
		bool flag = true;
		foreach (ColumnEncryptionKeyValue columnEncryptionKeyValue in ColumnEncryptionKeyValues)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.AppendLine(",");
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}{1}COLUMN_MASTER_KEY = {2},{3}{4}ALGORITHM = '{5}',{6}{7}ENCRYPTED_VALUE = {8}{9})", sp.NewLine, Globals.tab, SqlSmoObject.MakeSqlBraket(columnEncryptionKeyValue.ColumnMasterKeyName), sp.NewLine, Globals.tab, columnEncryptionKeyValue.EncryptionAlgorithm.Replace("'", "''"), sp.NewLine, Globals.tab, columnEncryptionKeyValue.EncryptedValueAsSqlBinaryString, sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (base.State == SqlSmoState.Creating)
		{
			return;
		}
		string text = FormatFullNameForScripting(sp);
		this.ThrowIfNotSupported(GetType(), ExceptionTemplatesImpl.ColumnEncryptionKeyDownlevel(text, SqlSmoObject.GetSqlServerName(sp)), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ColumnEncryptionKey", text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (ColumnEncryptionKeyValues.Count > 0)
		{
			foreach (ColumnEncryptionKeyValue columnEncryptionKeyValue in ColumnEncryptionKeyValues)
			{
				if (columnEncryptionKeyValue.State == SqlSmoState.Creating)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER COLUMN ENCRYPTION KEY {0}{1}ADD VALUE{1}", new object[2] { text, sp.NewLine });
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}{1}COLUMN_MASTER_KEY = {2},{3}{4}ALGORITHM = '{5}',{6}{7}ENCRYPTED_VALUE = {8}{9})", sp.NewLine, Globals.tab, SqlSmoObject.MakeSqlBraket(columnEncryptionKeyValue.ColumnMasterKeyName), sp.NewLine, Globals.tab, columnEncryptionKeyValue.EncryptionAlgorithm.Replace("'", "''"), sp.NewLine, Globals.tab, columnEncryptionKeyValue.EncryptedValueAsSqlBinaryString, sp.NewLine);
				}
				else if (columnEncryptionKeyValue.State == SqlSmoState.ToBeDropped)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER COLUMN ENCRYPTION KEY {0}", new object[1] { text });
					stringBuilder.Append(sp.NewLine);
					stringBuilder.AppendLine("DROP VALUE");
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}{1}COLUMN_MASTER_KEY = {2}{3})", sp.NewLine, Globals.tab, SqlSmoObject.MakeSqlBraket(columnEncryptionKeyValue.ColumnMasterKeyName), sp.NewLine);
				}
			}
			stringBuilder.Append(sp.NewLine);
		}
		alterQuery.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		ArrayList arrayList = new ArrayList();
		arrayList.Add(new PropagateInfo(ColumnEncryptionKeyValues, bWithScript: false, bPropagateScriptToChildLevel: false));
		PropagateInfo[] array = new PropagateInfo[arrayList.Count];
		arrayList.CopyTo(array, 0);
		return array;
	}

	public IList<Column> GetColumnsEncrypted()
	{
		IList<Column> list = new List<Column>();
		string sqlCommand = "select [schem].[name] [schemaName], [tab].[name] [tableName], [col].[name] [columnName]\r\n                                    from [sys].[columns] [col]\r\n                                    join [sys].[tables] [tab]\r\n                                    on [col].[object_id] = [tab].[object_id]\r\n                                    join [sys].[schemas] [schem]\r\n                                    on [schem].[schema_id] = [tab].[schema_id]\r\n                                    where [column_encryption_key_id] = " + ID;
		DataSet dataSet = Parent.ExecuteWithResults(sqlCommand);
		Parent.Tables.Refresh();
		foreach (DataRow row in dataSet.Tables[0].Rows)
		{
			string schema = (string)row["schemaName"];
			string name = (string)row["tableName"];
			string name2 = (string)row["columnName"];
			list.Add(Parent.Tables[name, schema].Columns[name2]);
		}
		return list;
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return null;
	}
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[StateChangeEvent("CREATE_COLUMN_ENCRYPTION_KEY", "COLUMN_ENCRYPTION_KEY", "COLUMN ENCRYPTION KEY")]
[PhysicalFacet]
[StateChangeEvent("ALTER_COLUMN_ENCRYPTION_KEY", "COLUMN_ENCRYPTION_KEY", "COLUMN ENCRYPTION KEY")]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class ColumnEncryptionKeyValue : SqlSmoObject, ICreatable, IDroppable, IDropIfExists, IMarkForDrop
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 5, 5, 5 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 5 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("ColumnEncryptionKeyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ColumnMasterKeyID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ColumnMasterKeyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptedValue", expensive: false, readOnly: false, typeof(byte[])),
			new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("ColumnEncryptionKeyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ColumnMasterKeyID", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("ColumnMasterKeyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("EncryptedValue", expensive: false, readOnly: false, typeof(byte[])),
			new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: false, typeof(string))
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
					"ColumnEncryptionKeyName" => 0, 
					"ColumnMasterKeyID" => 1, 
					"ColumnMasterKeyName" => 2, 
					"EncryptedValue" => 3, 
					"EncryptionAlgorithm" => 4, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ColumnEncryptionKeyName" => 0, 
				"ColumnMasterKeyID" => 1, 
				"ColumnMasterKeyName" => 2, 
				"EncryptedValue" => 3, 
				"EncryptionAlgorithm" => 4, 
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
	public ColumnEncryptionKey Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as ColumnEncryptionKey;
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ColumnEncryptionKeyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ColumnEncryptionKeyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ColumnEncryptionKeyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ColumnMasterKeyID
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("ColumnMasterKeyID");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ColumnMasterKeyID", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string ColumnMasterKeyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ColumnMasterKeyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ColumnMasterKeyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] EncryptedValue
	{
		get
		{
			return (byte[])base.Properties.GetValueWithNullReplacement("EncryptedValue");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptedValue", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string EncryptionAlgorithm
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EncryptionAlgorithm");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptionAlgorithm", value);
		}
	}

	public static string UrnSuffix => "ColumnEncryptionKeyValue";

	public string EncryptedValueAsSqlBinaryString
	{
		get
		{
			byte[] encryptedValue = EncryptedValue;
			StringBuilder stringBuilder = new StringBuilder(2 + encryptedValue.Length * 2);
			stringBuilder.Append("0x");
			byte[] array = encryptedValue;
			foreach (byte b in array)
			{
				stringBuilder.AppendFormat("{0:X2}", b);
			}
			return stringBuilder.ToString();
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[5] { "ColumnEncryptionKeyName", "ColumnMasterKeyID", "ColumnMasterKeyName", "EncryptedValue", "EncryptionAlgorithm" };
	}

	internal ColumnEncryptionKeyValue(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ColumnEncryptionKeyValue(ColumnEncryptionKey parent, ColumnMasterKey cmk, string encryptionAlgorithm, byte[] encryptedValue)
	{
		key = new ColumnEncryptionKeyValueObjectKey(cmk.ID);
		SetParentImpl(parent);
		ColumnMasterKeyName = cmk.Name;
		EncryptionAlgorithm = encryptionAlgorithm;
		EncryptedValue = encryptedValue;
		ColumnEncryptionKeyName = parent.Name;
		ColumnMasterKeyID = cmk.ID;
	}

	public void MarkForDrop(bool dropOnAlter)
	{
		MarkForDropImpl(dropOnAlter);
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
		string fullQualifiedName = Parent.FullQualifiedName;
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			SqlSmoObject.ThrowIfBelowVersion130(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ColumnEncryptionKeyDownlevel(fullQualifiedName, SqlSmoObject.GetSqlServerName(sp)));
		}
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_CEK_VALUE, new object[3]
			{
				string.Empty,
				Parent.ID,
				ColumnMasterKeyID
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER COLUMN ENCRYPTION KEY {0}", new object[1] { fullQualifiedName });
		stringBuilder.Append(sp.NewLine);
		stringBuilder.AppendLine("DROP VALUE");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}{1}COLUMN_MASTER_KEY = {2}{3})", sp.NewLine, Globals.tab, SqlSmoObject.MakeSqlBraket(ColumnMasterKeyName), sp.NewLine);
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
		string fullQualifiedName = Parent.FullQualifiedName;
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			SqlSmoObject.ThrowIfBelowVersion130(sp.TargetServerVersionInternal, ExceptionTemplatesImpl.ColumnEncryptionKeyDownlevel(fullQualifiedName, SqlSmoObject.GetSqlServerName(sp)));
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader("ColumnEncryptionKey", fullQualifiedName, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_CEK_VALUE, new object[3]
			{
				string.Empty,
				Parent.ID,
				ColumnMasterKeyID
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER COLUMN ENCRYPTION KEY {0}", new object[1] { fullQualifiedName });
		stringBuilder.Append(sp.NewLine);
		stringBuilder.AppendLine("ADD VALUE");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "({0}{1}COLUMN_MASTER_KEY = {2},{3}{4}ALGORITHM = '{5}',{6}{7}ENCRYPTED_VALUE = {8}{9})", sp.NewLine, Globals.tab, SqlSmoObject.MakeSqlBraket(ColumnMasterKeyName), sp.NewLine, Globals.tab, EncryptionAlgorithm.Replace("'", "''"), sp.NewLine, Globals.tab, EncryptedValueAsSqlBinaryString, sp.NewLine);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[5] { "ColumnMasterKeyID", "ColumnEncryptionKeyName", "ColumnMasterKeyDescriptionName", "EncryptionAlgorithm", "EncryptedValue" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

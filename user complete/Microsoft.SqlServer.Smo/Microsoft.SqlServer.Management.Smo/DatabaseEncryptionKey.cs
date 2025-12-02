using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
public sealed class DatabaseEncryptionKey : SqlSmoObject, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 10, 10, 10, 10, 10, 10, 10 };

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
				"EncryptionAlgorithm" => 1, 
				"EncryptionState" => 2, 
				"EncryptionType" => 3, 
				"EncryptorName" => 4, 
				"ModifyDate" => 5, 
				"OpenedDate" => 6, 
				"RegenerateDate" => 7, 
				"SetDate" => 8, 
				"Thumbprint" => 9, 
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
			staticMetadata = new StaticMetadata[10]
			{
				new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: false, typeof(DatabaseEncryptionAlgorithm)),
				new StaticMetadata("EncryptionState", expensive: false, readOnly: true, typeof(DatabaseEncryptionState)),
				new StaticMetadata("EncryptionType", expensive: false, readOnly: false, typeof(DatabaseEncryptionType)),
				new StaticMetadata("EncryptorName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ModifyDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("OpenedDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("RegenerateDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("SetDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("Thumbprint", expensive: false, readOnly: true, typeof(byte[]))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DatabaseEncryptionAlgorithm EncryptionAlgorithm
	{
		get
		{
			return (DatabaseEncryptionAlgorithm)base.Properties.GetValueWithNullReplacement("EncryptionAlgorithm");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptionAlgorithm", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DatabaseEncryptionState EncryptionState => (DatabaseEncryptionState)base.Properties.GetValueWithNullReplacement("EncryptionState");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DatabaseEncryptionType EncryptionType
	{
		get
		{
			return (DatabaseEncryptionType)base.Properties.GetValueWithNullReplacement("EncryptionType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptionType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string EncryptorName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EncryptorName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptorName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime ModifyDate => (DateTime)base.Properties.GetValueWithNullReplacement("ModifyDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime OpenedDate => (DateTime)base.Properties.GetValueWithNullReplacement("OpenedDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime RegenerateDate => (DateTime)base.Properties.GetValueWithNullReplacement("RegenerateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime SetDate => (DateTime)base.Properties.GetValueWithNullReplacement("SetDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public byte[] Thumbprint => (byte[])base.Properties.GetValueWithNullReplacement("Thumbprint");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Database;
		}
		set
		{
			SetParentImpl(value);
			SetState(SqlSmoState.Creating);
		}
	}

	public static string UrnSuffix => "DatabaseEncryptionKey";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal DatabaseEncryptionKey(Database parentdb, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentdb;
		SetServerObject(parentdb.Parent);
		m_comparer = parentdb.StringComparer;
	}

	public DatabaseEncryptionKey()
	{
	}

	internal override void ValidateParent(SqlSmoObject newParent)
	{
		singletonParent = (Database)newParent;
		m_comparer = newParent.StringComparer;
		SetServerObject(newParent.GetServerObject());
		this.ThrowIfNotSupported(GetType());
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected internal override string GetDBName()
	{
		return ((Database)singletonParent).Name;
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		if (!SmoUtility.IsSupportedObject(GetType(), base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition))
		{
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.UnsupportedFeature(ExceptionTemplatesImpl.DatabaseEncryptionKey));
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, string.Empty, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE_ENCRYPTION_KEY, new object[2] { "NOT", Parent.ID });
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(Scripts.BEGIN);
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.DatabaseContext)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append("CREATE DATABASE ENCRYPTION KEY");
			stringBuilder.Append(sp.NewLine);
			DatabaseEncryptionAlgorithm encryptAlgo = (DatabaseEncryptionAlgorithm)GetPropValue("EncryptionAlgorithm");
			string encryptionAlgorithm = GetEncryptionAlgorithm(encryptAlgo);
			stringBuilder.AppendFormat("WITH ALGORITHM = {0}", encryptionAlgorithm);
			stringBuilder.Append(sp.NewLine);
			DatabaseEncryptionType encryptionType = (DatabaseEncryptionType)GetPropValue("EncryptionType");
			string encryptionType2 = GetEncryptionType(encryptionType);
			stringBuilder.AppendFormat("ENCRYPTION BY {0}", encryptionType2);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(EncryptorName) });
			stringBuilder.Append(sp.NewLine);
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.Append(Scripts.END);
				stringBuilder.Append(sp.NewLine);
			}
			createQuery.Add(stringBuilder.ToString());
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = false;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			if (sp.IncludeScripts.DatabaseContext)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) });
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append("ALTER DATABASE ENCRYPTION KEY");
			stringBuilder.Append(sp.NewLine);
			Property property = base.Properties.Get("EncryptionAlgorithm");
			if (property.Value != null && property.Dirty)
			{
				string encryptionAlgorithm = GetEncryptionAlgorithm((DatabaseEncryptionAlgorithm)property.Value);
				stringBuilder.AppendFormat("REGENERATE WITH ALGORITHM = {0}", encryptionAlgorithm);
				stringBuilder.Append(sp.NewLine);
				flag = true;
			}
			Property property2 = base.Properties.Get("EncryptorName");
			Property property3 = base.Properties.Get("EncryptionType");
			if ((!string.IsNullOrEmpty((string)property2.Value) && property2.Dirty) || (property3.Value != null && property3.Dirty))
			{
				string encryptionType = GetEncryptionType((DatabaseEncryptionType)property3.Value);
				stringBuilder.AppendFormat("ENCRYPTION BY {0}", encryptionType);
				string s = property2.Value.ToString();
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(s) });
				stringBuilder.Append(sp.NewLine);
				flag = true;
			}
			if (flag)
			{
				alterQuery.Add(stringBuilder.ToString());
			}
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
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
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, string.Empty, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE_ENCRYPTION_KEY, new object[2]
				{
					string.Empty,
					Parent.ID
				});
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(Scripts.BEGIN);
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append("DROP DATABASE ENCRYPTION KEY");
			stringBuilder.Append(sp.NewLine);
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.Append(Scripts.END);
				stringBuilder.Append(sp.NewLine);
			}
			dropQuery.Add(stringBuilder.ToString());
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Regenerate(DatabaseEncryptionAlgorithm encryptAlgo)
	{
		StringCollection stringCollection = new StringCollection();
		string encryptionAlgorithm = GetEncryptionAlgorithm(encryptAlgo);
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		stringCollection.Add($"ALTER DATABASE ENCRYPTION KEY REGENERATE WITH ALGORITHM = {encryptionAlgorithm}");
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	public void Reencrypt(string encryptorName, DatabaseEncryptionType encryptionType)
	{
		StringCollection stringCollection = new StringCollection();
		string encryptionType2 = GetEncryptionType(encryptionType);
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER DATABASE ENCRYPTION KEY ENCRYPTION BY {0} {1}", new object[2]
		{
			encryptionType2,
			SqlSmoObject.MakeSqlBraket(encryptorName)
		}));
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	private string GetEncryptionAlgorithm(DatabaseEncryptionAlgorithm encryptAlgo)
	{
		string empty = string.Empty;
		return encryptAlgo switch
		{
			DatabaseEncryptionAlgorithm.Aes128 => "AES_128", 
			DatabaseEncryptionAlgorithm.Aes192 => "AES_192", 
			DatabaseEncryptionAlgorithm.Aes256 => "AES_256", 
			DatabaseEncryptionAlgorithm.TripleDes => "TRIPLE_DES_3KEY", 
			_ => throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("EncryptionAlgorithm")), 
		};
	}

	private string GetEncryptionType(DatabaseEncryptionType encryptionType)
	{
		string empty = string.Empty;
		return encryptionType switch
		{
			DatabaseEncryptionType.ServerCertificate => "SERVER CERTIFICATE ", 
			DatabaseEncryptionType.ServerAsymmetricKey => "SERVER ASYMMETRIC KEY ", 
			_ => throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("EncryptionType")), 
		};
	}
}

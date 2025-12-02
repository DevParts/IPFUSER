using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class MasterKey : SqlSmoObject, ISfcSupportsDesignMode, IDroppable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 4, 4, 4, 4, 4, 4, 4, 4 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 4 };

		private static int sqlDwPropertyCount = 3;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("IsEncryptedByServer", expensive: false, readOnly: true, typeof(bool))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("IsEncryptedByServer", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsOpen", expensive: false, readOnly: true, typeof(bool))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("IsEncryptedByServer", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsOpen", expensive: false, readOnly: true, typeof(bool))
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
						"IsEncryptedByServer" => 2, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"IsEncryptedByServer" => 2, 
					"IsOpen" => 3, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"IsEncryptedByServer" => 2, 
				"IsOpen" => 3, 
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

	private SqlSecureString encryptionPwd;

	private SqlSecureString decryptionPwd;

	private string path;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsEncryptedByServer => (bool)base.Properties.GetValueWithNullReplacement("IsEncryptedByServer");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsOpen => (bool)base.Properties.GetValueWithNullReplacement("IsOpen");

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

	public static string UrnSuffix => "MasterKey";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal MasterKey(Database parentdb, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentdb;
		SetServerObject(parentdb.Parent);
		m_comparer = parentdb.StringComparer;
	}

	public MasterKey()
	{
	}

	public MasterKey(Database parent)
		: base(new ObjectKeyBase(), SqlSmoState.Creating)
	{
		singletonParent = parent;
		SetServerObject(parent.GetServerObject());
		m_comparer = parent.StringComparer;
	}

	internal override void ValidateParent(SqlSmoObject newParent)
	{
		singletonParent = (Database)newParent;
		m_comparer = newParent.StringComparer;
		SetServerObject(newParent.GetServerObject());
		this.ThrowIfNotSupported(typeof(MasterKey));
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

	public void Create(string encryptionPassword)
	{
		DoesMkExist();
		try
		{
			encryptionPwd = encryptionPassword;
			CreateImpl();
		}
		finally
		{
			encryptionPwd = null;
		}
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		if (null == encryptionPwd)
		{
			throw new ArgumentNullException("encryptionPassword");
		}
		if (path == null)
		{
			queries.Add(string.Format(SmoApplication.DefaultCulture, "CREATE MASTER KEY ENCRYPTION BY PASSWORD = N'{0}'", new object[1] { SqlSmoObject.SqlString(encryptionPwd.ToString()) }));
			return;
		}
		if (null == decryptionPwd)
		{
			throw new ArgumentNullException("decryptionPassword");
		}
		queries.Add(string.Format(SmoApplication.DefaultCulture, "RESTORE MASTER KEY FROM FILE = N'{0}' DECRYPTION BY PASSWORD = N'{1}' ENCRYPTION BY PASSWORD = N'{2}'", new object[3]
		{
			SqlSmoObject.SqlString(path),
			SqlSmoObject.SqlString(decryptionPwd.ToString()),
			SqlSmoObject.SqlString(encryptionPwd.ToString())
		}));
	}

	public void Create(string path, string decryptionPassword, string encryptionPassword)
	{
		DoesMkExist();
		try
		{
			encryptionPwd = encryptionPassword;
			decryptionPwd = decryptionPassword;
			if (path == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("path"));
			}
			this.path = path;
			CreateImpl();
		}
		finally
		{
			encryptionPwd = null;
			decryptionPwd = null;
			this.path = null;
		}
	}

	private void DoesMkExist()
	{
		if (base.IsDesignMode && Parent.DoesMasterKeyAlreadyExist())
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.ObjectAlreadyExists("Database's", "MasterKey"));
		}
	}

	protected override void PostCreate()
	{
		if (base.IsDesignMode)
		{
			Parent.SetRefMasterKey(this);
		}
	}

	public void Drop()
	{
		if (base.IsDesignMode)
		{
			Parent.SetNullRefMasterKey();
		}
		DropImpl();
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		queries.Add("DROP MASTER KEY");
	}

	public void Import(string path, string decryptionPassword, string encryptionPassword)
	{
		Import(path, decryptionPassword, encryptionPassword, forceRegeneration: false);
	}

	public void Import(string path, string decryptionPassword, string encryptionPassword, bool forceRegeneration)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (decryptionPassword == null)
			{
				throw new ArgumentNullException("decryptionPassword");
			}
			if (encryptionPassword == null)
			{
				throw new ArgumentNullException("encryptionPassword");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "RESTORE MASTER KEY FROM FILE = N'{0}' DECRYPTION BY PASSWORD = N'{1}' ENCRYPTION BY PASSWORD = N'{2}' {3}", SqlSmoObject.SqlString(path), SqlSmoObject.SqlString(decryptionPassword), SqlSmoObject.SqlString(encryptionPassword), forceRegeneration ? "FORCE" : ""));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ImportMasterKey, this, ex);
		}
	}

	public void Export(string path, string password)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "BACKUP MASTER KEY TO FILE = N'{0}' ENCRYPTION BY PASSWORD = N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(path),
				SqlSmoObject.SqlString(password)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExportMasterKey, this, ex);
		}
	}

	public void AddPasswordEncryption(string password)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER MASTER KEY ADD ENCRYPTION BY PASSWORD = N'{0}'", new object[1] { SqlSmoObject.SqlString(password) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddEncryptionMasterKey, this, ex);
		}
	}

	public void AddServiceKeyEncryption()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add("ALTER MASTER KEY ADD ENCRYPTION BY SERVICE MASTER KEY");
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsEncryptedByServer").SetValue(true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddEncryptionMasterKey, this, ex);
		}
	}

	public void Close()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add("CLOSE MASTER KEY");
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(false);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Close, this, ex);
		}
	}

	public void Open(string password)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "OPEN MASTER KEY DECRYPTION BY PASSWORD = N'{0}'", new object[1] { SqlSmoObject.SqlString(password) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Open, this, ex);
		}
	}

	public void DropPasswordEncryption(string password)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER MASTER KEY DROP ENCRYPTION BY PASSWORD = N'{0}'", new object[1] { SqlSmoObject.SqlString(password) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropEncryptionMasterKey, this, ex);
		}
	}

	public void DropServiceKeyEncryption()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add("ALTER MASTER KEY DROP ENCRYPTION BY SERVICE MASTER KEY");
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsEncryptedByServer").SetValue(false);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropEncryptionMasterKey, this, ex);
		}
	}

	public void Regenerate(string password)
	{
		Regenerate(password, forceRegeneration: false);
	}

	public void Regenerate(string password, bool forceRegeneration)
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER MASTER KEY {0}REGENERATE WITH ENCRYPTION BY PASSWORD = N'{1}'", new object[2]
			{
				forceRegeneration ? "FORCE " : "",
				SqlSmoObject.SqlString(password)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RegenerateMasterKey, this, ex);
		}
	}

	public DataTable EnumKeyEncryptions()
	{
		try
		{
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/Encryption")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumKeyEncryptions, this, ex);
		}
	}
}

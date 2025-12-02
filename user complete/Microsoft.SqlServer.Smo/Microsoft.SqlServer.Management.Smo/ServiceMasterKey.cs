using System;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[SfcElementType("MasterKey")]
public sealed class ServiceMasterKey : SqlSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount;

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
			return -1;
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
			int[] array = new int[10];
			versionCount = array;
			int[] array2 = new int[3];
			cloudVersionCount = array2;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[0];
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
	}

	public static string UrnSuffix => "MasterKey";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ServiceMasterKey(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(singletonParent as Server);
		m_comparer = parentsrv.Databases["master"].StringComparer;
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	protected internal override string GetDBName()
	{
		return "master";
	}

	public void ChangeAccount(string newAccount, string newPassword)
	{
		try
		{
			if (newAccount == null)
			{
				throw new ArgumentNullException("newAccount");
			}
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER SERVICE MASTER KEY WITH NEW_ACCOUNT=N'{0}', NEW_PASSWORD=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(newAccount),
				SqlSmoObject.SqlString(newPassword)
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangeAcctMasterKey, this, ex);
		}
	}

	public void Import(string path, string password)
	{
		try
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "RESTORE SERVICE MASTER KEY FROM FILE = N'{0}' DECRYPTION BY PASSWORD = N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(path),
				SqlSmoObject.SqlString(password)
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ImportMasterKey, this, ex);
		}
	}

	public void Recover(string oldAccount, string oldPassword)
	{
		try
		{
			if (oldAccount == null)
			{
				throw new ArgumentNullException("oldAccount");
			}
			if (oldPassword == null)
			{
				throw new ArgumentNullException("oldPassword");
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER SERVICE MASTER KEY WITH OLD_ACCOUNT=N'{0}', OLD_PASSWORD=N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(oldAccount),
				SqlSmoObject.SqlString(oldPassword)
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RecoverMasterKey, this, ex);
		}
	}

	public void Regenerate()
	{
		Regenerate(forceRegeneration: false);
	}

	public void Regenerate(bool forceRegeneration)
	{
		try
		{
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "ALTER SERVICE MASTER KEY {0}REGENERATE", new object[1] { forceRegeneration ? "FORCE " : "" }));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RegenerateMasterKey, this, ex);
		}
	}

	public void Export(string path, string password)
	{
		try
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			ExecutionManager.ExecuteNonQuery(string.Format(SmoApplication.DefaultCulture, "BACKUP SERVICE MASTER KEY TO FILE = N'{0}' ENCRYPTION BY PASSWORD = N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(path),
				SqlSmoObject.SqlString(password)
			}));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExportMasterKey, this, ex);
		}
	}
}

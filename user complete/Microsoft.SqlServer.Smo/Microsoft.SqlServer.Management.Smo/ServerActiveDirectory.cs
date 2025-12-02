using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[Obsolete]
[SfcElement(SfcElementFlags.Standalone)]
public sealed class ServerActiveDirectory : SqlSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 2, 2, 2, 2, 2, 2, 2, 2, 2 };

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
				"IsEnabled" => 0, 
				"IsRegistered" => 1, 
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
			staticMetadata = new StaticMetadata[2]
			{
				new StaticMetadata("IsEnabled", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("IsRegistered", expensive: false, readOnly: true, typeof(bool))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsEnabled => (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsRegistered => (bool)base.Properties.GetValueWithNullReplacement("IsRegistered");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Server;
		}
	}

	public static string UrnSuffix => "ServerActiveDirectory";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal ServerActiveDirectory(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	public void Register()
	{
		Register(registerDatabases: false);
	}

	public void Register(bool registerDatabases)
	{
		try
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			if (registerDatabases)
			{
				stringCollection.Add("master.dbo.sp_ActiveDirectory_SCP @Action = N'CREATE_WITH_DB'");
			}
			else
			{
				stringCollection.Add("master.dbo.sp_ActiveDirectory_SCP @Action = N'create'");
			}
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				Property property = base.Properties.Get("IsRegistered");
				property.SetValue(true);
				property.SetRetrieved(retrieved: true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ServerActiveDirectoryRegister, this, ex);
		}
	}

	public void UpdateRegistration()
	{
		try
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("master.dbo.sp_ActiveDirectory_SCP @Action = N'update'");
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ServerActiveDirectoryUpdateRegistration, this, ex);
		}
	}

	public void Unregister()
	{
		try
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("master.dbo.sp_ActiveDirectory_SCP @Action = N'delete'");
			ExecutionManager.ExecuteNonQuery(stringCollection);
			if (!ExecutionManager.Recording)
			{
				Property property = base.Properties.Get("IsRegistered");
				property.SetValue(false);
				property.SetRetrieved(retrieved: true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ServerActiveDirectoryUnregister, this, ex);
		}
	}
}

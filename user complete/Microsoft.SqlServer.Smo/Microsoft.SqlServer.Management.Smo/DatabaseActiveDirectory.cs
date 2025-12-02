using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone)]
[Obsolete]
public sealed class DatabaseActiveDirectory : SqlSmoObject
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

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
			string text;
			if ((text = propertyName) != null && text == "IsRegistered")
			{
				return 0;
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
			int[] array = new int[3];
			cloudVersionCount = array;
			sqlDwPropertyCount = 0;
			sqlDwStaticMetadata = new StaticMetadata[0];
			cloudStaticMetadata = new StaticMetadata[0];
			staticMetadata = new StaticMetadata[1]
			{
				new StaticMetadata("IsRegistered", expensive: false, readOnly: true, typeof(bool))
			};
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsRegistered => (bool)base.Properties.GetValueWithNullReplacement("IsRegistered");

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Database Parent
	{
		get
		{
			CheckObjectState();
			return singletonParent as Database;
		}
	}

	public static string UrnSuffix => "DatabaseActiveDirectory";

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Contact
	{
		get
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["Contact"];
			if (extendedProperty == null)
			{
				return string.Empty;
			}
			return extendedProperty.Value as string;
		}
		set
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["Contact"];
			if (extendedProperty == null)
			{
				extendedProperty = new ExtendedProperty(Parent, "Contact", value);
				extendedProperty.Create();
			}
			else
			{
				extendedProperty.Value = value;
				extendedProperty.Alter();
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Description
	{
		get
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["Description"];
			if (extendedProperty == null)
			{
				return string.Empty;
			}
			return extendedProperty.Value as string;
		}
		set
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["Description"];
			if (extendedProperty == null)
			{
				extendedProperty = new ExtendedProperty(Parent, "Description", value);
				extendedProperty.Create();
			}
			else
			{
				extendedProperty.Value = value;
				extendedProperty.Alter();
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Alias
	{
		get
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["Alias"];
			if (extendedProperty == null)
			{
				return string.Empty;
			}
			return extendedProperty.Value as string;
		}
		set
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["Alias"];
			if (extendedProperty == null)
			{
				extendedProperty = new ExtendedProperty(Parent, "Alias", value);
				extendedProperty.Create();
			}
			else
			{
				extendedProperty.Value = value;
				extendedProperty.Alter();
			}
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string InformationUrl
	{
		get
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["InformationUrl"];
			if (extendedProperty == null)
			{
				return string.Empty;
			}
			return extendedProperty.Value as string;
		}
		set
		{
			ExtendedProperty extendedProperty = Parent.ExtendedProperties["InformationUrl"];
			if (extendedProperty == null)
			{
				extendedProperty = new ExtendedProperty(Parent, "InformationUrl", value);
				extendedProperty.Create();
			}
			else
			{
				extendedProperty.Value = value;
				extendedProperty.Alter();
			}
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal DatabaseActiveDirectory(Database parentsrv, ObjectKeyBase key, SqlSmoState state)
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
		try
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "sp_ActiveDirectory_Obj @Action = N'create', @ObjType = N'database', @ObjName = N'{0}'", new object[1] { SqlSmoObject.SqlString(Parent.Name) }));
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
			throw new FailedOperationException(ExceptionTemplatesImpl.DatabaseActiveDirectoryRegister, this, ex);
		}
	}

	public void UpdateRegistration()
	{
		try
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "sp_ActiveDirectory_Obj @Action = N'update', @ObjType = N'database', @ObjName = N'{0}'", new object[1] { SqlSmoObject.SqlString(Parent.Name) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DatabaseActiveDirectoryUpdateRegistration, this, ex);
		}
	}

	public void Unregister()
	{
		try
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "sp_ActiveDirectory_Obj @Action = N'delete', @ObjType = N'database', @ObjName = N'{0}'", new object[1] { SqlSmoObject.SqlString(Parent.Name) }));
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
			throw new FailedOperationException(ExceptionTemplatesImpl.DatabaseActiveDirectoryUnregister, this, ex);
		}
	}
}

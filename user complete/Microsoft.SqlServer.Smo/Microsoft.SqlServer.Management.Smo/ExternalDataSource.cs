using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class ExternalDataSource : NamedSmoObject, ISfcSupportsDesignMode, IObjectPermission, IAlterable, ICreatable, IDroppable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 0, 5, 5, 5 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 6 };

		private static int sqlDwPropertyCount = 4;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[4]
		{
			new StaticMetadata("Credential", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataSourceType", expensive: false, readOnly: false, typeof(ExternalDataSourceType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Location", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[6]
		{
			new StaticMetadata("Credential", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DatabaseName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataSourceType", expensive: false, readOnly: false, typeof(ExternalDataSourceType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Location", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ShardMapName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[5]
		{
			new StaticMetadata("Credential", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DataSourceType", expensive: false, readOnly: false, typeof(ExternalDataSourceType)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Location", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ResourceManagerLocation", expensive: false, readOnly: false, typeof(string))
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
						"Credential" => 0, 
						"DataSourceType" => 1, 
						"ID" => 2, 
						"Location" => 3, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"Credential" => 0, 
					"DatabaseName" => 1, 
					"DataSourceType" => 2, 
					"ID" => 3, 
					"Location" => 4, 
					"ShardMapName" => 5, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"Credential" => 0, 
				"DataSourceType" => 1, 
				"ID" => 2, 
				"Location" => 3, 
				"ResourceManagerLocation" => 4, 
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

	private const string ExternalDataSourceLocationWasb = "wasb";

	private const string ExternalDataSourceLocationAsv = "asv";

	[SfcObject(SfcObjectRelationship.ParentObject, SfcObjectFlags.Design)]
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

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcReference(typeof(Credential), "Server[@Name = '{0}']/Database[@Name = '{1}']/Credential[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Credential" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[CLSCompliant(false)]
	public string Credential
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Credential");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Credential", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ExternalDataSourceType DataSourceType
	{
		get
		{
			return (ExternalDataSourceType)base.Properties.GetValueWithNullReplacement("DataSourceType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DataSourceType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string ResourceManagerLocation
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ResourceManagerLocation");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ResourceManagerLocation", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public string DatabaseName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DatabaseName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DatabaseName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.SqlAzureDatabase)]
	public string ShardMapName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ShardMapName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ShardMapName", value);
		}
	}

	public static string UrnSuffix => "ExternalDataSource";

	public ExternalDataSource()
	{
	}

	public ExternalDataSource(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string[] granteeNames, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, granteeNames, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string[] granteeNames, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, granteeNames, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string[] granteeNames, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, granteeNames, null, revokeGrant, cascade, asRole);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Deny(ObjectPermissionSet permission, string granteeName, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Deny, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, null);
	}

	public void Grant(ObjectPermissionSet permission, string granteeName, bool grantGrant, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Grant, this, permission, new string[1] { granteeName }, null, grantGrant, cascade: false, asRole);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, grantGrant: false, cascade: false, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, null);
	}

	public void Revoke(ObjectPermissionSet permission, string granteeName, bool revokeGrant, bool cascade, string asRole)
	{
		PermissionWorker.Execute(PermissionState.Revoke, this, permission, new string[1] { granteeName }, null, revokeGrant, cascade, asRole);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions()
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, null);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, null, permissions);
	}

	public ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions)
	{
		return (ObjectPermissionInfo[])PermissionWorker.EnumPermissions(PermissionWorker.PermissionEnumKind.Object, this, granteeName, permissions);
	}

	internal ExternalDataSource(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ExternalDataSource(Database parent, string name, ExternalDataSourceType dataSourceType, string location)
	{
		Parent = parent;
		base.Name = name;
		DataSourceType = dataSourceType;
		Location = location;
	}

	public void Alter()
	{
		AlterImpl();
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Drop()
	{
		DropImpl();
	}

	public void DropIfExists()
	{
		DropImpl(isDropIfExists: true);
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		string text = FormatFullNameForScripting(sp);
		this.ThrowIfNotSupported(typeof(ExternalDataSource), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_EXTERNAL_DATA_SOURCE, new object[2]
			{
				string.Empty,
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			}));
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append("DROP EXTERNAL DATA SOURCE " + text);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(typeof(ExternalDataSource), sp);
		ExternalDataSourceType externalDataSourceType = ExternalDataSourceType.Hadoop;
		string text = string.Empty;
		ValidatePropertySet("DataSourceType", sp);
		ValidatePropertySet("Location", sp);
		if (IsSupportedProperty("DataSourceType", sp))
		{
			externalDataSourceType = (ExternalDataSourceType)GetPropValue("DataSourceType");
			if (!Enum.IsDefined(typeof(ExternalDataSourceType), externalDataSourceType))
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration(externalDataSourceType.ToString()));
			}
		}
		if (IsSupportedProperty("Location", sp))
		{
			text = (string)GetPropValue("Location");
		}
		switch (externalDataSourceType)
		{
		case ExternalDataSourceType.Rdbms:
			SqlSmoObject.ThrowIfNotCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
			ValidatePropertySet("Credential", sp);
			ValidatePropertySet("DatabaseName", sp);
			break;
		case ExternalDataSourceType.ShardMapManager:
			SqlSmoObject.ThrowIfNotCloud(sp.TargetDatabaseEngineType, ExceptionTemplatesImpl.UnsupportedEngineTypeException);
			ValidatePropertySet("Credential", sp);
			ValidatePropertySet("DatabaseName", sp);
			ValidatePropertySet("ShardMapName", sp);
			break;
		}
		switch (externalDataSourceType)
		{
		case ExternalDataSourceType.Hadoop:
			ValidatePropertyUnset("DatabaseName", externalDataSourceType, sp);
			ValidatePropertyUnset("ShardMapName", externalDataSourceType, sp);
			break;
		case ExternalDataSourceType.Rdbms:
			ValidatePropertyUnset("ResourceManagerLocation", externalDataSourceType, sp);
			ValidatePropertyUnset("ShardMapName", externalDataSourceType, sp);
			break;
		case ExternalDataSourceType.ShardMapManager:
			ValidatePropertyUnset("ResourceManagerLocation", externalDataSourceType, sp);
			break;
		}
		string text2 = FormatFullNameForScripting(sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text2, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_EXTERNAL_DATA_SOURCE, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.BEGIN);
			stringBuilder.Append(sp.NewLine);
		}
		TypeConverter typeConverter = SmoManagementUtil.GetTypeConverter(typeof(ExternalDataSourceType));
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE EXTERNAL DATA SOURCE {0} WITH ", new object[1] { text2 });
		stringBuilder.Append(Globals.LParen);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "TYPE = {0}", new object[1] { typeConverter.ConvertToInvariantString(externalDataSourceType) });
		stringBuilder.Append(", ");
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "LOCATION = {0}", new object[1] { Util.MakeSqlString(text) });
		if (IsSupportedProperty("ResourceManagerLocation", sp) && !GetPropertyOptional("ResourceManagerLocation").IsNull)
		{
			string text3 = Convert.ToString(GetPropValueOptional("ResourceManagerLocation"), SmoApplication.DefaultCulture);
			if (!string.IsNullOrEmpty(text3))
			{
				ValidateResourceManagerLocation(text3, text);
				stringBuilder.Append(", ");
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESOURCE_MANAGER_LOCATION = {0}", new object[1] { Util.MakeSqlString(text3) });
			}
		}
		if (IsSupportedProperty("Credential", sp) && !GetPropertyOptional("Credential").IsNull)
		{
			string text4 = Convert.ToString(GetPropValueOptional("Credential"), SmoApplication.DefaultCulture);
			if (!string.IsNullOrEmpty(text4))
			{
				stringBuilder.Append(", ");
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREDENTIAL = {0}", new object[1] { SqlSmoObject.MakeSqlBraket(text4) });
			}
		}
		if (IsSupportedProperty("DatabaseName", sp))
		{
			Property propertyOptional = GetPropertyOptional("DatabaseName");
			if (!propertyOptional.IsNull)
			{
				string value = Convert.ToString(propertyOptional.Value, SmoApplication.DefaultCulture);
				if (!string.IsNullOrEmpty(value))
				{
					stringBuilder.Append(", ");
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DATABASE_NAME = {0}", new object[1] { Util.MakeSqlString(value) });
				}
			}
		}
		if (IsSupportedProperty("ShardMapName", sp))
		{
			Property propertyOptional2 = GetPropertyOptional("ShardMapName");
			if (!propertyOptional2.IsNull)
			{
				string value2 = Convert.ToString(propertyOptional2.Value, SmoApplication.DefaultCulture);
				if (!string.IsNullOrEmpty(value2))
				{
					stringBuilder.Append(", ");
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "SHARD_MAP_NAME = {0}", new object[1] { Util.MakeSqlString(value2) });
				}
			}
		}
		stringBuilder.Append(Globals.RParen);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(Scripts.END);
			stringBuilder.Append(sp.NewLine);
		}
		createQuery.Add(stringBuilder.ToString());
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		bool flag = false;
		this.ThrowIfNotSupported(typeof(ExternalDataSource), sp);
		if (base.State == SqlSmoState.Creating)
		{
			return;
		}
		if (IsSupportedProperty("DataSourceType", sp))
		{
			Property propertyOptional = GetPropertyOptional("DataSourceType");
			if (!propertyOptional.IsNull)
			{
				if (propertyOptional.Dirty)
				{
					throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.UnsupportedPropertyForAlter, new object[1] { "DataSourceType" }));
				}
				ExternalDataSourceType externalDataSourceType = (ExternalDataSourceType)propertyOptional.Value;
				if (externalDataSourceType == ExternalDataSourceType.ShardMapManager || externalDataSourceType == ExternalDataSourceType.Rdbms)
				{
					throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.AlterNotSupportedForRelationalTypes, new object[1] { externalDataSourceType }));
				}
			}
		}
		if (IsSupportedProperty("DatabaseName", sp))
		{
			Property propertyOptional2 = GetPropertyOptional("DatabaseName");
			if (!propertyOptional2.IsNull && propertyOptional2.Dirty)
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.UnsupportedPropertyForAlter, new object[1] { "DatabaseName" }));
			}
		}
		if (IsSupportedProperty("ShardMapName", sp))
		{
			Property propertyOptional3 = GetPropertyOptional("ShardMapName");
			if (!propertyOptional3.IsNull && propertyOptional3.Dirty)
			{
				throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.UnsupportedPropertyForAlter, new object[1] { "ShardMapName" }));
			}
		}
		string text4 = FormatFullNameForScripting(sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text4, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER EXTERNAL DATA SOURCE {0} SET ", new object[1] { text4 });
		if (IsSupportedProperty("Location", sp))
		{
			Property propertyOptional4 = GetPropertyOptional("Location");
			if (!propertyOptional4.IsNull)
			{
				text = Convert.ToString(GetPropValueOptional("Location"), SmoApplication.DefaultCulture);
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "LOCATION = {0}", new object[1] { Util.MakeSqlString(text) });
					flag = true;
				}
			}
		}
		if (IsSupportedProperty("ResourceManagerLocation", sp) && !GetPropertyOptional("ResourceManagerLocation").IsNull)
		{
			text2 = Convert.ToString(GetPropValueOptional("ResourceManagerLocation"), SmoApplication.DefaultCulture);
			if (!string.IsNullOrEmpty(text2))
			{
				ValidateResourceManagerLocation(text2, text);
				if (flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "RESOURCE_MANAGER_LOCATION = {0}", new object[1] { Util.MakeSqlString(text2) });
				flag = true;
			}
		}
		if (IsSupportedProperty("Credential", sp) && !GetPropertyOptional("Credential").IsNull)
		{
			text3 = Convert.ToString(GetPropValueOptional("Credential"), SmoApplication.DefaultCulture);
			if (!string.IsNullOrEmpty(text3))
			{
				if (flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREDENTIAL = {0}", new object[1] { SqlSmoObject.MakeSqlBraket(text3) });
				flag = true;
			}
		}
		if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text3))
		{
			alterQuery.Add(stringBuilder.ToString());
		}
	}

	private void ValidatePropertySet(string propertyName, ScriptingPreferences sp)
	{
		if (IsSupportedProperty(propertyName, sp))
		{
			if (GetPropertyOptional(propertyName).IsNull)
			{
				throw new ArgumentNullException(propertyName);
			}
			string value = GetPropValue(propertyName).ToString();
			if (string.IsNullOrEmpty(value))
			{
				throw new PropertyNotSetException(propertyName);
			}
		}
	}

	private void ValidatePropertyUnset(string propertyName, ExternalDataSourceType dataSourceType, ScriptingPreferences sp)
	{
		if (IsSupportedProperty(propertyName, sp) && !GetPropertyOptional(propertyName).IsNull && !string.IsNullOrEmpty(GetPropValue(propertyName).ToString()))
		{
			throw new WrongPropertyValueException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.UnsupportedParamForDataSourceType, new object[2] { propertyName, dataSourceType }));
		}
	}

	private void ValidateResourceManagerLocation(string externalDataSourceResourceManagerLocaiton, string externalDataSourceLocation)
	{
		if ((!string.IsNullOrEmpty(externalDataSourceResourceManagerLocaiton) && externalDataSourceLocation.StartsWith("wasb", StringComparison.OrdinalIgnoreCase)) || externalDataSourceLocation.StartsWith("asv", StringComparison.OrdinalIgnoreCase))
		{
			throw new SmoException(string.Format(SmoApplication.DefaultCulture, ExceptionTemplatesImpl.UnsupportedResourceManagerLocationProperty, new object[1] { "ResourceManagerLocation" }));
		}
	}
}

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

[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class SmartAdmin : SqlSmoObject, ISfcSupportsDesignMode, IAlterable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 0, 0, 0, 8, 8, 8, 8 };

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
				"BackupEnabled" => 0, 
				"BackupRetentionPeriodInDays" => 1, 
				"CredentialName" => 2, 
				"EncryptionAlgorithm" => 3, 
				"EncryptorName" => 4, 
				"EncryptorType" => 5, 
				"MasterSwitch" => 6, 
				"StorageUrl" => 7, 
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
			staticMetadata = new StaticMetadata[8]
			{
				new StaticMetadata("BackupEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("BackupRetentionPeriodInDays", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("CredentialName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EncryptorName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("EncryptorType", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("MasterSwitch", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("StorageUrl", expensive: false, readOnly: false, typeof(string))
			};
		}
	}

	private string m_databaseName;

	private bool? m_isDroppedDB = false;

	private bool? m_isAvailabilityDB = false;

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool BackupEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("BackupEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BackupEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int BackupRetentionPeriodInDays
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("BackupRetentionPeriodInDays");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("BackupRetentionPeriodInDays", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string CredentialName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("CredentialName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("CredentialName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
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
	public string EncryptorType
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("EncryptorType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("EncryptorType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool MasterSwitch
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("MasterSwitch");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MasterSwitch", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string StorageUrl
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("StorageUrl");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StorageUrl", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Computed)]
	public string DatabaseName
	{
		get
		{
			return m_databaseName;
		}
		set
		{
			if (NetCoreHelpers.StringCompare(m_databaseName, value, ignoreCase: true, SmoApplication.DefaultCulture) != 0 && (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(m_databaseName)))
			{
				BypassValues();
			}
			m_databaseName = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Computed)]
	public bool? IsDroppedDB
	{
		get
		{
			if (string.IsNullOrEmpty(m_databaseName))
			{
				return null;
			}
			return m_isDroppedDB;
		}
		set
		{
			m_isDroppedDB = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Computed)]
	public bool? IsAvailabilityDB
	{
		get
		{
			if (string.IsNullOrEmpty(m_databaseName))
			{
				return null;
			}
			return m_isAvailabilityDB;
		}
		set
		{
			m_isAvailabilityDB = value;
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
		internal set
		{
			SetParentImpl(value);
		}
	}

	public static string UrnSuffix => "SmartAdmin";

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	public SmartAdmin()
	{
	}

	internal SmartAdmin(Server parentsrv, ObjectKeyBase key, SqlSmoState state)
		: base(key, state)
	{
		singletonParent = parentsrv;
		SetServerObject(parentsrv.GetServerObject());
		m_comparer = parentsrv.StringComparer;
	}

	public void Alter()
	{
		AlterImpl();
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public DataTable EnumHealthStatus()
	{
		return EnumHealthStatus(null, null);
	}

	public DataTable EnumHealthStatus(DateTime? startDate, DateTime? endDate)
	{
		ThrowIfCloud();
		string text = "default";
		string text2 = "default";
		if (startDate.HasValue && startDate.HasValue)
		{
			text = string.Format(SmoApplication.DefaultCulture, "'{0}'", new object[1] { SqlSmoObject.SqlDateString(startDate.Value) });
		}
		if (endDate.HasValue && endDate.HasValue)
		{
			text2 = string.Format(SmoApplication.DefaultCulture, "'{0}'", new object[1] { SqlSmoObject.SqlDateString(endDate.Value) });
		}
		string query = string.Format(SmoApplication.DefaultCulture, "SELECT number_of_storage_connectivity_errors,\r\n                        number_of_sql_errors,\r\n                        number_of_invalid_credential_errors,\r\n                        number_of_other_errors,\r\n                        number_of_corrupted_or_deleted_backups,\r\n                        number_of_backup_loops,\r\n                        number_of_retention_loops\r\n                    FROM  [msdb].[smart_admin].[fn_get_health_status]({0}, {1} )", new object[2] { text, text2 });
		DataSet dataSet = ExecutionManager.ExecuteWithResults(query);
		DataTable result = null;
		if (dataSet != null && dataSet.Tables.Count > 0)
		{
			result = dataSet.Tables[0];
		}
		return result;
	}

	public sealed override void Refresh()
	{
		if (string.IsNullOrEmpty(DatabaseName))
		{
			base.Refresh();
		}
		else
		{
			RefreshDBLevelProperties();
		}
	}

	protected sealed override void GetUrnRecursive(StringBuilder urnbuilder, UrnIdOption idOption)
	{
		Parent.GetUrnRecImpl(urnbuilder, idOption);
		urnbuilder.AppendFormat(SmoApplication.DefaultCulture, "/{0}", new object[1] { UrnSuffix });
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		ScriptProperties(queries, sp);
	}

	private void SetParameters(List<string> parameters, ScriptingPreferences sp)
	{
		bool flag = false;
		Property property = base.Properties.Get("BackupEnabled");
		if (null != property && (property.Dirty || !sp.ScriptForAlter))
		{
			if ((bool)property.Value)
			{
				parameters.Add("@enable_backup = 1");
			}
			else
			{
				parameters.Add("@enable_backup = 0");
			}
		}
		if (!flag)
		{
			if (!string.IsNullOrEmpty(m_databaseName))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@database_name = {0}", new object[1] { m_databaseName }));
			}
			Property property2 = base.Properties.Get("BackupRetentionPeriodInDays");
			if (null != property2 && (property2.Dirty || !sp.ScriptForAlter))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@retention_days = {0}", new object[1] { property2.Value.ToString() }));
			}
			Property property3 = base.Properties.Get("CredentialName");
			if (null != property3 && (property3.Dirty || !sp.ScriptForAlter))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@credential_name = N'{0}'", new object[1] { property3.Value.ToString() }));
			}
			Property property4 = base.Properties.Get("StorageUrl");
			if (null != property4 && (property4.Dirty || !sp.ScriptForAlter))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@storage_url = N'{0}'", new object[1] { property4.Value.ToString() }));
			}
			Property property5 = base.Properties.Get("EncryptionAlgorithm");
			if (null != property5 && (property5.Dirty || !sp.ScriptForAlter))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@encryption_algorithm = N'{0}'", new object[1] { property5.Value.ToString() }));
			}
			Property property6 = base.Properties.Get("EncryptorType");
			if (null != property6 && (property6.Dirty || !sp.ScriptForAlter))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@encryptor_type = N'{0}'", new object[1] { property6.Value.ToString() }));
			}
			Property property7 = base.Properties.Get("EncryptorName");
			if (null != property7 && (property7.Dirty || !sp.ScriptForAlter))
			{
				parameters.Add(string.Format(SmoApplication.DefaultCulture, "@encryptor_name = N'{0}'", new object[1] { property7.Value.ToString() }));
			}
		}
	}

	private void ScriptProperties(StringCollection queries, ScriptingPreferences sp)
	{
		ThrowIfSourceOrDestBelowVersion120(sp.TargetServerVersionInternal);
		Property property = base.Properties.Get("MasterSwitch");
		if (null != property && string.IsNullOrEmpty(DatabaseName) && (property.Dirty || !sp.ScriptForAlter))
		{
			if ((bool)property.Value)
			{
				queries.Add("EXEC [msdb].[smart_admin].[sp_backup_master_switch] @new_state  = 1;");
			}
			else
			{
				queries.Add("EXEC [msdb].[smart_admin].[sp_backup_master_switch] @new_state  = 0;");
			}
		}
		List<string> list = new List<string>();
		SetParameters(list, sp);
		if (list.Count > 0)
		{
			if (string.IsNullOrEmpty(m_databaseName))
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC [msdb].[smart_admin].[sp_set_instance_backup] {0};", new object[1] { string.Join(", ", list.ToArray()) }));
			}
			else
			{
				queries.Add(string.Format(SmoApplication.DefaultCulture, "EXEC [msdb].[smart_admin].[sp_set_db_backup] {0};", new object[1] { string.Join(", ", list.ToArray()) }));
			}
		}
	}

	private void RefreshDBLevelProperties()
	{
		string query = string.Format(SmoApplication.DefaultCulture, "SELECT is_availability_database,\r\n                        is_dropped,\r\n                        is_managed_backup_enabled,\r\n                        credential_name,\r\n                        retention_days,\r\n                        storage_url,\r\n                        encryption_algorithm,\r\n                        encryptor_type,\r\n                        encryptor_name\r\n                    FROM  [msdb].[smart_admin].[fn_backup_db_config]('{0}')", new object[1] { m_databaseName });
		DataSet dataSet = ExecutionManager.ExecuteWithResults(query);
		DataTable dataTable = null;
		if (dataSet == null || dataSet.Tables.Count <= 0)
		{
			return;
		}
		dataTable = dataSet.Tables[0];
		if (dataTable.Rows.Count == 1)
		{
			DataRow dataRow = dataTable.Rows[0];
			IsDroppedDB = (bool)dataRow["is_dropped"];
			IsAvailabilityDB = (bool)dataRow["is_availability_database"];
			base.Properties.SetValueWithConsistencyCheck("BackupEnabled", dataRow["is_managed_backup_enabled"]);
			base.Properties.SetValueWithConsistencyCheck("CredentialName", dataRow["credential_name"]);
			base.Properties.SetValueWithConsistencyCheck("BackupRetentionPeriodInDays", dataRow["retention_days"]);
			base.Properties.SetValueWithConsistencyCheck("StorageUrl", dataRow["storage_url"]);
			base.Properties.SetValueWithConsistencyCheck("EncryptionAlgorithm", dataRow["encryption_algorithm"]);
			if (dataRow["encryptor_type"] == null || DBNull.Value.Equals(dataRow["encryptor_type"]))
			{
				base.Properties.SetValueWithConsistencyCheck("EncryptorType", string.Empty);
			}
			else
			{
				base.Properties.SetValueWithConsistencyCheck("EncryptorType", dataRow["encryptor_type"]);
			}
			if (dataRow["encryptor_name"] == null || DBNull.Value.Equals(dataRow["encryptor_name"]))
			{
				base.Properties.SetValueWithConsistencyCheck("EncryptorName", string.Empty);
			}
			else
			{
				base.Properties.SetValueWithConsistencyCheck("EncryptorName", dataRow["encryptor_name"]);
			}
			base.Properties.SetAllDirty(val: false);
			return;
		}
		if (dataTable.Rows.Count == 0)
		{
			throw new ArgumentException(string.Format(SmoApplication.DefaultCulture, LocalizableResources.SmartAdmin_NoSuchDB, new object[1] { DatabaseName }));
		}
		throw new ArgumentException(string.Format(SmoApplication.DefaultCulture, LocalizableResources.SmartAdmin_WrongRecords, new object[2]
		{
			DatabaseName,
			dataTable.Rows.Count
		}));
	}

	private void BypassValues()
	{
		base.Properties.SetValueWithConsistencyCheck("BackupEnabled", BackupEnabled);
		base.Properties.SetValueWithConsistencyCheck("CredentialName", CredentialName);
		base.Properties.SetValueWithConsistencyCheck("BackupRetentionPeriodInDays", BackupRetentionPeriodInDays);
		base.Properties.SetValueWithConsistencyCheck("StorageUrl", StorageUrl);
		base.Properties.SetValueWithConsistencyCheck("EncryptionAlgorithm", EncryptionAlgorithm);
		base.Properties.SetValueWithConsistencyCheck("EncryptorType", EncryptorType, allowNull: true);
		base.Properties.SetValueWithConsistencyCheck("EncryptorName", EncryptorName, allowNull: true);
	}
}

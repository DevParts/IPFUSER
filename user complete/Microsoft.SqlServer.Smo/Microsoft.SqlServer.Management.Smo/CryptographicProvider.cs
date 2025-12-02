using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
public sealed class CryptographicProvider : NamedSmoObject, ISfcSupportsDesignMode, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 16, 16, 16, 16, 16, 16, 16 };

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
				"AsymmetricKeyExportable" => 0, 
				"AsymmetricKeyImportable" => 1, 
				"AsymmetricKeyPersistable" => 2, 
				"AsymmetricKeySupported" => 3, 
				"AuthenticationType" => 4, 
				"DllPath" => 5, 
				"Enabled" => 6, 
				"ID" => 7, 
				"PolicyHealthState" => 8, 
				"ProviderGuid" => 9, 
				"SqlCryptographicVersionString" => 10, 
				"SymmetricKeyExportable" => 11, 
				"SymmetricKeyImportable" => 12, 
				"SymmetricKeyPersistable" => 13, 
				"SymmetricKeySupported" => 14, 
				"VersionString" => 15, 
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
			staticMetadata = new StaticMetadata[16]
			{
				new StaticMetadata("AsymmetricKeyExportable", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("AsymmetricKeyImportable", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("AsymmetricKeyPersistable", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("AsymmetricKeySupported", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("AuthenticationType", expensive: false, readOnly: true, typeof(ProviderAuthenticationType)),
				new StaticMetadata("DllPath", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Enabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("ProviderGuid", expensive: false, readOnly: true, typeof(Guid)),
				new StaticMetadata("SqlCryptographicVersionString", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("SymmetricKeyExportable", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("SymmetricKeyImportable", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("SymmetricKeyPersistable", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("SymmetricKeySupported", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("VersionString", expensive: false, readOnly: true, typeof(string))
			};
		}
	}

	[SfcObject(SfcObjectRelationship.ParentObject)]
	public Server Parent
	{
		get
		{
			CheckObjectState();
			return base.ParentColl.ParentInstance as Server;
		}
		set
		{
			SetParentImpl(value);
		}
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AsymmetricKeyExportable => (bool)base.Properties.GetValueWithNullReplacement("AsymmetricKeyExportable");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AsymmetricKeyImportable => (bool)base.Properties.GetValueWithNullReplacement("AsymmetricKeyImportable");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AsymmetricKeyPersistable => (bool)base.Properties.GetValueWithNullReplacement("AsymmetricKeyPersistable");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool AsymmetricKeySupported => (bool)base.Properties.GetValueWithNullReplacement("AsymmetricKeySupported");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ProviderAuthenticationType AuthenticationType => (ProviderAuthenticationType)base.Properties.GetValueWithNullReplacement("AuthenticationType");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string DllPath
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DllPath");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DllPath", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "true")]
	public bool Enabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("Enabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Enabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Guid ProviderGuid => (Guid)base.Properties.GetValueWithNullReplacement("ProviderGuid");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SymmetricKeyExportable => (bool)base.Properties.GetValueWithNullReplacement("SymmetricKeyExportable");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SymmetricKeyImportable => (bool)base.Properties.GetValueWithNullReplacement("SymmetricKeyImportable");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SymmetricKeyPersistable => (bool)base.Properties.GetValueWithNullReplacement("SymmetricKeyPersistable");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool SymmetricKeySupported => (bool)base.Properties.GetValueWithNullReplacement("SymmetricKeySupported");

	public static string UrnSuffix => "CryptographicProvider";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Version Version
	{
		get
		{
			string text = (string)GetPropValue("VersionString");
			if (!string.IsNullOrEmpty(text))
			{
				return new Version(text);
			}
			return null;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public Version SqlCryptographicVersion
	{
		get
		{
			string text = (string)GetPropValue("SqlCryptographicVersionString");
			if (!string.IsNullOrEmpty(text))
			{
				return new Version(text);
			}
			return null;
		}
	}

	public CryptographicProvider()
	{
	}

	public CryptographicProvider(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "Enabled")
		{
			return true;
		}
		return base.GetPropertyDefaultValue(propname);
	}

	internal CryptographicProvider(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		Enabled = true;
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
		string text = (string)GetPropValue("DllPath");
		if (string.IsNullOrEmpty(text))
		{
			throw new PropertyNotSetException("DllPath");
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_CRYPTOGRAPHIC_PROVIDER, "NOT", FormatFullNameForScripting(sp, nameIsIndentifier: false));
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE CRYPTOGRAPHIC PROVIDER {0} FROM FILE = {1}", new object[2]
		{
			FullQualifiedName,
			SqlSmoObject.MakeSqlString(text)
		});
		queries.Add(stringBuilder.ToString());
		Property property = base.Properties.Get("Enabled");
		if (property.Dirty)
		{
			queries.Add(ScriptEnableDisable((bool)property.Value));
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
		Property property = base.Properties.Get("DllPath");
		if (property.Dirty)
		{
			if (string.IsNullOrEmpty((string)property.Value))
			{
				throw new PropertyNotSetException("DllPath");
			}
			queries.Add(ScriptUpgrade((string)property.Value));
		}
		Property property2 = base.Properties.Get("Enabled");
		if (property2.Dirty)
		{
			queries.Add(ScriptEnableDisable((bool)property2.Value));
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

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_CRYPTOGRAPHIC_PROVIDER, "", FormatFullNameForScripting(sp, nameIsIndentifier: false));
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP CRYPTOGRAPHIC PROVIDER {0}", new object[1] { FullQualifiedName });
		queries.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Upgrade(string pathToDll)
	{
		CheckObjectState(throwIfNotCreated: true);
		this.ThrowIfNotSupported(typeof(CryptographicProvider));
		try
		{
			if (!base.IsDesignMode)
			{
				ExecutionManager.ExecuteNonQuery(ScriptUpgrade(pathToDll));
			}
			if (!ExecutionManager.Recording)
			{
				SetDllPath(pathToDll);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.UpgradeDll, this, ex);
		}
	}

	private void SetDllPath(string pathToDll)
	{
		int index = base.Properties.LookupID("DllPath", PropertyAccessPurpose.Write);
		base.Properties.SetValue(index, pathToDll);
		base.Properties.SetRetrieved(index, val: true);
	}

	private void SetEnable(bool isEnabled)
	{
		int index = base.Properties.LookupID("Enabled", PropertyAccessPurpose.Write);
		base.Properties.SetValue(index, isEnabled);
		base.Properties.SetRetrieved(index, val: true);
	}

	private string ScriptUpgrade(string pathToDll)
	{
		return string.Format(SmoApplication.DefaultCulture, "ALTER CRYPTOGRAPHIC PROVIDER {0} FROM FILE = {1}", new object[2]
		{
			FullQualifiedName,
			SqlSmoObject.MakeSqlString(pathToDll)
		});
	}

	public void Enable()
	{
		CheckObjectState(throwIfNotCreated: true);
		this.ThrowIfNotSupported(typeof(CryptographicProvider));
		try
		{
			if (!base.IsDesignMode)
			{
				ExecutionManager.ExecuteNonQuery(ScriptEnableDisable(enable: true));
			}
			if (!ExecutionManager.Recording)
			{
				if (!SmoApplication.eventsSingleton.IsNullObjectAltered())
				{
					SmoApplication.eventsSingleton.CallObjectAltered(GetServerObject(), new ObjectAlteredEventArgs(base.Urn, this));
				}
				SetEnable(isEnabled: true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ProviderEnable, this, ex);
		}
	}

	public void Disable()
	{
		CheckObjectState(throwIfNotCreated: true);
		this.ThrowIfNotSupported(typeof(CryptographicProvider));
		try
		{
			if (!base.IsDesignMode)
			{
				ExecutionManager.ExecuteNonQuery(ScriptEnableDisable(enable: false));
			}
			if (!ExecutionManager.Recording)
			{
				if (!SmoApplication.eventsSingleton.IsNullObjectAltered())
				{
					SmoApplication.eventsSingleton.CallObjectAltered(GetServerObject(), new ObjectAlteredEventArgs(base.Urn, this));
				}
				SetEnable(isEnabled: false);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ProviderDisable, this, ex);
		}
	}

	private string ScriptEnableDisable(bool enable)
	{
		return string.Format(SmoApplication.DefaultCulture, "ALTER CRYPTOGRAPHIC PROVIDER {0} {1}", new object[2]
		{
			FullQualifiedName,
			enable ? "ENABLE" : "DISABLE"
		});
	}

	public DataTable EnumEncryptionAlgorithms()
	{
		CheckObjectState(throwIfNotCreated: true);
		this.ThrowIfNotSupported(typeof(CryptographicProvider));
		try
		{
			string query = string.Format(SmoApplication.DefaultCulture, "SELECT * from sys.dm_cryptographic_provider_algorithms({0})", new object[1] { ID });
			return ExecutionManager.ExecuteWithResults(query).Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumEncryptionAlgorithms, this, ex);
		}
	}

	public DataTable EnumProviderKeys()
	{
		CheckObjectState(throwIfNotCreated: true);
		this.ThrowIfNotSupported(typeof(CryptographicProvider));
		try
		{
			string query = string.Format(SmoApplication.DefaultCulture, "SELECT * from sys.dm_cryptographic_provider_keys({0})", new object[1] { ID });
			return ExecutionManager.ExecuteWithResults(query).Tables[0];
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumProviderKeys, this, ex);
		}
	}
}

using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

[StateChangeEvent("ALTER_ASYMMETRIC_KEY", "ASYMMETRICKEY", "ASYMMETRIC KEY")]
[StateChangeEvent("CREATE_ASYMMETRIC_KEY", "ASYMMETRICKEY", "ASYMMETRIC KEY")]
[StateChangeEvent("ALTER_AUTHORIZATION_DATABASE", "ASYMMETRIC KEY")]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class AsymmetricKey : NamedSmoObject, ISfcSupportsDesignMode, IObjectPermission, IAlterable, IDroppable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 8, 10, 10, 10, 10, 10, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 8 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[8]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("KeyEncryptionAlgorithm", expensive: false, readOnly: true, typeof(AsymmetricKeyEncryptionAlgorithm)),
			new StaticMetadata("KeyLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PrivateKeyEncryptionType", expensive: false, readOnly: true, typeof(PrivateKeyEncryptionType)),
			new StaticMetadata("PublicKey", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("Thumbprint", expensive: false, readOnly: true, typeof(byte[]))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("KeyEncryptionAlgorithm", expensive: false, readOnly: true, typeof(AsymmetricKeyEncryptionAlgorithm)),
			new StaticMetadata("KeyLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PrivateKeyEncryptionType", expensive: false, readOnly: true, typeof(PrivateKeyEncryptionType)),
			new StaticMetadata("PublicKey", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("Thumbprint", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("ProviderName", expensive: false, readOnly: false, typeof(string))
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
					"ID" => 0, 
					"KeyEncryptionAlgorithm" => 1, 
					"KeyLength" => 2, 
					"Owner" => 3, 
					"PrivateKeyEncryptionType" => 4, 
					"PublicKey" => 5, 
					"Sid" => 6, 
					"Thumbprint" => 7, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"KeyEncryptionAlgorithm" => 1, 
				"KeyLength" => 2, 
				"Owner" => 3, 
				"PrivateKeyEncryptionType" => 4, 
				"PublicKey" => 5, 
				"Sid" => 6, 
				"Thumbprint" => 7, 
				"PolicyHealthState" => 8, 
				"ProviderName" => 9, 
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

	private class CreateInfo
	{
		public bool createWithSource;

		public AsymmetricKeyEncryptionAlgorithm encryptionAlgorithm;

		public string keySource;

		public AsymmetricKeySourceType sourceType;

		public string providerAlgorithm;

		public string providerKeyName;

		public CreateDispositionType createDispositionType;

		public SqlSecureString password;

		public CreateInfo(AsymmetricKeyEncryptionAlgorithm encryptionAlgorithm, SqlSecureString password)
		{
			createWithSource = false;
			this.encryptionAlgorithm = encryptionAlgorithm;
			this.password = password;
		}

		public CreateInfo(string keySource, AsymmetricKeySourceType sourceType, SqlSecureString password)
		{
			createWithSource = true;
			this.password = password;
			this.keySource = keySource;
			this.sourceType = sourceType;
		}

		public CreateInfo(string providerAlgorithm, string providerKeyName, CreateDispositionType createDispositionType, AsymmetricKeySourceType sourceType)
		{
			createWithSource = true;
			this.providerAlgorithm = providerAlgorithm;
			this.providerKeyName = providerKeyName;
			this.createDispositionType = createDispositionType;
			this.sourceType = sourceType;
		}
	}

	private bool removeProviderKey;

	private CreateInfo createInfo;

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

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public AsymmetricKeyEncryptionAlgorithm KeyEncryptionAlgorithm => (AsymmetricKeyEncryptionAlgorithm)base.Properties.GetValueWithNullReplacement("KeyEncryptionAlgorithm");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int KeyLength => (int)base.Properties.GetValueWithNullReplacement("KeyLength");

	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Owner
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Owner");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Owner", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public PrivateKeyEncryptionType PrivateKeyEncryptionType => (PrivateKeyEncryptionType)base.Properties.GetValueWithNullReplacement("PrivateKeyEncryptionType");

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone)]
	[SfcReference(typeof(CryptographicProvider), "Server[@Name = '{0}']/CryptographicProvider[@Name = '{1}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "ProviderName" })]
	public string ProviderName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ProviderName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ProviderName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] PublicKey => (byte[])base.Properties.GetValueWithNullReplacement("PublicKey");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] Sid => (byte[])base.Properties.GetValueWithNullReplacement("Sid");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] Thumbprint => (byte[])base.Properties.GetValueWithNullReplacement("Thumbprint");

	public static string UrnSuffix => "AsymmetricKey";

	public AsymmetricKey()
	{
	}

	public AsymmetricKey(Database database, string name)
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

	public void Create(AsymmetricKeyEncryptionAlgorithm encryptionAlgorithm)
	{
		createInfo = new CreateInfo(encryptionAlgorithm, null);
		CreateImpl();
		SetProperties();
	}

	public void Create(AsymmetricKeyEncryptionAlgorithm encryptionAlgorithm, string password)
	{
		try
		{
			CheckNullArgument(password, "password");
			createInfo = new CreateInfo(encryptionAlgorithm, password);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
		SetProperties();
	}

	public void Create(string keySource, AsymmetricKeySourceType sourceType)
	{
		createInfo = new CreateInfo(keySource, sourceType, null);
		CreateImpl();
	}

	public void Create(string keySource, AsymmetricKeySourceType sourceType, string password)
	{
		try
		{
			CheckNullArgument(password, "password");
			createInfo = new CreateInfo(keySource, sourceType, password);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	public void Create(string providerAlgorithm, string providerKeyName, CreateDispositionType createDispositionType, AsymmetricKeySourceType sourceType)
	{
		try
		{
			ThrowIfBelowVersion100();
			CheckNullArgument(providerAlgorithm, "providerAlgorithm");
			CheckNullArgument(providerKeyName, "providerKeyName");
			ValidateAlgorithm(providerAlgorithm);
			createInfo = new CreateInfo(providerAlgorithm, providerKeyName, createDispositionType, sourceType);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	private void SetProperties()
	{
		if (!ExecutionManager.Recording && base.IsDesignMode)
		{
			SetEncryptionAlgorithm();
			createInfo = null;
		}
	}

	private void SetEncryptionAlgorithm()
	{
		int index = base.Properties.LookupID("KeyEncryptionAlgorithm", PropertyAccessPurpose.Write);
		base.Properties.SetValue(index, createInfo.encryptionAlgorithm);
		base.Properties.SetRetrieved(index, val: true);
	}

	private void ValidateAlgorithm(string providerAlgorithm)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.AddRange(new string[5] { "RSA_512", "RSA_1024", "RSA_2048", "RSA_3072", "RSA_4096" });
		if (!stringCollection.Contains(providerAlgorithm.ToUpper()))
		{
			throw new ArgumentException(ExceptionTemplatesImpl.InvalidAlgorithm("AsymmetricKey", providerAlgorithm));
		}
	}

	public void Drop()
	{
		DropImpl();
	}

	public void Drop(bool removeProviderKey)
	{
		this.removeProviderKey = removeProviderKey;
		DropImpl();
	}

	public void AddPrivateKey(string password)
	{
		try
		{
			CheckObjectState();
			if (PrivateKeyEncryptionType == PrivateKeyEncryptionType.Provider)
			{
				throw new Exception(ExceptionTemplatesImpl.CannotAlterKeyWithProvider);
			}
			CheckNullArgument(password, "password");
			StringBuilder stringBuilder = new StringBuilder("ALTER ASYMMETRIC KEY ");
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			stringBuilder.Append(FormatFullNameForScripting(scriptingPreferences));
			stringBuilder.Append(scriptingPreferences.NewLine);
			stringBuilder.Append("WITH PRIVATE KEY (ENCRYPTION BY PASSWORD=");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(password));
			stringBuilder.Append(")");
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			stringBuilder.Length = 0;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddPrivateKey, this, ex);
		}
	}

	public void RemovePrivateKey()
	{
		try
		{
			CheckObjectState();
			if (PrivateKeyEncryptionType == PrivateKeyEncryptionType.Provider)
			{
				throw new Exception(ExceptionTemplatesImpl.CannotAlterKeyWithProvider);
			}
			StringBuilder stringBuilder = new StringBuilder("ALTER ASYMMETRIC KEY ");
			ScriptingPreferences sp = new ScriptingPreferences();
			stringBuilder.Append(FormatFullNameForScripting(sp));
			stringBuilder.Append(" REMOVE PRIVATE KEY");
			Parent.ExecuteNonQuery(stringBuilder.ToString());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemovePrivateKey, this, ex);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	public void ChangePrivateKeyPassword(string oldPassword, string newPassword)
	{
		try
		{
			CheckObjectState();
			if (PrivateKeyEncryptionType == PrivateKeyEncryptionType.Provider)
			{
				throw new Exception(ExceptionTemplatesImpl.CannotAlterKeyWithProvider);
			}
			CheckNullArgument(oldPassword, "oldPassword");
			CheckNullArgument(newPassword, "newPassword");
			StringBuilder stringBuilder = new StringBuilder("ALTER ASYMMETRIC KEY ");
			ScriptingPreferences scriptingPreferences = new ScriptingPreferences();
			stringBuilder.Append(FormatFullNameForScripting(scriptingPreferences));
			stringBuilder.Append(scriptingPreferences.NewLine);
			stringBuilder.Append("WITH PRIVATE KEY (DECRYPTION BY PASSWORD=");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(oldPassword));
			stringBuilder.Append(", ENCRYPTION BY PASSWORD=");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(newPassword));
			stringBuilder.Append(")");
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			stringBuilder.Length = 0;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePrivateKeyPassword, this, ex);
		}
	}

	internal AsymmetricKey(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "ProviderName" };
	}

	private void CheckNullArgument(object arg, string argName)
	{
		if (arg == null)
		{
			throw new ArgumentNullException(argName);
		}
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ScriptChangeOwner(query, sp);
	}

	internal override void ScriptDrop(StringCollection query, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder("DROP ASYMMETRIC KEY ");
		stringBuilder.Append(FormatFullNameForScripting(sp));
		if (removeProviderKey && base.ServerVersion.Major >= 10)
		{
			stringBuilder.Append(" REMOVE PROVIDER KEY");
		}
		query.Add(stringBuilder.ToString());
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		CreateInfo createInfo = this.createInfo;
		if (!base.IsDesignMode || ExecutionManager.Recording)
		{
			this.createInfo = null;
		}
		this.ThrowIfNotSupported(typeof(AsymmetricKey));
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo, "caller should initialize createInfo it before calling");
		StringBuilder stringBuilder = new StringBuilder("CREATE ASYMMETRIC KEY ");
		stringBuilder.Append(FormatFullNameForScripting(sp));
		stringBuilder.Append(sp.NewLine);
		if (sp.IncludeScripts.Owner && GetPropValueOptional("Owner") != null)
		{
			stringBuilder.Append("AUTHORIZATION ");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket((string)GetPropValueOptional("Owner")));
			stringBuilder.Append(sp.NewLine);
		}
		if (createInfo.createWithSource)
		{
			stringBuilder.Append("FROM ");
			switch (createInfo.sourceType)
			{
			case AsymmetricKeySourceType.File:
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo.keySource, "keySource cannot be null");
				stringBuilder.Append("FILE = ");
				stringBuilder.Append(SqlSmoObject.MakeSqlString(createInfo.keySource));
				break;
			case AsymmetricKeySourceType.Executable:
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo.keySource, "keySource cannot be null");
				stringBuilder.Append("EXECUTABLE FILE = ");
				stringBuilder.Append(SqlSmoObject.MakeSqlString(createInfo.keySource));
				break;
			case AsymmetricKeySourceType.SqlAssembly:
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo.keySource, "keySource cannot be null");
				stringBuilder.Append("ASSEMBLY ");
				stringBuilder.Append(SqlSmoObject.MakeSqlString(createInfo.keySource));
				break;
			case AsymmetricKeySourceType.Provider:
			{
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo.providerKeyName, "providerKeyName cannot be null");
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo.providerAlgorithm, "providerAlgorithm cannot be null");
				stringBuilder.Append("PROVIDER ");
				string text = (string)GetPropValue("ProviderName");
				if (string.IsNullOrEmpty(text))
				{
					throw new PropertyNotSetException("ProviderName");
				}
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket(text));
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append("WITH ");
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(Globals.tab);
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "PROVIDER_KEY_NAME = '{0}', ", new object[1] { createInfo.providerKeyName }));
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(Globals.tab);
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "ALGORITHM = {0}, ", new object[1] { createInfo.providerAlgorithm }));
				stringBuilder.Append(Globals.newline);
				stringBuilder.Append(Globals.tab);
				stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "CREATION_DISPOSITION = {0}", new object[1] { (createInfo.createDispositionType == CreateDispositionType.CreateNew) ? "CREATE_NEW" : "OPEN_EXISTING" }));
				break;
			}
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("AsymmetricKeySourceType"));
			}
		}
		else
		{
			stringBuilder.Append("WITH ALGORITHM = ");
			switch (createInfo.encryptionAlgorithm)
			{
			case AsymmetricKeyEncryptionAlgorithm.Rsa512:
				stringBuilder.Append("RSA_512");
				break;
			case AsymmetricKeyEncryptionAlgorithm.Rsa1024:
				stringBuilder.Append("RSA_1024");
				break;
			case AsymmetricKeyEncryptionAlgorithm.Rsa2048:
				stringBuilder.Append("RSA_2048");
				break;
			case AsymmetricKeyEncryptionAlgorithm.Rsa3072:
				stringBuilder.Append("RSA_3072");
				break;
			case AsymmetricKeyEncryptionAlgorithm.Rsa4096:
				stringBuilder.Append("RSA_4096");
				break;
			case AsymmetricKeyEncryptionAlgorithm.CryptographicProviderDefined:
				throw new ArgumentException(ExceptionTemplatesImpl.SourceTypeShouldBeProvider);
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("AsymmetricKeyEncryptionAlgorithm"));
			}
		}
		if (null != createInfo.password)
		{
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("ENCRYPTION BY PASSWORD = ");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(createInfo.password.ToString()));
		}
		query.Add(stringBuilder.ToString());
		stringBuilder.Length = 0;
	}
}

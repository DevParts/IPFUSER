using System;
using System.Collections.Specialized;
using System.Data;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class SymmetricKey : NamedSmoObject, IObjectPermission, IAlterable, IDroppable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 8, 10, 10, 10, 10, 10, 10, 10 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 8 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[8]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: true, typeof(SymmetricKeyEncryptionAlgorithm)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsOpen", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("KeyGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("KeyLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[10]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("EncryptionAlgorithm", expensive: false, readOnly: true, typeof(SymmetricKeyEncryptionAlgorithm)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsOpen", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("KeyGuid", expensive: false, readOnly: true, typeof(Guid)),
			new StaticMetadata("KeyLength", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
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
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"EncryptionAlgorithm" => 2, 
					"ID" => 3, 
					"IsOpen" => 4, 
					"KeyGuid" => 5, 
					"KeyLength" => 6, 
					"Owner" => 7, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"EncryptionAlgorithm" => 2, 
				"ID" => 3, 
				"IsOpen" => 4, 
				"KeyGuid" => 5, 
				"KeyLength" => 6, 
				"Owner" => 7, 
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
		public SymmetricKeyEncryption[] keyEncryptions;

		public SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm;

		public SqlSecureString password;

		public string identityPhrase;

		public string providerAlgorithm;

		public string providerKeyName;

		public CreateDispositionType createDispositionType;

		public CreateInfo(SymmetricKeyEncryption[] keyEncryptions, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, SqlSecureString password, string identityPhrase)
		{
			this.keyEncryptions = keyEncryptions;
			this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
			this.password = password;
			this.identityPhrase = identityPhrase;
		}

		public CreateInfo(SymmetricKeyEncryption keyEncryption, string providerAlgorithm, string providerKeyName, CreateDispositionType createDispositionType)
		{
			keyEncryptions = new SymmetricKeyEncryption[1] { keyEncryption };
			keyEncryptionAlgorithm = SymmetricKeyEncryptionAlgorithm.CryptographicProviderDefined;
			this.providerAlgorithm = providerAlgorithm;
			this.providerKeyName = providerKeyName;
			this.createDispositionType = createDispositionType;
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public SymmetricKeyEncryptionAlgorithm EncryptionAlgorithm => (SymmetricKeyEncryptionAlgorithm)base.Properties.GetValueWithNullReplacement("EncryptionAlgorithm");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsOpen => (bool)base.Properties.GetValueWithNullReplacement("IsOpen");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public Guid KeyGuid => (Guid)base.Properties.GetValueWithNullReplacement("KeyGuid");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int KeyLength => (int)base.Properties.GetValueWithNullReplacement("KeyLength");

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

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	public static string UrnSuffix => "SymmetricKey";

	public SymmetricKey()
	{
	}

	public SymmetricKey(Database database, string name)
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

	public void Alter()
	{
		AlterImpl();
	}

	public void Create(SymmetricKeyEncryption keyEncryption, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			createInfo = new CreateInfo(new SymmetricKeyEncryption[1] { keyEncryption }, keyEncryptionAlgorithm, null, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	public void Create(SymmetricKeyEncryption keyEncryption, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, string passPhrase)
	{
		Create(keyEncryption, keyEncryptionAlgorithm, (passPhrase != null) ? new SqlSecureString(passPhrase) : null);
	}

	public void Create(SymmetricKeyEncryption keyEncryption, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, SecureString passPhrase)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckNullArgument(passPhrase, "passPhrase");
			if (passPhrase.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.PassPhraseNotSpecified);
			}
			createInfo = new CreateInfo(new SymmetricKeyEncryption[1] { keyEncryption }, keyEncryptionAlgorithm, passPhrase, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	public void Create(SymmetricKeyEncryption keyEncryption, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, string passPhrase, string identityPhrase)
	{
		Create(keyEncryption, keyEncryptionAlgorithm, (passPhrase != null) ? new SqlSecureString(passPhrase) : null, identityPhrase);
	}

	public void Create(SymmetricKeyEncryption keyEncryption, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, SecureString passPhrase, string identityPhrase)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckNullArgument(passPhrase, "passPhrase");
			CheckNullArgument(identityPhrase, "identityPhrase");
			if (passPhrase.Length == 0 && identityPhrase.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.PassPhraseAndIdentityNotSpecified);
			}
			createInfo = new CreateInfo(new SymmetricKeyEncryption[1] { keyEncryption }, keyEncryptionAlgorithm, (passPhrase.Length > 0) ? passPhrase : null, (identityPhrase.Length > 0) ? identityPhrase : null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	public void Create(SymmetricKeyEncryption keyEncryption, string providerAlgorithm, string providerKeyName, CreateDispositionType createDispositionType)
	{
		try
		{
			ThrowIfBelowVersion100();
			CheckNullArgument(providerAlgorithm, "providerAlgorithm");
			CheckNullArgument(providerKeyName, "providerKeyName");
			ValidateAlgorithm(providerAlgorithm);
			createInfo = new CreateInfo(keyEncryption, providerAlgorithm, providerKeyName, createDispositionType);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	private void ValidateAlgorithm(string providerAlgorithm)
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.AddRange(new string[10] { "DES", "TRIPLE_DES", "RC2", "RC4", "RC4_128", "DESX", "TRIPLE_DES_3KEY", "AES_128", "AES_192", "AES_256" });
		if (!stringCollection.Contains(providerAlgorithm.ToUpper()))
		{
			throw new ArgumentException(ExceptionTemplatesImpl.InvalidAlgorithm("SymmetricKey", providerAlgorithm));
		}
	}

	public void Create(SymmetricKeyEncryption[] keyEncryptions, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckNullArgument(keyEncryptions, "keyEncryptions");
			createInfo = new CreateInfo(keyEncryptions, keyEncryptionAlgorithm, null, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	public void Create(SymmetricKeyEncryption[] keyEncryptions, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, string passPhrase)
	{
		Create(keyEncryptions, keyEncryptionAlgorithm, (passPhrase != null) ? new SqlSecureString(passPhrase) : null);
	}

	public void Create(SymmetricKeyEncryption[] keyEncryptions, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, SecureString passPhrase)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckNullArgument(keyEncryptions, "keyEncryptions");
			CheckNullArgument(passPhrase, "passPhrase");
			if (passPhrase.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.PassPhraseNotSpecified);
			}
			createInfo = new CreateInfo(keyEncryptions, keyEncryptionAlgorithm, passPhrase, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
	}

	public void Create(SymmetricKeyEncryption[] keyEncryptions, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, string passPhrase, string identityPhrase)
	{
		Create(keyEncryptions, keyEncryptionAlgorithm, (passPhrase != null) ? new SqlSecureString(passPhrase) : null, identityPhrase);
	}

	public void Create(SymmetricKeyEncryption[] keyEncryptions, SymmetricKeyEncryptionAlgorithm keyEncryptionAlgorithm, SecureString passPhrase, string identityPhrase)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckNullArgument(keyEncryptions, "keyEncryptions");
			CheckNullArgument(passPhrase, "passPhrase");
			CheckNullArgument(identityPhrase, "identityPhrase");
			if (passPhrase.Length == 0 && identityPhrase.Length == 0)
			{
				throw new ArgumentException(ExceptionTemplatesImpl.PassPhraseAndIdentityNotSpecified);
			}
			createInfo = new CreateInfo(keyEncryptions, keyEncryptionAlgorithm, (passPhrase.Length > 0) ? new SqlSecureString(passPhrase) : null, (identityPhrase.Length > 0) ? identityPhrase : null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		CreateImpl();
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

	public DataTable EnumKeyEncryptions()
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/KeyEncryption")));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumKeyEncryptions, this, ex);
		}
	}

	public void AddKeyEncryption(SymmetricKeyEncryption keyEncryption)
	{
		AddKeyEncryption(new SymmetricKeyEncryption[1] { keyEncryption });
	}

	public void AddKeyEncryption(SymmetricKeyEncryption[] keyEncryptions)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			if (EncryptionAlgorithm == SymmetricKeyEncryptionAlgorithm.CryptographicProviderDefined)
			{
				throw new Exception(ExceptionTemplatesImpl.CannotAlterKeyWithProvider);
			}
			CheckNullArgument(keyEncryptions, "keyEncryptions");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(keyEncryptions.Length > 0);
			if (keyEncryptions.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder("ALTER SYMMETRIC KEY ");
				ScriptingPreferences sp = new ScriptingPreferences();
				stringBuilder.Append(FormatFullNameForScripting(sp));
				stringBuilder.Append(" ADD ");
				string text = ScriptSymmetricKeyEncryptions(keyEncryptions);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(text.Length > 0);
				stringBuilder.Append(text);
				Parent.ExecuteNonQuery(stringBuilder.ToString());
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddKeyEncryption, this, ex);
		}
	}

	public void DropKeyEncryption(SymmetricKeyEncryption keyEncryption)
	{
		DropKeyEncryption(new SymmetricKeyEncryption[1] { keyEncryption });
	}

	public void DropKeyEncryption(SymmetricKeyEncryption[] keyEncryptions)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			if (EncryptionAlgorithm == SymmetricKeyEncryptionAlgorithm.CryptographicProviderDefined)
			{
				throw new Exception(ExceptionTemplatesImpl.CannotAlterKeyWithProvider);
			}
			CheckNullArgument(keyEncryptions, "keyEncryptions");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(keyEncryptions.Length > 0);
			if (keyEncryptions.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder("ALTER SYMMETRIC KEY ");
				ScriptingPreferences sp = new ScriptingPreferences();
				stringBuilder.Append(FormatFullNameForScripting(sp));
				stringBuilder.Append(" DROP ");
				string text = ScriptSymmetricKeyEncryptions(keyEncryptions);
				Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(text.Length > 0);
				stringBuilder.Append(text);
				Parent.ExecuteNonQuery(stringBuilder.ToString());
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropKeyEncryption, this, ex);
		}
	}

	public void OpenWithCertificate(string certificateName)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			CheckNullArgument(certificateName, "certificate name");
			StringBuilder stringBuilder = new StringBuilder("OPEN SYMMETRIC KEY ");
			stringBuilder.Append(FormatFullNameForScripting(new ScriptingPreferences()));
			stringBuilder.Append(" DECRYPTION BY CERTIFICATE ");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(certificateName));
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SymmetricKeyOpen, this, ex);
		}
	}

	public void OpenWithCertificate(string certificateName, string privateKeyPassword)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			CheckNullArgument(certificateName, "certificate name");
			CheckNullArgument(privateKeyPassword, "private key password");
			StringBuilder stringBuilder = new StringBuilder("OPEN SYMMETRIC KEY ");
			stringBuilder.Append(FormatFullNameForScripting(new ScriptingPreferences()));
			stringBuilder.Append(" DECRYPTION BY CERTIFICATE ");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(certificateName));
			stringBuilder.Append(" WITH PASSWORD = ");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(privateKeyPassword));
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SymmetricKeyOpen, this, ex);
		}
	}

	public void OpenWithSymmetricKey(string symmetricKeyName)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			CheckNullArgument(symmetricKeyName, "symmetric key name");
			StringBuilder stringBuilder = new StringBuilder("OPEN SYMMETRIC KEY ");
			stringBuilder.Append(FormatFullNameForScripting(new ScriptingPreferences()));
			stringBuilder.Append(" DECRYPTION BY SYMMETRIC KEY ");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(symmetricKeyName));
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SymmetricKeyOpen, this, ex);
		}
	}

	public void Open(string password)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			CheckNullArgument(password, "password");
			StringBuilder stringBuilder = new StringBuilder("OPEN SYMMETRIC KEY ");
			stringBuilder.Append(FormatFullNameForScripting(new ScriptingPreferences()));
			stringBuilder.Append(" DECRYPTION BY PASSWORD = ");
			stringBuilder.Append(SqlSmoObject.MakeSqlString(password));
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(true);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SymmetricKeyOpen, this, ex);
		}
	}

	public void Close()
	{
		try
		{
			this.ThrowIfNotSupported(typeof(SymmetricKey));
			CheckObjectState(throwIfNotCreated: true);
			StringBuilder stringBuilder = new StringBuilder("CLOSE SYMMETRIC KEY ");
			stringBuilder.Append(FormatFullNameForScripting(new ScriptingPreferences()));
			Parent.ExecuteNonQuery(stringBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("IsOpen").SetValue(false);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.SymmetricKeyClose, this, ex);
		}
	}

	internal SymmetricKey(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ScriptChangeOwner(query, sp);
	}

	private void CheckNullArgument(object arg, string argName)
	{
		if (arg == null)
		{
			throw new ArgumentNullException(argName);
		}
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		CreateInfo createInfo = this.createInfo;
		this.createInfo = null;
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != createInfo, "caller should initialize createInfo it before calling");
		StringBuilder stringBuilder = new StringBuilder("CREATE SYMMETRIC KEY ");
		stringBuilder.Append(FormatFullNameForScripting(sp));
		stringBuilder.Append(sp.NewLine);
		if (sp.IncludeScripts.Owner && GetPropValueOptional("Owner") != null)
		{
			stringBuilder.Append("AUTHORIZATION ");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket((string)GetPropValueOptional("Owner")));
			stringBuilder.Append(sp.NewLine);
		}
		if (createInfo.keyEncryptionAlgorithm == SymmetricKeyEncryptionAlgorithm.CryptographicProviderDefined)
		{
			ThrowIfBelowVersion100();
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(createInfo.keyEncryptions.Length == 1, "There should be only one keyEncryptionType which is the provider");
			SymmetricKeyEncryption symmetricKeyEncryption = createInfo.keyEncryptions[0];
			CheckNullArgument(symmetricKeyEncryption, "keyEncryption");
			CheckNullArgument(symmetricKeyEncryption.ObjectNameOrPassword, "keyEncryption.ObjectNameOrPassword");
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(symmetricKeyEncryption.KeyEncryptionType == KeyEncryptionType.Provider);
			stringBuilder.Append(string.Format(SmoApplication.DefaultCulture, "FROM PROVIDER {0}", new object[1] { SqlSmoObject.MakeSqlBraket(symmetricKeyEncryption.ObjectNameOrPassword) }));
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
		}
		else
		{
			stringBuilder.Append("WITH ALGORITHM = ");
			switch (createInfo.keyEncryptionAlgorithm)
			{
			case SymmetricKeyEncryptionAlgorithm.RC2:
				stringBuilder.Append("RC2");
				break;
			case SymmetricKeyEncryptionAlgorithm.RC4:
				stringBuilder.Append("RC4");
				break;
			case SymmetricKeyEncryptionAlgorithm.Des:
				stringBuilder.Append("DES");
				break;
			case SymmetricKeyEncryptionAlgorithm.TripleDes:
				stringBuilder.Append("TRIPLE_DES");
				break;
			case SymmetricKeyEncryptionAlgorithm.DesX:
				stringBuilder.Append("DESX");
				break;
			case SymmetricKeyEncryptionAlgorithm.TripleDes3Key:
				stringBuilder.Append("TRIPLE_DES_3KEY");
				break;
			case SymmetricKeyEncryptionAlgorithm.Aes128:
				stringBuilder.Append("AES_128");
				break;
			case SymmetricKeyEncryptionAlgorithm.Aes192:
				stringBuilder.Append("AES_192");
				break;
			case SymmetricKeyEncryptionAlgorithm.Aes256:
				stringBuilder.Append("AES_256");
				break;
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("SymmetricKeyEncryptionAlgorithm"));
			}
			if (null != createInfo.password)
			{
				stringBuilder.Append(", KEY_SOURCE = ");
				stringBuilder.Append(SqlSmoObject.MakeSqlString((string)createInfo.password));
			}
			if (createInfo.identityPhrase != null)
			{
				stringBuilder.Append(", IDENTITY_VALUE = ");
				stringBuilder.Append(SqlSmoObject.MakeSqlString(createInfo.identityPhrase));
			}
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append(ScriptSymmetricKeyEncryptions(createInfo.keyEncryptions));
		}
		query.Add(stringBuilder.ToString());
	}

	private string ScriptSymmetricKeyEncryptions(SymmetricKeyEncryption[] keyEncryptions)
	{
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != keyEncryptions);
		if (keyEncryptions.Length <= 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder("ENCRYPTION BY ");
		bool flag = false;
		foreach (SymmetricKeyEncryption symmetricKeyEncryption in keyEncryptions)
		{
			CheckNullArgument(symmetricKeyEncryption, "keyEncryption");
			CheckNullArgument(symmetricKeyEncryption.ObjectNameOrPassword, "keyEncryption.ObjectNameOrPassword");
			if (flag)
			{
				stringBuilder.Append(", ");
			}
			switch (symmetricKeyEncryption.KeyEncryptionType)
			{
			case KeyEncryptionType.SymmetricKey:
				stringBuilder.Append("SYMMETRIC KEY ");
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket(symmetricKeyEncryption.ObjectNameOrPassword));
				break;
			case KeyEncryptionType.Certificate:
				stringBuilder.Append("CERTIFICATE ");
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket(symmetricKeyEncryption.ObjectNameOrPassword));
				break;
			case KeyEncryptionType.Password:
				stringBuilder.Append("PASSWORD = ");
				stringBuilder.Append(SqlSmoObject.MakeSqlString(symmetricKeyEncryption.ObjectNameOrPassword));
				break;
			case KeyEncryptionType.AsymmetricKey:
				stringBuilder.Append("ASYMMETRIC KEY ");
				stringBuilder.Append(SqlSmoObject.MakeSqlBraket(symmetricKeyEncryption.ObjectNameOrPassword));
				break;
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("KeyEncryptionType"));
			}
			flag = true;
		}
		return stringBuilder.ToString();
	}

	internal override void ScriptDrop(StringCollection query, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder("DROP SYMMETRIC KEY ");
		stringBuilder.Append(FormatFullNameForScripting(sp));
		if (removeProviderKey)
		{
			ThrowIfBelowVersion100();
			stringBuilder.Append(" REMOVE PROVIDER KEY");
		}
		query.Add(stringBuilder.ToString());
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[1] { "ProviderName" };
	}
}

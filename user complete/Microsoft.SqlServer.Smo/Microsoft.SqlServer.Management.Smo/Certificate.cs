using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class Certificate : NamedSmoObject, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IAlterable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 11, 13, 13, 13, 13, 13, 13, 13 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 11 };

		private static int sqlDwPropertyCount = 11;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("ActiveForServiceBrokerDialog", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ExpirationDate", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Issuer", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PrivateKeyEncryptionType", expensive: false, readOnly: true, typeof(PrivateKeyEncryptionType)),
			new StaticMetadata("Serial", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("StartDate", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("Subject", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Thumbprint", expensive: false, readOnly: true, typeof(byte[]))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[11]
		{
			new StaticMetadata("ActiveForServiceBrokerDialog", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ExpirationDate", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Issuer", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PrivateKeyEncryptionType", expensive: false, readOnly: true, typeof(PrivateKeyEncryptionType)),
			new StaticMetadata("Serial", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("StartDate", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("Subject", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Thumbprint", expensive: false, readOnly: true, typeof(byte[]))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[13]
		{
			new StaticMetadata("ActiveForServiceBrokerDialog", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ExpirationDate", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Issuer", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PrivateKeyEncryptionType", expensive: false, readOnly: true, typeof(PrivateKeyEncryptionType)),
			new StaticMetadata("Serial", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("StartDate", expensive: false, readOnly: false, typeof(DateTime)),
			new StaticMetadata("Subject", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Thumbprint", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("LastBackupDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
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
						"ActiveForServiceBrokerDialog" => 0, 
						"ExpirationDate" => 1, 
						"ID" => 2, 
						"Issuer" => 3, 
						"Owner" => 4, 
						"PrivateKeyEncryptionType" => 5, 
						"Serial" => 6, 
						"Sid" => 7, 
						"StartDate" => 8, 
						"Subject" => 9, 
						"Thumbprint" => 10, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"ActiveForServiceBrokerDialog" => 0, 
					"ExpirationDate" => 1, 
					"ID" => 2, 
					"Issuer" => 3, 
					"Owner" => 4, 
					"PrivateKeyEncryptionType" => 5, 
					"Serial" => 6, 
					"Sid" => 7, 
					"StartDate" => 8, 
					"Subject" => 9, 
					"Thumbprint" => 10, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ActiveForServiceBrokerDialog" => 0, 
				"ExpirationDate" => 1, 
				"ID" => 2, 
				"Issuer" => 3, 
				"Owner" => 4, 
				"PrivateKeyEncryptionType" => 5, 
				"Serial" => 6, 
				"Sid" => 7, 
				"StartDate" => 8, 
				"Subject" => 9, 
				"Thumbprint" => 10, 
				"LastBackupDate" => 11, 
				"PolicyHealthState" => 12, 
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

	private CertificateEvents events;

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
	public bool ActiveForServiceBrokerDialog
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ActiveForServiceBrokerDialog");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ActiveForServiceBrokerDialog", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime ExpirationDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("ExpirationDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExpirationDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Issuer => (string)base.Properties.GetValueWithNullReplacement("Issuer");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime LastBackupDate => (DateTime)base.Properties.GetValueWithNullReplacement("LastBackupDate");

	[CLSCompliant(false)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Serial => (string)base.Properties.GetValueWithNullReplacement("Serial");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] Sid => (byte[])base.Properties.GetValueWithNullReplacement("Sid");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime StartDate
	{
		get
		{
			return (DateTime)base.Properties.GetValueWithNullReplacement("StartDate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("StartDate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Subject
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Subject");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Subject", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] Thumbprint => (byte[])base.Properties.GetValueWithNullReplacement("Thumbprint");

	public CertificateEvents Events
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (SqlContext.IsAvailable)
			{
				throw new SmoException(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
			}
			if (events == null)
			{
				events = new CertificateEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "Certificate";

	public Certificate()
	{
	}

	public Certificate(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[3] { "ExpirationDate", "StartDate", "Subject" };
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

	public void Create()
	{
		try
		{
			this.ThrowIfNotSupported(typeof(Certificate));
			CreateInternal(null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	public void Create(string encryptionPassword)
	{
		try
		{
			this.ThrowIfNotSupported(typeof(Certificate));
			CheckNullArgument(encryptionPassword, "encryptionPassword");
			CreateInternal(encryptionPassword);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	public void Alter()
	{
		AlterImpl();
	}

	public void AddPrivateKey(string privateKeyPath, string decryptionPassword)
	{
		try
		{
			CheckObjectState();
			CheckNullArgument(privateKeyPath, "privateKeyPath");
			CheckNullArgument(decryptionPassword, "decryptionPassword");
			AddPrivateKeyInternal(privateKeyPath, decryptionPassword, null);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("PrivateKeyEncryptionType").SetValue(PrivateKeyEncryptionType.MasterKey);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddPrivateKey, this, ex);
		}
	}

	public void AddPrivateKey(string privateKeyPath, string decryptionPassword, string encryptionPassword)
	{
		try
		{
			CheckObjectState();
			CheckNullArgument(privateKeyPath, "privateKeyPath");
			CheckNullArgument(decryptionPassword, "decryptionPassword");
			CheckNullArgument(encryptionPassword, "encryptionPassword");
			AddPrivateKeyInternal(privateKeyPath, decryptionPassword, encryptionPassword);
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("PrivateKeyEncryptionType").SetValue(PrivateKeyEncryptionType.Password);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddPrivateKey, this, ex);
		}
	}

	public void Drop()
	{
		DropImpl();
	}

	public void Create(string certificateSource, CertificateSourceType sourceType)
	{
		try
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(Certificate));
			CheckNullArgument(certificateSource, "certificateSource");
			ImportInternal(certificateSource, sourceType, null, null, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	public void Create(string certificateSource, CertificateSourceType sourceType, string privateKeyPath, string privateKeyDecryptionPassword)
	{
		try
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(Certificate));
			CheckNullArgument(certificateSource, "certificateSource");
			CheckNullArgument(privateKeyPath, "privateKeyPath");
			CheckNullArgument(privateKeyDecryptionPassword, "privateKeyDecryptionPassword");
			ImportInternal(certificateSource, sourceType, privateKeyPath, privateKeyDecryptionPassword, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	public void Create(string certificateSource, CertificateSourceType sourceType, string privateKeyPath, string privateKeyDecryptionPassword, string privateKeyEncryptionPassword)
	{
		try
		{
			CheckObjectState();
			this.ThrowIfNotSupported(typeof(Certificate));
			CheckNullArgument(certificateSource, "certificateSource");
			CheckNullArgument(privateKeyPath, "privateKeyPath");
			CheckNullArgument(privateKeyDecryptionPassword, "privateKeyDecryptionPassword");
			CheckNullArgument(privateKeyEncryptionPassword, "privateKeyEncryptionPassword");
			ImportInternal(certificateSource, sourceType, privateKeyPath, privateKeyDecryptionPassword, privateKeyEncryptionPassword);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	public void Export(string certificatePath)
	{
		try
		{
			CheckObjectState();
			CheckNullArgument(certificatePath, "certificatePath");
			ExportInternal(certificatePath, null, null, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExportCertificate, this, ex);
		}
	}

	public void Export(string certificatePath, string privateKeyPath, string encryptionPassword)
	{
		try
		{
			CheckObjectState();
			CheckNullArgument(certificatePath, "certificatePath");
			CheckNullArgument(privateKeyPath, "privateKeyPath");
			CheckNullArgument(encryptionPassword, "encryptionPassword");
			ExportInternal(certificatePath, privateKeyPath, encryptionPassword, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExportCertificate, this, ex);
		}
	}

	public void Export(string certificatePath, string privateKeyPath, string encryptionPassword, string decryptionPassword)
	{
		try
		{
			CheckObjectState();
			CheckNullArgument(certificatePath, "certificatePath");
			CheckNullArgument(privateKeyPath, "privateKeyPath");
			CheckNullArgument(encryptionPassword, "encryptionPassword");
			CheckNullArgument(decryptionPassword, "decryptionPassword");
			ExportInternal(certificatePath, privateKeyPath, encryptionPassword, decryptionPassword);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ExportCertificate, this, ex);
		}
	}

	public void ChangePrivateKeyPassword(string oldPassword, string newPassword)
	{
		try
		{
			CheckObjectState();
			CheckNullArgument(oldPassword, "oldPassword");
			CheckNullArgument(newPassword, "newPassword");
			StringBuilder certificateBuilder = GetCertificateBuilder("ALTER");
			certificateBuilder.Append(" WITH PRIVATE KEY (");
			if (AddToStringBuilderIfNotNull(certificateBuilder, "DECRYPTION BY PASSWORD=", oldPassword, braket: false))
			{
				AddToStringBuilderIfNotNull(certificateBuilder, ", ENCRYPTION BY PASSWORD=", newPassword, braket: false);
			}
			certificateBuilder.Append(")");
			Parent.ExecuteNonQuery(certificateBuilder.ToString());
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePrivateKeyPassword, this, ex);
		}
	}

	public void RemovePrivateKey()
	{
		try
		{
			CheckObjectState();
			StringBuilder certificateBuilder = GetCertificateBuilder("ALTER");
			certificateBuilder.Append(" REMOVE PRIVATE KEY");
			Parent.ExecuteNonQuery(certificateBuilder.ToString());
			if (!ExecutionManager.Recording)
			{
				base.Properties.Get("PrivateKeyEncryptionType").SetValue(PrivateKeyEncryptionType.NoKey);
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.RemovePrivateKey, this, ex);
		}
	}

	internal Certificate(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	private StringBuilder GetCertificateBuilder(string operationName)
	{
		return GetCertificateBuilder(operationName, new ScriptingPreferences());
	}

	private StringBuilder GetCertificateBuilder(string operationName, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(Scripts.INCLUDE_EXISTS_CERTIFICATE, (string.Compare("CREATE", operationName, StringComparison.OrdinalIgnoreCase) == 0) ? "NOT" : "", ID);
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.Append(operationName);
		stringBuilder.Append(" CERTIFICATE ");
		if (Name != null)
		{
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(Name));
		}
		stringBuilder.Append(sp.NewLine);
		return stringBuilder;
	}

	private void CheckNullArgument(string arg, string argName)
	{
		if (arg == null)
		{
			throw new ArgumentNullException(argName);
		}
	}

	private bool AddToStringBuilderIfNotNull(StringBuilder sb, string prefix, object data, bool braket)
	{
		if (data == null)
		{
			return false;
		}
		sb.Append(prefix);
		if (!(data is string))
		{
			if (data is DateTime dateTime)
			{
				data = dateTime.ToString("MM/dd/yyyy", DateTimeFormatInfo.InvariantInfo);
			}
			else if (data is bool)
			{
				sb.Append(((bool)data) ? "ON" : "OFF");
				return true;
			}
		}
		if (braket)
		{
			sb.Append(SqlSmoObject.MakeSqlBraket(data.ToString()));
		}
		else
		{
			sb.Append(SqlSmoObject.MakeSqlString(data.ToString()));
		}
		return true;
	}

	private void CreateInternal(string encryptionPassword)
	{
		CreateImplInit(out var createQuery, out var sp);
		StringBuilder certificateBuilder = GetCertificateBuilder("CREATE", sp);
		if (sp.IncludeScripts.Owner && AddToStringBuilderIfNotNull(certificateBuilder, " AUTHORIZATION ", GetPropValueOptional("Owner"), braket: true))
		{
			certificateBuilder.Append(sp.NewLine);
		}
		if (AddToStringBuilderIfNotNull(certificateBuilder, " ENCRYPTION BY PASSWORD = ", encryptionPassword, braket: false))
		{
			certificateBuilder.Append(sp.NewLine);
		}
		AddToStringBuilderIfNotNull(certificateBuilder, "WITH SUBJECT = ", GetPropValue("Subject"), braket: false);
		certificateBuilder.Append(sp.NewLine);
		bool flag = AddToStringBuilderIfNotNull(certificateBuilder, ", START_DATE = ", GetPropValueOptional("StartDate"), braket: false);
		bool flag2 = AddToStringBuilderIfNotNull(certificateBuilder, ", EXPIRY_DATE = ", GetPropValueOptional("ExpirationDate"), braket: false);
		if (flag || flag2)
		{
			certificateBuilder.Append(sp.NewLine);
		}
		AddToStringBuilderIfNotNull(certificateBuilder, "ACTIVE FOR BEGIN_DIALOG = ", GetPropValueOptional("ActiveForServiceBrokerDialog"), braket: false);
		createQuery.Add(certificateBuilder.ToString());
		CreateImplFinish(createQuery, sp);
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		Property property = base.Properties.Get("ActiveForServiceBrokerDialog");
		if (property.Dirty)
		{
			StringBuilder certificateBuilder = GetCertificateBuilder("ALTER", sp);
			Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != property.Value);
			AddToStringBuilderIfNotNull(certificateBuilder, " WITH ACTIVE FOR BEGIN_DIALOG = ", property.Value, braket: false);
			query.Add(certificateBuilder.ToString());
		}
		ScriptChangeOwner(query, sp);
	}

	private void AddPrivateKeyInternal(string privateKeyPath, string decryptionPassword, string encryptionPassword)
	{
		StringBuilder certificateBuilder = GetCertificateBuilder("ALTER");
		if (AddToStringBuilderIfNotNull(certificateBuilder, " WITH PRIVATE KEY (FILE = ", privateKeyPath, braket: false))
		{
			AddToStringBuilderIfNotNull(certificateBuilder, " , DECRYPTION BY PASSWORD = ", decryptionPassword, braket: false);
			AddToStringBuilderIfNotNull(certificateBuilder, ",  ENCRYPTION BY PASSWORD = ", encryptionPassword, braket: false);
			certificateBuilder.Append(")");
		}
		Parent.ExecuteNonQuery(certificateBuilder.ToString());
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		dropQuery.Add(GetCertificateBuilder("DROP", sp).ToString());
	}

	private void ImportInternal(string certificateSource, CertificateSourceType sourceType, string privateKeyPath, string decryptionPassword, string encryptionPassword)
	{
		CreateImplInit(out var createQuery, out var sp);
		StringBuilder certificateBuilder = GetCertificateBuilder("CREATE", sp);
		if (sp.IncludeScripts.Owner)
		{
			AddToStringBuilderIfNotNull(certificateBuilder, " AUTHORIZATION ", GetPropValueOptional("Owner"), braket: true);
			certificateBuilder.Append(sp.NewLine);
		}
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(null != certificateSource, "certificateSource cannot be null");
		certificateBuilder.Append(" FROM ");
		switch (sourceType)
		{
		case CertificateSourceType.File:
			AddToStringBuilderIfNotNull(certificateBuilder, "FILE = ", certificateSource, braket: false);
			break;
		case CertificateSourceType.Executable:
			AddToStringBuilderIfNotNull(certificateBuilder, "EXECUTABLE FILE = ", certificateSource, braket: false);
			break;
		case CertificateSourceType.SqlAssembly:
			AddToStringBuilderIfNotNull(certificateBuilder, "ASSEMBLY ", certificateSource, braket: false);
			break;
		default:
			throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("CertificateSourceType"));
		}
		certificateBuilder.Append(sp.NewLine);
		if (AddToStringBuilderIfNotNull(certificateBuilder, " WITH PRIVATE KEY ( FILE=", privateKeyPath, braket: false))
		{
			if (AddToStringBuilderIfNotNull(certificateBuilder, ", DECRYPTION BY PASSWORD=", decryptionPassword, braket: false))
			{
				AddToStringBuilderIfNotNull(certificateBuilder, ", ENCRYPTION BY PASSWORD=", encryptionPassword, braket: false);
			}
			certificateBuilder.Append(")");
		}
		createQuery.Add(certificateBuilder.ToString());
		CreateImplFinish(createQuery, sp);
	}

	private void ExportInternal(string certificatePath, string privateKeyPath, string encryptionPassword, string decryptionPassword)
	{
		StringBuilder certificateBuilder = GetCertificateBuilder("BACKUP");
		AddToStringBuilderIfNotNull(certificateBuilder, " TO FILE=", certificatePath, braket: false);
		if (AddToStringBuilderIfNotNull(certificateBuilder, " WITH PRIVATE KEY( FILE =", privateKeyPath, braket: false))
		{
			AddToStringBuilderIfNotNull(certificateBuilder, ", ENCRYPTION BY PASSWORD=", encryptionPassword, braket: false);
			AddToStringBuilderIfNotNull(certificateBuilder, ", DECRYPTION BY PASSWORD=", decryptionPassword, braket: false);
			certificateBuilder.Append(")");
		}
		Parent.ExecuteNonQuery(certificateBuilder.ToString());
	}
}

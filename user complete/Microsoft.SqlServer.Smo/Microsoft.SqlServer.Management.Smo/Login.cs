using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Management.Smo.Internal;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[PhysicalFacet]
public sealed class Login : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IAlterable, IDroppable, IDropIfExists, IRenamable, IScriptable, ILoginOptions, IDmfFacet
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 11, 11, 21, 22, 22, 24, 24, 24, 24, 24 };

		private static int[] cloudVersionCount = new int[3] { 12, 12, 12 };

		private static int sqlDwPropertyCount = 12;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultDatabase", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Language", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("LanguageAlias", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("LoginType", expensive: false, readOnly: false, typeof(LoginType)),
			new StaticMetadata("PasswordExpirationEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PasswordPolicyEnforced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Sid", expensive: false, readOnly: false, typeof(byte[]))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultDatabase", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Language", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("LanguageAlias", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("LoginType", expensive: false, readOnly: false, typeof(LoginType)),
			new StaticMetadata("PasswordExpirationEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PasswordPolicyEnforced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Sid", expensive: false, readOnly: false, typeof(byte[]))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[24]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultDatabase", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DenyWindowsLogin", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("HasAccess", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Language", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("LanguageAlias", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("LoginType", expensive: false, readOnly: false, typeof(LoginType)),
			new StaticMetadata("Sid", expensive: false, readOnly: false, typeof(byte[])),
			new StaticMetadata("WindowsLoginAccessType", expensive: false, readOnly: true, typeof(WindowsLoginAccessType)),
			new StaticMetadata("AsymmetricKey", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Certificate", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("Credential", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsDisabled", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsLocked", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsPasswordExpired", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MustChangePassword", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("PasswordExpirationEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PasswordPolicyEnforced", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("PasswordHashAlgorithm", expensive: false, readOnly: true, typeof(PasswordHashAlgorithm)),
			new StaticMetadata("SidHexString", expensive: false, readOnly: true, typeof(string))
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
						"DefaultDatabase" => 2, 
						"ID" => 3, 
						"IsDisabled" => 4, 
						"IsSystemObject" => 5, 
						"Language" => 6, 
						"LanguageAlias" => 7, 
						"LoginType" => 8, 
						"PasswordExpirationEnabled" => 9, 
						"PasswordPolicyEnforced" => 10, 
						"Sid" => 11, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"DefaultDatabase" => 2, 
					"ID" => 3, 
					"IsDisabled" => 4, 
					"IsSystemObject" => 5, 
					"Language" => 6, 
					"LanguageAlias" => 7, 
					"LoginType" => 8, 
					"PasswordExpirationEnabled" => 9, 
					"PasswordPolicyEnforced" => 10, 
					"Sid" => 11, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"DefaultDatabase" => 2, 
				"DenyWindowsLogin" => 3, 
				"HasAccess" => 4, 
				"IsSystemObject" => 5, 
				"Language" => 6, 
				"LanguageAlias" => 7, 
				"LoginType" => 8, 
				"Sid" => 9, 
				"WindowsLoginAccessType" => 10, 
				"AsymmetricKey" => 11, 
				"Certificate" => 12, 
				"Credential" => 13, 
				"ID" => 14, 
				"IsDisabled" => 15, 
				"IsLocked" => 16, 
				"IsPasswordExpired" => 17, 
				"MustChangePassword" => 18, 
				"PasswordExpirationEnabled" => 19, 
				"PasswordPolicyEnforced" => 20, 
				"PolicyHealthState" => 21, 
				"PasswordHashAlgorithm" => 22, 
				"SidHexString" => 23, 
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

	private LoginEvents events;

	private SqlSecureString password;

	private bool passwordIsHashed;

	private bool mustChangePassword;

	private StringCollection credentialCollection = new StringCollection();

	private string oldCredential = string.Empty;

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

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone, "string.empty")]
	[SfcReference(typeof(AsymmetricKey), "Server[@Name = '{0}']/Database[@Name = 'master']/AsymmetricKey[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "AsymmetricKey" })]
	[CLSCompliant(false)]
	public string AsymmetricKey
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AsymmetricKey");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AsymmetricKey", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone, "string.empty")]
	[SfcReference(typeof(Certificate), "Server[@Name = '{0}']/Database[@Name = 'master']/Certificate[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "Certificate" })]
	[CLSCompliant(false)]
	public string Certificate
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Certificate");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Certificate", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone, "string.empty")]
	[SfcReference(typeof(Credential), "Server[@Name = '{0}']/Credential[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "Credential" })]
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
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "master")]
	[CLSCompliant(false)]
	[SfcReference(typeof(Database), "Server[@Name = '{0}']/Database[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "DefaultDatabase" })]
	public string DefaultDatabase
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultDatabase");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultDatabase", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool DenyWindowsLogin
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("DenyWindowsLogin");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DenyWindowsLogin", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "true")]
	public bool HasAccess => (bool)base.Properties.GetValueWithNullReplacement("HasAccess");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "false")]
	public bool IsDisabled => (bool)base.Properties.GetValueWithNullReplacement("IsDisabled");

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool IsLocked => (bool)base.Properties.GetValueWithNullReplacement("IsLocked");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsPasswordExpired => (bool)base.Properties.GetValueWithNullReplacement("IsPasswordExpired");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "false")]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Language
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Language");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Language", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string LanguageAlias => (string)base.Properties.GetValueWithNullReplacement("LanguageAlias");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public LoginType LoginType
	{
		get
		{
			return (LoginType)base.Properties.GetValueWithNullReplacement("LoginType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("LoginType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone, "false")]
	public bool MustChangePassword => (bool)base.Properties.GetValueWithNullReplacement("MustChangePassword");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "false")]
	public bool PasswordExpirationEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("PasswordExpirationEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PasswordExpirationEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public PasswordHashAlgorithm PasswordHashAlgorithm => (PasswordHashAlgorithm)base.Properties.GetValueWithNullReplacement("PasswordHashAlgorithm");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "false")]
	public bool PasswordPolicyEnforced
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("PasswordPolicyEnforced");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PasswordPolicyEnforced", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] Sid
	{
		get
		{
			return (byte[])base.Properties.GetValueWithNullReplacement("Sid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Sid", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public WindowsLoginAccessType WindowsLoginAccessType => (WindowsLoginAccessType)base.Properties.GetValueWithNullReplacement("WindowsLoginAccessType");

	public LoginEvents Events
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
				events = new LoginEvents(this);
			}
			return events;
		}
	}

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	public static string UrnSuffix => "Login";

	public Login()
	{
	}

	public Login(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[4] { "AsymmetricKey", "Certificate", "LoginType", "Sid" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		return propname switch
		{
			"AsymmetricKey" => "string.empty", 
			"Certificate" => "string.empty", 
			"Credential" => "string.empty", 
			"DefaultDatabase" => "master", 
			"DenyWindowsLogin" => false, 
			"HasAccess" => true, 
			"IsDisabled" => false, 
			"IsLocked" => false, 
			"IsSystemObject" => false, 
			"MustChangePassword" => false, 
			"PasswordExpirationEnabled" => false, 
			"PasswordPolicyEnforced" => false, 
			_ => base.GetPropertyDefaultValue(propname), 
		};
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

	internal Login(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		InitVariables();
	}

	private void InitVariables()
	{
		password = null;
		passwordIsHashed = false;
		mustChangePassword = false;
	}

	public StringCollection EnumCredentials()
	{
		ThrowIfBelowVersion100();
		StringCollection stringCollection = new StringCollection();
		if (base.IsDesignMode)
		{
			StringEnumerator enumerator = credentialCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					stringCollection.Add(current);
				}
				return stringCollection;
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}
		string query = string.Format(SmoApplication.DefaultCulture, "select name from sys.server_principal_credentials as p join sys.credentials as c on c.credential_id = p.credential_id where p.principal_id = {0}", new object[1] { ID.ToString() });
		SqlExecutionModes sqlExecutionModes = ExecutionManager.ConnectionContext.SqlExecutionModes;
		ExecutionManager.ConnectionContext.SqlExecutionModes = SqlExecutionModes.ExecuteSql;
		try
		{
			DataTable dataTable = ExecutionManager.ExecuteWithResults(query).Tables[0];
			foreach (DataRow row in dataTable.Rows)
			{
				stringCollection.Add((string)row["name"]);
			}
			return stringCollection;
		}
		finally
		{
			ExecutionManager.ConnectionContext.SqlExecutionModes = sqlExecutionModes;
		}
	}

	public void ChangePassword(string newPassword)
	{
		SqlSecureString sqlSecureString = null;
		if (newPassword != null)
		{
			sqlSecureString = new SqlSecureString(newPassword);
		}
		ChangePassword(sqlSecureString);
	}

	public void ChangePassword(SecureString newPassword)
	{
		try
		{
			CheckObjectState();
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			ExecuteLoginPasswordOptions(newPassword, null, bUnlock: false, bMustChange: false);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	public void ChangePassword(string oldPassword, string newPassword)
	{
		SqlSecureString sqlSecureString = null;
		SqlSecureString sqlSecureString2 = null;
		if (oldPassword != null)
		{
			sqlSecureString = new SqlSecureString(oldPassword);
		}
		if (newPassword != null)
		{
			sqlSecureString2 = new SqlSecureString(newPassword);
		}
		ChangePassword(sqlSecureString, sqlSecureString2);
	}

	public void ChangePassword(SecureString oldPassword, SecureString newPassword)
	{
		try
		{
			CheckObjectState();
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			if (oldPassword == null)
			{
				throw new ArgumentNullException("oldPassword");
			}
			ExecuteLoginPasswordOptions(newPassword, oldPassword, bUnlock: false, bMustChange: false);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	public void ChangePassword(string newPassword, bool unlock, bool mustChange)
	{
		SqlSecureString sqlSecureString = null;
		if (newPassword != null)
		{
			sqlSecureString = new SqlSecureString(newPassword);
		}
		ChangePassword(sqlSecureString, unlock, mustChange);
	}

	public void ChangePassword(SecureString newPassword, bool unlock, bool mustChange)
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			ExecuteLoginPasswordOptions(newPassword, null, unlock, mustChange);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	public void Create()
	{
		CreateImpl();
		SetProperties();
	}

	private void SetProperties()
	{
		if (!ExecutionManager.Recording && base.IsDesignMode)
		{
			SetWindowsLoginType();
			SetHasAccess();
			SetCredential();
		}
	}

	private void SetHasAccess()
	{
		bool flag = true;
		Property property = base.Properties.Get("DenyWindowsLogin");
		if (property.Value != null && (bool)property.Value)
		{
			flag = false;
		}
		int index = base.Properties.LookupID("HasAccess", PropertyAccessPurpose.Write);
		base.Properties.SetValue(index, flag);
		base.Properties.SetRetrieved(index, val: true);
	}

	private void SetMustChangePassword()
	{
		if (!ExecutionManager.Recording && base.IsDesignMode && IsVersion90AndAbove())
		{
			int index = base.Properties.LookupID("MustChangePassword", PropertyAccessPurpose.Write);
			base.Properties.SetValue(index, mustChangePassword);
			base.Properties.SetRetrieved(index, val: true);
		}
	}

	private void SetCredential()
	{
		if (!IsVersion90AndAbove())
		{
			return;
		}
		string value = base.Properties.Get("Credential").Value as string;
		if (string.IsNullOrEmpty(value))
		{
			if (!string.IsNullOrEmpty(oldCredential) && credentialCollection.Contains(oldCredential))
			{
				credentialCollection.Remove(oldCredential);
			}
		}
		else if (!credentialCollection.Contains(value))
		{
			credentialCollection.Add(value);
		}
		oldCredential = value;
	}

	private void SetWindowsLoginType()
	{
		WindowsLoginAccessType windowsLoginAccessType = WindowsLoginAccessType.Undefined;
		Property property = base.Properties.Get("LoginType");
		if (property.Value != null && ((LoginType)property.Value == LoginType.WindowsUser || (LoginType)property.Value == LoginType.WindowsGroup))
		{
			Property property2 = base.Properties.Get("DenyWindowsLogin");
			windowsLoginAccessType = ((property2.Value == null || !(bool)property2.Value) ? WindowsLoginAccessType.Grant : WindowsLoginAccessType.Deny);
		}
		else
		{
			windowsLoginAccessType = WindowsLoginAccessType.NonNTLogin;
		}
		int index = base.Properties.LookupID("WindowsLoginAccessType", PropertyAccessPurpose.Write);
		base.Properties.SetValue(index, windowsLoginAccessType);
		base.Properties.SetRetrieved(index, val: true);
	}

	public void Create(string password)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("password"));
		}
		Create(new SqlSecureString(password));
	}

	public void Create(SecureString password)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("password"));
		}
		this.password = password;
		Create();
	}

	public void Create(string password, LoginCreateOptions options)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("password"));
		}
		Create(new SqlSecureString(password), options);
	}

	public void Create(SecureString password, LoginCreateOptions options)
	{
		if (password == null)
		{
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("password"));
		}
		passwordIsHashed = (options & LoginCreateOptions.IsHashed) != 0;
		mustChangePassword = (options & LoginCreateOptions.MustChange) != 0;
		Create(password);
	}

	internal void ScriptCreateCheck(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			ScriptLogin(query, sp, bForCreate: true, bForServerCreateCheck: true);
		}
		else
		{
			ScriptCreateLess9(query, sp);
		}
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType)
		{
			ScriptCreateForCloud(query, sp);
		}
		else if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			ScriptCreateGreaterEqual9(query, sp);
		}
		else
		{
			ScriptCreateLess9(query, sp);
		}
	}

	private void AddComma(StringBuilder sb, ref bool bStuffAdded)
	{
		if (bStuffAdded)
		{
			sb.Append(Globals.commaspace);
		}
		else
		{
			bStuffAdded = true;
		}
	}

	private void AppendSid(object oSid, StringBuilder sb)
	{
		sb.Append("0x");
		byte[] array = (byte[])oSid;
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			sb.Append(b.ToString("X2", SmoApplication.DefaultCulture));
		}
	}

	private string BoolToOnOff(object oBool)
	{
		if (!(bool)oBool)
		{
			return "OFF";
		}
		return "ON";
	}

	private void ExecuteLoginPasswordOptions(SqlSecureString password, SqlSecureString oldPassword, bool bUnlock, bool bMustChange)
	{
		if (LoginType != LoginType.SqlLogin)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotChangePassword);
		}
		StringCollection stringCollection = new StringCollection();
		if (base.ServerVersion.Major < 9)
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_password @old={0}, @new={1}, @loginame={2}", new object[3]
			{
				(oldPassword == null) ? "NULL" : SqlSmoObject.MakeSqlString((string)oldPassword),
				SqlSmoObject.MakeSqlString((string)password),
				FormatFullNameForScripting(new ScriptingPreferences())
			}));
		}
		else
		{
			stringCollection.Add("USE [master]");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ALTER LOGIN ");
			stringBuilder.Append(FormatFullNameForScripting(new ScriptingPreferences()));
			stringBuilder.Append(" WITH ");
			AddPasswordOptions(null, stringBuilder, password, oldPassword, bIsHashed: false, bMustChange, bUnlock);
			stringCollection.Add(stringBuilder.ToString());
		}
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	private void AddPasswordOptions(ScriptingPreferences sp, StringBuilder sb, SqlSecureString password, SqlSecureString oldPassword, bool bIsHashed, bool bMustChange, bool bUnlock)
	{
		if (null != password)
		{
			sb.Append("PASSWORD=");
			if (bIsHashed)
			{
				ValidatePasswordHash((string)password);
				sb.Append((string)password);
			}
			else
			{
				sb.Append(SqlSmoObject.MakeSqlString((string)password));
			}
			if (null != oldPassword)
			{
				sb.Append(" OLD_PASSWORD=");
				sb.Append(SqlSmoObject.MakeSqlString((string)oldPassword));
			}
			if (bUnlock)
			{
				sb.Append(" UNLOCK");
			}
			if (bIsHashed)
			{
				sb.Append(" HASHED");
			}
			if (bMustChange)
			{
				sb.Append(" MUST_CHANGE");
			}
		}
		else
		{
			if (sp == null || sp.ScriptForCreateDrop || sp.ScriptForAlter)
			{
				throw new PropertyNotSetException("password");
			}
			sb.Append("PASSWORD=");
			sb.Append(SqlSmoObject.MakeSqlString(SecurityUtils.GenerateRandomPassword()));
		}
	}

	private void ValidatePasswordHash(string passwordHash)
	{
		foreach (char c in passwordHash)
		{
			if (c == ' ')
			{
				throw new SmoException(ExceptionTemplatesImpl.InvalidPasswordHash);
			}
		}
	}

	private bool HasMainDdlDirtyProps()
	{
		foreach (Property property in base.Properties)
		{
			if (property.Dirty && property.Name != "DenyWindowsLogin")
			{
				return true;
			}
		}
		return false;
	}

	private void ScriptLogin(StringCollection sc, ScriptingPreferences sp, bool bForCreate, bool bForServerCreateCheck)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		StringCollection stringCollection = new StringCollection();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (bForCreate || HasMainDdlDirtyProps())
		{
			if (bForCreate && sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LOGIN90, new object[2]
				{
					"NOT",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append(bForCreate ? "CREATE LOGIN " : "ALTER LOGIN ");
			string text = FormatFullNameForScripting(sp);
			stringBuilder.Append(text);
			StringBuilder stringBuilder3 = new StringBuilder();
			bool bStuffAdded = false;
			if (LoginType == LoginType.SqlLogin)
			{
				if (bForCreate && !bForServerCreateCheck)
				{
					if (!sp.ScriptForCreateDrop)
					{
						stringBuilder.Insert(0, Globals.newline);
						stringBuilder.Insert(0, "/* For security reasons the login is created disabled and with a random password. */");
					}
					AddPasswordOptions(sp, stringBuilder3, password, null, passwordIsHashed, mustChangePassword, bUnlock: false);
					if (stringBuilder3.Length > 0)
					{
						bStuffAdded = true;
					}
					if (!sp.ScriptForCreateDrop)
					{
						if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType && sp.IncludeScripts.Associations)
						{
							ScriptAssociations(stringCollection, sp);
						}
						stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "ALTER LOGIN {0} DISABLE", new object[1] { text });
					}
				}
				object propValueOptional = GetPropValueOptional("Sid");
				if (propValueOptional != null && (sp.ScriptForCreateDrop || sp.Security.Sid))
				{
					if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
					{
						throw new InvalidSmoOperationException(ExceptionTemplatesImpl.CloudSidNotApplicableOnStandalone);
					}
					AddComma(stringBuilder3, ref bStuffAdded);
					stringBuilder3.Append("SID=");
					AppendSid(propValueOptional, stringBuilder3);
				}
				string text2 = (string)GetPropValueOptional("DefaultDatabase");
				if (text2 != null && text2.Length > 0)
				{
					AddComma(stringBuilder3, ref bStuffAdded);
					stringBuilder3.Append("DEFAULT_DATABASE=");
					stringBuilder3.Append(SqlSmoObject.MakeSqlBraket(text2));
				}
				text2 = (string)GetPropValueOptional("Language");
				if (text2 != null && text2.Length > 0)
				{
					AddComma(stringBuilder3, ref bStuffAdded);
					stringBuilder3.Append("DEFAULT_LANGUAGE=");
					stringBuilder3.Append(SqlSmoObject.MakeSqlBraket(text2));
				}
				if (base.ServerVersion.Major >= 9)
				{
					object propValueOptional2 = GetPropValueOptional("PasswordExpirationEnabled");
					if (propValueOptional2 != null)
					{
						AddComma(stringBuilder3, ref bStuffAdded);
						stringBuilder3.Append("CHECK_EXPIRATION=");
						stringBuilder3.Append(BoolToOnOff(propValueOptional2));
					}
					propValueOptional2 = GetPropValueOptional("PasswordPolicyEnforced");
					if (propValueOptional2 != null)
					{
						AddComma(stringBuilder3, ref bStuffAdded);
						stringBuilder3.Append("CHECK_POLICY=");
						stringBuilder3.Append(BoolToOnOff(propValueOptional2));
					}
					if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
					{
						Property property = base.Properties.Get("Credential");
						if (property.Value != null && (property.Dirty || bForCreate))
						{
							string text3 = (string)property.Value;
							if (text3.Length == 0)
							{
								if (!bForCreate)
								{
									AddComma(stringBuilder3, ref bStuffAdded);
									stringBuilder3.Append("NO CREDENTIAL");
								}
							}
							else
							{
								AddComma(stringBuilder3, ref bStuffAdded);
								stringBuilder3.Append("CREDENTIAL = ");
								stringBuilder3.Append(SqlSmoObject.MakeSqlBraket(text3));
							}
						}
					}
				}
			}
			else if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
			{
				if (bForCreate && (LoginType == LoginType.Certificate || LoginType == LoginType.AsymmetricKey))
				{
					string text4 = ((LoginType == LoginType.Certificate) ? "Certificate" : "AsymmetricKey");
					string text5 = ((LoginType == LoginType.Certificate) ? "CERTIFICATE" : "ASYMMETRIC KEY");
					string text6 = (string)GetPropValue(text4);
					if (text6 == null || text6.Length == 0)
					{
						throw new PropertyNotSetException(text4);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FROM {0} {1}", new object[2]
					{
						text5,
						SqlSmoObject.MakeSqlBraket(text6)
					});
				}
				else
				{
					if (bForCreate)
					{
						stringBuilder.Append(" FROM WINDOWS");
					}
					_ = stringBuilder3.Length;
					object propValueOptional3 = GetPropValueOptional("DefaultDatabase");
					if (propValueOptional3 != null)
					{
						bStuffAdded = true;
						stringBuilder3.Append("DEFAULT_DATABASE=");
						stringBuilder3.Append(SqlSmoObject.MakeSqlBraket((string)propValueOptional3));
					}
					propValueOptional3 = GetPropValueOptional("Language");
					if (propValueOptional3 != null && ((string)propValueOptional3).Length > 0)
					{
						AddComma(stringBuilder3, ref bStuffAdded);
						stringBuilder3.Append("DEFAULT_LANGUAGE=");
						stringBuilder3.Append(SqlSmoObject.MakeSqlBraket((string)propValueOptional3));
					}
				}
			}
			if (stringBuilder3.Length > 0)
			{
				stringBuilder.Append(" WITH ");
				stringBuilder.Append(stringBuilder3.ToString());
			}
		}
		if (stringBuilder.Length > 0)
		{
			sc.Add(stringBuilder.ToString());
			StringEnumerator enumerator = stringCollection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					sc.Add(current);
				}
			}
			finally
			{
				if (enumerator is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			if (stringBuilder2.Length > 0)
			{
				sc.Add(stringBuilder2.ToString());
			}
		}
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
		{
			return;
		}
		Property property2 = base.Properties.Get("DenyWindowsLogin");
		if (bForCreate && property2.Value != null && (bool)property2.Value)
		{
			sc.Add("DENY CONNECT SQL TO " + FormatFullNameForScripting(sp));
		}
		if (!bForCreate && property2.Dirty)
		{
			if ((bool)property2.Value)
			{
				sc.Add("DENY CONNECT SQL TO " + FormatFullNameForScripting(sp));
			}
			else
			{
				sc.Add("GRANT CONNECT SQL TO " + FormatFullNameForScripting(sp));
			}
		}
	}

	internal override void ScriptAssociations(StringCollection rolesCmd, ScriptingPreferences sp)
	{
		if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			return;
		}
		StringCollection stringCollection = ListMembers();
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				rolesCmd.Add(GetAddToRoleDdl(current, sp));
			}
		}
		finally
		{
			if (enumerator is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
	}

	private void ScriptCreateForCloud(StringCollection query, ScriptingPreferences sp)
	{
		if (LoginType != LoginType.SqlLogin)
		{
			throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "LoginType", LoginType.ToString(), LocalizableResources.EngineCloud));
		}
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		stringBuilder.Append("CREATE LOGIN ");
		string text = FormatFullNameForScripting(sp);
		stringBuilder.Append(text);
		StringBuilder stringBuilder3 = new StringBuilder();
		if (!sp.ScriptForCreateDrop)
		{
			stringBuilder.Insert(0, Globals.newline);
			stringBuilder.Insert(0, "/* For security reasons the login is created disabled and with a random password. */");
		}
		if (null != password)
		{
			stringBuilder3.Append("PASSWORD=");
			stringBuilder3.Append(SqlSmoObject.MakeSqlString((string)password));
		}
		else
		{
			if (sp == null || sp.ScriptForCreateDrop)
			{
				throw new PropertyNotSetException("password");
			}
			stringBuilder3.Append("PASSWORD=");
			stringBuilder3.Append(SqlSmoObject.MakeSqlString(SecurityUtils.GenerateRandomPassword()));
		}
		if (!sp.ScriptForCreateDrop)
		{
			stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "ALTER LOGIN {0} DISABLE", new object[1] { text });
		}
		if (stringBuilder3.Length > 0)
		{
			stringBuilder.Append(" WITH ");
			stringBuilder.Append(stringBuilder3.ToString());
		}
		if (stringBuilder.Length > 0)
		{
			query.Add(stringBuilder.ToString());
			if (stringBuilder2.Length > 0)
			{
				query.Add(stringBuilder2.ToString());
			}
		}
	}

	private void ScriptCreateGreaterEqual9(StringCollection query, ScriptingPreferences sp)
	{
		ScriptLogin(query, sp, bForCreate: true, bForServerCreateCheck: false);
	}

	private void ScriptCreateLess9(StringCollection query, ScriptingPreferences sp)
	{
		bool suppressDirtyCheck = sp.SuppressDirtyCheck;
		StringBuilder stringBuilder = new StringBuilder();
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LOGIN80, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.Append(sp.NewLine);
			stringBuilder.Append("BEGIN");
			stringBuilder.Append(sp.NewLine);
		}
		if (LoginType == LoginType.SqlLogin)
		{
			stringBuilder.Append("EXEC master.dbo.sp_addlogin ");
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@loginame = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
			int num = 0;
			if (null != password)
			{
				stringBuilder.Append(Globals.commaspace);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@passwd = {0}", new object[1] { SqlSmoObject.MakeSqlString((string)password) });
				num++;
			}
			else
			{
				if (sp.ScriptForCreateDrop)
				{
					throw new PropertyNotSetException("password");
				}
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				SecurityUtils.ScriptRandomPwd(stringBuilder2);
				stringBuilder.Insert(0, stringBuilder2.ToString());
				stringBuilder.Append(", @passwd = @randomPwd");
				num++;
			}
			Property property = base.Properties.Get("DefaultDatabase");
			if (property.Value != null && (suppressDirtyCheck || property.Dirty))
			{
				stringBuilder.Append(Globals.commaspace);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@defdb = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)property.Value) });
				num++;
			}
			object propValueOptional = GetPropValueOptional("Sid");
			if (propValueOptional != null && (sp.ScriptForCreateDrop || sp.Security.Sid))
			{
				if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
				{
					throw new InvalidSmoOperationException(ExceptionTemplatesImpl.CloudSidNotApplicableOnStandalone);
				}
				stringBuilder.Append(", @sid=");
				AppendSid(propValueOptional, stringBuilder);
			}
			property = base.Properties.Get("Language");
			if (property.Value != null && (suppressDirtyCheck || property.Dirty) && 0 < ((string)property.Value).Length)
			{
				stringBuilder.Append(Globals.commaspace);
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "@deflanguage = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)property.Value) });
				num++;
			}
		}
		else if (DatabaseEngineType.SqlAzureDatabase != DatabaseEngineType)
		{
			if (LoginType != LoginType.WindowsUser && LoginType != LoginType.WindowsGroup)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnknownEnumeration(LoginType.GetType().Name));
			}
			bool flag = false;
			Property property2 = base.Properties.Get("DenyWindowsLogin");
			if (property2.Value != null && (suppressDirtyCheck || property2.Dirty))
			{
				flag = (bool)property2.Value;
			}
			if (flag)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_denylogin @loginame = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
				stringBuilder.Append(Globals.newline);
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_grantlogin @loginame = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
				stringBuilder.Append(Globals.newline);
				Property property3 = base.Properties.Get("DefaultDatabase");
				if (property3.Value != null && (suppressDirtyCheck || property3.Dirty))
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_defaultdb @loginame = {0}, @defdb = N'{1}'", new object[2]
					{
						FormatFullNameForScripting(sp, nameIsIndentifier: false),
						SqlSmoObject.SqlString((string)property3.Value)
					});
					stringBuilder.Append(Globals.newline);
				}
				GetLanguageDDL(stringBuilder, suppressDirtyCheck);
			}
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.Append(Globals.newline);
			stringBuilder.Append("END");
		}
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Insert(0, Globals.newline);
			stringBuilder.Insert(0, ExceptionTemplates.IncludeHeader(UrnSuffix, FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
		}
		query.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		AlterImpl();
		SetProperties();
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			ScriptAlterGreaterEqual9(query, sp);
		}
		else
		{
			ScriptAlterLess9(query, sp);
		}
	}

	private void ScriptAlterGreaterEqual9(StringCollection query, ScriptingPreferences sp)
	{
		ScriptLogin(query, sp, bForCreate: false, bForServerCreateCheck: false);
	}

	private void ScriptAlterLess9(StringCollection query, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Property property = base.Properties.Get("DefaultDatabase");
		if (property.Value != null && property.Dirty)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_defaultdb @loginame= N'{0}', @defdb= N'{1}'", new object[2]
			{
				SqlSmoObject.SqlString(Name),
				SqlSmoObject.SqlString(property.Value.ToString())
			});
			query.Add(stringBuilder.ToString());
			stringBuilder.Length = 0;
		}
		LoginType loginType = (LoginType)base.Properties.Get("LoginType").Value;
		if (loginType == LoginType.WindowsUser || loginType == LoginType.WindowsGroup)
		{
			if (null != password)
			{
				throw new SmoException(ExceptionTemplatesImpl.PasswdModiOnlyForStandardLogin);
			}
			Property property2 = base.Properties.Get("DenyWindowsLogin");
			if (property2.Value != null && property2.Dirty)
			{
				if ((bool)property2.Value)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_denylogin @loginame=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
					query.Add(stringBuilder.ToString());
				}
				else
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_grantlogin @loginame=N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
					query.Add(stringBuilder.ToString());
				}
			}
		}
		else if (base.Properties.Get("DenyWindowsLogin").Dirty)
		{
			throw new SmoException(ExceptionTemplatesImpl.DenyLoginModiNotForStandardLogin);
		}
		stringBuilder.Length = 0;
		GetLanguageDDL(stringBuilder, bSuppressDirtyCheck: false);
		if (stringBuilder.Length > 0)
		{
			query.Add(stringBuilder.ToString());
		}
	}

	protected override void PostCreate()
	{
		SetMustChangePassword();
		InitVariables();
	}

	internal void GetLanguageDDL(StringBuilder statement, bool bSuppressDirtyCheck)
	{
		Property property = base.Properties.Get("Language");
		if (property.Value != null && (bSuppressDirtyCheck || property.Dirty))
		{
			statement.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_defaultlanguage @loginame = N'{0}'", new object[1] { SqlSmoObject.SqlString(Name) });
			if (0 < ((string)property.Value).Length)
			{
				statement.AppendFormat(SmoApplication.DefaultCulture, ", @language = N'{0}'", new object[1] { SqlSmoObject.SqlString((string)property.Value) });
			}
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

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			if (DatabaseEngineType.SqlAzureDatabase != sp.TargetDatabaseEngineType && sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LOGIN90, new object[2]
				{
					"",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP LOGIN {0}", new object[1] { FormatFullNameForScripting(sp) });
		}
		else
		{
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_LOGIN80, new object[2]
				{
					"",
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
				stringBuilder.Append(sp.NewLine);
			}
			LoginType loginType = (LoginType)base.Properties["LoginType"].Value;
			if (loginType == LoginType.WindowsUser || loginType == LoginType.WindowsGroup)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_revokelogin @loginame = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC master.dbo.sp_droplogin @loginame = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
			}
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public StringCollection ListMembers()
	{
		try
		{
			CheckObjectState();
			StringCollection stringCollection = new StringCollection();
			if (!base.IsDesignMode)
			{
				Request request = new Request(string.Format(SmoApplication.DefaultCulture, "Server[@Name='{0}']/Role", new object[1] { Urn.EscapeString(GetServerName()) }));
				request.Fields = new string[1] { "Name" };
				foreach (DataRow row in ExecutionManager.GetEnumeratorData(request).Rows)
				{
					request = new Request(string.Format(SmoApplication.DefaultCulture, "Server[@Name='{0}']/Role[@Name='{1}']/Member[@Name='{2}']", new object[3]
					{
						Urn.EscapeString(GetServerName()),
						Urn.EscapeString((string)row["Name"]),
						Urn.EscapeString(Name)
					}));
					request.Fields = new string[1] { "Name" };
					if (ExecutionManager.GetEnumeratorData(request).Rows.Count != 0)
					{
						stringCollection.Add((string)row["Name"]);
					}
				}
			}
			else
			{
				stringCollection = EnumRolesForLogin(Name);
			}
			return stringCollection;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumMembers, this, ex);
		}
	}

	public DatabaseMapping[] EnumDatabaseMappings()
	{
		try
		{
			CheckObjectState();
			DatabaseMapping[] array = null;
			if (!base.IsDesignMode)
			{
				Urn urn = base.Urn.ToString() + "/DatabaseMapping";
				Request req = new Request(urn);
				DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
				if (enumeratorData.Rows.Count <= 0)
				{
					return null;
				}
				array = new DatabaseMapping[enumeratorData.Rows.Count];
				for (int i = 0; i < enumeratorData.Rows.Count; i++)
				{
					DataRow dataRow = enumeratorData.Rows[i];
					array[i] = new DatabaseMapping(Convert.ToString(dataRow["LoginName"], SmoApplication.DefaultCulture), Convert.ToString(dataRow["DBName"], SmoApplication.DefaultCulture), Convert.ToString(dataRow["UserName"], SmoApplication.DefaultCulture));
				}
			}
			return array;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.EnumDatabaseMappings, this, ex);
		}
	}

	public bool IsMember(string role)
	{
		try
		{
			if (role == null)
			{
				throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("role"));
			}
			CheckObjectState();
			if (base.IsDesignMode)
			{
				return EnumLoginsForRole(role).Contains(Name);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Server[@Name='");
			stringBuilder.Append(Urn.EscapeString(base.ParentColl.ParentInstance.InternalName));
			stringBuilder.Append("']/Role[@Name='");
			stringBuilder.Append(Urn.EscapeString(role));
			stringBuilder.Append("']/Member[@Name='");
			stringBuilder.Append(Urn.EscapeString(Name));
			stringBuilder.Append("']");
			Request req = new Request(stringBuilder.ToString());
			return ExecutionManager.GetEnumeratorData(req).Rows.Count > 0;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.IsMember, this, ex);
		}
	}

	public string GetDatabaseUser(string databaseName)
	{
		try
		{
			if (databaseName == null)
			{
				throw new ArgumentNullException("databaseName");
			}
			CheckObjectState();
			User user = Parent.Databases[databaseName].Users.Cast<User>().FirstOrDefault((User u) => u.Login == Name);
			return (user == null) ? string.Empty : user.Name;
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.GetDatabaseUser, this, ex);
		}
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void AddToRole(string role)
	{
		try
		{
			CheckObjectState();
			if (base.IsDesignMode)
			{
				AddLoginToRole(role, Name);
			}
			else
			{
				ExecutionManager.ExecuteNonQuery(GetAddToRoleDdl(role, new ScriptingPreferences(this)));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddToRole, this, ex);
		}
	}

	private string GetAddToRoleDdl(string role, ScriptingPreferences sp)
	{
		if (role == null)
		{
			throw new ArgumentNullException("role");
		}
		if (VersionUtils.IsTargetServerVersionSQl11OrLater(sp.TargetServerVersionInternal))
		{
			return string.Format(SmoApplication.DefaultCulture, "ALTER SERVER ROLE {0} ADD MEMBER {1}", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(role),
				SqlSmoObject.MakeSqlBraket(Name)
			});
		}
		string text = ((sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90) ? "master.dbo" : "sys");
		return string.Format(SmoApplication.DefaultCulture, "EXEC {0}.sp_addsrvrolemember @loginame = {1}, @rolename = {2}", new object[3]
		{
			text,
			SqlSmoObject.MakeSqlString(Name),
			SqlSmoObject.MakeSqlString(role)
		});
	}

	public void Rename(string newname)
	{
		RenameImpl(newname);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		if (base.ServerVersion.Major < 9)
		{
			throw new InvalidVersionSmoOperationException(base.ServerVersion);
		}
		AddDatabaseContext(renameQuery, sp);
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER LOGIN {0} WITH NAME={1}", new object[2]
		{
			FormatFullNameForScripting(new ScriptingPreferences()),
			SqlSmoObject.MakeSqlBraket(newName)
		}));
	}

	public DataTable EnumAgentProxyAccounts()
	{
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC msdb.dbo.sp_enum_login_for_proxy @name={0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
		stringCollection.Add(stringBuilder.ToString());
		return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
	}

	public void Disable()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			if (!base.IsDesignMode)
			{
				string cmd = string.Format(SmoApplication.DefaultCulture, "ALTER LOGIN {0} DISABLE", new object[1] { FormatFullNameForScripting(new ScriptingPreferences()) });
				ExecutionManager.ExecuteNonQuery(cmd);
			}
			int index = base.Properties.LookupID("IsDisabled", PropertyAccessPurpose.Write);
			base.Properties.SetValue(index, true);
			base.Properties.SetRetrieved(index, val: true);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.LoginDisable, this, ex);
		}
	}

	public void Enable()
	{
		try
		{
			ThrowIfBelowVersion90();
			CheckObjectState();
			if (!base.IsDesignMode)
			{
				string cmd = string.Format(SmoApplication.DefaultCulture, "ALTER LOGIN {0} ENABLE", new object[1] { FormatFullNameForScripting(new ScriptingPreferences()) });
				ExecutionManager.ExecuteNonQuery(cmd);
			}
			int index = base.Properties.LookupID("IsDisabled", PropertyAccessPurpose.Write);
			base.Properties.SetValue(index, false);
			base.Properties.SetRetrieved(index, val: true);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.LoginEnable, this, ex);
		}
	}

	public override void Refresh()
	{
		base.Refresh();
	}

	public void AddCredential(string credentialName)
	{
		try
		{
			ThrowIfBelowVersion100();
			if (base.IsDesignMode)
			{
				if (base.State != SqlSmoState.Existing)
				{
					throw new InvalidSmoOperationException("AddCredential", base.State);
				}
				if (credentialCollection.Contains(credentialName))
				{
					throw new SmoException(ExceptionTemplatesImpl.CannotAddObject("Credential", credentialName));
				}
				credentialCollection.Add(credentialName);
			}
			else
			{
				ExecutionManager.ExecuteNonQuery(ScriptAddDropCredential(add: true, credentialName));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.AddCredential, this, ex);
		}
	}

	public void DropCredential(string credentialName)
	{
		try
		{
			ThrowIfBelowVersion100();
			if (base.IsDesignMode)
			{
				if (base.State != SqlSmoState.Existing)
				{
					throw new InvalidSmoOperationException("DropCredential", base.State);
				}
				if (!credentialCollection.Contains(credentialName))
				{
					throw new MissingObjectException(ExceptionTemplatesImpl.ObjectDoesNotExist("Credential", credentialName));
				}
				credentialCollection.Remove(credentialName);
			}
			else
			{
				ExecutionManager.ExecuteNonQuery(ScriptAddDropCredential(add: false, credentialName));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.DropCredential, this, ex);
		}
	}

	private string ScriptAddDropCredential(bool add, string credentialName)
	{
		return string.Format(SmoApplication.DefaultCulture, "ALTER LOGIN {0} {1} CREDENTIAL {2}", new object[3]
		{
			FullQualifiedName,
			add ? "ADD" : "DROP",
			SqlSmoObject.MakeSqlBraket(credentialName)
		});
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[5] { "LoginType", "DefaultDatabase", "Sid", "Language", "DenyWindowsLogin" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80 && base.ServerVersion.Major > 8 && !SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
		{
			base.AddScriptPermission(query, sp);
		}
	}
}

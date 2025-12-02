using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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

[PhysicalFacet]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class User : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IAlterable, IRenamable, IExtendedProperties, IScriptable, IUserOptions, IDmfFacet
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 9, 9, 13, 14, 14, 16, 16, 16, 16, 16 };

		private static int[] cloudVersionCount = new int[3] { 10, 10, 13 };

		private static int sqlDwPropertyCount = 12;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("AuthenticationType", expensive: false, readOnly: true, typeof(AuthenticationType)),
			new StaticMetadata("Certificate", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasDBAccess", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Login", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("LoginType", expensive: false, readOnly: true, typeof(LoginType)),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("UserType", expensive: false, readOnly: false, typeof(UserType))
		};

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[13]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("HasDBAccess", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Login", expensive: true, readOnly: false, typeof(string)),
			new StaticMetadata("LoginType", expensive: false, readOnly: true, typeof(LoginType)),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("UserType", expensive: true, readOnly: false, typeof(UserType)),
			new StaticMetadata("AsymmetricKey", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("AuthenticationType", expensive: false, readOnly: true, typeof(AuthenticationType)),
			new StaticMetadata("Certificate", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[16]
		{
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("HasDBAccess", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("Login", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("LoginType", expensive: false, readOnly: true, typeof(LoginType)),
			new StaticMetadata("Sid", expensive: false, readOnly: true, typeof(byte[])),
			new StaticMetadata("UserType", expensive: false, readOnly: false, typeof(UserType)),
			new StaticMetadata("AsymmetricKey", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("AuthenticationType", expensive: false, readOnly: true, typeof(AuthenticationType)),
			new StaticMetadata("Certificate", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("DefaultSchema", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
			new StaticMetadata("DefaultLanguageLcid", expensive: false, readOnly: false, typeof(int)),
			new StaticMetadata("DefaultLanguageName", expensive: false, readOnly: false, typeof(string))
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
						"AuthenticationType" => 0, 
						"Certificate" => 1, 
						"CreateDate" => 2, 
						"DateLastModified" => 3, 
						"DefaultSchema" => 4, 
						"HasDBAccess" => 5, 
						"ID" => 6, 
						"IsSystemObject" => 7, 
						"Login" => 8, 
						"LoginType" => 9, 
						"Sid" => 10, 
						"UserType" => 11, 
						_ => -1, 
					};
				}
				return propertyName switch
				{
					"CreateDate" => 0, 
					"DateLastModified" => 1, 
					"DefaultSchema" => 2, 
					"HasDBAccess" => 3, 
					"ID" => 4, 
					"IsSystemObject" => 5, 
					"Login" => 6, 
					"LoginType" => 7, 
					"Sid" => 8, 
					"UserType" => 9, 
					"AsymmetricKey" => 10, 
					"AuthenticationType" => 11, 
					"Certificate" => 12, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"HasDBAccess" => 2, 
				"ID" => 3, 
				"IsSystemObject" => 4, 
				"Login" => 5, 
				"LoginType" => 6, 
				"Sid" => 7, 
				"UserType" => 8, 
				"AsymmetricKey" => 9, 
				"AuthenticationType" => 10, 
				"Certificate" => 11, 
				"DefaultSchema" => 12, 
				"PolicyHealthState" => 13, 
				"DefaultLanguageLcid" => 14, 
				"DefaultLanguageName" => 15, 
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

	private UserEvents events;

	private SqlSecureString password;

	private DefaultLanguage defaultLanguageObj;

	private bool userContainmentInProgress;

	private bool isDefaultLanguageModified;

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

	[SfcReference(typeof(AsymmetricKey), "Server[@Name = '{0}']/Database[@Name = '{1}']/AsymmetricKey[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "AsymmetricKey" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public AuthenticationType AuthenticationType => (AuthenticationType)base.Properties.GetValueWithNullReplacement("AuthenticationType");

	[SfcReference(typeof(Certificate), "Server[@Name = '{0}']/Database[@Name = '{1}']/Certificate[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Certificate" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
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

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(Schema), "Server[@Name = '{0}']/Database[@Name = '{1}']/Schema[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "DefaultSchema" })]
	[CLSCompliant(false)]
	public string DefaultSchema
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("DefaultSchema");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DefaultSchema", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasDBAccess => (bool)base.Properties.GetValueWithNullReplacement("HasDBAccess");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase, "false")]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[CLSCompliant(false)]
	[SfcReference(typeof(Login), "Server[@Name = '{0}']/Login[@Name = '{1}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Login" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string Login
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Login");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Login", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public LoginType LoginType => (LoginType)base.Properties.GetValueWithNullReplacement("LoginType");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public byte[] Sid => (byte[])base.Properties.GetValueWithNullReplacement("Sid");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public UserType UserType
	{
		get
		{
			return (UserType)base.Properties.GetValueWithNullReplacement("UserType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("UserType", value);
		}
	}

	public UserEvents Events
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
				events = new UserEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "User";

	[SfcKey(0)]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
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
	public DefaultLanguage DefaultLanguage
	{
		get
		{
			ThrowIfCloudProp("DefaultLanguage");
			ThrowIfBelowVersion110Prop("DefaultLanguage");
			if (defaultLanguageObj == null)
			{
				defaultLanguageObj = new DefaultLanguage(this, "DefaultLanguage");
			}
			return defaultLanguageObj;
		}
		internal set
		{
			ThrowIfCloudProp("DefaultLanguage");
			ThrowIfBelowVersion110Prop("DefaultLanguage");
			if (value.IsProperlyInitialized())
			{
				defaultLanguageObj = value;
			}
			else
			{
				defaultLanguageObj = value.Copy(this, "DefaultLanguage");
			}
		}
	}

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(ExtendedProperty))]
	public ExtendedPropertyCollection ExtendedProperties
	{
		get
		{
			ThrowIfBelowVersion80();
			CheckObjectState();
			if (m_ExtendedProperties == null)
			{
				m_ExtendedProperties = new ExtendedPropertyCollection(this);
			}
			return m_ExtendedProperties;
		}
	}

	public User()
	{
	}

	public User(Database database, string name)
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
		return new string[4] { "AsymmetricKey", "Certificate", "Login", "UserType" };
	}

	internal override object GetPropertyDefaultValue(string propname)
	{
		string text;
		if ((text = propname) != null && text == "IsSystemObject")
		{
			return false;
		}
		return base.GetPropertyDefaultValue(propname);
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

	public Urn[] EnumOwnedObjects()
	{
		return PermissionWorker.EnumOwnedObjects(this);
	}

	internal User(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		InitVariables();
	}

	private void InitVariables()
	{
		password = null;
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
		if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType && 8 == base.ServerVersion.Major)
		{
			ScriptDropFrom80ToCloud(dropQuery, sp);
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && base.ServerVersion.Major >= 9)
		{
			if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_USER90, new object[2]
				{
					"",
					SqlSmoObject.SqlString(Name)
				});
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP USER {0}{1}", new object[2]
			{
				(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
				FormatFullNameForScripting(sp)
			});
		}
		else
		{
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_USER80, new object[2]
				{
					"",
					SqlSmoObject.SqlString(Name)
				});
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_revokedbaccess {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	private void ScriptDropFrom80ToCloud(StringCollection dropQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "if {0} <> 'guest'", new object[1] { SqlSmoObject.MakeSqlString(Name) });
		stringBuilder.AppendLine();
		stringBuilder.Append(Scripts.BEGIN);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.IF_SCHEMA_EXISTS_WITH_GIVEN_OWNER, new object[2]
		{
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(Name)
		});
		stringBuilder.Append(Scripts.BEGIN);
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "DROP SCHEMA {0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC sys.sp_executesql N'{0}'", new object[1] { SqlSmoObject.SqlString(stringBuilder2.ToString()) });
		stringBuilder.AppendLine();
		stringBuilder.Append(Scripts.END);
		stringBuilder.AppendLine();
		stringBuilder.Append(Scripts.END);
		dropQuery.Add(stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_USER90, new object[2]
			{
				"",
				SqlSmoObject.SqlString(Name)
			});
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP USER {0}", new object[1] { FormatFullNameForScripting(sp) });
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Create()
	{
		CreateImpl();
	}

	public void Create(string password)
	{
		try
		{
			ThrowIfBelowVersion110();
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			Create(new SqlSecureString(password));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
	}

	public void Create(SecureString password)
	{
		try
		{
			ThrowIfBelowVersion110();
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.password = password;
			Create();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, ex);
		}
		finally
		{
			InitVariables();
		}
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		ValidateVersionAndEngineTypeForScripting(sp);
		ValidateBeforeScriptCreate();
		if (DatabaseEngineType.SqlAzureDatabase == sp.TargetDatabaseEngineType && 8 == base.ServerVersion.Major)
		{
			ScriptCreateFrom80ToCloud(createQuery, sp);
			return;
		}
		bool suppressDirtyCheck = sp.SuppressDirtyCheck;
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && base.ServerVersion.Major >= 9)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			bool optionAdded = false;
			Property propertyOptional = GetPropertyOptional("Login");
			UserType userType = GetPropValueOptional("UserType", UserType.SqlLogin);
			if (userType == UserType.SqlLogin && propertyOptional != null && (string)propertyOptional.Value == "\\")
			{
				userType = UserType.NoLogin;
			}
			if (sp.IncludeScripts.ExistenceCheck && (sp.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase || userType != UserType.SqlLogin))
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_USER90, new object[2]
				{
					"NOT",
					SqlSmoObject.SqlString(Name)
				});
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.Append("CREATE USER ");
			stringBuilder.Append(FormatFullNameForScripting(sp));
			if (userType == UserType.Certificate || userType == UserType.AsymmetricKey)
			{
				if (SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType))
				{
					throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "UserType", UserType.ToString(), LocalizableResources.EngineCloud));
				}
				string propValueOptional = GetPropValueOptional("Login", string.Empty);
				if (propValueOptional != null && propValueOptional.Length > 0)
				{
					userType = UserType.SqlLogin;
				}
			}
			if (userType == UserType.External && (!SqlSmoObject.IsCloudAtSrcOrDest(DatabaseEngineType, sp.TargetDatabaseEngineType) || 12 > base.ServerVersion.Major))
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "UserType", UserType.ToString(), SqlSmoObject.GetSqlServerName(sp)));
			}
			string text = null;
			string text2 = null;
			bool flag = false;
			switch (userType)
			{
			case UserType.SqlLogin:
				text = "Login";
				text2 = "LOGIN";
				flag = true;
				break;
			case UserType.Certificate:
				text = "Certificate";
				text2 = "CERTIFICATE";
				break;
			case UserType.AsymmetricKey:
				text = "AsymmetricKey";
				text2 = "ASYMMETRIC KEY";
				break;
			case UserType.NoLogin:
				text = null;
				text2 = " WITHOUT LOGIN";
				break;
			case UserType.External:
				text = null;
				text2 = " EXTERNAL PROVIDER";
				break;
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("UserType"));
			}
			string text3 = null;
			switch (userType)
			{
			case UserType.NoLogin:
				if (base.State == SqlSmoState.Creating || !IsSystemObject)
				{
					stringBuilder.Append(text2);
				}
				break;
			case UserType.External:
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FROM {0} ", new object[1] { text2 });
				break;
			default:
			{
				text3 = (string)GetPropValueOptional(text);
				if (text3 != null && text3.Length > 0)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FOR {0} {1}", new object[2]
					{
						text2,
						SqlSmoObject.MakeSqlBraket(text3)
					});
				}
				else if (!flag)
				{
					throw new PropertyNotSetException(text3);
				}
				AuthenticationType authenticationType = AuthenticationType.Instance;
				if (IsSupportedProperty("AuthenticationType"))
				{
					authenticationType = GetPropValueOptional("AuthenticationType", AuthenticationType.Instance);
				}
				if (string.IsNullOrEmpty(text3) && userType == UserType.SqlLogin && ((base.State == SqlSmoState.Creating && password != null) || (base.State != SqlSmoState.Creating && authenticationType == AuthenticationType.Database)))
				{
					AddPasswordOptions(sp, stringBuilder2, password, null, ref optionAdded);
				}
				break;
			}
			}
			AddDefaultLanguageOptionToScript(stringBuilder2, sp, ref optionAdded);
			text3 = (string)GetPropValueOptional("DefaultSchema");
			LoginType propValueOptional2 = GetPropValueOptional("LoginType", LoginType.SqlLogin);
			if (text3 != null && text3.Length > 0 && (propValueOptional2 != LoginType.WindowsGroup || base.ServerVersion.Major >= 11))
			{
				AddComma(stringBuilder2, ref optionAdded);
				stringBuilder2.Append("DEFAULT_SCHEMA=");
				stringBuilder2.Append(SqlSmoObject.MakeSqlBraket(text3));
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder.Append(" WITH ");
				stringBuilder.Append(stringBuilder2.ToString());
			}
		}
		else
		{
			UserType propValueOptional3 = GetPropValueOptional("UserType", UserType.SqlLogin);
			if (propValueOptional3 == UserType.Certificate || propValueOptional3 == UserType.AsymmetricKey || propValueOptional3 == UserType.External)
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "UserType", UserType.ToString(), SqlSmoObject.GetSqlServerName(sp)));
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_USER80, new object[2]
				{
					"NOT",
					SqlSmoObject.SqlString(Name)
				});
				stringBuilder.Append(sp.NewLine);
			}
			if (base.StringComparer.Compare(Name, "guest") == 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_grantdbaccess @loginame = {0}", new object[1] { FormatFullNameForScripting(sp, nameIsIndentifier: false) });
			}
			else
			{
				Property property = base.Properties["Login"];
				if (property.Value == null || property.Value.ToString() == string.Empty || (!suppressDirtyCheck && !property.Dirty))
				{
					throw new SmoException(ExceptionTemplatesImpl.UsersWithoutLoginsDownLevel(SqlSmoObject.GetSqlServerName(sp)));
				}
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_grantdbaccess @loginame = {0}, @name_in_db = {1}", new object[2]
				{
					SqlSmoObject.MakeSqlString((string)property.Value),
					FormatFullNameForScripting(sp, nameIsIndentifier: false)
				});
			}
		}
		createQuery.Add(stringBuilder.ToString());
		if (!sp.ScriptForCreateDrop && sp.IncludeScripts.Associations)
		{
			ScriptAssociations(createQuery, sp);
		}
	}

	private void AddDefaultLanguageOptionToScript(StringBuilder sbOption, ScriptingPreferences sp, ref bool optionAdded)
	{
		Database database = base.ParentColl.ParentInstance as Database;
		if (!database.IsSupportedProperty("ContainmentType") || !IsSupportedProperty("DefaultLanguageLcid", sp))
		{
			return;
		}
		if (database.GetPropValueOptional("ContainmentType", ContainmentType.None) != ContainmentType.None)
		{
			bool flag = false;
			string text = string.Empty;
			if (GetPropertyOptional("DefaultLanguageName").Dirty)
			{
				Property propertyOptional = GetPropertyOptional("DefaultLanguageName");
				if (propertyOptional.Dirty || !sp.ForDirectExecution)
				{
					string text2 = propertyOptional.Value as string;
					if (!string.IsNullOrEmpty(text2))
					{
						text = text2;
					}
					else if (propertyOptional.Dirty && sp.ScriptForAlter)
					{
						text = "NONE";
					}
					flag = true;
				}
			}
			if (!flag)
			{
				Property propertyOptional2 = GetPropertyOptional("DefaultLanguageLcid");
				if (!propertyOptional2.IsNull && (propertyOptional2.Dirty || !sp.ForDirectExecution))
				{
					if ((int)propertyOptional2.Value >= 0)
					{
						text = propertyOptional2.Value.ToString();
					}
					else if (propertyOptional2.Dirty && sp.ScriptForAlter)
					{
						text = "NONE";
					}
				}
			}
			if (text.Length > 0)
			{
				AddComma(sbOption, ref optionAdded);
				sbOption.Append("DEFAULT_LANGUAGE=");
				if (flag)
				{
					sbOption.Append(SqlSmoObject.MakeSqlBraket(text));
				}
				else
				{
					sbOption.Append(text);
				}
			}
		}
		else
		{
			ValidateDefaultLanguageNotDirty();
		}
	}

	private void ValidateVersionAndEngineTypeForScripting(ScriptingPreferences sp)
	{
		AuthenticationType authenticationType = AuthenticationType.Instance;
		if (IsSupportedProperty("AuthenticationType"))
		{
			authenticationType = GetPropValueOptional("AuthenticationType", AuthenticationType.Instance);
		}
		if ((base.State == SqlSmoState.Creating && password != null) || (base.State != SqlSmoState.Creating && authenticationType == AuthenticationType.Database))
		{
			SqlSmoObject.ThrowIfBelowVersion110(sp.TargetServerVersionInternal);
		}
	}

	private void ValidateBeforeScriptCreate()
	{
		UserType propValueOptional = GetPropValueOptional("UserType", UserType.SqlLogin);
		string value = (string)GetPropValueOptional("Login");
		if (password != null && (propValueOptional == UserType.AsymmetricKey || propValueOptional == UserType.Certificate || propValueOptional == UserType.NoLogin || (propValueOptional == UserType.SqlLogin && !string.IsNullOrEmpty(value))))
		{
			throw new SmoException(ExceptionTemplatesImpl.PasswordOnlyForDatabaseAuthenticatedNonWindowsUser);
		}
		if (IsSupportedProperty("DefaultLanguageLcid"))
		{
			if (propValueOptional == UserType.AsymmetricKey || propValueOptional == UserType.Certificate)
			{
				ValidateDefaultLanguageNotDirty();
			}
			else
			{
				DefaultLanguage.VerifyBothLcidAndNameNotDirty(isLanguageValueNoneAllowed: true);
			}
		}
	}

	public void ChangePassword(string newPassword)
	{
		try
		{
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			ChangePassword(new SqlSecureString(newPassword));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	public void ChangePassword(SecureString newPassword)
	{
		try
		{
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			ExecuteUserPasswordOptions(newPassword, null);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	public void ChangePassword(string oldPassword, string newPassword)
	{
		try
		{
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			if (oldPassword == null)
			{
				throw new ArgumentNullException("oldPassword");
			}
			ChangePassword(new SqlSecureString(oldPassword), new SqlSecureString(newPassword));
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	public void ChangePassword(SecureString oldPassword, SecureString newPassword)
	{
		try
		{
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			if (oldPassword == null)
			{
				throw new ArgumentNullException("oldPassword");
			}
			ExecuteUserPasswordOptions(newPassword, oldPassword);
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.ChangePassword, this, ex);
		}
	}

	private void ExecuteUserPasswordOptions(SqlSecureString password, SqlSecureString oldPassword)
	{
		CheckObjectState();
		ThrowIfBelowVersion110();
		ThrowIfCloud();
		if (UserType != UserType.SqlLogin || GetPropValueOptional("AuthenticationType", AuthenticationType.Instance) != AuthenticationType.Database)
		{
			throw new SmoException(ExceptionTemplatesImpl.CannotChangePasswordForUser);
		}
		StringCollection stringCollection = new StringCollection();
		AddDatabaseContext(stringCollection);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("ALTER USER ");
		ScriptingPreferences scriptingPreferences = new ScriptingPreferences(this);
		scriptingPreferences.ForDirectExecution = true;
		stringBuilder.Append(FormatFullNameForScripting(scriptingPreferences));
		StringBuilder stringBuilder2 = new StringBuilder();
		bool optionAdded = false;
		AddPasswordOptions(scriptingPreferences, stringBuilder2, password, oldPassword, ref optionAdded);
		if (stringBuilder2.Length > 0)
		{
			stringBuilder.Append(" WITH ");
			stringBuilder.Append(stringBuilder2.ToString());
		}
		stringCollection.Add(stringBuilder.ToString());
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	protected override void ExecuteRenameQuery(string newName)
	{
		if (!userContainmentInProgress)
		{
			base.ExecuteRenameQuery(newName);
		}
	}

	public void MakeContained(bool copyLoginName, bool disableLogin)
	{
		try
		{
			CheckObjectState();
			ThrowIfBelowVersion110();
			ThrowIfCloud();
			userContainmentInProgress = true;
			if (UserType != UserType.SqlLogin || GetPropValueOptional("AuthenticationType", AuthenticationType.Instance) != AuthenticationType.Instance)
			{
				throw new SmoException(ExceptionTemplatesImpl.CannotCopyPasswordToUser);
			}
			string login = Login;
			ExecutionManager.ExecuteNonQuery(GetMakeContainedScript(copyLoginName, disableLogin));
			if (disableLogin && !ExecutionManager.Recording)
			{
				SimpleObjectKey simpleObjectKey = new SimpleObjectKey(login);
				if (Parent.Parent.Logins.NoFaultLookup(simpleObjectKey) is Login login2)
				{
					Property propertyOptional = login2.GetPropertyOptional("IsDisabled");
					propertyOptional.SetValue(true);
					propertyOptional.SetRetrieved(retrieved: true);
				}
			}
			if (copyLoginName)
			{
				RenameImpl(login);
			}
			Refresh();
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			throw new FailedOperationException(ExceptionTemplatesImpl.MakeContained, this, ex);
		}
		finally
		{
			userContainmentInProgress = false;
		}
	}

	private StringCollection GetMakeContainedScript(bool copyLoginName, bool disableLogin)
	{
		StringCollection stringCollection = new StringCollection();
		AddDatabaseContext(stringCollection);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC sys.sp_migrate_user_to_contained @username = {0}, @rename = N'{1}', @disablelogin = N'{2}'", new object[3]
		{
			SqlSmoObject.MakeSqlString(Name),
			copyLoginName ? "copy_login_name" : "keep_name",
			disableLogin ? "disable_login" : "do_not_disable_login"
		});
		stringCollection.Add(stringBuilder.ToString());
		return stringCollection;
	}

	private void ScriptCreateFrom80ToCloud(StringCollection createQuery, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (sp.IncludeScripts.Header)
		{
			createQuery.Add(ExceptionTemplates.IncludeHeader(UrnSuffix, FormatFullNameForScripting(sp), DateTime.Now.ToString(GetDbCulture())));
		}
		stringBuilder.Append("CREATE USER ");
		stringBuilder.Append(FormatFullNameForScripting(sp));
		string text = (string)GetPropValue("Login");
		if (!string.IsNullOrEmpty(text))
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " FOR LOGIN {0}", new object[1] { SqlSmoObject.MakeSqlBraket(text) });
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, " WITH DEFAULT_SCHEMA={0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
		createQuery.Add(stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.IF_SCHEMA_NOT_EXISTS_WITH_GIVEN_OWNER, new object[2]
		{
			SqlSmoObject.SqlString(Name),
			SqlSmoObject.SqlString(Name)
		});
		stringBuilder.Append(Scripts.BEGIN);
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendFormat(SmoApplication.DefaultCulture, "CREATE SCHEMA {0} AUTHORIZATION {0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC sys.sp_executesql N'{0}'", new object[1] { SqlSmoObject.SqlString(stringBuilder2.ToString()) });
		stringBuilder.AppendLine();
		stringBuilder.Append(Scripts.END);
		createQuery.Add(stringBuilder.ToString());
		if (!sp.ScriptForCreateDrop && sp.IncludeScripts.Associations)
		{
			ScriptAssociations(createQuery, sp);
		}
	}

	internal override void ScriptAssociations(StringCollection createQuery, ScriptingPreferences sp)
	{
		StringCollection stringCollection = EnumRoles();
		StringEnumerator enumerator = stringCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				createQuery.Add(ScriptAddToRole(current, sp));
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

	private void AddPasswordOptions(ScriptingPreferences sp, StringBuilder sb, SqlSecureString password, SqlSecureString oldPassword, ref bool optionAdded)
	{
		if (!VersionUtils.IsSql11OrLater(sp.TargetServerVersionInternal, base.ServerVersion))
		{
			return;
		}
		if (null != password)
		{
			AddComma(sb, ref optionAdded);
			sb.Append("PASSWORD=");
			sb.Append(SqlSmoObject.MakeSqlString((string)password));
			if (null != oldPassword)
			{
				sb.Append(" OLD_PASSWORD=");
				sb.Append(SqlSmoObject.MakeSqlString((string)oldPassword));
			}
		}
		else
		{
			if (sp == null || sp.ForDirectExecution)
			{
				throw new ArgumentNullException("password");
			}
			AddComma(sb, ref optionAdded);
			sb.Append("PASSWORD=");
			sb.Append(SqlSmoObject.MakeSqlString(SecurityUtils.GenerateRandomPassword()));
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

	protected override void PostCreate()
	{
		CheckObjectState();
		InitVariables();
		isDefaultLanguageModified = IsDefaultLanguageModified();
		base.PostCreate();
	}

	protected override void PostAlter()
	{
		CheckObjectState();
		isDefaultLanguageModified = IsDefaultLanguageModified();
		base.PostAlter();
	}

	private bool IsDefaultLanguageModified()
	{
		if (IsSupportedProperty("DefaultLanguageLcid"))
		{
			StringCollection stringCollection = new StringCollection();
			stringCollection.Add("DefaultLanguageName");
			stringCollection.Add("DefaultLanguageLcid");
			return base.Properties.ArePropertiesDirty(stringCollection);
		}
		return false;
	}

	protected override void CleanObject()
	{
		base.CleanObject();
		if (isDefaultLanguageModified)
		{
			Property property = base.Properties.Get("DefaultLanguageName");
			Property property2 = base.Properties.Get("DefaultLanguageLcid");
			property.SetRetrieved(retrieved: false);
			property2.SetRetrieved(retrieved: false);
			propertyBagState = PropertyBagState.Lazy;
		}
		isDefaultLanguageModified = false;
	}

	public void Alter()
	{
		AlterImpl();
	}

	private void ValidateAlterInputs()
	{
		if (base.State != SqlSmoState.Creating)
		{
			UserType propValueOptional = GetPropValueOptional("UserType", UserType.SqlLogin);
			AuthenticationType authenticationType = AuthenticationType.Instance;
			if (IsSupportedProperty("AuthenticationType"))
			{
				authenticationType = GetPropValueOptional("AuthenticationType", AuthenticationType.Instance);
			}
			if (IsSupportedProperty("DefaultLanguageLcid") && (propValueOptional != UserType.SqlLogin || (authenticationType != AuthenticationType.Database && authenticationType != AuthenticationType.Windows)))
			{
				ValidateDefaultLanguageNotDirty();
			}
			else if (IsSupportedProperty("DefaultLanguageLcid"))
			{
				DefaultLanguage.VerifyBothLcidAndNameNotDirty(isLanguageValueNoneAllowed: true);
			}
		}
	}

	private void ValidateDefaultLanguageNotDirty()
	{
		Property propertyOptional = GetPropertyOptional("DefaultLanguageLcid");
		Property propertyOptional2 = GetPropertyOptional("DefaultLanguageName");
		if (propertyOptional.Dirty || propertyOptional2.Dirty)
		{
			throw new SmoException(ExceptionTemplatesImpl.DefaultLanguageOnlyForDatabaseAuthenticatedUser);
		}
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ValidateVersionAndEngineTypeForScripting(sp);
		if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90 || Parent.Parent.ConnectionContext.ServerVersion.Major < 9)
		{
			return;
		}
		ValidateAlterInputs();
		StringBuilder stringBuilder = new StringBuilder();
		bool bStuffAdded = false;
		Property propertyOptional = GetPropertyOptional("DefaultSchema");
		if (!propertyOptional.IsNull && propertyOptional.Dirty && propertyOptional.Value.ToString().Length > 0)
		{
			AddComma(stringBuilder, ref bStuffAdded);
			stringBuilder.Append("DEFAULT_SCHEMA=");
			stringBuilder.Append(SqlSmoObject.MakeSqlBraket(propertyOptional.Value.ToString()));
		}
		else
		{
			LoginType propValueOptional = GetPropValueOptional("LoginType", LoginType.SqlLogin);
			if (propValueOptional == LoginType.WindowsGroup && base.ServerVersion.Major >= 11)
			{
				query.Add(string.Format(SmoApplication.DefaultCulture, "ALTER USER {0} WITH DEFAULT_SCHEMA=NULL", new object[1] { FormatFullNameForScripting(sp) }));
			}
		}
		AddDefaultLanguageOptionToScript(stringBuilder, sp, ref bStuffAdded);
		StringBuilder stringBuilder2 = new StringBuilder();
		if (stringBuilder.Length > 0)
		{
			ScriptIncludeHeaders(stringBuilder2, sp, UrnSuffix);
			stringBuilder2.Append("ALTER USER ");
			stringBuilder2.Append(FormatFullNameForScripting(sp));
			stringBuilder2.Append(" WITH ");
			stringBuilder2.Append(stringBuilder.ToString());
			query.Add(stringBuilder2.ToString());
		}
	}

	public bool IsMember(string role)
	{
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(base.ParentColl.ParentInstance.Urn);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "/Role[@Name='{0}']/Member[@Name='{1}']", new object[2]
		{
			Urn.EscapeString(role),
			Urn.EscapeString(Name)
		});
		Request req = new Request(stringBuilder.ToString());
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
		return enumeratorData.Rows.Count > 0;
	}

	public StringCollection EnumRoles()
	{
		CheckObjectState();
		StringCollection stringCollection = new StringCollection();
		StringBuilder stringBuilder = new StringBuilder(base.ParentColl.ParentInstance.Urn);
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "/Role/Member[@Name='{0}']", new object[1] { Urn.EscapeString(Name) });
		Request request = new Request(stringBuilder.ToString(), new string[0]);
		request.ParentPropertiesRequests = new PropertiesRequest[1]
		{
			new PropertiesRequest(new string[1] { "Name" })
		};
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(request);
		if (enumeratorData.Rows.Count == 0)
		{
			return stringCollection;
		}
		foreach (DataRow row in enumeratorData.Rows)
		{
			stringCollection.Add(Convert.ToString(row[0], SmoApplication.DefaultCulture));
		}
		return stringCollection;
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return new PropagateInfo[1]
			{
				new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
			};
		}
		return null;
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (sp.TargetServerVersionInternal > SqlServerVersionInternal.Version80 && base.ServerVersion.Major > 8)
		{
			base.AddScriptPermission(query, sp);
		}
	}

	private string ScriptAddToRole(string role, ScriptingPreferences sp)
	{
		if (VersionUtils.IsTargetServerVersionSQl11OrLater(sp.TargetServerVersionInternal) && sp.TargetDatabaseEngineType != DatabaseEngineType.SqlAzureDatabase)
		{
			return string.Format(SmoApplication.DefaultCulture, "ALTER ROLE {0} ADD MEMBER {1}", new object[2]
			{
				SqlSmoObject.MakeSqlBraket(role),
				SqlSmoObject.MakeSqlBraket(Name)
			});
		}
		string text = ((sp != null) ? ((sp.TargetServerVersion < SqlServerVersion.Version90) ? "dbo" : "sys") : ((base.ServerVersion.Major < 9) ? "dbo" : "sys"));
		string text2 = ((sp == null) ? SqlSmoObject.MakeSqlString(Name) : FormatFullNameForScripting(sp, nameIsIndentifier: false));
		return string.Format(SmoApplication.DefaultCulture, "{0}.sp_addrolemember @rolename = {1}, @membername = {2}", new object[3]
		{
			text,
			SqlSmoObject.MakeSqlString(role),
			text2
		});
	}

	public void AddToRole(string role)
	{
		CheckObjectState();
		if (role == null)
		{
			throw new SmoException(ExceptionTemplatesImpl.InnerException, new ArgumentNullException("role"));
		}
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(GetDBName()) }));
		stringCollection.Add(ScriptAddToRole(role, new ScriptingPreferences(this)));
		ExecutionManager.ExecuteNonQuery(stringCollection);
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
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
		renameQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER USER {0} WITH NAME={1}", new object[2]
		{
			FormatFullNameForScripting(new ScriptingPreferences()),
			SqlSmoObject.MakeSqlBraket(newName)
		}));
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		string[] fields = new string[9] { "AuthenticationType", "DefaultLanguageLcid", "DefaultLanguageName", "DefaultSchema", "ID", "IsSystemObject", "Login", "LoginType", "UserType" };
		List<string> supportedScriptFields = SqlSmoObject.GetSupportedScriptFields(typeof(PropertyMetadataProvider), fields, version, databaseEngineType, databaseEngineEdition);
		return supportedScriptFields.ToArray();
	}
}

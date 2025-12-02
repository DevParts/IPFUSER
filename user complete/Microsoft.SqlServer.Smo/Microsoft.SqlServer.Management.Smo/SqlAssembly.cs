using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;
using Microsoft.SqlServer.Server;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class SqlAssembly : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, IAlterable, IDroppable, IDropIfExists, IExtendedProperties, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 12, 12, 12, 12, 12, 12, 12, 12 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 12 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("AssemblySecurityLevel", expensive: false, readOnly: false, typeof(AssemblySecurityLevel)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("Culture", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsVisible", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PublicKey", expensive: false, readOnly: false, typeof(byte[])),
			new StaticMetadata("VersionBuild", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMajor", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VersionRevision", expensive: false, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[12]
		{
			new StaticMetadata("AssemblySecurityLevel", expensive: false, readOnly: false, typeof(AssemblySecurityLevel)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("Culture", expensive: false, readOnly: true, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("IsVisible", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PublicKey", expensive: false, readOnly: false, typeof(byte[])),
			new StaticMetadata("VersionBuild", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMajor", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VersionMinor", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("VersionRevision", expensive: false, readOnly: true, typeof(int))
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
					"AssemblySecurityLevel" => 0, 
					"CreateDate" => 1, 
					"Culture" => 2, 
					"ID" => 3, 
					"IsSystemObject" => 4, 
					"IsVisible" => 5, 
					"Owner" => 6, 
					"PublicKey" => 7, 
					"VersionBuild" => 8, 
					"VersionMajor" => 9, 
					"VersionMinor" => 10, 
					"VersionRevision" => 11, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AssemblySecurityLevel" => 0, 
				"CreateDate" => 1, 
				"Culture" => 2, 
				"ID" => 3, 
				"IsSystemObject" => 4, 
				"IsVisible" => 5, 
				"Owner" => 6, 
				"PublicKey" => 7, 
				"VersionBuild" => 8, 
				"VersionMajor" => 9, 
				"VersionMinor" => 10, 
				"VersionRevision" => 11, 
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

	private SqlAssemblyEvents events;

	private string primaryAssemblyServerPath;

	private string[] assemblyLocalPaths;

	private AssemblyAlterOptions assemblyAlterMethod;

	private SqlAssemblyFileCollection sqlAssemblyFiles;

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
	public AssemblySecurityLevel AssemblySecurityLevel
	{
		get
		{
			return (AssemblySecurityLevel)base.Properties.GetValueWithNullReplacement("AssemblySecurityLevel");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AssemblySecurityLevel", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Culture => (string)base.Properties.GetValueWithNullReplacement("Culture");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsVisible
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsVisible");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsVisible", value);
		}
	}

	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
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
	public byte[] PublicKey
	{
		get
		{
			return (byte[])base.Properties.GetValueWithNullReplacement("PublicKey");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("PublicKey", value);
		}
	}

	public SqlAssemblyEvents Events
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
				events = new SqlAssemblyEvents(this);
			}
			return events;
		}
	}

	public static string UrnSuffix => "SqlAssembly";

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

	[SfcObject(SfcContainerRelationship.ChildContainer, SfcContainerCardinality.ZeroToAny, typeof(SqlAssemblyFile))]
	public SqlAssemblyFileCollection SqlAssemblyFiles
	{
		get
		{
			CheckObjectState();
			if (sqlAssemblyFiles == null)
			{
				sqlAssemblyFiles = new SqlAssemblyFileCollection(this);
			}
			return sqlAssemblyFiles;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public Version Version => new Version((int)base.Properties.GetValueWithNullReplacement("VersionMajor"), (int)base.Properties.GetValueWithNullReplacement("VersionMinor"), (int)base.Properties.GetValueWithNullReplacement("VersionBuild"), (int)base.Properties.GetValueWithNullReplacement("VersionRevision"));

	public SqlAssembly()
	{
	}

	public SqlAssembly(Database database, string name)
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

	private void init()
	{
		primaryAssemblyServerPath = null;
		assemblyLocalPaths = null;
		assemblyAlterMethod = AssemblyAlterOptions.None;
		sqlAssemblyFiles = null;
	}

	internal SqlAssembly(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
		if (SqlContext.IsAvailable)
		{
			throw new Exception(ExceptionTemplatesImpl.SmoSQLCLRUnAvailable);
		}
		init();
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	public void Create(string[] assemblyLocalPaths)
	{
		try
		{
			if (assemblyLocalPaths == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("assemblyLocalPaths"), ExceptionTemplatesImpl.InnerException);
			}
			if (assemblyLocalPaths.Length == 0)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, null, ExceptionTemplatesImpl.EmptyInputParam("assemblyLocalPaths", "Collection"));
			}
			this.assemblyLocalPaths = assemblyLocalPaths;
			CreateImpl();
		}
		finally
		{
			if (!base.IsDesignMode)
			{
				this.assemblyLocalPaths = null;
			}
		}
	}

	public void Create(string primaryAssemblyServerPath)
	{
		try
		{
			if (primaryAssemblyServerPath == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, new ArgumentNullException("primaryAssemblyServerPath"), ExceptionTemplatesImpl.InnerException);
			}
			if (string.Empty == primaryAssemblyServerPath)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Create, this, null, ExceptionTemplatesImpl.EmptyInputParam("primaryAssemblyServerPath", "string"));
			}
			this.primaryAssemblyServerPath = primaryAssemblyServerPath;
			CreateImpl();
		}
		finally
		{
			if (!base.IsDesignMode)
			{
				this.primaryAssemblyServerPath = null;
			}
		}
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		StringCollection stringCollection = new StringCollection();
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_ASSEMBLY100, new object[2]
				{
					"NOT",
					SqlSmoObject.SqlString(GetName(sp))
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_ASSEMBLY, new object[2]
				{
					"NOT",
					SqlSmoObject.SqlString(GetName(sp))
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat("CREATE ASSEMBLY {0}", text);
		stringBuilder.Append(sp.NewLine);
		Property property;
		if (sp.IncludeScripts.Owner && (property = base.Properties.Get("Owner")).Value != null)
		{
			stringBuilder.AppendFormat("AUTHORIZATION [{0}]", SqlSmoObject.SqlBraket(property.Value.ToString()));
			stringBuilder.Append(sp.NewLine);
		}
		if (primaryAssemblyServerPath != null)
		{
			stringBuilder.AppendFormat("FROM N'{0}'", SqlSmoObject.SqlString(primaryAssemblyServerPath));
			stringBuilder.Append(sp.NewLine);
		}
		else if (assemblyLocalPaths != null)
		{
			stringBuilder.Append("FROM 0x");
			int num = 0;
			string[] array = assemblyLocalPaths;
			foreach (string assemblyLocalPath in array)
			{
				if (num++ != 0)
				{
					stringBuilder.Append(", 0x");
				}
				AppendAssemblyFile(stringBuilder, assemblyLocalPath);
			}
			stringBuilder.Append(sp.NewLine);
		}
		else
		{
			stringBuilder.Append("FROM ");
			foreach (SqlAssemblyFile sqlAssemblyFile in SqlAssemblyFiles)
			{
				if (sqlAssemblyFile.ID == 1)
				{
					stringBuilder.Append(sqlAssemblyFile.GetFileText());
					continue;
				}
				StringBuilder stringBuilder2 = new StringBuilder(Globals.INIT_BUFFER_SIZE);
				stringBuilder2.AppendFormat("ALTER ASSEMBLY {0}", text);
				stringBuilder2.Append(sp.NewLine);
				stringBuilder2.Append("ADD FILE FROM ");
				stringBuilder2.Append(sqlAssemblyFile.GetFileText());
				stringBuilder2.Append(sp.NewLine);
				stringBuilder2.AppendFormat("AS N'{0}'", SqlSmoObject.SqlString(sqlAssemblyFile.Name));
				stringBuilder2.Append(sp.NewLine);
				stringCollection.Add(stringBuilder2.ToString());
			}
			stringBuilder.Append(sp.NewLine);
		}
		int num2 = 0;
		if ((property = base.Properties.Get("AssemblySecurityLevel")).Value != null)
		{
			if (num2++ == 0)
			{
				stringBuilder.Append("WITH");
			}
			stringBuilder.Append(" PERMISSION_SET = ");
			switch ((AssemblySecurityLevel)property.Value)
			{
			case AssemblySecurityLevel.Unrestricted:
				stringBuilder.Append("UNSAFE");
				break;
			case AssemblySecurityLevel.External:
				stringBuilder.Append("EXTERNAL_ACCESS");
				break;
			case AssemblySecurityLevel.Safe:
				stringBuilder.Append("SAFE");
				break;
			default:
				stringBuilder.Append("SAFE");
				break;
			}
		}
		if (num2 > 0)
		{
			stringBuilder.Append(sp.NewLine);
		}
		queries.Add(stringBuilder.ToString());
		StringEnumerator enumerator2 = stringCollection.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				string current = enumerator2.Current;
				queries.Add(current);
			}
		}
		finally
		{
			if (enumerator2 is IDisposable disposable)
			{
				disposable.Dispose();
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
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_ASSEMBLY100, new object[2]
				{
					"",
					SqlSmoObject.SqlString(GetName(sp))
				});
			}
			else
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_ASSEMBLY, new object[2]
				{
					"",
					SqlSmoObject.SqlString(GetName(sp))
				});
			}
			stringBuilder.Append(sp.NewLine);
		}
		stringBuilder.AppendFormat("DROP ASSEMBLY {0}{1}", (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty, text);
		stringBuilder.Append(sp.NewLine);
		dropQuery.Add(stringBuilder.ToString());
	}

	public void Alter()
	{
		assemblyAlterMethod = AssemblyAlterOptions.None;
		AlterImpl();
	}

	public void Alter(AssemblyAlterOptions assemblyAlterMethod)
	{
		try
		{
			this.assemblyAlterMethod = assemblyAlterMethod;
			AlterImpl();
		}
		finally
		{
			if (!base.IsDesignMode)
			{
				this.assemblyAlterMethod = AssemblyAlterOptions.NoChecks;
			}
		}
	}

	public void Alter(AssemblyAlterOptions assemblyAlterMethod, string primaryAssemblyServerPath)
	{
		try
		{
			if (primaryAssemblyServerPath == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, new ArgumentNullException("primaryAssemblyServerPath"), ExceptionTemplatesImpl.InnerException);
			}
			if (string.Empty == primaryAssemblyServerPath)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.EmptyInputParam("primaryAssemblyServerPath", "string"));
			}
			this.assemblyAlterMethod = assemblyAlterMethod;
			this.primaryAssemblyServerPath = primaryAssemblyServerPath;
			AlterImpl();
		}
		finally
		{
			if (!base.IsDesignMode)
			{
				this.assemblyAlterMethod = AssemblyAlterOptions.NoChecks;
				this.primaryAssemblyServerPath = null;
			}
		}
	}

	public void Alter(AssemblyAlterOptions assemblyAlterMethod, string[] assemblyLocalPaths)
	{
		try
		{
			if (assemblyLocalPaths == null)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, new ArgumentNullException("assemblyLocalPaths"), ExceptionTemplatesImpl.InnerException);
			}
			if (assemblyLocalPaths.Length == 0)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Alter, this, null, ExceptionTemplatesImpl.EmptyInputParam("assemblyLocalPaths", "Collection"));
			}
			this.assemblyLocalPaths = assemblyLocalPaths;
			AlterImpl();
		}
		finally
		{
			if (!base.IsDesignMode)
			{
				this.assemblyAlterMethod = AssemblyAlterOptions.NoChecks;
				this.assemblyLocalPaths = null;
			}
		}
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		if (!IsObjectDirty())
		{
			return;
		}
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		stringBuilder.AppendFormat("ALTER ASSEMBLY {0}", FullQualifiedName);
		stringBuilder.Append(sp.NewLine);
		int length = stringBuilder.Length;
		if (primaryAssemblyServerPath != null)
		{
			stringBuilder.AppendFormat("FROM N'{0}'", SqlSmoObject.SqlString(primaryAssemblyServerPath));
			stringBuilder.Append(sp.NewLine);
		}
		else if (assemblyLocalPaths != null)
		{
			stringBuilder.Append("FROM 0x");
			int num = 0;
			string[] array = assemblyLocalPaths;
			foreach (string assemblyLocalPath in array)
			{
				if (num++ != 0)
				{
					stringBuilder.Append(", 0x");
				}
				AppendAssemblyFile(stringBuilder, assemblyLocalPath);
			}
			stringBuilder.Append(sp.NewLine);
		}
		bool needsComma = false;
		Property property;
		if ((property = base.Properties.Get("AssemblySecurityLevel")).Value != null && property.Dirty)
		{
			AppendWithCommaText(stringBuilder, "PERMISSION_SET = ", ref needsComma);
			switch ((AssemblySecurityLevel)property.Value)
			{
			case AssemblySecurityLevel.Unrestricted:
				stringBuilder.Append("UNSAFE");
				break;
			case AssemblySecurityLevel.External:
				stringBuilder.Append("EXTERNAL_ACCESS");
				break;
			case AssemblySecurityLevel.Safe:
				stringBuilder.Append("SAFE");
				break;
			default:
				throw new WrongPropertyValueException(base.Properties.Get("AssemblySecurityLevel"));
			}
		}
		if ((property = base.Properties.Get("IsVisible")).Value != null && property.Dirty)
		{
			AppendWithCommaText(stringBuilder, "VISIBILITY = ", ref needsComma);
			stringBuilder.Append(((bool)property.Value) ? "ON" : "OFF");
		}
		switch (assemblyAlterMethod)
		{
		case AssemblyAlterOptions.NoChecks:
			AppendWithCommaText(stringBuilder, "UNCHECKED DATA", ref needsComma);
			break;
		default:
			throw new WrongPropertyValueException(base.Properties.Get("AssemblySecurityLevel"));
		case AssemblyAlterOptions.None:
			break;
		}
		if (needsComma)
		{
			stringBuilder.Append(sp.NewLine);
		}
		if (stringBuilder.Length > length)
		{
			alterQuery.Add(stringBuilder.ToString());
		}
		if ((property = base.Properties.Get("Owner")).Value != null && property.Dirty)
		{
			alterQuery.Add(string.Format(SmoApplication.DefaultCulture, "ALTER AUTHORIZATION ON ASSEMBLY::{0} TO {1}", new object[2]
			{
				FullQualifiedName,
				SqlSmoObject.MakeSqlBraket(property.Value.ToString())
			}));
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

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			if (primaryAssemblyServerPath == null)
			{
				return null != assemblyLocalPaths;
			}
			return true;
		}
		return true;
	}

	private void AppendAssemblyFile(StringBuilder sb, string assemblyLocalPath)
	{
		FileStream fileStream = null;
		try
		{
			fileStream = new FileStream(assemblyLocalPath, FileMode.Open, FileAccess.Read);
			byte[] array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				sb.Append(b.ToString("X2", SmoApplication.DefaultCulture));
			}
		}
		finally
		{
			fileStream?.Close();
		}
	}

	protected override void MarkDropped()
	{
		base.MarkDropped();
		if (sqlAssemblyFiles != null)
		{
			sqlAssemblyFiles.MarkAllDropped();
		}
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[3] { "Owner", "AssemblySecurityLevel", "IsSystemObject" };
	}
}

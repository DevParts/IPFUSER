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

[PhysicalFacet]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
public sealed class FullTextCatalog : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IAlterable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 10, 10, 14, 15, 15, 15, 15, 15, 15, 15 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 14 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[14]
		{
			new StaticMetadata("ErrorLogSize", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("FileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("FullTextIndexSize", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("HasFullTextIndexedTables", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("IsAccentSensitive", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("ItemCount", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("PopulationCompletionAge", expensive: false, readOnly: true, typeof(TimeSpan)),
			new StaticMetadata("PopulationCompletionDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("PopulationStatus", expensive: false, readOnly: true, typeof(CatalogPopulationStatus)),
			new StaticMetadata("RootPath", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UniqueKeyCount", expensive: true, readOnly: true, typeof(int))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[15]
		{
			new StaticMetadata("ErrorLogSize", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("FullTextIndexSize", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("HasFullTextIndexedTables", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ItemCount", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("PopulationCompletionAge", expensive: false, readOnly: true, typeof(TimeSpan)),
			new StaticMetadata("PopulationCompletionDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("PopulationStatus", expensive: false, readOnly: true, typeof(CatalogPopulationStatus)),
			new StaticMetadata("RootPath", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("UniqueKeyCount", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("FileGroup", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("IsAccentSensitive", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsDefault", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string)),
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
					return -1;
				}
				return propertyName switch
				{
					"ErrorLogSize" => 0, 
					"FileGroup" => 1, 
					"FullTextIndexSize" => 2, 
					"HasFullTextIndexedTables" => 3, 
					"ID" => 4, 
					"IsAccentSensitive" => 5, 
					"IsDefault" => 6, 
					"ItemCount" => 7, 
					"Owner" => 8, 
					"PopulationCompletionAge" => 9, 
					"PopulationCompletionDate" => 10, 
					"PopulationStatus" => 11, 
					"RootPath" => 12, 
					"UniqueKeyCount" => 13, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ErrorLogSize" => 0, 
				"FullTextIndexSize" => 1, 
				"HasFullTextIndexedTables" => 2, 
				"ID" => 3, 
				"ItemCount" => 4, 
				"PopulationCompletionAge" => 5, 
				"PopulationCompletionDate" => 6, 
				"PopulationStatus" => 7, 
				"RootPath" => 8, 
				"UniqueKeyCount" => 9, 
				"FileGroup" => 10, 
				"IsAccentSensitive" => 11, 
				"IsDefault" => 12, 
				"Owner" => 13, 
				"PolicyHealthState" => 14, 
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

	private enum CatalogPopulationActionEx
	{
		Full = 1,
		Incremental,
		Stop
	}

	internal override UserPermissionCollection Permissions => GetUserPermissions();

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ErrorLogSize => (int)base.Properties.GetValueWithNullReplacement("ErrorLogSize");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string FileGroup
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FileGroup");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FileGroup", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int FullTextIndexSize => (int)base.Properties.GetValueWithNullReplacement("FullTextIndexSize");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool HasFullTextIndexedTables => (bool)base.Properties.GetValueWithNullReplacement("HasFullTextIndexedTables");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsAccentSensitive
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsAccentSensitive");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsAccentSensitive", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsDefault
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsDefault");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsDefault", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ItemCount => (int)base.Properties.GetValueWithNullReplacement("ItemCount");

	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[CLSCompliant(false)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(ApplicationRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/ApplicationRole[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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
	public TimeSpan PopulationCompletionAge => (TimeSpan)base.Properties.GetValueWithNullReplacement("PopulationCompletionAge");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime PopulationCompletionDate => (DateTime)base.Properties.GetValueWithNullReplacement("PopulationCompletionDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public CatalogPopulationStatus PopulationStatus => (CatalogPopulationStatus)base.Properties.GetValueWithNullReplacement("PopulationStatus");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string RootPath
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("RootPath");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RootPath", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int UniqueKeyCount => (int)base.Properties.GetValueWithNullReplacement("UniqueKeyCount");

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

	public static string UrnSuffix => "FullTextCatalog";

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

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	internal override string[] GetNonAlterableProperties()
	{
		return new string[2] { "IsDefault", "RootPath" };
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

	public FullTextCatalog()
	{
	}

	public FullTextCatalog(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		if (database != null && database.IsExpressSku() && !database.Parent.IsFullTextInstalled)
		{
			throw new UnsupportedFeatureException(ExceptionTemplatesImpl.UnsupportedFeatureFullText);
		}
		Parent = database;
	}

	internal FullTextCatalog(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		ScriptCreateCatalog(queries, sp);
	}

	public void Alter()
	{
		AlterImpl();
	}

	internal override void ScriptAlter(StringCollection alterQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		if (IsObjectDirty())
		{
			ScriptAlterCatalog(alterQuery, sp);
		}
	}

	private void ScriptCreateCatalog(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		string name = GetName(sp);
		if (sp.IncludeScripts.Header)
		{
			stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_CATALOG, new object[2]
			{
				"NOT",
				SqlSmoObject.SqlString(name)
			});
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE FULLTEXT CATALOG {0} ", new object[1] { text });
			if (sp.TargetServerVersionInternal == SqlServerVersionInternal.Version90)
			{
				Property propertyOptional;
				if (base.ServerVersion.Major >= 9 && sp.Storage.FileGroup && (propertyOptional = GetPropertyOptional("FileGroup")).Value != null && propertyOptional.Value.ToString().Length > 0 && base.StringComparer.Compare(propertyOptional.Value.ToString(), "PRIMARY") != 0)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ON FILEGROUP [{0}]", new object[1] { SqlSmoObject.SqlBraket(propertyOptional.Value.ToString()) });
				}
				stringBuilder.Append(sp.NewLine);
				if (sp.OldOptions.IncludeFullTextCatalogRootPath && (propertyOptional = base.Properties.Get("RootPath")).Value != null && propertyOptional.Value.ToString().Length > 0)
				{
					string text2 = propertyOptional.Value.ToString();
					if (text2.EndsWith("\\" + Name, StringComparison.Ordinal) || text2.EndsWith("/" + Name, StringComparison.Ordinal))
					{
						text2 = text2.Substring(0, text2.Length - Name.Length - 1);
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "IN PATH N'{0}'", new object[1] { SqlSmoObject.SqlString(text2) });
					stringBuilder.Append(sp.NewLine);
				}
			}
			if (base.ServerVersion.Major >= 9)
			{
				Property propertyOptional;
				if ((propertyOptional = base.Properties.Get("IsAccentSensitive")).Value != null)
				{
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "WITH ACCENT_SENSITIVITY = {0}", new object[1] { ((bool)propertyOptional.Value) ? "ON" : "OFF" });
					stringBuilder.Append(sp.NewLine);
				}
				if ((propertyOptional = base.Properties.Get("IsDefault")).Value != null && (bool)propertyOptional.Value)
				{
					stringBuilder.Append("AS DEFAULT");
					stringBuilder.Append(sp.NewLine);
				}
				if (sp.IncludeScripts.Owner && (propertyOptional = base.Properties.Get("Owner")).Value != null && propertyOptional.Value.ToString().Length > 0)
				{
					stringBuilder.AppendFormat("AUTHORIZATION [{0}]", SqlSmoObject.SqlBraket(propertyOptional.Value.ToString()));
					stringBuilder.Append(sp.NewLine);
				}
			}
		}
		else
		{
			_ = (Database)base.ParentColl.ParentInstance;
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_catalog @ftcat=N'{0}', @action=N'create'", new object[1] { SqlSmoObject.SqlString(name) });
			Property propertyOptional;
			if (sp.OldOptions.IncludeFullTextCatalogRootPath && (propertyOptional = base.Properties.Get("RootPath")).Value != null && propertyOptional.Value.ToString().Length > 0)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, ", @path=N'{0}'", new object[1] { SqlSmoObject.SqlString(propertyOptional.Value.ToString()) });
			}
			stringBuilder.Append(sp.NewLine);
		}
		queries.Add(stringBuilder.ToString());
	}

	private void ScriptAlterCatalog(StringCollection queries, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		bool flag = false;
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90 && base.ServerVersion.Major >= 9)
		{
			Property property;
			if ((property = base.Properties.Get("Owner")).Value != null && property.Dirty)
			{
				stringBuilder.AppendFormat("ALTER AUTHORIZATION ON FULLTEXT CATALOG::{0} TO {1}", FullQualifiedName, SqlSmoObject.MakeSqlBraket(property.Value.ToString()));
				flag = true;
			}
			stringBuilder.Append(sp.NewLine);
		}
		if (flag)
		{
			queries.Add(stringBuilder.ToString());
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

	internal override void AddScriptPermission(StringCollection query, ScriptingPreferences sp)
	{
		if (Parent.Parent.Information.Version.Major >= 9)
		{
			base.AddScriptPermission(query, sp);
		}
	}

	internal override void ScriptDrop(StringCollection dropQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		string name = GetName(sp);
		if (sp.IncludeScripts.Header)
		{
			dropQuery.Add(ExceptionTemplates.IncludeHeader(UrnSuffix, text, DateTime.Now.ToString(GetDbCulture())));
		}
		if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version90)
		{
			stringBuilder.Length = 0;
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_CATALOG, new object[2]
				{
					"",
					SqlSmoObject.SqlString(name)
				});
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_catalog @ftcat=N'{0}', @action=N'stop'", new object[1] { SqlSmoObject.SqlString(name) });
			dropQuery.Add(stringBuilder.ToString());
		}
		stringBuilder.Length = 0;
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_CATALOG, new object[2]
			{
				"",
				SqlSmoObject.SqlString(name)
			});
			stringBuilder.Append(sp.NewLine);
		}
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version90)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP FULLTEXT CATALOG {0}", new object[1] { text });
		}
		else
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_catalog @ftcat=N'{0}', @action=N'drop'", new object[1] { SqlSmoObject.SqlString(name) });
		}
		dropQuery.Add(stringBuilder.ToString());
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public void Rebuild()
	{
		StringCollection stringCollection = new StringCollection();
		Database database = (Database)base.ParentColl.ParentInstance;
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
		if (base.ServerVersion.Major >= 9)
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT CATALOG [{0}] REBUILD", new object[1] { SqlSmoObject.SqlBraket(Name) }));
		}
		else
		{
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_catalog @ftcat=N'{0}', @action=N'rebuild'", new object[1] { SqlSmoObject.SqlString(Name) }));
		}
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	public void Rebuild(bool accentSensitive)
	{
		if (base.ServerVersion.Major >= 9)
		{
			StringCollection stringCollection = new StringCollection();
			Database database = (Database)base.ParentColl.ParentInstance;
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT CATALOG [{0}] REBUILD WITH ACCENT_SENSITIVITY = {1}", new object[2]
			{
				SqlSmoObject.SqlBraket(Name),
				accentSensitive ? "ON" : "OFF"
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
	}

	public void Reorganize()
	{
		if (base.ServerVersion.Major >= 9)
		{
			StringCollection stringCollection = new StringCollection();
			Database database = (Database)base.ParentColl.ParentInstance;
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT CATALOG [{0}] REORGANIZE", new object[1] { SqlSmoObject.SqlBraket(Name) }));
			ExecutionManager.ExecuteNonQuery(stringCollection);
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
	}

	private void StartOrStopPopulation(CatalogPopulationActionEx action)
	{
		Database database = (Database)base.ParentColl.ParentInstance;
		if (base.ServerVersion.Major >= 9)
		{
			DataTable dataTable = EnumTables();
			{
				foreach (DataRow row in dataTable.Rows)
				{
					Table table = database.Tables[(string)row["Table_Name"], (string)row["Table_Schema"]];
					int num;
					switch (action)
					{
					case CatalogPopulationActionEx.Stop:
						table.FullTextIndex.StopPopulation();
						continue;
					default:
						num = 1;
						break;
					case CatalogPopulationActionEx.Incremental:
						num = 2;
						break;
					}
					IndexPopulationAction action2 = (IndexPopulationAction)num;
					table.FullTextIndex.StartPopulation(action2);
				}
				return;
			}
		}
		StringCollection stringCollection = new StringCollection();
		string text = ((action != CatalogPopulationActionEx.Stop) ? ((action == CatalogPopulationActionEx.Incremental) ? "start_incremental" : "start_full") : "stop");
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(database.Name) }));
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "EXEC dbo.sp_fulltext_catalog @ftcat=N'{0}', @action=N'{1}'", new object[2]
		{
			SqlSmoObject.SqlString(Name),
			text
		}));
		database.ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	public void StartPopulation(CatalogPopulationAction action)
	{
		StartOrStopPopulation((CatalogPopulationActionEx)action);
	}

	public void StopPopulation()
	{
		StartOrStopPopulation(CatalogPopulationActionEx.Stop);
	}

	private DataTable EnumTables()
	{
		new StringCollection();
		Database database = (Database)base.ParentColl.ParentInstance;
		Request request = new Request(new Urn(string.Format(SmoApplication.DefaultCulture, string.Concat(database.Urn, "/Table/FullTextIndex[@CatalogName='{0}']"), new object[1] { Urn.EscapeString(Name) })));
		request.Fields = new string[1] { "Name" };
		request.ParentPropertiesRequests = new PropertiesRequest[1];
		PropertiesRequest propertiesRequest = new PropertiesRequest();
		propertiesRequest.Fields = new string[2] { "Schema", "Name" };
		propertiesRequest.OrderByList = new OrderBy[2]
		{
			new OrderBy("Schema", OrderBy.Direction.Asc),
			new OrderBy("Name", OrderBy.Direction.Asc)
		};
		request.ParentPropertiesRequests[0] = propertiesRequest;
		return ExecutionManager.GetEnumeratorData(request);
	}

	internal void Validate_set_IsDefault(Property prop, object newValue)
	{
		if (base.State != SqlSmoState.Creating)
		{
			throw new PropertyReadOnlyException(prop.Name);
		}
		if (!(bool)newValue)
		{
			return;
		}
		SmoInternalStorage internalStorage = ((FullTextCatalogCollection)base.ParentColl).InternalStorage;
		foreach (FullTextCatalog item in internalStorage)
		{
			Property property = item.Properties.Get("IsDefault");
			if (this != item && (bool)property.Value && item.State == SqlSmoState.Creating)
			{
				property.SetValue(false);
			}
		}
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (base.ServerVersion.Major >= 9 && prop.Name == "IsDefault")
		{
			Validate_set_IsDefault(prop, value);
		}
	}

	protected override void PostCreate()
	{
		if (base.ServerVersion.Major >= 9)
		{
			Property property = base.Properties.Get("IsDefault");
			if (property.Value != null && (bool)property.Value)
			{
				Property property2 = Parent.Properties.Get("DefaultFullTextCatalog");
				property2.SetValue(Name);
				property2.SetRetrieved(retrieved: true);
			}
		}
	}

	public DataTable EnumErrorLogs()
	{
		ThrowIfBelowVersion90();
		return ExecutionManager.GetEnumeratorData(new Request(string.Concat(base.Urn, "/ErrorLog")));
	}

	public DataTable ReadErrorLog()
	{
		return ReadErrorLog(0);
	}

	public DataTable ReadErrorLog(int logNumber)
	{
		ThrowIfBelowVersion90();
		if (logNumber < 0)
		{
			logNumber = 0;
		}
		return ExecutionManager.GetEnumeratorData(new Request((string)base.Urn + "/ErrorLog[@ArchiveNo=" + logNumber + "]/LogEntry"));
	}
}

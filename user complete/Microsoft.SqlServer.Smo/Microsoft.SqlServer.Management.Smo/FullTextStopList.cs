using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[PhysicalFacet]
public sealed class FullTextStopList : ScriptNameObjectBase, ISfcSupportsDesignMode, IObjectPermission, ICreatable, IDroppable, IDropIfExists, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 3, 3, 3, 3, 3, 3, 3 };

		private static int[] cloudVersionCount = new int[3] { 0, 0, 2 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[2]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("Owner", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[3]
		{
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
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
					"ID" => 0, 
					"Owner" => 1, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"ID" => 0, 
				"Owner" => 1, 
				"PolicyHealthState" => 2, 
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

	private Dictionary<string, List<string>> stopListCollection = new Dictionary<string, List<string>>();

	internal string srcFullTextStopListName = string.Empty;

	internal string srcDbName = string.Empty;

	internal bool srcSystemDefault;

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

	[CLSCompliant(false)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(DatabaseRole), "Server[@Name = '{0}']/Database[@Name = '{1}']/Role[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "Owner" })]
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

	public static string UrnSuffix => "FullTextStopList";

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

	public FullTextStopList()
	{
	}

	public FullTextStopList(Database database, string name)
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
		return new string[1] { "Owner" };
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

	internal FullTextStopList(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		CreateImpl();
	}

	public void CreateFromExistingStopList(string stoplistName)
	{
		srcFullTextStopListName = stoplistName;
		CreateImpl();
	}

	public void CreateFromExistingStopList(string dbName, string stoplistName)
	{
		srcDbName = dbName;
		srcFullTextStopListName = stoplistName;
		CreateImpl();
	}

	public void CreateFromSystemStopList()
	{
		srcSystemDefault = true;
		CreateImpl();
	}

	internal override void ScriptCreate(StringCollection createQuery, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, Name, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_STOPLIST, new object[2]
				{
					"NOT",
					SqlSmoObject.SqlString(Name)
				});
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(Scripts.BEGIN);
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "CREATE FULLTEXT STOPLIST {0}", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
			stringBuilder.Append(sp.NewLine);
			if ((srcFullTextStopListName != string.Empty || srcSystemDefault) && base.State == SqlSmoState.Creating)
			{
				stringBuilder.Append("FROM ");
				if (srcSystemDefault)
				{
					stringBuilder.Append("SYSTEM STOPLIST");
				}
				else
				{
					if (srcDbName != string.Empty)
					{
						stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}.", new object[1] { SqlSmoObject.MakeSqlBraket(srcDbName) });
					}
					stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "{0}", new object[1] { SqlSmoObject.MakeSqlBraket(srcFullTextStopListName) });
				}
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.Owner)
			{
				Property property = base.Properties.Get("Owner");
				if (property.Value != null && property.Value.ToString().Length > 0)
				{
					stringBuilder.AppendFormat("AUTHORIZATION {0}", SqlSmoObject.MakeSqlBraket(property.Value.ToString()));
				}
			}
			stringBuilder.Append(";");
			stringBuilder.Append(sp.NewLine);
			if (base.State == SqlSmoState.Existing)
			{
				if (base.IsDesignMode)
				{
					foreach (KeyValuePair<string, List<string>> item in stopListCollection)
					{
						foreach (string item2 in item.Value)
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT STOPLIST {0} ADD '{1}' LANGUAGE '{2}';", new object[3]
							{
								SqlSmoObject.MakeSqlBraket(Name),
								item2,
								item.Key
							});
							stringBuilder.Append(sp.NewLine);
						}
					}
				}
				else
				{
					DataTable dataTable = EnumStopWords();
					foreach (DataRow row in dataTable.Rows)
					{
						if (!row.IsNull("language"))
						{
							stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER FULLTEXT STOPLIST {0} ADD '{1}' LANGUAGE '{2}';", new object[3]
							{
								SqlSmoObject.MakeSqlBraket(Name),
								SqlSmoObject.SqlString(row["stopword"].ToString()),
								SqlSmoObject.SqlString(row["language"].ToString())
							});
						}
						stringBuilder.Append(sp.NewLine);
					}
				}
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.Append(Scripts.END);
				stringBuilder.Append(sp.NewLine);
			}
			createQuery.Add(stringBuilder.ToString());
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
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
		if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version100 && base.ServerVersion.Major >= 10)
		{
			if (sp.IncludeScripts.Header)
			{
				stringBuilder.Append(ExceptionTemplates.IncludeHeader(UrnSuffix, Name, DateTime.Now.ToString(GetDbCulture())));
				stringBuilder.Append(sp.NewLine);
			}
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_FT_STOPLIST, new object[2]
				{
					string.Empty,
					SqlSmoObject.SqlString(Name)
				});
				stringBuilder.Append(sp.NewLine);
				stringBuilder.Append(Scripts.BEGIN);
				stringBuilder.Append(sp.NewLine);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP FULLTEXT STOPLIST {0};", new object[1] { SqlSmoObject.MakeSqlBraket(Name) });
			stringBuilder.Append(sp.NewLine);
			if (sp.IncludeScripts.ExistenceCheck)
			{
				stringBuilder.Append(Scripts.END);
				stringBuilder.Append(sp.NewLine);
			}
			dropQuery.Add(stringBuilder.ToString());
			return;
		}
		throw new UnsupportedVersionException(ExceptionTemplatesImpl.UnsupportedVersion(base.ServerVersion.ToString()));
	}

	public StringCollection Script()
	{
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		return ScriptImpl(scriptingOptions);
	}

	public DataTable EnumStopWords()
	{
		StringCollection stringCollection = new StringCollection();
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, Scripts.USEDB, new object[1] { SqlSmoObject.SqlBraket(Parent.Name) }));
		stringCollection.Add("select stopword, language from sys.fulltext_stopwords where stoplist_id=" + ID);
		return ExecutionManager.ExecuteWithResults(stringCollection).Tables[0];
	}

	public bool HasStopWord(string stopword, string language)
	{
		if (base.IsDesignMode)
		{
			if (!stopListCollection.ContainsKey(language))
			{
				return false;
			}
			List<string> list = stopListCollection[language];
			if (list.Contains(stopword))
			{
				return true;
			}
			return false;
		}
		DataTable dataTable = EnumStopWords();
		System.StringComparer stringComparer = NetCoreHelpers.InvariantCulture.GetStringComparer(ignoreCase: true);
		foreach (DataRow row in dataTable.Rows)
		{
			if (stringComparer.Compare((string)row["stopword"], stopword) == 0 && stringComparer.Compare((string)row["language"], language) == 0)
			{
				return true;
			}
		}
		return false;
	}

	public void AddStopWord(string stopword, string language)
	{
		if (base.IsDesignMode)
		{
			if (!stopListCollection.ContainsKey(language))
			{
				stopListCollection.Add(language, new List<string>());
			}
			List<string> list = stopListCollection[language];
			if (!list.Contains(stopword))
			{
				list.Add(stopword);
			}
		}
		else
		{
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT STOPLIST {0} ADD '{1}' LANGUAGE '{2}';", new object[3]
			{
				SqlSmoObject.MakeSqlBraket(Name),
				SqlSmoObject.SqlString(stopword),
				SqlSmoObject.SqlString(language)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
	}

	public void RemoveStopWord(string stopword, string language)
	{
		if (base.IsDesignMode)
		{
			if (stopListCollection.ContainsKey(language))
			{
				List<string> list = stopListCollection[language];
				if (list.Contains(stopword))
				{
					list.Remove(stopword);
				}
			}
		}
		else
		{
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
			stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT STOPLIST {0} DROP '{1}' LANGUAGE '{2}';", new object[3]
			{
				SqlSmoObject.MakeSqlBraket(Name),
				SqlSmoObject.SqlString(stopword),
				SqlSmoObject.SqlString(language)
			}));
			ExecutionManager.ExecuteNonQuery(stringCollection);
		}
	}

	public void RemoveAllStopWords()
	{
		if (base.IsDesignMode)
		{
			stopListCollection.Clear();
			return;
		}
		StringCollection stringCollection = new StringCollection();
		AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT STOPLIST {0} DROP ALL;", new object[1] { SqlSmoObject.MakeSqlBraket(Name) }));
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}

	public void RemoveAllStopWords(string language)
	{
		if (base.IsDesignMode)
		{
			if (stopListCollection.ContainsKey(language))
			{
				stopListCollection.Remove(language);
			}
			return;
		}
		StringCollection stringCollection = new StringCollection();
		AddDatabaseContext(stringCollection, new ScriptingPreferences(this));
		stringCollection.Add(string.Format(SmoApplication.DefaultCulture, "ALTER FULLTEXT STOPLIST {0} DROP ALL LANGUAGE '{1}';", new object[2]
		{
			SqlSmoObject.MakeSqlBraket(Name),
			SqlSmoObject.SqlString(language)
		}));
		ExecutionManager.ExecuteNonQuery(stringCollection);
	}
}

using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Diagnostics;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElement(SfcElementFlags.Standalone)]
[PhysicalFacet]
public sealed class Audit : ScriptNameObjectBase, ISfcSupportsDesignMode, ICreatable, IAlterable, IDroppable, IDropIfExists, IRenamable, IScriptable
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 0, 15, 15, 17, 17, 17, 17, 18 };

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
				"CreateDate" => 0, 
				"DateLastModified" => 1, 
				"DestinationType" => 2, 
				"Enabled" => 3, 
				"FileName" => 4, 
				"FilePath" => 5, 
				"Guid" => 6, 
				"ID" => 7, 
				"MaximumFileSize" => 8, 
				"MaximumFileSizeUnit" => 9, 
				"MaximumRolloverFiles" => 10, 
				"OnFailure" => 11, 
				"PolicyHealthState" => 12, 
				"QueueDelay" => 13, 
				"ReserveDiskSpace" => 14, 
				"Filter" => 15, 
				"MaximumFiles" => 16, 
				"RetentionDays" => 17, 
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
			staticMetadata = new StaticMetadata[18]
			{
				new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DestinationType", expensive: false, readOnly: false, typeof(AuditDestinationType)),
				new StaticMetadata("Enabled", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("FileName", expensive: false, readOnly: true, typeof(string)),
				new StaticMetadata("FilePath", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("Guid", expensive: false, readOnly: false, typeof(Guid)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("MaximumFileSize", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("MaximumFileSizeUnit", expensive: false, readOnly: false, typeof(AuditFileSizeUnit)),
				new StaticMetadata("MaximumRolloverFiles", expensive: false, readOnly: false, typeof(long)),
				new StaticMetadata("OnFailure", expensive: false, readOnly: false, typeof(OnFailureAction)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState)),
				new StaticMetadata("QueueDelay", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("ReserveDiskSpace", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Filter", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("MaximumFiles", expensive: false, readOnly: false, typeof(int)),
				new StaticMetadata("RetentionDays", expensive: false, readOnly: false, typeof(int))
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
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AuditDestinationType DestinationType
	{
		get
		{
			return (AuditDestinationType)base.Properties.GetValueWithNullReplacement("DestinationType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DestinationType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool Enabled => (bool)base.Properties.GetValueWithNullReplacement("Enabled");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FileName => (string)base.Properties.GetValueWithNullReplacement("FileName");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string FilePath
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("FilePath");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("FilePath", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public string Filter
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("Filter");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Filter", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone)]
	public Guid Guid
	{
		get
		{
			return (Guid)base.Properties.GetValueWithNullReplacement("Guid");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("Guid", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumFiles
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumFiles");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumFiles", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int MaximumFileSize
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("MaximumFileSize");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumFileSize", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public AuditFileSizeUnit MaximumFileSizeUnit
	{
		get
		{
			return (AuditFileSizeUnit)base.Properties.GetValueWithNullReplacement("MaximumFileSizeUnit");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumFileSizeUnit", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public long MaximumRolloverFiles
	{
		get
		{
			return (long)base.Properties.GetValueWithNullReplacement("MaximumRolloverFiles");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MaximumRolloverFiles", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public OnFailureAction OnFailure
	{
		get
		{
			return (OnFailureAction)base.Properties.GetValueWithNullReplacement("OnFailure");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("OnFailure", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int QueueDelay
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("QueueDelay");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QueueDelay", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool ReserveDiskSpace
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("ReserveDiskSpace");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ReserveDiskSpace", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int RetentionDays
	{
		get
		{
			return (int)base.Properties.GetValueWithNullReplacement("RetentionDays");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("RetentionDays", value);
		}
	}

	public static string UrnSuffix => "Audit";

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

	public Audit()
	{
	}

	public Audit(Server server, string name)
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
		return new string[1] { "Guid" };
	}

	internal Audit(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public void Create()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		CreateImpl();
	}

	public void Alter()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		AlterImpl();
	}

	public void Drop()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		DropImpl();
	}

	public void DropIfExists()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		DropImpl(isDropIfExists: true);
	}

	public void Rename(string newname)
	{
		this.ThrowIfNotSupported(typeof(Audit));
		RenameImpl(newname);
	}

	public StringCollection Script()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		return ScriptImpl();
	}

	public StringCollection Script(ScriptingOptions scriptingOptions)
	{
		this.ThrowIfNotSupported(typeof(Audit));
		return ScriptImpl(scriptingOptions);
	}

	public void Enable()
	{
		EnableDisable(enable: true);
	}

	public void Disable()
	{
		EnableDisable(enable: false);
	}

	private void EnableDisable(bool enable)
	{
		this.ThrowIfNotSupported(typeof(Audit));
		CheckObjectState();
		try
		{
			StringCollection stringCollection = new StringCollection();
			AddDatabaseContext(stringCollection);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER AUDIT {0}", new object[1] { FullQualifiedName });
			stringBuilder.AppendLine();
			if (enable)
			{
				stringBuilder.Append("WITH (STATE = ON)");
			}
			else
			{
				stringBuilder.Append("WITH (STATE = OFF)");
			}
			stringCollection.Add(stringBuilder.ToString());
			if (!base.IsDesignMode)
			{
				ExecutionManager.ExecuteNonQuery(stringCollection);
			}
			Property property = base.Properties.Get("Enabled");
			property.SetValue(enable);
			property.SetRetrieved(retrieved: true);
			if (!ExecutionManager.Recording && !SmoApplication.eventsSingleton.IsNullObjectAltered())
			{
				SmoApplication.eventsSingleton.CallObjectAltered(GetServerObject(), new ObjectAlteredEventArgs(base.Urn, this));
			}
		}
		catch (Exception ex)
		{
			SqlSmoObject.FilterException(ex);
			if (enable)
			{
				throw new FailedOperationException(ExceptionTemplatesImpl.Enable, this, ex);
			}
			throw new FailedOperationException(ExceptionTemplatesImpl.Disable, this, ex);
		}
	}

	public string EnumServerAuditSpecification()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		CheckObjectState();
		Urn urn = new Urn("Server/ServerAuditSpecification[@Guid='" + Guid.ToString() + "']");
		string[] fields = new string[1] { "Name" };
		Request req = new Request(urn, fields);
		DataTable enumeratorData = ExecutionManager.GetEnumeratorData(req);
		Microsoft.SqlServer.Management.Diagnostics.TraceHelper.Assert(enumeratorData.Rows.Count <= 1, "There can be max one ServerAuditSpecification per Audit");
		if (enumeratorData.Rows.Count == 1)
		{
			return enumeratorData.Rows[0]["Name"].ToString();
		}
		return string.Empty;
	}

	public DataTable EnumDatabaseAuditSpecification()
	{
		this.ThrowIfNotSupported(typeof(Audit));
		CheckObjectState();
		Urn urn = new Urn("Server/Database/DatabaseAuditSpecification[@Guid='" + Guid.ToString() + "']");
		string[] fields = new string[2] { "DatabaseName", "Name" };
		Request req = new Request(urn, fields);
		return ExecutionManager.GetEnumeratorData(req);
	}

	internal override void ScriptCreate(StringCollection query, ScriptingPreferences sp)
	{
		ScriptAudit(query, sp, create: true);
	}

	internal override void ScriptAlter(StringCollection query, ScriptingPreferences sp)
	{
		ScriptAudit(query, sp, create: false);
	}

	internal override void ScriptRename(StringCollection renameQuery, ScriptingPreferences sp, string newName)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER AUDIT {0} MODIFY NAME = {1}", new object[2]
		{
			FormatFullNameForScripting(sp),
			SqlSmoObject.MakeSqlBraket(newName)
		});
		renameQuery.Add(stringBuilder.ToString());
	}

	internal override void ScriptDrop(StringCollection query, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AUDIT, new object[2]
			{
				"",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendLine();
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP SERVER AUDIT {0}", new object[1] { FormatFullNameForScripting(sp) });
		query.Add(stringBuilder.ToString());
	}

	private void ScriptAudit(StringCollection query, ScriptingPreferences sp, bool create)
	{
		SqlSmoObject.ThrowIfBelowVersion100(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (create && sp.IncludeScripts.ExistenceCheck)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_AUDIT, new object[2]
			{
				"NOT",
				FormatFullNameForScripting(sp, nameIsIndentifier: false)
			});
			stringBuilder.AppendLine();
		}
		stringBuilder.Append(create ? "CREATE SERVER AUDIT " : "ALTER SERVER AUDIT ");
		stringBuilder.Append(FormatFullNameForScripting(sp));
		stringBuilder.AppendLine();
		Property property = base.Properties.Get("DestinationType");
		if (create || property.Dirty || NeedToScriptTO())
		{
			stringBuilder.Append("TO ");
			if (property.IsNull)
			{
				throw new PropertyNotSetException("DestinationType");
			}
			switch ((AuditDestinationType)property.Value)
			{
			case AuditDestinationType.File:
				stringBuilder.Append("FILE ");
				stringBuilder.Append(ScriptFileOptions(create, property.Dirty, sp));
				break;
			case AuditDestinationType.ApplicationLog:
				stringBuilder.Append("APPLICATION_LOG");
				break;
			case AuditDestinationType.SecurityLog:
				stringBuilder.Append("SECURITY_LOG");
				break;
			case AuditDestinationType.Url:
				if (sp.TargetDatabaseEngineEdition != DatabaseEngineEdition.SqlManagedInstance)
				{
					throw new NotSupportedException(ExceptionTemplatesImpl.InvalidPropertyValueForVersion(GetType().Name, "DestinationType", property.Value.ToString(), GetSqlServerVersionName()));
				}
				stringBuilder.Append("URL ");
				stringBuilder.Append(scriptUrlOptions(create));
				break;
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("DestinationType"));
			}
		}
		stringBuilder.Append(ScriptAuditOptions(create, sp));
		if (IsSupportedProperty("Filter"))
		{
			Property propertyOptional = GetPropertyOptional("Filter");
			string value = propertyOptional.Value as string;
			if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version110 && sp.TargetDatabaseEngineType == DatabaseEngineType.Standalone)
			{
				if ((create || propertyOptional.Dirty) && !string.IsNullOrEmpty(value))
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("WHERE ");
					stringBuilder.Append(value);
				}
				else if (!create && propertyOptional.Dirty)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("REMOVE WHERE");
				}
			}
			else if (!string.IsNullOrEmpty(value))
			{
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.PropertySupportedOnlyOn110("Filter")).SetHelpContext("SupportedOnlyOn110");
			}
		}
		if (create)
		{
			Property property2 = base.Properties.Get("Enabled");
			if (!property2.IsNull)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "ALTER SERVER AUDIT {0} WITH (STATE = {1})", new object[2]
				{
					FullQualifiedName,
					((bool)property2.Value) ? "ON" : "OFF"
				});
			}
			query.Add(stringBuilder.ToString());
		}
		else if (base.Properties.Dirty || NeedToScriptTO())
		{
			query.Add(stringBuilder.ToString());
		}
	}

	private bool NeedToScriptTO()
	{
		Property property = base.Properties.Get("FilePath");
		Property property2 = base.Properties.Get("MaximumRolloverFiles");
		Property property3 = base.Properties.Get("ReserveDiskSpace");
		Property property4 = base.Properties.Get("MaximumFileSize");
		Property property5 = base.Properties.Get("MaximumFileSizeUnit");
		bool flag = property.Dirty || property2.Dirty || property3.Dirty || property4.Dirty || property5.Dirty;
		if (IsSupportedProperty("MaximumFiles"))
		{
			Property propertyOptional = GetPropertyOptional("MaximumFiles");
			flag = flag || propertyOptional.Dirty;
		}
		if (IsSupportedProperty("RetentionDays"))
		{
			Property property6 = properties.Get("RetentionDays");
			flag = flag || property6.Dirty;
		}
		return flag;
	}

	private string ScriptAuditOptions(bool create, ScriptingPreferences sp)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Property property = base.Properties.Get("QueueDelay");
		bool flag = false;
		if (property.Dirty || (create && !property.IsNull))
		{
			flag = true;
			stringBuilder.AppendLine();
			stringBuilder.Append("WITH");
			stringBuilder.AppendLine();
			stringBuilder.Append(Globals.LParen);
			stringBuilder.Append(Globals.tab);
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "QUEUE_DELAY = {0}", new object[1] { property.Value.ToString() });
		}
		Property property2 = base.Properties.Get("OnFailure");
		if (property2.Dirty || (create && !property2.IsNull))
		{
			stringBuilder.AppendLine();
			if (flag)
			{
				stringBuilder.Append(Globals.tab);
				stringBuilder.Append(Globals.comma);
			}
			else
			{
				flag = true;
				stringBuilder.Append("WITH");
				stringBuilder.AppendLine();
				stringBuilder.Append(Globals.LParen);
				stringBuilder.Append(Globals.tab);
			}
			stringBuilder.Append("ON_FAILURE = ");
			switch ((OnFailureAction)property2.Value)
			{
			case OnFailureAction.Continue:
				stringBuilder.Append("CONTINUE");
				break;
			case OnFailureAction.Shutdown:
				stringBuilder.Append("SHUTDOWN");
				break;
			case OnFailureAction.FailOperation:
				if (sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version110 && sp.TargetDatabaseEngineType == DatabaseEngineType.Standalone)
				{
					stringBuilder.Append("FAIL_OPERATION");
					break;
				}
				throw new UnsupportedVersionException(ExceptionTemplatesImpl.PropertyValueSupportedOnlyOn110("OnFailure", "FailOperation")).SetHelpContext("PropertyValueSupportedOnlyOn110");
			default:
				throw new ArgumentException(ExceptionTemplatesImpl.UnknownEnumeration("OnFailure"));
			}
		}
		Property property3 = base.Properties.Get("Guid");
		if (property3.Dirty || (create && !property3.IsNull))
		{
			stringBuilder.AppendLine();
			if (flag)
			{
				stringBuilder.Append(Globals.tab);
				stringBuilder.Append(Globals.comma);
			}
			else
			{
				flag = true;
				stringBuilder.Append("WITH");
				stringBuilder.AppendLine();
				stringBuilder.Append(Globals.LParen);
				stringBuilder.Append(Globals.tab);
			}
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "AUDIT_GUID = '{0}'", new object[1] { property3.Value.ToString() });
		}
		if (flag)
		{
			stringBuilder.AppendLine();
			stringBuilder.Append(Globals.RParen);
		}
		return stringBuilder.ToString();
	}

	private string ScriptFileOptions(bool create, bool mustHaveFilePath, ScriptingPreferences sp)
	{
		bool needComma = false;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		stringBuilder.Append(Globals.LParen);
		stringBuilder.Append(Globals.tab);
		Property property = base.Properties.Get("FilePath");
		if ((create || mustHaveFilePath) && property.IsNull)
		{
			throw new PropertyNotSetException("FilePath");
		}
		if (property.Dirty || create || mustHaveFilePath)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "FILEPATH = {0}", new object[1] { SqlSmoObject.MakeSqlString(property.Value.ToString()) });
			stringBuilder.AppendLine();
			needComma = true;
		}
		Property property2 = base.Properties.Get("MaximumFileSize");
		Property property3 = base.Properties.Get("MaximumFileSizeUnit");
		if (!property2.IsNull && (create || property2.Dirty || (!property3.IsNull && property3.Dirty)))
		{
			AppendCommaOption(stringBuilder, "MAXSIZE = {0} ", property2.Value.ToString(), appendLine: false, ref needComma);
			if (property3.IsNull)
			{
				stringBuilder.Append(AuditFileSizeUnit.Mb.ToString().ToUpperInvariant());
			}
			else
			{
				if (!Enum.IsDefined(typeof(AuditFileSizeUnit), property3.Value))
				{
					throw new WrongPropertyValueException(ExceptionTemplatesImpl.UnknownEnumeration("AuditFileSizeUnit"));
				}
				stringBuilder.Append(property3.Value.ToString().ToUpperInvariant());
			}
			stringBuilder.AppendLine();
		}
		Property property4 = base.Properties.Get("MaximumRolloverFiles");
		bool flag = false;
		if (IsSupportedProperty("MaximumFiles"))
		{
			Property propertyOptional = GetPropertyOptional("MaximumFiles");
			if (property4.Dirty && propertyOptional.Dirty && (long)property4.Value != int.MaxValue && (int)propertyOptional.Value != 0)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.MutuallyExclusiveProperties("MaximumRolloverFiles", "MaximumFiles"));
			}
			if ((propertyOptional.Dirty || (create && !propertyOptional.IsNull)) && ((int)propertyOptional.Value != 0 || (!property4.Dirty && !create)))
			{
				if (sp.TargetServerVersionInternal < SqlServerVersionInternal.Version110 || sp.TargetDatabaseEngineType != DatabaseEngineType.Standalone)
				{
					throw new UnsupportedVersionException(ExceptionTemplatesImpl.PropertySupportedOnlyOn110("MaximumFiles")).SetHelpContext("SupportedOnlyOn110");
				}
				AppendCommaOption(stringBuilder, "MAX_FILES = {0}", propertyOptional.Value.ToString(), appendLine: true, ref needComma);
				flag = true;
			}
		}
		if ((property4.Dirty || (create && !property4.IsNull)) && !flag)
		{
			AppendCommaOption(stringBuilder, "MAX_ROLLOVER_FILES = {0}", property4.Value.ToString(), appendLine: true, ref needComma);
		}
		Property property5 = base.Properties.Get("ReserveDiskSpace");
		if (property5.Dirty || (create && !property5.IsNull))
		{
			AppendCommaOption(stringBuilder, "RESERVE_DISK_SPACE = {0}", ((bool)property5.Value) ? "ON" : "OFF", appendLine: true, ref needComma);
		}
		stringBuilder.Append(Globals.RParen);
		return stringBuilder.ToString();
	}

	private string scriptUrlOptions(bool create)
	{
		ScriptStringBuilder scriptStringBuilder = new ScriptStringBuilder(string.Empty);
		Property property = base.Properties.Get("FilePath");
		if (create && property.IsNull)
		{
			throw new PropertyNotSetException("FilePath");
		}
		if (property.Dirty || create)
		{
			scriptStringBuilder.SetParameter("PATH", SqlSmoObject.MakeSqlString(property.Value.ToString()), ParameterValueFormat.NotString);
		}
		if (IsSupportedProperty("RetentionDays"))
		{
			Property property2 = properties.Get("RetentionDays");
			if ((int)property2.Value < 0)
			{
				throw new WrongPropertyValueException(ExceptionTemplatesImpl.WrongPropertyValueException("RetentionDays", property2.Value.ToString()));
			}
			if (property2.Dirty && !property2.IsNull)
			{
				scriptStringBuilder.SetParameter("RETENTION_DAYS", property2.Value.ToString(), ParameterValueFormat.NotString);
			}
		}
		return scriptStringBuilder.ToString(scriptSemiColon: false);
	}

	private void AppendCommaOption(StringBuilder sb, string optionName, string optionValue, bool appendLine, ref bool needComma)
	{
		sb.Append(Globals.tab);
		if (needComma)
		{
			sb.Append(Globals.comma);
		}
		else
		{
			needComma = true;
		}
		sb.AppendFormat(SmoApplication.DefaultCulture, optionName, new object[1] { optionValue });
		if (appendLine)
		{
			sb.AppendLine();
		}
	}
}

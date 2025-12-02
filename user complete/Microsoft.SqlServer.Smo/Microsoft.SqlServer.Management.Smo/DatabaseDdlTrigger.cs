using System;
using System.Collections.Specialized;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Dmf;
using Microsoft.SqlServer.Management.Facets;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Sdk.Sfc.Metadata;

namespace Microsoft.SqlServer.Management.Smo;

[PhysicalFacet(PhysicalFacetOptions.ReadOnly)]
[SfcElement(SfcElementFlags.Standalone | SfcElementFlags.SqlAzureDatabase)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
[SfcElementType("DdlTrigger")]
public sealed class DatabaseDdlTrigger : DdlTriggerBase, ISfcSupportsDesignMode, IExtendedProperties
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 18, 19, 19, 19, 19, 19, 19, 19 };

		private static int[] cloudVersionCount = new int[3] { 15, 15, 18 };

		private static int sqlDwPropertyCount = 0;

		internal static StaticMetadata[] sqlDwStaticMetadata = new StaticMetadata[0];

		internal static StaticMetadata[] cloudStaticMetadata = new StaticMetadata[18]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DdlTriggerEvents", expensive: true, readOnly: false, typeof(DatabaseDdlTriggerEventSet)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(DatabaseDdlTriggerExecutionContext)),
			new StaticMetadata("ExecutionContextUser", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string))
		};

		internal static StaticMetadata[] staticMetadata = new StaticMetadata[19]
		{
			new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
			new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
			new StaticMetadata("DdlTriggerEvents", expensive: true, readOnly: false, typeof(DatabaseDdlTriggerEventSet)),
			new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(DatabaseDdlTriggerExecutionContext)),
			new StaticMetadata("ExecutionContextUser", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
			new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
			new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
			new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
			new StaticMetadata("NotForReplication", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
			new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
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
					"AnsiNullsStatus" => 0, 
					"BodyStartIndex" => 1, 
					"CreateDate" => 2, 
					"DateLastModified" => 3, 
					"DdlTriggerEvents" => 4, 
					"ExecutionContext" => 5, 
					"ExecutionContextUser" => 6, 
					"ID" => 7, 
					"ImplementationType" => 8, 
					"IsEnabled" => 9, 
					"IsEncrypted" => 10, 
					"IsSystemObject" => 11, 
					"NotForReplication" => 12, 
					"QuotedIdentifierStatus" => 13, 
					"Text" => 14, 
					"AssemblyName" => 15, 
					"ClassName" => 16, 
					"MethodName" => 17, 
					_ => -1, 
				};
			}
			return propertyName switch
			{
				"AnsiNullsStatus" => 0, 
				"AssemblyName" => 1, 
				"BodyStartIndex" => 2, 
				"ClassName" => 3, 
				"CreateDate" => 4, 
				"DateLastModified" => 5, 
				"DdlTriggerEvents" => 6, 
				"ExecutionContext" => 7, 
				"ExecutionContextUser" => 8, 
				"ID" => 9, 
				"ImplementationType" => 10, 
				"IsEnabled" => 11, 
				"IsEncrypted" => 12, 
				"IsSystemObject" => 13, 
				"MethodName" => 14, 
				"NotForReplication" => 15, 
				"QuotedIdentifierStatus" => 16, 
				"Text" => 17, 
				"PolicyHealthState" => 18, 
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

	internal enum DatabaseDdlTriggerEventValues
	{
		AddRoleMember,
		AddSensitivityClassification,
		AddSignature,
		AddSignatureSchemaObject,
		AlterApplicationRole,
		AlterAssembly,
		AlterAsymmetricKey,
		AlterAudit,
		AlterAuthorizationDatabase,
		AlterBrokerPriority,
		AlterCertificate,
		AlterColumnEncryptionKey,
		AlterDatabaseAuditSpecification,
		AlterDatabaseEncryptionKey,
		AlterDatabaseScopedConfiguration,
		AlterExtendedProperty,
		AlterExternalLibrary,
		AlterFulltextCatalog,
		AlterFulltextIndex,
		AlterFulltextStoplist,
		AlterFunction,
		AlterIndex,
		AlterMasterKey,
		AlterMessageType,
		AlterPartitionFunction,
		AlterPartitionScheme,
		AlterPlanGuide,
		AlterProcedure,
		AlterQueue,
		AlterRemoteServiceBinding,
		AlterRole,
		AlterRoute,
		AlterSchema,
		AlterSearchPropertyList,
		AlterSecurityPolicy,
		AlterSequence,
		AlterService,
		AlterSymmetricKey,
		AlterTable,
		AlterTrigger,
		AlterUser,
		AlterView,
		AlterXmlSchemaCollection,
		BindDefault,
		BindRule,
		CreateApplicationRole,
		CreateAssembly,
		CreateAsymmetricKey,
		CreateAudit,
		CreateBrokerPriority,
		CreateCertificate,
		CreateColumnEncryptionKey,
		CreateColumnMasterKey,
		CreateContract,
		CreateDatabaseAuditSpecification,
		CreateDatabaseEncryptionKey,
		CreateDefault,
		CreateEventNotification,
		CreateExtendedProperty,
		CreateExternalLibrary,
		CreateFulltextCatalog,
		CreateFulltextIndex,
		CreateFulltextStoplist,
		CreateFunction,
		CreateIndex,
		CreateMasterKey,
		CreateMessageType,
		CreatePartitionFunction,
		CreatePartitionScheme,
		CreatePlanGuide,
		CreateProcedure,
		CreateQueue,
		CreateRemoteServiceBinding,
		CreateRole,
		CreateRoute,
		CreateRule,
		CreateSchema,
		CreateSearchPropertyList,
		CreateSecurityPolicy,
		CreateSequence,
		CreateService,
		CreateSpatialIndex,
		CreateStatistics,
		CreateSymmetricKey,
		CreateSynonym,
		CreateTable,
		CreateTrigger,
		CreateType,
		CreateUser,
		CreateView,
		CreateXmlIndex,
		CreateXmlSchemaCollection,
		DenyDatabase,
		DropApplicationRole,
		DropAssembly,
		DropAsymmetricKey,
		DropAudit,
		DropBrokerPriority,
		DropCertificate,
		DropColumnEncryptionKey,
		DropColumnMasterKey,
		DropContract,
		DropDatabaseAuditSpecification,
		DropDatabaseEncryptionKey,
		DropDefault,
		DropEventNotification,
		DropExtendedProperty,
		DropExternalLibrary,
		DropFulltextCatalog,
		DropFulltextIndex,
		DropFulltextStoplist,
		DropFunction,
		DropIndex,
		DropMasterKey,
		DropMessageType,
		DropPartitionFunction,
		DropPartitionScheme,
		DropPlanGuide,
		DropProcedure,
		DropQueue,
		DropRemoteServiceBinding,
		DropRole,
		DropRoleMember,
		DropRoute,
		DropRule,
		DropSchema,
		DropSearchPropertyList,
		DropSecurityPolicy,
		DropSensitivityClassification,
		DropSequence,
		DropService,
		DropSignature,
		DropSignatureSchemaObject,
		DropStatistics,
		DropSymmetricKey,
		DropSynonym,
		DropTable,
		DropTrigger,
		DropType,
		DropUser,
		DropView,
		DropXmlSchemaCollection,
		GrantDatabase,
		Rename,
		RevokeDatabase,
		UnbindDefault,
		UnbindRule,
		UpdateStatistics
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool AnsiNullsStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("AnsiNullsStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AnsiNullsStatus", value);
		}
	}

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcReference(typeof(SqlAssembly), "Server[@Name = '{0}']/Database[@Name = '{1}']/SqlAssembly[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "AssemblyName" })]
	public string AssemblyName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("AssemblyName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("AssemblyName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int BodyStartIndex => (int)base.Properties.GetValueWithNullReplacement("BodyStartIndex");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string ClassName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ClassName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ClassName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public DatabaseDdlTriggerExecutionContext ExecutionContext
	{
		get
		{
			return (DatabaseDdlTriggerExecutionContext)base.Properties.GetValueWithNullReplacement("ExecutionContext");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContext", value);
		}
	}

	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	[SfcReference(typeof(User), "Server[@Name = '{0}']/Database[@Name = '{1}']/User[@Name = '{2}']", new string[] { "Parent.Parent.ConnectionContext.TrueName", "Parent.Name", "ExecutionContextUser" })]
	public string ExecutionContextUser
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ExecutionContextUser");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContextUser", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public ImplementationType ImplementationType
	{
		get
		{
			return (ImplementationType)base.Properties.GetValueWithNullReplacement("ImplementationType");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ImplementationType", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsEnabled
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEnabled");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEnabled", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Deploy)]
	public bool IsEncrypted
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("IsEncrypted");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("IsEncrypted", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	public string MethodName
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("MethodName");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("MethodName", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool NotForReplication
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("NotForReplication");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("NotForReplication", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public bool QuotedIdentifierStatus
	{
		get
		{
			return (bool)base.Properties.GetValueWithNullReplacement("QuotedIdentifierStatus");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("QuotedIdentifierStatus", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public string Text => (string)base.Properties.GetValueWithNullReplacement("Text");

	[SfcProperty(SfcPropertyFlags.ReadOnlyAfterCreation | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase | SfcPropertyFlags.Design)]
	[SfcKey(0)]
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

	public static string UrnSuffix => "DdlTrigger";

	public DatabaseDdlTriggerEventSet DdlTriggerEvents
	{
		get
		{
			if (base.State == SqlSmoState.Creating)
			{
				Property property = base.Properties.Get("DdlTriggerEvents");
				if (property.Value == null)
				{
					property.Value = new DatabaseDdlTriggerEventSet();
				}
			}
			return (DatabaseDdlTriggerEventSet)base.Properties.GetValueWithNullReplacement("DdlTriggerEvents");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DdlTriggerEvents", value);
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public override string TextBody
	{
		get
		{
			return base.TextBody;
		}
		set
		{
			base.TextBody = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public override string TextHeader
	{
		get
		{
			return base.TextHeader;
		}
		set
		{
			base.TextHeader = value;
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone | SfcPropertyFlags.SqlAzureDatabase)]
	public override bool TextMode
	{
		get
		{
			return base.TextMode;
		}
		set
		{
			base.TextMode = value;
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	private string StringFromDatabaseDdlTriggerEvent(int evt)
	{
		return evt switch
		{
			0 => "ADD_ROLE_MEMBER", 
			1 => "ADD_SENSITIVITY_CLASSIFICATION", 
			2 => "ADD_SIGNATURE", 
			3 => "ADD_SIGNATURE_SCHEMA_OBJECT", 
			4 => "ALTER_APPLICATION_ROLE", 
			5 => "ALTER_ASSEMBLY", 
			6 => "ALTER_ASYMMETRIC_KEY", 
			7 => "ALTER_AUDIT", 
			8 => "ALTER_AUTHORIZATION_DATABASE", 
			9 => "ALTER_BROKER_PRIORITY", 
			10 => "ALTER_CERTIFICATE", 
			11 => "ALTER_COLUMN_ENCRYPTION_KEY", 
			12 => "ALTER_DATABASE_AUDIT_SPECIFICATION", 
			13 => "ALTER_DATABASE_ENCRYPTION_KEY", 
			14 => "ALTER_DATABASE_SCOPED_CONFIGURATION", 
			15 => "ALTER_EXTENDED_PROPERTY", 
			16 => "ALTER_EXTERNAL_LIBRARY", 
			17 => "ALTER_FULLTEXT_CATALOG", 
			18 => "ALTER_FULLTEXT_INDEX", 
			19 => "ALTER_FULLTEXT_STOPLIST", 
			20 => "ALTER_FUNCTION", 
			21 => "ALTER_INDEX", 
			22 => "ALTER_MASTER_KEY", 
			23 => "ALTER_MESSAGE_TYPE", 
			24 => "ALTER_PARTITION_FUNCTION", 
			25 => "ALTER_PARTITION_SCHEME", 
			26 => "ALTER_PLAN_GUIDE", 
			27 => "ALTER_PROCEDURE", 
			28 => "ALTER_QUEUE", 
			29 => "ALTER_REMOTE_SERVICE_BINDING", 
			30 => "ALTER_ROLE", 
			31 => "ALTER_ROUTE", 
			32 => "ALTER_SCHEMA", 
			33 => "ALTER_SEARCH_PROPERTY_LIST", 
			34 => "ALTER_SECURITY_POLICY", 
			35 => "ALTER_SEQUENCE", 
			36 => "ALTER_SERVICE", 
			37 => "ALTER_SYMMETRIC_KEY", 
			38 => "ALTER_TABLE", 
			39 => "ALTER_TRIGGER", 
			40 => "ALTER_USER", 
			41 => "ALTER_VIEW", 
			42 => "ALTER_XML_SCHEMA_COLLECTION", 
			43 => "BIND_DEFAULT", 
			44 => "BIND_RULE", 
			45 => "CREATE_APPLICATION_ROLE", 
			46 => "CREATE_ASSEMBLY", 
			47 => "CREATE_ASYMMETRIC_KEY", 
			48 => "CREATE_AUDIT", 
			49 => "CREATE_BROKER_PRIORITY", 
			50 => "CREATE_CERTIFICATE", 
			51 => "CREATE_COLUMN_ENCRYPTION_KEY", 
			52 => "CREATE_COLUMN_MASTER_KEY", 
			53 => "CREATE_CONTRACT", 
			54 => "CREATE_DATABASE_AUDIT_SPECIFICATION", 
			55 => "CREATE_DATABASE_ENCRYPTION_KEY", 
			56 => "CREATE_DEFAULT", 
			57 => "CREATE_EVENT_NOTIFICATION", 
			58 => "CREATE_EXTENDED_PROPERTY", 
			59 => "CREATE_EXTERNAL_LIBRARY", 
			60 => "CREATE_FULLTEXT_CATALOG", 
			61 => "CREATE_FULLTEXT_INDEX", 
			62 => "CREATE_FULLTEXT_STOPLIST", 
			63 => "CREATE_FUNCTION", 
			64 => "CREATE_INDEX", 
			65 => "CREATE_MASTER_KEY", 
			66 => "CREATE_MESSAGE_TYPE", 
			67 => "CREATE_PARTITION_FUNCTION", 
			68 => "CREATE_PARTITION_SCHEME", 
			69 => "CREATE_PLAN_GUIDE", 
			70 => "CREATE_PROCEDURE", 
			71 => "CREATE_QUEUE", 
			72 => "CREATE_REMOTE_SERVICE_BINDING", 
			73 => "CREATE_ROLE", 
			74 => "CREATE_ROUTE", 
			75 => "CREATE_RULE", 
			76 => "CREATE_SCHEMA", 
			77 => "CREATE_SEARCH_PROPERTY_LIST", 
			78 => "CREATE_SECURITY_POLICY", 
			79 => "CREATE_SEQUENCE", 
			80 => "CREATE_SERVICE", 
			81 => "CREATE_SPATIAL_INDEX", 
			82 => "CREATE_STATISTICS", 
			83 => "CREATE_SYMMETRIC_KEY", 
			84 => "CREATE_SYNONYM", 
			85 => "CREATE_TABLE", 
			86 => "CREATE_TRIGGER", 
			87 => "CREATE_TYPE", 
			88 => "CREATE_USER", 
			89 => "CREATE_VIEW", 
			90 => "CREATE_XML_INDEX", 
			91 => "CREATE_XML_SCHEMA_COLLECTION", 
			92 => "DENY_DATABASE", 
			93 => "DROP_APPLICATION_ROLE", 
			94 => "DROP_ASSEMBLY", 
			95 => "DROP_ASYMMETRIC_KEY", 
			96 => "DROP_AUDIT", 
			97 => "DROP_BROKER_PRIORITY", 
			98 => "DROP_CERTIFICATE", 
			99 => "DROP_COLUMN_ENCRYPTION_KEY", 
			100 => "DROP_COLUMN_MASTER_KEY", 
			101 => "DROP_CONTRACT", 
			102 => "DROP_DATABASE_AUDIT_SPECIFICATION", 
			103 => "DROP_DATABASE_ENCRYPTION_KEY", 
			104 => "DROP_DEFAULT", 
			105 => "DROP_EVENT_NOTIFICATION", 
			106 => "DROP_EXTENDED_PROPERTY", 
			107 => "DROP_EXTERNAL_LIBRARY", 
			108 => "DROP_FULLTEXT_CATALOG", 
			109 => "DROP_FULLTEXT_INDEX", 
			110 => "DROP_FULLTEXT_STOPLIST", 
			111 => "DROP_FUNCTION", 
			112 => "DROP_INDEX", 
			113 => "DROP_MASTER_KEY", 
			114 => "DROP_MESSAGE_TYPE", 
			115 => "DROP_PARTITION_FUNCTION", 
			116 => "DROP_PARTITION_SCHEME", 
			117 => "DROP_PLAN_GUIDE", 
			118 => "DROP_PROCEDURE", 
			119 => "DROP_QUEUE", 
			120 => "DROP_REMOTE_SERVICE_BINDING", 
			121 => "DROP_ROLE", 
			122 => "DROP_ROLE_MEMBER", 
			123 => "DROP_ROUTE", 
			124 => "DROP_RULE", 
			125 => "DROP_SCHEMA", 
			126 => "DROP_SEARCH_PROPERTY_LIST", 
			127 => "DROP_SECURITY_POLICY", 
			128 => "DROP_SENSITIVITY_CLASSIFICATION", 
			129 => "DROP_SEQUENCE", 
			130 => "DROP_SERVICE", 
			131 => "DROP_SIGNATURE", 
			132 => "DROP_SIGNATURE_SCHEMA_OBJECT", 
			133 => "DROP_STATISTICS", 
			134 => "DROP_SYMMETRIC_KEY", 
			135 => "DROP_SYNONYM", 
			136 => "DROP_TABLE", 
			137 => "DROP_TRIGGER", 
			138 => "DROP_TYPE", 
			139 => "DROP_USER", 
			140 => "DROP_VIEW", 
			141 => "DROP_XML_SCHEMA_COLLECTION", 
			142 => "GRANT_DATABASE", 
			143 => "RENAME", 
			144 => "REVOKE_DATABASE", 
			145 => "UNBIND_DEFAULT", 
			146 => "UNBIND_RULE", 
			147 => "UPDATE_STATISTICS", 
			_ => throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration("DatabaseDdlTriggerEvent")), 
		};
	}

	internal DatabaseDdlTrigger(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public DatabaseDdlTrigger()
	{
	}

	public DatabaseDdlTrigger(Database database, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = database;
	}

	public DatabaseDdlTrigger(Database parent, string name, DatabaseDdlTriggerEventSet events, string textBody)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		base.Properties.Get("DdlTriggerEvents").Value = events;
		TextBody = textBody;
		base.Properties.Get("ImplementationType").Value = ImplementationType.TransactSql;
	}

	public DatabaseDdlTrigger(Database parent, string name, DatabaseDdlTriggerEventSet events, string assemblyName, string className, string method)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		base.Properties.Get("DdlTriggerEvents").Value = events;
		base.Properties.Get("ImplementationType").Value = ImplementationType.SqlClr;
		base.Properties.Get("AssemblyName").Value = assemblyName;
		base.Properties.Get("ClassName").Value = className;
		base.Properties.Get("MethodName").Value = method;
	}

	internal override void ScriptDrop(StringCollection queries, ScriptingPreferences sp)
	{
		this.ThrowIfNotSupported(GetType(), sp);
		CheckObjectState();
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		string text = FormatFullNameForScripting(sp);
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendFormat(SmoApplication.DefaultCulture, GetIfNotExistStatement(sp, ""));
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP TRIGGER {0}{1} ON DATABASE", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	internal override void AddDdlTriggerEvents(StringBuilder sb, ScriptingPreferences sp)
	{
		DatabaseDdlTriggerEventSet propValueOptional = GetPropValueOptional("DdlTriggerEvents", new DatabaseDdlTriggerEventSet());
		int num = 0;
		for (int i = 0; i < propValueOptional.NumberOfElements; i++)
		{
			if (propValueOptional.GetBitAt(i))
			{
				if (num++ > 0)
				{
					sb.Append(Globals.commaspace);
				}
				sb.Append(StringFromDatabaseDdlTriggerEvent(i));
			}
		}
		if (num == 0)
		{
			throw new PropertyNotSetException("DdlTriggerEvents");
		}
	}

	internal override PropagateInfo[] GetPropagateInfo(PropagateAction action)
	{
		if (DatabaseEngineType.SqlAzureDatabase == DatabaseEngineType)
		{
			return null;
		}
		return new PropagateInfo[1]
		{
			new PropagateInfo((base.ServerVersion.Major < 8) ? null : ExtendedProperties, bWithScript: true, ExtendedProperty.UrnSuffix)
		};
	}

	internal override void ValidateProperty(Property prop, object value)
	{
		if (prop.Name == "IsEncrypted")
		{
			ScriptNameObjectBase.Validate_set_TextObjectDDLProperty(prop, value);
		}
	}

	protected override bool IsObjectDirty()
	{
		if (!base.IsObjectDirty())
		{
			return IsEventSetDirty();
		}
		return true;
	}

	protected override void CleanObject()
	{
		base.CleanObject();
		DatabaseDdlTriggerEventSet databaseDdlTriggerEventSet = (DatabaseDdlTriggerEventSet)base.Properties.Get("DdlTriggerEvents").Value;
		if (databaseDdlTriggerEventSet != null)
		{
			databaseDdlTriggerEventSet.Dirty = false;
		}
	}

	protected override bool IsEventSetDirty()
	{
		bool result = false;
		DatabaseDdlTriggerEventSet databaseDdlTriggerEventSet = (DatabaseDdlTriggerEventSet)base.Properties.Get("DdlTriggerEvents").Value;
		if (databaseDdlTriggerEventSet != null)
		{
			result = databaseDdlTriggerEventSet.Dirty;
		}
		return result;
	}

	internal override string GetIfNotExistStatement(ScriptingPreferences sp, string prefix)
	{
		return string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_DATABASE_DDL_TRIGGER, new object[2]
		{
			prefix,
			FormatFullNameForScripting(sp, nameIsIndentifier: false)
		});
	}

	internal static string[] GetScriptFields(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode)
	{
		return new string[1] { "IsSystemObject" };
	}

	internal static string[] GetScriptFields2(Type parentType, ServerVersion version, DatabaseEngineType databaseEngineType, DatabaseEngineEdition databaseEngineEdition, bool defaultTextMode, ScriptingPreferences sp)
	{
		return new string[1] { "Text" };
	}
}

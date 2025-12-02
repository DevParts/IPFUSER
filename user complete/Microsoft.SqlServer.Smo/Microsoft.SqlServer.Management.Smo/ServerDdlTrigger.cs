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
[SfcElementType("DdlTrigger")]
[SfcElement(SfcElementFlags.Standalone)]
[EvaluationMode(/*Could not decode attribute arguments.*/)]
public sealed class ServerDdlTrigger : DdlTriggerBase, ISfcSupportsDesignMode
{
	internal class PropertyMetadataProvider : SqlPropertyMetadataProvider
	{
		private static int[] versionCount = new int[10] { 0, 0, 17, 18, 18, 18, 18, 18, 18, 18 };

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
				"AnsiNullsStatus" => 0, 
				"AssemblyName" => 1, 
				"BodyStartIndex" => 2, 
				"ClassName" => 3, 
				"CreateDate" => 4, 
				"DateLastModified" => 5, 
				"DdlTriggerEvents" => 6, 
				"ExecutionContext" => 7, 
				"ExecutionContextLogin" => 8, 
				"ID" => 9, 
				"ImplementationType" => 10, 
				"IsEnabled" => 11, 
				"IsEncrypted" => 12, 
				"IsSystemObject" => 13, 
				"MethodName" => 14, 
				"QuotedIdentifierStatus" => 15, 
				"Text" => 16, 
				"PolicyHealthState" => 17, 
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
				new StaticMetadata("AnsiNullsStatus", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("AssemblyName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("BodyStartIndex", expensive: true, readOnly: true, typeof(int)),
				new StaticMetadata("ClassName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("CreateDate", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DateLastModified", expensive: false, readOnly: true, typeof(DateTime)),
				new StaticMetadata("DdlTriggerEvents", expensive: true, readOnly: false, typeof(ServerDdlTriggerEventSet)),
				new StaticMetadata("ExecutionContext", expensive: false, readOnly: false, typeof(ServerDdlTriggerExecutionContext)),
				new StaticMetadata("ExecutionContextLogin", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("ID", expensive: false, readOnly: true, typeof(int)),
				new StaticMetadata("ImplementationType", expensive: false, readOnly: false, typeof(ImplementationType)),
				new StaticMetadata("IsEnabled", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsEncrypted", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("IsSystemObject", expensive: false, readOnly: true, typeof(bool)),
				new StaticMetadata("MethodName", expensive: false, readOnly: false, typeof(string)),
				new StaticMetadata("QuotedIdentifierStatus", expensive: false, readOnly: false, typeof(bool)),
				new StaticMetadata("Text", expensive: true, readOnly: true, typeof(string)),
				new StaticMetadata("PolicyHealthState", expensive: true, readOnly: false, typeof(PolicyHealthState))
			};
		}
	}

	internal enum ServerDdlTriggerEventValues
	{
		AddRoleMember,
		AddSensitivityClassification,
		AddServerRoleMember,
		AddSignature,
		AddSignatureSchemaObject,
		AlterApplicationRole,
		AlterAssembly,
		AlterAsymmetricKey,
		AlterAudit,
		AlterAuthorizationDatabase,
		AlterAuthorizationServer,
		AlterAvailabilityGroup,
		AlterBrokerPriority,
		AlterCertificate,
		AlterColumnEncryptionKey,
		AlterCredential,
		AlterCryptographicProvider,
		AlterDatabase,
		AlterDatabaseAuditSpecification,
		AlterDatabaseEncryptionKey,
		AlterDatabaseScopedConfiguration,
		AlterEndpoint,
		AlterEventSession,
		AlterExtendedProperty,
		AlterExternalLibrary,
		AlterExternalResourcePool,
		AlterFulltextCatalog,
		AlterFulltextIndex,
		AlterFulltextStoplist,
		AlterFunction,
		AlterIndex,
		AlterInstance,
		AlterLinkedServer,
		AlterLogin,
		AlterMasterKey,
		AlterMessage,
		AlterMessageType,
		AlterPartitionFunction,
		AlterPartitionScheme,
		AlterPlanGuide,
		AlterProcedure,
		AlterQueue,
		AlterRemoteServer,
		AlterRemoteServiceBinding,
		AlterResourceGovernorConfig,
		AlterResourcePool,
		AlterRole,
		AlterRoute,
		AlterSchema,
		AlterSearchPropertyList,
		AlterSecurityPolicy,
		AlterSequence,
		AlterServerAudit,
		AlterServerAuditSpecification,
		AlterServerRole,
		AlterService,
		AlterServiceMasterKey,
		AlterSymmetricKey,
		AlterTable,
		AlterTrigger,
		AlterUser,
		AlterView,
		AlterWorkloadGroup,
		AlterXmlSchemaCollection,
		BindDefault,
		BindRule,
		CreateApplicationRole,
		CreateAssembly,
		CreateAsymmetricKey,
		CreateAudit,
		CreateAvailabilityGroup,
		CreateBrokerPriority,
		CreateCertificate,
		CreateColumnEncryptionKey,
		CreateColumnMasterKey,
		CreateContract,
		CreateCredential,
		CreateCryptographicProvider,
		CreateDatabase,
		CreateDatabaseAuditSpecification,
		CreateDatabaseEncryptionKey,
		CreateDefault,
		CreateEndpoint,
		CreateEventNotification,
		CreateEventSession,
		CreateExtendedProcedure,
		CreateExtendedProperty,
		CreateExternalLibrary,
		CreateExternalResourcePool,
		CreateFulltextCatalog,
		CreateFulltextIndex,
		CreateFulltextStoplist,
		CreateFunction,
		CreateIndex,
		CreateLinkedServer,
		CreateLinkedServerLogin,
		CreateLogin,
		CreateMasterKey,
		CreateMessage,
		CreateMessageType,
		CreatePartitionFunction,
		CreatePartitionScheme,
		CreatePlanGuide,
		CreateProcedure,
		CreateQueue,
		CreateRemoteServer,
		CreateRemoteServiceBinding,
		CreateResourcePool,
		CreateRole,
		CreateRoute,
		CreateRule,
		CreateSchema,
		CreateSearchPropertyList,
		CreateSecurityPolicy,
		CreateSequence,
		CreateServerAudit,
		CreateServerAuditSpecification,
		CreateServerRole,
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
		CreateWorkloadGroup,
		CreateXmlIndex,
		CreateXmlSchemaCollection,
		DenyDatabase,
		DenyServer,
		DropApplicationRole,
		DropAssembly,
		DropAsymmetricKey,
		DropAudit,
		DropAvailabilityGroup,
		DropBrokerPriority,
		DropCertificate,
		DropColumnEncryptionKey,
		DropColumnMasterKey,
		DropContract,
		DropCredential,
		DropCryptographicProvider,
		DropDatabase,
		DropDatabaseAuditSpecification,
		DropDatabaseEncryptionKey,
		DropDefault,
		DropEndpoint,
		DropEventNotification,
		DropEventSession,
		DropExtendedProcedure,
		DropExtendedProperty,
		DropExternalLibrary,
		DropExternalResourcePool,
		DropFulltextCatalog,
		DropFulltextIndex,
		DropFulltextStoplist,
		DropFunction,
		DropIndex,
		DropLinkedServer,
		DropLinkedServerLogin,
		DropLogin,
		DropMasterKey,
		DropMessage,
		DropMessageType,
		DropPartitionFunction,
		DropPartitionScheme,
		DropPlanGuide,
		DropProcedure,
		DropQueue,
		DropRemoteServer,
		DropRemoteServiceBinding,
		DropResourcePool,
		DropRole,
		DropRoleMember,
		DropRoute,
		DropRule,
		DropSchema,
		DropSearchPropertyList,
		DropSecurityPolicy,
		DropSensitivityClassification,
		DropSequence,
		DropServerAudit,
		DropServerAuditSpecification,
		DropServerRole,
		DropServerRoleMember,
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
		DropWorkloadGroup,
		DropXmlSchemaCollection,
		GrantDatabase,
		GrantServer,
		Logon,
		Rename,
		RevokeDatabase,
		RevokeServer,
		UnbindDefault,
		UnbindRule,
		UpdateStatistics
	}

	bool ISfcSupportsDesignMode.IsDesignMode => base.IsDesignMode;

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcReference(typeof(SqlAssembly), "Server[@Name = '{0}']/Database[@Name = 'master']/SqlAssembly[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "AssemblyName" })]
	[CLSCompliant(false)]
	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public int BodyStartIndex => (int)base.Properties.GetValueWithNullReplacement("BodyStartIndex");

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime CreateDate => (DateTime)base.Properties.GetValueWithNullReplacement("CreateDate");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public DateTime DateLastModified => (DateTime)base.Properties.GetValueWithNullReplacement("DateLastModified");

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public ServerDdlTriggerExecutionContext ExecutionContext
	{
		get
		{
			return (ServerDdlTriggerExecutionContext)base.Properties.GetValueWithNullReplacement("ExecutionContext");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContext", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	[SfcReference(typeof(Login), "Server[@Name = '{0}']/Login[@Name = '{1}']", new string[] { "Parent.ConnectionContext.TrueName", "ExecutionContextLogin" })]
	[CLSCompliant(false)]
	public string ExecutionContextLogin
	{
		get
		{
			return (string)base.Properties.GetValueWithNullReplacement("ExecutionContextLogin");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("ExecutionContextLogin", value);
		}
	}

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public int ID => (int)base.Properties.GetValueWithNullReplacement("ID");

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
	public bool IsSystemObject => (bool)base.Properties.GetValueWithNullReplacement("IsSystemObject");

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Standalone)]
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

	[SfcProperty(SfcPropertyFlags.Expensive | SfcPropertyFlags.Standalone)]
	public string Text => (string)base.Properties.GetValueWithNullReplacement("Text");

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

	public static string UrnSuffix => "DdlTrigger";

	public ServerDdlTriggerEventSet DdlTriggerEvents
	{
		get
		{
			if (base.State == SqlSmoState.Creating)
			{
				Property property = base.Properties.Get("DdlTriggerEvents");
				if (property.Value == null)
				{
					property.Value = new ServerDdlTriggerEventSet();
				}
			}
			return (ServerDdlTriggerEventSet)base.Properties.GetValueWithNullReplacement("DdlTriggerEvents");
		}
		set
		{
			base.Properties.SetValueWithConsistencyCheck("DdlTriggerEvents", value);
		}
	}

	internal override SqlPropertyMetadataProvider GetPropertyMetadataProvider()
	{
		return new PropertyMetadataProvider(base.ServerVersion, DatabaseEngineType, DatabaseEngineEdition);
	}

	private string StringFromServerDdlTriggerEvent(int evt)
	{
		return evt switch
		{
			0 => "ADD_ROLE_MEMBER", 
			1 => "ADD_SENSITIVITY_CLASSIFICATION", 
			2 => "ADD_SERVER_ROLE_MEMBER", 
			3 => "ADD_SIGNATURE", 
			4 => "ADD_SIGNATURE_SCHEMA_OBJECT", 
			5 => "ALTER_APPLICATION_ROLE", 
			6 => "ALTER_ASSEMBLY", 
			7 => "ALTER_ASYMMETRIC_KEY", 
			8 => "ALTER_AUDIT", 
			9 => "ALTER_AUTHORIZATION_DATABASE", 
			10 => "ALTER_AUTHORIZATION_SERVER", 
			11 => "ALTER_AVAILABILITY_GROUP", 
			12 => "ALTER_BROKER_PRIORITY", 
			13 => "ALTER_CERTIFICATE", 
			14 => "ALTER_COLUMN_ENCRYPTION_KEY", 
			15 => "ALTER_CREDENTIAL", 
			16 => "ALTER_CRYPTOGRAPHIC_PROVIDER", 
			17 => "ALTER_DATABASE", 
			18 => "ALTER_DATABASE_AUDIT_SPECIFICATION", 
			19 => "ALTER_DATABASE_ENCRYPTION_KEY", 
			20 => "ALTER_DATABASE_SCOPED_CONFIGURATION", 
			21 => "ALTER_ENDPOINT", 
			22 => "ALTER_EVENT_SESSION", 
			23 => "ALTER_EXTENDED_PROPERTY", 
			24 => "ALTER_EXTERNAL_LIBRARY", 
			25 => "ALTER_EXTERNAL_RESOURCE_POOL", 
			26 => "ALTER_FULLTEXT_CATALOG", 
			27 => "ALTER_FULLTEXT_INDEX", 
			28 => "ALTER_FULLTEXT_STOPLIST", 
			29 => "ALTER_FUNCTION", 
			30 => "ALTER_INDEX", 
			31 => "ALTER_INSTANCE", 
			32 => "ALTER_LINKED_SERVER", 
			33 => "ALTER_LOGIN", 
			34 => "ALTER_MASTER_KEY", 
			35 => "ALTER_MESSAGE", 
			36 => "ALTER_MESSAGE_TYPE", 
			37 => "ALTER_PARTITION_FUNCTION", 
			38 => "ALTER_PARTITION_SCHEME", 
			39 => "ALTER_PLAN_GUIDE", 
			40 => "ALTER_PROCEDURE", 
			41 => "ALTER_QUEUE", 
			42 => "ALTER_REMOTE_SERVER", 
			43 => "ALTER_REMOTE_SERVICE_BINDING", 
			44 => "ALTER_RESOURCE_GOVERNOR_CONFIG", 
			45 => "ALTER_RESOURCE_POOL", 
			46 => "ALTER_ROLE", 
			47 => "ALTER_ROUTE", 
			48 => "ALTER_SCHEMA", 
			49 => "ALTER_SEARCH_PROPERTY_LIST", 
			50 => "ALTER_SECURITY_POLICY", 
			51 => "ALTER_SEQUENCE", 
			52 => "ALTER_SERVER_AUDIT", 
			53 => "ALTER_SERVER_AUDIT_SPECIFICATION", 
			54 => "ALTER_SERVER_ROLE", 
			55 => "ALTER_SERVICE", 
			56 => "ALTER_SERVICE_MASTER_KEY", 
			57 => "ALTER_SYMMETRIC_KEY", 
			58 => "ALTER_TABLE", 
			59 => "ALTER_TRIGGER", 
			60 => "ALTER_USER", 
			61 => "ALTER_VIEW", 
			62 => "ALTER_WORKLOAD_GROUP", 
			63 => "ALTER_XML_SCHEMA_COLLECTION", 
			64 => "BIND_DEFAULT", 
			65 => "BIND_RULE", 
			66 => "CREATE_APPLICATION_ROLE", 
			67 => "CREATE_ASSEMBLY", 
			68 => "CREATE_ASYMMETRIC_KEY", 
			69 => "CREATE_AUDIT", 
			70 => "CREATE_AVAILABILITY_GROUP", 
			71 => "CREATE_BROKER_PRIORITY", 
			72 => "CREATE_CERTIFICATE", 
			73 => "CREATE_COLUMN_ENCRYPTION_KEY", 
			74 => "CREATE_COLUMN_MASTER_KEY", 
			75 => "CREATE_CONTRACT", 
			76 => "CREATE_CREDENTIAL", 
			77 => "CREATE_CRYPTOGRAPHIC_PROVIDER", 
			78 => "CREATE_DATABASE", 
			79 => "CREATE_DATABASE_AUDIT_SPECIFICATION", 
			80 => "CREATE_DATABASE_ENCRYPTION_KEY", 
			81 => "CREATE_DEFAULT", 
			82 => "CREATE_ENDPOINT", 
			83 => "CREATE_EVENT_NOTIFICATION", 
			84 => "CREATE_EVENT_SESSION", 
			85 => "CREATE_EXTENDED_PROCEDURE", 
			86 => "CREATE_EXTENDED_PROPERTY", 
			87 => "CREATE_EXTERNAL_LIBRARY", 
			88 => "CREATE_EXTERNAL_RESOURCE_POOL", 
			89 => "CREATE_FULLTEXT_CATALOG", 
			90 => "CREATE_FULLTEXT_INDEX", 
			91 => "CREATE_FULLTEXT_STOPLIST", 
			92 => "CREATE_FUNCTION", 
			93 => "CREATE_INDEX", 
			94 => "CREATE_LINKED_SERVER", 
			95 => "CREATE_LINKED_SERVER_LOGIN", 
			96 => "CREATE_LOGIN", 
			97 => "CREATE_MASTER_KEY", 
			98 => "CREATE_MESSAGE", 
			99 => "CREATE_MESSAGE_TYPE", 
			100 => "CREATE_PARTITION_FUNCTION", 
			101 => "CREATE_PARTITION_SCHEME", 
			102 => "CREATE_PLAN_GUIDE", 
			103 => "CREATE_PROCEDURE", 
			104 => "CREATE_QUEUE", 
			105 => "CREATE_REMOTE_SERVER", 
			106 => "CREATE_REMOTE_SERVICE_BINDING", 
			107 => "CREATE_RESOURCE_POOL", 
			108 => "CREATE_ROLE", 
			109 => "CREATE_ROUTE", 
			110 => "CREATE_RULE", 
			111 => "CREATE_SCHEMA", 
			112 => "CREATE_SEARCH_PROPERTY_LIST", 
			113 => "CREATE_SECURITY_POLICY", 
			114 => "CREATE_SEQUENCE", 
			115 => "CREATE_SERVER_AUDIT", 
			116 => "CREATE_SERVER_AUDIT_SPECIFICATION", 
			117 => "CREATE_SERVER_ROLE", 
			118 => "CREATE_SERVICE", 
			119 => "CREATE_SPATIAL_INDEX", 
			120 => "CREATE_STATISTICS", 
			121 => "CREATE_SYMMETRIC_KEY", 
			122 => "CREATE_SYNONYM", 
			123 => "CREATE_TABLE", 
			124 => "CREATE_TRIGGER", 
			125 => "CREATE_TYPE", 
			126 => "CREATE_USER", 
			127 => "CREATE_VIEW", 
			128 => "CREATE_WORKLOAD_GROUP", 
			129 => "CREATE_XML_INDEX", 
			130 => "CREATE_XML_SCHEMA_COLLECTION", 
			131 => "DENY_DATABASE", 
			132 => "DENY_SERVER", 
			133 => "DROP_APPLICATION_ROLE", 
			134 => "DROP_ASSEMBLY", 
			135 => "DROP_ASYMMETRIC_KEY", 
			136 => "DROP_AUDIT", 
			137 => "DROP_AVAILABILITY_GROUP", 
			138 => "DROP_BROKER_PRIORITY", 
			139 => "DROP_CERTIFICATE", 
			140 => "DROP_COLUMN_ENCRYPTION_KEY", 
			141 => "DROP_COLUMN_MASTER_KEY", 
			142 => "DROP_CONTRACT", 
			143 => "DROP_CREDENTIAL", 
			144 => "DROP_CRYPTOGRAPHIC_PROVIDER", 
			145 => "DROP_DATABASE", 
			146 => "DROP_DATABASE_AUDIT_SPECIFICATION", 
			147 => "DROP_DATABASE_ENCRYPTION_KEY", 
			148 => "DROP_DEFAULT", 
			149 => "DROP_ENDPOINT", 
			150 => "DROP_EVENT_NOTIFICATION", 
			151 => "DROP_EVENT_SESSION", 
			152 => "DROP_EXTENDED_PROCEDURE", 
			153 => "DROP_EXTENDED_PROPERTY", 
			154 => "DROP_EXTERNAL_LIBRARY", 
			155 => "DROP_EXTERNAL_RESOURCE_POOL", 
			156 => "DROP_FULLTEXT_CATALOG", 
			157 => "DROP_FULLTEXT_INDEX", 
			158 => "DROP_FULLTEXT_STOPLIST", 
			159 => "DROP_FUNCTION", 
			160 => "DROP_INDEX", 
			161 => "DROP_LINKED_SERVER", 
			162 => "DROP_LINKED_SERVER_LOGIN", 
			163 => "DROP_LOGIN", 
			164 => "DROP_MASTER_KEY", 
			165 => "DROP_MESSAGE", 
			166 => "DROP_MESSAGE_TYPE", 
			167 => "DROP_PARTITION_FUNCTION", 
			168 => "DROP_PARTITION_SCHEME", 
			169 => "DROP_PLAN_GUIDE", 
			170 => "DROP_PROCEDURE", 
			171 => "DROP_QUEUE", 
			172 => "DROP_REMOTE_SERVER", 
			173 => "DROP_REMOTE_SERVICE_BINDING", 
			174 => "DROP_RESOURCE_POOL", 
			175 => "DROP_ROLE", 
			176 => "DROP_ROLE_MEMBER", 
			177 => "DROP_ROUTE", 
			178 => "DROP_RULE", 
			179 => "DROP_SCHEMA", 
			180 => "DROP_SEARCH_PROPERTY_LIST", 
			181 => "DROP_SECURITY_POLICY", 
			182 => "DROP_SENSITIVITY_CLASSIFICATION", 
			183 => "DROP_SEQUENCE", 
			184 => "DROP_SERVER_AUDIT", 
			185 => "DROP_SERVER_AUDIT_SPECIFICATION", 
			186 => "DROP_SERVER_ROLE", 
			187 => "DROP_SERVER_ROLE_MEMBER", 
			188 => "DROP_SERVICE", 
			189 => "DROP_SIGNATURE", 
			190 => "DROP_SIGNATURE_SCHEMA_OBJECT", 
			191 => "DROP_STATISTICS", 
			192 => "DROP_SYMMETRIC_KEY", 
			193 => "DROP_SYNONYM", 
			194 => "DROP_TABLE", 
			195 => "DROP_TRIGGER", 
			196 => "DROP_TYPE", 
			197 => "DROP_USER", 
			198 => "DROP_VIEW", 
			199 => "DROP_WORKLOAD_GROUP", 
			200 => "DROP_XML_SCHEMA_COLLECTION", 
			201 => "GRANT_DATABASE", 
			202 => "GRANT_SERVER", 
			203 => "LOGON", 
			204 => "RENAME", 
			205 => "REVOKE_DATABASE", 
			206 => "REVOKE_SERVER", 
			207 => "UNBIND_DEFAULT", 
			208 => "UNBIND_RULE", 
			209 => "UPDATE_STATISTICS", 
			_ => throw new SmoException(ExceptionTemplatesImpl.UnknownEnumeration("ServerDdlTriggerEvent")), 
		};
	}

	internal ServerDdlTrigger(AbstractCollectionBase parentColl, ObjectKeyBase key, SqlSmoState state)
		: base(parentColl, key, state)
	{
	}

	public ServerDdlTrigger()
	{
	}

	public ServerDdlTrigger(Server server, string name)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = server;
	}

	public ServerDdlTrigger(Server parent, string name, ServerDdlTriggerEventSet events, string textBody)
	{
		ValidateName(name);
		key = new SimpleObjectKey(name);
		Parent = parent;
		base.Properties.Get("DdlTriggerEvents").Value = events;
		TextBody = textBody;
		base.Properties.Get("ImplementationType").Value = ImplementationType.TransactSql;
	}

	public ServerDdlTrigger(Server parent, string name, ServerDdlTriggerEventSet events, string assemblyName, string className, string method)
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
		SqlSmoObject.ThrowIfBelowVersion90(sp.TargetServerVersionInternal);
		StringBuilder stringBuilder = new StringBuilder(Globals.INIT_BUFFER_SIZE);
		string text = FormatFullNameForScripting(sp);
		ScriptIncludeHeaders(stringBuilder, sp, UrnSuffix);
		if (sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal < SqlServerVersionInternal.Version130)
		{
			stringBuilder.AppendLine(GetIfNotExistStatement(sp, ""));
		}
		stringBuilder.AppendFormat(SmoApplication.DefaultCulture, "DROP TRIGGER {0}{1} ON ALL SERVER", new object[2]
		{
			(sp.IncludeScripts.ExistenceCheck && sp.TargetServerVersionInternal >= SqlServerVersionInternal.Version130) ? "IF EXISTS " : string.Empty,
			text
		});
		queries.Add(stringBuilder.ToString());
	}

	internal override void AddDdlTriggerEvents(StringBuilder sb, ScriptingPreferences sp)
	{
		ServerDdlTriggerEventSet propValueOptional = GetPropValueOptional("DdlTriggerEvents", new ServerDdlTriggerEventSet());
		int num = 0;
		for (int i = 0; i < propValueOptional.NumberOfElements; i++)
		{
			if (propValueOptional.GetBitAt(i))
			{
				if (num++ > 0)
				{
					sb.Append(Globals.commaspace);
				}
				sb.Append(StringFromServerDdlTriggerEvent(i));
			}
		}
		if (num == 0)
		{
			throw new PropertyNotSetException("DdlTriggerEvents");
		}
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
		ServerDdlTriggerEventSet serverDdlTriggerEventSet = (ServerDdlTriggerEventSet)base.Properties.Get("DdlTriggerEvents").Value;
		if (serverDdlTriggerEventSet != null)
		{
			serverDdlTriggerEventSet.Dirty = false;
		}
	}

	protected override bool IsEventSetDirty()
	{
		bool result = false;
		ServerDdlTriggerEventSet serverDdlTriggerEventSet = (ServerDdlTriggerEventSet)base.Properties.Get("DdlTriggerEvents").Value;
		if (serverDdlTriggerEventSet != null)
		{
			result = serverDdlTriggerEventSet.Dirty;
		}
		return result;
	}

	internal override string GetIfNotExistStatement(ScriptingPreferences sp, string prefix)
	{
		return string.Format(SmoApplication.DefaultCulture, Scripts.INCLUDE_EXISTS_SERVER_DDL_TRIGGER, new object[2]
		{
			prefix,
			FormatFullNameForScripting(sp, nameIsIndentifier: false)
		});
	}
}

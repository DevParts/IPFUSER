namespace Microsoft.SqlServer.Management.Smo;

public enum DatabasePermissionSetValue
{
	[PermissionType("AL")]
	[PermissionName("ALTER")]
	Alter,
	[PermissionType("ALAK")]
	[PermissionName("ALTER ANY ASYMMETRIC KEY")]
	AlterAnyAsymmetricKey,
	[PermissionType("ALAR")]
	[PermissionName("ALTER ANY APPLICATION ROLE")]
	AlterAnyApplicationRole,
	[PermissionType("ALAS")]
	[PermissionName("ALTER ANY ASSEMBLY")]
	AlterAnyAssembly,
	[PermissionName("ALTER ANY CERTIFICATE")]
	[PermissionType("ALCF")]
	AlterAnyCertificate,
	[PermissionType("ALDS")]
	[PermissionName("ALTER ANY DATASPACE")]
	AlterAnyDataspace,
	[PermissionName("ALTER ANY DATABASE EVENT NOTIFICATION")]
	[PermissionType("ALED")]
	AlterAnyDatabaseEventNotification,
	[PermissionType("ALFT")]
	[PermissionName("ALTER ANY FULLTEXT CATALOG")]
	AlterAnyFulltextCatalog,
	[PermissionName("ALTER ANY MESSAGE TYPE")]
	[PermissionType("ALMT")]
	AlterAnyMessageType,
	[PermissionType("ALRL")]
	[PermissionName("ALTER ANY ROLE")]
	AlterAnyRole,
	[PermissionType("ALRT")]
	[PermissionName("ALTER ANY ROUTE")]
	AlterAnyRoute,
	[PermissionName("ALTER ANY REMOTE SERVICE BINDING")]
	[PermissionType("ALSB")]
	AlterAnyRemoteServiceBinding,
	[PermissionName("ALTER ANY CONTRACT")]
	[PermissionType("ALSC")]
	AlterAnyContract,
	[PermissionName("ALTER ANY SYMMETRIC KEY")]
	[PermissionType("ALSK")]
	AlterAnySymmetricKey,
	[PermissionName("ALTER ANY SCHEMA")]
	[PermissionType("ALSM")]
	AlterAnySchema,
	[PermissionType("ALSV")]
	[PermissionName("ALTER ANY SERVICE")]
	AlterAnyService,
	[PermissionType("ALTG")]
	[PermissionName("ALTER ANY DATABASE DDL TRIGGER")]
	AlterAnyDatabaseDdlTrigger,
	[PermissionName("ALTER ANY USER")]
	[PermissionType("ALUS")]
	AlterAnyUser,
	[PermissionType("AUTH")]
	[PermissionName("AUTHENTICATE")]
	Authenticate,
	[PermissionName("BACKUP DATABASE")]
	[PermissionType("BADB")]
	BackupDatabase,
	[PermissionName("BACKUP LOG")]
	[PermissionType("BALO")]
	BackupLog,
	[PermissionName("CONTROL")]
	[PermissionType("CL")]
	Control,
	[PermissionType("CO")]
	[PermissionName("CONNECT")]
	Connect,
	[PermissionName("CONNECT REPLICATION")]
	[PermissionType("CORP")]
	ConnectReplication,
	[PermissionType("CP")]
	[PermissionName("CHECKPOINT")]
	Checkpoint,
	[PermissionName("CREATE AGGREGATE")]
	[PermissionType("CRAG")]
	CreateAggregate,
	[PermissionType("CRAK")]
	[PermissionName("CREATE ASYMMETRIC KEY")]
	CreateAsymmetricKey,
	[PermissionType("CRAS")]
	[PermissionName("CREATE ASSEMBLY")]
	CreateAssembly,
	[PermissionName("CREATE CERTIFICATE")]
	[PermissionType("CRCF")]
	CreateCertificate,
	[PermissionName("CREATE DATABASE")]
	[PermissionType("CRDB")]
	CreateDatabase,
	[PermissionName("CREATE DEFAULT")]
	[PermissionType("CRDF")]
	CreateDefault,
	[PermissionType("CRED")]
	[PermissionName("CREATE DATABASE DDL EVENT NOTIFICATION")]
	CreateDatabaseDdlEventNotification,
	[PermissionType("CRFN")]
	[PermissionName("CREATE FUNCTION")]
	CreateFunction,
	[PermissionName("CREATE FULLTEXT CATALOG")]
	[PermissionType("CRFT")]
	CreateFulltextCatalog,
	[PermissionType("CRMT")]
	[PermissionName("CREATE MESSAGE TYPE")]
	CreateMessageType,
	[PermissionName("CREATE PROCEDURE")]
	[PermissionType("CRPR")]
	CreateProcedure,
	[PermissionName("CREATE QUEUE")]
	[PermissionType("CRQU")]
	CreateQueue,
	[PermissionName("CREATE ROLE")]
	[PermissionType("CRRL")]
	CreateRole,
	[PermissionType("CRRT")]
	[PermissionName("CREATE ROUTE")]
	CreateRoute,
	[PermissionType("CRRU")]
	[PermissionName("CREATE RULE")]
	CreateRule,
	[PermissionName("CREATE REMOTE SERVICE BINDING")]
	[PermissionType("CRSB")]
	CreateRemoteServiceBinding,
	[PermissionName("CREATE CONTRACT")]
	[PermissionType("CRSC")]
	CreateContract,
	[PermissionName("CREATE SYMMETRIC KEY")]
	[PermissionType("CRSK")]
	CreateSymmetricKey,
	[PermissionType("CRSM")]
	[PermissionName("CREATE SCHEMA")]
	CreateSchema,
	[PermissionType("CRSN")]
	[PermissionName("CREATE SYNONYM")]
	CreateSynonym,
	[PermissionName("CREATE SERVICE")]
	[PermissionType("CRSV")]
	CreateService,
	[PermissionType("CRTB")]
	[PermissionName("CREATE TABLE")]
	CreateTable,
	[PermissionName("CREATE TYPE")]
	[PermissionType("CRTY")]
	CreateType,
	[PermissionType("CRVW")]
	[PermissionName("CREATE VIEW")]
	CreateView,
	[PermissionName("CREATE XML SCHEMA COLLECTION")]
	[PermissionType("CRXS")]
	CreateXmlSchemaCollection,
	[PermissionName("DELETE")]
	[PermissionType("DL")]
	Delete,
	[PermissionName("EXECUTE")]
	[PermissionType("EX")]
	Execute,
	[PermissionType("IN")]
	[PermissionName("INSERT")]
	Insert,
	[PermissionName("REFERENCES")]
	[PermissionType("RF")]
	References,
	[PermissionType("SL")]
	[PermissionName("SELECT")]
	Select,
	[PermissionType("SPLN")]
	[PermissionName("SHOWPLAN")]
	Showplan,
	[PermissionName("SUBSCRIBE QUERY NOTIFICATIONS")]
	[PermissionType("SUQN")]
	SubscribeQueryNotifications,
	[PermissionType("TO")]
	[PermissionName("TAKE OWNERSHIP")]
	TakeOwnership,
	[PermissionName("UPDATE")]
	[PermissionType("UP")]
	Update,
	[PermissionType("VW")]
	[PermissionName("VIEW DEFINITION")]
	ViewDefinition,
	[PermissionType("VWDS")]
	[PermissionName("VIEW DATABASE STATE")]
	ViewDatabaseState,
	[PermissionName("ALTER ANY DATABASE AUDIT")]
	[PermissionType("ALDA")]
	AlterAnyDatabaseAudit,
	[PermissionType("ALSP")]
	[PermissionName("ALTER ANY SECURITY POLICY")]
	AlterAnySecurityPolicy,
	[PermissionName("ALTER ANY EXTERNAL DATA SOURCE")]
	[PermissionType("AEDS")]
	AlterAnyExternalDataSource,
	[PermissionType("AEFF")]
	[PermissionName("ALTER ANY EXTERNAL FILE FORMAT")]
	AlterAnyExternalFileFormat,
	[PermissionType("AAMK")]
	[PermissionName("ALTER ANY MASK")]
	AlterAnyMask,
	[PermissionType("UMSK")]
	[PermissionName("UNMASK")]
	Unmask,
	[PermissionName("VIEW ANY COLUMN ENCRYPTION KEY DEFINITION")]
	[PermissionType("VWCK")]
	ViewAnyColumnEncryptionKeyDefinition,
	[PermissionType("VWCM")]
	[PermissionName("VIEW ANY COLUMN MASTER KEY DEFINITION")]
	ViewAnyColumnMasterKeyDefinition,
	[PermissionType("AADS")]
	[PermissionName("ALTER ANY DATABASE EVENT SESSION")]
	AlterAnyDatabaseEventSession,
	[PermissionName("ALTER ANY COLUMN ENCRYPTION KEY")]
	[PermissionType("ALCK")]
	AlterAnyColumnEncryptionKey,
	[PermissionName("ALTER ANY COLUMN MASTER KEY")]
	[PermissionType("ALCM")]
	AlterAnyColumnMasterKey,
	[PermissionName("ALTER ANY DATABASE SCOPED CONFIGURATION")]
	[PermissionType("ALDC")]
	AlterAnyDatabaseScopedConfiguration,
	[PermissionName("ALTER ANY EXTERNAL LIBRARY")]
	[PermissionType("ALEL")]
	AlterAnyExternalLibrary,
	[PermissionName("ADMINISTER DATABASE BULK OPERATIONS")]
	[PermissionType("DABO")]
	AdministerDatabaseBulkOperations,
	[PermissionType("EAES")]
	[PermissionName("EXECUTE ANY EXTERNAL SCRIPT")]
	ExecuteAnyExternalScript,
	[PermissionName("KILL DATABASE CONNECTION")]
	[PermissionType("KIDC")]
	KillDatabaseConnection,
	[PermissionName("CREATE EXTERNAL LIBRARY")]
	[PermissionType("CREL")]
	CreateExternalLibrary
}

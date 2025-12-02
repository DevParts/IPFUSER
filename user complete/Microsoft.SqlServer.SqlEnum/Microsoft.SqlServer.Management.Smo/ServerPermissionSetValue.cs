namespace Microsoft.SqlServer.Management.Smo;

public enum ServerPermissionSetValue
{
	[PermissionName("ADMINISTER BULK OPERATIONS")]
	[PermissionType("ADBO")]
	AdministerBulkOperations,
	[PermissionType("ALCD")]
	[PermissionName("ALTER ANY CREDENTIAL")]
	AlterAnyCredential,
	[PermissionName("ALTER ANY CONNECTION")]
	[PermissionType("ALCO")]
	AlterAnyConnection,
	[PermissionName("ALTER ANY DATABASE")]
	[PermissionType("ALDB")]
	AlterAnyDatabase,
	[PermissionType("ALES")]
	[PermissionName("ALTER ANY EVENT NOTIFICATION")]
	AlterAnyEventNotification,
	[PermissionType("ALHE")]
	[PermissionName("ALTER ANY ENDPOINT")]
	AlterAnyEndpoint,
	[PermissionName("ALTER ANY LOGIN")]
	[PermissionType("ALLG")]
	AlterAnyLogin,
	[PermissionName("ALTER ANY LINKED SERVER")]
	[PermissionType("ALLS")]
	AlterAnyLinkedServer,
	[PermissionName("ALTER RESOURCES")]
	[PermissionType("ALRS")]
	AlterResources,
	[PermissionType("ALSS")]
	[PermissionName("ALTER SERVER STATE")]
	AlterServerState,
	[PermissionName("ALTER SETTINGS")]
	[PermissionType("ALST")]
	AlterSettings,
	[PermissionType("ALTR")]
	[PermissionName("ALTER TRACE")]
	AlterTrace,
	[PermissionName("AUTHENTICATE SERVER")]
	[PermissionType("AUTH")]
	AuthenticateServer,
	[PermissionName("CONTROL SERVER")]
	[PermissionType("CL")]
	ControlServer,
	[PermissionName("CONNECT SQL")]
	[PermissionType("COSQ")]
	ConnectSql,
	[PermissionType("CRDB")]
	[PermissionName("CREATE ANY DATABASE")]
	CreateAnyDatabase,
	[PermissionName("CREATE DDL EVENT NOTIFICATION")]
	[PermissionType("CRDE")]
	CreateDdlEventNotification,
	[PermissionName("CREATE ENDPOINT")]
	[PermissionType("CRHE")]
	CreateEndpoint,
	[PermissionName("CREATE TRACE EVENT NOTIFICATION")]
	[PermissionType("CRTE")]
	CreateTraceEventNotification,
	[PermissionName("SHUTDOWN")]
	[PermissionType("SHDN")]
	Shutdown,
	[PermissionName("VIEW ANY DEFINITION")]
	[PermissionType("VWAD")]
	ViewAnyDefinition,
	[PermissionName("VIEW ANY DATABASE")]
	[PermissionType("VWDB")]
	ViewAnyDatabase,
	[PermissionName("VIEW SERVER STATE")]
	[PermissionType("VWSS")]
	ViewServerState,
	[PermissionName("EXTERNAL ACCESS ASSEMBLY")]
	[PermissionType("XA")]
	ExternalAccessAssembly,
	[PermissionType("XU")]
	[PermissionName("UNSAFE ASSEMBLY")]
	UnsafeAssembly,
	[PermissionName("ALTER ANY SERVER AUDIT")]
	[PermissionType("ALAA")]
	AlterAnyServerAudit,
	[PermissionName("ALTER ANY SERVER ROLE")]
	[PermissionType("ALSR")]
	AlterAnyServerRole,
	[PermissionName("CREATE SERVER ROLE")]
	[PermissionType("CRSR")]
	CreateServerRole,
	[PermissionType("ALAG")]
	[PermissionName("ALTER ANY AVAILABILITY GROUP")]
	AlterAnyAvailabilityGroup,
	[PermissionType("CRAC")]
	[PermissionName("CREATE AVAILABILITY GROUP")]
	CreateAvailabilityGroup,
	[PermissionName("ALTER ANY EVENT SESSION")]
	[PermissionType("AAES")]
	AlterAnyEventSession,
	[PermissionType("SUS")]
	[PermissionName("SELECT ALL USER SECURABLES")]
	SelectAllUserSecurables,
	[PermissionName("CONNECT ANY DATABASE")]
	[PermissionType("CADB")]
	ConnectAnyDatabase,
	[PermissionType("IAL")]
	[PermissionName("IMPERSONATE ANY LOGIN")]
	ImpersonateAnyLogin
}

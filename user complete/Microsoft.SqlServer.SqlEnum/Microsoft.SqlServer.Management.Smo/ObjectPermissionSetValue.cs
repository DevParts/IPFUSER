namespace Microsoft.SqlServer.Management.Smo;

public enum ObjectPermissionSetValue
{
	[PermissionName("ALTER")]
	[PermissionType("AL")]
	Alter,
	[PermissionName("CONTROL")]
	[PermissionType("CL")]
	Control,
	[PermissionType("CO")]
	[PermissionName("CONNECT")]
	Connect,
	[PermissionName("DELETE")]
	[PermissionType("DL")]
	Delete,
	[PermissionType("EX")]
	[PermissionName("EXECUTE")]
	Execute,
	[PermissionName("IMPERSONATE")]
	[PermissionType("IM")]
	Impersonate,
	[PermissionType("IN")]
	[PermissionName("INSERT")]
	Insert,
	[PermissionType("RC")]
	[PermissionName("RECEIVE")]
	Receive,
	[PermissionName("REFERENCES")]
	[PermissionType("RF")]
	References,
	[PermissionType("SL")]
	[PermissionName("SELECT")]
	Select,
	[PermissionName("SEND")]
	[PermissionType("SN")]
	Send,
	[PermissionType("TO")]
	[PermissionName("TAKE OWNERSHIP")]
	TakeOwnership,
	[PermissionName("UPDATE")]
	[PermissionType("UP")]
	Update,
	[PermissionType("VW")]
	[PermissionName("VIEW DEFINITION")]
	ViewDefinition,
	[PermissionType("VWCT")]
	[PermissionName("VIEW CHANGE TRACKING")]
	ViewChangeTracking,
	[PermissionName("CREATE SEQUENCE")]
	[PermissionType("CRSO")]
	CreateSequence
}

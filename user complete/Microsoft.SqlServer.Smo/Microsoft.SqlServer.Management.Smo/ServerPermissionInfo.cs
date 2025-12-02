namespace Microsoft.SqlServer.Management.Smo;

public class ServerPermissionInfo : PermissionInfo
{
	public ServerPermissionSet PermissionType => (ServerPermissionSet)base.PermissionTypeInternal;
}

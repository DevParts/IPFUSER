namespace Microsoft.SqlServer.Management.Smo;

public class DatabasePermissionInfo : PermissionInfo
{
	public DatabasePermissionSet PermissionType => (DatabasePermissionSet)base.PermissionTypeInternal;
}

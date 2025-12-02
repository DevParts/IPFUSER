namespace Microsoft.SqlServer.Management.Smo;

public class ObjectPermissionInfo : PermissionInfo
{
	public ObjectPermissionSet PermissionType => (ObjectPermissionSet)base.PermissionTypeInternal;
}

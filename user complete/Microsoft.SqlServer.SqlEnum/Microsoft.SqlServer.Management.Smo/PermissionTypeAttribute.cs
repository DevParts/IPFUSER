namespace Microsoft.SqlServer.Management.Smo;

internal sealed class PermissionTypeAttribute : StringValueAttribute
{
	public PermissionTypeAttribute(string permissionType)
		: base(permissionType)
	{
	}
}

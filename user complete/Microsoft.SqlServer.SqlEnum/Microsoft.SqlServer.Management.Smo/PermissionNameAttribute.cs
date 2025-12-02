namespace Microsoft.SqlServer.Management.Smo;

internal sealed class PermissionNameAttribute : StringValueAttribute
{
	public PermissionNameAttribute(string permissionName)
		: base(permissionName)
	{
	}
}

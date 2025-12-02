namespace Microsoft.SqlServer.Management.Smo;

public interface IColumnPermission : IObjectPermission
{
	void Deny(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames);

	void Deny(ObjectPermissionSet permissions, string granteeName, string[] columnNames);

	void Deny(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames, bool cascade);

	void Deny(ObjectPermissionSet permissions, string granteeName, string[] columnNames, bool cascade);

	void Grant(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames);

	void Grant(ObjectPermissionSet permissions, string granteeName, string[] columnNames);

	void Grant(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames, bool grantGrant);

	void Grant(ObjectPermissionSet permissions, string granteeName, string[] columnNames, bool grantGrant);

	void Grant(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames, bool grantGrant, string asRole);

	void Grant(ObjectPermissionSet permissions, string granteeName, string[] columnNames, bool grantGrant, string asRole);

	void Revoke(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames);

	void Revoke(ObjectPermissionSet permissions, string granteeName, string[] columnNames);

	void Revoke(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade);

	void Revoke(ObjectPermissionSet permissions, string granteeName, string[] columnNames, bool revokeGrant, bool cascade);

	void Revoke(ObjectPermissionSet permissions, string[] granteeNames, string[] columnNames, bool revokeGrant, bool cascade, string asRole);

	void Revoke(ObjectPermissionSet permissions, string granteeName, string[] columnNames, bool revokeGrant, bool cascade, string asRole);

	ObjectPermissionInfo[] EnumColumnPermissions(string granteeName);

	ObjectPermissionInfo[] EnumColumnPermissions(string granteeName, ObjectPermissionSet permissions);
}

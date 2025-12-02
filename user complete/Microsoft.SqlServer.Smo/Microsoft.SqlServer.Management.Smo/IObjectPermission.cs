namespace Microsoft.SqlServer.Management.Smo;

public interface IObjectPermission
{
	void Deny(ObjectPermissionSet permissions, string[] granteeNames);

	void Deny(ObjectPermissionSet permissions, string granteeName);

	void Deny(ObjectPermissionSet permissions, string[] granteeNames, bool cascade);

	void Deny(ObjectPermissionSet permissions, string granteeName, bool cascade);

	void Grant(ObjectPermissionSet permissions, string[] granteeNames);

	void Grant(ObjectPermissionSet permissions, string granteeName);

	void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant);

	void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant);

	void Grant(ObjectPermissionSet permissions, string[] granteeNames, bool grantGrant, string asRole);

	void Grant(ObjectPermissionSet permissions, string granteeName, bool grantGrant, string asRole);

	void Revoke(ObjectPermissionSet permissions, string[] granteeNames);

	void Revoke(ObjectPermissionSet permissions, string granteeName);

	void Revoke(ObjectPermissionSet permissions, string[] granteeNames, bool revokeGrant, bool cascade);

	void Revoke(ObjectPermissionSet permissions, string granteeName, bool revokeGrant, bool cascade, string asRole);

	ObjectPermissionInfo[] EnumObjectPermissions();

	ObjectPermissionInfo[] EnumObjectPermissions(string granteeName);

	ObjectPermissionInfo[] EnumObjectPermissions(ObjectPermissionSet permissions);

	ObjectPermissionInfo[] EnumObjectPermissions(string granteeName, ObjectPermissionSet permissions);
}

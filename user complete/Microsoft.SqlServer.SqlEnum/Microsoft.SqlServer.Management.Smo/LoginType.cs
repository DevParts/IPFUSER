namespace Microsoft.SqlServer.Management.Smo;

public enum LoginType
{
	WindowsUser,
	WindowsGroup,
	SqlLogin,
	Certificate,
	AsymmetricKey,
	ExternalUser,
	ExternalGroup
}

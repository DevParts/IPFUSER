namespace Microsoft.SqlServer.Management.Smo;

public enum UserType
{
	SqlLogin = 0,
	SqlUser = 0,
	Certificate = 1,
	AsymmetricKey = 2,
	NoLogin = 3,
	External = 4
}

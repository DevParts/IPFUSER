using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum RoleTypes
{
	Database = 1,
	Server = 2,
	All = 3
}

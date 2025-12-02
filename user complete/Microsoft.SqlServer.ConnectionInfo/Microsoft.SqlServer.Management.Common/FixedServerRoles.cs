using System;

namespace Microsoft.SqlServer.Management.Common;

[Flags]
public enum FixedServerRoles
{
	None = 0,
	SysAdmin = 1,
	ServerAdmin = 2,
	SetupAdmin = 4,
	SecurityAdmin = 8,
	ProcessAdmin = 0x10,
	DBCreator = 0x20,
	DiskAdmin = 0x40,
	BulkAdmin = 0x80
}

using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum SqlServerVersions
{
	Unknown = 0,
	Version70 = 1,
	Version80 = 2,
	Version90 = 4,
	Version100 = 8,
	Version105 = 0x10,
	Version110 = 0x20,
	Version120 = 0x40,
	Version130 = 0x80,
	Version140 = 0x100,
	Version150 = 0x200
}

using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum MediaTypes
{
	All = 0xF,
	CDRom = 8,
	FixedDisk = 2,
	Floppy = 1,
	SharedFixedDisk = 0x10,
	Tape = 4
}

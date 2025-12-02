using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum PrivilegeTypes
{
	Unknown = 0,
	Select = 1,
	Insert = 2,
	Update = 4,
	Delete = 8,
	Execute = 0x10,
	References = 0x20,
	ViewDefinition = 0x100000,
	Control = 0x200000,
	Alter = 0x400000,
	Drop = 0x800000,
	AllObjectPrivileges = 0xF0003F,
	CreateTable = 0x80,
	CreateDatabase = 0x100,
	CreateView = 0x200,
	CreateProcedure = 0x400,
	DumpDatabase = 0x800,
	CreateDefault = 0x1000,
	DumpTransaction = 0x2000,
	CreateRule = 0x4000,
	DumpTable = 0x8000,
	CreateFunction = 0x10000,
	CreateType = 0x20000,
	AllDatabasePrivileges = 0x3FF80,
	BackupDatabase = 0x40000,
	BackupLog = 0x80000
}

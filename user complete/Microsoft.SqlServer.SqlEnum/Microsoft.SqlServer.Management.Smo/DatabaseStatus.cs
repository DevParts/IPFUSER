using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum DatabaseStatus
{
	Normal = 1,
	Restoring = 2,
	RecoveryPending = 4,
	Recovering = 8,
	Suspect = 0x10,
	Offline = 0x20,
	Standby = 0x40,
	Shutdown = 0x80,
	EmergencyMode = 0x100,
	AutoClosed = 0x200,
	Inaccessible = 0x3E
}

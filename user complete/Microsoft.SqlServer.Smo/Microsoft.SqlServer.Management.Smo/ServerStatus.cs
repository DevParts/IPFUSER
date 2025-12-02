using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum ServerStatus
{
	Unknown = 0,
	Online = 1,
	OnlinePending = 3,
	Offline = 0x10,
	OfflinePending = 0x30
}

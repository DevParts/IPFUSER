using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum TargetServerStatus
{
	Normal = 1,
	SuspectedOffline = 2,
	Blocked = 4
}

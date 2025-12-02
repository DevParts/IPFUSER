using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum AuditLevel
{
	None = 0,
	Success = 1,
	Failure = 2,
	All = 3
}

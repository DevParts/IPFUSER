using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum HttpPortTypes
{
	None = 0,
	Ssl = 1,
	Clear = 2,
	All = 3
}

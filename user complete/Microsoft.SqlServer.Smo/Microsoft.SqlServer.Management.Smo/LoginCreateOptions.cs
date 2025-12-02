using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum LoginCreateOptions
{
	None = 0,
	IsHashed = 1,
	MustChange = 2
}

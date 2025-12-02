using System;

namespace Microsoft.SqlServer.Management.Common;

[Flags]
public enum ServerUserProfiles
{
	None = 0,
	SALogin = 1,
	CreateDatabase = 2,
	CreateXP = 4,
	All = 7
}

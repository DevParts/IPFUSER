using System;

namespace Microsoft.SqlServer.Management.Sdk.Differencing;

[Flags]
public enum DiffType
{
	None = 0,
	Equivalent = 1,
	Created = 2,
	Deleted = 4,
	Updated = 8
}

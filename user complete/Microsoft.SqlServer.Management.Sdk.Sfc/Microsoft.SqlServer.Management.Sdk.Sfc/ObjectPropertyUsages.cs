using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Flags]
public enum ObjectPropertyUsages
{
	None = 0,
	Filter = 1,
	Request = 2,
	OrderBy = 4,
	Reserved1 = 8,
	All = 7
}

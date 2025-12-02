using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum PartitioningScheme
{
	None = 0,
	Table = 1,
	Index = 2,
	All = 3
}

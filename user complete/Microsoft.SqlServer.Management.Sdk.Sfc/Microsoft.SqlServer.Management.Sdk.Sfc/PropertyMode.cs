using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Flags]
public enum PropertyMode
{
	None = 0,
	Design = 1,
	Deploy = 2,
	All = 3
}

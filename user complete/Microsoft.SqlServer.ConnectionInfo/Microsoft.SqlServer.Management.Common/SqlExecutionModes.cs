using System;

namespace Microsoft.SqlServer.Management.Common;

[Flags]
public enum SqlExecutionModes
{
	ExecuteSql = 1,
	CaptureSql = 2,
	ExecuteAndCaptureSql = 3
}

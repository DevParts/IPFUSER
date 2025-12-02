using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[Flags]
public enum RequestFieldsTypes
{
	Request = 1,
	IncludeExpensiveInResult = 2,
	Reject = 0
}

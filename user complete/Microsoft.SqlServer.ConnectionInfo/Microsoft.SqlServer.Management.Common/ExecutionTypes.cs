using System;

namespace Microsoft.SqlServer.Management.Common;

[Flags]
public enum ExecutionTypes
{
	Default = 0,
	NoCommands = 1,
	ContinueOnError = 2,
	NoExec = 4,
	ParseOnly = 8,
	QuotedIdentifierOn = 0x10
}

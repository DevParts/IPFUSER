using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum TransactionTypes
{
	Versioned = 1,
	UnVersioned = 2,
	Both = 3
}

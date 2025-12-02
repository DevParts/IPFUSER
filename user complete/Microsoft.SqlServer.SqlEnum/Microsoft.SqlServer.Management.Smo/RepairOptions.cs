using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum RepairOptions
{
	None = 0,
	AllErrorMessages = 1,
	ExtendedLogicalChecks = 2,
	NoInformationMessages = 4,
	TableLock = 8,
	EstimateOnly = 0x10
}

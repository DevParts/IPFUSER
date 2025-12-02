using System;

namespace Microsoft.SqlServer.Management.Smo;

[Flags]
public enum RetentionPeriodUnits
{
	None = 0,
	Minutes = 1,
	Hours = 2,
	Days = 3
}

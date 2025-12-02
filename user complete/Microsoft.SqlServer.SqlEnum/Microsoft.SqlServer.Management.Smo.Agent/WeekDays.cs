using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum WeekDays
{
	Sunday = 1,
	Monday = 2,
	Tuesday = 4,
	Wednesday = 8,
	Thursday = 0x10,
	Friday = 0x20,
	Saturday = 0x40,
	EveryDay = 0x7F,
	WeekDays = 0x3E,
	WeekEnds = 0x41
}

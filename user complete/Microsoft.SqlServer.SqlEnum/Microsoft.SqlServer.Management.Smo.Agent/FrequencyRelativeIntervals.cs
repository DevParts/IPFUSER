using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum FrequencyRelativeIntervals
{
	First = 1,
	Second = 2,
	Third = 4,
	Fourth = 8,
	Last = 0x10
}

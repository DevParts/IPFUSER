using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum FrequencySubDayTypes
{
	Hour = 8,
	Minute = 4,
	Second = 2,
	Once = 1,
	Unknown = 0
}

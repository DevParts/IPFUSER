using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum FrequencyTypes
{
	AutoStart = 0x40,
	Daily = 4,
	Monthly = 0x10,
	MonthlyRelative = 0x20,
	OneTime = 1,
	OnIdle = 0x80,
	Unknown = 0,
	Weekly = 8
}

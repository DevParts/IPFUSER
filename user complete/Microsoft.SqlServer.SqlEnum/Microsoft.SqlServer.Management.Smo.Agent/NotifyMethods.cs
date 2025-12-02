using System;

namespace Microsoft.SqlServer.Management.Smo.Agent;

[Flags]
public enum NotifyMethods
{
	None = 0,
	NotifyEmail = 1,
	Pager = 2,
	NetSend = 4,
	NotifyAll = 7
}

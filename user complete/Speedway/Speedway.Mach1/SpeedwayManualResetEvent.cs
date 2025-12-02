using System;
using System.Threading;

namespace Speedway.Mach1;

[Serializable]
public class SpeedwayManualResetEvent
{
	public ManualResetEvent evt;

	public byte[] data;

	public SpeedwayManualResetEvent(bool status)
	{
		evt = new ManualResetEvent(status);
	}
}

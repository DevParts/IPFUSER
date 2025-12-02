using System;
using System.Threading;

namespace NLog.Time;

/// <summary>
/// Fast time source that updates current time only once per tick (15.6 milliseconds).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
public abstract class CachedTimeSource : TimeSource
{
	private int _lastTicks = -1;

	private DateTime _lastTime = DateTime.MinValue;

	/// <summary>
	/// Gets raw uncached time from derived time source.
	/// </summary>
	protected abstract DateTime FreshTime { get; }

	/// <summary>
	/// Gets current time cached for one system tick (15.6 milliseconds).
	/// </summary>
	public override DateTime Time
	{
		get
		{
			int tickCount = Environment.TickCount;
			if (tickCount != _lastTicks)
			{
				return RetrieveFreshTime(tickCount);
			}
			return _lastTime;
		}
	}

	private DateTime RetrieveFreshTime(int tickCount)
	{
		DateTime result = (_lastTime = FreshTime);
		Thread.MemoryBarrier();
		_lastTicks = tickCount;
		return result;
	}
}

using System;

namespace NLog.Time;

/// <summary>
/// Fast local time source that is updated once per tick (15.6 milliseconds).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
[TimeSource("FastLocal")]
public sealed class FastLocalTimeSource : CachedTimeSource
{
	/// <summary>
	/// Gets uncached local time directly from DateTime.Now.
	/// </summary>
	protected override DateTime FreshTime => DateTime.Now;

	/// <summary>
	///  Converts the specified system time to the same form as the time value originated from this time source.
	/// </summary>
	/// <param name="systemTime">The system originated time value to convert.</param>
	/// <returns>
	///  The value of <paramref name="systemTime" /> converted to local time.
	/// </returns>
	public override DateTime FromSystemTime(DateTime systemTime)
	{
		return systemTime.ToLocalTime();
	}
}

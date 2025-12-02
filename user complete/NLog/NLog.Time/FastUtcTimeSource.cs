using System;

namespace NLog.Time;

/// <summary>
/// Fast UTC time source that is updated once per tick (15.6 milliseconds).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
[TimeSource("FastUTC")]
public sealed class FastUtcTimeSource : CachedTimeSource
{
	/// <summary>
	/// Gets uncached UTC time directly from DateTime.UtcNow.
	/// </summary>
	protected override DateTime FreshTime => DateTime.UtcNow;

	/// <summary>
	///  Converts the specified system time to the same form as the time value originated from this time source.
	/// </summary>
	/// <param name="systemTime">The system originated time value to convert.</param>
	/// <returns>
	///  The value of <paramref name="systemTime" /> converted to UTC time.
	/// </returns>
	public override DateTime FromSystemTime(DateTime systemTime)
	{
		return systemTime.ToUniversalTime();
	}
}

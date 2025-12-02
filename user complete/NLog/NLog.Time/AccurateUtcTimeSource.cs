using System;

namespace NLog.Time;

/// <summary>
/// Current UTC time retrieved directly from DateTime.UtcNow.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
[TimeSource("AccurateUTC")]
public sealed class AccurateUtcTimeSource : TimeSource
{
	/// <summary>
	/// Gets current UTC time directly from DateTime.UtcNow.
	/// </summary>
	public override DateTime Time => DateTime.UtcNow;

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

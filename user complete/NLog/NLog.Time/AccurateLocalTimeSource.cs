using System;

namespace NLog.Time;

/// <summary>
/// Current local time retrieved directly from DateTime.Now.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
[TimeSource("AccurateLocal")]
public sealed class AccurateLocalTimeSource : TimeSource
{
	/// <summary>
	/// Gets current local time directly from DateTime.Now.
	/// </summary>
	public override DateTime Time => DateTime.Now;

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

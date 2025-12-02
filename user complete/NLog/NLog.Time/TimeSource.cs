using System;
using NLog.Config;
using NLog.Internal;

namespace NLog.Time;

/// <summary>
/// Defines source of current time.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Time-Source">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Time-Source">Documentation on NLog Wiki</seealso>
[NLogConfigurationItem]
public abstract class TimeSource
{
	/// <summary>
	/// Gets current time.
	/// </summary>
	public abstract DateTime Time { get; }

	/// <summary>
	/// Gets or sets current global time source used in all log events.
	/// </summary>
	/// <remarks>
	/// Default time source is <see cref="T:NLog.Time.FastLocalTimeSource" />.
	/// </remarks>
	public static TimeSource Current { get; set; } = new FastLocalTimeSource();

	/// <summary>
	/// Returns a <see cref="T:System.String" /> that represents this instance.
	/// </summary>
	/// <returns>
	/// A <see cref="T:System.String" /> that represents this instance.
	/// </returns>
	public override string ToString()
	{
		TimeSourceAttribute firstCustomAttribute = GetType().GetFirstCustomAttribute<TimeSourceAttribute>();
		if (firstCustomAttribute != null)
		{
			return firstCustomAttribute.Name + " (time source)";
		}
		return GetType().Name;
	}

	/// <summary>
	///  Converts the specified system time to the same form as the time value originated from this time source.
	/// </summary>
	/// <param name="systemTime">The system originated time value to convert.</param>
	/// <returns>
	///  The value of <paramref name="systemTime" /> converted to the same form
	///  as time values originated from this source.
	/// </returns>
	/// <remarks>
	///  <para>
	///   There are situations when NLog have to compare the time originated from TimeSource
	///   to the time originated externally in the system.
	///   To be able to provide meaningful result of such comparisons the system time must be expressed in
	///   the same form as TimeSource time.
	/// </para>
	/// <para>
	///   Examples:
	///    - If the TimeSource provides time values of local time, it should also convert the provided
	///      <paramref name="systemTime" /> to the local time.
	///    - If the TimeSource shifts or skews its time values, it should also apply
	///      the same transform to the given <paramref name="systemTime" />.
	/// </para>
	/// </remarks>
	public abstract DateTime FromSystemTime(DateTime systemTime);
}

using System;
using NLog.Config;

namespace NLog.Time;

/// <summary>
/// Marks class as a time source and assigns a name to it.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TimeSourceAttribute : NameBaseAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Time.TimeSourceAttribute" /> class.
	/// </summary>
	/// <param name="name">The Time type-alias for use in NLog configuration.</param>
	public TimeSourceAttribute(string name)
		: base(name)
	{
	}
}

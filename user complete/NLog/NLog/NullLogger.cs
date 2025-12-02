using NLog.Internal;

namespace NLog;

/// <summary>
/// It works as a normal <see cref="T:NLog.Logger" /> but it discards all messages which an application requests
/// to be logged.
///
/// It effectively implements the "Null Object" pattern for <see cref="T:NLog.Logger" /> objects.
/// </summary>
public sealed class NullLogger : Logger
{
	/// <summary>
	/// Initializes a new instance of <see cref="T:NLog.NullLogger" />.
	/// </summary>
	/// <param name="factory">The factory class to be used for the creation of this logger.</param>
	public NullLogger(LogFactory factory)
	{
		Guard.ThrowIfNull(factory, "factory");
		string empty = string.Empty;
		ITargetWithFilterChain[] noTargetsByLevel = TargetWithFilterChain.NoTargetsByLevel;
		Initialize(empty, noTargetsByLevel, factory);
	}
}

namespace NLog.Config;

internal interface ILoggingRuleLevelFilter
{
	/// <summary>
	/// Level enabled flags for each LogLevel ordinal
	/// </summary>
	bool[] LogLevels { get; }

	LogLevel? FinalMinLevel { get; }

	/// <summary>
	/// Converts the filter into a simple <see cref="T:NLog.Config.LoggingRuleLevelFilter" />
	/// </summary>
	LoggingRuleLevelFilter GetSimpleFilterForUpdate();
}

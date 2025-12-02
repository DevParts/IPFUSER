using System;

namespace NLog.Config;

/// <summary>
/// Default filtering with static level config
/// </summary>
internal sealed class LoggingRuleLevelFilter : ILoggingRuleLevelFilter
{
	public static readonly ILoggingRuleLevelFilter Off = new LoggingRuleLevelFilter();

	public bool[] LogLevels { get; }

	public LogLevel? FinalMinLevel { get; private set; }

	public LoggingRuleLevelFilter(bool[]? logLevels = null, LogLevel? finalMinLevel = null)
	{
		LogLevels = new bool[LogLevel.MaxLevel.Ordinal + 1];
		if (logLevels != null)
		{
			for (int i = 0; i < Math.Min(logLevels.Length, LogLevels.Length); i++)
			{
				LogLevels[i] = logLevels[i];
			}
		}
		FinalMinLevel = finalMinLevel;
	}

	public LoggingRuleLevelFilter GetSimpleFilterForUpdate()
	{
		if (this == Off)
		{
			return new LoggingRuleLevelFilter();
		}
		return this;
	}

	public LoggingRuleLevelFilter SetLoggingLevels(LogLevel minLevel, LogLevel maxLevel, bool enable)
	{
		for (int i = minLevel.Ordinal; i <= Math.Min(maxLevel.Ordinal, LogLevels.Length - 1); i++)
		{
			LogLevels[i] = enable;
		}
		return this;
	}

	public LoggingRuleLevelFilter SetFinalMinLevel(LogLevel? finalMinLevel)
	{
		FinalMinLevel = finalMinLevel;
		return this;
	}
}

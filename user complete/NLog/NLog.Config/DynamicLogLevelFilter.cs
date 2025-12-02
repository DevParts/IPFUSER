using System;
using System.Collections.Generic;
using NLog.Common;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Config;

/// <summary>
/// Dynamic filtering with a positive list of enabled levels
/// </summary>
internal sealed class DynamicLogLevelFilter : ILoggingRuleLevelFilter
{
	private readonly LoggingRule _loggingRule;

	private readonly SimpleLayout? _levelFilter;

	private readonly SimpleLayout? _finalMinLevelFilter;

	private KeyValuePair<string, bool[]> _activeFilter;

	public bool[] LogLevels => GenerateLogLevels();

	public LogLevel? FinalMinLevel => GenerateFinalMinLevel();

	public DynamicLogLevelFilter(LoggingRule loggingRule, SimpleLayout? levelFilter, SimpleLayout? finalMinLevelFilter)
	{
		_loggingRule = loggingRule;
		_levelFilter = levelFilter;
		_finalMinLevelFilter = finalMinLevelFilter;
		_activeFilter = new KeyValuePair<string, bool[]>(string.Empty, LoggingRuleLevelFilter.Off.LogLevels);
	}

	public LoggingRuleLevelFilter GetSimpleFilterForUpdate()
	{
		return new LoggingRuleLevelFilter(LogLevels, FinalMinLevel);
	}

	private bool[] GenerateLogLevels()
	{
		string text = _levelFilter?.Render(LogEventInfo.CreateNullEvent())?.Trim() ?? string.Empty;
		if (string.IsNullOrEmpty(text))
		{
			return LoggingRuleLevelFilter.Off.LogLevels;
		}
		KeyValuePair<string, bool[]> keyValuePair = _activeFilter;
		if (keyValuePair.Key != text)
		{
			bool[] array = ((text.IndexOf(',') < 0) ? ParseSingleLevel(text) : ParseLevels(text));
			if (array == LoggingRuleLevelFilter.Off.LogLevels)
			{
				return array;
			}
			keyValuePair = (_activeFilter = new KeyValuePair<string, bool[]>(text, array));
		}
		return keyValuePair.Value;
	}

	private LogLevel? GenerateFinalMinLevel()
	{
		string logLevel = _finalMinLevelFilter?.Render(LogEventInfo.CreateNullEvent())?.Trim() ?? string.Empty;
		return ParseLogLevel(logLevel, null);
	}

	private bool[] ParseSingleLevel(string levelFilter)
	{
		LogLevel logLevel = ParseLogLevel(levelFilter, null);
		if ((object)logLevel == null || logLevel == LogLevel.Off)
		{
			return LoggingRuleLevelFilter.Off.LogLevels;
		}
		bool[] array = new bool[LogLevel.MaxLevel.Ordinal + 1];
		array[logLevel.Ordinal] = true;
		return array;
	}

	private LogLevel? ParseLogLevel(string logLevel, LogLevel? levelIfEmpty)
	{
		try
		{
			if (string.IsNullOrEmpty(logLevel))
			{
				return levelIfEmpty;
			}
			return LogLevel.FromString(logLevel);
		}
		catch (ArgumentException ex)
		{
			InternalLogger.Warn(ex, "Logging rule {0} with pattern '{1}' has invalid loglevel: {2}", _loggingRule.RuleName, _loggingRule.LoggerNamePattern, logLevel);
			return null;
		}
	}

	private bool[] ParseLevels(string levelFilter)
	{
		string[] array = levelFilter.SplitAndTrimTokens(',');
		if (array.Length == 0)
		{
			return LoggingRuleLevelFilter.Off.LogLevels;
		}
		if (array.Length == 1)
		{
			return ParseSingleLevel(array[0]);
		}
		bool[] array2 = new bool[LogLevel.MaxLevel.Ordinal + 1];
		string[] array3 = array;
		foreach (string levelName in array3)
		{
			try
			{
				LogLevel logLevel = LogLevel.FromString(levelName);
				if (!(logLevel == LogLevel.Off))
				{
					array2[logLevel.Ordinal] = true;
				}
			}
			catch (ArgumentException ex)
			{
				InternalLogger.Warn(ex, "Logging rule {0} with pattern '{1}' has invalid loglevel: {2}", _loggingRule.RuleName, _loggingRule.LoggerNamePattern, levelFilter);
			}
		}
		return array2;
	}
}

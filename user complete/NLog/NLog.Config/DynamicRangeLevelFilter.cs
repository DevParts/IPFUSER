using System;
using System.Collections.Generic;
using NLog.Common;
using NLog.Layouts;

namespace NLog.Config;

/// <summary>
/// Dynamic filtering with a minlevel and maxlevel range
/// </summary>
internal sealed class DynamicRangeLevelFilter : ILoggingRuleLevelFilter
{
	private struct MinMaxLevels : IEquatable<MinMaxLevels>
	{
		private readonly string _minLevel;

		private readonly string _maxLevel;

		public MinMaxLevels(string minLevel, string maxLevel)
		{
			_minLevel = minLevel;
			_maxLevel = maxLevel;
		}

		public bool Equals(MinMaxLevels other)
		{
			if (_minLevel == other._minLevel)
			{
				return _maxLevel == other._maxLevel;
			}
			return false;
		}
	}

	private readonly LoggingRule _loggingRule;

	private readonly SimpleLayout? _minLevel;

	private readonly SimpleLayout? _maxLevel;

	private readonly SimpleLayout? _finalMinLevelFilter;

	private KeyValuePair<MinMaxLevels, bool[]> _activeFilter;

	public bool[] LogLevels => GenerateLogLevels();

	public LogLevel? FinalMinLevel => GenerateFinalMinLevel();

	public DynamicRangeLevelFilter(LoggingRule loggingRule, SimpleLayout? minLevel, SimpleLayout? maxLevel, SimpleLayout? finalMinLevelFilter)
	{
		_loggingRule = loggingRule;
		_minLevel = minLevel;
		_maxLevel = maxLevel;
		_finalMinLevelFilter = finalMinLevelFilter;
		_activeFilter = new KeyValuePair<MinMaxLevels, bool[]>(new MinMaxLevels(string.Empty, string.Empty), LoggingRuleLevelFilter.Off.LogLevels);
	}

	public LoggingRuleLevelFilter GetSimpleFilterForUpdate()
	{
		return new LoggingRuleLevelFilter(LogLevels, FinalMinLevel);
	}

	private bool[] GenerateLogLevels()
	{
		string text = _minLevel?.Render(LogEventInfo.CreateNullEvent())?.Trim() ?? string.Empty;
		string text2 = _maxLevel?.Render(LogEventInfo.CreateNullEvent())?.Trim() ?? string.Empty;
		KeyValuePair<MinMaxLevels, bool[]> keyValuePair = _activeFilter;
		if (!keyValuePair.Key.Equals(new MinMaxLevels(text, text2)))
		{
			bool[] value = ParseLevelRange(text, text2);
			keyValuePair = (_activeFilter = new KeyValuePair<MinMaxLevels, bool[]>(new MinMaxLevels(text, text2), value));
		}
		return keyValuePair.Value;
	}

	private LogLevel? GenerateFinalMinLevel()
	{
		string logLevel = _finalMinLevelFilter?.Render(LogEventInfo.CreateNullEvent())?.Trim() ?? string.Empty;
		return ParseLogLevel(logLevel, null);
	}

	private bool[] ParseLevelRange(string minLevelFilter, string maxLevelFilter)
	{
		if (string.IsNullOrEmpty(minLevelFilter) && string.IsNullOrEmpty(maxLevelFilter))
		{
			return LoggingRuleLevelFilter.Off.LogLevels;
		}
		LogLevel logLevel = ParseLogLevel(minLevelFilter, LogLevel.MinLevel);
		LogLevel logLevel2 = ParseLogLevel(maxLevelFilter, LogLevel.MaxLevel);
		bool[] array = new bool[LogLevel.MaxLevel.Ordinal + 1];
		if (logLevel != null && logLevel2 != null)
		{
			for (int i = logLevel.Ordinal; i <= array.Length - 1 && i <= logLevel2.Ordinal; i++)
			{
				array[i] = true;
			}
		}
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
}

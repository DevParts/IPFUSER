using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using NLog.Filters;
using NLog.Internal;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.Config;

/// <summary>
/// Represents a logging rule. An equivalent of &lt;logger /&gt; configuration element.
/// </summary>
[NLogConfigurationItem]
public class LoggingRule
{
	private ILoggingRuleLevelFilter _logLevelFilter = LoggingRuleLevelFilter.Off;

	private LoggerNameMatcher _loggerNameMatcher = LoggerNameMatcher.Off;

	private readonly List<Target> _targets = new List<Target>();

	[Obsolete("Very exotic feature without any unit-tests, not sure if it works. Marked obsolete with NLog v5.3")]
	private readonly List<LoggingRule> _childRules = new List<LoggingRule>();

	/// <summary>
	/// Rule identifier to allow rule lookup
	/// </summary>
	public string? RuleName { get; set; }

	/// <summary>
	/// Gets a collection of targets that should be written to when this rule matches.
	/// </summary>
	public IList<Target> Targets => _targets;

	/// <summary>
	/// Obsolete since too exotic feature with NLog v5.3.
	///
	/// Gets a collection of child rules to be evaluated when this rule matches.
	/// </summary>
	[Obsolete("Very exotic feature without any unit-tests, not sure if it works. Marked obsolete with NLog v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IList<LoggingRule> ChildRules => _childRules;

	/// <summary>
	/// Gets a collection of filters to be checked before writing to targets.
	/// </summary>
	public IList<Filter> Filters { get; } = new List<Filter>();

	/// <summary>
	/// Gets or sets a value indicating whether to quit processing any following rules when this one matches.
	/// </summary>
	public bool Final { get; set; }

	/// <summary>
	/// Gets or sets the <see cref="T:NLog.LogLevel" /> whether to quit processing any following rules when lower severity and this one matches.
	/// </summary>
	/// <remarks>
	/// Loggers matching will be restricted to specified minimum level for following rules.
	/// </remarks>
	public LogLevel? FinalMinLevel
	{
		get
		{
			return _logLevelFilter.FinalMinLevel;
		}
		set
		{
			_logLevelFilter = _logLevelFilter.GetSimpleFilterForUpdate().SetFinalMinLevel(value);
		}
	}

	/// <summary>
	/// Gets or sets logger name pattern.
	/// </summary>
	/// <remarks>
	/// Logger name pattern used by <see cref="M:NLog.Config.LoggingRule.NameMatches(System.String)" /> to check if a logger name matches this rule.
	/// It may include one or more '*' or '?' wildcards at any position.
	/// <list type="bullet">
	/// <item>'*' means zero or more occurrences of any character</item>
	/// <item>'?' means exactly one occurrence of any character</item>
	/// </list>
	/// </remarks>
	public string LoggerNamePattern
	{
		get
		{
			return _loggerNameMatcher.Pattern;
		}
		set
		{
			_loggerNameMatcher = LoggerNameMatcher.Create(value);
		}
	}

	internal bool[] LogLevels => _logLevelFilter.LogLevels;

	/// <summary>
	/// Gets the collection of log levels enabled by this rule.
	/// </summary>
	[NLogConfigurationIgnoreProperty]
	public ReadOnlyCollection<LogLevel> Levels
	{
		get
		{
			List<LogLevel> list = new List<LogLevel>();
			bool[] logLevels = _logLevelFilter.LogLevels;
			for (int i = LogLevel.MinLevel.Ordinal; i <= LogLevel.MaxLevel.Ordinal; i++)
			{
				if (logLevels[i])
				{
					list.Add(LogLevel.FromOrdinal(i));
				}
			}
			return list.AsReadOnly();
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Config.LoggingRule.FilterDefaultAction" /> with NLog v5.
	///
	/// Default action when filters not matching
	/// </summary>
	/// <remarks>
	/// NLog v4.6 introduced the setting with default value <see cref="F:NLog.Filters.FilterResult.Neutral" />.
	/// NLog v5 marked it as obsolete and change default value to <see cref="F:NLog.Filters.FilterResult.Ignore" />
	/// </remarks>
	[Obsolete("Replaced by FilterDefaultAction. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public FilterResult DefaultFilterResult
	{
		get
		{
			return FilterDefaultAction;
		}
		set
		{
			FilterDefaultAction = value;
		}
	}

	/// <summary>
	/// Default action if none of the filters match
	/// </summary>
	/// <remarks>
	/// NLog v5 changed default value to <see cref="F:NLog.Filters.FilterResult.Ignore" />
	/// </remarks>
	public FilterResult FilterDefaultAction { get; set; } = FilterResult.Ignore;

	/// <summary>
	/// Create an empty <see cref="T:NLog.Config.LoggingRule" />.
	/// </summary>
	public LoggingRule()
	{
	}

	/// <summary>
	/// Create an empty <see cref="T:NLog.Config.LoggingRule" />.
	/// </summary>
	public LoggingRule(string? ruleName)
	{
		RuleName = ruleName;
	}

	/// <summary>
	/// Create a new <see cref="T:NLog.Config.LoggingRule" /> with a <paramref name="minLevel" /> and  <paramref name="maxLevel" /> which writes to <paramref name="target" />.
	/// </summary>
	/// <param name="loggerNamePattern">Logger name pattern used for <see cref="P:NLog.Config.LoggingRule.LoggerNamePattern" />. It may include one or more '*' or '?' wildcards at any position.</param>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="maxLevel">Maximum log level needed to trigger this rule.</param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	public LoggingRule(string loggerNamePattern, LogLevel minLevel, LogLevel maxLevel, Target target)
	{
		Guard.ThrowIfNull(target, "target");
		LoggerNamePattern = loggerNamePattern;
		_targets.Add(target);
		EnableLoggingForLevels(minLevel, maxLevel);
	}

	/// <summary>
	/// Create a new <see cref="T:NLog.Config.LoggingRule" /> with a <paramref name="minLevel" /> which writes to <paramref name="target" />.
	/// </summary>
	/// <param name="loggerNamePattern">Logger name pattern used for <see cref="P:NLog.Config.LoggingRule.LoggerNamePattern" />. It may include one or more '*' or '?' wildcards at any position.</param>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	public LoggingRule(string loggerNamePattern, LogLevel minLevel, Target target)
	{
		Guard.ThrowIfNull(target, "target");
		LoggerNamePattern = loggerNamePattern;
		_targets.Add(target);
		EnableLoggingForLevels(minLevel, LogLevel.MaxLevel);
	}

	/// <summary>
	/// Create a (disabled) <see cref="T:NLog.Config.LoggingRule" />. You should call <see cref="M:NLog.Config.LoggingRule.EnableLoggingForLevel(NLog.LogLevel)" /> or <see cref="M:NLog.Config.LoggingRule.EnableLoggingForLevels(NLog.LogLevel,NLog.LogLevel)" /> to enable logging.
	/// </summary>
	/// <param name="loggerNamePattern">Logger name pattern used for <see cref="P:NLog.Config.LoggingRule.LoggerNamePattern" />. It may include one or more '*' or '?' wildcards at any position.</param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	public LoggingRule(string loggerNamePattern, Target target)
	{
		Guard.ThrowIfNull(target, "target");
		LoggerNamePattern = loggerNamePattern;
		_targets.Add(target);
	}

	[Obsolete("Very exotic feature without any unit-tests, not sure if it works. Marked obsolete with NLog v5.3")]
	internal LoggingRule[] GetChildRulesThreadSafe()
	{
		lock (_childRules)
		{
			return _childRules.ToArray();
		}
	}

	internal Target[] GetTargetsThreadSafe()
	{
		lock (_targets)
		{
			return (_targets.Count == 0) ? ArrayHelper.Empty<Target>() : _targets.ToArray();
		}
	}

	internal bool RemoveTargetThreadSafe(Target target)
	{
		lock (_targets)
		{
			return _targets.Remove(target);
		}
	}

	/// <summary>
	/// Enables logging for a particular level.
	/// </summary>
	/// <param name="level">Level to be enabled.</param>
	public void EnableLoggingForLevel(LogLevel level)
	{
		if (!(level == LogLevel.Off))
		{
			Guard.ThrowIfNull(level, "level");
			_logLevelFilter = _logLevelFilter.GetSimpleFilterForUpdate().SetLoggingLevels(level, level, enable: true);
		}
	}

	/// <summary>
	/// Enables logging for a particular levels between (included) <paramref name="minLevel" /> and <paramref name="maxLevel" />.
	/// </summary>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="maxLevel">Maximum log level needed to trigger this rule.</param>
	public void EnableLoggingForLevels(LogLevel minLevel, LogLevel maxLevel)
	{
		if (!(minLevel == LogLevel.Off))
		{
			Guard.ThrowIfNull(minLevel, "minLevel");
			Guard.ThrowIfNull(maxLevel, "maxLevel");
			_logLevelFilter = _logLevelFilter.GetSimpleFilterForUpdate().SetLoggingLevels(minLevel, maxLevel, enable: true);
		}
	}

	internal void EnableLoggingForLevelLayout(SimpleLayout? simpleLayout, SimpleLayout? finalMinLevel)
	{
		_logLevelFilter = new DynamicLogLevelFilter(this, simpleLayout, finalMinLevel);
	}

	internal void EnableLoggingForLevelsLayout(SimpleLayout? minLevel, SimpleLayout? maxLevel, SimpleLayout? finalMinLevel)
	{
		_logLevelFilter = new DynamicRangeLevelFilter(this, minLevel, maxLevel, finalMinLevel);
	}

	/// <summary>
	/// Disables logging for a particular level.
	/// </summary>
	/// <param name="level">Level to be disabled.</param>
	public void DisableLoggingForLevel(LogLevel level)
	{
		if (!(level == LogLevel.Off))
		{
			Guard.ThrowIfNull(level, "level");
			_logLevelFilter = _logLevelFilter.GetSimpleFilterForUpdate().SetLoggingLevels(level, level, enable: false);
		}
	}

	/// <summary>
	/// Disables logging for particular levels between (included) <paramref name="minLevel" /> and <paramref name="maxLevel" />.
	/// </summary>
	/// <param name="minLevel">Minimum log level to be disables.</param>
	/// <param name="maxLevel">Maximum log level to be disabled.</param>
	public void DisableLoggingForLevels(LogLevel minLevel, LogLevel maxLevel)
	{
		if (!(minLevel == LogLevel.Off))
		{
			Guard.ThrowIfNull(minLevel, "minLevel");
			Guard.ThrowIfNull(maxLevel, "maxLevel");
			_logLevelFilter = _logLevelFilter.GetSimpleFilterForUpdate().SetLoggingLevels(minLevel, maxLevel, enable: false);
		}
	}

	/// <summary>
	/// Enables logging the levels between (included) <paramref name="minLevel" /> and <paramref name="maxLevel" />. All the other levels will be disabled.
	/// </summary>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="maxLevel">Maximum log level needed to trigger this rule.</param>
	public void SetLoggingLevels(LogLevel minLevel, LogLevel maxLevel)
	{
		DisableLoggingForLevels(LogLevel.MinLevel, LogLevel.MaxLevel);
		EnableLoggingForLevels(minLevel, maxLevel);
	}

	/// <summary>
	/// Returns a string representation of <see cref="T:NLog.Config.LoggingRule" />. Used for debugging.
	/// </summary>
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_loggerNameMatcher.ToString());
		stringBuilder.Append(" levels: [ ");
		Target[] targetsThreadSafe = GetTargetsThreadSafe();
		bool[] logLevels = _logLevelFilter.LogLevels;
		for (int i = 0; i < logLevels.Length; i++)
		{
			if (targetsThreadSafe.Length == 0 && !Final && FinalMinLevel != null)
			{
				if (i < FinalMinLevel.Ordinal)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} ", LogLevel.FromOrdinal(i).ToString());
				}
			}
			else if (logLevels[i])
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} ", LogLevel.FromOrdinal(i).ToString());
			}
		}
		stringBuilder.Append("] writeTo: [ ");
		Target[] array = targetsThreadSafe;
		foreach (Target target in array)
		{
			string arg = (string.IsNullOrEmpty(target.Name) ? target.ToString() : target.Name);
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0} ", arg);
		}
		stringBuilder.Append(']');
		if (Final)
		{
			stringBuilder.Append(" final: True");
		}
		if (FinalMinLevel != null)
		{
			stringBuilder.Append(" finalMinLevel: ").Append(FinalMinLevel);
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	/// Checks whether the particular log level is enabled for this rule.
	/// </summary>
	/// <param name="level">Level to be checked.</param>
	/// <returns>A value of <see langword="true" /> when the log level is enabled, <see langword="false" /> otherwise.</returns>
	public bool IsLoggingEnabledForLevel(LogLevel level)
	{
		if (level == LogLevel.Off)
		{
			return false;
		}
		return _logLevelFilter.LogLevels[level.Ordinal];
	}

	/// <summary>
	/// Checks whether given name matches the <see cref="P:NLog.Config.LoggingRule.LoggerNamePattern" />.
	/// </summary>
	/// <param name="loggerName">String to be matched.</param>
	/// <returns>A value of <see langword="true" /> when the name matches, <see langword="false" /> otherwise.</returns>
	public bool NameMatches(string loggerName)
	{
		return _loggerNameMatcher.NameMatches(loggerName);
	}
}

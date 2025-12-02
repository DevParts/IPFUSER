using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog.Common;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;

namespace NLog.Internal;

/// <summary>
/// Represents target with a chain of filters which determine
/// whether logging should happen.
/// </summary>
internal sealed class TargetWithFilterChain : ITargetWithFilterChain
{
	private struct CallSiteKey : IEquatable<CallSiteKey>
	{
		public readonly string MethodName;

		public readonly string FileSourceName;

		public readonly int FileSourceLineNumber;

		public CallSiteKey(string? methodName, string? fileSourceName, int fileSourceLineNumber)
		{
			MethodName = methodName ?? string.Empty;
			FileSourceName = fileSourceName ?? string.Empty;
			FileSourceLineNumber = fileSourceLineNumber;
		}

		public override int GetHashCode()
		{
			return MethodName.GetHashCode() ^ FileSourceName.GetHashCode() ^ FileSourceLineNumber;
		}

		public override bool Equals(object obj)
		{
			if (obj is CallSiteKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(CallSiteKey other)
		{
			if (FileSourceLineNumber == other.FileSourceLineNumber && string.Equals(FileSourceName, other.FileSourceName, StringComparison.Ordinal))
			{
				return string.Equals(MethodName, other.MethodName, StringComparison.Ordinal);
			}
			return false;
		}
	}

	internal static readonly TargetWithFilterChain[] NoTargetsByLevel = CreateLoggerConfiguration();

	private MruCache<CallSiteKey, string>? _callSiteClassNameCache;

	/// <summary>
	/// Gets the target.
	/// </summary>
	/// <value>The target.</value>
	public Target Target { get; }

	/// <summary>
	/// Gets the filter chain.
	/// </summary>
	/// <value>The filter chain.</value>
	public IList<Filter> FilterChain { get; }

	/// <summary>
	/// Gets or sets the next <see cref="T:NLog.Internal.TargetWithFilterChain" /> item in the chain.
	/// </summary>
	/// <value>The next item in the chain.</value>
	/// <example>This is for example the 'target2' logger in writeTo='target1,target2'  </example>
	public TargetWithFilterChain? NextInChain { get; set; }

	/// <summary>
	/// Gets the stack trace usage.
	/// </summary>
	/// <returns>A <see cref="P:NLog.Internal.TargetWithFilterChain.StackTraceUsage" /> value that determines stack trace handling.</returns>
	public StackTraceUsage StackTraceUsage { get; private set; }

	/// <summary>
	/// Default action if none of the filters match.
	/// </summary>
	public FilterResult FilterDefaultAction { get; }

	private static TargetWithFilterChain[] CreateLoggerConfiguration()
	{
		return new TargetWithFilterChain[LogLevel.MaxLevel.Ordinal + 2];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Internal.TargetWithFilterChain" /> class.
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="filterChain">The filter chain.</param>
	/// <param name="filterDefaultAction">Default action if none of the filters match.</param>
	public TargetWithFilterChain(Target target, IList<Filter> filterChain, FilterResult filterDefaultAction)
	{
		Target = target;
		FilterChain = filterChain;
		FilterDefaultAction = filterDefaultAction;
	}

	internal StackTraceUsage PrecalculateStackTraceUsage()
	{
		StackTraceUsage stackTraceUsage = StackTraceUsage.None;
		if (Target != null)
		{
			stackTraceUsage = Target.StackTraceUsage;
		}
		if (NextInChain != null && (stackTraceUsage & StackTraceUsage.WithSource) != StackTraceUsage.WithSource)
		{
			StackTraceUsage stackTraceUsage2 = NextInChain.PrecalculateStackTraceUsage();
			stackTraceUsage |= stackTraceUsage2;
		}
		StackTraceUsage = stackTraceUsage;
		return stackTraceUsage;
	}

	internal static TargetWithFilterChain[] BuildLoggerConfiguration(string loggerName, LoggingRule[] loggingRules, LogLevel globalLogLevel)
	{
		TargetWithFilterChain[] array = CreateLoggerConfiguration();
		TargetWithFilterChain[] lastTargetsByLevel = CreateLoggerConfiguration();
		bool[] suppressedLevels = new bool[LogLevel.MaxLevel.Ordinal + 1];
		if (!GetTargetsByLevelForLogger(loggerName, loggingRules, globalLogLevel, array, lastTargetsByLevel, suppressedLevels))
		{
			return NoTargetsByLevel;
		}
		return array;
	}

	private static bool GetTargetsByLevelForLogger(string name, LoggingRule[] loggingRules, LogLevel globalLogLevel, TargetWithFilterChain[] targetsByLevel, TargetWithFilterChain[] lastTargetsByLevel, bool[] suppressedLevels)
	{
		IList<KeyValuePair<FilterResult?, IList<Filter>>> finalMinLevelWithFilters = null;
		bool flag = false;
		foreach (LoggingRule loggingRule in loggingRules)
		{
			if (loggingRule.NameMatches(name))
			{
				if (LoggingRuleHasFinalMinLevelFilters(loggingRule))
				{
					CollectFinalMinLevelFiltersFromRule(loggingRule, ref finalMinLevelWithFilters);
				}
				flag = AddTargetsFromLoggingRule(loggingRule, name, globalLogLevel, targetsByLevel, lastTargetsByLevel, suppressedLevels) || flag;
				if (loggingRule.ChildRules.Count != 0)
				{
					flag = GetTargetsByLevelForLogger(name, loggingRule.GetChildRulesThreadSafe(), globalLogLevel, targetsByLevel, lastTargetsByLevel, suppressedLevels) || flag;
				}
			}
		}
		for (int j = 0; j <= LogLevel.MaxLevel.Ordinal; j++)
		{
			TargetWithFilterChain targetWithFilterChain = targetsByLevel[j];
			if (targetWithFilterChain == null)
			{
				continue;
			}
			if (finalMinLevelWithFilters != null && finalMinLevelWithFilters.Count > 0)
			{
				KeyValuePair<FilterResult?, IList<Filter>> keyValuePair = finalMinLevelWithFilters[j];
				IList<Filter> value = keyValuePair.Value;
				if (value != null && value.Count > 0 && keyValuePair.Key.HasValue)
				{
					targetWithFilterChain = (targetsByLevel[j] = AppendFinalMinLevelFilters(targetWithFilterChain, keyValuePair.Value, keyValuePair.Key.Value));
				}
			}
			targetWithFilterChain.PrecalculateStackTraceUsage();
		}
		return flag;
	}

	private static bool LoggingRuleHasFinalMinLevelFilters(LoggingRule rule)
	{
		if (rule.FinalMinLevel != LogLevel.Off && rule.Filters.Count != 0)
		{
			return rule.Targets.Count == 0;
		}
		return false;
	}

	private static void CollectFinalMinLevelFiltersFromRule(LoggingRule rule, ref IList<KeyValuePair<FilterResult?, IList<Filter>>>? finalMinLevelWithFilters)
	{
		LogLevel finalMinLevel = rule.FinalMinLevel;
		if ((object)finalMinLevel == null)
		{
			return;
		}
		finalMinLevelWithFilters = finalMinLevelWithFilters ?? new KeyValuePair<FilterResult?, IList<Filter>>[LogLevel.MaxLevel.Ordinal + 1];
		for (int i = 0; i <= LogLevel.MaxLevel.Ordinal; i++)
		{
			if (i < finalMinLevel.Ordinal)
			{
				continue;
			}
			FilterResult filterResult = finalMinLevelWithFilters[i].Key ?? rule.FilterDefaultAction;
			if (filterResult == rule.FilterDefaultAction)
			{
				IList<Filter> value = finalMinLevelWithFilters[i].Value;
				IList<Filter> list;
				if (value == null || value.Count <= 0)
				{
					list = rule.Filters;
				}
				else
				{
					IList<Filter> list2 = finalMinLevelWithFilters[i].Value.Concat<Filter>(rule.Filters).ToArray();
					list = list2;
				}
				IList<Filter> value2 = list;
				finalMinLevelWithFilters[i] = new KeyValuePair<FilterResult?, IList<Filter>>(filterResult, value2);
			}
		}
	}

	private static TargetWithFilterChain AppendFinalMinLevelFilters(TargetWithFilterChain targetsByLevel, IList<Filter> finalMinLevelFilters, FilterResult finalMinLevelDefaultResult)
	{
		IList<Filter> filterChain = targetsByLevel.FilterChain;
		if (filterChain != null && filterChain.Count > 0 && targetsByLevel.FilterDefaultAction != finalMinLevelDefaultResult)
		{
			return targetsByLevel;
		}
		IList<Filter> filterChain2 = targetsByLevel.FilterChain;
		IList<Filter> list;
		if (filterChain2 == null || filterChain2.Count <= 0)
		{
			list = finalMinLevelFilters;
		}
		else
		{
			IList<Filter> list2 = finalMinLevelFilters.Concat<Filter>(targetsByLevel.FilterChain).ToArray();
			list = list2;
		}
		IList<Filter> filterChain3 = list;
		TargetWithFilterChain targetWithFilterChain = new TargetWithFilterChain(targetsByLevel.Target, filterChain3, finalMinLevelDefaultResult);
		TargetWithFilterChain targetWithFilterChain2 = ((targetsByLevel.NextInChain == null) ? null : AppendFinalMinLevelFilters(targetsByLevel.NextInChain, finalMinLevelFilters, finalMinLevelDefaultResult));
		targetWithFilterChain.NextInChain = targetWithFilterChain2 ?? targetsByLevel.NextInChain;
		return targetWithFilterChain;
	}

	private static bool AddTargetsFromLoggingRule(LoggingRule rule, string loggerName, LogLevel globalLogLevel, TargetWithFilterChain[] targetsByLevel, TargetWithFilterChain[] lastTargetsByLevel, bool[] suppressedLevels)
	{
		bool result = false;
		bool flag = false;
		LogLevel finalMinLevel = rule.FinalMinLevel;
		bool[] logLevels = rule.LogLevels;
		for (int i = 0; i <= LogLevel.MaxLevel.Ordinal; i++)
		{
			if (SuppressLogLevel(rule, logLevels, finalMinLevel, globalLogLevel, i, ref suppressedLevels[i]))
			{
				continue;
			}
			Target[] targetsThreadSafe = rule.GetTargetsThreadSafe();
			foreach (Target target in targetsThreadSafe)
			{
				result = true;
				TargetWithFilterChain targetWithFilterChain = CreateTargetChainFromLoggingRule(rule, target, targetsByLevel[i]);
				if (targetWithFilterChain == null)
				{
					if (!flag)
					{
						InternalLogger.Warn("Logger: {0} configured with duplicate output to target: {1}. LoggingRule with NamePattern='{2}' and Level={3} has been skipped.", loggerName, target, rule.LoggerNamePattern, LogLevel.FromOrdinal(i));
					}
					flag = true;
				}
				else
				{
					if (lastTargetsByLevel[i] != null)
					{
						lastTargetsByLevel[i].NextInChain = targetWithFilterChain;
					}
					else
					{
						targetsByLevel[i] = targetWithFilterChain;
					}
					lastTargetsByLevel[i] = targetWithFilterChain;
				}
			}
		}
		return result;
	}

	private static bool SuppressLogLevel(LoggingRule rule, bool[] ruleLogLevels, LogLevel? finalMinLevel, LogLevel globalLogLevel, int logLevelOrdinal, ref bool suppressedLevels)
	{
		if (logLevelOrdinal < globalLogLevel.Ordinal)
		{
			return true;
		}
		if ((object)finalMinLevel == null)
		{
			if (suppressedLevels)
			{
				return true;
			}
		}
		else
		{
			suppressedLevels = finalMinLevel.Ordinal > logLevelOrdinal;
		}
		if (!ruleLogLevels[logLevelOrdinal])
		{
			return true;
		}
		if (rule.Final)
		{
			suppressedLevels = true;
		}
		return false;
	}

	private static TargetWithFilterChain? CreateTargetChainFromLoggingRule(LoggingRule rule, Target target, TargetWithFilterChain existingTargets)
	{
		IList<Filter> list;
		if (rule.Filters.Count != 0)
		{
			list = rule.Filters;
		}
		else
		{
			IList<Filter> list2 = ArrayHelper.Empty<Filter>();
			list = list2;
		}
		IList<Filter> filterChain = list;
		TargetWithFilterChain targetWithFilterChain = new TargetWithFilterChain(target, filterChain, rule.FilterDefaultAction);
		if (existingTargets != null && targetWithFilterChain.FilterChain.Count == 0)
		{
			for (TargetWithFilterChain targetWithFilterChain2 = existingTargets; targetWithFilterChain2 != null; targetWithFilterChain2 = targetWithFilterChain2.NextInChain)
			{
				if (target == targetWithFilterChain2.Target && targetWithFilterChain2.FilterChain.Count == 0)
				{
					return null;
				}
			}
		}
		return targetWithFilterChain;
	}

	internal bool TryRememberCallSiteClassName(LogEventInfo logEvent)
	{
		if (string.IsNullOrEmpty(logEvent.CallSiteInformation?.CallerFilePath))
		{
			return false;
		}
		string text = logEvent.CallSiteInformation?.GetCallerClassName(null, includeNameSpace: true, cleanAsyncMoveNext: true, cleanAnonymousDelegates: true);
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (_callSiteClassNameCache == null)
		{
			return false;
		}
		string value = ((logEvent.LoggerName == text) ? logEvent.LoggerName : string.Intern(text));
		CallSiteKey key = new CallSiteKey(logEvent.CallerMemberName, logEvent.CallerFilePath, logEvent.CallerLineNumber);
		return _callSiteClassNameCache.TryAddValue(key, value);
	}

	internal bool TryLookupCallSiteClassName(LogEventInfo logEvent, out string? callSiteClassName)
	{
		callSiteClassName = logEvent.CallSiteInformation?.CallerClassName;
		if (!string.IsNullOrEmpty(callSiteClassName))
		{
			return true;
		}
		if (_callSiteClassNameCache == null)
		{
			Interlocked.CompareExchange(ref _callSiteClassNameCache, new MruCache<CallSiteKey, string>(1000), null);
		}
		CallSiteKey key = new CallSiteKey(logEvent.CallerMemberName, logEvent.CallerFilePath, logEvent.CallerLineNumber);
		return _callSiteClassNameCache.TryGetValue(key, out callSiteClassName);
	}

	public void WriteToLoggerTargets(Type loggerType, LogEventInfo logEvent, LogFactory logFactory)
	{
		LoggerImpl.Write(loggerType, this, logEvent, logFactory);
	}
}

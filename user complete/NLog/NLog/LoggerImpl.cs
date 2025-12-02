using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLog.Common;
using NLog.Config;
using NLog.Filters;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Implementation of logging engine.
/// </summary>
internal static class LoggerImpl
{
	private const int StackTraceSkipMethods = 0;

	internal static void Write(Type loggerType, TargetWithFilterChain targetsForLevel, LogEventInfo logEvent, LogFactory logFactory)
	{
		logEvent.SetMessageFormatter(logFactory.ActiveMessageFormatter, (targetsForLevel.NextInChain == null) ? logFactory.SingleTargetMessageFormatter : null);
		if (targetsForLevel.StackTraceUsage != StackTraceUsage.None)
		{
			CaptureCallSiteInfo(loggerType, targetsForLevel, logEvent, logFactory);
		}
		AsyncContinuation asyncContinuation = SingleCallContinuation.Completed;
		if (logFactory.ThrowExceptions)
		{
			int originalThreadId = AsyncHelpers.GetManagedThreadId();
			asyncContinuation = delegate(Exception? ex)
			{
				if (ex != null && AsyncHelpers.GetManagedThreadId() == originalThreadId)
				{
					throw new NLogRuntimeException("Exception occurred in NLog", ex);
				}
			};
		}
		IList<Filter> list = ArrayHelper.Empty<Filter>();
		FilterResult filterResult = FilterResult.Neutral;
		for (TargetWithFilterChain targetWithFilterChain = targetsForLevel; targetWithFilterChain != null; targetWithFilterChain = targetWithFilterChain.NextInChain)
		{
			IList<Filter> filterChain = targetWithFilterChain.FilterChain;
			FilterResult filterResult2 = ((list == filterChain) ? filterResult : GetFilterResult(filterChain, logEvent, targetWithFilterChain.FilterDefaultAction));
			if (filterResult2 != FilterResult.Ignore && filterResult2 != FilterResult.IgnoreFinal)
			{
				targetWithFilterChain.Target.WriteAsyncLogEvent(logEvent.WithContinuation(asyncContinuation));
				if (filterResult2 == FilterResult.LogFinal)
				{
					break;
				}
			}
			else
			{
				InternalLogger.Debug("{0} [{1}] Rejecting message because of a filter.", logEvent.LoggerName, logEvent.Level);
				if (filterResult2 == FilterResult.IgnoreFinal)
				{
					break;
				}
			}
			filterResult = filterResult2;
			list = filterChain;
		}
	}

	private static void CaptureCallSiteInfo(Type loggerType, TargetWithFilterChain targetsForLevel, LogEventInfo logEvent, LogFactory logFactory)
	{
		StackTraceUsage stackTraceUsage = targetsForLevel.StackTraceUsage;
		bool flag = TryCallSiteClassNameOptimization(stackTraceUsage, logEvent);
		if (flag && targetsForLevel.TryLookupCallSiteClassName(logEvent, out string callSiteClassName))
		{
			logEvent.GetCallSiteInformationInternal().CallerClassName = callSiteClassName;
		}
		else
		{
			if (!flag && !MustCaptureStackTrace(stackTraceUsage, logEvent))
			{
				return;
			}
			try
			{
				bool fNeedFileInfo = (stackTraceUsage & StackTraceUsage.WithFileNameAndLineNumber) != 0;
				StackTrace stackTrace = new StackTrace(0, fNeedFileInfo);
				logEvent.GetCallSiteInformationInternal().SetStackTrace(stackTrace, null, loggerType);
			}
			catch (Exception ex)
			{
				if (logFactory.ThrowExceptions || LogManager.ThrowExceptions)
				{
					throw;
				}
				InternalLogger.Warn(ex, "{0} Failed to capture CallSite. Platform might not support ${{callsite}}", logEvent.LoggerName);
			}
			if (flag)
			{
				targetsForLevel.TryRememberCallSiteClassName(logEvent);
			}
		}
	}

	internal static bool TryCallSiteClassNameOptimization(StackTraceUsage stackTraceUsage, LogEventInfo logEvent)
	{
		if ((stackTraceUsage & (StackTraceUsage.WithStackTrace | StackTraceUsage.WithCallSiteClassName)) != StackTraceUsage.WithCallSiteClassName)
		{
			return false;
		}
		if (string.IsNullOrEmpty(logEvent.CallSiteInformation?.CallerFilePath))
		{
			return false;
		}
		if (logEvent.HasStackTrace)
		{
			return false;
		}
		return true;
	}

	internal static bool MustCaptureStackTrace(StackTraceUsage stackTraceUsage, LogEventInfo logEvent)
	{
		if (logEvent.HasStackTrace)
		{
			return false;
		}
		if ((stackTraceUsage & StackTraceUsage.WithStackTrace) != StackTraceUsage.None)
		{
			return true;
		}
		if ((stackTraceUsage & StackTraceUsage.WithCallSite) != StackTraceUsage.None && string.IsNullOrEmpty(logEvent.CallSiteInformation?.CallerMethodName) && string.IsNullOrEmpty(logEvent.CallSiteInformation?.CallerFilePath))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gets the filter result.
	/// </summary>
	/// <param name="filterChain">The filter chain.</param>
	/// <param name="logEvent">The log event.</param>
	/// <param name="filterDefaultAction">default result if there are no filters, or none of the filters decides.</param>
	/// <returns>The result of the filter.</returns>
	private static FilterResult GetFilterResult(IList<Filter> filterChain, LogEventInfo logEvent, FilterResult filterDefaultAction)
	{
		if (filterChain.Count == 0)
		{
			return FilterResult.Neutral;
		}
		try
		{
			for (int i = 0; i < filterChain.Count; i++)
			{
				FilterResult filterResult = filterChain[i].GetFilterResult(logEvent);
				if (filterResult != FilterResult.Neutral)
				{
					return filterResult;
				}
			}
			return filterDefaultAction;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Exception during filter evaluation. Message will be ignore.");
			if (ex.MustBeRethrown())
			{
				throw;
			}
			return FilterResult.Ignore;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using NLog.Common;

namespace NLog.Internal;

internal sealed class CallSiteInformation
{
	private static readonly object lockObject = new object();

	private static ICollection<Assembly> _hiddenAssemblies = ArrayHelper.Empty<Assembly>();

	private static ICollection<Type> _hiddenTypes = ArrayHelper.Empty<Type>();

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.LogEventInfo.CallerMemberName" /> or ${callsite} with NLog v5.3.
	///
	/// Gets the stack frame of the method that did the logging.
	/// </summary>
	[Obsolete("Instead use ${callsite} or CallerMemberName. Marked obsolete on NLog 5.3")]
	public StackFrame? UserStackFrame => StackTrace?.GetFrame(UserStackFrameNumberLegacy ?? UserStackFrameNumber);

	/// <summary>
	/// Gets the number index of the stack frame that represents the user
	/// code (not the NLog code).
	/// </summary>
	public int UserStackFrameNumber { get; private set; }

	/// <summary>
	/// Legacy attempt to skip async MoveNext, but caused source file line number to be lost
	/// </summary>
	public int? UserStackFrameNumberLegacy { get; private set; }

	/// <summary>
	/// Gets the entire stack trace.
	/// </summary>
	public StackTrace? StackTrace { get; private set; }

	public string? CallerClassName { get; internal set; }

	public string? CallerMethodName { get; private set; }

	public string? CallerFilePath { get; private set; }

	public int? CallerLineNumber { get; private set; }

	internal static bool IsHiddenAssembly(Assembly assembly)
	{
		if (_hiddenAssemblies.Count != 0)
		{
			return _hiddenAssemblies.Contains(assembly);
		}
		return false;
	}

	internal static bool IsHiddenClassType(Type type)
	{
		if (_hiddenTypes.Count != 0)
		{
			return _hiddenTypes.Contains(type);
		}
		return false;
	}

	/// <summary>
	/// Adds the given assembly which will be skipped
	/// when NLog is trying to find the calling method on stack trace.
	/// </summary>
	/// <param name="assembly">The assembly to skip.</param>
	public static void AddCallSiteHiddenAssembly(Assembly assembly)
	{
		if (_hiddenAssemblies.Contains(assembly) || (object)assembly == null)
		{
			return;
		}
		lock (lockObject)
		{
			if (_hiddenAssemblies.Contains(assembly))
			{
				return;
			}
			_hiddenAssemblies = new HashSet<Assembly>(_hiddenAssemblies) { assembly };
		}
		InternalLogger.Trace("Assembly '{0}' will be hidden in callsite stacktrace", assembly.FullName);
	}

	public static void AddCallSiteHiddenClassType(Type classType)
	{
		if (_hiddenTypes.Contains(classType) || (object)classType == null)
		{
			return;
		}
		lock (lockObject)
		{
			if (_hiddenTypes.Contains(classType))
			{
				return;
			}
			_hiddenTypes = new HashSet<Type>(_hiddenTypes) { classType };
		}
		InternalLogger.Trace("Type '{0}' will be hidden in callsite stacktrace", classType);
	}

	/// <summary>
	/// Sets the stack trace for the event info.
	/// </summary>
	/// <param name="stackTrace">The stack trace.</param>
	/// <param name="userStackFrame">Index of the first user stack frame within the stack trace.</param>
	/// <param name="loggerType">Type of the logger or logger wrapper. This is still Logger if it's a subclass of Logger.</param>
	public void SetStackTrace(StackTrace stackTrace, int? userStackFrame = null, Type? loggerType = null)
	{
		StackTrace = stackTrace;
		if (!userStackFrame.HasValue && stackTrace != null)
		{
			StackFrame[] frames = stackTrace.GetFrames();
			int? num = ((loggerType != null) ? FindCallingMethodOnStackTrace(frames, loggerType) : new int?(0));
			int? num2 = (num.HasValue ? new int?(SkipToUserStackFrameLegacy(frames, num.Value)) : num);
			UserStackFrameNumber = num.GetValueOrDefault();
			UserStackFrameNumberLegacy = ((num2 != num) ? num2 : ((int?)null));
		}
		else
		{
			UserStackFrameNumber = userStackFrame.GetValueOrDefault();
			UserStackFrameNumberLegacy = null;
		}
	}

	/// <summary>
	/// Sets the details retrieved from the Caller Information Attributes
	/// </summary>
	/// <param name="callerClassName"></param>
	/// <param name="callerMethodName"></param>
	/// <param name="callerFilePath"></param>
	/// <param name="callerLineNumber"></param>
	public void SetCallerInfo(string? callerClassName, string? callerMethodName, string? callerFilePath, int callerLineNumber)
	{
		CallerClassName = callerClassName;
		CallerMethodName = callerMethodName;
		CallerFilePath = callerFilePath;
		CallerLineNumber = callerLineNumber;
	}

	public MethodBase? GetCallerStackFrameMethod(int skipFrames)
	{
		return StackTraceUsageUtils.GetStackMethod(StackTrace?.GetFrame(UserStackFrameNumber + skipFrames));
	}

	public string GetCallerClassName(MethodBase? method, bool includeNameSpace, bool cleanAsyncMoveNext, bool cleanAnonymousDelegates)
	{
		string text = CallerClassName ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			if (includeNameSpace)
			{
				return text;
			}
			int num = text.LastIndexOf('.');
			if (num < 0 || num >= text.Length - 1)
			{
				return text;
			}
			return text.Substring(num + 1);
		}
		method = method ?? GetCallerStackFrameMethod(0);
		if ((object)method == null)
		{
			return string.Empty;
		}
		cleanAsyncMoveNext = cleanAsyncMoveNext || UserStackFrameNumberLegacy.HasValue;
		cleanAnonymousDelegates = cleanAnonymousDelegates || UserStackFrameNumberLegacy.HasValue;
		return StackTraceUsageUtils.GetStackFrameMethodClassName(method, includeNameSpace, cleanAsyncMoveNext, cleanAnonymousDelegates) ?? string.Empty;
	}

	public string GetCallerMethodName(MethodBase? method, bool includeMethodInfo, bool cleanAsyncMoveNext, bool cleanAnonymousDelegates)
	{
		string text = CallerMethodName ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		method = method ?? GetCallerStackFrameMethod(0);
		if ((object)method == null)
		{
			return string.Empty;
		}
		cleanAsyncMoveNext = cleanAsyncMoveNext || UserStackFrameNumberLegacy.HasValue;
		cleanAnonymousDelegates = cleanAnonymousDelegates || UserStackFrameNumberLegacy.HasValue;
		return StackTraceUsageUtils.GetStackFrameMethodName(method, includeMethodInfo, cleanAsyncMoveNext, cleanAnonymousDelegates) ?? string.Empty;
	}

	public string GetCallerFilePath(int skipFrames)
	{
		string text = CallerFilePath ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return (StackTrace?.GetFrame(UserStackFrameNumber + skipFrames))?.GetFileName() ?? string.Empty;
	}

	public int GetCallerLineNumber(int skipFrames)
	{
		if (CallerLineNumber.HasValue)
		{
			return CallerLineNumber.Value;
		}
		return (StackTrace?.GetFrame(UserStackFrameNumber + skipFrames))?.GetFileLineNumber() ?? 0;
	}

	/// <summary>
	///  Finds first user stack frame in a stack trace
	/// </summary>
	/// <param name="stackFrames">The stack trace of the logging method invocation</param>
	/// <param name="loggerType">Type of the logger or logger wrapper. This is still Logger if it's a subclass of Logger.</param>
	/// <returns>Index of the first user stack frame or 0 if all stack frames are non-user</returns>
	private static int? FindCallingMethodOnStackTrace(StackFrame[] stackFrames, Type loggerType)
	{
		if (stackFrames == null || stackFrames.Length == 0)
		{
			return null;
		}
		int? num = null;
		int? num2 = null;
		for (int i = 0; i < stackFrames.Length; i++)
		{
			MethodBase stackMethod = StackTraceUsageUtils.GetStackMethod(stackFrames[i]);
			if (!SkipStackFrameWhenHidden(stackMethod))
			{
				if (!num2.HasValue)
				{
					num2 = i;
				}
				if (SkipStackFrameWhenLoggerType(stackMethod, loggerType))
				{
					num = null;
				}
				else if (!num.HasValue)
				{
					num = i;
				}
			}
		}
		return num ?? num2;
	}

	/// <summary>
	/// This is only done for legacy reason, as the correct method-name and line-number should be extracted from the MoveNext-StackFrame
	/// </summary>
	/// <param name="stackFrames">The stack trace of the logging method invocation</param>
	/// <param name="firstUserStackFrame">Starting point for skipping async MoveNext-frames</param>
	private static int SkipToUserStackFrameLegacy(StackFrame[] stackFrames, int firstUserStackFrame)
	{
		for (int i = firstUserStackFrame; i < stackFrames.Length; i++)
		{
			MethodBase stackMethod = StackTraceUsageUtils.GetStackMethod(stackFrames[i]);
			if (SkipStackFrameWhenHidden(stackMethod))
			{
				continue;
			}
			if (stackMethod?.Name == "MoveNext" && stackFrames.Length > i)
			{
				Type type = StackTraceUsageUtils.GetStackMethod(stackFrames[i + 1])?.DeclaringType;
				if (type?.Namespace == "System.Runtime.CompilerServices" || type == typeof(ExecutionContext))
				{
					continue;
				}
			}
			return i;
		}
		return firstUserStackFrame;
	}

	/// <summary>
	/// Skip StackFrame when from hidden Assembly / ClassType
	/// </summary>
	private static bool SkipStackFrameWhenHidden(MethodBase? stackMethod)
	{
		Assembly assembly = StackTraceUsageUtils.LookupAssemblyFromMethod(stackMethod);
		if ((object)assembly == null || IsHiddenAssembly(assembly))
		{
			return true;
		}
		if ((object)stackMethod != null)
		{
			return IsHiddenClassType(stackMethod.DeclaringType);
		}
		return true;
	}

	/// <summary>
	/// Skip StackFrame when type of the logger
	/// </summary>
	private static bool SkipStackFrameWhenLoggerType(MethodBase? stackMethod, Type loggerType)
	{
		Type type = stackMethod?.DeclaringType;
		if (type != null)
		{
			if (!(loggerType == type) && !type.IsSubclassOf(loggerType))
			{
				return loggerType.IsAssignableFrom(type);
			}
			return true;
		}
		return false;
	}
}

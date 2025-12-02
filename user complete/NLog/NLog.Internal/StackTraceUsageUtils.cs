using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NLog.Config;

namespace NLog.Internal;

/// <summary>
/// Utilities for dealing with <see cref="T:NLog.Config.StackTraceUsage" /> values.
/// </summary>
internal static class StackTraceUsageUtils
{
	private static readonly Assembly nlogAssembly = typeof(StackTraceUsageUtils).Assembly;

	private static readonly Assembly mscorlibAssembly = typeof(string).Assembly;

	private static readonly Assembly systemAssembly = typeof(Debug).Assembly;

	public static StackTraceUsage GetStackTraceUsage(bool includeFileName, int skipFrames, bool captureStackTrace)
	{
		if (!captureStackTrace)
		{
			return StackTraceUsage.None;
		}
		if (skipFrames != 0)
		{
			if (!includeFileName)
			{
				return StackTraceUsage.WithStackTrace;
			}
			return StackTraceUsage.WithSource;
		}
		if (includeFileName)
		{
			return StackTraceUsage.WithFileNameAndLineNumber | StackTraceUsage.WithCallSite;
		}
		return StackTraceUsage.WithCallSite;
	}

	public static int GetFrameCount(this StackTrace strackTrace)
	{
		return strackTrace.FrameCount;
	}

	public static string GetStackFrameMethodName(MethodBase method, bool includeMethodInfo, bool cleanAsyncMoveNext, bool cleanAnonymousDelegates)
	{
		if ((object)method == null)
		{
			return string.Empty;
		}
		string text = method.Name;
		Type declaringType = method.DeclaringType;
		if (cleanAsyncMoveNext && text == "MoveNext" && declaringType?.DeclaringType != null && declaringType.Name.IndexOf('<') == 0)
		{
			int num = declaringType.Name.IndexOf('>', 1);
			if (num > 1)
			{
				text = declaringType.Name.Substring(1, num - 1);
				if (text.IndexOf('<') == 0)
				{
					text = text.Substring(1);
				}
			}
		}
		if (cleanAnonymousDelegates && text.IndexOf('<') == 0 && text.IndexOf("__", StringComparison.Ordinal) >= 0 && text.IndexOf('>') >= 0)
		{
			int num2 = text.IndexOf('<') + 1;
			int num3 = text.IndexOf('>');
			text = text.Substring(num2, num3 - num2);
		}
		if (includeMethodInfo && text == method.Name)
		{
			text = method.ToString();
		}
		return text;
	}

	public static string GetStackFrameMethodClassName(MethodBase method, bool includeNameSpace, bool cleanAsyncMoveNext, bool cleanAnonymousDelegates)
	{
		if ((object)method == null)
		{
			return string.Empty;
		}
		Type declaringType = method.DeclaringType;
		if (cleanAsyncMoveNext && method.Name == "MoveNext" && declaringType?.DeclaringType != null)
		{
			string name = declaringType.Name;
			if (name != null && name.IndexOf('<') == 0 && declaringType.Name.IndexOf('>', 1) > 1)
			{
				declaringType = declaringType.DeclaringType;
			}
		}
		if ((object)declaringType == null)
		{
			return string.Empty;
		}
		string text = (includeNameSpace ? declaringType.FullName : declaringType.Name);
		if (cleanAnonymousDelegates && text != null && text.IndexOf("<>", StringComparison.Ordinal) >= 0)
		{
			if (!includeNameSpace && declaringType.DeclaringType != null && declaringType.IsNested)
			{
				text = declaringType.DeclaringType.Name;
			}
			else
			{
				int num = text.IndexOf("+<>", StringComparison.Ordinal);
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
			}
		}
		if (includeNameSpace && text != null && text.IndexOf('.') == -1)
		{
			string namespaceFromTypeAssembly = GetNamespaceFromTypeAssembly(declaringType);
			text = (string.IsNullOrEmpty(namespaceFromTypeAssembly) ? text : (namespaceFromTypeAssembly + "." + text));
		}
		return text ?? string.Empty;
	}

	private static string GetNamespaceFromTypeAssembly(Type? callerClassType)
	{
		Assembly assembly = callerClassType?.Assembly;
		if (assembly != null && assembly != mscorlibAssembly && assembly != systemAssembly)
		{
			string fullName = assembly.FullName;
			if (fullName != null && fullName.IndexOf(',') >= 0 && !fullName.StartsWith("System.", StringComparison.Ordinal) && !fullName.StartsWith("Microsoft.", StringComparison.Ordinal))
			{
				return fullName.Substring(0, fullName.IndexOf(','));
			}
		}
		return string.Empty;
	}

	[UnconditionalSuppressMessage("Trimming - Allow callsite logic", "IL2026")]
	public static MethodBase? GetStackMethod(StackFrame? stackFrame)
	{
		return stackFrame?.GetMethod();
	}

	/// <summary>
	/// Gets the fully qualified name of the class invoking the calling method, including the
	/// namespace but not the assembly.
	/// </summary>
	/// <param name="stackFrame">StackFrame from the calling method</param>
	/// <returns>Fully qualified class name</returns>
	public static string GetClassFullName(StackFrame? stackFrame)
	{
		string text = LookupClassNameFromStackFrame(stackFrame);
		if (string.IsNullOrEmpty(text))
		{
			text = GetClassFullName(new StackTrace(fNeedFileInfo: false));
			if (string.IsNullOrEmpty(text))
			{
				text = GetStackMethod(stackFrame)?.Name ?? string.Empty;
			}
		}
		return text;
	}

	private static string GetClassFullName(StackTrace stackTrace)
	{
		StackFrame[] frames = stackTrace.GetFrames();
		for (int i = 0; i < frames.Length; i++)
		{
			string text = LookupClassNameFromStackFrame(frames[i]);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// Returns the assembly from the provided StackFrame (If not internal assembly)
	/// </summary>
	/// <returns>Valid assembly, or null if assembly was internal</returns>
	public static Assembly? LookupAssemblyFromMethod(MethodBase? method)
	{
		Assembly assembly = method?.DeclaringType?.Assembly ?? method?.Module?.Assembly;
		if (assembly == nlogAssembly)
		{
			return null;
		}
		if (assembly == mscorlibAssembly)
		{
			return null;
		}
		if (assembly == systemAssembly)
		{
			return null;
		}
		return assembly;
	}

	/// <summary>
	/// Returns the classname from the provided StackFrame (If not from internal assembly)
	/// </summary>
	/// <param name="stackFrame"></param>
	/// <returns>Valid class name, or empty string if assembly was internal</returns>
	public static string LookupClassNameFromStackFrame(StackFrame? stackFrame)
	{
		MethodBase stackMethod = GetStackMethod(stackFrame);
		if (stackMethod != null && LookupAssemblyFromMethod(stackMethod) != null)
		{
			string stackFrameMethodClassName = GetStackFrameMethodClassName(stackMethod, includeNameSpace: true, cleanAsyncMoveNext: true, cleanAnonymousDelegates: true);
			if (!string.IsNullOrEmpty(stackFrameMethodClassName))
			{
				if (!stackFrameMethodClassName.StartsWith("System.", StringComparison.Ordinal))
				{
					return stackFrameMethodClassName;
				}
			}
			else
			{
				stackFrameMethodClassName = stackMethod.Name ?? string.Empty;
				if (stackFrameMethodClassName != "lambda_method" && stackFrameMethodClassName != "MoveNext")
				{
					return stackFrameMethodClassName;
				}
			}
		}
		return string.Empty;
	}
}

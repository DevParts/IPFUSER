using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The call site (class name, method name and source information).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Callsite-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Callsite-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("callsite")]
[ThreadAgnostic]
public class CallSiteLayoutRenderer : LayoutRenderer, IUsesStackTrace
{
	/// <summary>
	/// Gets or sets a value indicating whether to render the class name.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool ClassName { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to render the include the namespace with <see cref="P:NLog.LayoutRenderers.CallSiteLayoutRenderer.ClassName" />.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeNamespace { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to render the method name.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool MethodName { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the method name will be cleaned up if it is detected as an anonymous delegate.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool CleanNamesOfAnonymousDelegates { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the method and class names will be cleaned up if it is detected as an async continuation
	/// (everything after an await-statement inside of an async method).
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool CleanNamesOfAsyncContinuations { get; set; } = true;

	/// <summary>
	/// Gets or sets the number of frames to skip.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int SkipFrames { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to render the source file name and line number.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool FileName { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to include source file path.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeSourcePath { get; set; } = true;

	/// <summary>
	/// Logger should capture StackTrace, if it was not provided manually
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool CaptureStackTrace { get; set; } = true;

	/// <inheritdoc />
	StackTraceUsage IUsesStackTrace.StackTraceUsage => (StackTraceUsage)((int)StackTraceUsageUtils.GetStackTraceUsage(FileName, SkipFrames, CaptureStackTrace) | (ClassName ? 8 : 0));

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		CallSiteInformation callSiteInformation = logEvent.CallSiteInformation;
		if (callSiteInformation == null)
		{
			AppendExceptionCallSite(builder, logEvent);
			return;
		}
		if (ClassName || MethodName)
		{
			MethodBase callerStackFrameMethod = callSiteInformation.GetCallerStackFrameMethod(SkipFrames);
			if (ClassName)
			{
				string callerClassName = callSiteInformation.GetCallerClassName(callerStackFrameMethod, IncludeNamespace, CleanNamesOfAsyncContinuations, CleanNamesOfAnonymousDelegates);
				builder.Append(string.IsNullOrEmpty(callerClassName) ? "<no type>" : callerClassName);
			}
			if (MethodName)
			{
				string callerMethodName = callSiteInformation.GetCallerMethodName(callerStackFrameMethod, includeMethodInfo: false, CleanNamesOfAsyncContinuations, CleanNamesOfAnonymousDelegates);
				if (ClassName)
				{
					builder.Append('.');
				}
				builder.Append(string.IsNullOrEmpty(callerMethodName) ? "<no method>" : callerMethodName);
			}
		}
		if (FileName)
		{
			string callerFilePath = callSiteInformation.GetCallerFilePath(SkipFrames);
			if (!string.IsNullOrEmpty(callerFilePath))
			{
				int callerLineNumber = callSiteInformation.GetCallerLineNumber(SkipFrames);
				AppendFileName(builder, callerFilePath, callerLineNumber);
			}
		}
	}

	private void AppendFileName(StringBuilder builder, string fileName, int lineNumber)
	{
		builder.Append('(');
		if (IncludeSourcePath)
		{
			builder.Append(fileName);
		}
		else
		{
			builder.Append(Path.GetFileName(fileName));
		}
		builder.Append(':');
		builder.AppendInvariant(lineNumber);
		builder.Append(')');
	}

	[UnconditionalSuppressMessage("Trimming - Allow callsite logic", "IL2026")]
	private void AppendExceptionCallSite(StringBuilder builder, LogEventInfo logEvent)
	{
		MethodBase methodBase = logEvent?.Exception?.TargetSite;
		if (methodBase != null)
		{
			if (ClassName)
			{
				string stackFrameMethodClassName = StackTraceUsageUtils.GetStackFrameMethodClassName(methodBase, includeNameSpace: true, CleanNamesOfAsyncContinuations, CleanNamesOfAnonymousDelegates);
				builder.Append(stackFrameMethodClassName);
			}
			if (MethodName)
			{
				if (ClassName)
				{
					builder.Append('.');
				}
				string stackFrameMethodName = StackTraceUsageUtils.GetStackFrameMethodName(methodBase, includeMethodInfo: false, CleanNamesOfAsyncContinuations, CleanNamesOfAnonymousDelegates);
				builder.Append(stackFrameMethodName);
			}
		}
		else if (ClassName || FileName)
		{
			string value = logEvent?.Exception?.Source;
			builder.Append(value);
		}
	}
}

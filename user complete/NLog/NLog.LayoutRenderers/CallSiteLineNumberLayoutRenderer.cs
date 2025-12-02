using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The call site source line number. Full callsite <see cref="T:NLog.LayoutRenderers.CallSiteLayoutRenderer" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Callsite-line-number-layout-renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Callsite-line-number-layout-renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("callsite-linenumber")]
[ThreadAgnostic]
public class CallSiteLineNumberLayoutRenderer : LayoutRenderer, IUsesStackTrace, IRawValue
{
	/// <summary>
	/// Gets or sets the number of frames to skip.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int SkipFrames { get; set; }

	/// <summary>
	/// Logger should capture StackTrace, if it was not provided manually
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool CaptureStackTrace { get; set; } = true;

	/// <inheritdoc />
	StackTraceUsage IUsesStackTrace.StackTraceUsage => StackTraceUsageUtils.GetStackTraceUsage(includeFileName: true, SkipFrames, CaptureStackTrace);

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		int? lineNumber = GetLineNumber(logEvent);
		if (lineNumber.HasValue)
		{
			builder.AppendInvariant(lineNumber.Value);
		}
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		value = GetLineNumber(logEvent);
		return true;
	}

	private int? GetLineNumber(LogEventInfo logEvent)
	{
		if (logEvent.CallSiteInformation == null)
		{
			return null;
		}
		return logEvent.CallSiteInformation.GetCallerLineNumber(SkipFrames);
	}
}

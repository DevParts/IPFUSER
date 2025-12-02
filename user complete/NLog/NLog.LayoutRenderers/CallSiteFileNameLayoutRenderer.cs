using System.IO;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The call site source file name. Full callsite <see cref="T:NLog.LayoutRenderers.CallSiteLayoutRenderer" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Callsite-file-name-layout-renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Callsite-file-name-layout-renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("callsite-filename")]
[ThreadAgnostic]
public class CallSiteFileNameLayoutRenderer : LayoutRenderer, IUsesStackTrace, IStringValueRenderer
{
	/// <summary>
	/// Gets or sets a value indicating whether to include source file path.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IncludeSourcePath { get; set; } = true;

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
		builder.Append(GetStringValue(logEvent));
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		return GetStringValue(logEvent);
	}

	private string GetStringValue(LogEventInfo logEvent)
	{
		if (logEvent.CallSiteInformation != null)
		{
			string callerFilePath = logEvent.CallSiteInformation.GetCallerFilePath(SkipFrames);
			if (!string.IsNullOrEmpty(callerFilePath))
			{
				if (!IncludeSourcePath)
				{
					return Path.GetFileName(callerFilePath);
				}
				return callerFilePath;
			}
		}
		return string.Empty;
	}
}

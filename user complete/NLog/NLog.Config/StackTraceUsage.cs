using System;
using System.ComponentModel;

namespace NLog.Config;

/// <summary>
/// Value indicating how stack trace should be captured when processing the log event.
/// </summary>
[Flags]
public enum StackTraceUsage
{
	/// <summary>
	/// No Stack trace needs to be captured.
	/// </summary>
	None = 0,
	/// <summary>
	/// Stack trace should be captured. This option won't add the filenames and linenumbers
	/// </summary>
	WithStackTrace = 1,
	/// <summary>
	/// Capture also filenames and linenumbers
	/// </summary>
	WithFileNameAndLineNumber = 2,
	/// <summary>
	/// Capture the location of the call
	/// </summary>
	WithCallSite = 4,
	/// <summary>
	/// Capture the class name for location of the call
	/// </summary>
	WithCallSiteClassName = 8,
	/// <summary>
	/// Stack trace should be captured. This option won't add the filenames and linenumbers.
	/// </summary>
	[Obsolete("Replace with `WithStackTrace`. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	WithoutSource = 1,
	/// <summary>
	/// Stack trace should be captured including filenames and linenumbers.
	/// </summary>
	WithSource = 3,
	/// <summary>
	/// Capture maximum amount of the stack trace information supported on the platform.
	/// </summary>
	Max = 3
}

using System;
using System.ComponentModel;

namespace NLog.Config;

/// <summary>
/// Marks the layout or layout renderer as thread safe - it producing correct results
/// regardless of the number of threads it's running on.
///
/// Without this attribute then the target concurrency will be reduced
/// </summary>
[Obsolete("All LayoutRenderer's and Layout's should be ThreadSafe by default. Marked obsolete with NLog 5.0")]
[AttributeUsage(AttributeTargets.Class)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ThreadSafeAttribute : Attribute
{
}

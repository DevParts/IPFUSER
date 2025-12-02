using System;

namespace NLog.Config;

/// <summary>
/// Marks the layout or layout renderer as thread independent - it producing correct results
/// regardless of the thread it's running on.
///
/// Layout or layout-renderer depends on <see cref="P:NLog.LogEventInfo.Properties" /> or <see cref="P:NLog.LogEventInfo.Exception" />,
/// and requires that LogEvent-state is recognized as immutable.
/// </summary>
/// <remarks>
/// Must be used in combination with <see cref="T:NLog.Config.ThreadAgnosticAttribute" />, else it will have no effect
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ThreadAgnosticImmutableAttribute : Attribute
{
}

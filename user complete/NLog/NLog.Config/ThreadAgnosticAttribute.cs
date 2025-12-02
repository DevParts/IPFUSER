using System;

namespace NLog.Config;

/// <summary>
/// Marks the layout or layout renderer as thread independent - it producing correct results
/// regardless of the thread it's running on.
///
/// Without this attribute everything is rendered on the main thread.
/// </summary>
/// <remarks>
/// If this attribute is set on a layout, it could be rendered on the another thread.
/// This could be more efficient as it's skipped when not needed.
///
/// If context like <c>HttpContext.Current</c> is needed, which is only available on the main thread, this attribute should not be applied.
///
/// See the AsyncTargetWrapper and BufferTargetWrapper with the <see cref="M:NLog.Targets.Target.PrecalculateVolatileLayouts(NLog.LogEventInfo)" /> , using <see cref="M:NLog.Layouts.Layout.Precalculate(NLog.LogEventInfo)" />
///
/// Apply this attribute when:
/// - The result can we rendered in another thread. Delaying this could be more efficient. And/Or,
/// - The result should not be precalculated, for example the target sends some extra context information.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ThreadAgnosticAttribute : Attribute
{
}

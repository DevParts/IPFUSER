using System;
using System.ComponentModel;

namespace NLog.Config;

/// <summary>
/// Obsolete and replaced by <see cref="T:NLog.Config.ThreadAgnosticImmutableAttribute" /> with NLog v5.3.
///
/// Marks the layout or layout renderer depends on mutable objects from the LogEvent
///
/// This can be <see cref="P:NLog.LogEventInfo.Properties" /> or <see cref="P:NLog.LogEventInfo.Exception" />
/// </summary>
[Obsolete("Marked obsolete on NLog 5.3, instead use ThreadAgnosticImmutableAttribute")]
[AttributeUsage(AttributeTargets.Class)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class MutableUnsafeAttribute : Attribute
{
}

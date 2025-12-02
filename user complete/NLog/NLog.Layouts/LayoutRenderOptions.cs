using System;
using System.ComponentModel;

namespace NLog.Layouts;

/// <summary>
/// Options available for <see cref="M:NLog.Layouts.Layout.FromMethod(System.Func{NLog.LogEventInfo,System.Object},NLog.Layouts.LayoutRenderOptions)" />
/// </summary>
[Flags]
public enum LayoutRenderOptions
{
	/// <summary>
	/// Default options
	/// </summary>
	None = 0,
	/// <summary>
	/// Layout renderer method can handle concurrent threads
	/// </summary>
	[Obsolete("All LayoutRenderers and Layout should be ThreadSafe by default. Marked obsolete with NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	ThreadSafe = 1,
	/// <summary>
	/// Layout renderer method is agnostic to current thread context. This means it will render the same result independent of thread-context.
	/// </summary>
	ThreadAgnostic = 3
}

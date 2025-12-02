using NLog.Layouts;

namespace NLog;

/// <summary>
/// Extensions for NLog <see cref="T:NLog.Layouts.Layout`1" />.
/// </summary>
public static class LayoutTypedExtensions
{
	/// <summary>
	/// Renders the logevent into a result-value by using the provided layout
	/// </summary>
	/// <remarks>Inside a <see cref="T:NLog.Targets.Target" />, <see cref="M:NLog.Targets.Target.RenderLogEvent(NLog.Layouts.Layout,NLog.LogEventInfo)" /> is preferred for performance reasons.</remarks>
	/// <typeparam name="T"></typeparam>
	/// <param name="layout">The layout.</param>
	/// <param name="logEvent">The logevent info.</param>
	/// <param name="defaultValue">Fallback value when no value available</param>
	/// <returns>Result value when available, else fallback to defaultValue</returns>
	public static T? RenderValue<T>(this Layout<T>? layout, LogEventInfo logEvent, T? defaultValue = default(T?))
	{
		if (layout != null)
		{
			return layout.RenderTypedValue(logEvent, defaultValue);
		}
		return defaultValue;
	}
}

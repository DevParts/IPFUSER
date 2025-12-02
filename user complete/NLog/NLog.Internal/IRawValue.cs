namespace NLog.Internal;

/// <summary>
/// Get the Raw, unformatted value without stringify
/// </summary>
/// <remarks>
/// Implementors must has the  [ThreadAgnostic] attribute
/// </remarks>
internal interface IRawValue
{
	/// <summary>
	/// Get the raw value
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="value">The value</param>
	/// <returns>RawValue supported?</returns>
	bool TryGetRawValue(LogEventInfo logEvent, out object? value);
}

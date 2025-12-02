namespace NLog.Internal;

/// <summary>
/// Supports rendering as string value with limited or no allocations (preferred)
/// </summary>
/// <remarks>
/// Implementors must not have the [AppDomainFixedOutput] attribute
/// </remarks>
internal interface IStringValueRenderer
{
	/// <summary>
	/// Renders the value of layout renderer in the context of the specified log event
	/// </summary>
	/// <param name="logEvent"></param>
	/// <returns>null if not possible or unknown</returns>
	string? GetFormattedString(LogEventInfo logEvent);
}

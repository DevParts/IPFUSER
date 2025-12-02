namespace NLog.LayoutRenderers;

/// <summary>
/// Format of the ${level} layout renderer output.
/// </summary>
public enum LevelFormat
{
	/// <summary>
	/// Render the LogLevel standard name.
	/// </summary>
	Name = 0,
	/// <summary>
	/// Render the first character of the level.
	/// </summary>
	FirstCharacter = 1,
	/// <summary>
	/// Render the first character of the level.
	/// </summary>
	OneLetter = 1,
	/// <summary>
	/// Render the ordinal (aka number) for the level.
	/// </summary>
	Ordinal = 2,
	/// <summary>
	/// Render the LogLevel full name, expanding Warn / Info abbreviations
	/// </summary>
	FullName = 3,
	/// <summary>
	/// Render the LogLevel as 3 letter abbreviations (Trc, Dbg, Inf, Wrn, Err, Ftl)
	/// </summary>
	TriLetter = 4
}

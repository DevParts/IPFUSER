namespace NLog.Targets;

/// <summary>
/// Colored console output color.
/// </summary>
/// <remarks>
/// Note that this enumeration is defined to be binary compatible with
/// .NET 2.0 System.ConsoleColor + some additions
/// </remarks>
public enum ConsoleOutputColor
{
	/// <summary>
	/// Black Color (#000000).
	/// </summary>
	Black,
	/// <summary>
	/// Dark blue Color (#000080).
	/// </summary>
	DarkBlue,
	/// <summary>
	/// Dark green Color (#008000).
	/// </summary>
	DarkGreen,
	/// <summary>
	/// Dark Cyan Color (#008080).
	/// </summary>
	DarkCyan,
	/// <summary>
	/// Dark Red Color (#800000).
	/// </summary>
	DarkRed,
	/// <summary>
	/// Dark Magenta Color (#800080).
	/// </summary>
	DarkMagenta,
	/// <summary>
	/// Dark Yellow Color (#808000).
	/// </summary>
	DarkYellow,
	/// <summary>
	/// Gray Color (#C0C0C0).
	/// </summary>
	Gray,
	/// <summary>
	/// Dark Gray Color (#808080).
	/// </summary>
	DarkGray,
	/// <summary>
	/// Blue Color (#0000FF).
	/// </summary>
	Blue,
	/// <summary>
	/// Green Color (#00FF00).
	/// </summary>
	Green,
	/// <summary>
	/// Cyan Color (#00FFFF).
	/// </summary>
	Cyan,
	/// <summary>
	/// Red Color (#FF0000).
	/// </summary>
	Red,
	/// <summary>
	/// Magenta Color (#FF00FF).
	/// </summary>
	Magenta,
	/// <summary>
	/// Yellow Color (#FFFF00).
	/// </summary>
	Yellow,
	/// <summary>
	/// White Color (#FFFFFF).
	/// </summary>
	White,
	/// <summary>
	/// Don't change the color.
	/// </summary>
	NoChange
}

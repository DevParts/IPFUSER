namespace NLog.Internal;

/// <summary>
/// Supported operating systems.
/// </summary>
/// <remarks>
/// If you add anything here, make sure to add the appropriate detection
/// code to <see cref="T:NLog.Internal.PlatformDetector" />
/// </remarks>
internal enum RuntimeOS
{
	/// <summary>
	/// Unknown operating system.
	/// </summary>
	Unknown,
	/// <summary>
	/// Unix/Linux operating systems.
	/// </summary>
	Linux,
	/// <summary>
	/// Desktop versions of Windows (95,98,ME).
	/// </summary>
	Windows9x,
	/// <summary>
	/// Windows NT, 2000, 2003 and future versions based on NT technology.
	/// </summary>
	WindowsNT,
	/// <summary>
	/// Macintosh Mac OSX
	/// </summary>
	MacOSX
}

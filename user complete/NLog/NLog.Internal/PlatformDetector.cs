using System;

namespace NLog.Internal;

/// <summary>
/// Detects the platform the NLog is running on.
/// </summary>
internal static class PlatformDetector
{
	private static RuntimeOS? _currentOS;

	private static bool? _isMono;

	/// <summary>
	/// Gets the current runtime OS.
	/// </summary>
	public static RuntimeOS CurrentOS
	{
		get
		{
			RuntimeOS? currentOS = _currentOS;
			if (!currentOS.HasValue)
			{
				RuntimeOS? runtimeOS = (_currentOS = GetCurrentRuntimeOS());
				return runtimeOS.Value;
			}
			return currentOS.GetValueOrDefault();
		}
	}

	/// <summary>
	/// Gets a value indicating whether current OS is Win32-based (desktop or mobile).
	/// </summary>
	public static bool IsWin32
	{
		get
		{
			if (CurrentOS != RuntimeOS.WindowsNT)
			{
				return CurrentOS == RuntimeOS.Windows9x;
			}
			return true;
		}
	}

	/// <summary>
	/// Gets a value indicating whether current OS is Unix-based.
	/// </summary>
	public static bool IsUnix
	{
		get
		{
			if (CurrentOS != RuntimeOS.Linux)
			{
				return CurrentOS == RuntimeOS.MacOSX;
			}
			return true;
		}
	}

	/// <summary>
	/// Gets a value indicating whether current runtime is Mono-based
	/// </summary>
	public static bool IsMono
	{
		get
		{
			bool? isMono = _isMono;
			if (!isMono.HasValue)
			{
				bool? flag = (_isMono = Type.GetType("Mono.Runtime") != null);
				return flag.Value;
			}
			return isMono == true;
		}
	}

	private static RuntimeOS GetCurrentRuntimeOS()
	{
		switch (Environment.OSVersion.Platform)
		{
		case PlatformID.Unix:
		case (PlatformID)128:
			return RuntimeOS.Linux;
		case PlatformID.Win32Windows:
			return RuntimeOS.Windows9x;
		case PlatformID.Win32NT:
			return RuntimeOS.WindowsNT;
		default:
			return RuntimeOS.Unknown;
		}
	}
}

using System;
using System.IO;

namespace NLog.Internal;

internal static class FileInfoHelper
{
	internal const int MinReliableBirthYear = 1980;

	public static DateTime LookupValidFileCreationTimeUtc(this FileInfo fileInfo)
	{
		return LookupValidFileCreationTimeUtc(fileInfo, (FileInfo f) => f.CreationTimeUtc, (FileInfo f) => f.LastWriteTimeUtc);
	}

	private static DateTime LookupValidFileCreationTimeUtc<T>(T fileInfo, Func<T, DateTime> primary, Func<T, DateTime> fallback, Func<T, DateTime>? finalFallback = null)
	{
		DateTime result = primary(fileInfo);
		if (result.Year < 1980 && !PlatformDetector.IsWin32)
		{
			result = fallback(fileInfo);
			if (finalFallback != null && result.Year < 1980)
			{
				result = finalFallback(fileInfo);
			}
		}
		return result;
	}

	public static bool IsRelativeFilePath(string filepath)
	{
		filepath = filepath?.TrimStart(ArrayHelper.Empty<char>()) ?? string.Empty;
		if (string.IsNullOrEmpty(filepath))
		{
			return false;
		}
		char c = filepath[0];
		if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
		{
			return false;
		}
		if (c == '.')
		{
			return true;
		}
		if (filepath.Length >= 2 && filepath[1] == Path.VolumeSeparatorChar && Path.VolumeSeparatorChar != Path.DirectorySeparatorChar)
		{
			return false;
		}
		return true;
	}
}

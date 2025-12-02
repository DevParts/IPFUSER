using System;
using System.IO;

namespace NLog.Internal;

internal static class PathHelpers
{
	/// <summary>
	/// Cached directory separator char array to avoid memory allocation on each method call.
	/// </summary>
	private static readonly char[] DirectorySeparatorChars = new char[2]
	{
		Path.DirectorySeparatorChar,
		Path.AltDirectorySeparatorChar
	};

	/// <summary>
	/// Combine paths
	/// </summary>
	/// <param name="path">basepath, not null</param>
	/// <param name="dir">optional dir</param>
	/// <param name="file">optional file</param>
	/// <returns></returns>
	internal static string CombinePaths(string path, string dir, string file)
	{
		if (!string.IsNullOrEmpty(dir))
		{
			path = Path.Combine(path, dir);
		}
		if (!string.IsNullOrEmpty(file))
		{
			path = Path.Combine(path, file);
		}
		return path;
	}

	/// <summary>
	/// Trims directory separators from the path
	/// </summary>
	/// <param name="path">path, could be null</param>
	/// <returns>never null</returns>
	public static string TrimDirectorySeparators(string path)
	{
		string text = path?.TrimEnd(DirectorySeparatorChars) ?? string.Empty;
		if (text.EndsWith(":", StringComparison.Ordinal))
		{
			return path ?? string.Empty;
		}
		return text;
	}

	public static bool IsTempDir(string directory, string tempDir)
	{
		tempDir = TrimDirectorySeparators(tempDir);
		if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(tempDir))
		{
			return false;
		}
		string fullPath = Path.GetFullPath(directory);
		if (string.IsNullOrEmpty(fullPath))
		{
			return false;
		}
		if (fullPath.StartsWith(tempDir, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (tempDir.StartsWith("/tmp", StringComparison.Ordinal) && directory.StartsWith("/var/tmp/", StringComparison.Ordinal))
		{
			return true;
		}
		return false;
	}
}

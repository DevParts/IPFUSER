using System;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo.SqlEnum;

namespace Microsoft.SqlServer.Management.Smo;

public static class PathWrapper
{
	public static string PathSeparatorFromServerConnection(ServerConnection serverConnection)
	{
		if (!(serverConnection.HostPlatform == "Linux"))
		{
			return "\\";
		}
		return "/";
	}

	public static string Combine(string path1, string path2)
	{
		return Combine(path1, path2, PathType.Unspecified);
	}

	public static string Combine(string path1, string path2, PathType pathType)
	{
		if (path1 == null)
		{
			throw new ArgumentNullException("path1");
		}
		if (path2 == null)
		{
			throw new ArgumentNullException("path2");
		}
		if (pathType == PathType.Unspecified)
		{
			pathType = (path1.StartsWith("/") ? PathType.Linux : PathType.Windows);
		}
		if (Uri.TryCreate(path1, UriKind.Absolute, out var result) && result.IsUriSchemeHttps())
		{
			return path1 + "/" + path2;
		}
		if (pathType == PathType.Windows)
		{
			return Path.Combine(path1, path2);
		}
		if (path1.Trim().Length == 0 || path2.StartsWith("/"))
		{
			return path2;
		}
		return $"{path1.TrimEnd('/')}/{path2}";
	}

	public static string GetDirectoryName(string s1)
	{
		return GetDirectoryName(s1, PathType.Unspecified);
	}

	public static string GetDirectoryName(string s1, PathType pathType)
	{
		if (pathType == PathType.Unspecified)
		{
			pathType = (s1.StartsWith("/") ? PathType.Linux : PathType.Windows);
		}
		if (Uri.TryCreate(s1, UriKind.Absolute, out var result) && result.IsUriSchemeHttps())
		{
			int length = s1.LastIndexOf("/", StringComparison.Ordinal);
			return s1.Substring(0, length);
		}
		if (pathType == PathType.Windows)
		{
			return Path.GetDirectoryName(s1);
		}
		if (s1 == null || s1.Trim().Length == 0)
		{
			throw new ArgumentNullException("s1");
		}
		if (s1 == "/")
		{
			return null;
		}
		int num = s1.LastIndexOf("/", StringComparison.Ordinal);
		if (num < 0)
		{
			return string.Empty;
		}
		if (num == s1.Length - 1)
		{
			return s1.TrimEnd('/');
		}
		if (num == 0)
		{
			return "/";
		}
		return s1.Substring(0, num);
	}

	public static bool IsXIPath(string s1)
	{
		if (Uri.TryCreate(s1, UriKind.Absolute, out var result) && result.IsUriSchemeHttps())
		{
			return true;
		}
		return false;
	}

	public static bool IsRooted(string path)
	{
		return IsRooted(path, PathType.Unspecified);
	}

	public static bool IsRooted(string path, PathType pathType)
	{
		if (pathType == PathType.Unspecified)
		{
			pathType = (path.StartsWith("/") ? PathType.Linux : PathType.Windows);
		}
		if (pathType != PathType.Windows)
		{
			return path.StartsWith("/");
		}
		return Path.IsPathRooted(path);
	}
}

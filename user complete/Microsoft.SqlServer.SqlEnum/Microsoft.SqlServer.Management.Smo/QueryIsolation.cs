using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Microsoft.SqlServer.Management.Smo;

internal static class QueryIsolation
{
	internal const string RegPathFormat = "Software\\Microsoft\\Microsoft SQL Server\\SMO\\QueryIsolation\\{0}";

	private const string PrefixValue = "Prefix";

	private const string PostfixValue = "Postfix";

	private const string IsolationFormat = "SET TRANSACTION ISOLATION LEVEL {0};";

	private static string[] IsolationLevels;

	internal static string cachedPrefix;

	internal static string cachedPostfix;

	private static string RegKeyName => Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName);

	static QueryIsolation()
	{
		IsolationLevels = new string[5] { "read uncommitted", "read committed", "serializable", "snapshot", "repeatable read" };
		cachedPrefix = null;
		cachedPostfix = null;
		InitIfNeeded();
	}

	private static void InitIfNeeded()
	{
		if (cachedPrefix == null)
		{
			cachedPrefix = GetIsolationLevel("Prefix");
			cachedPostfix = GetIsolationLevel("Postfix");
		}
	}

	private static string GetIsolationLevel(string regValue)
	{
		string text = string.Empty;
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey($"Software\\Microsoft\\Microsoft SQL Server\\SMO\\QueryIsolation\\{RegKeyName}", writable: false);
			text = ((registryKey != null) ? registryKey.GetValue(regValue, string.Empty).ToString() : string.Empty);
		}
		catch (Exception)
		{
		}
		if (!IsolationLevels.Contains(text.ToLowerInvariant()))
		{
			return string.Empty;
		}
		return $"SET TRANSACTION ISOLATION LEVEL {text};";
	}

	public static string GetQueryPrefix()
	{
		InitIfNeeded();
		if (string.IsNullOrEmpty(cachedPrefix) || string.IsNullOrEmpty(cachedPostfix))
		{
			return string.Empty;
		}
		return cachedPrefix;
	}

	public static string GetQueryPostfix()
	{
		InitIfNeeded();
		if (string.IsNullOrEmpty(cachedPostfix) || string.IsNullOrEmpty(cachedPrefix))
		{
			return string.Empty;
		}
		return cachedPostfix;
	}
}

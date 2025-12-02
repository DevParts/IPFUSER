using System;
using System.Security;

namespace NLog.Internal;

/// <summary>
/// Safe way to get environment variables.
/// </summary>
internal static class EnvironmentHelper
{
	internal static string GetMachineName()
	{
		try
		{
			return Environment.MachineName;
		}
		catch (SecurityException)
		{
			return string.Empty;
		}
	}

	internal static string GetSafeEnvironmentVariable(string name)
	{
		try
		{
			return Environment.GetEnvironmentVariable(name) ?? string.Empty;
		}
		catch (SecurityException)
		{
			return string.Empty;
		}
	}
}

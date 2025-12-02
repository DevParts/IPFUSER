using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Security;
using NLog.Common;

namespace NLog.Internal;

/// <summary>
/// Helpers for <see cref="T:System.Reflection.Assembly" />.
/// </summary>
internal static class AssemblyHelpers
{
	[UnconditionalSuppressMessage("Returns empty string for assemblies embedded in a single-file app", "IL3000")]
	public static string GetAssemblyFileLocation(Assembly assembly)
	{
		if ((object)assembly == null)
		{
			return string.Empty;
		}
		string fullName = assembly.FullName;
		try
		{
			if (!Uri.TryCreate(assembly.CodeBase, UriKind.RelativeOrAbsolute, out var result))
			{
				InternalLogger.Debug("Ignoring assembly location because code base is unknown: '{0}' ({1})", assembly.CodeBase, fullName);
				return string.Empty;
			}
			string directoryName = Path.GetDirectoryName(result.LocalPath);
			if (string.IsNullOrEmpty(directoryName))
			{
				InternalLogger.Debug("Ignoring assembly location because it is not a valid directory: '{0}' ({1})", result.LocalPath, fullName);
				return string.Empty;
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
			if (!directoryInfo.Exists)
			{
				InternalLogger.Debug("Ignoring assembly location because directory doesn't exists: '{0}' ({1})", directoryName, fullName);
				return string.Empty;
			}
			InternalLogger.Debug("Found assembly location directory: '{0}' ({1})", directoryInfo.FullName, fullName);
			string path = (string.IsNullOrEmpty(assembly.Location) ? Path.GetFileName(result.LocalPath) : Path.GetFileName(assembly.Location));
			return Path.Combine(directoryInfo.FullName, path);
		}
		catch (PlatformNotSupportedException ex)
		{
			InternalLogger.Warn(ex, "Ignoring assembly location because assembly lookup is not supported: {0}", fullName);
			if (ex.MustBeRethrown())
			{
				throw;
			}
			return string.Empty;
		}
		catch (SecurityException ex2)
		{
			InternalLogger.Warn(ex2, "Ignoring assembly location because assembly lookup is not allowed: {0}", fullName);
			if (ex2.MustBeRethrown())
			{
				throw;
			}
			return string.Empty;
		}
		catch (UnauthorizedAccessException ex3)
		{
			InternalLogger.Warn(ex3, "Ignoring assembly location because assembly lookup is not allowed: {0}", fullName);
			if (ex3.MustBeRethrown())
			{
				throw;
			}
			return string.Empty;
		}
	}

	/// <summary>
	/// Logs the assembly version and file version of the given Assembly.
	/// </summary>
	/// <param name="assembly">The assembly to log.</param>
	public static void LogAssemblyVersion(Assembly assembly)
	{
		try
		{
			if (InternalLogger.IsInfoEnabled)
			{
				string text = assembly.GetFirstCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
				string text2 = assembly.GetFirstCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
				bool flag = false;
				if (assembly.GlobalAssemblyCache)
				{
					flag = true;
				}
				InternalLogger.Info("{0}. File version: {1}. Product version: {2}. GlobalAssemblyCache: {3}", assembly.FullName, text, text2, flag);
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "Error logging version of assembly {0}.", assembly?.FullName);
		}
	}
}

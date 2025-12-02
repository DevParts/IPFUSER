using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using NLog.Common;

namespace NLog.Internal.Fakeables;

internal sealed class AppEnvironmentWrapper : IAppEnvironment, IFileSystem
{
	private const string LongUNCPrefix = "\\\\?\\UNC\\";

	private const string UnknownProcessName = "[unknown]";

	private string? _entryAssemblyLocation;

	private string? _entryAssemblyFileName;

	private string? _currentProcessFilePath;

	private string? _currentProcessBaseName;

	private string? _appDomainBaseDirectory;

	private string? _appDomainConfigFile;

	private string? _appDomainFriendlyName;

	private IEnumerable<string>? _appDomainPrivateBinPath;

	private int? _appDomainId;

	private int? _currentProcessId;

	/// <inheritdoc />
	public string EntryAssemblyLocation => _entryAssemblyLocation ?? (_entryAssemblyLocation = LookupEntryAssemblyLocation());

	/// <inheritdoc />
	public string EntryAssemblyFileName => _entryAssemblyFileName ?? (_entryAssemblyFileName = LookupEntryAssemblyFileName());

	/// <inheritdoc />
	public string CurrentProcessFilePath => _currentProcessFilePath ?? (_currentProcessFilePath = LookupCurrentProcessFilePathWithFallback());

	/// <inheritdoc />
	public string CurrentProcessBaseName => _currentProcessBaseName ?? (_currentProcessBaseName = LookupCurrentProcessNameWithFallback());

	/// <inheritdoc />
	public int CurrentProcessId
	{
		get
		{
			int? currentProcessId = _currentProcessId;
			if (!currentProcessId.HasValue)
			{
				int? num = (_currentProcessId = LookupCurrentProcessIdWithFallback());
				return num.Value;
			}
			return currentProcessId.GetValueOrDefault();
		}
	}

	/// <inheritdoc />
	public string AppDomainBaseDirectory => _appDomainBaseDirectory ?? (_appDomainBaseDirectory = LookupAppDomainBaseDirectory());

	/// <inheritdoc />
	public string AppDomainConfigurationFile => _appDomainConfigFile ?? (_appDomainConfigFile = LookupAppDomainConfigFileSafe());

	/// <inheritdoc />
	public string AppDomainFriendlyName
	{
		get
		{
			string text = _appDomainFriendlyName;
			if (text == null)
			{
				string obj = LookupAppDomainFriendlyName() ?? CurrentProcessBaseName;
				string text2 = obj;
				_appDomainFriendlyName = obj;
				text = text2;
			}
			return text;
		}
	}

	/// <inheritdoc />
	public int AppDomainId
	{
		get
		{
			int? appDomainId = _appDomainId;
			if (!appDomainId.HasValue)
			{
				int? num = (_appDomainId = AppDomain.CurrentDomain.Id);
				return num.Value;
			}
			return appDomainId.GetValueOrDefault();
		}
	}

	/// <inheritdoc />
	public IEnumerable<string> AppDomainPrivateBinPath => _appDomainPrivateBinPath ?? (_appDomainPrivateBinPath = LookupAppDomainPrivateBinPathSafe());

	/// <inheritdoc />
	public string UserTempFilePath => Path.GetTempPath();

	/// <inheritdoc />
	public event EventHandler ProcessExit
	{
		add
		{
			AppDomain.CurrentDomain.ProcessExit += value;
			AppDomain.CurrentDomain.DomainUnload += value;
		}
		remove
		{
			AppDomain.CurrentDomain.DomainUnload -= value;
			AppDomain.CurrentDomain.ProcessExit -= value;
		}
	}

	/// <inheritdoc />
	public IEnumerable<Assembly> GetAppDomainRuntimeAssemblies()
	{
		return AppDomain.CurrentDomain?.GetAssemblies() ?? ArrayHelper.Empty<Assembly>();
	}

	/// <inheritdoc />
	public bool FileExists(string path)
	{
		return File.Exists(path);
	}

	public TextReader LoadTextFile(string path)
	{
		return new StreamReader(path);
	}

	/// <summary>
	/// Long UNC paths does not allow relative-path-logic using '..', and also cannot be loaded into Uri by XmlReader
	/// </summary>
	internal static string FixFilePathWithLongUNC(string filepath)
	{
		if (!string.IsNullOrEmpty(filepath) && filepath.StartsWith("\\\\?\\UNC\\", StringComparison.Ordinal) && filepath.Length < 260)
		{
			filepath = "\\\\" + filepath.Substring("\\\\?\\UNC\\".Length);
		}
		return filepath;
	}

	private static string LookupAppDomainBaseDirectory()
	{
		try
		{
			return AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug(ex, "AppDomain.BaseDirectory Failed");
			return string.Empty;
		}
	}

	private static string[] LookupAppDomainPrivateBinPathSafe()
	{
		try
		{
			return LookupAppDomainPrivateBinPath();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug(ex, "AppDomain.CurrentDomain.SetupInformation.PrivateBinPath Failed");
			return ArrayHelper.Empty<string>();
		}
	}

	private static string[] LookupAppDomainPrivateBinPath()
	{
		try
		{
			string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
			return string.IsNullOrEmpty(privateBinPath) ? ArrayHelper.Empty<string>() : privateBinPath.SplitAndTrimTokens(';');
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug(ex, "AppDomain.CurrentDomain.SetupInformation.PrivateBinPath Failed");
			return ArrayHelper.Empty<string>();
		}
	}

	private static string LookupAppDomainConfigFileSafe()
	{
		try
		{
			return LookupAppDomainConfigFile();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug(ex, "AppDomain.SetupInformation.ConfigurationFile Failed");
			return string.Empty;
		}
	}

	private static string LookupAppDomainConfigFile()
	{
		try
		{
			return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile ?? string.Empty;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug(ex, "AppDomain.SetupInformation.ConfigurationFile Failed");
			return string.Empty;
		}
	}

	private static string LookupAppDomainFriendlyName()
	{
		try
		{
			string friendlyName = AppDomain.CurrentDomain.FriendlyName;
			return (!string.IsNullOrEmpty(friendlyName)) ? friendlyName : LookupEntryAssemblyFriendlyName();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug(ex, "AppDomain.FriendlyName Failed");
			return LookupEntryAssemblyFriendlyName();
		}
	}

	private static string LookupEntryAssemblyFriendlyName()
	{
		try
		{
			return Assembly.GetEntryAssembly()?.GetName()?.Name ?? "[unknown]";
		}
		catch
		{
			return "[unknown]";
		}
	}

	private static string LookupEntryAssemblyLocation()
	{
		string assemblyFileLocation = AssemblyHelpers.GetAssemblyFileLocation(Assembly.GetEntryAssembly());
		if (!string.IsNullOrEmpty(assemblyFileLocation))
		{
			return Path.GetDirectoryName(assemblyFileLocation);
		}
		return string.Empty;
	}

	private static string LookupEntryAssemblyFileName()
	{
		try
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			string assemblyFileLocation = AssemblyHelpers.GetAssemblyFileLocation(entryAssembly);
			if (!string.IsNullOrEmpty(assemblyFileLocation))
			{
				return Path.GetFileName(assemblyFileLocation);
			}
			string text = entryAssembly?.GetName()?.Name;
			if (!string.IsNullOrEmpty(text))
			{
				return text + ".dll";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupEntryAssemblyFileName Failed - {0}", ex.Message);
			return string.Empty;
		}
	}

	private static string LookupCurrentProcessFilePathWithFallback()
	{
		try
		{
			return LookupCurrentProcessFilePath() ?? LookupCurrentProcessFilePathNative();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessFilePath Failed - {0}", ex.Message);
			return LookupCurrentProcessFilePathNative();
		}
	}

	private static string? LookupCurrentProcessFilePath()
	{
		try
		{
			string text = Process.GetCurrentProcess()?.MainModule?.FileName;
			return (!string.IsNullOrEmpty(text)) ? text : null;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessFilePath Managed Failed - {0}", ex.Message);
			return null;
		}
	}

	private static int LookupCurrentProcessIdWithFallback()
	{
		try
		{
			return LookupCurrentProcessId() ?? LookupCurrentProcessIdNative();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessId Failed - {0}", ex.Message);
			return LookupCurrentProcessIdNative();
		}
	}

	private static int? LookupCurrentProcessId()
	{
		try
		{
			return Process.GetCurrentProcess()?.Id;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessId Managed Failed - {0}", ex.Message);
			return null;
		}
	}

	private static string LookupCurrentProcessNameWithFallback()
	{
		try
		{
			return LookupCurrentProcessName() ?? LookupCurrentProcessNameNative();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessName Failed - {0}", ex.Message);
			return LookupCurrentProcessNameNative();
		}
	}

	private static string? LookupCurrentProcessName()
	{
		try
		{
			string text = Process.GetCurrentProcess()?.ProcessName;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessName Managed Failed - {0}", ex.Message);
		}
		return null;
	}

	private static string LookupCurrentProcessNameNative()
	{
		string text = LookupCurrentProcessFilePath();
		if (!string.IsNullOrEmpty(text))
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
			if (!string.IsNullOrEmpty(fileNameWithoutExtension))
			{
				return fileNameWithoutExtension;
			}
		}
		string text2 = LookupEntryAssemblyFileName();
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = Path.GetFileNameWithoutExtension(text2);
			if (!string.IsNullOrEmpty(text2))
			{
				return text2;
			}
		}
		return "[unknown]";
	}

	private static string LookupCurrentProcessFilePathNative()
	{
		try
		{
			if (!PlatformDetector.IsWin32)
			{
				return string.Empty;
			}
			return LookupCurrentProcessFilePathWin32();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessFilePath Native Failed - {0}", ex.Message);
			return string.Empty;
		}
	}

	[SecuritySafeCritical]
	private static string LookupCurrentProcessFilePathWin32()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (NativeMethods.GetModuleFileName(IntPtr.Zero, stringBuilder, stringBuilder.Capacity) == 0)
			{
				throw new InvalidOperationException("Cannot determine program name.");
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessFilePath Win32 Failed - {0}", ex.Message);
			return string.Empty;
		}
	}

	private static int LookupCurrentProcessIdNative()
	{
		try
		{
			if (!PlatformDetector.IsWin32)
			{
				return 0;
			}
			return LookupCurrentProcessIdWin32();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessId Native Failed - {0}", ex.Message);
			return 0;
		}
	}

	[SecuritySafeCritical]
	private static int LookupCurrentProcessIdWin32()
	{
		try
		{
			return NativeMethods.GetCurrentProcessId();
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Debug("LookupCurrentProcessId Win32 Failed - {0}", ex.Message);
			return 0;
		}
	}
}

using System;
using System.ComponentModel;
using NLog.Internal;
using NLog.Targets;

namespace NLog.Config;

/// <summary>
/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
///
/// Provides simple programmatic configuration API used for trivial logging cases.
///
/// Warning, these methods will overwrite the current config.
/// </summary>
[Obsolete("Use LogManager.Setup().LoadConfiguration() instead. Marked obsolete on NLog 5.2")]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SimpleConfigurator
{
	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupLoadConfigurationExtensions.WriteToConsole(NLog.Config.ISetupConfigurationTargetBuilder,NLog.Layouts.Layout,System.Text.Encoding,System.Boolean,System.Boolean,System.Boolean)" /> with NLog v5.2.
	///
	/// Configures NLog for console logging so that all messages above and including
	/// the <see cref="F:NLog.LogLevel.Info" /> level are output to the console.
	/// </summary>
	[Obsolete("Use LogManager.Setup().LoadConfiguration(c => c.ForLogger().WriteToConsole()) instead. Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConfigureForConsoleLogging()
	{
		ConfigureForConsoleLogging(LogLevel.Info);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupLoadConfigurationExtensions.WriteToConsole(NLog.Config.ISetupConfigurationTargetBuilder,NLog.Layouts.Layout,System.Text.Encoding,System.Boolean,System.Boolean,System.Boolean)" /> with NLog v5.2.
	///
	/// Configures NLog for console logging so that all messages above and including
	/// the specified level are output to the console.
	/// </summary>
	/// <param name="minLevel">The minimal logging level.</param>
	[Obsolete("Use LogManager.Setup().LoadConfiguration(c => c.ForLogger(minLevel).WriteToConsole()) instead. Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConfigureForConsoleLogging(LogLevel minLevel)
	{
		ConsoleTarget target = new ConsoleTarget();
		LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
		loggingConfiguration.AddRule(minLevel, LogLevel.MaxLevel, target);
		LogManager.Configuration = loggingConfiguration;
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Configures NLog for to log to the specified target so that all messages
	/// above and including the <see cref="F:NLog.LogLevel.Info" /> level are output.
	/// </summary>
	/// <param name="target">The target to log all messages to.</param>
	[Obsolete("Use LogManager.Setup().LoadConfiguration(c => c.ForLogger().WriteTo(target)) instead. Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConfigureForTargetLogging(Target target)
	{
		Guard.ThrowIfNull(target, "target");
		ConfigureForTargetLogging(target, LogLevel.Info);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Configures NLog for to log to the specified target so that all messages
	/// above and including the specified level are output.
	/// </summary>
	/// <param name="target">The target to log all messages to.</param>
	/// <param name="minLevel">The minimal logging level.</param>
	[Obsolete("Use LogManager.Setup().LoadConfiguration(c => c.ForLogger(minLevel).WriteTo(target)) instead. Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConfigureForTargetLogging(Target target, LogLevel minLevel)
	{
		Guard.ThrowIfNull(target, "target");
		LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
		loggingConfiguration.AddRule(minLevel, LogLevel.MaxLevel, target);
		LogManager.Configuration = loggingConfiguration;
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupLoadConfigurationExtensions.WriteToFile(NLog.Config.ISetupConfigurationTargetBuilder,NLog.Layouts.Layout,NLog.Layouts.Layout,System.Text.Encoding,NLog.Targets.LineEndingMode,System.Boolean,System.Int64,System.Int32,System.Int32)" /> with NLog v5.2.
	///
	/// Configures NLog for file logging so that all messages above and including
	/// the <see cref="F:NLog.LogLevel.Info" /> level are written to the specified file.
	/// </summary>
	/// <param name="fileName">Log file name.</param>
	[Obsolete("Use LogManager.Setup().LoadConfiguration(c => c.ForLogger().WriteToFile(fileName)) instead. Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConfigureForFileLogging(string fileName)
	{
		ConfigureForFileLogging(fileName, LogLevel.Info);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupLoadConfigurationExtensions.WriteToFile(NLog.Config.ISetupConfigurationTargetBuilder,NLog.Layouts.Layout,NLog.Layouts.Layout,System.Text.Encoding,NLog.Targets.LineEndingMode,System.Boolean,System.Int64,System.Int32,System.Int32)" /> with NLog v5.2.
	///
	/// Configures NLog for file logging so that all messages above and including
	/// the specified level are written to the specified file.
	/// </summary>
	/// <param name="fileName">Log file name.</param>
	/// <param name="minLevel">The minimal logging level.</param>
	[Obsolete("Use LogManager.Setup().LoadConfiguration(c => c.ForLogger(minLevel).WriteToFile(fileName)) instead. Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ConfigureForFileLogging(string fileName, LogLevel minLevel)
	{
		ConfigureForTargetLogging(new FileTarget
		{
			FileName = fileName
		}, minLevel);
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace NLog.Config;

/// <summary>
/// Provides context for install/uninstall operations.
/// </summary>
public sealed class InstallationContext : IDisposable
{
	/// <summary>
	/// Mapping between log levels and console output colors.
	/// </summary>
	private static readonly Dictionary<LogLevel, ConsoleColor> LogLevel2ConsoleColor = new Dictionary<LogLevel, ConsoleColor>
	{
		{
			NLog.LogLevel.Trace,
			ConsoleColor.DarkGray
		},
		{
			NLog.LogLevel.Debug,
			ConsoleColor.Gray
		},
		{
			NLog.LogLevel.Info,
			ConsoleColor.White
		},
		{
			NLog.LogLevel.Warn,
			ConsoleColor.Yellow
		},
		{
			NLog.LogLevel.Error,
			ConsoleColor.Red
		},
		{
			NLog.LogLevel.Fatal,
			ConsoleColor.DarkRed
		}
	};

	/// <summary>
	/// Gets or sets the installation log level.
	/// </summary>
	public LogLevel LogLevel { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to ignore failures during installation.
	/// </summary>
	public bool IgnoreFailures { get; set; }

	/// <summary>
	/// Whether installation exceptions should be rethrown. If IgnoreFailures is set to true,
	/// this property has no effect (there are no exceptions to rethrow).
	/// </summary>
	public bool ThrowExceptions { get; set; }

	/// <summary>
	/// Gets the installation parameters.
	/// </summary>
	public IDictionary<string, string> Parameters { get; }

	/// <summary>
	/// Gets or sets the log output.
	/// </summary>
	public TextWriter? LogOutput { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.InstallationContext" /> class.
	/// </summary>
	public InstallationContext()
		: this(TextWriter.Null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.InstallationContext" /> class.
	/// </summary>
	/// <param name="logOutput">The log output.</param>
	public InstallationContext(TextWriter logOutput)
	{
		LogOutput = logOutput;
		Parameters = new Dictionary<string, string>();
		LogLevel = NLog.LogLevel.Info;
		ThrowExceptions = false;
	}

	/// <summary>
	/// Logs the specified trace message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="arguments">The arguments.</param>
	public void Trace([Localizable(false)] string message, params object[] arguments)
	{
		Log(NLog.LogLevel.Trace, message, arguments);
	}

	/// <summary>
	/// Logs the specified debug message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="arguments">The arguments.</param>
	public void Debug([Localizable(false)] string message, params object[] arguments)
	{
		Log(NLog.LogLevel.Debug, message, arguments);
	}

	/// <summary>
	/// Logs the specified informational message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="arguments">The arguments.</param>
	public void Info([Localizable(false)] string message, params object[] arguments)
	{
		Log(NLog.LogLevel.Info, message, arguments);
	}

	/// <summary>
	/// Logs the specified warning message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="arguments">The arguments.</param>
	public void Warning([Localizable(false)] string message, params object[] arguments)
	{
		Log(NLog.LogLevel.Warn, message, arguments);
	}

	/// <summary>
	/// Logs the specified error message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="arguments">The arguments.</param>
	public void Error([Localizable(false)] string message, params object[] arguments)
	{
		Log(NLog.LogLevel.Error, message, arguments);
	}

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	public void Dispose()
	{
		LogOutput?.Close();
		LogOutput = null;
	}

	/// <summary>
	/// Creates the log event which can be used to render layouts during install/uninstall.
	/// </summary>
	/// <returns>Log event info object.</returns>
	public LogEventInfo CreateLogEvent()
	{
		LogEventInfo logEventInfo = LogEventInfo.CreateNullEvent();
		foreach (KeyValuePair<string, string> parameter in Parameters)
		{
			logEventInfo.Properties.Add(parameter.Key, parameter.Value);
		}
		return logEventInfo;
	}

	private void Log(LogLevel logLevel, string message, object[] arguments)
	{
		if (logLevel >= LogLevel)
		{
			if (arguments != null && arguments.Length != 0)
			{
				message = string.Format(CultureInfo.InvariantCulture, message, arguments);
			}
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = LogLevel2ConsoleColor[logLevel];
			try
			{
				LogOutput?.WriteLine(message);
			}
			finally
			{
				Console.ForegroundColor = foregroundColor;
			}
		}
	}
}

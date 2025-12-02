using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using NLog.Internal;
using NLog.Internal.Fakeables;
using NLog.LayoutRenderers;
using NLog.Targets;
using NLog.Time;

namespace NLog.Common;

/// <summary>
/// NLog internal logger.
///
/// Writes to file, console or custom text writer (see <see cref="P:NLog.Common.InternalLogger.LogWriter" />)
/// </summary>
/// <seealso href="https://github.com/NLog/NLog/wiki/Internal-Logging">Documentation on NLog Wiki</seealso>
public static class InternalLogger
{
	private static readonly object LockObject = new object();

	private static LogLevel _logLevel = NLog.LogLevel.Off;

	private static bool _logToConsole;

	private static bool _logToConsoleError;

	private static string? _logFile;

	/// <summary>
	/// Gets a value indicating whether internal log includes Trace messages.
	/// </summary>
	public static bool IsTraceEnabled => IsLogLevelEnabled(NLog.LogLevel.Trace);

	/// <summary>
	/// Gets a value indicating whether internal log includes Debug messages.
	/// </summary>
	public static bool IsDebugEnabled => IsLogLevelEnabled(NLog.LogLevel.Debug);

	/// <summary>
	/// Gets a value indicating whether internal log includes Info messages.
	/// </summary>
	public static bool IsInfoEnabled => IsLogLevelEnabled(NLog.LogLevel.Info);

	/// <summary>
	/// Gets a value indicating whether internal log includes Warn messages.
	/// </summary>
	public static bool IsWarnEnabled => IsLogLevelEnabled(NLog.LogLevel.Warn);

	/// <summary>
	/// Gets a value indicating whether internal log includes Error messages.
	/// </summary>
	public static bool IsErrorEnabled => IsLogLevelEnabled(NLog.LogLevel.Error);

	/// <summary>
	/// Gets a value indicating whether internal log includes Fatal messages.
	/// </summary>
	public static bool IsFatalEnabled => IsLogLevelEnabled(NLog.LogLevel.Fatal);

	/// <summary>
	/// Gets or sets the minimal internal log level.
	/// </summary>
	/// <example>If set to <see cref="F:NLog.LogLevel.Info" />, then messages of the levels <see cref="F:NLog.LogLevel.Info" />, <see cref="F:NLog.LogLevel.Error" /> and <see cref="F:NLog.LogLevel.Fatal" /> will be written.</example>
	public static LogLevel LogLevel
	{
		get
		{
			return _logLevel;
		}
		set
		{
			_logLevel = value ?? NLog.LogLevel.Off;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether internal messages should be written to the console output stream.
	/// </summary>
	/// <remarks>Your application must be a console application.</remarks>
	public static bool LogToConsole
	{
		get
		{
			return _logToConsole;
		}
		set
		{
			if (_logToConsole != value)
			{
				InternalEventOccurred -= LogToConsoleSubscription;
				if (value)
				{
					InternalEventOccurred += LogToConsoleSubscription;
				}
				_logToConsole = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether internal messages should be written to the console error stream.
	/// </summary>
	/// <remarks>Your application must be a console application.</remarks>
	public static bool LogToConsoleError
	{
		get
		{
			return _logToConsoleError;
		}
		set
		{
			if (_logToConsoleError != value)
			{
				InternalEventOccurred -= LogToConsoleErrorSubscription;
				if (value)
				{
					InternalEventOccurred += LogToConsoleErrorSubscription;
				}
				_logToConsoleError = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the file path of the internal log file.
	/// </summary>
	/// <remarks>A value of <see langword="null" /> value disables internal logging to a file.</remarks>
	public static string? LogFile
	{
		get
		{
			return _logFile;
		}
		set
		{
			if (!string.Equals(_logFile, value, StringComparison.Ordinal))
			{
				InternalEventOccurred -= LogToFileSubscription;
				if (!string.IsNullOrEmpty(value))
				{
					InternalEventOccurred += LogToFileSubscription;
				}
				_logFile = value;
			}
			string text = (_logFile = ((value != null && !string.IsNullOrEmpty(value)) ? ExpandFilePathVariables(value) : null));
			if (text != null)
			{
				CreateDirectoriesIfNeeded(text);
			}
		}
	}

	/// <summary>
	/// Gets or sets the text writer that will receive internal logs.
	/// </summary>
	public static TextWriter? LogWriter { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether timestamp should be included in internal log output.
	/// </summary>
	public static bool IncludeTimestamp { get; set; } = true;

	/// <summary>
	/// Is there an <see cref="T:System.Exception" /> thrown when writing the message?
	/// </summary>
	internal static bool ExceptionThrowWhenWriting { get; private set; }

	/// <summary>
	/// Internal LogEvent written to the InternalLogger
	/// </summary>
	/// <remarks>
	/// EventHandler will only be triggered for events, where severity matches the configured <see cref="P:NLog.Common.InternalLogger.LogLevel" />.
	///
	/// Never use/call NLog Logger-objects when handling these internal events, as it will lead to deadlock / stackoverflow.
	/// </remarks>
	public static event InternalEventOccurredHandler? InternalEventOccurred;

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Trace([Localizable(false)] string message, params object?[] args)
	{
		Write(null, NLog.LogLevel.Trace, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public static void Trace([Localizable(false)] string message)
	{
		Write(null, NLog.LogLevel.Trace, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Trace.
	/// </summary>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Trace(Func<string> messageFunc)
	{
		if (IsTraceEnabled)
		{
			Write(null, NLog.LogLevel.Trace, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Trace(Exception? ex, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, NLog.LogLevel.Trace, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	[StringFormatMethod("message")]
	public static void Trace<TArgument1>([Localizable(false)] string message, TArgument1? arg0)
	{
		if (IsTraceEnabled)
		{
			Log(null, NLog.LogLevel.Trace, message, arg0);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	[StringFormatMethod("message")]
	public static void Trace<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1)
	{
		if (IsTraceEnabled)
		{
			Log(null, NLog.LogLevel.Trace, message, arg0, arg1);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	/// <param name="arg2">Argument {2} to the message.</param>
	[StringFormatMethod("message")]
	public static void Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1, TArgument3? arg2)
	{
		if (IsTraceEnabled)
		{
			Log(null, NLog.LogLevel.Trace, message, arg0, arg1, arg2);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Log message.</param>
	public static void Trace(Exception? ex, [Localizable(false)] string message)
	{
		Write(ex, NLog.LogLevel.Trace, message, null);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Trace level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Trace.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Trace(Exception? ex, Func<string> messageFunc)
	{
		if (IsTraceEnabled)
		{
			Write(ex, NLog.LogLevel.Trace, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Debug level.
	/// </summary>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Debug([Localizable(false)] string message, params object?[] args)
	{
		Write(null, NLog.LogLevel.Debug, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Debug level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public static void Debug([Localizable(false)] string message)
	{
		Write(null, NLog.LogLevel.Debug, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Debug level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Debug.
	/// </summary>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Debug(Func<string> messageFunc)
	{
		if (IsDebugEnabled)
		{
			Write(null, NLog.LogLevel.Debug, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Debug level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Debug(Exception? ex, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, NLog.LogLevel.Debug, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	[StringFormatMethod("message")]
	public static void Debug<TArgument1>([Localizable(false)] string message, TArgument1? arg0)
	{
		if (IsDebugEnabled)
		{
			Log(null, NLog.LogLevel.Debug, message, arg0);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	[StringFormatMethod("message")]
	public static void Debug<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1)
	{
		if (IsDebugEnabled)
		{
			Log(null, NLog.LogLevel.Debug, message, arg0, arg1);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	/// <param name="arg2">Argument {2} to the message.</param>
	[StringFormatMethod("message")]
	public static void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1, TArgument3? arg2)
	{
		if (IsDebugEnabled)
		{
			Log(null, NLog.LogLevel.Debug, message, arg0, arg1, arg2);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Debug level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Log message.</param>
	public static void Debug(Exception? ex, [Localizable(false)] string message)
	{
		Write(ex, NLog.LogLevel.Debug, message, null);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Debug level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Debug.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Debug(Exception? ex, Func<string> messageFunc)
	{
		if (IsDebugEnabled)
		{
			Write(ex, NLog.LogLevel.Debug, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Info level.
	/// </summary>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Info([Localizable(false)] string message, params object?[] args)
	{
		Write(null, NLog.LogLevel.Info, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Info level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public static void Info([Localizable(false)] string message)
	{
		Write(null, NLog.LogLevel.Info, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Info level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Info.
	/// </summary>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Info(Func<string> messageFunc)
	{
		if (IsInfoEnabled)
		{
			Write(null, NLog.LogLevel.Info, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Info level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Info(Exception? ex, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, NLog.LogLevel.Info, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	[StringFormatMethod("message")]
	public static void Info<TArgument1>([Localizable(false)] string message, TArgument1? arg0)
	{
		if (IsInfoEnabled)
		{
			Log(null, NLog.LogLevel.Info, message, arg0);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	[StringFormatMethod("message")]
	public static void Info<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1)
	{
		if (IsInfoEnabled)
		{
			Log(null, NLog.LogLevel.Info, message, arg0, arg1);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	/// <param name="arg2">Argument {2} to the message.</param>
	[StringFormatMethod("message")]
	public static void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1, TArgument3? arg2)
	{
		if (IsInfoEnabled)
		{
			Log(null, NLog.LogLevel.Info, message, arg0, arg1, arg2);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Info level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Log message.</param>
	public static void Info(Exception? ex, [Localizable(false)] string message)
	{
		Write(ex, NLog.LogLevel.Info, message, null);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Info level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Info.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Info(Exception? ex, Func<string> messageFunc)
	{
		if (IsInfoEnabled)
		{
			Write(ex, NLog.LogLevel.Info, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Warn level.
	/// </summary>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Warn([Localizable(false)] string message, params object?[] args)
	{
		Write(null, NLog.LogLevel.Warn, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Warn level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public static void Warn([Localizable(false)] string message)
	{
		Write(null, NLog.LogLevel.Warn, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Warn level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Warn.
	/// </summary>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Warn(Func<string> messageFunc)
	{
		if (IsWarnEnabled)
		{
			Write(null, NLog.LogLevel.Warn, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Warn level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Warn(Exception? ex, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, NLog.LogLevel.Warn, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	[StringFormatMethod("message")]
	public static void Warn<TArgument1>([Localizable(false)] string message, TArgument1? arg0)
	{
		if (IsWarnEnabled)
		{
			Log(null, NLog.LogLevel.Warn, message, arg0);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	[StringFormatMethod("message")]
	public static void Warn<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1)
	{
		if (IsWarnEnabled)
		{
			Log(null, NLog.LogLevel.Warn, message, arg0, arg1);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	/// <param name="arg2">Argument {2} to the message.</param>
	[StringFormatMethod("message")]
	public static void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1, TArgument3? arg2)
	{
		if (IsWarnEnabled)
		{
			Log(null, NLog.LogLevel.Warn, message, arg0, arg1, arg2);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Warn level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Log message.</param>
	public static void Warn(Exception? ex, [Localizable(false)] string message)
	{
		Write(ex, NLog.LogLevel.Warn, message, null);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Warn level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Warn.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Warn(Exception? ex, Func<string> messageFunc)
	{
		if (IsWarnEnabled)
		{
			Write(ex, NLog.LogLevel.Warn, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Error level.
	/// </summary>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Error([Localizable(false)] string message, params object?[] args)
	{
		Write(null, NLog.LogLevel.Error, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Error level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public static void Error([Localizable(false)] string message)
	{
		Write(null, NLog.LogLevel.Error, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Error level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Error.
	/// </summary>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Error(Func<string> messageFunc)
	{
		if (IsErrorEnabled)
		{
			Write(null, NLog.LogLevel.Error, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Error level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Error(Exception? ex, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, NLog.LogLevel.Error, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	[StringFormatMethod("message")]
	public static void Error<TArgument1>([Localizable(false)] string message, TArgument1? arg0)
	{
		if (IsErrorEnabled)
		{
			Log(null, NLog.LogLevel.Error, message, arg0);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	[StringFormatMethod("message")]
	public static void Error<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1)
	{
		if (IsErrorEnabled)
		{
			Log(null, NLog.LogLevel.Error, message, arg0, arg1);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	/// <param name="arg2">Argument {2} to the message.</param>
	[StringFormatMethod("message")]
	public static void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1, TArgument3? arg2)
	{
		if (IsErrorEnabled)
		{
			Log(null, NLog.LogLevel.Error, message, arg0, arg1, arg2);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Error level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Log message.</param>
	public static void Error(Exception? ex, [Localizable(false)] string message)
	{
		Write(ex, NLog.LogLevel.Error, message, null);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Error level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Error.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Error(Exception? ex, Func<string> messageFunc)
	{
		if (IsErrorEnabled)
		{
			Write(ex, NLog.LogLevel.Error, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Fatal level.
	/// </summary>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Fatal([Localizable(false)] string message, params object?[] args)
	{
		Write(null, NLog.LogLevel.Fatal, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Fatal level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public static void Fatal([Localizable(false)] string message)
	{
		Write(null, NLog.LogLevel.Fatal, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Fatal level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Fatal.
	/// </summary>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Fatal(Func<string> messageFunc)
	{
		if (IsFatalEnabled)
		{
			Write(null, NLog.LogLevel.Fatal, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Fatal level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Fatal(Exception? ex, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, NLog.LogLevel.Fatal, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	[StringFormatMethod("message")]
	public static void Fatal<TArgument1>([Localizable(false)] string message, TArgument1? arg0)
	{
		if (IsFatalEnabled)
		{
			Log(null, NLog.LogLevel.Fatal, message, arg0);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	[StringFormatMethod("message")]
	public static void Fatal<TArgument1, TArgument2>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1)
	{
		if (IsFatalEnabled)
		{
			Log(null, NLog.LogLevel.Fatal, message, arg0, arg1);
		}
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the Trace level.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="arg0">Argument {0} to the message.</param>
	/// <param name="arg1">Argument {1} to the message.</param>
	/// <param name="arg2">Argument {2} to the message.</param>
	[StringFormatMethod("message")]
	public static void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)] string message, TArgument1? arg0, TArgument2? arg1, TArgument3? arg2)
	{
		if (IsFatalEnabled)
		{
			Log(null, NLog.LogLevel.Fatal, message, arg0, arg1, arg2);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Fatal level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="message">Log message.</param>
	public static void Fatal(Exception? ex, [Localizable(false)] string message)
	{
		Write(ex, NLog.LogLevel.Fatal, message, null);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the Fatal level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level  Fatal.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	[Obsolete("Avoid delegate capture allocations. Marked obsolete with v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Fatal(Exception? ex, Func<string> messageFunc)
	{
		if (IsFatalEnabled)
		{
			Write(ex, NLog.LogLevel.Fatal, messageFunc(), null);
		}
	}

	/// <summary>
	/// Set the config of the InternalLogger with defaults and config.
	/// </summary>
	public static void Reset()
	{
		ExceptionThrowWhenWriting = false;
		LogWriter = null;
		InternalLogger.InternalEventOccurred = null;
		LogLevel = NLog.LogLevel.Off;
		IncludeTimestamp = true;
		LogToConsole = false;
		LogToConsoleError = false;
		LogFile = null;
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the specified level.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Log(LogLevel level, [Localizable(false)] string message, params object?[] args)
	{
		Write(null, level, message, args);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the specified level.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="message">Log message.</param>
	public static void Log(LogLevel level, [Localizable(false)] string message)
	{
		Write(null, level, message, null);
	}

	/// <summary>
	/// Logs the specified message without an <see cref="T:System.Exception" /> at the specified level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level <paramref name="level" />.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	public static void Log(LogLevel level, [Localizable(false)] Func<string> messageFunc)
	{
		if (IsLogLevelEnabled(level))
		{
			Write(null, level, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the specified level.
	/// <paramref name="messageFunc" /> will be only called when logging is enabled for level <paramref name="level" />.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="level">Log level.</param>
	/// <param name="messageFunc">Function that returns the log message.</param>
	public static void Log(Exception? ex, LogLevel level, [Localizable(false)] Func<string> messageFunc)
	{
		if (IsLogLevelEnabled(level))
		{
			Write(ex, level, messageFunc(), null);
		}
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the specified level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="level">Log level.</param>
	/// <param name="message">Message which may include positional parameters.</param>
	/// <param name="args">Arguments to the message.</param>
	[StringFormatMethod("message")]
	public static void Log(Exception? ex, LogLevel level, [Localizable(false)] string message, params object?[] args)
	{
		Write(ex, level, message, args);
	}

	/// <summary>
	/// Logs the specified message with an <see cref="T:System.Exception" /> at the specified level.
	/// </summary>
	/// <param name="ex">Exception to be logged.</param>
	/// <param name="level">Log level.</param>
	/// <param name="message">Log message.</param>
	public static void Log(Exception? ex, LogLevel level, [Localizable(false)] string message)
	{
		Write(ex, level, message, null);
	}

	/// <summary>
	/// Write to internallogger.
	/// </summary>
	/// <param name="ex">optional exception to be logged.</param>
	/// <param name="level">level</param>
	/// <param name="message">message</param>
	/// <param name="args">optional args for <paramref name="message" /></param>
	private static void Write(Exception? ex, LogLevel level, string message, object?[]? args)
	{
		if (!IsLogLevelEnabled(level) || IsSeriousException(ex) || (InternalLogger.InternalEventOccurred == null && LogWriter == null))
		{
			return;
		}
		string fullMessage = message;
		try
		{
			fullMessage = ((args != null && args.Length != 0) ? string.Format(CultureInfo.InvariantCulture, message, args) : message);
		}
		catch (Exception ex2)
		{
			if (ex == null)
			{
				ex = ex2;
			}
			if (NLog.LogLevel.Error > level)
			{
				level = NLog.LogLevel.Error;
			}
		}
		try
		{
			IInternalLoggerContext loggerContext = ((args != null && args.Length != 0) ? (args[0] as IInternalLoggerContext) : null);
			WriteToLog(level, ex, fullMessage, loggerContext);
			ex?.MarkAsLoggedToInternalLogger();
		}
		catch (Exception exception)
		{
			ExceptionThrowWhenWriting = true;
			if (exception.MustBeRethrownImmediately())
			{
				throw;
			}
		}
	}

	private static void WriteToLog(LogLevel level, Exception? ex, string fullMessage, IInternalLoggerContext? loggerContext)
	{
		if (LogWriter != null)
		{
			string value = CreateLogLine(ex, level, fullMessage);
			lock (LockObject)
			{
				LogWriter?.WriteLine(value);
			}
		}
		if (InternalLogger.InternalEventOccurred != null)
		{
			string senderName = ((loggerContext != null && !string.IsNullOrEmpty(loggerContext.Name)) ? loggerContext.Name : loggerContext?.ToString());
			InternalLogger.InternalEventOccurred?.Invoke(null, new InternalLogEventArgs(fullMessage, level, ex, loggerContext?.GetType(), senderName));
		}
	}

	/// <summary>
	/// Create log line with timestamp, exception message etc (if configured)
	/// </summary>
	private static string CreateLogLine(Exception? ex, LogLevel level, string fullMessage)
	{
		if (IncludeTimestamp)
		{
			return TimeSource.Current.Time.ToString("yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture) + " " + level.ToString() + " " + fullMessage + ((ex != null) ? " Exception: " : string.Empty) + (ex?.ToString() ?? "");
		}
		return level.ToString() + " " + fullMessage + ((ex != null) ? " Exception: " : string.Empty) + (ex?.ToString() ?? string.Empty);
	}

	/// <summary>
	/// Determine if logging should be avoided because of exception type.
	/// </summary>
	/// <param name="exception">The exception to check.</param>
	/// <returns><see langword="true" /> if logging should be avoided; otherwise, <see langword="false" />.</returns>
	private static bool IsSeriousException(Exception? exception)
	{
		return exception?.MustBeRethrownImmediately() ?? false;
	}

	/// <summary>
	/// Determine if logging is enabled for given LogLevel
	/// </summary>
	/// <param name="logLevel">The <see cref="P:NLog.Common.InternalLogger.LogLevel" /> for the log event.</param>
	/// <returns><see langword="true" /> if logging is enabled; otherwise, <see langword="false" />.</returns>
	private static bool IsLogLevelEnabled(LogLevel logLevel)
	{
		if ((object)_logLevel != NLog.LogLevel.Off)
		{
			return _logLevel.CompareTo(logLevel) <= 0;
		}
		return false;
	}

	/// <summary>
	/// Determine if logging is enabled.
	/// </summary>
	/// <returns><see langword="true" /> if logging is enabled; otherwise, <see langword="false" />.</returns>
	internal static bool HasActiveLoggers()
	{
		if (InternalLogger.InternalEventOccurred == null && LogWriter == null)
		{
			return false;
		}
		return true;
	}

	private static void CreateDirectoriesIfNeeded(string filename)
	{
		try
		{
			if (!(LogLevel == NLog.LogLevel.Off))
			{
				string directoryName = Path.GetDirectoryName(filename);
				if (!string.IsNullOrEmpty(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
			}
		}
		catch (Exception ex)
		{
			Error(ex, "Cannot create needed directories to '{0}'.", filename);
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
		}
	}

	private static string ExpandFilePathVariables(string internalLogFile)
	{
		try
		{
			if (ContainsSubStringIgnoreCase(internalLogFile, "${currentdir}", out string result))
			{
				internalLogFile = internalLogFile.Replace(result, Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar);
			}
			if (ContainsSubStringIgnoreCase(internalLogFile, "${basedir}", out string result2))
			{
				internalLogFile = internalLogFile.Replace(result2, LogManager.LogFactory.CurrentAppEnvironment.AppDomainBaseDirectory + Path.DirectorySeparatorChar);
			}
			if (ContainsSubStringIgnoreCase(internalLogFile, "${tempdir}", out string result3))
			{
				internalLogFile = internalLogFile.Replace(result3, LogManager.LogFactory.CurrentAppEnvironment.UserTempFilePath + Path.DirectorySeparatorChar);
			}
			if (ContainsSubStringIgnoreCase(internalLogFile, "${processdir}", out string result4))
			{
				internalLogFile = internalLogFile.Replace(result4, Path.GetDirectoryName(LogManager.LogFactory.CurrentAppEnvironment.CurrentProcessFilePath) + Path.DirectorySeparatorChar);
			}
			if (ContainsSubStringIgnoreCase(internalLogFile, "${commonApplicationDataDir}", out string result5))
			{
				internalLogFile = internalLogFile.Replace(result5, SpecialFolderLayoutRenderer.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar);
			}
			if (ContainsSubStringIgnoreCase(internalLogFile, "${userApplicationDataDir}", out string result6))
			{
				internalLogFile = internalLogFile.Replace(result6, SpecialFolderLayoutRenderer.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar);
			}
			if (ContainsSubStringIgnoreCase(internalLogFile, "${userLocalApplicationDataDir}", out string result7))
			{
				internalLogFile = internalLogFile.Replace(result7, SpecialFolderLayoutRenderer.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar);
			}
			if (internalLogFile.IndexOf('%') >= 0)
			{
				internalLogFile = Environment.ExpandEnvironmentVariables(internalLogFile);
			}
			if (!string.IsNullOrEmpty(internalLogFile) && internalLogFile.IndexOf('.') >= 0)
			{
				internalLogFile = AppEnvironmentWrapper.FixFilePathWithLongUNC(internalLogFile);
			}
			return internalLogFile;
		}
		catch
		{
			return internalLogFile;
		}
	}

	private static bool ContainsSubStringIgnoreCase(string haystack, string needle, out string? result)
	{
		int num = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase);
		result = ((num >= 0) ? haystack.Substring(num, needle.Length) : null);
		return result != null;
	}

	private static void LogToConsoleSubscription(object? sender, InternalLogEventArgs eventArgs)
	{
		string message = CreateLogLine(eventArgs.Exception, eventArgs.Level, eventArgs.Message);
		ConsoleTargetHelper.WriteLineThreadSafe(Console.Out, message);
	}

	private static void LogToConsoleErrorSubscription(object? sender, InternalLogEventArgs eventArgs)
	{
		string message = CreateLogLine(eventArgs.Exception, eventArgs.Level, eventArgs.Message);
		ConsoleTargetHelper.WriteLineThreadSafe(Console.Error, message);
	}

	private static void LogToFileSubscription(object? sender, InternalLogEventArgs eventArgs)
	{
		string value = CreateLogLine(eventArgs.Exception, eventArgs.Level, eventArgs.Message);
		lock (LockObject)
		{
			try
			{
				using StreamWriter streamWriter = File.AppendText(_logFile);
				streamWriter.WriteLine(value);
			}
			catch (IOException)
			{
			}
		}
	}
}

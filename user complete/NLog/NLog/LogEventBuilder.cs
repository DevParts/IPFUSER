using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NLog.Internal;

namespace NLog;

/// <summary>
/// A fluent builder for logging events to NLog.
/// </summary>
[CLSCompliant(false)]
public struct LogEventBuilder
{
	private readonly ILogger _logger;

	private readonly LogEventInfo? _logEvent;

	/// <summary>
	/// The logger to write the log event to
	/// </summary>
	public ILogger Logger => _logger;

	/// <summary>
	/// Logging event that will be written
	/// </summary>
	public LogEventInfo? LogEvent
	{
		get
		{
			if (_logEvent != null)
			{
				return ResolveLogEvent(_logEvent);
			}
			return null;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventBuilder" /> class.
	/// </summary>
	/// <param name="logger">The <see cref="T:NLog.Logger" /> to send the log event.</param>
	public LogEventBuilder(ILogger logger)
	{
		_logger = Guard.ThrowIfNull(logger, "logger");
		_logEvent = new LogEventInfo
		{
			LoggerName = _logger.Name
		};
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventBuilder" /> class.
	/// </summary>
	/// <param name="logger">The <see cref="T:NLog.Logger" /> to send the log event.</param>
	/// <param name="logLevel">The log level. LogEvent is only created when <see cref="T:NLog.LogLevel" /> is enabled for <paramref name="logger" /></param>
	public LogEventBuilder(ILogger logger, LogLevel logLevel)
	{
		_logger = Guard.ThrowIfNull(logger, "logger");
		Guard.ThrowIfNull(logLevel, "logLevel");
		if (logger.IsEnabled(logLevel))
		{
			_logEvent = new LogEventInfo
			{
				LoggerName = _logger.Name,
				Level = logLevel
			};
		}
		else
		{
			_logEvent = null;
		}
	}

	/// <summary>
	/// Sets a per-event context property on the logging event.
	/// </summary>
	/// <param name="propertyName">The name of the context property.</param>
	/// <param name="propertyValue">The value of the context property.</param>
	public LogEventBuilder Property<T>(string propertyName, T? propertyValue)
	{
		Guard.ThrowIfNull(propertyName, "propertyName");
		if (_logEvent != null)
		{
			_logEvent.Properties[propertyName] = propertyValue;
		}
		return this;
	}

	/// <summary>
	/// Sets multiple per-event context properties on the logging event.
	/// </summary>
	/// <param name="properties">The properties to set.</param>
	public LogEventBuilder Properties(IEnumerable<KeyValuePair<string, object?>> properties)
	{
		Guard.ThrowIfNull(properties, "properties");
		if (_logEvent != null)
		{
			foreach (KeyValuePair<string, object> property in properties)
			{
				_logEvent.Properties[property.Key] = property.Value;
			}
		}
		return this;
	}

	/// <summary>
	/// Sets the <paramref name="exception" /> information of the logging event.
	/// </summary>
	/// <param name="exception">The exception information of the logging event.</param>
	public LogEventBuilder Exception(Exception? exception)
	{
		if (_logEvent != null)
		{
			_logEvent.Exception = exception;
		}
		return this;
	}

	/// <summary>
	/// Sets the timestamp of the logging event.
	/// </summary>
	/// <param name="timeStamp">The timestamp of the logging event.</param>
	public LogEventBuilder TimeStamp(DateTime timeStamp)
	{
		if (_logEvent != null)
		{
			_logEvent.TimeStamp = timeStamp;
		}
		return this;
	}

	/// <summary>
	/// Sets the log message on the logging event.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	public LogEventBuilder Message([Localizable(false)] string message)
	{
		if (_logEvent != null)
		{
			_logEvent.Parameters = null;
			_logEvent.Message = message;
		}
		return this;
	}

	/// <summary>
	/// Sets the log message and parameters for formatting for the logging event.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public LogEventBuilder Message<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (_logEvent != null)
		{
			_logEvent.Message = message;
			_logEvent.Parameters = new object[1] { argument };
		}
		return this;
	}

	/// <summary>
	/// Sets the log message and parameters for formatting on the logging event.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public LogEventBuilder Message<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (_logEvent != null)
		{
			_logEvent.Message = message;
			_logEvent.Parameters = new object[2] { argument1, argument2 };
		}
		return this;
	}

	/// <summary>
	/// Sets the log message and parameters for formatting on the logging event.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public LogEventBuilder Message<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (_logEvent != null)
		{
			_logEvent.Message = message;
			_logEvent.Parameters = new object[3] { argument1, argument2, argument3 };
		}
		return this;
	}

	/// <summary>
	/// Sets the log message and parameters for formatting on the logging event.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public LogEventBuilder Message([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (_logEvent != null)
		{
			_logEvent.Message = message;
			_logEvent.Parameters = args;
		}
		return this;
	}

	/// <summary>
	/// Sets the log message and parameters for formatting on the logging event.
	/// </summary>
	/// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public LogEventBuilder Message(IFormatProvider formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (_logEvent != null)
		{
			_logEvent.FormatProvider = formatProvider;
			_logEvent.Message = message;
			_logEvent.Parameters = args;
		}
		return this;
	}

	/// <summary>
	/// Writes the log event to the underlying logger.
	/// </summary>
	/// <param name="callerClassName">The class of the caller to the method. This is captured by the NLog engine when necessary</param>
	/// <param name="callerMemberName">The method or property name of the caller to the method. This is set at by the compiler.</param>
	/// <param name="callerFilePath">The full path of the source file that contains the caller. This is set at by the compiler.</param>
	/// <param name="callerLineNumber">The line number in the source file at which the method is called. This is set at by the compiler.</param>
	public LogEventBuilder Callsite(string? callerClassName = null, [CallerMemberName] string? callerMemberName = null, [CallerFilePath] string? callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
	{
		if (_logEvent != null)
		{
			_logEvent.SetCallerInfo(callerClassName, callerMemberName, callerFilePath, callerLineNumber);
		}
		return this;
	}

	/// <summary>
	/// Writes the log event to the underlying logger.
	/// </summary>
	/// <param name="logLevel">The log level. Optional but when assigned to <see cref="F:NLog.LogLevel.Off" /> then it will discard the LogEvent.</param>
	/// <param name="callerMemberName">The method or property name of the caller to the method. This is set at by the compiler.</param>
	/// <param name="callerFilePath">The full path of the source file that contains the caller. This is set at by the compiler.</param>
	/// <param name="callerLineNumber">The line number in the source file at which the method is called. This is set at by the compiler.</param>
	public void Log(LogLevel? logLevel = null, [CallerMemberName] string? callerMemberName = null, [CallerFilePath] string? callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
	{
		if (_logEvent != null)
		{
			LogEventInfo logEventInfo = ResolveLogEvent(_logEvent, logLevel);
			if (logEventInfo.CallSiteInformation == null && _logger.IsEnabled(logEventInfo.Level))
			{
				_logEvent.SetCallerInfo(null, callerMemberName, callerFilePath, callerLineNumber);
			}
			_logger.Log(logEventInfo);
		}
	}

	/// <summary>
	/// Writes the log event to the underlying logger.
	/// </summary>
	/// <param name="wrapperType">Type of custom Logger wrapper.</param>
	public void Log(Type wrapperType)
	{
		if (_logEvent != null)
		{
			LogEventInfo logEvent = ResolveLogEvent(_logEvent);
			_logger.Log(wrapperType, logEvent);
		}
	}

	private LogEventInfo ResolveLogEvent(LogEventInfo logEvent, LogLevel? logLevel = null)
	{
		if ((object)logLevel == null)
		{
			if ((object)logEvent.Level == null)
			{
				logEvent.Level = ((logEvent.Exception != null) ? LogLevel.Error : LogLevel.Info);
			}
		}
		else
		{
			logEvent.Level = logLevel;
		}
		if ((logEvent.Message == null || (object)logEvent.Message == string.Empty) && logEvent.Exception != null && _logger.IsEnabled(logEvent.Level))
		{
			logEvent.FormatProvider = ExceptionMessageFormatProvider.Instance;
			logEvent.Message = "{0}";
			logEvent.Parameters = new object[1] { logEvent.Exception };
		}
		return logEvent;
	}
}

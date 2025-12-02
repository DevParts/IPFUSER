using System;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Extensions for NLog <see cref="T:NLog.ILogger" />.
/// </summary>
[CLSCompliant(false)]
public static class ILoggerExtensions
{
	/// <summary>
	/// Starts building a log event with the specified <see cref="T:NLog.LogLevel" />.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <param name="logLevel">The log level. When not</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForLogEvent(this ILogger logger, LogLevel? logLevel = null)
	{
		if ((object)logLevel != null)
		{
			return new LogEventBuilder(logger, logLevel);
		}
		return new LogEventBuilder(logger);
	}

	/// <summary>
	/// Starts building a log event at the <c>Trace</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForTraceEvent(this ILogger logger)
	{
		return new LogEventBuilder(logger, LogLevel.Trace);
	}

	/// <summary>
	/// Starts building a log event at the <c>Debug</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForDebugEvent(this ILogger logger)
	{
		return new LogEventBuilder(logger, LogLevel.Debug);
	}

	/// <summary>
	/// Starts building a log event at the <c>Info</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForInfoEvent(this ILogger logger)
	{
		return new LogEventBuilder(logger, LogLevel.Info);
	}

	/// <summary>
	/// Starts building a log event at the <c>Warn</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForWarnEvent(this ILogger logger)
	{
		return new LogEventBuilder(logger, LogLevel.Warn);
	}

	/// <summary>
	/// Starts building a log event at the <c>Error</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForErrorEvent(this ILogger logger)
	{
		return new LogEventBuilder(logger, LogLevel.Error);
	}

	/// <summary>
	/// Starts building a log event at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForFatalEvent(this ILogger logger)
	{
		return new LogEventBuilder(logger, LogLevel.Fatal);
	}

	/// <summary>
	/// Starts building a log event at the <c>Exception</c> level.
	/// </summary>
	/// <param name="logger">The logger to write the log event to.</param>
	/// <param name="exception">The exception information of the logging event.</param>
	/// <param name="logLevel">The <see cref="T:NLog.LogLevel" /> for the log event. Defaults to <see cref="F:NLog.LogLevel.Error" /> when not specified.</param>
	/// <returns><see cref="T:NLog.LogEventBuilder" /> for chaining calls.</returns>
	public static LogEventBuilder ForExceptionEvent(this ILogger logger, Exception? exception, LogLevel? logLevel = null)
	{
		return logger.ForLogEvent(logLevel ?? LogLevel.Error).Exception(exception);
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public static void ConditionalDebug<T>(this ILogger logger, T? value)
	{
		logger.Debug(value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public static void ConditionalDebug<T>(this ILogger logger, IFormatProvider? formatProvider, T? value)
	{
		logger.Debug(formatProvider, value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	[Conditional("DEBUG")]
	public static void ConditionalDebug(this ILogger logger, LogMessageGenerator messageFunc)
	{
		logger.Debug(messageFunc);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug(this ILogger logger, Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Debug(exception, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug(this ILogger logger, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Debug(exception, formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">Log message.</param>
	[Conditional("DEBUG")]
	public static void ConditionalDebug(this ILogger logger, [Localizable(false)] string message)
	{
		logger.Debug(message);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Debug(message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug(this ILogger logger, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Debug(formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug<TArgument>(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (logger.IsDebugEnabled)
		{
			logger.Debug<object>(message, (object?)argument);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug<TArgument1, TArgument2>(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (logger.IsDebugEnabled)
		{
			logger.Debug<object, object>(message, (object?)argument1, (object?)argument2);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalDebug<TArgument1, TArgument2, TArgument3>(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (logger.IsDebugEnabled)
		{
			logger.Debug<object, object, object>(message, (object?)argument1, (object?)argument2, (object?)argument3);
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public static void ConditionalTrace<T>(this ILogger logger, T? value)
	{
		logger.Trace(value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public static void ConditionalTrace<T>(this ILogger logger, IFormatProvider? formatProvider, T? value)
	{
		logger.Trace(formatProvider, value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	[Conditional("DEBUG")]
	public static void ConditionalTrace(this ILogger logger, LogMessageGenerator messageFunc)
	{
		logger.Trace(messageFunc);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace(this ILogger logger, Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Trace(exception, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace(this ILogger logger, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Trace(exception, formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">Log message.</param>
	[Conditional("DEBUG")]
	public static void ConditionalTrace(this ILogger logger, [Localizable(false)] string message)
	{
		logger.Trace(message);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Trace(message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace(this ILogger logger, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		logger.Trace(formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace<TArgument>(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (logger.IsTraceEnabled)
		{
			logger.Trace<object>(message, (object?)argument);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace<TArgument1, TArgument2>(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (logger.IsTraceEnabled)
		{
			logger.Trace<object, object>(message, (object?)argument1, (object?)argument2);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public static void ConditionalTrace<TArgument1, TArgument2, TArgument3>(this ILogger logger, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (logger.IsTraceEnabled)
		{
			logger.Trace<object, object, object>(message, (object?)argument1, (object?)argument2, (object?)argument3);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="level">The log level.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Log(this ILogger logger, LogLevel level, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsEnabled(level))
		{
			logger.Log(level, exception, messageFunc(), ArrayHelper.Empty<object>());
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Trace(this ILogger logger, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsTraceEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			logger.Trace(exception, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Debug(this ILogger logger, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsDebugEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			logger.Debug(exception, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Info(this ILogger logger, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsInfoEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			logger.Info(exception, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Warn(this ILogger logger, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsWarnEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			logger.Warn(exception, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Error(this ILogger logger, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsErrorEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			logger.Error(exception, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="logger">A logger implementation that will handle the message.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public static void Fatal(this ILogger logger, Exception? exception, LogMessageGenerator messageFunc)
	{
		if (logger.IsFatalEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			logger.Fatal(exception, messageFunc());
		}
	}
}

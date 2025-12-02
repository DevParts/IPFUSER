using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace NLog;

/// <content>
/// Auto-generated Logger members for binary compatibility with NLog 1.0.
/// </content>
///  <summary>
///  Obsolete and replaced by <see cref="T:NLog.ILogger" /> with NLog v5.3.
///
///  Logger with only generic methods (passing 'LogLevel' to methods) and core properties.
///  </summary>
[CLSCompliant(false)]
[Obsolete("ILoggerBase should be replaced with ILogger. Marked obsolete with NLog v5.3")]
[EditorBrowsable(EditorBrowsableState.Never)]
public interface ILoggerBase
{
	/// <summary>
	/// Gets the name of the logger.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Obsolete on the ILogger-interface, instead use <see cref="P:NLog.Logger.Factory" /> with NLog v5.3.
	/// Gets the factory that created this logger.
	/// </summary>
	[Obsolete("Factory-property is hard to mock for ILogger-interface. Instead use Logger.Factory. Marked obsolete with NLog v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	LogFactory Factory { get; }

	/// <summary>
	/// Obsolete on the ILogger-interface, instead use <see cref="E:NLog.Logger.LoggerReconfigured" /> with NLog v5.3.
	/// Occurs when logger configuration changes.
	/// </summary>
	[Obsolete("LoggerReconfigured-EventHandler is very exotic for ILogger-interface. Instead use Logger.LoggerReconfigured. Marked obsolete with NLog v5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	event EventHandler<EventArgs> LoggerReconfigured;

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Log(LogLevel level, object? value);

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Log(LogLevel level, IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the specified level.
	/// </summary>
	/// <param name="level">Log level to be checked.</param>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the specified level, otherwise it returns <see langword="false" />.</returns>
	bool IsEnabled(LogLevel level);

	/// <summary>
	/// Writes the specified diagnostic message.
	/// </summary>
	/// <param name="logEvent">Log event.</param>
	void Log(LogEventInfo logEvent);

	/// <summary>
	/// Writes the specified diagnostic message.
	/// </summary>
	/// <param name="wrapperType">Type of custom Logger wrapper.</param>
	/// <param name="logEvent">Log event.</param>
	void Log(Type wrapperType, LogEventInfo logEvent);

	/// <overloads>
	/// Writes the diagnostic message at the specified level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="value">The value to be written.</param>
	void Log<T>(LogLevel level, T? value);

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Log<T>(LogLevel level, IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Log(LogLevel level, LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	/// <param name="exception">An exception to be logged.</param>
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	/// <param name="exception">An exception to be logged.</param>
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">Log message.</param>
	void Log(LogLevel level, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log<TArgument>(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log<TArgument>(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log<TArgument1, TArgument2>(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILoggerBase.Log(NLog.LogLevel,System.Exception,System.String,System.Object[])" /> - Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Log(LogLevel level, Exception exception, [Localizable(false)] string message, params object[] args) instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Log(LogLevel level, [Localizable(false)] string message, Exception? exception);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILoggerBase.Log(NLog.LogLevel,System.Exception,System.String,System.Object[])" /> - Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Log(LogLevel level, Exception exception, [Localizable(false)] string message, params object[] args) instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void LogException(LogLevel level, [Localizable(false)] string message, Exception? exception);
}

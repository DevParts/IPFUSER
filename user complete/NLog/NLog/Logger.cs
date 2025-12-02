using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog.Common;
using NLog.Internal;

namespace NLog;

/// <content>
/// Logging methods which only are executed when the DEBUG conditional compilation symbol is set.
///
/// Remarks:
/// The DEBUG conditional compilation symbol is default enabled (only) in a debug build.
///
/// If the DEBUG conditional compilation symbol isn't set in the calling library, the compiler will remove all the invocations to these methods.
/// This could lead to better performance.
///
/// See: https://msdn.microsoft.com/en-us/library/4xssyw96%28v=vs.90%29.aspx
/// </content>
///             <summary>
///             Provides logging interface and utility functions.
///             </summary>
///             <content>
///             Auto-generated Logger members for binary compatibility with NLog 1.0.
///             </content>
///             <summary>
///             Provides logging interface and utility functions.
///             </summary>
[CLSCompliant(true)]
public class Logger : ILogger, ISuppress, ILoggerBase
{
	internal static readonly Type DefaultLoggerType = typeof(Logger);

	private ITargetWithFilterChain[] _targetsByLevel = TargetWithFilterChain.NoTargetsByLevel;

	private Logger _contextLogger;

	private ThreadSafeDictionary<string, object?>? _contextProperties;

	private volatile bool _isTraceEnabled;

	private volatile bool _isDebugEnabled;

	private volatile bool _isInfoEnabled;

	private volatile bool _isWarnEnabled;

	private volatile bool _isErrorEnabled;

	private volatile bool _isFatalEnabled;

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Trace</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Trace</c> level, otherwise it returns <see langword="false" />.</returns>
	public bool IsTraceEnabled => _contextLogger._isTraceEnabled;

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Debug</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Debug</c> level, otherwise it returns <see langword="false" />.</returns>
	public bool IsDebugEnabled => _contextLogger._isDebugEnabled;

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Info</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Info</c> level, otherwise it returns <see langword="false" />.</returns>
	public bool IsInfoEnabled => _contextLogger._isInfoEnabled;

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Warn</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Warn</c> level, otherwise it returns <see langword="false" />.</returns>
	public bool IsWarnEnabled => _contextLogger._isWarnEnabled;

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Error</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Error</c> level, otherwise it returns <see langword="false" />.</returns>
	public bool IsErrorEnabled => _contextLogger._isErrorEnabled;

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Fatal</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Fatal</c> level, otherwise it returns <see langword="false" />.</returns>
	public bool IsFatalEnabled => _contextLogger._isFatalEnabled;

	/// <summary>
	/// Gets the name of the logger.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// Gets the factory that created this logger.
	/// </summary>
	public LogFactory Factory { get; private set; }

	/// <summary>
	/// Collection of context properties for the Logger. The logger will append it for all log events
	/// </summary>
	/// <remarks>
	/// It is recommended to use <see cref="M:NLog.Logger.WithProperty(System.String,System.Object)" /> for modifying context properties
	/// when same named logger is used at multiple locations or shared by different thread contexts.
	/// </remarks>
	public IDictionary<string, object?> Properties => _contextProperties ?? Interlocked.CompareExchange(ref _contextProperties, CreateContextPropertiesDictionary(null), null) ?? _contextProperties;

	/// <summary>
	/// Occurs when logger configuration changes.
	/// </summary>
	public event EventHandler<EventArgs>? LoggerReconfigured;

	/// <overloads>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public void ConditionalDebug<T>(T? value)
	{
		Debug(value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public void ConditionalDebug<T>(IFormatProvider? formatProvider, T? value)
	{
		Debug(formatProvider, value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	[Conditional("DEBUG")]
	public void ConditionalDebug(LogMessageGenerator messageFunc)
	{
		Debug(messageFunc);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Debug(exception, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Debug(exception, formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters and formatting them with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Debug(formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">Log message.</param>
	[Conditional("DEBUG")]
	public void ConditionalDebug([Localizable(false)] string message)
	{
		Debug(message);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Debug(message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		Debug(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		Debug(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified arguments formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		Debug(formatProvider, message, argument1, argument2);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		Debug(message, argument1, argument2);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified arguments formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		Debug(formatProvider, message, argument1, argument2, argument3);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		Debug(message, argument1, argument2, argument3);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ConditionalDebug(object? value)
	{
		this.Debug<object>(value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ConditionalDebug(IFormatProvider? formatProvider, object? value)
	{
		this.Debug<object>(formatProvider, value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		this.Debug<object, object>(message, arg1, arg2);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		this.Debug<object, object, object>(message, arg1, arg2, arg3);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		this.Debug<bool>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		this.Debug<bool>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		this.Debug<char>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		this.Debug<char>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		this.Debug<byte>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		this.Debug<byte>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		this.Debug<string>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		this.Debug<string>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		this.Debug<int>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		this.Debug<int>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		this.Debug<long>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		this.Debug<long>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		this.Debug<float>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		this.Debug<float>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		this.Debug<double>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		this.Debug<double>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		this.Debug<decimal>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		this.Debug<decimal>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		this.Debug<object>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalDebug([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		this.Debug<object>(message, argument);
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public void ConditionalTrace<T>(T? value)
	{
		Trace(value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	[Conditional("DEBUG")]
	public void ConditionalTrace<T>(IFormatProvider? formatProvider, T? value)
	{
		Trace(formatProvider, value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	[Conditional("DEBUG")]
	public void ConditionalTrace(LogMessageGenerator messageFunc)
	{
		Trace(messageFunc);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Trace(exception, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Trace(exception, formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters and formatting them with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Trace(formatProvider, message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">Log message.</param>
	[Conditional("DEBUG")]
	public void ConditionalTrace([Localizable(false)] string message)
	{
		Trace(message);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		Trace(message, args);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		Trace(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		Trace(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified arguments formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		Trace(formatProvider, message, argument1, argument2);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		Trace(message, argument1, argument2);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified arguments formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		Trace(formatProvider, message, argument1, argument2, argument3);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[Conditional("DEBUG")]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		Trace(message, argument1, argument2, argument3);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ConditionalTrace(object value)
	{
		this.Trace<object>(value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ConditionalTrace(IFormatProvider? formatProvider, object? value)
	{
		this.Trace<object>(formatProvider, value);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		this.Trace<object, object>(message, arg1, arg2);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		this.Trace<object, object, object>(message, arg1, arg2, arg3);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		this.Trace<bool>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		this.Trace<bool>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		this.Trace<char>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		this.Trace<char>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		this.Trace<byte>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		this.Trace<byte>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		this.Trace<string>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		this.Trace<string>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		this.Trace<int>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		this.Trace<int>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		this.Trace<long>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		this.Trace<long>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		this.Trace<float>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		this.Trace<float>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		this.Trace<double>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		this.Trace<double>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		this.Trace<decimal>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		this.Trace<decimal>(message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		this.Trace<object>(formatProvider, message, argument);
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// Only executed when the DEBUG conditional compilation symbol is set.</summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[Conditional("DEBUG")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void ConditionalTrace([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		this.Trace<object>(message, argument);
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	public void Trace<T>(T? value)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Trace<T>(IFormatProvider? formatProvider, T? value)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Trace(LogMessageGenerator messageFunc)
	{
		if (IsTraceEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(LogLevel.Trace, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public void Trace([Localizable(false)] string message)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, message);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	public void Trace(Exception? exception, [Localizable(false)] string message)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, exception, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	public void Debug<T>(T? value)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Debug<T>(IFormatProvider? formatProvider, T? value)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Debug(LogMessageGenerator messageFunc)
	{
		if (IsDebugEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(LogLevel.Debug, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public void Debug([Localizable(false)] string message)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, message);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	public void Debug(Exception? exception, [Localizable(false)] string message)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, exception, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	public void Info<T>(T? value)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Info<T>(IFormatProvider? formatProvider, T? value)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Info(LogMessageGenerator messageFunc)
	{
		if (IsInfoEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(LogLevel.Info, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public void Info([Localizable(false)] string message)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, message);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	public void Info(Exception? exception, [Localizable(false)] string message)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, exception, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	public void Warn<T>(T? value)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Warn<T>(IFormatProvider? formatProvider, T? value)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Warn(LogMessageGenerator messageFunc)
	{
		if (IsWarnEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(LogLevel.Warn, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public void Warn([Localizable(false)] string message)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, message);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	public void Warn(Exception? exception, [Localizable(false)] string message)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, exception, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	public void Error<T>(T? value)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Error<T>(IFormatProvider? formatProvider, T? value)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Error(LogMessageGenerator messageFunc)
	{
		if (IsErrorEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(LogLevel.Error, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public void Error([Localizable(false)] string message)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, message);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	public void Error(Exception? exception, [Localizable(false)] string message)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, exception, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	public void Fatal<T>(T? value)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Fatal<T>(IFormatProvider? formatProvider, T? value)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Fatal(LogMessageGenerator messageFunc)
	{
		if (IsFatalEnabled)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(LogLevel.Fatal, messageFunc());
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	public void Fatal([Localizable(false)] string message)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, message);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	public void Fatal(Exception? exception, [Localizable(false)] string message)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, exception, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified arguments formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <typeparam name="TArgument3">The type of the third argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	/// <param name="argument3">The third argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Log(LogLevel level, object? value)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Log(LogLevel level, IFormatProvider? formatProvider, object? value)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

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
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, string message, int argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified value as a parameter.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Trace(object? value)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Trace(IFormatProvider? formatProvider, object? value)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, string message, int argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Trace([Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsTraceEnabled)
		{
			WriteToTargets(LogLevel.Trace, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Debug(object? value)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Debug(IFormatProvider? formatProvider, object? value)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Debug([Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsDebugEnabled)
		{
			WriteToTargets(LogLevel.Debug, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Info(object? value)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Info(IFormatProvider? formatProvider, object? value)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Info([Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsInfoEnabled)
		{
			WriteToTargets(LogLevel.Info, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Warn(object? value)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Warn(IFormatProvider? formatProvider, object? value)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Warn([Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsWarnEnabled)
		{
			WriteToTargets(LogLevel.Warn, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Error(object? value)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Error(IFormatProvider? formatProvider, object? value)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Error([Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsErrorEnabled)
		{
			WriteToTargets(LogLevel.Error, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Fatal(object? value)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void Fatal(IFormatProvider? formatProvider, object? value)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[2] { arg1, arg2 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[3] { arg1, arg2, arg3 });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, bool argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, char argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, byte argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, string? argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, int argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, long argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, float argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, double argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, decimal argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, uint argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[CLSCompliant(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	public void Fatal([Localizable(false)][StructuredMessageTemplate] string message, ulong argument)
	{
		if (IsFatalEnabled)
		{
			WriteToTargets(LogLevel.Fatal, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

	void ILoggerBase.LogException(LogLevel level, [Localizable(false)] string message, Exception? exception)
	{
		Log(level, exception, message, ArrayHelper.Empty<object>());
	}

	void ILoggerBase.Log(LogLevel level, [Localizable(false)] string message, Exception? exception)
	{
		Log(level, exception, message, ArrayHelper.Empty<object>());
	}

	/// <inheritdoc />
	[Obsolete("Use Trace(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void TraceException([Localizable(false)] string message, Exception? exception)
	{
		Trace(exception, message);
	}

	void ILogger.Trace([Localizable(false)] string message, Exception? exception)
	{
		Trace(exception, message);
	}

	/// <inheritdoc />
	[Obsolete("Use Debug(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void DebugException([Localizable(false)] string message, Exception? exception)
	{
		Debug(exception, message);
	}

	void ILogger.Debug([Localizable(false)] string message, Exception? exception)
	{
		Debug(exception, message);
	}

	/// <inheritdoc />
	[Obsolete("Use Info(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void InfoException([Localizable(false)] string message, Exception? exception)
	{
		Info(exception, message);
	}

	void ILogger.Info([Localizable(false)] string message, Exception? exception)
	{
		Info(exception, message);
	}

	/// <inheritdoc />
	[Obsolete("Use Warn(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void WarnException([Localizable(false)] string message, Exception? exception)
	{
		Warn(exception, message);
	}

	void ILogger.Warn([Localizable(false)] string message, Exception? exception)
	{
		Warn(exception, message);
	}

	/// <inheritdoc />
	[Obsolete("Use Error(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ErrorException([Localizable(false)] string message, Exception? exception)
	{
		Error(exception, message);
	}

	void ILogger.Error([Localizable(false)] string message, Exception? exception)
	{
		Error(exception, message);
	}

	/// <inheritdoc />
	[Obsolete("Use Fatal(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void FatalException([Localizable(false)] string message, Exception? exception)
	{
		Fatal(exception, message);
	}

	void ILogger.Fatal([Localizable(false)] string message, Exception? exception)
	{
		Fatal(exception, message);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Logger" /> class.
	/// </summary>
	protected internal Logger()
	{
		_contextLogger = this;
		Name = string.Empty;
		Factory = LogManager.LogFactory;
	}

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the specified level.
	/// </summary>
	/// <param name="level">Log level to be checked.</param>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the specified level, otherwise it returns <see langword="false" />.</returns>
	public bool IsEnabled(LogLevel level)
	{
		return GetTargetsForLevelSafe(level) != null;
	}

	/// <summary>
	/// Creates new logger that automatically appends the specified property to all log events (without changing current logger)
	///
	/// With <see cref="P:NLog.Logger.Properties" /> property, all properties can be enumerated.
	/// </summary>
	/// <param name="propertyKey">Property Name</param>
	/// <param name="propertyValue">Property Value</param>
	/// <returns>New Logger object that automatically appends specified property</returns>
	public Logger WithProperty(string propertyKey, object? propertyValue)
	{
		if (string.IsNullOrEmpty(propertyKey))
		{
			throw new ArgumentException("propertyKey");
		}
		Logger logger = CreateChildLogger();
		logger.Properties[propertyKey] = propertyValue;
		return logger;
	}

	/// <summary>
	/// Creates new logger that automatically appends the specified properties to all log events (without changing current logger)
	///
	/// With <see cref="P:NLog.Logger.Properties" /> property, all properties can be enumerated.
	/// </summary>
	/// <param name="properties">Collection of key-value pair properties</param>
	/// <returns>New Logger object that automatically appends specified properties</returns>
	public Logger WithProperties(IEnumerable<KeyValuePair<string, object?>> properties)
	{
		Guard.ThrowIfNull(properties, "properties");
		Logger logger = CreateChildLogger();
		IDictionary<string, object> properties2 = logger.Properties;
		foreach (KeyValuePair<string, object> property in properties)
		{
			properties2[property.Key] = property.Value;
		}
		return logger;
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.Logger.WithProperty(System.String,System.Object)" /> that prevents unexpected side-effects in Logger-state.
	///
	/// Updates the specified context property for the current logger. The logger will append it for all log events.
	///
	/// With <see cref="P:NLog.Logger.Properties" /> property, all properties can be enumerated (or updated).
	/// </summary>
	/// <remarks>
	/// It is highly recommended to ONLY use <see cref="M:NLog.Logger.WithProperty(System.String,System.Object)" /> for modifying context properties.
	/// This method will affect all locations/contexts that makes use of the same named logger object. And can cause
	/// unexpected surprises at multiple locations and other thread contexts.
	/// </remarks>
	/// <param name="propertyKey">Property Name</param>
	/// <param name="propertyValue">Property Value</param>
	[Obsolete("Instead use WithProperty which is safe. If really necessary then one can use Properties-property. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetProperty(string propertyKey, object? propertyValue)
	{
		if (string.IsNullOrEmpty(propertyKey))
		{
			throw new ArgumentException("propertyKey");
		}
		Properties[propertyKey] = propertyValue;
	}

	private static ThreadSafeDictionary<string, object?> CreateContextPropertiesDictionary(ThreadSafeDictionary<string, object?>? contextProperties)
	{
		contextProperties = ((contextProperties == null) ? new ThreadSafeDictionary<string, object>() : new ThreadSafeDictionary<string, object>(contextProperties));
		return contextProperties;
	}

	/// <summary>
	/// Updates the <see cref="T:NLog.ScopeContext" /> with provided property
	/// </summary>
	/// <param name="propertyName">Name of property</param>
	/// <param name="propertyValue">Value of property</param>
	/// <returns>A disposable object that removes the properties from logical context scope on dispose.</returns>
	/// <remarks><see cref="T:NLog.ScopeContext" /> property-dictionary-keys are case-insensitive</remarks>
	public IDisposable PushScopeProperty(string propertyName, object? propertyValue)
	{
		return ScopeContext.PushProperty(propertyName, propertyValue);
	}

	/// <summary>
	/// Updates the <see cref="T:NLog.ScopeContext" /> with provided property
	/// </summary>
	/// <param name="propertyName">Name of property</param>
	/// <param name="propertyValue">Value of property</param>
	/// <returns>A disposable object that removes the properties from logical context scope on dispose.</returns>
	/// <remarks><see cref="T:NLog.ScopeContext" /> property-dictionary-keys are case-insensitive</remarks>
	public IDisposable PushScopeProperty<TValue>(string propertyName, TValue? propertyValue)
	{
		return ScopeContext.PushProperty(propertyName, propertyValue);
	}

	/// <summary>
	/// Updates the <see cref="T:NLog.ScopeContext" /> with provided properties
	/// </summary>
	/// <param name="scopeProperties">Properties being added to the scope dictionary</param>
	/// <returns>A disposable object that removes the properties from logical context scope on dispose.</returns>
	/// <remarks><see cref="T:NLog.ScopeContext" /> property-dictionary-keys are case-insensitive</remarks>
	public IDisposable PushScopeProperties(IReadOnlyCollection<KeyValuePair<string, object?>> scopeProperties)
	{
		return ScopeContext.PushProperties(scopeProperties);
	}

	/// <summary>
	/// Updates the <see cref="T:NLog.ScopeContext" /> with provided properties
	/// </summary>
	/// <param name="scopeProperties">Properties being added to the scope dictionary</param>
	/// <returns>A disposable object that removes the properties from logical context scope on dispose.</returns>
	/// <remarks><see cref="T:NLog.ScopeContext" /> property-dictionary-keys are case-insensitive</remarks>
	public IDisposable PushScopeProperties<TValue>(IReadOnlyCollection<KeyValuePair<string, TValue?>> scopeProperties)
	{
		return ScopeContext.PushProperties(scopeProperties);
	}

	/// <summary>
	/// Pushes new state on the logical context scope stack
	/// </summary>
	/// <param name="nestedState">Value to added to the scope stack</param>
	/// <returns>A disposable object that pops the nested scope state on dispose.</returns>
	public IDisposable PushScopeNested<T>(T nestedState)
	{
		return ScopeContext.PushNestedState(nestedState);
	}

	/// <summary>
	/// Pushes new state on the logical context scope stack
	/// </summary>
	/// <param name="nestedState">Value to added to the scope stack</param>
	/// <returns>A disposable object that pops the nested scope state on dispose.</returns>
	public IDisposable PushScopeNested(object nestedState)
	{
		return ScopeContext.PushNestedState(nestedState);
	}

	/// <summary>
	/// Writes the specified diagnostic message.
	/// </summary>
	/// <param name="logEvent">Log event.</param>
	public void Log(LogEventInfo logEvent)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(logEvent.Level);
		if (targetsForLevelSafe != null)
		{
			if (logEvent.LoggerName == null || (object)logEvent.LoggerName == string.Empty)
			{
				logEvent.LoggerName = Name;
			}
			if (logEvent.FormatProvider == null)
			{
				logEvent.FormatProvider = Factory.DefaultCultureInfo;
			}
			WriteLogEventToTargets(logEvent, targetsForLevelSafe);
		}
	}

	/// <summary>
	/// Writes the specified diagnostic message.
	/// </summary>
	/// <param name="wrapperType">Type of custom Logger wrapper.</param>
	/// <param name="logEvent">Log event.</param>
	public void Log(Type wrapperType, LogEventInfo logEvent)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(logEvent.Level);
		if (targetsForLevelSafe != null)
		{
			if (logEvent.LoggerName == null || (object)logEvent.LoggerName == string.Empty)
			{
				logEvent.LoggerName = Name;
			}
			if (logEvent.FormatProvider == null)
			{
				logEvent.FormatProvider = Factory.DefaultCultureInfo;
			}
			WriteLogEventToTargets(wrapperType, logEvent, targetsForLevelSafe);
		}
	}

	/// <overloads>
	/// Writes the diagnostic message at the specified level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="value">The value to be written.</param>
	public void Log<T>(LogLevel level, T? value)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, Factory.DefaultCultureInfo, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	public void Log<T>(LogLevel level, IFormatProvider? formatProvider, T? value)
	{
		if (IsEnabled(level))
		{
			WriteToTargets(level, formatProvider, value);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	public void Log(LogLevel level, LogMessageGenerator messageFunc)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			Guard.ThrowIfNull(messageFunc, "messageFunc");
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, messageFunc(), null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">Log message.</param>
	public void Log(LogLevel level, [Localizable(false)] string message)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, null);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameters.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, exception, Factory.DefaultCultureInfo, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message and exception at the specified level.
	/// </summary>
	/// <param name="level">The log level.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Log(LogLevel level, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, exception, formatProvider, message, args);
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Log<TArgument>(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, formatProvider, message, new object[1] { argument });
		}
	}

	/// <summary>
	/// Writes the diagnostic message at the specified level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="level">The log level.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	public void Log<TArgument>(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, new object[1] { argument });
		}
	}

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
	public void Log<TArgument1, TArgument2>(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, formatProvider, message, new object[2] { argument1, argument2 });
		}
	}

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
	public void Log<TArgument1, TArgument2>(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, new object[2] { argument1, argument2 });
		}
	}

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
	public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, formatProvider, message, new object[3] { argument1, argument2, argument3 });
		}
	}

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
	public void Log<TArgument1, TArgument2, TArgument3>(LogLevel level, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3)
	{
		ITargetWithFilterChain targetsForLevelSafe = GetTargetsForLevelSafe(level);
		if (targetsForLevelSafe != null)
		{
			WriteToTargets(targetsForLevelSafe, level, null, Factory.DefaultCultureInfo, message, new object[3] { argument1, argument2, argument3 });
		}
	}

	private LogEventInfo PrepareLogEventInfo(LogEventInfo logEvent)
	{
		if (_contextProperties != null)
		{
			foreach (KeyValuePair<string, object> contextProperty in _contextProperties)
			{
				if (!logEvent.Properties.ContainsKey(contextProperty.Key))
				{
					logEvent.Properties[contextProperty.Key] = contextProperty.Value;
				}
			}
		}
		return logEvent;
	}

	/// <summary>
	/// Runs the provided action. If the action throws, the exception is logged at <c>Error</c> level. The exception is not propagated outside of this method.
	/// </summary>
	/// <param name="action">Action to execute.</param>
	public void Swallow(Action action)
	{
		try
		{
			action();
		}
		catch (Exception value)
		{
			Error(value);
		}
	}

	/// <summary>
	/// Runs the provided function and returns its result. If an exception is thrown, it is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a default value is returned instead.
	/// </summary>
	/// <typeparam name="T">Return type of the provided function.</typeparam>
	/// <param name="func">Function to run.</param>
	/// <returns>Result returned by the provided function or the default value of type <typeparamref name="T" /> in case of exception.</returns>
	public T? Swallow<T>(Func<T?> func)
	{
		return Swallow(func, default(T));
	}

	/// <summary>
	/// Runs the provided function and returns its result. If an exception is thrown, it is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a fallback value is returned instead.
	/// </summary>
	/// <typeparam name="T">Return type of the provided function.</typeparam>
	/// <param name="func">Function to run.</param>
	/// <param name="fallback">Fallback value to return in case of exception.</param>
	/// <returns>Result returned by the provided function or fallback value in case of exception.</returns>
	public T? Swallow<T>(Func<T?> func, T? fallback)
	{
		try
		{
			return func();
		}
		catch (Exception value)
		{
			Error(value);
			return fallback;
		}
	}

	/// <summary>
	/// Logs an exception is logged at <c>Error</c> level if the provided task does not run to completion.
	/// </summary>
	/// <param name="task">The task for which to log an error if it does not run to completion.</param>
	/// <remarks>This method is useful in fire-and-forget situations, where application logic does not depend on completion of task. This method is avoids C# warning CS4014 in such situations.</remarks>
	public async void Swallow(Task task)
	{
		try
		{
			await task.ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception value)
		{
			Error(value);
		}
	}

	/// <summary>
	/// Returns a task that completes when a specified task to completes. If the task does not run to completion, an exception is logged at <c>Error</c> level. The returned task always runs to completion.
	/// </summary>
	/// <param name="task">The task for which to log an error if it does not run to completion.</param>
	/// <returns>A task that completes in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state when <paramref name="task" /> completes.</returns>
	public async Task SwallowAsync(Task task)
	{
		try
		{
			await task.ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception value)
		{
			Error(value);
		}
	}

	/// <summary>
	/// Runs async action. If the action throws, the exception is logged at <c>Error</c> level. The exception is not propagated outside of this method.
	/// </summary>
	/// <param name="asyncAction">Async action to execute.</param>
	public async Task SwallowAsync(Func<Task> asyncAction)
	{
		try
		{
			await asyncAction().ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception value)
		{
			Error(value);
		}
	}

	/// <summary>
	/// Runs the provided async function and returns its result. If the task does not run to completion, an exception is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a default value is returned instead.
	/// </summary>
	/// <typeparam name="TResult">Return type of the provided function.</typeparam>
	/// <param name="asyncFunc">Async function to run.</param>
	/// <returns>A task that represents the completion of the supplied task. If the supplied task ends in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state, the result of the new task will be the result of the supplied task; otherwise, the result of the new task will be the default value of type <typeparamref name="TResult" />.</returns>
	public async Task<TResult?> SwallowAsync<TResult>(Func<Task<TResult?>> asyncFunc)
	{
		return await SwallowAsync(asyncFunc, default(TResult)).ConfigureAwait(continueOnCapturedContext: false);
	}

	/// <summary>
	/// Runs the provided async function and returns its result. If the task does not run to completion, an exception is logged at <c>Error</c> level.
	/// The exception is not propagated outside of this method; a fallback value is returned instead.
	/// </summary>
	/// <typeparam name="TResult">Return type of the provided function.</typeparam>
	/// <param name="asyncFunc">Async function to run.</param>
	/// <param name="fallback">Fallback value to return if the task does not end in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state.</param>
	/// <returns>A task that represents the completion of the supplied task. If the supplied task ends in the <see cref="F:System.Threading.Tasks.TaskStatus.RanToCompletion" /> state, the result of the new task will be the result of the supplied task; otherwise, the result of the new task will be the fallback value.</returns>
	public async Task<TResult?> SwallowAsync<TResult>(Func<Task<TResult?>> asyncFunc, TResult? fallback)
	{
		try
		{
			return await asyncFunc().ConfigureAwait(continueOnCapturedContext: false);
		}
		catch (Exception value)
		{
			Error(value);
			return fallback;
		}
	}

	internal void Initialize(string name, ITargetWithFilterChain[] targetsByLevel, LogFactory factory)
	{
		Name = name;
		Factory = factory;
		SetConfiguration(targetsByLevel);
	}

	private void WriteToTargets(ITargetWithFilterChain targetsForLevel, LogLevel level, Exception? exception, IFormatProvider? formatProvider, string message, object?[]? args)
	{
		LogEventInfo logEvent = LogEventInfo.Create(level, Name, exception, formatProvider, message, args);
		WriteLogEventToTargets(logEvent, targetsForLevel);
	}

	private void WriteToTargets(LogLevel level, IFormatProvider? formatProvider, string message, object?[] args)
	{
		ITargetWithFilterChain targetsForLevel = GetTargetsForLevel(level);
		if (targetsForLevel != null)
		{
			LogEventInfo logEvent = LogEventInfo.Create(level, Name, formatProvider, message, args);
			WriteLogEventToTargets(logEvent, targetsForLevel);
		}
	}

	private void WriteToTargets(LogLevel level, string message)
	{
		ITargetWithFilterChain targetsForLevel = GetTargetsForLevel(level);
		if (targetsForLevel != null)
		{
			LogEventInfo logEvent = LogEventInfo.Create(level, Name, Factory.DefaultCultureInfo, message, null);
			WriteLogEventToTargets(logEvent, targetsForLevel);
		}
	}

	private void WriteToTargets<T>(LogLevel level, IFormatProvider? formatProvider, T? value)
	{
		ITargetWithFilterChain targetsForLevel = GetTargetsForLevel(level);
		if (targetsForLevel != null)
		{
			LogEventInfo logEvent = LogEventInfo.Create(level, Name, formatProvider, value);
			WriteLogEventToTargets(logEvent, targetsForLevel);
		}
	}

	private void WriteToTargets(LogLevel level, Exception? ex, IFormatProvider? formatProvider, string message, object?[]? args)
	{
		ITargetWithFilterChain targetsForLevel = GetTargetsForLevel(level);
		if (targetsForLevel != null)
		{
			LogEventInfo logEvent = LogEventInfo.Create(level, Name, ex, formatProvider, message, args);
			WriteLogEventToTargets(logEvent, targetsForLevel);
		}
	}

	private void WriteLogEventToTargets(LogEventInfo logEvent, ITargetWithFilterChain targetsForLevel)
	{
		try
		{
			targetsForLevel.WriteToLoggerTargets(DefaultLoggerType, PrepareLogEventInfo(logEvent), Factory);
		}
		catch (Exception ex)
		{
			if (Factory.ThrowExceptions || LogManager.ThrowExceptions)
			{
				throw;
			}
			InternalLogger.Error(ex, "Failed to write LogEvent");
		}
	}

	private void WriteLogEventToTargets(Type wrapperType, LogEventInfo logEvent, ITargetWithFilterChain targetsForLevel)
	{
		try
		{
			targetsForLevel.WriteToLoggerTargets(wrapperType ?? DefaultLoggerType, PrepareLogEventInfo(logEvent), Factory);
		}
		catch (Exception ex)
		{
			if (Factory.ThrowExceptions || LogManager.ThrowExceptions)
			{
				throw;
			}
			InternalLogger.Error(ex, "Failed to write LogEvent");
		}
	}

	internal void SetConfiguration(ITargetWithFilterChain[] targetsByLevel)
	{
		_targetsByLevel = targetsByLevel;
		_isTraceEnabled = IsEnabled(LogLevel.Trace);
		_isDebugEnabled = IsEnabled(LogLevel.Debug);
		_isInfoEnabled = IsEnabled(LogLevel.Info);
		_isWarnEnabled = IsEnabled(LogLevel.Warn);
		_isErrorEnabled = IsEnabled(LogLevel.Error);
		_isFatalEnabled = IsEnabled(LogLevel.Fatal);
		OnLoggerReconfigured(EventArgs.Empty);
	}

	private ITargetWithFilterChain GetTargetsForLevelSafe(LogLevel? level)
	{
		if ((object)level == null)
		{
			throw new InvalidOperationException("Log level must be defined");
		}
		return GetTargetsForLevel(level);
	}

	private ITargetWithFilterChain GetTargetsForLevel(LogLevel level)
	{
		if (_contextLogger == this)
		{
			return _targetsByLevel[level.Ordinal];
		}
		return _contextLogger.GetTargetsForLevel(level);
	}

	/// <summary>
	/// Raises the event when the logger is reconfigured.
	/// </summary>
	/// <param name="e">Event arguments</param>
	protected virtual void OnLoggerReconfigured(EventArgs e)
	{
		this.LoggerReconfigured?.Invoke(this, e);
	}

	private Logger CreateChildLogger()
	{
		Logger obj = (Logger)MemberwiseClone();
		obj.Initialize(Name, _targetsByLevel, Factory);
		obj._contextProperties = CreateContextPropertiesDictionary(_contextProperties);
		obj._contextLogger = _contextLogger;
		return obj;
	}
}

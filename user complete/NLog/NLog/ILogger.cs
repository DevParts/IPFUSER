using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace NLog;

/// <content>
/// Auto-generated Logger members for binary compatibility with NLog 1.0.
/// </content>
/// <summary>
/// Provides logging interface and utility functions.
/// </summary>
[CLSCompliant(false)]
public interface ILogger : ISuppress, ILoggerBase
{
	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Trace</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Trace</c> level, otherwise it returns <see langword="false" />.</returns>
	bool IsTraceEnabled { get; }

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Debug</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Debug</c> level, otherwise it returns <see langword="false" />.</returns>
	bool IsDebugEnabled { get; }

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Info</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Info</c> level, otherwise it returns <see langword="false" />.</returns>
	bool IsInfoEnabled { get; }

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Warn</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Warn</c> level, otherwise it returns <see langword="false" />.</returns>
	bool IsWarnEnabled { get; }

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Error</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Error</c> level, otherwise it returns <see langword="false" />.</returns>
	bool IsErrorEnabled { get; }

	/// <summary>
	/// Gets a value indicating whether logging is enabled for the <c>Fatal</c> level.
	/// </summary>
	/// <returns>A value of <see langword="true" /> if logging is enabled for the <c>Fatal</c> level, otherwise it returns <see langword="false" />.</returns>
	bool IsFatalEnabled { get; }

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Trace(object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Trace(IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>s
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Debug(object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Debug(IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Info(object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Info(IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Warn(object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Warn(IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Error(object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Error(IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, object argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Fatal(object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">A <see langword="object" /> to be written.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Fatal(IFormatProvider? formatProvider, object? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="arg1">First argument to format.</param>
	/// <param name="arg2">Second argument to format.</param>
	/// <param name="arg3">Third argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? arg1, object? arg2, object? arg3);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, bool argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, char argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, byte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, string? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, int argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, long argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, float argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, double argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, decimal argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, object? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, sbyte argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, uint argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified value as a parameter.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[EditorBrowsable(EditorBrowsableState.Never)]
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, ulong argument);

	/// <overloads>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	void Trace<T>(T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Trace<T>(IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Trace(LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	void Trace(Exception? exception, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	void Trace([Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Trace(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Trace(Exception exception, string message) method instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Trace([Localizable(false)] string message, Exception? exception);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

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
	void Trace<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Trace<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

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
	void Trace<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

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
	void Trace<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Trace(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Trace</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Trace(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void TraceException([Localizable(false)] string message, Exception? exception);

	/// <overloads>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	void Debug<T>(T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Debug<T>(IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Debug(LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	void Debug(Exception? exception, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	void Debug([Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

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
	void Debug<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Debug<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

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
	void Debug<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

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
	void Debug<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Debug(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Debug(Exception exception, string message) method instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Debug([Localizable(false)] string message, Exception exception);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Debug(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Debug</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Debug(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void DebugException([Localizable(false)] string message, Exception? exception);

	/// <overloads>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	void Info<T>(T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Info<T>(IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Info(LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	void Info(Exception? exception, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	void Info([Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument argument);

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
	void Info<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Info<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

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
	void Info<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

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
	void Info<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Info(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Info(Exception exception, string message) method instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Info([Localizable(false)] string message, Exception? exception);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Info(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Info</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Info(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void InfoException([Localizable(false)] string message, Exception? exception);

	/// <overloads>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	void Warn<T>(T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Warn<T>(IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Warn(LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	void Warn(Exception? exception, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	void Warn([Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

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
	void Warn<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Warn<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

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
	void Warn<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

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
	void Warn<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Warn(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Warn(Exception exception, string message) method instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Warn([Localizable(false)] string message, Exception? exception);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Warn(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Warn</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Warn(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void WarnException([Localizable(false)] string message, Exception? exception);

	/// <overloads>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	void Error<T>(T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Error<T>(IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Error(LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	void Error(Exception? exception, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	void Error([Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

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
	void Error<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Error</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Error<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

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
	void Error<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

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
	void Error<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Error(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Error(Exception exception, string message) method instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Error([Localizable(false)] string message, Exception? exception);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Error(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Error</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Error(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void ErrorException([Localizable(false)] string message, Exception? exception);

	/// <overloads>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified format provider and format parameters.
	/// </overloads>
	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="value">The value to be written.</param>
	void Fatal<T>(T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <typeparam name="T">Type of the value.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="value">The value to be written.</param>
	void Fatal<T>(IFormatProvider? formatProvider, T? value);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="messageFunc">A function returning message to be written. Function is not evaluated if logging is not enabled.</param>
	void Fatal(LogMessageGenerator messageFunc);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	void Fatal(Exception? exception, [Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal(Exception? exception, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal(Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters and formatting them with the supplied format provider.
	/// </summary>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">Log message.</param>
	void Fatal([Localizable(false)] string message);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <param name="message">A <see langword="string" /> containing format items.</param>
	/// <param name="args">Arguments to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal([Localizable(false)][StructuredMessageTemplate] string message, params object?[] args);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameter and formatting it with the supplied format provider.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal<TArgument>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameter.
	/// </summary>
	/// <typeparam name="TArgument">The type of the argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument">The argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal<TArgument>([Localizable(false)][StructuredMessageTemplate] string message, TArgument? argument);

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
	void Fatal<TArgument1, TArgument2>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

	/// <summary>
	/// Writes the diagnostic message at the <c>Fatal</c> level using the specified parameters.
	/// </summary>
	/// <typeparam name="TArgument1">The type of the first argument.</typeparam>
	/// <typeparam name="TArgument2">The type of the second argument.</typeparam>
	/// <param name="message">A <see langword="string" /> containing one format item.</param>
	/// <param name="argument1">The first argument to format.</param>
	/// <param name="argument2">The second argument to format.</param>
	[MessageTemplateFormatMethod("message")]
	void Fatal<TArgument1, TArgument2>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2);

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
	void Fatal<TArgument1, TArgument2, TArgument3>(IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

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
	void Fatal<TArgument1, TArgument2, TArgument3>([Localizable(false)][StructuredMessageTemplate] string message, TArgument1? argument1, TArgument2? argument2, TArgument3? argument3);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Fatal(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Fatal(Exception exception, string message) method instead. Marked obsolete with v4.3.11")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void Fatal([Localizable(false)] string message, Exception? exception);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.ILogger.Fatal(System.Exception,System.String)" /> - Writes the diagnostic message and exception at the <c>Fatal</c> level.
	/// </summary>
	/// <param name="message">A <see langword="string" /> to be written.</param>
	/// <param name="exception">An exception to be logged.</param>
	/// <remarks>This method was marked as obsolete before NLog 4.3.11 and it may be removed in a future release.</remarks>
	[Obsolete("Use Fatal(Exception exception, string message) method instead. Marked obsolete with v4.3.11 (Only here because of LibLog)")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void FatalException([Localizable(false)] string message, Exception? exception);
}

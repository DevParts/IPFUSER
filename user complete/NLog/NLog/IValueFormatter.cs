using System;
using System.Text;
using NLog.MessageTemplates;

namespace NLog;

/// <summary>
/// Render a message template property to a string
/// </summary>
public interface IValueFormatter
{
	/// <summary>
	/// Serialization of an object, e.g. JSON and append to <paramref name="builder" />
	/// </summary>
	/// <param name="value">The object to serialize to string.</param>
	/// <param name="format">Parameter Format</param>
	/// <param name="captureType">Parameter CaptureType</param>
	/// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
	/// <param name="builder">Output destination.</param>
	/// <returns>Serialize succeeded (true/false)</returns>
	bool FormatValue(object? value, string? format, CaptureType captureType, IFormatProvider? formatProvider, StringBuilder builder);
}

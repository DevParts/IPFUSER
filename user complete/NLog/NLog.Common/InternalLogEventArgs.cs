using System;

namespace NLog.Common;

/// <summary>
/// Internal LogEvent details from <see cref="T:NLog.Common.InternalLogger" />
/// </summary>
public struct InternalLogEventArgs
{
	/// <summary>
	/// The rendered message
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// The log level
	/// </summary>
	public LogLevel Level { get; }

	/// <summary>
	/// The exception. Could be null.
	/// </summary>
	public Exception? Exception { get; }

	/// <summary>
	/// The type that triggered this internal log event, for example the FileTarget.
	/// This property is not always populated.
	/// </summary>
	public Type? SenderType { get; }

	/// <summary>
	/// The context name that triggered this internal log event, for example the name of the Target.
	/// This property is not always populated.
	/// </summary>
	public string? SenderName { get; }

	internal InternalLogEventArgs(string message, LogLevel level, Exception? exception, Type? senderType, string? senderName)
	{
		Message = message;
		Level = level;
		Exception = exception;
		SenderType = senderType;
		SenderName = senderName;
	}
}

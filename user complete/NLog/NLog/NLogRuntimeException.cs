using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NLog;

/// <summary>
/// Exception thrown during log event processing.
/// </summary>
[Serializable]
public class NLogRuntimeException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogRuntimeException" /> class.
	/// </summary>
	public NLogRuntimeException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogRuntimeException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public NLogRuntimeException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Obsolete and replaced by using normal string-interpolation with <see cref="M:NLog.NLogRuntimeException.#ctor(System.String)" /> in NLog v5.
	/// Initializes a new instance of the <see cref="T:NLog.NLogRuntimeException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="messageParameters">Parameters for the message</param>
	[Obsolete("Instead use string interpolation. Marked obsolete with NLog 5.0")]
	[StringFormatMethod("message")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public NLogRuntimeException(string message, params object?[] messageParameters)
		: base(string.Format(message, messageParameters))
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogRuntimeException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public NLogRuntimeException(string message, Exception? innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogRuntimeException" /> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
	/// <exception cref="T:System.ArgumentNullException">
	/// The <paramref name="info" /> parameter is null.
	/// </exception>
	/// <exception cref="T:System.Runtime.Serialization.SerializationException">
	/// The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).
	/// </exception>
	protected NLogRuntimeException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

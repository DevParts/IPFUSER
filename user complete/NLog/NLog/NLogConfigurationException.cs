using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NLog;

/// <summary>
/// Exception thrown during NLog configuration.
/// </summary>
[Serializable]
public class NLogConfigurationException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogConfigurationException" /> class.
	/// </summary>
	public NLogConfigurationException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogConfigurationException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public NLogConfigurationException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Obsolete and replaced by using normal string-interpolation with <see cref="M:NLog.NLogConfigurationException.#ctor(System.String)" /> in NLog v5.
	/// Initializes a new instance of the <see cref="T:NLog.NLogConfigurationException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="messageParameters">Parameters for the message</param>
	[Obsolete("Instead use string interpolation. Marked obsolete with NLog 5.0")]
	[StringFormatMethod("message")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public NLogConfigurationException(string message, params object?[] messageParameters)
		: base(string.Format(message, messageParameters))
	{
	}

	/// <summary>
	/// Obsolete and replaced by using normal string-interpolation with <see cref="M:NLog.NLogConfigurationException.#ctor(System.String)" /> in NLog v5.
	/// Initializes a new instance of the <see cref="T:NLog.NLogConfigurationException" /> class.
	/// </summary>
	/// <param name="innerException">The inner exception.</param>
	/// <param name="message">The message.</param>
	/// <param name="messageParameters">Parameters for the message</param>
	[Obsolete("Instead use string interpolation. Marked obsolete with NLog 5.0")]
	[StringFormatMethod("message")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public NLogConfigurationException(Exception? innerException, string message, params object?[] messageParameters)
		: base(string.Format(message, messageParameters), innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogConfigurationException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public NLogConfigurationException(string message, Exception? innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.NLogConfigurationException" /> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
	/// <exception cref="T:System.ArgumentNullException">
	/// The <paramref name="info" /> parameter is null.
	/// </exception>
	/// <exception cref="T:System.Runtime.Serialization.SerializationException">
	/// The class name is null or <see cref="P:System.Exception.HResult" /> is zero (0).
	/// </exception>
	protected NLogConfigurationException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}

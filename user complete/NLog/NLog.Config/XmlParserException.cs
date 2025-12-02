using System;

namespace NLog.Config;

/// <summary>
/// Exception thrown during XML parsing
/// </summary>
public sealed class XmlParserException : NLogConfigurationException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlParserException" /> class.
	/// </summary>
	public XmlParserException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlParserException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public XmlParserException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlParserException" /> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public XmlParserException(string message, Exception? innerException)
		: base(message, innerException)
	{
	}
}

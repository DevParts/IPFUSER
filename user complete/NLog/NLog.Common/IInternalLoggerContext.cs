namespace NLog.Common;

/// <summary>
/// Enables <see cref="T:NLog.Common.InternalLogger" /> to extract extra context details for <see cref="E:NLog.Common.InternalLogger.InternalEventOccurred" />
/// </summary>
internal interface IInternalLoggerContext
{
	/// <summary>
	/// Name of context
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The current LogFactory next to LogManager
	/// </summary>
	LogFactory? LogFactory { get; }
}

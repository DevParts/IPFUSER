namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of LogFactory options for enabling NLog <see cref="T:NLog.Common.InternalLogger" />
/// </summary>
public interface ISetupInternalLoggerBuilder
{
	/// <summary>
	/// LogFactory under configuration
	/// </summary>
	LogFactory LogFactory { get; }
}

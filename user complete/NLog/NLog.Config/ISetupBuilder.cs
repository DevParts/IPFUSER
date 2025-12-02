namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of LogFactory options
/// </summary>
public interface ISetupBuilder
{
	/// <summary>
	/// LogFactory under configuration
	/// </summary>
	LogFactory LogFactory { get; }
}

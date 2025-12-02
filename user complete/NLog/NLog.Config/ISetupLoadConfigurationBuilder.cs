namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of LoggingConfiguration for LogFactory
/// </summary>
public interface ISetupLoadConfigurationBuilder
{
	/// <summary>
	/// LogFactory under configuration
	/// </summary>
	LogFactory LogFactory { get; }

	/// <summary>
	/// LoggingConfiguration being built
	/// </summary>
	LoggingConfiguration Configuration { get; set; }
}

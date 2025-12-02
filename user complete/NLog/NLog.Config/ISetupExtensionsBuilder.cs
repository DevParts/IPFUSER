namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of LogFactory options for extension loading
/// </summary>
public interface ISetupExtensionsBuilder
{
	/// <summary>
	/// LogFactory under configuration
	/// </summary>
	LogFactory LogFactory { get; }
}

namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of LogFactory options for logevent serialization
/// </summary>
public interface ISetupSerializationBuilder
{
	/// <summary>
	/// LogFactory under configuration
	/// </summary>
	LogFactory LogFactory { get; }
}

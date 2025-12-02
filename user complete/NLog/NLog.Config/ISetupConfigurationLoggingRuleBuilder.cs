namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of LoggingRules for LoggingConfiguration
/// </summary>
public interface ISetupConfigurationLoggingRuleBuilder : ISetupConfigurationTargetBuilder
{
	/// <summary>
	/// LoggingRule being built
	/// </summary>
	LoggingRule LoggingRule { get; }
}

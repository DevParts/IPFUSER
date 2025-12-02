using System;

namespace NLog.Config;

/// <summary>
/// Arguments for <see cref="E:NLog.LogFactory.ConfigurationChanged" /> events.
/// </summary>
public class LoggingConfigurationChangedEventArgs : EventArgs
{
	/// <summary>
	/// Gets the old configuration.
	/// </summary>
	/// <value>The old configuration.</value>
	public LoggingConfiguration? DeactivatedConfiguration { get; }

	/// <summary>
	/// Gets the new configuration.
	/// </summary>
	/// <remarks>
	/// New value can be <c>null</c> when unloading configuration during shutdown.
	/// </remarks>
	/// <value>The new configuration.</value>
	public LoggingConfiguration? ActivatedConfiguration { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.LoggingConfigurationChangedEventArgs" /> class.
	/// </summary>
	/// <param name="activatedConfiguration">The new configuration.</param>
	/// <param name="deactivatedConfiguration">The old configuration.</param>
	public LoggingConfigurationChangedEventArgs(LoggingConfiguration? activatedConfiguration, LoggingConfiguration? deactivatedConfiguration)
	{
		ActivatedConfiguration = activatedConfiguration;
		DeactivatedConfiguration = deactivatedConfiguration;
	}
}

using System;
using System.Configuration;
using System.Xml;
using NLog.Common;
using NLog.Internal;

namespace NLog.Config;

/// <summary>
/// Represents NLog ConfigSection for loading <see cref="T:NLog.Config.XmlLoggingConfiguration" /> from app.config / web.config
/// </summary>
/// <remarks><code>
/// &lt;configSections&gt;
///   &lt;section name="NLog" type="NLog.Config.ConfigSectionHandler, NLog" /&gt;
/// &lt;/configSections&gt;
/// </code></remarks>
public sealed class ConfigSectionHandler : ConfigurationSection
{
	private XmlLoggingConfiguration? _config;

	/// <summary>
	/// Gets the default <see cref="T:NLog.Config.LoggingConfiguration" /> object by parsing
	/// the application configuration file (<c>app.exe.config</c>).
	/// </summary>
	public static LoggingConfiguration? AppConfig
	{
		get
		{
			try
			{
				return System.Configuration.ConfigurationManager.GetSection("nlog") as LoggingConfiguration;
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "Failed loading XML configuration from NLog ConfigSection in application configuration file (app.config / web.config)");
				if (ex.MustBeRethrown())
				{
					throw;
				}
			}
			return null;
		}
	}

	/// <summary>
	/// Overriding base implementation to just store <see cref="T:System.Xml.XmlReader" />
	/// of the relevant app.config section.
	/// </summary>
	/// <param name="reader">The XmlReader that reads from the configuration file.</param>
	/// <param name="serializeCollectionKey"><see langword="true" /> to serialize only the collection key properties; otherwise, <see langword="false" />.</param>
	protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
	{
		try
		{
			string appDomainConfigurationFile = LogFactory.DefaultAppEnvironment.AppDomainConfigurationFile;
			_config = new XmlLoggingConfiguration(reader, appDomainConfigurationFile, LogManager.LogFactory);
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "Failed loading XML configuration from NLog ConfigSection in app.config");
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
	}

	/// <summary>
	/// Override base implementation to return a <see cref="T:NLog.Config.LoggingConfiguration" /> object
	/// for <see cref="M:System.Configuration.ConfigurationManager.GetSection(System.String)" />
	/// instead of the <see cref="T:NLog.Config.ConfigSectionHandler" /> instance.
	/// </summary>
	/// <returns>
	/// A <see cref="T:NLog.Config.LoggingConfiguration" /> instance, that has been deserialized from app.config.
	/// </returns>
	protected override object? GetRuntimeObject()
	{
		return _config;
	}
}

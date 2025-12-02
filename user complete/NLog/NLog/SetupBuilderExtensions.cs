using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog;

/// <summary>
/// Extension methods to setup LogFactory options
/// </summary>
public static class SetupBuilderExtensions
{
	/// <summary>
	/// Gets the logger with the full name of the current class, so namespace and class name.
	/// </summary>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Logger GetCurrentClassLogger(this ISetupBuilder setupBuilder)
	{
		string classFullName = StackTraceUsageUtils.GetClassFullName(new StackFrame(1, fNeedFileInfo: false));
		return setupBuilder.LogFactory.GetLogger(classFullName);
	}

	/// <summary>
	/// Gets the specified named logger.
	/// </summary>
	public static Logger GetLogger(this ISetupBuilder setupBuilder, string name)
	{
		return setupBuilder.LogFactory.GetLogger(name);
	}

	/// <summary>
	/// Configures general options for NLog LogFactory before loading NLog config
	/// </summary>
	public static ISetupBuilder SetupLogFactory(this ISetupBuilder setupBuilder, Action<ISetupLogFactoryBuilder> logfactoryBuilder)
	{
		logfactoryBuilder(new SetupLogFactoryBuilder(setupBuilder.LogFactory));
		return setupBuilder;
	}

	/// <summary>
	/// Configures loading of NLog extensions for Targets and LayoutRenderers
	/// </summary>
	public static ISetupBuilder SetupExtensions(this ISetupBuilder setupBuilder, Action<ISetupExtensionsBuilder> extensionsBuilder)
	{
		extensionsBuilder(new SetupExtensionsBuilder(setupBuilder.LogFactory));
		return setupBuilder;
	}

	/// <summary>
	/// Configures the output of NLog <see cref="T:NLog.Common.InternalLogger" /> for diagnostics / troubleshooting
	/// </summary>
	public static ISetupBuilder SetupInternalLogger(this ISetupBuilder setupBuilder, Action<ISetupInternalLoggerBuilder> internalLoggerBuilder)
	{
		internalLoggerBuilder(new SetupInternalLoggerBuilder(setupBuilder.LogFactory));
		return setupBuilder;
	}

	/// <summary>
	/// Configures serialization and transformation of LogEvents
	/// </summary>
	public static ISetupBuilder SetupSerialization(this ISetupBuilder setupBuilder, Action<ISetupSerializationBuilder> serializationBuilder)
	{
		serializationBuilder(new SetupSerializationBuilder(setupBuilder.LogFactory));
		return setupBuilder;
	}

	/// <summary>
	/// Loads NLog config created by the method <paramref name="configBuilder" />
	/// </summary>
	public static ISetupBuilder LoadConfiguration(this ISetupBuilder setupBuilder, Action<ISetupLoadConfigurationBuilder> configBuilder)
	{
		LoggingConfiguration config = setupBuilder.LogFactory._config;
		SetupLoadConfigurationBuilder setupLoadConfigurationBuilder = new SetupLoadConfigurationBuilder(setupBuilder.LogFactory, config);
		configBuilder(setupLoadConfigurationBuilder);
		LoggingConfiguration configuration = setupLoadConfigurationBuilder._configuration;
		bool flag = config != setupBuilder.LogFactory._config;
		if (configuration == setupBuilder.LogFactory._config)
		{
			if (config != configuration || configuration != null)
			{
				setupBuilder.LogFactory.ReconfigExistingLoggers();
			}
		}
		else if (!flag || config != configuration)
		{
			setupBuilder.LogFactory.Configuration = configuration;
		}
		return setupBuilder;
	}

	/// <summary>
	/// Loads NLog config provided in <paramref name="loggingConfiguration" />
	/// </summary>
	public static ISetupBuilder LoadConfiguration(this ISetupBuilder setupBuilder, LoggingConfiguration loggingConfiguration)
	{
		setupBuilder.LogFactory.Configuration = loggingConfiguration;
		return setupBuilder;
	}

	/// <summary>
	/// Loads NLog config from filename <paramref name="configFile" /> if provided, else fallback to scanning for NLog.config
	/// </summary>
	/// <param name="setupBuilder">Fluent interface parameter.</param>
	/// <param name="configFile">Explicit configuration file to be read (Default NLog.config from candidates paths)</param>
	/// <param name="optional">Whether to allow application to run when NLog config is not available</param>
	public static ISetupBuilder LoadConfigurationFromFile(this ISetupBuilder setupBuilder, string? configFile = null, bool optional = true)
	{
		setupBuilder.LogFactory.LoadConfiguration(configFile, optional);
		return setupBuilder;
	}

	/// <summary>
	/// Loads NLog config from file-paths <paramref name="candidateFilePaths" /> if provided, else fallback to scanning for NLog.config
	/// </summary>
	/// <param name="setupBuilder">Fluent interface parameter.</param>
	/// <param name="candidateFilePaths">Candidates file paths (including filename) where to scan for NLog config files</param>
	/// <param name="optional">Whether to allow application to run when NLog config is not available</param>
	public static ISetupBuilder LoadConfigurationFromFile(this ISetupBuilder setupBuilder, IEnumerable<string> candidateFilePaths, bool optional = true)
	{
		if (candidateFilePaths == null)
		{
			setupBuilder.LogFactory.ResetCandidateConfigFilePath();
		}
		else if (optional)
		{
			IEnumerable<string> candidateConfigFilePaths = setupBuilder.LogFactory.GetCandidateConfigFilePaths();
			List<string> list = new List<string>(candidateFilePaths);
			HashSet<string> hashSet = new HashSet<string>(list, StringComparer.OrdinalIgnoreCase);
			foreach (string item in candidateConfigFilePaths)
			{
				if (hashSet.Add(item))
				{
					list.Add(item);
				}
			}
			setupBuilder.LogFactory.SetCandidateConfigFilePaths(list);
		}
		else
		{
			setupBuilder.LogFactory.SetCandidateConfigFilePaths(candidateFilePaths);
		}
		return setupBuilder.LoadConfigurationFromFile((string?)null, optional);
	}

	/// <summary>
	/// Loads NLog config from XML in <paramref name="configXml" />
	/// </summary>
	public static ISetupBuilder LoadConfigurationFromXml(this ISetupBuilder setupBuilder, string configXml)
	{
		setupBuilder.LogFactory.Configuration = XmlLoggingConfiguration.CreateFromXmlString(configXml, setupBuilder.LogFactory);
		return setupBuilder;
	}

	/// <summary>
	/// Loads NLog config located in embedded resource from main application assembly.
	/// </summary>
	/// <param name="setupBuilder">Fluent interface parameter.</param>
	/// <param name="applicationAssembly">Assembly for the main Application project with embedded resource</param>
	/// <param name="resourceName">Name of the manifest resource for NLog config XML</param>
	public static ISetupBuilder LoadConfigurationFromAssemblyResource(this ISetupBuilder setupBuilder, Assembly applicationAssembly, string resourceName = "NLog.config")
	{
		Guard.ThrowIfNull(applicationAssembly, "applicationAssembly");
		Guard.ThrowIfNullOrEmpty(resourceName, "resourceName");
		List<string> list = (from x in applicationAssembly.GetManifestResourceNames()
			where x.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase)
			select x).ToList();
		if (list.Count == 1)
		{
			Stream manifestResourceStream = applicationAssembly.GetManifestResourceStream(list[0]);
			if (manifestResourceStream != null && manifestResourceStream.Length > 0)
			{
				InternalLogger.Info("Loading NLog XML config from assembly embedded resource '{0}'", resourceName);
				using StreamReader xmlSource = new StreamReader(manifestResourceStream);
				setupBuilder.LoadConfiguration(new XmlLoggingConfiguration(xmlSource, null, setupBuilder.LogFactory));
			}
			else
			{
				InternalLogger.Debug("No NLog config loaded. Empty Embedded resource '{0}' found in assembly: {1}", resourceName, applicationAssembly.FullName);
			}
		}
		else if (list.Count == 0)
		{
			InternalLogger.Debug("No NLog config loaded. No matching embedded resource '{0}' found in assembly: {1}", resourceName, applicationAssembly.FullName);
		}
		else
		{
			InternalLogger.Error("No NLog config loaded. Multiple matching embedded resource '{0}' found in assembly: {1}", resourceName, applicationAssembly.FullName);
		}
		return setupBuilder;
	}

	/// <summary>
	/// Reloads the current logging configuration and activates it
	/// </summary>
	/// <remarks>Logevents produced during the configuration-reload can become lost, as targets are unavailable while closing and initializing.</remarks>
	public static ISetupBuilder ReloadConfiguration(this ISetupBuilder setupBuilder)
	{
		try
		{
			InternalLogger.Debug("Reloading NLog LoggingConfiguration");
			LoggingConfiguration loggingConfiguration = setupBuilder.LogFactory._config?.Reload();
			if (loggingConfiguration == null)
			{
				return setupBuilder;
			}
			setupBuilder.LogFactory.Configuration = loggingConfiguration;
			return setupBuilder;
		}
		catch (NLogConfigurationException ex)
		{
			InternalLogger.Error(ex, "Failed to reload NLog LoggingConfiguration");
			if (ex.MustBeRethrown() || (setupBuilder.LogFactory.ThrowConfigExceptions ?? setupBuilder.LogFactory.ThrowExceptions))
			{
				throw;
			}
			return setupBuilder;
		}
		catch (Exception ex2)
		{
			InternalLogger.Error(ex2, "Failed to reload NLog LoggingConfiguration");
			if (ex2.MustBeRethrownImmediately())
			{
				throw;
			}
			return setupBuilder;
		}
	}
}

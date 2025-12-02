using System;
using System.Globalization;
using System.Reflection;
using NLog.Config;
using NLog.Internal;
using NLog.Time;

namespace NLog;

/// <summary>
/// Extension methods to setup general option before loading NLog LoggingConfiguration
/// </summary>
public static class SetupLogFactoryBuilderExtensions
{
	/// <summary>
	/// Configures the global time-source used for all logevents
	/// </summary>
	/// <remarks>
	/// Available by default: <see cref="T:NLog.Time.AccurateLocalTimeSource" />, <see cref="T:NLog.Time.AccurateUtcTimeSource" />, <see cref="T:NLog.Time.FastLocalTimeSource" />, <see cref="T:NLog.Time.FastUtcTimeSource" />
	/// </remarks>
	public static ISetupLogFactoryBuilder SetTimeSource(this ISetupLogFactoryBuilder configBuilder, TimeSource timeSource)
	{
		TimeSource.Current = timeSource;
		return configBuilder;
	}

	/// <summary>
	/// Configures the global time-source used for all logevents to use <see cref="T:NLog.Time.AccurateUtcTimeSource" />
	/// </summary>
	public static ISetupLogFactoryBuilder SetTimeSourcAccurateUtc(this ISetupLogFactoryBuilder configBuilder)
	{
		return configBuilder.SetTimeSource(TimeSource.Current);
	}

	/// <summary>
	/// Configures the global time-source used for all logevents to use <see cref="T:NLog.Time.AccurateLocalTimeSource" />
	/// </summary>
	public static ISetupLogFactoryBuilder SetTimeSourcAccurateLocal(this ISetupLogFactoryBuilder configBuilder)
	{
		return configBuilder.SetTimeSource(TimeSource.Current);
	}

	/// <summary>
	/// Updates the dictionary <see cref="T:NLog.GlobalDiagnosticsContext" /> ${gdc:item=} with the name-value-pair
	/// </summary>
	public static ISetupLogFactoryBuilder SetGlobalContextProperty(this ISetupLogFactoryBuilder configBuilder, string name, object value)
	{
		GlobalDiagnosticsContext.Set(name, value);
		return configBuilder;
	}

	/// <summary>
	/// Sets whether to automatically call <see cref="M:NLog.LogFactory.Shutdown" /> on AppDomain.Unload or AppDomain.ProcessExit
	/// </summary>
	public static ISetupLogFactoryBuilder SetAutoShutdown(this ISetupLogFactoryBuilder configBuilder, bool enabled)
	{
		configBuilder.LogFactory.AutoShutdown = enabled;
		return configBuilder;
	}

	/// <summary>
	/// Sets the default culture info to use as <see cref="P:NLog.LogEventInfo.FormatProvider" />.
	/// </summary>
	public static ISetupLogFactoryBuilder SetDefaultCultureInfo(this ISetupLogFactoryBuilder configBuilder, CultureInfo cultureInfo)
	{
		configBuilder.LogFactory.DefaultCultureInfo = cultureInfo;
		return configBuilder;
	}

	/// <summary>
	/// Sets the global log level threshold. Log events below this threshold are not logged.
	/// </summary>
	public static ISetupLogFactoryBuilder SetGlobalThreshold(this ISetupLogFactoryBuilder configBuilder, LogLevel logLevel)
	{
		configBuilder.LogFactory.GlobalThreshold = logLevel;
		return configBuilder;
	}

	/// <summary>
	/// Gets or sets a value indicating whether <see cref="T:NLog.NLogConfigurationException" /> should be thrown on configuration errors
	/// </summary>
	public static ISetupLogFactoryBuilder SetThrowConfigExceptions(this ISetupLogFactoryBuilder configBuilder, bool enabled)
	{
		configBuilder.LogFactory.ThrowConfigExceptions = enabled;
		return configBuilder;
	}

	/// <summary>
	/// Mark Assembly as hidden, so Assembly methods are excluded when resolving ${callsite} from StackTrace
	/// </summary>
	public static ISetupLogFactoryBuilder AddCallSiteHiddenAssembly(this ISetupLogFactoryBuilder configBuilder, Assembly assembly)
	{
		CallSiteInformation.AddCallSiteHiddenAssembly(assembly);
		return configBuilder;
	}

	/// <summary>
	/// Mark Type as hidden, so Type methods are excluded when resolving ${callsite} from StackTrace
	/// </summary>
	public static ISetupLogFactoryBuilder AddCallSiteHiddenClassType(this ISetupLogFactoryBuilder configBuilder, Type type)
	{
		CallSiteInformation.AddCallSiteHiddenClassType(type);
		return configBuilder;
	}
}

using NLog.Config;

namespace NLog.Internal;

internal sealed class SetupLoadConfigurationBuilder : ISetupLoadConfigurationBuilder
{
	internal LoggingConfiguration? _configuration;

	public LogFactory LogFactory { get; }

	public LoggingConfiguration Configuration
	{
		get
		{
			return _configuration ?? (_configuration = new LoggingConfiguration(LogFactory));
		}
		set
		{
			_configuration = value;
		}
	}

	internal SetupLoadConfigurationBuilder(LogFactory logFactory, LoggingConfiguration? configuration)
	{
		LogFactory = logFactory;
		_configuration = configuration;
	}
}

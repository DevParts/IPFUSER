using NLog.Config;

namespace NLog.Internal;

internal sealed class SetupInternalLoggerBuilder : ISetupInternalLoggerBuilder
{
	public LogFactory LogFactory { get; }

	internal SetupInternalLoggerBuilder(LogFactory logFactory)
	{
		LogFactory = logFactory;
	}
}

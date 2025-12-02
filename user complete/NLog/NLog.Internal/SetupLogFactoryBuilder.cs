using NLog.Config;

namespace NLog.Internal;

internal sealed class SetupLogFactoryBuilder : ISetupLogFactoryBuilder
{
	public LogFactory LogFactory { get; }

	internal SetupLogFactoryBuilder(LogFactory logFactory)
	{
		LogFactory = logFactory;
	}
}

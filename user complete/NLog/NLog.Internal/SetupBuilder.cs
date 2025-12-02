using NLog.Config;

namespace NLog.Internal;

internal sealed class SetupBuilder : ISetupBuilder
{
	public LogFactory LogFactory { get; }

	internal SetupBuilder(LogFactory logFactory)
	{
		LogFactory = logFactory;
	}
}

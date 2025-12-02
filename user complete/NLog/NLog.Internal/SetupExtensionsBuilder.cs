using NLog.Config;

namespace NLog.Internal;

internal sealed class SetupExtensionsBuilder : ISetupExtensionsBuilder
{
	public LogFactory LogFactory { get; }

	internal SetupExtensionsBuilder(LogFactory logFactory)
	{
		LogFactory = logFactory;
	}
}

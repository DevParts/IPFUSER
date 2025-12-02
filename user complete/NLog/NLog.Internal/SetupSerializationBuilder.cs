using NLog.Config;

namespace NLog.Internal;

internal sealed class SetupSerializationBuilder : ISetupSerializationBuilder
{
	public LogFactory LogFactory { get; }

	internal SetupSerializationBuilder(LogFactory logFactory)
	{
		LogFactory = logFactory;
	}
}

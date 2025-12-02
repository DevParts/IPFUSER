using System;

namespace NLog.Internal;

internal interface ITargetWithFilterChain
{
	void WriteToLoggerTargets(Type loggerType, LogEventInfo logEvent, LogFactory logFactory);
}

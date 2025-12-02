using System.Collections.Generic;
using NLog.Targets;

namespace NLog.Config;

/// <summary>
/// Interface for fluent setup of target for LoggingRule
/// </summary>
public interface ISetupConfigurationTargetBuilder
{
	/// <summary>
	/// LoggingConfiguration being built
	/// </summary>
	LoggingConfiguration Configuration { get; }

	/// <summary>
	/// LogFactory under configuration
	/// </summary>
	LogFactory LogFactory { get; }

	/// <summary>
	/// Collection of targets that should be written to
	/// </summary>
	IList<Target> Targets { get; }
}

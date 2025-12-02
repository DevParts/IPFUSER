using System.Collections.Generic;

namespace NLog.Config;

/// <summary>
/// Interface for accessing configuration details
/// </summary>
public interface ILoggingConfigurationElement
{
	/// <summary>
	/// Name of this configuration element
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Configuration Key/Value Pairs
	/// </summary>
	IEnumerable<KeyValuePair<string, string?>> Values { get; }

	/// <summary>
	/// Child configuration elements
	/// </summary>
	IEnumerable<ILoggingConfigurationElement> Children { get; }
}

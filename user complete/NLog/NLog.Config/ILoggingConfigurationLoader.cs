using System;
using System.Collections.Generic;

namespace NLog.Config;

/// <summary>
/// Interface for loading NLog <see cref="T:NLog.Config.LoggingConfiguration" />
/// </summary>
internal interface ILoggingConfigurationLoader : IDisposable
{
	/// <summary>
	/// Finds and loads the NLog configuration
	/// </summary>
	/// <param name="logFactory">LogFactory that owns the NLog configuration</param>
	/// <param name="filename">Name of NLog.config file (optional)</param>
	/// <returns>NLog configuration (or null if none found)</returns>
	LoggingConfiguration? Load(LogFactory logFactory, string? filename = null);

	/// <summary>
	/// Get file paths (including filename) for the possible NLog config files.
	/// </summary>
	/// <param name="filename">Name of NLog.config file (optional)</param>
	/// <returns>The file paths to the possible config file</returns>
	IEnumerable<string> GetDefaultCandidateConfigFilePaths(string? filename = null);
}

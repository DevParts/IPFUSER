using System.Collections.Specialized;
using System.Configuration;

namespace NLog.Internal;

/// <summary>
/// Interface for the wrapper around System.Configuration.ConfigurationManager.
/// </summary>
internal interface IConfigurationManager
{
	/// <summary>
	/// Gets the wrapper around ConfigurationManager.AppSettings.
	/// </summary>
	NameValueCollection AppSettings { get; }

	ConnectionStringSettings LookupConnectionString(string name);
}

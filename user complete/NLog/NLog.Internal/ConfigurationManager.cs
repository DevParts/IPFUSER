using System.Collections.Specialized;
using System.Configuration;

namespace NLog.Internal;

/// <summary>
/// Internal configuration manager used to read .NET configuration files.
/// Just a wrapper around the BCL ConfigurationManager, but used to enable
/// unit testing.
/// </summary>
internal class ConfigurationManager : IConfigurationManager
{
	public NameValueCollection AppSettings => System.Configuration.ConfigurationManager.AppSettings;

	public ConnectionStringSettings LookupConnectionString(string name)
	{
		return System.Configuration.ConfigurationManager.ConnectionStrings[name];
	}
}

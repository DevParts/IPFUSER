using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NLog.Config;

internal interface IAssemblyExtensionLoader
{
	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	void ScanForAutoLoadExtensions(ConfigurationItemFactory factory);

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	void LoadAssemblyFromPath(ConfigurationItemFactory factory, string assemblyFileName, string? baseDirectory, string itemNamePrefix);

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	void LoadAssemblyFromName(ConfigurationItemFactory factory, string assemblyName, string itemNamePrefix);

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	void LoadAssembly(ConfigurationItemFactory factory, Assembly assembly, string itemNamePrefix);
}

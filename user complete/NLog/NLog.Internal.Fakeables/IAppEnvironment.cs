using System;
using System.Collections.Generic;
using System.Reflection;

namespace NLog.Internal.Fakeables;

/// <summary>
/// Abstract calls for the application environment
/// </summary>
internal interface IAppEnvironment : IFileSystem
{
	string AppDomainBaseDirectory { get; }

	string AppDomainConfigurationFile { get; }

	string AppDomainFriendlyName { get; }

	int AppDomainId { get; }

	IEnumerable<string> AppDomainPrivateBinPath { get; }

	string CurrentProcessFilePath { get; }

	/// <summary>
	/// Gets current process name (excluding filename extension, if any).
	/// </summary>
	string CurrentProcessBaseName { get; }

	int CurrentProcessId { get; }

	string EntryAssemblyLocation { get; }

	string EntryAssemblyFileName { get; }

	string UserTempFilePath { get; }

	/// <summary>
	/// Process exit event.
	/// </summary>
	event EventHandler ProcessExit;

	IEnumerable<Assembly> GetAppDomainRuntimeAssemblies();
}

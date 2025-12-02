using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using NLog.Common;
using NLog.Filters;
using NLog.Internal;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Targets;

namespace NLog.Config;

[Obsolete("Instead use RegisterType<T>, as dynamic Assembly loading will be moved out. Marked obsolete with NLog v5.2")]
internal sealed class AssemblyExtensionLoader : IAssemblyExtensionLoader
{
	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	public void LoadAssemblyFromName(ConfigurationItemFactory factory, string assemblyName, string itemNamePrefix)
	{
		if (SkipAlreadyLoadedAssembly(factory, assemblyName, itemNamePrefix))
		{
			InternalLogger.Debug("Skipped already loaded assembly name: {0}", assemblyName);
			return;
		}
		InternalLogger.Info("Loading assembly name: {0}{1}", assemblyName, string.IsNullOrEmpty(itemNamePrefix) ? "" : (" (Prefix=" + itemNamePrefix + ")"));
		Assembly assembly = LoadAssemblyFromName(assemblyName);
		LoadAssembly(factory, assembly, itemNamePrefix);
	}

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	public void LoadAssembly(ConfigurationItemFactory factory, Assembly assembly, string itemNamePrefix)
	{
		AssemblyHelpers.LogAssemblyVersion(assembly);
		InternalLogger.Debug("ScanAssembly('{0}')", assembly.FullName);
		Type[] array = SafeGetTypes(assembly);
		if (array != null && array.Length != 0)
		{
			if ((object)assembly == typeof(LogFactory).Assembly)
			{
				array = array.Where((Type t) => t.IsPublic && t.IsClass).ToArray();
			}
			Type[] array2 = array;
			foreach (Type type in array2)
			{
				try
				{
					RegisterTypeFromAssembly(factory, type, itemNamePrefix);
				}
				catch (Exception ex)
				{
					InternalLogger.Error(ex, "Failed to add type '{0}'.", type.FullName);
					if (ex.MustBeRethrown())
					{
						throw;
					}
				}
			}
		}
		InternalLogger.Debug("Loading assembly name: {0} succeeded!", assembly.FullName);
	}

	/// <summary>
	/// Gets all usable exported types from the given assembly.
	/// </summary>
	/// <param name="assembly">Assembly to scan.</param>
	/// <returns>Usable types from the given assembly.</returns>
	/// <remarks>Types which cannot be loaded are skipped.</remarks>
	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	private static Type[] SafeGetTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			Type[] array = ex.Types?.Where((Type t) => t != null).ToArray() ?? ArrayHelper.Empty<Type>();
			InternalLogger.Warn(ex, "Loaded {0} valid types from Assembly: {1}", array.Length, assembly.FullName);
			Exception[] array2 = ex.LoaderExceptions ?? ArrayHelper.Empty<Exception>();
			for (int num = 0; num < array2.Length; num++)
			{
				InternalLogger.Warn(array2[num], "Type load exception.");
			}
			return array;
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Failed to load types from Assembly: {0}", assembly.FullName);
			return ArrayHelper.Empty<Type>();
		}
	}

	[UnconditionalSuppressMessage("Trimming - Allow extension loading from config", "IL2072")]
	[UnconditionalSuppressMessage("Trimming - Allow extension loading from config", "IL2067")]
	private static void RegisterTypeFromAssembly(ConfigurationItemFactory factory, Type type, string itemNamePrefix)
	{
		factory.RegisterType(type, itemNamePrefix);
	}

	private static bool SkipAlreadyLoadedAssembly(ConfigurationItemFactory factory, string assemblyName, string itemNamePrefix)
	{
		try
		{
			Dictionary<Assembly, Type> dictionary = ResolveLoadedAssemblyTypes(factory);
			if (dictionary.Count > 1)
			{
				foreach (KeyValuePair<Assembly, Type> item in dictionary)
				{
					if (string.Equals(item.Key.GetName()?.Name, assemblyName, StringComparison.OrdinalIgnoreCase) && IsNLogItemTypeAlreadyRegistered(factory, item.Value, itemNamePrefix))
					{
						return true;
					}
				}
			}
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrown())
			{
				throw;
			}
			InternalLogger.Warn(ex, "Failed checking loading assembly name: {0}", assemblyName);
		}
		return false;
	}

	private static Dictionary<Assembly, Type?> ResolveLoadedAssemblyTypes(ConfigurationItemFactory factory)
	{
		Dictionary<Assembly, Type> dictionary = new Dictionary<Assembly, Type>();
		foreach (Type itemType in factory.ItemTypes)
		{
			Assembly assembly = itemType.Assembly;
			if ((object)assembly == null)
			{
				continue;
			}
			if (dictionary.TryGetValue(assembly, out var value))
			{
				if ((object)value == null && IsNLogConfigurationItemType(itemType))
				{
					dictionary[assembly] = itemType;
				}
			}
			else
			{
				dictionary.Add(assembly, IsNLogConfigurationItemType(itemType) ? itemType : null);
			}
		}
		return dictionary;
	}

	private static bool IsNLogConfigurationItemType(Type? itemType)
	{
		if ((object)itemType == null)
		{
			return false;
		}
		if (typeof(Layout).IsAssignableFrom(itemType))
		{
			return !string.IsNullOrEmpty(itemType.GetFirstCustomAttribute<LayoutAttribute>()?.Name);
		}
		if (typeof(LayoutRenderer).IsAssignableFrom(itemType))
		{
			return !string.IsNullOrEmpty(itemType.GetFirstCustomAttribute<LayoutRendererAttribute>()?.Name);
		}
		if (typeof(Target).IsAssignableFrom(itemType))
		{
			return !string.IsNullOrEmpty(itemType.GetFirstCustomAttribute<TargetAttribute>()?.Name);
		}
		if (typeof(Filter).IsAssignableFrom(itemType))
		{
			return !string.IsNullOrEmpty(itemType.GetFirstCustomAttribute<FilterAttribute>()?.Name);
		}
		return false;
	}

	private static bool IsNLogItemTypeAlreadyRegistered(ConfigurationItemFactory factory, Type? itemType, string itemNamePrefix)
	{
		if ((object)itemType == null)
		{
			return false;
		}
		if (typeof(Layout).IsAssignableFrom(itemType))
		{
			return IsNLogItemTypeAlreadyRegistered<Layout, LayoutAttribute>(factory.LayoutFactory, itemType, itemNamePrefix);
		}
		if (typeof(LayoutRenderer).IsAssignableFrom(itemType))
		{
			return IsNLogItemTypeAlreadyRegistered<LayoutRenderer, LayoutRendererAttribute>(factory.LayoutRendererFactory, itemType, itemNamePrefix);
		}
		if (typeof(Target).IsAssignableFrom(itemType))
		{
			return IsNLogItemTypeAlreadyRegistered<Target, TargetAttribute>(factory.TargetFactory, itemType, itemNamePrefix);
		}
		if (typeof(Filter).IsAssignableFrom(itemType))
		{
			return IsNLogItemTypeAlreadyRegistered<Filter, FilterAttribute>(factory.FilterFactory, itemType, itemNamePrefix);
		}
		return false;
	}

	private static bool IsNLogItemTypeAlreadyRegistered<TBaseType, TAttribute>(IFactory<TBaseType> factory, Type itemType, string itemNamePrefix) where TBaseType : class where TAttribute : NameBaseAttribute
	{
		string text = itemType.GetFirstCustomAttribute<TAttribute>()?.Name ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			string typeAlias = (string.IsNullOrEmpty(itemNamePrefix) ? text : (itemNamePrefix + text));
			TBaseType result;
			return factory.TryCreateInstance(typeAlias, out result);
		}
		return false;
	}

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	public void LoadAssemblyFromPath(ConfigurationItemFactory factory, string assemblyFileName, string? baseDirectory, string itemNamePrefix)
	{
		Assembly assembly = LoadAssemblyFromPath(assemblyFileName, baseDirectory);
		if (assembly != null)
		{
			LoadAssembly(factory, assembly, itemNamePrefix);
		}
	}

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	public void ScanForAutoLoadExtensions(ConfigurationItemFactory factory)
	{
		try
		{
			Assembly assembly = typeof(LogFactory).Assembly;
			string text = string.Empty;
			string[] array = ArrayHelper.Empty<string>();
			foreach (KeyValuePair<string, string> autoLoadingFileLocation in GetAutoLoadingFileLocations())
			{
				if (!string.IsNullOrEmpty(autoLoadingFileLocation.Key))
				{
					if (string.IsNullOrEmpty(text))
					{
						text = autoLoadingFileLocation.Key;
					}
					array = GetNLogExtensionFiles(autoLoadingFileLocation.Key);
					if (array.Length != 0)
					{
						text = autoLoadingFileLocation.Key;
						break;
					}
				}
			}
			InternalLogger.Debug("Start auto loading, location: {0}", text);
			HashSet<string> alreadyRegistered = LoadNLogExtensionAssemblies(factory, assembly, array);
			RegisterAppDomainAssemblies(factory, assembly, alreadyRegistered);
		}
		catch (SecurityException ex)
		{
			InternalLogger.Warn(ex, "Seems that we do not have permission");
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
		catch (UnauthorizedAccessException ex2)
		{
			InternalLogger.Warn(ex2, "Seems that we do not have permission");
			if (ex2.MustBeRethrown())
			{
				throw;
			}
		}
		InternalLogger.Debug("Auto loading done");
	}

	[RequiresUnreferencedCode("Assembly.LoadFrom() Incompatible with trimming.")]
	private HashSet<string> LoadNLogExtensionAssemblies(ConfigurationItemFactory factory, Assembly nlogAssembly, string[] extensionDlls)
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { nlogAssembly.FullName };
		foreach (string text in extensionDlls)
		{
			try
			{
				Assembly assembly = LoadAssemblyFromPath(text);
				if (assembly != null)
				{
					LoadAssembly(factory, assembly, string.Empty);
					hashSet.Add(assembly.FullName);
				}
			}
			catch (Exception ex)
			{
				if (ex.MustBeRethrownImmediately())
				{
					throw;
				}
				InternalLogger.Warn(ex, "Auto loading assembly file: {0} failed! Skipping this file.", text);
				if (ex.MustBeRethrown())
				{
					throw;
				}
			}
		}
		return hashSet;
	}

	[RequiresUnreferencedCode("Assembly.GetTypes() Incompatible with trimming.")]
	private void RegisterAppDomainAssemblies(ConfigurationItemFactory factory, Assembly nlogAssembly, HashSet<string> alreadyRegistered)
	{
		alreadyRegistered.Add(nlogAssembly.FullName);
		foreach (Assembly appDomainRuntimeAssembly in LogFactory.DefaultAppEnvironment.GetAppDomainRuntimeAssemblies())
		{
			if (appDomainRuntimeAssembly.FullName.StartsWith("NLog.", StringComparison.OrdinalIgnoreCase) && !alreadyRegistered.Contains(appDomainRuntimeAssembly.FullName))
			{
				LoadAssembly(factory, appDomainRuntimeAssembly, string.Empty);
			}
			if (IncludeAsHiddenAssembly(appDomainRuntimeAssembly.FullName))
			{
				LogManager.AddHiddenAssembly(appDomainRuntimeAssembly);
			}
		}
	}

	private static bool IncludeAsHiddenAssembly(string assemblyFullName)
	{
		if (assemblyFullName.StartsWith("NLog.Extensions.Logging,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (assemblyFullName.StartsWith("NLog.Web,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (assemblyFullName.StartsWith("NLog.Web.AspNetCore,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (assemblyFullName.StartsWith("Microsoft.Extensions.Logging,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (assemblyFullName.StartsWith("Microsoft.Extensions.Logging.Abstractions,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (assemblyFullName.StartsWith("Microsoft.Extensions.Logging.Filter,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (assemblyFullName.StartsWith("Microsoft.Logging,", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}

	internal static IEnumerable<KeyValuePair<string, string>> GetAutoLoadingFileLocations()
	{
		string assemblyFileLocation = AssemblyHelpers.GetAssemblyFileLocation(typeof(LogFactory).Assembly);
		string nlogAssemblyDirectory = (string.IsNullOrEmpty(assemblyFileLocation) ? assemblyFileLocation : PathHelpers.TrimDirectorySeparators(Path.GetDirectoryName(assemblyFileLocation)));
		InternalLogger.Debug("Auto loading based on NLog-Assembly found location: {0}", nlogAssemblyDirectory);
		if (!string.IsNullOrEmpty(nlogAssemblyDirectory))
		{
			yield return new KeyValuePair<string, string>(nlogAssemblyDirectory, "nlogAssemblyLocation");
		}
		string entryAssemblyLocation = PathHelpers.TrimDirectorySeparators(LogFactory.DefaultAppEnvironment.EntryAssemblyLocation);
		InternalLogger.Debug("Auto loading based on GetEntryAssembly-Assembly found location: {0}", entryAssemblyLocation);
		if (!string.IsNullOrEmpty(entryAssemblyLocation) && !string.Equals(entryAssemblyLocation, nlogAssemblyDirectory, StringComparison.OrdinalIgnoreCase))
		{
			yield return new KeyValuePair<string, string>(entryAssemblyLocation, "entryAssemblyLocation");
		}
		string text = PathHelpers.TrimDirectorySeparators(LogFactory.DefaultAppEnvironment.AppDomainBaseDirectory);
		InternalLogger.Debug("Auto loading based on AppDomain-BaseDirectory found location: {0}", text);
		if (!string.IsNullOrEmpty(text) && !string.Equals(text, nlogAssemblyDirectory, StringComparison.OrdinalIgnoreCase) && !string.Equals(text, entryAssemblyLocation, StringComparison.OrdinalIgnoreCase))
		{
			yield return new KeyValuePair<string, string>(text, "baseDirectory");
		}
	}

	private static string[] GetNLogExtensionFiles(string assemblyLocation)
	{
		try
		{
			InternalLogger.Debug("Search for auto loading files in location: {0}", assemblyLocation);
			if (string.IsNullOrEmpty(assemblyLocation))
			{
				return ArrayHelper.Empty<string>();
			}
			return (from x in Directory.GetFiles(assemblyLocation, "NLog*.dll").Select(Path.GetFileName)
				where !x.Equals("NLog.dll", StringComparison.OrdinalIgnoreCase)
				where !x.Equals("NLog.UnitTests.dll", StringComparison.OrdinalIgnoreCase)
				select Path.Combine(assemblyLocation, x)).ToArray();
		}
		catch (DirectoryNotFoundException ex)
		{
			InternalLogger.Warn(ex, "Skipping auto loading location because assembly directory does not exist: {0}", assemblyLocation);
			if (ex.MustBeRethrown())
			{
				throw;
			}
			return ArrayHelper.Empty<string>();
		}
		catch (SecurityException ex2)
		{
			InternalLogger.Warn(ex2, "Skipping auto loading location because access not allowed to assembly directory: {0}", assemblyLocation);
			if (ex2.MustBeRethrown())
			{
				throw;
			}
			return ArrayHelper.Empty<string>();
		}
		catch (UnauthorizedAccessException ex3)
		{
			InternalLogger.Warn(ex3, "Skipping auto loading location because access not allowed to assembly directory: {0}", assemblyLocation);
			if (ex3.MustBeRethrown())
			{
				throw;
			}
			return ArrayHelper.Empty<string>();
		}
	}

	/// <summary>
	/// Load from url
	/// </summary>
	/// <param name="assemblyFileName">file or path, including .dll</param>
	/// <param name="baseDirectory">basepath, optional</param>
	/// <returns></returns>
	[RequiresUnreferencedCode("Assembly.LoadFrom() Incompatible with trimming.")]
	private static Assembly LoadAssemblyFromPath(string assemblyFileName, string? baseDirectory = null)
	{
		string text = assemblyFileName;
		if (!string.IsNullOrEmpty(baseDirectory))
		{
			text = Path.Combine(baseDirectory, assemblyFileName);
		}
		InternalLogger.Info("Loading assembly file: {0}", text);
		return Assembly.LoadFrom(text);
	}

	/// <summary>
	/// Load from url
	/// </summary>
	private static Assembly LoadAssemblyFromName(string assemblyName)
	{
		try
		{
			return Assembly.Load(assemblyName);
		}
		catch (FileNotFoundException)
		{
			AssemblyName name = new AssemblyName(assemblyName);
			InternalLogger.Trace("Try find '{0}' in current domain", assemblyName);
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault((Assembly domainAssembly) => IsAssemblyMatch(name, domainAssembly.GetName()));
			if (assembly != null)
			{
				InternalLogger.Trace("Found '{0}' in current domain", assemblyName);
				return assembly;
			}
			InternalLogger.Trace("Haven't found' '{0}' in current domain", assemblyName);
			throw;
		}
	}

	private static bool IsAssemblyMatch(AssemblyName expected, AssemblyName actual)
	{
		if (expected == null || actual == null)
		{
			return false;
		}
		if (expected.Name != actual.Name)
		{
			return false;
		}
		if (expected.Version != null && expected.Version != actual.Version)
		{
			return false;
		}
		if (expected.CultureInfo != null && expected.CultureInfo.Name != actual.CultureInfo.Name)
		{
			return false;
		}
		return expected.GetPublicKeyToken()?.SequenceEqual(actual.GetPublicKeyToken()) ?? true;
	}
}

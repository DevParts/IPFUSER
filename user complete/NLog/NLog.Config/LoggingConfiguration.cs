using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using NLog.Common;
using NLog.Internal;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace NLog.Config;

/// <summary>
/// Keeps logging configuration and provides simple API to modify it.
/// </summary>
///             <remarks>This class is thread-safe.<c>.ToList()</c> is used for that purpose.</remarks>
public class LoggingConfiguration
{
	private readonly IDictionary<string, Target> _targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);

	private List<object> _configItems = new List<object>();

	private bool _missingServiceTypes;

	private readonly ConfigVariablesDictionary _variables;

	private readonly List<LoggingRule> _loggingRules = new List<LoggingRule>();

	/// <summary>
	/// Gets the factory that will be configured
	/// </summary>
	public LogFactory LogFactory { get; }

	/// <summary>
	/// Gets the variables defined in the configuration or assigned from API
	/// </summary>
	/// <remarks>Name is case insensitive.</remarks>
	public IDictionary<string, Layout> Variables => _variables;

	/// <summary>
	/// Gets a collection of named targets specified in the configuration.
	/// </summary>
	/// <returns>
	/// A list of named targets.
	/// </returns>
	/// <remarks>
	/// Unnamed targets (such as those wrapped by other targets) are not returned.
	/// </remarks>
	public ReadOnlyCollection<Target> ConfiguredNamedTargets => GetAllTargetsThreadSafe().AsReadOnly();

	/// <summary>
	/// Gets the collection of file names which should be watched for changes by NLog.
	/// </summary>
	[Obsolete("NLog LogFactory no longer supports FileWatcher. Marked obsolete with NLog v6")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual IEnumerable<string> FileNamesToWatch => ArrayHelper.Empty<string>();

	/// <summary>
	/// Gets the collection of logging rules.
	/// </summary>
	public IList<LoggingRule> LoggingRules => _loggingRules;

	/// <summary>
	/// Gets or sets the default culture info to use as <see cref="P:NLog.LogEventInfo.FormatProvider" />.
	/// </summary>
	/// <value>
	/// Specific culture info or null to use <see cref="P:System.Globalization.CultureInfo.CurrentCulture" />
	/// </value>
	public CultureInfo? DefaultCultureInfo { get; set; }

	/// <summary>
	/// Gets all targets.
	/// </summary>
	public ReadOnlyCollection<Target> AllTargets => new HashSet<Target>(_configItems.OfType<Target>().Concat<Target>(GetAllTargetsThreadSafe()), SingleItemOptimizedHashSet<Target>.ReferenceEqualityComparer.Default).ToList().AsReadOnly();

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.LoggingConfiguration" /> class.
	/// </summary>
	public LoggingConfiguration()
		: this(LogManager.LogFactory)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.LoggingConfiguration" /> class.
	/// </summary>
	public LoggingConfiguration(LogFactory logFactory)
	{
		LogFactory = logFactory ?? LogManager.LogFactory;
		_variables = new ConfigVariablesDictionary(this);
		DefaultCultureInfo = LogFactory._defaultCultureInfo;
	}

	internal LoggingRule[] GetLoggingRulesThreadSafe()
	{
		lock (_loggingRules)
		{
			return _loggingRules.ToArray();
		}
	}

	private void AddLoggingRulesThreadSafe(LoggingRule rule)
	{
		lock (_loggingRules)
		{
			_loggingRules.Add(rule);
		}
	}

	private bool TryGetTargetThreadSafe(string name, out Target target)
	{
		lock (_targets)
		{
			return _targets.TryGetValue(name, out target);
		}
	}

	private List<Target> GetAllTargetsThreadSafe()
	{
		lock (_targets)
		{
			return _targets.Values.ToList();
		}
	}

	private Target? RemoveTargetThreadSafe(string name)
	{
		Target value;
		lock (_targets)
		{
			if (_targets.TryGetValue(name, out value))
			{
				_targets.Remove(name);
			}
		}
		if (value != null)
		{
			InternalLogger.Debug("Unregistered target {0}(Name={1})", value.GetType(), value.Name);
		}
		return value;
	}

	private void AddTargetThreadSafe(Target target, string? targetAlias = null)
	{
		lock (_targets)
		{
			if (targetAlias == null || string.IsNullOrEmpty(targetAlias))
			{
				targetAlias = target.Name ?? string.Empty;
				if (_targets.ContainsKey(targetAlias))
				{
					return;
				}
			}
			if (_targets.TryGetValue(targetAlias, out Target value) && value == target)
			{
				return;
			}
			_targets[targetAlias] = target;
		}
		if (!string.IsNullOrEmpty(target.Name) && !string.Equals(target.Name, targetAlias, StringComparison.OrdinalIgnoreCase))
		{
			InternalLogger.Info("Registered target {0}(Name={1}) (Extra alias={2})", target.GetType(), target.Name, targetAlias);
		}
		else
		{
			InternalLogger.Info("Registered target {0}(Name={1})", target.GetType(), target.Name);
		}
	}

	/// <summary>
	/// Inserts NLog Config Variable without overriding NLog Config Variable assigned from API
	/// </summary>
	internal void InsertParsedConfigVariable(string key, Layout value)
	{
		_variables.InsertParsedConfigVariable(key, value, LogFactory.KeepVariablesOnReload);
	}

	/// <summary>
	/// Lookup NLog Config Variable Layout
	/// </summary>
	internal bool TryLookupDynamicVariable(string key, out Layout value)
	{
		return _variables.TryLookupDynamicVariable(key, out value);
	}

	/// <summary>
	/// Registers the specified target object. The name of the target is read from <see cref="P:NLog.Targets.Target.Name" />.
	/// </summary>
	/// <param name="target">
	/// The target object with a non <see langword="null" /> <see cref="P:NLog.Targets.Target.Name" />
	/// </param>
	/// <exception cref="T:System.ArgumentNullException">when <paramref name="target" /> is <see langword="null" /></exception>
	public void AddTarget(Target target)
	{
		Guard.ThrowIfNull(target, "target");
		InternalLogger.Debug("Adding target {0}(Name={1})", target.GetType(), target.Name);
		if (string.IsNullOrEmpty(target.Name))
		{
			throw new ArgumentException("target.Name cannot be empty", "target");
		}
		AddTargetThreadSafe(target, target.Name);
	}

	/// <summary>
	/// Registers the specified target object under a given name.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	/// <param name="target">The target object.</param>
	/// <exception cref="T:System.ArgumentException">when <paramref name="name" /> is <see langword="null" /></exception>
	/// <exception cref="T:System.ArgumentNullException">when <paramref name="target" /> is <see langword="null" /></exception>
	public void AddTarget(string name, Target target)
	{
		Guard.ThrowIfNull(name, "name");
		Guard.ThrowIfNull(target, "target");
		InternalLogger.Debug("Adding target {0}(Name={1})", target.GetType(), string.IsNullOrEmpty(name) ? target.Name : name);
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Target name cannot be empty", "name");
		}
		AddTargetThreadSafe(target, name);
	}

	/// <summary>
	/// Finds the target with the specified name.
	/// </summary>
	/// <param name="name">
	/// The name of the target to be found.
	/// </param>
	/// <returns>
	/// Found target or <see langword="null" /> when the target is not found.
	/// </returns>
	public Target? FindTargetByName(string name)
	{
		Guard.ThrowIfNull(name, "name");
		if (TryGetTargetThreadSafe(name, out Target target))
		{
			return target;
		}
		return null;
	}

	/// <summary>
	/// Finds the target with the specified name and specified type.
	/// </summary>
	/// <param name="name">
	/// The name of the target to be found.
	/// </param>
	/// <typeparam name="TTarget">Type of the target</typeparam>
	/// <returns>
	/// Found target or <see langword="null" /> when the target is not found of not of type <typeparamref name="TTarget" />
	/// </returns>
	public TTarget? FindTargetByName<TTarget>(string name) where TTarget : Target
	{
		Guard.ThrowIfNull(name, "name");
		Target target = FindTargetByName(name);
		if (target is TTarget result)
		{
			return result;
		}
		if (target is WrapperTargetBase wrapperTargetBase)
		{
			if (wrapperTargetBase.WrappedTarget is TTarget result2)
			{
				return result2;
			}
			if (wrapperTargetBase.WrappedTarget is WrapperTargetBase { WrappedTarget: TTarget wrappedTarget })
			{
				return wrappedTarget;
			}
		}
		return null;
	}

	/// <summary>
	/// Add a rule with min- and maxLevel.
	/// </summary>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="maxLevel">Maximum log level needed to trigger this rule.</param>
	/// <param name="targetName">Name of the target to be written when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	public void AddRule(LogLevel minLevel, LogLevel maxLevel, string targetName, string loggerNamePattern = "*")
	{
		Guard.ThrowIfNull(targetName, "targetName");
		Target target = FindTargetByName(targetName);
		if (target == null)
		{
			throw new NLogConfigurationException("Target '" + targetName + "' not found");
		}
		AddRule(minLevel, maxLevel, target, loggerNamePattern, final: false);
	}

	/// <summary>
	/// Add a rule with min- and maxLevel.
	/// </summary>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="maxLevel">Maximum log level needed to trigger this rule.</param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	public void AddRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern = "*")
	{
		AddRule(minLevel, maxLevel, target, loggerNamePattern, final: false);
	}

	/// <summary>
	/// Add a rule with min- and maxLevel.
	/// </summary>
	/// <param name="minLevel">Minimum log level needed to trigger this rule.</param>
	/// <param name="maxLevel">Maximum log level needed to trigger this rule.</param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	/// <param name="final">Gets or sets a value indicating whether to quit processing any further rule when this one matches.</param>
	public void AddRule(LogLevel minLevel, LogLevel maxLevel, Target target, string loggerNamePattern, bool final)
	{
		Guard.ThrowIfNull(target, "target");
		AddTargetThreadSafe(target);
		AddLoggingRulesThreadSafe(new LoggingRule(loggerNamePattern, minLevel, maxLevel, target)
		{
			Final = final
		});
	}

	/// <summary>
	/// Add a rule object.
	/// </summary>
	/// <param name="rule">rule object to add</param>
	public void AddRule(LoggingRule rule)
	{
		Guard.ThrowIfNull(rule, "rule");
		IList<Target> targets = rule.Targets;
		if (targets != null && targets.Count > 0)
		{
			foreach (Target target in rule.Targets)
			{
				AddTargetThreadSafe(target);
			}
		}
		AddLoggingRulesThreadSafe(rule);
	}

	/// <summary>
	/// Add a rule for one loglevel.
	/// </summary>
	/// <param name="level">log level needed to trigger this rule. </param>
	/// <param name="targetName">Name of the target to be written when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	public void AddRuleForOneLevel(LogLevel level, string targetName, string loggerNamePattern = "*")
	{
		Guard.ThrowIfNull(level, "level");
		AddRule(level, level, targetName, loggerNamePattern);
	}

	/// <summary>
	/// Add a rule for one loglevel.
	/// </summary>
	/// <param name="level">log level needed to trigger this rule. </param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	public void AddRuleForOneLevel(LogLevel level, Target target, string loggerNamePattern = "*")
	{
		Guard.ThrowIfNull(level, "level");
		AddRule(level, level, target, loggerNamePattern, final: false);
	}

	/// <summary>
	/// Add a rule for one loglevel.
	/// </summary>
	/// <param name="level">log level needed to trigger this rule. </param>
	/// <param name="target">Target to be written to when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	/// <param name="final">Gets or sets a value indicating whether to quit processing any further rule when this one matches.</param>
	public void AddRuleForOneLevel(LogLevel level, Target target, string loggerNamePattern, bool final)
	{
		Guard.ThrowIfNull(level, "level");
		AddRule(level, level, target, loggerNamePattern, final);
	}

	/// <summary>
	/// Add a rule for all loglevels.
	/// </summary>
	/// <param name="targetName">Name of the target to be written when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	public void AddRuleForAllLevels(string targetName, string loggerNamePattern = "*")
	{
		AddRule(LogLevel.MinLevel, LogLevel.MaxLevel, targetName, loggerNamePattern);
	}

	/// <summary>
	/// Add a rule for all loglevels.
	/// </summary>
	/// <param name="target">Target to be written to when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	public void AddRuleForAllLevels(Target target, string loggerNamePattern = "*")
	{
		AddRule(LogLevel.MinLevel, LogLevel.MaxLevel, target, loggerNamePattern, final: false);
	}

	/// <summary>
	/// Add a rule for all loglevels.
	/// </summary>
	/// <param name="target">Target to be written to when the rule matches.</param>
	/// <param name="loggerNamePattern">Logger name pattern. It may include the '*' wildcard at the beginning, at the end or at both ends.</param>
	/// <param name="final">Gets or sets a value indicating whether to quit processing any further rule when this one matches.</param>
	public void AddRuleForAllLevels(Target target, string loggerNamePattern, bool final)
	{
		AddRule(LogLevel.MinLevel, LogLevel.MaxLevel, target, loggerNamePattern, final);
	}

	/// <summary>
	/// Lookup the logging rule with matching <see cref="P:NLog.Config.LoggingRule.RuleName" />
	/// </summary>
	/// <param name="ruleName">The name of the logging rule to be found.</param>
	/// <returns>Found logging rule or <see langword="null" /> when not found.</returns>
	public LoggingRule? FindRuleByName(string ruleName)
	{
		Guard.ThrowIfNull(ruleName, "ruleName");
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		for (int num = loggingRulesThreadSafe.Length - 1; num >= 0; num--)
		{
			if (string.Equals(loggingRulesThreadSafe[num].RuleName, ruleName, StringComparison.OrdinalIgnoreCase))
			{
				return loggingRulesThreadSafe[num];
			}
		}
		return null;
	}

	/// <summary>
	/// Removes the specified named logging rule with matching <see cref="P:NLog.Config.LoggingRule.RuleName" />
	/// </summary>
	/// <param name="ruleName">The name of the logging rule to be removed.</param>
	/// <returns>Found one or more logging rule to remove, or <see langword="false" /> when not found.</returns>
	public bool RemoveRuleByName(string ruleName)
	{
		Guard.ThrowIfNull(ruleName, "ruleName");
		HashSet<LoggingRule> hashSet = new HashSet<LoggingRule>();
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		foreach (LoggingRule loggingRule in loggingRulesThreadSafe)
		{
			if (string.Equals(loggingRule.RuleName, ruleName, StringComparison.OrdinalIgnoreCase))
			{
				hashSet.Add(loggingRule);
			}
		}
		if (hashSet.Count > 0)
		{
			lock (LoggingRules)
			{
				for (int num = LoggingRules.Count - 1; num >= 0; num--)
				{
					if (hashSet.Contains(LoggingRules[num]))
					{
						LoggingRules.RemoveAt(num);
					}
				}
			}
		}
		return hashSet.Count > 0;
	}

	/// <summary>
	/// Loads the NLog LoggingConfiguration from its original source (Ex. read from original config-file after it was updated)
	/// </summary>
	/// <returns>
	/// A new instance of <see cref="T:NLog.Config.LoggingConfiguration" /> that represents the updated configuration.
	/// </returns>
	/// <remarks>Must assign the returned object to LogManager.Configuration to activate it</remarks>
	public virtual LoggingConfiguration Reload()
	{
		return this;
	}

	/// <summary>
	/// Allow this new configuration to capture state from the old configuration
	/// </summary>
	/// <param name="oldConfig">Old config that is about to be replaced</param>
	/// <remarks>Checks KeepVariablesOnReload and copies all NLog Config Variables assigned from API into the new config</remarks>
	protected void PrepareForReload(LoggingConfiguration oldConfig)
	{
		if (LogFactory.KeepVariablesOnReload)
		{
			_variables.PrepareForReload(oldConfig._variables);
		}
	}

	/// <summary>
	/// Notify the configuration when <see cref="P:NLog.LogFactory.Configuration" /> has been assigned / unassigned.
	/// </summary>
	/// <param name="logFactory">LogFactory that configuration has been assigned to.</param>
	protected internal virtual void OnConfigurationAssigned(LogFactory? logFactory)
	{
		if (logFactory != LogFactory && logFactory != null)
		{
			if (LogFactory == LogManager.LogFactory)
			{
				InternalLogger.Info("Configuration assigned to local LogFactory, but constructed using global LogFactory");
			}
			else
			{
				InternalLogger.Info("Configuration assigned to LogFactory, but constructed using other LogFactory");
			}
		}
	}

	/// <summary>
	/// Removes the specified named target.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	public void RemoveTarget(string name)
	{
		Guard.ThrowIfNull(name, "name");
		HashSet<Target> hashSet = new HashSet<Target>();
		Target target = RemoveTargetThreadSafe(name);
		if (target != null)
		{
			hashSet.Add(target);
		}
		if (!string.IsNullOrEmpty(name) || target != null)
		{
			CleanupRulesForRemovedTarget(name, target, hashSet);
		}
		if (hashSet.Count <= 0)
		{
			return;
		}
		LogFactory.ReconfigExistingLoggers();
		ManualResetEvent flushCompleted = new ManualResetEvent(initialState: false);
		foreach (Target item in hashSet)
		{
			flushCompleted.Reset();
			item.Flush(delegate
			{
				flushCompleted.Set();
			});
			flushCompleted.WaitOne(TimeSpan.FromSeconds(15.0));
			item.Close();
		}
	}

	private void CleanupRulesForRemovedTarget(string name, Target? removedTarget, HashSet<Target> removedTargets)
	{
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		foreach (LoggingRule loggingRule in loggingRulesThreadSafe)
		{
			Target[] targetsThreadSafe = loggingRule.GetTargetsThreadSafe();
			foreach (Target target in targetsThreadSafe)
			{
				if (removedTarget == target || (!string.IsNullOrEmpty(name) && target.Name == name))
				{
					removedTargets.Add(target);
					loggingRule.RemoveTargetThreadSafe(target);
				}
			}
		}
	}

	/// <summary>
	/// Installs target-specific objects on current system.
	/// </summary>
	/// <param name="installationContext">The installation context.</param>
	/// <remarks>
	/// Installation typically runs with administrative permissions.
	/// </remarks>
	public void Install(InstallationContext installationContext)
	{
		Guard.ThrowIfNull(installationContext, "installationContext");
		InitializeAll();
		foreach (IInstallable installableItem in GetInstallableItems())
		{
			installationContext.Info("Installing '{0}'", installableItem);
			try
			{
				installableItem.Install(installationContext);
				installationContext.Info("Finished installing '{0}'.", installableItem);
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "Install of '{0}' failed.", installableItem);
				if (ex.MustBeRethrownImmediately() || installationContext.ThrowExceptions)
				{
					throw;
				}
				installationContext.Error("Install of '{0}' failed: {1}.", installableItem, ex);
			}
		}
	}

	/// <summary>
	/// Uninstalls target-specific objects from current system.
	/// </summary>
	/// <param name="installationContext">The installation context.</param>
	/// <remarks>
	/// Uninstallation typically runs with administrative permissions.
	/// </remarks>
	public void Uninstall(InstallationContext installationContext)
	{
		Guard.ThrowIfNull(installationContext, "installationContext");
		InitializeAll();
		foreach (IInstallable installableItem in GetInstallableItems())
		{
			installationContext.Info("Uninstalling '{0}'", installableItem);
			try
			{
				installableItem.Uninstall(installationContext);
				installationContext.Info("Finished uninstalling '{0}'.", installableItem);
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "Uninstall of '{0}' failed.", installableItem);
				if (ex.MustBeRethrownImmediately())
				{
					throw;
				}
				installationContext.Error("Uninstall of '{0}' failed: {1}.", installableItem, ex);
			}
		}
	}

	/// <summary>
	/// Closes all targets and releases any unmanaged resources.
	/// </summary>
	internal void Close()
	{
		InternalLogger.Debug("Closing logging configuration...");
		foreach (ISupportsInitialize supportsInitialize in GetSupportsInitializes())
		{
			InternalLogger.Trace("Closing {0}", supportsInitialize);
			try
			{
				supportsInitialize.Close();
			}
			catch (Exception ex)
			{
				InternalLogger.Warn(ex, "Exception while closing.");
				if (ex.MustBeRethrown())
				{
					throw;
				}
			}
		}
		InternalLogger.Debug("Finished closing logging configuration.");
	}

	/// <summary>
	/// Log to the internal (NLog) logger the information about the <see cref="T:NLog.Targets.Target" /> and <see cref="T:NLog.Config.LoggingRule" /> associated with this <see cref="T:NLog.Config.LoggingConfiguration" /> instance.
	/// </summary>
	/// <remarks>
	/// The information are only recorded in the internal logger if Debug level is enabled, otherwise nothing is
	/// recorded.
	/// </remarks>
	internal void Dump()
	{
		if (!InternalLogger.IsDebugEnabled)
		{
			return;
		}
		InternalLogger.Debug("--- NLog configuration dump ---");
		InternalLogger.Debug("Targets:");
		foreach (Target item in GetAllTargetsThreadSafe())
		{
			InternalLogger.Debug("{0}", item);
		}
		InternalLogger.Debug("Rules:");
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		foreach (LoggingRule arg in loggingRulesThreadSafe)
		{
			InternalLogger.Debug("{0}", arg);
		}
		InternalLogger.Debug("--- End of NLog configuration dump ---");
	}

	internal HashSet<Target> GetAllTargetsToFlush()
	{
		HashSet<Target> hashSet = new HashSet<Target>(SingleItemOptimizedHashSet<Target>.ReferenceEqualityComparer.Default);
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		for (int i = 0; i < loggingRulesThreadSafe.Length; i++)
		{
			Target[] targetsThreadSafe = loggingRulesThreadSafe[i].GetTargetsThreadSafe();
			foreach (Target item in targetsThreadSafe)
			{
				hashSet.Add(item);
			}
		}
		return hashSet;
	}

	/// <summary>
	/// Validates the configuration.
	/// </summary>
	internal void ValidateConfig()
	{
		List<object> list = new List<object>();
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		foreach (LoggingRule item in loggingRulesThreadSafe)
		{
			list.Add(item);
		}
		foreach (Target item2 in GetAllTargetsThreadSafe())
		{
			list.Add(item2);
		}
		_configItems = ObjectGraphScanner.FindReachableObjects<object>(ConfigurationItemFactory.Default, aggressiveSearch: true, list.ToArray());
	}

	internal void InitializeAll()
	{
		bool num = _configItems.Count == 0;
		if (num && (LogFactory.ThrowExceptions || LogManager.ThrowExceptions))
		{
			InternalLogger.Info("LogManager.ThrowExceptions = true can crash the application! Use only for unit-testing and last resort troubleshooting.");
		}
		ValidateConfig();
		if (num && _targets.Count > 0)
		{
			CheckUnusedTargets();
		}
		foreach (ISupportsInitialize supportsInitialize in GetSupportsInitializes(reverse: true))
		{
			InternalLogger.Trace("Initializing {0}", supportsInitialize);
			try
			{
				supportsInitialize.Initialize(this);
			}
			catch (Exception exception)
			{
				if (exception.MustBeRethrown(supportsInitialize as IInternalLoggerContext))
				{
					throw;
				}
			}
			if (supportsInitialize is Target target && target.InitializeException is NLogDependencyResolveException)
			{
				_missingServiceTypes = true;
			}
		}
	}

	internal void CheckForMissingServiceTypes(Type serviceType)
	{
		if (!_missingServiceTypes)
		{
			return;
		}
		bool flag = false;
		foreach (Target allTarget in AllTargets)
		{
			if (allTarget.InitializeException is NLogDependencyResolveException resolveException)
			{
				flag = true;
				if (typeof(IServiceProvider).IsAssignableFrom(serviceType) || IsMissingServiceType(resolveException, serviceType))
				{
					allTarget.Close();
				}
			}
		}
		_missingServiceTypes = flag;
		if (flag)
		{
			InitializeAll();
		}
	}

	private static bool IsMissingServiceType(NLogDependencyResolveException resolveException, Type serviceType)
	{
		if (resolveException.ServiceType.IsAssignableFrom(serviceType))
		{
			return true;
		}
		if (resolveException.InnerException is NLogDependencyResolveException resolveException2)
		{
			return IsMissingServiceType(resolveException2, serviceType);
		}
		return false;
	}

	private List<IInstallable> GetInstallableItems()
	{
		return _configItems.OfType<IInstallable>().ToList();
	}

	private List<ISupportsInitialize> GetSupportsInitializes(bool reverse = false)
	{
		IEnumerable<ISupportsInitialize> source = _configItems.OfType<ISupportsInitialize>();
		if (reverse)
		{
			source = source.Reverse();
		}
		return source.ToList();
	}

	/// <summary>
	/// Replace a simple variable with a value. The original value is removed and thus we cannot redo this in a later stage.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	internal string ExpandSimpleVariables(string? input)
	{
		string matchingVariableName;
		return ExpandSimpleVariables(input, out matchingVariableName);
	}

	internal string ExpandSimpleVariables(string? input, out string? matchingVariableName)
	{
		string text = input;
		matchingVariableName = null;
		if (text != null && !StringHelpers.IsNullOrWhiteSpace(text) && Variables.Count > 0 && text.IndexOf('$') >= 0)
		{
			StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
			foreach (KeyValuePair<string, Layout> variable in _variables)
			{
				Layout value = variable.Value;
				if (value == null)
				{
					continue;
				}
				string text2 = "${" + variable.Key + "}";
				if (text.IndexOf(text2, stringComparison) >= 0)
				{
					if (value is SimpleLayout simpleLayout)
					{
						text = StringHelpers.Replace(text, text2, simpleLayout.OriginalText, stringComparison);
						matchingVariableName = null;
					}
					else if (string.Equals(text2, input?.Trim() ?? string.Empty, stringComparison))
					{
						matchingVariableName = variable.Key;
					}
				}
			}
		}
		return text ?? string.Empty;
	}

	/// <summary>
	/// Checks whether unused targets exist. If found any, just write an internal log at Warn level.
	/// <remarks>If initializing not started or failed, then checking process will be canceled</remarks>
	/// </summary>
	internal void CheckUnusedTargets()
	{
		if (!InternalLogger.IsWarnEnabled)
		{
			return;
		}
		List<Target> allTargetsThreadSafe = GetAllTargetsThreadSafe();
		InternalLogger.Debug("Unused target checking is started... Rule Count: {0}, Target Count: {1}", LoggingRules.Count, allTargetsThreadSafe.Count);
		HashSet<string> targetNamesAtRules = new HashSet<string>(from t in GetLoggingRulesThreadSafe().SelectMany((LoggingRule r) => r.Targets)
			select t.Name);
		ReadOnlyCollection<Target> allTargets = AllTargets;
		ILookup<Target?, Target> wrappedTargets = allTargets.OfType<WrapperTargetBase>().ToLookup((Func<WrapperTargetBase, Target>)((WrapperTargetBase wt) => wt.WrappedTarget), (Func<WrapperTargetBase, Target>)((WrapperTargetBase wt) => wt));
		ILookup<Target?, Target> compoundTargets = allTargets.OfType<CompoundTargetBase>().SelectMany((CompoundTargetBase wt) => wt.Targets.Select((Target t) => new KeyValuePair<Target, Target>(t, wt))).ToLookup((KeyValuePair<Target, Target> p) => p.Key, (KeyValuePair<Target, Target> p) => p.Value);
		int arg = allTargetsThreadSafe.Count(delegate(Target target)
		{
			if (targetNamesAtRules.Contains(target.Name))
			{
				return false;
			}
			if (!IsUnusedInList(target, wrappedTargets))
			{
				return false;
			}
			if (!IsUnusedInList(target, compoundTargets))
			{
				return false;
			}
			InternalLogger.Warn("Unused target detected. Add a rule for this target to the configuration. TargetName: {0}", target.Name);
			return true;
		});
		InternalLogger.Debug("Unused target checking is completed. Total Rule Count: {0}, Total Target Count: {1}, Unused Target Count: {2}", LoggingRules.Count, allTargetsThreadSafe.Count, arg);
		bool IsUnusedInList(Target target1, ILookup<Target?, Target> targets)
		{
			if (targets.Contains(target1))
			{
				foreach (Target item in targets[target1])
				{
					if (targetNamesAtRules.Contains(item.Name))
					{
						return false;
					}
					if (wrappedTargets.Contains(item))
					{
						return false;
					}
					if (compoundTargets.Contains(item))
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	internal AsyncContinuation? FlushAllTargets(AsyncContinuation flushCompletion)
	{
		HashSet<Target> pendingTargets = GetAllTargetsToFlush();
		if (pendingTargets.Count == 0)
		{
			flushCompletion(null);
			return null;
		}
		InternalLogger.Trace("Flushing all {0} targets...", pendingTargets.Count);
		Exception lastException = null;
		Action<Target, Exception?> flushAction = delegate(Target t, Exception? ex)
		{
			if (ex != null)
			{
				InternalLogger.Warn(ex, "Flush failed for target {0}(Name={1})", t.GetType(), t.Name);
			}
			bool flag = false;
			lock (pendingTargets)
			{
				if (ex != null)
				{
					lastException = ex;
				}
				if (pendingTargets.Remove(t) && pendingTargets.Count == 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (lastException != null)
				{
					InternalLogger.Warn("Flush completed with errors");
				}
				else
				{
					InternalLogger.Debug("Flush completed");
				}
				flushCompletion(lastException);
			}
		};
		Target[] array = pendingTargets.ToArray();
		foreach (Target target in array)
		{
			Target flushTarget = target;
			AsyncHelpers.StartAsyncTask(delegate
			{
				try
				{
					flushTarget.Flush(delegate(Exception? ex)
					{
						flushAction(flushTarget, ex);
					});
				}
				catch (Exception arg)
				{
					flushAction(flushTarget, arg);
					throw;
				}
			}, null);
		}
		return delegate
		{
			lock (pendingTargets)
			{
				foreach (Target item in pendingTargets)
				{
					InternalLogger.Warn("Flush timeout for target {0}(Name={1})", item.GetType(), item.Name);
				}
				pendingTargets.Clear();
			}
		};
	}

	internal ITargetWithFilterChain[] BuildLoggerConfiguration(string loggerName, LogLevel globalLogLevel)
	{
		if (LoggingRules.Count == 0 || LogLevel.Off.Equals(globalLogLevel))
		{
			return TargetWithFilterChain.NoTargetsByLevel;
		}
		LoggingRule[] loggingRulesThreadSafe = GetLoggingRulesThreadSafe();
		TargetWithFilterChain[] array = TargetWithFilterChain.BuildLoggerConfiguration(loggerName, loggingRulesThreadSafe, globalLogLevel);
		if (InternalLogger.IsDebugEnabled && !DumpTargetConfigurationForLogger(loggerName, array))
		{
			InternalLogger.Debug("Targets not configured for Logger: {0}", loggerName);
		}
		return array ?? TargetWithFilterChain.NoTargetsByLevel;
	}

	private static bool DumpTargetConfigurationForLogger(string loggerName, TargetWithFilterChain[] targetsByLevel)
	{
		if (targetsByLevel == null)
		{
			return false;
		}
		StringBuilder stringBuilder = null;
		for (int i = 0; i <= LogLevel.MaxLevel.Ordinal; i++)
		{
			if (stringBuilder != null)
			{
				stringBuilder.Length = 0;
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Logger {0} [{1}] =>", loggerName, LogLevel.FromOrdinal(i));
			}
			for (TargetWithFilterChain targetWithFilterChain = targetsByLevel[i]; targetWithFilterChain != null; targetWithFilterChain = targetWithFilterChain.NextInChain)
			{
				if (stringBuilder == null)
				{
					InternalLogger.Debug("Targets configured when LogLevel >= {0} for Logger: {1}", LogLevel.FromOrdinal(i), loggerName);
					stringBuilder = new StringBuilder();
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Logger {0} [{1}] =>", loggerName, LogLevel.FromOrdinal(i));
				}
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " {0}", targetWithFilterChain.Target.Name);
				if (targetWithFilterChain.FilterChain.Count > 0)
				{
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0} filters)", targetWithFilterChain.FilterChain.Count);
				}
			}
			if (stringBuilder != null)
			{
				InternalLogger.Debug(stringBuilder.ToString());
			}
		}
		return stringBuilder != null;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		ICollection<Target> collection = GetAllTargetsToFlush();
		if (collection.Count == 0)
		{
			collection = GetAllTargetsThreadSafe();
		}
		if (collection.Count == 0)
		{
			collection = AllTargets;
		}
		if (collection.Count > 0 && collection.Count < 5)
		{
			return string.Format("TargetNames={0}, ConfigItems={1}", string.Join(", ", (from t in collection
				select t.Name into n
				where !string.IsNullOrEmpty(n)
				select n).ToArray()), _configItems.Count);
		}
		return $"Targets={collection.Count}, ConfigItems={_configItems.Count}";
	}
}

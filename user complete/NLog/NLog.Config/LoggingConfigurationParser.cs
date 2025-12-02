using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using NLog.Common;
using NLog.Filters;
using NLog.Internal;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;
using NLog.Time;

namespace NLog.Config;

/// <summary>
/// Loads NLog configuration from <see cref="T:NLog.Config.ILoggingConfigurationElement" />
/// </summary>
/// <remarks>
/// Make sure to update official NLog.xsd schema, when adding new config-options outside targets/layouts
/// </remarks>
public abstract class LoggingConfigurationParser : LoggingConfiguration
{
	/// <summary>
	/// Config element that's validated and having extra context
	/// </summary>
	private sealed class ValidatedConfigurationElement : ILoggingConfigurationElement
	{
		private readonly ILoggingConfigurationElement _element;

		private readonly bool _throwConfigExceptions;

		private IList<ValidatedConfigurationElement>? _validChildren;

		private readonly IDictionary<string, string?>? _valueLookup;

		public string Name { get; }

		public ICollection<KeyValuePair<string, string?>> Values
		{
			get
			{
				ICollection<KeyValuePair<string, string>> valueLookup = _valueLookup;
				return valueLookup ?? ArrayHelper.Empty<KeyValuePair<string, string>>();
			}
		}

		public IEnumerable<ValidatedConfigurationElement> ValidChildren
		{
			get
			{
				if (_validChildren == null)
				{
					return YieldAndCacheValidChildren();
				}
				return _validChildren;
			}
		}

		/// <remarks>
		/// Explicit cast because NET35 doesn't support covariance.
		/// </remarks>
		IEnumerable<ILoggingConfigurationElement> ILoggingConfigurationElement.Children => ValidChildren.Cast<ILoggingConfigurationElement>();

		IEnumerable<KeyValuePair<string, string?>> ILoggingConfigurationElement.Values => Values;

		public static ValidatedConfigurationElement Create(ILoggingConfigurationElement element, LogFactory logFactory)
		{
			if (element is ValidatedConfigurationElement result)
			{
				return result;
			}
			bool throwConfigExceptions = (logFactory.ThrowConfigExceptions ?? logFactory.ThrowExceptions) || (LogManager.ThrowConfigExceptions ?? LogManager.ThrowExceptions);
			return new ValidatedConfigurationElement(element, throwConfigExceptions);
		}

		public ValidatedConfigurationElement(ILoggingConfigurationElement element, bool throwConfigExceptions)
		{
			_throwConfigExceptions = throwConfigExceptions;
			Name = element.Name.Trim();
			_valueLookup = CreateValueLookup(element, throwConfigExceptions);
			_element = element;
		}

		private IEnumerable<ValidatedConfigurationElement> YieldAndCacheValidChildren()
		{
			IList<ValidatedConfigurationElement> validChildren = null;
			foreach (ILoggingConfigurationElement child in _element.Children)
			{
				validChildren = validChildren ?? new List<ValidatedConfigurationElement>();
				ValidatedConfigurationElement validatedConfigurationElement = new ValidatedConfigurationElement(child, _throwConfigExceptions);
				validChildren.Add(validatedConfigurationElement);
				yield return validatedConfigurationElement;
			}
			_validChildren = validChildren ?? ArrayHelper.Empty<ValidatedConfigurationElement>();
		}

		public string GetRequiredValue(string attributeName, string section)
		{
			string optionalValue = GetOptionalValue(attributeName, null);
			if (optionalValue == null)
			{
				throw new NLogConfigurationException("Expected " + attributeName + " on " + Name + " in " + section);
			}
			if (StringHelpers.IsNullOrWhiteSpace(optionalValue))
			{
				throw new NLogConfigurationException("Expected non-empty " + attributeName + " on " + Name + " in " + section);
			}
			return optionalValue;
		}

		public string? GetOptionalValue(string attributeName, string? defaultValue)
		{
			if (_valueLookup == null)
			{
				return defaultValue;
			}
			_valueLookup.TryGetValue(attributeName, out string value);
			return value ?? defaultValue;
		}

		private static IDictionary<string, string?>? CreateValueLookup(ILoggingConfigurationElement element, bool throwConfigExceptions)
		{
			IDictionary<string, string> dictionary = null;
			List<string> list = null;
			foreach (KeyValuePair<string, string> value in element.Values)
			{
				string text = value.Key?.Trim() ?? string.Empty;
				dictionary = dictionary ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				if (!string.IsNullOrEmpty(text) && !dictionary.ContainsKey(text))
				{
					dictionary[text] = value.Value;
					continue;
				}
				string text2 = (string.IsNullOrEmpty(text) ? ("Invalid property for '" + element.Name + "' without name. Value=" + value.Value) : ("Duplicate value for '" + element.Name + "'. PropertyName=" + text + ". Skips Value=" + value.Value + ". Existing Value=" + dictionary[text]));
				InternalLogger.Debug("Skipping {0}", text2);
				if (throwConfigExceptions)
				{
					list = list ?? new List<string>();
					list.Add(text2);
				}
			}
			if (throwConfigExceptions && list != null && list.Count > 0)
			{
				throw new NLogConfigurationException(StringHelpers.Join(Environment.NewLine, list));
			}
			return dictionary;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="logFactory"></param>
	protected LoggingConfigurationParser(LogFactory logFactory)
		: base(logFactory)
	{
	}

	/// <summary>
	/// Loads NLog configuration from provided config section
	/// </summary>
	/// <param name="nlogConfig"></param>
	/// <param name="basePath">Directory where the NLog-config-file was loaded from</param>
	protected void LoadConfig(ILoggingConfigurationElement nlogConfig, string? basePath)
	{
		InternalLogger.Trace("ParseNLogConfig");
		nlogConfig.AssertName("nlog");
		SetNLogElementSettings(nlogConfig);
		ValidatedConfigurationElement validatedConfigurationElement = ValidatedConfigurationElement.Create(nlogConfig, base.LogFactory);
		foreach (ValidatedConfigurationElement validChild in validatedConfigurationElement.ValidChildren)
		{
			if (validChild.MatchesName("extensions"))
			{
				ParseExtensionsElement(validChild, basePath);
			}
		}
		List<ValidatedConfigurationElement> list = new List<ValidatedConfigurationElement>();
		int count = base.LoggingRules.Count;
		foreach (ValidatedConfigurationElement validChild2 in validatedConfigurationElement.ValidChildren)
		{
			if (validChild2.MatchesName("rules"))
			{
				if (list.Count == 0)
				{
					count = base.LoggingRules.Count;
				}
				list.Add(validChild2);
			}
			else if (!validChild2.MatchesName("extensions") && !ParseNLogSection(validChild2))
			{
				NLogConfigurationException ex = new NLogConfigurationException("Unrecognized element '" + validChild2.Name + "' from section 'NLog'");
				if (MustThrowConfigException(ex))
				{
					throw ex;
				}
			}
		}
		foreach (ValidatedConfigurationElement item in list)
		{
			ParseRulesElement(item, base.LoggingRules, count);
		}
	}

	private void SetNLogElementSettings(ILoggingConfigurationElement nlogConfig)
	{
		ICollection<KeyValuePair<string, string?>> collection = CreateUniqueSortedListFromConfig(nlogConfig);
		CultureInfo cultureInfo = base.DefaultCultureInfo ?? base.LogFactory._defaultCultureInfo;
		bool? enable = base.LogFactory.ServiceRepository.ResolveParseMessageTemplates();
		bool flag = false;
		bool flag2 = false;
		foreach (KeyValuePair<string, string> item in collection)
		{
			switch (item.Key.ToUpperInvariant())
			{
			case "THROWEXCEPTIONS":
				base.LogFactory.ThrowExceptions = ParseBooleanValue(item.Key, item.Value ?? string.Empty, base.LogFactory.ThrowExceptions);
				continue;
			case "THROWCONFIGEXCEPTIONS":
				base.LogFactory.ThrowConfigExceptions = ParseNullableBooleanValue(item.Key, item.Value ?? string.Empty, defaultValue: false);
				continue;
			case "INTERNALLOGLEVEL":
				InternalLogger.LogLevel = ParseLogLevelSafe(item.Key, item.Value ?? string.Empty, InternalLogger.LogLevel);
				flag = InternalLogger.LogLevel != LogLevel.Off;
				continue;
			case "USEINVARIANTCULTURE":
				if (ParseBooleanValue(item.Key, item.Value ?? string.Empty, defaultValue: false))
				{
					CultureInfo cultureInfo2 = (base.DefaultCultureInfo = CultureInfo.InvariantCulture);
					cultureInfo = cultureInfo2;
				}
				continue;
			case "KEEPVARIABLESONRELOAD":
				base.LogFactory.KeepVariablesOnReload = ParseBooleanValue(item.Key, item.Value ?? string.Empty, base.LogFactory.KeepVariablesOnReload);
				continue;
			case "INTERNALLOGTOCONSOLE":
				InternalLogger.LogToConsole = ParseBooleanValue(item.Key, item.Value ?? string.Empty, InternalLogger.LogToConsole);
				continue;
			case "INTERNALLOGTOCONSOLEERROR":
				InternalLogger.LogToConsoleError = ParseBooleanValue(item.Key, item.Value ?? string.Empty, InternalLogger.LogToConsoleError);
				continue;
			case "INTERNALLOGFILE":
				InternalLogger.LogFile = item.Value?.Trim();
				continue;
			case "INTERNALLOGINCLUDETIMESTAMP":
				InternalLogger.IncludeTimestamp = ParseBooleanValue(item.Key, item.Value ?? string.Empty, InternalLogger.IncludeTimestamp);
				continue;
			case "GLOBALTHRESHOLD":
				base.LogFactory.GlobalThreshold = ParseLogLevelSafe(item.Key, item.Value ?? string.Empty, base.LogFactory.GlobalThreshold);
				continue;
			case "PARSEMESSAGETEMPLATES":
				enable = ParseNullableBooleanValue(item.Key, item.Value ?? string.Empty, defaultValue: true);
				continue;
			case "AUTOSHUTDOWN":
				base.LogFactory.AutoShutdown = ParseBooleanValue(item.Key, item.Value ?? string.Empty, defaultValue: true);
				continue;
			case "AUTOLOADEXTENSIONS":
				flag2 = ParseBooleanValue(item.Key, item.Value ?? string.Empty, defaultValue: false);
				continue;
			case "AUTORELOAD":
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Unrecognized value '" + item.Key + "'='" + item.Value + "' for element '" + nlogConfig.Name + "'");
			if (MustThrowConfigException(ex))
			{
				throw ex;
			}
		}
		if (cultureInfo != null && base.DefaultCultureInfo != cultureInfo)
		{
			base.DefaultCultureInfo = cultureInfo;
		}
		if (!flag && !InternalLogger.HasActiveLoggers())
		{
			InternalLogger.LogLevel = LogLevel.Off;
		}
		if (flag2)
		{
			ScanForAutoLoadExtensions();
		}
		base.LogFactory.ServiceRepository.ParseMessageTemplates(base.LogFactory, enable);
	}

	[UnconditionalSuppressMessage("Trimming - Allow extension loading from config", "IL2026")]
	private static void ScanForAutoLoadExtensions()
	{
		ConfigurationItemFactory.Default.AssemblyLoader.ScanForAutoLoadExtensions(ConfigurationItemFactory.Default);
	}

	/// <summary>
	/// Builds list with unique keys, using last value of duplicates. High priority keys placed first.
	/// </summary>
	/// <param name="nlogConfig"></param>
	/// <returns></returns>
	private ICollection<KeyValuePair<string, string?>> CreateUniqueSortedListFromConfig(ILoggingConfigurationElement nlogConfig)
	{
		ValidatedConfigurationElement validatedConfigurationElement = ValidatedConfigurationElement.Create(nlogConfig, base.LogFactory);
		ICollection<KeyValuePair<string, string>> values = validatedConfigurationElement.Values;
		if (values.Count <= 1)
		{
			return values;
		}
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(values.Count);
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "ThrowExceptions", "ThrowConfigExceptions", "InternalLogLevel", "InternalLogFile", "InternalLogToConsole" };
		foreach (string item in hashSet)
		{
			string optionalValue = validatedConfigurationElement.GetOptionalValue(item, null);
			if (optionalValue != null)
			{
				list.Add(new KeyValuePair<string, string>(item, optionalValue));
			}
		}
		foreach (KeyValuePair<string, string> value in validatedConfigurationElement.Values)
		{
			if (!hashSet.Contains(value.Key))
			{
				list.Add(value);
			}
		}
		return list;
	}

	/// <summary>
	/// Parse loglevel, but don't throw if exception throwing is disabled
	/// </summary>
	/// <param name="propertyName">Name of attribute for logging.</param>
	/// <param name="propertyValue">Value of parse.</param>
	/// <param name="fallbackValue">Used if there is an exception</param>
	/// <returns></returns>
	private LogLevel ParseLogLevelSafe(string propertyName, string propertyValue, LogLevel fallbackValue)
	{
		try
		{
			return LogLevel.FromString(propertyValue?.Trim() ?? string.Empty);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException($"Property '{propertyName}' assigned invalid LogLevel value '{propertyValue}'. Fallback to '{fallbackValue}'", ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
			return fallbackValue;
		}
	}

	/// <summary>
	/// Parses a single config section within the NLog-config
	/// </summary>
	/// <param name="configSection"></param>
	/// <returns>Section was recognized</returns>
	protected virtual bool ParseNLogSection(ILoggingConfigurationElement configSection)
	{
		switch (configSection.Name?.Trim().ToUpperInvariant())
		{
		case "TIME":
			ParseTimeElement(ValidatedConfigurationElement.Create(configSection, base.LogFactory));
			return true;
		case "VARIABLE":
			ParseVariableElement(ValidatedConfigurationElement.Create(configSection, base.LogFactory));
			return true;
		case "VARIABLES":
			ParseVariablesElement(ValidatedConfigurationElement.Create(configSection, base.LogFactory));
			return true;
		case "APPENDERS":
		case "TARGETS":
			ParseTargetsElement(ValidatedConfigurationElement.Create(configSection, base.LogFactory));
			return true;
		default:
			return false;
		}
	}

	private void ParseExtensionsElement(ValidatedConfigurationElement extensionsElement, string? baseDirectory)
	{
		extensionsElement.AssertName("extensions");
		foreach (ValidatedConfigurationElement validChild in extensionsElement.ValidChildren)
		{
			string text = string.Empty;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			foreach (KeyValuePair<string, string> value in validChild.Values)
			{
				if (MatchesName(value.Key, "prefix"))
				{
					text = value.Value + ".";
					continue;
				}
				if (MatchesName(value.Key, "type"))
				{
					text2 = value.Value;
					continue;
				}
				if (MatchesName(value.Key, "assemblyFile"))
				{
					text3 = value.Value;
					continue;
				}
				if (MatchesName(value.Key, "assembly"))
				{
					text4 = value.Value;
					continue;
				}
				NLogConfigurationException ex = new NLogConfigurationException("Unrecognized value '" + value.Key + "'='" + value.Value + "' for element '" + validChild.Name + "' in section '" + extensionsElement.Name + "'");
				if (!MustThrowConfigException(ex))
				{
					continue;
				}
				throw ex;
			}
			if (text2 != null && !StringHelpers.IsNullOrWhiteSpace(text2))
			{
				RegisterExtension(text2, text);
			}
			if (text3 != null && !StringHelpers.IsNullOrWhiteSpace(text3))
			{
				ParseExtensionWithAssemblyFile(text3, baseDirectory, text);
			}
			else if (text4 != null && !StringHelpers.IsNullOrWhiteSpace(text4))
			{
				ParseExtensionWithAssemblyName(text4.Trim(), text);
			}
		}
	}

	[UnconditionalSuppressMessage("Trimming - Allow extension loading from config", "IL2072")]
	private void RegisterExtension(string typeName, string itemNamePrefix)
	{
		try
		{
			Type type = PropertyTypeConverter.ConvertToType(typeName, throwOnError: true);
			ConfigurationItemFactory.Default.RegisterType(type, itemNamePrefix);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException("Error loading extensions: " + typeName, ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
		}
	}

	[UnconditionalSuppressMessage("Trimming - Allow extension loading from config", "IL2026")]
	private void ParseExtensionWithAssemblyFile(string assemblyFile, string? baseDirectory, string prefix)
	{
		try
		{
			ConfigurationItemFactory.Default.AssemblyLoader.LoadAssemblyFromPath(ConfigurationItemFactory.Default, assemblyFile, baseDirectory, prefix);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException("Error loading extensions: " + assemblyFile, ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
		}
	}

	private bool RegisterExtensionFromAssemblyName(string assemblyName, string originalTypeName)
	{
		InternalLogger.Debug("Loading Assembly-Name '{0}' for type: {1}", assemblyName, originalTypeName);
		return ParseExtensionWithAssemblyName(assemblyName, string.Empty);
	}

	[UnconditionalSuppressMessage("Trimming - Allow extension loading from config", "IL2026")]
	private bool ParseExtensionWithAssemblyName(string assemblyName, string itemNamePrefix)
	{
		try
		{
			ConfigurationItemFactory.Default.AssemblyLoader.LoadAssemblyFromName(ConfigurationItemFactory.Default, assemblyName, itemNamePrefix);
			return true;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException("Error loading extensions: " + assemblyName, ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
		}
		return false;
	}

	private void ParseVariableElement(ValidatedConfigurationElement variableElement)
	{
		string text = null;
		string text2 = null;
		foreach (KeyValuePair<string, string> value in variableElement.Values)
		{
			if (MatchesName(value.Key, "name"))
			{
				text = value.Value;
				continue;
			}
			if (MatchesName(value.Key, "value") || MatchesName(value.Key, "layout"))
			{
				text2 = value.Value;
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Unrecognized value '" + value.Key + "'='" + value.Value + "' for element '" + variableElement.Name + "' in section 'variables'");
			if (!MustThrowConfigException(ex))
			{
				continue;
			}
			throw ex;
		}
		if (AssertNonEmptyValue(text, "name", variableElement.Name, "variables") && text != null)
		{
			Layout layout = ((text2 == null) ? ParseVariableLayoutValue(variableElement) : CreateSimpleLayout(ExpandSimpleVariables(text2)));
			if (AssertNotNullValue(layout, "value", variableElement.Name, "variables") && layout != null)
			{
				InsertParsedConfigVariable(text, layout);
			}
		}
	}

	private Layout? ParseVariableLayoutValue(ValidatedConfigurationElement variableElement)
	{
		ValidatedConfigurationElement validatedConfigurationElement = variableElement.ValidChildren.FirstOrDefault();
		if (validatedConfigurationElement != null)
		{
			return TryCreateLayoutInstance(validatedConfigurationElement, typeof(Layout));
		}
		return null;
	}

	private void ParseVariablesElement(ValidatedConfigurationElement variableElement)
	{
		variableElement.AssertName("variables");
		foreach (ValidatedConfigurationElement validChild in variableElement.ValidChildren)
		{
			ParseVariableElement(validChild);
		}
	}

	private void ParseTimeElement(ValidatedConfigurationElement timeElement)
	{
		timeElement.AssertName("time");
		string text = null;
		foreach (KeyValuePair<string, string> value in timeElement.Values)
		{
			if (MatchesName(value.Key, "type"))
			{
				text = value.Value;
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Unrecognized value '" + value.Key + "'='" + value.Value + "' for element '" + timeElement.Name + "'");
			if (!MustThrowConfigException(ex))
			{
				continue;
			}
			throw ex;
		}
		if (AssertNonEmptyValue(text, "type", timeElement.Name, string.Empty) && text != null)
		{
			TimeSource timeSource = FactoryCreateInstance(text, ConfigurationItemFactory.Default.TimeSourceFactory);
			if (timeSource != null)
			{
				ConfigureFromAttributesAndElements(timeSource, timeElement);
				InternalLogger.Info("Selecting time source {0}", timeSource);
				TimeSource.Current = timeSource;
			}
		}
	}

	[ContractAnnotation("value:notnull => true")]
	private bool AssertNotNullValue(object? value, string propertyName, string elementName, string sectionName)
	{
		if (value == null)
		{
			return AssertNonEmptyValue(string.Empty, propertyName, elementName, sectionName);
		}
		return true;
	}

	[ContractAnnotation("value:null => false")]
	private bool AssertNonEmptyValue(string? value, string propertyName, string elementName, string sectionName)
	{
		if (!StringHelpers.IsNullOrWhiteSpace(value))
		{
			return true;
		}
		NLogConfigurationException ex = new NLogConfigurationException("Property '" + propertyName + "' has blank value, for element '" + elementName + "' in section '" + sectionName + "'");
		if (MustThrowConfigException(ex))
		{
			throw ex;
		}
		return false;
	}

	/// <summary>
	/// Parse {Rules} xml element
	/// </summary>
	private void ParseRulesElement(ValidatedConfigurationElement rulesElement, IList<LoggingRule> rulesCollection, int rulesInsertPosition)
	{
		InternalLogger.Trace("ParseRulesElement");
		rulesElement.AssertName("rules");
		if (rulesInsertPosition > rulesCollection.Count)
		{
			rulesInsertPosition = rulesCollection.Count;
		}
		foreach (ValidatedConfigurationElement validChild in rulesElement.ValidChildren)
		{
			LoggingRule loggingRule = ParseRuleElement(validChild);
			if (loggingRule != null)
			{
				lock (rulesCollection)
				{
					rulesCollection.Insert(rulesInsertPosition++, loggingRule);
				}
			}
		}
	}

	/// <summary>
	/// Parse {Logger} xml element
	/// </summary>
	/// <param name="loggerElement"></param>
	private LoggingRule? ParseRuleElement(ValidatedConfigurationElement loggerElement)
	{
		string minLevel = null;
		string maxLevel = null;
		string finalMinLevel = null;
		string enableLevels = null;
		string text = null;
		string text2 = null;
		bool flag = true;
		bool flag2 = false;
		string text3 = null;
		string filterDefaultAction = null;
		foreach (KeyValuePair<string, string> value in loggerElement.Values)
		{
			switch (value.Key?.Trim().ToUpperInvariant())
			{
			case "NAME":
				if (loggerElement.MatchesName("logger"))
				{
					text2 = value.Value;
				}
				else
				{
					text = value.Value;
				}
				continue;
			case "RULENAME":
				text = value.Value;
				continue;
			case "LOGGER":
				text2 = value.Value;
				continue;
			case "ENABLED":
				flag = ParseBooleanValue(value.Key, value.Value ?? string.Empty, defaultValue: true);
				continue;
			case "APPENDTO":
				text3 = value.Value;
				continue;
			case "WRITETO":
				text3 = value.Value;
				continue;
			case "FINAL":
				flag2 = ParseBooleanValue(value.Key, value.Value ?? string.Empty, defaultValue: false);
				continue;
			case "LEVELS":
			case "LEVEL":
				enableLevels = value.Value;
				continue;
			case "MINLEVEL":
				minLevel = value.Value;
				continue;
			case "MAXLEVEL":
				maxLevel = value.Value;
				continue;
			case "FINALMINLEVEL":
				finalMinLevel = value.Value;
				continue;
			case "FILTERDEFAULTACTION":
				filterDefaultAction = value.Value;
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Unrecognized value '" + value.Key + "'='" + value.Value + "' for element '" + loggerElement.Name + "' in section 'rules'");
			if (MustThrowConfigException(ex))
			{
				throw ex;
			}
		}
		if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2) && string.IsNullOrEmpty(text3) && !flag2)
		{
			InternalLogger.Debug("Logging rule without name or filter or targets is ignored");
			return null;
		}
		text2 = text2 ?? "*";
		if (!flag)
		{
			InternalLogger.Debug("Logging rule {0} with name pattern `{1}` is disabled", text, text2);
			return null;
		}
		LoggingRule loggingRule = new LoggingRule(text)
		{
			LoggerNamePattern = text2,
			Final = flag2
		};
		EnableLevelsForRule(loggingRule, enableLevels, minLevel, maxLevel, finalMinLevel);
		ParseLoggingRuleTargets(text3, loggingRule);
		ParseLoggingRuleChildren(loggerElement, loggingRule, filterDefaultAction);
		ValidateLoggingRuleFilters(loggingRule);
		return loggingRule;
	}

	private void EnableLevelsForRule(LoggingRule rule, string? enableLevels, string? minLevel, string? maxLevel, string? finalMinLevel)
	{
		if (enableLevels != null)
		{
			enableLevels = ExpandSimpleVariables(enableLevels).Trim();
			finalMinLevel = ExpandSimpleVariables(finalMinLevel).Trim();
			if (IsLevelLayout(enableLevels) || IsLevelLayout(finalMinLevel))
			{
				SimpleLayout simpleLayout = ParseLevelLayout(enableLevels);
				SimpleLayout finalMinLevel2 = ParseLevelLayout(finalMinLevel);
				rule.EnableLoggingForLevelLayout(simpleLayout, finalMinLevel2);
				return;
			}
			string[] array = enableLevels.SplitAndTrimTokens(',');
			foreach (string levelName in array)
			{
				rule.EnableLoggingForLevel(LogLevel.FromString(levelName));
			}
			if (!string.IsNullOrEmpty(finalMinLevel))
			{
				rule.FinalMinLevel = LogLevel.FromString(finalMinLevel);
			}
			return;
		}
		minLevel = ExpandSimpleVariables(minLevel).Trim();
		maxLevel = ExpandSimpleVariables(maxLevel).Trim();
		finalMinLevel = ExpandSimpleVariables(finalMinLevel).Trim();
		if (IsLevelLayout(minLevel) || IsLevelLayout(maxLevel) || IsLevelLayout(finalMinLevel))
		{
			SimpleLayout simpleLayout2 = ParseLevelLayout(finalMinLevel);
			SimpleLayout minLevel2 = ParseLevelLayout(minLevel) ?? simpleLayout2;
			SimpleLayout maxLevel2 = ParseLevelLayout(maxLevel);
			rule.EnableLoggingForLevelsLayout(minLevel2, maxLevel2, simpleLayout2);
			return;
		}
		LogLevel logLevel = (string.IsNullOrEmpty(finalMinLevel) ? null : LogLevel.FromString(finalMinLevel));
		LogLevel minLevel3 = (string.IsNullOrEmpty(minLevel) ? (logLevel ?? LogLevel.MinLevel) : LogLevel.FromString(minLevel));
		LogLevel maxLevel3 = (string.IsNullOrEmpty(maxLevel) ? LogLevel.MaxLevel : LogLevel.FromString(maxLevel));
		rule.SetLoggingLevels(minLevel3, maxLevel3);
		if (logLevel != null)
		{
			rule.FinalMinLevel = logLevel;
		}
	}

	private static bool IsLevelLayout(string? level)
	{
		if (level == null)
		{
			return false;
		}
		return level.IndexOf('{') >= 0;
	}

	private SimpleLayout? ParseLevelLayout(string levelLayout)
	{
		if (levelLayout == null || StringHelpers.IsNullOrWhiteSpace(levelLayout))
		{
			return null;
		}
		SimpleLayout simpleLayout = CreateSimpleLayout(levelLayout);
		simpleLayout.Initialize(this);
		return simpleLayout;
	}

	private void ParseLoggingRuleTargets(string? writeTargets, LoggingRule rule)
	{
		writeTargets = ExpandSimpleVariables(writeTargets).Trim();
		if (string.IsNullOrEmpty(writeTargets))
		{
			return;
		}
		string[] array = writeTargets.SplitAndTrimTokens(',');
		foreach (string text in array)
		{
			Target target = FindTargetByName(text);
			if (target != null)
			{
				rule.Targets.Add(target);
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Target '" + text + "' not found for logging rule: " + (string.IsNullOrEmpty(rule.RuleName) ? rule.LoggerNamePattern : rule.RuleName) + ".");
			if (MustThrowConfigException(ex))
			{
				throw ex;
			}
		}
	}

	[Obsolete("Very exotic feature without any unit-tests, not sure if it works. Marked obsolete with NLog v5.3")]
	private void ParseLoggingRuleChildren(ValidatedConfigurationElement loggerElement, LoggingRule rule, string? filterDefaultAction = null)
	{
		foreach (ValidatedConfigurationElement validChild in loggerElement.ValidChildren)
		{
			LoggingRule loggingRule = null;
			if (validChild.MatchesName("filters"))
			{
				ParseLoggingRuleFilters(rule, validChild, filterDefaultAction);
			}
			else if (validChild.MatchesName("logger") && loggerElement.MatchesName("logger"))
			{
				loggingRule = ParseRuleElement(validChild);
			}
			else if (validChild.MatchesName("rule") && loggerElement.MatchesName("rule"))
			{
				loggingRule = ParseRuleElement(validChild);
			}
			else
			{
				NLogConfigurationException ex = new NLogConfigurationException("Unrecognized child element '" + validChild.Name + "' for element '" + loggerElement.Name + "' in section 'rules'");
				if (MustThrowConfigException(ex))
				{
					throw ex;
				}
			}
			if (loggingRule != null)
			{
				ValidateLoggingRuleFilters(rule);
				lock (rule.ChildRules)
				{
					rule.ChildRules.Add(loggingRule);
				}
			}
		}
	}

	private void ParseLoggingRuleFilters(LoggingRule rule, ValidatedConfigurationElement filtersElement, string? filterDefaultAction = null)
	{
		filtersElement.AssertName("filters");
		filterDefaultAction = filtersElement.GetOptionalValue("defaultAction", null) ?? filtersElement.GetOptionalValue("FilterDefaultAction", null) ?? filterDefaultAction;
		if (filterDefaultAction != null)
		{
			if (ConversionHelpers.TryParseEnum(filterDefaultAction, typeof(FilterResult), out object resultValue) && resultValue != null)
			{
				rule.FilterDefaultAction = (FilterResult)resultValue;
			}
			else
			{
				NLogConfigurationException ex = new NLogConfigurationException("Failed to parse Enum-value to assign property 'FilterDefaultAction'='" + filterDefaultAction + "' for logging rule: " + (string.IsNullOrEmpty(rule.RuleName) ? rule.LoggerNamePattern : rule.RuleName) + ".");
				if (MustThrowConfigException(ex))
				{
					throw ex;
				}
			}
		}
		foreach (ValidatedConfigurationElement validChild in filtersElement.ValidChildren)
		{
			string typeName = validChild.GetOptionalValue("type", null) ?? validChild.Name;
			Filter filter = FactoryCreateInstance(typeName, ConfigurationItemFactory.Default.FilterFactory);
			if (filter != null)
			{
				ConfigureFromAttributesAndElements(filter, validChild);
				rule.Filters.Add(filter);
			}
		}
	}

	private void ValidateLoggingRuleFilters(LoggingRule rule)
	{
		bool flag = rule.Filters.Count == 0 || rule.FilterDefaultAction != FilterResult.Ignore;
		for (int i = 0; i < rule.Filters.Count; i++)
		{
			if (rule.Filters[i].Action != FilterResult.Ignore && rule.Filters[i].Action != FilterResult.IgnoreFinal && rule.Filters[i].Action != FilterResult.Neutral)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			NLogConfigurationException ex = new NLogConfigurationException($"LoggingRule where all filters and FilterDefaultAction=Ignore : {rule}");
			if (MustThrowConfigException(ex))
			{
				throw ex;
			}
		}
	}

	private void ParseTargetsElement(ValidatedConfigurationElement targetsElement)
	{
		targetsElement.AssertName("targets", "appenders");
		bool asyncWrap = ParseBooleanValue("async", targetsElement.GetOptionalValue("async", "false") ?? string.Empty, defaultValue: false);
		ValidatedConfigurationElement defaultWrapperElement = null;
		Dictionary<string, ValidatedConfigurationElement> typeNameToDefaultTargetParameters = null;
		foreach (ValidatedConfigurationElement validChild in targetsElement.ValidChildren)
		{
			string configItemTypeAttribute = validChild.GetConfigItemTypeAttribute();
			string text = validChild.GetOptionalValue("name", null) ?? string.Empty;
			text = (string.IsNullOrEmpty(text) ? (validChild.Name ?? string.Empty) : (validChild.Name + "(Name=" + text + ")"));
			switch (validChild.Name?.Trim().ToUpperInvariant())
			{
			case "DEFAULT-WRAPPER":
			case "TARGETDEFAULTWRAPPER":
				if (AssertNonEmptyValue(configItemTypeAttribute, "type", text, targetsElement.Name))
				{
					defaultWrapperElement = validChild;
				}
				continue;
			case "DEFAULT-TARGET-PARAMETERS":
			case "TARGETDEFAULTPARAMETERS":
				if (AssertNonEmptyValue(configItemTypeAttribute, "type", text, targetsElement.Name))
				{
					typeNameToDefaultTargetParameters = RegisterNewTargetDefaultParameters(typeNameToDefaultTargetParameters, validChild, configItemTypeAttribute);
				}
				continue;
			case "COMPOUND-TARGET":
			case "TARGET":
			case "APPENDER":
			case "WRAPPER":
			case "WRAPPER-TARGET":
				if (AssertNonEmptyValue(configItemTypeAttribute, "type", text, targetsElement.Name))
				{
					AddNewTargetFromConfig(configItemTypeAttribute, validChild, asyncWrap, typeNameToDefaultTargetParameters, defaultWrapperElement);
				}
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Unrecognized element '" + text + "' in section '" + targetsElement.Name + "'");
			if (MustThrowConfigException(ex))
			{
				throw ex;
			}
		}
	}

	private static Dictionary<string, ValidatedConfigurationElement> RegisterNewTargetDefaultParameters(Dictionary<string, ValidatedConfigurationElement>? typeNameToDefaultTargetParameters, ValidatedConfigurationElement targetElement, string targetTypeName)
	{
		if (typeNameToDefaultTargetParameters == null)
		{
			typeNameToDefaultTargetParameters = new Dictionary<string, ValidatedConfigurationElement>(StringComparer.OrdinalIgnoreCase);
		}
		typeNameToDefaultTargetParameters[targetTypeName.Trim()] = targetElement;
		return typeNameToDefaultTargetParameters;
	}

	private void AddNewTargetFromConfig(string targetTypeName, ValidatedConfigurationElement targetElement, bool asyncWrap, Dictionary<string, ValidatedConfigurationElement>? typeNameToDefaultTargetParameters = null, ValidatedConfigurationElement? defaultWrapperElement = null)
	{
		Target target = null;
		try
		{
			target = CreateTargetType(targetTypeName);
			if (target != null)
			{
				ParseTargetElement(target, targetElement, typeNameToDefaultTargetParameters);
				if (asyncWrap)
				{
					target = WrapWithAsyncTargetWrapper(target);
				}
				if (defaultWrapperElement != null)
				{
					target = WrapWithDefaultWrapper(target, defaultWrapperElement);
				}
				AddTarget(target);
			}
		}
		catch (NLogConfigurationException configException)
		{
			if (MustThrowConfigException(configException))
			{
				throw;
			}
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException configException2 = new NLogConfigurationException("Target '" + (target?.ToString() ?? targetTypeName) + "' has invalid config. Error: " + ex.Message, ex);
			if (MustThrowConfigException(configException2))
			{
				throw;
			}
		}
	}

	private Target? CreateTargetType(string targetTypeName)
	{
		return FactoryCreateInstance(targetTypeName, ConfigurationItemFactory.Default.TargetFactory);
	}

	private void ParseTargetElement(Target target, ValidatedConfigurationElement targetElement, Dictionary<string, ValidatedConfigurationElement>? typeNameToDefaultTargetParameters = null)
	{
		string configItemTypeAttribute = targetElement.GetConfigItemTypeAttribute("targets");
		if (typeNameToDefaultTargetParameters != null && typeNameToDefaultTargetParameters.TryGetValue(configItemTypeAttribute, out ValidatedConfigurationElement value))
		{
			ParseTargetElement(target, value);
		}
		CompoundTargetBase compoundTargetBase = target as CompoundTargetBase;
		WrapperTargetBase wrapperTargetBase = target as WrapperTargetBase;
		ConfigureObjectFromAttributes(target, targetElement);
		foreach (ValidatedConfigurationElement validChild in targetElement.ValidChildren)
		{
			if ((compoundTargetBase == null || !ParseCompoundTarget(compoundTargetBase, validChild, typeNameToDefaultTargetParameters, null)) && (wrapperTargetBase == null || !ParseTargetWrapper(wrapperTargetBase, validChild, typeNameToDefaultTargetParameters)))
			{
				SetPropertyValuesFromElement(target, validChild, targetElement);
			}
		}
	}

	private bool ParseTargetWrapper(WrapperTargetBase wrapper, ValidatedConfigurationElement childElement, Dictionary<string, ValidatedConfigurationElement>? typeNameToDefaultTargetParameters)
	{
		if (IsTargetRefElement(childElement.Name))
		{
			string requiredValue = childElement.GetRequiredValue("name", GetName(wrapper));
			Target target = FindTargetByName(requiredValue);
			if (target == null)
			{
				NLogConfigurationException ex = new NLogConfigurationException("Referenced target '" + requiredValue + "' not found.");
				if (MustThrowConfigException(ex))
				{
					throw ex;
				}
			}
			wrapper.WrappedTarget = target;
			return true;
		}
		if (IsTargetElement(childElement.Name))
		{
			string configItemTypeAttribute = childElement.GetConfigItemTypeAttribute(GetName(wrapper));
			Target target2 = CreateTargetType(configItemTypeAttribute);
			if (target2 != null)
			{
				ParseTargetElement(target2, childElement, typeNameToDefaultTargetParameters);
				if (!string.IsNullOrEmpty(target2.Name))
				{
					AddTarget(target2.Name, target2);
				}
				else if (!string.IsNullOrEmpty(wrapper.Name))
				{
					target2.Name = wrapper.Name + "_wrapped";
				}
				if (wrapper.WrappedTarget != null)
				{
					NLogConfigurationException ex2 = new NLogConfigurationException("Failed to assign wrapped target " + configItemTypeAttribute + ", because target " + wrapper.Name + " already has one.");
					if (MustThrowConfigException(ex2))
					{
						throw ex2;
					}
				}
			}
			wrapper.WrappedTarget = target2;
			return true;
		}
		return false;
	}

	private bool ParseCompoundTarget(CompoundTargetBase compound, ValidatedConfigurationElement childElement, Dictionary<string, ValidatedConfigurationElement>? typeNameToDefaultTargetParameters, string? targetName)
	{
		if (MatchesName(childElement.Name, "targets") || MatchesName(childElement.Name, "appenders"))
		{
			foreach (ValidatedConfigurationElement validChild in childElement.ValidChildren)
			{
				ParseCompoundTarget(compound, validChild, typeNameToDefaultTargetParameters, null);
			}
			return true;
		}
		if (IsTargetRefElement(childElement.Name))
		{
			targetName = childElement.GetRequiredValue("name", GetName(compound));
			Target target = FindTargetByName(targetName);
			if (target == null)
			{
				throw new NLogConfigurationException("Referenced target '" + targetName + "' not found.");
			}
			compound.Targets.Add(target);
			return true;
		}
		if (IsTargetElement(childElement.Name))
		{
			string configItemTypeAttribute = childElement.GetConfigItemTypeAttribute(GetName(compound));
			Target target2 = CreateTargetType(configItemTypeAttribute);
			if (target2 != null)
			{
				if (targetName != null)
				{
					target2.Name = targetName;
				}
				ParseTargetElement(target2, childElement, typeNameToDefaultTargetParameters);
				if (target2.Name != null)
				{
					AddTarget(target2.Name, target2);
				}
				compound.Targets.Add(target2);
			}
			return true;
		}
		return false;
	}

	private void ConfigureObjectFromAttributes<T>(T targetObject, ValidatedConfigurationElement element, bool ignoreType = true) where T : class
	{
		foreach (KeyValuePair<string, string> value2 in element.Values)
		{
			string key = value2.Key;
			string value = value2.Value;
			if (!ignoreType || !MatchesName(key, "type"))
			{
				SetPropertyValueFromString(targetObject, key, value, element);
			}
		}
	}

	private void SetPropertyValueFromString<T>(T targetObject, string propertyName, string? propertyValue, ValidatedConfigurationElement element) where T : class
	{
		try
		{
			if (targetObject == null)
			{
				throw new NLogConfigurationException("'" + typeof(T).Name + "' is null, and cannot assign property '" + propertyName + "'='" + propertyValue + "'");
			}
			if (!PropertyHelper.TryGetPropertyInfo(ConfigurationItemFactory.Default, targetObject, propertyName, out PropertyInfo result) || (object)result == null)
			{
				throw new NLogConfigurationException("'" + targetObject.GetType()?.Name + "' cannot assign unknown property '" + propertyName + "'='" + propertyValue + "'");
			}
			string matchingVariableName;
			string stringValue = ExpandSimpleVariables(propertyValue, out matchingVariableName);
			if (matchingVariableName != null && TryLookupDynamicVariable(matchingVariableName, out Layout value) && result.PropertyType.IsAssignableFrom(value.GetType()))
			{
				PropertyHelper.SetPropertyValueForObject(targetObject, value, result);
			}
			else
			{
				PropertyHelper.SetPropertyFromString(targetObject, result, stringValue, ConfigurationItemFactory.Default);
			}
		}
		catch (NLogConfigurationException configException)
		{
			if (MustThrowConfigException(configException))
			{
				throw;
			}
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException configException2 = new NLogConfigurationException("'" + targetObject.GetType()?.Name + "' cannot assign property '" + propertyName + "'='" + propertyValue + "' in section '" + element.Name + "'. Error: " + ex.Message, ex);
			if (MustThrowConfigException(configException2))
			{
				throw;
			}
		}
	}

	private void SetPropertyValuesFromElement<T>(T targetObject, ValidatedConfigurationElement childElement, ILoggingConfigurationElement parentElement) where T : class
	{
		PropertyInfo result;
		object propertyValue;
		if (targetObject == null)
		{
			NLogConfigurationException ex = new NLogConfigurationException("'" + typeof(T).Name + "' is null, and cannot assign property '" + childElement.Name + "' in section '" + parentElement.Name + "'");
			if (MustThrowConfigException(ex))
			{
				throw ex;
			}
		}
		else if (!PropertyHelper.TryGetPropertyInfo(ConfigurationItemFactory.Default, targetObject, childElement.Name, out result) || (object)result == null)
		{
			NLogConfigurationException ex2 = new NLogConfigurationException("'" + targetObject.GetType()?.Name + "' cannot assign unknown property '" + childElement.Name + "' in section '" + parentElement.Name + "'");
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
		}
		else if (!AddArrayItemFromElement(targetObject, result, childElement) && !SetLayoutFromElement(targetObject, result, childElement) && !SetFilterFromElement(targetObject, result, childElement) && TryGetPropertyValue(targetObject, result, out propertyValue) && propertyValue != null)
		{
			ConfigureFromAttributesAndElements(propertyValue, childElement);
		}
	}

	private bool TryGetPropertyValue<T>(T targetObject, PropertyInfo propInfo, out object? propertyValue) where T : class
	{
		try
		{
			propertyValue = propInfo.GetValue(targetObject, null);
			return true;
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException("Failed getting property " + propInfo.Name + " for type: " + typeof(T).Name, ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
			propertyValue = null;
			return false;
		}
	}

	private bool AddArrayItemFromElement(object o, PropertyInfo propInfo, ValidatedConfigurationElement element)
	{
		Type arrayItemType = PropertyHelper.GetArrayItemType(propInfo);
		if (arrayItemType != null && TryGetPropertyValue(o, propInfo, out object propertyValue) && propertyValue != null)
		{
			IList list = (IList)propertyValue;
			if (string.Equals(propInfo.Name, element.Name, StringComparison.OrdinalIgnoreCase))
			{
				bool flag = false;
				foreach (ValidatedConfigurationElement validChild in element.ValidChildren)
				{
					flag = true;
					list.Add(ParseArrayItemFromElement(arrayItemType, validChild));
				}
				if (flag)
				{
					return true;
				}
			}
			object value = ParseArrayItemFromElement(arrayItemType, element);
			list.Add(value);
			return true;
		}
		return false;
	}

	private object? ParseArrayItemFromElement(Type elementType, ValidatedConfigurationElement element)
	{
		object instance = TryCreateLayoutInstance(element, elementType);
		if (instance == null)
		{
			if (!ConfigurationItemFactory.Default.TryCreateInstance(elementType, out instance) || instance == null)
			{
				throw new NLogConfigurationException($"Factory returned null for {elementType}");
			}
			ConfigureFromAttributesAndElements(instance, element);
		}
		return instance;
	}

	private bool SetLayoutFromElement(object o, PropertyInfo propInfo, ValidatedConfigurationElement element)
	{
		Layout layout = TryCreateLayoutInstance(element, propInfo.PropertyType);
		if (layout != null)
		{
			PropertyHelper.SetPropertyValueForObject(o, layout, propInfo);
			return true;
		}
		return false;
	}

	private bool SetFilterFromElement(object o, PropertyInfo propInfo, ValidatedConfigurationElement element)
	{
		Filter filter = TryCreateFilterInstance(element, propInfo.PropertyType);
		if (filter != null)
		{
			PropertyHelper.SetPropertyValueForObject(o, filter, propInfo);
			return true;
		}
		return false;
	}

	private SimpleLayout CreateSimpleLayout(string layoutText)
	{
		return new SimpleLayout(layoutText, ConfigurationItemFactory.Default, base.LogFactory.ThrowConfigExceptions);
	}

	private Layout? TryCreateLayoutInstance(ValidatedConfigurationElement element, Type type)
	{
		if (!typeof(Layout).IsAssignableFrom(type))
		{
			return null;
		}
		string configItemTypeAttribute = element.GetConfigItemTypeAttribute();
		if (string.IsNullOrEmpty(configItemTypeAttribute))
		{
			return null;
		}
		string matchingVariableName;
		string text = ExpandSimpleVariables(configItemTypeAttribute, out matchingVariableName);
		if (matchingVariableName != null && TryLookupDynamicVariable(matchingVariableName, out Layout value) && type.IsAssignableFrom(value.GetType()))
		{
			return value;
		}
		if ("SimpleLayout".Equals(text, StringComparison.OrdinalIgnoreCase) && TryCreateSimpleLayoutInstance(element, out SimpleLayout simpleLayout))
		{
			return simpleLayout;
		}
		Layout layout = FactoryCreateInstance(text, ConfigurationItemFactory.Default.LayoutFactory);
		if (layout != null)
		{
			ConfigureFromAttributesAndElements(layout, element);
			return layout;
		}
		return null;
	}

	private bool TryCreateSimpleLayoutInstance(ValidatedConfigurationElement element, out SimpleLayout? simpleLayout)
	{
		if (!element.ValidChildren.Any())
		{
			ICollection<KeyValuePair<string, string>> values = element.Values;
			if (values.Count == 2)
			{
				string text = ("Text".Equals(values.First().Key, StringComparison.OrdinalIgnoreCase) ? (values.First().Value ?? string.Empty) : null) ?? ("Text".Equals(values.Last().Key, StringComparison.OrdinalIgnoreCase) ? (values.Last().Value ?? string.Empty) : null);
				if (text != null)
				{
					string txt = ExpandSimpleVariables(text);
					simpleLayout = (string.IsNullOrEmpty(text) ? SimpleLayout.Default : new SimpleLayout(txt, ConfigurationItemFactory.Default));
					return true;
				}
			}
		}
		simpleLayout = null;
		return false;
	}

	private Filter? TryCreateFilterInstance(ValidatedConfigurationElement element, Type type)
	{
		Filter filter = TryCreateInstance(element, type, ConfigurationItemFactory.Default.FilterFactory);
		if (filter != null)
		{
			ConfigureFromAttributesAndElements(filter, element);
			return filter;
		}
		return null;
	}

	private T? TryCreateInstance<T>(ValidatedConfigurationElement element, Type type, IFactory<T> factory) where T : class
	{
		if (!typeof(T).IsAssignableFrom(type))
		{
			return null;
		}
		string configItemTypeAttribute = element.GetConfigItemTypeAttribute();
		if (string.IsNullOrEmpty(configItemTypeAttribute))
		{
			return null;
		}
		return FactoryCreateInstance(configItemTypeAttribute, factory);
	}

	private T? FactoryCreateInstance<T>(string typeName, IFactory<T> factory) where T : class
	{
		T result = null;
		try
		{
			typeName = ExpandSimpleVariables(typeName).Trim();
			if (typeName.Contains(','))
			{
				string text = typeName.Substring(0, typeName.IndexOf(',')).Trim();
				if (factory.TryCreateInstance(text, out result) && result != null)
				{
					return result;
				}
				string text2 = typeName.Substring(typeName.IndexOf(',') + 1).Trim();
				if (!string.IsNullOrEmpty(text2) && RegisterExtensionFromAssemblyName(text2, typeName))
				{
					typeName = text;
				}
			}
			result = factory.CreateInstance(typeName);
			if (result == null)
			{
				throw new NLogConfigurationException("Failed to create " + typeof(T).Name + " of type: '" + typeName + "' - Factory returned null");
			}
		}
		catch (NLogConfigurationException configException)
		{
			if (MustThrowConfigException(configException))
			{
				throw;
			}
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException("Failed to create " + typeof(T).Name + " of type: " + typeName, ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
		}
		return result;
	}

	private void ConfigureFromAttributesAndElements<T>(T targetObject, ValidatedConfigurationElement element) where T : class
	{
		ConfigureObjectFromAttributes(targetObject, element);
		foreach (ValidatedConfigurationElement validChild in element.ValidChildren)
		{
			SetPropertyValuesFromElement(targetObject, validChild, element);
		}
	}

	private static Target WrapWithAsyncTargetWrapper(Target target)
	{
		if (target is AsyncTaskTarget)
		{
			InternalLogger.Debug("Skip wrapping target '{0}' with AsyncTargetWrapper", target.Name);
			return target;
		}
		if (target is AsyncTargetWrapper)
		{
			InternalLogger.Debug("Skip wrapping target '{0}' with AsyncTargetWrapper", target.Name);
			return target;
		}
		AsyncTargetWrapper asyncTargetWrapper = new AsyncTargetWrapper();
		asyncTargetWrapper.WrappedTarget = target;
		asyncTargetWrapper.Name = target.Name;
		target.Name += "_wrapped";
		InternalLogger.Debug("Wrapping target '{0}' with AsyncTargetWrapper and renaming to '{1}'", asyncTargetWrapper.Name, target.Name);
		target = asyncTargetWrapper;
		return target;
	}

	private Target WrapWithDefaultWrapper(Target target, ValidatedConfigurationElement defaultWrapperElement)
	{
		string configItemTypeAttribute = defaultWrapperElement.GetConfigItemTypeAttribute("targets");
		if (!(CreateTargetType(configItemTypeAttribute) is WrapperTargetBase wrapperTargetBase))
		{
			throw new NLogConfigurationException("Target type '" + configItemTypeAttribute + "' cannot be used as default target wrapper.");
		}
		WrapperTargetBase wrapperTargetBase2 = wrapperTargetBase;
		ParseTargetElement(wrapperTargetBase, defaultWrapperElement);
		while (wrapperTargetBase2.WrappedTarget != null)
		{
			if (wrapperTargetBase2.WrappedTarget is WrapperTargetBase wrapperTargetBase3)
			{
				wrapperTargetBase2 = wrapperTargetBase3;
				continue;
			}
			throw new NLogConfigurationException($"Target type '{configItemTypeAttribute}' with nested {wrapperTargetBase2.WrappedTarget.GetType()} cannot be used as default target wrapper.");
		}
		if (target is AsyncTaskTarget && wrapperTargetBase is AsyncTargetWrapper && wrapperTargetBase == wrapperTargetBase2)
		{
			InternalLogger.Debug("Skip wrapping target '{0}' with AsyncTargetWrapper", target.Name);
			return target;
		}
		wrapperTargetBase2.WrappedTarget = target;
		wrapperTargetBase.Name = target.Name;
		target.Name += "_wrapped";
		InternalLogger.Debug("Wrapping target '{0}' with '{1}' and renaming to '{2}'", wrapperTargetBase.Name, wrapperTargetBase.GetType(), target.Name);
		return wrapperTargetBase;
	}

	/// <summary>
	/// Parse boolean
	/// </summary>
	/// <param name="propertyName">Name of the property for logging.</param>
	/// <param name="value">value to parse</param>
	/// <param name="defaultValue">Default value to return if the parse failed</param>
	/// <returns>Boolean attribute value or default.</returns>
	private bool ParseBooleanValue(string propertyName, string value, bool defaultValue)
	{
		try
		{
			return Convert.ToBoolean(value?.Trim(), CultureInfo.InvariantCulture);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException($"'{propertyName}' hasn't a valid boolean value '{value}'. {defaultValue} will be used", ex);
			if (MustThrowConfigException(ex2))
			{
				throw ex2;
			}
			return defaultValue;
		}
	}

	private bool? ParseNullableBooleanValue(string propertyName, string value, bool defaultValue)
	{
		if (!StringHelpers.IsNullOrWhiteSpace(value))
		{
			return ParseBooleanValue(propertyName, value, defaultValue);
		}
		return null;
	}

	private bool MustThrowConfigException(NLogConfigurationException configException)
	{
		if (configException.MustBeRethrown())
		{
			return true;
		}
		if (base.LogFactory.ThrowConfigExceptions ?? base.LogFactory.ThrowExceptions)
		{
			return true;
		}
		return false;
	}

	private static bool MatchesName(string key, string expectedKey)
	{
		return string.Equals(key?.Trim(), expectedKey, StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsTargetElement(string name)
	{
		if (!name.Equals("target", StringComparison.OrdinalIgnoreCase) && !name.Equals("wrapper", StringComparison.OrdinalIgnoreCase) && !name.Equals("wrapper-target", StringComparison.OrdinalIgnoreCase))
		{
			return name.Equals("compound-target", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static bool IsTargetRefElement(string name)
	{
		if (!name.Equals("target-ref", StringComparison.OrdinalIgnoreCase) && !name.Equals("wrapper-target-ref", StringComparison.OrdinalIgnoreCase))
		{
			return name.Equals("compound-target-ref", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static string GetName(Target target)
	{
		if (!string.IsNullOrEmpty(target.Name))
		{
			return target.Name;
		}
		return target.GetType().Name;
	}
}

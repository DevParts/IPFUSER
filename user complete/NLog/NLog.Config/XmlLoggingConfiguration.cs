using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using NLog.Common;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Config;

/// <summary>
/// Loads NLog LoggingConfiguration from xml-file
/// </summary>
/// <remarks>
/// Make sure to update official NLog.xsd schema, when adding new config-options outside targets/layouts
/// </remarks>
public class XmlLoggingConfiguration : LoggingConfigurationParser
{
	private sealed class AutoReloadConfigFileWatcher : IDisposable
	{
		private readonly LogFactory _logFactory;

		private readonly MultiFileWatcher _fileWatcher = new MultiFileWatcher();

		private readonly object _lockObject = new object();

		private Timer? _reloadTimer;

		private bool _isDisposing;

		internal bool IsDisposed => _isDisposing;

		public AutoReloadConfigFileWatcher(LogFactory logFactory)
		{
			_logFactory = logFactory;
			_fileWatcher.FileChanged += FileWatcher_FileChanged;
		}

		private void FileWatcher_FileChanged(object sender, FileSystemEventArgs e)
		{
			lock (_lockObject)
			{
				if (_isDisposing)
				{
					return;
				}
				Timer reloadTimer = _reloadTimer;
				if (reloadTimer == null)
				{
					LoggingConfiguration configuration = _logFactory.Configuration;
					if (configuration != null)
					{
						_reloadTimer = new Timer(delegate(object s)
						{
							ReloadTimer(s);
						}, configuration, 1000, -1);
					}
				}
				else
				{
					reloadTimer.Change(1000, -1);
				}
			}
		}

		private void ReloadTimer(object state)
		{
			if (_isDisposing)
			{
				return;
			}
			LoggingConfiguration loggingConfiguration = (LoggingConfiguration)state;
			InternalLogger.Info("AutoReload Config File Monitor reloading configuration...");
			lock (_lockObject)
			{
				if (_isDisposing)
				{
					return;
				}
				Timer? reloadTimer = _reloadTimer;
				_reloadTimer = null;
				reloadTimer?.Dispose();
			}
			LoggingConfiguration loggingConfiguration2 = null;
			try
			{
				if (_logFactory.Configuration != loggingConfiguration)
				{
					InternalLogger.Debug("AutoReload Config File Monitor skipping reload, since existing NLog config has changed.");
					return;
				}
				loggingConfiguration2 = loggingConfiguration.Reload();
				if (loggingConfiguration2 == null || loggingConfiguration2 == loggingConfiguration)
				{
					InternalLogger.Debug("AutoReload Config File Monitor skipping reload, since new configuration has not changed.");
					return;
				}
				if (_logFactory.Configuration != loggingConfiguration)
				{
					InternalLogger.Debug("AutoReload Config File Monitor skipping reload, since existing NLog config has changed.");
					return;
				}
			}
			catch (Exception ex)
			{
				InternalLogger.Warn(ex, "AutoReload Config File Monitor failed to reload NLog LoggingConfiguration.");
				return;
			}
			try
			{
				TryUnwatchConfigFile();
				_logFactory.Configuration = loggingConfiguration2;
			}
			catch (Exception ex2)
			{
				InternalLogger.Warn(ex2, "AutoReload Config File Monitor failed to activate new NLog LoggingConfiguration.");
				_fileWatcher.Watch((loggingConfiguration as XmlLoggingConfiguration)?.AutoReloadFileNames ?? ArrayHelper.Empty<string>());
			}
		}

		public void RefreshFileWatcher(IEnumerable<string> fileNamesToWatch)
		{
			_fileWatcher.Watch(fileNamesToWatch);
		}

		public void Dispose()
		{
			Timer reloadTimer = _reloadTimer;
			lock (_lockObject)
			{
				if (_isDisposing)
				{
					return;
				}
				reloadTimer = _reloadTimer;
				_isDisposing = true;
				_reloadTimer = null;
			}
			_fileWatcher.FileChanged -= FileWatcher_FileChanged;
			reloadTimer?.Dispose();
			_fileWatcher.Dispose();
		}

		private void TryUnwatchConfigFile()
		{
			try
			{
				_fileWatcher?.StopWatching();
			}
			catch (Exception ex)
			{
				InternalLogger.Warn(ex, "AutoReload Config File Monitor failed to stop file watcher.");
				if (LogManager.ThrowExceptions || _logFactory.ThrowExceptions)
				{
					throw;
				}
			}
		}
	}

	private static readonly Dictionary<LogFactory, AutoReloadConfigFileWatcher> _watchers = new Dictionary<LogFactory, AutoReloadConfigFileWatcher>();

	private readonly Dictionary<string, bool> _fileMustAutoReloadLookup = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

	private string? _originalFileName;

	private readonly Stack<string> _currentFilePath = new Stack<string>();

	/// <summary>
	/// Gets or sets a value indicating whether any of the configuration files
	/// should be watched for changes and reloaded automatically when changed.
	/// </summary>
	public bool AutoReload
	{
		get
		{
			return AutoReloadFileNames.Any();
		}
		set
		{
			foreach (string item in _fileMustAutoReloadLookup.Keys.ToList())
			{
				_fileMustAutoReloadLookup[item] = value;
			}
		}
	}

	/// <summary>
	/// Gets the collection of file names which should be watched for changes by NLog.
	/// This is the list of configuration files processed.
	/// If the <c>autoReload</c> attribute is not set it returns empty collection.
	/// </summary>
	public IEnumerable<string> AutoReloadFileNames
	{
		get
		{
			if (_fileMustAutoReloadLookup.Count == 0)
			{
				return ArrayHelper.Empty<string>();
			}
			return from entry in _fileMustAutoReloadLookup
				where entry.Value
				select entry.Key;
		}
	}

	/// <inheritdoc />
	[Obsolete("Replaced by AutoReloadFileNames. Marked obsolete with NLog v6")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override IEnumerable<string> FileNamesToWatch => AutoReloadFileNames;

	internal XmlLoggingConfiguration(LogFactory logFactory)
		: base(logFactory)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="fileName">Path to the config-file to read.</param>
	public XmlLoggingConfiguration(string fileName)
		: this(fileName, LogManager.LogFactory)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="fileName">Path to the config-file to read.</param>
	/// <param name="logFactory">The <see cref="T:NLog.LogFactory" /> to which to apply any applicable configuration values.</param>
	public XmlLoggingConfiguration(string fileName, LogFactory logFactory)
		: base(logFactory)
	{
		Guard.ThrowIfNullOrEmpty(fileName, "fileName");
		LoadFromXmlFile(fileName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="xmlSource">Configuration file to be read.</param>
	public XmlLoggingConfiguration(TextReader xmlSource)
		: this(xmlSource, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="xmlSource">Configuration file to be read.</param>
	/// <param name="filePath">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	public XmlLoggingConfiguration(TextReader xmlSource, string? filePath)
		: this(xmlSource, filePath, LogManager.LogFactory)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="xmlSource">Configuration file to be read.</param>
	/// <param name="filePath">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	/// <param name="logFactory">The <see cref="T:NLog.LogFactory" /> to which to apply any applicable configuration values.</param>
	public XmlLoggingConfiguration(TextReader xmlSource, string? filePath, LogFactory logFactory)
		: base(logFactory)
	{
		Guard.ThrowIfNull(xmlSource, "xmlSource");
		ParseFromTextReader(xmlSource, filePath);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="reader">XML reader to read from.</param>
	[Obsolete("Instead use TextReader as input. Marked obsolete with NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public XmlLoggingConfiguration(XmlReader reader)
		: this(reader, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="reader">XmlReader containing the configuration section.</param>
	/// <param name="fileName">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	[Obsolete("Instead use TextReader as input. Marked obsolete with NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public XmlLoggingConfiguration(XmlReader reader, string? fileName)
		: this(reader, fileName, LogManager.LogFactory)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="reader">XmlReader containing the configuration section.</param>
	/// <param name="fileName">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	/// <param name="logFactory">The <see cref="T:NLog.LogFactory" /> to which to apply any applicable configuration values.</param>
	[Obsolete("Instead use TextReader as input. Marked obsolete with NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public XmlLoggingConfiguration(XmlReader reader, string? fileName, LogFactory logFactory)
		: base(logFactory)
	{
		Guard.ThrowIfNull(reader, "reader");
		ParseFromXmlReader(reader, fileName);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Config.XmlLoggingConfiguration" /> class.
	/// </summary>
	/// <param name="xmlContents">NLog configuration as XML string.</param>
	/// <param name="filePath">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	/// <param name="logFactory">The <see cref="T:NLog.LogFactory" /> to which to apply any applicable configuration values.</param>
	internal XmlLoggingConfiguration(string xmlContents, string filePath, LogFactory logFactory)
		: base(logFactory)
	{
		Guard.ThrowIfNullOrEmpty(xmlContents, "xmlContents");
		LoadFromXmlContent(xmlContents, filePath);
	}

	/// <summary>
	/// Parse XML string as NLog configuration
	/// </summary>
	/// <param name="xml">NLog configuration in XML to be parsed</param>
	public static XmlLoggingConfiguration CreateFromXmlString(string xml)
	{
		return CreateFromXmlString(xml, LogManager.LogFactory);
	}

	/// <summary>
	/// Parse XML string as NLog configuration
	/// </summary>
	/// <param name="xml">NLog configuration in XML to be parsed</param>
	/// <param name="logFactory">NLog LogFactory</param>
	public static XmlLoggingConfiguration CreateFromXmlString(string xml, LogFactory logFactory)
	{
		return new XmlLoggingConfiguration(xml, string.Empty, logFactory);
	}

	/// <summary>
	/// Loads the NLog LoggingConfiguration from its original configuration file and returns the new <see cref="T:NLog.Config.LoggingConfiguration" /> object.
	/// </summary>
	/// <returns>The newly loaded <see cref="T:NLog.Config.XmlLoggingConfiguration" /> instance.</returns>
	/// <remarks>Must assign the returned object to LogManager.Configuration to activate it</remarks>
	public override LoggingConfiguration Reload()
	{
		string text = _originalFileName ?? string.Empty;
		if (!string.IsNullOrEmpty(text))
		{
			XmlLoggingConfiguration xmlLoggingConfiguration = new XmlLoggingConfiguration(base.LogFactory);
			xmlLoggingConfiguration.PrepareForReload(this);
			xmlLoggingConfiguration.LoadFromXmlFile(text);
			return xmlLoggingConfiguration;
		}
		return base.Reload();
	}

	/// <inheritdoc />
	protected internal override void OnConfigurationAssigned(LogFactory? logFactory)
	{
		base.OnConfigurationAssigned(logFactory);
		try
		{
			LogFactory logFactory2 = logFactory ?? base.LogFactory ?? LogManager.LogFactory;
			AutoReloadConfigFileWatcher value = null;
			lock (_watchers)
			{
				_watchers.TryGetValue(logFactory2, out value);
			}
			if (logFactory == null || !AutoReload)
			{
				if (value != null && !value.IsDisposed)
				{
					InternalLogger.Debug("AutoReload Config File Monitor stopping, since no active configuration");
					value.Dispose();
				}
				return;
			}
			InternalLogger.Debug("AutoReload Config File Monitor refreshing after configuration changed");
			if (value == null || value.IsDisposed)
			{
				InternalLogger.Debug("AutoReload Config File Monitor starting");
				value = new AutoReloadConfigFileWatcher(logFactory2);
				lock (_watchers)
				{
					_watchers[logFactory2] = value;
				}
			}
			value.RefreshFileWatcher(AutoReloadFileNames);
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "AutoReload Config File Monitor failed to refresh after configuration changed.");
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Get file paths (including filename) for the possible NLog config files.
	/// </summary>
	/// <returns>The file paths to the possible config file</returns>
	[Obsolete("Replaced by chaining LogManager.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static IEnumerable<string> GetCandidateConfigFilePaths()
	{
		return LogManager.LogFactory.GetCandidateConfigFilePaths();
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Overwrite the paths (including filename) for the possible NLog config files.
	/// </summary>
	/// <param name="filePaths">The file paths to the possible config file</param>
	[Obsolete("Replaced by chaining LogManager.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void SetCandidateConfigFilePaths(IEnumerable<string> filePaths)
	{
		LogManager.LogFactory.SetCandidateConfigFilePaths(filePaths);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Clear the candidate file paths and return to the defaults.
	/// </summary>
	[Obsolete("Replaced by chaining LogManager.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void ResetCandidateConfigFilePath()
	{
		LogManager.LogFactory.ResetCandidateConfigFilePath();
	}

	private void LoadFromXmlFile(string filePath)
	{
		using TextReader textReader = base.LogFactory.CurrentAppEnvironment.LoadTextFile(filePath);
		ParseFromTextReader(textReader, filePath);
	}

	internal void LoadFromXmlContent(string xmlContents, string filePath)
	{
		using StringReader textReader = new StringReader(xmlContents);
		ParseFromTextReader(textReader, filePath);
	}

	[Obsolete("Instead use TextReader as input. Marked obsolete with NLog 6.0")]
	private void ParseFromXmlReader(XmlReader reader, string? filePath)
	{
		try
		{
			_originalFileName = ((filePath == null || StringHelpers.IsNullOrWhiteSpace(filePath)) ? null : GetFileLookupKey(filePath));
			reader.MoveToContent();
			XmlLoggingConfigurationElement content = new XmlLoggingConfigurationElement(reader);
			if (!string.IsNullOrEmpty(_originalFileName))
			{
				InternalLogger.Info("Loading NLog config from XML file: {0}", _originalFileName);
				ParseTopLevel(content, filePath, autoReloadDefault: false);
			}
			else
			{
				ParseTopLevel(content, null, autoReloadDefault: false);
			}
		}
		catch (Exception ex)
		{
			NLogConfigurationException ex2 = new NLogConfigurationException("Exception when loading configuration " + filePath, ex);
			InternalLogger.Error(ex, ex2.Message);
			throw ex2;
		}
	}

	private void ParseFromTextReader(TextReader textReader, string? filePath)
	{
		try
		{
			_originalFileName = ((filePath == null || StringHelpers.IsNullOrWhiteSpace(filePath)) ? null : GetFileLookupKey(filePath));
			IList<XmlParser.XmlParserElement> processingInstructions;
			XmlParserConfigurationElement content = new XmlParserConfigurationElement(new XmlParser(textReader).LoadDocument(out processingInstructions));
			if (!string.IsNullOrEmpty(_originalFileName))
			{
				InternalLogger.Info("Loading NLog config from XML file: {0}", _originalFileName);
				ParseTopLevel(content, filePath, autoReloadDefault: false);
			}
			else
			{
				ParseTopLevel(content, null, autoReloadDefault: false);
			}
		}
		catch (Exception ex)
		{
			string text = (string.IsNullOrEmpty(filePath) ? "" : (" FilePath: " + filePath));
			NLogConfigurationException ex2 = new NLogConfigurationException("Failed loading NLog configuration. " + ex.Message + " " + text, ex);
			InternalLogger.Error(ex2, ex2.Message);
			throw ex2;
		}
	}

	/// <summary>
	/// Include new file into the configuration. Check if not already included.
	/// </summary>
	private void IncludeNewConfigFile(string filePath, bool autoReloadDefault)
	{
		if (!_fileMustAutoReloadLookup.ContainsKey(GetFileLookupKey(filePath)))
		{
			using (TextReader xmlSource = base.LogFactory.CurrentAppEnvironment.LoadTextFile(filePath))
			{
				IList<XmlParser.XmlParserElement> processingInstructions;
				XmlParserConfigurationElement content = new XmlParserConfigurationElement(new XmlParser(xmlSource).LoadDocument(out processingInstructions), nestedElement: false);
				ParseTopLevel(content, filePath, autoReloadDefault);
			}
		}
	}

	/// <summary>
	/// Parse the root
	/// </summary>
	/// <param name="content"></param>
	/// <param name="filePath">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	/// <param name="autoReloadDefault">The default value for the autoReload option.</param>
	private void ParseTopLevel(ILoggingConfigurationElement content, string? filePath, bool autoReloadDefault)
	{
		content.AssertName("nlog", "configuration");
		string text = content.Name.ToUpperInvariant();
		if (!(text == "CONFIGURATION"))
		{
			if (text == "NLOG")
			{
				ParseNLogElement(content, filePath, autoReloadDefault);
			}
		}
		else
		{
			ParseConfigurationElement(content, filePath, autoReloadDefault);
		}
	}

	/// <summary>
	/// Parse {configuration} xml element.
	/// </summary>
	/// <param name="configurationElement"></param>
	/// <param name="filePath">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	/// <param name="autoReloadDefault">The default value for the autoReload option.</param>
	private void ParseConfigurationElement(ILoggingConfigurationElement configurationElement, string? filePath, bool autoReloadDefault)
	{
		InternalLogger.Trace("ParseConfigurationElement");
		configurationElement.AssertName("configuration");
		foreach (ILoggingConfigurationElement item in configurationElement.FilterChildren("nlog"))
		{
			ParseNLogElement(item, filePath, autoReloadDefault);
		}
	}

	/// <summary>
	/// Parse {NLog} xml element.
	/// </summary>
	/// <param name="nlogElement"></param>
	/// <param name="filePath">Path to the config-file that contains the element (to be used as a base for including other files). <c>null</c> is allowed.</param>
	/// <param name="autoReloadDefault">The default value for the autoReload option.</param>
	private void ParseNLogElement(ILoggingConfigurationElement nlogElement, string? filePath, bool autoReloadDefault)
	{
		InternalLogger.Trace("ParseNLogElement");
		nlogElement.AssertName("nlog");
		bool optionalBooleanValue = nlogElement.GetOptionalBooleanValue("autoReload", autoReloadDefault);
		try
		{
			string basePath = null;
			if (filePath != null && !StringHelpers.IsNullOrWhiteSpace(filePath))
			{
				_fileMustAutoReloadLookup[GetFileLookupKey(filePath)] = optionalBooleanValue;
				_currentFilePath.Push(filePath);
				basePath = Path.GetDirectoryName(filePath);
			}
			LoadConfig(nlogElement, basePath);
		}
		finally
		{
			if (!string.IsNullOrEmpty(filePath))
			{
				_currentFilePath.Pop();
			}
		}
	}

	/// <summary>
	/// Parses a single config section within the NLog-config
	/// </summary>
	/// <param name="configSection"></param>
	/// <returns>Section was recognized</returns>
	protected override bool ParseNLogSection(ILoggingConfigurationElement configSection)
	{
		if (configSection.MatchesName("include"))
		{
			string text = _currentFilePath.Peek();
			bool autoReloadDefault = !string.IsNullOrEmpty(text) && _fileMustAutoReloadLookup[GetFileLookupKey(text)];
			ParseIncludeElement(configSection, (!string.IsNullOrEmpty(text)) ? Path.GetDirectoryName(text) : null, autoReloadDefault);
			return true;
		}
		return base.ParseNLogSection(configSection);
	}

	private void ParseIncludeElement(ILoggingConfigurationElement includeElement, string? baseDirectory, bool autoReloadDefault)
	{
		includeElement.AssertName("include");
		string text = includeElement.GetRequiredValue("file", "nlog");
		bool optionalBooleanValue = includeElement.GetOptionalBooleanValue("ignoreErrors", defaultValue: false);
		try
		{
			text = ExpandSimpleVariables(text);
			text = SimpleLayout.Evaluate(text, this);
			string text2 = text;
			if (baseDirectory != null)
			{
				text2 = Path.Combine(baseDirectory, text);
			}
			if (File.Exists(text2))
			{
				InternalLogger.Debug("Including file '{0}'", text2);
				IncludeNewConfigFile(text2, autoReloadDefault);
				return;
			}
			if (text.IndexOf('*') >= 0)
			{
				IncludeConfigFilesByMask(baseDirectory ?? ".", text, autoReloadDefault);
				return;
			}
			if (optionalBooleanValue)
			{
				InternalLogger.Debug("Skipping included file '{0}' as it can't be found", text2);
				return;
			}
			throw new FileNotFoundException("Included file not found: " + text2);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			NLogConfigurationException ex2 = new NLogConfigurationException("Error when including '" + text + "'.", ex);
			InternalLogger.Error(ex, ex2.Message);
			if (!optionalBooleanValue)
			{
				throw ex2;
			}
		}
	}

	/// <summary>
	/// Include (multiple) files by filemask, e.g. *.nlog
	/// </summary>
	/// <param name="baseDirectory">base directory in case if <paramref name="fileMask" /> is relative</param>
	/// <param name="fileMask">relative or absolute fileMask</param>
	/// <param name="autoReloadDefault"></param>
	private void IncludeConfigFilesByMask(string baseDirectory, string fileMask, bool autoReloadDefault)
	{
		string text = baseDirectory;
		if (Path.IsPathRooted(fileMask))
		{
			text = Path.GetDirectoryName(fileMask);
			if (text == null)
			{
				InternalLogger.Warn("directory is empty for include of '{0}'", fileMask);
				return;
			}
			string fileName = Path.GetFileName(fileMask);
			if (fileName == null)
			{
				InternalLogger.Warn("filename is empty for include of '{0}'", fileMask);
				return;
			}
			fileMask = fileName;
		}
		string[] files = Directory.GetFiles(text, fileMask);
		foreach (string filePath in files)
		{
			IncludeNewConfigFile(filePath, autoReloadDefault);
		}
	}

	private static string GetFileLookupKey(string fileName)
	{
		return Path.GetFullPath(fileName);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (AutoReload)
		{
			return base.ToString() + ", AutoReload=true, FilePath=" + _originalFileName;
		}
		return base.ToString() + ", FilePath=" + _originalFileName;
	}
}

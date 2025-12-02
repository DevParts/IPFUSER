using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Internal.Fakeables;

namespace NLog;

/// <summary>
/// Specialized LogFactory that can return instances of custom logger types.
/// </summary>
/// <remarks>Use this only when a custom Logger type is defined.</remarks>
/// <typeparam name="T">The type of the logger to be returned. Must inherit from <see cref="T:NLog.Logger" />.</typeparam>
public class LogFactory<T> : LogFactory where T : Logger, new()
{
	/// <summary>
	/// Gets the logger with type <typeparamref name="T" />.
	/// </summary>
	/// <param name="name">The logger name.</param>
	/// <returns>An instance of <typeparamref name="T" />.</returns>
	public new T GetLogger(string name)
	{
		return GetLogger<T>(name);
	}

	/// <summary>
	/// Gets a custom logger with the full name of the current class (so namespace and class name) and type <typeparamref name="T" />.
	/// </summary>
	/// <returns>An instance of <typeparamref name="T" />.</returns>
	/// <remarks>This is a slow-running method.
	/// Make sure you're not doing this in a loop.</remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public new T GetCurrentClassLogger()
	{
		string classFullName = StackTraceUsageUtils.GetClassFullName(new StackFrame(1, fNeedFileInfo: false));
		return GetLogger(classFullName);
	}
}
/// <summary>
/// Creates and manages instances of <see cref="T:NLog.Logger" /> objects.
/// </summary>
public class LogFactory : IDisposable
{
	/// <summary>
	/// Logger cache key.
	/// </summary>
	private struct LoggerCacheKey : IEquatable<LoggerCacheKey>
	{
		public readonly string Name;

		public readonly Type ConcreteType;

		public LoggerCacheKey(string name, Type concreteType)
		{
			Name = name;
			ConcreteType = concreteType;
		}

		public override int GetHashCode()
		{
			return ConcreteType.GetHashCode() ^ Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is LoggerCacheKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public bool Equals(LoggerCacheKey other)
		{
			if (ConcreteType == other.ConcreteType)
			{
				return string.Equals(other.Name, Name, StringComparison.Ordinal);
			}
			return false;
		}
	}

	/// <summary>
	/// Logger cache.
	/// </summary>
	private sealed class LoggerCache
	{
		private readonly Dictionary<LoggerCacheKey, WeakReference> _loggerCache = new Dictionary<LoggerCacheKey, WeakReference>();

		public int Count => _loggerCache.Count;

		/// <summary>
		/// Inserts or updates.
		/// </summary>
		/// <param name="cacheKey"></param>
		/// <param name="logger"></param>
		public void InsertOrUpdate(LoggerCacheKey cacheKey, Logger logger)
		{
			_loggerCache[cacheKey] = new WeakReference(logger);
		}

		public Logger? Retrieve(LoggerCacheKey cacheKey)
		{
			if (_loggerCache.TryGetValue(cacheKey, out WeakReference value))
			{
				return value.Target as Logger;
			}
			return null;
		}

		public List<Logger> GetLoggers()
		{
			List<Logger> list = new List<Logger>(_loggerCache.Count);
			foreach (KeyValuePair<LoggerCacheKey, WeakReference> item2 in _loggerCache)
			{
				if (item2.Value.Target is Logger item)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public void Reset()
		{
			_loggerCache.Clear();
		}

		/// <summary>
		/// Loops through all cached loggers and removes dangling loggers that have been garbage collected.
		/// </summary>
		public void PurgeObsoleteLoggers()
		{
			foreach (LoggerCacheKey item in _loggerCache.Keys.ToList())
			{
				if (Retrieve(item) == null)
				{
					_loggerCache.Remove(item);
				}
			}
		}
	}

	/// <summary>
	/// Enables logging in <see cref="M:System.IDisposable.Dispose" /> implementation.
	/// </summary>
	private sealed class LogEnabler : IDisposable
	{
		private readonly LogFactory _factory;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NLog.LogFactory.LogEnabler" /> class.
		/// </summary>
		/// <param name="factory">The factory.</param>
		public LogEnabler(LogFactory factory)
		{
			_factory = factory;
		}

		/// <summary>
		/// Enables logging.
		/// </summary>
		void IDisposable.Dispose()
		{
			_factory.ResumeLogging();
		}
	}

	private static readonly TimeSpan DefaultFlushTimeout = TimeSpan.FromSeconds(15.0);

	private static AppEnvironmentWrapper? defaultAppEnvironment;

	/// <remarks>
	/// Internal for unit tests
	/// </remarks>
	internal readonly object _syncRoot = new object();

	private readonly LoggerCache _loggerCache = new LoggerCache();

	private readonly ServiceRepositoryInternal _serviceRepository;

	private readonly IAppEnvironment _currentAppEnvironment;

	internal LoggingConfiguration? _config;

	internal LogMessageFormatter ActiveMessageFormatter;

	internal LogMessageFormatter? SingleTargetMessageFormatter;

	internal LogMessageTemplateFormatter? AutoMessageTemplateFormatter;

	private LogLevel _globalThreshold = LogLevel.MinLevel;

	private bool _configLoaded;

	private int _supendLoggingCounter;

	/// <summary>
	/// Overwrite possible file paths (including filename) for possible NLog config files.
	/// When this property is <c>null</c>, the default file paths (<see cref="M:NLog.LogFactory.GetCandidateConfigFilePaths" /> are used.
	/// </summary>
	[Obsolete("Replaced by LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	private List<string>? _candidateConfigFilePaths;

	private readonly ILoggingConfigurationLoader _configLoader;

	private bool _autoShutdown = true;

	internal CultureInfo? _defaultCultureInfo;

	/// <summary>
	/// Currently this <see cref="T:NLog.LogFactory" /> is disposing?
	/// </summary>
	private bool _isDisposing;

	internal static IAppEnvironment DefaultAppEnvironment => defaultAppEnvironment ?? (defaultAppEnvironment = new AppEnvironmentWrapper());

	internal IAppEnvironment CurrentAppEnvironment => _currentAppEnvironment;

	/// <summary>
	/// Repository of interfaces used by NLog to allow override for dependency injection
	/// </summary>
	public ServiceRepository ServiceRepository => _serviceRepository;

	/// <summary>
	/// Gets or sets a value indicating whether exceptions should be thrown. See also <see cref="P:NLog.LogFactory.ThrowConfigExceptions" />.
	/// </summary>
	/// <value>A value of <see langword="true" /> if exception should be thrown; otherwise, <see langword="false" />.</value>
	/// <remarks>By default exceptions are not thrown under any circumstances.</remarks>
	public bool ThrowExceptions { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether <see cref="T:NLog.NLogConfigurationException" /> should be thrown.
	///
	/// If <c>null</c> then <see cref="P:NLog.LogFactory.ThrowExceptions" /> is used.
	/// </summary>
	/// <value>A value of <see langword="true" /> if exception should be thrown; otherwise, <see langword="false" />.</value>
	/// <remarks>
	/// This option is for backwards-compatibility.
	/// By default exceptions are not thrown under any circumstances.
	/// </remarks>
	public bool? ThrowConfigExceptions { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether Variables should be kept on configuration reload.
	/// </summary>
	public bool KeepVariablesOnReload { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to automatically call <see cref="M:NLog.LogFactory.Shutdown" />
	/// on AppDomain.Unload or AppDomain.ProcessExit
	/// </summary>
	public bool AutoShutdown
	{
		get
		{
			return _autoShutdown;
		}
		set
		{
			if (value != _autoShutdown)
			{
				_autoShutdown = value;
				LoggerShutdown -= OnStopLogging;
				if (value)
				{
					LoggerShutdown += OnStopLogging;
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets the current logging configuration.
	/// </summary>
	/// <remarks>
	/// Setter will re-configure all <see cref="T:NLog.Logger" />-objects, so no need to also call <see cref="M:NLog.LogFactory.ReconfigExistingLoggers" />
	/// </remarks>
	[CanBeNull]
	public LoggingConfiguration? Configuration
	{
		get
		{
			if (_configLoaded)
			{
				return _config;
			}
			lock (_syncRoot)
			{
				if (_configLoaded || _isDisposing)
				{
					return _config;
				}
				LoggingConfiguration loggingConfiguration = _configLoader.Load(this);
				if (loggingConfiguration != null)
				{
					ActivateLoggingConfiguration(loggingConfiguration);
				}
				return _config;
			}
		}
		set
		{
			lock (_syncRoot)
			{
				LoggingConfiguration config = _config;
				if (config != null)
				{
					InternalLogger.Info("Closing old configuration.");
					config.OnConfigurationAssigned(null);
					Flush();
					config.Close();
				}
				if (value == null)
				{
					_config = value;
					_configLoaded = false;
				}
				else
				{
					ActivateLoggingConfiguration(value);
				}
				OnConfigurationChanged(new LoggingConfigurationChangedEventArgs(value, config));
			}
		}
	}

	/// <summary>
	/// Gets or sets the global log level threshold. Log events below this threshold are not logged.
	/// </summary>
	public LogLevel GlobalThreshold
	{
		get
		{
			return _globalThreshold;
		}
		set
		{
			lock (_syncRoot)
			{
				if (_globalThreshold != value)
				{
					InternalLogger.Info("LogFactory GlobalThreshold changing to LogLevel: {0}", value);
				}
				_globalThreshold = value ?? LogLevel.MinLevel;
				ReconfigExistingLoggers();
			}
		}
	}

	/// <summary>
	/// Gets or sets the default culture info to use as <see cref="P:NLog.LogEventInfo.FormatProvider" />.
	/// </summary>
	/// <value>
	/// Specific culture info or null to use <see cref="P:System.Globalization.CultureInfo.CurrentCulture" />
	/// </value>
	public CultureInfo? DefaultCultureInfo
	{
		get
		{
			if (_config != null)
			{
				return _config.DefaultCultureInfo;
			}
			return _defaultCultureInfo;
		}
		set
		{
			if (_config != null && (_config.DefaultCultureInfo == _defaultCultureInfo || _config.DefaultCultureInfo == null))
			{
				_config.DefaultCultureInfo = value;
			}
			_defaultCultureInfo = value;
		}
	}

	/// <summary>
	/// Occurs when logging <see cref="P:NLog.LogFactory.Configuration" /> changes. Both when assigned to new config or config unloaded.
	/// </summary>
	/// <remarks>
	/// Note <see cref="P:NLog.Config.LoggingConfigurationChangedEventArgs.ActivatedConfiguration" /> can be <c>null</c> when unloading configuration at shutdown.
	/// </remarks>
	public event EventHandler<LoggingConfigurationChangedEventArgs>? ConfigurationChanged;

	/// <summary>
	/// Event that is raised when the current Process / AppDomain terminates.
	/// </summary>
	private static event EventHandler<EventArgs> LoggerShutdown
	{
		add
		{
			if (LogFactory._loggerShutdown == null)
			{
				InternalLogger.Debug("Registered shutdown event handler for ProcessExit.");
				DefaultAppEnvironment.ProcessExit += OnLoggerShutdown;
			}
			_loggerShutdown += value;
		}
		remove
		{
			_loggerShutdown -= value;
			if (LogFactory._loggerShutdown == null && defaultAppEnvironment != null)
			{
				InternalLogger.Debug("Unregistered shutdown event handler for ProcessExit.");
				defaultAppEnvironment.ProcessExit -= OnLoggerShutdown;
			}
		}
	}

	private static event EventHandler<EventArgs>? _loggerShutdown;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogFactory" /> class.
	/// </summary>
	public LogFactory()
		: this(new LoggingConfigurationFileLoader(DefaultAppEnvironment))
	{
	}

	/// <summary>
	/// Obsolete instead use <see cref="T:NLog.LogFactory" /> default-constructor, and assign <see cref="P:NLog.LogFactory.Configuration" /> with NLog 5.0.
	///
	/// Initializes a new instance of the <see cref="T:NLog.LogFactory" /> class.
	/// </summary>
	/// <param name="config">The config.</param>
	[Obsolete("Constructor with LoggingConfiguration as parameter should not be used. Instead provide LogFactory as parameter when constructing LoggingConfiguration. Marked obsolete in NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public LogFactory(LoggingConfiguration config)
		: this()
	{
		Configuration = config;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogFactory" /> class.
	/// </summary>
	/// <param name="configLoader">The config loader</param>
	/// <param name="appEnvironment">The custom AppEnvironmnet override</param>
	internal LogFactory(ILoggingConfigurationLoader configLoader, IAppEnvironment? appEnvironment = null)
	{
		_configLoader = configLoader;
		_currentAppEnvironment = appEnvironment ?? DefaultAppEnvironment;
		LoggerShutdown += OnStopLogging;
		_serviceRepository = new ServiceRepositoryInternal(this);
		_serviceRepository.TypeRegistered += ServiceRepository_TypeRegistered;
		ActiveMessageFormatter = RefreshMessageFormatter();
	}

	private void ActivateLoggingConfiguration(LoggingConfiguration config)
	{
		if (_config == null)
		{
			LogNLogAssemblyVersion();
		}
		_config = config;
		_configLoaded = true;
		_config.OnConfigurationAssigned(this);
		_config.Dump();
		ReconfigExistingLoggers();
		InternalLogger.Info("Configuration initialized: {0}", config);
	}

	private void ServiceRepository_TypeRegistered(object sender, ServiceRepositoryUpdateEventArgs e)
	{
		_config?.CheckForMissingServiceTypes(e.ServiceType);
		if (e.ServiceType == typeof(ILogMessageFormatter))
		{
			RefreshMessageFormatter();
		}
	}

	private LogMessageFormatter RefreshMessageFormatter()
	{
		ILogMessageFormatter service = _serviceRepository.GetService<ILogMessageFormatter>();
		ActiveMessageFormatter = service.FormatMessage;
		if (service is LogMessageTemplateFormatter logMessageTemplateFormatter)
		{
			LogMessageTemplateFormatter logMessageTemplateFormatter2 = new LogMessageTemplateFormatter(this, logMessageTemplateFormatter.EnableMessageTemplateParser == true, singleTargetOnly: true);
			SingleTargetMessageFormatter = logMessageTemplateFormatter2.FormatMessage;
			AutoMessageTemplateFormatter = ((logMessageTemplateFormatter.EnableMessageTemplateParser == true) ? null : logMessageTemplateFormatter2);
		}
		else
		{
			SingleTargetMessageFormatter = null;
			AutoMessageTemplateFormatter = null;
		}
		return ActiveMessageFormatter;
	}

	internal static void LogNLogAssemblyVersion()
	{
		try
		{
			AssemblyHelpers.LogAssemblyVersion(typeof(LogFactory).Assembly);
		}
		catch (Exception ex)
		{
			InternalLogger.Debug(ex, "Not running in full trust");
		}
	}

	/// <summary>
	/// Shutdown logging
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Begins configuration of the LogFactory options using fluent interface
	/// </summary>
	public ISetupBuilder Setup()
	{
		return new SetupBuilder(this);
	}

	/// <summary>
	/// Begins configuration of the LogFactory options using fluent interface
	/// </summary>
	public LogFactory Setup(Action<ISetupBuilder> setupBuilder)
	{
		Guard.ThrowIfNull(setupBuilder, "setupBuilder");
		setupBuilder(new SetupBuilder(this));
		return this;
	}

	/// <summary>
	/// Creates a logger that discards all log messages.
	/// </summary>
	/// <returns>Null logger instance.</returns>
	public Logger CreateNullLogger()
	{
		return new NullLogger(this);
	}

	/// <summary>
	/// Gets the logger with the full name of the current class, so namespace and class name.
	/// </summary>
	/// <returns>The logger.</returns>
	/// <remarks>This method introduces performance hit, because of StackTrace capture.
	/// Make sure you are not calling this method in a loop.</remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public Logger GetCurrentClassLogger()
	{
		string classFullName = StackTraceUsageUtils.GetClassFullName(new StackFrame(1, fNeedFileInfo: false));
		return GetLogger(classFullName);
	}

	/// <summary>
	/// Gets the logger with the full name of the current class, so namespace and class name.
	/// Use <typeparamref name="T" />  to create instance of a custom <see cref="T:NLog.Logger" />.
	/// If you haven't defined your own <see cref="T:NLog.Logger" /> class, then use the overload without the type parameter.
	/// </summary>
	/// <returns>The logger with type <typeparamref name="T" />.</returns>
	/// <typeparam name="T">Type of the logger</typeparam>
	/// <remarks>This method introduces performance hit, because of StackTrace capture.
	/// Make sure you are not calling this method in a loop.</remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public T GetCurrentClassLogger<T>() where T : Logger, new()
	{
		string classFullName = StackTraceUsageUtils.GetClassFullName(new StackFrame(1, fNeedFileInfo: false));
		return GetLogger<T>(classFullName);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogFactory.GetCurrentClassLogger``1" /> with NLog v5.2.
	/// Gets a custom logger with the full name of the current class, so namespace and class name.
	/// Use <paramref name="loggerType" /> to create instance of a custom <see cref="T:NLog.Logger" />.
	/// If you haven't defined your own <see cref="T:NLog.Logger" /> class, then use the overload without the loggerType.
	/// </summary>
	/// <param name="loggerType">The type of the logger to create. The type must inherit from <see cref="T:NLog.Logger" /></param>
	/// <returns>The logger of type <paramref name="loggerType" />.</returns>
	/// <remarks>This method introduces performance hit, because of StackTrace capture.
	/// Make sure you are not calling this method in a loop.</remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	[Obsolete("Replaced by GetCurrentClassLogger<T>(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Logger GetCurrentClassLogger([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type loggerType)
	{
		string classFullName = StackTraceUsageUtils.GetClassFullName(new StackFrame(1, fNeedFileInfo: false));
		return GetLogger(classFullName, loggerType ?? typeof(Logger));
	}

	/// <summary>
	/// Gets the specified named logger.
	/// </summary>
	/// <param name="name">Name of the logger.</param>
	/// <returns>The logger reference. Multiple calls to <c>GetLogger</c> with the same argument
	/// are not guaranteed to return the same logger reference.</returns>
	public Logger GetLogger(string name)
	{
		return GetLoggerThreadSafe(name, Logger.DefaultLoggerType, (Type t) => new Logger());
	}

	/// <summary>
	/// Gets the specified named logger.
	/// Use <typeparamref name="T" />  to create instance of a custom <see cref="T:NLog.Logger" />.
	/// If you haven't defined your own <see cref="T:NLog.Logger" /> class, then use the overload without the type parameter.
	/// </summary>
	/// <param name="name">Name of the logger.</param>
	/// <typeparam name="T">Type of the logger</typeparam>
	/// <returns>The logger reference with type <typeparamref name="T" />. Multiple calls to <c>GetLogger</c> with the same argument
	/// are not guaranteed to return the same logger reference.</returns>
	public T GetLogger<T>(string name) where T : Logger, new()
	{
		return (T)GetLoggerThreadSafe(name, typeof(T), (Type t) => new T());
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogFactory.GetLogger``1(System.String)" /> with NLog v5.2.
	/// Gets the specified named logger.
	/// Use <paramref name="loggerType" /> to create instance of a custom <see cref="T:NLog.Logger" />.
	/// If you haven't defined your own <see cref="T:NLog.Logger" /> class, then use the overload without the loggerType.
	/// </summary>
	/// <param name="name">Name of the logger.</param>
	/// <param name="loggerType">The type of the logger to create. The type must inherit from <see cref="T:NLog.Logger" />.</param>
	/// <returns>The logger of type <paramref name="loggerType" />. Multiple calls to <c>GetLogger</c> with the
	/// same argument aren't guaranteed to return the same logger reference.</returns>
	[Obsolete("Replaced by GetLogger<T>(). Marked obsolete on NLog 5.2")]
	[UnconditionalSuppressMessage("Trimming - Ignore since obsolete", "IL2067")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Logger GetLogger(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type loggerType)
	{
		return GetLoggerThreadSafe(name, loggerType ?? typeof(Logger), (Type t) => (!Logger.DefaultLoggerType.IsAssignableFrom(t)) ? null : (Activator.CreateInstance(t, nonPublic: true) as Logger));
	}

	private bool RefreshExistingLoggers()
	{
		List<Logger> loggers;
		bool result;
		lock (_syncRoot)
		{
			_config?.InitializeAll();
			loggers = _loggerCache.GetLoggers();
			result = loggers.Count != _loggerCache.Count;
		}
		foreach (Logger item in loggers)
		{
			item.SetConfiguration(BuildLoggerConfiguration(item.Name));
		}
		return result;
	}

	/// <summary>
	/// Loops through all loggers previously returned by GetLogger and recalculates their
	/// target and filter list. Useful after modifying the configuration programmatically
	/// to ensure that all loggers have been properly configured.
	/// </summary>
	public void ReconfigExistingLoggers()
	{
		RefreshExistingLoggers();
	}

	/// <summary>
	/// Loops through all loggers previously returned by GetLogger and recalculates their
	/// target and filter list. Useful after modifying the configuration programmatically
	/// to ensure that all loggers have been properly configured.
	/// </summary>
	/// <param name="purgeObsoleteLoggers">Purge garbage collected logger-items from the cache</param>
	public void ReconfigExistingLoggers(bool purgeObsoleteLoggers)
	{
		purgeObsoleteLoggers = RefreshExistingLoggers() && purgeObsoleteLoggers;
		if (purgeObsoleteLoggers)
		{
			lock (_syncRoot)
			{
				_loggerCache.PurgeObsoleteLoggers();
			}
		}
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets) with the default timeout of 15 seconds.
	/// </summary>
	public void Flush()
	{
		Flush(DefaultFlushTimeout);
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets).
	/// </summary>
	/// <param name="timeout">Maximum time to allow for the flush. Any messages after that time
	/// will be discarded.</param>
	public void Flush(TimeSpan timeout)
	{
		FlushInternal(timeout, null);
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets).
	/// </summary>
	/// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages
	/// after that time will be discarded.</param>
	public void Flush(int timeoutMilliseconds)
	{
		Flush(TimeSpan.FromMilliseconds(timeoutMilliseconds));
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets).
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	public void Flush(AsyncContinuation asyncContinuation)
	{
		Flush(asyncContinuation, TimeSpan.MaxValue);
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets).
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	/// <param name="timeoutMilliseconds">Maximum time to allow for the flush. Any messages
	/// after that time will be discarded.</param>
	public void Flush(AsyncContinuation asyncContinuation, int timeoutMilliseconds)
	{
		Flush(asyncContinuation, TimeSpan.FromMilliseconds(timeoutMilliseconds));
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets).
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	/// <param name="timeout">Maximum time to allow for the flush. Any messages after that time will be discarded.</param>
	public void Flush(AsyncContinuation asyncContinuation, TimeSpan timeout)
	{
		FlushInternal(timeout, asyncContinuation);
	}

	private bool FlushInternal(TimeSpan flushTimeout, AsyncContinuation? asyncContinuation)
	{
		InternalLogger.Debug("LogFactory Starting Flush with timeout={0} secs", flushTimeout.TotalSeconds);
		bool result;
		try
		{
			LoggingConfiguration loggingConfiguration;
			lock (_syncRoot)
			{
				loggingConfiguration = _config;
				if (!_configLoaded)
				{
					loggingConfiguration = null;
				}
			}
			if (loggingConfiguration == null)
			{
				asyncContinuation?.Invoke(null);
				result = true;
			}
			else
			{
				asyncContinuation = ((asyncContinuation == null) ? null : AsyncHelpers.PreventMultipleCalls(asyncContinuation));
				using ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
				ManualResetEvent ev = manualResetEvent;
				AsyncContinuation asyncContinuation2 = loggingConfiguration.FlushAllTargets(delegate(Exception? exception)
				{
					asyncContinuation?.Invoke(exception);
					ev?.Set();
				});
				if (asyncContinuation2 == null)
				{
					asyncContinuation?.Invoke(null);
					result = true;
				}
				else
				{
					bool flag = manualResetEvent.WaitOne(flushTimeout);
					ev = null;
					asyncContinuation2(null);
					asyncContinuation?.Invoke(flag ? null : new NLogRuntimeException("LogFactory Flush Timeout"));
					result = flag;
				}
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "LogFactory failed to flush targets.");
			asyncContinuation?.Invoke(ex);
			result = false;
		}
		return result;
	}

	/// <summary>
	/// Flush any pending log messages
	/// </summary>
	public Task FlushAsync(CancellationToken cancellationToken)
	{
		InternalLogger.Debug("LogFactory Starting Flush Async");
		LoggingConfiguration config;
		lock (_syncRoot)
		{
			config = _config;
			if (config == null || !_configLoaded)
			{
				return Task.CompletedTask;
			}
		}
		TaskCompletionSource<bool> flushCompleted = new TaskCompletionSource<bool>();
		AsyncContinuation flushTimeoutHandler = config.FlushAllTargets(delegate
		{
			flushCompleted.SetResult(result: true);
		});
		if (flushTimeoutHandler == null)
		{
			return Task.CompletedTask;
		}
		Task task = Task.Delay(cancellationToken.CanBeCanceled ? (-1) : ((int)DefaultFlushTimeout.TotalMilliseconds), cancellationToken).ContinueWith(delegate
		{
			throw new TaskCanceledException("NLog LogFactory Flush Timeout");
		});
		task.ContinueWith(delegate
		{
			flushTimeoutHandler(null);
			flushCompleted.TrySetCanceled();
		});
		return Task.WhenAny(flushCompleted.Task, task).Unwrap();
	}

	/// <summary>
	/// Suspends the logging, and returns object for using-scope so scope-exit calls <see cref="M:NLog.LogFactory.ResumeLogging" />
	/// </summary>
	/// <remarks>
	/// Logging is suspended when the number of <see cref="M:NLog.LogFactory.SuspendLogging" /> calls are greater
	/// than the number of <see cref="M:NLog.LogFactory.ResumeLogging" /> calls.
	/// </remarks>
	/// <returns>An object that implements IDisposable whose Dispose() method re-enables logging.
	/// To be used with C# <c>using ()</c> statement.</returns>
	public IDisposable SuspendLogging()
	{
		lock (_syncRoot)
		{
			_supendLoggingCounter++;
			if (_supendLoggingCounter == 1)
			{
				ReconfigExistingLoggers();
			}
		}
		return new LogEnabler(this);
	}

	/// <summary>
	/// Resumes logging if having called <see cref="M:NLog.LogFactory.SuspendLogging" />.
	/// </summary>
	/// <remarks>
	/// Logging is suspended when the number of <see cref="M:NLog.LogFactory.SuspendLogging" /> calls are greater
	/// than the number of <see cref="M:NLog.LogFactory.ResumeLogging" /> calls.
	/// </remarks>
	public void ResumeLogging()
	{
		lock (_syncRoot)
		{
			_supendLoggingCounter--;
			if (_supendLoggingCounter == 0)
			{
				ReconfigExistingLoggers();
			}
		}
	}

	/// <summary>
	/// Returns <see langword="true" /> if logging is currently enabled.
	/// </summary>
	/// <remarks>
	/// Logging is suspended when the number of <see cref="M:NLog.LogFactory.SuspendLogging" /> calls are greater
	/// than the number of <see cref="M:NLog.LogFactory.ResumeLogging" /> calls.
	/// </remarks>
	/// <returns>A value of <see langword="true" /> if logging is currently enabled,
	/// <see langword="false" /> otherwise.</returns>
	public bool IsLoggingEnabled()
	{
		return _supendLoggingCounter <= 0;
	}

	/// <summary>
	/// Raises the event when the configuration is reloaded.
	/// </summary>
	/// <param name="e">Event arguments.</param>
	protected virtual void OnConfigurationChanged(LoggingConfigurationChangedEventArgs e)
	{
		this.ConfigurationChanged?.Invoke(this, e);
	}

	internal ITargetWithFilterChain[] BuildLoggerConfiguration(string loggerName)
	{
		LogLevel globalLogLevel = (IsLoggingEnabled() ? GlobalThreshold : LogLevel.Off);
		return _config?.BuildLoggerConfiguration(loggerName, globalLogLevel) ?? TargetWithFilterChain.NoTargetsByLevel;
	}

	private void DisposeInternal(bool closeConfig = true)
	{
		if (_isDisposing)
		{
			return;
		}
		_isDisposing = true;
		_serviceRepository.TypeRegistered -= ServiceRepository_TypeRegistered;
		LoggerShutdown -= OnStopLogging;
		if (Monitor.TryEnter(_syncRoot, 500))
		{
			try
			{
				_configLoader.Dispose();
				LoggingConfiguration config = _config;
				if (_configLoaded && config != null)
				{
					if (closeConfig)
					{
						CloseOldConfig(config);
					}
					else
					{
						InternalLogger.Warn("Target flush timeout. One or more targets did not complete flush operation, skipping target close.");
					}
				}
			}
			finally
			{
				Monitor.Exit(_syncRoot);
				this.ConfigurationChanged = null;
			}
		}
		else
		{
			this.ConfigurationChanged = null;
		}
		InternalLogger.Info("LogFactory has been disposed.");
	}

	private void CloseOldConfig(LoggingConfiguration oldConfig)
	{
		try
		{
			oldConfig.OnConfigurationAssigned(null);
			_config = null;
			_configLoaded = true;
			ReconfigExistingLoggers();
			oldConfig.Close();
			OnConfigurationChanged(new LoggingConfigurationChangedEventArgs(null, oldConfig));
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "LogFactory failed to close NLog LoggingConfiguration.");
		}
	}

	/// <summary>
	/// Shutdown logging without flushing async
	/// </summary>
	/// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources;
	/// <see langword="false" /> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			DisposeInternal();
		}
	}

	/// <summary>
	/// Dispose all targets, and shutdown logging.
	/// </summary>
	public void Shutdown()
	{
		InternalLogger.Info("LogFactory shutting down ...");
		if (!_isDisposing && _configLoaded)
		{
			lock (_syncRoot)
			{
				if (_isDisposing || !_configLoaded)
				{
					return;
				}
				Configuration = null;
				_configLoaded = true;
				ReconfigExistingLoggers();
			}
		}
		InternalLogger.Info("LogFactory shutdown completed.");
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Get file paths (including filename) for the possible NLog config files.
	/// </summary>
	/// <returns>The file paths to the possible config file</returns>
	[Obsolete("Replaced by LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public IEnumerable<string> GetCandidateConfigFilePaths()
	{
		if (_candidateConfigFilePaths != null)
		{
			return _candidateConfigFilePaths.AsReadOnly();
		}
		return _configLoader.GetDefaultCandidateConfigFilePaths();
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Get file paths (including filename) for the possible NLog config files.
	/// </summary>
	/// <returns>The file paths to the possible config file</returns>
	[Obsolete("Replaced by chaining LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	internal IEnumerable<string> GetCandidateConfigFilePaths(string? filename)
	{
		if (_candidateConfigFilePaths != null)
		{
			return GetCandidateConfigFilePaths();
		}
		return _configLoader.GetDefaultCandidateConfigFilePaths(string.IsNullOrEmpty(filename) ? null : filename);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Overwrite the candidates paths (including filename) for the possible NLog config files.
	/// </summary>
	/// <param name="filePaths">The file paths to the possible config file</param>
	[Obsolete("Replaced by chaining LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetCandidateConfigFilePaths(IEnumerable<string> filePaths)
	{
		_candidateConfigFilePaths = new List<string>();
		if (filePaths != null)
		{
			_candidateConfigFilePaths.AddRange(filePaths);
		}
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Clear the candidate file paths and return to the defaults.
	/// </summary>
	[Obsolete("Replaced by chaining LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void ResetCandidateConfigFilePath()
	{
		_candidateConfigFilePaths = null;
	}

	private Logger GetLoggerThreadSafe(string name, Type loggerType, Func<Type, Logger?> loggerCreator)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name", "Name of logger cannot be null");
		}
		LoggerCacheKey cacheKey = new LoggerCacheKey(name, loggerType);
		lock (_syncRoot)
		{
			Logger logger = _loggerCache.Retrieve(cacheKey);
			if (logger != null)
			{
				return logger;
			}
			Logger logger2 = CreateNewLogger(loggerType, loggerCreator);
			if (logger2 == null)
			{
				cacheKey = new LoggerCacheKey(cacheKey.Name, typeof(Logger));
				logger2 = new Logger();
			}
			LoggingConfiguration? obj = _config ?? ((_loggerCache.Count == 0) ? Configuration : null);
			logger2.Initialize(name, BuildLoggerConfiguration(name), this);
			if (obj == null && _loggerCache.Count == 0)
			{
				InternalLogger.Info("NLog Configuration has not been loaded.");
			}
			_loggerCache.InsertOrUpdate(cacheKey, logger2);
			return logger2;
		}
	}

	internal Logger CreateNewLogger(Type loggerType, Func<Type, Logger?> loggerCreator)
	{
		try
		{
			Logger logger = loggerCreator(loggerType);
			if (logger != null)
			{
				return logger;
			}
			if (Logger.DefaultLoggerType.IsAssignableFrom(loggerType))
			{
				throw new NLogRuntimeException($"GetLogger / GetCurrentClassLogger with type '{loggerType}' could not create instance of NLog Logger");
			}
			if (ThrowExceptions || LogManager.ThrowExceptions)
			{
				throw new NLogRuntimeException($"GetLogger / GetCurrentClassLogger with type '{loggerType}' does not inherit from NLog Logger");
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "GetLogger / GetCurrentClassLogger. Cannot create instance of type '{0}'. It should have an default constructor.", loggerType);
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
		return new Logger();
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> and <see cref="M:NLog.SetupBuilderExtensions.LoadConfigurationFromFile(NLog.Config.ISetupBuilder,System.String,System.Boolean)" /> with NLog v5.2.
	///
	/// Loads logging configuration from file (Currently only XML configuration files supported)
	/// </summary>
	/// <param name="configFile">Configuration file to be read</param>
	/// <returns>LogFactory instance for fluent interface</returns>
	[Obsolete("Replaced by LogFactory.Setup().LoadConfigurationFromFile(). Marked obsolete on NLog 5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public LogFactory LoadConfiguration(string configFile)
	{
		return LoadConfiguration(configFile, optional: false);
	}

	internal LogFactory LoadConfiguration(string? configFile, bool optional)
	{
		string text = ((configFile == null || string.IsNullOrEmpty(configFile)) ? "NLog.config" : configFile);
		if (optional && string.Equals(text.Trim(), "NLog.config", StringComparison.OrdinalIgnoreCase) && _config != null)
		{
			return this;
		}
		LoggingConfiguration loggingConfiguration = _configLoader.Load(this, configFile);
		if (loggingConfiguration == null)
		{
			if (!optional)
			{
				throw new FileNotFoundException(CreateFileNotFoundMessage(configFile), text);
			}
			return this;
		}
		Configuration = loggingConfiguration;
		return this;
	}

	private string CreateFileNotFoundMessage(string? configFile)
	{
		StringBuilder stringBuilder = new StringBuilder("Failed to load NLog LoggingConfiguration.");
		try
		{
			HashSet<string> hashSet = new HashSet<string>(_configLoader.GetDefaultCandidateConfigFilePaths(configFile));
			stringBuilder.AppendLine(" Searched the following locations:");
			foreach (string item in hashSet)
			{
				stringBuilder.Append("- ");
				stringBuilder.AppendLine(item);
			}
		}
		catch (Exception arg)
		{
			InternalLogger.Debug("Failed to GetDefaultCandidateConfigFilePaths in CreateFileNotFoundMessage: {0}", arg);
		}
		return stringBuilder.ToString();
	}

	/// <remarks>
	/// Internal for unit tests
	/// </remarks>
	internal int ResetLoggerCache()
	{
		int count = _loggerCache.Count;
		_loggerCache.Reset();
		return count;
	}

	private static void OnLoggerShutdown(object sender, EventArgs args)
	{
		try
		{
			LogFactory._loggerShutdown?.Invoke(null, args);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Error(ex, "LogFactory failed to shutdown properly.");
		}
		finally
		{
			LogFactory._loggerShutdown = null;
			if (defaultAppEnvironment != null)
			{
				defaultAppEnvironment.ProcessExit -= OnLoggerShutdown;
			}
		}
	}

	private void OnStopLogging(object sender, EventArgs args)
	{
		try
		{
			InternalLogger.Info("AppDomain Shutting down. LogFactory closing...");
			bool closeConfig = true;
			if (PlatformDetector.IsWin32)
			{
				closeConfig = FlushInternal(TimeSpan.FromMilliseconds(1500.0), null);
			}
			DisposeInternal(closeConfig);
		}
		catch (Exception ex)
		{
			if (ex.MustBeRethrownImmediately())
			{
				throw;
			}
			InternalLogger.Error(ex, "LogFactory failed to close down.");
		}
	}
}

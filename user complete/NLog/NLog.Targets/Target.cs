using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Represents logging target.
/// </summary>
[NLogConfigurationItem]
public abstract class Target : ISupportsInitialize, IInternalLoggerContext, IDisposable
{
	internal string? _tostring;

	private Layout[] _allLayouts = ArrayHelper.Empty<Layout>();

	/// <summary> Are all layouts in this target thread-agnostic, if so we don't precalculate the layouts </summary>
	private bool _allLayoutsAreThreadAgnostic;

	private bool _anyLayoutsAreThreadAgnosticImmutable;

	private bool _scannedForLayouts;

	private Exception? _initializeException;

	private string _name = string.Empty;

	private bool? _optimizeBufferReuse;

	internal bool? _layoutWithLock;

	private volatile bool _isInitialized;

	internal readonly ReusableBuilderCreator ReusableLayoutBuilder = new ReusableBuilderCreator();

	private StringBuilderPool? _precalculateStringBuilderPool;

	/// <summary>
	/// The Max StackTraceUsage of all the <see cref="T:NLog.Layouts.Layout" /> in this Target
	/// </summary>
	internal StackTraceUsage StackTraceUsage { get; private set; }

	internal Exception? InitializeException => _initializeException;

	/// <summary>
	/// Gets or sets the name of the target.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="General Options" order="1" />
	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
			_tostring = null;
		}
	}

	/// <summary>
	/// Target supports reuse of internal buffers, and doesn't have to constantly allocate new buffers
	/// Required for legacy NLog-targets, that expects buffers to remain stable after Write-method exit
	/// </summary>
	/// <docgen category="Performance Tuning Options" order="10" />
	[Obsolete("No longer used, and always returns true. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool OptimizeBufferReuse
	{
		get
		{
			return _optimizeBufferReuse ?? true;
		}
		set
		{
			_optimizeBufferReuse = (value ? new bool?(true) : ((bool?)null));
		}
	}

	/// <summary>
	/// NLog Layout are by default threadsafe, so multiple threads can be rendering logevents at the same time.
	/// This ensure high concurrency with no lock-congestion for the application-threads, especially when using <see cref="T:NLog.Targets.Wrappers.AsyncTargetWrapper" />
	/// or AsyncTaskTarget.
	///
	/// But if using custom <see cref="T:NLog.Layouts.Layout" /> or <see cref="T:NLog.LayoutRenderers.LayoutRenderer" /> that are not
	/// threadsafe, then this option can enabled to protect against thread-concurrency-issues. Allowing one
	/// to update to NLog 5.0 without having to fix custom/external layout-dependencies.
	/// </summary>
	/// <docgen category="Performance Tuning Options" order="10" />
	[Obsolete("Temporary workaround for broken Layout Renderers that are not threadsafe. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool LayoutWithLock
	{
		get
		{
			return _layoutWithLock == true;
		}
		set
		{
			_layoutWithLock = value;
		}
	}

	/// <summary>
	/// Gets the object which can be used to synchronize asynchronous operations that must rely on the .
	/// </summary>
	protected object SyncRoot { get; } = new object();

	/// <summary>
	/// Gets the logging configuration this target is part of.
	/// </summary>
	protected LoggingConfiguration? LoggingConfiguration { get; private set; }

	LogFactory? IInternalLoggerContext.LogFactory => LoggingConfiguration?.LogFactory;

	/// <summary>
	/// Gets a value indicating whether the target has been initialized.
	/// </summary>
	protected bool IsInitialized
	{
		get
		{
			if (_isInitialized)
			{
				return true;
			}
			lock (SyncRoot)
			{
				return _isInitialized;
			}
		}
	}

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="configuration">The configuration.</param>
	void ISupportsInitialize.Initialize(LoggingConfiguration? configuration)
	{
		lock (SyncRoot)
		{
			bool isInitialized = _isInitialized;
			Initialize(configuration);
			if (isInitialized && configuration != null)
			{
				FindAllLayouts();
			}
		}
	}

	/// <summary>
	/// Closes this instance.
	/// </summary>
	void ISupportsInitialize.Close()
	{
		Close();
	}

	/// <summary>
	/// Closes the target.
	/// </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Flush any pending log messages (in case of asynchronous targets).
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	public void Flush(AsyncContinuation asyncContinuation)
	{
		Guard.ThrowIfNull(asyncContinuation, "asyncContinuation");
		asyncContinuation = AsyncHelpers.PreventMultipleCalls(asyncContinuation);
		if (Monitor.TryEnter(SyncRoot, 15000))
		{
			try
			{
				if (!IsInitialized)
				{
					asyncContinuation(null);
					return;
				}
				try
				{
					FlushAsync(asyncContinuation);
					return;
				}
				catch (Exception exception)
				{
					if (ExceptionMustBeRethrown(exception, "Flush"))
					{
						throw;
					}
					asyncContinuation(exception);
					return;
				}
			}
			finally
			{
				Monitor.Exit(SyncRoot);
			}
		}
		asyncContinuation(new NLogRuntimeException($"Target {this} failed to flush after lock timeout."));
	}

	/// <summary>
	/// Calls the <see cref="M:NLog.Layouts.Layout.Precalculate(NLog.LogEventInfo)" /> on each volatile layout
	/// used by this target.
	/// This method won't prerender if all layouts in this target are thread-agnostic.
	/// </summary>
	/// <param name="logEvent">
	/// The log event.
	/// </param>
	public void PrecalculateVolatileLayouts(LogEventInfo logEvent)
	{
		if ((!_allLayoutsAreThreadAgnostic || (_anyLayoutsAreThreadAgnosticImmutable && !logEvent.IsLogEventThreadAgnosticImmutable())) && IsInitialized)
		{
			if (_layoutWithLock == true)
			{
				PrecalculateVolatileLayoutsWithLock(logEvent);
			}
			else
			{
				PrecalculateVolatileLayoutsConcurrent(logEvent);
			}
		}
	}

	private void PrecalculateVolatileLayoutsConcurrent(LogEventInfo logEvent)
	{
		if (_precalculateStringBuilderPool == null)
		{
			Interlocked.CompareExchange(ref _precalculateStringBuilderPool, new StringBuilderPool(Environment.ProcessorCount * 2), null);
		}
		StringBuilderPool.ItemHolder itemHolder = _precalculateStringBuilderPool.Acquire();
		try
		{
			Layout[] allLayouts = _allLayouts;
			foreach (Layout obj in allLayouts)
			{
				itemHolder.Item.ClearBuilder();
				obj.PrecalculateBuilder(logEvent, itemHolder.Item);
			}
		}
		finally
		{
			((IDisposable)itemHolder/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void PrecalculateVolatileLayoutsWithLock(LogEventInfo logEvent)
	{
		lock (SyncRoot)
		{
			ReusableObjectCreator<System.Text.StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
			try
			{
				Layout[] allLayouts = _allLayouts;
				foreach (Layout obj in allLayouts)
				{
					lockOject.Result.ClearBuilder();
					obj.PrecalculateBuilder(logEvent, lockOject.Result);
				}
			}
			finally
			{
				((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
			}
		}
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return _tostring ?? (_tostring = GenerateTargetToString(targetWrapper: false));
	}

	internal string GenerateTargetToString(bool targetWrapper, string? targetName = null)
	{
		TargetAttribute firstCustomAttribute = GetType().GetFirstCustomAttribute<TargetAttribute>();
		string text = (firstCustomAttribute?.Name ?? GetType().Name).Trim();
		targetWrapper = targetWrapper || (firstCustomAttribute != null && firstCustomAttribute.IsCompound) || (firstCustomAttribute?.IsWrapper ?? false);
		if (!targetWrapper && text.IndexOf("Target", StringComparison.OrdinalIgnoreCase) < 0)
		{
			text += "Target";
		}
		targetName = targetName ?? Name;
		if (string.IsNullOrEmpty(targetName))
		{
			if (!targetWrapper)
			{
				return text + "([unnamed])";
			}
			return text;
		}
		return text + "(Name=" + targetName + ")";
	}

	/// <summary>
	/// Writes the log to the target.
	/// </summary>
	/// <param name="logEvent">Log event to write.</param>
	public void WriteAsyncLogEvent(AsyncLogEventInfo logEvent)
	{
		if (!IsInitialized)
		{
			lock (SyncRoot)
			{
				logEvent.Continuation(null);
				return;
			}
		}
		if (_initializeException != null)
		{
			lock (SyncRoot)
			{
				WriteFailedNotInitialized(logEvent, _initializeException);
				return;
			}
		}
		AsyncContinuation asyncContinuation = AsyncHelpers.PreventMultipleCalls(logEvent.Continuation);
		AsyncLogEventInfo logEvent2 = logEvent.LogEvent.WithContinuation(asyncContinuation);
		try
		{
			WriteAsyncThreadSafe(logEvent2);
		}
		catch (Exception exception)
		{
			if (ExceptionMustBeRethrown(exception, "WriteAsyncLogEvent"))
			{
				throw;
			}
			logEvent2.Continuation(exception);
		}
	}

	/// <summary>
	/// Writes the array of log events.
	/// </summary>
	/// <param name="logEvents">The log events.</param>
	public void WriteAsyncLogEvents(params AsyncLogEventInfo[] logEvents)
	{
		if (logEvents != null && logEvents.Length != 0)
		{
			WriteAsyncLogEvents((IList<AsyncLogEventInfo>)logEvents);
		}
	}

	/// <summary>
	/// Writes the array of log events.
	/// </summary>
	/// <param name="logEvents">The log events.</param>
	public void WriteAsyncLogEvents(IList<AsyncLogEventInfo> logEvents)
	{
		if (logEvents == null || logEvents.Count == 0)
		{
			return;
		}
		if (!IsInitialized)
		{
			lock (SyncRoot)
			{
				for (int i = 0; i < logEvents.Count; i++)
				{
					logEvents[i].Continuation(null);
				}
				return;
			}
		}
		if (_initializeException != null)
		{
			lock (SyncRoot)
			{
				for (int j = 0; j < logEvents.Count; j++)
				{
					WriteFailedNotInitialized(logEvents[j], _initializeException);
				}
				return;
			}
		}
		for (int k = 0; k < logEvents.Count; k++)
		{
			logEvents[k] = logEvents[k].LogEvent.WithContinuation(AsyncHelpers.PreventMultipleCalls(logEvents[k].Continuation));
		}
		try
		{
			WriteAsyncThreadSafe(logEvents);
		}
		catch (Exception exception)
		{
			if (ExceptionMustBeRethrown(exception, "WriteAsyncLogEvents"))
			{
				throw;
			}
			for (int l = 0; l < logEvents.Count; l++)
			{
				logEvents[l].Continuation(exception);
			}
		}
	}

	/// <summary>
	/// LogEvent is written to target, but target failed to successfully initialize
	/// </summary>
	protected virtual void WriteFailedNotInitialized(AsyncLogEventInfo logEvent, Exception initializeException)
	{
		if (!_scannedForLayouts)
		{
			_scannedForLayouts = true;
			InternalLogger.Error(_initializeException, "{0}: No output because target failed initialize.", this);
		}
		else
		{
			InternalLogger.Debug("{0}: No output because target failed initialize. {1} {2}", this, _initializeException?.GetType(), _initializeException?.Message);
		}
		NLogRuntimeException exception = new NLogRuntimeException($"Target {this} failed to initialize.", initializeException);
		logEvent.Continuation(exception);
	}

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="configuration">The configuration.</param>
	internal void Initialize(LoggingConfiguration? configuration)
	{
		lock (SyncRoot)
		{
			LoggingConfiguration = configuration;
			if (IsInitialized)
			{
				return;
			}
			try
			{
				_scannedForLayouts = false;
				InitializeTarget();
				_initializeException = null;
				if (!_scannedForLayouts)
				{
					InternalLogger.Debug("{0}: InitializeTarget is done but not scanned For Layouts", this);
					FindAllLayouts();
				}
			}
			catch (NLogDependencyResolveException initializeException)
			{
				NLogDependencyResolveException exception = (NLogDependencyResolveException)(_initializeException = initializeException);
				_scannedForLayouts = false;
				if (ExceptionMustBeRethrown(exception, "Initialize"))
				{
					throw;
				}
			}
			catch (Exception initializeException2)
			{
				Exception ex = (_initializeException = initializeException2);
				_scannedForLayouts = false;
				if (ExceptionMustBeRethrown(ex, "Initialize"))
				{
					throw;
				}
				LogFactory logFactory = LoggingConfiguration?.LogFactory ?? LogManager.LogFactory;
				if (logFactory.ThrowConfigExceptions ?? logFactory.ThrowExceptions)
				{
					throw new NLogConfigurationException($"Error during initialization of target {this}", ex);
				}
			}
			finally
			{
				_isInitialized = true;
			}
		}
	}

	/// <summary>
	/// Closes this instance.
	/// </summary>
	internal void Close()
	{
		lock (SyncRoot)
		{
			LoggingConfiguration = null;
			if (!IsInitialized)
			{
				return;
			}
			_isInitialized = false;
			try
			{
				if (_initializeException == null)
				{
					InternalLogger.Debug("{0}: Closing...", this);
					CloseTarget();
					InternalLogger.Debug("{0}: Closed.", this);
				}
			}
			catch (Exception exception)
			{
				if (ExceptionMustBeRethrown(exception, "Close"))
				{
					throw;
				}
			}
		}
	}

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources.
	/// </summary>
	/// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (disposing && _isInitialized)
		{
			_isInitialized = false;
			if (_initializeException == null)
			{
				CloseTarget();
			}
		}
	}

	/// <summary>
	/// Initializes the target before writing starts
	/// </summary>
	protected virtual void InitializeTarget()
	{
		FindAllLayouts();
	}

	private void FindAllLayouts()
	{
		List<Layout> list = ObjectGraphScanner.FindReachableObjects<Layout>(ConfigurationItemFactory.Default, aggressiveSearch: false, new object[1] { this });
		InternalLogger.Trace("{0} has {1} layouts", this, list.Count);
		_allLayoutsAreThreadAgnostic = list.All((Layout layout) => layout.ThreadAgnostic);
		_anyLayoutsAreThreadAgnosticImmutable = _allLayoutsAreThreadAgnostic && list.Any((Layout layout) => layout.ThreadAgnosticImmutable);
		StackTraceUsage stackTraceUsage = list.Aggregate(StackTraceUsage.None, (StackTraceUsage agg, Layout layout) => agg | layout.StackTraceUsage);
		StackTraceUsage = stackTraceUsage | ((this as IUsesStackTrace)?.StackTraceUsage ?? StackTraceUsage.None);
		_allLayouts = list.Where((Layout l) => !l.ThreadAgnostic || l.ThreadAgnosticImmutable || !(l is SimpleLayout)).Distinct(SingleItemOptimizedHashSet<Layout>.ReferenceEqualityComparer.Default).ToArray();
		_scannedForLayouts = true;
	}

	/// <summary>
	/// Closes the target to release any initialized resources
	/// </summary>
	protected virtual void CloseTarget()
	{
	}

	/// <summary>
	/// Flush any pending log messages
	/// </summary>
	/// <remarks>The asynchronous continuation parameter must be called on flush completed</remarks>
	/// <param name="asyncContinuation">The asynchronous continuation to be called on flush completed.</param>
	protected virtual void FlushAsync(AsyncContinuation asyncContinuation)
	{
		asyncContinuation(null);
	}

	/// <summary>
	/// Writes logging event to the target destination
	/// </summary>
	/// <param name="logEvent">Logging event to be written out.</param>
	protected virtual void Write(LogEventInfo logEvent)
	{
	}

	/// <summary>
	/// Writes async log event to the log target.
	/// </summary>
	/// <param name="logEvent">Async Log event to be written out.</param>
	protected virtual void Write(AsyncLogEventInfo logEvent)
	{
		try
		{
			Write(logEvent.LogEvent);
			logEvent.Continuation(null);
		}
		catch (Exception exception)
		{
			if (ExceptionMustBeRethrown(exception, "Write"))
			{
				throw;
			}
			logEvent.Continuation(exception);
		}
	}

	/// <summary>
	/// Writes a log event to the log target, in a thread safe manner.
	/// Any override of this method has to provide their own synchronization mechanism.
	///
	/// !WARNING! Custom targets should only override this method if able to provide their
	/// own synchronization mechanism. <see cref="T:NLog.Layouts.Layout" />-objects are not guaranteed to be
	/// thread-safe, so using them without a SyncRoot-object can be dangerous.
	/// </summary>
	/// <param name="logEvent">Log event to be written out.</param>
	protected virtual void WriteAsyncThreadSafe(AsyncLogEventInfo logEvent)
	{
		lock (SyncRoot)
		{
			if (!IsInitialized)
			{
				logEvent.Continuation(null);
			}
			else
			{
				Write(logEvent);
			}
		}
	}

	/// <summary>
	/// Writes an array of logging events to the log target. By default it iterates on all
	/// events and passes them to "Write" method. Inheriting classes can use this method to
	/// optimize batch writes.
	/// </summary>
	/// <param name="logEvents">Logging events to be written out.</param>
	protected virtual void Write(IList<AsyncLogEventInfo> logEvents)
	{
		for (int i = 0; i < logEvents.Count; i++)
		{
			Write(logEvents[i]);
		}
	}

	/// <summary>
	/// Writes an array of logging events to the log target, in a thread safe manner.
	/// Any override of this method has to provide their own synchronization mechanism.
	///
	/// !WARNING! Custom targets should only override this method if able to provide their
	/// own synchronization mechanism. <see cref="T:NLog.Layouts.Layout" />-objects are not guaranteed to be
	/// thread-safe, so using them without a SyncRoot-object can be dangerous.
	/// </summary>
	/// <param name="logEvents">Logging events to be written out.</param>
	protected virtual void WriteAsyncThreadSafe(IList<AsyncLogEventInfo> logEvents)
	{
		lock (SyncRoot)
		{
			if (!IsInitialized)
			{
				for (int i = 0; i < logEvents.Count; i++)
				{
					logEvents[i].Continuation(null);
				}
			}
			else
			{
				Write(logEvents);
			}
		}
	}

	/// <summary>
	/// Merges (copies) the event context properties from any event info object stored in
	/// parameters of the given event info object.
	/// </summary>
	/// <param name="logEvent">The event info object to perform the merge to.</param>
	[Obsolete("Logger.Trace(logEvent) now automatically captures the logEvent Properties. Marked obsolete on NLog 4.6")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected void MergeEventProperties(LogEventInfo logEvent)
	{
		if (logEvent.Parameters == null || logEvent.Parameters.Length == 0)
		{
			return;
		}
		for (int i = 0; i < logEvent.Parameters.Length; i++)
		{
			if (!(logEvent.Parameters[i] is LogEventInfo { HasProperties: not false } logEventInfo))
			{
				continue;
			}
			foreach (object key in logEventInfo.Properties.Keys)
			{
				logEvent.Properties.Add(key, logEventInfo.Properties[key]);
			}
			logEventInfo.Properties.Clear();
		}
	}

	/// <summary>
	/// Renders the logevent into a string-result using the provided layout
	/// </summary>
	/// <param name="layout">The layout.</param>
	/// <param name="logEvent">The logevent info.</param>
	/// <returns>String representing log event.</returns>
	protected string RenderLogEvent(Layout? layout, LogEventInfo logEvent)
	{
		if (layout == null || logEvent == null)
		{
			return string.Empty;
		}
		if (layout is IStringValueRenderer stringValueRenderer)
		{
			string formattedString = stringValueRenderer.GetFormattedString(logEvent);
			if (formattedString != null)
			{
				return formattedString;
			}
		}
		if (TryGetCachedValue(layout, logEvent, out object value))
		{
			return value?.ToString() ?? string.Empty;
		}
		ReusableObjectCreator<System.Text.StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
		try
		{
			return layout.RenderAllocateBuilder(logEvent, lockOject.Result);
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
	}

	/// <summary>
	/// Renders the logevent into a result-value by using the provided layout
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="layout">The layout.</param>
	/// <param name="logEvent">The logevent info.</param>
	/// <param name="defaultValue">Fallback value when no value available</param>
	/// <returns>Result value when available, else fallback to defaultValue</returns>
	protected T? RenderLogEvent<T>(Layout<T>? layout, LogEventInfo logEvent, T? defaultValue = default(T?))
	{
		if (layout == null)
		{
			return defaultValue;
		}
		if (layout.IsFixed)
		{
			return layout.FixedValue;
		}
		if (logEvent == null)
		{
			return defaultValue;
		}
		if (TryGetCachedValue(layout, logEvent, out object value))
		{
			if (value != null)
			{
				return (T)value;
			}
			return defaultValue;
		}
		ReusableObjectCreator<System.Text.StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
		try
		{
			return layout.RenderTypedValue(logEvent, lockOject.Result, defaultValue);
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
	}

	/// <summary>
	/// Resolve from DI <see cref="P:NLog.LogFactory.ServiceRepository" />
	/// </summary>
	/// <remarks>Avoid calling this while handling a LogEvent, since random deadlocks can occur.</remarks>
	protected T ResolveService<T>() where T : class
	{
		return LoggingConfiguration.GetServiceProvider().ResolveService<T>(IsInitialized);
	}

	/// <summary>
	/// Should the exception be rethrown?
	/// </summary>
	/// <remarks>Upgrade to private protected when using C# 7.2 </remarks>
	internal bool ExceptionMustBeRethrown(Exception exception, [CallerMemberName] string? callerMemberName = null)
	{
		return exception.MustBeRethrown(this, callerMemberName);
	}

	private static bool TryGetCachedValue(Layout layout, LogEventInfo logEvent, out object? value)
	{
		if (layout.ThreadAgnostic && !layout.ThreadAgnosticImmutable)
		{
			value = null;
			return false;
		}
		return logEvent.TryGetCachedLayoutValue(layout, out value);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom Target.
	/// </summary>
	/// <remarks>Short-cut for registering to default <see cref="T:NLog.Config.ConfigurationItemFactory" /></remarks>
	/// <typeparam name="T">Type of the Target.</typeparam>
	/// <param name="name">The target type-alias for use in NLog configuration</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string name) where T : Target
	{
		Type typeFromHandle = typeof(T);
		Register(name, typeFromHandle);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom Target.
	/// </summary>
	/// <remarks>Short-cut for registering to default <see cref="T:NLog.Config.ConfigurationItemFactory" /></remarks>
	/// <param name="targetType">Type of the Target.</param>
	/// <param name="name">The target type-alias for use in NLog configuration</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] Type targetType)
	{
		ConfigurationItemFactory.Default.GetTargetFactory().RegisterDefinition(name, targetType);
	}
}

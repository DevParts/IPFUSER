using System;
using System.Diagnostics;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Writes log message to the Event Log.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/EventLog-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/EventLog-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/EventLog/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/EventLog/Simple/Example.cs" />
/// </example>
[Target("EventLog")]
public class EventLogTarget : TargetWithLayout, IInstallable
{
	/// <summary>
	/// A wrapper for Windows event log.
	/// </summary>
	internal interface IEventLogWrapper
	{
		/// <summary>
		/// A wrapper for the property <see cref="P:System.Diagnostics.EventLog.Source" />.
		/// </summary>
		string Source { get; }

		/// <summary>
		/// A wrapper for the property <see cref="P:System.Diagnostics.EventLog.Log" />.
		/// </summary>
		string Log { get; }

		/// <summary>
		/// A wrapper for the property <see cref="P:System.Diagnostics.EventLog.MachineName" />.
		/// </summary>
		string MachineName { get; }

		/// <summary>
		/// A wrapper for the property <see cref="P:System.Diagnostics.EventLog.MaximumKilobytes" />.
		/// </summary>
		long MaximumKilobytes { get; set; }

		/// <summary>
		/// Indicates whether an event log instance is associated.
		/// </summary>
		bool IsEventLogAssociated { get; }

		/// <summary>
		/// A wrapper for the method <see cref="M:System.Diagnostics.EventLog.WriteEntry(System.String,System.Diagnostics.EventLogEntryType,System.Int32,System.Int16)" />.
		/// </summary>
		void WriteEntry(string message, EventLogEntryType entryType, int eventId, short category);

		/// <summary>
		/// Creates a new association with an instance of the event log.
		/// </summary>
		void AssociateNewEventLog(string logName, string machineName, string source);

		/// <summary>
		/// A wrapper for the static method <see cref="M:System.Diagnostics.EventLog.DeleteEventSource(System.String,System.String)" />.
		/// </summary>
		void DeleteEventSource(string source, string machineName);

		/// <summary>
		/// A wrapper for the static method <see cref="M:System.Diagnostics.EventLog.SourceExists(System.String,System.String)" />.
		/// </summary>
		bool SourceExists(string source, string machineName);

		/// <summary>
		/// A wrapper for the static method <see cref="M:System.Diagnostics.EventLog.LogNameFromSourceName(System.String,System.String)" />.
		/// </summary>
		string LogNameFromSourceName(string source, string machineName);

		/// <summary>
		/// A wrapper for the static method <see cref="M:System.Diagnostics.EventLog.CreateEventSource(System.Diagnostics.EventSourceCreationData)" />.
		/// </summary>
		void CreateEventSource(EventSourceCreationData sourceData);
	}

	/// <summary>
	/// The implementation of <see cref="T:NLog.Targets.EventLogTarget.IEventLogWrapper" />, that uses Windows <see cref="T:System.Diagnostics.EventLog" />.
	/// </summary>
	private sealed class EventLogWrapper : IEventLogWrapper, IDisposable
	{
		private EventLog? _windowsEventLog;

		public string Source { get; private set; } = string.Empty;

		public string Log { get; private set; } = string.Empty;

		public string MachineName { get; private set; } = string.Empty;

		public long MaximumKilobytes
		{
			get
			{
				return _windowsEventLog?.MaximumKilobytes ?? 0;
			}
			set
			{
				if (_windowsEventLog != null)
				{
					_windowsEventLog.MaximumKilobytes = value;
				}
			}
		}

		public bool IsEventLogAssociated => _windowsEventLog != null;

		public void WriteEntry(string message, EventLogEntryType entryType, int eventId, short category)
		{
			_windowsEventLog?.WriteEntry(message, entryType, eventId, category);
		}

		/// <summary>
		/// Creates a new association with an instance of Windows <see cref="T:System.Diagnostics.EventLog" />.
		/// </summary>
		public void AssociateNewEventLog(string logName, string machineName, string source)
		{
			EventLog? windowsEventLog = _windowsEventLog;
			_windowsEventLog = new EventLog(logName, machineName, source);
			Source = source;
			Log = logName;
			MachineName = machineName;
			windowsEventLog?.Dispose();
		}

		public void DeleteEventSource(string source, string machineName)
		{
			EventLog.DeleteEventSource(source, machineName);
		}

		public bool SourceExists(string source, string machineName)
		{
			return EventLog.SourceExists(source, machineName);
		}

		public string LogNameFromSourceName(string source, string machineName)
		{
			return EventLog.LogNameFromSourceName(source, machineName);
		}

		public void CreateEventSource(EventSourceCreationData sourceData)
		{
			EventLog.CreateEventSource(sourceData);
		}

		public void Dispose()
		{
			_windowsEventLog?.Dispose();
			_windowsEventLog = null;
		}
	}

	/// <summary>
	/// Max size in characters (limitation of the EventLog API).
	/// </summary>
	/// <seealso href="https://docs.microsoft.com/en-gb/windows/win32/api/winbase/nf-winbase-reporteventw" />
	internal const int EventLogMaxMessageLength = 30000;

	private readonly IEventLogWrapper _eventLogWrapper;

	/// <summary>
	/// Gets or sets the name of the machine on which Event Log service is running.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout MachineName { get; set; } = NLog.Layouts.Layout.Empty;

	/// <summary>
	/// Gets or sets the layout that renders event ID.
	/// </summary>
	/// <remarks>Default: <code>${event-properties:item=EventId}</code></remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout<int> EventId { get; set; } = "${event-properties:item=EventId}";

	/// <summary>
	/// Gets or sets the layout that renders event Category.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout<short>? Category { get; set; }

	/// <summary>
	/// Optional entry type. When not set, or when not convertible to <see cref="T:System.Diagnostics.EventLogEntryType" /> then determined by <see cref="T:NLog.LogLevel" />
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout<EventLogEntryType>? EntryType { get; set; }

	/// <summary>
	/// Gets or sets the value to be used as the event Source.
	/// </summary>
	/// <remarks>
	/// <b>[Required]</b> Default: <see cref="P:System.AppDomain.FriendlyName" />
	/// </remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout Source { get; set; } = NLog.Layouts.Layout.Empty;

	/// <summary>
	/// Gets or sets the name of the Event Log to write to. This can be System, Application or any user-defined name.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <c>Application</c></remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout Log { get; set; } = "Application";

	/// <summary>
	/// Gets or sets the message length limit to write to the Event Log.
	/// </summary>
	/// <remarks>Default: <see langword="30000" /></remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout<int> MaxMessageLength { get; set; } = 30000;

	/// <summary>
	/// Gets or sets the maximum Event log size in kilobytes.
	/// </summary>
	/// <remarks>
	/// <value>MaxKilobytes</value> cannot be less than 64 or greater than 4194240 or not a multiple of 64.
	/// If <c>null</c>, the value will not be specified while creating the Event log.
	/// </remarks>
	/// <docgen category="Event Log Options" order="10" />
	public Layout<long>? MaxKilobytes { get; set; }

	/// <summary>
	/// Gets or sets the action to take if the message is larger than the <see cref="P:NLog.Targets.EventLogTarget.MaxMessageLength" /> option.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.Targets.EventLogTargetOverflowAction.Truncate" /></remarks>
	/// <docgen category="Event Log Options" order="100" />
	public EventLogTargetOverflowAction OnOverflow { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.EventLogTarget" /> class.
	/// </summary>
	public EventLogTarget()
	{
		Source = AppDomain.CurrentDomain.FriendlyName;
		_eventLogWrapper = new EventLogWrapper();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.EventLogTarget" /> class.
	/// </summary>
	/// <param name="name">Name of the target.</param>
	public EventLogTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.EventLogTarget" /> class.
	/// </summary>
	internal EventLogTarget(IEventLogWrapper eventLogWrapper, string sourceName)
	{
		_eventLogWrapper = eventLogWrapper;
		Source = (string.IsNullOrEmpty(sourceName) ? AppDomain.CurrentDomain.FriendlyName : sourceName);
	}

	/// <summary>
	/// Performs installation which requires administrative permissions.
	/// </summary>
	/// <param name="installationContext">The installation context.</param>
	public void Install(InstallationContext installationContext)
	{
		CreateEventSourceIfNeeded(GetFixedSource(), alwaysThrowError: true);
	}

	/// <summary>
	/// Performs uninstallation which requires administrative permissions.
	/// </summary>
	/// <param name="installationContext">The installation context.</param>
	public void Uninstall(InstallationContext installationContext)
	{
		string fixedSource = GetFixedSource();
		if (string.IsNullOrEmpty(fixedSource))
		{
			InternalLogger.Debug("{0}: Skipping removing of event source because it contains layout renderers", this);
		}
		else
		{
			_eventLogWrapper.DeleteEventSource(fixedSource, ".");
		}
	}

	/// <summary>
	/// Determines whether the item is installed.
	/// </summary>
	/// <param name="installationContext">The installation context.</param>
	/// <returns>
	/// Value indicating whether the item is installed or null if it is not possible to determine.
	/// </returns>
	public bool? IsInstalled(InstallationContext installationContext)
	{
		string fixedSource = GetFixedSource();
		if (string.IsNullOrEmpty(fixedSource))
		{
			InternalLogger.Debug("{0}: Unclear if event source exists because it contains layout renderers", this);
			return null;
		}
		return _eventLogWrapper.SourceExists(fixedSource, ".");
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		base.InitializeTarget();
		if (Source == null || Source == NLog.Layouts.Layout.Empty)
		{
			throw new NLogConfigurationException("EventLogTarget Source-property must be assigned. Source is needed for EventLog writing.");
		}
		Layout<long>? maxKilobytes = MaxKilobytes;
		long num = ((maxKilobytes != null && maxKilobytes.IsFixed) ? MaxKilobytes.FixedValue : 0);
		if (num > 0 && (num < 64 || num > 4194240 || num % 64 != 0L))
		{
			throw new NLogConfigurationException("EventLogTarget MaxKilobytes must be a multiple of 64, and between 64 and 4194240");
		}
		CreateEventSourceIfNeeded(GetFixedSource(), alwaysThrowError: false);
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		string text = RenderLogEvent(Layout, logEvent);
		EventLogEntryType entryType = GetEntryType(logEvent);
		int eventId = RenderLogEvent(EventId, logEvent, 0);
		short category = RenderLogEvent<short>(Category, logEvent, 0);
		string text2 = RenderLogEvent(Source, logEvent);
		if (string.IsNullOrEmpty(text2))
		{
			InternalLogger.Warn("{0}: WriteEntry discarded because Source rendered as empty string", this);
			return;
		}
		string text3 = RenderLogEvent(Log, logEvent);
		if (string.IsNullOrEmpty(text3))
		{
			InternalLogger.Warn("{0}: WriteEntry discarded because Log rendered as empty string", this);
			return;
		}
		string text4 = RenderLogEvent(MachineName, logEvent);
		if (string.IsNullOrEmpty(text4))
		{
			text4 = ".";
		}
		int num = RenderLogEvent(MaxMessageLength, logEvent, 30000);
		if (num > 0 && text.Length > num)
		{
			if (OnOverflow == EventLogTargetOverflowAction.Truncate)
			{
				text = text.Substring(0, num);
				WriteEntry(text2, text3, text4, text, entryType, eventId, category);
			}
			else if (OnOverflow == EventLogTargetOverflowAction.Split)
			{
				for (int i = 0; i < text.Length; i += num)
				{
					string message = text.Substring(i, Math.Min(num, text.Length - i));
					WriteEntry(text2, text3, text4, message, entryType, eventId, category);
				}
			}
			else if (OnOverflow == EventLogTargetOverflowAction.Discard)
			{
				InternalLogger.Debug("{0}: WriteEntry discarded because too big message size: {1}", this, text.Length);
			}
		}
		else
		{
			WriteEntry(text2, text3, text4, text, entryType, eventId, category);
		}
	}

	private void WriteEntry(string eventLogSource, string eventLogName, string eventLogMachine, string message, EventLogEntryType entryType, int eventId, short category)
	{
		if (!_eventLogWrapper.IsEventLogAssociated || !(_eventLogWrapper.Source == eventLogSource) || !string.Equals(_eventLogWrapper.Log, eventLogName, StringComparison.OrdinalIgnoreCase) || !string.Equals(_eventLogWrapper.MachineName, eventLogMachine, StringComparison.OrdinalIgnoreCase))
		{
			InternalLogger.Debug("{0}: Refresh EventLog Source {1} and Log {2}", this, eventLogSource, eventLogName);
			_eventLogWrapper.AssociateNewEventLog(eventLogName, eventLogMachine, eventLogSource);
			try
			{
				if (!_eventLogWrapper.SourceExists(eventLogSource, eventLogMachine))
				{
					InternalLogger.Warn("{0}: Source {1} does not exist", this, eventLogSource);
				}
				else
				{
					string text = _eventLogWrapper.LogNameFromSourceName(eventLogSource, eventLogMachine);
					if (!text.Equals(eventLogName, StringComparison.OrdinalIgnoreCase))
					{
						InternalLogger.Debug("{0}: Source {1} should be mapped to Log {2}, but EventLog.LogNameFromSourceName returns {3}", this, eventLogSource, eventLogName, text);
					}
				}
			}
			catch (Exception ex)
			{
				if (LogManager.ThrowExceptions)
				{
					throw;
				}
				InternalLogger.Warn(ex, "{0}: Exception thrown when checking if Source {1} and Log {2} are valid", this, eventLogSource, eventLogName);
			}
		}
		_eventLogWrapper.WriteEntry(message, entryType, eventId, category);
	}

	/// <summary>
	/// Get the entry type for logging the message.
	/// </summary>
	/// <param name="logEvent">The logging event - for rendering the <see cref="P:NLog.Targets.EventLogTarget.EntryType" /></param>
	private EventLogEntryType GetEntryType(LogEventInfo logEvent)
	{
		EventLogEntryType eventLogEntryType = RenderLogEvent(EntryType, logEvent, (EventLogEntryType)0);
		if (eventLogEntryType != 0)
		{
			return eventLogEntryType;
		}
		if (logEvent.Level >= LogLevel.Error)
		{
			return EventLogEntryType.Error;
		}
		if (logEvent.Level >= LogLevel.Warn)
		{
			return EventLogEntryType.Warning;
		}
		return EventLogEntryType.Information;
	}

	/// <summary>
	/// Get the source, if and only if the source is fixed.
	/// </summary>
	/// <returns><c>null</c> when not <see cref="P:NLog.Layouts.SimpleLayout.IsFixedText" /></returns>
	/// <remarks>Internal for unit tests</remarks>
	internal string GetFixedSource()
	{
		if (Source is SimpleLayout { IsFixedText: not false } simpleLayout && Log is SimpleLayout { IsFixedText: not false } && (MachineName == null || (MachineName is SimpleLayout { IsFixedText: not false } simpleLayout3 && (".".Equals(simpleLayout3.FixedText, StringComparison.Ordinal) || string.IsNullOrEmpty(simpleLayout3.FixedText)))))
		{
			return simpleLayout.FixedText ?? string.Empty;
		}
		return string.Empty;
	}

	/// <summary>
	/// (re-)create an event source, if it isn't there. Works only with fixed source names.
	/// </summary>
	/// <param name="fixedSource">The source name. If source is not fixed (see <see cref="P:NLog.Layouts.SimpleLayout.IsFixedText" />, then pass <c>null</c> or <see cref="F:System.String.Empty" />.</param>
	/// <param name="alwaysThrowError">always throw an Exception when there is an error</param>
	private void CreateEventSourceIfNeeded(string fixedSource, bool alwaysThrowError)
	{
		if (string.IsNullOrEmpty(fixedSource))
		{
			InternalLogger.Debug("{0}: Skipping creation of event source because it contains layout renderers", this);
			return;
		}
		string text = RenderLogEvent(Log, LogEventInfo.CreateNullEvent());
		string text2 = RenderLogEvent(MachineName, LogEventInfo.CreateNullEvent());
		if (string.IsNullOrEmpty(text2))
		{
			text2 = ".";
		}
		long num = RenderLogEvent(MaxKilobytes, LogEventInfo.CreateNullEvent(), 0L);
		try
		{
			if (_eventLogWrapper.SourceExists(fixedSource, text2))
			{
				string text3 = _eventLogWrapper.LogNameFromSourceName(fixedSource, text2);
				if (!text3.Equals(text, StringComparison.OrdinalIgnoreCase))
				{
					InternalLogger.Debug("{0}: Updating source {1} to use log {2}, instead of {3} (Computer restart is needed)", this, fixedSource, text, text3);
					_eventLogWrapper.DeleteEventSource(fixedSource, text2);
					EventSourceCreationData sourceData = new EventSourceCreationData(fixedSource, text)
					{
						MachineName = text2
					};
					_eventLogWrapper.CreateEventSource(sourceData);
				}
			}
			else
			{
				InternalLogger.Debug("{0}: Creating source {1} to use log {2}", this, fixedSource, text);
				EventSourceCreationData sourceData2 = new EventSourceCreationData(fixedSource, text)
				{
					MachineName = text2
				};
				_eventLogWrapper.CreateEventSource(sourceData2);
			}
			_eventLogWrapper.AssociateNewEventLog(text, text2, fixedSource);
			if (num > 0 && _eventLogWrapper.MaximumKilobytes < num)
			{
				_eventLogWrapper.MaximumKilobytes = num;
			}
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "{0}: Error when connecting to EventLog. Source={1} in Log={2}", this, fixedSource, text);
			if (alwaysThrowError || LogManager.ThrowExceptions)
			{
				throw;
			}
		}
	}
}

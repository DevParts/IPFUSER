using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using NLog.Common;
using NLog.Internal;
using NLog.Layouts;
using NLog.MessageTemplates;
using NLog.Time;

namespace NLog;

/// <summary>
/// Represents the logging event.
/// </summary>
public class LogEventInfo
{
	/// <summary>
	/// Gets the date of the first log event created.
	/// </summary>
	public static readonly DateTime ZeroDate = DateTime.UtcNow;

	private static int globalSequenceId;

	/// <summary>
	/// The formatted log message.
	/// </summary>
	private string? _formattedMessage;

	/// <summary>
	/// The log message including any parameter placeholders
	/// </summary>
	private string _message;

	private object?[]? _parameters;

	private IFormatProvider? _formatProvider;

	private LogMessageFormatter? _messageFormatter;

	private IDictionary<Layout, object?>? _layoutCache;

	private PropertiesDictionary? _properties;

	private int _sequenceId;

	/// <summary>
	/// Gets the sequence number for this LogEvent, which monotonously increasing for each LogEvent until int-overflow
	///
	/// Marked obsolete with NLog 6.0, instead use ${counter:sequence=global} or ${guid:GeneratedFromLogEvent=true}
	/// </summary>
	[Obsolete("Use ${counter:sequence=global} instead of ${sequenceid}. Marked obsolete with NLog 6.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int SequenceID
	{
		get
		{
			if (_sequenceId == 0)
			{
				Interlocked.CompareExchange(ref _sequenceId, Interlocked.Increment(ref globalSequenceId), 0);
			}
			return _sequenceId;
		}
	}

	/// <summary>
	/// Gets or sets the timestamp of the logging event.
	/// </summary>
	public DateTime TimeStamp { get; set; }

	/// <summary>
	/// Gets or sets the level of the logging event.
	/// </summary>
	public LogLevel Level { get; set; }

	internal CallSiteInformation? CallSiteInformation { get; private set; }

	/// <summary>
	/// Gets a value indicating whether stack trace has been set for this event.
	/// </summary>
	public bool HasStackTrace => CallSiteInformation?.StackTrace != null;

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.LogEventInfo.CallerMemberName" /> or ${callsite} with NLog v5.3.
	///
	/// Gets the stack frame of the method that did the logging.
	/// </summary>
	[Obsolete("Instead use ${callsite} or CallerMemberName. Marked obsolete with NLog 5.3")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public StackFrame? UserStackFrame => CallSiteInformation?.UserStackFrame;

	/// <summary>
	/// Gets the number index of the stack frame that represents the user
	/// code (not the NLog code).
	/// </summary>
	[Obsolete("Instead use ${callsite} or CallerMemberName. Marked obsolete with NLog 5.4")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public int UserStackFrameNumber => CallSiteInformation?.UserStackFrameNumberLegacy ?? CallSiteInformation?.UserStackFrameNumber ?? 0;

	/// <summary>
	/// Gets the entire stack trace.
	/// </summary>
	public StackTrace? StackTrace => CallSiteInformation?.StackTrace;

	/// <summary>
	/// Gets the callsite class name
	/// </summary>
	public string? CallerClassName => CallSiteInformation?.GetCallerClassName(null, includeNameSpace: true, cleanAsyncMoveNext: true, cleanAnonymousDelegates: true);

	/// <summary>
	/// Gets the callsite member function name
	/// </summary>
	public string? CallerMemberName => CallSiteInformation?.GetCallerMethodName(null, includeMethodInfo: false, cleanAsyncMoveNext: true, cleanAnonymousDelegates: true);

	/// <summary>
	/// Gets the callsite source file path
	/// </summary>
	public string? CallerFilePath => CallSiteInformation?.GetCallerFilePath(0);

	/// <summary>
	/// Gets the callsite source file line number
	/// </summary>
	public int CallerLineNumber => CallSiteInformation?.GetCallerLineNumber(0) ?? 0;

	/// <summary>
	/// Gets or sets the exception information.
	/// </summary>
	public Exception? Exception { get; set; }

	/// <summary>
	/// Gets or sets the logger name.
	/// </summary>
	public string LoggerName { get; set; }

	/// <summary>
	/// Gets or sets the log message including any parameter placeholders.
	/// </summary>
	public string Message
	{
		get
		{
			return _message;
		}
		set
		{
			bool rebuildMessageTemplateParameters = ResetMessageTemplateParameters();
			_message = value;
			ResetFormattedMessage(rebuildMessageTemplateParameters);
		}
	}

	/// <summary>
	/// Gets or sets the parameter values or null if no parameters have been specified.
	/// </summary>
	public object?[]? Parameters
	{
		get
		{
			return _parameters;
		}
		set
		{
			bool rebuildMessageTemplateParameters = ResetMessageTemplateParameters();
			_parameters = value;
			ResetFormattedMessage(rebuildMessageTemplateParameters);
		}
	}

	/// <summary>
	/// Gets or sets the format provider that was provided while logging or <see langword="null" />
	/// when no formatProvider was specified.
	/// </summary>
	public IFormatProvider? FormatProvider
	{
		get
		{
			return _formatProvider;
		}
		set
		{
			if (_formatProvider != value)
			{
				_formatProvider = value;
				ResetFormattedMessage(rebuildMessageTemplateParameters: false);
			}
		}
	}

	/// <summary>
	/// Gets or sets the message formatter for generating <see cref="P:NLog.LogEventInfo.FormattedMessage" />
	/// Uses string.Format(...) when nothing else has been configured.
	/// </summary>
	public LogMessageFormatter MessageFormatter
	{
		get
		{
			return _messageFormatter ?? LogManager.LogFactory.ActiveMessageFormatter;
		}
		set
		{
			LogMessageFormatter logMessageFormatter = value ?? LogMessageStringFormatter.Default.MessageFormatter;
			if ((object)_messageFormatter != logMessageFormatter)
			{
				_messageFormatter = logMessageFormatter;
				_formattedMessage = null;
				ResetFormattedMessage(rebuildMessageTemplateParameters: false);
			}
		}
	}

	/// <summary>
	/// Gets the formatted message.
	/// </summary>
	public string FormattedMessage
	{
		get
		{
			if (_formattedMessage == null)
			{
				CalcFormattedMessage();
			}
			return _formattedMessage ?? Message;
		}
	}

	/// <summary>
	/// Checks if any per-event properties (Without allocation)
	/// </summary>
	public bool HasProperties
	{
		get
		{
			if (_properties == null)
			{
				PropertiesDictionary? propertiesDictionary = TryCreatePropertiesInternal();
				if (propertiesDictionary == null)
				{
					return false;
				}
				return propertiesDictionary.Count > 0;
			}
			return _properties.Count > 0;
		}
	}

	/// <summary>
	/// Gets the dictionary of per-event context properties.
	/// </summary>
	public IDictionary<object, object?> Properties => _properties ?? CreatePropertiesInternal();

	private bool HasMessageTemplateParameters
	{
		get
		{
			if (_formattedMessage == null)
			{
				object?[]? parameters = _parameters;
				if (parameters != null && parameters.Length != 0)
				{
					return (MessageFormatter.Target as ILogMessageFormatter)?.HasProperties(this) ?? false;
				}
			}
			return false;
		}
	}

	/// <summary>
	/// Gets the named parameters extracted from parsing <see cref="P:NLog.LogEventInfo.Message" /> as MessageTemplate
	/// </summary>
	public MessageTemplateParameters MessageTemplateParameters
	{
		get
		{
			PropertiesDictionary? properties = _properties;
			if (properties != null && properties.MessageProperties.Count > 0)
			{
				return new MessageTemplateParameters(_properties.MessageProperties, _message, _parameters);
			}
			object?[]? parameters = _parameters;
			if (parameters != null && parameters.Length != 0)
			{
				return new MessageTemplateParameters(_message, _parameters);
			}
			return NLog.MessageTemplates.MessageTemplateParameters.Empty;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	public LogEventInfo()
	{
		TimeStamp = TimeSource.Current.Time;
		_message = string.Empty;
		LoggerName = string.Empty;
		Level = LogLevel.Off;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="message">Log message including parameter placeholders.</param>
	public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string message)
		: this(level, loggerName, null, message, null, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="message">Log message including parameter placeholders.</param>
	/// <param name="messageTemplateParameters">Already parsed message template parameters.</param>
	public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string message, IList<MessageTemplateParameter>? messageTemplateParameters)
		: this(level, loggerName, null, message, null, null)
	{
		if (messageTemplateParameters == null)
		{
			return;
		}
		int count = messageTemplateParameters.Count;
		if (count > 0)
		{
			MessageTemplateParameter[] array = new MessageTemplateParameter[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = messageTemplateParameters[i];
			}
			_properties = new PropertiesDictionary(array);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="formattedMessage">Pre-formatted log message for ${message}.</param>
	/// <param name="messageTemplate">Log message-template including parameter placeholders for ${message:raw=true}.</param>
	/// <param name="messageTemplateParameters">Already parsed message template parameters.</param>
	public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string formattedMessage, [Localizable(false)] string messageTemplate, IList<MessageTemplateParameter>? messageTemplateParameters)
		: this(level, loggerName, messageTemplate, messageTemplateParameters)
	{
		_formattedMessage = formattedMessage;
		_messageFormatter = (LogEventInfo l) => l._formattedMessage ?? l.Message ?? string.Empty;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="message">Log message.</param>
	/// <param name="eventProperties">List of event-properties</param>
	public LogEventInfo(LogLevel level, string? loggerName, [Localizable(false)] string message, IReadOnlyList<KeyValuePair<object, object?>>? eventProperties)
		: this(level, loggerName, null, message, null, null)
	{
		if (eventProperties == null)
		{
			return;
		}
		int count = eventProperties.Count;
		if (count > 0)
		{
			_properties = new PropertiesDictionary(count);
			for (int i = 0; i < count; i++)
			{
				KeyValuePair<object, object> keyValuePair = eventProperties[i];
				_properties[keyValuePair.Key] = keyValuePair.Value;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">Log message including parameter placeholders.</param>
	/// <param name="parameters">Parameter array.</param>
	public LogEventInfo(LogLevel level, string? loggerName, IFormatProvider? formatProvider, [Localizable(false)] string message, object?[]? parameters)
		: this(level, loggerName, formatProvider, message, parameters, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LogEventInfo" /> class.
	/// </summary>
	/// <param name="level">Log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="formatProvider">An IFormatProvider that supplies culture-specific formatting information.</param>
	/// <param name="message">Log message including parameter placeholders.</param>
	/// <param name="parameters">Parameter array.</param>
	/// <param name="exception">Exception information.</param>
	public LogEventInfo(LogLevel level, string? loggerName, IFormatProvider? formatProvider, [Localizable(false)] string message, object?[]? parameters, Exception? exception)
	{
		TimeStamp = TimeSource.Current.Time;
		Level = level;
		LoggerName = loggerName ?? string.Empty;
		_formatProvider = formatProvider;
		_message = message;
		Parameters = parameters;
		Exception = exception;
	}

	internal CallSiteInformation GetCallSiteInformationInternal()
	{
		return CallSiteInformation ?? (CallSiteInformation = new CallSiteInformation());
	}

	/// <summary>
	/// Gets the dictionary of per-event context properties.
	/// </summary>
	/// <param name="templateParameters">Provided when having parsed the message template and capture template parameters (else null)</param>
	internal PropertiesDictionary? TryCreatePropertiesInternal(IList<MessageTemplateParameter>? templateParameters = null)
	{
		PropertiesDictionary properties = _properties;
		if (properties == null)
		{
			if ((templateParameters != null && templateParameters.Count > 0) || (templateParameters == null && HasMessageTemplateParameters))
			{
				return CreatePropertiesInternal(templateParameters);
			}
		}
		else if (templateParameters != null)
		{
			properties.ResetMessageProperties(templateParameters);
		}
		return properties;
	}

	internal PropertiesDictionary CreatePropertiesInternal(IList<MessageTemplateParameter>? templateParameters = null, int initialCapacity = 0)
	{
		if (_properties == null)
		{
			PropertiesDictionary value = ((templateParameters == null) ? new PropertiesDictionary(initialCapacity) : new PropertiesDictionary(templateParameters));
			Interlocked.CompareExchange(ref _properties, value, null);
			if (templateParameters == null && HasMessageTemplateParameters)
			{
				CalcFormattedMessage();
			}
		}
		return _properties;
	}

	/// <summary>
	/// Creates the null event.
	/// </summary>
	/// <returns>Null log event.</returns>
	public static LogEventInfo CreateNullEvent()
	{
		return new LogEventInfo(LogLevel.Off, string.Empty, null, string.Empty, null, null);
	}

	/// <summary>
	/// Creates the log event.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="message">The message.</param>
	/// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
	public static LogEventInfo Create(LogLevel logLevel, string? loggerName, [Localizable(false)] string message)
	{
		return new LogEventInfo(logLevel, loggerName, null, message, null, null);
	}

	/// <summary>
	/// Creates the log event.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="formatProvider">The format provider.</param>
	/// <param name="message">The message.</param>
	/// <param name="parameters">The parameters.</param>
	/// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
	[MessageTemplateFormatMethod("message")]
	public static LogEventInfo Create(LogLevel logLevel, string? loggerName, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object?[]? parameters)
	{
		return new LogEventInfo(logLevel, loggerName, formatProvider, message, parameters, null);
	}

	/// <summary>
	/// Creates the log event.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="formatProvider">The format provider.</param>
	/// <param name="message">The message.</param>
	/// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
	public static LogEventInfo Create(LogLevel logLevel, string? loggerName, IFormatProvider? formatProvider, object? message)
	{
		Exception ex = message as Exception;
		if (ex == null && message is LogEventInfo logEventInfo)
		{
			logEventInfo.LoggerName = loggerName ?? logEventInfo.LoggerName;
			logEventInfo.Level = logLevel;
			logEventInfo.FormatProvider = formatProvider ?? logEventInfo.FormatProvider;
			return logEventInfo;
		}
		formatProvider = formatProvider ?? ((ex != null) ? ExceptionMessageFormatProvider.Instance : null);
		return new LogEventInfo(logLevel, loggerName, formatProvider, "{0}", new object[1] { message }, ex);
	}

	/// <summary>
	/// Creates the log event.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="exception">The exception.</param>
	/// <param name="formatProvider">The format provider.</param>
	/// <param name="message">The message.</param>
	/// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
	public static LogEventInfo Create(LogLevel logLevel, string? loggerName, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)] string message)
	{
		return new LogEventInfo(logLevel, loggerName, formatProvider, message, null, exception);
	}

	/// <summary>
	/// Creates the log event.
	/// </summary>
	/// <param name="logLevel">The log level.</param>
	/// <param name="loggerName">Override default Logger name. Default <see cref="P:NLog.Logger.Name" /> is used when <c>null</c></param>
	/// <param name="exception">The exception.</param>
	/// <param name="formatProvider">The format provider.</param>
	/// <param name="message">The message.</param>
	/// <param name="parameters">The parameters.</param>
	/// <returns>Instance of <see cref="T:NLog.LogEventInfo" />.</returns>
	[MessageTemplateFormatMethod("message")]
	public static LogEventInfo Create(LogLevel logLevel, string? loggerName, Exception? exception, IFormatProvider? formatProvider, [Localizable(false)][StructuredMessageTemplate] string message, object?[]? parameters)
	{
		return new LogEventInfo(logLevel, loggerName, formatProvider, message, parameters, exception);
	}

	/// <summary>
	/// Creates <see cref="T:NLog.Common.AsyncLogEventInfo" /> from this <see cref="T:NLog.LogEventInfo" /> by attaching the specified asynchronous continuation.
	/// </summary>
	/// <param name="asyncContinuation">The asynchronous continuation.</param>
	/// <returns>Instance of <see cref="T:NLog.Common.AsyncLogEventInfo" /> with attached continuation.</returns>
	public AsyncLogEventInfo WithContinuation(AsyncContinuation asyncContinuation)
	{
		return new AsyncLogEventInfo(this, asyncContinuation);
	}

	/// <summary>
	/// Returns a string representation of this log event.
	/// </summary>
	/// <returns>String representation of the log event.</returns>
	public override string ToString()
	{
		return $"Log Event: Logger='{LoggerName}' Level={Level} Message='{FormattedMessage}'";
	}

	/// <summary>
	/// Sets the stack trace for the event info.
	/// </summary>
	/// <param name="stackTrace">The stack trace.</param>
	public void SetStackTrace(StackTrace stackTrace)
	{
		GetCallSiteInformationInternal().SetStackTrace(stackTrace);
	}

	/// <summary>
	/// Sets the stack trace for the event info.
	/// </summary>
	/// <param name="stackTrace">The stack trace.</param>
	/// <param name="userStackFrame">Index of the first user stack frame within the stack trace (Negative means NLog should skip stackframes from System-assemblies).</param>
	[Obsolete("Instead use SetStackTrace or SetCallerInfo. Marked obsolete with NLog 5.4")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetStackTrace(StackTrace stackTrace, int userStackFrame)
	{
		GetCallSiteInformationInternal().SetStackTrace(stackTrace, (userStackFrame >= 0) ? new int?(userStackFrame) : ((int?)null));
	}

	/// <summary>
	/// Sets the details retrieved from the Caller Information Attributes
	/// </summary>
	/// <param name="callerClassName"></param>
	/// <param name="callerMemberName"></param>
	/// <param name="callerFilePath"></param>
	/// <param name="callerLineNumber"></param>
	public void SetCallerInfo(string? callerClassName, string? callerMemberName, string? callerFilePath, int callerLineNumber)
	{
		GetCallSiteInformationInternal().SetCallerInfo(callerClassName, callerMemberName, callerFilePath, callerLineNumber);
	}

	internal void AddCachedLayoutValue(Layout layout, object? value)
	{
		if (_layoutCache == null)
		{
			Dictionary<Layout, object> dictionary = new Dictionary<Layout, object>();
			dictionary[layout] = value;
			if (Interlocked.CompareExchange(ref _layoutCache, dictionary, null) == null)
			{
				return;
			}
		}
		lock (_layoutCache)
		{
			_layoutCache[layout] = value;
		}
	}

	internal bool TryGetCachedLayoutValue(Layout layout, out object? value)
	{
		if (_layoutCache == null)
		{
			value = null;
			return false;
		}
		lock (_layoutCache)
		{
			return _layoutCache.TryGetValue(layout, out value);
		}
	}

	private static bool NeedToPreformatMessage(object?[]? parameters)
	{
		if (parameters == null || parameters.Length == 0)
		{
			return false;
		}
		if (parameters.Length > 5)
		{
			return true;
		}
		for (int i = 0; i < parameters.Length; i++)
		{
			if (!IsSafeToDeferFormatting(parameters[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsSafeToDeferFormatting(object? value)
	{
		return Convert.GetTypeCode(value) != TypeCode.Object;
	}

	internal bool IsLogEventThreadAgnosticImmutable()
	{
		if (Exception != null)
		{
			return false;
		}
		if (_formattedMessage != null)
		{
			object?[]? parameters = _parameters;
			if (parameters != null && parameters.Length != 0)
			{
				return false;
			}
		}
		PropertiesDictionary propertiesDictionary = TryCreatePropertiesInternal();
		if (propertiesDictionary == null || propertiesDictionary.Count == 0)
		{
			return true;
		}
		if (propertiesDictionary.Count > 5)
		{
			return false;
		}
		if (propertiesDictionary.Count == _parameters?.Length && propertiesDictionary.Count == propertiesDictionary.MessageProperties.Count)
		{
			return true;
		}
		return HasImmutableProperties(propertiesDictionary);
	}

	private static bool HasImmutableProperties(PropertiesDictionary properties)
	{
		if (properties.Count == properties.MessageProperties.Count)
		{
			for (int i = 0; i < properties.MessageProperties.Count; i++)
			{
				if (!IsSafeToDeferFormatting(properties.MessageProperties[i].Value))
				{
					return false;
				}
			}
		}
		else
		{
			using PropertiesDictionary.PropertyDictionaryEnumerator propertyDictionaryEnumerator = properties.GetPropertyEnumerator();
			while (propertyDictionaryEnumerator.MoveNext())
			{
				if (!IsSafeToDeferFormatting(propertyDictionaryEnumerator.Current.Value))
				{
					return false;
				}
			}
		}
		return true;
	}

	internal void SetMessageFormatter(LogMessageFormatter messageFormatter, LogMessageFormatter? singleTargetMessageFormatter)
	{
		bool num = _messageFormatter != null;
		if (!num)
		{
			_messageFormatter = messageFormatter;
		}
		if (num || NeedToPreformatMessage(_parameters))
		{
			CalcFormattedMessage();
		}
		else
		{
			if (singleTargetMessageFormatter == null)
			{
				return;
			}
			object?[]? parameters = _parameters;
			if (parameters != null && parameters.Length != 0)
			{
				string message = _message;
				if (message != null && message.Length < 256)
				{
					_messageFormatter = singleTargetMessageFormatter;
				}
			}
		}
	}

	private void CalcFormattedMessage()
	{
		try
		{
			_formattedMessage = MessageFormatter(this);
		}
		catch (Exception ex)
		{
			_formattedMessage = Message ?? string.Empty;
			InternalLogger.Warn(ex, "Error when formatting a message.");
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
	}

	internal void AppendFormattedMessage(ILogMessageFormatter messageFormatter, StringBuilder builder)
	{
		if (_formattedMessage != null)
		{
			builder.Append(_formattedMessage);
			return;
		}
		object?[]? parameters = _parameters;
		if (parameters != null && parameters.Length != 0 && !string.IsNullOrEmpty(_message))
		{
			int length = builder.Length;
			try
			{
				messageFormatter.AppendFormattedMessage(this, builder);
				return;
			}
			catch (Exception ex)
			{
				builder.Length = length;
				builder.Append(_message);
				InternalLogger.Warn(ex, "Error when formatting a message.");
				if (ex.MustBeRethrown())
				{
					throw;
				}
				return;
			}
		}
		builder.Append(FormattedMessage);
	}

	private void ResetFormattedMessage(bool rebuildMessageTemplateParameters)
	{
		if (_messageFormatter == null || _messageFormatter.Target is ILogMessageFormatter)
		{
			_formattedMessage = null;
		}
		if (rebuildMessageTemplateParameters && HasMessageTemplateParameters)
		{
			CalcFormattedMessage();
		}
	}

	private bool ResetMessageTemplateParameters()
	{
		if (_properties == null)
		{
			return false;
		}
		if (HasMessageTemplateParameters)
		{
			_properties.ResetMessageProperties();
			return true;
		}
		return _properties.MessageProperties.Count == 0;
	}
}

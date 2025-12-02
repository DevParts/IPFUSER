using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;
using NLog.MessageTemplates;

namespace NLog.LayoutRenderers;

/// <summary>
/// Exception information provided through
/// a call to one of the Logger.*Exception() methods.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Exception-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Exception-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("exception")]
[ThreadAgnostic]
public class ExceptionLayoutRenderer : LayoutRenderer, IRawValue
{
	private static readonly Dictionary<string, ExceptionRenderingFormat> _formatsMapping = new Dictionary<string, ExceptionRenderingFormat>(StringComparer.OrdinalIgnoreCase)
	{
		{
			"MESSAGE",
			ExceptionRenderingFormat.Message
		},
		{
			"TYPE",
			ExceptionRenderingFormat.Type
		},
		{
			"SHORTTYPE",
			ExceptionRenderingFormat.ShortType
		},
		{
			"TOSTRING",
			ExceptionRenderingFormat.ToString
		},
		{
			"METHOD",
			ExceptionRenderingFormat.Method
		},
		{
			"TARGETSITE",
			ExceptionRenderingFormat.Method
		},
		{
			"SOURCE",
			ExceptionRenderingFormat.Source
		},
		{
			"STACKTRACE",
			ExceptionRenderingFormat.StackTrace
		},
		{
			"DATA",
			ExceptionRenderingFormat.Data
		},
		{
			"@",
			ExceptionRenderingFormat.Serialize
		},
		{
			"HRESULT",
			ExceptionRenderingFormat.HResult
		},
		{
			"PROPERTIES",
			ExceptionRenderingFormat.Properties
		}
	};

	private static readonly Dictionary<ExceptionRenderingFormat, Action<ExceptionLayoutRenderer, StringBuilder, Exception, Exception?>> _renderingfunctions = new Dictionary<ExceptionRenderingFormat, Action<ExceptionLayoutRenderer, StringBuilder, Exception, Exception>>
	{
		{
			ExceptionRenderingFormat.Message,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendMessage(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.Type,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendType(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.ShortType,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendShortType(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.ToString,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendToString(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.Method,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendMethod(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.Source,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendSource(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.StackTrace,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendStackTrace(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.Data,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendData(sb, ex, aggex);
			}
		},
		{
			ExceptionRenderingFormat.Serialize,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendSerializeObject(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.HResult,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendHResult(sb, ex);
			}
		},
		{
			ExceptionRenderingFormat.Properties,
			delegate(ExceptionLayoutRenderer layout, StringBuilder sb, Exception ex, Exception? aggex)
			{
				layout.AppendProperties(sb, ex);
			}
		}
	};

	private static readonly HashSet<string> ExcludeDefaultProperties = new HashSet<string>(new string[9] { "Type", "Data", "HelpLink", "HResult", "InnerException", "Message", "Source", "StackTrace", "TargetSite" }, StringComparer.Ordinal);

	private ObjectReflectionCache? _objectReflectionCache;

	private List<ExceptionRenderingFormat> _formats = new List<ExceptionRenderingFormat>();

	private List<ExceptionRenderingFormat>? _innerFormats;

	private string _format;

	private string? _innerFormat;

	private string _separator = " ";

	private string _separatorOriginal = " ";

	private string _exceptionDataSeparator = ";";

	private string _exceptionDataSeparatorOriginal = ";";

	private ObjectReflectionCache ObjectReflectionCache => _objectReflectionCache ?? (_objectReflectionCache = new ObjectReflectionCache(base.LoggingConfiguration.GetServiceProvider()));

	/// <summary>
	/// Gets or sets the format of the output. Must be a comma-separated list of exception
	/// properties: Message, Type, ShortType, ToString, Method, StackTrace.
	/// This parameter value is case-insensitive.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <c>ToString,Data</c></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Format
	{
		get
		{
			return _format;
		}
		set
		{
			_format = value;
			_formats.Clear();
		}
	}

	/// <summary>
	/// Gets or sets the format of the output of inner exceptions. Must be a comma-separated list of exception
	/// properties: Message, Type, ShortType, ToString, Method, StackTrace.
	/// This parameter value is case-insensitive.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string? InnerFormat
	{
		get
		{
			return _innerFormat;
		}
		set
		{
			_innerFormat = value;
			_innerFormats = null;
		}
	}

	/// <summary>
	/// Gets or sets the separator used to concatenate parts specified in the Format.
	/// </summary>
	/// <remarks>Default: <c> </c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string Separator
	{
		get
		{
			return _separatorOriginal ?? _separator;
		}
		set
		{
			_separatorOriginal = value;
			_separator = SimpleLayout.Evaluate(value, base.LoggingConfiguration, null, false);
		}
	}

	/// <summary>
	/// Gets or sets the separator used to concatenate exception data specified in the Format.
	/// </summary>
	/// <remarks>Default: <c>;</c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string ExceptionDataSeparator
	{
		get
		{
			return _exceptionDataSeparatorOriginal ?? _exceptionDataSeparator;
		}
		set
		{
			_exceptionDataSeparatorOriginal = value;
			_exceptionDataSeparator = SimpleLayout.Evaluate(value, base.LoggingConfiguration, null, false);
		}
	}

	/// <summary>
	/// Gets or sets the maximum number of inner exceptions to include in the output.
	/// By default inner exceptions are not enabled for compatibility with NLog 1.0.
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public int MaxInnerExceptionLevel { get; set; }

	/// <summary>
	/// Gets or sets the separator between inner exceptions.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Environment.NewLine" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string InnerExceptionSeparator { get; set; } = Environment.NewLine;

	/// <summary>
	/// Gets or sets whether to render innermost Exception from <see cref="M:System.Exception.GetBaseException" />
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool BaseException { get; set; }

	/// <summary>
	/// Gets or sets whether to collapse exception tree using <see cref="M:System.AggregateException.Flatten" />
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public bool FlattenException { get; set; } = true;

	/// <summary>
	/// Gets the formats of the output of inner exceptions to be rendered in target. <see cref="T:NLog.Config.ExceptionRenderingFormat" />
	/// </summary>
	/// <docgen category="Layout Options" order="50" />
	public IEnumerable<ExceptionRenderingFormat> Formats => _formats;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.LayoutRenderers.ExceptionLayoutRenderer" /> class.
	/// </summary>
	public ExceptionLayoutRenderer()
	{
		_format = "TOSTRING,DATA";
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		value = GetTopException(logEvent);
		return true;
	}

	private Exception? GetTopException(LogEventInfo logEvent)
	{
		if (!BaseException)
		{
			return logEvent.Exception;
		}
		return logEvent.Exception?.GetBaseException();
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		_formats = CompileFormat(Format, "Format");
		_innerFormats = ((InnerFormat == null) ? null : CompileFormat(InnerFormat, "InnerFormat"));
		if (_separatorOriginal != null)
		{
			_separator = SimpleLayout.Evaluate(_separatorOriginal, base.LoggingConfiguration);
		}
		if (_exceptionDataSeparatorOriginal != null)
		{
			_exceptionDataSeparator = SimpleLayout.Evaluate(_exceptionDataSeparatorOriginal, base.LoggingConfiguration);
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		Exception topException = GetTopException(logEvent);
		if (topException == null)
		{
			return;
		}
		int num = 0;
		if (logEvent.Exception is AggregateException ex)
		{
			topException = (FlattenException ? GetPrimaryException(ex) : ex);
			AppendException(topException, _formats, builder, ex);
			if (num >= MaxInnerExceptionLevel)
			{
				return;
			}
			num = AppendInnerExceptionTree(topException, num, builder);
			if (num < MaxInnerExceptionLevel)
			{
				ReadOnlyCollection<Exception> innerExceptions = ex.InnerExceptions;
				if (innerExceptions != null && innerExceptions.Count > 1)
				{
					AppendAggregateException(ex, num, builder);
				}
			}
		}
		else
		{
			AppendException(topException, _formats, builder);
			if (num < MaxInnerExceptionLevel)
			{
				AppendInnerExceptionTree(topException, num, builder);
			}
		}
	}

	private static Exception GetPrimaryException(AggregateException aggregateException)
	{
		if (aggregateException.InnerExceptions.Count == 1)
		{
			Exception ex = aggregateException.InnerExceptions[0];
			if (!(ex is AggregateException))
			{
				return ex;
			}
		}
		aggregateException = aggregateException.Flatten();
		if (aggregateException.InnerExceptions.Count == 1)
		{
			return aggregateException.InnerExceptions[0];
		}
		return aggregateException;
	}

	private void AppendAggregateException(AggregateException primaryException, int currentLevel, StringBuilder builder)
	{
		AggregateException ex = primaryException.Flatten();
		if (ex.InnerExceptions == null)
		{
			return;
		}
		int num = 0;
		while (num < ex.InnerExceptions.Count && currentLevel < MaxInnerExceptionLevel)
		{
			Exception ex2 = ex.InnerExceptions[num];
			if (ex2 != primaryException.InnerException)
			{
				if (ex2 == null)
				{
					InternalLogger.Debug("Skipping rendering exception as exception is null");
				}
				else
				{
					AppendInnerException(ex2, builder);
					currentLevel++;
					currentLevel = AppendInnerExceptionTree(ex2, currentLevel, builder);
				}
			}
			num++;
			currentLevel++;
		}
	}

	private int AppendInnerExceptionTree(Exception currentException, int currentLevel, StringBuilder sb)
	{
		currentException = currentException.InnerException;
		while (currentException != null && currentLevel < MaxInnerExceptionLevel)
		{
			AppendInnerException(currentException, sb);
			currentLevel++;
			currentException = currentException.InnerException;
		}
		return currentLevel;
	}

	private void AppendInnerException(Exception currentException, StringBuilder builder)
	{
		builder.Append(InnerExceptionSeparator);
		AppendException(currentException, _innerFormats ?? _formats, builder);
	}

	private void AppendException(Exception currentException, List<ExceptionRenderingFormat> renderFormats, StringBuilder builder, Exception? aggregateException = null)
	{
		int length = builder.Length;
		foreach (ExceptionRenderingFormat renderFormat in renderFormats)
		{
			int length2 = builder.Length;
			_renderingfunctions[renderFormat](this, builder, currentException, aggregateException);
			if (builder.Length != length2)
			{
				length = builder.Length;
				builder.Append(_separator);
			}
		}
		builder.Length = length;
	}

	/// <summary>
	/// Appends the Message of an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The exception containing the Message to append.</param>
	protected virtual void AppendMessage(StringBuilder sb, Exception ex)
	{
		try
		{
			sb.Append(ex.Message);
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Exception-LayoutRenderer Could not output Message for Exception: {0}", ex.GetType());
			sb.Append(ex.GetType().ToString());
			sb.Append(" Message-property threw ");
			sb.Append(ex2.GetType().ToString());
		}
	}

	/// <summary>
	/// Appends the method name from Exception's stack trace to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose method name should be appended.</param>
	[UnconditionalSuppressMessage("Trimming - Allow callsite logic", "IL2026")]
	protected virtual void AppendMethod(StringBuilder sb, Exception ex)
	{
		try
		{
			sb.Append(ex.TargetSite?.ToString());
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Exception-LayoutRenderer Could not output TargetSite for Exception: {0}", ex.GetType());
			sb.Append(ex.GetType().ToString());
			sb.Append(" TargetSite-property threw ");
			sb.Append(ex2.GetType().ToString());
		}
	}

	/// <summary>
	/// Appends the stack trace from an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose stack trace should be appended.</param>
	protected virtual void AppendStackTrace(StringBuilder sb, Exception ex)
	{
		try
		{
			sb.Append(ex.StackTrace);
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Exception-LayoutRenderer Could not output StackTrace for Exception: {0}", ex.GetType());
			sb.Append(ex.GetType().ToString());
			sb.Append(" StackTrace-property threw ");
			sb.Append(ex2.GetType().ToString());
		}
	}

	/// <summary>
	/// Appends the result of calling ToString() on an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose call to ToString() should be appended.</param>
	protected virtual void AppendToString(StringBuilder sb, Exception ex)
	{
		string arg = string.Empty;
		Exception ex2 = null;
		try
		{
			ex2 = ex.InnerException;
			arg = ex.Message;
			sb.Append(ex.ToString());
		}
		catch (Exception ex3)
		{
			InternalLogger.Warn(ex3, "Exception-LayoutRenderer Could not output ToString for Exception: {0}", ex.GetType());
			sb.Append($"{ex.GetType()}: {arg}");
			if (ex2 != null)
			{
				sb.AppendLine();
				AppendToString(sb, ex2);
			}
		}
	}

	/// <summary>
	/// Appends the type of an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose type should be appended.</param>
	protected virtual void AppendType(StringBuilder sb, Exception ex)
	{
		sb.Append(ex.GetType().ToString());
	}

	/// <summary>
	/// Appends the short type of an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose short type should be appended.</param>
	protected virtual void AppendShortType(StringBuilder sb, Exception ex)
	{
		sb.Append(ex.GetType().Name);
	}

	/// <summary>
	/// Appends the application source of an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose source should be appended.</param>
	protected virtual void AppendSource(StringBuilder sb, Exception ex)
	{
		try
		{
			sb.Append(ex.Source);
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Exception-LayoutRenderer Could not output Source for Exception: {0}", ex.GetType());
			sb.Append(ex.GetType().ToString());
			sb.Append(" Source-property threw ");
			sb.Append(ex2.GetType().ToString());
		}
	}

	/// <summary>
	/// Appends the HResult of an Exception to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose HResult should be appended.</param>
	protected virtual void AppendHResult(StringBuilder sb, Exception ex)
	{
		if (ex.HResult != 0 && ex.HResult != 1)
		{
			sb.AppendFormat("0x{0:X8}", ex.HResult);
		}
	}

	private void AppendData(StringBuilder builder, Exception ex, Exception? aggregateException)
	{
		if (aggregateException != null && aggregateException.Data?.Count > 0 && ex != aggregateException)
		{
			AppendData(builder, aggregateException);
			builder.Append(_separator);
		}
		AppendData(builder, ex);
	}

	/// <summary>
	/// Appends the contents of an Exception's Data property to the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose Data property elements should be appended.</param>
	protected virtual void AppendData(StringBuilder sb, Exception ex)
	{
		IDictionary data = ex.Data;
		if (data == null || data.Count <= 0)
		{
			return;
		}
		string value = string.Empty;
		foreach (object key in ex.Data.Keys)
		{
			sb.Append(value);
			try
			{
				sb.AppendFormat("{0}: ", key);
				value = _exceptionDataSeparator;
				sb.AppendFormat("{0}", ex.Data[key]);
			}
			catch (Exception ex2)
			{
				InternalLogger.Warn(ex2, "Exception-LayoutRenderer Could not output Data-collection for Exception: {0}", ex.GetType());
			}
		}
	}

	/// <summary>
	/// Appends all the serialized properties of an Exception into the specified <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose properties should be appended.</param>
	protected virtual void AppendSerializeObject(StringBuilder sb, Exception ex)
	{
		base.ValueFormatter.FormatValue(ex, null, CaptureType.Serialize, null, sb);
	}

	/// <summary>
	/// Appends all the additional properties of an Exception like Data key-value-pairs
	/// </summary>
	/// <param name="sb">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="ex">The Exception whose properties should be appended.</param>
	protected virtual void AppendProperties(StringBuilder sb, Exception ex)
	{
		string value = string.Empty;
		foreach (ObjectReflectionCache.ObjectPropertyList.PropertyValue item in ObjectReflectionCache.LookupObjectProperties(ex))
		{
			if (ExcludeDefaultProperties.Contains(item.Name))
			{
				continue;
			}
			try
			{
				string text = item.Value?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					sb.Append(value);
					sb.Append(item.Name);
					value = _exceptionDataSeparator;
					sb.Append(": ");
					sb.AppendFormat("{0}", text);
				}
			}
			catch (Exception ex2)
			{
				InternalLogger.Warn(ex2, "Exception-LayoutRenderer Could not output Property-collection for Exception: {0}", ex.GetType());
			}
		}
	}

	/// <summary>
	/// Split the string and then compile into list of Rendering formats.
	/// </summary>
	private List<ExceptionRenderingFormat> CompileFormat(string formatSpecifier, string propertyName)
	{
		List<ExceptionRenderingFormat> list = new List<ExceptionRenderingFormat>();
		string[] array = formatSpecifier.SplitAndTrimTokens(',');
		foreach (string text in array)
		{
			if (_formatsMapping.TryGetValue(text, out var value))
			{
				list.Add(value);
				continue;
			}
			NLogConfigurationException ex = new NLogConfigurationException("Exception-LayoutRenderer assigned unknown " + propertyName + ": " + text);
			if (ex.MustBeRethrown() || (base.LoggingConfiguration?.LogFactory?.ThrowConfigExceptions ?? base.LoggingConfiguration?.LogFactory?.ThrowExceptions) == true)
			{
				throw ex;
			}
		}
		return list;
	}
}

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// Render environmental information related to logging events.
/// </summary>
[NLogConfigurationItem]
public abstract class LayoutRenderer : ISupportsInitialize, IRenderable
{
	private bool _isInitialized;

	private IValueFormatter? _valueFormatter;

	/// <summary>
	/// Gets the logging configuration this target is part of.
	/// </summary>
	protected LoggingConfiguration? LoggingConfiguration { get; private set; }

	/// <summary>
	/// Value formatter
	/// </summary>
	protected IValueFormatter ValueFormatter => _valueFormatter ?? (_valueFormatter = ResolveService<IValueFormatter>());

	/// <inheritdoc />
	public override string ToString()
	{
		LayoutRendererAttribute firstCustomAttribute = GetType().GetFirstCustomAttribute<LayoutRendererAttribute>();
		if (firstCustomAttribute != null)
		{
			return "Layout Renderer: ${" + firstCustomAttribute.Name + "}";
		}
		return GetType().Name;
	}

	/// <summary>
	/// Renders the value of layout renderer in the context of the specified log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <returns>String representation of a layout renderer.</returns>
	public string Render(LogEventInfo logEvent)
	{
		using AppendBuilderCreator appendBuilderCreator = new AppendBuilderCreator(mustBeEmpty: true);
		RenderAppendBuilder(logEvent, appendBuilderCreator.Builder);
		return appendBuilderCreator.Builder.ToString();
	}

	/// <inheritdoc />
	void ISupportsInitialize.Initialize(LoggingConfiguration? configuration)
	{
		Initialize(configuration);
	}

	/// <inheritdoc />
	void ISupportsInitialize.Close()
	{
		Close();
	}

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="configuration">The configuration.</param>
	internal void Initialize(LoggingConfiguration? configuration)
	{
		if (LoggingConfiguration == null)
		{
			LoggingConfiguration = configuration;
		}
		if (!_isInitialized)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		try
		{
			InitializeLayoutRenderer();
		}
		catch (Exception ex)
		{
			InternalLogger.Error(ex, "Exception in layout renderer initialization.");
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
		finally
		{
			_isInitialized = true;
		}
	}

	/// <summary>
	/// Closes this instance.
	/// </summary>
	internal void Close()
	{
		if (_isInitialized)
		{
			LoggingConfiguration = null;
			_valueFormatter = null;
			_isInitialized = false;
			CloseLayoutRenderer();
		}
	}

	/// <summary>
	/// Renders the value of layout renderer in the context of the specified log event.
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <param name="builder">The layout render output is appended to builder</param>
	internal void RenderAppendBuilder(LogEventInfo logEvent, StringBuilder builder)
	{
		try
		{
			if (!_isInitialized)
			{
				Initialize();
			}
			Append(builder, logEvent);
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Exception in '{0}.Append()'", GetType());
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
	}

	/// <summary>
	/// Renders the value of layout renderer in the context of the specified log event into <see cref="T:System.Text.StringBuilder" />.
	/// </summary>
	/// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
	/// <param name="logEvent">Logging event.</param>
	protected abstract void Append(StringBuilder builder, LogEventInfo logEvent);

	internal void AppendFormattedValue(StringBuilder builder, LogEventInfo logEvent, object? value, string? format, CultureInfo? culture)
	{
		if (format == null && value is string value2)
		{
			builder.Append(value2);
			return;
		}
		IFormatProvider formatProvider = GetFormatProvider(logEvent, culture);
		builder.AppendFormattedValue(value, format, formatProvider, ValueFormatter);
	}

	/// <summary>
	/// Initializes the layout renderer.
	/// </summary>
	protected virtual void InitializeLayoutRenderer()
	{
	}

	/// <summary>
	/// Closes the layout renderer.
	/// </summary>
	protected virtual void CloseLayoutRenderer()
	{
	}

	/// <summary>
	/// Get the <see cref="T:System.IFormatProvider" /> for rendering the messages to a <see cref="T:System.String" />
	/// </summary>
	/// <param name="logEvent">LogEvent with culture</param>
	/// <param name="layoutCulture">Culture in on Layout level</param>
	/// <returns></returns>
	protected IFormatProvider? GetFormatProvider(LogEventInfo logEvent, IFormatProvider? layoutCulture = null)
	{
		object obj = layoutCulture;
		if (obj == null)
		{
			obj = logEvent.FormatProvider;
			if (obj == null)
			{
				LoggingConfiguration? loggingConfiguration = LoggingConfiguration;
				if (loggingConfiguration == null)
				{
					return null;
				}
				obj = loggingConfiguration.DefaultCultureInfo;
			}
		}
		return (IFormatProvider?)obj;
	}

	/// <summary>
	/// Get the <see cref="T:System.Globalization.CultureInfo" /> for rendering the messages to a <see cref="T:System.String" />
	/// </summary>
	/// <param name="logEvent">LogEvent with culture</param>
	/// <param name="layoutCulture">Culture in on Layout level</param>
	/// <returns></returns>
	/// <remarks>
	/// <see cref="M:NLog.LayoutRenderers.LayoutRenderer.GetFormatProvider(NLog.LogEventInfo,System.IFormatProvider)" /> is preferred
	/// </remarks>
	protected CultureInfo GetCulture(LogEventInfo logEvent, CultureInfo? layoutCulture = null)
	{
		return layoutCulture ?? (logEvent.FormatProvider as CultureInfo) ?? LoggingConfiguration?.DefaultCultureInfo ?? CultureInfo.CurrentCulture;
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom layout renderer.
	/// </summary>
	/// <remarks>Short-cut for registering to default <see cref="T:NLog.Config.ConfigurationItemFactory" /></remarks>
	/// <typeparam name="T">Type of the layout renderer.</typeparam>
	/// <param name="name">The layout-renderer type-alias for use in NLog configuration - without '${ }'</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string name) where T : LayoutRenderer
	{
		Type typeFromHandle = typeof(T);
		Register(name, typeFromHandle);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom layout renderer.
	/// </summary>
	/// <remarks>Short-cut for registering to default <see cref="T:NLog.Config.ConfigurationItemFactory" /></remarks>
	/// <param name="layoutRendererType"> Type of the layout renderer.</param>
	/// <param name="name">The layout-renderer type-alias for use in NLog configuration - without '${ }'</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] Type layoutRendererType)
	{
		Guard.ThrowIfNull(layoutRendererType, "layoutRendererType");
		Guard.ThrowIfNullOrEmpty(name, "name");
		ConfigurationItemFactory.Default.GetLayoutRendererFactory().RegisterDefinition(name, layoutRendererType);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom layout renderer with a callback function <paramref name="func" />. The callback receives the logEvent.
	/// </summary>
	/// <param name="name">The layout-renderer type-alias for use in NLog configuration - without '${ }'</param>
	/// <param name="func">Callback that returns the value for the layout renderer.</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register(string name, Func<LogEventInfo, object> func)
	{
		Guard.ThrowIfNull(func, "func");
		Register(name, (LogEventInfo info, LoggingConfiguration? configuration) => func(info));
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom layout renderer with a callback function <paramref name="func" />. The callback receives the logEvent and the current configuration.
	/// </summary>
	/// <param name="name">The layout-renderer type-alias for use in NLog configuration - without '${ }'</param>
	/// <param name="func">Callback that returns the value for the layout renderer.</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register(string name, Func<LogEventInfo, LoggingConfiguration?, object> func)
	{
		Guard.ThrowIfNull(func, "func");
		Register(new FuncLayoutRenderer(name, func));
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom layout renderer with a callback function <paramref name="layoutRenderer" />. The callback receives the logEvent and the current configuration.
	/// </summary>
	/// <param name="layoutRenderer">Renderer with callback func</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register(FuncLayoutRenderer layoutRenderer)
	{
		Guard.ThrowIfNull(layoutRenderer, "layoutRenderer");
		ConfigurationItemFactory.Default.GetLayoutRendererFactory().RegisterFuncLayout(layoutRenderer.LayoutRendererName, layoutRenderer);
	}

	/// <summary>
	/// Resolves the interface service-type from the service-repository
	/// </summary>
	protected T ResolveService<T>() where T : class
	{
		return LoggingConfiguration.GetServiceProvider().ResolveService<T>(_isInitialized);
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.LayoutRenderers;

namespace NLog.Layouts;

/// <summary>
/// Abstract interface that layouts must implement.
/// </summary>
[NLogConfigurationItem]
public abstract class Layout : ISupportsInitialize, IRenderable
{
	/// <summary>
	/// Default Layout-value that renders string.Empty
	/// </summary>
	public static readonly Layout Empty = new SimpleLayout();

	/// <summary>
	/// Is this layout initialized? See <see cref="M:NLog.Layouts.Layout.Initialize(NLog.Config.LoggingConfiguration)" />
	/// </summary>
	internal bool IsInitialized;

	private bool _scannedForObjects;

	/// <summary>
	/// Gets a value indicating whether this layout is thread-agnostic (can be rendered on any thread).
	/// </summary>
	/// <remarks>
	/// Layout is thread-agnostic if it has been marked with [ThreadAgnostic] attribute and all its children are
	/// like that as well.
	///
	/// Thread-agnostic layouts only use contents of <see cref="T:NLog.LogEventInfo" /> for its output.
	/// </remarks>
	internal bool ThreadAgnostic { get; set; }

	internal bool ThreadAgnosticImmutable { get; set; }

	/// <summary>
	/// Gets the level of stack trace information required for rendering.
	/// </summary>
	internal StackTraceUsage StackTraceUsage { get; set; }

	/// <summary>
	/// Gets the logging configuration this target is part of.
	/// </summary>
	protected internal LoggingConfiguration? LoggingConfiguration { get; private set; }

	/// <summary>
	/// Implicitly converts the specified string as LayoutRenderer-expression into a <see cref="T:NLog.Layouts.Layout" />.
	/// </summary>
	/// <param name="text">Text to be converted.</param>
	/// <returns><see cref="T:NLog.Layouts.SimpleLayout" /> object represented by the text.</returns>
	public static implicit operator Layout([Localizable(false)] string text)
	{
		if (text == null)
		{
			return new Layout<string>(null);
		}
		return FromString(text, ConfigurationItemFactory.Default);
	}

	/// <summary>
	/// Parses the specified string as LayoutRenderer-expression into a <see cref="T:NLog.Layouts.SimpleLayout" />.
	/// </summary>
	/// <param name="layoutText">The layout string.</param>
	/// <returns>Instance of <see cref="T:NLog.Layouts.SimpleLayout" />.</returns>'
	public static Layout FromString([Localizable(false)] string layoutText)
	{
		return FromString(layoutText, ConfigurationItemFactory.Default);
	}

	/// <summary>
	/// Parses the specified string as LayoutRenderer-expression into a <see cref="T:NLog.Layouts.SimpleLayout" />.
	/// </summary>
	/// <param name="layoutText">The layout string.</param>
	/// <param name="configurationItemFactory">The NLog factories to use when resolving layout renderers.</param>
	/// <returns>Instance of <see cref="T:NLog.Layouts.SimpleLayout" />.</returns>
	public static Layout FromString([Localizable(false)] string layoutText, ConfigurationItemFactory configurationItemFactory)
	{
		if (!string.IsNullOrEmpty(layoutText))
		{
			return new SimpleLayout(layoutText, configurationItemFactory);
		}
		return Empty;
	}

	/// <summary>
	/// Parses the specified string as LayoutRenderer-expression into a <see cref="T:NLog.Layouts.SimpleLayout" />.
	/// </summary>
	/// <param name="layoutText">The layout string.</param>
	/// <param name="throwConfigExceptions">Whether <see cref="T:NLog.NLogConfigurationException" /> should be thrown on parse errors (<see langword="false" /> = replace unrecognized tokens with a space).</param>
	/// <returns>Instance of <see cref="T:NLog.Layouts.SimpleLayout" />.</returns>
	public static Layout FromString([Localizable(false)] string layoutText, bool throwConfigExceptions)
	{
		try
		{
			return string.IsNullOrEmpty(layoutText) ? Empty : new SimpleLayout(layoutText, ConfigurationItemFactory.Default, throwConfigExceptions);
		}
		catch (NLogConfigurationException)
		{
			throw;
		}
		catch (Exception ex2)
		{
			if (!throwConfigExceptions || ex2.MustBeRethrownImmediately())
			{
				throw;
			}
			throw new NLogConfigurationException("Invalid Layout: " + layoutText, ex2);
		}
	}

	/// <summary>
	/// Create a <see cref="T:NLog.Layouts.SimpleLayout" /> containing literal value
	/// </summary>
	public static Layout FromLiteral([Localizable(false)] string literalText)
	{
		if (string.IsNullOrEmpty(literalText))
		{
			return Empty;
		}
		LayoutRenderer[] layoutRenderers = new LiteralLayoutRenderer[1]
		{
			new LiteralLayoutRenderer(literalText)
		};
		return new SimpleLayout(layoutRenderers, literalText);
	}

	/// <summary>
	/// Create a <see cref="T:NLog.Layouts.SimpleLayout" /> from a lambda method.
	/// </summary>
	/// <param name="layoutMethod">Method that renders the layout.</param>
	/// <param name="options">Whether method is ThreadAgnostic and doesn't depend on context of the logging application thread.</param>
	/// <returns>Instance of <see cref="T:NLog.Layouts.SimpleLayout" />.</returns>
	public static Layout FromMethod(Func<LogEventInfo, object> layoutMethod, LayoutRenderOptions options = LayoutRenderOptions.None)
	{
		Guard.ThrowIfNull(layoutMethod, "layoutMethod");
		string name = layoutMethod.Method?.DeclaringType?.ToString() + "." + layoutMethod.Method?.Name;
		FuncLayoutRenderer funcLayoutRenderer = CreateFuncLayoutRenderer((LogEventInfo l, LoggingConfiguration? c) => layoutMethod(l), options, name);
		LayoutRenderer[] layoutRenderers = new FuncLayoutRenderer[1] { funcLayoutRenderer };
		return new SimpleLayout(layoutRenderers, funcLayoutRenderer.LayoutRendererName);
	}

	internal static FuncLayoutRenderer CreateFuncLayoutRenderer(Func<LogEventInfo, LoggingConfiguration?, object> layoutMethod, LayoutRenderOptions options, string name)
	{
		if ((options & LayoutRenderOptions.ThreadAgnostic) == LayoutRenderOptions.ThreadAgnostic)
		{
			return new FuncThreadAgnosticLayoutRenderer(name, layoutMethod);
		}
		return new FuncLayoutRenderer(name, layoutMethod);
	}

	/// <summary>
	/// Precalculates the layout for the specified log event and stores the result
	/// in per-log event cache.
	///
	/// Skips context capture when Layout have [ThreadAgnostic], and only contains layouts with [ThreadAgnostic].
	/// </summary>
	/// <param name="logEvent">The log event.</param>
	/// <remarks>
	/// Override this method to make it conditional whether to capture Layout output-value for <paramref name="logEvent" />
	/// </remarks>
	public virtual void Precalculate(LogEventInfo logEvent)
	{
		if (!ThreadAgnostic || ThreadAgnosticImmutable)
		{
			using (AppendBuilderCreator appendBuilderCreator = new AppendBuilderCreator(mustBeEmpty: true))
			{
				PrecalculateCachedLayoutValue(logEvent, appendBuilderCreator.Builder);
			}
		}
	}

	/// <summary>
	/// Renders formatted output using the log event as context.
	/// </summary>
	/// <remarks>Inside a <see cref="T:NLog.Targets.Target" />, <see cref="M:NLog.Targets.Target.RenderLogEvent(NLog.Layouts.Layout,NLog.LogEventInfo)" /> is preferred for performance reasons.</remarks>
	/// <param name="logEvent">The logging event.</param>
	/// <returns>The formatted output as string.</returns>
	public string Render(LogEventInfo logEvent)
	{
		if (!IsInitialized)
		{
			Initialize(LoggingConfiguration);
		}
		if ((!ThreadAgnostic || ThreadAgnosticImmutable) && logEvent.TryGetCachedLayoutValue(this, out object value))
		{
			return value?.ToString() ?? string.Empty;
		}
		return GetFormattedMessage(logEvent) ?? string.Empty;
	}

	/// <summary>
	/// Optimized version of <see cref="M:NLog.Layouts.Layout.Render(NLog.LogEventInfo)" /> that works best when
	/// override of <see cref="M:NLog.Layouts.Layout.RenderFormattedMessage(NLog.LogEventInfo,System.Text.StringBuilder)" /> is available.
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	/// <param name="target">Appends the formatted output to target</param>
	public void Render(LogEventInfo logEvent, StringBuilder target)
	{
		if (!IsInitialized)
		{
			Initialize(LoggingConfiguration);
		}
		if ((!ThreadAgnostic || ThreadAgnosticImmutable) && logEvent.TryGetCachedLayoutValue(this, out object value))
		{
			target.Append(value?.ToString());
		}
		else
		{
			RenderFormattedMessage(logEvent, target);
		}
	}

	internal virtual void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		Precalculate(logEvent);
	}

	private void PrecalculateCachedLayoutValue(LogEventInfo logEvent, StringBuilder target)
	{
		if (!IsInitialized)
		{
			Initialize(LoggingConfiguration);
		}
		if ((!ThreadAgnostic || ThreadAgnosticImmutable) && !logEvent.TryGetCachedLayoutValue(this, out object _))
		{
			RenderFormattedMessage(logEvent, target);
			logEvent.AddCachedLayoutValue(this, target.ToString());
		}
	}

	/// <summary>
	/// Valid default implementation of <see cref="M:NLog.Layouts.Layout.GetFormattedMessage(NLog.LogEventInfo)" />, when having implemented the optimized <see cref="M:NLog.Layouts.Layout.RenderFormattedMessage(NLog.LogEventInfo,System.Text.StringBuilder)" />
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	/// <returns>The rendered layout.</returns>
	internal string RenderAllocateBuilder(LogEventInfo logEvent)
	{
		using AppendBuilderCreator appendBuilderCreator = new AppendBuilderCreator(mustBeEmpty: true);
		RenderFormattedMessage(logEvent, appendBuilderCreator.Builder);
		return appendBuilderCreator.Builder.ToString();
	}

	internal string RenderAllocateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		RenderFormattedMessage(logEvent, target);
		return target.ToString();
	}

	/// <summary>
	/// Renders formatted output using the log event as context.
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	/// <param name="target">Appends the formatted output to target</param>
	protected virtual void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		target.Append(GetFormattedMessage(logEvent));
	}

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="configuration">The configuration.</param>
	void ISupportsInitialize.Initialize(LoggingConfiguration? configuration)
	{
		Initialize(configuration);
	}

	/// <summary>
	/// Closes this instance.
	/// </summary>
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
		if (IsInitialized)
		{
			return;
		}
		try
		{
			LoggingConfiguration = configuration;
			_scannedForObjects = false;
			InitializeLayout();
			if (!_scannedForObjects)
			{
				InternalLogger.Debug("{0} Initialized Layout done but not scanned for objects", GetType());
				PerformObjectScanning();
			}
		}
		finally
		{
			IsInitialized = true;
		}
	}

	internal void PerformObjectScanning()
	{
		List<IRenderable> list = ObjectGraphScanner.FindReachableObjects<IRenderable>(ConfigurationItemFactory.Default, aggressiveSearch: true, new object[1] { this });
		HashSet<Type> hashSet = new HashSet<Type>(list.Select((IRenderable o) => o.GetType()));
		hashSet.Remove(typeof(SimpleLayout));
		hashSet.Remove(typeof(LiteralLayoutRenderer));
		hashSet.Remove(typeof(LiteralWithRawValueLayoutRenderer));
		ThreadAgnostic = hashSet.All((Type t) => t.IsDefined(typeof(ThreadAgnosticAttribute), inherit: true));
		ThreadAgnosticImmutable = ThreadAgnostic && hashSet.Any((Type t) => t.IsDefined(typeof(ThreadAgnosticImmutableAttribute), inherit: true));
		if (list.Count > 1 && hashSet.Count > 0)
		{
			foreach (Layout item in list.OfType<Layout>())
			{
				if (item != this)
				{
					item.Initialize(LoggingConfiguration);
					ThreadAgnostic = item.ThreadAgnostic && ThreadAgnostic;
					ThreadAgnosticImmutable = ThreadAgnostic && (item.ThreadAgnosticImmutable || ThreadAgnosticImmutable);
				}
			}
		}
		StackTraceUsage = StackTraceUsage.None;
		StackTraceUsage = list.OfType<IUsesStackTrace>().DefaultIfEmpty().Aggregate(StackTraceUsage.None, (StackTraceUsage usage, IUsesStackTrace item) => (usage | item?.StackTraceUsage).GetValueOrDefault());
		_scannedForObjects = true;
	}

	internal Layout[]? ResolveLayoutPrecalculation(IEnumerable<Layout> allLayouts)
	{
		if (!_scannedForObjects || (ThreadAgnostic && !ThreadAgnosticImmutable))
		{
			return null;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (Layout allLayout in allLayouts)
		{
			num++;
			if ((allLayout != null && !allLayout.ThreadAgnostic) || (allLayout != null && allLayout.ThreadAgnosticImmutable))
			{
				num2++;
				if (allLayout is SimpleLayout)
				{
					num3++;
				}
			}
		}
		if (num <= 1 || num2 > 4 || num2 - num3 > 2 || num - num3 <= 1 || num2 == 0)
		{
			return null;
		}
		return allLayouts.Where((Layout layout) => (layout != null && !layout.ThreadAgnostic) || (layout?.ThreadAgnosticImmutable ?? false)).ToArray();
	}

	/// <summary>
	/// Closes this instance.
	/// </summary>
	internal void Close()
	{
		if (IsInitialized)
		{
			LoggingConfiguration = null;
			IsInitialized = false;
			CloseLayout();
		}
	}

	/// <summary>
	/// Initializes the layout.
	/// </summary>
	protected virtual void InitializeLayout()
	{
		PerformObjectScanning();
	}

	/// <summary>
	/// Closes the layout.
	/// </summary>
	protected virtual void CloseLayout()
	{
	}

	/// <summary>
	/// Renders formatted output using the log event as context.
	/// </summary>
	/// <param name="logEvent">The logging event.</param>
	/// <returns>The formatted output.</returns>
	protected abstract string GetFormattedMessage(LogEventInfo logEvent);

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom Layout.
	/// </summary>
	/// <remarks>Short-cut for registering to default <see cref="T:NLog.Config.ConfigurationItemFactory" /></remarks>
	/// <typeparam name="T"> Type of the Layout.</typeparam>
	/// <param name="name"> Name of the Layout.</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string name) where T : Layout
	{
		Type typeFromHandle = typeof(T);
		Register(name, typeFromHandle);
	}

	/// <summary>
	/// Obsolete and replaced by <see cref="M:NLog.LogManager.Setup" /> with NLog v5.2.
	///
	/// Register a custom Layout.
	/// </summary>
	/// <remarks>Short-cut for registering to default <see cref="T:NLog.Config.ConfigurationItemFactory" /></remarks>
	/// <param name="layoutType"> Type of the Layout.</param>
	/// <param name="name"> Name of the Layout.</param>
	[Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static void Register(string name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties)] Type layoutType)
	{
		ConfigurationItemFactory.Default.GetLayoutFactory().RegisterDefinition(name, layoutType);
	}

	/// <summary>
	/// Optimized version of <see cref="M:NLog.Layouts.Layout.Precalculate(NLog.LogEventInfo)" /> for internal Layouts, when
	/// override of <see cref="M:NLog.Layouts.Layout.RenderFormattedMessage(NLog.LogEventInfo,System.Text.StringBuilder)" /> is available.
	/// </summary>
	internal void PrecalculateBuilderInternal(LogEventInfo logEvent, StringBuilder target, Layout[]? precalculateLayout)
	{
		if (ThreadAgnostic && !ThreadAgnosticImmutable)
		{
			return;
		}
		if (precalculateLayout == null)
		{
			PrecalculateCachedLayoutValue(logEvent, target);
			return;
		}
		foreach (Layout obj in precalculateLayout)
		{
			target.ClearBuilder();
			obj.PrecalculateBuilder(logEvent, target);
		}
	}

	internal string ToStringWithNestedItems<T>(IList<T> nestedItems, Func<T, string> nextItemToString)
	{
		if (nestedItems != null && nestedItems.Count > 0)
		{
			string[] value = nestedItems.Select(nextItemToString).ToArray();
			return GetType().Name + ": " + string.Join("|", value);
		}
		return GetType().Name;
	}

	/// <summary>
	/// Try get value
	/// </summary>
	/// <param name="logEvent"></param>
	/// <param name="rawValue">rawValue if return result is <see langword="true" /></param>
	/// <returns><see langword="false" /> if we could not determine the rawValue</returns>
	internal virtual bool TryGetRawValue(LogEventInfo logEvent, out object? rawValue)
	{
		rawValue = null;
		return false;
	}

	/// <summary>
	/// Resolve from DI <see cref="P:NLog.LogFactory.ServiceRepository" />
	/// </summary>
	/// <remarks>Avoid calling this while handling a LogEvent, since random deadlocks can occur</remarks>
	protected T ResolveService<T>() where T : class
	{
		return LoggingConfiguration.GetServiceProvider().ResolveService<T>(IsInitialized);
	}
}
/// <summary>
/// Typed Layout for easy conversion from NLog Layout logic to a simple value (ex. integer or enum)
/// </summary>
/// <typeparam name="T"></typeparam>
[ThreadAgnostic]
public sealed class Layout<T> : Layout, ILayoutTypeValue<T>, ILayoutTypeValue, IEquatable<T>
{
	private sealed class LayoutGenericTypeValue : LayoutTypeValue, ILayoutTypeValue<T>, ILayoutTypeValue
	{
		private readonly Layout<T> _ownerLayout;

		public override IPropertyTypeConverter ValueTypeConverter => _ownerLayout.ValueTypeConverter;

		public LayoutGenericTypeValue(Layout layout, string? parseValueFormat, CultureInfo? parseValueCulture, Layout<T> ownerLayout)
			: base(layout, typeof(T), parseValueFormat, parseValueCulture, null)
		{
			_ownerLayout = ownerLayout;
		}

		public void InitializeLayout()
		{
			InitializeLayout(_ownerLayout);
		}

		public void CloseLayout()
		{
			Close();
		}

		public bool TryRenderValue(LogEventInfo logEvent, StringBuilder? stringBuilder, out T? value)
		{
			if (RenderObjectValue(logEvent, stringBuilder) is T val)
			{
				value = val;
				return true;
			}
			value = default(T);
			return false;
		}
	}

	private sealed class FuncMethodValue : ILayoutTypeValue<T>, ILayoutTypeValue
	{
		private readonly Func<LogEventInfo, T> _layoutMethod;

		public bool ThreadAgnostic { get; }

		ILayoutTypeValue ILayoutTypeValue.InnerLayout => this;

		Type ILayoutTypeValue.InnerType => typeof(T);

		LoggingConfiguration? ILayoutTypeValue<T>.LoggingConfiguration => null;

		bool ILayoutTypeValue<T>.ThreadAgnosticImmutable => false;

		StackTraceUsage ILayoutTypeValue<T>.StackTraceUsage => StackTraceUsage.None;

		public FuncMethodValue(Func<LogEventInfo, T> layoutMethod, LayoutRenderOptions options)
		{
			_layoutMethod = layoutMethod;
			ThreadAgnostic = options == LayoutRenderOptions.ThreadAgnostic;
		}

		public void InitializeLayout()
		{
		}

		public void CloseLayout()
		{
		}

		public bool TryRenderValue(LogEventInfo logEvent, StringBuilder? stringBuilder, out T? value)
		{
			value = _layoutMethod(logEvent);
			return true;
		}

		public object? RenderObjectValue(LogEventInfo logEvent, StringBuilder? stringBuilder)
		{
			return _layoutMethod(logEvent);
		}

		public override string ToString()
		{
			return _layoutMethod.ToString();
		}
	}

	private readonly T? _fixedValue;

	private object? _fixedObjectValue;

	private readonly ILayoutTypeValue<T> _layoutValue;

	private IPropertyTypeConverter? _valueTypeConverter;

	ILayoutTypeValue ILayoutTypeValue.InnerLayout => _layoutValue;

	Type ILayoutTypeValue.InnerType => typeof(T);

	bool ILayoutTypeValue<T>.ThreadAgnostic => true;

	bool ILayoutTypeValue<T>.ThreadAgnosticImmutable => false;

	StackTraceUsage ILayoutTypeValue<T>.StackTraceUsage => StackTraceUsage.None;

	LoggingConfiguration? ILayoutTypeValue<T>.LoggingConfiguration => base.LoggingConfiguration;

	/// <summary>
	/// Is fixed value?
	/// </summary>
	public bool IsFixed => this == _layoutValue;

	/// <summary>
	/// Fixed value
	/// </summary>
	public T? FixedValue => _fixedValue;

	private object? FixedObjectValue => _fixedObjectValue ?? (_fixedObjectValue = _fixedValue);

	private IPropertyTypeConverter ValueTypeConverter => _valueTypeConverter ?? (_valueTypeConverter = ResolveService<IPropertyTypeConverter>());

	void ILayoutTypeValue<T>.InitializeLayout()
	{
	}

	void ILayoutTypeValue<T>.CloseLayout()
	{
	}

	bool ILayoutTypeValue<T>.TryRenderValue(LogEventInfo logEvent, StringBuilder? stringBuilder, out T? value)
	{
		value = _fixedValue;
		return true;
	}

	object? ILayoutTypeValue.RenderObjectValue(LogEventInfo logEvent, StringBuilder? stringBuilder)
	{
		return FixedObjectValue;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.Layout`1" /> class.
	/// </summary>
	/// <param name="layout">Dynamic NLog Layout</param>
	public Layout(Layout layout)
		: this(layout, (string?)null, CultureInfo.InvariantCulture)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.Layout`1" /> class.
	/// </summary>
	/// <param name="layout">Dynamic NLog Layout</param>
	/// <param name="parseValueFormat">Format used for parsing string-value into result value type</param>
	/// <param name="parseValueCulture">Culture used for parsing string-value into result value type</param>
	public Layout(Layout layout, string? parseValueFormat, CultureInfo? parseValueCulture)
	{
		if (PropertyTypeConverter.IsComplexType(typeof(T)))
		{
			throw new NLogConfigurationException("Layout<" + typeof(T).ToString() + "> not supported. Immutable value type is recommended");
		}
		if (TryParseFixedValue(layout, parseValueFormat, parseValueCulture, ref _fixedValue))
		{
			_layoutValue = this;
		}
		else
		{
			_layoutValue = new LayoutGenericTypeValue(layout, parseValueFormat, parseValueCulture, this);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.Layout`1" /> class.
	/// </summary>
	/// <param name="value">Fixed value</param>
	public Layout(T? value)
	{
		_fixedValue = value;
		_layoutValue = this;
	}

	private Layout(Func<LogEventInfo, T> layoutMethod, LayoutRenderOptions options)
	{
		Guard.ThrowIfNull(layoutMethod, "layoutMethod");
		_layoutValue = new FuncMethodValue(layoutMethod, options);
	}

	/// <summary>
	/// Render Value
	/// </summary>
	/// <param name="logEvent">Log event for rendering</param>
	/// <param name="defaultValue">Fallback value when no value available</param>
	/// <returns>Result value when available, else fallback to defaultValue</returns>
	internal T? RenderTypedValue(LogEventInfo logEvent, T? defaultValue = default(T?))
	{
		return RenderTypedValue(logEvent, null, defaultValue);
	}

	internal T? RenderTypedValue(LogEventInfo logEvent, StringBuilder? stringBuilder, T? defaultValue)
	{
		if (IsFixed)
		{
			return _fixedValue;
		}
		if (logEvent == null)
		{
			return defaultValue;
		}
		if (logEvent.TryGetCachedLayoutValue(this, out object value))
		{
			if (value != null)
			{
				return (T)value;
			}
			return defaultValue;
		}
		if (!IsInitialized)
		{
			Initialize(base.LoggingConfiguration ?? _layoutValue.LoggingConfiguration);
		}
		if (_layoutValue.TryRenderValue(logEvent, stringBuilder, out T value2))
		{
			return value2;
		}
		return defaultValue;
	}

	private object? RenderObjectValue(LogEventInfo logEvent, StringBuilder? stringBuilder)
	{
		if (logEvent == null)
		{
			return null;
		}
		if (logEvent.TryGetCachedLayoutValue(this, out object value))
		{
			return value;
		}
		return _layoutValue.RenderObjectValue(logEvent, stringBuilder);
	}

	/// <summary>
	/// Renders the value and converts the value into string format
	/// </summary>
	/// <remarks>
	/// Only to implement abstract method from <see cref="T:NLog.Layouts.Layout" />, and only used when calling <see cref="M:NLog.Layouts.Layout.Render(NLog.LogEventInfo)" />
	/// </remarks>
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		return FormatHelper.TryFormatToString(IsFixed ? FixedObjectValue : RenderObjectValue(logEvent, null), null, CultureInfo.InvariantCulture);
	}

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		base.InitializeLayout();
		_layoutValue.InitializeLayout();
		base.ThreadAgnostic = _layoutValue.ThreadAgnostic;
		base.ThreadAgnosticImmutable = _layoutValue.ThreadAgnosticImmutable;
		base.StackTraceUsage = _layoutValue.StackTraceUsage;
		_valueTypeConverter = null;
		_fixedObjectValue = null;
	}

	/// <inheritdoc />
	protected override void CloseLayout()
	{
		_layoutValue.CloseLayout();
		_valueTypeConverter = null;
		_fixedObjectValue = null;
		base.CloseLayout();
	}

	/// <inheritdoc />
	public override void Precalculate(LogEventInfo logEvent)
	{
		PrecalculateInnerLayout(logEvent, null);
	}

	/// <inheritdoc />
	internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder? target)
	{
		PrecalculateInnerLayout(logEvent, target);
	}

	/// <summary>
	/// Create a typed layout from a lambda method.
	/// </summary>
	/// <param name="layoutMethod">Method that renders the layout.</param>
	/// <param name="options">Whether method is ThreadAgnostic and doesn't depend on context of the logging application thread.</param>
	/// <returns>Instance of typed layout.</returns>
	public static Layout<T> FromMethod(Func<LogEventInfo, T> layoutMethod, LayoutRenderOptions options = LayoutRenderOptions.None)
	{
		return new Layout<T>(layoutMethod, options);
	}

	private void PrecalculateInnerLayout(LogEventInfo logEvent, StringBuilder? target)
	{
		if (!IsFixed && (!_layoutValue.ThreadAgnostic || _layoutValue.ThreadAgnosticImmutable))
		{
			object value = RenderObjectValue(logEvent, target);
			logEvent.AddCachedLayoutValue(this, value);
		}
	}

	private bool TryParseFixedValue(Layout layout, string? parseValueFormat, CultureInfo? parseValueCulture, ref T? fixedValue)
	{
		if (layout is SimpleLayout { IsFixedText: not false } simpleLayout)
		{
			if (simpleLayout.FixedText != null && !string.IsNullOrEmpty(simpleLayout.FixedText))
			{
				try
				{
					fixedValue = (T)ParseValueFromObject(simpleLayout.FixedText, parseValueFormat, parseValueCulture);
					return true;
				}
				catch (Exception innerException)
				{
					NLogConfigurationException ex = new NLogConfigurationException($"Failed converting into type {typeof(T)}. Value='{simpleLayout.FixedText}'", innerException);
					if (ex.MustBeRethrown())
					{
						throw ex;
					}
				}
			}
			else
			{
				if (typeof(T) == typeof(string))
				{
					fixedValue = (T)(object)(simpleLayout.FixedText ?? string.Empty);
					return true;
				}
				if (Nullable.GetUnderlyingType(typeof(T)) != null)
				{
					fixedValue = default(T);
					return true;
				}
			}
		}
		else if (layout == null)
		{
			fixedValue = default(T);
			return true;
		}
		fixedValue = default(T);
		return false;
	}

	private object? ParseValueFromObject(object rawValue, string? parseValueFormat, CultureInfo? parseValueCulture)
	{
		return ValueTypeConverter.Convert(rawValue, typeof(T), parseValueFormat, parseValueCulture);
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (!IsFixed)
		{
			return _layoutValue.ToString();
		}
		return FixedObjectValue?.ToString() ?? "null";
	}

	/// <inheritdoc />
	public override bool Equals(object obj)
	{
		if (IsFixed)
		{
			if (obj is Layout<T> layout)
			{
				if (layout.IsFixed)
				{
					return object.Equals(FixedObjectValue, layout.FixedObjectValue);
				}
				return false;
			}
			if (obj is T)
			{
				return object.Equals(FixedObjectValue, obj);
			}
			return obj == FixedObjectValue;
		}
		return this == obj;
	}

	/// <summary>
	/// Implements Equals using <see cref="P:NLog.Layouts.Layout`1.FixedValue" />
	/// </summary>
	public bool Equals(T other)
	{
		if (IsFixed)
		{
			return object.Equals(FixedObjectValue, other);
		}
		return false;
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		if (IsFixed)
		{
			return FixedObjectValue?.GetHashCode() ?? typeof(T).GetHashCode();
		}
		return RuntimeHelpers.GetHashCode(this);
	}

	/// <summary>
	/// Converts a given value to a <see cref="T:NLog.Layouts.Layout`1" />.
	/// </summary>
	/// <param name="value">Text to be converted.</param>
	public static implicit operator Layout<T>(T value)
	{
		return new Layout<T>(value);
	}

	/// <summary>
	/// Converts a given text to a <see cref="T:NLog.Layouts.Layout`1" />.
	/// </summary>
	/// <param name="layout">Text to be converted.</param>
	public static implicit operator Layout<T>([Localizable(false)] string layout)
	{
		if (layout != null || typeof(T).IsValueType)
		{
			return new Layout<T>(layout ?? string.Empty);
		}
		return new Layout<T>(default(T));
	}

	/// <summary>
	/// Implements the operator == using <see cref="P:NLog.Layouts.Layout`1.FixedValue" />
	/// </summary>
	public static bool operator ==(Layout<T> left, T right)
	{
		if (left == null || !left.Equals(right))
		{
			if (left == null)
			{
				return object.Equals(right, default(T));
			}
			return false;
		}
		return true;
	}

	/// <summary>
	/// Implements the operator != using <see cref="P:NLog.Layouts.Layout`1.FixedValue" />
	/// </summary>
	public static bool operator !=(Layout<T> left, T right)
	{
		if (left == null || !left.Equals(right))
		{
			if (left == null)
			{
				return !object.Equals(right, default(T));
			}
			return true;
		}
		return false;
	}
}

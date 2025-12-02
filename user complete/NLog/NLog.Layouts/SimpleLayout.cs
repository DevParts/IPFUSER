using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.LayoutRenderers;

namespace NLog.Layouts;

/// <summary>
/// Represents a string with embedded placeholders that can render contextual information.
/// </summary>
/// <remarks>
/// <para>
/// This layout is not meant to be used explicitly. Instead you can just use a string containing layout
/// renderers everywhere the layout is required.
/// </para>
/// <a href="https://github.com/NLog/NLog/wiki/SimpleLayout">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/SimpleLayout">Documentation on NLog Wiki</seealso>
[Layout("SimpleLayout")]
[ThreadAgnostic]
[AppDomainFixedOutput]
public sealed class SimpleLayout : Layout, IUsesStackTrace, IStringValueRenderer
{
	private readonly IRawValue? _rawValueRenderer;

	private IStringValueRenderer? _stringValueRenderer;

	private ReadOnlyCollection<LayoutRenderer>? _renderers;

	private readonly LayoutRenderer[] _layoutRenderers;

	internal static SimpleLayout Default => (SimpleLayout)Layout.Empty;

	/// <summary>
	/// Original text before parsing as Layout renderes.
	/// </summary>
	public string OriginalText { get; }

	/// <summary>
	/// Gets or sets the layout text that could be parsed.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	public string Text { get; }

	/// <summary>
	/// Is the message fixed? (no Layout renderers used)
	/// </summary>
	public bool IsFixedText => FixedText != null;

	/// <summary>
	/// Get the fixed text. Only set when <see cref="P:NLog.Layouts.SimpleLayout.IsFixedText" /> is <see langword="true" />
	/// </summary>
	public string? FixedText { get; }

	/// <summary>
	/// Is the message a simple formatted string? (Can skip StringBuilder)
	/// </summary>
	internal bool IsSimpleStringText => _stringValueRenderer != null;

	/// <summary>
	/// Gets a collection of <see cref="T:NLog.LayoutRenderers.LayoutRenderer" /> objects that make up this layout.
	/// </summary>
	[NLogConfigurationIgnoreProperty]
	public ReadOnlyCollection<LayoutRenderer> Renderers => _renderers ?? (_renderers = new ReadOnlyCollection<LayoutRenderer>(_layoutRenderers));

	/// <summary>
	/// Gets a collection of <see cref="T:NLog.LayoutRenderers.LayoutRenderer" /> objects that make up this layout.
	/// </summary>
	public IEnumerable<LayoutRenderer> LayoutRenderers => _layoutRenderers;

	/// <summary>
	/// Gets the level of stack trace information required for rendering.
	/// </summary>
	public new StackTraceUsage StackTraceUsage => base.StackTraceUsage;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.SimpleLayout" /> class.
	/// </summary>
	public SimpleLayout()
		: this(ArrayHelper.Empty<LayoutRenderer>(), string.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.SimpleLayout" /> class.
	/// </summary>
	/// <param name="txt">The layout string to parse.</param>
	public SimpleLayout([Localizable(false)] string txt)
		: this(txt, ConfigurationItemFactory.Default)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.SimpleLayout" /> class.
	/// </summary>
	/// <param name="txt">The layout string to parse.</param>
	/// <param name="configurationItemFactory">The NLog factories to use when creating references to layout renderers.</param>
	public SimpleLayout([Localizable(false)] string txt, ConfigurationItemFactory configurationItemFactory)
		: this(txt, configurationItemFactory, null)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Layouts.SimpleLayout" /> class.
	/// </summary>
	/// <param name="txt">The layout string to parse.</param>
	/// <param name="configurationItemFactory">The NLog factories to use when creating references to layout renderers.</param>
	/// <param name="throwConfigExceptions">Whether <see cref="T:NLog.NLogConfigurationException" /> should be thrown on parse errors.</param>
	internal SimpleLayout([Localizable(false)] string txt, ConfigurationItemFactory configurationItemFactory, bool? throwConfigExceptions)
		: this(LayoutParser.CompileLayout(txt, configurationItemFactory, throwConfigExceptions, out string parsedText), parsedText)
	{
		OriginalText = txt ?? string.Empty;
	}

	internal SimpleLayout(LayoutRenderer[] layoutRenderers, [Localizable(false)] string txt)
	{
		Text = txt ?? string.Empty;
		OriginalText = txt ?? string.Empty;
		_layoutRenderers = layoutRenderers ?? ArrayHelper.Empty<LayoutRenderer>();
		_renderers = null;
		FixedText = null;
		_rawValueRenderer = null;
		_stringValueRenderer = null;
		if (_layoutRenderers.Length == 0)
		{
			FixedText = string.Empty;
			_stringValueRenderer = this;
		}
		else if (_layoutRenderers.Length == 1)
		{
			if (_layoutRenderers[0] is LiteralLayoutRenderer literalLayoutRenderer)
			{
				FixedText = literalLayoutRenderer.Text;
				_stringValueRenderer = this;
			}
			else if (_layoutRenderers[0] is IStringValueRenderer stringValueRenderer)
			{
				_stringValueRenderer = stringValueRenderer;
			}
			if (_layoutRenderers[0] is IRawValue rawValueRenderer)
			{
				_rawValueRenderer = rawValueRenderer;
			}
		}
	}

	/// <summary>
	/// Implicitly converts the specified string as LayoutRenderer-expression into a <see cref="T:NLog.Layouts.SimpleLayout" />.
	/// </summary>
	/// <param name="text">Text to be converted.</param>
	/// <returns>A <see cref="T:NLog.Layouts.SimpleLayout" /> object.</returns>
	public static implicit operator SimpleLayout?([Localizable(false)] string text)
	{
		if (text == null)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return new SimpleLayout(text);
		}
		return Default;
	}

	/// <summary>
	/// Escapes the passed text so that it can
	/// be used literally in all places where
	/// layout is normally expected without being
	/// treated as layout.
	/// </summary>
	/// <param name="text">The text to be escaped.</param>
	/// <returns>The escaped text.</returns>
	/// <remarks>
	/// Escaping is done by replacing all occurrences of
	/// '${' with '${literal:text=${}'
	/// </remarks>
	[Obsolete("Instead use Layout.FromLiteral()")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static string Escape([Localizable(false)] string text)
	{
		return text.Replace("${", "${literal:text=\\$\\{}");
	}

	/// <summary>
	/// Evaluates the specified text by expanding all layout renderers.
	/// </summary>
	/// <param name="text">The text to be evaluated.</param>
	/// <param name="logEvent">Log event to be used for evaluation.</param>
	/// <returns>The input text with all occurrences of ${} replaced with
	/// values provided by the appropriate layout renderers.</returns>
	public static string Evaluate([Localizable(false)] string text, LogEventInfo logEvent)
	{
		return Evaluate(text, null, logEvent);
	}

	/// <summary>
	/// Evaluates the specified text by expanding all layout renderers
	/// in new <see cref="T:NLog.LogEventInfo" /> context.
	/// </summary>
	/// <param name="text">The text to be evaluated.</param>
	/// <returns>The input text with all occurrences of ${} replaced with
	/// values provided by the appropriate layout renderers.</returns>
	public static string Evaluate([Localizable(false)] string text)
	{
		return Evaluate(text, null, null, null);
	}

	internal static string Evaluate(string text, LoggingConfiguration? loggingConfiguration, LogEventInfo? logEventInfo = null, bool? throwConfigExceptions = null)
	{
		try
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			if (text.IndexOf('$') < 0 || text.IndexOf('{') < 0 || text.IndexOf('}') < 0)
			{
				return text;
			}
			throwConfigExceptions = throwConfigExceptions ?? loggingConfiguration?.LogFactory.ThrowConfigExceptions;
			Layout layout = Layout.FromString(text, throwConfigExceptions ?? LogManager.ThrowConfigExceptions ?? LogManager.ThrowExceptions);
			layout.Initialize(loggingConfiguration);
			return layout.Render(logEventInfo ?? LogEventInfo.CreateNullEvent());
		}
		catch (NLogConfigurationException ex)
		{
			if (throwConfigExceptions ?? LogManager.ThrowConfigExceptions ?? LogManager.ThrowExceptions)
			{
				throw;
			}
			InternalLogger.Warn(ex, "Failed to Evaluate SimpleLayout: {0}", text);
			return text;
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Failed to Evaluate SimpleLayout: {0}", text);
			return text;
		}
	}

	/// <inheritdoc />
	public override string ToString()
	{
		if (string.IsNullOrEmpty(Text) && !IsFixedText && _layoutRenderers.Length != 0)
		{
			return ToStringWithNestedItems(_layoutRenderers, (LayoutRenderer r) => r.ToString());
		}
		return Text;
	}

	/// <inheritdoc />
	protected override void InitializeLayout()
	{
		LayoutRenderer[] layoutRenderers = _layoutRenderers;
		foreach (LayoutRenderer layoutRenderer in layoutRenderers)
		{
			try
			{
				layoutRenderer.Initialize(base.LoggingConfiguration);
			}
			catch (Exception ex)
			{
				if (InternalLogger.IsWarnEnabled || InternalLogger.IsErrorEnabled)
				{
					InternalLogger.Warn(ex, "Exception in '{0}.Initialize()'", layoutRenderer.GetType());
				}
				if (ex.MustBeRethrown())
				{
					throw;
				}
			}
		}
		base.InitializeLayout();
	}

	/// <inheritdoc />
	public override void Precalculate(LogEventInfo logEvent)
	{
		if (PrecalculateMustRenderLayoutValue(logEvent))
		{
			using (AppendBuilderCreator appendBuilderCreator = new AppendBuilderCreator(mustBeEmpty: true))
			{
				RenderFormattedMessage(logEvent, appendBuilderCreator.Builder);
				logEvent.AddCachedLayoutValue(this, appendBuilderCreator.Builder.ToString());
			}
		}
	}

	internal override void PrecalculateBuilder(LogEventInfo logEvent, StringBuilder target)
	{
		if (PrecalculateMustRenderLayoutValue(logEvent))
		{
			RenderFormattedMessage(logEvent, target);
			logEvent.AddCachedLayoutValue(this, target.ToString());
		}
	}

	private bool PrecalculateMustRenderLayoutValue(LogEventInfo logEvent)
	{
		if (!IsInitialized)
		{
			Initialize(base.LoggingConfiguration);
		}
		if (base.ThreadAgnostic && !base.ThreadAgnosticImmutable)
		{
			return false;
		}
		if (_rawValueRenderer != null && TryGetRawValue(logEvent, out object rawValue) && IsRawValueImmutable(rawValue))
		{
			return false;
		}
		if (logEvent.TryGetCachedLayoutValue(this, out object _))
		{
			return false;
		}
		if (IsSimpleStringText)
		{
			string formattedMessage = GetFormattedMessage(logEvent);
			logEvent.AddCachedLayoutValue(this, formattedMessage);
			return false;
		}
		return true;
	}

	private static bool IsRawValueImmutable(object? value)
	{
		if (value != null)
		{
			if (Convert.GetTypeCode(value) == TypeCode.Object)
			{
				return value.GetType().IsValueType;
			}
			return true;
		}
		return false;
	}

	/// <inheritdoc />
	internal override bool TryGetRawValue(LogEventInfo logEvent, out object? rawValue)
	{
		if (_rawValueRenderer == null)
		{
			rawValue = null;
			return false;
		}
		return TryGetSafeRawValue(logEvent, out rawValue);
	}

	private bool TryGetSafeRawValue(LogEventInfo logEvent, out object? rawValue)
	{
		try
		{
			if (_rawValueRenderer == null)
			{
				rawValue = null;
				return false;
			}
			if (!IsInitialized)
			{
				Initialize(base.LoggingConfiguration);
			}
			if ((!base.ThreadAgnostic || base.ThreadAgnosticImmutable) && logEvent.TryGetCachedLayoutValue(this, out object _))
			{
				rawValue = null;
				return false;
			}
			return _rawValueRenderer.TryGetRawValue(logEvent, out rawValue);
		}
		catch (Exception ex)
		{
			if (InternalLogger.IsWarnEnabled || InternalLogger.IsErrorEnabled)
			{
				InternalLogger.Warn(ex, "Exception in TryGetRawValue using '{0}.TryGetRawValue()'", _rawValueRenderer?.GetType());
			}
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
		rawValue = null;
		return false;
	}

	/// <inheritdoc />
	protected override string GetFormattedMessage(LogEventInfo logEvent)
	{
		string stringValue = FixedText;
		if (stringValue != null)
		{
			return stringValue;
		}
		if (_stringValueRenderer == null || !TryGetSafeStringValue(logEvent, out stringValue))
		{
			return RenderAllocateBuilder(logEvent);
		}
		return stringValue;
	}

	private bool TryGetSafeStringValue(LogEventInfo logEvent, out string stringValue)
	{
		try
		{
			if (!IsInitialized)
			{
				Initialize(base.LoggingConfiguration);
			}
			string text = _stringValueRenderer?.GetFormattedString(logEvent);
			if (text == null)
			{
				stringValue = string.Empty;
				_stringValueRenderer = null;
				return false;
			}
			stringValue = text;
			return true;
		}
		catch (Exception ex)
		{
			if (InternalLogger.IsWarnEnabled || InternalLogger.IsErrorEnabled)
			{
				InternalLogger.Warn(ex, "Exception in '{0}.GetFormattedString()'", _stringValueRenderer?.GetType());
			}
			if (ex.MustBeRethrown())
			{
				throw;
			}
		}
		stringValue = string.Empty;
		return false;
	}

	/// <inheritdoc />
	protected override void RenderFormattedMessage(LogEventInfo logEvent, StringBuilder target)
	{
		string fixedText = FixedText;
		if (fixedText == null)
		{
			LayoutRenderer[] layoutRenderers = _layoutRenderers;
			for (int i = 0; i < layoutRenderers.Length; i++)
			{
				layoutRenderers[i].RenderAppendBuilder(logEvent, target);
			}
		}
		else
		{
			target.Append(fixedText);
		}
	}

	string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		string text = FixedText;
		if (text == null)
		{
			if (!IsSimpleStringText)
			{
				return null;
			}
			text = Render(logEvent);
		}
		return text;
	}
}

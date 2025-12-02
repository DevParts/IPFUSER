using System;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Applies caching to another layout output.
/// </summary>
/// <remarks>
/// The value of the inner layout will be rendered only once and reused subsequently.
///
/// <a href="https://github.com/NLog/NLog/wiki/Cached-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Cached-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("cached")]
[AmbientProperty("Cached")]
[AmbientProperty("ClearCache")]
[AmbientProperty("CachedSeconds")]
[ThreadAgnostic]
public sealed class CachedLayoutRendererWrapper : WrapperLayoutRendererBase, IStringValueRenderer
{
	/// <summary>
	/// A value indicating when the cache is cleared.
	/// </summary>
	[Flags]
	public enum ClearCacheOption
	{
		/// <summary>Never clear the cache.</summary>
		None = 0,
		/// <summary>Clear the cache whenever the <see cref="T:NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper" /> is initialized.</summary>
		OnInit = 1,
		/// <summary>Clear the cache whenever the <see cref="T:NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper" /> is closed.</summary>
		OnClose = 2
	}

	private readonly object _lockObject = new object();

	private string? _cachedValue;

	private string? _renderedCacheKey;

	private DateTime _cachedValueExpires;

	private TimeSpan? _cachedValueTimeout;

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="T:NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper" /> is enabled.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool Cached { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating when the cache is cleared.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper.ClearCacheOption.OnInit" /> | <see cref="F:NLog.LayoutRenderers.Wrappers.CachedLayoutRendererWrapper.ClearCacheOption.OnClose" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public ClearCacheOption ClearCache { get; set; } = ClearCacheOption.OnInit | ClearCacheOption.OnClose;

	/// <summary>
	/// Gets or sets whether to reset cached value when CacheKey output changes. Example CacheKey could render current day, so the cached-value is reset on day roll.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout? CacheKey { get; set; }

	/// <summary>
	/// Gets or sets a value indicating how many seconds the value should stay cached until it expires
	/// </summary>
	/// <remarks>Default: <see langword="0" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public int CachedSeconds
	{
		get
		{
			return (int)(_cachedValueTimeout?.TotalSeconds ?? 0.0);
		}
		set
		{
			_cachedValueTimeout = TimeSpan.FromSeconds(value);
			if (_cachedValueTimeout > TimeSpan.Zero)
			{
				Cached = true;
			}
		}
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		if ((ClearCache & ClearCacheOption.OnInit) == ClearCacheOption.OnInit)
		{
			_cachedValue = null;
		}
	}

	/// <inheritdoc />
	protected override void CloseLayoutRenderer()
	{
		base.CloseLayoutRenderer();
		if ((ClearCache & ClearCacheOption.OnClose) == ClearCacheOption.OnClose)
		{
			_cachedValue = null;
		}
	}

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		return text;
	}

	/// <inheritdoc />
	protected override string RenderInner(LogEventInfo logEvent)
	{
		if (Cached)
		{
			string text = CacheKey?.Render(logEvent) ?? string.Empty;
			string text2 = LookupValidCachedValue(logEvent, text);
			if (text2 == null)
			{
				lock (_lockObject)
				{
					text2 = LookupValidCachedValue(logEvent, text);
					if (text2 == null)
					{
						text2 = (_cachedValue = base.RenderInner(logEvent));
						_renderedCacheKey = text;
						if (_cachedValueTimeout.HasValue)
						{
							_cachedValueExpires = logEvent.TimeStamp + _cachedValueTimeout.Value;
						}
					}
				}
			}
			return text2;
		}
		return base.RenderInner(logEvent);
	}

	private string? LookupValidCachedValue(LogEventInfo logEvent, string newCacheKey)
	{
		if (_renderedCacheKey != newCacheKey)
		{
			return null;
		}
		if (_cachedValueTimeout.HasValue && logEvent.TimeStamp > _cachedValueExpires)
		{
			return null;
		}
		return _cachedValue;
	}

	string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		if (!Cached)
		{
			return null;
		}
		return RenderInner(logEvent);
	}
}

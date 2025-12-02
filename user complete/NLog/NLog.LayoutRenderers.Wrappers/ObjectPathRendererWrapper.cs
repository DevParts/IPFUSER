using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.LayoutRenderers.Wrappers;

/// <summary>
/// Render a single property of a object
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ObjectPath-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ObjectPath-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("Object-Path")]
[AmbientProperty("ObjectPath")]
[ThreadAgnostic]
public sealed class ObjectPathRendererWrapper : WrapperLayoutRendererBase, IRawValue
{
	private ObjectReflectionCache? _objectReflectionCache;

	private ObjectPropertyPath _objectPropertyPath;

	private ObjectReflectionCache ObjectReflectionCache => _objectReflectionCache ?? (_objectReflectionCache = new ObjectReflectionCache(base.LoggingConfiguration.GetServiceProvider()));

	/// <summary>
	/// Gets or sets the object-property-navigation-path for lookup of nested property
	///
	/// Shortcut for <see cref="P:NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper.ObjectPath" />
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string Path
	{
		get
		{
			return ObjectPath;
		}
		set
		{
			ObjectPath = value;
		}
	}

	/// <summary>
	/// Gets or sets the object-property-navigation-path for lookup of nested property
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string ObjectPath
	{
		get
		{
			return _objectPropertyPath.Value ?? string.Empty;
		}
		set
		{
			_objectPropertyPath.Value = value;
		}
	}

	/// <summary>
	/// Format string for conversion from object to string.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets the culture used for rendering.
	/// </summary>
	/// <remarks>Default: <see cref="P:System.Globalization.CultureInfo.InvariantCulture" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

	/// <inheritdoc />
	protected override string Transform(string text)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	protected override void RenderInnerAndTransform(LogEventInfo logEvent, StringBuilder builder, int orgLength)
	{
		if (TryGetRawPropertyValue(logEvent, out object propertyValue))
		{
			AppendFormattedValue(builder, logEvent, propertyValue, Format, Culture);
		}
	}

	private bool TryGetRawPropertyValue(LogEventInfo logEvent, out object? propertyValue)
	{
		Layout inner = base.Inner;
		if (inner != null && inner.TryGetRawValue(logEvent, out object rawValue) && rawValue != null && TryGetPropertyValue(rawValue, out propertyValue))
		{
			return true;
		}
		propertyValue = null;
		return false;
	}

	/// <summary>
	/// Lookup property-value from source object based on <see cref="P:NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper.ObjectPath" />
	/// </summary>
	/// <returns>Could resolve property-value?</returns>
	public bool TryGetPropertyValue(object sourceObject, out object? propertyValue)
	{
		return ObjectReflectionCache.TryGetObjectProperty(sourceObject, _objectPropertyPath.PathNames, out propertyValue);
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object? value)
	{
		return TryGetRawPropertyValue(logEvent, out value);
	}
}

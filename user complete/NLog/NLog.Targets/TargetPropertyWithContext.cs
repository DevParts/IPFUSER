using System;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Attribute details for <see cref="T:NLog.Targets.TargetWithContext" />
/// </summary>
[NLogConfigurationItem]
public class TargetPropertyWithContext
{
	private readonly ValueTypeLayoutInfo _layoutInfo = new ValueTypeLayoutInfo();

	private bool _includeEmptyValue;

	/// <summary>
	/// Gets or sets the name of the property.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="1" />
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the layout used for rendering the property value.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public Layout Layout
	{
		get
		{
			return _layoutInfo.Layout;
		}
		set
		{
			_layoutInfo.Layout = value;
		}
	}

	/// <summary>
	/// Gets or sets the type of the property.
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public Type PropertyType
	{
		get
		{
			return _layoutInfo.ValueType ?? typeof(string);
		}
		set
		{
			_layoutInfo.ValueType = value;
		}
	}

	/// <summary>
	/// Gets or sets the fallback value when result value is not available
	/// </summary>
	/// <remarks>Default: <see langword="null" /></remarks>
	/// <docgen category="Layout Options" order="100" />
	public Layout? DefaultValue
	{
		get
		{
			return _layoutInfo.DefaultValue;
		}
		set
		{
			_layoutInfo.DefaultValue = value;
		}
	}

	/// <summary>
	/// Gets or sets whether empty property value should be included in the output.
	/// </summary>
	/// <remarks>Default: <see langword="false" /> . Empty value is either null or empty string</remarks>
	/// <docgen category="Layout Options" order="100" />
	public bool IncludeEmptyValue
	{
		get
		{
			return _includeEmptyValue;
		}
		set
		{
			_includeEmptyValue = value;
			_layoutInfo.ForceDefaultValueNull = !value;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.TargetPropertyWithContext" /> class.
	/// </summary>
	public TargetPropertyWithContext()
		: this(string.Empty, NLog.Layouts.Layout.Empty)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.TargetPropertyWithContext" /> class.
	/// </summary>
	/// <param name="name">The name of the attribute.</param>
	/// <param name="layout">The layout of the attribute's value.</param>
	public TargetPropertyWithContext(string name, Layout layout)
	{
		Name = name;
		Layout = layout;
	}

	/// <summary>
	/// Render Result Value
	/// </summary>
	/// <param name="logEvent">Log event for rendering</param>
	/// <returns>Result value when available, else fallback to defaultValue</returns>
	public object? RenderValue(LogEventInfo logEvent)
	{
		return _layoutInfo.RenderValue(logEvent);
	}
}

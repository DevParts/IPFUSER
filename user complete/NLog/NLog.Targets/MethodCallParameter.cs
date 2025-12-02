using System;
using System.ComponentModel;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// A parameter to MethodCall.
/// </summary>
[NLogConfigurationItem]
public class MethodCallParameter
{
	private readonly ValueTypeLayoutInfo _layoutInfo = new ValueTypeLayoutInfo();

	/// <summary>
	/// Gets or sets the name of the parameter.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Parameter Options" order="1" />
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the layout used for rendering the method-parameter value.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:NLog.Layouts.Layout.Empty" /></remarks>
	/// <docgen category="Parameter Options" order="10" />
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
	/// Obsolete and replaced by <see cref="P:NLog.Targets.MethodCallParameter.ParameterType" /> with NLog v4.6.
	/// Gets or sets the type of the parameter. Obsolete alias for <see cref="P:NLog.Targets.MethodCallParameter.ParameterType" />
	/// </summary>
	/// <remarks>Default: <c>typeof(string)</c></remarks>
	/// <docgen category="Parameter Options" order="50" />
	[Obsolete("Use property ParameterType instead. Marked obsolete on NLog 4.6")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Type Type
	{
		get
		{
			return ParameterType;
		}
		set
		{
			ParameterType = value;
		}
	}

	/// <summary>
	/// Gets or sets the type of the parameter.
	/// </summary>
	/// <remarks>Default: <c>typeof(string)</c></remarks>
	/// <docgen category="Parameter Options" order="50" />
	public Type ParameterType
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
	/// <docgen category="Parameter Options" order="50" />
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
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MethodCallParameter" /> class.
	/// </summary>
	public MethodCallParameter()
	{
		ParameterType = typeof(string);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MethodCallParameter" /> class.
	/// </summary>
	/// <param name="layout">The layout to use for parameter value.</param>
	public MethodCallParameter(Layout layout)
	{
		ParameterType = typeof(string);
		Layout = layout;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MethodCallParameter" /> class.
	/// </summary>
	/// <param name="parameterName">Name of the parameter.</param>
	/// <param name="layout">The layout.</param>
	public MethodCallParameter(string parameterName, Layout layout)
	{
		ParameterType = typeof(string);
		Name = parameterName;
		Layout = layout;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.MethodCallParameter" /> class.
	/// </summary>
	/// <param name="name">The name of the parameter.</param>
	/// <param name="layout">The layout.</param>
	/// <param name="type">The type of the parameter.</param>
	public MethodCallParameter(string name, Layout layout, Type type)
	{
		ParameterType = type;
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

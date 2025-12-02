using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The information about the running process.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ProcessInfo-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ProcessInfo-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("processinfo")]
public class ProcessInfoLayoutRenderer : LayoutRenderer
{
	private Process? _process;

	private ReflectionHelpers.LateBoundMethod? _lateBoundPropertyGet;

	/// <summary>
	/// Gets or sets the property to retrieve.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.LayoutRenderers.ProcessInfoProperty.Id" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public ProcessInfoProperty Property { get; set; } = ProcessInfoProperty.Id;

	/// <summary>
	/// Gets or sets the format string used when converting the property value to a string, when the
	/// property supports formatting (e.g., <see cref="T:System.DateTime" />, <see cref="T:System.TimeSpan" />, or enum types).
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
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		PropertyInfo property = typeof(Process).GetProperty(Property.ToString());
		if ((object)property == null)
		{
			throw new ArgumentException($"Property '{Property}' not found in System.Diagnostics.Process");
		}
		_lateBoundPropertyGet = ReflectionHelpers.CreateLateBoundMethod(property.GetGetMethod());
		_process = Process.GetCurrentProcess();
	}

	/// <inheritdoc />
	protected override void CloseLayoutRenderer()
	{
		_process?.Close();
		_process = null;
		base.CloseLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		if (_process != null && _lateBoundPropertyGet != null)
		{
			object obj = _lateBoundPropertyGet(_process, ArrayHelper.Empty<object>());
			if (obj != null)
			{
				AppendFormattedValue(builder, logEvent, obj, Format, Culture);
			}
		}
	}
}

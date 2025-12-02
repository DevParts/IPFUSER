using System;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers;

/// <summary>
/// Lookup parameter value from <see cref="P:NLog.Config.InstallationContext.Parameters" />
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/InstallContext-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/InstallContext-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("install-context")]
public class InstallContextLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the name of the parameter.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Parameter { get; set; } = string.Empty;

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		object value = GetValue(logEvent);
		if (value != null)
		{
			IFormatProvider formatProvider = GetFormatProvider(logEvent);
			builder.Append(Convert.ToString(value, formatProvider));
		}
	}

	private object? GetValue(LogEventInfo logEvent)
	{
		if (logEvent.Properties.TryGetValue(Parameter, out object value))
		{
			return value;
		}
		return null;
	}
}

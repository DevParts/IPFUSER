using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.LayoutRenderers;

/// <summary>
/// Render a NLog Configuration variable assigned from API or loaded from config-file
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Var-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Var-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("var")]
public class VariableLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the name of the NLog variable.
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the default value to be used when the variable is not set.
	/// </summary>
	/// <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string Default { get; set; } = string.Empty;

	/// <summary>
	/// Gets the configuration variable layout matching the configured Name
	/// </summary>
	/// <remarks>Mostly relevant for the scanning of active NLog Layouts (Ex. CallSite capture)</remarks>
	public Layout? ActiveLayout
	{
		get
		{
			if (!TryGetLayout(out Layout layout))
			{
				return null;
			}
			return layout;
		}
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		if (string.IsNullOrEmpty(Name))
		{
			throw new NLogConfigurationException("Var-LayoutRenderer Name-property must be assigned. Lookup blank value not supported.");
		}
		if (TryGetLayout(out Layout layout))
		{
			layout?.Initialize(base.LoggingConfiguration);
		}
		base.InitializeLayoutRenderer();
	}

	/// <summary>
	/// Try lookup the configuration variable layout matching the configured Name
	/// </summary>
	private bool TryGetLayout(out Layout? layout)
	{
		layout = null;
		if (Name != null)
		{
			return base.LoggingConfiguration?.TryLookupDynamicVariable(Name, out layout) ?? false;
		}
		return false;
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		if (Name != null)
		{
			if (TryGetLayout(out Layout layout))
			{
				layout?.Render(logEvent, builder);
			}
			else if (Default != null)
			{
				builder.Append(Default);
			}
		}
	}
}

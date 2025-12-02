using System;
using System.ComponentModel;
using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// Render value for Application setting retrieved from App.config or Web.config file.
/// </summary>
/// <remarks>
/// <code lang="NLog Layout Renderer">
/// ${appsetting:item=mysetting:default=mydefault} - produces "mydefault" if no appsetting
/// </code>
/// <a href="https://github.com/NLog/NLog/wiki/AppSetting-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/AppSetting-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("appsetting")]
[ThreadAgnostic]
public sealed class AppSettingLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	private string? _connectionStringName;

	/// <summary>
	///  The AppSetting item-name
	/// </summary>
	///  <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	///  <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Item { get; set; } = string.Empty;

	/// <summary>
	///  Obsolete and replaced by <see cref="P:NLog.LayoutRenderers.AppSettingLayoutRenderer.Item" /> with NLog v4.6.
	///
	///  The AppSetting item-name
	/// </summary>
	[Obsolete("Allows easier conversion from NLog.Extended. Instead use Item-property. Marked obsolete in NLog 4.6")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string Name
	{
		get
		{
			return Item;
		}
		set
		{
			Item = value;
		}
	}

	/// <summary>
	///  The default value to render if the AppSetting value is null.
	/// </summary>
	///  <remarks>Default: <see cref="F:System.String.Empty" /></remarks>
	///  <docgen category="Layout Options" order="10" />
	public string Default { get; set; } = string.Empty;

	internal IConfigurationManager ConfigurationManager { get; set; } = new ConfigurationManager();

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		string text = "ConnectionStrings.";
		string item = Item;
		_connectionStringName = ((item != null && item.TrimStart().StartsWith(text, StringComparison.OrdinalIgnoreCase)) ? Item.TrimStart().Substring(text.Length) : null);
		if (string.IsNullOrEmpty(Item))
		{
			throw new NLogConfigurationException("AppSetting-LayoutRenderer Item-property must be assigned. Lookup blank value not supported.");
		}
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		builder.Append(GetStringValue());
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		return GetStringValue();
	}

	private string GetStringValue()
	{
		if (string.IsNullOrEmpty(Item))
		{
			return Default;
		}
		return ((_connectionStringName == null) ? ConfigurationManager.AppSettings[Item] : ConfigurationManager.LookupConnectionString(_connectionStringName)?.ConnectionString) ?? Default ?? string.Empty;
	}
}

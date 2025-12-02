using System.Security.Principal;
using System.Text;
using System.Threading;

namespace NLog.LayoutRenderers;

/// <summary>
/// Thread identity information (name and authentication information).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Identity-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Identity-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("identity")]
public class IdentityLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the separator to be used when concatenating
	/// parts of identity information.
	/// </summary>
	/// <remarks>Default: <c>:</c></remarks>
	/// <docgen category="Layout Options" order="50" />
	public string Separator { get; set; } = ":";

	/// <summary>
	/// Gets or sets a value indicating whether to render Thread.CurrentPrincipal.Identity.Name.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool Name { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to render Thread.CurrentPrincipal.Identity.AuthenticationType.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool AuthType { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to render Thread.CurrentPrincipal.Identity.IsAuthenticated.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool IsAuthenticated { get; set; } = true;

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		IIdentity value = GetValue();
		if (value != null)
		{
			string value2 = string.Empty;
			if (IsAuthenticated)
			{
				builder.Append(value2);
				value2 = Separator;
				builder.Append(value.IsAuthenticated ? "auth" : "notauth");
			}
			if (AuthType)
			{
				builder.Append(value2);
				value2 = Separator;
				builder.Append(value.AuthenticationType);
			}
			if (Name)
			{
				builder.Append(value2);
				builder.Append(value.Name);
			}
		}
	}

	private static IIdentity? GetValue()
	{
		return Thread.CurrentPrincipal?.Identity;
	}
}

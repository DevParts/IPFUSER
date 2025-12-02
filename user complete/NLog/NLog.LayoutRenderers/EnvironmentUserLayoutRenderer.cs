using System;
using System.Text;
using NLog.Common;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// Thread identity information (username).
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Environment-User-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Environment-User-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("environment-user")]
public class EnvironmentUserLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	/// <summary>
	/// Gets or sets a value indicating whether username should be included.
	/// </summary>
	/// <remarks>Default: <see langword="true" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool UserName { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether domain name should be included.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool Domain { get; set; }

	/// <summary>
	/// Gets or sets the default value to be used when the User is not set.
	/// </summary>
	/// <remarks>Default: <c>UserUnknown</c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string DefaultUser { get; set; } = "UserUnknown";

	/// <summary>
	/// Gets or sets the default value to be used when the Domain is not set.
	/// </summary>
	/// <docgen category="Layout Options" order="10" />
	/// <remarks>Default: <c>DomainUnknown</c></remarks>
	public string DefaultDomain { get; set; } = "DomainUnknown";

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
		if (UserName)
		{
			if (!Domain)
			{
				return GetUserName();
			}
			return GetDomainName() + "\\" + GetUserName();
		}
		if (!Domain)
		{
			return string.Empty;
		}
		return GetDomainName();
	}

	private string GetUserName()
	{
		return GetValueSafe(() => Environment.UserName, DefaultUser);
	}

	private string GetDomainName()
	{
		return GetValueSafe(() => Environment.UserDomainName, DefaultDomain);
	}

	private static string GetValueSafe(Func<string> getValue, string defaultValue)
	{
		try
		{
			string text = getValue();
			return string.IsNullOrEmpty(text) ? (defaultValue ?? string.Empty) : text;
		}
		catch (Exception ex)
		{
			InternalLogger.Warn(ex, "Failed to lookup Environment-User. Fallback value={0}", defaultValue);
			return defaultValue ?? string.Empty;
		}
	}
}

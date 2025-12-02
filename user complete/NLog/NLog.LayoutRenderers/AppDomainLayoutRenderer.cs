using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.Internal.Fakeables;

namespace NLog.LayoutRenderers;

/// <summary>
///  Used to render the application domain name.
///  </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/AppDomain-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/AppDomain-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("appdomain")]
[AppDomainFixedOutput]
[ThreadAgnostic]
public class AppDomainLayoutRenderer : LayoutRenderer
{
	private const string ShortFormat = "{0:00}";

	private const string LongFormat = "{0:0000}:{1}";

	private const string FriendlyFormat = "{1}";

	private const string LongFormatCode = "Long";

	private const string ShortFormatCode = "Short";

	private const string FriendlyFormatCode = "Friendly";

	private readonly IAppEnvironment _currentAppEnvironment;

	private string? _assemblyName;

	/// <summary>
	/// Gets or sets format-string for displaying <see cref="T:System.AppDomain" /> details.
	/// This string is used in <see cref="M:System.String.Format(System.String,System.Object[])" /> where
	/// first parameter is <see cref="P:System.AppDomain.Id" /> and second is <see cref="P:System.AppDomain.FriendlyName" />
	/// </summary>
	/// <remarks>
	/// Default: <c>Long</c> . How alias names are mapped:
	/// <code>Short = {0:00}</code>
	/// <code>Long = {0:0000}:{1}</code>
	/// <code>Friendly = {1}</code>
	/// </remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Format { get; set; } = "Long";

	/// <summary>
	/// Create a new renderer
	/// </summary>
	public AppDomainLayoutRenderer()
		: this(LogFactory.DefaultAppEnvironment)
	{
	}

	/// <summary>
	/// Create a new renderer
	/// </summary>
	internal AppDomainLayoutRenderer(IAppEnvironment appEnvironment)
	{
		_currentAppEnvironment = appEnvironment;
	}

	/// <inheritdoc />
	protected override void InitializeLayoutRenderer()
	{
		_assemblyName = null;
		base.InitializeLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void CloseLayoutRenderer()
	{
		_assemblyName = null;
		base.CloseLayoutRenderer();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		if (_assemblyName == null)
		{
			string formattingString = GetFormattingString(Format);
			_assemblyName = string.Format(CultureInfo.InvariantCulture, formattingString, _currentAppEnvironment.AppDomainId, _currentAppEnvironment.AppDomainFriendlyName);
		}
		builder.Append(_assemblyName);
	}

	private static string GetFormattingString(string format)
	{
		if (string.Equals(format, "Long", StringComparison.OrdinalIgnoreCase))
		{
			return "{0:0000}:{1}";
		}
		if (string.Equals(format, "Short", StringComparison.OrdinalIgnoreCase))
		{
			return "{0:00}";
		}
		if (string.Equals(format, "Friendly", StringComparison.OrdinalIgnoreCase))
		{
			return "{1}";
		}
		return format;
	}
}

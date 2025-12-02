using System;
using System.Globalization;
using System.Text;
using NLog.Config;

namespace NLog.LayoutRenderers;

/// <summary>
/// Render information of <see cref="P:System.Exception.Data" />
/// for the exception passed to the logger call
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/ExceptionData-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/ExceptionData-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("exceptiondata")]
[LayoutRenderer("exception-data")]
[ThreadAgnostic]
[ThreadAgnosticImmutable]
public class ExceptionDataLayoutRenderer : LayoutRenderer
{
	/// <summary>
	/// Gets or sets the key to search the exception Data for
	/// </summary>
	/// <remarks><b>[Required]</b> Default: <see cref="F:System.String.Empty" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	[DefaultParameter]
	public string Item { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets whether to render innermost Exception from <see cref="M:System.Exception.GetBaseException" />
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool BaseException { get; set; }

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
	protected override void InitializeLayoutRenderer()
	{
		base.InitializeLayoutRenderer();
		if (string.IsNullOrEmpty(Item))
		{
			throw new NLogConfigurationException("ExceptionData-LayoutRenderer Item-property must be assigned. Lookup blank value not supported.");
		}
	}

	private Exception? GetTopException(LogEventInfo logEvent)
	{
		if (!BaseException)
		{
			return logEvent.Exception;
		}
		return logEvent.Exception?.GetBaseException();
	}

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		Exception topException = GetTopException(logEvent);
		if (topException != null)
		{
			object obj = topException.Data[Item];
			if (obj != null)
			{
				AppendFormattedValue(builder, logEvent, obj, Format, Culture);
			}
		}
	}
}

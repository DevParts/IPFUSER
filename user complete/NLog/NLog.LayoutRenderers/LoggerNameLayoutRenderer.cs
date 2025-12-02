using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The logger name.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Logger-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Logger-Layout-Renderer">Documentation on NLog Wiki</seealso>///
[LayoutRenderer("loggername")]
[LayoutRenderer("logger")]
[ThreadAgnostic]
public class LoggerNameLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	/// <summary>
	/// Gets or sets a value indicating whether to render short logger name (the part after the trailing dot character).
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool ShortName { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to render prefix of logger name (the part before the trailing dot character).
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool PrefixName { get; set; }

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		string loggerName = logEvent.LoggerName;
		if (ShortName)
		{
			int num = TryGetLastDotForShortName(loggerName);
			if (num >= 0)
			{
				builder.Append(loggerName, num + 1, loggerName.Length - num - 1);
				return;
			}
		}
		else if (PrefixName)
		{
			int num2 = TryGetLastDotForShortName(loggerName);
			if (num2 > 0)
			{
				builder.Append(loggerName, 0, num2);
				return;
			}
		}
		builder.Append(loggerName);
	}

	string IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		string loggerName = logEvent.LoggerName;
		if (ShortName)
		{
			int num = TryGetLastDotForShortName(loggerName);
			if (num >= 0)
			{
				return loggerName.Substring(num + 1);
			}
		}
		else if (PrefixName)
		{
			int num2 = TryGetLastDotForShortName(loggerName);
			if (num2 > 0)
			{
				return loggerName.Substring(0, num2);
			}
		}
		return loggerName ?? string.Empty;
	}

	private static int TryGetLastDotForShortName(string loggerName)
	{
		return loggerName?.LastIndexOf('.') ?? (-1);
	}
}

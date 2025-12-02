using System.Text;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The log level.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Level-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Level-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("level")]
[LayoutRenderer("loglevel")]
[ThreadAgnostic]
public class LevelLayoutRenderer : LayoutRenderer, IRawValue, IStringValueRenderer
{
	private static readonly string[] _upperCaseMapper = new string[7]
	{
		LogLevel.Trace.ToString().ToUpperInvariant(),
		LogLevel.Debug.ToString().ToUpperInvariant(),
		LogLevel.Info.ToString().ToUpperInvariant(),
		LogLevel.Warn.ToString().ToUpperInvariant(),
		LogLevel.Error.ToString().ToUpperInvariant(),
		LogLevel.Fatal.ToString().ToUpperInvariant(),
		LogLevel.Off.ToString().ToUpperInvariant()
	};

	/// <summary>
	/// Gets or sets a value indicating the output format of the level.
	/// </summary>
	/// <remarks>Default: <see cref="F:NLog.LayoutRenderers.LevelFormat.Name" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public LevelFormat Format { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether upper case conversion should be applied.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool Uppercase { get; set; }

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		LogLevel value = GetValue(logEvent);
		switch (Format)
		{
		case LevelFormat.Name:
			builder.Append(Uppercase ? GetUpperCaseString(value) : value.ToString());
			break;
		case LevelFormat.FirstCharacter:
			builder.Append(value.ToString()[0]);
			break;
		case LevelFormat.Ordinal:
			builder.AppendInvariant(value.Ordinal);
			break;
		case LevelFormat.FullName:
			builder.Append(GetFullNameString(value));
			break;
		case LevelFormat.TriLetter:
			builder.Append(GetTriLetterString(value));
			break;
		}
	}

	private static string GetUpperCaseString(LogLevel level)
	{
		int ordinal = level.Ordinal;
		if (ordinal < 0 || ordinal >= _upperCaseMapper.Length)
		{
			return level.ToString().ToUpperInvariant();
		}
		return _upperCaseMapper[ordinal];
	}

	private string GetFullNameString(LogLevel level)
	{
		if (level == LogLevel.Info)
		{
			if (!Uppercase)
			{
				return "Information";
			}
			return "INFORMATION";
		}
		if (level == LogLevel.Warn)
		{
			if (!Uppercase)
			{
				return "Warning";
			}
			return "WARNING";
		}
		if (!Uppercase)
		{
			return level.ToString();
		}
		return GetUpperCaseString(level);
	}

	private string GetTriLetterString(LogLevel level)
	{
		if (level == LogLevel.Debug)
		{
			if (!Uppercase)
			{
				return "Dbg";
			}
			return "DBG";
		}
		if (level == LogLevel.Info)
		{
			if (!Uppercase)
			{
				return "Inf";
			}
			return "INF";
		}
		if (level == LogLevel.Warn)
		{
			if (!Uppercase)
			{
				return "Wrn";
			}
			return "WRN";
		}
		if (level == LogLevel.Error)
		{
			if (!Uppercase)
			{
				return "Err";
			}
			return "ERR";
		}
		if (level == LogLevel.Fatal)
		{
			if (!Uppercase)
			{
				return "Ftl";
			}
			return "FTL";
		}
		if (!Uppercase)
		{
			return "Trc";
		}
		return "TRC";
	}

	bool IRawValue.TryGetRawValue(LogEventInfo logEvent, out object value)
	{
		value = GetValue(logEvent);
		return true;
	}

	string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		if (Format == LevelFormat.Name)
		{
			LogLevel value = GetValue(logEvent);
			if (!Uppercase)
			{
				return value.ToString();
			}
			return GetUpperCaseString(value);
		}
		return null;
	}

	private static LogLevel GetValue(LogEventInfo logEvent)
	{
		return logEvent.Level;
	}
}

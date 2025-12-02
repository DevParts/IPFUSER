using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog.Conditions;

namespace NLog.Targets;

/// <summary>
/// Color formatting for <see cref="T:NLog.Targets.ColoredConsoleTarget" /> using ANSI Color Codes
/// </summary>
internal sealed class ColoredConsoleAnsiPrinter : IColoredConsolePrinter
{
	private static string TerminalDefaultForegroundColorEscapeCode => "\u001b[39m\u001b[22m";

	private static string TerminalDefaultBackgroundColorEscapeCode => "\u001b[49m";

	/// <summary>
	/// Resets both foreground and background color.
	/// </summary>
	private static string TerminalDefaultColorEscapeCode => "\u001b[0m";

	/// <summary>
	/// ANSI have 8 color-codes (30-37) by default. The "bright" (or "intense") color-codes (90-97) are extended values not supported by all terminals
	/// </summary>
	public IList<ConsoleRowHighlightingRule> DefaultConsoleRowHighlightingRules { get; } = new List<ConsoleRowHighlightingRule>
	{
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Fatal", (LogEventInfo evt) => evt.Level == LogLevel.Fatal), ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Error", (LogEventInfo evt) => evt.Level == LogLevel.Error), ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Warn", (LogEventInfo evt) => evt.Level == LogLevel.Warn), ConsoleOutputColor.DarkYellow, ConsoleOutputColor.NoChange)
	};

	public TextWriter AcquireTextWriter(TextWriter consoleStream, StringBuilder reusableBuilder)
	{
		return new StringWriter(reusableBuilder ?? new StringBuilder(50), consoleStream.FormatProvider);
	}

	public void ReleaseTextWriter(TextWriter consoleWriter, TextWriter consoleStream, ConsoleColor? oldForegroundColor, ConsoleColor? oldBackgroundColor, bool flush)
	{
		StringBuilder stringBuilder = (consoleWriter as StringWriter)?.GetStringBuilder();
		if (stringBuilder != null)
		{
			stringBuilder.Append(TerminalDefaultColorEscapeCode);
			ConsoleTargetHelper.WriteLineThreadSafe(consoleStream, stringBuilder.ToString(), flush);
		}
	}

	public ConsoleColor? ChangeForegroundColor(TextWriter consoleWriter, ConsoleColor? foregroundColor, ConsoleColor? oldForegroundColor = null)
	{
		if (foregroundColor.HasValue)
		{
			consoleWriter.Write(GetForegroundColorEscapeCode(foregroundColor.Value));
		}
		return null;
	}

	public ConsoleColor? ChangeBackgroundColor(TextWriter consoleWriter, ConsoleColor? backgroundColor, ConsoleColor? oldBackgroundColor = null)
	{
		if (backgroundColor.HasValue)
		{
			consoleWriter.Write(GetBackgroundColorEscapeCode(backgroundColor.Value));
		}
		return null;
	}

	public void ResetDefaultColors(TextWriter consoleWriter, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
	{
		consoleWriter.Write(TerminalDefaultColorEscapeCode);
	}

	public void WriteSubString(TextWriter consoleWriter, string text, int index, int endIndex)
	{
		for (int i = index; i < endIndex; i++)
		{
			consoleWriter.Write(text[i]);
		}
	}

	public void WriteChar(TextWriter consoleWriter, char text)
	{
		consoleWriter.Write(text);
	}

	public void WriteLine(TextWriter consoleWriter, string text)
	{
		consoleWriter.Write(text);
	}

	/// <summary>
	/// Not using bold to get light colors, as it has to be cleared
	/// </summary>
	private static string GetForegroundColorEscapeCode(ConsoleColor color)
	{
		return color switch
		{
			ConsoleColor.Black => "\u001b[30m", 
			ConsoleColor.Blue => "\u001b[94m", 
			ConsoleColor.Cyan => "\u001b[96m", 
			ConsoleColor.DarkBlue => "\u001b[34m", 
			ConsoleColor.DarkCyan => "\u001b[36m", 
			ConsoleColor.DarkGray => "\u001b[90m", 
			ConsoleColor.DarkGreen => "\u001b[32m", 
			ConsoleColor.DarkMagenta => "\u001b[35m", 
			ConsoleColor.DarkRed => "\u001b[31m", 
			ConsoleColor.DarkYellow => "\u001b[33m", 
			ConsoleColor.Gray => "\u001b[37m", 
			ConsoleColor.Green => "\u001b[92m", 
			ConsoleColor.Magenta => "\u001b[95m", 
			ConsoleColor.Red => "\u001b[91m", 
			ConsoleColor.White => "\u001b[97m", 
			ConsoleColor.Yellow => "\u001b[93m", 
			_ => TerminalDefaultForegroundColorEscapeCode, 
		};
	}

	/// <summary>
	/// Not using bold to get light colors, as it has to be cleared (And because it only works for text, and not background)
	/// </summary>
	private static string GetBackgroundColorEscapeCode(ConsoleColor color)
	{
		return color switch
		{
			ConsoleColor.Black => "\u001b[40m", 
			ConsoleColor.Blue => "\u001b[104m", 
			ConsoleColor.Cyan => "\u001b[106m", 
			ConsoleColor.DarkBlue => "\u001b[44m", 
			ConsoleColor.DarkCyan => "\u001b[46m", 
			ConsoleColor.DarkGray => "\u001b[100m", 
			ConsoleColor.DarkGreen => "\u001b[42m", 
			ConsoleColor.DarkMagenta => "\u001b[45m", 
			ConsoleColor.DarkRed => "\u001b[41m", 
			ConsoleColor.DarkYellow => "\u001b[43m", 
			ConsoleColor.Gray => "\u001b[47m", 
			ConsoleColor.Green => "\u001b[102m", 
			ConsoleColor.Magenta => "\u001b[105m", 
			ConsoleColor.Red => "\u001b[101m", 
			ConsoleColor.White => "\u001b[107m", 
			ConsoleColor.Yellow => "\u001b[103m", 
			_ => TerminalDefaultBackgroundColorEscapeCode, 
		};
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NLog.Conditions;

namespace NLog.Targets;

/// <summary>
/// Color formatting for <see cref="T:NLog.Targets.ColoredConsoleTarget" /> using <see cref="P:System.Console.ForegroundColor" />
/// and <see cref="P:System.Console.BackgroundColor" />
/// </summary>
internal sealed class ColoredConsoleSystemPrinter : IColoredConsolePrinter
{
	public IList<ConsoleRowHighlightingRule> DefaultConsoleRowHighlightingRules { get; } = new List<ConsoleRowHighlightingRule>
	{
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Fatal", (LogEventInfo evt) => evt.Level == LogLevel.Fatal), ConsoleOutputColor.Red, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Error", (LogEventInfo evt) => evt.Level == LogLevel.Error), ConsoleOutputColor.Red, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Warn", (LogEventInfo evt) => evt.Level == LogLevel.Warn), ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Info", (LogEventInfo evt) => evt.Level == LogLevel.Info), ConsoleOutputColor.White, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Debug", (LogEventInfo evt) => evt.Level == LogLevel.Debug), ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange),
		new ConsoleRowHighlightingRule(ConditionMethodExpression.CreateMethodNoParameters("level == LogLevel.Trace", (LogEventInfo evt) => evt.Level == LogLevel.Trace), ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange)
	};

	public TextWriter AcquireTextWriter(TextWriter consoleStream, StringBuilder reusableBuilder)
	{
		return consoleStream;
	}

	public void ReleaseTextWriter(TextWriter consoleWriter, TextWriter consoleStream, ConsoleColor? oldForegroundColor, ConsoleColor? oldBackgroundColor, bool flush)
	{
		ResetDefaultColors(consoleWriter, oldForegroundColor, oldBackgroundColor);
		if (flush)
		{
			consoleWriter.Flush();
		}
	}

	public ConsoleColor? ChangeForegroundColor(TextWriter consoleWriter, ConsoleColor? foregroundColor, ConsoleColor? oldForegroundColor = null)
	{
		ConsoleColor consoleColor = oldForegroundColor ?? Console.ForegroundColor;
		if (foregroundColor.HasValue && consoleColor != foregroundColor.Value)
		{
			Console.ForegroundColor = foregroundColor.Value;
		}
		return consoleColor;
	}

	public ConsoleColor? ChangeBackgroundColor(TextWriter consoleWriter, ConsoleColor? backgroundColor, ConsoleColor? oldBackgroundColor = null)
	{
		ConsoleColor consoleColor = oldBackgroundColor ?? Console.BackgroundColor;
		if (backgroundColor.HasValue && consoleColor != backgroundColor.Value)
		{
			Console.BackgroundColor = backgroundColor.Value;
		}
		return consoleColor;
	}

	public void ResetDefaultColors(TextWriter consoleWriter, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
	{
		if (foregroundColor.HasValue)
		{
			Console.ForegroundColor = foregroundColor.Value;
		}
		if (backgroundColor.HasValue)
		{
			Console.BackgroundColor = backgroundColor.Value;
		}
	}

	public void WriteSubString(TextWriter consoleWriter, string text, int index, int endIndex)
	{
		consoleWriter.Write(text.Substring(index, endIndex - index));
	}

	public void WriteChar(TextWriter consoleWriter, char text)
	{
		consoleWriter.Write(text);
	}

	public void WriteLine(TextWriter consoleWriter, string text)
	{
		consoleWriter.WriteLine(text);
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NLog.Targets;

/// <summary>
/// Controls the text and color formatting for <see cref="T:NLog.Targets.ColoredConsoleTarget" />
/// </summary>
internal interface IColoredConsolePrinter
{
	/// <summary>
	/// Default row highlight rules for the console printer
	/// </summary>
	IList<ConsoleRowHighlightingRule> DefaultConsoleRowHighlightingRules { get; }

	/// <summary>
	/// Creates a TextWriter for the console to start building a colored text message
	/// </summary>
	/// <param name="consoleStream">Active console stream</param>
	/// <param name="reusableBuilder">Optional StringBuilder to optimize performance</param>
	/// <returns>TextWriter for the console</returns>
	TextWriter AcquireTextWriter(TextWriter consoleStream, StringBuilder reusableBuilder);

	/// <summary>
	/// Releases the TextWriter for the console after having built a colored text message (Restores console colors)
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="consoleStream">Active console stream</param>
	/// <param name="oldForegroundColor">Original foreground color for console (If changed)</param>
	/// <param name="oldBackgroundColor">Original background color for console (If changed)</param>
	/// <param name="flush">Flush TextWriter</param>
	void ReleaseTextWriter(TextWriter consoleWriter, TextWriter consoleStream, ConsoleColor? oldForegroundColor, ConsoleColor? oldBackgroundColor, bool flush);

	/// <summary>
	/// Changes foreground color for the Colored TextWriter
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="foregroundColor">New foreground color for the console</param>
	/// <param name="oldForegroundColor">Old previous backgroundColor color for the console</param>
	/// <returns>Old foreground color for the console</returns>
	ConsoleColor? ChangeForegroundColor(TextWriter consoleWriter, ConsoleColor? foregroundColor, ConsoleColor? oldForegroundColor = null);

	/// <summary>
	/// Changes backgroundColor color for the Colored TextWriter
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="backgroundColor">New backgroundColor color for the console</param>
	/// <param name="oldBackgroundColor">Old previous backgroundColor color for the console</param>
	/// <returns>Old backgroundColor color for the console</returns>
	ConsoleColor? ChangeBackgroundColor(TextWriter consoleWriter, ConsoleColor? backgroundColor, ConsoleColor? oldBackgroundColor = null);

	/// <summary>
	/// Restores console colors back to their original state
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="foregroundColor">Original foregroundColor color for the console</param>
	/// <param name="backgroundColor">Original backgroundColor color for the console</param>
	void ResetDefaultColors(TextWriter consoleWriter, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor);

	/// <summary>
	/// Writes multiple characters to console in one operation (faster)
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="text">Output Text</param>
	/// <param name="index">Start Index</param>
	/// <param name="endIndex">End Index</param>
	void WriteSubString(TextWriter consoleWriter, string text, int index, int endIndex);

	/// <summary>
	/// Writes single character to console
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="text">Output Text</param>
	void WriteChar(TextWriter consoleWriter, char text);

	/// <summary>
	/// Writes whole string and completes with newline
	/// </summary>
	/// <param name="consoleWriter">Colored TextWriter</param>
	/// <param name="text">Output Text</param>
	void WriteLine(TextWriter consoleWriter, string text);
}

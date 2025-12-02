using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;
using NLog.Layouts;

namespace NLog.Targets;

/// <summary>
/// Writes log messages to the console with customizable coloring.
/// </summary>
/// <remarks>
/// <a href="https://github.com/nlog/nlog/wiki/ColoredConsole-target">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/nlog/nlog/wiki/ColoredConsole-target">Documentation on NLog Wiki</seealso>
/// <example>
/// <p>
/// To set up the target in the <a href="https://github.com/NLog/NLog/wiki/Configuration-file">configuration file</a>,
/// use the following syntax:
/// </p>
/// <code lang="XML" source="examples/targets/Configuration File/ColoredConsole/NLog.config" />
/// <p>
/// To set up the log target programmatically use code like this:
/// </p>
/// <code lang="C#" source="examples/targets/Configuration API/ColoredConsole/Simple/Example.cs" />
/// </example>
[Target("ColoredConsole")]
public sealed class ColoredConsoleTarget : TargetWithLayoutHeaderAndFooter
{
	private readonly Func<bool, TextWriter> _resolveConsoleStream;

	/// <summary>
	/// Should logging being paused/stopped because of the race condition bug in Console.Writeline?
	/// </summary>
	/// <remarks>
	///   Console.Out.Writeline / Console.Error.Writeline could throw 'IndexOutOfRangeException', which is a bug.
	/// See https://stackoverflow.com/questions/33915790/console-out-and-console-error-race-condition-error-in-a-windows-service-written
	/// and https://connect.microsoft.com/VisualStudio/feedback/details/2057284/console-out-probable-i-o-race-condition-issue-in-multi-threaded-windows-service
	///
	/// Full error:
	///   Error during session close: System.IndexOutOfRangeException: Probable I/ O race condition detected while copying memory.
	///   The I/ O package is not thread safe by default. In multi-threaded applications,
	///   a stream must be accessed in a thread-safe way, such as a thread - safe wrapper returned by TextReader's or
	///   TextWriter's Synchronized methods.This also applies to classes like StreamWriter and StreamReader.
	///
	/// </remarks>
	private bool _pauseLogging;

	private bool _disableColors;

	private IColoredConsolePrinter _consolePrinter;

	private Encoding? _encoding;

	/// <summary>
	/// Obsolete and replaced by <see cref="P:NLog.Targets.ColoredConsoleTarget.StdErr" /> with NLog v5.
	/// Gets or sets a value indicating whether the error stream (stderr) should be used instead of the output stream (stdout).
	/// </summary>
	/// <docgen category="Console Options" order="10" />
	[Obsolete("Replaced by StdErr to align with ConsoleTarget. Marked obsolete on NLog 5.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool ErrorStream
	{
		get
		{
			Layout<bool>? stdErr = StdErr;
			if (stdErr != null && stdErr.IsFixed)
			{
				return StdErr.FixedValue;
			}
			return false;
		}
		set
		{
			StdErr = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to send the log messages to the standard error instead of the standard output.
	/// </summary>
	/// <docgen category="Console Options" order="10" />
	public Layout<bool>? StdErr { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to use default row highlighting rules.
	/// </summary>
	/// <remarks>
	/// Default: <see langword="true" /> which enables the following rules:
	/// <table>
	/// <tr>
	/// <th>Condition</th>
	/// <th>Foreground Color</th>
	/// <th>Background Color</th>
	/// </tr>
	/// <tr>
	/// <td>level == LogLevel.Fatal</td>
	/// <td>Red</td>
	/// <td>NoChange</td>
	/// </tr>
	/// <tr>
	/// <td>level == LogLevel.Error</td>
	/// <td>Red</td>
	/// <td>NoChange</td>
	/// </tr>
	/// <tr>
	/// <td>level == LogLevel.Warn</td>
	/// <td>Yellow</td>
	/// <td>NoChange</td>
	/// </tr>
	/// <tr>
	/// <td>level == LogLevel.Info</td>
	/// <td>White</td>
	/// <td>NoChange</td>
	/// </tr>
	/// <tr>
	/// <td>level == LogLevel.Debug</td>
	/// <td>Gray</td>
	/// <td>NoChange</td>
	/// </tr>
	/// <tr>
	/// <td>level == LogLevel.Trace</td>
	/// <td>Gray</td>
	/// <td>NoChange</td>
	/// </tr>
	/// </table>
	/// </remarks>
	/// <docgen category="Highlighting Rules" order="9" />
	public bool UseDefaultRowHighlightingRules { get; set; } = true;

	/// <summary>
	/// The encoding for writing messages to the <see cref="T:System.Console" />.
	///  </summary>
	/// <remarks>Has side effect</remarks>
	/// <docgen category="Console Options" order="10" />
	public Encoding Encoding
	{
		get
		{
			return ConsoleTargetHelper.GetConsoleOutputEncoding(_encoding, base.IsInitialized, _pauseLogging);
		}
		set
		{
			if (ConsoleTargetHelper.SetConsoleOutputEncoding(value, base.IsInitialized, _pauseLogging))
			{
				_encoding = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether to auto-check if the console is available.
	///  - Disables console writing if Environment.UserInteractive = <see langword="false" /> (Windows Service)
	///  - Disables console writing if Console Standard Input is not available (Non-Console-App)
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Console Options" order="10" />
	public bool DetectConsoleAvailable { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to auto-check if the console has been redirected to file
	///   - Disables coloring logic when System.Console.IsOutputRedirected = <see langword="true" />
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Console Options" order="11" />
	public bool DetectOutputRedirected { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether to auto-flush after <see cref="M:System.Console.WriteLine" />
	/// </summary>
	/// <remarks>
	/// Default: <see langword="false" /> .
	/// Normally not required as standard Console.Out will have <see cref="P:System.IO.StreamWriter.AutoFlush" /> = true, but not when pipe to file
	/// </remarks>
	/// <docgen category="Console Options" order="11" />
	public bool AutoFlush { get; set; }

	/// <summary>
	/// Enables output using ANSI Color Codes
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Console Options" order="10" />
	public bool EnableAnsiOutput { get; set; }

	/// <summary>
	/// Support NO_COLOR=1 environment variable. See also <see href="https://no-color.org/" />
	/// </summary>
	/// <remarks>Default: <c>NO_COLOR=1</c></remarks>
	/// <docgen category="Console Options" order="10" />
	public Layout<bool> NoColor { get; set; } = Layout<bool>.FromMethod((LogEventInfo evt) => new string[2] { "1", "TRUE" }.Contains(EnvironmentHelper.GetSafeEnvironmentVariable("NO_COLOR")?.Trim().ToUpper()));

	/// <summary>
	/// Gets the row highlighting rules.
	/// </summary>
	/// <docgen category="Highlighting Rules" order="10" />
	[ArrayParameter(typeof(ConsoleRowHighlightingRule), "highlight-row")]
	public IList<ConsoleRowHighlightingRule> RowHighlightingRules { get; } = new List<ConsoleRowHighlightingRule>();

	/// <summary>
	/// Gets the word highlighting rules.
	/// </summary>
	/// <docgen category="Highlighting Rules" order="11" />
	[ArrayParameter(typeof(ConsoleWordHighlightingRule), "highlight-word")]
	public IList<ConsoleWordHighlightingRule> WordHighlightingRules { get; } = new List<ConsoleWordHighlightingRule>();

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ColoredConsoleTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	public ColoredConsoleTarget()
	{
		_consolePrinter = CreateConsolePrinter(EnableAnsiOutput);
		_resolveConsoleStream = GetOutputStream;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ColoredConsoleTarget" /> class.
	/// </summary>
	/// <remarks>
	/// The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}</code>
	/// </remarks>
	/// <param name="name">Name of the target.</param>
	public ColoredConsoleTarget(string name)
		: this()
	{
		base.Name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:NLog.Targets.ColoredConsoleTarget" /> class for Unit-Testing.
	/// </summary>
	internal ColoredConsoleTarget(Func<bool, TextWriter> resolveConsoleStream)
		: this()
	{
		_resolveConsoleStream = Guard.ThrowIfNull(resolveConsoleStream, "resolveConsoleStream");
	}

	/// <inheritdoc />
	protected override void InitializeTarget()
	{
		_pauseLogging = false;
		_disableColors = false;
		if (DetectConsoleAvailable)
		{
			_pauseLogging = !ConsoleTargetHelper.IsConsoleAvailable(out string reason);
			if (_pauseLogging)
			{
				InternalLogger.Info("{0}: Console detected as turned off. Set DetectConsoleAvailable=false to skip detection. Reason: {1}", this, reason);
			}
		}
		if (_encoding != null)
		{
			ConsoleTargetHelper.SetConsoleOutputEncoding(_encoding, isInitialized: true, _pauseLogging);
		}
		if (DetectOutputRedirected)
		{
			try
			{
				bool flag = RenderLogEvent(StdErr, LogEventInfo.CreateNullEvent(), defaultValue: false);
				_disableColors = (flag ? Console.IsErrorRedirected : Console.IsOutputRedirected);
				if (_disableColors)
				{
					InternalLogger.Info("{0}: Console output is redirected so no colors. Set DetectOutputRedirected=false to skip detection.", this);
					if (!AutoFlush && _resolveConsoleStream(flag) is StreamWriter { AutoFlush: false })
					{
						AutoFlush = true;
					}
				}
			}
			catch (Exception ex)
			{
				InternalLogger.Error(ex, "{0}: Failed checking if Console Output Redirected.", this);
			}
		}
		if (!_disableColors)
		{
			Layout<bool> noColor = NoColor;
			if (noColor != null && noColor.RenderValue(LogEventInfo.CreateNullEvent(), defaultValue: false))
			{
				_disableColors = true;
				InternalLogger.Info("{0}: Environment with NO_COLOR, so colors are disabled. Set NoColor=false to skip detection.", this);
			}
		}
		base.InitializeTarget();
		if (base.Header != null)
		{
			LogEventInfo logEvent = LogEventInfo.CreateNullEvent();
			WriteToOutput(logEvent, RenderLogEvent(base.Header, logEvent));
		}
		_consolePrinter = CreateConsolePrinter(EnableAnsiOutput);
	}

	private static IColoredConsolePrinter CreateConsolePrinter(bool enableAnsiOutput)
	{
		if (!enableAnsiOutput)
		{
			return new ColoredConsoleSystemPrinter();
		}
		return new ColoredConsoleAnsiPrinter();
	}

	/// <inheritdoc />
	protected override void CloseTarget()
	{
		if (base.Footer != null)
		{
			LogEventInfo logEvent = LogEventInfo.CreateNullEvent();
			WriteToOutput(logEvent, RenderLogEvent(base.Footer, logEvent));
		}
		ExplicitConsoleFlush();
		base.CloseTarget();
	}

	/// <inheritdoc />
	protected override void FlushAsync(AsyncContinuation asyncContinuation)
	{
		try
		{
			ExplicitConsoleFlush();
			base.FlushAsync(asyncContinuation);
		}
		catch (Exception exception)
		{
			asyncContinuation(exception);
		}
	}

	private void ExplicitConsoleFlush()
	{
		if (!_pauseLogging && !AutoFlush)
		{
			Layout<bool>? stdErr = StdErr;
			if (stdErr == null || stdErr.IsFixed)
			{
				bool arg = StdErr?.FixedValue ?? false;
				_resolveConsoleStream(arg).Flush();
			}
			else
			{
				_resolveConsoleStream(arg: false).Flush();
				_resolveConsoleStream(arg: true).Flush();
			}
		}
	}

	/// <inheritdoc />
	protected override void Write(LogEventInfo logEvent)
	{
		if (!_pauseLogging)
		{
			WriteToOutput(logEvent, RenderLogEvent(Layout, logEvent));
		}
	}

	private void WriteToOutput(LogEventInfo logEvent, string message)
	{
		try
		{
			WriteToOutputWithColor(logEvent, message ?? string.Empty);
		}
		catch (Exception ex) when (ex is OverflowException || ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
		{
			_pauseLogging = true;
			InternalLogger.Warn(ex, "{0}: {1} has been thrown and this is probably due to a race condition.Logging to the console will be paused. Enable by reloading the config or re-initialize the targets", this, ex.GetType());
		}
	}

	private void WriteToOutputWithColor(LogEventInfo logEvent, string message)
	{
		string text = message;
		ConsoleColor? newForegroundColor = null;
		ConsoleColor? newBackgroundColor = null;
		if (!_disableColors)
		{
			ConsoleRowHighlightingRule matchingRowHighlightingRule = GetMatchingRowHighlightingRule(logEvent);
			if (WordHighlightingRules.Count > 0)
			{
				text = GenerateColorEscapeSequences(logEvent, message);
			}
			newForegroundColor = ((matchingRowHighlightingRule.ForegroundColor != ConsoleOutputColor.NoChange) ? new ConsoleColor?((ConsoleColor)matchingRowHighlightingRule.ForegroundColor) : ((ConsoleColor?)null));
			newBackgroundColor = ((matchingRowHighlightingRule.BackgroundColor != ConsoleOutputColor.NoChange) ? new ConsoleColor?((ConsoleColor)matchingRowHighlightingRule.BackgroundColor) : ((ConsoleColor?)null));
		}
		bool arg = RenderLogEvent(StdErr, logEvent, defaultValue: false);
		TextWriter textWriter = _resolveConsoleStream(arg);
		if ((object)text == message && !newForegroundColor.HasValue && !newBackgroundColor.HasValue)
		{
			ConsoleTargetHelper.WriteLineThreadSafe(textWriter, message, AutoFlush);
			return;
		}
		bool flag = (object)text != message;
		if (!flag && message.IndexOf('\n') >= 0)
		{
			flag = true;
			text = EscapeColorCodes(message);
		}
		WriteToOutputWithPrinter(textWriter, text, newForegroundColor, newBackgroundColor, flag);
	}

	private void WriteToOutputWithPrinter(TextWriter consoleStream, string colorMessage, ConsoleColor? newForegroundColor, ConsoleColor? newBackgroundColor, bool wordHighlighting)
	{
		ReusableObjectCreator<StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
		try
		{
			TextWriter consoleWriter = _consolePrinter.AcquireTextWriter(consoleStream, lockOject.Result);
			ConsoleColor? consoleColor = null;
			ConsoleColor? consoleColor2 = null;
			try
			{
				if (wordHighlighting)
				{
					consoleColor = _consolePrinter.ChangeForegroundColor(consoleWriter, newForegroundColor);
					consoleColor2 = _consolePrinter.ChangeBackgroundColor(consoleWriter, newBackgroundColor);
					ConsoleColor? rowForegroundColor = newForegroundColor ?? consoleColor;
					ConsoleColor? rowBackgroundColor = newBackgroundColor ?? consoleColor2;
					ColorizeEscapeSequences(_consolePrinter, consoleWriter, colorMessage, consoleColor, consoleColor2, rowForegroundColor, rowBackgroundColor);
					_consolePrinter.WriteLine(consoleWriter, string.Empty);
					return;
				}
				if (newForegroundColor.HasValue)
				{
					consoleColor = _consolePrinter.ChangeForegroundColor(consoleWriter, newForegroundColor.Value);
					if (consoleColor == newForegroundColor)
					{
						consoleColor = null;
					}
				}
				if (newBackgroundColor.HasValue)
				{
					consoleColor2 = _consolePrinter.ChangeBackgroundColor(consoleWriter, newBackgroundColor.Value);
					if (consoleColor2 == newBackgroundColor)
					{
						consoleColor2 = null;
					}
				}
				_consolePrinter.WriteLine(consoleWriter, colorMessage);
			}
			finally
			{
				_consolePrinter.ReleaseTextWriter(consoleWriter, consoleStream, consoleColor, consoleColor2, AutoFlush);
			}
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private ConsoleRowHighlightingRule GetMatchingRowHighlightingRule(LogEventInfo logEvent)
	{
		ConsoleRowHighlightingRule matchingRowHighlightingRule = GetMatchingRowHighlightingRule(RowHighlightingRules, logEvent);
		if (matchingRowHighlightingRule == null && UseDefaultRowHighlightingRules)
		{
			matchingRowHighlightingRule = GetMatchingRowHighlightingRule(_consolePrinter.DefaultConsoleRowHighlightingRules, logEvent);
		}
		return matchingRowHighlightingRule ?? ConsoleRowHighlightingRule.Default;
	}

	private static ConsoleRowHighlightingRule? GetMatchingRowHighlightingRule(IList<ConsoleRowHighlightingRule> rules, LogEventInfo logEvent)
	{
		for (int i = 0; i < rules.Count; i++)
		{
			ConsoleRowHighlightingRule consoleRowHighlightingRule = rules[i];
			if (consoleRowHighlightingRule.CheckCondition(logEvent))
			{
				return consoleRowHighlightingRule;
			}
		}
		return null;
	}

	private string GenerateColorEscapeSequences(LogEventInfo logEvent, string message)
	{
		if (string.IsNullOrEmpty(message))
		{
			return message;
		}
		message = EscapeColorCodes(message);
		ReusableObjectCreator<StringBuilder>.LockOject lockOject = ReusableLayoutBuilder.Allocate();
		try
		{
			StringBuilder stringBuilder = lockOject.Result;
			for (int i = 0; i < WordHighlightingRules.Count; i++)
			{
				ConsoleWordHighlightingRule consoleWordHighlightingRule = WordHighlightingRules[i];
				if (!consoleWordHighlightingRule.CheckCondition(logEvent))
				{
					continue;
				}
				IEnumerable<KeyValuePair<int, int>> wordsForHighlighting = consoleWordHighlightingRule.GetWordsForHighlighting(message);
				if (wordsForHighlighting == null)
				{
					continue;
				}
				if (stringBuilder != null)
				{
					stringBuilder.Length = 0;
				}
				int num = 0;
				foreach (KeyValuePair<int, int> item in wordsForHighlighting)
				{
					stringBuilder = stringBuilder ?? new StringBuilder(message.Length + 5);
					stringBuilder.Append(message, num, item.Key - num);
					stringBuilder.Append('\a');
					stringBuilder.Append((char)(consoleWordHighlightingRule.ForegroundColor + 65));
					stringBuilder.Append((char)(consoleWordHighlightingRule.BackgroundColor + 65));
					for (int j = 0; j < item.Value; j++)
					{
						stringBuilder.Append(message[j + item.Key]);
					}
					stringBuilder.Append('\a');
					stringBuilder.Append('X');
					num = item.Key + item.Value;
				}
				if (stringBuilder != null && stringBuilder.Length > 0)
				{
					stringBuilder.Append(message, num, message.Length - num);
					message = stringBuilder.ToString();
				}
			}
			return message;
		}
		finally
		{
			((IDisposable)lockOject/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static string EscapeColorCodes(string message)
	{
		if (message.IndexOf('\a') >= 0)
		{
			message = message.Replace("\a", "\a\a");
		}
		return message;
	}

	private static void ColorizeEscapeSequences(IColoredConsolePrinter consolePrinter, TextWriter consoleWriter, string message, ConsoleColor? defaultForegroundColor, ConsoleColor? defaultBackgroundColor, ConsoleColor? rowForegroundColor, ConsoleColor? rowBackgroundColor)
	{
		Stack<KeyValuePair<ConsoleColor?, ConsoleColor?>> stack = new Stack<KeyValuePair<ConsoleColor?, ConsoleColor?>>();
		stack.Push(new KeyValuePair<ConsoleColor?, ConsoleColor?>(rowForegroundColor, rowBackgroundColor));
		int num = 0;
		while (num < message.Length)
		{
			int i;
			for (i = num; i < message.Length && message[i] >= ' '; i++)
			{
			}
			if (i != num)
			{
				consolePrinter.WriteSubString(consoleWriter, message, num, i);
			}
			if (i >= message.Length)
			{
				num = i;
				break;
			}
			char c = message[i];
			switch (c)
			{
			case '\n':
			case '\r':
			{
				KeyValuePair<ConsoleColor?, ConsoleColor?> keyValuePair3 = stack.Peek();
				ConsoleColor? foregroundColor = ((keyValuePair3.Key != defaultForegroundColor) ? defaultForegroundColor : ((ConsoleColor?)null));
				ConsoleColor? backgroundColor = ((keyValuePair3.Value != defaultBackgroundColor) ? defaultBackgroundColor : ((ConsoleColor?)null));
				consolePrinter.ResetDefaultColors(consoleWriter, foregroundColor, backgroundColor);
				if (i + 1 < message.Length && message[i + 1] == '\n')
				{
					consolePrinter.WriteSubString(consoleWriter, message, i, i + 2);
					num = i + 2;
				}
				else
				{
					consolePrinter.WriteChar(consoleWriter, c);
					num = i + 1;
				}
				consolePrinter.ChangeForegroundColor(consoleWriter, keyValuePair3.Key, defaultForegroundColor);
				consolePrinter.ChangeBackgroundColor(consoleWriter, keyValuePair3.Value, defaultBackgroundColor);
				break;
			}
			case '\a':
				if (i + 1 < message.Length)
				{
					char c2 = message[i + 1];
					switch (c2)
					{
					case '\a':
						consolePrinter.WriteChar(consoleWriter, '\a');
						num = i + 2;
						break;
					case 'X':
					{
						KeyValuePair<ConsoleColor?, ConsoleColor?> keyValuePair = stack.Pop();
						KeyValuePair<ConsoleColor?, ConsoleColor?> keyValuePair2 = stack.Peek();
						if (keyValuePair2.Key != keyValuePair.Key || keyValuePair2.Value != keyValuePair.Value)
						{
							if ((keyValuePair.Key.HasValue && !keyValuePair2.Key.HasValue) || (keyValuePair.Value.HasValue && !keyValuePair2.Value.HasValue))
							{
								consolePrinter.ResetDefaultColors(consoleWriter, defaultForegroundColor, defaultBackgroundColor);
							}
							consolePrinter.ChangeForegroundColor(consoleWriter, keyValuePair2.Key, keyValuePair.Key);
							consolePrinter.ChangeBackgroundColor(consoleWriter, keyValuePair2.Value, keyValuePair.Value);
						}
						num = i + 2;
						break;
					}
					default:
					{
						ConsoleColor? consoleColor = stack.Peek().Key;
						ConsoleColor? consoleColor2 = stack.Peek().Value;
						ConsoleOutputColor consoleOutputColor = (ConsoleOutputColor)(c2 - 65);
						ConsoleOutputColor consoleOutputColor2 = (ConsoleOutputColor)(message[i + 2] - 65);
						if (consoleOutputColor != ConsoleOutputColor.NoChange)
						{
							consoleColor = (ConsoleColor)consoleOutputColor;
							consolePrinter.ChangeForegroundColor(consoleWriter, consoleColor);
						}
						if (consoleOutputColor2 != ConsoleOutputColor.NoChange)
						{
							consoleColor2 = (ConsoleColor)consoleOutputColor2;
							consolePrinter.ChangeBackgroundColor(consoleWriter, consoleColor2);
						}
						stack.Push(new KeyValuePair<ConsoleColor?, ConsoleColor?>(consoleColor, consoleColor2));
						num = i + 3;
						break;
					}
					}
					break;
				}
				goto default;
			default:
				consolePrinter.WriteChar(consoleWriter, c);
				num = i + 1;
				break;
			}
		}
		if (num < message.Length)
		{
			consolePrinter.WriteSubString(consoleWriter, message, num, message.Length);
		}
	}

	private static TextWriter GetOutputStream(bool stdErr)
	{
		if (!stdErr)
		{
			return Console.Out;
		}
		return Console.Error;
	}
}

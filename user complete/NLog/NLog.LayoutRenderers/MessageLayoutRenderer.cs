using System;
using System.Collections.ObjectModel;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.Internal;

namespace NLog.LayoutRenderers;

/// <summary>
/// The formatted log message.
/// </summary>
/// <remarks>
/// <a href="https://github.com/NLog/NLog/wiki/Message-Layout-Renderer">See NLog Wiki</a>
/// </remarks>
/// <seealso href="https://github.com/NLog/NLog/wiki/Message-Layout-Renderer">Documentation on NLog Wiki</seealso>
[LayoutRenderer("message")]
[ThreadAgnostic]
public class MessageLayoutRenderer : LayoutRenderer, IStringValueRenderer
{
	/// <summary>
	/// Gets or sets a value indicating whether to log exception along with message.
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool WithException { get; set; }

	/// <summary>
	/// Gets or sets the string that separates message from the exception.
	/// </summary>
	/// <remarks>Default: <c>|</c></remarks>
	/// <docgen category="Layout Options" order="10" />
	public string ExceptionSeparator { get; set; } = "|";

	/// <summary>
	/// Gets or sets whether it should render the raw message without formatting parameters
	/// </summary>
	/// <remarks>Default: <see langword="false" /></remarks>
	/// <docgen category="Layout Options" order="10" />
	public bool Raw { get; set; }

	/// <inheritdoc />
	protected override void Append(StringBuilder builder, LogEventInfo logEvent)
	{
		int num;
		if (logEvent.Exception != null && WithException)
		{
			object?[]? parameters = logEvent.Parameters;
			if (parameters != null && parameters.Length == 1 && logEvent.Parameters[0] == logEvent.Exception)
			{
				num = ((logEvent.Message == "{0}") ? 1 : 0);
				goto IL_0047;
			}
		}
		num = 0;
		goto IL_0047;
		IL_0047:
		bool flag = (byte)num != 0;
		if (Raw)
		{
			builder.Append(logEvent.Message);
		}
		else if (!flag)
		{
			if (logEvent.MessageFormatter?.Target is ILogMessageFormatter messageFormatter)
			{
				logEvent.AppendFormattedMessage(messageFormatter, builder);
			}
			else
			{
				builder.Append(logEvent.FormattedMessage);
			}
		}
		if (WithException && logEvent.Exception != null)
		{
			Exception primaryException = GetPrimaryException(logEvent.Exception);
			if (!flag)
			{
				builder.Append(ExceptionSeparator);
			}
			AppendExceptionToString(builder, primaryException);
		}
	}

	private static void AppendExceptionToString(StringBuilder builder, Exception exception)
	{
		string arg = string.Empty;
		Exception ex = null;
		try
		{
			ex = exception.InnerException;
			arg = exception.Message;
			builder.Append(exception.ToString());
		}
		catch (Exception ex2)
		{
			InternalLogger.Warn(ex2, "Message-LayoutRenderer Could not output ToString for Exception: {0}", exception.GetType());
			builder.Append($"{exception.GetType()}: {arg}");
			if (ex != null)
			{
				builder.AppendLine();
				AppendExceptionToString(builder, ex);
			}
		}
	}

	private static Exception GetPrimaryException(Exception exception)
	{
		if (exception is AggregateException ex)
		{
			ReadOnlyCollection<Exception> innerExceptions = ex.InnerExceptions;
			if (innerExceptions != null && innerExceptions.Count == 1)
			{
				Exception ex2 = ex.InnerExceptions[0];
				if (!(ex2 is AggregateException))
				{
					return ex2;
				}
			}
			AggregateException ex3 = ex.Flatten();
			ReadOnlyCollection<Exception> innerExceptions2 = ex3.InnerExceptions;
			if (innerExceptions2 != null && innerExceptions2.Count == 1)
			{
				return ex3.InnerExceptions[0];
			}
			return ex3;
		}
		return exception;
	}

	string? IStringValueRenderer.GetFormattedString(LogEventInfo logEvent)
	{
		if (WithException)
		{
			return null;
		}
		return (Raw ? logEvent.Message : logEvent.FormattedMessage) ?? string.Empty;
	}
}

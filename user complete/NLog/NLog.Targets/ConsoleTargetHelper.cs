using System;
using System.IO;
using System.Text;
using NLog.Common;
using NLog.Internal;

namespace NLog.Targets;

internal static class ConsoleTargetHelper
{
	private static readonly object _lockObject = new object();

	public static bool IsConsoleAvailable(out string reason)
	{
		reason = string.Empty;
		try
		{
			if (!Environment.UserInteractive)
			{
				if (PlatformDetector.IsMono && Console.In is StreamReader)
				{
					return true;
				}
				reason = "Environment.UserInteractive = False";
				return false;
			}
			if (Console.OpenStandardInput(1) == Stream.Null)
			{
				reason = "Console.OpenStandardInput = Null";
				return false;
			}
		}
		catch (Exception ex)
		{
			reason = "Unexpected exception: " + ex.GetType().Name + ":" + ex.Message;
			InternalLogger.Warn(ex, "Failed to detect whether console is available.");
			return false;
		}
		return true;
	}

	public static Encoding GetConsoleOutputEncoding(Encoding? currentEncoding, bool isInitialized, bool pauseLogging)
	{
		if (currentEncoding != null)
		{
			return currentEncoding;
		}
		if ((isInitialized && !pauseLogging) || IsConsoleAvailable(out string _))
		{
			return Console.OutputEncoding;
		}
		return Encoding.Default;
	}

	public static bool SetConsoleOutputEncoding(Encoding newEncoding, bool isInitialized, bool pauseLogging)
	{
		if (!isInitialized)
		{
			return true;
		}
		if (!pauseLogging)
		{
			try
			{
				Console.OutputEncoding = newEncoding;
				return true;
			}
			catch (Exception ex)
			{
				InternalLogger.Warn(ex, "Failed changing Console.OutputEncoding to {0}", newEncoding);
			}
		}
		return false;
	}

	public static void WriteLineThreadSafe(TextWriter console, string message, bool flush = false)
	{
		lock (_lockObject)
		{
			console.WriteLine(message);
			if (flush)
			{
				console.Flush();
			}
		}
	}

	public static void WriteBufferThreadSafe(TextWriter console, char[] buffer, int length, bool flush = false)
	{
		lock (_lockObject)
		{
			console.Write(buffer, 0, length);
			if (flush)
			{
				console.Flush();
			}
		}
	}
}

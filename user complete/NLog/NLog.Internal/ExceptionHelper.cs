using System;
using System.Threading;
using NLog.Common;

namespace NLog.Internal;

/// <summary>
/// Helper class for dealing with exceptions.
/// </summary>
internal static class ExceptionHelper
{
	private const string LoggedKey = "NLog.ExceptionLoggedToInternalLogger";

	/// <summary>
	/// Mark this exception as logged to the <see cref="T:NLog.Common.InternalLogger" />.
	/// </summary>
	/// <param name="exception"></param>
	/// <returns></returns>
	public static void MarkAsLoggedToInternalLogger(this Exception exception)
	{
		if (exception != null)
		{
			exception.Data["NLog.ExceptionLoggedToInternalLogger"] = true;
		}
	}

	/// <summary>
	/// Is this exception logged to the <see cref="T:NLog.Common.InternalLogger" />?
	/// </summary>
	/// <param name="exception"></param>
	/// <returns><see langword="true" /> if the <paramref name="exception" /> has been logged to the <see cref="T:NLog.Common.InternalLogger" />.</returns>
	public static bool IsLoggedToInternalLogger(this Exception exception)
	{
		if (exception != null && exception.Data?.Count > 0)
		{
			return exception.Data["NLog.ExceptionLoggedToInternalLogger"] as bool? == true;
		}
		return false;
	}

	/// <summary>
	/// Determines whether the exception must be rethrown and logs the error to the <see cref="T:NLog.Common.InternalLogger" /> if <see cref="M:NLog.Internal.ExceptionHelper.IsLoggedToInternalLogger(System.Exception)" /> is <see langword="false" />.
	///
	/// Advised to log first the error to the <see cref="T:NLog.Common.InternalLogger" /> before calling this method.
	/// </summary>
	/// <param name="exception">The exception to check.</param>
	/// <param name="loggerContext">Target Object context of the exception.</param>
	/// <param name="callerMemberName">Target Method context of the exception.</param>
	/// <returns><see langword="true" /> if the <paramref name="exception" /> must be rethrown, <see langword="false" /> otherwise.</returns>
	public static bool MustBeRethrown(this Exception exception, IInternalLoggerContext? loggerContext = null, string? callerMemberName = null)
	{
		if (exception.MustBeRethrownImmediately())
		{
			return true;
		}
		bool num = exception is NLogConfigurationException;
		LogFactory logFactory = loggerContext?.LogFactory;
		bool flag = (logFactory != null && logFactory.ThrowExceptions) || LogManager.ThrowExceptions;
		bool flag2 = ((!num) ? flag : (logFactory?.ThrowConfigExceptions ?? LogManager.ThrowConfigExceptions ?? flag));
		if (!exception.IsLoggedToInternalLogger())
		{
			LogLevel level = (flag2 ? LogLevel.Error : LogLevel.Warn);
			if (loggerContext != null)
			{
				if (string.IsNullOrEmpty(callerMemberName))
				{
					InternalLogger.Log(exception, level, "{0}: Error has been raised.", loggerContext);
				}
				else
				{
					InternalLogger.Log(exception, level, "{0}: Exception in {1}", loggerContext, callerMemberName);
				}
			}
			else
			{
				InternalLogger.Log(exception, level, "Error has been raised.");
			}
		}
		return flag2;
	}

	/// <summary>
	/// Determines whether the exception must be rethrown immediately, without logging the error to the <see cref="T:NLog.Common.InternalLogger" />.
	///
	/// Only used this method in special cases.
	/// </summary>
	/// <param name="exception">The exception to check.</param>
	/// <returns><see langword="true" /> if the <paramref name="exception" /> must be rethrown, <see langword="false" /> otherwise.</returns>
	public static bool MustBeRethrownImmediately(this Exception exception)
	{
		if (exception is StackOverflowException)
		{
			return true;
		}
		if (exception is ThreadAbortException)
		{
			return true;
		}
		if (exception is OutOfMemoryException)
		{
			return true;
		}
		return false;
	}
}

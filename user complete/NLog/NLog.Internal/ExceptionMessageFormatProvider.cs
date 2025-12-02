using System;
using System.Collections.ObjectModel;

namespace NLog.Internal;

/// <summary>
/// FormatProvider that renders an exception-object as $"{ex.GetType()}: {ex.Message}"
/// </summary>
internal sealed class ExceptionMessageFormatProvider : IFormatProvider, ICustomFormatter
{
	internal static readonly ExceptionMessageFormatProvider Instance = new ExceptionMessageFormatProvider();

	string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
	{
		if (arg is Exception exception)
		{
			Exception primaryException = GetPrimaryException(exception);
			return primaryException.GetType().ToString() + ": " + primaryException.Message;
		}
		return $"{arg}";
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
		}
		return exception;
	}

	object? IFormatProvider.GetFormat(Type formatType)
	{
		if (!(formatType == typeof(ICustomFormatter)))
		{
			return null;
		}
		return this;
	}
}

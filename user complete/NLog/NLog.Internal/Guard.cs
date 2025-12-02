using System;
using System.Runtime.CompilerServices;

namespace NLog.Internal;

internal static class Guard
{
	internal static T ThrowIfNull<T>(T arg, [CallerArgumentExpression("arg")] string param = "") where T : class
	{
		if (arg == null)
		{
			throw new ArgumentNullException(string.IsNullOrEmpty(param) ? typeof(T).Name : param);
		}
		return arg;
	}

	internal static string ThrowIfNullOrEmpty(string arg, [CallerArgumentExpression("arg")] string param = "")
	{
		if (string.IsNullOrEmpty(arg))
		{
			throw new ArgumentNullException(string.IsNullOrEmpty(param) ? "arg" : param);
		}
		return arg;
	}
}

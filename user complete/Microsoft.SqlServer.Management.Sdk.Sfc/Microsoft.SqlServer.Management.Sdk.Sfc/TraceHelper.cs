using System;

namespace Microsoft.SqlServer.Management.Sdk.Sfc;

[CLSCompliant(false)]
public static class TraceHelper
{
	private const uint TraceLevelAssert = 2147483648u;

	private const uint TraceLevelException = 2097152u;

	private static string AddDate(string str)
	{
		return string.Format("{0} - {1}", DateTime.Now.ToString("o"), str);
	}

	public static void Trace(string strComponentName, string strFormat, params object[] args)
	{
	}

	public static void Trace(string strComponentName, uint traceLevel, string strFormat, params object[] args)
	{
	}

	public static void Assert(bool condition)
	{
	}

	public static void Assert(bool condition, string strFormat)
	{
	}

	public static void LogExCatch(Exception ex)
	{
	}
}

using System;
using System.Collections.Generic;

namespace Microsoft.SqlServer.Management.Smo;

internal static class SqlEnumStatementBuilderTestHook
{
	[ThreadStatic]
	private static Dictionary<string, string> sqlStatementFragmentDictionary;

	private static Dictionary<string, string> FragmentDictionary
	{
		get
		{
			if (sqlStatementFragmentDictionary == null)
			{
				sqlStatementFragmentDictionary = new Dictionary<string, string>();
			}
			return sqlStatementFragmentDictionary;
		}
	}

	public static IReadOnlyDictionary<string, string> SqlStatementFragmentsToBeReplaced => FragmentDictionary;

	public static void AddSqlStatementFragmentReplacement(string originalString, string replacementString)
	{
		FragmentDictionary[originalString] = replacementString;
	}

	public static void Clear()
	{
		FragmentDictionary.Clear();
	}
}
